// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.AggregateModels.DelegaAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate
{
    public class AggregazioneDocumentale : ContentElement
    {
        #region Public Members

        protected AggregazioneDocumentale() : base()
        { }

        public AggregazioneDocumentale(
            string idTenant,
            DateTime creationDate,
            TextValue descrizione,
            TipiAggregazioneEnum tipoAggregazione,
            TipologieFascicoloEnum? tipologiaFascicolo = null,
            TipologieVisibilitaEnum? tipologiaVisibilita = null)
            : this(null, idTenant, creationDate, descrizione, tipoAggregazione, tipologiaFascicolo, tipologiaVisibilita)
        {
        }

        public AggregazioneDocumentale(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue descrizione,
            TipiAggregazioneEnum tipoAggregazione,
            TipologieFascicoloEnum? tipologiaFascicolo = null,
            TipologieVisibilitaEnum? tipologiaVisibilita = null)
        {
            this.ApplyChange(new AggregazioneDocumentaleCreatedEvent()
            {
                Id = id,
                IdTenant = idTenant,
                TypeName = "AggregazioneDocumentale",
                CreationDate = creationDate,                
                Description = descrizione,
                TipoAggregazione = tipoAggregazione,
                TipologiaFascicolo = tipologiaFascicolo,
                TipologiaVisibilita = tipologiaVisibilita
            });
        }

        public override IEnumerable<Pi3Exception> GetErrors()
        {
            var errors = new List<Pi3Exception>();

            if (!this._tipoAggregazione.HasValue)
            {
                errors.Add(new TipoAggregazioneNonDefinitaPi3Exception());
            }
            
            if (this._tipoAggregazione == TipiAggregazioneEnum.Fascicolo 
                    && !this._tipologiaFascicolo.HasValue)
            {
                errors.Add(new TipologiaFascicoloNonDefinitaPi3Exception());
            }

            if (this._tipoAggregazione == TipiAggregazioneEnum.SerieDocumentale
                    && this._serieDocumentale == null)
            {
                errors.Add(new SerieDocumentaleNonDefinitaPi3Exception());
            }
            
            if (!this._classifications.Any())
            {
                errors.Add(new NessunaClassificazionePresentePi3Exception());
            }

            //if (this._datiRegistrazione == null
            //    && !this.GetUncommittedChanges().Any(c => c.GetType() == typeof(RegistrazioneRichiestaEvent)))
            //{
            //    errors.Add(new RichiestaRegistrazioneNonEffettuataPi3Exception());
            //}

            if (this._registro == null
                && !this.GetUncommittedChanges().Any(c => c.GetType() == typeof(RegistroAssignedEvent)))
            {
                errors.Add(new RegistroNonDefinitoPi3Exception());
            }

            return errors;
        }

        public virtual void AddClassification(string id, TextValue? name = null, string? idSerieDocumentale = null, TextValue? nameSerieDocumentale = null)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

 

            if (this._classifications.Any())
                throw new NotSupportedPi3Exception(ErrorDescriptions.ClassificazioneNonConsentita, ErrorDescriptions.ResourceManager);

 

            base.AddClassification(id, name);


            //modificare SerieDocumentaleNonConsentita
            if (!string.IsNullOrWhiteSpace(idSerieDocumentale) && this._tipoAggregazione != TipiAggregazioneEnum.SerieDocumentale)
                throw new NotSupportedPi3Exception(ErrorDescriptions.SerieDocumentaleNonConsentita, ErrorDescriptions.ResourceManager);

 

            this.ApplyChange(new SerieDocumentaleAssignedEvent()
            {
                Id = this.Id,
                IdSerieDocumentale = idSerieDocumentale,
                Denominazione = nameSerieDocumentale
            });
        }

 
        public override void AddClassification(string id, TextValue? name = null)
        {
            throw new NotImplementedException();
        }

        public override void ChangeName(TextValue newName)
        {
            throw new MethodNotImplementedPi3Exception(nameof(ChangeName));
        }

        public override void ChangeDescription(TextValue newDescription)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            base.ChangeDescription(newDescription);
        }

        public TipiAggregazioneEnum? TipoAggregazione
        {
            get
            {
                return this._tipoAggregazione;
            }
        }

        public TipologieVisibilitaEnum? TipologiaVisibilita
        {
            get
            {
                return this._tipologiaVisibilita;
            }
        }

        //public virtual void AssignSerieDocumentale(string id, TextValue? denominazione = null)
        //{
        //    this.AssertAggregazioneDocumentaleInStatoChiuso();

        //    if (this._tipoAggregazione != TipiAggregazioneEnum.SerieDocumentale)
        //        throw new NotSupportedPi3Exception(ErrorDescriptions.SerieDocumentaleNonConsentita, ErrorDescriptions.ResourceManager);

        //    this.ApplyChange(new SerieDocumentaleAssignedEvent()
        //    {
        //        Id = this.Id,
        //        IdSerieDocumentale = id,
        //        Denominazione = denominazione
        //    });
        //}

        public SerieDocumentale? SerieDocumentale
        {
            get
            {
                return this._serieDocumentale;
            }
        }

        public virtual void AssignTipologiaFascicolo(TipologieFascicoloEnum tipologiaFascicolo)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            this.ApplyChange(new TipologiaFascicoloAssignedEvent() { Id = this.Id, NewTipologiaFascicolo = tipologiaFascicolo });
        }

        public TipologieFascicoloEnum? TipologiaFascicolo
        {
            get
            {
                return this._tipologiaFascicolo;
            }
        }

        public virtual void ChangeTipoVisibilita(TipologieVisibilitaEnum newTipologiaVisibilita)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            this.ApplyChange(new TipologiaVisibilitaEvent() { Id = this.Id, NewTipologiaVisibilita = newTipologiaVisibilita });
        }

        public virtual void AssignAmministrazioneTitolare(AmministrazioneTitolare? amministrazioneTitolare)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            if (amministrazioneTitolare != null)
                Validator.ValidateObject(amministrazioneTitolare, new ValidationContext(amministrazioneTitolare), true);

            this.ApplyChange(new AmministrazioneTitolareAssignedEvent() { Id = this.Id, AmministrazioneTitolare = amministrazioneTitolare });
        }

        public AmministrazioneTitolare? AmministrazioneTitolare
        {
            get
            {
                return this._amministrazioneTitolare;
            }
        }

        public virtual void AssignAmministrazionePartecipante(AmministrazionePartecipante? amministrazionePartecipante)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            if (amministrazionePartecipante != null)
                Validator.ValidateObject(amministrazionePartecipante, new ValidationContext(amministrazionePartecipante), true);

            this.ApplyChange(new AmministrazionePartecipanteAssignedEvent() { Id = this.Id, AmministrazionePartecipante = amministrazionePartecipante });
        }

        public AmministrazionePartecipante? AmministrazionePartecipante
        {
            get
            {
                return this._amministrazionePartecipante;
            }
        }

        public virtual void AssignSoggettoIntestatarioPersonaGiuridica(SoggettoIntestatarioPersonaGiuridica? soggettoIntestatarioPersonaGiuridica)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            if (soggettoIntestatarioPersonaGiuridica != null)
                Validator.ValidateObject(soggettoIntestatarioPersonaGiuridica, new ValidationContext(soggettoIntestatarioPersonaGiuridica), true);

            this.ApplyChange(new SoggettoIntestatarioPersonaGiuridicaAssignedEvent() { Id = this.Id, SoggettoIntestatarioPersonaGiuridica = soggettoIntestatarioPersonaGiuridica });
        }

        public SoggettoIntestatarioPersonaGiuridica? SoggettoIntestatarioPersonaGiuridica
        {
            get
            {
                return this._soggettoIntestatarioPersonaGiuridica;
            }
        }

        public virtual void AssignSoggettoIntestatarioPersonaFisica(SoggettoIntestatarioPersonaFisica? soggettoIntestatarioPersonaFisica)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            if (soggettoIntestatarioPersonaFisica != null)
                Validator.ValidateObject(soggettoIntestatarioPersonaFisica, new ValidationContext(soggettoIntestatarioPersonaFisica), true);

            this.ApplyChange(new SoggettoIntestatarioPersonaFisicaAssignedEvent() { Id = this.Id, SoggettoIntestatarioPersonaFisica = soggettoIntestatarioPersonaFisica });
        }

        public SoggettoIntestatarioPersonaFisica? SoggettoIntestatarioPersonaFisica
        {
            get
            {
                return this._soggettoIntestatarioPersonaFisica;
            }
        }

        public virtual void AssignRUP(RUP? rup)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            if (rup != null)
                Validator.ValidateObject(rup, new ValidationContext(rup), true);

            this.ApplyChange(new RUPAssignedEvent() { Id = this.Id, RUP = rup });
        }

        public RUP? RUP
        {
            get
            {
                return this._rup;
            }
        }

        public virtual void AssignAssegnatario(Assegnatario? assegnatario)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            if (assegnatario != null)
                Validator.ValidateObject(assegnatario, new ValidationContext(assegnatario), true);

            this.ApplyChange(new AssegnatarioAssignedEvent() { Id = this.Id, Assegnatario = assegnatario });
        }

        public Assegnatario? Assegnatario
        {
            get
            {
                return this._assegnatario;
            }
        }

        public virtual void AddAssegnazione(Assegnazione assegnazione)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            assegnazione = assegnazione ?? throw new ArgumentNullException(nameof(assegnazione));
            
            Validator.ValidateObject(assegnazione, new ValidationContext(assegnazione), true);

            this.ApplyChange(new AssegnazioneAddedEvent() { Id = this.Id, Assegnazione = assegnazione });
        }

        public IReadOnlyList<Assegnazione> Assegnazioni
        {
            get
            {
                return this._assegnazioni.AsReadOnly();
            }
        }

        public virtual void Apri(DateTime? newDataApertura = null)
        {
            this.AssertAggregazioneDocumentaleInStatoAperto();

            newDataApertura = newDataApertura ?? DateTime.Now;

            if (newDataApertura < this.DataChiusura)
                throw new DataAperturaNonValidaPi3Exception(newDataApertura.Value);

            this.ApplyChange(new ApertoEvent() { Id = this.Id, NewDataApertura = newDataApertura });
        }

        public DateTime DataApertura
        {
            get
            {
                return this._dataApertura;
            }
        }

        public virtual void Chiudi(DateTime? newDataChiusura = null)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            newDataChiusura = newDataChiusura ?? DateTime.Now;

            this.ApplyChange(new ChiusoEvent() { Id = this.Id, NewDataChiusura = newDataChiusura });
        }

        public DateTime? DataChiusura
        {
            get
            {
                return this._dataChiusura;
            }
        }

        public virtual void AssignProgressivo(int progressivo)
        {
            this.AssertAggregazioneRegistrata();
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            this.ApplyChange(new ProgressivoAssignedEvent() { Id = this.Id, Progressivo = progressivo });
        }

        public int? Progressivo
        {
            get
            {
                return this._progressivo;
            }
        }

        public virtual void AssignProcedimentoAmministrativo(ProcedimentoAmministrativo procedimentoAmministrativo)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            procedimentoAmministrativo = procedimentoAmministrativo ?? throw new ArgumentNullException(nameof(procedimentoAmministrativo));

            Validator.ValidateObject(procedimentoAmministrativo, new ValidationContext(procedimentoAmministrativo), true);

            this.ApplyChange(new ProcedimentoAmministrativoAssignedEvent() { Id = this.Id, ProcedimentoAmministrativo = procedimentoAmministrativo });
        }

        public ProcedimentoAmministrativo? ProcedimentoAmministrativo
        {
            get
            {
                return this._procedimentoAmministrativo;
            }
        }

        public virtual void AddIdDoc(IdDoc idDoc, string? idFolder = null)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            idDoc = idDoc ?? throw new ArgumentNullException(nameof(idDoc));

            Validator.ValidateObject(idDoc, new ValidationContext(idDoc), true);

            if (!string.IsNullOrWhiteSpace(idFolder))
            {
                var folder = this.FindFolderById(idFolder);
                if (folder == null)
                    throw new FolderNotFoundPi3Exception(idFolder);

                if (folder.IdDocs.Contains(idDoc))
                    throw new IdDocPi3Exception(ErrorDescriptions.DocumentoGiaPresente, ErrorDescriptions.ResourceManager, idDoc);
            }

            this.ApplyChange(new IdDocAddedEvent() { Id = this.Id, IdDoc = idDoc, IdFolder = idFolder });
        }

        public virtual void RemoveIdDoc(IdDoc idDoc, string? idFolder = null)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            idDoc = idDoc ?? throw new ArgumentNullException(nameof(idDoc));

            Validator.ValidateObject(idDoc, new ValidationContext(idDoc), true);

            if (!string.IsNullOrWhiteSpace(idFolder))
            {
                var folder = this.FindFolderById(idFolder);
                if (folder == null)
                    throw new FolderNotFoundPi3Exception(idFolder);

                if (!folder.IdDocs.Contains(idDoc))
                    throw new IdDocNotFoundPi3Exception(idDoc);
            }

            this.ApplyChange(new IdDocRemovedEvent() { Id = this.Id, IdDoc = idDoc, IdFolder = idFolder });
        }

        public IReadOnlyList<IdDoc> IdDocs
        {
            get
            {
                return this._idDocs.AsReadOnly();
            }
        }

        public virtual void AssignPosizioneFisicaAggregazioneDocumentale(string? newPosizioneFisicaAggregazioneDocumentale)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            this.ApplyChange(new PosizioneFisicaAggregazioneDocumentaleAssignedEvent() { Id = this.Id, NewPosizioneFisicaAggregazioneDocumentale = newPosizioneFisicaAggregazioneDocumentale });
        }

        public string? PosizioneFisicaAggregazioneDocumentale
        {
            get
            {
                return this._posizioneFisicaAggregazioneDocumentale;
            }
        }

        public virtual void AssignIdAggPrimario(IdAgg? newIdAggPrimario)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            if (newIdAggPrimario != null)
                Validator.ValidateObject(newIdAggPrimario, new ValidationContext(newIdAggPrimario), true);

            this.ApplyChange(new IdAggPrimarioAssignedEvent() { Id = this.Id, NewIdAggPrimario = newIdAggPrimario });
        }

        public IdAgg? IdAggPrimario
        {
            get
            {
                return this._idAggPrimario;
            }
        }

        public virtual void ChangeTempoDiConservazione(int? tempoDiConservazione)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            this.ApplyChange(new TempoDiConservazioneChangedEvent()
            {
                Id = this.Id,
                TempoDiConservazione = tempoDiConservazione
            });
        }

        public int? TempoDiConservazione
        {
            get
            {
                return this._tempoDiConservazione;
            }
        }

        public virtual void AddNota(string id, TipologiaVisibilitaNotaEnum tipologiaVisibilita, TextValue testo)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            testo = testo ?? throw new ArgumentNullException(nameof(testo));

            this.ApplyChange(new NotaAddedEvent()
            {
                Id = this.Id,
                IdNota = id,
                TipologiaVisibilita = tipologiaVisibilita,
                Testo = testo
            });
        }

        public IReadOnlyList<Nota>? Note
        {
            get
            {
                return this._note?.AsReadOnly();
            }
        }

        public virtual void CreateFolderHierarchy(FolderHierarcy folderHierarcy)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            this.ApplyChange(new FolderHierarchyCreatedEvent()
            {
                Id = this.Id,
                FolderHierarcy = folderHierarcy
            });
        }

        public virtual void AddFolder(string id, TextValue name, IReadOnlyList<IdDoc>? idDocs = null, string? idParent = null)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            id = id ?? throw new ArgumentNullException(nameof(id));
            name = name ?? throw new ArgumentNullException(nameof(name));

            if (!string.IsNullOrWhiteSpace(idParent))
            {
                var parentFolder = this.FindFolderById(idParent);

                if (parentFolder == null)
                    throw new FolderNotFoundPi3Exception(idParent);

                parentFolder.AddFolder(new Folder(id, name, idDocs));
            }
            else
                this._folders.Add(new Folder(id, name, idDocs));
        }

        public IReadOnlyList<Folder> Folders
        {
            get
            {
                return this._folders.AsReadOnly();
            }
        }

        public virtual void AssignDatiRegistrazione(DatiRegistrazione datiRegistrazione)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            Validator.ValidateObject(datiRegistrazione, new ValidationContext(datiRegistrazione), true);

            this.ApplyChange(new DatiRegistrazioneAssignedEvent()
            {
                Id = this.Id,
                DatiRegistrazione = datiRegistrazione
            });
        }

        public virtual void AssignCollocazioneFisica(CollocazioneFisica collocazioneFisica)
        {
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            this.ApplyChange(new CollocazioneFisicaAssignedEvent() { Id = this.Id, CollocazioneFisica = collocazioneFisica });
        }

        public DatiRegistrazione? DatiRegistrazione
        {
            get
            {
                return this._datiRegistrazione;
            }
        }

        public virtual void RichiediRegistrazione(DatiRichiestaRegistrazione registrazione)
        {
            this.AssertAggregazioneRegistrata();
            this.AssertAggregazioneDocumentaleInStatoChiuso();

            Validator.ValidateObject(registrazione, new ValidationContext(registrazione), true);

            this.ApplyChange(new RegistrazioneRichiestaEvent()
            {
                Id = this.Id,
                Registrazione = registrazione
            });
        }


        public Registro Registro { get; protected set; }

        public void AssignRegistro(string id, string? descrizione)
        {
            //AssertStatoDelega();
            if (id != null)
                Validator.ValidateObject(id, new ValidationContext(id), true);

            this.ApplyChange(new RegistroAssignedEvent()
            {
                Id = this.Id,
                IdRegistro = id,
                Description = descrizione
            });
        }

        public DateTime? DataScadenza
        {
            get;
            protected set;
        }

        public virtual void AssignDataScadenza(DateTime? newDataScadenza)
        {
            this.ApplyChange(new DataScadenzaAssignedEvent()
            {
                Id = this.Id,
                NewDataScadenza = newDataScadenza
            });
        }
        
        #endregion

        #region Private Members

        protected TipiAggregazioneEnum? _tipoAggregazione;
        protected TipologieVisibilitaEnum? _tipologiaVisibilita;
        protected SerieDocumentale? _serieDocumentale;
        protected TipologieFascicoloEnum? _tipologiaFascicolo;
        protected AmministrazioneTitolare? _amministrazioneTitolare;
        protected AmministrazionePartecipante? _amministrazionePartecipante;
        protected SoggettoIntestatarioPersonaGiuridica? _soggettoIntestatarioPersonaGiuridica;
        protected SoggettoIntestatarioPersonaFisica? _soggettoIntestatarioPersonaFisica;
        protected RUP? _rup;
        protected Assegnatario? _assegnatario;
        protected List<Assegnazione> _assegnazioni;
        protected DateTime _dataApertura;
        protected DateTime? _dataChiusura;
        protected int? _progressivo;
        protected ProcedimentoAmministrativo? _procedimentoAmministrativo;
        protected List<IdDoc> _idDocs;
        protected string? _posizioneFisicaAggregazioneDocumentale;
        protected IdAgg? _idAggPrimario;
        protected int? _tempoDiConservazione;
        protected List<Nota> _note;
        protected List<Folder> _folders;
        protected DatiRegistrazione? _datiRegistrazione;
        protected CollocazioneFisica? _collocazioneFisica;
        protected Registro _registro;

        protected virtual Folder? FindFolderById(string id)
        {
            foreach (var f in this._folders)
            {
                if (f.Id == id)
                    return f;
                else
                    return f.FindFolderById(id);
            }
                

            return null;
        }

        protected virtual void AssertAggregazioneDocumentaleInStatoAperto()
        {
            if (!this._dataChiusura.HasValue)
                throw new NotSupportedPi3Exception(ErrorDescriptions.AggregazioneDocumentaleInStatoAperto, ErrorDescriptions.ResourceManager);
        }


        protected virtual void AssertAggregazioneDocumentaleInStatoChiuso()
        {
            if (this._dataChiusura.HasValue)
                throw new NotSupportedPi3Exception(ErrorDescriptions.AggregazioneDocumentaleInStatoChiuso, ErrorDescriptions.ResourceManager);
        }

        protected virtual void AssertAggregazioneRegistrata()
        {
            if (this._datiRegistrazione != null)
                throw new NotSupportedPi3Exception(ErrorDescriptions.AggregazioneDocumentaleGiaRegistrata, ErrorDescriptions.ResourceManager);
        }

        protected virtual void AssertRegistroAssociato()
        {
            if (this._registro != null)
                throw new NotSupportedPi3Exception(ErrorDescriptions.AggregazioneDocumentaleGiaRegistrata, ErrorDescriptions.ResourceManager);
        }

        protected virtual void Handle(AggregazioneDocumentaleCreatedEvent @event)
        {
            base.Handle((ElementCreatedEvent) @event);

            this._tipoAggregazione = @event.TipoAggregazione;
            this._tipologiaFascicolo = @event.TipologiaFascicolo;
            this._tipologiaVisibilita = @event.TipologiaVisibilita ?? TipologieVisibilitaEnum.Gerarchica;
            this._assegnazioni = new List<Assegnazione>();
            this._dataApertura = @event.CreationDate;
            this._idDocs = new List<IdDoc>();
            this._note = new List<Nota>();
            this._folders = new List<Folder>();
        }

        protected virtual void Handle(TipologiaFascicoloAssignedEvent @event)
        {
            this._tipologiaFascicolo = @event.NewTipologiaFascicolo;
        }

        protected virtual void Handle(TipologiaVisibilitaEvent @event)
        {
            this._tipologiaVisibilita = @event.NewTipologiaVisibilita;
        }

        protected virtual void Handle(SerieDocumentaleAssignedEvent @event)
        {
            this._serieDocumentale = new SerieDocumentale(@event.Id, @event.Denominazione);
        }

        protected virtual void Handle(AmministrazioneTitolareAssignedEvent @event)
        {
            this._amministrazioneTitolare = @event.AmministrazioneTitolare;
        }

        protected virtual void Handle(AmministrazionePartecipanteAssignedEvent @event)
        {
            this._amministrazionePartecipante = @event.AmministrazionePartecipante;
        }

        protected virtual void Handle(SoggettoIntestatarioPersonaGiuridicaAssignedEvent @event)
        {
            this._soggettoIntestatarioPersonaGiuridica = @event.SoggettoIntestatarioPersonaGiuridica;
        }

        protected virtual void Handle(SoggettoIntestatarioPersonaFisicaAssignedEvent @event)
        {
            this._soggettoIntestatarioPersonaFisica = @event.SoggettoIntestatarioPersonaFisica;
        }

        protected virtual void Handle(RUPAssignedEvent @event)
        {
            this._rup = @event.RUP;
        }

        protected virtual void Handle(AssegnatarioAssignedEvent @event)
        {
            this._assegnatario = @event.Assegnatario;
        }

        protected virtual void Handle(AssegnazioneAddedEvent @event)
        {
            this._assegnazioni.Add(@event.Assegnazione);
        }

        protected virtual void Handle(ApertoEvent @event)
        {
            this._dataApertura = @event.NewDataApertura ?? DateTime.Now;
        }

        protected virtual void Handle(ChiusoEvent @event)
        {
            this._dataChiusura = @event.NewDataChiusura ?? DateTime.Now;
        }

        protected virtual void Handle(ProgressivoAssignedEvent @event)
        {
            this._progressivo = @event.Progressivo;
        }

        protected virtual void Handle(ProcedimentoAmministrativoAssignedEvent @event)
        {
            this._procedimentoAmministrativo = @event.ProcedimentoAmministrativo;
        }

        protected virtual void Handle(IdDocAddedEvent @event)
        {
            if (!string.IsNullOrWhiteSpace(@event.IdFolder))
                this.FindFolderById(@event.IdFolder).AddIdDoc(@event.IdDoc);
            else 
                this._idDocs.Add(@event.IdDoc);
        }

        protected virtual void Handle(IdDocRemovedEvent @event)
        {
            if (!string.IsNullOrWhiteSpace(@event.IdFolder))
                this.FindFolderById(@event.IdFolder).RemoveIdDoc(@event.IdDoc);
            else
                this._idDocs.Remove(@event.IdDoc);
        }

        protected virtual void Handle(PosizioneFisicaAggregazioneDocumentaleAssignedEvent @event)
        {
            this._posizioneFisicaAggregazioneDocumentale = @event.NewPosizioneFisicaAggregazioneDocumentale;
        }

        protected virtual void Handle(IdAggPrimarioAssignedEvent @event)
        {
            this._idAggPrimario = @event.NewIdAggPrimario;
        }

        protected virtual void Handle(TempoDiConservazioneChangedEvent @event)
        {
            this._tempoDiConservazione = @event.TempoDiConservazione;
        }

        protected virtual void Handle(NotaAddedEvent @event)
        {
            this._note.Add(new Nota(@event.Id, @event.TipologiaVisibilita, @event.Testo));
        }

        protected virtual void Handle(FolderHierarchyCreatedEvent @event)
        {
        }

        protected virtual void Handle(DatiRegistrazioneAssignedEvent @event)
        {
            this._datiRegistrazione = @event.DatiRegistrazione;
        }

        protected virtual void Handle(RegistrazioneRichiestaEvent @event)
        {
        }


        protected virtual void Handle(CollocazioneFisicaAssignedEvent @event)
        {
            this._collocazioneFisica = @event.CollocazioneFisica;
        }

        protected virtual void Handle(RegistroAssignedEvent @event)
        {
            this._registro = new Registro(@event.IdRegistro, @event.Description);
        }

        protected virtual void Handle(DataScadenzaAssignedEvent @event)
        {
            this.DataScadenza = @event.NewDataScadenza;
        }
        #endregion
    }
}
