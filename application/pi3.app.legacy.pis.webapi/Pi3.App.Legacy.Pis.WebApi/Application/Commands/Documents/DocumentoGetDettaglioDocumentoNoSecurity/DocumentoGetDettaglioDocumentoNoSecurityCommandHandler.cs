// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using DocsPaVO.utente;
using Pi3.Core.Extensions;
using DocsPaVO.documento;
using DocsPaVO.Note;
using DocsPaVO.ProfilazioneDinamica;
using Microsoft.EntityFrameworkCore;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity
{
    public class DocumentoGetDettaglioDocumentoNoSecurityCommandHandler : IRequestHandler<DocumentoGetDettaglioDocumentoNoSecurityCommand, DocumentoGetDettaglioDocumentoNoSecurityCommandResponse>
    {
        public DocumentoGetDettaglioDocumentoNoSecurityCommandHandler(
            ILogger<DocumentoGetDettaglioDocumentoNoSecurityCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDistributedCache distributedCache
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._mapper = this.InitializeMapper();
            this._webMethodLoggerService = webMethodLoggerService;
            this._distributedCache = distributedCache;
        }


        public async Task<DocumentoGetDettaglioDocumentoNoSecurityCommandResponse> Handle(DocumentoGetDettaglioDocumentoNoSecurityCommand request, CancellationToken cancellationToken)
        {
            var idAmministrazioneAsNumber = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idUserAsNumber = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroupAsNumber = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idProfileAsNumber = request.IdProfile.AsLong();
            var idCorrGlobali = string.IsNullOrEmpty(request.Infoutente.idCorrGlobali) ? "0".AsLong() : request.Infoutente.idCorrGlobali.AsLong();
            ArrayList allegati = new System.Collections.ArrayList();

            var schedaDocumento = new SchedaDocumento();

            try
            {
                // Caricamento entità del documento
                var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking().FirstOrDefaultAsync(p => p.SYSTEM_ID == idProfileAsNumber);

                if (profileEntity == null)
                    throw new DocumentoNotFoundPi3Exception(idProfileAsNumber);

                AuthorEntity authorEntity = await this._dbContext.PeopleEntities.AsNoTracking().Where(p => p.SYSTEM_ID == profileEntity.AUTHOR).Select(p => new AuthorEntity() { SYSTEM_ID = p.SYSTEM_ID, USER_ID = p.USER_ID, ID_AMM = p.ID_AMM }).FirstAsync();
                AuthorGroupEntity authorGroupEntity = null;
                if (profileEntity.ID_RUOLO_CREATORE.HasValue)
                    authorGroupEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(cg => cg.SYSTEM_ID == profileEntity.ID_RUOLO_CREATORE).Select(cg => new AuthorGroupEntity() { ID_GRUPPO = cg.ID_GRUPPO }).FirstAsync();

                RegistroEntity registroEntity = null;
                if (profileEntity.ID_REGISTRO.HasValue)
                    registroEntity = await this._dbContext.RegistroEntities.AsNoTracking().FirstOrDefaultAsync(r => r.SYSTEM_ID == profileEntity.ID_REGISTRO.Value);

                AuthorUOEntity uoCreatoreEntity = null;
                if (profileEntity.ID_UO_CREATORE.HasValue)
                    uoCreatoreEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(uo => uo.SYSTEM_ID == profileEntity.ID_UO_CREATORE).Select(uo => new AuthorUOEntity() { SYSTEM_ID = uo.SYSTEM_ID, VAR_CODICE = uo.VAR_CODICE }).FirstOrDefaultAsync();

                AuthorEntity authorProtEntity = null;
                if (profileEntity.ID_PEOPLE_PROT.HasValue)
                    await this._dbContext.PeopleEntities.AsNoTracking().Where(p => p.SYSTEM_ID == profileEntity.ID_PEOPLE_PROT).Select(p => new AuthorEntity() { SYSTEM_ID = p.SYSTEM_ID, USER_ID = p.USER_ID, ID_AMM = p.ID_AMM }).FirstAsync();

                AuthorGroupEntity authorProtGroupEntity = null;
                if (profileEntity.ID_RUOLO_PROT.HasValue)
                    authorProtGroupEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(cg => cg.SYSTEM_ID == profileEntity.ID_RUOLO_PROT).Select(cg => new AuthorGroupEntity() { ID_GRUPPO = cg.ID_GRUPPO }).FirstAsync();

                AuthorUOEntity uoProtocollatoreEntity = null;
                if (profileEntity.ID_UO_PROT.HasValue)
                    uoProtocollatoreEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(uo => uo.SYSTEM_ID == profileEntity.ID_UO_PROT).Select(uo => new AuthorUOEntity() { SYSTEM_ID = uo.SYSTEM_ID, VAR_CODICE = uo.VAR_CODICE }).FirstOrDefaultAsync();

                DocumentTypesEntity documentTypesEntity = await this._dbContext.DocumentTypesEntities.AsNoTracking().Where(documentType => documentType.SYSTEM_ID == profileEntity.DOCUMENTTYPE).Select(documentType => documentType).FirstOrDefaultAsync();

                var versionEntities = await (from versions in this._dbContext.VersionEntities
                                             join people in this._dbContext.PeopleEntities
                                                 on versions.AUTHOR equals people.SYSTEM_ID
                                             join peopleDelegato in this._dbContext.PeopleEntities
                                                 on versions.ID_PEOPLE_DELEGATO equals peopleDelegato.SYSTEM_ID into delegato
                                             from peopleDelegato in delegato.DefaultIfEmpty()
                                             join firmaVers in this._dbContext.FirmaVersEntities
                                                on versions.VERSION_ID equals firmaVers.ID_VERSIONE into firmaVersions
                                             from firmaVers in firmaVersions.DefaultIfEmpty()
                                             join firmatari in this._dbContext.FirmatarioEntities
                                                  on firmaVers.ID_FIRMATARIO equals firmatari.SYSTEM_ID into firmatario
                                             from firmatari in firmatario.DefaultIfEmpty()
                                             where versions.DOCNUMBER == profileEntity.SYSTEM_ID
                                             select new { Version = versions, AuthorUserId = people.USER_ID, DelegatoUserId = peopleDelegato.USER_ID, Firmatario = firmatari })
                                           .AsNoTracking()
                                           .ToListAsync();

                var componentEntities = await (from components in this._dbContext.ComponentEntities
                                               join people in this._dbContext.PeopleEntities
                                                   on components.ID_PEOPLE_PUTFILE equals people.SYSTEM_ID into autore
                                               from people in autore.DefaultIfEmpty()
                                               join peopleDelegato in this._dbContext.PeopleEntities
                                                  on components.ID_PEOPLE_DELEGATO_PUTFILE equals peopleDelegato.SYSTEM_ID into delegato
                                               from peopleDelegato in delegato.DefaultIfEmpty()
                                               where components.DOCNUMBER == profileEntity.SYSTEM_ID
                                               select new { Component = components, AuthorUserId = people.USER_ID, DelegatoUserId = peopleDelegato.USER_ID })
                                            .AsNoTracking()
                                            .ToListAsync();

                schedaDocumento.accessRights = "255";

                (await this._dbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.ID_DOCUMENTO_PRINCIPALE == profileEntity.SYSTEM_ID)
                    .Select(p => new { SYSTEM_ID = p.SYSTEM_ID, IN_LIBROFIRMA = p.IN_LIBROFIRMA, p.FORWARDING_SOURCE, p.VAR_PROF_OGGETTO })
                    .ToListAsync())
                    .ForEach(async a =>
                    {
                        var lastVersionAllegatoEntity = await (from versions in this._dbContext.VersionEntities
                                                               join people in this._dbContext.PeopleEntities
                                                                   on versions.AUTHOR equals people.SYSTEM_ID
                                                               join peopleDelegato in this._dbContext.PeopleEntities
                                                                   on versions.ID_PEOPLE_DELEGATO equals peopleDelegato.SYSTEM_ID into delegato
                                                               from peopleDelegato in delegato.DefaultIfEmpty()
                                                               where versions.DOCNUMBER == a.SYSTEM_ID
                                                               select new { Version = versions, AuthorUserId = people.USER_ID, DelegatoUserId = peopleDelegato.USER_ID })
                                                        .AsNoTracking()
                                                        .OrderByDescending(v => v.Version.VERSION_ID)
                                                        .FirstAsync();

                        var lastComponentAllegatoEntity = await (from components in this._dbContext.ComponentEntities
                                                                 join people in this._dbContext.PeopleEntities
                                                                     on components.ID_PEOPLE_PUTFILE equals people.SYSTEM_ID into utente
                                                                 from peopleUtente in utente.DefaultIfEmpty()
                                                                 join peopleDelegato in this._dbContext.PeopleEntities
                                                                     on components.ID_PEOPLE_DELEGATO_PUTFILE equals peopleDelegato.SYSTEM_ID into delegato
                                                                 from peopleDelegato in delegato.DefaultIfEmpty()
                                                                 where components.VERSION_ID == lastVersionAllegatoEntity.Version.VERSION_ID
                                                                 select new { Component = components, AuthorUserId = peopleUtente.USER_ID, DelegatoUserId = peopleDelegato.USER_ID })
                                                        .AsNoTracking()
                                                        .FirstAsync();

                        var allegato = new DocsPaVO.documento.Allegato();
                        allegato.autore = lastVersionAllegatoEntity.AuthorUserId;
                        allegato.autoreFile = lastComponentAllegatoEntity.AuthorUserId;
                        allegato.cartaceo = lastVersionAllegatoEntity.Version.CARTACEO.GetValueOrDefault() > 0;
                        allegato.conSegnaturaPermanente = lastVersionAllegatoEntity.Version.CHA_SEGNATURA == "1";
                        allegato.dataAcquisizione = lastComponentAllegatoEntity.Component.DTA_FILE_ACQUIRED.AsDateTimeFormat();
                        allegato.dataInserimento = lastVersionAllegatoEntity.Version.DTA_CREAZIONE.AsDateTimeFormat();
                        allegato.descrizione = lastVersionAllegatoEntity.Version.VERSION == 0 ? lastVersionAllegatoEntity.Version.COMMENTS : a.VAR_PROF_OGGETTO;
                        allegato.docNumber = lastVersionAllegatoEntity.Version.DOCNUMBER.ToString();
                        allegato.fileName = lastComponentAllegatoEntity.Component.VAR_NOMEORIGINALE ?? string.Empty;
                        allegato.fileSize = (lastComponentAllegatoEntity.Component.FILE_SIZE ?? 0).ToString();
                        allegato.firmato = (lastComponentAllegatoEntity.Component.CHA_FIRMATO != null ? lastComponentAllegatoEntity.Component.CHA_FIRMATO : string.Empty);
                        allegato.ForwardingSource = a.FORWARDING_SOURCE.HasValue ? a.FORWARDING_SOURCE.ToString() : null;
                        allegato.idPeople = lastVersionAllegatoEntity.Version.AUTHOR.ToString();
                        allegato.idPeopleDelegato = lastVersionAllegatoEntity.Version.ID_PEOPLE_DELEGATO.ToString();
                        allegato.impronta = lastComponentAllegatoEntity.Component.VAR_IMPRONTA;
                        allegato.inLibroFirma = a.IN_LIBROFIRMA == "1";
                        allegato.numeroPagine = (int)lastVersionAllegatoEntity.Version.NUM_PAG_ALLEGATI.GetValueOrDefault();
                        allegato.path = lastComponentAllegatoEntity.Component.PATH;
                        allegato.position = allegati.Count + 1;
                        allegato.subVersion = lastVersionAllegatoEntity.Version.SUBVERSION;
                        allegato.tipoFirma = lastComponentAllegatoEntity.Component.CHA_TIPO_FIRMA;
                        allegato.version = lastVersionAllegatoEntity.Version.VERSION.GetValueOrDefault().ToString();
                        allegato.versionId = lastVersionAllegatoEntity.Version.VERSION_ID.GetValueOrDefault().ToString();
                        allegato.versionLabel = string.Format("A{0:0#}", allegati.Count + 1);

                        switch (lastVersionAllegatoEntity.Version.CHA_ALLEGATI_ESTERNO)
                        {
                            case "P":
                                allegato.TypeAttachment = 2;
                                break;
                            case "I":
                                allegato.TypeAttachment = 3;
                                break;
                            case "1":
                                allegato.TypeAttachment = 4;
                                break;
                            case "D":
                                allegato.TypeAttachment = 5;
                                break;
                            case "S":
                                allegato.TypeAttachment = 6; //Allegato per il file segnatura.xml
                                break;
                            default:
                                allegato.TypeAttachment = 1;
                                break;
                        }

                        allegati.Add(allegato);

                    });

                schedaDocumento.allegati = allegati.Cast<Allegato>().ToArray();
                schedaDocumento.appId = (profileEntity.APPLICATION.HasValue ? profileEntity.APPLICATION.ToString() : null);
                schedaDocumento.idPeople = profileEntity.AUTHOR.GetValueOrDefault().ToString();
                schedaDocumento.autore = authorEntity.SYSTEM_ID.ToString();
                schedaDocumento.assegnato = profileEntity.CHA_ASSEGNATO;

                var checkInOutEntity = await this._dbContext.CheckinCheckoutEntities
                    .Join(this._dbContext.PeopleEntities, c => c.ID_USER, p => p.SYSTEM_ID, (c, p) => new { c, p })
                    .Join(this._dbContext.CorrGlobaliEntities, j1 => j1.c.ID_ROLE, cg => cg.SYSTEM_ID, (j1, cg) => new { c = j1.c, p = j1.p, cg })
                    .Join(this._dbContext.ProfileEntities, j2 => j2.c.ID_DOCUMENT, pr => pr.SYSTEM_ID, (j2, pr) => new { c = j2.c, p = j2.p, cg = j2.cg, pr })
                    .Where(c => c.c.ID_DOCUMENT == profileEntity.SYSTEM_ID && c.c.ID_USER == c.p.SYSTEM_ID)
                    .Select(x => new
                    {
                        SYSTEM_ID = x.c.SYSTEM_ID,
                        ID_DOCUMENT = x.c.ID_DOCUMENT,
                        DOCUMENT_NUMBER = x.c.DOCUMENT_NUMBER,
                        VAR_SEGNATURA = x.pr.VAR_SEGNATURA,
                        ID_USER = x.c.ID_USER,
                        USER_NAME = x.p.USER_ID,
                        ID_ROLE = x.c.ID_ROLE,
                        ID_DOCUMENTO_PRINCIPALE = x.pr.ID_DOCUMENTO_PRINCIPALE,
                        ROLE_NAME = x.cg.VAR_DESC_CORR,
                        CHECK_OUT_DATE = x.c.CHECK_OUT_DATE,
                        DOCUMENT_LOCATION = x.c.DOCUMENT_LOCATION,
                        MACHINE_NAME = x.c.MACHINE_NAME
                    })
                    .FirstOrDefaultAsync();
                if (checkInOutEntity != null)
                {
                    schedaDocumento.checkOutStatus = new DocsPaVO.CheckInOut.CheckOutStatus(
                        checkInOutEntity.SYSTEM_ID.ToString(),
                        checkInOutEntity.CHECK_OUT_DATE,
                        checkInOutEntity.DOCUMENT_LOCATION,
                        checkInOutEntity.MACHINE_NAME)
                    {
                        IDRole = checkInOutEntity.ID_ROLE.ToString(),
                        IDUser = checkInOutEntity.ID_USER.ToString(),
                        IDDocument = checkInOutEntity.ID_DOCUMENT.ToString(),
                        DocumentNumber = checkInOutEntity.DOCUMENT_NUMBER.ToString(),
                        Segnature = checkInOutEntity.VAR_SEGNATURA,
                        UserName = checkInOutEntity.USER_NAME,
                        RoleName = checkInOutEntity.ROLE_NAME,
                        IsAllegato = checkInOutEntity.ID_DOCUMENTO_PRINCIPALE != null,
                        DocumentLocation = checkInOutEntity.DOCUMENT_LOCATION ?? string.Empty
                    };
                }

                schedaDocumento.codiceApplicazione = profileEntity.COD_EXT_APP;
                schedaDocumento.commissioneRef = profileEntity.VAR_COMM_REF;

                if (profileEntity.CONSOLIDATION_DATE.HasValue)
                {
                    schedaDocumento.ConsolidationState = new DocumentConsolidationStateInfo()
                    {
                        Author = profileEntity.CONSOLIDATION_AUTHOR.ToString(),
                        Role = profileEntity.CONSOLIDATION_ROLE.ToString(),
                        Date = profileEntity.CONSOLIDATION_DATE.AsDateTimeFormat(),
                        State = Enum.Parse<DocumentConsolidationStateEnum>(profileEntity.CONSOLIDATION_STATE, true)
                    };
                }

                schedaDocumento.creatoreDocumento = new CreatoreDocumento(
                    idPeople: profileEntity.AUTHOR.GetValueOrDefault().ToString(),
                    idRuolo: authorGroupEntity != null ? authorGroupEntity.ID_GRUPPO.ToString() : null,
                    idUo: uoCreatoreEntity != null ? uoCreatoreEntity.SYSTEM_ID.ToString() : null,
                    codiceUo: uoCreatoreEntity != null ? uoCreatoreEntity.VAR_CODICE : null)
                {
                    idCorrGlob_Ruolo = profileEntity.ID_RUOLO_CREATORE.GetValueOrDefault().ToString(),
                    idCorrGlob_UO = uoCreatoreEntity != null ? uoCreatoreEntity.SYSTEM_ID.ToString() : null,
                    uo_codiceCorrGlobali = uoCreatoreEntity != null ? uoCreatoreEntity.VAR_CODICE.ToString() : null,
                    idPeopleDelegato = profileEntity.ID_PEOPLE_DELEGATO.GetValueOrDefault().ToString()
                };
                schedaDocumento.dataCreazione = profileEntity.CREATION_DATE.AsDateTimeFormat();
                schedaDocumento.dataScadenza = profileEntity.DTA_SCADENZA.AsDateFormat();

                if (profileEntity.DTA_PROTO_EME.HasValue)
                {
                    schedaDocumento.datiEmergenza = new DatiEmergenza()
                    {
                        dataProtocollazioneEmergenza = profileEntity.DTA_PROTO_EME.AsDateTimeFormat(),
                        cognomeProtocollatoreEmergenza = profileEntity.VAR_COGNOME_EME,
                        nomeProtocollatoreEmergenza = profileEntity.VAR_NOME_EME,
                        protocolloEmergenza = profileEntity.VAR_PROTO_EME
                    };
                }

                var mezzoSpedizione = await (from ms in this._dbContext.CollMSpedizDocumentoEntities
                                             join dt in this._dbContext.DocumentTypesEntities on ms.ID_DOCUMENTTYPES equals dt.SYSTEM_ID
                                             where ms.ID_PROFILE == profileEntity.SYSTEM_ID
                                             select new
                                             {
                                                 ID = ms.ID_DOCUMENTTYPES.ToString(),
                                                 DESCRIPTION = dt.DESCRIPTION
                                             })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                if (mezzoSpedizione != null)
                {
                    schedaDocumento.mezzoSpedizione = mezzoSpedizione.ID;
                    schedaDocumento.descMezzoSpedizione = mezzoSpedizione.DESCRIPTION;
                }

                schedaDocumento.docNumber = profileEntity.DOCNUMBER.ToString();

                List<Documento> documentiList = new List<Documento>();
                foreach (var versionEntity in versionEntities.OrderByDescending(v => v.Version.VERSION_ID).ToList())
                {
                    var componentEntity = componentEntities
                        .Where(c => c.Component.VERSION_ID == versionEntity.Version.VERSION_ID)
                        .Select(c => c.Component)
                        .FirstOrDefault();

                    if (schedaDocumento.documenti == null)
                        schedaDocumento.documenti = documentiList.ToArray();

                    var documento = new DocsPaVO.documento.Documento();

                    documento.autore = versionEntity.AuthorUserId;
                    documento.autoreFile = (componentEntity != null ? componentEntity.ID_PEOPLE_PUTFILE.ToString() : null);
                    documento.cartaceo = versionEntity.Version.CARTACEO.GetValueOrDefault() > 0;
                    documento.conSegnaturaPermanente = versionEntity.Version.CHA_SEGNATURA == "1";
                    documento.dataAcquisizione = (componentEntity != null ? componentEntity.DTA_FILE_ACQUIRED.AsDateTimeFormat() : null);
                    documento.dataInserimento = versionEntity.Version.DTA_CREAZIONE.AsDateTimeFormat();
                    documento.descrizione = versionEntity.Version.COMMENTS;
                    documento.docNumber = versionEntity.Version.DOCNUMBER.ToString();
                    documento.fileName = ((componentEntity != null && componentEntity.VAR_NOMEORIGINALE != null) ? componentEntity.VAR_NOMEORIGINALE : string.Empty);
                    documento.fileSize = (componentEntity != null ? (componentEntity.FILE_SIZE ?? 0).ToString() : null);
                    documento.firmato = (componentEntity != null ? componentEntity.CHA_FIRMATO : string.Empty);
                    if (versionEntity.Firmatario != null)
                    {
                        documento.firmatari[0] =
                        new DocsPaVO.documento.Firmatario()
                        {
                            systemId = versionEntity.Firmatario.SYSTEM_ID.ToString(),
                            cognome = versionEntity.Firmatario.VAR_COGNOME,
                            nome = versionEntity.Firmatario.VAR_NOME,
                            codiceFiscale = versionEntity.Firmatario.VAR_COD_FISCALE
                        };
                    }
                    documento.idPeople = versionEntity.Version.AUTHOR.ToString();
                    documento.idPeopleDelegato = versionEntity.Version.ID_PEOPLE_DELEGATO.ToString();
                    documento.impronta = (componentEntity != null ? componentEntity.VAR_IMPRONTA : null);
                    documento.inLibroFirma = profileEntity.IN_LIBROFIRMA == "1";
                    documento.path = (componentEntity != null ? componentEntity.PATH : null);
                    documento.repositoryContext = null;
                    documento.subVersion = versionEntity.Version.SUBVERSION;
                    documento.tipoFirma = (componentEntity != null ? componentEntity.CHA_TIPO_FIRMA : null);
                    documento.version = versionEntity.Version.VERSION.GetValueOrDefault().ToString();
                    documento.versionId = versionEntity.Version.VERSION_ID.GetValueOrDefault().ToString();
                    documento.versionLabel = versionEntity.Version.VERSION_LABEL;

                    documentiList.Add(documento);
                }
                schedaDocumento.documenti = documentiList.ToArray();

                if (profileEntity.ID_DOCUMENTO_PRINCIPALE.HasValue)
                {
                    var tempDocPrinc = await
                            (from docPrincipale in this._dbContext.ProfileEntities
                             join people in this._dbContext.PeopleEntities on docPrincipale.AUTHOR equals people.SYSTEM_ID
                             join tAtto in this._dbContext.TipoAttoEntities on docPrincipale.ID_TIPO_ATTO equals tAtto.SYSTEM_ID into tipoAtto
                             from tAtto in tipoAtto.DefaultIfEmpty()
                             where docPrincipale.SYSTEM_ID == profileEntity.ID_DOCUMENTO_PRINCIPALE
                             select new
                             {
                                 idProfile = docPrincipale.SYSTEM_ID.ToString(),
                                 docNumber = docPrincipale.DOCNUMBER,
                                 acquisitaImmagine = docPrincipale.CHA_IMG,
                                 oggetto = docPrincipale.VAR_PROF_OGGETTO,
                                 autore = people.USER_ID,
                                 daProtocollare = docPrincipale.CHA_DA_PROTO,
                                 dataApertura = (docPrincipale.DTA_PROTO_IN.HasValue ? docPrincipale.DTA_PROTO_IN.AsDateTimeFormat() : null),
                                 dataAnnullamento = (docPrincipale.DTA_ANNULLA.HasValue ? docPrincipale.DTA_ANNULLA.AsDateTimeFormat() : null),
                                 inCestino = docPrincipale.CHA_IN_CESTINO,
                                 inArchivio = docPrincipale.CHA_IN_ARCHIVIO,
                                 evidenza = docPrincipale.CHA_EVIDENZA,
                                 cha_firmato = docPrincipale.CHA_FIRMATO,
                                 idTipoAtto = (docPrincipale.ID_TIPO_ATTO.HasValue ? docPrincipale.ID_TIPO_ATTO.ToString() : null),
                                 tipoAtto = (tAtto != null ? tAtto.VAR_DESC_ATTO : null),
                                 numProt = (docPrincipale.NUM_PROTO.HasValue ? docPrincipale.NUM_PROTO.ToString() : null),
                                 privato = docPrincipale.CHA_PRIVATO,
                                 idRegistro = (docPrincipale.ID_REGISTRO.HasValue ? docPrincipale.ID_REGISTRO.ToString() : null),
                                 personale = docPrincipale.CHA_PERSONALE,
                                 segnatura = docPrincipale.VAR_SEGNATURA,
                                 tipoProto = docPrincipale.CHA_TIPO_PROTO,
                                 codiceApplicazione = docPrincipale.COD_EXT_APP,
                                 allegato = false,
                             })
                            .AsNoTracking()
                            .FirstAsync();

                    schedaDocumento.documentoPrincipale = new InfoDocumento()
                    {
                        idProfile = tempDocPrinc.idProfile,
                        docNumber = tempDocPrinc.docNumber != null ? tempDocPrinc.docNumber.ToString() : string.Empty,
                        acquisitaImmagine = tempDocPrinc.acquisitaImmagine ?? string.Empty,
                        oggetto = tempDocPrinc.oggetto ?? string.Empty,
                        autore = tempDocPrinc.autore ?? string.Empty,
                        daProtocollare = tempDocPrinc.daProtocollare ?? string.Empty,
                        dataApertura = tempDocPrinc.dataApertura ?? string.Empty,
                        dataAnnullamento = tempDocPrinc.dataAnnullamento ?? string.Empty,
                        inCestino = tempDocPrinc.inCestino ?? string.Empty,
                        inArchivio = tempDocPrinc.inArchivio ?? string.Empty,
                        evidenza = tempDocPrinc.evidenza ?? string.Empty,
                        cha_firmato = tempDocPrinc.cha_firmato ?? string.Empty,
                        idTipoAtto = tempDocPrinc.idTipoAtto ?? string.Empty,
                        tipoAtto = tempDocPrinc.tipoAtto ?? string.Empty,
                        numProt = tempDocPrinc.numProt ?? string.Empty,
                        privato = tempDocPrinc.privato ?? string.Empty,
                        idRegistro = tempDocPrinc.idRegistro ?? string.Empty,
                        personale = tempDocPrinc.personale ?? string.Empty,
                        segnatura = tempDocPrinc.segnatura ?? string.Empty,
                        tipoProto = tempDocPrinc.tipoProto ?? string.Empty,
                        codiceApplicazione = tempDocPrinc.codiceApplicazione ?? string.Empty,
                        allegato = false,
                    };

                    schedaDocumento.documentoPrincipale.privato = schedaDocumento.documentoPrincipale.privato == null ? "0" : schedaDocumento.documentoPrincipale.privato;
                    schedaDocumento.documentoPrincipale.personale = schedaDocumento.documentoPrincipale.personale == null ? "0" : schedaDocumento.documentoPrincipale.personale;
                }
                schedaDocumento.documento_da_pec = profileEntity.CHA_DOCUMENTO_DA_PEC;
                schedaDocumento.evidenza = profileEntity.CHA_EVIDENZA;
                schedaDocumento.fascicolato = profileEntity.CHA_FASCICOLATO;
                schedaDocumento.idFascProtoTit = profileEntity.ID_FASC_PROT_TIT.ToString();
                schedaDocumento.idPeople = (profileEntity.AUTHOR.HasValue ? profileEntity.AUTHOR.ToString() : null);
                schedaDocumento.idTitolario = profileEntity.ID_TITOLARIO.ToString();
                schedaDocumento.inArchivio = profileEntity.CHA_IN_ARCHIVIO;
                schedaDocumento.inCestino = profileEntity.CHA_IN_CESTINO == null ? "0" : profileEntity.CHA_IN_CESTINO;
                if (!string.IsNullOrWhiteSpace(profileEntity.CHA_COD_T_A))
                    schedaDocumento.InfoAtipicita = this._mapper.Map<DocsPaVO.Security.InfoAtipicita>(profileEntity);
                schedaDocumento.interop = profileEntity.CHA_INTEROP;
                schedaDocumento.isRiprodotto = false;
                schedaDocumento.LastForward = profileEntity.LAST_FORWARD.ToString();
                schedaDocumento.numInFasc = (profileEntity.NUM_IN_FASC.HasValue ? profileEntity.NUM_IN_FASC.ToString() : null);
                schedaDocumento.numProtTit = (profileEntity.NUM_PROT_TIT.HasValue ? profileEntity.NUM_PROT_TIT.ToString() : null);
                schedaDocumento.oggetto = new Oggetto((profileEntity.ID_OGGETTO.HasValue ? profileEntity.ID_OGGETTO.ToString() : null), profileEntity.VAR_PROF_OGGETTO);
                schedaDocumento.oraCreazione = profileEntity.DTA_PROTO.HasValue ? profileEntity.DTA_PROTO.AsHoursMinutesSecondsFormat() : profileEntity.CREATION_TIME.AsHoursMinutesSecondsFormat();
                schedaDocumento.personale = profileEntity.CHA_PERSONALE == null ? "0" : profileEntity.CHA_PERSONALE;
                //schedaDocumento.predisponiProtocollazione = profileEntity.CHA_DA_PROTO == "1";
                schedaDocumento.pregresso = false;
                schedaDocumento.previousVersionsHidden = false;// TODO determinato da ORACLE function "hasVersionsFullVisibility()"
                schedaDocumento.privato = profileEntity.CHA_PRIVATO == null ? "0" : profileEntity.CHA_PRIVATO;
                schedaDocumento.protocollatore = new Protocollatore(
                    idPeople: profileEntity.ID_PEOPLE_PROT.GetValueOrDefault().ToString(),
                    idRuolo: authorProtGroupEntity != null ? authorProtGroupEntity.ID_GRUPPO.ToString() : null,
                    idUo: uoProtocollatoreEntity != null ? uoProtocollatoreEntity.SYSTEM_ID.ToString() : null,
                    codiceUo: uoProtocollatoreEntity != null ? uoProtocollatoreEntity.VAR_CODICE : null)
                {
                    ruolo_idCorrGlobali = profileEntity.ID_RUOLO_PROT.GetValueOrDefault().ToString(),
                    uo_idCorrGlobali = uoProtocollatoreEntity != null ? uoProtocollatoreEntity.SYSTEM_ID.ToString() : null,
                    utente_idPeople = profileEntity.ID_PEOPLE_PROT.GetValueOrDefault().ToString(),
                    uo_codiceCorrGlobali = uoProtocollatoreEntity != null ? uoProtocollatoreEntity.VAR_CODICE.ToString() : null,
                };

                var soggettiProtocollo = await (from dap in this._dbContext.DocArrivoParEntities
                                                join cg in this._dbContext.CorrGlobaliEntities on dap.ID_MITT_DEST equals cg.SYSTEM_ID
                                                join dg in this._dbContext.DettGlobaliEntities
                                                    on cg.SYSTEM_ID equals dg.ID_CORR_GLOBALI into dettGlobali
                                                from dg in dettGlobali.DefaultIfEmpty()
                                                join cc in this._dbContext.CanaleCorrEntities
                                                    on cg.SYSTEM_ID equals cc.ID_CORR_GLOBALE into CanaleCorr
                                                from cc in CanaleCorr.DefaultIfEmpty()
                                                where dap.ID_PROFILE == profileEntity.SYSTEM_ID
                                                select new SoggettoProtocolloEntity()
                                                {
                                                    DocArrivoPar = dap,
                                                    CorrGlobali = cg,
                                                    DettCorrGlobali = dg,
                                                    CanaleCorr = cc
                                                })
                        .ToListAsync();

                var mittenti = soggettiProtocollo
                    .Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "M")
                    .Select(s => this._mapper.Map<Corrispondente>(s))
                    .ToList();

                mittenti.AddRange(soggettiProtocollo
                    .Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "MD")
                    .Select(s => this._mapper.Map<Corrispondente>(s)));

                var destinatari = new List<Corrispondente>();
                foreach (var s in soggettiProtocollo.Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "D" || s.DocArrivoPar.CHA_TIPO_MITT_DEST == "F"))
                {
                    Corrispondente destinatario = null;
                    switch (s.CorrGlobali.CHA_TIPO_URP)
                    {
                        case "U":
                            destinatario = this._mapper.Map<UnitaOrganizzativa>(s);
                            break;
                        case "P":
                            destinatario = this._mapper.Map<Utente>(s);
                            break;
                        case "R":
                            destinatario = this._mapper.Map<Ruolo>(s);
                            break;
                        case "F":
                            destinatario = this._mapper.Map<RaggruppamentoFunzionale>(s);
                            break;
                        default:
                            destinatario = this._mapper.Map<Corrispondente>(s);
                            break;
                    }

                    if (s.CanaleCorr != null)
                    {
                        destinatario.canalePref = await (from dt in this._dbContext.DocumentTypesEntities
                                                         where dt.SYSTEM_ID == s.CanaleCorr.ID_DOCUMENTTYPE
                                                         select new Canale
                                                         {
                                                             systemId = dt.SYSTEM_ID.ToString(),
                                                             descrizione = dt.DESCRIPTION,
                                                             typeId = dt.TYPE_ID,
                                                             tipoCanale = dt.TYPE_ID //IL FE USA tipoCanale con valore del TYPE_ID
                                                         })
                                                .FirstOrDefaultAsync();
                    }

                    destinatari.Add(destinatario);
                }

                //var destinatari = soggettiProtocollo
                //    .Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "D")
                //    .Select(s => this._mapper.Map<Corrispondente>(s))
                //    .ToList();

                var destinatariCC = new List<Corrispondente>();
                foreach (var s in soggettiProtocollo.Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "C"))
                {
                    Corrispondente destinatario = null;
                    switch (s.CorrGlobali.CHA_TIPO_URP)
                    {
                        case "U":
                            destinatario = this._mapper.Map<UnitaOrganizzativa>(s);
                            break;
                        case "P":
                            destinatario = this._mapper.Map<Utente>(s);
                            break;
                        case "R":
                            destinatario = this._mapper.Map<Ruolo>(s);
                            break;
                        case "F":
                            destinatario = this._mapper.Map<RaggruppamentoFunzionale>(s);
                            break;
                        default:
                            destinatario = this._mapper.Map<Corrispondente>(s);
                            break;
                    }

                    if (s.CanaleCorr != null)
                    {
                        destinatario.canalePref = await (from dt in this._dbContext.DocumentTypesEntities
                                                         where dt.SYSTEM_ID == s.CanaleCorr.ID_DOCUMENTTYPE
                                                         select new Canale
                                                         {
                                                             systemId = dt.SYSTEM_ID.ToString(),
                                                             descrizione = dt.DESCRIPTION,
                                                             typeId = dt.TYPE_ID,
                                                             tipoCanale = dt.TYPE_ID //IL FE USA tipoCanale con valore del TYPE_ID
                                                         })
                                                .FirstOrDefaultAsync();
                    }

                    destinatariCC.Add(destinatario);
                }

                //var destinatariCC = soggettiProtocollo
                //    .Where(s => s.DocArrivoPar.CHA_TIPO_MITT_DEST == "C")
                //    .Select(s => this._mapper.Map<Corrispondente>(s))
                //    .ToList();

                Protocollo protocollo = null;

                switch (profileEntity.CHA_TIPO_PROTO)
                {
                    case "A":
                        protocollo = new ProtocolloEntrata()
                        {
                            dataProtocolloMittente = (profileEntity.DTA_PROTO_IN.HasValue ? profileEntity.DTA_PROTO_IN.AsDateFormat() : null),
                            mittente = mittenti.FirstOrDefault(),
                            mittenti = mittenti.Skip(1).ToArray(),
                            descrizioneProtocolloMittente = profileEntity.VAR_PROTO_IN
                        };
                        break;
                    case "P":
                        protocollo = new ProtocolloUscita()
                        {
                            mittente = mittenti.FirstOrDefault(),
                            destinatari = destinatari.ToArray(),
                            destinatariConoscenza = destinatariCC.ToArray()
                        };
                        break;
                    case "I":
                        protocollo = new ProtocolloInterno()
                        {
                            mittente = mittenti.FirstOrDefault(),
                            destinatari = destinatari.ToArray(),
                            destinatariConoscenza = destinatariCC.ToArray()
                        };
                        break;
                    default:
                        break;
                }

                if (protocollo != null)
                {
                    protocollo.numero = profileEntity.NUM_PROTO.ToString();
                    protocollo.dataProtocollazione = profileEntity.DTA_PROTO?.AsDateTimeFormat();
                    protocollo.segnatura = profileEntity.VAR_SEGNATURA?.ToString();
                    protocollo.anno = profileEntity.NUM_ANNO_PROTO.ToString();
                    protocollo.daProtocollare = profileEntity.CHA_DA_PROTO;
                }

                if (profileEntity.DTA_ANNULLA.HasValue)
                {
                    protocollo.protocolloAnnullato = new ProtocolloAnnullato()
                    {
                        dataAnnullamento = profileEntity.DTA_ANNULLA.AsDateTimeFormat(),
                        autorizzazione = profileEntity.VAR_AUT_ANNULLA
                    };
                }

                schedaDocumento.protocollo = protocollo;

                if (registroEntity != null)
                {
                    schedaDocumento.registro = this._mapper.Map<DocsPaVO.utente.Registro>(registroEntity);
                    schedaDocumento.id_rf_prot = schedaDocumento.registro.systemId;
                    schedaDocumento.cod_rf_prot = schedaDocumento.registro.codRegistro;

                    if (registroEntity.ID_AMM.HasValue)
                        schedaDocumento.registro.codAmministrazione = await this._dbContext.AmministraEntities
                            .AsNoTracking()
                            .Where(a => a.SYSTEM_ID == registroEntity.ID_AMM)
                            .Select(a => a.VAR_CODICE_AMM)
                            .FirstOrDefaultAsync();
                }

                schedaDocumento.riferimentoMittente = profileEntity.CHA_RIFF_MITT;

                if (profileEntity.ID_PARENT.HasValue)
                {
                    schedaDocumento.rispostaDocumento =
                        await this._dbContext.ProfileEntities
                        .Where(p => p.SYSTEM_ID == profileEntity.ID_PARENT)
                        .AsNoTracking()
                        .Select(p => new InfoDocumento()
                        {
                            idProfile = p.SYSTEM_ID.ToString(),
                            segnatura = p.VAR_SEGNATURA,
                            tipoProto = p.CHA_TIPO_PROTO,
                            docNumber = p.DOCNUMBER.ToString()
                        })
                        .FirstOrDefaultAsync();
                }

                if (profileEntity.ID_TIPO_ATTO.HasValue)
                {
                    var tipoAttoEntity = await this._dbContext
                            .TipoAttoEntities
                            .AsNoTracking()
                            .FirstAsync(ta => ta.SYSTEM_ID == profileEntity.ID_TIPO_ATTO);

                    schedaDocumento.tipologiaAtto = new TipologiaAtto()
                    {
                        systemId = tipoAttoEntity.SYSTEM_ID.ToString(),
                        descrizione = tipoAttoEntity.VAR_DESC_ATTO
                    };




                    schedaDocumento.template = this._mapper.Map<DocsPaVO.ProfilazioneDinamica.Templates>(tipoAttoEntity,
                        opt => opt.AfterMap((src, dest) =>
                        {
                            var elencoOggetti = (from dat in this._dbContext.AssociazioneTemplatesEntities
                                                 join docc in this._dbContext.OggettiCustomCompEntities on dat.ID_OGGETTO equals docc.ID_OGG_CUSTOM
                                                 join doc in this._dbContext.OggettiCustomEntities on docc.ID_OGG_CUSTOM equals doc.SYSTEM_ID
                                                 join dto in this._dbContext.TipoOggettoEntities on doc.ID_TIPO_OGGETTO equals dto.SYSTEM_ID
                                                 orderby docc.POSIZIONE
                                                 where dat.DOC_NUMBER == profileEntity.DOCNUMBER.ToString() && docc.ID_TEMPLATE == dat.ID_TEMPLATE
                                                 orderby docc.POSIZIONE
                                                 select new
                                                 {
                                                     SYSTEM_ID = doc.SYSTEM_ID,
                                                     VALORE_DATABASE = dat.VALORE_OGGETTO_DB,
                                                     ANNO = dat.ANNO,
                                                     DESCRIZIONE = doc.DESCRIZIONE,
                                                     TIPO = new TipoOggetto()
                                                     {
                                                         SYSTEM_ID = Convert.ToInt32(dto.SYSTEM_ID),
                                                         DESCRIZIONE_TIPO = dto.DESCRIZIONE
                                                     },
                                                     ORIZZONTALE_VERTICALE = doc.ORIZZONTALE_VERTICALE,
                                                     CAMPO_OBBLIGATORIO = doc.CAMPO_OBBLIGATORIO,
                                                     MULTILINEA = doc.MULTILINEA,
                                                     NUMERO_DI_LINEE = doc.NUMERO_DI_LINEE,
                                                     NUMERO_DI_CARATTERI = doc.NUMERO_DI_CARATTERI,
                                                     CAMPO_DI_RICERCA = doc.CAMPO_DI_RICERCA,
                                                     RESETTA_CONTATORE_INIZIO_ANNO = doc.RESET_ANNO,
                                                     FORMATO_CONTATORE = doc.FORMATO_CONTATORE,
                                                     ID_RUOLO_DEFAULT = doc.ID_R_DEFAULT,
                                                     TIPO_RICERCA_CORR = doc.RICERCA_CORR,
                                                     CONTA_DOPO = doc.CONTA_DOPO,
                                                     REPERTORIO = doc.REPERTORIO,
                                                     CAMPO_COMUNE = doc.CAMPO_COMUNE,
                                                     DA_VISUALIZZARE_RICERCA = doc.DA_VISUALIZZARE_RICERCA,
                                                     FORMATO_ORA = doc.FORMATO_ORA,
                                                     TIPO_LINK = doc.TIPO_LINK,
                                                     TIPO_OBJ_LINK = doc.TIPO_OBJ_LINK,
                                                     CONFIG_OBJ_EST = doc.CONFIG_OBJ_EST,
                                                     MODULO_SOTTOCONTATORE = doc.MODULO_SOTTOCONTATORE,
                                                     CONSOLIDAMENTO = doc.CHA_CONSOLIDAMENTO,
                                                     CONSERVAZIONE = doc.CHA_CONSERVAZIONE,
                                                     CONS_REPERTORIO = doc.CHA_CONS_REPERTORIO,
                                                     DAT_ID_OGG = dat.ID_OGGETTO,
                                                     ASTERISCO_OBBLIGATORIETA = doc.CAMPO_OBBLIGATORIO,
                                                     POSIZIONE = docc.POSIZIONE,
                                                     ID_AOO_RF = dat.ID_AOO_RF,
                                                     DATA_INSERIMENTO = dat.DTA_INS,
                                                     DATA_ANNULLAMENTO = dat.DTA_ANNULLAMENTO
                                                 });

                            OggettoCustom no = new OggettoCustom();
                            List<OggettoCustom> lo = new List<OggettoCustom>();
                            HashSet<long?> distinct = new HashSet<long?>();


                            foreach (var o in elencoOggetti)
                            {
                                if (distinct.Contains(o.DAT_ID_OGG))
                                {
                                    continue;
                                }

                                List<string> valoriSelezionati = new List<string>();
                                List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto> elencoValoriList = new List<DocsPaVO.ProfilazioneDinamica.ValoreOggetto>();
                                if (o.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                                {
                                    var oggCustomValues = _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                    .Where(x => x.ID_OGGETTO == o.SYSTEM_ID && x.DOC_NUMBER == request.DocNumber).Select(o => new
                                    {
                                        o.VALORE_OGGETTO_DB
                                    })
                                    .ToList();
                                    oggCustomValues.ForEach(v =>
                                    {

                                        valoriSelezionati.Add(v.VALORE_OGGETTO_DB ?? string.Empty);

                                    });

                                }


                                if (o.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione") ||
                                    o.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina") ||
                                    o.TIPO.DESCRIZIONE_TIPO.Equals("SelezioneEsclusiva"))
                                {
                                    var oggCustomValues = (from val in _dbContext.AssociazioneValoriEntities.AsNoTracking()
                                                           where val.ID_OGGETTO_CUSTOM == o.SYSTEM_ID
                                                           orderby val.SYSTEM_ID
                                                           select val).ToList();

                                    for (int k = 0; k < oggCustomValues.Count; k++)
                                    {
                                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valOg = null;
                                        switch (o.TIPO.DESCRIZIONE_TIPO)
                                        {
                                            case "CasellaDiSelezione":
                                                if (k < valoriSelezionati.Count)
                                                {
                                                    valOg = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto
                                                    {
                                                        SYSTEM_ID = Convert.ToInt32(oggCustomValues[k].SYSTEM_ID),
                                                        DESCRIZIONE_VALORE = oggCustomValues[k].DESCRIZIONE_VALORE,
                                                        VALORE = oggCustomValues[k].VALORE,
                                                        VALORE_DI_DEFAULT = oggCustomValues[k].VALORE_DI_DEFAULT,
                                                        COLOR_BG = oggCustomValues[k].COLOR_BG,
                                                        ABILITATO = Convert.ToInt32(oggCustomValues[k].ABILITATO)
                                                    };
                                                    elencoValoriList.Add(valOg);
                                                }
                                                break;

                                            default:
                                                valOg = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto
                                                {
                                                    SYSTEM_ID = Convert.ToInt32(oggCustomValues[k].SYSTEM_ID),
                                                    DESCRIZIONE_VALORE = oggCustomValues[k].DESCRIZIONE_VALORE,
                                                    VALORE = oggCustomValues[k].VALORE,
                                                    VALORE_DI_DEFAULT = oggCustomValues[k].VALORE_DI_DEFAULT,
                                                    COLOR_BG = oggCustomValues[k].COLOR_BG,
                                                    ABILITATO = Convert.ToInt32(oggCustomValues[k].ABILITATO)
                                                };
                                                elencoValoriList.Add(valOg);
                                                break;
                                        }
                                    }

                                }

                                no = new OggettoCustom()
                                {
                                    SYSTEM_ID = Convert.ToInt32(o.SYSTEM_ID),
                                    VALORE_DATABASE = string.IsNullOrEmpty(o.VALORE_DATABASE) ? string.Empty : o.VALORE_DATABASE,
                                    ANNO = (o.ANNO.HasValue ? o.ANNO.ToString() : null),
                                    DESCRIZIONE = o.DESCRIZIONE,
                                    TIPO = o.TIPO,
                                    ORIZZONTALE_VERTICALE = o.ORIZZONTALE_VERTICALE,
                                    CAMPO_OBBLIGATORIO = o.CAMPO_OBBLIGATORIO,
                                    MULTILINEA = o.MULTILINEA,
                                    NUMERO_DI_LINEE = o.NUMERO_DI_LINEE,
                                    NUMERO_DI_CARATTERI = o.NUMERO_DI_CARATTERI,
                                    CAMPO_DI_RICERCA = o.CAMPO_DI_RICERCA,
                                    RESETTA_CONTATORE_INIZIO_ANNO = o.RESETTA_CONTATORE_INIZIO_ANNO,
                                    FORMATO_CONTATORE = o.FORMATO_CONTATORE,
                                    ID_RUOLO_DEFAULT = o.ID_RUOLO_DEFAULT,
                                    TIPO_RICERCA_CORR = o.TIPO_RICERCA_CORR,
                                    CONTA_DOPO = (o.CONTA_DOPO.HasValue ? o.CONTA_DOPO.ToString() : null),
                                    REPERTORIO = (o.REPERTORIO.HasValue ? o.REPERTORIO.ToString() : null),
                                    CAMPO_COMUNE = o.CAMPO_COMUNE != null && o.CAMPO_COMUNE == 1 ? "1" : "0",
                                    DA_VISUALIZZARE_RICERCA = (o.DA_VISUALIZZARE_RICERCA.HasValue ? o.DA_VISUALIZZARE_RICERCA.ToString() : null),
                                    FORMATO_ORA = o.FORMATO_ORA != null ? o.FORMATO_ORA : string.Empty,
                                    TIPO_LINK = o.TIPO_LINK,
                                    TIPO_OBJ_LINK = o.TIPO_OBJ_LINK,
                                    CONFIG_OBJ_EST = o.CONFIG_OBJ_EST,
                                    MODULO_SOTTOCONTATORE = (o.MODULO_SOTTOCONTATORE.HasValue ? o.MODULO_SOTTOCONTATORE.ToString() : null),
                                    CONSOLIDAMENTO = o.CONSOLIDAMENTO,
                                    CONSERVAZIONE = o.CONSERVAZIONE,
                                    CONS_REPERTORIO = o.CONS_REPERTORIO,
                                    ELENCO_VALORI = elencoValoriList.ToArray(),
                                    VALORI_SELEZIONATI = valoriSelezionati.ToArray(),
                                    ASTERISCO_OBBLIGATORIETA = o.ASTERISCO_OBBLIGATORIETA,
                                    POSIZIONE = o.POSIZIONE != null ? o.POSIZIONE.ToString() : string.Empty,
                                    ID_AOO_RF = o.ID_AOO_RF.HasValue ? o.ID_AOO_RF.ToString() : null,
                                    DATA_INSERIMENTO = o.DATA_INSERIMENTO.HasValue ? o.DATA_INSERIMENTO.AsDateTimeFormat() : null,
                                    DATA_ANNULLAMENTO = o.DATA_ANNULLAMENTO.HasValue ? o.DATA_ANNULLAMENTO.AsDateTimeFormat() : null
                                };
                                lo.Add(no);
                                distinct.Add(o.DAT_ID_OGG);
                            }

                            dest.ELENCO_OGGETTI = lo.ToArray();
                        }));

                }

                schedaDocumento.tipoProto = profileEntity.CHA_TIPO_PROTO;
                schedaDocumento.typeId = (documentTypesEntity != null ? documentTypesEntity.TYPE_ID : null);
                schedaDocumento.systemId = profileEntity.SYSTEM_ID.ToString();
                schedaDocumento.userId = (authorProtEntity ?? authorEntity).USER_ID;

                if (_dbContext.NoteEntities.Where(w => w.IDOGGETTOASSOCIATO == schedaDocumento.systemId.AsLong()).Any())
                {
                    schedaDocumento.noteDocumento = await _dbContext.NoteEntities.Join(_dbContext.PeopleEntities, n => n.IDUTENTECREATORE, p => p.SYSTEM_ID, (n, p) => new { n, p }).
                         Join(_dbContext.GroupEntities, j => j.n.IDRUOLOCREATORE, r => r.SYSTEM_ID, (j, r) => new { j.n, j.p, r }).
                         Where(w => w.n.IDOGGETTOASSOCIATO == schedaDocumento.systemId.AsLong()
                            && (w.n.TIPOVISIBILITA == "T"
                            || (w.n.TIPOVISIBILITA == "F" && this._dbContext.RuoloRegistroEntities.AsNoTracking().Any(r => r.ID_REGISTRO == w.n.IDRFASSOCIATO && r.ID_RUOLO_IN_UO == idCorrGlobali))
                            || (w.n.TIPOVISIBILITA == "P" && w.n.IDUTENTECREATORE == idUserAsNumber)
                            || (w.n.TIPOVISIBILITA == "R" && w.n.IDRUOLOCREATORE == idGroupAsNumber))
                         )
                         .Select(s => new InfoNota()
                         {
                             DataCreazione = s.n.DATACREAZIONE,
                             IdPeopleDelegato = s.n.IDPEOPLEDELEGATO != null ? s.n.IDPEOPLEDELEGATO.ToString() : string.Empty,
                             Id = s.n.SYSTEM_ID.ToString(),
                             IdRfAssociato = s.n.IDRFASSOCIATO != null ? s.n.IDRFASSOCIATO.ToString() : string.Empty,
                             Testo = s.n.TESTO!,
                             TipoVisibilita = s.n.TIPOVISIBILITA == "T" ? TipiVisibilitaNotaEnum.Tutti :
                                  s.n.TIPOVISIBILITA == "R" ? TipiVisibilitaNotaEnum.Ruolo :
                                  s.n.TIPOVISIBILITA == "F" ? TipiVisibilitaNotaEnum.RF :
                                  TipiVisibilitaNotaEnum.Personale,
                             UtenteCreatore = new InfoUtenteCreatoreNota()
                             {
                                 IdUtente = s.n.IDUTENTECREATORE.ToString(),
                                 DescrizioneUtente = s.p.FULL_NAME,
                                 IdRuolo = s.n.IDRUOLOCREATORE.ToString(),
                                 DescrizioneRuolo = s.r.GROUP_NAME
                             },
                             DaInserire = false,
                             DaRimuovere = false,


                         }).ToListAsync();
                }



                if (_dbContext.ProfParoleEntities.Where(w => w.ID_PROFILE == schedaDocumento.systemId.AsLong()).Any())
                {
                    schedaDocumento.paroleChiave = await _dbContext.ProfParoleEntities.Join(_dbContext.ParolaEntities, pp => pp.ID_PAROLA, p => p.SYSTEM_ID, (pp, p) => new { pp, p }).
                         Where(w => w.pp.ID_PROFILE == schedaDocumento.systemId.AsLong()).Select(s => new ParolaChiave()
                         {
                             systemId = s.p.SYSTEM_ID.ToString(),
                             descrizione = s.p.VAR_DESC_PAROLA,
                             idAmministrazione = s.p.ID_AMM.ToString(),
                             idRegistro = s.p.ID_REGISTRO.ToString()
                         }).ToArrayAsync();

                }



                await this._webMethodLoggerService.LogOK(
                    "DOCUMENTOGETDETTAGLIODOCUMENTO",
                    schedaDocumento.systemId,
                    (schedaDocumento.protocollo != null ?
                            string.Format(Resources.LogDescriptorProtocollo, schedaDocumento.docNumber, schedaDocumento.protocollo.segnatura) :
                            string.Format(Resources.LogDescriptorNonProtocollato, schedaDocumento.docNumber, schedaDocumento.dataCreazione)));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);

                await this._webMethodLoggerService.LogKO(
                    "DOCUMENTOGETDETTAGLIODOCUMENTO",
                    request.IdProfile);

                schedaDocumento = null;
            }

            return new()
            {
                Output = schedaDocumento
            };
        }



        #region Private Members

        protected readonly ILogger<DocumentoGetDettaglioDocumentoNoSecurityCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMapper _mapper = null;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDistributedCache _distributedCache;

        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VersionEntity, DocsPaVO.documento.Documento>();

                cfg.CreateMap<ComponentEntity, DocsPaVO.documento.Documento>();

                cfg.CreateMap<ProfileEntity, DocsPaVO.Security.InfoAtipicita>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CHA_COD_T_A))
                        {
                            dest.IdDocFasc = src.SYSTEM_ID.ToString();
                            dest.TipoOggetto = DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO;
                            dest.CodiceAtipicita = src.CHA_COD_T_A;
                        }
                    });

                cfg.CreateMap<TipoAttoEntity, DocsPaVO.ProfilazioneDinamica.Templates>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.DESCRIZIONE, opt => opt.MapFrom(src => src.VAR_DESC_ATTO))
                    .ForMember(dest => dest.ID_TIPO_ATTO, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.PATH_MODELLO_1, opt => opt.MapFrom(src => src.PATH_MOD_1))
                    .ForMember(dest => dest.PATH_MODELLO_2, opt => opt.MapFrom(src => src.PATH_MOD_2))
                    .ForMember(dest => dest.PATH_MODELLO_1_EXT, opt => opt.MapFrom(src => src.EXT_MOD_1))
                    .ForMember(dest => dest.PATH_MODELLO_2_EXT, opt => opt.MapFrom(src => src.EXT_MOD_2))
                    .ForMember(dest => dest.PATH_MODELLO_STAMPA_UNIONE, opt => opt.MapFrom(src => src.PATH_MOD_SU))
                    .ForMember(dest => dest.PATH_MODELLO_EXCEL, opt => opt.MapFrom(src => src.PATH_MOD_EXC))
                    .ForMember(dest => dest.PATH_XSD_ASSOCIATO, opt => opt.MapFrom(src => src.PATH_XSD_ASSOCIATO))
                    .ForMember(dest => dest.PATH_ALLEGATO_1, opt => opt.MapFrom(src => src.PATH_ALL_1))
                    .AfterMap((src, dest) =>
                    {

                    });

                cfg.CreateMap<RegistroEntity, DocsPaVO.utente.Registro>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.codRegistro, opt => opt.MapFrom(src => src.VAR_CODICE))
                    .ForMember(dest => dest.codice, opt => opt.MapFrom(src => src.VAR_CODICE))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_REGISTRO))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.VAR_EMAIL_REGISTRO))
                    .ForMember(dest => dest.stato, opt => opt.MapFrom(src => src.CHA_STATO))
                    .ForMember(dest => dest.dataApertura, opt => opt.MapFrom(src => (src.DTA_OPEN.HasValue ? src.DTA_OPEN.AsDateFormat() : null)))
                    .ForMember(dest => dest.dataChiusura, opt => opt.MapFrom(src => (src.DTA_CLOSE.HasValue ? src.DTA_CLOSE.AsDateFormat() : null)))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.ID_AMM))
                    .ForMember(dest => dest.dataUltimoProtocollo, opt => opt.MapFrom(src => (src.DTA_ULTIMO_PROTO.HasValue ? src.DTA_ULTIMO_PROTO.AsDateFormat() : null)))
                    .ForMember(dest => dest.ultimoNumeroProtocollo, opt => opt.MapFrom(src => src.NUM_RIF))
                    .ForMember(dest => dest.idRuoloResp, opt => opt.MapFrom(src => src.ID_RUOLO_RESP))
                    .ForMember(dest => dest.idRuoloAOO, opt => opt.MapFrom(src => src.ID_RUOLO_RESP))
                    .ForMember(dest => dest.idAOOCollegata, opt => opt.MapFrom(src => src.ID_AOO_COLLEGATA))
                    .ForMember(dest => dest.idUtenteAOO, opt => opt.MapFrom(src => src.ID_PEOPLE_AOO))
                    .ForMember(dest => dest.autoInterop, opt => opt.MapFrom(src => src.CHA_AUTO_INTEROP))
                    .ForMember(dest => dest.chaRF, opt => opt.MapFrom(src => src.CHA_RF))
                    .ForMember(dest => dest.rfDisabled, opt => opt.MapFrom(src => src.CHA_DISABILITATO))
                    .ForMember(dest => dest.Diritto_Ruolo_AOO, opt => opt.MapFrom(src => src.DIRITTO_RUOLO_AOO))
                    .ForMember(dest => dest.invioRicevutaManuale, opt => opt.MapFrom(src => src.INVIO_RICEVUTA_MANUALE))
                    .ForMember(dest => dest.FlagWspia, opt => opt.MapFrom(src => src.FLAG_WSPIA != null ? src.FLAG_WSPIA : "0"))
                    .ForMember(dest => dest.flag_pregresso, opt => opt.MapFrom(src => (!string.IsNullOrWhiteSpace(src.VAR_PREG) ? src.VAR_PREG == "1" : false)))
                    .ForMember(dest => dest.anno_pregresso, opt => opt.MapFrom(src => src.ANNO_PREG))
                    .ForMember(dest => dest.codiceIpa, opt => opt.MapFrom(src => src.VAR_CODICE_IPA));

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.Corrispondente>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.Emails, opt => opt.Ignore())
                    .ForMember(dest => dest.info, opt => opt.Ignore())
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR))
                    .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                    .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                    .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                    .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                    .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                    .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                    .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                    .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                    .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                    .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                    .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                    .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                    .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                    .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                    .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                    .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                    .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                    .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                    .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                    .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                    .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                    .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                        {
                            dest.Url = new List<Corrispondente.UrlInfo>()
                            {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                            };
                        }
                    });

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.Utente>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.Emails, opt => opt.Ignore())
                    .ForMember(dest => dest.info, opt => opt.Ignore())
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR))
                    .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                    .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                    .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                    .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                    .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                    .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                    .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                    .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                    .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                    .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                    .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                    .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                    .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                    .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                    .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                    .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                    .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                    .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                    .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                    .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                    .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                    .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                        {
                            dest.Url = new List<Corrispondente.UrlInfo>()
                            {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                            };
                        }
                    });

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.UnitaOrganizzativa>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.Emails, opt => opt.Ignore())
                    .ForMember(dest => dest.info, opt => opt.Ignore())
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR))
                    .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                    .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                    .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                    .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                    .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                    .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                    .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                    .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                    .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                    .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                    .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                    .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                    .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                    .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                    .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                    .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                    .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                    .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                    .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                    .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                    .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                    .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                        {
                            dest.Url = new List<Corrispondente.UrlInfo>()
                            {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                            };
                        }
                    });

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.Ruolo>()
                    .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForMember(dest => dest.Emails, opt => opt.Ignore())
                    .ForMember(dest => dest.info, opt => opt.Ignore())
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                    .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR))
                    .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                    .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                    .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                    .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                    .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                    .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                    .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                    .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                    .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                    .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                    .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                    .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                    .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                    .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                    .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                    .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                    .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                    .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                    .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                    .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                    .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                    .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                    .AfterMap((src, dest) =>
                    {
                        if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                        {
                            dest.Url = new List<Corrispondente.UrlInfo>()
                            {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                            };
                        }
                    });

                cfg.CreateMap<SoggettoProtocolloEntity, DocsPaVO.utente.RaggruppamentoFunzionale>()
                   .IgnoreAllPropertiesWithAnInaccessibleSetter()
                   .ForMember(dest => dest.Emails, opt => opt.Ignore())
                   .ForMember(dest => dest.info, opt => opt.Ignore())
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.CorrGlobali.SYSTEM_ID))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR))
                   .ForMember(dest => dest.codiceCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE))
                   .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COD_RUBRICA))
                   .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.CorrGlobali.ID_AMM))
                   .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR))
                   .ForMember(dest => dest.tipoIE, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_IE))
                   .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.CorrGlobali.ID_REGISTRO))
                   .ForMember(dest => dest.dettagli, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DETTAGLI == "1"))
                   .ForMember(dest => dest.idOld, opt => opt.MapFrom(src => src.CorrGlobali.ID_OLD))
                   .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.CorrGlobali.VAR_EMAIL))
                   .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AOO))
                   .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.CorrGlobali.VAR_CODICE_AMM))
                   .ForMember(dest => dest.dta_fine, opt => opt.MapFrom(src => src.CorrGlobali.DTA_FINE.AsDateTimeFormat()))
                   .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_NOME))
                   .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.CorrGlobali.VAR_COGNOME))
                   .ForMember(dest => dest.inRubricaComune, opt => opt.MapFrom(src => src.CorrGlobali.CHA_TIPO_CORR == "C"))
                   .ForMember(dest => dest.oldDescrizione, opt => opt.MapFrom(src => src.CorrGlobali.VAR_DESC_CORR_OLD))
                   .ForMember(dest => dest.disabledTrasm, opt => opt.MapFrom(src => src.CorrGlobali.CHA_DISABLED_TRASM == "1"))
                   .ForMember(dest => dest.rubricaEsterna, opt => opt.MapFrom(src => src.CorrGlobali.RUBRICA_ESTERNA))
                   .ForMember(dest => dest.citta, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CITTA : null)))
                   .ForMember(dest => dest.cap, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_CAP : null)))
                   .ForMember(dest => dest.prov, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_PROVINCIA : null)))
                   .ForMember(dest => dest.nazionalita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NAZIONE : null)))
                   .ForMember(dest => dest.telefono1, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO : null)))
                   .ForMember(dest => dest.telefono2, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TELEFONO2 : null)))
                   .ForMember(dest => dest.fax, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_FAX : null)))
                   .ForMember(dest => dest.codfisc, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISC : null)))
                   .ForMember(dest => dest.partitaiva, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_COD_FISCALE : null)))
                   .ForMember(dest => dest.note, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_NOTE : null)))
                   .ForMember(dest => dest.localita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LOCALITA : null)))
                   .ForMember(dest => dest.luogoDINascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_LUOGO_NASCITA : null)))
                   .ForMember(dest => dest.dataNascita, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.DTA_NASCITA : null)))
                   .ForMember(dest => dest.titolo, opt => opt.MapFrom(src => (src.DettCorrGlobali != null ? src.DettCorrGlobali.VAR_TITOLO : null)))
                   .AfterMap((src, dest) =>
                   {
                       if (!string.IsNullOrWhiteSpace(src.CorrGlobali.INTEROPURL))
                       {
                           dest.Url = new List<Corrispondente.UrlInfo>()
                           {
                                new Corrispondente.UrlInfo()
                                {
                                    Url = src.CorrGlobali.INTEROPURL
                                }
                           };
                       }
                   });
            });

            return configuration.CreateMapper();
        }

        protected class SoggettoProtocolloEntity
        {
            public DocArrivoParEntity DocArrivoPar { get; set; }
            public CorrGlobaliEntity CorrGlobali { get; set; }
            public DettGlobaliEntity DettCorrGlobali { get; set; }
            public CanaleCorrEntity CanaleCorr { get; set; }
        }

        protected class AuthorEntity
        {
            public long SYSTEM_ID { get; set; }
            public string USER_ID { get; set; }
            public long? ID_AMM { get; set; }
        }

        protected class AuthorGroupEntity
        {
            public long? ID_GRUPPO { get; set; }
        }

        protected class AuthorUOEntity
        {
            public long SYSTEM_ID { get; set; }
            public string VAR_CODICE { get; set; }
        }

        protected class UOProtocollatoreEntity
        {
            public long SYSTEM_ID { get; set; }
            public string VAR_CODICE { get; set; }
        }

        #endregion
    }
}
