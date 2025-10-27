// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.GetListaRegistriByRuolo;
using AutoMapper;
using DocsPaVO.Modelli_Trasmissioni;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using System.Security.Claims;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.DocumentMetadata.DocumentMetadataManagement;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetInfoFile;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetAllegati;
using DocsPaVO.DocumentMetadata.DocumentoAmministrativoInformatico;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentMetadata.DocumentMetadataManagement
{
    // Richiede libreria MediatR
    public class DocumentMetadataManagementCommandHandler : MessageQueueBaseCommandHandler<DocumentMetadataManagementCommand>
    {
        #region Public Members

        public DocumentMetadataManagementCommandHandler(ILogger<DocumentMetadataManagementCommandHandler> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        { }

        protected override async Task InternalHandle(IServiceProvider serviceProvider, DocumentMetadataManagementCommand message)
        {

            var claimsPrincipalService = serviceProvider.GetRequiredService<IClaimsPrincipalService>();
            IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
            IPi3DbContext dbContext = serviceProvider.GetRequiredService<IPi3DbContext>();

            try
            {
                var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var anagraficaLogEntity = await dbContext.AnagraficaLogEntities
                    .AsNoTracking()
                    .Where(a => a.VAR_METODO == message.Method
                            && (a.ID_AMM == idTenant || a.ID_AMM == null))
                    .FirstOrDefaultAsync() ?? throw new CodiceEventoNotFoundPi3Exception(message.Method);
                switch (anagraficaLogEntity.VAR_OGGETTO)
                {
                    case "DOCUMENTO":
                        if (anagraficaLogEntity.VAR_CODICE.Equals("AGG_DOC_GRIGIO") || anagraficaLogEntity.VAR_CODICE.Equals("AGG_PROT"))
                            await CreateDocumentoAmministrativoInformatico(anagraficaLogEntity,
                                dbContext,
                                claimsPrincipalService,
                                mediator,
                                message
                                );
                        else
                            await UpdateDocumentoAmministrativoInformatico(anagraficaLogEntity,
                                dbContext,
                                claimsPrincipalService,
                                mediator,
                                message
                                );
                        break;
                    case "FASCICOLO":
                    case "FOLDER":
                        await CreateUpdateAggregazioniDocumentaliInformatiche(anagraficaLogEntity,
                               dbContext,
                               claimsPrincipalService,
                               mediator,
                               message
                               );
                        break;
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                _logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: ex.Message);
            }


            await ((DbContext)dbContext).SaveChangesAsync();
        }

        #endregion

        #region Private Members


        protected async Task CreateDocumentoAmministrativoInformatico(AnagraficaLogEntity anagraficaLogEntity,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            DocumentMetadataManagementCommand message)
        {
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idGroup = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeople = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idCorrGlobali = await dbContext.CorrGlobaliEntities
                .AsNoTracking()
                .Where(c => c.ID_GRUPPO == idGroup)
                .Select(c => c.SYSTEM_ID)
                .FirstOrDefaultAsync();

            DocsPaVO.DocumentMetadata.MetadatiDocumento metadati = null;
            DocumentoAmministrativoInformaticoType documentoAmministrativoInformatico = null;
            try
            {
                InfoUtente infoUtente = new InfoUtente()
                {
                    idPeople = idPeople.ToString(),
                    idGruppo = idGroup.ToString(),
                    idAmministrazione = idTenant.ToString(),
                    idCorrGlobali = idCorrGlobali.ToString()
                };
                DocsPaVO.documento.SchedaDocumento schedaDoc = (await mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
                {
                    Infoutente = infoUtente,
                    DocNumber = message.IdOggetto.ToString(),
                    IdProfile = message.IdOggetto.ToString()
                })).Output;

                documentoAmministrativoInformatico = await AddDocumentoAmministrativoInformatico(schedaDoc,
                    infoUtente,
                    dbContext,
                    claimsPrincipalService,
                    mediator);

                if (documentoAmministrativoInformatico != null)
                {
                    var metadatiDocumentoEntity = new MetadatiDocumentoEntity()
                    {
                        ID_PROFILE = schedaDoc.systemId.AsLong(),
                        ID_VERSION = schedaDoc.documenti[0].versionId.AsLong(),
                        METADATI_XML = documentoAmministrativoInformatico.ToXmlString(true, false, false, Encoding.UTF8),
                        DTA_INSERIMENTO = await dbContext.GetSystemDateTime(),
                        DTA_AZIONE = message.DataAzione,
                        VAR_COD_AZIONE = anagraficaLogEntity.VAR_CODICE,
                        VAR_DESC_AZIONE = anagraficaLogEntity.VAR_DESCRIZIONE,
                        VAR_OGGETTO = anagraficaLogEntity.VAR_OGGETTO
                    };

                    await dbContext.MetadatiDocumentoEntities.AddAsync(metadatiDocumentoEntity);

                    await ((DbContext)dbContext).SaveChangesAsync();

                    //if (result)
                    //{
                    //    db.DeleteEventDocument(eventDoc.SystemId);
                    //}
                    //else
                    //    db.UpdateEventDocument(eventDoc.SystemId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: ex.Message, "Errore in CreateDocumentoAmministrativoInformatico, event: " + message.IdOggetto);
                //db.UpdateEventDocument(eventDoc.SystemId);
            }
        }

        protected async Task UpdateDocumentoAmministrativoInformatico(AnagraficaLogEntity anagraficaLogEntity,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            DocumentMetadataManagementCommand message)
        {
            DocumentoAmministrativoInformaticoType documentoAmministrativoInformatico = null;
            bool result = true;
            bool aggiornaMetadati = true;
            try
            {
                var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idGroup = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idPeople = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idCorrGlobali = await dbContext.CorrGlobaliEntities
                    .AsNoTracking()
                    .Where(c => c.ID_GRUPPO == idGroup)
                    .Select(c => c.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                var idProfile = message.IdOggetto;


                InfoUtente infoUtente = new InfoUtente()
                {
                    idPeople = idPeople.ToString(),
                    idGruppo = idGroup.ToString(),
                    idAmministrazione = idTenant.ToString(),
                    idCorrGlobali = idCorrGlobali.ToString()
                };

                DocsPaVO.documento.SchedaDocumento schedaDoc = null;

                long? oldMetadato = null;
                bool isAllegato = false;
                var idDocumentoPrincipale = await dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == message.IdOggetto)
                    .Select(p => p.ID_DOCUMENTO_PRINCIPALE)
                    .FirstOrDefaultAsync();

                if (idDocumentoPrincipale != null)
                {
                    idProfile = idDocumentoPrincipale.Value;
                    isAllegato = true;
                }

                var metadatiDocumentoEntity = await dbContext.MetadatiDocumentoEntities
                    .Where(m => m.ID_PROFILE == idProfile)
                    .OrderByDescending(m => m.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                if (metadatiDocumentoEntity != null)
                {
                    oldMetadato = metadatiDocumentoEntity.SYSTEM_ID;
                    documentoAmministrativoInformatico = FromXmlString(documentoAmministrativoInformatico, metadatiDocumentoEntity.METADATI_XML);
                }
                else
                {
                    schedaDoc = (await mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
                    {
                        Infoutente = infoUtente,
                        DocNumber = message.IdOggetto.ToString(),
                        IdProfile = message.IdOggetto.ToString()
                    })).Output;

                    documentoAmministrativoInformatico = await AddDocumentoAmministrativoInformatico(schedaDoc, infoUtente, dbContext, claimsPrincipalService, mediator);
                    metadatiDocumentoEntity = new MetadatiDocumentoEntity()
                    {
                        ID_PROFILE = schedaDoc.docNumber.AsLong(),
                        ID_VERSION = schedaDoc.documenti[0].versionId.AsLong()
                    };
                }

                switch (anagraficaLogEntity.VAR_CODICE)
                {
                    case "DOC_ADD_INCLASS":
                    case "DOC_DEL_FROM_FOLDER":
                    case "DOC_ADD_INFASC":
                    case "DOC_ADD_INFOLDER":
                        documentoAmministrativoInformatico = await AddClassificaAgg(documentoAmministrativoInformatico,
                            message.IdOggetto,
                            dbContext);
                        break;
                    case "ADD_VERSION":
                    case "DOC_RIMUOVI_VERSIONE":
                    case "PUT_FILE":
                    case "DOCUMENTOCONVERSIONEPDF":
                    case "DOC_SIGNATURE":
                    case "DOC_SIGNATURE_P":
                        documentoAmministrativoInformatico = await UpdateVersione(documentoAmministrativoInformatico,
                            message.DataAzione,
                            anagraficaLogEntity.VAR_CODICE,
                            message.IdOggetto,
                            isAllegato,
                            mediator,
                            dbContext,
                            infoUtente,
                            claimsPrincipalService);
                        break;
                    case "DOC_NEW_ALLEGATO":
                    case "ADD_ALLEGATO":
                    case "DOC_RIMUOVI_ALLEGATO":
                        documentoAmministrativoInformatico = await UpdateAllegati(documentoAmministrativoInformatico,
                            message.DataAzione,
                            message.IdOggetto,
                            infoUtente,
                            anagraficaLogEntity.VAR_CODICE,
                            dbContext, mediator);
                        break;
                    case "MODIFIED_OBJECT_DOC":
                    case "MODIFIED_OBJECT_PROTO":
                        documentoAmministrativoInformatico = await UpdateChiaveDescrittiva(documentoAmministrativoInformatico,
                            message.IdOggetto,
                            dbContext);
                        break;
                    case "MOD_MITT_DEST":
                        if (schedaDoc == null || !string.IsNullOrEmpty(schedaDoc.docNumber))
                        {
                            schedaDoc = (await mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
                            {
                                Infoutente = infoUtente,
                                DocNumber = message.IdOggetto.ToString(),
                                IdProfile = message.IdOggetto.ToString()
                            })).Output;
                        }
                        documentoAmministrativoInformatico = UpdateMittDest(documentoAmministrativoInformatico, schedaDoc, infoUtente);
                        break;
                    case "ACCEPT_TRASM_DOCUMENT":
                    case "CHECK_TRASM_DOCUMENT":
                        var utente = await GetUtente(infoUtente.idPeople.AsLong(), dbContext);
                        documentoAmministrativoInformatico = AddSoggettoOperatore(documentoAmministrativoInformatico, utente);
                        break;
                    case "ANNULLA_PROTO":
                    case "DOCUMENTO_EXEC_ANNULLA_REPERTORIO":
                        documentoAmministrativoInformatico = await AddTracciaturaAnnullamento(documentoAmministrativoInformatico,
                            message.DataAzione,
                            infoUtente,
                            dbContext);
                        break;
                    case "RECORD_PREDISPOSED":
                        if (schedaDoc == null || !string.IsNullOrEmpty(schedaDoc.docNumber))
                        {
                            schedaDoc = (await mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
                            {
                                Infoutente = infoUtente,
                                DocNumber = message.IdOggetto.ToString(),
                                IdProfile = message.IdOggetto.ToString()
                            })).Output;
                        }
                        documentoAmministrativoInformatico = UpdateDatiProtocollo(documentoAmministrativoInformatico, schedaDoc, infoUtente);
                        break;
                    case "DOCUMENTO_REPERTORIATO":
                        aggiornaMetadati = false;
                        if (string.IsNullOrEmpty(documentoAmministrativoInformatico.IdDoc.Segnatura))
                        {
                            aggiornaMetadati = true;
                            documentoAmministrativoInformatico = await UpdateSegnaturaRepertorio(documentoAmministrativoInformatico,
                                message.IdOggetto,
                                dbContext);
                        }
                        break;
                    case "SALVADOCUMENTO":
                        aggiornaMetadati = false;
                        if (documentoAmministrativoInformatico.TipologiaDocumentale.Equals("Protocollo") || documentoAmministrativoInformatico.TipologiaDocumentale.Equals("Documento non protocollato"))
                        {
                            var template = await dbContext.ProfileEntities.AsNoTracking()
                                .Join(dbContext.TipoAttoEntities.AsNoTracking(),
                                    p => p.ID_TIPO_ATTO,
                                    t => t.SYSTEM_ID,
                                    (p, t) => new { p, t })
                                .Where(j => j.p.SYSTEM_ID == message.IdOggetto)
                                .Select(j => new
                                {
                                    j.t.SYSTEM_ID,
                                    j.t.VAR_DESC_ATTO
                                })
                                .FirstOrDefaultAsync();

                            if (template != null)
                            {
                                documentoAmministrativoInformatico.TipologiaDocumentale = template.VAR_DESC_ATTO;
                                aggiornaMetadati = true;
                            }
                        }
                        break;
                    case "SCAMBIA_DOC":
                        documentoAmministrativoInformatico = await ScambiaDocumento(documentoAmministrativoInformatico,
                            message.DescrizioneOggetto,
                            message.IdOggetto,
                            infoUtente,
                            mediator,
                            claimsPrincipalService,
                            dbContext);
                        break;
                }
                if (documentoAmministrativoInformatico != null)
                {
                    if (aggiornaMetadati)
                    {
                        var metadatiDocumentoNewEntity = new MetadatiDocumentoEntity()
                        {
                            ID_PROFILE = metadatiDocumentoEntity.ID_PROFILE,
                            ID_VERSION = metadatiDocumentoEntity.ID_VERSION,
                            METADATI_XML = documentoAmministrativoInformatico.ToXmlString(true, false, false, Encoding.UTF8),
                            DTA_AZIONE = message.DataAzione,
                            VAR_COD_AZIONE = anagraficaLogEntity.VAR_CODICE,
                            VAR_DESC_AZIONE = anagraficaLogEntity.VAR_DESCRIZIONE,
                            VAR_OGGETTO = anagraficaLogEntity.VAR_OGGETTO,
                            DTA_INSERIMENTO = await dbContext.GetSystemDateTime()
                        };

                        await dbContext.MetadatiDocumentoEntities.AddAsync(metadatiDocumentoNewEntity);

                        if (oldMetadato != null) //Rimuovo il vecchio metadato
                        {
                            dbContext.MetadatiDocumentoEntities.Remove(metadatiDocumentoEntity);
                        }

                        await ((DbContext)dbContext).SaveChangesAsync();
                    }
                    //if (result)
                    //{
                    //    db.DeleteEventDocument(eventDoc.SystemId);
                    //}
                    //else
                    //    db.UpdateEventDocument(eventDoc.SystemId);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: ex.Message);

                //DocsPaDB.Query_DocsPAWS.DocumentMetadata db = new DocsPaDB.Query_DocsPAWS.DocumentMetadata();
                //db.UpdateEventDocument(eventDoc.SystemId);
            }
        }

        protected async Task CreateUpdateAggregazioniDocumentaliInformatiche(AnagraficaLogEntity anagraficaLogEntity,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            DocumentMetadataManagementCommand message)
        {
            bool aggiornaMetadati = false;
            MetadatiFascicoloEntity? metadatiFascicoloEntity = null;
            DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType aggregazioniDocumentaliInformatiche = null;

            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idGroup = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeople = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idCorrGlobali = await dbContext.CorrGlobaliEntities
                .AsNoTracking()
                .Where(c => c.ID_GRUPPO == idGroup)
                .Select(c => c.SYSTEM_ID)
                .FirstOrDefaultAsync();

            InfoUtente infoUtente = new InfoUtente()
            {
                idPeople = idPeople.ToString(),
                idGruppo = idGroup.ToString(),
                idAmministrazione = idTenant.ToString(),
                idCorrGlobali = idCorrGlobali.ToString()
            };
            try
            {
                long? oldMetadato = null;
                switch (anagraficaLogEntity.VAR_CODICE)
                {
                    case "NEW_FASC":
                        aggregazioniDocumentaliInformatiche = await AddAggregazioniDocumentaliInformatiche(message.IdOggetto,
                            infoUtente,
                            dbContext,
                            claimsPrincipalService,
                            mediator);

                        if (aggregazioniDocumentaliInformatiche != null)
                        {
                            metadatiFascicoloEntity = new MetadatiFascicoloEntity()
                            {
                                ID_PROJECT = message.IdOggetto
                            };

                            aggiornaMetadati = true;
                        }

                        break;
                    case "FASC_ADD_DOC":
                    case "FOLDER_ADD_DOC":
                        metadatiFascicoloEntity = await dbContext.MetadatiFascicoloEntities
                            .Where(m => m.ID_PROJECT == message.IdOggetto)
                            .OrderByDescending(m => m.SYSTEM_ID)
                            .FirstOrDefaultAsync();
                        if (metadatiFascicoloEntity != null)
                        {
                            oldMetadato = metadatiFascicoloEntity.SYSTEM_ID;
                            aggregazioniDocumentaliInformatiche = FromXmlString(aggregazioniDocumentaliInformatiche, metadatiFascicoloEntity.METADATI_XML);
                            aggregazioniDocumentaliInformatiche = await AddDocumento(aggregazioniDocumentaliInformatiche, message.DescrizioneOggetto, dbContext);
                            if (aggregazioniDocumentaliInformatiche != null)
                                aggiornaMetadati = true;
                        }
                        break;
                    case "FASC_DEL_DOC":
                    case "FOLDER_DEL_DOC":
                    case "DOC_DEL_FROM_FOLDER":
                        metadatiFascicoloEntity = await dbContext.MetadatiFascicoloEntities
                           .Where(m => m.ID_PROJECT == message.IdOggetto)
                           .OrderByDescending(m => m.SYSTEM_ID)
                           .FirstOrDefaultAsync();
                        if (metadatiFascicoloEntity != null)
                        {
                            oldMetadato = metadatiFascicoloEntity.SYSTEM_ID;
                            aggregazioniDocumentaliInformatiche = FromXmlString(aggregazioniDocumentaliInformatiche, metadatiFascicoloEntity.METADATI_XML);
                            aggregazioniDocumentaliInformatiche = RimuoviDocumento(aggregazioniDocumentaliInformatiche, message.DescrizioneOggetto);
                            if (aggregazioniDocumentaliInformatiche != null)
                                aggiornaMetadati = true;
                        }
                        break;
                    case "FASC_MODIFY":
                        metadatiFascicoloEntity = await dbContext.MetadatiFascicoloEntities
                           .Where(m => m.ID_PROJECT == message.IdOggetto)
                           .OrderByDescending(m => m.SYSTEM_ID)
                           .FirstOrDefaultAsync();
                        if (metadatiFascicoloEntity != null)
                        {
                            oldMetadato = metadatiFascicoloEntity.SYSTEM_ID;
                            aggregazioniDocumentaliInformatiche = FromXmlString(aggregazioniDocumentaliInformatiche, metadatiFascicoloEntity.METADATI_XML);
                            aggregazioniDocumentaliInformatiche = await UpdateAggregazioniDocumentaliInformatiche(aggregazioniDocumentaliInformatiche, message.IdOggetto, infoUtente, dbContext);
                            if (aggregazioniDocumentaliInformatiche != null)
                                aggiornaMetadati = true;
                        }
                        break;
                    case "ACCEPT_TRASM_FOLDER":
                        metadatiFascicoloEntity = await dbContext.MetadatiFascicoloEntities
                           .Where(m => m.ID_PROJECT == message.IdOggetto)
                           .OrderByDescending(m => m.SYSTEM_ID)
                           .FirstOrDefaultAsync();
                        if (metadatiFascicoloEntity != null)
                        {
                            oldMetadato = metadatiFascicoloEntity.SYSTEM_ID;
                            aggregazioniDocumentaliInformatiche = FromXmlString(aggregazioniDocumentaliInformatiche, metadatiFascicoloEntity.METADATI_XML);
                            aggregazioniDocumentaliInformatiche = await AddAssegnazione(aggregazioniDocumentaliInformatiche,
                                message.IdOggetto, message.DataAzione,
                                message.IdTrasmissione.Value,
                                infoUtente,
                                dbContext,
                                mediator,
                                claimsPrincipalService);
                            if (aggregazioniDocumentaliInformatiche != null)
                                aggiornaMetadati = true;
                        }
                        break;
                }

                if (aggiornaMetadati)
                {
                    var metadatiFascicoloNewEntity = new MetadatiFascicoloEntity()
                    {
                        ID_PROJECT = metadatiFascicoloEntity.ID_PROJECT,
                        METADATI_XML = aggregazioniDocumentaliInformatiche.ToXmlString(true, false, false, Encoding.UTF8),
                        DTA_AZIONE = message.DataAzione,
                        VAR_COD_AZIONE = anagraficaLogEntity.VAR_CODICE,
                        VAR_DESC_AZIONE = anagraficaLogEntity.VAR_DESCRIZIONE,
                        VAR_OGGETTO = anagraficaLogEntity.VAR_OGGETTO,
                        DTA_INSERIMENTO = await dbContext.GetSystemDateTime()
                    };

                    await dbContext.MetadatiFascicoloEntities.AddAsync(metadatiFascicoloNewEntity);

                    if (oldMetadato != null) //Rimuovo il vecchio metadato
                    {
                        dbContext.MetadatiFascicoloEntities.Remove(metadatiFascicoloEntity);
                    }

                    await ((DbContext)dbContext).SaveChangesAsync();
                }

                //if (result)
                //    db.DeleteEventDocument(eventDoc.SystemId);
                //else
                //    db.UpdateEventDocument(eventDoc.SystemId);

            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: ex.Message);
                //db.UpdateEventDocument(eventDoc.SystemId);
            }
        }

        #region DOCUMENTO AMMINISTRATIVO INFORMATICO


        protected async Task<DocumentoAmministrativoInformaticoType> AddDocumentoAmministrativoInformatico(DocsPaVO.documento.SchedaDocumento schedaDoc,
            InfoUtente infoUtente,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator)
        {
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var docnumberAsLong = schedaDoc.docNumber.AsLong();

            DocumentoAmministrativoInformaticoType documentoAmmInformatico = new DocumentoAmministrativoInformaticoType();

            try
            {
                DocsPaVO.documento.CreatoreDocumento creatore = schedaDoc.creatoreDocumento;
                var idPeopleCreatore = creatore.idPeople.AsLong();

                var peopleEntity = await dbContext.PeopleEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == idPeopleCreatore)
                    .Select(p => new
                    {
                        p.VAR_COGNOME,
                        p.VAR_NOME,
                        p.FULL_NAME,
                        p.CHA_SYSTEM_USER
                    })
                    .FirstAsync();

                var amministraEntity = await dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.SYSTEM_ID == idTenant)
                    .FirstAsync();

                DocsPaVO.documento.FileDocumento fileDocumento = null;
                if (!string.IsNullOrEmpty(schedaDoc.documenti[0].fileSize) && Convert.ToInt32(schedaDoc.documenti[0].fileSize) > 0)
                    fileDocumento = (await mediator.Send(new DocumentoGetInfoFileCommand()
                    {
                        fileRequest = schedaDoc.documenti[0],
                        infoUtente = infoUtente
                    })).output;

                var fascicoli = await dbContext.ProjectComponentEntities.AsNoTracking()
                    .Join(dbContext.ProjectEntities.AsNoTracking(),
                        pc => pc.PROJECT_ID,
                        p => p.SYSTEM_ID,
                        (pc, p) => new { pc, p })
                    .Where(j => j.pc.LINK == docnumberAsLong)
                    .Select(j => new
                    {
                        j.p.SYSTEM_ID,
                        j.p.CHA_TIPO_PROJ,
                        j.p.VAR_CODICE,
                        j.p.DESCRIPTION
                    })
                    .ToListAsync();

                var classificazione = fascicoli?.Count() > 0 ? fascicoli.Where(f => f.CHA_TIPO_PROJ == "G").FirstOrDefault() : null;

                Registro registro = null;
                if (schedaDoc.registro != null && !string.IsNullOrEmpty(schedaDoc.registro.systemId))
                    registro = schedaDoc.registro;
                else
                    registro = (await mediator.Send(new GetListaRegistriByRuoloCommand()
                    {
                        IdRuolo = creatore.idCorrGlob_Ruolo
                    })).output[0];

                #region IdDoc

                var segnatura = schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.protocollo.segnatura) ? schedaDoc.protocollo.segnatura : string.Empty;

                //Se non è protocollato, verifico se è presente un rpertorio
                if (string.IsNullOrEmpty(segnatura) && schedaDoc.template != null)
                {
                    string dataAnnullamento = string.Empty;

                    segnatura = await dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                        .Join(dbContext.OggettiCustomEntities.AsNoTracking(),
                            a => a.ID_OGGETTO,
                            o => o.SYSTEM_ID,
                            (a, o) => new { a, o })
                        .Where(j => j.a.DOC_NUMBER == schedaDoc.docNumber && j.o.REPERTORIO == 1 && j.a.VALORE_OGGETTO_DB != null)
                        .Select(j => j.a.VAR_SEGNATURA)
                        .FirstOrDefaultAsync();
                }

                documentoAmmInformatico.IdDoc = new IdDocType()
                {
                    Segnatura = segnatura,
                    Identificativo = schedaDoc.docNumber
                };
                if (fileDocumento != null)
                {
                    documentoAmmInformatico.IdDoc.ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                    {
                        Impronta = ComputeHashAsSHA256((schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest).impronta),
                        Algoritmo = "SHA256"
                    };
                }
                #endregion

                #region Modalità di formazione
                documentoAmmInformatico.ModalitaDiFormazione = ModalitaDiFormazioneType.memorizzazionesusupportoinformaticoinformatodigitaledelleinformazionirisultantidatransazionioprocessiinformaticiodallapresentazionetelematicadidatiattraversomodulioformulariresidisponibiliadutente;
                #endregion

                #region Tipologia documentale
                if (schedaDoc.template != null && !string.IsNullOrEmpty(schedaDoc.template.SYSTEM_ID.ToString()))
                    documentoAmmInformatico.TipologiaDocumentale = schedaDoc.template.DESCRIZIONE;
                else
                    documentoAmmInformatico.TipologiaDocumentale = schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.protocollo.numero) ? "Protocollo" : "Documento non protocollato";
                #endregion

                #region Dati di registrazione

                //ATTENZIONE, in PiTre non abbiamo il multi registro, quindi possiamo prendere il registro dell'utente
                if (schedaDoc.protocollo == null || string.IsNullOrEmpty(schedaDoc.protocollo.numero))
                {
                    string numeroRegistrazione = schedaDoc.docNumber;
                    var dataRegistrazione = schedaDoc.dataCreazione.AsDateTime();

                    //Se repertoriato inserisco il numero di repertorio
                    if (schedaDoc.template != null && schedaDoc.template.SYSTEM_ID > 0)
                    {
                        var repertorio = (from DocsPaVO.ProfilazioneDinamica.OggettoCustom o in schedaDoc.template.ELENCO_OGGETTI
                                          where o.REPERTORIO.Equals("1") && !string.IsNullOrEmpty(o.VALORE_DATABASE)
                                          select o).FirstOrDefault();
                        if (repertorio != null)
                        {
                            numeroRegistrazione = repertorio.VALORE_DATABASE;
                            dataRegistrazione = repertorio.DATA_INSERIMENTO.AsDateTime();
                        }
                    }

                    documentoAmmInformatico.DatiDiRegistrazione = new DatiDiRegistrazioneType()
                    {
                        TipologiaDiFlusso = TipologiaDiFlussoType.I,
                        TipoRegistro = new TipoRegistroType()
                        {
                            Item = new NoProtocolloType()
                            {
                                DataRegistrazioneDocumento = dataRegistrazione,
                                OraRegistrazioneDocumento = dataRegistrazione,
                                OraRegistrazioneDocumentoSpecified = true,
                                NumeroRegistrazioneDocumento = numeroRegistrazione,
                                CodiceRegistro = registro.codRegistro
                            }
                        }
                    };
                }
                else
                    documentoAmmInformatico.DatiDiRegistrazione = new DatiDiRegistrazioneType()
                    {
                        TipologiaDiFlusso = schedaDoc.tipoProto == "A" ? TipologiaDiFlussoType.E : schedaDoc.tipoProto == "P" ? TipologiaDiFlussoType.U : TipologiaDiFlussoType.I,
                        TipoRegistro = new TipoRegistroType()
                        {
                            Item = new ProtocolloType()
                            {
                                DataProtocollazioneDocumento = schedaDoc.protocollo.dataProtocollazione.AsDateTime(),
                                OraProtocollazioneDocumento = schedaDoc.protocollo.dataProtocollazione.AsDateTime(),
                                OraProtocollazioneDocumentoSpecified = true,
                                NumeroProtocolloDocumento = CalcolaNumProto(schedaDoc.protocollo.numero),
                                CodiceRegistro = registro.codRegistro,
                            }
                        }
                    };
                #endregion

                #region Soggetti
                //Amministrazione che effettua la registrazione
                RuoloType ruoloAmmRegistrazione = new RuoloType()
                {
                    Item = new TipoSoggetto1Type()
                    {
                        PAI = new PAIType()
                        {
                            IPAAmm = new CodiceIPAType()
                            {
                                Denominazione = amministraEntity.VAR_DESC_AMM,
                                CodiceIPA = amministraEntity.VAR_CODICE_AMM_IPA
                            },
                            IPAAOO = new CodiceIPAType()
                            {
                                Denominazione = registro.descrizione,
                                CodiceIPA = registro.codiceIpa
                            },
                            IndirizziDigitaliDiRiferimento = new string[1] { amministraEntity.VAR_INDIRIZZO_DIGITALE_RIF }
                        }
                    }
                };
                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, ruoloAmmRegistrazione);

                #region Autore
                RuoloType autore = new RuoloType()
                {
                    Item = new TipoSoggetto41Type()
                    {
                        Item = new PFType()
                        {
                            Nome = peopleEntity.VAR_NOME,
                            Cognome = peopleEntity.VAR_COGNOME
                        }
                    }
                };
                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, autore);
                #endregion
                #region Produttore
                if (peopleEntity.CHA_SYSTEM_USER == "1")
                {

                    RuoloType produttore = new RuoloType()
                    {
                        Item = new TipoSoggetto5Type()
                        {
                            SW = new SWType()
                            {
                                DenominazioneSistema = peopleEntity.FULL_NAME
                            }
                        }
                    };
                    documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, produttore);
                }
                #endregion
                #region Mittente/Destinatario

                if (schedaDoc.protocollo != null)
                {
                    RuoloType mittente;
                    RuoloType destinatario;
                    switch (schedaDoc.tipoProto)
                    {
                        case "P":
                            DocsPaVO.documento.ProtocolloUscita protocolloUscita = schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita;
                            //Destinatari
                            if (protocolloUscita.destinatari != null)
                            {
                                foreach (Corrispondente dest in protocolloUscita.destinatari)
                                {
                                    documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddDestinatario(dest));
                                }
                            }
                            //Destinatari in conoscenza
                            if (protocolloUscita.destinatariConoscenza != null)
                            {
                                foreach (Corrispondente dest in protocolloUscita.destinatariConoscenza)
                                {
                                    documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddDestinatario(dest));
                                }
                            }
                            //Mittente
                            if (protocolloUscita.mittente != null)
                                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddMittente(protocolloUscita.mittente));

                            break;
                        case "A":
                            DocsPaVO.documento.ProtocolloEntrata protocolloEntrata = schedaDoc.protocollo as DocsPaVO.documento.ProtocolloEntrata;
                            if (protocolloEntrata.mittente != null)
                                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddMittente(protocolloEntrata.mittente));
                            break;
                        case "I":
                            DocsPaVO.documento.ProtocolloInterno protocolloInterno = schedaDoc.protocollo as DocsPaVO.documento.ProtocolloInterno;
                            //Destinatari
                            if (protocolloInterno.destinatari != null)
                            {
                                foreach (Corrispondente dest in protocolloInterno.destinatari)
                                {
                                    documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddDestinatario(dest));
                                }
                            }
                            //Destinatari in conoscenza
                            if (protocolloInterno.destinatariConoscenza != null)
                            {
                                foreach (Corrispondente dest in protocolloInterno.destinatariConoscenza)
                                {
                                    documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddDestinatario(dest));
                                }
                            }
                            //Mittente
                            if (protocolloInterno.mittente != null)
                                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddMittente(protocolloInterno.mittente));
                            break;
                    }
                }

                #endregion

                #endregion

                #region Chiave Descrittiva
                documentoAmmInformatico.ChiaveDescrittiva = new ChiaveDescrittivaType()
                {
                    Oggetto = schedaDoc.oggetto.descrizione
                };
                #endregion

                #region Allegati
                if (schedaDoc.allegati != null && schedaDoc.allegati.Count() > 0)
                {
                    documentoAmmInformatico.Allegati = new AllegatiType()
                    {
                        NumeroAllegati = schedaDoc.allegati.Count().ToString()
                    };

                    foreach (DocsPaVO.documento.Allegato allegato in schedaDoc.allegati)
                    {
                        IndiceAllegatiType indiceAllegatoType = new IndiceAllegatiType()
                        {
                            Descrizione = allegato.descrizione,
                            IdDoc = new IdDocType()
                            {
                                Identificativo = allegato.docNumber
                            }
                        };
                        if (!string.IsNullOrEmpty(allegato.fileSize) && Convert.ToInt32(allegato.fileSize) > 0)
                        {
                            indiceAllegatoType.IdDoc.ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                            {
                                Algoritmo = "SHA256",
                                Impronta = ComputeHashAsSHA256(allegato.impronta)
                            };
                        }
                        documentoAmmInformatico.Allegati.IndiceAllegati = AddAllegato(documentoAmmInformatico, indiceAllegatoType);
                    }
                }
                else
                {
                    documentoAmmInformatico.Allegati = new AllegatiType()
                    {
                        NumeroAllegati = "0",
                        IndiceAllegati = new IndiceAllegatiType[0]
                    };
                }
                #endregion

                #region Classificazione
                if (classificazione != null)
                {
                    documentoAmmInformatico.Classificazione = new ClassificazioneType()
                    {
                        IndiceDiClassificazione = classificazione.VAR_CODICE,
                        Descrizione = classificazione.DESCRIPTION
                    };
                }
                #endregion

                #region Riservato 
                documentoAmmInformatico.Riservato = schedaDoc.privato == "1";
                #endregion

                #region Identificativo del formato
                documentoAmmInformatico.IdentificativoDelFormato = new IdentificativoDelFormatoType()
                {
                    Formato = fileDocumento != null ? fileDocumento.estensioneFile : string.Empty
                };
                #endregion

                #region Verifica (OBBLIGATORIO nel caso di modalità di formazione doc = a/b)
                bool isFirmato = schedaDoc.documenti[0].firmato == "1";
                documentoAmmInformatico.Verifica = new VerificaType()
                {
                    FirmatoDigitalmente = isFirmato
                };
                #endregion

                #region Agg
                if (fascicoli != null && fascicoli.Count > 0)
                {
                    foreach (var fasc in fascicoli)
                    {
                        IdAggType agg = new IdAggType()
                        {
                            IdAggregazione = fasc.SYSTEM_ID.ToString(),
                            TipoAggregazione = TipoAggregazioneType.Fascicolo
                        };

                        documentoAmmInformatico.Agg = AddAgg(documentoAmmInformatico, agg);
                    }
                }
                #endregion

                #region Id identificativo documento primario (NON OBBLIGATORIO)
                #endregion

                #region Nome del documento
                if (fileDocumento != null)
                    documentoAmmInformatico.NomeDelDocumento = fileDocumento.nomeOriginale;
                #endregion

                #region Versione del documento
                documentoAmmInformatico.VersioneDelDocumento = (schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest).versionLabel;
                #endregion

                #region Tracciature modifiche documento (NON OBBLIGATORIO)
                #endregion

                #region Tempo di conservazione (NON OBBLIGATORIO)

                documentoAmmInformatico.TempoDiConservazione = await GetMaxTempoConservazioneNumberDocumento(docnumberAsLong, dbContext);

                #endregion

                #region Note (NON OBBLIGATORIO)
                #endregion

            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: ex.Message, "Errore in AddDocumentoAmministrativoInformatico");
            }
            return documentoAmmInformatico;
        }

        protected DocumentoAmministrativoInformaticoType AddSoggettoOperatore(DocumentoAmministrativoInformaticoType documentoAmmInformatico, Utente utente)
        {
            bool presente = false;
            if (documentoAmmInformatico.Soggetti != null)
                presente = (from s in documentoAmmInformatico.Soggetti
                            where s.Item is TipoSoggetto42Type && (s.Item as TipoSoggetto42Type).PF.Nome.Equals(utente.nome) &&
                            (s.Item as TipoSoggetto42Type).PF.Cognome.Equals(utente.cognome)
                            select s).FirstOrDefault() != null;
            if (!presente)
            {
                RuoloType ruoloAmmRegistrazione = new RuoloType()
                {
                    Item = new TipoSoggetto42Type()
                    {
                        PF = new PFType()
                        {
                            Nome = utente.nome,
                            Cognome = utente.cognome
                        }
                    }
                };
                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, ruoloAmmRegistrazione);
            }
            return documentoAmmInformatico;
        }

        protected async Task<DocumentoAmministrativoInformaticoType> UpdateChiaveDescrittiva(DocumentoAmministrativoInformaticoType documentoAmmInformatico,
            long docnumber,
            IPi3DbContext dbContext)
        {
            #region Chiave Descrittiva

            documentoAmmInformatico.ChiaveDescrittiva = new ChiaveDescrittivaType()
            {
                Oggetto = await dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == docnumber && p.ID_DOCUMENTO_PRINCIPALE == null
                          || p.SYSTEM_ID == dbContext.ProfileEntities.AsNoTracking().Where(a => a.SYSTEM_ID == docnumber)
                                            .Select(a => a.ID_DOCUMENTO_PRINCIPALE)
                                            .FirstOrDefault())
                    .Select(p => p.VAR_PROF_OGGETTO)
                    .FirstOrDefaultAsync()
            };

            #endregion
            return documentoAmmInformatico;
        }
        protected DocumentoAmministrativoInformaticoType UpdateMittDest(DocumentoAmministrativoInformaticoType documentoAmmInformatico, DocsPaVO.documento.SchedaDocumento schedaDoc, InfoUtente infoUtente)
        {
            RuoloType[] soggetti = documentoAmmInformatico.Soggetti;
            documentoAmmInformatico.Soggetti = null;
            if (soggetti != null)
            {
                foreach (RuoloType soggetto in soggetti)
                {
                    if (!(soggetto.Item is TipoSoggetto31Type || soggetto.Item is TipoSoggetto32Type))
                    {
                        documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, soggetto);
                    }
                }
            }
            #region Mittente/Destinatario

            if (schedaDoc.protocollo != null)
            {
                RuoloType mittente;
                RuoloType destinatario;
                switch (schedaDoc.tipoProto)
                {
                    case "P":
                        DocsPaVO.documento.ProtocolloUscita protocolloUscita = schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita;
                        //Destinatari
                        if (protocolloUscita.destinatari != null)
                        {
                            foreach (Corrispondente dest in protocolloUscita.destinatari)
                            {
                                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddDestinatario(dest));
                            }
                        }
                        //Destinatari in conoscenza
                        if (protocolloUscita.destinatariConoscenza != null)
                        {
                            foreach (Corrispondente dest in protocolloUscita.destinatariConoscenza)
                            {
                                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddDestinatario(dest));
                            }
                        }
                        //Mittente
                        if (protocolloUscita.mittente != null)
                            documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddMittente(protocolloUscita.mittente));
                        break;
                    case "A":
                        DocsPaVO.documento.ProtocolloEntrata protocolloEntrata = schedaDoc.protocollo as DocsPaVO.documento.ProtocolloEntrata;
                        if (protocolloEntrata.mittente != null)
                        {
                            documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddMittente(protocolloEntrata.mittente));
                        }
                        break;
                    case "I":
                        DocsPaVO.documento.ProtocolloInterno protocolloInterno = schedaDoc.protocollo as DocsPaVO.documento.ProtocolloInterno;
                        //Destinatari
                        if (protocolloInterno.destinatari != null)
                        {
                            foreach (Corrispondente dest in protocolloInterno.destinatari)
                            {
                                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddDestinatario(dest));
                            }
                        }
                        //Destinatari in conoscenza
                        if (protocolloInterno.destinatariConoscenza != null)
                        {
                            foreach (Corrispondente dest in protocolloInterno.destinatariConoscenza)
                            {
                                documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddDestinatario(dest));
                            }
                        }
                        //Mittente
                        if (protocolloInterno.mittente != null)
                        {
                            documentoAmmInformatico.Soggetti = AddSoggetto(documentoAmmInformatico, AddMittente(protocolloInterno.mittente));
                        }
                        break;
                }
            }

            #endregion
            return documentoAmmInformatico;
        }

        protected async Task<DocumentoAmministrativoInformaticoType> UpdateSegnaturaRepertorio(DocumentoAmministrativoInformaticoType documentoAmmInformatico,
            long docnumber,
            IPi3DbContext dbContext)
        {
            var docnumberAsString = docnumber.ToString();

            var repertorio = await dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                        .Join(dbContext.OggettiCustomEntities.AsNoTracking(),
                            a => a.ID_OGGETTO,
                            o => o.SYSTEM_ID,
                            (a, o) => new { a, o })
                        .Where(j => j.a.DOC_NUMBER == docnumberAsString && j.o.REPERTORIO == 1 && j.a.VALORE_OGGETTO_DB != null)
                        .Select(j => new
                        {
                            j.a.VAR_SEGNATURA,
                            j.a.DTA_INS,
                            j.a.VALORE_OGGETTO_DB
                        })
                        .FirstOrDefaultAsync();

            documentoAmmInformatico.DatiDiRegistrazione.TipoRegistro.Item = new NoProtocolloType()
            {
                DataRegistrazioneDocumento = repertorio.DTA_INS.Value,
                OraRegistrazioneDocumento = ConvertToTime(repertorio.DTA_INS.ToString()),
                OraRegistrazioneDocumentoSpecified = true,
                NumeroRegistrazioneDocumento = repertorio.VALORE_OGGETTO_DB,
                CodiceRegistro = "0"
            };

            #region Aggiorno tempo di conservazione

            documentoAmmInformatico.TempoDiConservazione = await GetMaxTempoConservazioneNumberDocumento(docnumber, dbContext);

            #endregion

            return documentoAmmInformatico;
        }

        protected DocumentoAmministrativoInformaticoType UpdateDatiProtocollo(DocumentoAmministrativoInformaticoType documentoAmmInformatico,
            DocsPaVO.documento.SchedaDocumento schedaDoc,
            InfoUtente infoUtente)
        {
            documentoAmmInformatico.IdDoc.Segnatura = schedaDoc.protocollo.segnatura;

            documentoAmmInformatico.DatiDiRegistrazione = new DatiDiRegistrazioneType()
            {
                TipologiaDiFlusso = schedaDoc.tipoProto == "A" ? TipologiaDiFlussoType.E : schedaDoc.tipoProto == "P" ? TipologiaDiFlussoType.U : TipologiaDiFlussoType.I,
                TipoRegistro = new TipoRegistroType()
                {
                    Item = new ProtocolloType()
                    {
                        DataProtocollazioneDocumento = schedaDoc.protocollo.dataProtocollazione.AsDateTime(),
                        OraProtocollazioneDocumento = ConvertToTime(schedaDoc.oraCreazione),
                        OraProtocollazioneDocumentoSpecified = true,
                        NumeroProtocolloDocumento = CalcolaNumProto(schedaDoc.protocollo.numero),
                        CodiceRegistro = schedaDoc.registro.codice,
                    }
                }
            };

            documentoAmmInformatico = UpdateMittDest(documentoAmmInformatico, schedaDoc, infoUtente);
            return documentoAmmInformatico;
        }

        protected async Task<DocumentoAmministrativoInformaticoType> UpdateVersione(DocumentoAmministrativoInformaticoType documentoAmmInformatico,
            DateTime dataAzione,
            string codiceAzione,
            long docnumber,
        bool isAllegato,
        IMediator mediator,
        IPi3DbContext dbContext,
            InfoUtente infoUtente,
            IClaimsPrincipalService claimsPrincipalService)
        {

            var docs = await DBUtils.GetVersionsMainDocument(
                    infoUtente,
                    claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser),
                    claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup),
                    docnumber,
                    dbContext);
            DocsPaVO.documento.Documento doc = docs[0];
            if (!isAllegato)
            {
                DocsPaVO.documento.FileDocumento fileDocumento = !string.IsNullOrEmpty(doc.fileSize) && Convert.ToInt32(doc.fileSize) > 0 ?
                    (await mediator.Send(new DocumentoGetInfoFileCommand()
                    {
                        fileRequest = doc,
                        infoUtente = infoUtente
                    })).output : null;

                #region IdDoc
                if (fileDocumento != null)
                {
                    documentoAmmInformatico.IdDoc.ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                    {
                        Impronta = ComputeHashAsSHA256(doc.impronta),
                        Algoritmo = "SHA256"
                    };
                }
                else
                {
                    documentoAmmInformatico.IdDoc.ImprontaCrittograficaDelDocumento = null;
                }
                #endregion

                #region Versione del documento
                documentoAmmInformatico.VersioneDelDocumento = (doc as DocsPaVO.documento.FileRequest).versionLabel;
                #endregion

                #region Nome del documento
                if (fileDocumento != null)
                    documentoAmmInformatico.NomeDelDocumento = fileDocumento.nomeOriginale;
                else
                    documentoAmmInformatico.NomeDelDocumento = string.Empty;
                #endregion

                #region Identificativo del formato
                documentoAmmInformatico.IdentificativoDelFormato = new IdentificativoDelFormatoType()
                {
                    Formato = fileDocumento != null ? fileDocumento.estensioneFile : string.Empty
                };
                #endregion

                bool isFirmato = doc.firmato == "1";
                documentoAmmInformatico.Verifica = new VerificaType()
                {
                    FirmatoDigitalmente = isFirmato
                };

                #region Tracciature modifiche documento

                if (codiceAzione.Equals("ADD_VERSION"))
                {
                    var utente = await GetUtente(infoUtente.idPeople.AsLong(), dbContext);

                    documentoAmmInformatico = AddSoggettoOperatore(documentoAmmInformatico, utente);

                    documentoAmmInformatico.TracciatureModificheDocumento = new TracciatureModificheDocumentoType()
                    {
                        TipoModifica = TipoModificaType.Rettifica,
                        SoggettoAutoreDellaModifica = new PFType()
                        {
                            Nome = utente.nome,
                            Cognome = utente.cognome
                        },
                        OraModificaSpecified = true,
                        DataModifica = dataAzione,
                        OraModifica = dataAzione
                    };
                    if (docs.Count() > 1)
                    {
                        DocsPaVO.documento.Documento docVersionePrecedente = docs[1];
                        documentoAmmInformatico.TracciatureModificheDocumento.IdDocVersionePrecedente = new IdDocType()
                        {
                            Identificativo = docVersionePrecedente.versionId,
                        };
                        if (!string.IsNullOrEmpty(docVersionePrecedente.fileSize) && Convert.ToInt32(docVersionePrecedente.fileSize) > 0 != null)
                        {
                            documentoAmmInformatico.TracciatureModificheDocumento.IdDocVersionePrecedente.ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                            {
                                Impronta = ComputeHashAsSHA256(docVersionePrecedente.impronta),
                                Algoritmo = "SHA256"
                            };
                        }
                    }
                }
                #endregion
            }
            else
            {
                IndiceAllegatiType allegato = null;
                if (documentoAmmInformatico.Allegati == null || documentoAmmInformatico.Allegati.IndiceAllegati == null || documentoAmmInformatico.Allegati.IndiceAllegati.Length == 0)
                {
                    documentoAmmInformatico.Allegati = new AllegatiType()
                    {
                        NumeroAllegati = "0"
                    };
                }
                else
                {
                    allegato = (from a in documentoAmmInformatico.Allegati.IndiceAllegati where a.IdDoc.Identificativo.Equals(docnumber) select a).FirstOrDefault();
                }
                if (allegato != null)
                {
                    if (!string.IsNullOrEmpty(doc.fileSize) && Convert.ToInt32(doc.fileSize) > 0)
                    {
                        allegato.IdDoc.ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                        {
                            Algoritmo = "SHA256",
                            Impronta = ComputeHashAsSHA256(doc.impronta)
                        };
                    }
                    else
                    {
                        allegato.IdDoc.ImprontaCrittograficaDelDocumento = null;
                    }
                }
                else
                {
                    documentoAmmInformatico.Allegati.NumeroAllegati = (Convert.ToInt32(documentoAmmInformatico.Allegati.NumeroAllegati) + 1).ToString();

                    allegato = new IndiceAllegatiType()
                    {
                        Descrizione = doc.descrizione,
                        IdDoc = new IdDocType()
                        {
                            Identificativo = doc.docNumber
                        }
                    };
                    if (!string.IsNullOrEmpty(doc.fileSize) && Convert.ToInt32(doc.fileSize) > 0)
                    {
                        allegato.IdDoc.ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                        {
                            Algoritmo = "SHA256",
                            Impronta = ComputeHashAsSHA256(doc.impronta)
                        };
                    }
                    documentoAmmInformatico.Allegati.IndiceAllegati = AddAllegato(documentoAmmInformatico, allegato);
                }
            }
            return documentoAmmInformatico;
        }

        protected async Task<DocumentoAmministrativoInformaticoType> UpdateAllegati(DocumentoAmministrativoInformaticoType documentoAmmInformatico,
            DateTime dataAzione,
            long docnumber,
            InfoUtente infoUtente,
        string azione,
        IPi3DbContext dbContext,
            IMediator mediator)
        {
            var allegati = (await mediator.Send(new DocumentoGetAllegatiCommand()
            {
                DocNumber = docnumber.ToString(),
                FilterAllegatiPec = string.Empty,
                SimplifiedInteroperabilityId = string.Empty
            })).output;
            #region Allegati

            if (allegati != null && allegati.Any())
            {
                documentoAmmInformatico.Allegati.NumeroAllegati = allegati.Length.ToString();
                if (azione.Equals("DOC_NEW_ALLEGATO"))
                {
                    foreach (DocsPaVO.documento.Allegato allegato in allegati)
                    {
                        bool presente = false;
                        if (documentoAmmInformatico.Allegati.IndiceAllegati != null)
                            presente = (from a in documentoAmmInformatico.Allegati.IndiceAllegati where a.IdDoc.Identificativo.Equals(allegato.docNumber) select a).FirstOrDefault() != null;
                        if (!presente)
                        {
                            IndiceAllegatiType indiceAllegatoType = new IndiceAllegatiType()
                            {
                                Descrizione = allegato.descrizione,
                                IdDoc = new IdDocType()
                                {
                                    Identificativo = allegato.docNumber,
                                    ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                                    {
                                        Algoritmo = "SHA256",
                                        Impronta = ComputeHashAsSHA256(allegato.impronta)
                                    }
                                }
                            };
                            documentoAmmInformatico.Allegati.IndiceAllegati = AddAllegato(documentoAmmInformatico, indiceAllegatoType);

                            #region Tracciature modifiche documento

                            var utente = await GetUtente(infoUtente.idPeople.AsLong(), dbContext);

                            documentoAmmInformatico = AddSoggettoOperatore(documentoAmmInformatico, utente);
                            documentoAmmInformatico.TracciatureModificheDocumento = new TracciatureModificheDocumentoType()
                            {
                                TipoModifica = TipoModificaType.Integrazione,
                                SoggettoAutoreDellaModifica = new PFType()
                                {
                                    Nome = utente.nome,
                                    Cognome = utente.cognome
                                },
                                OraModificaSpecified = true,
                                DataModifica = dataAzione,
                                OraModifica = dataAzione,
                                IdDocVersionePrecedente = documentoAmmInformatico.IdDoc
                            };
                            #endregion
                        }
                    }
                }
                else //Caso di rimozione allegato
                {
                    IndiceAllegatiType[] allegatiType = documentoAmmInformatico.Allegati.IndiceAllegati;
                    documentoAmmInformatico.Allegati.NumeroAllegati = allegati.Length.ToString();
                    documentoAmmInformatico.Allegati.IndiceAllegati = null;
                    foreach (IndiceAllegatiType indiceAllegatoType in allegatiType)
                    {
                        bool presente = (from DocsPaVO.documento.Allegato a in allegati where a.docNumber.Equals(indiceAllegatoType.IdDoc.Identificativo) select a).FirstOrDefault() != null;
                        if (presente)
                            documentoAmmInformatico.Allegati.IndiceAllegati = AddAllegato(documentoAmmInformatico, indiceAllegatoType);
                    }
                }
            }
            else
            {
                documentoAmmInformatico.Allegati = new AllegatiType()
                {
                    NumeroAllegati = "0",
                    IndiceAllegati = new IndiceAllegatiType[0]
                };
            }
            #endregion

            return documentoAmmInformatico;
        }

        protected async Task<DocumentoAmministrativoInformaticoType> AddTracciaturaAnnullamento(DocumentoAmministrativoInformaticoType documentoAmmInformatico,
            DateTime dataAzione,
            InfoUtente infoUtente,
            IPi3DbContext dbContext)
        {

            #region Tracciature modifiche documento

            var utente = await GetUtente(infoUtente.idPeople.AsLong(), dbContext);

            documentoAmmInformatico = AddSoggettoOperatore(documentoAmmInformatico, utente);
            documentoAmmInformatico.TracciatureModificheDocumento = new TracciatureModificheDocumentoType()
            {
                TipoModifica = TipoModificaType.Annullamento,
                SoggettoAutoreDellaModifica = new PFType()
                {
                    Nome = utente.nome,
                    Cognome = utente.cognome
                },
                OraModificaSpecified = true,
                DataModifica = dataAzione,
                OraModifica = dataAzione,
                IdDocVersionePrecedente = documentoAmmInformatico.IdDoc
            };
            #endregion

            return documentoAmmInformatico;
        }


        protected async Task<DocumentoAmministrativoInformaticoType> AddClassificaAgg(DocumentoAmministrativoInformaticoType documentoAmmInformatico,
            long docnumber,
            IPi3DbContext dbContext)
        {
            var fascicoli = await dbContext.ProjectComponentEntities.AsNoTracking()
                .Join(dbContext.ProjectEntities.AsNoTracking(),
                    pc => pc.PROJECT_ID,
                    p => p.SYSTEM_ID,
                    (pc, p) => new { pc, p })
                .Where(j => j.pc.LINK == docnumber)
                .Select(j => new
                {
                    j.p.SYSTEM_ID,
                    j.p.CHA_TIPO_PROJ,
                    j.p.VAR_CODICE,
                    j.p.DESCRIPTION
                })
                .ToListAsync();

            var classificazione = fascicoli?.Count() > 0 ? fascicoli.Where(f => f.CHA_TIPO_PROJ == "G").FirstOrDefault() : null;

            #region Classificazione
            if (classificazione != null)
            {
                documentoAmmInformatico.Classificazione = new ClassificazioneType()
                {
                    IndiceDiClassificazione = classificazione.VAR_CODICE,
                    Descrizione = classificazione.DESCRIPTION
                };
            }
            #endregion

            #region Agg
            documentoAmmInformatico.Agg = null;
            if (fascicoli != null && fascicoli.Any())
            {
                foreach (var fasc in fascicoli)
                {
                    IdAggType agg = new IdAggType()
                    {
                        IdAggregazione = fasc.SYSTEM_ID.ToString(),
                        TipoAggregazione = TipoAggregazioneType.Fascicolo
                    };

                    documentoAmmInformatico.Agg = AddAgg(documentoAmmInformatico, agg);
                }
            }
            #endregion

            #region Aggiorno tempo di conservazione
            documentoAmmInformatico.TempoDiConservazione = await GetMaxTempoConservazioneNumberDocumento(docnumber, dbContext);
            #endregion

            return documentoAmmInformatico;
        }

        protected async Task<DocumentoAmministrativoInformaticoType> ScambiaDocumento(DocumentoAmministrativoInformaticoType documentoAmmInformatico,
            string descrizioneAzione,
        long docnumber,
        InfoUtente infoUtente,
            IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext)
        {
            DocsPaVO.documento.Documento doc = (await DBUtils.GetVersionsMainDocument(
                    infoUtente,
                    claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser),
                    claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup),
                    docnumber,
                    dbContext)).FirstOrDefault();

            DocsPaVO.documento.FileDocumento fileDocumento = !string.IsNullOrEmpty(doc.fileSize) && Convert.ToInt32(doc.fileSize) > 0 ?
                     (await mediator.Send(new DocumentoGetInfoFileCommand()
                     {
                         fileRequest = doc,
                         infoUtente = infoUtente
                     })).output : null;

            #region IdDoc
            if (fileDocumento != null)
            {
                documentoAmmInformatico.IdDoc.ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                {
                    Impronta = ComputeHashAsSHA256(doc.impronta),
                    Algoritmo = "SHA256"
                };
            }
            else
            {
                documentoAmmInformatico.IdDoc.ImprontaCrittograficaDelDocumento = null;
            }
            #endregion

            #region Nome del documento
            if (fileDocumento != null)
                documentoAmmInformatico.NomeDelDocumento = fileDocumento.nomeOriginale;
            else
                documentoAmmInformatico.NomeDelDocumento = string.Empty;
            #endregion

            #region Identificativo del formato
            documentoAmmInformatico.IdentificativoDelFormato = new IdentificativoDelFormatoType()
            {
                Formato = fileDocumento != null ? fileDocumento.estensioneFile : string.Empty
            };
            #endregion

            var allegati = (await mediator.Send(new DocumentoGetAllegatiCommand()
            {
                DocNumber = docnumber.ToString(),
                FilterAllegatiPec = string.Empty,
                SimplifiedInteroperabilityId = string.Empty
            })).output;

            if (allegati != null && allegati.Any())
            {
                string codAllegato = descrizioneAzione.Split(new[] { "con l'allegato" }, StringSplitOptions.None)[1].Split('(')[0].Trim();
                DocsPaVO.documento.Allegato allegato = (from DocsPaVO.documento.Allegato a in allegati where a.versionLabel.Equals(codAllegato) select a).FirstOrDefault();
                if (allegato != null)
                {
                    IndiceAllegatiType allegatoType = (from a in documentoAmmInformatico.Allegati.IndiceAllegati where a.IdDoc.Identificativo.Equals(allegato.docNumber) select a).FirstOrDefault();
                    if (!string.IsNullOrEmpty(allegato.fileSize) && Convert.ToInt32(allegato.fileSize) > 0)
                    {
                        allegatoType.IdDoc.ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDelDocumentoType()
                        {
                            Algoritmo = "SHA256",
                            Impronta = ComputeHashAsSHA256(allegato.impronta)
                        };
                    }
                    else
                    {
                        allegatoType.IdDoc.ImprontaCrittograficaDelDocumento = null;
                    }
                }
            }

            return documentoAmmInformatico;
        }

        protected async Task<Utente> GetUtente(long idPeople, IPi3DbContext dbContext)
        {
            var peopleEntity = await dbContext.PeopleEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == idPeople)
                .Select(p => new
                {
                    p.SYSTEM_ID,
                    p.VAR_COGNOME,
                    p.VAR_NOME,
                    p.FULL_NAME,
                    p.CHA_SYSTEM_USER
                })
                .FirstAsync();

            var utente = new Utente()
            {
                idPeople = peopleEntity.SYSTEM_ID.ToString(),
                nome = peopleEntity.VAR_NOME,
                cognome = peopleEntity.VAR_COGNOME,
                descrizione = peopleEntity.FULL_NAME
            };

            return utente;
        }

        protected async Task<string> GetMaxTempoConservazioneNumberDocumento(long docnumber, IPi3DbContext dbContext)
        {
            long? maxTempoCon = await (from f1 in dbContext.ProjectEntities.AsNoTracking()
                                       from f2 in dbContext.ProjectEntities.AsNoTracking()
                                       from c in dbContext.ProjectComponentEntities.AsNoTracking()
                                       from t in dbContext.PianoConservazioneEntities.AsNoTracking()
                                       from a in dbContext.PianoConsAssTempoConsEntities.AsNoTracking()
                                       where c.LINK == docnumber &&
                                       c.PROJECT_ID == f1.SYSTEM_ID &&
                                       f1.ID_FASCICOLO == f2.SYSTEM_ID &&
                                       f2.CHA_TIPO_FASCICOLO != null &&
                                       f2.CHA_TIPO_FASCICOLO.Equals("P") &&
                                       t.SYSTEM_ID == f2.ID_PIANO_CONSERVAZIONE &&
                                       a.TEMPO_CONSERVAZIONE.Equals(t.TEMPO_CONSERVAZIONE)
                                       select new
                                       {
                                           a.TEMPO_CONSERVAZIONE_IN_ANNI,
                                           a.TEMPO_CONSERVAZIONE
                                       }).Union(from p in dbContext.ProfileEntities.AsNoTracking()
                                                from f1 in dbContext.ProjectEntities.AsNoTracking()
                                                from f2 in dbContext.ProjectEntities.AsNoTracking()
                                                from c in dbContext.ProjectComponentEntities.AsNoTracking()
                                                from t in dbContext.PianoConservazioneEntities.AsNoTracking()
                                                from a in dbContext.PianoConsAssTempoConsEntities.AsNoTracking()
                                                from pt in dbContext.PianoConsTipoAttoEntities.AsNoTracking()
                                                where p.SYSTEM_ID == docnumber &&
                                                p.SYSTEM_ID == c.LINK &&
                                                c.PROJECT_ID == f1.SYSTEM_ID &&
                                                f1.ID_FASCICOLO == f2.SYSTEM_ID &&
                                                f2.CHA_TIPO_FASCICOLO != null &&
                                                f2.CHA_TIPO_FASCICOLO.Equals("G") &&
                                                t.ID_CLASSIFICAZIONE == f2.ID_PARENT &&
                                                a.TEMPO_CONSERVAZIONE.Equals(t.TEMPO_CONSERVAZIONE) &&
                                                pt.ID_TIPO_ATTO == p.ID_TIPO_ATTO &&
                                                t.SYSTEM_ID == pt.ID_PIANO_CONSERVAZIONE
                                                select new
                                                {
                                                    a.TEMPO_CONSERVAZIONE_IN_ANNI,
                                                    a.TEMPO_CONSERVAZIONE
                                                }).MaxAsync(e => e.TEMPO_CONSERVAZIONE_IN_ANNI);

            return maxTempoCon != null ? maxTempoCon.ToString() : string.Empty;

        }



        private static RuoloType AddDestinatario(Corrispondente dest)
        {
            RuoloType destinatario = null;
            if (dest != null && !string.IsNullOrEmpty(dest.systemId))
            {
                if (dest.tipoCorrispondente == "P")
                {
                    destinatario = new RuoloType()
                    {
                        Item = new TipoSoggetto31Type()
                        {
                            Item = new PFType()
                            {
                                Nome = (dest as Utente).nome,
                                Cognome = (dest as Utente).cognome
                            }
                        }
                    };
                }
                else
                {
                    destinatario = new RuoloType()
                    {
                        Item = new TipoSoggetto31Type()
                        {
                            Item = new PGType
                            {
                                DenominazioneOrganizzazione = dest.descrizione
                            }
                        }
                    };
                }
            }
            return destinatario;
        }

        private static RuoloType AddMittente(Corrispondente mitt)
        {
            RuoloType mittente = null;

            if (mitt != null && !string.IsNullOrEmpty(mitt.systemId))
            {
                if (mitt.tipoCorrispondente == "P")
                {
                    mittente = new RuoloType()
                    {
                        Item = new TipoSoggetto32Type()
                        {
                            Item = new PFType()
                            {
                                Nome = mitt.nome,
                                Cognome = mitt.cognome
                            }
                        }
                    };
                }
                else
                {
                    mittente = new RuoloType()
                    {
                        Item = new TipoSoggetto32Type()
                        {
                            Item = new PGType
                            {
                                DenominazioneOrganizzazione = mitt.descrizione
                            }
                        }
                    };
                }
            }

            return mittente;
        }

        private static RuoloType[] AddSoggetto(DocumentoAmministrativoInformaticoType documentoAmmInformatico, RuoloType soggetto)
        {
            var soggetti = new List<RuoloType>();

            if (documentoAmmInformatico.Soggetti != null)
                soggetti.AddRange(documentoAmmInformatico.Soggetti);

            soggetti.Add(soggetto);

            return soggetti.ToArray();
        }

        private static IndiceAllegatiType[] AddAllegato(DocumentoAmministrativoInformaticoType documentoAmmInformatico, IndiceAllegatiType indiceAllegatoType)
        {
            var allegati = new List<IndiceAllegatiType>();

            if (documentoAmmInformatico.Allegati.IndiceAllegati != null)
                allegati.AddRange(documentoAmmInformatico.Allegati.IndiceAllegati);

            allegati.Add(indiceAllegatoType);

            return allegati.ToArray();
        }

        private static IdAggType[] AddAgg(DocumentoAmministrativoInformaticoType documentoAmmInformatico, IdAggType agg)
        {
            var tipoAgg = new List<IdAggType>();

            if (documentoAmmInformatico.Agg != null)
            {
                bool presente = (from f in documentoAmmInformatico.Agg where f.IdAggregazione.Equals(agg.IdAggregazione) select f).FirstOrDefault() != null;
                if (!presente)
                    tipoAgg.AddRange(documentoAmmInformatico.Agg);
            }

            tipoAgg.Add(agg);

            return tipoAgg.ToArray();
        }


        #endregion

        #region AGGREGAZIONI DOCUMENTALI INFORMATICHE

        protected async Task<DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType> AddAggregazioniDocumentaliInformatiche(long idOggetto,
            InfoUtente infoUtente,
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator)
        {
            bool result = true;
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var amministraEntity = await dbContext.AmministraEntities.AsNoTracking()
                  .Where(a => a.SYSTEM_ID == idTenant)
                  .FirstAsync();

            DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType aggregazioniDocumentaliInformatiche = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType();

            try
            {
                var projectEntity = await dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == idOggetto)
                    .FirstOrDefaultAsync();

                string idAggregazione = projectEntity.SYSTEM_ID.ToString();
                if (projectEntity.ID_TIPO_FASC != null)
                {
                    var repertorio = await dbContext.AssTemplatesFascEntities.AsNoTracking()
                        .Join(dbContext.OggettiCustomFascEntities.AsNoTracking(),
                            a => a.ID_OGGETTO,
                            o => o.SYSTEM_ID,
                            (a, o) => new { a, o })
                        .Where(j => j.a.ID_PROJECT == projectEntity.SYSTEM_ID.ToString() && j.o.REPERTORIO == 1 && j.a.VALORE_OGGETTO_DB != null)
                        .Select(j => j.a.VALORE_OGGETTO_DB)
                        .FirstOrDefaultAsync();

                    if (repertorio != null)
                        idAggregazione = repertorio;
                }
                aggregazioniDocumentaliInformatiche.IdAgg = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.IdAggType()
                {
                    TipoAggregazione = DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoAggregazioneType.Fascicolo,
                    IdAggregazione = idAggregazione
                };

                aggregazioniDocumentaliInformatiche.TipologiaFascicolo = DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipologiaFascicoloType.attivita;

                #region Soggetti
                //In pitre non sono attualmente disponibili queste informazioni che dovrebbero essere inserite dall’operatore e non possono essere desunte automaticamente dal PiTRE.
                //Per questo motivo proponiamo di inserire un unico valore: Amministrazione titolare = Amministrazione creatrice del fascicolo
                Registro registro = (await mediator.Send(new GetListaRegistriByRuoloCommand()
                {
                    IdRuolo = infoUtente.idCorrGlobali
                })).output[0];
                DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.RuoloType amministrazioneTitolare = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.RuoloType()
                {
                    Item = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoSoggetto1Type()
                    {
                        PAI = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.PAIType()
                        {
                            IPAAmm = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.CodiceIPAType()
                            {
                                Denominazione = amministraEntity.VAR_DESC_AMM,
                                CodiceIPA = amministraEntity.VAR_CODICE_AMM_IPA
                            },
                            IPAAOO = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.CodiceIPAType()
                            {
                                Denominazione = registro.descrizione,
                                CodiceIPA = registro.codiceIpa
                            },
                            IndirizziDigitaliDiRiferimento = new string[1] { amministraEntity.VAR_INDIRIZZO_DIGITALE_RIF }
                        }
                    }
                };
                aggregazioniDocumentaliInformatiche.Soggetti = AddSoggetto(aggregazioniDocumentaliInformatiche, amministrazioneTitolare);

                #endregion

                #region Assegnazione

                aggregazioniDocumentaliInformatiche.Assegnazione = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoAssegnazioneType[0];

                #endregion

                #region Data apertura

                aggregazioniDocumentaliInformatiche.DataApertura = projectEntity.DTA_APERTURA.Value;

                #endregion

                #region Classificazione

                var classificazioneEntity = await dbContext.ProjectEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == projectEntity.ID_PARENT)
                    .Select(c => new
                    {
                        c.VAR_CODICE,
                        c.DESCRIPTION
                    })
                    .FirstOrDefaultAsync();

                aggregazioniDocumentaliInformatiche.Classificazione = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.ClassificazioneType()
                {

                    IndiceDiClassificazione = classificazioneEntity.VAR_CODICE,
                    Descrizione = classificazioneEntity.DESCRIPTION
                };
                #endregion

                #region Progressivo
                aggregazioniDocumentaliInformatiche.Progressivo = projectEntity.NUM_FASCICOLO.ToString();
                #endregion

                #region Chiave Descrittiva
                aggregazioniDocumentaliInformatiche.ChiaveDescrittiva = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.ChiaveDescrittivaType()
                {
                    Oggetto = projectEntity.DESCRIPTION
                };
                #endregion

                #region Data chiusura
                aggregazioniDocumentaliInformatiche.DataChiusura = projectEntity.DTA_CHIUSURA.GetValueOrDefault();
                #endregion

                #region Procedimento amministrativo (Obbligatorio nel caso di Tipologia fascicolo = procedimento amministrativo.)
                #endregion

                #region Indice Documenti

                var idFolder = await dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.ID_FASCICOLO == projectEntity.SYSTEM_ID && p.CHA_TIPO_PROJ == "C")
                    .Select(c => c.SYSTEM_ID)
                    .ToListAsync();

                var infoDocs = await dbContext.ProjectComponentEntities.AsNoTracking()
                    .Join(dbContext.ProfileEntities.AsNoTracking(),
                            pc => pc.LINK,
                            p => p.SYSTEM_ID,
                            (pc, p) => new { pc, p })
                    .Where(j => idFolder.Contains(j.pc.PROJECT_ID.Value) && (j.p.CHA_IN_CESTINO ?? "0") == "0")
                    .Select(j => new
                    {
                        j.pc.LINK,
                        j.p.NUM_PROTO,
                        j.p.DOCNUMBER,
                        j.p.VAR_SEGNATURA,
                        j.p.ID_TIPO_ATTO
                    })
                    .ToListAsync();

                if (infoDocs != null && infoDocs.Any())
                {
                    DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoDocumentoType tipoDoc = null;
                    foreach (var infoDoc in infoDocs)
                    {
                        var segnatura = infoDoc.VAR_SEGNATURA;
                        if (string.IsNullOrEmpty(segnatura) && infoDoc.ID_TIPO_ATTO != null)
                        {
                            segnatura = await dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                .Join(dbContext.OggettiCustomEntities.AsNoTracking(),
                                    a => a.ID_OGGETTO,
                                    o => o.SYSTEM_ID,
                                    (a, o) => new { a, o })
                                .Where(j => j.a.DOC_NUMBER == infoDoc.DOCNUMBER.ToString() && j.o.REPERTORIO == 1 && j.a.VALORE_OGGETTO_DB != null)
                                .Select(j => j.a.VAR_SEGNATURA)
                                .FirstOrDefaultAsync();
                        }
                        var impronta = await dbContext.ComponentEntities.AsNoTracking()
                            .Where(c => c.DOCNUMBER == infoDoc.DOCNUMBER && c.VERSION_ID == dbContext.VersionEntities.AsNoTracking()
                                .Where(v => v.DOCNUMBER == infoDoc.DOCNUMBER)
                                .OrderByDescending(v => v.VERSION_ID)
                                .Select(v => v.VERSION_ID)
                                .First())
                            .Select(c => c.VAR_IMPRONTA)
                            .FirstOrDefaultAsync();

                        tipoDoc = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoDocumentoType()
                        {
                            Item = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.IdDoc_DAIType()
                            {
                                Identificativo = infoDoc.DOCNUMBER.ToString(),
                                Segnatura = segnatura,
                                ImprontaCrittograficaDelDocumento = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.ImprontaCrittograficaDelDocumentoType()
                                {
                                    Impronta = ComputeHashAsSHA256(impronta),
                                    Algoritmo = "SHA256"
                                }
                            }
                        };

                        aggregazioniDocumentaliInformatiche.IndiceDocumenti = AddIndiceDocumento(aggregazioniDocumentaliInformatiche, tipoDoc);
                    }
                }

                #endregion

                #region Posizione fisica Aggregazione documentale

                if (projectEntity.ID_UO_LF != null)
                {
                    aggregazioniDocumentaliInformatiche.PosizioneFisicaAggregazioneDocumentale = await dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == projectEntity.ID_UO_LF)
                        .Select(c => c.VAR_DESC_CORR)
                        .FirstOrDefaultAsync();
                }

                #endregion

                #region IdAggPrimario (NON OBBLIGATORIO)

                #endregion

                #region TempoDiConservazione
                //Ad oggi la PAT non ci ha comunicato i tempi di conservazione dei fascicoli, in quanto attività facente parte di una mev non ancora sviluppata,
                //abbiamo ipotizzato di mettere sempre 9999 se e solo obbligatorio e di modificare il valore, in seguito all’entrata in vigore della futura mev.
                //if (!string.IsNullOrEmpty(fascicolo.chiusura))
                //    aggregazioniDocumentaliInformatiche.TempoDiConservazione = 9999.ToString();

                if (projectEntity.ID_PIANO_CONSERVAZIONE != null)
                {
                    //estraggo il valore numerico corrispondente al tempo di conservazione
                    var tempoConservazione = await dbContext.PianoConservazioneEntities.AsNoTracking()
                        .Join(dbContext.PianoConsAssTempoConsEntities.AsNoTracking(),
                            p => p.TEMPO_CONSERVAZIONE,
                            t => t.TEMPO_CONSERVAZIONE,
                            (p, t) => new { p, t })
                        .Where(j => j.p.SYSTEM_ID == projectEntity.ID_PIANO_CONSERVAZIONE)
                        .Select(j => j.t.TEMPO_CONSERVAZIONE_IN_ANNI)
                        .FirstOrDefaultAsync();

                    aggregazioniDocumentaliInformatiche.TempoDiConservazione = tempoConservazione.HasValue ? tempoConservazione.ToString() : string.Empty;
                }
                #endregion

                #region Note (NON OBBLIGATORIO)
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: ex.Message);
                aggregazioniDocumentaliInformatiche = null;
            }
            return aggregazioniDocumentaliInformatiche;
        }


        protected async Task<DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType> UpdateAggregazioniDocumentaliInformatiche(DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType aggregazioniDocumentaliInformatiche,
          long idOggetto,
          InfoUtente infoUtente,
          IPi3DbContext dbContext)
        {
            bool result = true;
            try
            {
                var projectEntity = await dbContext.ProjectEntities.AsNoTracking()
                   .Where(p => p.SYSTEM_ID == idOggetto)
                   .FirstOrDefaultAsync();

                if (projectEntity.ID_TIPO_FASC != null)
                {
                    var repertorio = await dbContext.AssTemplatesFascEntities.AsNoTracking()
                        .Join(dbContext.OggettiCustomFascEntities.AsNoTracking(),
                            a => a.ID_OGGETTO,
                            o => o.SYSTEM_ID,
                            (a, o) => new { a, o })
                        .Where(j => j.a.ID_PROJECT == projectEntity.SYSTEM_ID.ToString() && j.o.REPERTORIO == 1 && j.a.VALORE_OGGETTO_DB != null)
                        .Select(j => j.a.VALORE_OGGETTO_DB)
                        .FirstOrDefaultAsync();

                    if (repertorio != null)
                        aggregazioniDocumentaliInformatiche.IdAgg.IdAggregazione = repertorio;
                }

                #region Chiave Descrittiva
                aggregazioniDocumentaliInformatiche.ChiaveDescrittiva.Oggetto = projectEntity.DESCRIPTION;
                #endregion

                #region Data chiusura
                aggregazioniDocumentaliInformatiche.DataChiusura = projectEntity.DTA_CHIUSURA.GetValueOrDefault();
                #endregion


                #region Posizione fisica Aggregazione documentale

                aggregazioniDocumentaliInformatiche.PosizioneFisicaAggregazioneDocumentale = projectEntity.ID_UO_LF == null ? string.Empty : await dbContext.CorrGlobaliEntities.AsNoTracking()
                       .Where(c => c.SYSTEM_ID == projectEntity.ID_UO_LF)
                       .Select(c => c.VAR_DESC_CORR)
                       .FirstOrDefaultAsync();

                #endregion

                #region TempoDiConservazione
                //Ad oggi la PAT non ci ha comunicato i tempi di conservazione dei fascicoli, in quanto attività facente parte di una mev non ancora sviluppata,
                //abbiamo ipotizzato di mettere sempre 9999 se e solo obbligatorio e di modificare il valore, in seguito all’entrata in vigore della futura mev.
                if (projectEntity.DTA_CHIUSURA.HasValue)
                    aggregazioniDocumentaliInformatiche.TempoDiConservazione = 9999.ToString();
                else
                    aggregazioniDocumentaliInformatiche.TempoDiConservazione = string.Empty;
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: ex.Message);
                aggregazioniDocumentaliInformatiche = null;
            }
            return aggregazioniDocumentaliInformatiche;
        }



        protected async Task<DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType> AddAssegnazione(DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType aggregazioniDocumentaliInformatiche,
            long idOggetto,
            DateTime dataAzione,
            long idTrasmissione,
            InfoUtente infoUtente,
            IPi3DbContext dbContext,
            IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService)
        {
            bool result = true;
            Registro registro = (await mediator.Send(new GetListaRegistriByRuoloCommand()
            {
                IdRuolo = infoUtente.idCorrGlobali
            })).output[0];

            try
            {
                var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                var trasmissioneEntity = await dbContext.TrasmissioneEntities.AsNoTracking()
                    .Join(dbContext.TrasmSingolaEntities.AsNoTracking(),
                        t => t.SYSTEM_ID,
                        s => s.ID_TRASMISSIONE,
                        (t, s) => new { t, s })
                    .Where(j => j.s.SYSTEM_ID == idTrasmissione)
                    .Select(j => new
                    {
                        j.t.SYSTEM_ID,
                        j.t.DTA_INVIO
                    })
                    .FirstOrDefaultAsync();

                if (trasmissioneEntity != null)
                {
                    var amministraEntity = await dbContext.AmministraEntities.AsNoTracking()
                                        .Where(a => a.SYSTEM_ID == idTenant)
                                        .FirstAsync();
                    //Aggiungo l'assegnazione
                    var utente = await GetUtente(infoUtente.idPeople.AsLong(), dbContext);

                    DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.ASType soggettoAssegnatario = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.ASType()
                    {
                        Cognome = utente.nome,
                        Nome = utente.cognome,
                        IPAAmm = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.CodiceIPAType()
                        {
                            Denominazione = amministraEntity.VAR_DESC_AMM,
                            CodiceIPA = amministraEntity.VAR_CODICE_AMM_IPA
                        },
                        IPAAOO = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.CodiceIPAType()
                        {
                            Denominazione = registro.descrizione,
                            CodiceIPA = registro.codiceIpa
                        },
                        IPAUOR = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.CodiceIPAType()
                        {
                            Denominazione = amministraEntity.VAR_DESC_AMM,
                            CodiceIPA = amministraEntity.VAR_CODICE_AMM_IPA
                        },
                        IndirizziDigitaliDiRiferimento = new string[1] { amministraEntity.VAR_INDIRIZZO_DIGITALE_RIF }
                    };
                    DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoAssegnazioneType assegnazione = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoAssegnazioneType()
                    {
                        Item = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AssType()
                        {
                            DataInizioAssegnazione = trasmissioneEntity.DTA_INVIO.Value,
                            OraInizioAssegnazione = trasmissioneEntity.DTA_INVIO.Value,
                            OraInizioAssegnazioneSpecified = true,
                            DataFineAssegnazione = dataAzione,
                            OraFineAssegnazione = dataAzione,
                            OraFineAssegnazioneSpecified = true,
                            SoggettoAssegnatario = soggettoAssegnatario
                        }
                    };

                    var assegnazioni = new List<DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoAssegnazioneType>();
                    if (aggregazioniDocumentaliInformatiche.Assegnazione != null)
                        assegnazioni.AddRange(aggregazioniDocumentaliInformatiche.Assegnazione);
                    assegnazioni.Add(assegnazione);
                    aggregazioniDocumentaliInformatiche.Assegnazione = assegnazioni.ToArray();
                }
                else
                    aggregazioniDocumentaliInformatiche = null;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: ex.Message);
                aggregazioniDocumentaliInformatiche = null;
            }
            return aggregazioniDocumentaliInformatiche;
        }


        protected async Task<DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType> AddDocumento(DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType aggregazioniDocumentaliInformatiche,
            string descrizioneAzione,
            IPi3DbContext dbContext)
        {
            string idDocumento = descrizioneAzione.Split(new[] { "Inserimento doc " }, StringSplitOptions.None)[1].Split('i')[0].Trim();
            if (!string.IsNullOrEmpty(idDocumento))
            {
                bool presente = false;
                if (aggregazioniDocumentaliInformatiche.IndiceDocumenti != null && aggregazioniDocumentaliInformatiche.IndiceDocumenti.Count() > 0)
                {
                    presente = (from DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoDocumentoType i in aggregazioniDocumentaliInformatiche.IndiceDocumenti
                                where (i.Item as DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.IdDoc_DAIType).Identificativo.Equals(idDocumento)
                                select i).FirstOrDefault() != null;
                }
                if (!presente)
                {
                    var profileEntity = await dbContext.ProfileEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == idDocumento.AsLong())
                        .Select(p => new
                        {
                            p.NUM_PROTO,
                            p.DOCNUMBER,
                            p.VAR_SEGNATURA,
                            p.ID_TIPO_ATTO
                        })
                    .FirstOrDefaultAsync();

                    if (profileEntity != null)
                    {
                        var segnatura = profileEntity.VAR_SEGNATURA;
                        if (string.IsNullOrEmpty(segnatura) && profileEntity.ID_TIPO_ATTO != null)
                        {
                            segnatura = await dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                .Join(dbContext.OggettiCustomEntities.AsNoTracking(),
                                    a => a.ID_OGGETTO,
                                    o => o.SYSTEM_ID,
                                    (a, o) => new { a, o })
                                .Where(j => j.a.DOC_NUMBER == profileEntity.DOCNUMBER.ToString() && j.o.REPERTORIO == 1 && j.a.VALORE_OGGETTO_DB != null)
                                .Select(j => j.a.VAR_SEGNATURA)
                                .FirstOrDefaultAsync();
                        }
                        var impronta = await dbContext.ComponentEntities.AsNoTracking()
                            .Where(c => c.DOCNUMBER == profileEntity.DOCNUMBER && c.VERSION_ID == dbContext.VersionEntities.AsNoTracking()
                                .Where(v => v.DOCNUMBER == profileEntity.DOCNUMBER)
                                .OrderByDescending(v => v.VERSION_ID)
                                .Select(v => v.VERSION_ID)
                                .First())
                            .Select(c => c.VAR_IMPRONTA)
                            .FirstOrDefaultAsync();

                        DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoDocumentoType tipoDoc = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoDocumentoType()
                        {
                            Item = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.IdDoc_DAIType()
                            {
                                Identificativo = profileEntity.DOCNUMBER.ToString(),
                                Segnatura = segnatura,
                                ImprontaCrittograficaDelDocumento = new DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.ImprontaCrittograficaDelDocumentoType()
                                {
                                    Impronta = ComputeHashAsSHA256(impronta),
                                    Algoritmo = "SHA256"
                                }
                            }
                        };

                        aggregazioniDocumentaliInformatiche.IndiceDocumenti = AddIndiceDocumento(aggregazioniDocumentaliInformatiche, tipoDoc);
                    }
                }
            }
            return aggregazioniDocumentaliInformatiche;
        }

        protected DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType RimuoviDocumento(DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType aggregazioniDocumentaliInformatiche,
            string descrizioneAzione)
        {
            string idDocumento = descrizioneAzione.Split(new[] { "Rimozione doc N.ro " }, StringSplitOptions.None)[1].Split('d')[0].Trim();
            if (!string.IsNullOrEmpty(idDocumento))
            {
                aggregazioniDocumentaliInformatiche.IndiceDocumenti = (from DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoDocumentoType i in aggregazioniDocumentaliInformatiche.IndiceDocumenti
                                                                       where !(i.Item as DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.IdDoc_DAIType).Identificativo.Equals(idDocumento)
                                                                       select i).ToArray();
            }
            return aggregazioniDocumentaliInformatiche;
        }


        protected DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoDocumentoType[] AddIndiceDocumento(DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType aggregazioneDocumentaliInformatiche, DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoDocumentoType tipoDoc)
        {
            var tipoDocumentoTypes = new List<DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.TipoDocumentoType>();

            if (aggregazioneDocumentaliInformatiche.IndiceDocumenti != null)
                tipoDocumentoTypes.AddRange(aggregazioneDocumentaliInformatiche.IndiceDocumenti);

            tipoDocumentoTypes.Add(tipoDoc);

            return tipoDocumentoTypes.ToArray();
        }

        protected DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.RuoloType[] AddSoggetto(DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.AggregazioneDocumentaliInformaticheType aggregazioneDocumentaliInformatiche, DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.RuoloType soggetto)
        {
            var soggetti = new List<DocsPaVO.DocumentMetadata.AggregazioniDocumentaliInformatiche.RuoloType>();

            if (aggregazioneDocumentaliInformatiche.Soggetti != null)
                soggetti.AddRange(aggregazioneDocumentaliInformatiche.Soggetti);

            soggetti.Add(soggetto);

            return soggetti.ToArray();
        }

        #endregion

        #region Utils
        private byte[] ComputeHashAsSHA256(string hash)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                int NumberChars = hash.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(hash.Substring(i, 2), 16);
                return bytes;
            }
            else
                return null;
        }

        protected string CalcolaNumProto(string numProto)
        {
            int MAX_LENGTH = 7;
            string zeroes = "";
            string numero = numProto;
            for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
            {
                zeroes = zeroes + "0";
            }
            numero = zeroes + numProto;

            return numero;
        }

        protected DateTime ConvertToTime(string value)
        {
            try
            {
                value = value.Trim();
                int length = value.Length;
                if (length > 11)
                {
                    value = value.Substring(11, length - 11).Replace(":", ".");
                    return DateTime.Parse(value, new System.Globalization.CultureInfo("it-IT", true));
                }
                else
                {
                    return DateTime.Parse("00:00:00", new System.Globalization.CultureInfo("it-IT", true));
                }
            }
            catch (Exception ex)
            {
                return DateTime.Parse("00:00:00", new System.Globalization.CultureInfo("it-IT", true));
            }
        }

        protected string ToXmlString<T>(T value)
        {
            XmlSerializerNamespaces namespaces = null;

            var settings = new XmlWriterSettings();
            settings.CheckCharacters = false;

            using (var stream = new StringWriterWithEncoding(Encoding.UTF8))
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value, namespaces);
                return stream.ToString();
            }

        }

        protected T FromXmlString<T>(T value, string xml)
        {
            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(reader);
            }
        }
        #endregion



        #endregion
    }

}
