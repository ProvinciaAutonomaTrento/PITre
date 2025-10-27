// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.Interoperabilita;
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cmp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFile;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSpedizioneDocumento
{
    public class GetSpedizioneDocumentoCommandHandler : IRequestHandler<GetSpedizioneDocumentoCommand, GetSpedizioneDocumentoCommandResponse>
    {
        #region Public Members

        public GetSpedizioneDocumentoCommandHandler(
           ILogger<GetSpedizioneDocumentoCommandHandler> logger,
           IClaimsPrincipalService claimsPrincipalService,
           IMediator mediator,
           IPi3DbContext pi3DbContext,
           IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._configurationService = configurationService;

            this.InitializeMapper();
        }

        public async Task<GetSpedizioneDocumentoCommandResponse> Handle(GetSpedizioneDocumentoCommand request, CancellationToken cancellationToken)
        {
            DocsPaVO.Spedizione.SpedizioneDocumento output = null!;

            try
            {
                await this.AssertProtocolloUscita(request.Documento);

                output = new DocsPaVO.Spedizione.SpedizioneDocumento();
                output.IdDocumento = request.Documento.systemId;

                // Per impostazione predefinita, viene impostato il registro del protocollo del documento
                output.IdRegistroRfMittente = request.Documento.registro.systemId;

                // Reperimento dei destinari del documento
                await this.FetchDestinatari(output, request.InfoUtente, request.Documento);

                // Determina se il documento è stato spedito almeno una volta ad uno o più destinatari
                output.Spedito =
                    (output.DestinatariEsterni.Count(e => !string.IsNullOrEmpty(e.DataUltimaSpedizione)) +
                    output.DestinatariInterni.Count(e => !string.IsNullOrEmpty(e.DataUltimaSpedizione)) > 0 +
                    output.DestinatariInterni.Count(e => e.DisabledTrasm));
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;

                this._logger.LogError(pi3Ex, $"{pi3Ex.Message} ID DOCUMENTO: {request.Documento.systemId}");
            }
            catch (Exception ex)
            {
                output = null!;

                this._logger.LogCritical(ex, ex.Message);
            }

            return new GetSpedizioneDocumentoCommandResponse()
            {
                SpedizioneDocumento = output,
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetSpedizioneDocumentoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IConfigurationService _configurationService;

        protected IMapper _mapper = null;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, UnitaOrganizzativa>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_CORR))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.livello, src => src.MapFrom(opt => opt.NUM_LIVELLO))
                    .ForMember(dest => dest.codiceRubrica, src => src.MapFrom(opt => opt.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.interoperante, src => src.MapFrom(opt => opt.CHA_PA == "1"))
                    .ForMember(dest => dest.codiceAOO, src => src.MapFrom(opt => opt.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, src => src.MapFrom(opt => opt.VAR_CODICE_AMM))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL))
                    .ForMember(dest => dest.tipoIE, src => src.MapFrom(opt => opt.CHA_TIPO_IE))
                    .ForMember(dest => dest.tipoCorrispondente, src => src.MapFrom(opt => opt.CHA_TIPO_URP))
                    .ForMember(dest => dest.classificaUO, src => src.MapFrom(opt => opt.CLASSIFICA_UO));

                cfg.CreateMap<RegistroEntity, DocsPaVO.utente.Registro>()
                   .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                   .ForMember(dest => dest.codRegistro, src => src.MapFrom(opt => opt.VAR_CODICE))
                   .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.NUM_RIF))
                   .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_REGISTRO))
                   .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL_REGISTRO))
                   .ForMember(dest => dest.stato, src => src.MapFrom(opt => opt.CHA_STATO))
                   .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                   .ForMember(dest => dest.dataApertura, src => src.MapFrom(opt => opt.DTA_OPEN.AsDateFormat()))
                   .ForMember(dest => dest.dataChiusura, src => src.MapFrom(opt => opt.DTA_CLOSE.AsDateFormat()))
                   .ForMember(dest => dest.dataUltimoProtocollo, src => src.MapFrom(opt => opt.DTA_ULTIMO_PROTO.AsDateFormat()))
                   .ForMember(dest => dest.idRuoloAOO, src => src.MapFrom(opt => opt.ID_RUOLO_AOO))
                   .ForMember(dest => dest.idRuoloResp, src => src.MapFrom(opt => opt.ID_RUOLO_RESP))
                   .ForMember(dest => dest.idUtenteAOO, src => src.MapFrom(opt => opt.ID_PEOPLE_AOO))
                   .ForMember(dest => dest.autoInterop, src => src.MapFrom(opt => opt.CHA_AUTO_INTEROP))
                   .ForMember(dest => dest.Diritto_Ruolo_AOO, src => src.MapFrom(opt => opt.DIRITTO_RUOLO_AOO))
                   .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE == 0 ? "0" : "1"))
                   .ForMember(dest => dest.FlagWspia, src => src.MapFrom(opt => opt.FLAG_WSPIA == null ? "0" : opt.FLAG_WSPIA))
                   .ForMember(dest => dest.flag_pregresso, src => src.MapFrom(opt => opt.VAR_PREG == "1"))
                   .ForMember(dest => dest.anno_pregresso, src => src.MapFrom(opt => opt.VAR_PREG == "1" ? opt.ANNO_PREG : string.Empty))
                   .ForMember(dest => dest.codiceIpa, src => src.MapFrom(opt => opt.VAR_CODICE_IPA));
            });

            this._mapper = configuration.CreateMapper();
        }

        private const string InteroperabilityCode = "SIMPLIFIEDINTEROPERABILITY";

        protected virtual async Task AssertProtocolloUscita(DocsPaVO.documento.SchedaDocumento documento)
        {
            if (string.IsNullOrEmpty(documento.systemId))
                throw new GetSpedizioneDocumentoPi3Exception("Documento non salvato");

            DocsPaVO.documento.ProtocolloUscita protocolloUscita = documento.protocollo as DocsPaVO.documento.ProtocolloUscita;

            if (protocolloUscita == null || (protocolloUscita != null && string.IsNullOrEmpty(protocolloUscita.numero)))
                throw new GetSpedizioneDocumentoPi3Exception(string.Format("Documento con id {0} non protocollato in uscita", documento.systemId));
        }

        protected virtual async Task FetchDestinatari(DocsPaVO.Spedizione.SpedizioneDocumento output, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento)
        {
            // Caricamento dei destinari esterni del documento
            await this.FetchDestinatariEsterni(output, infoUtente, documento);

            // Caricamento dei destinatari interni del documento
            await this.FetchDestinatariInterni(output, infoUtente, documento);
        }

        protected virtual async Task FetchDestinatariEsterni(
            DocsPaVO.Spedizione.SpedizioneDocumento output,
            DocsPaVO.utente.InfoUtente infoUtente,
            DocsPaVO.documento.SchedaDocumento documento)
        {
            DocsPaVO.documento.ProtocolloUscita protocolloUscita = (DocsPaVO.documento.ProtocolloUscita)documento.protocollo;

            var response = new DocsPaVO.Interoperabilita.SendDocumentResponse();

            var destinatariDivisi = await this.DividiDestinatari(
                (protocolloUscita.destinatari ?? new Corrispondente[0])
                    .Concat(protocolloUscita.destinatariConoscenza ?? new Corrispondente[0]).ToArray(),
                documento.registro.codRegistro,
                documento.registro.email,
                response,
                infoUtente.idAmministrazione);

            var corrispondentiInteroperanti = new List<DocsPaVO.utente.Corrispondente>();

            foreach (DictionaryEntry entry in destinatariDivisi)
            {
                ArrayList listDestinatari = (ArrayList)entry.Value;

                foreach (DocsPaVO.utente.Corrispondente destinatario in listDestinatari)
                {
                    var destinatarioEsterno = new DocsPaVO.Spedizione.DestinatarioEsterno();
                    destinatarioEsterno.Id = destinatario.systemId;
                    destinatarioEsterno.Interoperante = true;
                    destinatarioEsterno.Email = entry.Key.ToString();
                    destinatarioEsterno.DatiDestinatari.Add(destinatario);
                    corrispondentiInteroperanti.Add(destinatario);

                    //se il corrispondente è interoperabilità passo l'url come indirizzo
                    /*
                     * if (destinatario.canalePref != null &&
                        destinatario.canalePref.typeId.Equals("SIMPLIFIEDINTEROPERABILITY") &&
                        destinatario.Url != null &&
                        destinatario.Url.Any())
                    {
                        destinatarioEsterno.DataUltimaSpedizione = await this.GetDataUltimaSpedizione(infoUtente, documento, destinatario);
                    }
                    else
                    {
                        destinatarioEsterno.DataUltimaSpedizione = await this.GetDataUltimaSpedizione(infoUtente, documento, destinatario);
                    }
                    */

                    destinatarioEsterno.DataUltimaSpedizione = await this.GetDataUltimaSpedizione(infoUtente, documento, destinatario);

                    // Determina se il documento è stato spedito almeno una volta al destinatario
                    bool spedito = !string.IsNullOrEmpty(destinatarioEsterno.DataUltimaSpedizione);
                    destinatarioEsterno.IncludiInSpedizione = !spedito;

                    if (spedito)
                    {
                        // Se è stato spedito ma è presente mancata consegna devo rispedire di nuovo
                        DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum stato = await this.GetStatoInvio(infoUtente, documento.systemId, destinatario.systemId);
                        destinatarioEsterno.StatoSpedizione = this.GetStatoSpedizione(stato);

                        if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaRispedire)
                            destinatarioEsterno.IncludiInSpedizione = true;
                    }
                    else
                        destinatarioEsterno.StatoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaSpedire);

                    output.DestinatariEsterni.Add(destinatarioEsterno);
                }
            }

            //recupero le caselle di posta associate ai corrispondenti esterni interoperanti 
            await this.AssociaCaselleDestinatariEsterni(output.DestinatariEsterni);

            // Caricamento dei destinatari esterni non interoperanti
            foreach (DocsPaVO.utente.Corrispondente item in
                (protocolloUscita.destinatari ?? new Corrispondente[0])
                    .Concat(protocolloUscita.destinatariConoscenza ?? new Corrispondente[0]))
            {
                if (!await this.IsDestinatarioInterno(item, documento.registro.systemId, infoUtente))
                {
                    // Determina se il corrispondente esterno è già stato inserito tra gli interoperanti
                    if (corrispondentiInteroperanti.Count(e => e.systemId == item.systemId) == 0)
                    {
                        DocsPaVO.Spedizione.DestinatarioEsterno destinatario = new DocsPaVO.Spedizione.DestinatarioEsterno
                        {
                            Id = item.systemId,
                            IncludiInSpedizione = false,
                            Interoperante = false,
                            Email = item.email,
                            StatoSpedizione = GetStatoDestinatarioEsternoNonInteroperante(item) // Reperimento stato di spedizione verso il destinatario classificato come non interoperante
                        };

                        destinatario.DatiDestinatari.Add(item);
                        output.DestinatariEsterni.Add(destinatario);
                    }
                }
            }

            if (output.DestinatariEsterni != null)
                output.DestinatariEsterni = new List<DocsPaVO.Spedizione.DestinatarioEsterno>(output.DestinatariEsterni.OrderBy(destEst => destEst.StatoSpedizione.Descrizione));
        }

        protected virtual async Task FetchDestinatariInterni(DocsPaVO.Spedizione.SpedizioneDocumento output, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento)
        {
            DocsPaVO.documento.ProtocolloUscita protocolloUscita = (DocsPaVO.documento.ProtocolloUscita)documento.protocollo;

            ArrayList destinatari = new ArrayList();
            if (protocolloUscita.destinatari != null)
                destinatari.AddRange(protocolloUscita.destinatari);
            if (protocolloUscita.destinatariConoscenza != null)
                destinatari.AddRange(protocolloUscita.destinatariConoscenza);

            foreach (DocsPaVO.utente.Corrispondente destinatario in destinatari)
            {
                if (await this.IsDestinatarioInterno(destinatario, documento.registro.systemId, infoUtente))
                {
                    // Benché interno, determina se il corrispondente non appartenga ad un'altra AOO
                    // verificando se è considerato tra i destinatari esterni
                    if (output.DestinatariEsterni.Count(e => e.DatiDestinatari.Any(d => d.systemId == destinatario.systemId)) == 0)
                    {
                        DocsPaVO.Spedizione.DestinatarioInterno item = new DocsPaVO.Spedizione.DestinatarioInterno();

                        // Impostazione dell'id del corrispondente come identificativo univoco del destinatario
                        item.Id = destinatario.systemId;
                        item.Email = destinatario.email;
                        item.DatiDestinatario = destinatario;
                        item.DataUltimaSpedizione = await this.GetDataUltimaTrasmissione(infoUtente, documento, destinatario);

                        // Determina se il documento è stato spedito almeno una volta al destinatario
                        bool spedito = !string.IsNullOrEmpty(item.DataUltimaSpedizione);

                        // Nel caso non sia stato spedito, viene forzata l'impostazione di spedizione
                        item.IncludiInSpedizione = !spedito;

                        if (this.IsDestinatarioOccasionale(destinatario))
                            item.StatoSpedizione = this.GetStatoErroreInTrasmissione("Impossibile spedire il documento ad un occasionale");
                        else if (spedito)
                            item.StatoSpedizione = this.GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito);
                        else
                            item.StatoSpedizione = this.GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaSpedire);

                        //In funzione dell'abilitazione alle trasmissioni vengono impostate le proprietà
                        if (destinatario.disabledTrasm)
                        {
                            item.DisabledTrasm = destinatario.disabledTrasm;
                            item.IncludiInSpedizione = !destinatario.disabledTrasm;
                            item.StatoSpedizione = GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DisabilitatoTrasmissioni);
                        }

                        output.DestinatariInterni.Add(item);
                    }
                }
            }

            if (output.DestinatariInterni != null)
                output.DestinatariInterni = new List<DocsPaVO.Spedizione.DestinatarioInterno>(output.DestinatariInterni.OrderBy(destInt => destInt.StatoSpedizione.Descrizione));
        }

        private async Task<string> GetDataUltimaSpedizione(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.utente.Corrispondente dest)
        {
            // Se il documento è stato spedito per interoperabilità,
            // l'informazione relativa alla data di spedizione è nella DPA_STATO_INVIO,
            // altrimenti l'informazione viene reperita dalla DPA_TRASMISSIONE
            string dataSpedizione = string.Empty;

            // Reperimento della data di ultima spedizione del documento
            var dataUltimaSpedizione = await (this._pi3DbContext.StatoInvioEntities.AsNoTracking()
                       .Where(i => i.ID_CORR_GLOBALE == dest.systemId.AsLong()
                            && i.ID_PROFILE == documento.systemId.AsLong())
                       .OrderByDescending(i => i.SYSTEM_ID)
                       .Select(i => i.DTA_SPEDIZIONE))
                       .FirstOrDefaultAsync();

            if (!dataUltimaSpedizione.HasValue || (dataUltimaSpedizione.HasValue && dataUltimaSpedizione.Value == DateTime.MinValue))
                // Il documento risulta non spedito oppure l'ultima spedizione effettuata per 
                // non è andata a buon fine
                dataSpedizione = string.Empty;
            else
                dataSpedizione = dataUltimaSpedizione.Value.AsDateTimeFormat();

            return dataSpedizione;
        }

        private async Task<string> GetDataUltimaTrasmissione(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.utente.Corrispondente destinatario)
        {
            string idDestinatario = string.Empty;

            if (destinatario is DocsPaVO.utente.UnitaOrganizzativa)
            {
                // Quando il destinatario di una trasmissione è una UO,
                // la trasmissione in realtà viene effettuata ai ruoli di riferimento dello stesso.
                // Pertanto nella ricerca della data ultima trasmissione, 
                // sarà reperita quella relativa alla trasmissione
                // inoltrata agli utenti del primo ruolo di riferimento trovato nell'UO.
                var chaRiferimentoResult = await (this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(cg => cg.ID_UO == destinatario.systemId.AsLong()
                                && cg.CHA_TIPO_URP == "R"
                                && cg.DTA_FINE == null
                                && cg.CHA_RIFERIMENTO == "1")
                        .Select(cg => cg.SYSTEM_ID))
                        .FirstOrDefaultAsync();

                if (chaRiferimentoResult > 0)
                    idDestinatario = chaRiferimentoResult.ToString();
            }
            else
                // Il destinatario è un utente o un ruolo
                idDestinatario = destinatario.systemId;

            if (string.IsNullOrEmpty(idDestinatario))
                return string.Empty;

            var dataInvio = await (from t in this._pi3DbContext.TrasmissioneEntities.AsNoTracking()
                                   join ts in this._pi3DbContext.TrasmSingolaEntities.AsNoTracking() on t.SYSTEM_ID equals ts.ID_TRASMISSIONE
                                   where t.CHA_TIPO_OGGETTO == "D"
                                           && t.ID_PROFILE == documento.systemId.AsLong()
                                           && ts.ID_CORR_GLOBALE == idDestinatario.AsLong()
                                   orderby t.SYSTEM_ID descending
                                   select t.DTA_INVIO)
                            .FirstOrDefaultAsync();

            if (dataInvio.HasValue)
                return dataInvio.Value.AsDateTimeFormat();
            else
                return String.Empty;
        }

        private DocsPaVO.Spedizione.StatoSpedizioneDocumento GetStatoErroreInTrasmissione(string errore)
        {
            DocsPaVO.Spedizione.StatoSpedizioneDocumento statoSpedizione = GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione);
            statoSpedizione.Descrizione = errore;
            return statoSpedizione;
        }

        private DocsPaVO.Spedizione.StatoSpedizioneDocumento GetStatoTrasmissione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum stato)
        {
            DocsPaVO.Spedizione.StatoSpedizioneDocumento statoSpedizione = new DocsPaVO.Spedizione.StatoSpedizioneDocumento();

            statoSpedizione.Stato = stato;
            if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaSpedire)
                statoSpedizione.Descrizione = "Da trasmettere";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito)
                statoSpedizione.Descrizione = "Trasmesso";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione)
                statoSpedizione.Descrizione = "Errore nella trasmissione";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DisabilitatoTrasmissioni)
                statoSpedizione.Descrizione = "Non abilitato alla ricezione delle trasmissioni";
            return statoSpedizione;
        }

        private DocsPaVO.Spedizione.StatoSpedizioneDocumento GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum stato)
        {
            DocsPaVO.Spedizione.StatoSpedizioneDocumento statoSpedizione = new DocsPaVO.Spedizione.StatoSpedizioneDocumento();

            statoSpedizione.Stato = stato;
            if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaSpedire)
                statoSpedizione.Descrizione = "Da spedire";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito)
                statoSpedizione.Descrizione = "Spedito";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione)
                statoSpedizione.Descrizione = "Errore nella spedizione";
            else if (stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.DaRispedire)
                statoSpedizione.Descrizione = "Da rispedire";

            return statoSpedizione;
        }

        private async Task<DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum> GetStatoInvio(
            DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string idDest)
        {
            StatiSpedizioneDocumentoEnum statoSpedizione = StatiSpedizioneDocumentoEnum.Spedito;

            var stato = await (this._pi3DbContext.StatoInvioEntities.AsNoTracking()
                    .Where(s => s.ID_CORR_GLOBALE == idDest.AsLong()
                        && s.ID_PROFILE == idProfile.AsLong())
                    .Select(s => s.STATUS_C_MASK))
                    .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(stato) && stato.Substring(0, 1) == "X")
                statoSpedizione = StatiSpedizioneDocumentoEnum.DaRispedire;

            return statoSpedizione;
        }

        private bool IsDestinatarioOccasionale(DocsPaVO.utente.Corrispondente destinatario)
        {
            return destinatario.tipoCorrispondente == "O";
        }

        private DocsPaVO.Spedizione.StatoSpedizioneDocumento GetStatoDestinatarioEsternoNonInteroperante(DocsPaVO.utente.Corrispondente destinatario)
        {
            DocsPaVO.Spedizione.StatoSpedizioneDocumento statoSpedizione = GetStatoSpedizione(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.ErroreInSpedizione);

            if (this.IsDestinatarioOccasionale(destinatario))
            {
                statoSpedizione.Descrizione = "Impossibile spedire il documento ad un destinatario occasionale";
            }
            else if (string.IsNullOrEmpty(destinatario.email))
            {
                statoSpedizione.Descrizione = "Impossibile spedire il documento al destinatario per mancanza mail";
            }
            else if (destinatario.canalePref == null ||
                        (destinatario.canalePref != null &&
                         destinatario.canalePref.descrizione != "MAIL" &&
                         destinatario.canalePref.descrizione != "INTEROPERABILITA"))
            {
                // La mail non e' canale preferenziale della UO. Destinatario, quindi per noi non interoperante " + i + " scartato
                statoSpedizione.Descrizione = "La mail non è canale preferenziale della UO, destinatario non interoperante";
            }

            return statoSpedizione;
        }

        private async Task<bool> IsDestinatarioInterno(
                DocsPaVO.utente.Corrispondente destinatario,
                string idRegistro,
                DocsPaVO.utente.InfoUtente infoUtente)
        {

            if (!string.IsNullOrEmpty(destinatario.tipoIE) && destinatario.tipoIE == "I")
            {
                var isCorrispondenteInternoResult = await this._pi3DbContext.CorrGlobaliEntities
                    .AsNoTracking()
                    .Where(cg => cg.SYSTEM_ID == destinatario.systemId.AsLong())
                    .Select(cg =>
                        new
                        {
                            SYSTEM_ID = cg.SYSTEM_ID,
                            INTERNO = IPi3DbContextMappedFunctions.IsCorrispondenteInterno(destinatario.systemId.AsLong(), idRegistro.AsLong())
                        })
                    .FirstOrDefaultAsync();

                return isCorrispondenteInternoResult?.INTERNO == "1";
            }

            return false;
        }

        private async Task AssociaCaselleDestinatariEsterni(List<DocsPaVO.Spedizione.DestinatarioEsterno> destinatari)
        {
            foreach (var dest in destinatari)
            {
                var listCorr = dest.DatiDestinatari;

                foreach (var corr in listCorr
                    .Where(c => c.canalePref != null &&
                        (c.canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")
                            || c.canalePref.descrizione.ToUpper().Equals("MAIL")
                            || c.canalePref.descrizione.ToUpper().Equals("PORTALE"))))
                {
                    corr.Emails = await (from cg in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                         join m in this._pi3DbContext.MailCorrEsterniEntities.AsNoTracking() on cg.SYSTEM_ID equals m.ID_CORR
                                         where cg.SYSTEM_ID == corr.systemId.AsLong()
                                         select new DocsPaVO.utente.MailCorrispondente()
                                         {
                                             systemId = cg.SYSTEM_ID.ToString(),
                                             Email = m.VAR_EMAIL,
                                             Principale = m.VAR_PRINCIPALE,
                                             Note = m.VAR_NOTE
                                         })
                            .ToListAsync();
                }
            }
        }

        private async Task<System.Collections.Hashtable> DividiDestinatari(
            Corrispondente[] elencoDestinatari,
            string codRegistroMitt,
            string emailReg,
            SendDocumentResponse retValue,
            string idAmm)
        {
            var result = new System.Collections.Hashtable();
            var sendDocumentResponses = new List<DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse>();

            foreach (var destinatario in elencoDestinatari)
            {
                int i = 0;

                try
                {
                    if (destinatario.GetType() != typeof(DocsPaVO.utente.Corrispondente))
                    {
                        bool isMailPrefCorr;
                        bool isMailPref;
                        string emailCorr = null!;
                        string varCodiceAOO = string.Empty;
                        string varCodiceAMM = string.Empty;

                        DocsPaVO.utente.UnitaOrganizzativa uo = null!;
                        DocsPaVO.utente.RaggruppamentoFunzionale rf = null!;
                        string tipoIE = string.Empty;

                        // Booleano utilizzato per indicare se bisogna spedire per interoperabilità semplificata
                        bool isSimpInterop = false;
                        String varUrl = String.Empty;

                        if (destinatario.GetType() == typeof(DocsPaVO.utente.Utente))
                        {
                            var destinatarioUtente = (DocsPaVO.utente.Utente)destinatario;
                            tipoIE = destinatarioUtente.tipoIE; // RICAVO IL TIPO I/E

                            if (tipoIE == "I")
                            {
                                this._logger.LogDebug("Destinatario " + i + ": utente INTERNO");

                                if (string.IsNullOrWhiteSpace(destinatarioUtente.idPeople))
                                {
                                    destinatarioUtente.idPeople = await this._pi3DbContext.CorrGlobaliEntities
                                                .AsNoTracking()
                                                .Where(cg => cg.SYSTEM_ID == destinatarioUtente.systemId.AsLong())
                                                .Select(cg => cg.ID_PEOPLE.HasValue ? cg.ID_PEOPLE.ToString() : null)
                                                .FirstOrDefaultAsync();
                                }

                                var ruoliUtente = await GetListaRuoliUtente(destinatarioUtente.idPeople.AsLong());

                                if (ruoliUtente != null && ruoliUtente.Any())
                                {
                                    uo = ruoliUtente[0].uo;

                                    if ((!string.IsNullOrEmpty(uo.codiceAOO) && uo.codiceAOO != codRegistroMitt) ||
                                        (!string.IsNullOrEmpty(uo.email) && uo.email != emailReg))
                                    {
                                        foreach (DocsPaVO.utente.Ruolo ruolo in ruoliUtente)
                                        {
                                            var hasRegistro = ruoliUtente.Where(r =>
                                                r.registri.Any(reg =>
                                                    reg.codRegistro.Equals(codRegistroMitt, StringComparison.InvariantCultureIgnoreCase)))
                                                .Any();

                                            if (hasRegistro)
                                            {
                                                uo = (DocsPaVO.utente.UnitaOrganizzativa)
                                                     (await this._mediator.Send(new AddressbookGetCorrispondenteBySystemIdCommand()
                                                     {
                                                         SystemId = ruolo.uo.systemId,
                                                     }))
                                                     .Output;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    uo = null!;
                                }
                            }
                            else
                            {
                                this._logger.LogDebug("Destinatario " + i + ": utente ESTERNO");
                                uo = null!;
                            }

                            if (uo != null) // caso di utenti interni
                            {
                                emailCorr = uo.email;
                                varCodiceAOO = uo.codiceAOO;
                                varCodiceAMM = uo.codiceAmm;

                                destinatarioUtente.codiceAmm = varCodiceAMM;
                                destinatarioUtente.codiceAOO = varCodiceAOO;
                            }
                            else // caso di utenti esterni
                            {
                                emailCorr = destinatarioUtente.email;
                                varCodiceAMM = destinatarioUtente.codiceAmm;
                                varCodiceAOO = destinatarioUtente.codiceAOO;
                            }
                        }
                        else if (destinatario.GetType() == typeof(DocsPaVO.utente.Ruolo))
                        {
                            var destinatarioRuolo = (DocsPaVO.utente.Ruolo)destinatario;

                            tipoIE = destinatarioRuolo.tipoIE; // RICAVO IL TIPO I/E

                            if (destinatarioRuolo.tipoIE != null && destinatarioRuolo.tipoIE == "I")
                            {
                                this._logger.LogDebug("Destinatario " + i + ": ruolo INTERNO");
                                uo = destinatarioRuolo.uo;

                                if (uo != null)
                                {
                                    emailCorr = uo.email;
                                    varCodiceAOO = uo.codiceAOO;
                                    varCodiceAMM = uo.codiceAmm;
                                }
                            }
                            else
                            {
                                this._logger.LogDebug("Destinatario " + i + ": ruolo ESTERNO");
                                emailCorr = destinatarioRuolo.email;
                                varCodiceAOO = destinatarioRuolo.codiceAOO;
                                varCodiceAMM = destinatarioRuolo.codiceAmm;
                            }
                        }
                        else if (destinatario.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                        {
                            var destinatarioUO = (DocsPaVO.utente.UnitaOrganizzativa)destinatario;

                            tipoIE = destinatarioUO.tipoIE; // RICAVO IL TIPO I/E

                            this._logger.LogDebug("Destinatario " + i + ": unita' organizzativa");

                            uo = destinatarioUO;
                            emailCorr = destinatarioUO.email;
                            varCodiceAOO = destinatarioUO.codiceAOO;
                            varCodiceAMM = destinatarioUO.codiceAmm;

                            // Verifica se utilizzare l'interoperabilità semplificata
                            isSimpInterop = destinatarioUO.canalePref != null
                                && (destinatarioUO.canalePref.tipoCanale == InteroperabilityCode
                                    || destinatarioUO.canalePref.typeId == InteroperabilityCode)
                                && destinatarioUO.Url != null && destinatarioUO.Url.Count > 0 && !String.IsNullOrEmpty(destinatarioUO.Url[0].Url)
                                && Uri.IsWellFormedUriString(destinatarioUO.Url[0].Url, UriKind.Absolute);

                            varUrl = destinatarioUO.Url != null && destinatarioUO.Url.Count > 0 ? destinatarioUO.Url[0].Url : String.Empty;
                        }
                        else if (destinatario.GetType() == typeof(DocsPaVO.utente.RaggruppamentoFunzionale))
                        {
                            var destinatarioRF = (DocsPaVO.utente.RaggruppamentoFunzionale)destinatario;
                            rf = destinatarioRF;

                            tipoIE = destinatarioRF.tipoIE;

                            this._logger.LogDebug("Destinatario " + i + ": unita' organizzativa");

                            emailCorr = destinatarioRF.email;
                            varCodiceAOO = destinatarioRF.codiceAOO;
                            varCodiceAMM = destinatarioRF.codiceAmm;

                            // Verifica se utilizzare l'interoperabilità semplificata per l'RF
                            isSimpInterop = destinatarioRF.canalePref != null &&
                                (destinatarioRF.canalePref.tipoCanale == InteroperabilityCode
                                                    || destinatarioRF.canalePref.typeId == InteroperabilityCode)
                                                    && destinatarioRF.Url != null && destinatarioRF.Url.Count > 0
                                                    && !String.IsNullOrEmpty(destinatarioRF.Url[0].Url)
                                                    && Uri.IsWellFormedUriString(destinatarioRF.Url[0].Url, UriKind.Absolute);

                            varUrl = destinatarioRF.Url != null && destinatarioRF.Url.Count > 0 ? destinatarioRF.Url[0].Url : String.Empty;
                        }

                        // Se il canale preferito è interoperabilità semplificata, questa si può utilizzare solo se attiva
                        if (isSimpInterop)
                            isSimpInterop &= await this.IsEnabledSimplifiedInteroperability(idAmm);

                        if (tipoIE.Equals("E"))
                        {
                            isMailPref = false;
                            if (destinatario.GetType() != typeof(DocsPaVO.utente.Utente)) //corrispondente di tipo uo/ruolo/rf
                                if (uo != null)
                                    isMailPref = await this.IsMailPreferred(uo);
                                else
                                    isMailPref = await this.IsMailPreferred(rf);

                            if (!isMailPref && destinatario.GetType() != typeof(DocsPaVO.utente.Utente))
                            {
                                //se canale preferenziale non interop, verifico il mezzo di spedizione
                                if (uo != null)
                                    isMailPref = (uo.canalePref.descrizione.ToUpper().Equals("MAIL") || uo.canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) ? true : false;
                                else if (rf != null)
                                    isMailPref = (rf.canalePref.descrizione.ToUpper().Equals("MAIL") || rf.canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) ? true : false;
                            }

                            isMailPrefCorr = await this.IsMailPreferred((DocsPaVO.utente.Corrispondente)destinatario);

                            if (!isMailPrefCorr)
                            {
                                //se canale preferenziale non interop, verifico il mezzo di spedizione
                                var canalePreferenziale = destinatario?.canalePref?.descrizione?.ToUpper();
                                isMailPrefCorr = canalePreferenziale == "MAIL"
                                    || canalePreferenziale == "INTEROPERABILITA"
                                    || canalePreferenziale == "PORTALE";
                            }
                        }
                        else
                        {
                            // per i corrisp interni nel caso di interoperabilità si suppone che il canale
                            // preferenziale sia la mail
                            isMailPref = true;
                            isMailPrefCorr = false;
                        }

                        string email = null!;

                        if (uo != null && uo.interoperante)
                        {
                            this._logger.LogDebug("UO interoperante");

                            if (isMailPref)
                            {
                                email = await this.GetMailAOOInteropByDest(uo);

                                this._logger.LogDebug("Email destinatario " + i + ": " + email);
                            }
                            else
                            {
                                this._logger.LogDebug("La mail non e' canale preferenziale della UO. Destinatario, quindi per noi non interoperante " + i + " scartato");
                                email = null!;
                            }
                        }
                        else
                        {
                            this._logger.LogDebug("UO nulla o non interoperante");

                            //IMPORTANTE: UTENTI E RUOLI INTERNI POSSONO INTEROPERARE SOLO SE APPARTENGONO AD UNA UO INTEROPERANTE
                            //IN QUESTO CASO SI PRENDE L'INDIRIZZO MAIL DELL'UTENTE SE QUESTO HA COME CANALE PREFERENZIALE LA MAIL 
                            //RUOLI; UTENTI interni all'amministrazione non devono essere inseriti nella DPA_T_CANALE_CORR
                            if (isMailPrefCorr)
                            {
                                email = emailCorr;
                                this._logger.LogDebug("Mail destinatario " + i + ": " + email);
                            }
                            else
                            {
                                this._logger.LogDebug("La mail non e' canale preferenziale del destinatario. Destinatario " + i + " scartato");
                                email = null!;
                            }
                        }

                        //aggiunta condizione per spedire solamente a quei destinatari che appartengono ad AOO differenti da quella del mittente
                        if ((!string.IsNullOrEmpty(varCodiceAOO) && varCodiceAOO != codRegistroMitt && tipoIE == "I") || (
                            !string.IsNullOrEmpty(email) &&
                            email != emailReg) ||
                            isSimpInterop)
                        {
                            if (email == null)
                                email = string.Empty;

                            // Se si è nel caso di spedizione per interoperabilità semplificata, la chiave è l'url
                            if (isSimpInterop)
                                email = varUrl;

                            if (result.ContainsKey(email))
                            {
                                ((System.Collections.ArrayList)result[email]).Add(destinatario);
                            }
                            else
                            {

                                var al = new System.Collections.ArrayList();
                                al.Add(destinatario);
                                result.Add(email, al);
                            }
                        }
                        else //da Scartare
                        {
                            bool interop_no_mail = false;
                            if (!string.IsNullOrEmpty(destinatario.email))
                                email = destinatario.email;

                            var r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse
                                (email, true, "KO");

                            r.Destinatari = new Corrispondente[1] { destinatario };

                            if (varCodiceAOO == codRegistroMitt)
                                r.SendErrorMessage = "Destinatario Non Interoperante, poichè appartenente alla stessa AOO";

                            if (!interop_no_mail || varCodiceAOO == codRegistroMitt)
                            {
                                if (email != emailReg)
                                    r.SendErrorMessage = "Destinatario Non Interoperante, poichè appartenente alla stessa AOO";
                                else
                                    if (emailReg != null && emailReg == "")
                                    r.SendErrorMessage = "Attenzione, AOO mittente non ha alcuna mail associata.";
                                r.SendSucceded = false;
                                r.MailNonInteroperante = true;
                            }
                            else
                            {
                                var al = new System.Collections.ArrayList();
                                al.Add(destinatario);
                                result.Add(destinatario.codiceAOO + i.ToString(), al);
                            }

                            sendDocumentResponses.Add(r);

                        }
                    }
                    else //OCCASIONALI sono da Scartare
                    {
                        if (string.IsNullOrEmpty(destinatario.email))
                        {
                            var r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(destinatario.email, true, "OK");

                            r.Destinatari = new Corrispondente[1] { destinatario };
                            r.SendErrorMessage = "Destinatario Occasionale non interoperante";
                            r.SendSucceded = false;
                            r.MailNonInteroperante = true;

                            sendDocumentResponses.Add(r);
                        }
                        else
                        {
                            var al = new System.Collections.ArrayList();
                            al.Add(destinatario);
                            result.Add(destinatario.email, al);
                        }
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError(e.Message);
                }
                finally
                {
                    retValue.SendDocumentMailResponseList = sendDocumentResponses.ToArray();
                }

                i++;
            }

            return result;
        }

        private async Task<bool> IsEnabledSimplifiedInteroperability(string idTenant)
        {
            return await this._configurationService.GetValue<string>(idTenant, "INTEROP_SERVICE_ACTIVE") == "1";
        }

        private async Task<bool> IsMailPreferred(DocsPaVO.utente.Corrispondente corr)
        {
            return await (from cc in this._pi3DbContext.CanaleCorrEntities.AsNoTracking()
                          join dt in this._pi3DbContext.DocumentTypesEntities.AsNoTracking() on cc.ID_DOCUMENTTYPE equals dt.SYSTEM_ID
                          where (dt.CHA_TIPO_CANALE == "M" || dt.CHA_TIPO_CANALE == "I")
                          && cc.ID_CORR_GLOBALE == corr.systemId.AsLong()
                          && cc.CHA_PREFERITO == "1"
                          select cc.SYSTEM_ID)
                  .AnyAsync();
        }

        private async Task<string> GetMailAOOInteropByDest(DocsPaVO.utente.Corrispondente corrispondente)
        {
            var emailAoo = string.Empty;

            if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
            {
                switch (corrispondente.tipoIE)
                {
                    case "I":
                        emailAoo = await (this._pi3DbContext.RegistroEntities.AsNoTracking()
                            .Where(r => r.VAR_CODICE.ToUpper() == corrispondente.codiceAOO.ToUpper()
                                    && r.ID_AMM == corrispondente.idAmministrazione.AsLong())
                            .Select(r => r.VAR_EMAIL_REGISTRO))
                            .FirstOrDefaultAsync();

                        break;
                    case "E":
                        emailAoo = corrispondente.email;
                        break;
                }
            }

            return emailAoo!;
        }

        public async Task<List<Ruolo>> GetListaRuoliUtente(long idPeople)
        {
            var ruoli = new List<Ruolo>();

            try
            {
                var ruoliEntity = await this._pi3DbContext.PeopleGroupEntities.AsNoTracking()
                                    .Join(this._pi3DbContext.CorrGlobaliEntities, pg => pg.GROUPS_SYSTEM_ID, cg => cg.ID_GRUPPO, (pg, cg) => new { pg, cg })
                                    .Join(this._pi3DbContext.TipoRuoloEntities, j => j.cg.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (j, t) => new { j.pg, j.cg, t })
                                    .Where(j => j.pg.DTA_FINE == null && j.cg.DTA_FINE == null && j.pg.PEOPLE_SYSTEM_ID == idPeople)
                                    .Select(j => new
                                    {
                                        j.pg.PEOPLE_SYSTEM_ID,
                                        j.pg.CHA_PREFERITO,
                                        j.cg.SYSTEM_ID,
                                        j.cg.ID_GRUPPO,
                                        j.cg.ID_UO,
                                        j.cg.VAR_COD_RUBRICA,
                                        j.cg.ID_REGISTRO,
                                        j.cg.ID_AMM,
                                        j.cg.VAR_DESC_CORR,
                                        j.cg.CHA_RIFERIMENTO,
                                        j.cg.CHA_RESPONSABILE,
                                        j.cg.CHA_SEGRETARIO,
                                        j.t.NUM_LIVELLO,
                                        j.t.VAR_CODICE,
                                        j.t.VAR_DESC_RUOLO,
                                    })
                                    .OrderByDescending(c => c.CHA_PREFERITO != null)
                                    .ToListAsync();

                var corrGlobaliSystemId = await this._pi3DbContext
                    .CorrGlobaliEntities
                    .AsNoTracking()
                    .Where(c => c.ID_PEOPLE == idPeople)
                    .Select(c => c.SYSTEM_ID)
                    .FirstAsync();

                for (int r = 0; r < ruoliEntity.Count; r++)
                {
                    var ruolo = new Ruolo()
                    {
                        systemId = ruoliEntity[r].SYSTEM_ID.ToString(),
                        descrizione = ruoliEntity[r].VAR_DESC_CORR,
                        codice = ruoliEntity[r].VAR_CODICE,
                        livello = ruoliEntity[r].NUM_LIVELLO.ToString(),
                        idGruppo = ruoliEntity[r].ID_GRUPPO.ToString(),
                        tipoRuolo = new TipoRuolo()
                        {
                            codice = ruoliEntity[r].VAR_CODICE,
                            descrizione = ruoliEntity[r].VAR_DESC_RUOLO
                        },
                        codiceRubrica = ruoliEntity[r].VAR_COD_RUBRICA,
                        idRegistro = ruoliEntity[r].ID_REGISTRO.ToString(),
                        idAmministrazione = ruoliEntity[r].ID_AMM.ToString(),
                        tipoCorrispondente = "R",
                        Responsabile = ruoliEntity[r].CHA_RESPONSABILE == "1",
                        Segretario = ruoliEntity[r].CHA_SEGRETARIO == "1",
                        selezionato = ruoliEntity[r].CHA_PREFERITO == "1"
                    };

                    var uoEntity = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == ruoliEntity[r].ID_UO)
                        .Select(c => c)
                        .FirstAsync();

                    ruolo.uo = this._mapper.Map<UnitaOrganizzativa>(uoEntity);

                    var idParent = uoEntity.ID_PARENT;

                    while (idParent != null && idParent != 0)
                    {
                        var uoParentEntity = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idParent)
                        .Select(c => c)
                        .FirstAsync();

                        ruolo.uo.parent = this._mapper.Map<UnitaOrganizzativa>(uoParentEntity);

                        idParent = uoParentEntity.ID_PARENT;
                    }

                    var registriEntity = await this._pi3DbContext.RegistroEntities.AsNoTracking()
                        .Join(this._pi3DbContext.RuoloRegistroEntities, r => r.SYSTEM_ID, rr => rr.ID_REGISTRO, (r, rr) => new { r, rr })
                        .Where(j => j.r.CHA_RF == "0" && j.rr.ID_RUOLO_IN_UO == ruoliEntity[r].SYSTEM_ID)
                        .Select(j => new
                        {
                            REGISTRO = j.r,
                            j.rr.CHA_PREFERITO,
                            j.r.VAR_PREG
                        })
                        .OrderByDescending(j => j.CHA_PREFERITO != null)
                        .ThenBy(j => j.REGISTRO.VAR_CODICE)
                        .ThenBy(j => j.REGISTRO.VAR_DESC_REGISTRO)
                        .ToListAsync();

                    ruolo.registri = new Registro[registriEntity.Count];

                    for (int reg = 0; reg < registriEntity.Count; reg++)
                    {
                        ruolo.registri[reg] = (
                            this._mapper.Map<DocsPaVO.utente.Registro>(registriEntity[reg].REGISTRO));
                    }

                    var funzioniEntity = await this._pi3DbContext.FunzioneEntities.AsNoTracking()
                        .Join(this._pi3DbContext.TipoFunzioneEntities, f => f.ID_TIPO_FUNZIONE, t => t.SYSTEM_ID, (f, t) => new { f, t })
                        .Join(this._pi3DbContext.TipoFRuoloEntities, j => j.t.SYSTEM_ID, r => r.ID_TIPO_FUNZ, (j, r) => new { j.f, j.t, r })
                        .Where(j => j.r.ID_RUOLO_IN_UO == ruoliEntity[r].SYSTEM_ID)
                        .Select(j => new
                        {
                            j.f.SYSTEM_ID,
                            j.f.COD_FUNZIONE,
                            j.f.VAR_DESC_FUNZIONE,
                            j.f.ID_TIPO_FUNZIONE,
                            j.t.VAR_COD_TIPO,
                            j.t.VAR_DESC_TIPO_FUN
                        })
                        .ToListAsync();

                    ruolo.funzioni = new Funzione[funzioniEntity.Count()];

                    for (int i = 0; i < funzioniEntity.Count(); i++)
                    {
                        ruolo.funzioni[i] = new Funzione()
                        {
                            systemId = funzioniEntity[i].SYSTEM_ID.ToString(),
                            descrizione = funzioniEntity[i].VAR_DESC_FUNZIONE,
                            codice = funzioniEntity[i].COD_FUNZIONE,
                            idTipoFunzione = funzioniEntity[i].ID_TIPO_FUNZIONE.ToString(),
                            codTipoFunzione = funzioniEntity[i].VAR_COD_TIPO,
                            descTipoFunzione = funzioniEntity[i].VAR_DESC_TIPO_FUN
                        };
                    }

                    ruoli.Add(ruolo);
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                ruoli = null;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                ruoli = null;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return ruoli;
        }


        #endregion
    }
}
