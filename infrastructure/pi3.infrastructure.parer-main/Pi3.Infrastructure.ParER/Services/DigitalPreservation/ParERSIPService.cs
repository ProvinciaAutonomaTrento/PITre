// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Entities;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Metadati;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Entities;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Conservazione;
using Pi3.Core.Services.DigitalPreservation;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.ParER.Services.DigitalPreservation.Entities;
using Pi3.Infrastructure.ParER.Services.DigitalPreservation.ValueObjects;
using Pi3.Infrastructure.ParER.Services.Versamento;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pagination = Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories.Pagination;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.Intrinsics.X86;
using System.ComponentModel;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using System.IO;
using System.Reflection.Metadata;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.PAdES;
using ContatoreRepertorioFieldValue = Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ContatoreRepertorioFieldValue;
using System.Globalization;

namespace Pi3.Infrastructure.ParER.Services.DigitalPreservation
{
    public class ParERSIPService : ISIPService
    {
        public ParERSIPService(ILogger<ParERSIPService> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IOptions<ParEROptions> options,
            IDocumentoAmministrativoRepository docRepository,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
            IDocumentBlobRepository blobRepository,
            IVersamentoService versamentoService,
            IConfigurationService configurationService,
            ICAdESService cAdESService,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._docRepository = docRepository;
            this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
            this._blobRepository = blobRepository;
            this._versamentoService = versamentoService;
            this._configurationService = configurationService;
            this._dbContext = dbContext;
            this._cAdESService = cAdESService;

            this._options = options.Value;
        }

        public async Task<string> Get(string id)
        {
            var result = string.Empty;

            this._options.Ambiente = await this._configurationService.GetValue<string>("BE_VERSAMENTO_AMBIENTE");

            this._logger.LogInformation($"Richiesta recupero stato conservazione ParER ID={id}");

            // Verifichiamo che l'utente che sta operando abbia i diritti per reperire le informazioni sui documenti versati
            this._claimsPrincipalService.Current.AssertPi3Authorization("DO_SACER_RECUPERO");

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            aggregate = await this._docRepository.Get(idTenant.ToString(), id);

            var index = new Recupero
            {
                Versione = _options.VersionGet,
                Versatore = await this.GetVersatore(id.AsLong()),
                Chiave = await GetChiaveVersamento(aggregate)
            };

            var indexAsString = index.ToXmlString(true);

            this._logger.LogDebug($"Request recupero: {indexAsString}");

            try
            {
                var statoConservazione = await this._versamentoService.Get(
                    _options.VersionGet,
                    _options.UserName,
                    _options.Password,
                    indexAsString
                    );

                result = statoConservazione.ToXmlString(true);

                this._logger.LogDebug($"Response da ParER: {result}");
            }
            catch(Exception ex)
            {
                this._logger.LogError($"Errore nel recupero dello stato di conservazione per ID={id}: {ex.Message}");
                this._logger.LogDebug(ex.ToString());
            }

            return result;
        }

        public async Task<DigitalPreservationResult> Send(string id)
        {
            DigitalPreservationResult? result = default;

            this._logger.LogInformation($"Richiesta invio conservazione ParER ID={id}");

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                this._options.Ambiente = await this._configurationService.GetValue<string>("BE_VERSAMENTO_AMBIENTE");
                this._options.MultiAOO = (await this._configurationService.GetValue<string>(idTenant.ToString(), "BE_VERSAMENTO_MULTI_AOO", false, "0")) == "1";

                files = new List<StreamPart>();

                // Verifichiamo che l'utente che sta operando abbia i diritti per gestire l'invio in conservazione
                this._claimsPrincipalService.Current.AssertPi3Authorization("DO_SACER_VERSAMENTO");

                aggregate = await this._docRepository.Get(idTenant.ToString(), id, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadProfilesMetadata = true,
                        LoadClassifications = true,
                        LoadAllegati = true,
                        LoadAggregazioni = true,
                        LoadVersions = true,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false,
                        MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                    }
                });

               var version = aggregate.Versions.Last();

                var docPrincipaleAggregate = await this._blobRepository.Get(idTenant.ToString(), version.DocumentBlobRef.IdBlob);

                tipologiaUnitaDocumentaria = aggregate.GetTipologiaUnitaDocumentaria();

                this._logger.LogInformation($"Tipologia unita documentaria: {tipologiaUnitaDocumentaria.GetDescription()}");

                var metadati = new DocumentoAmministrativoInformaticoType().FromXmlString(aggregate.GetMetadatiXml());

                metadati.TipologiaDocumentale =  tipologiaUnitaDocumentaria.GetDescription();

                if(metadati.Classificazione == null)
                {
                    metadati.Classificazione = new ClassificazioneType
                    {
                        Descrizione = Resources.Resources.ND,
                        IndiceDiClassificazione = Resources.Resources.ND,
                        PianoDiClassificazione = Resources.Resources.ND
                    };
                }

                if(metadati.Agg == null)
                {
                    metadati.Agg = new IdAggType[1]
                    {
                        new IdAggType
                        {
                            IdAggregazione = Resources.Resources.ND,
                            TipoAggregazione = TipoAggregazioneType.Fascicolo
                        }
                    };
                }

                if(metadati.Soggetti.Any())
                {
                    var pai = metadati.Soggetti.Where(s => s.Item.GetType() == typeof(TipoSoggetto1Type))
                        .Select(s => s.Item)
                        .FirstOrDefault();

                    if((pai as TipoSoggetto1Type).PAI.IndirizziDigitaliDiRiferimento == null)
                        (pai as TipoSoggetto1Type).PAI.IndirizziDigitaliDiRiferimento = new string[1] { string.Empty };
                }

                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(metadati.ToXmlString(true));

                var indiceSIP = new UnitaDocumentaria
                { 
                    Intestazione = await this.GetIntestazioneUnitaDocumentaria(id.AsLong()),
                    Configurazione = this.GetConfigurazioneUnitaDocumentaria(),
                    ProfiloArchivistico = await this.GetProfiloArchivisticoUnitaDocumentaria(),
                    ProfiloUnitaDocumentaria = this.GetProfiloUnitaDocumentaria(),
                    DocumentiCollegati = await this.GetDocumentiCollegatiUnitaDocumentaria(),
                    DatiSpecificiMigrazione = null,
                    ProfiloNormativo = new ProfiloNormativoType()
                    {
                        versione = Resources.Resources.VersioneProfiloNormativo,
                        Any = xmlDocument.DocumentElement
                    }
                };

                this.SetDatiSpecifici(indiceSIP);

                var idComponente = new IdComponente(id);

                files.Add(new StreamPart(docPrincipaleAggregate.Stream, docPrincipaleAggregate.FileName, "application/octet-stream", idComponente.Current()));

                indiceSIP.DocumentoPrincipale = await this.GetDocumentoPrincipaleUnitaDocumentaria(docPrincipaleAggregate, idComponente.Next());

                var xmlAdditionalMetadata = await this.CreateAdditionalMetadata();

                List<DocumentoType>? listaAllegati = new List<DocumentoType>();
                List<DocumentoType>? listaAnnessi = new List<DocumentoType>();
                List<DocumentoType>? listaAnnotazioni = new List<DocumentoType>();


                //Annessi
                //Il primo annesso è il file XML con i metadati aggiuntivi dei documenti
                listaAnnessi.Add(this.GetAnnessoMetadati(idComponente.Current()));

                var additionalMetadataContent = Encoding.UTF8.GetBytes(xmlAdditionalMetadata);

                var ms = new MemoryStream(additionalMetadataContent);

                files.Add(new StreamPart(
                    ms,
                    Resources.Resources.MetadatiPITRENomeFile,
                    "application/octet-stream",
                    idComponente.Next()));

                listaAnnessi.First().StrutturaOriginale.Componenti[0].HashVersato = additionalMetadataContent.ComputeHashAsSha256String();

                // Allegati
                if (aggregate.Allegati?.Any() ?? false)
                {
                    var isFromInteropPiTre = await _dbContext.ProfileEntities.AsNoTracking().AnyAsync(p => p.SYSTEM_ID == aggregate.Id.AsLong() && p.CHA_TIPO_PROTO == "A" && p.CHA_INTEROP == "S");

                    //Allegati
                    var allegatiUtente = aggregate.Allegati
                        .Where(a => a.TipologiaAllegato == TipologieAllegatiEnum.Utente && !(isFromInteropPiTre && a.Descrizione.Value.ToUpper().Contains("ALLEGATO SEGNATURA")))
                        .OrderBy(a => a.IdDoc.Identiticativo)
                        .ToList();

                    if (tipologiaUnitaDocumentaria != TipologiaUnitaDocumentariaEnum.FatturaElettronica)
                    {
                        foreach (var a in allegatiUtente)
                            listaAllegati.Add(await this.GetAllegato(a, idComponente.Next(), TipoAllegatoConservazioneEnum.AllegatoUtente));

                    }
                    else
                    {
                        foreach (var a in allegatiUtente)
                        {
                            var descrizioneAllegato = a.Descrizione.Value.Replace(" ", string.Empty);
                            if (descrizioneAllegato != Resources.Resources.NotificaDecorrenzaTermini && descrizioneAllegato != Resources.Resources.NotificaEsitoCommittente)
                            {
                                DocumentoType documentoType = new DocumentoType();

                                if (a.IdDoc.FileName == filenameFattura)
                                {
                                    documentoType = await this.GetDocPrincipale(aggregate, idComponente.Next(), TipoAllegatoConservazioneEnum.AllegatoUtente);
                                    listaAnnessi.Add(documentoType);
                                }
                                else
                                {
                                    documentoType = await this.GetAllegato(a, idComponente.Next(), TipoAllegatoConservazioneEnum.AllegatoUtente);
                                    listaAllegati.Add(documentoType);
                                }
                            }
                            else
                            {
                                switch (descrizioneAllegato)
                                {
                                    case "NOTIFICADECORRENZATERMINI":
                                        listaAnnessi.Add(await this.GetAllegato(a, idComponente.Next(), TipoAllegatoConservazioneEnum.NotificaDecorrenzaTermini));
                                        break;
                                    case "NOTIFICAESITOCOMMITTENTE":
                                        listaAnnessi.Add(await this.GetAllegato(a, idComponente.Next(), TipoAllegatoConservazioneEnum.NotificaEsitoCommittente));
                                        break;
                                }
                            }
                        }
                    }

                    var altriAllegati = aggregate.Allegati.Where(a => a.TipologiaAllegato == TipologieAllegatiEnum.PEC
                                                                    || a.TipologiaAllegato == TipologieAllegatiEnum.PiTre
                                                                    || a.TipologiaAllegato == TipologieAllegatiEnum.SistemiEsterni)
                                                            .OrderBy(a => a.IdDoc.Identiticativo)
                                                            .ToList();
                    foreach (var a in altriAllegati)
                    {
                        var tipoAllegato = new TipoAllegatoConservazioneEnum();

                        switch (a.TipologiaAllegato)
                        {
                            case TipologieAllegatiEnum.PEC:
                                tipoAllegato = TipoAllegatoConservazioneEnum.AllegatoPEC;
                                break;
                            case TipologieAllegatiEnum.PiTre:
                                tipoAllegato = TipoAllegatoConservazioneEnum.AllegatoPITre;
                                break;
                            case TipologieAllegatiEnum.SistemiEsterni:
                                tipoAllegato = TipoAllegatoConservazioneEnum.AltriSistemi;
                                break;
                        }

                        listaAnnessi.Add(await this.GetAllegato(a, idComponente.Next(), tipoAllegato));
                    }

                    //Annotazione
                    var allegatoSegnatura = aggregate.Allegati.Where(a => a.TipologiaAllegato == TipologieAllegatiEnum.Utente && isFromInteropPiTre && a.Descrizione.Value.ToUpper().Contains("ALLEGATO SEGNATURA")).FirstOrDefault();
                    if (allegatoSegnatura is not null)
                        listaAnnotazioni.Add(await this.GetAllegato(allegatoSegnatura, idComponente.Next(), TipoAllegatoConservazioneEnum.Segnatura));

                    indiceSIP.Allegati = listaAllegati.Any() ? listaAllegati.ToArray() : null;
                    indiceSIP.NumeroAllegati = listaAllegati.Any() ? listaAllegati.Count.ToString() : null;

                    indiceSIP.Annotazioni = listaAnnotazioni.Any() ? listaAnnotazioni.ToArray() : null;
                    indiceSIP.NumeroAnnotazioni = listaAnnotazioni.Any() ? listaAnnotazioni.Count.ToString() : null;
                }

                indiceSIP.Annessi = listaAnnessi.ToArray();
                indiceSIP.NumeroAnnessi = listaAnnessi.Count.ToString();

                // Inserimento in coda
                var indexAsString = indiceSIP.ToXmlString(true, false, false, Encoding.UTF8);

                this._logger.LogDebug($"Indice SIP: {indexAsString}");
                
                var esito = await this._versamentoService.Send(
                        _options.Version,
                        _options.UserName,
                        _options.Password,
                        indexAsString,
                        files);

                result = new DigitalPreservationResult
                {
                    Status = esito.EsitoGenerale.CodiceEsito.AsDigitalPreservationStatus(),
                    RequestOutput = esito.ToXmlString(true)
                };

                this._logger.LogInformation($"Esito generale: {esito.EsitoGenerale.MessaggioErrore}");
                this._logger.LogInformation($"Codice esito: {esito.EsitoGenerale.CodiceEsito}");

                var versamentoEntity = await _dbContext.VersamentoEntities.Where(v => v.ID_PROFILE == aggregate.Id.AsLong()).FirstOrDefaultAsync();
                if (versamentoEntity != null)
                {
                    versamentoEntity.VAR_FILE_METADATI = xmlAdditionalMetadata;

                    if (esito.EsitoGenerale.CodiceEsito == ECEsitoExtType.WARNING)
                        versamentoEntity.CHA_WARNING = "1";

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }
                
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Errore nell'invio dell'unità documentaria: {ex.Message}");
                this._logger.LogDebug(ex.StackTrace);
                result = new DigitalPreservationResult
                {
                    Status = DigitalPreservationStatusEnum.InternalError
                };
            }

            return result;

        }

        #region Private members
        private readonly ILogger<ParERSIPService> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IPi3DbContext _dbContext;

        private readonly IDocumentoAmministrativoRepository _docRepository;
        private readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        private readonly IDocumentBlobRepository _blobRepository;
        private readonly IVersamentoService _versamentoService;
        private readonly IConfigurationService _configurationService;
        protected readonly ICAdESService _cAdESService;

        private ParEROptions _options;
        private TipologiaUnitaDocumentariaEnum? tipologiaUnitaDocumentaria;
        private DocumentoAmministrativo? aggregate;
        private List<StreamPart>? files;
        private string filenameFattura;

        private async Task<IntestazioneType> GetIntestazioneUnitaDocumentaria(long? idProfile)
        {
            return new IntestazioneType
            {
                Versione = _options!.Version,
                Versatore = await this.GetVersatore(idProfile),
                Chiave = await GetChiaveVersamento(aggregate),
                TipologiaUnitaDocumentaria = tipologiaUnitaDocumentaria!.GetDescription()
            };
        }

        private ConfigType GetConfigurazioneUnitaDocumentaria()
        {
            var tipoConservazione = (tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.FatturaElettronica ||
                                     tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.LottoDiFatture ||
                                     tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.FatturaAttiva ||
                                     tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.LottoDiFattureAttive) ?
                                     TipoConservazioneType.FISCALE : TipoConservazioneType.VERSAMENTO_ANTICIPATO;

            return new ConfigType
            {
                TipoConservazione = tipoConservazione,
                TipoConservazioneSpecified = true,
                ForzaConservazione = true,
                ForzaConservazioneSpecified = true,
                ForzaAccettazione = true,
                ForzaAccettazioneSpecified = true,
                ForzaCollegamento = true,
                ForzaCollegamentoSpecified = true,
                SimulaSalvataggioDatiInDB = false,
                SimulaSalvataggioDatiInDBSpecified = true
            };
        }

        private async Task<ProfiloArchivisticoType?> GetProfiloArchivisticoUnitaDocumentaria()
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var hasFascicolazioni = aggregate.Aggregazioni?.Any() ?? false;
            var hasClassificazioni = aggregate.Classifications?.Any() ?? false;

            if (!hasFascicolazioni && !hasClassificazioni) 
                return null;

            var projectEntities = await _dbContext.ProjectComponentEntities.AsNoTracking()
                .Join(_dbContext.ProjectEntities.AsNoTracking(),
                      pc => pc.PROJECT_ID,
                      p => p.SYSTEM_ID,
                      (pc, p) => new { pc, p })
                .Where(j => j.pc.LINK == aggregate.Id.AsLong())
                .Select(j => new
                {
                    j.p.SYSTEM_ID,
                    j.p.VAR_CODICE,
                    j.p.DESCRIPTION,
                    j.p.ID_FASCICOLO,
                    j.p.ID_PARENT
                })
                .ToListAsync();

            var idFascicolazionePrimaria = hasFascicolazioni ? aggregate.Aggregazioni.First().Id :  aggregate.Classifications.First().Id;

            var projectPrincipaleEntity = await _dbContext.ProjectEntities.AsNoTracking()
                       .Where(p => p.SYSTEM_ID == idFascicolazionePrimaria.AsLong())
                       .Select(p => new
                       {
                           p.DESCRIPTION,
                           p.VAR_CODICE,
                           p.ID_PARENT,
                           p.CHA_TIPO_FASCICOLO
                       })
                       .FirstAsync();

            var profiloArchivistico = new ProfiloArchivisticoType();

            profiloArchivistico.FascicoloPrincipale = new CamiciaFascicoloType
            {
                Classifica = await _dbContext.ProjectEntities.AsNoTracking().Where(p => p.SYSTEM_ID == projectPrincipaleEntity.ID_PARENT).Select(p => p.VAR_CODICE).FirstAsync(),
                Fascicolo = projectPrincipaleEntity.CHA_TIPO_FASCICOLO == "G" ? null : new FascicoloType()
                {
                    Identificativo = projectPrincipaleEntity.VAR_CODICE,
                    Oggetto = projectPrincipaleEntity.DESCRIPTION
                }
            };

            //Sottofascicolo
            var projectEntity = projectEntities.Where(p => p.ID_FASCICOLO == idFascicolazionePrimaria.AsLong()).First();
            if (projectEntity.ID_FASCICOLO != projectEntity.ID_PARENT)
            {
                profiloArchivistico.FascicoloPrincipale.SottoFascicolo = new FascicoloType
                {
                    Oggetto = projectEntity.DESCRIPTION,
                    Identificativo = $"{ await GetFolderTree(projectEntity.ID_PARENT.Value, projectEntity.ID_FASCICOLO.Value) }/{projectEntity.DESCRIPTION}"
                };
            }


           var fascicoliSecondari = new List<CamiciaFascicoloType>();
            foreach (var aggregazioneSecondaria in aggregate.Aggregazioni.Where(x => x.Id != idFascicolazionePrimaria))
            {
                var projectSecondarioEntity = await _dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == aggregazioneSecondaria.Id.AsLong())
                    .Select(p => new
                    {
                        p.DESCRIPTION,
                        p.VAR_CODICE,
                        p.ID_PARENT
                    })
                    .FirstAsync();

                var fascicoloSecondario = new CamiciaFascicoloType
                {
                    Classifica = await _dbContext.ProjectEntities.AsNoTracking().Where(p => p.SYSTEM_ID == projectSecondarioEntity.ID_PARENT).Select(p => p.VAR_CODICE).FirstAsync(),
                    Fascicolo = new FascicoloType()
                    {
                        Identificativo = projectSecondarioEntity.VAR_CODICE,
                        Oggetto = projectSecondarioEntity.DESCRIPTION
                    }
                };

                //Sottofascicolo
                projectEntity = projectEntities.Where(p => p.ID_FASCICOLO == aggregazioneSecondaria.Id.AsLong()).First();
                if (projectEntity.ID_FASCICOLO != projectEntity.ID_PARENT)
                {
                    profiloArchivistico.FascicoloPrincipale.SottoFascicolo = new FascicoloType
                    {
                        Oggetto = projectEntity.DESCRIPTION,
                        Identificativo = $"{await GetFolderTree(projectEntity.ID_PARENT.Value, projectEntity.ID_FASCICOLO.Value)}/{projectEntity.DESCRIPTION}"
                    };
                }

                fascicoliSecondari.Add(fascicoloSecondario);
            }

            foreach (var aggregazioneSecondaria in aggregate.Classifications.Where(x => x.Id != idFascicolazionePrimaria))
            {
                var projectSecondarioEntity = await _dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == aggregazioneSecondaria.Id.AsLong())
                    .Select(p => new
                    {
                        p.DESCRIPTION,
                        p.VAR_CODICE,
                        p.ID_PARENT
                    })
                    .FirstAsync();

                var fascicoloSecondario = new CamiciaFascicoloType
                {
                    Classifica = await _dbContext.ProjectEntities.AsNoTracking().Where(p => p.SYSTEM_ID == projectSecondarioEntity.ID_PARENT).Select(p => p.VAR_CODICE).FirstAsync()
                };

                fascicoliSecondari.Add(fascicoloSecondario);
            }

            profiloArchivistico.FascicoliSecondari = fascicoliSecondari.Any() ? fascicoliSecondari.ToArray() : null;

            return profiloArchivistico;
        }

        private ProfiloUnitaDocumentariaType GetProfiloUnitaDocumentaria()
        {
            var dateString = string.Empty;

            switch(tipologiaUnitaDocumentaria)
            {
                case TipologiaUnitaDocumentariaEnum.DocumentoProtocollato:
                    dateString = ((DatiRegistrazioneProtocollo)aggregate.DatiRegistrazione).DataProtocollazione.AsSIPIndexDateString();
                    break;

                case TipologiaUnitaDocumentariaEnum.DocumentoNonProtocollato:
                case TipologiaUnitaDocumentariaEnum.StampaRegistro:
                    dateString = aggregate!.CreationDate.AsSIPIndexDateString();
                    break;
            }

            return new ProfiloUnitaDocumentariaType
            {
                Oggetto = aggregate!.OggettoDelDocumento.Descrizione.Value,
                Data = dateString
            };
        }

        private async Task<DocumentoCollegatoTypeDocumentoCollegato[]?> GetDocumentiCollegatiUnitaDocumentaria()
        {
            var documentiCollegati = new List<DocumentoCollegatoTypeDocumentoCollegato>();

            if(aggregate.RelatedElements.Any())
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                foreach (var element in aggregate.RelatedElements)
                {
                    var relatedElementAggregate = await this._docRepository.Get(idTenant!, element.Id, new ILoadBehavior[1]
                    {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            BypassSecurityCheck = true,
                            LoadProfiles = true,
                            LoadProfilesMetadata = true,
                            LoadClassifications = false,
                            LoadAllegati = false,
                            LoadAggregazioni = false,
                            LoadVersions = false,
                            LoadPermissions = false,
                            LoadMittentiDestinatari = false,
                            LoadKeywords = false,
                            LoadNote = false
                        }
                    });

                    documentiCollegati.Add(new DocumentoCollegatoTypeDocumentoCollegato
                    {
                        ChiaveCollegamento = await GetChiaveVersamento(relatedElementAggregate),
                        DescrizioneCollegamento = tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.LottoDiFatture ?
                            Resources.Resources.DocumentiCollegatiLotto :
                            Resources.Resources.DocumentiCollegatiCatenaDocumentale
                    });
                }
            }

            return documentiCollegati.Any() ? documentiCollegati.ToArray() : null;
        }

        private async Task<DocumentoType> GetDocumentoPrincipaleUnitaDocumentaria(DocumentBlob blob, string idComponente)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            var listaSottoComponenti = new List<SottoComponenteType>();
            var sottocomponenti = 0;

            var tipoDocumento = TipoDocumentoEnum.DocumentoPrincipale.GetDescription();

            var tipologiaUnitaDocumentaria = aggregate.GetTipologiaUnitaDocumentaria();

            if (tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.LottoDiFatture)
                tipoDocumento = TipoDocumentoEnum.LottoDiFatture.GetDescription();

            if(tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.LottoDiFattureAttive)
                tipoDocumento = TipoDocumentoEnum.LottoDiFattureAttive.GetDescription();

            if (tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.FatturaElettronica || tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.FatturaAttiva)
                tipoDocumento = aggregate.GetFieldValueOrNull("TIPO ATTO");

            if (tipologiaUnitaDocumentaria == TipologiaUnitaDocumentariaEnum.VerbaleSinteticoDiSeduta)
                tipoDocumento = TipoDocumentoEnum.VerbaleSinteticoDiSeduta.GetDescription();

            var documentoPrincipale = new DocumentoType
            {
                IDDocumento = aggregate.Id,
                TipoDocumento = tipoDocumento,
                ProfiloDocumento = new ProfiloDocumentoType
                {
                    Descrizione = aggregate.OggettoDelDocumento.Descrizione.Value
                }
            };

            var currentVersion = aggregate.Versions.Last();

            documentoPrincipale.StrutturaOriginale = new StrutturaType
            {
                TipoStruttura = Resources.Resources.DocumentoPrincipaleTipoStruttura,
                Componenti = new ComponenteType[1] { new ComponenteType
                {
                    ID = idComponente,
                    OrdinePresentazione = "1",
                    TipoComponente = TipoComponenteEnum.Contenuto.GetDescription(),
                    TipoSupportoComponente = TipoSupportoType.FILE,
                    TipoSupportoComponenteSpecified = true,
                    DatiSpecifici = null
                } }
            };

            var componente = documentoPrincipale.StrutturaOriginale.Componenti.First();

            if (blob.Hash is null) blob.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

            if(tipologiaUnitaDocumentaria != TipologiaUnitaDocumentariaEnum.FatturaElettronica)
            {
                componente.NomeComponente = blob.FileName;
                componente.FormatoFileVersato = Path.GetExtension(blob.FileName).Substring(1);
                componente.HashVersato = Convert.ToBase64String(blob.Hash!);
                componente.IDComponenteVersato = currentVersion.Id;
            }
            else
            {
                //Fattura Elettronica
                if(!string.IsNullOrEmpty(currentVersion.DocumentBlobRef?.FileName))
                {
                    if(currentVersion.Name.Value == Resources.Resources.DocumentoConvertito && aggregate.Versions.Count > 1)
                    {
                        var previousVersion = aggregate.Versions[aggregate.Versions.Count - 2];
                        if (previousVersion.DocumentBlobRef is not null 
                            && (previousVersion.DocumentBlobRef.FileName.ToUpper().EndsWith("XML") || previousVersion.DocumentBlobRef.FileName.ToUpper().EndsWith("XML.P7M")))
                            currentVersion = previousVersion;
                    }

                    if(!await CtrlXMLFattura(currentVersion.DocumentBlobRef))
                    {
                        var allegatoFattura = aggregate.GetAllegatoFattura();
                        if(allegatoFattura is not null)
                        {
                            var allegatoFatturaAggregate = await this._docRepository.Get(idTenant, allegatoFattura.IdDoc.Identiticativo, new ILoadBehavior[1]
                                {
                                    new GetDocumentoAmministrativoLoadBehavior()
                                    {
                                        BypassSecurityCheck = true,
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

                            if (await CtrlXMLFattura(allegatoFatturaAggregate.Versions.Last().DocumentBlobRef))
                            {
                                filenameFattura = allegatoFattura.IdDoc.FileName;
                                componente.NomeComponente = allegatoFattura.IdDoc.FileName;
                                componente.FormatoFileVersato = Path.GetExtension(allegatoFattura.IdDoc.FileName).Substring(1);
                                componente.HashVersato = Convert.ToBase64String(allegatoFattura.IdDoc.ImprontaCrittograficaDelDocumento!.Impronta);
                                componente.IDComponenteVersato = allegatoFatturaAggregate.Versions.Last().Id;
                            }
                        }
                    }
                    else
                    {
                        componente.NomeComponente = currentVersion.DocumentBlobRef!.FileName;
                        componente.FormatoFileVersato = Path.GetExtension(currentVersion.DocumentBlobRef!.FileName).Substring(1);
                        componente.HashVersato = Convert.ToBase64String(currentVersion.DocumentBlobRef!.Hash);
                        componente.IDComponenteVersato = currentVersion.Id;
                    }
                }
            }

            if (currentVersion.DigitalSigned ?? false)
            {
                var riferimentoTemporale = await this.GetRiferimentoTemporale(aggregate, currentVersion);

                componente.RiferimentoTemporale = riferimentoTemporale.DataRiferimentoTemporale.Value;
                componente.RiferimentoTemporaleSpecified = true;
                componente.DescrizioneRiferimentoTemporale = riferimentoTemporale.TipoRiferimentoTemporale.GetDescription();
            }

            IdSottocomponente sottocomponente = new IdSottocomponente(idComponente);

            List<SottoComponenteType> listaSottocomponenti = new List<SottoComponenteType>();

            //Sottocomponente firma
            var sottocomponentiFirma = await GetSottocomponentiFirma(aggregate, currentVersion, sottocomponente);
            if (sottocomponentiFirma.Any())
                listaSottocomponenti.AddRange(sottocomponentiFirma);

            //Sottocomponente marca temporale
            var sottocomponentiMarcaTemporale = await GetSottocomponentiMarcaTemporale(aggregate, currentVersion, sottocomponente);
            if (sottocomponentiMarcaTemporale.Any())
                listaSottocomponenti.AddRange(sottocomponentiMarcaTemporale);

            if (listaSottocomponenti.Any())
                componente.SottoComponenti = listaSottocomponenti.ToArray();

            return documentoPrincipale;
        }

        private async Task<DocumentoType> GetAllegato(Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Allegato allegato, string idComponente, TipoAllegatoConservazioneEnum tipoAllegato)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            var aggregatoAttach = await this._docRepository.Get(idTenant, allegato.IdDoc.Identiticativo, new ILoadBehavior[1]
            {
                new GetDocumentoAmministrativoLoadBehavior()
                {
                    BypassSecurityCheck = true,
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

            var currentVersion = aggregatoAttach.Versions.Last();

            var isAcquired = currentVersion.DocumentBlobRef is not null;
            if (!isAcquired && tipoAllegato == TipoAllegatoConservazioneEnum.AllegatoUtente)
                tipoAllegato = TipoAllegatoConservazioneEnum.AllegatoUtenteNonAcquisito;

            var item = new DocumentoType
            {
                IDDocumento = allegato.IdDoc.Identiticativo,
                TipoDocumento = tipoAllegato.GetDescription(),
                ProfiloDocumento = new ProfiloDocumentoType
                {
                    Descrizione = allegato.Descrizione.Value
                },
                StrutturaOriginale = new StrutturaType
                {
                    TipoStruttura = Resources.Resources.DocumentoPrincipaleTipoStruttura,
                    Componenti = new ComponenteType[1]
                }
            };

            var componente = new ComponenteType
            {
                ID = idComponente,
                OrdinePresentazione = "1",
                TipoComponente = TipoComponenteEnum.Contenuto.GetDescription(),
                TipoSupportoComponente = isAcquired ? TipoSupportoType.FILE : TipoSupportoType.METADATI,
                TipoSupportoComponenteSpecified = true,
                DatiSpecifici = null
            };

            if(isAcquired)
            {
                var blob = await this._blobRepository.Get(idTenant.ToString(), currentVersion.DocumentBlobRef.IdBlob);
                blob.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                componente.IDComponenteVersato = currentVersion.Id.ToString();
                componente.NomeComponente = blob.FileName;
                componente.FormatoFileVersato = Path.GetExtension(blob.FileName).Substring(1);
                componente.HashVersato = Convert.ToBase64String(blob.Hash!);

                files.Add(new StreamPart(
                    blob!.Stream,
                    blob.FileName,
                    "application/octet-stream",
                    idComponente));
                }
            ;

            if (currentVersion.DigitalSigned ?? false)
            {
                var riferimentoTemporale = await this.GetRiferimentoTemporale(aggregatoAttach, currentVersion);

                componente.RiferimentoTemporale = riferimentoTemporale.DataRiferimentoTemporale.Value;
                componente.RiferimentoTemporaleSpecified = true;
                componente.DescrizioneRiferimentoTemporale = riferimentoTemporale.TipoRiferimentoTemporale.GetDescription();
            }

            IdSottocomponente sottocomponente = new IdSottocomponente(idComponente);

            List<SottoComponenteType> listaSottocomponenti = new List<SottoComponenteType>();
            var sottocomponentiFirma = await GetSottocomponentiFirma(aggregatoAttach, currentVersion, sottocomponente);
            if (sottocomponentiFirma.Any())
                listaSottocomponenti.AddRange(sottocomponentiFirma);

            var sottocomponentiMarcaTemporale = await GetSottocomponentiMarcaTemporale(aggregatoAttach, currentVersion, sottocomponente);
            if (sottocomponentiMarcaTemporale.Any())
                listaSottocomponenti.AddRange(sottocomponentiMarcaTemporale);

            if(listaSottocomponenti.Any())
                componente.SottoComponenti = listaSottocomponenti.ToArray();

            item.StrutturaOriginale.Componenti[0] = componente;

            return item;
        }

        private async Task<DocumentoType> GetDocPrincipale(DocumentoAmministrativo documentoAmministrativo, string idComponente, TipoAllegatoConservazioneEnum tipoAllegato)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            var currentVersion = documentoAmministrativo.Versions.Last();

            var isAcquired = currentVersion.DocumentBlobRef is not null;
            if (!isAcquired && tipoAllegato == TipoAllegatoConservazioneEnum.AllegatoUtente)
                tipoAllegato = TipoAllegatoConservazioneEnum.AllegatoUtenteNonAcquisito;

            var item = new DocumentoType
            {
                IDDocumento = documentoAmministrativo.Id,
                TipoDocumento = tipoAllegato.GetDescription(),
                ProfiloDocumento = new ProfiloDocumentoType
                {
                    Descrizione = documentoAmministrativo.OggettoDelDocumento.Descrizione.Value
                },
                StrutturaOriginale = new StrutturaType
                {
                    TipoStruttura = Resources.Resources.DocumentoPrincipaleTipoStruttura,
                    Componenti = new ComponenteType[1]
                }
            };

            var componente = new ComponenteType
            {
                ID = idComponente,
                OrdinePresentazione = "1",
                TipoComponente = TipoComponenteEnum.Contenuto.GetDescription(),
                TipoSupportoComponente = isAcquired ? TipoSupportoType.FILE : TipoSupportoType.METADATI,
                DatiSpecifici = null
            };

            if (isAcquired)
            {
                var blob = await this._blobRepository.Get(idTenant.ToString(), currentVersion.DocumentBlobRef.IdBlob);
                blob.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                componente.IDComponenteVersato = currentVersion.Id.ToString();
                componente.NomeComponente = blob.FileName;
                componente.FormatoFileVersato = Path.GetExtension(blob.FileName).Substring(1);
                componente.HashVersato = Convert.ToBase64String(blob.Hash!);

                files.Add(new StreamPart(
                    blob!.Stream,
                    blob.FileName,
                    "application/octet-stream",
                    idComponente));
            }

            if (currentVersion.DigitalSigned ?? false)
            {
                var riferimentoTemporale = await this.GetRiferimentoTemporale(documentoAmministrativo, currentVersion);

                componente.RiferimentoTemporale = riferimentoTemporale.DataRiferimentoTemporale.Value;
                componente.RiferimentoTemporaleSpecified = true;
                componente.DescrizioneRiferimentoTemporale = riferimentoTemporale.TipoRiferimentoTemporale.GetDescription();
            }

            IdSottocomponente sottocomponente = new IdSottocomponente(idComponente);

            List<SottoComponenteType> listaSottocomponenti = new List<SottoComponenteType>();
            var sottocomponentiFirma = await GetSottocomponentiFirma(documentoAmministrativo, currentVersion, sottocomponente);
            if (sottocomponentiFirma.Any())
                listaSottocomponenti.AddRange(sottocomponentiFirma);

            var sottocomponentiMarcaTemporale = await GetSottocomponentiMarcaTemporale(documentoAmministrativo, currentVersion, sottocomponente);
            if (sottocomponentiMarcaTemporale.Any())
                listaSottocomponenti.AddRange(sottocomponentiMarcaTemporale);

            if (listaSottocomponenti.Any())
                componente.SottoComponenti = listaSottocomponenti.ToArray();

            item.StrutturaOriginale.Componenti[0] = componente;

            return item;
        }


        private async Task<List<SottoComponenteType>> GetSottocomponentiFirma(DocumentoAmministrativo documentoAmministrativo, DocumentVersion version, IdSottocomponente sottocomponente)
        {
            List<SottoComponenteType> sottoComponenti = new List<SottoComponenteType>();
            var firmaElettronicaEntities = await _dbContext.FirmaElettronicaEntities.AsNoTracking()
                .Where(f => f.ID_DOCUMENTO == documentoAmministrativo.Id.AsLong()
                    && f.VERSION_ID == version.Id.AsLong()
                    && f.XML != null)
                .OrderBy(f => f.ID_FIRMA)
                .ToListAsync();

            if (firmaElettronicaEntities.Any())
            {
                sottoComponenti = new List<SottoComponenteType>();
                foreach (var firmaElettronica in firmaElettronicaEntities)
                {
                    sottocomponente.Next();
                    sottoComponenti.Add(new SottoComponenteType
                    {
                        ID = sottocomponente.Current(),
                        OrdinePresentazione = (sottocomponente.Id() + 1).ToString(),
                        TipoComponente = TipoComponenteEnum.FirmaElettronica.GetDescription(),
                        TipoSupportoComponente = TipoSupportoType.FILE,
                        TipoSupportoComponenteSpecified = true,
                        NomeComponente = $"{version.Id}_firma_elettronica_{sottocomponente.Id().ToString()}.xml",
                        FormatoFileVersato = "XML",
                        IDComponenteVersato = $"{version.Id}_{sottocomponente.Id()}",
                        DatiSpecifici = null
                        
                    });

                    files.Add(new StreamPart(new MemoryStream(Encoding.UTF8.GetBytes(firmaElettronica.XML)),
                        $"{version.Id}_firma_elettronica_{sottocomponente.Id()}.xml",
                        "application/octet-stream",
                        sottocomponente.Current()));
                }

            }

            return sottoComponenti;
        }

        private async Task<List<SottoComponenteType>> GetSottocomponentiMarcaTemporale(DocumentoAmministrativo documentoAmministrativo, DocumentVersion version, IdSottocomponente sottocomponente)
        {
            List<SottoComponenteType> sottoComponenti = new List<SottoComponenteType>();

            var timestampEntities = await _dbContext.TimestampDocEntities.AsNoTracking()
                   .Where(t => t.VERSION_ID == version.Id.AsLong()
                       && t.DOC_NUMBER == documentoAmministrativo.Id.AsLong())
                   .OrderByDescending(t => t.DTA_CREAZIONE)
                   .ToListAsync();

            if (timestampEntities.Any())
            {
                sottoComponenti = new List<SottoComponenteType>();
                foreach (var timestamp in timestampEntities)
                {
                    sottocomponente.Next();
                    sottoComponenti.Add(new SottoComponenteType
                    {
                        ID = sottocomponente.Current(),
                        OrdinePresentazione = (sottocomponente.Id() + 1).ToString(),
                        TipoComponente = TipoComponenteEnum.Marca.GetDescription(),
                        TipoSupportoComponente = TipoSupportoType.FILE,
                        TipoSupportoComponenteSpecified = true,
                        NomeComponente = $"{timestamp.DOC_NUMBER}_{timestamp.NUM_SERIE}.tsr",
                        FormatoFileVersato = "TSR",
                        IDComponenteVersato = $"{version.Id}_{timestamp.NUM_SERIE}",
                        DatiSpecifici = null

                    });

                    files.Add(new StreamPart(new MemoryStream(Encoding.UTF8.GetBytes(timestamp.TSR_FILE)),
                        $"{timestamp.DOC_NUMBER}_{timestamp.NUM_SERIE}.tsr",
                        "application/octet-stream",
                        sottocomponente.Current()));
                }
            }

            return sottoComponenti;
        }

        private DocumentoType GetAnnessoMetadati(string idComponente)
        {
            var item = new DocumentoType
            {
                IDDocumento = $"{aggregate.Id}_META",
                TipoDocumento = TipoAllegatoConservazioneEnum.MetadatiPITre.GetDescription(),
                ProfiloDocumento = new ProfiloDocumentoType
                {
                    Descrizione = Resources.Resources.MetadatiPITREDescrizione
                },
                StrutturaOriginale = new StrutturaType
                {
                    TipoStruttura = Resources.Resources.DocumentoPrincipaleTipoStruttura,
                    Componenti = new ComponenteType[1]
                }
            };

            item.StrutturaOriginale.Componenti[0] = new ComponenteType
            {
                ID = idComponente,
                OrdinePresentazione = "1",
                TipoComponente = TipoComponenteEnum.Contenuto.GetDescription(),
                TipoSupportoComponente = TipoSupportoType.FILE,
                TipoSupportoComponenteSpecified = true,
                NomeComponente = Resources.Resources.MetadatiPITRENomeFile,
                FormatoFileVersato = "xml",
                IDComponenteVersato = $"{aggregate.Id}_META",
                DatiSpecifici = null
            };

            return item;
        }

        private async Task<VersatoreType> GetVersatore(long? idProfile)
        {
            var amministrazione = aggregate!.Amministrazione.PAI.Amministrazione.Denominazione.Value;
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var codice = amministrazione.Split('-')[0].Trim();

            var ente = codice;
            var struttura = codice;

            if(idProfile.HasValue)
            {
                var customParamSacerEntity = await _dbContext.VersamentoEntities.AsNoTracking()
                    .Where(v => v.ID_PROFILE == idProfile)
                    .Select(v => new
                    {
                        v.VAR_CUSTOM_ENTE,
                        v.VAR_CUSTOM_STRUTTURA
                    })
                    .FirstOrDefaultAsync();
                if(customParamSacerEntity != null)
                {
                    ente = customParamSacerEntity.VAR_CUSTOM_ENTE ?? ente;
                    struttura = customParamSacerEntity.VAR_CUSTOM_STRUTTURA ?? struttura;
                }
            }

            if (_options!.MultiAOO 
                && await _dbContext.RegistroEntities.AsNoTracking().CountAsync(c => c.ID_AMM == idTenant && c.CHA_RF == "0") > 1)
            {
                switch (tipologiaUnitaDocumentaria)
                {
                    case TipologiaUnitaDocumentariaEnum.DocumentoProtocollato:
                        struttura = aggregate.Registro?.Codice;
                        break;
                    case TipologiaUnitaDocumentariaEnum.DocumentoRepertoriato:
                    case TipologiaUnitaDocumentariaEnum.FatturaElettronica:
                    case TipologiaUnitaDocumentariaEnum.LottoDiFatture:
                        if (aggregate.ConservaContatore())
                        {
                            struttura = aggregate.Registro?.Codice;
                            var contatore = aggregate.GetContatore();
                            var tipoContatore = await _dbContext.OggettiCustomEntities.AsNoTracking()
                                .Where(o => o.SYSTEM_ID == contatore.Id.AsLong())
                                .Select(o => o.CHA_TIPO_TAR)
                                .FirstOrDefaultAsync();

                            var registro = await _dbContext.RegistroEntities.AsNoTracking()
                                    .Where(r => r.SYSTEM_ID == ((Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ContatoreRepertorioFieldValue)contatore.Value).IdRegistro.AsLong())
                                    .Select(r => new
                                    {
                                        r.VAR_CODICE,
                                        r.ID_AOO_COLLEGATA
                                    })
                                    .FirstAsync();
                            if (tipoContatore == "A")
                            {
                                struttura = registro.VAR_CODICE;
                            }
                           
                            if(tipoContatore == "R")
                            {
                                struttura = await _dbContext.RegistroEntities.AsNoTracking()
                                    .Where(r => r.SYSTEM_ID == registro.ID_AOO_COLLEGATA)
                                    .Select(r => r.VAR_CODICE)
                                    .FirstOrDefaultAsync();
                            }
                        }
                        break;
                    case TipologiaUnitaDocumentariaEnum.StampaRegistro:
                        struttura = aggregate.Registro?.Codice;

                        long? idRegistro = null;
                        if(aggregate.DatiStampa.TipoStampa == TipologieStampaEnum.StampaRegistroProtocollo)
                        {
                            var datiStampaProtocollo = await _dbContext.StampaRegistriEntities.AsNoTracking()
                                .Where(s => s.DOCNUMBER == aggregate.Id.AsLong())
                                .FirstAsync();
                            idRegistro = datiStampaProtocollo.ID_REGISTRO;
                        }
                        
                        if(aggregate.DatiStampa.TipoStampa == TipologieStampaEnum.StampaRegistroRepertorio)
                        {
                            var datiStampaRepertorio = await _dbContext.StampaRepertoriEntities.AsNoTracking()
                                .Where(s => s.DOCNUMBER == aggregate.Id.AsLong())
                                .FirstAsync();
                            idRegistro = datiStampaRepertorio.REGISTRYID;

                            if(aggregate.DatiStampa.TipoContatore == TipologieContatoriRepertorioEnum.RF)
                            {
                                idRegistro = await _dbContext.RegistroEntities.AsNoTracking()
                                    .Where(r => r.SYSTEM_ID == idRegistro)
                                    .Select(r => r.ID_AOO_COLLEGATA)
                                    .FirstAsync();
                            }
                        }

                        if(idRegistro.HasValue)
                        {
                            struttura = await _dbContext.RegistroEntities.AsNoTracking()
                                .Where(r => r.SYSTEM_ID == idRegistro)
                                .Select(r => r.VAR_CODICE)
                                .FirstAsync();
                        }

                        break;
                    case TipologiaUnitaDocumentariaEnum.VerbaleSinteticoDiSeduta:
                        struttura = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Join(_dbContext.ProfileEntities.AsNoTracking(),
                                  c => c.SYSTEM_ID,
                                  p => p.ID_UO_CREATORE,
                                  (c, p) => new { c, p })
                            .Where(j => j.p.SYSTEM_ID == idProfile)
                            .Select(j => j.c.VAR_CODICE_AOO)
                            .FirstOrDefaultAsync();
                        break;
                }
            }

            return new VersatoreType
            {
                Ambiente = _options.Ambiente,
                Ente = ente,
                Struttura = struttura,
                UserID = _options.UserName
            };
        }

        private async Task<ChiaveType?> GetChiaveVersamento(DocumentoAmministrativo documentoAmministrativo)
        {
            ElementField contatore = null;
            var tipologia = string.Empty;
            var tipoContatore = string.Empty;
            var annoContatore = string.Empty;

            ChiaveType? chiaveType = new ChiaveType();

            switch (documentoAmministrativo.GetTipologiaUnitaDocumentaria())
            {
                case TipologiaUnitaDocumentariaEnum.DocumentoProtocollato:
                    var datiRegistrazioneProtocollo = documentoAmministrativo.DatiRegistrazione as DatiRegistrazioneProtocollo;

                    chiaveType.Numero = datiRegistrazioneProtocollo.NumeroProtocollo.ToString();
                    chiaveType.Anno = datiRegistrazioneProtocollo.DataProtocollazione.Value.Year.ToString();
                    chiaveType.TipoRegistro = string.Format(Resources.Resources.TipoRegistroDocumentoProtocollato, datiRegistrazioneProtocollo.CodiceRegistro.AsSIPIndexHeader());
                    break;
                case TipologiaUnitaDocumentariaEnum.DocumentoNonProtocollato:
                    chiaveType.Numero = documentoAmministrativo.Id;
                    chiaveType.Anno = documentoAmministrativo.CreationDate.Year.ToString();
                    chiaveType.TipoRegistro = Resources.Resources.TipoRegistroDocumentoNonProtocolllato;
                    break;
                case TipologiaUnitaDocumentariaEnum.StampaRegistro:
                    if (documentoAmministrativo.DatiStampa.TipoStampa == TipologieStampaEnum.StampaRegistroProtocollo)
                    {
                        this._logger.LogInformation($"Tipo stampa registro protocollo");
                        chiaveType.Numero = documentoAmministrativo.Id;
                        chiaveType.Anno = documentoAmministrativo.CreationDate.Year.ToString();
                        chiaveType.TipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroProtocollo, documentoAmministrativo.DatiStampa.CodiceRegistro);
                    }
                    else
                    {
                        this._logger.LogInformation($"Tipo stampa registro repertorio");

                        var infoStampaRepertorio = await _dbContext.StampaRepertoriEntities.AsNoTracking()
                                .Where(r => r.DOCNUMBER == documentoAmministrativo.Id.AsLong())
                                .FirstAsync();

                        this._logger.LogInformation($"Id repertorio: {infoStampaRepertorio.ID_REPERTORIO}");

                        tipologia = await _dbContext.OggettiCustomCompEntities.AsNoTracking()
                            .Join(_dbContext.TipoAttoEntities.AsNoTracking(),
                                  o => o.ID_TEMPLATE,
                                  t => t.SYSTEM_ID,
                                  (o, t) => new { o, t })
                            .Where(j => j.o.ID_OGG_CUSTOM == infoStampaRepertorio.ID_REPERTORIO)
                            .Select(j => j.t.VAR_DESC_ATTO)
                            .FirstOrDefaultAsync();

                        this._logger.LogInformation($"Tipologia: {tipologia}");

                        switch (documentoAmministrativo.DatiStampa.TipoContatore)
                        {
                            case TipologieContatoriRepertorioEnum.AOO:
                                this._logger.LogInformation($"Tipo contatore AOO");
                                chiaveType.Numero = documentoAmministrativo.Id;
                                chiaveType.Anno = documentoAmministrativo.CreationDate.Year.ToString();
                                chiaveType.TipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroRepertorioAOO, documentoAmministrativo.DatiStampa.CodiceRegistro!.AsSIPIndexHeader(), tipologia.AsSIPIndexHeader());

                                break;
                            case TipologieContatoriRepertorioEnum.RF:
                                this._logger.LogInformation($"Tipo contatore RF");
                                chiaveType.Numero = $"{documentoAmministrativo.DatiStampa.CodiceRegistro} - {documentoAmministrativo.Id}";
                                chiaveType.Anno = documentoAmministrativo.CreationDate.Year.ToString();
                                chiaveType.TipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroRepertorioRF, tipologia.AsSIPIndexHeader());

                                break ;
                            default:
                                this._logger.LogInformation($"Tipo contatore tipologia");
                                chiaveType.Numero = documentoAmministrativo.Id;
                                chiaveType.Anno = documentoAmministrativo.CreationDate.Year.ToString();
                                chiaveType.TipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroRepertorioRF, tipologia.AsSIPIndexHeader());

                                break;
                        }
                    }
                    break;
                case TipologiaUnitaDocumentariaEnum.DocumentoRepertoriato:
                case TipologiaUnitaDocumentariaEnum.LottoDiFatture:
                case TipologiaUnitaDocumentariaEnum.LottoDiFattureAttive:
                case TipologiaUnitaDocumentariaEnum.FatturaElettronica:
                case TipologiaUnitaDocumentariaEnum.FatturaAttiva:
                    if (documentoAmministrativo.GetTipologiaUnitaDocumentaria() == TipologiaUnitaDocumentariaEnum.DocumentoRepertoriato 
                        || documentoAmministrativo.ConservaContatore())
                    {
                        contatore = documentoAmministrativo.GetContatore();
                        tipoContatore = contatore.Metadata.GetValueOrDefault("CHA_TIPO_TAR");
                        tipologia = documentoAmministrativo.Profiles.FirstOrDefault()?.Name.Value.AsSIPIndexHeader();

                        chiaveType.Numero = contatore.Value.ToString();
                        chiaveType.Anno = contatore.Metadata.GetValueOrDefault("ANNO");
                        chiaveType.TipoRegistro = tipologia;

                        if (tipoContatore != "T")
                        {
                            var idAOORF = contatore.Metadata.GetValueOrDefault("ID_AOO_RF");
                            var registro = await _dbContext.RegistroEntities.AsNoTracking()
                                .Where(r => r.SYSTEM_ID == idAOORF.AsLong())
                                .Select(r => new
                                {
                                    r.VAR_CODICE,
                                    r.ID_AOO_COLLEGATA
                                })
                                .FirstAsync();

                            if (tipoContatore == "A")
                                chiaveType.TipoRegistro = registro.VAR_CODICE.AsSIPIndexHeader() + " - " + tipologia;

                            if (tipoContatore == "R")
                                chiaveType.Numero = registro.VAR_CODICE + " - " + ((Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ContatoreRepertorioFieldValue)contatore.Value).Value.ToString();
                        }
                    }
                    break;
                case TipologiaUnitaDocumentariaEnum.VerbaleSinteticoDiSeduta:
                    var codiceEnte = aggregate.GetFieldValueOrNull("ENTE ESPRESSO CON CODICE IPA");
                    var codiceOrgano = aggregate.GetFieldValueOrNull("ORGANO");
                    var numeroSeduta = aggregate.GetFieldValueOrNull("NUMERO SEDUTA");
                    var dataSeduta = aggregate.GetFieldValueOrNull("DATA SEDUTA");

                    chiaveType.Numero = $"{codiceEnte} - {codiceOrgano} - {numeroSeduta}";
                    chiaveType.Anno = dataSeduta.AsDateTime().Year.ToString();
                    chiaveType.TipoRegistro = Resources.Resources.TipoRegistroVerbaleSinteticoDiSeduta;
                    break;
            }

            return chiaveType;
        }


        private async Task SetDatiSpecifici(UnitaDocumentaria index)
        {
            var datiSpecifici = string.Empty;
            var versioneDatiSpecifici = string.Empty;

            switch(tipologiaUnitaDocumentaria)
            {
                case TipologiaUnitaDocumentariaEnum.DocumentoNonProtocollato:
                    datiSpecifici = await this.SetDatiSpecificiDocumentoNonProtocollato(index);
                    versioneDatiSpecifici = Resources.Resources.VersioneDatiSpecificiDocumentoNonProtocollato;
                    break;
                case TipologiaUnitaDocumentariaEnum.DocumentoProtocollato:
                    datiSpecifici = await this.SetDatiSpecificiDocumentoProtocollato(index);
                    versioneDatiSpecifici = Resources.Resources.VersioneDatiSpecificiDocumentoProtocollato;
                    break;
                case TipologiaUnitaDocumentariaEnum.StampaRegistro:
                    datiSpecifici = await this.SetDatiSpecificiStampaRegistro(index);
                    versioneDatiSpecifici = Resources.Resources.VersioneDatiSpecificiStampaRegistro;
                    break;
                case TipologiaUnitaDocumentariaEnum.DocumentoRepertoriato:
                    datiSpecifici = await this.SetDatiSpecificiDocumentoRepertoriato(index);
                    versioneDatiSpecifici = Resources.Resources.VersioneDatiSpecificiDocumentoRepertoriato;
                    break;
                case TipologiaUnitaDocumentariaEnum.FatturaElettronica:
                    datiSpecifici = await this.SetDatiSpecificiFatturaElettronica(index);
                    versioneDatiSpecifici = Resources.Resources.VersioneDatiSpecificiFatturaElettronica;
                    break;
                case TipologiaUnitaDocumentariaEnum.LottoDiFatture:
                    datiSpecifici = await this.SetDatiSpecificiLottoDiFatture(index);
                    versioneDatiSpecifici = Resources.Resources.VersioneDatiSpecificiLottoDiFatture;
                    break;
                case TipologiaUnitaDocumentariaEnum.FatturaAttiva:
                    datiSpecifici = await this.SetDatiSpecificiFatturaElettronicaAttiva(index);
                    versioneDatiSpecifici = Resources.Resources.VersioneDatiSpecificiFatturaAttiva;
                    break;
                case TipologiaUnitaDocumentariaEnum.LottoDiFattureAttive:
                    datiSpecifici = await this.SetDatiSpecificiLottoDiFattureAttive(index);
                    versioneDatiSpecifici = Resources.Resources.VersioneDatiSpecificiLottoDiFattureAttive;
                    break;
                case TipologiaUnitaDocumentariaEnum.VerbaleSinteticoDiSeduta:
                    datiSpecifici = await this.SetDatiSpecificiVerbaleSinteticoDiSeduta(index);
                    versioneDatiSpecifici = Resources.Resources.VersioneDatiSpecificiVerbaleSinteticoDiSeduta;
                    break;
            }

            var doc = new XmlDocument();
            doc.LoadXml(datiSpecifici);

            try
            {
                index.DatiSpecifici = new DatiSpecificiType
                {
                    VersioneDatiSpecifici = versioneDatiSpecifici,
                    Any = doc.DocumentElement.ChildNodes.Cast<XmlElement>().ToArray()
                };
            }
            catch(Exception)
            {
                index.DatiSpecifici = null;
            }

        }

        private async Task<string> SetDatiSpecificiDocumentoNonProtocollato(UnitaDocumentaria index)
        {
            var autore = (Autore)aggregate.Soggetti.First(x => x.Ruolo == "Autore");

            var ruoloCreatore = Resources.Resources.DatiSpecificiRuoloNonDefinito;
            var idRuoloCreatore = await _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                .Select(p => p.ID_RUOLO_CREATORE)
                .FirstAsync();

            if(idRuoloCreatore.HasValue)
            {
                ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idRuoloCreatore)
                    .Select(c => c.VAR_DESC_CORR)
                    .FirstOrDefaultAsync() ?? Resources.Resources.DatiSpecificiRuoloNonDefinito;
            }

            var datiSpecifici = new DatiSpecificiDocumentoNonProtocollato(
                aggregate.CreationDate.AsSIPIndexDateString(),
                aggregate.Autore?.ToString(),
                ruoloCreatore,
                autore.PF?.UOR.Denominazione.Value ?? Resources.Resources.DatiSpecificiStrutturaNonPresente);

            if (aggregate.Profiles.Any() && aggregate.ConservaTipologia())
            {
                datiSpecifici.TipologiaDocumentalePITre = aggregate.Profiles.First().Name.Value;
            }

            index.Intestazione.Chiave = new ChiaveType
            {
                Numero = aggregate.Id,
                Anno = aggregate.CreationDate.Year.ToString(),
                TipoRegistro = Resources.Resources.TipoRegistroDocumentoNonProtocolllato
            };

            index.ProfiloUnitaDocumentaria.Data = aggregate.CreationDate.AsSIPIndexDateString();

            return datiSpecifici.ToXmlString(true);
        }

        private async Task<string> SetDatiSpecificiDocumentoProtocollato(UnitaDocumentaria index)
        {
            var autore = (Autore)aggregate.Soggetti.First(x => x.Ruolo == "Autore");

            var ruoloCreatore = Resources.Resources.DatiSpecificiRuoloNonDefinito;
            var idRuoloCreatore = await _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                .Select(p => p.ID_RUOLO_CREATORE)
                .FirstAsync();

            if (idRuoloCreatore.HasValue)
            {
                ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idRuoloCreatore)
                    .Select(c => c.VAR_DESC_CORR)
                    .FirstOrDefaultAsync() ?? Resources.Resources.DatiSpecificiRuoloNonDefinito;
            }

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var codRegistro = await this._configurationService.GetValue<string>(idTenant.ToString(), "BE_VERSAMENTO_CUSTOM_REG_CODE") ?? aggregate.Registro?.Codice;
            var descRegistro = await this._configurationService.GetValue<string>(idTenant.ToString(), "BE_VERSAMENTO_CUSTOM_REG_DESC") ?? aggregate.Registro?.Descrizione?.Value;

            var datiRegistrazione = aggregate.DatiRegistrazione as DatiRegistrazioneProtocollo;

            var datiSpecifici = new DatiSpecificiDocumentoProtocollato(
                aggregate.CreationDate.AsSIPIndexDateString(),
                aggregate.Autore?.ToString(),
                ruoloCreatore,
                autore.PF?.UOR.Denominazione.Value ?? Resources.Resources.UONonPresente);

            datiSpecifici.NumeroProtocollo = datiRegistrazione.NumeroProtocollo.ToString();
            datiSpecifici.AnnoProtocollazione = datiRegistrazione.DataProtocollazione.Value.Year.ToString();
            datiSpecifici.TipoRegistroProtocollo = Resources.Resources.TipoRegistroProtocollo;
            datiSpecifici.CodiceRegistro = codRegistro;
            datiSpecifici.DescrizioneRegistro = descRegistro;
            datiSpecifici.SegnaturaProtocollo = aggregate.IdDoc?.Segnatura;

            switch(aggregate.TipologiaFlusso)
            {
                case TipologiaFlussoEnum.E:
                    datiSpecifici.TipoProtocollo = "A";
                    datiSpecifici.MezzoDiSpedizioneMittente = aggregate.MezzoSpedizione?.Descrizione?.ToString();
                    datiSpecifici.Mittente = aggregate.Mittente?.ToString();

                    if(aggregate.ProtocolloMittente is not null)
                    {
                        datiSpecifici.ProtocolloMittente = aggregate.ProtocolloMittente.Segnatura;
                        datiSpecifici.DataProtocolloMittente = aggregate.ProtocolloMittente.Data.AsSIPIndexDateString();
                        datiSpecifici.DataArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexDateString();
                        datiSpecifici.OraArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexTimeString();
                    }

                    break;
                case TipologiaFlussoEnum.U:
                case TipologiaFlussoEnum.I:
                    datiSpecifici.TipoProtocollo = aggregate.TipologiaFlusso == TipologiaFlussoEnum.U ? "P" : "I";
                    datiSpecifici.Mittente = aggregate.Mittente?.ToString();

                    if(aggregate.Destinatari is not null)
                    {
                        var destinatari = String.Join(";", aggregate.Destinatari.Select(d => d.ToString()));
                        if(destinatari.Length >= 4000)
                        {
                            destinatari = string.Format(Resources.Resources.DocumentoProtocollatoLunghezzaDestinatari, destinatari);
                            destinatari = destinatari.AsSIPIndexField()
                                .Replace("&", "&amp;")
                                .Substring(0, 3900)
                                .Replace("&amp;", "&");
                        }

                        datiSpecifici.Destinatari = destinatari;
                    }
                    break;
            }

            index.ProfiloUnitaDocumentaria.Data = datiRegistrazione.DataProtocollazione.AsSIPIndexDateString();

            index.Intestazione.Chiave = new ChiaveType
            {
                Numero = datiRegistrazione.NumeroProtocollo.ToString(),
                Anno = datiRegistrazione.DataProtocollazione.Value.Year.ToString(),
                TipoRegistro = string.Format(Resources.Resources.TipoRegistroDocumentoProtocollato, codRegistro.AsSIPIndexHeader())
            };

            return datiSpecifici.ToXmlString(true);
        }

        private async Task<string> SetDatiSpecificiDocumentoRepertoriato(UnitaDocumentaria index)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var autore = (Autore)aggregate.Soggetti.First(x => x.Ruolo == "Autore");

            var ruoloCreatore = Resources.Resources.DatiSpecificiRuoloNonDefinito;
            var idRuoloCreatore = await _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                .Select(p => p.ID_RUOLO_CREATORE)
                .FirstAsync();

            if (idRuoloCreatore.HasValue)
            {
                ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idRuoloCreatore)
                    .Select(c => c.VAR_DESC_CORR)
                    .FirstOrDefaultAsync() ?? Resources.Resources.DatiSpecificiRuoloNonDefinito;
            }

            var datiSpecifici = new DatiSpecificiDocumentoRepertoriato(
                aggregate.CreationDate.AsSIPIndexDateString(),
                aggregate.Autore?.ToString(),
                ruoloCreatore,
                autore.PF?.UOR.Denominazione.Value ?? Resources.Resources.DatiSpecificiStrutturaNonPresente);

            var contatore = aggregate!.GetContatore();            
            var tipologia = aggregate.Profiles.First().Name.Value.Trim();
            var numero = contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB");
            var tipoContatore = contatore.Metadata.GetValueOrDefault("CHA_TIPO_TAR");

            var tipoRegistro = tipologia;

            datiSpecifici.SegnaturaRepertorio = contatore.Metadata.GetValueOrDefault("VAR_SEGNATURA");
            datiSpecifici.TipologiaDocumentalePITre = aggregate.Profiles.First().Name.Value;

            if (tipoContatore != "T")
            {
                var registroEntity = await _dbContext.RegistroEntities.AsNoTracking().
                    Where(r => r.SYSTEM_ID == contatore.Metadata.GetValueOrDefault("ID_AOO_RF").AsLong())
                    .Select(r => new
                    {
                        r.VAR_CODICE,
                        r.VAR_DESC_REGISTRO
                    })
                    .FirstAsync();

                var codRegistro = await this._configurationService.GetValue<string>(idTenant.ToString(), "BE_VERSAMENTO_CUSTOM_REG_CODE") ?? registroEntity.VAR_CODICE;
                var descRegistro = await this._configurationService.GetValue<string>(idTenant.ToString(), "BE_VERSAMENTO_CUSTOM_REG_DESC") ?? registroEntity.VAR_DESC_REGISTRO;

                switch (tipoContatore)
                {
                    case "A":
                        tipoRegistro = $"{codRegistro} - {tipologia}";
                        datiSpecifici.CodiceRegistro_REP = codRegistro;
                        datiSpecifici.DescrizioneRegistro_REP = descRegistro;
                        break;
                    case "R":
                        numero = $"{codRegistro} - {contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB")}";
                        datiSpecifici.CodiceRF_REP = codRegistro;
                        datiSpecifici.DescrizioneRF_REP = descRegistro;
                        break;
                }
            }

            if (aggregate.DatiRegistrazione is not null
                && aggregate.DatiRegistrazione.IsRegistrato
                && aggregate.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo))
            {
                var datiRegistrazione = aggregate.DatiRegistrazione as DatiRegistrazioneProtocollo;

                if (datiSpecifici.GetType() == typeof(DatiSpecificiDocumentoRepertoriato))
                {
                    datiSpecifici.NumeroProtocollo = datiRegistrazione.NumeroProtocollo.ToString();
                    datiSpecifici.AnnoProtocollazione = datiRegistrazione.DataProtocollazione.Value.Year.ToString();
                    datiSpecifici.DataProtocollazione = datiRegistrazione.DataProtocollazione.Value.AsSIPIndexDateString();
                    datiSpecifici.TipoRegistroProtocollo = $"{aggregate.Registro?.Codice} - Protocollo";
                    datiSpecifici.CodiceRegistro_PROT = aggregate.Registro?.Codice;
                    datiSpecifici.DescrizioneRegistro_PROT = aggregate.Registro?.Descrizione?.Value;
                    datiSpecifici.SegnaturaProtocollo = aggregate.IdDoc?.Segnatura;

                    switch (aggregate.TipologiaFlusso)
                    {
                        case TipologiaFlussoEnum.E:
                            datiSpecifici.TipoProtocollo = "A";
                            datiSpecifici.MezzoDiSpedizioneMittente = aggregate.MezzoSpedizione?.Descrizione?.ToString();

                            if (aggregate.ProtocolloMittente is not null)
                            {
                                datiSpecifici.ProtocolloMittente = aggregate.ProtocolloMittente.Segnatura;
                                datiSpecifici.DataProtocolloMittente = aggregate.ProtocolloMittente.Data.AsSIPIndexDateString();
                                datiSpecifici.DataArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexDateString();
                                datiSpecifici.OraArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexTimeString();
                            }

                            break;
                        case TipologiaFlussoEnum.U:
                        case TipologiaFlussoEnum.I:
                            datiSpecifici.TipoProtocollo = aggregate.TipologiaFlusso == TipologiaFlussoEnum.U ? "P" : "I";
                            datiSpecifici.Mittente = aggregate.Mittente.ToString();

                            if (aggregate.Destinatari is not null)
                            {
                                var destinatari = String.Join(";", aggregate.Destinatari.Select(d => d.ToString()));
                                if (destinatari.Length >= 4000)
                                {
                                    destinatari = string.Format(Resources.Resources.DocumentoProtocollatoLunghezzaDestinatari, destinatari);
                                    destinatari = destinatari.AsSIPIndexField()
                                        .Replace("&", "&amp;")
                                        .Substring(0, 3900)
                                        .Replace("&amp;", "&");
                                }

                                datiSpecifici.Destinatari = destinatari;
                            }
                            break;
                    }
                }
            }

            index.ProfiloUnitaDocumentaria.Data = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime().AsSIPIndexDateString();

            index.Intestazione.Chiave = new ChiaveType
            {
                Numero = numero,
                Anno = contatore.Metadata.GetValueOrDefault("ANNO"),
                TipoRegistro = tipoRegistro.AsSIPIndexHeader()
            };

            return datiSpecifici.ToXmlString(true);
        }

        private async Task<string> SetDatiSpecificiStampaRegistro(UnitaDocumentaria index)
        {
            var autore = (Autore)aggregate.Soggetti.First(x => x.Ruolo == "Autore");
            var responsabile = (ResponsabileServizioProtocollo)aggregate.Soggetti.FirstOrDefault(x => x.Ruolo == "ResponsabileServizioProtocollo");

            var ruoloCreatore = Resources.Resources.DatiSpecificiRuoloNonDefinito;
            var idRuoloCreatore = await _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                .Select(p => p.ID_RUOLO_CREATORE)
                .FirstAsync();

            if (idRuoloCreatore.HasValue)
            {
                ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idRuoloCreatore)
                    .Select(c => c.VAR_DESC_CORR)
                    .FirstOrDefaultAsync() ?? Resources.Resources.DatiSpecificiRuoloNonDefinito;
            }

            var datiSpecifici = new DatiSpecificiStampaRegistro(
                aggregate.CreationDate.AsSIPIndexDateString(),
                aggregate.Autore?.ToString(),
                ruoloCreatore,
                autore.PF?.UOR?.Denominazione.Value ?? Resources.Resources.UONonPresente);

            datiSpecifici.FrequenzaDiStampa = Resources.Resources.DatiSpecificiFrequenzaStampa;
            datiSpecifici.RuoloResponsabileRegistro = responsabile?.ToString();

            var datiStampa = aggregate.DatiStampa!;

            if(datiStampa.TipoStampa == TipologieStampaEnum.StampaRegistroProtocollo)
            {
                var tipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroProtocollo, datiStampa.CodiceRegistro);

                datiSpecifici.TipoRegistro = tipoRegistro;
                
                datiSpecifici.PrimoElementoRegistrato = ((DatiRegistrazioneProtocollo)datiStampa.PrimoElementoStampato).NumeroProtocollo.ToString();
                datiSpecifici.DataPrimaRegistrazione = ((DatiRegistrazioneProtocollo)datiStampa.PrimoElementoStampato).DataProtocollazione.AsSIPIndexDateString();
                datiSpecifici.UltimoElementoRegistrato = ((DatiRegistrazioneProtocollo)datiStampa.UltimoElementoStampato).NumeroProtocollo.ToString();
                datiSpecifici.DataUltimaRegistrazione = ((DatiRegistrazioneProtocollo)datiStampa.UltimoElementoStampato).DataProtocollazione.AsSIPIndexDateString();
                datiSpecifici.CodiceRegistro_PROT = aggregate.Registro?.Codice;
                datiSpecifici.DescrizioneRegistro_PROT = aggregate.Registro?.Descrizione?.Value;

                index.Intestazione.Chiave = new ChiaveType
                {
                    Numero = aggregate.Id,
                    Anno = aggregate.CreationDate.Year.ToString(),
                    TipoRegistro = tipoRegistro
                };
            }
            else
            {
                var infoStampaRepertorio = await _dbContext.StampaRepertoriEntities.AsNoTracking()
                                .Where(r => r.DOCNUMBER == aggregate.Id.AsLong())
                                .FirstAsync();

                var tipologia = await _dbContext.OggettiCustomCompEntities.AsNoTracking()
                    .Join(_dbContext.TipoAttoEntities.AsNoTracking(),
                          o => o.ID_TEMPLATE,
                          t => t.SYSTEM_ID,
                          (o, t) => new { o, t })
                    .Where(j => j.o.ID_OGG_CUSTOM == infoStampaRepertorio.ID_REPERTORIO)
                    .Select(j => j.t.VAR_DESC_ATTO)
                    .FirstOrDefaultAsync();

                var tipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroRepertorioRF, tipologia.AsSIPIndexHeader());
                var numero = aggregate.Id;

                switch (datiStampa.TipoContatore)
                {
                    case TipologieContatoriRepertorioEnum.AOO:
                        var registroEntity = await _dbContext.RegistroEntities.AsNoTracking()
                            .Where(r => r.SYSTEM_ID == infoStampaRepertorio.REGISTRYID)
                            .Select(r => new
                            {
                                r.VAR_CODICE,
                                r.VAR_DESC_REGISTRO
                            })
                            .FirstAsync();

                        tipoRegistro = string.Format(Resources.Resources.TipoRegistroStampaRegistroRepertorioAOO, registroEntity.VAR_CODICE.AsSIPIndexHeader(), tipologia.AsSIPIndexHeader()); 

                        datiSpecifici.TipoRegistro = tipoRegistro;
                        datiSpecifici.CodiceRegistro_REP = registroEntity.VAR_CODICE;
                        datiSpecifici.DescrizioneRegistro_REP = registroEntity.VAR_DESC_REGISTRO;
                        break;
                    case TipologieContatoriRepertorioEnum.RF:
                        var rfEntity = await _dbContext.RegistroEntities.AsNoTracking()
                            .Where(r => r.SYSTEM_ID == infoStampaRepertorio.REGISTRYID)
                            .Select(r => new
                            {
                                r.VAR_CODICE,
                                r.VAR_DESC_REGISTRO
                            })
                            .FirstAsync();

                        numero = $"{rfEntity.VAR_CODICE} - {aggregate.Id}";

                        datiSpecifici.TipoRegistro = tipoRegistro;
                        datiSpecifici.CodiceRF_REP = rfEntity.VAR_CODICE;
                        datiSpecifici.DescrizioneRF_REP = rfEntity.VAR_DESC_REGISTRO;
                        break;
                }

                datiSpecifici.TipoRegistro = tipoRegistro;
                datiSpecifici.PrimoElementoRegistrato = ((DatiRegistrazioneRepertorio)datiStampa.PrimoElementoStampato).NumeroRegistrazione.ToString();
                datiSpecifici.DataPrimaRegistrazione = ((DatiRegistrazioneRepertorio)datiStampa.PrimoElementoStampato).DataRegistrazione.AsSIPIndexDateString();
                datiSpecifici.UltimoElementoRegistrato = ((DatiRegistrazioneRepertorio)datiStampa.UltimoElementoStampato).NumeroRegistrazione.ToString();
                datiSpecifici.DataUltimaRegistrazione = ((DatiRegistrazioneRepertorio)datiStampa.UltimoElementoStampato).DataRegistrazione.AsSIPIndexDateString();

                index.Intestazione.Chiave = new ChiaveType
                {
                    Numero = numero,
                    Anno = aggregate.CreationDate.Year.ToString(),
                    TipoRegistro = tipoRegistro
                };
            }

            index.ProfiloUnitaDocumentaria.Data = aggregate!.CreationDate.AsSIPIndexDateString();

            return datiSpecifici.ToXmlString(true);
        }

        private async Task<string> SetDatiSpecificiFatturaElettronica(UnitaDocumentaria index)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var autore = (Autore)aggregate.Soggetti.First(x => x.Ruolo == "Autore");

            var ruoloCreatore = Resources.Resources.DatiSpecificiRuoloNonDefinito;
            var idRuoloCreatore = await _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                .Select(p => p.ID_RUOLO_CREATORE)
                .FirstAsync();

            if (idRuoloCreatore.HasValue)
            {
                ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idRuoloCreatore)
                    .Select(c => c.VAR_DESC_CORR)
                    .FirstOrDefaultAsync() ?? Resources.Resources.DatiSpecificiRuoloNonDefinito;
            }

            var datiSpecifici = new DatiSpecificiFatturaElettronica(
                aggregate.CreationDate.AsSIPIndexDateString(),
                aggregate.Autore?.ToString(),
                ruoloCreatore,
                autore.PF?.UOR.Denominazione.Value ?? Resources.Resources.DatiSpecificiStrutturaNonPresente);

            datiSpecifici.TipologiaDocumentalePITre = aggregate.Profiles.First().Name.Value;

            var contatore = aggregate!.GetContatore();
            var tipologia = aggregate.Profiles.First().Name.Value.Trim();
            if (contatore != null && contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB") != null && aggregate.ConservaContatore())
            {
                var numero = contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB");            
                var tipoContatore = contatore.Metadata.GetValueOrDefault("CHA_TIPO_TAR");
                var tipoRegistro = tipologia;

                datiSpecifici.NumeroRepertorio = numero;
                datiSpecifici.DataRepertorio = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime().AsSIPIndexDateString();
                datiSpecifici.SegnaturaRepertorio = contatore.Metadata.GetValueOrDefault("VAR_SEGNATURA");
                if (tipoContatore != "T")
                {
                    var registroEntity = await _dbContext.RegistroEntities.AsNoTracking().
                        Where(r => r.SYSTEM_ID == contatore.Metadata.GetValueOrDefault("ID_AOO_RF").AsLong())
                        .Select(r => new
                        {
                            r.VAR_CODICE,
                            r.VAR_DESC_REGISTRO
                        })
                        .FirstAsync();

                    switch (tipoContatore)
                    {
                        case "A":
                            tipoRegistro = $"{registroEntity.VAR_CODICE} - {tipologia}";
                            datiSpecifici.CodiceRegistro_REP = registroEntity.VAR_CODICE;
                            datiSpecifici.DescrizioneRegistro_REP = registroEntity.VAR_DESC_REGISTRO;
                            break;
                        case "R":
                            numero = $"{registroEntity.VAR_CODICE} - {contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB")}";
                            datiSpecifici.CodiceRF_REP = registroEntity.VAR_CODICE;
                            datiSpecifici.DescrizioneRF_REP = registroEntity.VAR_DESC_REGISTRO;
                            break;
                    }
                }

                index.ProfiloUnitaDocumentaria.Data = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime().AsSIPIndexDateString();
                index.Intestazione.Chiave = new ChiaveType
                {
                    Numero = numero,
                    Anno = contatore.Metadata.GetValueOrDefault("ANNO"),
                    TipoRegistro = tipoRegistro.AsSIPIndexHeader()
                };
            }

            datiSpecifici.NumeroEmissione = aggregate.GetFieldValueOrNull("NUMERO FATTURA");

            DateTime dateField;
            datiSpecifici.DataEmissione = aggregate.GetFieldValueOrNull("DATA EMISSIONE");
            if(!string.IsNullOrEmpty(datiSpecifici.DataEmissione) &&
                DateTime.TryParseExact(
                                datiSpecifici.DataEmissione,
                                new string[]
                                {
                                "M/d/yyyy h:mm:ss tt",
                                "M/d/yyyy h:mm tt",
                                "M/d/yyyy",
                                "MM/dd/yyyy h:mm:ss tt",
                                "MM/dd/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
                                "dd/MM/yyyy",
                                "yyyy-MM-dd HH:mm:ss",
                                "yyyy-MM-ddTHH:mm:ss",
                                "yyyyMMddHHmmss",
                                "yyyyMMdd"
                                },
                                CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out dateField))
            {
                datiSpecifici.DataEmissione = dateField.AsSIPIndexDateString();
            }

            datiSpecifici.DenominazioneMittente = aggregate.GetFieldValueOrNull("FORNITORE"); 
            datiSpecifici.PartitaIvaMittente = aggregate.GetFieldValueOrNull("COD. FORNITORE") ?? aggregate.GetFieldValueOrNull("PARTITA IVA FORNITORE");
            datiSpecifici.CodiceFiscaleMittente = aggregate.GetFieldValueOrNull("CODICE FISCALE FORNITORE");
            datiSpecifici.CUP = aggregate.GetFieldValueOrNull("CODICE CUP");
            datiSpecifici.CIG = aggregate.GetFieldValueOrNull("CODICE CIG"); 
            datiSpecifici.IdentificativoSdI = aggregate.GetFieldValueOrNull("IDENTIFICATIVO SDI");
            datiSpecifici.AliquotaIvaReverseCharge = aggregate.GetFieldValueOrNull("ALIQUOTAIVAREVERSECHARGE"); 
            datiSpecifici.IvaTotaleReverseCharge = aggregate.GetFieldValueOrNull("IVATOTALEREVERSECHARGE"); 

            if (aggregate.DatiRegistrazione is not null
                && aggregate.DatiRegistrazione.IsRegistrato
                && aggregate.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo))
            {
               var datiRegistrazione = aggregate.DatiRegistrazione as DatiRegistrazioneProtocollo;

                datiSpecifici.NumeroProtocollo = datiRegistrazione.NumeroProtocollo.ToString();
                datiSpecifici.AnnoProtocollazione = datiRegistrazione.DataProtocollazione.Value.Year.ToString();
                datiSpecifici.DataProtocollazione = datiRegistrazione.DataProtocollazione.Value.AsSIPIndexDateString();
                datiSpecifici.TipoRegistroProtocollo = $"{aggregate.Registro?.Codice} - Protocollo";
                datiSpecifici.CodiceRegistro_PROT = aggregate.Registro?.Codice;
                datiSpecifici.DescrizioneRegistro_PROT = aggregate.Registro?.Descrizione?.Value;
                datiSpecifici.SegnaturaProtocollo = aggregate.IdDoc?.Segnatura;

                switch (aggregate.TipologiaFlusso)
                {
                    case TipologiaFlussoEnum.E:
                        datiSpecifici.TipoProtocollo = "A";
                        datiSpecifici.MezzoDiSpedizioneMittente = aggregate.MezzoSpedizione?.Descrizione?.ToString();

                        if (aggregate.ProtocolloMittente is not null)
                        {
                            datiSpecifici.ProtocolloMittente = aggregate.ProtocolloMittente.Segnatura;
                            datiSpecifici.DataProtocolloMittente = aggregate.ProtocolloMittente.Data.AsSIPIndexDateString();
                            datiSpecifici.DataArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexDateString();
                            datiSpecifici.OraArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexTimeString();
                        }

                        break;
                    case TipologiaFlussoEnum.U:
                    case TipologiaFlussoEnum.I:
                        datiSpecifici.TipoProtocollo = aggregate.TipologiaFlusso == TipologiaFlussoEnum.U ? "P" : "I";
                        datiSpecifici.Mittente = aggregate.Mittente.ToString();

                        //if (aggregate.Destinatari is not null)
                        //{
                        //    var destinatari = String.Join(";", aggregate.Destinatari.Select(d => d.ToString()));
                        //    if (destinatari.Length >= 4000)
                        //    {
                        //        destinatari = string.Format(Resources.Resources.DocumentoProtocollatoLunghezzaDestinatari, destinatari);
                        //        destinatari = destinatari.AsSIPIndexField()
                        //            .Replace("&", "&amp;")
                        //            .Substring(0, 3900)
                        //            .Replace("&amp;", "&");
                        //    }

                        //    datiSpecifici.Destinatari = destinatari;
                        //}
                        break;
                }
            }

            return datiSpecifici.ToXmlString(true);
        }

        private async Task<string> SetDatiSpecificiLottoDiFatture(UnitaDocumentaria index)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var autore = (Autore)aggregate.Soggetti.First(x => x.Ruolo == "Autore");

            var ruoloCreatore = Resources.Resources.DatiSpecificiRuoloNonDefinito;
            var idRuoloCreatore = await _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                .Select(p => p.ID_RUOLO_CREATORE)
                .FirstAsync();

            if (idRuoloCreatore.HasValue)
            {
                ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idRuoloCreatore)
                    .Select(c => c.VAR_DESC_CORR)
                    .FirstOrDefaultAsync() ?? Resources.Resources.DatiSpecificiRuoloNonDefinito;
            }

            var datiSpecifici = new DatiSpecificiLottoDiFatture(
                aggregate.CreationDate.AsSIPIndexDateString(),
                aggregate.Autore?.ToString(),
                ruoloCreatore,
                autore.PF?.UOR.Denominazione.Value ?? Resources.Resources.DatiSpecificiStrutturaNonPresente);

            datiSpecifici.TipologiaDocumentalePITre = aggregate.Profiles.First().Name.Value;

            var contatore = aggregate!.GetContatore();
            var tipologia = aggregate.Profiles.First().Name.Value.Trim();
            if (contatore != null && contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB") != null && aggregate.ConservaContatore())
            {
                var numero = contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB");
                var tipoContatore = contatore.Metadata.GetValueOrDefault("CHA_TIPO_TAR");
                var tipoRegistro = tipologia;

                datiSpecifici.NumeroRepertorio = numero;
                datiSpecifici.DataRepertorio = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime().AsSIPIndexDateString();
                datiSpecifici.SegnaturaRepertorio = contatore.Metadata.GetValueOrDefault("VAR_SEGNATURA");
                if (tipoContatore != "T")
                {
                    var registroEntity = await _dbContext.RegistroEntities.AsNoTracking().
                        Where(r => r.SYSTEM_ID == contatore.Metadata.GetValueOrDefault("ID_AOO_RF").AsLong())
                        .Select(r => new
                        {
                            r.VAR_CODICE,
                            r.VAR_DESC_REGISTRO
                        })
                        .FirstAsync();

                    switch (tipoContatore)
                    {
                        case "A":
                            tipoRegistro = $"{registroEntity.VAR_CODICE} - {tipologia}";
                            datiSpecifici.CodiceRegistro_REP = registroEntity.VAR_CODICE;
                            datiSpecifici.DescrizioneRegistro_REP = registroEntity.VAR_DESC_REGISTRO;
                            break;
                        case "R":
                            numero = $"{registroEntity.VAR_CODICE} - {contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB")}";
                            datiSpecifici.CodiceRF_REP = registroEntity.VAR_CODICE;
                            datiSpecifici.DescrizioneRF_REP = registroEntity.VAR_DESC_REGISTRO;
                            break;
                    }
                }

                index.ProfiloUnitaDocumentaria.Data = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime().AsSIPIndexDateString();
                index.Intestazione.Chiave = new ChiaveType
                {
                    Numero = numero,
                    Anno = contatore.Metadata.GetValueOrDefault("ANNO"),
                    TipoRegistro = tipoRegistro.AsSIPIndexHeader()
                };
            }

            datiSpecifici.DenominazioneMittente = aggregate.GetFieldValueOrNull("FORNITORE");
            datiSpecifici.PartitaIvaMittente = aggregate.GetFieldValueOrNull("COD. FORNITORE") ?? aggregate.GetFieldValueOrNull("PARTITA IVA FORNITORE");
            datiSpecifici.CodiceFiscaleMittente = aggregate.GetFieldValueOrNull("CODICE FISCALE FORNITORE");
            datiSpecifici.IdentificativoSdI = aggregate.GetFieldValueOrNull("IDENTIFICATIVO SDI");

            if (aggregate.DatiRegistrazione is not null
                && aggregate.DatiRegistrazione.IsRegistrato
                && aggregate.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo))
            {
                var datiRegistrazione = aggregate.DatiRegistrazione as DatiRegistrazioneProtocollo;

                datiSpecifici.NumeroProtocollo = datiRegistrazione.NumeroProtocollo.ToString();
                datiSpecifici.AnnoProtocollazione = datiRegistrazione.DataProtocollazione.Value.Year.ToString();
                datiSpecifici.DataProtocollazione = datiRegistrazione.DataProtocollazione.Value.AsSIPIndexDateString();
                datiSpecifici.TipoRegistroProtocollo = $"{aggregate.Registro?.Codice} - Protocollo";
                datiSpecifici.CodiceRegistro_PROT = aggregate.Registro?.Codice;
                datiSpecifici.DescrizioneRegistro_PROT = aggregate.Registro?.Descrizione?.Value;
                datiSpecifici.SegnaturaProtocollo = aggregate.IdDoc?.Segnatura;

                switch (aggregate.TipologiaFlusso)
                {
                    case TipologiaFlussoEnum.E:
                        datiSpecifici.TipoProtocollo = "A";
                        datiSpecifici.MezzoDiSpedizioneMittente = aggregate.MezzoSpedizione?.Descrizione?.ToString();

                        if (aggregate.ProtocolloMittente is not null)
                        {
                            datiSpecifici.ProtocolloMittente = aggregate.ProtocolloMittente.Segnatura;
                            datiSpecifici.DataProtocolloMittente = aggregate.ProtocolloMittente.Data.AsSIPIndexDateString();
                            datiSpecifici.DataArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexDateString();
                            datiSpecifici.OraArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexTimeString();
                        }

                        break;
                    case TipologiaFlussoEnum.U:
                    case TipologiaFlussoEnum.I:
                        datiSpecifici.TipoProtocollo = aggregate.TipologiaFlusso == TipologiaFlussoEnum.U ? "P" : "I";
                        datiSpecifici.Mittente = aggregate.Mittente.ToString();

                        if (aggregate.Destinatari is not null)
                        {
                            var destinatari = String.Join(";", aggregate.Destinatari.Select(d => d.ToString()));
                            if (destinatari.Length >= 4000)
                            {
                                destinatari = string.Format(Resources.Resources.DocumentoProtocollatoLunghezzaDestinatari, destinatari);
                                destinatari = destinatari.AsSIPIndexField()
                                    .Replace("&", "&amp;")
                                    .Substring(0, 3900)
                                    .Replace("&amp;", "&");
                            }

                            datiSpecifici.Destinatari = destinatari;
                        }
                        break;
                }
            }

            return datiSpecifici.ToXmlString(true);
        }

        private async Task<string> SetDatiSpecificiFatturaElettronicaAttiva(UnitaDocumentaria index)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var autore = (Autore)aggregate.Soggetti.First(x => x.Ruolo == "Autore");

            var ruoloCreatore = Resources.Resources.DatiSpecificiRuoloNonDefinito;
            var idRuoloCreatore = await _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                .Select(p => p.ID_RUOLO_CREATORE)
                .FirstAsync();

            if (idRuoloCreatore.HasValue)
            {
                ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idRuoloCreatore)
                    .Select(c => c.VAR_DESC_CORR)
                    .FirstOrDefaultAsync() ?? Resources.Resources.DatiSpecificiRuoloNonDefinito;
            }

            var datiSpecifici = new DatiSpecificiFatturaAttiva(
                aggregate.CreationDate.AsSIPIndexDateString(),
                aggregate.Autore?.ToString(),
                ruoloCreatore,
                autore.PF?.UOR.Denominazione.Value ?? Resources.Resources.DatiSpecificiStrutturaNonPresente);

            datiSpecifici.TipologiaDocumentalePITre = aggregate.Profiles.First().Name.Value;

            var contatore = aggregate!.GetContatore();
            var tipologia = aggregate.Profiles.First().Name.Value.Trim();
            if (contatore != null && contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB") != null && aggregate.ConservaContatore())
            {
                var numero = contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB");
                var tipoContatore = contatore.Metadata.GetValueOrDefault("CHA_TIPO_TAR");
                var tipoRegistro = tipologia;

                datiSpecifici.NumeroRepertorio = numero;
                datiSpecifici.DataRepertorio = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime().AsSIPIndexDateString();
                datiSpecifici.SegnaturaRepertorio = contatore.Metadata.GetValueOrDefault("VAR_SEGNATURA");
                if (tipoContatore != "T")
                {
                    var registroEntity = await _dbContext.RegistroEntities.AsNoTracking().
                        Where(r => r.SYSTEM_ID == contatore.Metadata.GetValueOrDefault("ID_AOO_RF").AsLong())
                        .Select(r => new
                        {
                            r.VAR_CODICE,
                            r.VAR_DESC_REGISTRO
                        })
                        .FirstAsync();

                    switch (tipoContatore)
                    {
                        case "A":
                            tipoRegistro = $"{registroEntity.VAR_CODICE} - {tipologia}";
                            datiSpecifici.CodiceRegistro_REP = registroEntity.VAR_CODICE;
                            datiSpecifici.DescrizioneRegistro_REP = registroEntity.VAR_DESC_REGISTRO;
                            break;
                        case "R":
                            numero = $"{registroEntity.VAR_CODICE} - {contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB")}";
                            datiSpecifici.CodiceRF_REP = registroEntity.VAR_CODICE;
                            datiSpecifici.DescrizioneRF_REP = registroEntity.VAR_DESC_REGISTRO;
                            break;
                    }
                }

                index.ProfiloUnitaDocumentaria.Data = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime().AsSIPIndexDateString();
                index.Intestazione.Chiave = new ChiaveType
                {
                    Numero = numero,
                    Anno = contatore.Metadata.GetValueOrDefault("ANNO"),
                    TipoRegistro = tipoRegistro.AsSIPIndexHeader()
                };
            }

            datiSpecifici.NumeroEmissione = aggregate.GetFieldValueOrNull("NUMERO FATTURA");
            DateTime dateField;
            datiSpecifici.DataEmissione = aggregate.GetFieldValueOrNull("DATA EMISSIONE");
            if (!string.IsNullOrEmpty(datiSpecifici.DataEmissione) &&
                DateTime.TryParseExact(
                                datiSpecifici.DataEmissione,
                                new string[]
                                {
                                "M/d/yyyy h:mm:ss tt",
                                "M/d/yyyy h:mm tt",
                                "M/d/yyyy",
                                "MM/dd/yyyy h:mm:ss tt",
                                "MM/dd/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
                                "dd/MM/yyyy",
                                "yyyy-MM-dd HH:mm:ss",
                                "yyyy-MM-ddTHH:mm:ss",
                                "yyyyMMddHHmmss",
                                "yyyyMMdd"
                                },
                                CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out dateField))
            {
                datiSpecifici.DataEmissione = dateField.AsSIPIndexDateString();
            }
            datiSpecifici.DenominazioneDestinatario = aggregate.GetFieldValueOrNull("CLIENTE"); 
            datiSpecifici.IdentificativoSdI = aggregate.GetFieldValueOrNull("IDENTIFICATIVO SDI");
            var pIVA = aggregate.GetFieldValueOrNull("PARTITA IVA CLIENTE"); 
            var cf = aggregate.GetFieldValueOrNull("CODICE FISCALE CLIENTE");
            if (!string.IsNullOrEmpty(pIVA) && pIVA != "999")
            {
                datiSpecifici.IdPaese = "IT";
                datiSpecifici.IdentificativoDestinatario = pIVA;
                datiSpecifici.TipoIdentificativoDestinatario = "PIVA";
            }
            else if (pIVA == "999")
            {
                datiSpecifici.IdPaese = "Extra_IT";
                datiSpecifici.IdentificativoDestinatario = cf;
                datiSpecifici.TipoIdentificativoDestinatario = "CF";
            }
            else
            {
                datiSpecifici.IdentificativoDestinatario = cf;
                datiSpecifici.TipoIdentificativoDestinatario = "CF";
            }

            if (aggregate.DatiRegistrazione is not null
                && aggregate.DatiRegistrazione.IsRegistrato
                && aggregate.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo))
            {
                var datiRegistrazione = aggregate.DatiRegistrazione as DatiRegistrazioneProtocollo;

                datiSpecifici.NumeroProtocollo = datiRegistrazione.NumeroProtocollo.ToString();
                datiSpecifici.AnnoProtocollazione = datiRegistrazione.DataProtocollazione.Value.Year.ToString();
                datiSpecifici.DataProtocollazione = datiRegistrazione.DataProtocollazione.Value.AsSIPIndexDateString();
                datiSpecifici.TipoRegistroProtocollo = $"{aggregate.Registro?.Codice} - Protocollo";
                datiSpecifici.CodiceRegistro_PROT = aggregate.Registro?.Codice;
                datiSpecifici.DescrizioneRegistro_PROT = aggregate.Registro?.Descrizione?.Value;
                datiSpecifici.SegnaturaProtocollo = aggregate.IdDoc?.Segnatura;

                switch (aggregate.TipologiaFlusso)
                {
                    case TipologiaFlussoEnum.E:
                        datiSpecifici.TipoProtocollo = "A";
                        datiSpecifici.MezzoDiSpedizioneMittente = aggregate.MezzoSpedizione?.Descrizione?.ToString();

                        if (aggregate.ProtocolloMittente is not null)
                        {
                            datiSpecifici.ProtocolloMittente = aggregate.ProtocolloMittente.Segnatura;
                            datiSpecifici.DataProtocolloMittente = aggregate.ProtocolloMittente.Data.AsSIPIndexDateString();
                            datiSpecifici.DataArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexDateString();
                            datiSpecifici.OraArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexTimeString();
                        }

                        break;
                    case TipologiaFlussoEnum.U:
                    case TipologiaFlussoEnum.I:
                        datiSpecifici.TipoProtocollo = aggregate.TipologiaFlusso == TipologiaFlussoEnum.U ? "P" : "I";
                        datiSpecifici.Mittente = aggregate.Mittente.ToString();

                        if (aggregate.Destinatari is not null)
                        {
                            var destinatari = String.Join(";", aggregate.Destinatari.Select(d => d.ToString()));
                            if (destinatari.Length >= 4000)
                            {
                                destinatari = string.Format(Resources.Resources.DocumentoProtocollatoLunghezzaDestinatari, destinatari);
                                destinatari = destinatari.AsSIPIndexField()
                                    .Replace("&", "&amp;")
                                    .Substring(0, 3900)
                                    .Replace("&amp;", "&");
                            }

                            datiSpecifici.Destinatari = destinatari;
                        }
                        break;
                }
            }

            return datiSpecifici.ToXmlString(true);
        }

        private async Task<string> SetDatiSpecificiLottoDiFattureAttive(UnitaDocumentaria index)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var autore = (Autore)aggregate.Soggetti.First(x => x.Ruolo == "Autore");

            var ruoloCreatore = Resources.Resources.DatiSpecificiRuoloNonDefinito;
            var idRuoloCreatore = await _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                .Select(p => p.ID_RUOLO_CREATORE)
                .FirstAsync();

            if (idRuoloCreatore.HasValue)
            {
                ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == idRuoloCreatore)
                    .Select(c => c.VAR_DESC_CORR)
                    .FirstOrDefaultAsync() ?? Resources.Resources.DatiSpecificiRuoloNonDefinito;
            }

            var datiSpecifici = new DatiSpecificiLottoDiFattureAttive(
                aggregate.CreationDate.AsSIPIndexDateString(),
                aggregate.Autore?.ToString(),
                ruoloCreatore,
                autore.PF?.UOR.Denominazione.Value ?? Resources.Resources.DatiSpecificiStrutturaNonPresente);

            datiSpecifici.TipologiaDocumentalePITre = aggregate.Profiles.First().Name.Value;

            var contatore = aggregate!.GetContatore();
            var tipologia = aggregate.Profiles.First().Name.Value.Trim();
            if (contatore != null && contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB") != null && aggregate.ConservaContatore())
            {
                var numero = contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB");
                var tipoContatore = contatore.Metadata.GetValueOrDefault("CHA_TIPO_TAR");
                var tipoRegistro = tipologia;

                datiSpecifici.NumeroRepertorio = numero;
                datiSpecifici.DataRepertorio = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime().AsSIPIndexDateString();
                datiSpecifici.SegnaturaRepertorio = contatore.Metadata.GetValueOrDefault("VAR_SEGNATURA");
                if (tipoContatore != "T")
                {
                    var registroEntity = await _dbContext.RegistroEntities.AsNoTracking().
                        Where(r => r.SYSTEM_ID == contatore.Metadata.GetValueOrDefault("ID_AOO_RF").AsLong())
                        .Select(r => new
                        {
                            r.VAR_CODICE,
                            r.VAR_DESC_REGISTRO
                        })
                        .FirstAsync();

                    switch (tipoContatore)
                    {
                        case "A":
                            tipoRegistro = $"{registroEntity.VAR_CODICE} - {tipologia}";
                            datiSpecifici.CodiceRegistro_REP = registroEntity.VAR_CODICE;
                            datiSpecifici.DescrizioneRegistro_REP = registroEntity.VAR_DESC_REGISTRO;
                            break;
                        case "R":
                            numero = $"{registroEntity.VAR_CODICE} - {contatore.Metadata.GetValueOrDefault("VALORE_OGGETTO_DB")}";
                            datiSpecifici.CodiceRF_REP = registroEntity.VAR_CODICE;
                            datiSpecifici.DescrizioneRF_REP = registroEntity.VAR_DESC_REGISTRO;
                            break;
                    }
                }

                index.ProfiloUnitaDocumentaria.Data = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime().AsSIPIndexDateString();
                index.Intestazione.Chiave = new ChiaveType
                {
                    Numero = numero,
                    Anno = contatore.Metadata.GetValueOrDefault("ANNO"),
                    TipoRegistro = tipoRegistro.AsSIPIndexHeader()
                };
            }

            datiSpecifici.NumeroEmissione = aggregate.GetFieldValueOrNull("NUMERO FATTURA");
            DateTime dateField;
            datiSpecifici.DataEmissione = aggregate.GetFieldValueOrNull("DATA EMISSIONE");
            if (!string.IsNullOrEmpty(datiSpecifici.DataEmissione) &&
                DateTime.TryParseExact(
                                datiSpecifici.DataEmissione,
                                new string[]
                                {
                                "M/d/yyyy h:mm:ss tt",
                                "M/d/yyyy h:mm tt",
                                "M/d/yyyy",
                                "MM/dd/yyyy h:mm:ss tt",
                                "MM/dd/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
                                "dd/MM/yyyy",
                                "yyyy-MM-dd HH:mm:ss",
                                "yyyy-MM-ddTHH:mm:ss",
                                "yyyyMMddHHmmss",
                                "yyyyMMdd"
                                },
                                CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out dateField))
            {
                datiSpecifici.DataEmissione = dateField.AsSIPIndexDateString();
            }
            datiSpecifici.DenominazioneDestinatario = aggregate.GetFieldValueOrNull("CLIENTE");
            datiSpecifici.IdentificativoSdI = aggregate.GetFieldValueOrNull("IDENTIFICATIVO SDI");
            var pIVA = aggregate.GetFieldValueOrNull("PARTITA IVA CLIENTE");
            var cf = aggregate.GetFieldValueOrNull("CODICE FISCALE CLIENTE");
            if (!string.IsNullOrEmpty(pIVA) && pIVA != "999")
            {
                datiSpecifici.IdPaese = "IT";
                datiSpecifici.IdentificativoDestinatario = pIVA;
                datiSpecifici.TipoIdentificativoDestinatario = "PIVA";
            }
            else
            {
                datiSpecifici.IdentificativoDestinatario = cf;
                datiSpecifici.TipoIdentificativoDestinatario = "CF";
            }

            if (aggregate.DatiRegistrazione is not null
                && aggregate.DatiRegistrazione.IsRegistrato
                && aggregate.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo))
            {
                var datiRegistrazione = aggregate.DatiRegistrazione as DatiRegistrazioneProtocollo;

                datiSpecifici.NumeroProtocollo = datiRegistrazione.NumeroProtocollo.ToString();
                datiSpecifici.AnnoProtocollazione = datiRegistrazione.DataProtocollazione.Value.Year.ToString();
                datiSpecifici.DataProtocollazione = datiRegistrazione.DataProtocollazione.Value.AsSIPIndexDateString();
                datiSpecifici.TipoRegistroProtocollo = $"{aggregate.Registro?.Codice} - Protocollo";
                datiSpecifici.CodiceRegistro_PROT = aggregate.Registro?.Codice;
                datiSpecifici.DescrizioneRegistro_PROT = aggregate.Registro?.Descrizione?.Value;
                datiSpecifici.SegnaturaProtocollo = aggregate.IdDoc?.Segnatura;

                switch (aggregate.TipologiaFlusso)
                {
                    case TipologiaFlussoEnum.E:
                        datiSpecifici.TipoProtocollo = "A";
                        datiSpecifici.MezzoDiSpedizioneMittente = aggregate.MezzoSpedizione?.Descrizione?.ToString();

                        if (aggregate.ProtocolloMittente is not null)
                        {
                            datiSpecifici.ProtocolloMittente = aggregate.ProtocolloMittente.Segnatura;
                            datiSpecifici.DataProtocolloMittente = aggregate.ProtocolloMittente.Data.AsSIPIndexDateString();
                            datiSpecifici.DataArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexDateString();
                            datiSpecifici.OraArrivo = aggregate.ProtocolloMittente.DataArrivo.AsSIPIndexTimeString();
                        }

                        break;
                    case TipologiaFlussoEnum.U:
                    case TipologiaFlussoEnum.I:
                        datiSpecifici.TipoProtocollo = aggregate.TipologiaFlusso == TipologiaFlussoEnum.U ? "P" : "I";
                        datiSpecifici.Mittente = aggregate.Mittente.ToString();

                        if (aggregate.Destinatari is not null)
                        {
                            var destinatari = String.Join(";", aggregate.Destinatari.Select(d => d.ToString()));
                            if (destinatari.Length >= 4000)
                            {
                                destinatari = string.Format(Resources.Resources.DocumentoProtocollatoLunghezzaDestinatari, destinatari);
                                destinatari = destinatari.AsSIPIndexField()
                                    .Replace("&", "&amp;")
                                    .Substring(0, 3900)
                                    .Replace("&amp;", "&");
                            }

                            datiSpecifici.Destinatari = destinatari;
                        }
                        break;
                }
            }

            return datiSpecifici.ToXmlString(true);
        }

        private async Task<string> SetDatiSpecificiVerbaleSinteticoDiSeduta(UnitaDocumentaria index)
        {
            var autore = (Autore)aggregate.Soggetti.First(x => x.Ruolo == "Autore");

            var datiSpecifici = new DatiSpecificiVerbaleSinteticoDiSeduta();

            datiSpecifici.Ente = aggregate.GetFieldValueOrNull("ENTE ESPRESSO CON CODICE IPA");
            datiSpecifici.Organo = aggregate.GetFieldValueOrNull("ORGANO");
            datiSpecifici.NumeroSeduta = aggregate.GetFieldValueOrNull("NUMERO SEDUTA");
            datiSpecifici.DataSeduta = aggregate.GetFieldValueOrNull("DATA SEDUTA");
            datiSpecifici.DescrizioneEnte = aggregate.GetFieldValueOrNull("DESCRIZIONE ENTE");

            index.ProfiloUnitaDocumentaria.Data = datiSpecifici.DataSeduta;
            index.Intestazione.Chiave = new ChiaveType
            {
                Numero = $"{datiSpecifici.Ente}-{datiSpecifici.Organo}-{datiSpecifici.NumeroSeduta}",
                Anno = datiSpecifici.DataSeduta.Split('-')[0],
                TipoRegistro = Resources.Resources.TipoRegistroVerbaliSinteticiDiSeduta
            };

            return datiSpecifici.ToXmlString(true);
        }

        private async Task<RiferimentoTemporale> GetRiferimentoTemporale(DocumentoAmministrativo documentoAmministrativo, DocumentVersion version)
        {
            // 1 - Ci sono marche
            var timestampEntities = await _dbContext.TimestampDocEntities.AsNoTracking()
                .Where(t => t.DOC_NUMBER == documentoAmministrativo.Id.AsLong()
                        && t.VERSION_ID == version.Id.AsLong())
                .OrderByDescending(t => t.DTA_CREAZIONE)
                .ToListAsync();

            if(timestampEntities.Any())
            {
                return new RiferimentoTemporale
                {
                    DataRiferimentoTemporale = timestampEntities[0].DTA_CREAZIONE,
                    TipoRiferimentoTemporale = TipoRiferimentoTemporaleEnum.MarcaTemporale
                };
            }

            // 2 - È protocollato
            if(aggregate!.DatiRegistrazione is not null && aggregate.DatiRegistrazione.IsRegistrato)
            {
                return new RiferimentoTemporale
                {
                    DataRiferimentoTemporale = (aggregate.DatiRegistrazione as DatiRegistrazioneProtocollo).DataProtocollazione,
                    TipoRiferimentoTemporale = TipoRiferimentoTemporaleEnum.DataProtocollazione
                };
            }

            // 3 - E' repertoriato
            var contatore = aggregate!.GetContatore();
            if(contatore is not null && aggregate!.ConservaContatore())
            {
                return new RiferimentoTemporale
                {
                    DataRiferimentoTemporale = contatore.Metadata.GetValueOrDefault("DTA_INS").AsDateTime(),
                    TipoRiferimentoTemporale = TipoRiferimentoTemporaleEnum.DataRepertoriazione
                };
            }

            return new RiferimentoTemporale
            {
                DataRiferimentoTemporale = DateTime.Now,
                TipoRiferimentoTemporale = TipoRiferimentoTemporaleEnum.DataVersamento
            };
        }

        private async Task<string> CreateAdditionalMetadata()
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var codRegistro = await this._configurationService.GetValue<string>(idTenant.ToString(), "BE_VERSAMENTO_CUSTOM_REG_CODE") ?? aggregate.Registro?.Codice;
            var descRegistro = await this._configurationService.GetValue<string>(idTenant.ToString(), "BE_VERSAMENTO_CUSTOM_REG_DESC") ?? aggregate.Registro?.Descrizione.Value;

            var profileEntity = await _dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                .Select(p => new
                {
                    p.SYSTEM_ID,
                    p.AUTHOR,
                    p.ID_RUOLO_CREATORE,
                    p.ID_PEOPLE_PROT,
                    p.ID_RUOLO_PROT
                })
                .FirstAsync();

            var author = await _dbContext.PeopleEntities.AsNoTracking()
                .Where(p => p.SYSTEM_ID == profileEntity.AUTHOR)
                .Select(p => new
                {
                    p.USER_ID,
                    p.FULL_NAME
                })
                .FirstAsync();

            string codiceRuoloCreatore = null;
            string descrizioneRuoloCreatore = null;
            long? idUOCreatore = null;
            if(profileEntity.ID_RUOLO_CREATORE.HasValue)
            {
                var ruoloCreatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == profileEntity.ID_RUOLO_CREATORE)
                    .Select(c => new
                    {
                        c.VAR_COD_RUBRICA,
                        c.VAR_DESC_CORR,
                        c.ID_UO
                    })
                    .FirstAsync();

                codiceRuoloCreatore = ruoloCreatore.VAR_COD_RUBRICA;
                descrizioneRuoloCreatore = ruoloCreatore.VAR_DESC_CORR;
                idUOCreatore = ruoloCreatore.ID_UO;
            }

            var pi3Metadata = new Documento
            {
                IdDocumento = aggregate.Id,
                DataCreazione = aggregate.CreationDate,
                Oggetto = aggregate.OggettoDelDocumento.Descrizione.Value,
                Tipo = (aggregate.TipologiaFlusso is not null) ? aggregate.DatiRegistrazione.IsRegistrato ? "Protocollato" : "Predisposto" : "Grigio"
            };

            if(aggregate.Versions.Last().DocumentBlobRef is not null
                && !string.IsNullOrEmpty(aggregate.Versions.Last().DocumentBlobRef.FileName))
            {
                var blobRef = aggregate.Versions.Last().DocumentBlobRef;

                pi3Metadata.File = new Entities.File
                {
                    Impronta = Convert.ToBase64String(blobRef.Hash!),
                    AlgoritmoHash = blobRef.HashName.ToString(),
                    Formato = Path.GetExtension(blobRef.FileName).Substring(1),
                    Dimensione =blobRef.FileSize.ToString()
                };
            }

            if (aggregate.TipologiaVisibilita == Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.TipologieVisibilitaEnum.Privata)
                pi3Metadata.LivelloRiservatezza = "Privato";

            var soggettoProduttore = new SoggettoProduttore
            {
                Amministrazione = new Entities.Amministrazione
                {
                    CodiceAmministrazione = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode),
                    DescrizioneAmministrazione = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantDescription)
                },
                Creatore = new Utente
                {
                    CodiceUtente = author.USER_ID,
                    DescrizioneUtente = author.FULL_NAME,
                    CodiceRuolo = codiceRuoloCreatore,
                    DescrizioneRuolo = descrizioneRuoloCreatore
                }
            };

            if(idUOCreatore.HasValue)
            {
                List<GerarchiaUO> gerarchiaUO = new List<GerarchiaUO>();
                var uo = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idUOCreatore)
                        .Select(c => new
                        {
                            c.VAR_COD_RUBRICA,
                            c.VAR_DESC_CORR,
                            c.NUM_LIVELLO,
                            c.ID_PARENT
                        })
                        .FirstAsync();

                gerarchiaUO.Add(new GerarchiaUO
                {
                    UnitàOrganizzativa = new UnitaOrganizzativa
                    {
                        CodiceUO = uo.VAR_COD_RUBRICA,
                        DescrizioneUO = uo.VAR_DESC_CORR,
                        Livello = uo.NUM_LIVELLO.ToString()
                    }
                });

                if(uo.NUM_LIVELLO > 1)
                {
                    int livello = Convert.ToInt32(uo.NUM_LIVELLO);
                    var idUoParent = uo.ID_PARENT;
                    for(int i = livello - 1; i > 0; i--)
                    {
                        uo = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idUoParent)
                        .Select(c => new
                        {
                            c.VAR_COD_RUBRICA,
                            c.VAR_DESC_CORR,
                            c.NUM_LIVELLO,
                            c.ID_PARENT
                        })
                        .FirstAsync();

                        gerarchiaUO.Add(new GerarchiaUO
                        {
                            UnitàOrganizzativa = new UnitaOrganizzativa
                            {
                                CodiceUO = uo.VAR_COD_RUBRICA,
                                DescrizioneUO = uo.VAR_DESC_CORR,
                                Livello = uo.NUM_LIVELLO.ToString()
                            }
                        });

                        idUoParent = uo.ID_PARENT;
                    }
                }

                gerarchiaUO.Reverse();
                soggettoProduttore.GerarchiaUO = gerarchiaUO.ToArray();
            }

            pi3Metadata.SoggettoProduttore = soggettoProduttore;

            //Registrazione
            if (aggregate.TipologiaFlusso is not null && aggregate.DatiRegistrazione is not null)
            {
                var datiRegistrazioneProtocollo = (DatiRegistrazioneProtocollo)aggregate.DatiRegistrazione;

                var registrazione = new Entities.Registrazione
                {
                    DataProtocollo = datiRegistrazioneProtocollo.DataProtocollazione?.AsDateFormat(),
                    OraProtocollo = datiRegistrazioneProtocollo.DataProtocollazione?.ToString("hh:mm:ss"),
                    NumeroProtocollo = datiRegistrazioneProtocollo.NumeroProtocollo.ToString(),
                    SegnaturaProtocollo = aggregate.IdDoc?.Segnatura,
                    TipoProtocollo = aggregate.GetTipoProtocollo(),
                    SegnaturaEmergenza = aggregate.ProtocolloEmergenza?.Segnatura,
                    NumeroProtocolloEmergenza = aggregate.ProtocolloEmergenza?.Segnatura,
                    DataProtocolloEmergenza = aggregate.ProtocolloEmergenza?.Data.ToString("dd/MM/yyyy"),
                    CodiceAOO = codRegistro,
                    DescrizioneAOO = descRegistro
                };

                registrazione.Protocollista = new Utente();
                if (profileEntity.ID_RUOLO_PROT.HasValue)
                {
                    var ruoloProtocollatore = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                       .Where(c => c.SYSTEM_ID == profileEntity.ID_RUOLO_CREATORE)
                       .Select(c => new
                       {
                           c.VAR_COD_RUBRICA,
                           c.VAR_DESC_CORR,
                           c.ID_UO
                       })
                       .FirstAsync();

                    registrazione.Protocollista.CodiceRuolo = ruoloProtocollatore.VAR_COD_RUBRICA;
                    registrazione.Protocollista.DescrizioneRuolo = ruoloProtocollatore.VAR_DESC_CORR;
                    registrazione.Protocollista.UOAppartenenza = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == ruoloProtocollatore.ID_UO).Select(c => c.VAR_COD_RUBRICA).FirstAsync();
                }

                if (profileEntity.ID_PEOPLE_PROT.HasValue)
                {
                    var peopleProt = await _dbContext.PeopleEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == profileEntity.AUTHOR)
                        .Select(p => new
                        {
                            p.USER_ID,
                            p.FULL_NAME
                        })
                        .FirstAsync();

                    registrazione.Protocollista.CodiceUtente = peopleProt.USER_ID;
                    registrazione.Protocollista.DescrizioneUtente = peopleProt.FULL_NAME;
                }

                var soggettiProtocollo = await _dbContext.DocArrivoParEntities.AsNoTracking()
                    .Join(_dbContext.CorrGlobaliEntities.AsNoTracking(),
                          p => p.ID_MITT_DEST,
                          c => c.SYSTEM_ID,
                          (p, c) => new { c, p })
                    .GroupJoin(_dbContext.DocumentTypesEntities.AsNoTracking(),
                        j => j.p.ID_DOCUMENTTYPES,
                        t => t.SYSTEM_ID,
                        (j, t) => new {j.p, j.c, t})
                    .SelectMany(j => j.t.DefaultIfEmpty(), 
                        (j, t) => new {j.p, j.c, t})
                    .Where(j => j.p.ID_PROFILE == profileEntity.SYSTEM_ID)
                    .Select(j => new 
                    {
                        j.p.CHA_TIPO_MITT_DEST,
                        j.c.VAR_COD_RUBRICA,
                        j.c.VAR_DESC_CORR,
                        j.c.VAR_EMAIL,
                        MEZZO_SPEDIZIONE = j.t.DESCRIPTION
                    })
                    .ToListAsync();

                List<Corrispondente> mittenti = new List<Corrispondente>();
                List<Corrispondente> destinatari = new List<Corrispondente>();
                switch (aggregate.TipologiaFlusso)
                {
                    case TipologiaFlussoEnum.E:
                        if (aggregate.ProtocolloMittente is not null)
                        {
                            registrazione.ProtocolloMittente = new Entities.ProtocolloMittente
                            {
                                Protocollo = aggregate.ProtocolloMittente.Segnatura,
                                Data = aggregate.ProtocolloMittente.Data.AsDateTimeFormat(),
                                MezzoSpedizione = aggregate.MezzoSpedizione?.Descrizione?.ToString(),
                                
                            };
                        }

                        var mittente = soggettiProtocollo.Where(s => s.CHA_TIPO_MITT_DEST == "M").First();
                        mittenti.Add(new Corrispondente
                        {
                            Codice = mittente.VAR_COD_RUBRICA,
                            Descrizione = mittente.VAR_DESC_CORR,
                            IndirizzoMail = mittente.VAR_EMAIL,
                            ProtocolloMittente = registrazione.ProtocolloMittente?.Protocollo,
                            DataProtocolloMittente = registrazione.ProtocolloMittente?.Data
                        });

                        foreach(var m in soggettiProtocollo.Where(s => s.CHA_TIPO_MITT_DEST == "MD").ToList())
                        {
                            mittenti.Add(new Corrispondente
                            {
                                Codice = m.VAR_COD_RUBRICA,
                                Descrizione = m.VAR_DESC_CORR,
                                IndirizzoMail = m.VAR_EMAIL,
                                ProtocolloMittente = registrazione.ProtocolloMittente?.Protocollo,
                                DataProtocolloMittente = registrazione.ProtocolloMittente?.Data
                            });
                        }

                        registrazione.Mittente = mittenti.ToArray();
                        break;
                    case TipologiaFlussoEnum.U:
                    case TipologiaFlussoEnum.I:
                        var mitt = soggettiProtocollo.Where(s => s.CHA_TIPO_MITT_DEST == "M").First();
                        mittenti.Add(new Corrispondente
                        {
                            Codice = mitt.VAR_COD_RUBRICA,
                            Descrizione = mitt.VAR_DESC_CORR,
                            IndirizzoMail = mitt.VAR_EMAIL,
                            ProtocolloMittente = registrazione.ProtocolloMittente?.Protocollo,
                            DataProtocolloMittente = registrazione.ProtocolloMittente?.Data
                        });

                        foreach (var d in soggettiProtocollo.Where(s => s.CHA_TIPO_MITT_DEST == "D" || s.CHA_TIPO_MITT_DEST == "C").ToList())
                        {
                            destinatari.Add(new Corrispondente
                            {
                                Codice = d.VAR_COD_RUBRICA,
                                Descrizione = d.VAR_DESC_CORR,
                                IndirizzoMail = d.VAR_EMAIL,
                                MezzoSpedizione = d.MEZZO_SPEDIZIONE
                            });
                        }

                        registrazione.Mittente = mittenti.ToArray();
                        registrazione.Destinatario = destinatari.ToArray();
                        break;
                }

                pi3Metadata.Registrazione = registrazione;             
            }

            //Contesto archivistico
            pi3Metadata.ContestoArchivistico = new ContestoArchivistico();
            if (aggregate.Aggregazioni is not null && aggregate.Aggregazioni.Count > 0)
            {
                List<Fascicolazione> fascicolazioni = new List<Fascicolazione>();
                var projectEntities = await _dbContext.ProjectComponentEntities.AsNoTracking()
                    .Join(_dbContext.ProjectEntities.AsNoTracking(),
                          pc => pc.PROJECT_ID,
                          p => p.SYSTEM_ID,
                          (pc, p) => new { pc, p })
                    .Where(j => j.pc.LINK == aggregate.Id.AsLong())
                    .Select(j => new
                    {
                        j.p.SYSTEM_ID,
                        j.p.VAR_CODICE,
                        j.p.DESCRIPTION,
                        j.p.ID_FASCICOLO,
                        j.p.ID_PARENT
                    })
                    .ToListAsync();

                foreach (var fascicolo in aggregate.Aggregazioni)
                {
                    var projectEntity = await _dbContext.ProjectEntities.AsNoTracking()
                           .Where(p => p.SYSTEM_ID == fascicolo.Id.AsLong())
                           .Select(p => new
                           {
                               p.DESCRIPTION,
                               p.VAR_CODICE,
                               p.ID_PARENT,
                               p.ID_TITOLARIO
                           })
                           .FirstAsync();

                    var titolario = await _dbContext.ProjectEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == projectEntity.ID_TITOLARIO)
                        .Select(p => p.DESCRIPTION)
                        .FirstAsync();

                    var fascicolazione = new Fascicolazione
                    {
                        CodiceFascicolo = projectEntity.VAR_CODICE,
                        DescrizioneFascicolo = projectEntity.DESCRIPTION,
                        TitolarioDiRiferimento = $"{titolario} - attivo al {DateTime.Now.ToString("dd/MM/yyyy")}"
                    };

                    var folderEntity = projectEntities.Where(p => p.ID_FASCICOLO == fascicolo.Id.AsLong()).First();
                    if (folderEntity.ID_FASCICOLO != folderEntity.ID_PARENT)
                    {
                        fascicolazione.CodiceSottoFascicolo = folderEntity.SYSTEM_ID.ToString();
                        fascicolazione.DescrizioneSottoFascicolo = folderEntity.DESCRIPTION;
                    }

                    fascicolazioni.Add(fascicolazione);
                }

                pi3Metadata.ContestoArchivistico.Fascicolazione = fascicolazioni.ToArray();
            }

            if (aggregate.Classifications is not null && aggregate.Classifications.Count > 0)
            {
                List<Classificazione> classifications = new List<Classificazione>();
                foreach(var classifica in aggregate.Classifications)
                {
                    var idTitolario = await _dbContext.ProjectEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == classifica.Id.AsLong())
                        .Select(p => p.ID_TITOLARIO)
                        .FirstAsync();

                    var titolario = await _dbContext.ProjectEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == idTitolario)
                        .Select(p => p.DESCRIPTION)
                        .FirstAsync();

                    classifications.Add(new Classificazione
                    {
                        CodiceClassificazione = classifica.Code,
                        TitolarioDiRiferimento = $"{titolario} - attivo al {DateTime.Now.ToString("dd/MM/yyyy")}"
                    });
                }
                pi3Metadata.ContestoArchivistico.Classificazione = classifications.ToArray();
            }

            if (aggregate.RelatedElements.Any())
            {
                var relatedElementAggregate = await this._docRepository.Get(idTenant.ToString(), aggregate.RelatedElements[0].Id, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        BypassSecurityCheck = true,
                        LoadProfiles = false,
                        LoadProfilesMetadata = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = false,
                        LoadKeywords = false,
                        LoadNote = false
                    }
                });

                var isProtocollato = (relatedElementAggregate.DatiRegistrazione as DatiRegistrazioneProtocollo) is not null && relatedElementAggregate.DatiRegistrazione.IsRegistrato;
                pi3Metadata.ContestoArchivistico.DocumentoCollegato = new DocumentoCollegato
                {
                    IdDocumento = relatedElementAggregate.Id,
                    DataCreazione = relatedElementAggregate.CreationDate.AsDateFormat(),
                    Oggetto = relatedElementAggregate.OggettoDelDocumento.Descrizione.Value,
                    DataProtocollo = isProtocollato ? (relatedElementAggregate.DatiRegistrazione as DatiRegistrazioneProtocollo)?.DataProtocollazione.AsDateFormat() : null,
                    NumeroProtocollo = isProtocollato ? (relatedElementAggregate.DatiRegistrazione as DatiRegistrazioneProtocollo)?.NumeroProtocollo.ToString(): null,
                    SegnaturaProtocollo = isProtocollato ? relatedElementAggregate.IdDoc?.Segnatura : null,
                };
            }

            //Tipologia
            var elementProfile = aggregate.Profiles?.FirstOrDefault();
            if(elementProfile is not null && aggregate.ConservaTipologia())
            {
                pi3Metadata.Tipologia = new Tipologia
                {
                    NomeTipologia = elementProfile.Name.Value,
                    CampiTipologia = new List<CampoTipologia>()
                };

                foreach (var field in elementProfile.Fields)
                {
                    var preservationEnabled = field.Metadata.FirstOrDefault(x => x.Key.ToUpper() == "CHA_CONSERVAZIONE").Value == "1";
                    if (preservationEnabled)
                    {
                        if (field.Value.GetType() == typeof(ElementFieldSingleValue))
                        {                 
                            pi3Metadata.Tipologia.CampiTipologia.Add(new CampoTipologia
                            {
                                NomeCampo = field.Name.Value,
                                ValoreCampo = ((ElementFieldSingleValue)field.Value).Value.ToString()
                            });
                        }
                        if (field.Value.GetType() == typeof(ElementFieldLookupValue))
                        {
                            pi3Metadata.Tipologia.CampiTipologia.Add(new CampoTipologia
                            {
                                NomeCampo = field.Name.Value,
                                ValoreCampo = ((ElementFieldLookupValue)field.Value).DescriptionValue.ToString()
                            });
                        }
                        if (field.Value.GetType() == typeof(ContatoreRepertorioFieldValue))
                        {
                            pi3Metadata.Tipologia.CampiTipologia.Add(new CampoTipologia
                            {
                                NomeCampo = field.Name.Value,
                                ValoreCampo = field.Metadata.FirstOrDefault(x => x.Key.ToUpper() == "VAR_SEGNATURA").Value
                            });
                        }

                        if (field.Value is ElementFieldMultiValue multiValue && multiValue.TextValue?.Any() == true)
                        {
                            var campoTipologia = new CampoTipologia
                            {
                                NomeCampo = field.Name.Value,
                                ValoreCampo = string.Join("-", multiValue.TextValue.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => x.Value))
                            };

                            pi3Metadata.Tipologia.CampiTipologia.Add(campoTipologia);
                        }
                    }
                }
            }

            if(aggregate.Allegati is not null 
                && aggregate.Allegati.Any())
            {
                List<Entities.Allegato> allegati = new List<Entities.Allegato>();
                
                foreach(var allegato in aggregate.Allegati)
                {
                    var tipoAllegato = string.Empty;
                    switch(allegato.TipologiaAllegato)
                    {
                        case TipologieAllegatiEnum.Utente:
                            tipoAllegato = TipoAllegatoEnum.AllegatoUtente.GetDescription();
                            break;
                        case TipologieAllegatiEnum.PEC:
                            tipoAllegato = TipoAllegatoEnum.AllegatoPec.GetDescription();
                            break;
                        case TipologieAllegatiEnum.PiTre:
                            tipoAllegato = TipoAllegatoEnum.AllegatoPitre.GetDescription();
                            break;
                        case TipologieAllegatiEnum.SistemiEsterni:
                            tipoAllegato = TipoAllegatoEnum.AllegatoSistemiEsterni.GetDescription();
                            break;
                    }
                    allegati.Add(new Entities.Allegato
                    {
                        ID = allegato.IdDoc.Identiticativo,
                        Descrizione = allegato.Descrizione.Value,
                        Tipo = tipoAllegato,
                        File = string.IsNullOrEmpty(allegato.IdDoc.FileName) ? null
                            : new Entities.File
                            {
                                Impronta = Convert.ToBase64String(allegato.IdDoc.ImprontaCrittograficaDelDocumento.Impronta),
                                AlgoritmoHash = allegato.IdDoc.ImprontaCrittograficaDelDocumento.Algoritmo,
                                 Formato = Path.GetExtension(allegato.IdDoc.FileName).Substring(1),
                                 Dimensione = await _dbContext.ComponentEntities
                                                .Where(c => c.DOCNUMBER == allegato.IdDoc.Identiticativo.AsLong())
                                                .OrderByDescending(c => c.VERSION_ID)
                                                .Select(c => c.FILE_SIZE.ToString())
                                                .FirstAsync()
                            }
                    });
                }

                pi3Metadata.Allegati = allegati.ToArray();
            }

            return pi3Metadata.ToXmlString(true);

        }

        private async Task<string> GetFolderTree(long idParentFolder, long idFascicoloFolder)
        {
            long? idParent = idParentFolder;
            long? idFascicolo = idFascicoloFolder;
            var folderTree = string.Empty;

            while(idParent != idFascicolo)
            {
                var p = await _dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == idParent)
                    .Select(p => new
                    {
                        p.ID_PARENT,
                        p.DESCRIPTION,
                        p.ID_FASCICOLO
                    })
                    .FirstAsync();
                idParent = p.ID_PARENT;
                idFascicolo = p.ID_FASCICOLO;

                folderTree = string.IsNullOrEmpty(folderTree) ? p.DESCRIPTION : $"/{p.DESCRIPTION}";
            }

            return folderTree;
        }

        private async Task<bool> CtrlXMLFattura(DocumentBlobRef documentBlobRef)
        {
            bool retval = false;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            try
            {
                string valoreInXml = "";
                string mappingNumFattura = "//*[name()='FatturaElettronicaBody']/*[name()='DatiGenerali']/*[name()='DatiGeneraliDocumento']/*[name()='Numero']";

                if (documentBlobRef.FileName!.ToUpper().EndsWith("XML") || documentBlobRef.FileName!.ToUpper().EndsWith("XML.P7M"))
                {
                    var file = await this._blobRepository.Get(idTenant, documentBlobRef.IdBlob);
                    byte[]? content = null;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        file.Stream.CopyTo(memoryStream);
                        content = memoryStream.ToArray();
                    }

                    if (documentBlobRef.FileName!.ToUpper().EndsWith("XML.P7M"))
                    {
                        using (var msSignedFile = new MemoryStream(content))
                        {
                            using (var msOriginalFile = new MemoryStream())
                            {
                                await this._cAdESService.LoadOriginalFile(documentBlobRef.FileName, msSignedFile, msOriginalFile);
                                content = msOriginalFile.ToArray();
                            }
                        }
                    }

                    string stringaXml = Encoding.UTF8.GetString(content).Trim();
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    if (stringaXml.Contains("xml version=\"1.1\""))
                        stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");

                    try
                    {
                        xmlDoc.LoadXml(stringaXml);
                    }
                    catch (Exception bomUTF8)
                    {
                        string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                        if (stringaXml.StartsWith(byteOrderMarkUtf8))
                        {
                            stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                        }
                        xmlDoc.LoadXml(stringaXml);
                    }

                    if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains(Resources.Resources.FatturaNamespaceURIFatturaPa) ||
                        xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains(Resources.Resources.FatturaNamespaceURIAgenziaEntrate))
                    {
                        var oggetto = aggregate.Profiles[0].Fields.Where(f => f.Name.Value.ToUpper() == "NUMERO FATTURA").FirstOrDefault();
                        if(oggetto != null)
                        {
                            valoreInXml = xmlDoc.DocumentElement.SelectSingleNode(mappingNumFattura).InnerXml;
                            if (valoreInXml.Contains("<![CDATA["))
                            {
                                valoreInXml = valoreInXml.Replace("<![CDATA[", "");
                                valoreInXml = valoreInXml.Replace("]]>", "");
                            }

                            if (oggetto.Value.ToString().Trim() == valoreInXml.Trim())
                                retval = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retval = false;
            }
            return retval;
        }

        #endregion
    }


}
