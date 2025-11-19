// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Interoperability;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Interoperability.Domain;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SpedizioneInteropPiTre
{
    public class SpedizioneInteropPiTreCommandHandler : MessageQueueBaseCommandHandler<SpedizioneInteropPiTreCommand>
    {
        #region Public Members
        public SpedizioneInteropPiTreCommandHandler(ILogger<SpedizioneInteropPiTreCommandHandler> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        { }


        protected override async Task InternalHandle(IServiceProvider serviceProvider, SpedizioneInteropPiTreCommand message)
        {
            var claimsPrincipalService = serviceProvider.GetRequiredService<IClaimsPrincipalService>();
            IPi3DbContext dbContext = serviceProvider.GetRequiredService<IPi3DbContext>();
            IInteroperabilityService interoperabilityService = serviceProvider.GetRequiredService<IInteroperabilityService>();
            IWebMethodLoggerService webMethodLoggerService = serviceProvider.GetRequiredService<IWebMethodLoggerService>();
            IReportGeneratorService reportGeneratorService = serviceProvider.GetRequiredService<IReportGeneratorService>();
            IDocumentBlobRepository documentBlobRepository = serviceProvider.GetRequiredService<IDocumentBlobRepository>();
            IDocumentoAmministrativoRepository documentoAmministrativoRepository = serviceProvider.GetRequiredService<IDocumentoAmministrativoRepository>();

            var elaborateNewInteroperabilityMessageResponse = await interoperabilityService.ElaborateNewInteroperabilityMessage(
                message.Instance,
                message.Authorization,
                message.Tenant,
                new ElaborateNewInteroperabilityMessageRequest()
                {
                    InteroperabilityMessage = message.InteroperabilityMessage
                });

            await OnAnalyzeInteroperabilityMessageCompleted(message.InteroperabilityMessage,
                elaborateNewInteroperabilityMessageResponse,
                claimsPrincipalService,
                webMethodLoggerService,
                dbContext,
                reportGeneratorService,
                documentBlobRepository,
                documentoAmministrativoRepository);

        }
        #endregion

        #region Private Members

        protected async Task OnAnalyzeInteroperabilityMessageCompleted(InteroperabilityMessage interoperabilityMessage,
            ElaborateNewInteroperabilityMessageResponse elaborateNewInteroperabilityMessageResponse,
            IClaimsPrincipalService claimsPrincipalService,
            IWebMethodLoggerService webMethodLoggerService,
            IPi3DbContext dbContext,
            IReportGeneratorService reportGeneratorService,
            IDocumentBlobRepository documentBlobRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            //Recupero il ruolo che ha effettuato l'ultima spedizione IS
            var idGruppo = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeople = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

            // Associazione della ricevuta di errore di consegna per tutti i corrispondenti per cui si è verificato un problema
            List<ReceiverInfo> receiverWithErrors = new List<ReceiverInfo>();
            foreach (var request in elaborateNewInteroperabilityMessageResponse.Result.SingleRequestErrors)
            {
                receiverWithErrors.AddRange(request.Receivers);
                foreach (var receiver in request.Receivers)
                {
                    await GenerateProofDeliveryFailure(interoperabilityMessage,
                      elaborateNewInteroperabilityMessageResponse.Result.MessageId,
                      request.ErrorMessage,
                      receiver,
                      dbContext,
                      claimsPrincipalService,
                      reportGeneratorService,
                      documentBlobRepository,
                      documentoAmministrativoRepository);

                    //Inserisco la notifica nel centro notifiche
                    await webMethodLoggerService.LogOK("NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY", interoperabilityMessage.MainDocument.DocumentNumber,
                            string.Format(Resources.LogMancataConsegnaInteroperabilitaSemplificata, request.ErrorMessage, receiver.Code));
                }
            }

            //Associazione della ricevuta di avvenuta consegna per tutti quelli a cui non è già stata associata una ricevuta di mancata consegna
            foreach (var receiver in interoperabilityMessage.Receivers)
            {
                if (!receiverWithErrors.Contains(receiver))
                {
                    await GenerateProofDelivered(interoperabilityMessage,
                      elaborateNewInteroperabilityMessageResponse.Result.MessageId,
                      receiver,
                      elaborateNewInteroperabilityMessageResponse.Result.DocumentDelivered,
                      dbContext,
                      claimsPrincipalService,
                      reportGeneratorService,
                      documentBlobRepository,
                      documentoAmministrativoRepository);
                }
            }
        }

        protected async Task GenerateProofDeliveryFailure(InteroperabilityMessage interoperabilityMessage,
            string messageId,
            string exeception,
            ReceiverInfo receiver,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IReportGeneratorService reportGeneratorService,
            IDocumentBlobRepository documentBlobRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            var date = await dbContext.GetSystemDateTime();
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            try
            {
                var report = new ReportModel()
                {
                    Size = PageSizes.A4,
                    Orientation = PageOrientations.Landscape
                };

                report.AddSection(new TextSectionModel
                {
                    Style = new TextSectionStyleModel
                    {
                        Justification = Justifications.Left
                    },
                    Content = new TextContentModel
                    {
                        Value = Resources.RicevutaMancataConsegna,
                        Style = new TextStyleModel
                        {
                            FontName = "Arial",
                            FontSize = 16,
                            FontIsBold = false
                        }
                    }
                });

                report.AddSection(new TextSectionModel
                {
                    Style = new TextSectionStyleModel
                    {
                        Justification = Justifications.Left
                    },
                    Content = new TextContentModel
                    {
                        Value = string.Format(Resources.DettaglioRicevutaMancataConsegna,
                            date.AsDateFormat(),
                            date.ToString("HH:mm:ss"),
                            interoperabilityMessage.Record.Subject,
                            interoperabilityMessage.Sender.Code,
                            receiver.Code,
                            exeception,
                            messageId
                            ),
                        Style = new TextStyleModel
                        {
                            FontName = "Arial",
                            FontSize = 16,
                            FontIsBold = false
                        }
                    }
                });

                using MemoryStream stream = new MemoryStream();
                var reportGenerated = await reportGeneratorService.Generate(report, stream);

                //Inserimento di informazioni nel log delle operazioni
                var insert = await ((DbContext)dbContext).Database.ExecuteSqlInterpolatedAsync($"INSERT INTO SimpInteropDbLog (PROFILEID, ERRORMESSAGE, TEXT) VALUES ({interoperabilityMessage.MainDocument.DocumentNumber}, {0}, {Resources.AggiuntaRicevutaMancataConsegna})");

                var newDocumentBlobAggregate = new DocumentBlob(idTenant, DateTime.Now, new TextValue(Resources.NomeFileMancataConsegna));
                newDocumentBlobAggregate.UploadStream(stream, Resources.NomeFileMancataConsegna);
                newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);
                await documentBlobRepository.Add(newDocumentBlobAggregate);

                await AddProofToDocument(interoperabilityMessage.MainDocument.DocumentNumber,
                    messageId,
                    newDocumentBlobAggregate,
                    receiver.Code,
                    date,
                    string.Format(Resources.DescrizioneAllegatoRicevutaMancataConsegna, receiver.Code),
                    dbContext,
                    claimsPrincipalService,
                    documentoAmministrativoRepository);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: "Errore in GenerateProofDeliveryFailure: " + ex.Message);
            }

            //PEC 4 Modifica Maschera Caratteri
            var statoInvioEntity = await dbContext.StatoInvioEntities
                .Where(s => s.VAR_CODICE_AMM.ToUpper() == receiver.AdministrationCode.ToUpper() &&
                    s.VAR_CODICE_AOO.ToUpper() == receiver.AOOCode.ToUpper() &&
                    s.ID_PROFILE == interoperabilityMessage.MainDocument.DocumentNumber.AsLong())
                .ToListAsync();

            if (statoInvioEntity != null && statoInvioEntity.Any())
            {
                statoInvioEntity.ForEach(s =>
                {
                    s.STATUS_C_MASK = "XNXNNNN";
                    s.CHA_ANNULLATO = "E";
                    s.VAR_MOTIVO_ANNULLA = Resources.ErroreConsegnaIS;
                });

                await ((DbContext)dbContext).SaveChangesAsync();
            }
        }

        private TextSectionModel CreateTextSectionModel(string text, int fontSize, bool isBold, Justifications justification)
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = text,
                    Style = new TextStyleModel()
                    {
                        FontSize = fontSize,
                        FontIsBold = isBold
                    }
                },
                Style = new TextSectionStyleModel()
                {
                    Justification = justification
                }
            };
        }

        protected async Task GenerateProofDelivered(InteroperabilityMessage interoperabilityMessage,
            string messageId,
            ReceiverInfo receiver,
            InfoDocumentDelivered documentDelivered,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IReportGeneratorService reportGeneratorService,
            IDocumentBlobRepository documentBlobRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            var date = await dbContext.GetSystemDateTime();
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            try
            {
                var report = new ReportModel()
                {
                    Size = PageSizes.A4,
                    Orientation = PageOrientations.Landscape
                };

                // Adding title with bold text
                report.AddSection(CreateTextSectionModel(Resources.RicevutaAvvenutaConsegna, 16, true, Justifications.Left));

                // Adding delivery result text as free text
                string deliveryResultText = string.Format(
                    "Il giorno {0} alle ore {1} il messaggio '{2}' proveniente da '{3}' ed indirizzato a '{4}' è stato consegnato al destinatario. Identificativo messaggio:",
                    date.AsDateFormat(), date.ToString("HH:mm:ss"), interoperabilityMessage.Record.Subject, interoperabilityMessage.Sender.Code, receiver.Code);
                report.AddSection(CreateTextSectionModel(deliveryResultText, 14, false, Justifications.Left));

                // Adding content information header as free text
                report.AddSection(CreateTextSectionModel(Resources.InformazioniContenutoSpedizione, 16, false, Justifications.Left));

                // Adding main document information header within a grid
                var model = new GridSectionModel()
                {
                    Style = new GridSectionStyleModel()
                    {
                        WithPercentage = 100
                    }
                };

                model.AddRow(CreateRowWithSingleCell(Resources.InformazioniContenutoSpedizioneDocPrincipale, 14, true, Justifications.Left));

                // Adding main document table headers
                model.AddRow(CreateHeaderRow(new[] { Resources.Descrizione, Resources.NomeFile, Resources.Impronta }));

                // Adding main document information
                model.AddRow(CreateContentRow(new[]
                {
                    documentDelivered.MainDocument.Name,
                    !string.IsNullOrEmpty(documentDelivered.MainDocument.FileName) ? documentDelivered.MainDocument.FileName : Resources.FileNonAcquisito,
                    !string.IsNullOrEmpty(documentDelivered.MainDocument.FileName) ? documentDelivered.MainDocument.Fingerprint : string.Empty
                }));

                report.AddSection(model);

                // Adding attachments if present within a grid
                if (documentDelivered.Attachments != null && documentDelivered.Attachments.Any())
                {
                    model = new GridSectionModel()
                    {
                        Style = new GridSectionStyleModel()
                        {
                            WithPercentage = 100
                        }
                    };

                    model.AddRow(CreateRowWithSingleCell(Resources.InformazioniContenutoSpedizioneAllegati, 14, true, Justifications.Left));

                    // Adding attachments table headers
                    model.AddRow(CreateHeaderRow(new[] { Resources.Descrizione, Resources.NomeFile, Resources.Impronta }));

                    // Adding attachments information
                    foreach (var attach in documentDelivered.Attachments)
                    {
                        model.AddRow(CreateContentRow(new[]
                        {
                    attach.Name,
                    !string.IsNullOrEmpty(attach.FileName) ? attach.FileName : Resources.FileNonAcquisito,
                    !string.IsNullOrEmpty(attach.FileName) ? attach.Fingerprint : string.Empty
                }));
                    }

                    report.AddSection(model);
                }

                using MemoryStream stream = new MemoryStream();
                var reportGenerated = await reportGeneratorService.Generate(report, stream);

                // Log information
                var insert = await ((DbContext)dbContext).Database.ExecuteSqlInterpolatedAsync($"INSERT INTO SimpInteropDbLog (PROFILEID, ERRORMESSAGE, TEXT) VALUES ({interoperabilityMessage.MainDocument.DocumentNumber}, {0}, {Resources.AggiuntaRicevutaAvvenutaConsegna})");

                var newDocumentBlobAggregate = new DocumentBlob(idTenant, DateTime.Now, new TextValue(Resources.NomeFileAvvenutaConsegna));
                newDocumentBlobAggregate.UploadStream(stream, Resources.NomeFileAvvenutaConsegna);
                newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);
                await documentBlobRepository.Add(newDocumentBlobAggregate);

                await AddProofToDocument(interoperabilityMessage.MainDocument.DocumentNumber,
                    messageId,
                    newDocumentBlobAggregate,
                    receiver.Code,
                    date,
                    string.Format(Resources.DescrizioneAllegatoRicevutaAvvenutaConsegna, receiver.Code),
                    dbContext,
                    claimsPrincipalService,
                    documentoAmministrativoRepository);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: "Errore in GenerateProofDeliveryFailure: " + ex.Message);
            }

            // PEC 4 - Update mask characters
            var statoInvioEntity = await dbContext.StatoInvioEntities
                .Where(s => s.VAR_CODICE_AMM.ToUpper() == receiver.AdministrationCode.ToUpper() &&
                    s.VAR_CODICE_AOO.ToUpper() == receiver.AOOCode.ToUpper() &&
                    s.ID_PROFILE == interoperabilityMessage.MainDocument.DocumentNumber.AsLong())
                .FirstOrDefaultAsync();

            if (statoInvioEntity != null)
            {
                statoInvioEntity.STATUS_C_MASK = "ANVAAAN";
                await ((DbContext)dbContext).SaveChangesAsync();
            }
        }


        protected GridCellModel CreateHeaderCell(string textContentModel)
        {
            var cell = new GridCellModel()
            {
                Style = new GridCellStyleModel()
                {
                    Justification = Justifications.Left,
                    VerticalAlignment = VerticalAlignments.Center
                },
                Content = new TextContentModel()
                {
                    Value = textContentModel,
                    Style = new TextStyleModel()
                    {
                        FontIsBold = true,
                        FontName = "Arial",
                        FontSize = 12
                    }
                }
            };

            return cell;
        }

        // Helper methods for creating rows and cells
        private GridRowModel CreateRowWithSingleCell(string content, int fontSize, bool isBold, Justifications justification)
        {

            var row = new GridRowModel();
            var cell = new GridCellModel()
            {
                Style = new GridCellStyleModel()
                {
                    Justification = justification,
                    VerticalAlignment = VerticalAlignments.Center
                },
                Content = new TextContentModel()
                {
                    Value = content,
                    Style = new TextStyleModel()
                    {
                        FontIsBold = isBold,
                        FontName = "Arial",
                        FontSize = fontSize
                    }
                }
            };
            row.AddCell(cell);
            return row;
        }


        private GridRowModel CreateHeaderRow(string[] headers)
        {
            var row = new GridRowModel();
            foreach (var header in headers)
            {
                row.AddCell(CreateHeaderCell(header));
            }
            return row;
        }

        private GridRowModel CreateContentRow(string[] contents)
        {
            var row = new GridRowModel();
            foreach (var content in contents)
            {
                row.AddCell(CreateCell(content));
            }
            return row;
        }

        private GridCellModel CreateCell(string content)
        {
            return new GridCellModel()
            {
                Style = new GridCellStyleModel() { WithPercentage = 33, Justification = Justifications.Left },
                Content = new TextContentModel()
                {
                    Value = content,
                    Style = new TextStyleModel { FontName = "Arial", FontSize = 14, FontIsBold = false }
                }
            };
        }

        protected async Task<bool> AddProofToDocument(string docnumber, string messageId,
            DocumentBlob proofContent,
            string receiver,
            DateTime proofDate,
            string descrizioneAllegato,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var docnumberAsLong = docnumber.AsLong();

            try
            {
                var aggregateAllegatoSegnatura = new DocumentoAmministrativo(idTenant.ToString(),
                                   DateTime.Now,
                                   new OggettoDelDocumento()
                                   {
                                       Descrizione = new TextValue(descrizioneAllegato)
                                   },
                                   null,
                                   null,
                                   null,
                                   new IdDoc()
                                   {
                                       Identiticativo = docnumber
                                   });

                aggregateAllegatoSegnatura.ChangeTipologiaAllegato(TipologieAllegatiEnum.PiTre);

                aggregateAllegatoSegnatura.AssignDocumentBlobRef(
                    new DocumentBlobRef()
                    {
                        IdBlob = proofContent.Id,
                        FileName = proofContent.FileName,
                        ContentType = proofContent.ContentType,
                        FileSize = proofContent.FileSize,
                        CreationDate = await dbContext.GetSystemDateTime(),
                        Hash = proofContent.Hash,
                        HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256
                    },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = true
                    });

                await documentoAmministrativoRepository.Add(aggregateAllegatoSegnatura);

            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: "Errore in AddProofToDocument: " + ex.Message);
            }

            return true;
        }


        #endregion
    }
}
