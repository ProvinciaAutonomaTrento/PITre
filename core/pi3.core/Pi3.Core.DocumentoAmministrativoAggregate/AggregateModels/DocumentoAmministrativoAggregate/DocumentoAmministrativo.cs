// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.AggregateModels.DocumentAggregate;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Exceptions;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Resources;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.ContentElementAggregate.Entities;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Pi3.Core.AggregateModels.ContentElementAggregate.Events;
using Pi3.Core.Extensions;
using System.Xml.Linq;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Segnatura;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate.Exceptions;
using AutoMapper;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Extensions;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Metadati;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
{
    public class DocumentoAmministrativo : Document
    {
        #region Public Members

        protected DocumentoAmministrativo() : base()
        { }

        public DocumentoAmministrativo(
            string idTenant,
            DateTime creationDate,
            OggettoDelDocumento oggettoDelDocumento,
            DatiRegistro? datiRegistro = null,
            TipologiaFlussoEnum? tipologiaFlusso = null,
            TipologieVisibilitaEnum? tipologiaVisibilita = TipologieVisibilitaEnum.Gerarchica,
            IdDoc? idDocPrimario = null)
            : this(null!, idTenant, creationDate, oggettoDelDocumento, datiRegistro, tipologiaFlusso, tipologiaVisibilita, idDocPrimario)
        {
        }

        public DocumentoAmministrativo(
            string id,
            string idTenant,
            DateTime creationDate,
            OggettoDelDocumento oggettoDelDocumento,
            DatiRegistro? datiRegistro = null,
            TipologiaFlussoEnum? tipologiaFlusso = null,
            TipologieVisibilitaEnum? tipologiaVisibilita = TipologieVisibilitaEnum.Gerarchica,
            IdDoc? idDocPrimario = null)
        {
            if (tipologiaFlusso.HasValue && idDocPrimario! != null!)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonConsentita, ErrorDescriptions.ResourceManager);

            if (datiRegistro! == null! && idDocPrimario! == null!)
                throw new NotSupportedPi3Exception(ErrorDescriptions.DatiRegistroNonPresenti, ErrorDescriptions.ResourceManager);

            if (datiRegistro! != null!)
            {
                if (idDocPrimario! != null!)
                    throw new NotSupportedPi3Exception(ErrorDescriptions.DatiRegistroNonConsentiti, ErrorDescriptions.ResourceManager);

                Validator.ValidateObject(datiRegistro, new ValidationContext(datiRegistro), true);
            }

            this.ApplyChange(new DocumentoAmministrativoCreatedEvent()
            {
                Id = id,
                IdTenant = idTenant,
                TypeName = nameof(DocumentoAmministrativo),
                CreationDate = creationDate,
                Name = new TextValue(id),
                Description = oggettoDelDocumento.Descrizione,
                OggettoDelDocumento = oggettoDelDocumento,
                DatiRegistro = datiRegistro,
                TipologiaFlusso = tipologiaFlusso,
                TipologiaVisibilita = tipologiaVisibilita,
                IdDocPrimario = idDocPrimario
            });
        }

        public override IEnumerable<Pi3Exception> GetErrors()
        {
            var errors = new List<Pi3Exception>();

            if (this.TipologiaFlusso.HasValue)
            {
                switch (this.TipologiaFlusso)
                {
                    case TipologiaFlussoEnum.E:
                        if (this._mittente == null!)
                            errors.Add(new MittenteNonAssegnatoPi3Exception());

                        break;
                    case TipologiaFlussoEnum.I:
                    case TipologiaFlussoEnum.U:
                        if (this._mittente == null!)
                            errors.Add(new MittenteNonAssegnatoPi3Exception());

                        if ((this.Destinatari ?? new Destinatario[0]).Count == 0)
                            errors.Add(new NessunDestinatarioPresentePi3Exception());

                        break;
                }
            }

            //if (this.Registro! == null! && this.IdDocPrimario! == null!)
            //    errors.Add(new NessunRegistroAssegnatoPi3Exception());

            return errors.AsReadOnly();
        }

        public override void AddVersion(string id, TextValue name, int versionNumber, DateTime creationDate, DocumentBlobRef? documentBlobRef = null, bool? digitalSigned = null, int? pageCount = null, bool? fromPaper = null)
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello1);

            base.AddVersion(id, name, versionNumber, creationDate, documentBlobRef, digitalSigned, pageCount, fromPaper);
        }

        public virtual void RichiediRegistrazione(DatiRichiestaRegistrazione registrazione)
        {
            Validator.ValidateObject(registrazione, new ValidationContext(registrazione), true);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new RegistrazioneRichiestaEvent()
            {
                Id = this.Id,
                Registrazione = registrazione
            });
        }

        public virtual void ChangeOggettoDelDocumento(OggettoDelDocumento newOggettoDelDocumento)
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            Validator.ValidateObject(newOggettoDelDocumento, new ValidationContext(newOggettoDelDocumento), true);

            this.ApplyChange(new OggettoDelDocumentoChangedEvent() { Id = this.Id, NewOggettoDelDocumento = newOggettoDelDocumento });
        }

        public virtual void AssignDatiRegistrazioneProtocollo(DatiRegistrazioneProtocollo datiRegistrazione)
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            this.AssertDocumentoAmministrativoInformaticoRegistrato();

            Validator.ValidateObject(datiRegistrazione, new ValidationContext(datiRegistrazione), true);

            this.ApplyChange(new DatiRegistrazioneProtocolloAssignedEvent() { Id = this.Id, DatiRegistrazione = datiRegistrazione });
        }

        public virtual void AssignDatiRegistrazioneRepertorio(DatiRegistrazioneRepertorio datiRegistrazione)
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            Validator.ValidateObject(datiRegistrazione, new ValidationContext(datiRegistrazione), true);

            this.ApplyChange(new DatiRegistrazioneRepertorioAssignedEvent() { Id = this.Id, DatiRegistrazione = datiRegistrazione });
        }

        public override void ChangeName(TextValue newName)
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertDocumentoAmministrativoInformaticoRegistrato();

            base.ChangeName(newName);
        }

        public override void ChangeDescription(TextValue newDescription)
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertDocumentoAmministrativoInformaticoRegistrato();

            base.ChangeDescription(newDescription);
        }

        public override void AssignId(string id)
        {
            this.AssertDocumentoAmministrativoInformaticoRegistrato();

            base.AssignId(id);
        }

        public override void AddClassification(string id, TextValue? name = null, string? code = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            base.AddClassification(id, name, code);
        }

        public override void RemoveClassification(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            base.RemoveClassification(id);
        }

        public OggettoDelDocumento OggettoDelDocumento
        {
            get
            {
                return this._oggettoDelDocumento;
            }
        }

        public Registro? Registro
        {
            get
            {
                return this._registro;
            }
        }

        public TipologiaFlussoEnum? TipologiaFlusso
        {
            get
            {
                return this._tipologiaFlusso;
            }
        }

        public TipologieVisibilitaEnum? TipologiaVisibilita
        {
            get
            {
                return this._tipologiaVisibilita;
            }
        }

        public DatiStampa? DatiStampa
        {
            get
            {
                return this._datiStampa;
            }
        }


        [JsonIgnore()]
        public DatiRegistrazione? DatiRegistrazione
        {
            get
            {
                return this._datiRegistrazione;
            }
        }

        public virtual void AssignIdDoc(IdDoc idDoc)
        {
            Validator.ValidateObject(idDoc, new ValidationContext(idDoc), true);

            this.ApplyChange(new IdDocAssignedEvent() { Id = this.Id, IdDoc = idDoc });
        }

        [JsonIgnore()]
        public IdDoc? IdDoc
        {
            get
            {
                return this._idDoc;
            }
        }

        public virtual void AssignTipologiaDocumentale(string? tipologiaDocumentale)
        {
            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new TipologiaDocumentaleAssignedEvent()
            {
                Id = this.Id,
                TipologiaDocumentale = tipologiaDocumentale
            });
        }

        public string? TipologiaDocumentale
        {
            get
            {
                return this._tipologiaDocumentale;
            }
        }

        public virtual void AssignAmministrazione(AmministrazioneCheEffettuaLaRegistrazione? amministrazione)
        {
            Validator.ValidateObject(amministrazione, new ValidationContext(amministrazione), true);

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new AmministrazioneAssignedEvent() { Id = this.Id, Amministrazione = amministrazione });
        }

        public AmministrazioneCheEffettuaLaRegistrazione? Amministrazione
        {
            get
            {
                return this._amministrazione;
            }
        }

        public virtual void AssignAutore(Autore autore)
        {
            Validator.ValidateObject(autore, new ValidationContext(autore), true);

            this.ApplyChange(new AutoreAssignedEvent()
            {
                Id = this.Id,
                Autore = autore
            });
        }

        public Autore? Autore
        {
            get
            {
                return this._autore;
            }
        }

        public virtual void AssignMezzoSpedizione(string id, TextValue? descrizione = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new MezzoSpedizioneAssignedEvent()
            {
                Id = this.Id,
                IdMezzoSpedizione = (string.IsNullOrWhiteSpace(id) ? null : id),
                Descrizione = (string.IsNullOrWhiteSpace(id) ? null : descrizione)
            });
        }

        public virtual void RemoveMezzoSpedizione()
        {
            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new MezzoSpedizioneRemovedEvent()
            {
                Id = this.Id
            });
        }

        public MezzoSpedizione? MezzoSpedizione
        {
            get
            {
                return this._mezzoSpedizione;
            }
        }

        public virtual void AssignMittente(Mittente? mittente)
        {
            Validator.ValidateObject(mittente, new ValidationContext(mittente), true);

            if (!this.TipologiaFlusso.HasValue)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonValida, ErrorDescriptions.ResourceManager);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new MittenteAssignedEvent()
            {
                Id = this.Id,
                Mittente = mittente
            });
        }

        public virtual void ChangeMittente(Mittente? mittente)
        {
            Validator.ValidateObject(mittente, new ValidationContext(mittente), true);

            if (!this.TipologiaFlusso.HasValue)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonValida, ErrorDescriptions.ResourceManager);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new MittenteChangedEvent()
            {
                Id = this.Id,
                Mittente = mittente
            });
        }

        public Mittente? Mittente
        {
            get
            {
                return this._mittente;
            }
        }
        public virtual void AddMittenteMultiplo(Mittente mittente)
        {
            Validator.ValidateObject(mittente, new ValidationContext(mittente), true);

            if (!this.TipologiaFlusso.HasValue)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonValida, ErrorDescriptions.ResourceManager);

            if (this._tipologiaFlusso != TipologiaFlussoEnum.E)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonValida, ErrorDescriptions.ResourceManager);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new MittenteMultiploAddedEvent()
            {
                Id = this.Id,
                Mittente = mittente
            });
        }

        public virtual void RemoveMittenteMultiplo(Mittente mittente)
        {
            Validator.ValidateObject(mittente, new ValidationContext(mittente), true);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new MittenteMultiploRemovedEvent()
            {
                Id = this.Id,
                Mittente = mittente
            });
        }

        public IReadOnlyList<Mittente>? MittentiMultipli
        {
            get
            {
                return this._mittentiMultipli?.AsReadOnly();
            }
        }

        public Mittente? MittenteIntermedio
        {
            get
            {
                return this._mittenteIntermedio;
            }
        }

        public virtual void AssignMittenteIntermedio(Mittente? mittente)
        {
            Validator.ValidateObject(mittente, new ValidationContext(mittente), true);

            if (!this.TipologiaFlusso.HasValue)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonValida, ErrorDescriptions.ResourceManager);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new MittenteIntermedioAssignedEvent()
            {
                Id = this.Id,
                Mittente = mittente
            });
        }


        public virtual void RemoveMittenteIntermedio()
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new MittenteIntermedioRemovedEvent()
            {
                Id = this.Id
            });
        }

        public virtual void AddDestinatario(Destinatario destinatario)
        {
            Validator.ValidateObject(destinatario, new ValidationContext(destinatario), true);

            if (!this.TipologiaFlusso.HasValue)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonValida, ErrorDescriptions.ResourceManager);

            if (this._tipologiaFlusso == TipologiaFlussoEnum.E)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonValida, ErrorDescriptions.ResourceManager);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new DestinatarioAddedEvent()
            {
                Id = this.Id,
                Destinatario = destinatario
            });
        }

        public virtual void RemoveDestinatario(Destinatario destinatario)
        {
            Validator.ValidateObject(destinatario, new ValidationContext(destinatario), true);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new DestinatarioRemovedEvent()
            {
                Id = this.Id,
                Destinatario = destinatario
            });
        }

        public IReadOnlyList<Destinatario>? Destinatari
        {
            get
            {
                return this._destinatari?.AsReadOnly();
            }
        }

        public virtual void AddDestinatarioCc(Destinatario destinatario)
        {
            Validator.ValidateObject(destinatario, new ValidationContext(destinatario), true);

            if (!this.TipologiaFlusso.HasValue)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonValida, ErrorDescriptions.ResourceManager);

            if (this._tipologiaFlusso == TipologiaFlussoEnum.E)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaFlussoNonValida, ErrorDescriptions.ResourceManager);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new DestinatarioCcAddedEvent()
            {
                Id = this.Id,
                Destinatario = destinatario
            });
        }

        public virtual void RemoveDestinatarioCc(Destinatario destinatario)
        {
            Validator.ValidateObject(destinatario, new ValidationContext(destinatario), true);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new DestinatarioCcRemovedEvent()
            {
                Id = this.Id,
                Destinatario = destinatario
            });
        }

        public IReadOnlyList<Destinatario>? DestinatariCc
        {
            get
            {
                return this._destinatariCc?.AsReadOnly();
            }
        }

        public virtual void AddAssegnatario(Assegnatario assegnatario)
        {
            Validator.ValidateObject(assegnatario, new ValidationContext(assegnatario), true);

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new AssegnatarioAddedEvent()
            {
                Id = this.Id,
                Assegnatario = assegnatario
            });
        }

        public IReadOnlyList<Assegnatario>? Assegnatari
        {
            get
            {
                return this._assegnatari?.AsReadOnly();
            }
        }

        public virtual void AddOperatore(Operatore operatore)
        {
            Validator.ValidateObject(operatore, new ValidationContext(operatore), true);

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new OperatoreAddedEvent()
            {
                Id = this.Id,
                Operatore = operatore
            });
        }

        public IReadOnlyList<Operatore>? Operatori
        {
            get
            {
                return this._operatori?.AsReadOnly(); ;
            }
        }

        public virtual void AssignResponsabileGestioneDocumentale(ResponsabileGestioneDocumentale? responsabile)
        {
            Validator.ValidateObject(responsabile, new ValidationContext(responsabile), true);

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new ResponsabileGestioneDocumentaleAssignedEvent()
            {
                Id = this.Id,
                ResponsabileGestioneDocumentale = responsabile
            });
        }

        public ResponsabileGestioneDocumentale? ResponsabileGestioneDocumentale
        {
            get
            {
                return this._responsabileGestioneDocumentale; ;
            }
        }

        public virtual void AssignResponsabileServizioProtocollo(ResponsabileServizioProtocollo? responsabile)
        {
            Validator.ValidateObject(responsabile, new ValidationContext(responsabile), true);

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new ResponsabileServizioProtocolloAssignedEvent()
            {
                Id = this.Id,
                ResponsabileServizioProtocollo = responsabile
            });
        }

        public ResponsabileServizioProtocollo? ResponsabileServizioProtocollo
        {
            get
            {
                return this._responsabileServizioProtocollo;
            }
        }

        public virtual void AssignRUP(RUP? rup)
        {
            Validator.ValidateObject(rup, new ValidationContext(rup), true);

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new RUPAssignedEvent()
            {
                Id = this.Id,
                RUP = rup
            });
        }

        public RUP? RUP
        {
            get
            {
                return this._rup;
            }
        }

        public virtual void AssignSwProduttore(SwProduttore? swProduttore)
        {
            Validator.ValidateObject(swProduttore, new ValidationContext(swProduttore), true);

            this.ApplyChange(new SwProduttoreAssignedEvent()
            {
                Id = this.Id,
                SwProduttore = swProduttore
            });
        }

        public SwProduttore? Produttore
        {
            get
            {
                return this._produttore;
            }
        }

        public virtual void AddAllegato(Allegato allegato)
        {
            Validator.ValidateObject(allegato, new ValidationContext(allegato), true);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello1);
            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new AllegatoAddedEvent()
            {
                Id = this.Id,
                Allegato = allegato
            });
        }

        public IReadOnlyList<Allegato>? Allegati
        {
            get
            {
                return this._allegati?.AsReadOnly();
            }
        }

        public virtual void ChangeTipoVisibilita(TipologieVisibilitaEnum tipoVisibilita)
        {
            this.AssertIdDocPrimarioAssociato();

            if (tipoVisibilita < this.TipologiaVisibilita)
                throw new NotSupportedPi3Exception(ErrorDescriptions.TipologiaVisibilitaNonConsentita, ErrorDescriptions.ResourceManager);

            this.ApplyChange(new TipologiaVisibilitaChangedEvent()
            {
                Id = this.Id,
                TipologiaVisibilita = tipoVisibilita
            });
        }

        public bool? Riservato
        {
            get
            {
                return this.TipologiaVisibilita <= TipologieVisibilitaEnum.Privata;
            }
        }

        public virtual void AddAggFascicolo(string id, TextValue? denominazione = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new AggFascicoloAddedEvent()
            {
                Id = this.Id,
                IdFascicolo = id,
                Denominazione = denominazione
            });
        }

        public virtual void RemoveAggFascicolo(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new AggFascicoloRemovedEvent()
            {
                Id = this.Id,
                IdFascicolo = id
            });
        }

        public virtual void AddAggSerieDocumentale(string id, TextValue? denominazione = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new AggSerieDocumentaleAddedEvent()
            {
                Id = this.Id,
                IdSerieDocumentale = id,
                Denominazione = denominazione
            });
        }

        public virtual void RemoveAggSerieDocumentale(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new AggSerieDocumentaleRemovedEvent()
            {
                Id = this.Id,
                IdSerieDocumentale = id
            });
        }

        public virtual void AddAggSerieDiFascicoli(string id, TextValue? denominazione = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new AggSerieDiFascicoliAddedEvent()
            {
                Id = this.Id,
                IdSerieDiFascicoli = id,
                Denominazione = denominazione
            });
        }

        public virtual void RemoveAggSerieDiFascicoli(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new AggSerieDiFascicoliRemovedEvent()
            {
                Id = this.Id,
                IdSerieDiFascicoli = id
            });
        }

        public IReadOnlyList<Aggregazione>? Aggregazioni
        {
            get
            {
                return this._aggregazioni?.AsReadOnly();
            }
        }

        public override void ChangeIdParentDocument(string newIdParentDocument)
        {
            throw new NotSupportedPi3Exception(ErrorDescriptions.ChangeIdParentDocumentNonSupportato, ErrorDescriptions.ResourceManager);
        }

        public IdDoc? IdDocPrimario
        {
            get
            {
                return this._idDocPrimario;
            }
        }

        public virtual void ChangeTempoDiConservazione(int? tempoDiConservazione)
        {
            this.AssertIdDocPrimarioAssociato();

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

        public virtual void AddNota(string id, TipologieVisibilitaNotaEnum tipologiaVisibilita, TextValue testo)
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

        public virtual void Annulla(Annullamento annullamento)
        {
            Validator.ValidateObject(annullamento, new ValidationContext(annullamento), true);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            this.AssertDocumentoAmministrativoInformaticoAnnullato();
            this.AssertDocumentoAmministrativoInLibroFirma();

            this.ApplyChange(new AnnullatoEvent()
            {
                Id = this.Id,
                Annullamento = annullamento
            });
        }

        public Annullamento? Annullamento
        {
            get
            {
                return this._annullamento;
            }
        }

        public Consolidamento? Consolidamento
        {
            get
            {
                return this._consolidamento;
            }
        }

        public virtual void Predisponi(TipologiaFlussoEnum tipologiaFlusso)
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            this.AssertDocumentoAmministrativoInformaticoRegistrato();
            this.AssertDocumentoAmministrativoPredisposto();

            this.ApplyChange(new PredispostoEvent()
            {
                Id = this.Id,
                TipologiaFlusso = tipologiaFlusso
            });
        }

        public virtual void AnnullaPredisposizione()
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            this.AssertDocumentoAmministrativoInformaticoRegistrato();

            this.ApplyChange(new PredisposizioneAnnullataEvent()
            {
                Id = this.Id
            });
        }

        public virtual void AssignProtocolloMittente(ProtocolloMittente? protocolloMittente)
        {
            Validator.ValidateObject(protocolloMittente, new ValidationContext(protocolloMittente), true);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new ProtocolloMittenteAssignedEvent()
            {
                Id = this.Id,
                ProtocolloMittente = protocolloMittente
            });
        }

        public ProtocolloMittente? ProtocolloMittente
        {
            get;
            protected set;
        }

        public virtual void AssignProtocolloEmergenza(ProtocolloEmergenza? protocolloEmergenza)
        {
            Validator.ValidateObject(protocolloEmergenza, new ValidationContext(protocolloEmergenza), true);

            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new ProtocolloEmergenzaAssignedEvent()
            {
                Id = this.Id,
                ProtocolloEmergenza = protocolloEmergenza
            });
        }

        public ProtocolloEmergenza? ProtocolloEmergenza
        {
            get;
            protected set;
        }

        public override void Recycle(TextValue noteRecycleBin)
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello1);
            this.AssertDocumentoAmministrativoInformaticoRegistrato();
            this.AssertDocumentoAmministrativoInLibroFirma();
            this.AssertReserved();

            base.Recycle(noteRecycleBin);
        }

        public override void Restore()
        {
            //this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello1);
            //this.AssertDocumentoAmministrativoInformaticoRegistrato();
            //this.AssertDocumentoAmministrativoInLibroFirma();
            //this.AssertReserved();

            base.Restore();
        }

        public override void Delete()
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello1);
            this.AssertDocumentoAmministrativoInformaticoRegistrato();
            this.AssertDocumentoAmministrativoInLibroFirma();

            base.Delete();
        }

        public virtual void Consolida(Consolidamento consolidamento)
        {
            Validator.ValidateObject(consolidamento, new ValidationContext(consolidamento), true);

            this.AssertDocumentoAmministrativoConsolidato(consolidamento.Stato);
            this.AssertDocumentoAmministrativoInLibroFirma();

            this.ApplyChange(new DocumentoConsolidatoEvent()
            {
                Id = this.Id,
                Consolidamento = consolidamento
            });
        }

        public virtual void AssignClassificationOrAggAsPrincipale(ContentElementClassification? classification)
        {
            Validator.ValidateObject(classification, new ValidationContext(classification), true);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);

            if (classification != null)
            {
                if (!this._classifications.Contains(classification))
                    throw new ClassificationOrAggNotFoundPi3Exception(classification.Id);
            }

            this.ApplyChange(new ClassificationOrAggAsPrincipaleAssigned()
            {
                Id = this.Id,
                ClassificationOrAggPrincipale = classification
            });
        }

        public virtual void AssignClassificationOrAggAsPrincipale(Aggregazione? agg)
        {
            Validator.ValidateObject(agg, new ValidationContext(agg), true);

            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);

            if (agg != null)
            {
                if (!this._aggregazioni.Contains(agg))
                    throw new ClassificationOrAggNotFoundPi3Exception(agg.Id);
            }

            this.ApplyChange(new ClassificationOrAggAsPrincipaleAssigned()
            {
                Id = this.Id,
                ClassificationOrAggPrincipale = agg
            });
        }

        public IEntity<string>? ClassificationOrAggPrincipale
        {
            get
            {
                return this._classificationOrAggPrincipale;
            }
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

        public IReadOnlyList<Soggetto> Soggetti
        {
            get
            {
                var soggetti = new List<Soggetto>();

                if (this._amministrazione != null)
                    soggetti.Add(this._amministrazione);

                if (this._autore != null)
                    soggetti.Add(this._autore);

                if (this._mittente != null)
                    soggetti.Add(this._mittente);

                if (this._mittentiMultipli != null)
                    soggetti.AddRange(this._mittentiMultipli);

                if (this._assegnatari != null)
                    soggetti.AddRange(this._assegnatari);

                if (this._operatori != null)
                    soggetti.AddRange(this._operatori);

                if (this._destinatari != null)
                    soggetti.AddRange(this._destinatari);

                if (this._destinatariCc != null)
                    soggetti.AddRange(this._destinatariCc);

                if (this._responsabileGestioneDocumentale != null)
                    soggetti.Add(this._responsabileGestioneDocumentale);

                if (this._responsabileServizioProtocollo != null)
                    soggetti.Add(this._responsabileServizioProtocollo);

                if (this._rup != null)
                    soggetti.Add(this._rup);

                if (this._produttore != null)
                    soggetti.Add(this._produttore);

                return soggetti.AsReadOnly();
            }
        }
         
        public virtual void AddInLibroFirma()
        {
            this.AssertDocumentoAmministrativoInLibroFirma();

            this.ApplyChange(new InLibroFirmaAddedEvent());
        }

        public virtual void RemoveFromLibroFirma()
        {
            this.ApplyChange(new FromLibroFirmaRemovedEvent());
        }

        public bool InLibroFirma
        {
            get;
            protected set;
        }

        public string? ReservedIdGroup { get; protected set; }

        public string? ReservedDocumentLocation { get; protected set; }

        public string? ReservedMachineName { get; protected set; }

        public override void Reserve(DateTime? reserveDate = null, string? reserveIdUser = null)
        {
            this.Reserve(reserveDate, reserveIdUser, null, null, null);
        }

        public virtual void Reserve(DateTime? reserveDate = null, string? reserveIdUser = null, string? reserveIdGroup = null, string? documentLocation = null, string? machineName = null)
        {
            this.AssertReserved();

            this.ApplyChange(new DocumentoAmministrativoReservedEvent()
            {
                ReservedDate = reserveDate,
                ReserveIdUser = reserveIdUser,
                ReserveIdGroup = reserveIdGroup,
                DocumentLocation = documentLocation,
                MachineName = machineName
            });
        }

        public virtual void ChangeTipologiaAllegato(TipologieAllegatiEnum? newTipologiaAllegato = null)
        {
            this.AssertIdDocPrimarioNonAssociato();

            this.ApplyChange(new TipologiaAllegatoChangedEvent()
            {
                NewTipologiaAllegato = newTipologiaAllegato
            });
        }

        public TipologieAllegatiEnum? TipologiaAllegato
        {
            get;
            protected set;
        }

        public virtual void ChangeNumeroPagineAllegato(int? newNumeroPagine = null)
        {
            this.AssertIdDocPrimarioNonAssociato();

            this.ApplyChange(new NumeroPagineAllegatoChangedEvent()
            {
                NewNumeroPagine = newNumeroPagine
            });
        }

        public int? NumeroPagineAllegato
        {
            get;
            protected set;
        }

        public virtual string GetMetadatiXml()
        {
            var type = new DocumentoAmministrativoInformaticoType()
            {
                ModalitaDiFormazione = ModalitaDiFormazioneType.memorizzazionesusupportoinformaticoinformatodigitaledelleinformazionirisultantidatransazionioprocessiinformaticiodallapresentazionetelematicadidatiattraversomodulioformulariresidisponibiliadutente
            };

            if (this.IdDoc! != null!)
            {
                type.IdDoc = this.IdDoc.AsIdDocType();
            };

            type.TipologiaDocumentale = this.TipologiaDocumentale;

            type.DatiDiRegistrazione = this.AsDatiRegistrazioneType();

            //if (this.DatiRegistrazione! != null!)
            //{
            //    type.DatiDiRegistrazione = this.DatiRegistrazione.AsDatiRegistrazioneType();
            //}

            var soggetti = new List<RuoloType>();

            if (this._amministrazione != null!)
            {
                soggetti.Add(new RuoloType()
                {
                    Item = this._amministrazione.AsTipoSoggetto1Type()
                });
            }

            foreach (var ass in this._assegnatari ?? new List<Assegnatario>())
            {
                soggetti.Add(new RuoloType()
                {
                    Item = ass.AsTipoSoggetto2Type()
                });
            }

            foreach (var dest in this._destinatari ?? new List<Destinatario>())
            {
                soggetti.Add(new RuoloType()
                {
                    Item = dest.AsTipoSoggetto31Type()
                });
            }

            if (this._mittente != null!)
            {
                soggetti.Add(new RuoloType()
                {
                    Item = this._mittente.AsTipoSoggetto32Type()
                });
            }

            if (this._autore != null!)
            {
                soggetti.Add(new RuoloType()
                {
                    Item = this._autore.AsTipoSoggetto41Type()
                });
            }

            foreach (var op in this._operatori ?? new List<Operatore>())
            {
                soggetti.Add(new RuoloType()
                {
                    Item = op.AsTipoSoggetto42Type()
                });
            }

            if (this._responsabileGestioneDocumentale != null!)
            {
                soggetti.Add(new RuoloType()
                {
                    Item = this._responsabileGestioneDocumentale.AsTipoSoggetto43Type()
                });
            }

            if (this._responsabileServizioProtocollo != null!)
            {
                soggetti.Add(new RuoloType()
                {
                    Item = this._responsabileServizioProtocollo.AsTipoSoggetto44Type()
                });
            }

            if (this._rup != null!)
            {
                soggetti.Add(new RuoloType()
                {
                    Item = this._rup.AsTipoSoggetto6Type()
                });
            }

            if (this._produttore != null!)
            {
                soggetti.Add(new RuoloType()
                {
                    Item = this._produttore.AsTipoSoggetto5Type()
                });
            }

            type.Soggetti = soggetti.ToArray();

            if (this.OggettoDelDocumento != null!)
            {
                type.ChiaveDescrittiva = new ChiaveDescrittivaType()
                {
                    Oggetto = this.OggettoDelDocumento?.Descrizione?.ToString(),
                    ParoleChiave = this.Keywords?.Select(k => k.ToString()).ToArray()
                };
            }

            type.Allegati = new AllegatiType()
            {
                NumeroAllegati = this.Allegati?.Count.ToString(),
                IndiceAllegati = this.Allegati?.Select(a => new IndiceAllegatiType()
                {
                    Descrizione = a.Descrizione?.ToString(),
                    IdDoc = a.IdDoc.AsIdDocType()
                }).ToArray()
            };

            if (this.Classifications != null!
                && this.Classifications.Any())
            {
                var firstClassification = this.Classifications.First();

                type.Classificazione = new ClassificazioneType()
                {
                    IndiceDiClassificazione = firstClassification.Code,
                    Descrizione = firstClassification.Name?.ToString()
                };
            }

            type.Riservato = this.Riservato.GetValueOrDefault();

            type.IdentificativoDelFormato = new IdentificativoDelFormatoType()
            {
                Formato = Path.GetExtension(this.CurrentVersion?.DocumentBlobRef?.FileName),
                ProdottoSoftware = (this._produttore! != null! ? new ProdottoSoftwareType()
                {
                    NomeProdotto = this._produttore.Value
                } : null)
            };            

            type.Verifica = new VerificaType()
            {
                FirmatoDigitalmente = this.CurrentVersion?.DocumentBlobRef?.TipoFirma.HasValue ?? false,
                SigillatoElettronicamente = this.CurrentVersion?.DocumentBlobRef?.SegnaturaPermanente.GetValueOrDefault() ?? false,
                MarcaturaTemporale = this.CurrentVersion?.DocumentBlobRef?.TipoFirma.GetValueOrDefault() == TipoFirmaEnum.TsdElettronica,
                ConformitaCopieImmagineSuSupportoInformatico = false
            };

            if (this.Aggregazioni != null
                && this.Aggregazioni.Any())
            {
                type.Agg = this.Aggregazioni?
                            .Select(agg => new IdAggType()
                            {
                                IdAggregazione = agg.Id,
                                TipoAggregazione = Enum.Parse<TipoAggregazioneType>(agg.Tipo, true)
                            })
                            .ToArray();
            }

            type.IdIdentificativoDocumentoPrimario = this.IdDocPrimario?.AsIdDocType();
            type.NomeDelDocumento = this.Name?.ToString();
            type.VersioneDelDocumento = (this.Versions.Any() ? this.Versions?.Max(v => v.VersionNumber).ToString() : 1.ToString());
            type.TempoDiConservazione = this.TempoDiConservazione?.ToString();
            type.Note = string.Join(", ",
                this.Note?
                .Where(n => n.TipologiaVisibilita == TipologieVisibilitaNotaEnum.Pubblica)
                .Select(n => n.Testo?.ToString()!)
                .ToArray()!);
                
            return type.ToXmlString();
        }

        public virtual string GetSegnaturaXml()
        {
            this.AssertIdDocPrimarioAssociato();

            Segnatura.SegnaturaInformaticaType segnaturaInformaticaType = new Segnatura.SegnaturaInformaticaType();

            #region Intestazione

            segnaturaInformaticaType.Intestazione = new Segnatura.IntestazioneType()
            {
                Identificatore = new Segnatura.IdentificatoreType()
                {
                    CodiceAmministrazione = new Segnatura.CodiceIPA()
                    { 
                        Value = _amministrazione.PAI.Amministrazione.CodiceIPA,
                        descrizione = _amministrazione.PAI.Amministrazione.Denominazione.ToString()
                    },
                    CodiceAOO = new Segnatura.CodiceIPA()
                    {
                        Value = _amministrazione.PAI.AOO.CodiceIPA,
                        descrizione = _amministrazione.PAI.AOO.Denominazione.ToString()
                    },
                    CodiceRegistro = ((DatiRegistrazioneProtocollo)_datiRegistrazione).CodiceRegistro,
                    NumeroRegistrazione = ((DatiRegistrazioneProtocollo)_datiRegistrazione).NumeroProtocolloAsString,
                    DataRegistrazione = ((DatiRegistrazioneProtocollo)_datiRegistrazione).DataProtocollazione.Value
                },
                Oggetto = OggettoDelDocumento.Descrizione.ToString(),
                Classifica = new Segnatura.ClassificaType()
                {
                    Denominazione = _classifications.Any() ? _classifications[0].Name.ToString() : string.Empty,
                    Item = _classifications.Any() ? _classifications[0].Code : string.Empty
                }
            };

            #endregion

            #region Descrizione

            segnaturaInformaticaType.Descrizione = new Segnatura.DescrizioneType()
            {
                Mittente = GetSoggettoMittenteType(),
                Destinatario = GetDestinatarioType(),
                DocumentoPrimario = new DocumentoType()
                {
                    Impronta = new ImprontaType()
                    {
                        Value = _idDoc.ImprontaCrittograficaDelDocumento == null ? null : _idDoc.ImprontaCrittograficaDelDocumento.Impronta,
                        algoritmo = _idDoc.ImprontaCrittograficaDelDocumento == null ? null : _idDoc.ImprontaCrittograficaDelDocumento.Algoritmo
                    },
                    Descrizione = _oggettoDelDocumento.Descrizione.ToString(),
                    nomeFile = !string.IsNullOrEmpty(_idDoc.FileName) ? _idDoc.FileName : "empty.TXT",
                    mimeType = !string.IsNullOrEmpty(_idDoc.FileName) ? _idDoc.ContentType : "text/plain"
                },
                Allegato = GetDocumentoTypesAllegato()
            };

            #endregion
            return segnaturaInformaticaType.ToXmlString(true);
        }

        public virtual void AnnullaContatoreRepertorio(string idProfile, string idField, DateTime? date = null)
        {
            this.AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum.Livello2);
            this.AssertIdDocPrimarioAssociato();
            this.AssertDocumentoAmministrativoContatoreRepertorioAnnullato(idProfile, idField);

            this.ApplyChange(new ContatoreRepertorioAnnullatoEvent()
            {
                 IdProfile = idProfile,
                  IdField = idField,
                  Data = date
            });
        }

        public virtual void AssignDatiStampa(DatiStampa datiStampa)
        {
            this.AssertIdDocPrimarioAssociato();

            this.ApplyChange(new DatiStampaAssignedEvent
            {
                Id = this.Id,
                DatiStampa = datiStampa
            });
        }

        #endregion

        #region Private Members

        protected OggettoDelDocumento _oggettoDelDocumento;
        protected Registro? _registro;
        protected TipologiaFlussoEnum? _tipologiaFlusso;
        protected TipologieVisibilitaEnum? _tipologiaVisibilita;
        [JsonPropertyName("idDoc")]
        protected IdDoc? _idDoc;
        [JsonPropertyName("datiRegistrazione")]
        protected DatiRegistrazione? _datiRegistrazione;
        protected string? _tipologiaDocumentale;
        protected AmministrazioneCheEffettuaLaRegistrazione _amministrazione;
        protected Autore _autore;
        protected MezzoSpedizione? _mezzoSpedizione;
        protected Mittente _mittente;
        protected Mittente _mittenteIntermedio;
        protected List<Mittente> _mittentiMultipli;
        protected List<Destinatario> _destinatari;
        protected List<Destinatario> _destinatariCc;
        protected List<Assegnatario> _assegnatari;
        protected List<Operatore> _operatori;
        protected ResponsabileGestioneDocumentale _responsabileGestioneDocumentale;
        protected ResponsabileServizioProtocollo _responsabileServizioProtocollo;
        protected RUP _rup;
        protected SwProduttore _produttore;
        protected List<Allegato> _allegati;
        protected List<Aggregazione> _aggregazioni;
        protected IdDoc? _idDocPrimario;
        protected int? _tempoDiConservazione;
        protected List<Nota>? _note;
        protected Annullamento? _annullamento;
        protected Consolidamento? _consolidamento;
        protected IEntity<string>? _classificationOrAggPrincipale;
        protected DatiStampa? _datiStampa;

        protected virtual Segnatura.SoggettoType GetSoggettoMittenteType()
        {
            SoggettoType mittente = new SoggettoType();

            if(_mittente.PF != null)
            {
                mittente.Item = new PersonaFisicaType()
                {
                    Nome = _mittente.PF.Nome,
                    Cognome = _mittente.PF.Cognome,
                    Contatti = _mittente.PF.IndirizziDigitaliDiRiferimento.Count == 0 ? null : new ContattiType()
                    {
                        IndirizzoTelematico = new IndirizzoTelematicoType[]
                        {
                            new IndirizzoTelematicoType()
                            {
                                 tipo = IndirizzoTelematicoTypeTipo.smtp,
                                 Value = _mittente.PF.IndirizziDigitaliDiRiferimento[0],
                            }
                        }
                    }
                };
            }
            else if(_mittente.PG != null)
            {
                mittente.Item = new PersonaGiuridicaType()
                {
                    Denominazione = _mittente.PG.DenominazioneOrganizzazione.ToString(),
                    ContattiPersonaGiuridica = _mittente.PG.IndirizziDigitaliDiRiferimento.Count == 0 ? null : new ContattiType()
                    {
                        IndirizzoTelematico = new IndirizzoTelematicoType[]
                        {
                            new IndirizzoTelematicoType()
                            {
                                 tipo = IndirizzoTelematicoTypeTipo.smtp,
                                 Value = _mittente.PG.IndirizziDigitaliDiRiferimento[0],
                            }
                        }
                    }
                };
            }
            else if(_mittente.PAI != null)
            {
                mittente.Item = new AmministrazioneType()
                {
                    DenominazioneAmministrazione = _mittente.PAI.Amministrazione.Denominazione.ToString(),
                    CodiceIPAAmministrazione = new CodiceIPA()
                    {
                        Value = _mittente.PAI.Amministrazione.CodiceIPA,
                        descrizione = _mittente.PAI.Amministrazione.Denominazione.ToString()
                    },
                    CodiceIPAAOO = new CodiceIPA()
                    {
                        Value = _mittente.PAI.AOO.CodiceIPA,
                        descrizione = _mittente.PAI.AOO.Denominazione.ToString()
                    },
                    ContattiAmministrazione = !_amministrazione.PAI.IndirizziDigitaliDiRiferimento.Any() ? null : new ContattiType()
                    {
                         
                         IndirizzoTelematico = new IndirizzoTelematicoType[]
                         {
                            new IndirizzoTelematicoType()
                            {
                                 tipo = IndirizzoTelematicoTypeTipo.smtp,
                                 Value = _amministrazione.PAI.IndirizziDigitaliDiRiferimento[0],
                            }
                         }
                    },
                     ContattiUO = !_mittente.PAI.IndirizziDigitaliDiRiferimento.Any() ? null : new ContattiType()
                     {
                         IndirizzoTelematico = new IndirizzoTelematicoType[]
                         {
                            new IndirizzoTelematicoType()
                            {
                                 tipo = IndirizzoTelematicoTypeTipo.smtp,
                                 Value = _mittente.PAI.IndirizziDigitaliDiRiferimento[0],
                            }
                         }
                     }
                };
            }

            return mittente;
        }

        protected virtual DestinatarioType[] GetDestinatarioType()
        {
            List<DestinatarioType> destinatarioTypes = new List<DestinatarioType>();
            foreach (var dest in _destinatari)
            {
                destinatarioTypes.Add(GetSoggettoDestinatarioType(dest));              
            }

            foreach (var dest in _destinatariCc)
            {
                destinatarioTypes.Add(GetSoggettoDestinatarioType(dest, true));
            }

            return destinatarioTypes.ToArray();
        }

        protected virtual DestinatarioType GetSoggettoDestinatarioType(Destinatario destinatario, bool conoscenza = false)
        {
            DestinatarioType destinatarioType = new DestinatarioType();
            if (destinatario.PF != null)
            {
                destinatarioType.Item = new PersonaFisicaType()
                {
                    Nome = destinatario.PF.Nome,
                    Cognome = destinatario.PF.Cognome,
                    Contatti = destinatario.PF.IndirizziDigitaliDiRiferimento.Count == 0 ? null : new ContattiType()
                    {
                        IndirizzoTelematico = new IndirizzoTelematicoType[]
                        {
                            new IndirizzoTelematicoType()
                            {
                                tipo = IndirizzoTelematicoTypeTipo.smtp,
                                Value = destinatario.PF.IndirizziDigitaliDiRiferimento[0],
                            }
                        }
                    }
                };
            }

            if (destinatario.PG != null)
            {
                destinatarioType.Item = new PersonaGiuridicaType()
                {
                    Denominazione = destinatario.PG.DenominazioneOrganizzazione.ToString(),
                    ContattiPersonaGiuridica = destinatario.PG.IndirizziDigitaliDiRiferimento.Count == 0 ? null : new ContattiType()
                    {
                        IndirizzoTelematico = new IndirizzoTelematicoType[]
                        {
                            new IndirizzoTelematicoType()
                            {
                                tipo = IndirizzoTelematicoTypeTipo.smtp,
                                Value = destinatario.PG.IndirizziDigitaliDiRiferimento[0],
                            }
                        }
                    }
                };
            }

            if (destinatario.PAI != null)
            {
                destinatarioType.Item = new AmministrazioneType()
                {
                    DenominazioneAmministrazione = destinatario.PAI.Amministrazione.Denominazione.ToString(),
                    CodiceIPAAmministrazione = new CodiceIPA()
                    {
                        Value = destinatario.PAI.Amministrazione.CodiceIPA
                    },
                    ContattiAmministrazione = destinatario.PAI.IndirizziDigitaliDiRiferimento.Count == 0 ? null : new ContattiType()
                    {

                        IndirizzoTelematico = new IndirizzoTelematicoType[]
                        {
                            new IndirizzoTelematicoType()
                            {
                                tipo = IndirizzoTelematicoTypeTipo.smtp,
                                Value = destinatario.PAI.IndirizziDigitaliDiRiferimento[0],
                            }
                        }
                    },
                    ContattiUO = new ContattiType()
                    {
                        IndirizzoTelematico = destinatario.PAI.IndirizziDigitaliDiRiferimento.Count == 0 ? null : new IndirizzoTelematicoType[]
                        {
                            new IndirizzoTelematicoType()
                            {
                                tipo = IndirizzoTelematicoTypeTipo.smtp,
                                Value = destinatario.PAI.IndirizziDigitaliDiRiferimento[0],
                            }
                        }
                    }
                };
            }
            destinatarioType.confermaRicezione = true;
            destinatarioType.perConoscenza = conoscenza;

            return destinatarioType;
        }

        protected virtual DocumentoType[] GetDocumentoTypesAllegato()
        {
            List<DocumentoType> documentoTypes = new List<DocumentoType>();

            int i = 1;
            foreach (var allegato in _allegati)
            {
                if (allegato.TipologiaAllegato == TipologieAllegatiEnum.Utente
                    || allegato.TipologiaAllegato == TipologieAllegatiEnum.SistemiEsterni
                    || allegato.TipologiaAllegato == TipologieAllegatiEnum.Derivati)
                {
                    documentoTypes.Add(new DocumentoType()
                    {
                        Impronta = string.IsNullOrEmpty(allegato.IdDoc.FileName) ? null : new ImprontaType()
                        {
                            Value = allegato.IdDoc.ImprontaCrittograficaDelDocumento.Impronta,
                            algoritmo = allegato.IdDoc.ImprontaCrittograficaDelDocumento.Algoritmo
                        },
                        Descrizione = allegato.Descrizione.ToString(),
                        nomeFile = !string.IsNullOrEmpty(allegato.IdDoc.FileName) ? allegato.IdDoc.FileName : $"Allegato_{i}",
                        mimeType = !string.IsNullOrEmpty(allegato.IdDoc.FileName) ? allegato.IdDoc.ContentType : "text/plain"
                    });
                }

                i++;
            }

            return documentoTypes.ToArray();
        }

        protected virtual bool IsDocumentoAmministrativoInformaticoRegistrato()
        {
            return (this.GetUncommittedChanges().Count(e => e.GetType() == typeof(DatiRegistrazioneProtocolloAssignedEvent)) == 0
                    && this.DatiRegistrazione?.TipologiaFlusso != TipologiaFlussoEnum.I
                    && (this.DatiRegistrazione?.IsRegistrato ?? false));
        }

        protected virtual void AssertDocumentoAmministrativoConsolidato(StatiConsolidamentoEnum stato)
        {
            if (this._consolidamento != null)
                if (stato <= this._consolidamento.Stato)
                    throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentoAmministrativoConsolidato, ErrorDescriptions.ResourceManager);
        }

        protected virtual void AssertIdDocPrimarioAssociato()
        {
            if (this._idDocPrimario != null)
                throw new NotSupportedPi3Exception(ErrorDescriptions.IdDocumentoPrimarioAssociato, ErrorDescriptions.ResourceManager);
        }

        protected virtual void AssertIdDocPrimarioNonAssociato()
        {
            if (this._idDocPrimario == null)
                throw new NotSupportedPi3Exception(ErrorDescriptions.IdDocumentoPrimarioNonAssociato, ErrorDescriptions.ResourceManager);
        }

        protected virtual void AssertDocumentoAmministrativoInformaticoRegistrato()
        {
            if (this.IsDocumentoAmministrativoInformaticoRegistrato())
                throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentoAmministrativoGiaRegistrato, ErrorDescriptions.ResourceManager);
        }

        protected virtual void AssertDocumentoAmministrativoInformaticoAnnullato()
        {
            if (this.GetUncommittedChanges().Count(e => e.GetType() == typeof(AnnullatoEvent)) == 0 && this._annullamento != null)
                throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentoAmministrativoGiaAnnullato, ErrorDescriptions.ResourceManager);
        }

        protected virtual void AssertDocumentoAmministrativoContatoreRepertorioAnnullato(string idProfile, string idField)
        {
            if (this.GetUncommittedChanges().Count(e => e.GetType() == typeof(ContatoreRepertorioAnnullatoEvent)) == 0)
            {
                var profile  = this.FindProfile(idProfile);

                var field = profile.Fields.FirstOrDefault(f => f.Id.Equals(idField, StringComparison.InvariantCultureIgnoreCase) && f.Value.GetType() == typeof(ContatoreRepertorioFieldValue));
                if (field == null)
                    throw new CampoContatoreRepertorioNotFoundPi3Exception(idField);

                if(((ContatoreRepertorioFieldValue)field.Value).DataAnnullamento != null)                    
                    throw new NotSupportedPi3Exception(ErrorDescriptions.ContatoreRepertorioGiaAnnullato, ErrorDescriptions.ResourceManager);
            }
        }

        protected virtual void AssertDocumentoAmministrativoInLibroFirma()
        {
            if (this.InLibroFirma)
                throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentoAmministrativoInLibroFirma, ErrorDescriptions.ResourceManager);
        }

        protected virtual void AssertReserved()
        {
            if (this.Reserved)
                throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentoAmministrativoGiaRiservato, ErrorDescriptions.ResourceManager);
        }

        protected virtual void AssertDocumentoAmministrativoPredisposto()
        {
            if (this._tipologiaFlusso != null)
                    throw new NotSupportedPi3Exception(ErrorDescriptions.DocumentoAmministrativoPredisposto, ErrorDescriptions.ResourceManager);
        }
        protected virtual void Handle(DocumentoAmministrativoCreatedEvent @event)
        {
            base.Handle((ElementCreatedEvent)@event);

            this._mittentiMultipli = new List<Mittente>();
            this._destinatari = new List<Destinatario>();
            this._destinatariCc = new List<Destinatario>();
            this._assegnatari = new List<Assegnatario>();
            this._operatori = new List<Operatore>();
            this._allegati = new List<Allegato>();
            this._aggregazioni = new List<Aggregazione>();
            this._oggettoDelDocumento = @event.OggettoDelDocumento;
            if (@event.DatiRegistro! != null!)
                this._registro = new Registro(@event.DatiRegistro.IdRegistro, @event.DatiRegistro.CodiceRegistro, @event.DatiRegistro.DescrizioneRegistro);
            this._tipologiaFlusso = @event.TipologiaFlusso;
            this._tipologiaVisibilita = @event.TipologiaVisibilita;
            this._idDocPrimario = @event.IdDocPrimario;
            this.IdParentDocument = @event?.IdDocPrimario?.Identiticativo;
            this._note = new List<Nota>();
        }

        protected virtual void Handle(OggettoDelDocumentoChangedEvent @event)
        {
            this._oggettoDelDocumento = @event.NewOggettoDelDocumento;
        }

        protected virtual void Handle(IdDocAssignedEvent @event)
        {
            this._idDoc = @event.IdDoc;
        }

        protected virtual void Handle(DatiRegistrazioneProtocolloAssignedEvent @event)
        {
            this._datiRegistrazione = @event.DatiRegistrazione;
        }

        protected virtual void Handle(DatiRegistrazioneRepertorioAssignedEvent @event)
        {
            this._datiRegistrazione = @event.DatiRegistrazione;
        }

        protected virtual void Handle(TipologiaDocumentaleAssignedEvent @event)
        {
            this._tipologiaDocumentale = @event.TipologiaDocumentale;
        }

        protected virtual void Handle(AmministrazioneAssignedEvent @event)
        {
            this._amministrazione = @event.Amministrazione;
        }

        protected virtual void Handle(AutoreAssignedEvent @event)
        {
            this._autore = @event.Autore;
        }

        protected virtual void Handle(MezzoSpedizioneAssignedEvent @event)
        {
            if (string.IsNullOrWhiteSpace(@event.IdMezzoSpedizione))
                this._mezzoSpedizione = null;
            else
                this._mezzoSpedizione = new MezzoSpedizione(@event.IdMezzoSpedizione, @event.Descrizione);
        }

        protected virtual void Handle(MezzoSpedizioneRemovedEvent @event)
        {
            this._mezzoSpedizione = null;
        }

        protected virtual void Handle(MittenteAssignedEvent @event)
        {
            this._mittente = @event.Mittente;
        }

        protected virtual void Handle(MittenteChangedEvent @event)
        {
            this._mittente = @event.Mittente;
        }

        protected virtual void Handle(MittenteMultiploAddedEvent @event)
        {
            this._mittentiMultipli.Add(@event.Mittente);
        }

        protected virtual void Handle(MittenteMultiploRemovedEvent @event)
        {
            if (this._mittentiMultipli.Contains(@event.Mittente))
                this._mittentiMultipli.Remove(@event.Mittente);
        }

        protected virtual void Handle(MittenteIntermedioAssignedEvent @event)
        {
            this._mittenteIntermedio = @event.Mittente;
        }

        protected virtual void Handle(MittenteIntermedioRemovedEvent @event)
        {
            this._mittenteIntermedio = null;
        }

        protected virtual void Handle(DestinatarioAddedEvent @event)
        {
            this._destinatari.Add(@event.Destinatario);
        }

        protected virtual void Handle(DestinatarioRemovedEvent @event)
        {
            if (this._destinatari.Contains(@event.Destinatario))
                this._destinatari.Remove(@event.Destinatario);
        }

        protected virtual void Handle(DestinatarioCcAddedEvent @event)
        {
            this._destinatariCc.Add(@event.Destinatario);
        }

        protected virtual void Handle(DestinatarioCcRemovedEvent @event)
        {
            if (this._destinatariCc.Contains(@event.Destinatario))
                this._destinatariCc.Remove(@event.Destinatario);
        }

        protected virtual void Handle(AssegnatarioAddedEvent @event)
        {
            this._assegnatari.Add(@event.Assegnatario);
        }

        protected virtual void Handle(OperatoreAddedEvent @event)
        {
            this._operatori.Add(@event.Operatore);
        }

        protected virtual void Handle(ResponsabileGestioneDocumentaleAssignedEvent @event)
        {
            this._responsabileGestioneDocumentale = @event.ResponsabileGestioneDocumentale;
        }

        protected virtual void Handle(ResponsabileServizioProtocolloAssignedEvent @event)
        {
            this._responsabileServizioProtocollo = @event.ResponsabileServizioProtocollo;
        }

        protected virtual void Handle(RUPAssignedEvent @event)
        {
            this._rup = @event.RUP;
        }

        protected virtual void Handle(SwProduttoreAssignedEvent @event)
        {
            this._produttore = @event.SwProduttore;
        }

        protected virtual void Handle(AllegatoAddedEvent @event)
        {
            this._allegati.Add(@event.Allegato);
        }

        protected virtual void Handle(TipologiaVisibilitaChangedEvent @event)
        {
            this._tipologiaVisibilita = @event.TipologiaVisibilita;
        }

        protected virtual void Handle(AggFascicoloAddedEvent @event)
        {
            this._aggregazioni.Add(new Fascicolo(@event.IdFascicolo, @event.Denominazione));
        }

        protected virtual void Handle(AggFascicoloRemovedEvent @event)
        {
            if (this._aggregazioni.Any(a => a.Id == @event.IdFascicolo))
                this._aggregazioni.Remove(this._aggregazioni.First(a => a.Id == @event.IdFascicolo));
        }

        protected virtual void Handle(AggSerieDocumentaleAddedEvent @event)
        {
            this._aggregazioni.Add(new SerieDocumentale(@event.IdSerieDocumentale, @event.Denominazione));
        }

        protected virtual void Handle(AggSerieDocumentaleRemovedEvent @event)
        {
            if (this._aggregazioni.Any(a => a.Id == @event.IdSerieDocumentale))
                this._aggregazioni.Remove(this._aggregazioni.First(a => a.Id == @event.IdSerieDocumentale));
        }

        protected virtual void Handle(AggSerieDiFascicoliAddedEvent @event)
        {
            this._aggregazioni.Add(new SerieDiFascicoli(@event.IdSerieDiFascicoli, @event.Denominazione));
        }

        protected virtual void Handle(AggSerieDiFascicoliRemovedEvent @event)
        {
            if (this._aggregazioni.Any(a => a.Id == @event.IdSerieDiFascicoli))
                this._aggregazioni.Remove(this._aggregazioni.First(a => a.Id == @event.IdSerieDiFascicoli));
        }

        protected virtual void Handle(TempoDiConservazioneChangedEvent @event)
        {
            this._tempoDiConservazione = @event.TempoDiConservazione;
        }

        protected virtual void Handle(AnnullatoEvent @event)
        {
            this._annullamento = @event.Annullamento;
        }

        protected virtual void Handle(PredisposizioneAnnullataEvent @event)
        {
            this._tipologiaFlusso = null;
        }
        protected virtual void Handle(PredispostoEvent @event)
        {
            this._tipologiaFlusso = @event.TipologiaFlusso;
        }
        
        protected virtual void Handle(ProtocolloMittenteAssignedEvent @event)
        {
            this.ProtocolloMittente = @event.ProtocolloMittente;
        }

        protected virtual void Handle(ProtocolloEmergenzaAssignedEvent @event)
        {
            this.ProtocolloEmergenza = @event.ProtocolloEmergenza;
        }

        protected virtual void Handle(RegistrazioneRichiestaEvent @event)
        {
        }

        protected virtual void Handle(DocumentoConsolidatoEvent @event)
        {
            this._consolidamento = @event.Consolidamento;
        }

        protected virtual void Handle(ClassificationOrAggAsPrincipaleAssigned @event)
        {
            this._classificationOrAggPrincipale = @event.ClassificationOrAggPrincipale;
        }

        protected virtual void Handle(NotaAddedEvent @event)
        {
            this._note.Add(new Nota(@event.IdNota, @event.TipologiaVisibilita, @event.Testo));
        }

        protected virtual void Handle(DataScadenzaAssignedEvent @event)
        {
            this.DataScadenza = @event.NewDataScadenza;
        }

        protected virtual void Handle(InLibroFirmaAddedEvent @event)
        {
            this.InLibroFirma = true;
        }

        protected virtual void Handle(FromLibroFirmaRemovedEvent @event)
        {
            this.InLibroFirma = true;
        }

        protected virtual void Handle(DocumentoAmministrativoReservedEvent @event)
        {
            base.Handle((ContentElementReservedEvent)@event);

            this.ReservedIdGroup = @event.ReserveIdGroup;
            this.ReservedDocumentLocation = @event.DocumentLocation;
            this.ReservedMachineName = @event.MachineName;
        }

        protected override void Handle(ContentElementUnreservedEvent @event)
        {
            base.Handle(@event);

            this.ReservedIdGroup = null;
            this.ReservedDocumentLocation = null;
            this.ReservedMachineName = null;
        }

        protected virtual void Handle(NumeroPagineAllegatoChangedEvent @event)
        {
            this.NumeroPagineAllegato = @event.NewNumeroPagine;
        }

        protected virtual void Handle(TipologiaAllegatoChangedEvent @event)
        {
            this.TipologiaAllegato = @event.NewTipologiaAllegato;
        }

        protected virtual void Handle(ContatoreRepertorioAnnullatoEvent @event)
        {
            
        }

        protected virtual void Handle(DatiStampaAssignedEvent @event)
        {
            this._datiStampa = @event.DatiStampa;
        }
        #endregion
    }
}