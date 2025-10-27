// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DocsPaVO.areaConservazione;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Pi3.Core.Services.File.FileValidator;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SuapManager.AttachXmlSuap
{

    public class AttachXmlSuapHandler : IRequestHandler<AttachXmlSuapRequest, AttachXmlSuapResult>
    {
        #region Public Members

        public AttachXmlSuapHandler(ILogger<AttachXmlSuapHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IConfigurationService configurationService,
            IDocumentBlobRepository documentBlobRepository,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._fileValidatorService = fileValidatorService;
        }

        public async Task<AttachXmlSuapResult> Handle(AttachXmlSuapRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.documento.Allegato> allegati = null;
            try
            {
                var schedaDocumento = (await _mediator.Send(new Requests.DocumentoGetDettaglioDocumentoNoSecurity(request.infoUtenteInterop, request.docnumber, request.docnumber))).output;
                allegati = schedaDocumento.allegati.ToList();

                if (schedaDocumento.template == null || schedaDocumento.template.DESCRIZIONE.ToUpper() != Descriptions.ENTESUAP)
                    return new AttachXmlSuapResult(schedaDocumento.allegati);

                if(string.IsNullOrEmpty(request.mailFrom))
                    return new AttachXmlSuapResult(schedaDocumento.allegati);

                var xml = await ExportEnteSuapXML(request.infoUtenteInterop, schedaDocumento, request.mailFrom);
                if(string.IsNullOrEmpty(xml))
                    return new AttachXmlSuapResult(schedaDocumento.allegati);

                DocsPaVO.documento.Allegato allEntesuap = null;
                foreach (var all in schedaDocumento.allegati)
                {
                    string originalName = await GetOriginalFileName(all.versionId, all.docNumber);
                    if (originalName != null && originalName.ToUpper().Equals(Descriptions.EnteSuapXML))
                    {
                        allEntesuap = all;
                        break;
                    }
                }

                var newDocumentBlobAggregate = new DocumentBlob(request.infoUtenteInterop.idAmministrazione, DateTime.Now, new TextValue(Descriptions.EnteSuapXML));
                newDocumentBlobAggregate.UploadStream(new MemoryStream(Encoding.UTF8.GetBytes(xml)), Descriptions.EnteSuapXML);
                newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                DocumentoAmministrativo documentoAmministrativoAllegatoEntesuap = null;
                if (allEntesuap == null)
                {
                    documentoAmministrativoAllegatoEntesuap = new DocumentoAmministrativo(request.infoUtenteInterop.idAmministrazione,
                              DateTime.Now,
                              new OggettoDelDocumento()
                              {
                                  Descrizione = new TextValue(Descriptions.EnteSuapXML)
                              },
                              null, null, null,
                              new IdDoc()
                              {
                                  Identiticativo = schedaDocumento.docNumber
                              });

                    documentoAmministrativoAllegatoEntesuap.ChangeNumeroPagineAllegato(1);

                    await _documentBlobRepository.Add(newDocumentBlobAggregate);

                    documentoAmministrativoAllegatoEntesuap.AssignDocumentBlobRef(
                        new DocumentBlobRef()
                        {
                            IdBlob = newDocumentBlobAggregate.Id,
                            FileName = newDocumentBlobAggregate.FileName,
                            ContentType = newDocumentBlobAggregate.ContentType,
                            FileSize = newDocumentBlobAggregate.FileSize,
                            CreationDate = await _dbContext.GetSystemDateTime(),
                            Hash = newDocumentBlobAggregate.Hash,
                            HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                            Cartaceo = false,
                            SegnaturaPermanente = false,
                            TipoFirma = TipoFirmaEnum.Nessuna
                        },
                        new TargetVersionBehavior()
                        {
                            CreateNewVersion = true
                        });

                    await _documentoAmministrativoRepository.Add(documentoAmministrativoAllegatoEntesuap);

                    var fileValidateAllegatoResult = await this._fileValidatorService.Validate(new FileToValidate()
                    {
                        Name = newDocumentBlobAggregate.FileName,
                        Stream = newDocumentBlobAggregate.Stream
                    });

                    await _mediator.Send(new Requests.DocumentoAddInfoFileRequest(new DocsPaVO.documento.FileRequest()
                    {
                        docNumber = documentoAmministrativoAllegatoEntesuap.Id,
                        versionId = documentoAmministrativoAllegatoEntesuap.CurrentVersion.Id,
                        fileName = newDocumentBlobAggregate.FileName,
                        dataAcquisizione = newDocumentBlobAggregate.CreationDate.AsDateTimeFormat()
                    },
                    documentoAmministrativoAllegatoEntesuap.IdDocPrimario?.Identiticativo.AsLong(),
                    fileValidateAllegatoResult));
                }
                else
                {
                    var componentsEntity = await _dbContext.ComponentEntities.AsNoTracking()
                           .Where(c => c.VERSION_ID == allEntesuap.versionId.AsLong())
                           .Select(c => new
                           {
                               c.PATH,
                               c.VAR_NOMEORIGINALE,
                               c.EXT
                           })
                           .FirstOrDefaultAsync();

                    DocumentBlob blob = await this._documentBlobRepository.Get(request.infoUtenteInterop.idAmministrazione, componentsEntity.PATH);
                    using (var memoryStream = new MemoryStream())
                    {
                        blob.Stream.CopyTo(memoryStream);

                        string xmlAll = System.Text.ASCIIEncoding.ASCII.GetString(memoryStream.ToArray());
                        if (CompareEnteSuapXml(xmlAll, xml))
                            return new AttachXmlSuapResult(schedaDocumento.allegati);
                    }

                    documentoAmministrativoAllegatoEntesuap = await this._documentoAmministrativoRepository.Get(request.infoUtenteInterop.idAmministrazione, allEntesuap.docNumber, new ILoadBehavior[1]
                    {
                            new GetDocumentoAmministrativoLoadBehavior()
                            {
                                LoadProfiles = false,
                                LoadClassifications = false,
                                LoadAllegati = false,
                                LoadAggregazioni = false,
                                LoadVersions = true,
                                LoadPermissions = false,
                                LoadMittentiDestinatari = false,
                                LoadKeywords = false,
                                LoadNote = false
                            }
                    });

                    await _documentBlobRepository.Add(newDocumentBlobAggregate);

                    documentoAmministrativoAllegatoEntesuap.AssignDocumentBlobRef(
                        new DocumentBlobRef()
                        {
                            IdBlob = newDocumentBlobAggregate.Id,
                            CreationDate = await _dbContext.GetSystemDateTime(),
                            ContentType = newDocumentBlobAggregate.ContentType,
                            FileName = newDocumentBlobAggregate.FileName,
                            FileSize = newDocumentBlobAggregate.FileSize,
                            Hash = newDocumentBlobAggregate.Hash,
                            HashName = newDocumentBlobAggregate.HashName == Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256
                                ? HashNamesEnum.SHA256 : HashNamesEnum.SHA512,
                            Cartaceo = false,
                            SegnaturaPermanente = false,
                            TipoFirma = TipoFirmaEnum.Nessuna
                        },
                        new TargetVersionBehavior()
                        {
                            CreateNewVersion = true,
                            Name = new TextValue(Descriptions.NuovaVersioneEntesuap)
                        });

                    await _documentoAmministrativoRepository.Update(documentoAmministrativoAllegatoEntesuap);

                    var fileValidateAllegatoResult = await this._fileValidatorService.Validate(new FileToValidate()
                    {
                        Name = newDocumentBlobAggregate.FileName,
                        Stream = newDocumentBlobAggregate.Stream
                    });

                    await _mediator.Send(new Requests.DocumentoAddInfoFileRequest(new DocsPaVO.documento.FileRequest()
                    {
                        docNumber = documentoAmministrativoAllegatoEntesuap.Id,
                        versionId = documentoAmministrativoAllegatoEntesuap.CurrentVersion.Id,
                        fileName = newDocumentBlobAggregate.FileName,
                        dataAcquisizione = newDocumentBlobAggregate.CreationDate.AsDateTimeFormat()
                    },
                    documentoAmministrativoAllegatoEntesuap.IdDocPrimario?.Identiticativo.AsLong(),
                    fileValidateAllegatoResult));
                }

                schedaDocumento = (await _mediator.Send(new Requests.DocumentoGetDettaglioDocumentoNoSecurity(request.infoUtenteInterop, request.docnumber, request.docnumber))).output;
                allegati = schedaDocumento.allegati.ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new AttachXmlSuapResult(allegati.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AttachXmlSuapHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IFileValidatorService _fileValidatorService;

        protected async Task<string> ExportEnteSuapXML(InfoUtente infoUtente, SchedaDocumento schedaDocumento, string mailFrom)
        {
            var xmlResult = string.Empty;

            try
            {
                var pecSuap = mailFrom;
                var enteMittSuap = (schedaDocumento.registro != null && !string.IsNullOrEmpty(schedaDocumento.registro.codAmministrazione)) ? schedaDocumento.registro.codAmministrazione : string.Empty;

                var attributoOggettoComunicazione = GetValoreOggettoGenerico(schedaDocumento.template, Descriptions.TipologiaOggettoComunicazioneENTESUAP);
                var attributoCodicePratica = GetValoreOggettoGenerico(schedaDocumento.template, Descriptions.TipologiaCodicePraticaENTESUAP);

                var valueOggettoComunicazione = schedaDocumento.oggetto.descrizione;

                var nomeFilePrincipale = await GetOriginalFileNameOfDocPrincipale(infoUtente, schedaDocumento);
                var testoComunicazione = attributoOggettoComunicazione.ToUpper().Trim() == Descriptions.Altro ? String.Format(Descriptions.ComunicazioneAllegataAltro, nomeFilePrincipale) 
                    : string.Format(Descriptions.ComunicazioneAllegata, attributoOggettoComunicazione.Replace("-", " "), nomeFilePrincipale);

                var idSuap = await this._configurationService.GetValue<long>(infoUtente.idAmministrazione, "BE_IDENTIFICATIVO_SUAP");
                if (idSuap == 0)
                    return xmlResult;

                if(string.IsNullOrEmpty(attributoCodicePratica))
                    return xmlResult;

                SUAPEnte.CooperazioneEnteSUAP enteSuap = new SUAPEnte.CooperazioneEnteSUAP
                {
                    infoschema = new SUAPEnte.CooperazioneEnteSUAPInfoschema 
                    { 
                        data = await _dbContext.GetSystemDateTime(),
                        versione = "1.0.0" 
                    },
                    intestazione = new SUAPEnte.CooperazioneEnteSUAPIntestazione
                    {
                        codicepratica = attributoCodicePratica,
                        oggettocomunicazione = new SUAPEnte.OggettoCooperazione { tipocooperazione = attributoOggettoComunicazione, Value = valueOggettoComunicazione },
                        testocomunicazione = testoComunicazione,
                        entemittente = new SUAPEnte.EstremiEnte
                        {
                            pec = pecSuap,
                            Value = enteMittSuap
                        },
                        suapcompetente = new SUAPEnte.EstremiSuap
                        {
                            identificativosuap = idSuap.ToString(),
                            codiceamministrazione = "-",
                            codiceaoo = "-"
                        },
                        impresa = new SUAPEnte.AnagraficaImpresa
                        {
                            formagiuridica = new SUAPEnte.FormaGiuridica { codice = SUAPEnte.FormaGiuridicaCodice.AA, Value = "-" },
                            ragionesociale = "-",
                            legalerappresentante = new SUAPEnte.AnagraficaRappresentante1
                            {
                                cognome = "-",
                                nome = "-",
                                codicefiscale = "AAAAAA00A00A000A",
                                carica = new SUAPEnte.Carica1 { codice = SUAPEnte.CaricaCodice.ACP, Value = "-" }

                            },
                            indirizzo = new SUAPEnte.IndirizzoConRecapiti
                            {
                                stato = new SUAPEnte.Stato { codice = "0", Value = "-" },
                                denominazionestradale = "-",
                                numerocivico = "-",
                                Items = new List<object>(){"-"}
                            }
                        },
                        oggettopratica = new SUAPEnte.OggettoComunicazione { Value = "-" },
                        protocollopraticasuap = new SUAPEnte.ProtocolloSUAP
                        {
                            numeroregistrazione = "0000000",
                            dataregistrazione = DateTime.MinValue,
                            codiceaoo = "-",
                            codiceamministrazione = "-"
                        }
                    }
                };

                if(schedaDocumento.protocollo != null)
                {
                    enteSuap.intestazione.protocollo = new SUAPEnte.ProtocolloSUAP
                    {
                        codiceamministrazione = schedaDocumento.registro.codAmministrazione,
                        codiceaoo = schedaDocumento.registro.codRegistro,
                        dataregistrazione = schedaDocumento.protocollo.dataProtocollazione.AsDateTime(),
                        numeroregistrazione = schedaDocumento.protocollo.numero.PadLeft(7, '0')
                    };
                }

                List<SUAPEnte.AllegatoCooperazione> allegati = new List<SUAPEnte.AllegatoCooperazione>();

                if (schedaDocumento.documenti != null && schedaDocumento.documenti.Length > 0)
                {
                    DocsPaVO.documento.FileRequest doc = schedaDocumento.documenti[0] as DocsPaVO.documento.FileRequest;
                    if (doc != null)
                    {
                        string descr = "---";
                        if (!String.IsNullOrEmpty(doc.descrizione))
                            descr = doc.descrizione;

                        string originalName = await GetOriginalFileName(doc.versionId, doc.docNumber);
                        if (originalName != null && !originalName.ToUpper().Equals(Descriptions.EnteSuapXML))
                        {
                            allegati.Add(new SUAPEnte.AllegatoCooperazione 
                            { 
                                cod = "ALLEG", 
                                descrizione = descr, 
                                dimensione = doc.fileSize.ToString(), 
                                mime = await GetContentType(originalName), 
                                nomefile = originalName, 
                                nomefileoriginale = originalName 
                            });
                        }
                    }
                }

                foreach (var all in schedaDocumento.allegati)
                {
                    string originalName = await GetOriginalFileName(all.versionId, all.docNumber);
                    if (originalName != null)
                    {
                        //solo allegati utente
                        if (all.TypeAttachment != 1)
                            continue;

                        if (originalName.ToUpper().Equals(Descriptions.EnteSuapXML))
                            continue;

                        allegati.Add(new SUAPEnte.AllegatoCooperazione 
                        { 
                            cod = "ALLEG", 
                            descrizione = all.descrizione,
                            dimensione = all.fileSize.ToString(), 
                            mime = await GetContentType(originalName),
                            nomefile = originalName,
                            nomefileoriginale = originalName
                        });
                    }
                }

                //aggiungo gli allegati
                enteSuap.allegato = allegati;
                xmlResult = enteSuap.Serialize();
                xmlResult = RemoveNodiInutilizzati(xmlResult);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return xmlResult;
        }

        private string GetValoreOggettoGenerico(DocsPaVO.ProfilazioneDinamica.Templates t, string nome)
        {
            string retval = string.Empty;
            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = t.ELENCO_OGGETTI.Where(o => o.DESCRIZIONE.ToLower().Equals(nome.ToLower())).First();
            if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina"))
            {
                retval = ogg.VALORE_DATABASE.Replace('_', '-');
            }
            else
            {
                retval = ogg.VALORE_DATABASE;
            }

            return retval;
        }

        private async Task<string> GetOriginalFileNameOfDocPrincipale(InfoUtente infoUtente, SchedaDocumento schedaDocumento)
        {
            string originalName = null;
            DocsPaVO.documento.FileRequest doc = schedaDocumento.documenti[0] as DocsPaVO.documento.FileRequest;
            if (doc != null)
                originalName = await GetOriginalFileName(doc.versionId, doc.docNumber);

            return originalName;
        }

        private async Task<string> GetOriginalFileName(string versionId, string docnumber)
        {
            var originalName = await _dbContext.ComponentEntities.AsNoTracking()
                    .Where(c => c.VERSION_ID == versionId.AsLong() && c.DOCNUMBER == docnumber.AsLong())
                    .Select(c => c.VAR_NOMEORIGINALE)
                    .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(originalName))
            {
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                foreach (char c in invalid)
                    originalName = originalName.Replace(c.ToString(), "_");
            }

            return originalName;
        }

        private async Task<string> GetContentType(string filename)
        {
            string[] extArr = filename.Split('.');
            string ext = extArr[extArr.Length - 1].ToLower();

            List<Applicazione> applications = new List<Applicazione>();

            var appsEntity = await this._dbContext.AppEntities.AsNoTracking()
                .Where(a => a.DEFAULT_EXTENSION.ToUpper() == ext.ToUpper())
                .Select(a => new
                {
                    a.DEFAULT_EXTENSION,
                    a.MIME_TYPE
                })
                .ToListAsync();

            appsEntity.ForEach(a =>
             applications.Add(new Applicazione
             {
                 estensione = a.DEFAULT_EXTENSION,
                 mimeType = a.MIME_TYPE
             })
            );

            if (applications.Count == 0)
            {
                var appEntity = new AppEntity
                {
                    APPLICATION = "GEN_" + ext,
                    DESCRIPTION = "GEN_" + ext,
                    FILING_SCHEME = 2,
                    DEFAULT_EXTENSION = ext
                };

                await this._dbContext.AppEntities.AddAsync(appEntity);

                await ((DbContext)_dbContext).SaveChangesAsync();

                applications.Add(new Applicazione
                {
                    estensione = appEntity.DEFAULT_EXTENSION,
                    mimeType = appEntity.MIME_TYPE
                });
            }
            var app = applications[0];

            return app != null && !string.IsNullOrEmpty(app.mimeType) ? app.mimeType : "application/x-" + ext;
        }

        private string RemoveNodiInutilizzati(string xml)
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(xml);

                XmlNode nodeRea = xd.SelectSingleNode("//impresa//codice-REA");
                nodeRea.ParentNode.RemoveChild(nodeRea);
                XmlNode nodeNazionalita = xd.SelectSingleNode("//impresa//legale-rappresentante//nazionalita");
                nodeNazionalita.ParentNode.RemoveChild(nodeNazionalita);

                XmlNode noderi = xd.SelectSingleNode("//protocollo-ri");
                noderi.ParentNode.RemoveChild(noderi);

                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.Formatting = Formatting.Indented;

                xd.WriteTo(xmlTextWriter);
                return stringWriter.ToString();
            }
            catch (Exception e)
            {
                return xml;
            }
        }

        protected bool CompareEnteSuapXml(string xml1, string xml2)
        {
            xml1 = RemoveInfoschema(xml1);
            xml2 = RemoveInfoschema(xml2);
            if (xml1 == xml2)
                return true;

            return false;
        }


        protected string RemoveInfoschema(string xml)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xml);

            XmlNode node = xd.SelectSingleNode("//info-schema");
            node.ParentNode.RemoveChild(node);

            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);

            xd.WriteTo(xmlTextWriter);
            return stringWriter.ToString();
        }

        #endregion
    }

}
