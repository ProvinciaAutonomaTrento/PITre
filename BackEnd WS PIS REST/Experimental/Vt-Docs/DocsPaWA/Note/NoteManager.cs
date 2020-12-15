using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Collections;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.Note
{
    /// <summary>
    /// Class factory per la creazione degli oggetti "INoteManager"
    /// </summary>
    public sealed class NoteManagerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private NoteManagerFactory()
        { }

        /// <summary>
        /// Creazione istanza oggetto "INoteManager"
        /// </summary>
        /// <param name="tipoOggettoAssociato"></param>
        /// <returns></returns>
        public static INoteManager CreateInstance(OggettiAssociazioniNotaEnum tipoOggettoAssociato)
        {
            if (tipoOggettoAssociato == OggettiAssociazioniNotaEnum.Documento)
                return new SchedaDocumentoNoteManager(DocumentManager.getDocumentoInLavorazione());
            else if (tipoOggettoAssociato == OggettiAssociazioniNotaEnum.Fascicolo)
                return new FascicoloNoteManager(FascicoliManager.getFascicoloSelezionato());
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoOggettoAssociato"></param>
        /// <param name="containerSessionKey">
        /// Chiave di sessione dell'oggetto contenitore delle note
        /// </param>
        /// <returns></returns>
        public static INoteManager CreateInstance(OggettiAssociazioniNotaEnum tipoOggettoAssociato, string containerSessionKey)
        {
            if (HttpContext.Current.Session[containerSessionKey] != null)
            {
                if (tipoOggettoAssociato == OggettiAssociazioniNotaEnum.Documento)
                    return new SchedaDocumentoNoteManager(HttpContext.Current.Session[containerSessionKey] as SchedaDocumento);
                else if (tipoOggettoAssociato == OggettiAssociazioniNotaEnum.Fascicolo)
                    return new FascicoloNoteManager(HttpContext.Current.Session[containerSessionKey] as Fascicolo);
                else
                    return null;
            }
            else
                return CreateInstance(tipoOggettoAssociato);
        }
    }

    /// <summary>
    /// Interfaccia per la gestione delle note
    /// </summary>
    public interface INoteManager
    {
        InfoNota[] GetNote(FiltroRicercaNote filtroRicerca);
        InfoNota GetUltimaNota();
        string GetUltimaNotaAsString();
        InfoNota GetNota(string idNota);
        string GetNotaAsString(string idNota);
        InfoNota InsertNota(InfoNota nota);
        InfoNota UpdateNota(InfoNota nota);
        void DeleteNota(string idNota);
        bool CanDeleteNota(InfoNota nota);
        bool CanUpdateNota(InfoNota nota);
        int CountNote { get; }
        bool IsNoteReadOnly { get; }
        void ClearNote();
    }
    
    /// <summary>
    /// Gestione note in memoria
    /// </summary>
    public abstract class InMemoryNoteManager : INoteManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="note"></param>
        public InMemoryNoteManager()
        {
        }

        #region Public methods

        /// <summary>
        /// Inserimento di una nuova nota da associare ad un documento / fascicolo
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public InfoNota InsertNota(InfoNota nota)
        {
            nota.DaInserire = true;
            nota.Id = this.NewId();
            nota.DataCreazione = DateTime.Now;
            nota.UtenteCreatore = new InfoUtenteCreatoreNota();

            InfoUtente utente = UserManager.getInfoUtente();
            nota.UtenteCreatore.IdUtente = utente.idPeople;
            nota.UtenteCreatore.DescrizioneUtente = utente.userId;
            if (utente.delegato != null)
            {
                nota.IdPeopleDelegato = utente.delegato.idPeople;
                nota.DescrPeopleDelegato = utente.delegato.userId;
            }
            Ruolo ruolo = UserManager.getRuolo();
            nota.UtenteCreatore.IdRuolo = ruolo.idGruppo;
            nota.UtenteCreatore.DescrizioneRuolo = ruolo.descrizione;

            // Inserimento della nota nella scheda documento (come primo elemento della lista, 
            // solo se il testo della nota da inserire ed il tipo di visibilità sono differenti
            // da quelli dell'ultima nota inserita)
            if (!String.IsNullOrEmpty(nota.Testo.Trim()) &&
                (this.Note.Length == 0 ||
                !this.Note[0].Testo.Trim().Equals(nota.Testo.Trim())
                || !this.Note[0].TipoVisibilita.Equals(nota.TipoVisibilita)))
            {
                List<InfoNota> note = new List<InfoNota>(this.Note);
                note.Insert(0, nota);
                this.Note = note.ToArray();
            }

            return nota;
        }

        /// <summary>
        /// Aggiornamento di una nota esistente
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public InfoNota UpdateNota(InfoNota nota)
        {
            for (int i = 0; i < this.Note.Length; i++)
            {
                if (this.Note[i].Id.Equals(nota.Id))
                {
                    this.Note[i] = nota;
                    break;
                }
            }

            return nota;
        }

        /// <summary>
        /// Verifica se la nota può essere cancellata
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public bool CanDeleteNota(InfoNota nota)
        {
            return !nota.SolaLettura;
        }

        /// <summary>
        /// Verifica se la nota può essere cancellata
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public bool CanUpdateNota(InfoNota nota)
        {
            return !nota.SolaLettura;
        }

        /// <summary>
        /// Cancellazione di una nota esistente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        public void DeleteNota(string idNota)
        {
            foreach (InfoNota nota in this.Note)
            {
                if (nota.Id.Equals(idNota))
                {
                    // Impostazione della nota come "DaRimuovere"
                    nota.DaRimuovere = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Reperimento di una nota esistente, solo se non è marcata come da rimuovere
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        /// <returns></returns>
        public InfoNota GetNota(string idNota)
        {
            foreach (InfoNota nota in this.Note)
                if (nota.Id.Equals(idNota) && !nota.DaRimuovere)
                    return nota;
            return null;
        }

        /// <summary>
        /// Reperimento di una nota esistente, solo se non è marcata come da rimuovere
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        /// <returns></returns>
        public string GetNotaAsString(string idNota)
        {
            InfoNota nota = this.GetNota(idNota);
            if (nota != null)
                return nota.Testo;
            else
                return string.Empty;
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento / fascicolo in ordine cronologico
        /// </summary>
        /// <returns></returns>
        public string GetUltimaNotaAsString()
        {
            InfoNota nota = this.GetUltimaNota();
            if (nota != null)
                return nota.Testo;
            else
                return string.Empty;
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento / fascicolo in ordine cronologico
        /// </summary>
        /// <returns></returns>
        public InfoNota GetUltimaNota()
        {
            InfoNota retValue = null;

            foreach (InfoNota nota in this.Note)
            {
                if (!nota.DaRimuovere)
                {
                    retValue = nota;
                    break;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento della lista delle note associate ad un documento / fascicolo
        /// </summary>
        /// <param name="filtroRicerca"></param>
        /// <returns></returns>
        public InfoNota[] GetNote(FiltroRicercaNote filtroRicerca)
        {
            List<InfoNota> note = new List<InfoNota>();
            foreach (InfoNota item in this.Note)
                if (!item.DaRimuovere)
                    note.Add(item);
            return note.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public int CountNote 
        {
            get
            {
                int count = 0;
                foreach (InfoNota item in this.Note)
                    if (!item.DaRimuovere)
                        count++;
                return count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract bool IsNoteReadOnly
        {
            get;
        }

        /// <summary>
        /// Rimozione di tutte le note dell'oggetto
        /// </summary>
        public abstract void ClearNote();

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected abstract InfoNota[] Note
        {
            get;
            set;
            //{
            //    if (this._note == null)
            //        this._note = new List<InfoNota>();
            //    return this._note;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string NewId()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion
    }

    /// <summary>
    /// Gestione note legate alla scheda documento correntemente in lavorazione
    /// </summary>
    public class SchedaDocumentoNoteManager : InMemoryNoteManager
    {
        /// <summary>
        /// 
        /// </summary>
        private SchedaDocumento _schedaDocumento = null;

        /// <summary>
        /// 
        /// </summary>
        public SchedaDocumentoNoteManager(SchedaDocumento schedaDocumento)
        {
            this._schedaDocumento = schedaDocumento;
        }

        /// <summary>
        /// Verifica se il documento è in stato readonly
        /// </summary>
        public override bool IsNoteReadOnly
        {
            get
            {
                return (this.SchedaDocumento != null && !string.IsNullOrEmpty(this.SchedaDocumento.systemId) &&
                        ((!string.IsNullOrEmpty(this.SchedaDocumento.inCestino) && this.SchedaDocumento.inCestino.Equals("1")) || 
                            this.SchedaDocumento.accessRights == "45"));
            }
        }

        /// <summary>
        /// Rimozione di tutte le note dell'oggetto
        /// </summary>
        public override void ClearNote()
        {
            //if (this.SchedaDocumento != null)
            //    this.SchedaDocumento.noteDocumento = new InfoNota[0];
        }

        #region Protected methods

        /// <summary>
        /// Note del documento
        /// </summary>
        protected override InfoNota[] Note
        {
            get
            {
                if (this.SchedaDocumento.noteDocumento == null)
                    return new InfoNota[0];
                else
                    return this.SchedaDocumento.noteDocumento;
            }
            set
            {
                this.SchedaDocumento.noteDocumento = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected SchedaDocumento SchedaDocumento
        {
            get
            {
                return this._schedaDocumento;
            }
            set
            {
                this._schedaDocumento = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Gestione note legate alla scheda fascicolo correntemente in lavorazione
    /// </summary>
    public class FascicoloNoteManager : InMemoryNoteManager
    {
        /// <summary>
        /// 
        /// </summary>
        private Fascicolo _fascicolo = null;

        /// <summary>
        /// 
        /// </summary>
        public FascicoloNoteManager(Fascicolo fascicolo)
        {
            this.Fascicolo = fascicolo;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsNoteReadOnly
        {
            get
            {
                return (this.Fascicolo != null &&
                        !string.IsNullOrEmpty(this.Fascicolo.systemID) &&
                        this.Fascicolo.accessRights == "45");
            }
        }

        /// <summary>
        /// Rimozione di tutte le note dell'oggetto
        /// </summary>
        public override void ClearNote()
        {
            //this.Fascicolo.noteFascicolo = new InfoNota[0];
        }

        #region Protected methods

        /// <summary>
        /// Note del fascicolo
        /// </summary>
        protected override InfoNota[] Note
        {
            get
            {
                if (this.Fascicolo.noteFascicolo == null)
                    return new InfoNota[0];
                else
                    return this.Fascicolo.noteFascicolo;
            }
            set
            {
                this.Fascicolo.noteFascicolo = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Fascicolo Fascicolo
        {
            get
            {
                return this._fascicolo;
            }
            set
            {
                this._fascicolo = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Gestione delle note tramite webservices
    /// </summary>
    public class NoteManager : INoteManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oggettoAssociato"></param>
        public NoteManager(AssociazioneNota oggettoAssociato)
        {
            this._oggettoAssociato = oggettoAssociato;
        }

        #region Public methods

        /// <summary>
        /// Aggiornamento batch delle note
        /// </summary>
        /// <param name="oggettoAssociato"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public static InfoNota[] Update(AssociazioneNota oggettoAssociato, InfoNota[] note)
        {
            return (new DocsPAWA.DocsPaWR.DocsPaWebService()).UpdateNote(UserManager.getInfoUtente(), oggettoAssociato, note);
        }

        /// <summary>
        /// Inserimento di una nuova nota da associare ad un documento / fascicolo
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public InfoNota InsertNota(InfoNota nota)
        {
            return this.WebServiceInstance.InsertNota(this.InfoUtente, this._oggettoAssociato, nota);
        }

        /// <summary>
        /// Aggiornamento di una nota esistente
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public InfoNota UpdateNota(InfoNota nota)
        {
            return this.WebServiceInstance.UpdateNota(this.InfoUtente, nota);
        }

        /// <summary>
        /// Verifica se la nota può essere cancellata
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public bool CanDeleteNota(InfoNota nota)
        {
            // return (nota.UtenteCreatore.IdUtente.Equals(UserManager.getInfoUtente().idPeople));

            return nota.SolaLettura;
        }

        /// <summary>
        /// Verifica se la nota può essere modificata dall'utente
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        public bool CanUpdateNota(InfoNota nota)
        {
            // return (nota.UtenteCreatore.IdUtente.Equals(UserManager.getInfoUtente().idPeople));

            return nota.SolaLettura;
        }

        /// <summary>
        /// Cancellazione di una nota esistente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        public void DeleteNota(string idNota)
        {
            this.WebServiceInstance.DeleteNota(this.InfoUtente, idNota);
        }

        /// <summary>
        /// Reperimento di una nota esistente, solo se non è marcata come da rimuovere
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        /// <returns></returns>
        public string GetNotaAsString(string idNota)
        {
            InfoNota nota = this.GetNota(idNota);
            if (nota != null)
                return nota.Testo;
            else
                return string.Empty;
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento / fascicolo in ordine cronologico
        /// </summary>
        /// <returns></returns>
        public string GetUltimaNotaAsString()
        {
            InfoNota nota = this.GetUltimaNota();
            if (nota != null)
                return nota.Testo;
            else
                return string.Empty;
        }

        /// <summary>
        /// Reperimento di una nota esistente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idNota"></param>
        /// <returns></returns>
        public InfoNota GetNota(string idNota)
        {
            return this.WebServiceInstance.GetNota(this.InfoUtente, idNota);
        }

        /// <summary>
        /// Reperimento ultima nota inserita per un documento / fascicolo in ordine cronologico
        /// </summary>
        /// <returns></returns>
        public InfoNota GetUltimaNota()
        {
            return this.WebServiceInstance.GetUltimaNota(this.InfoUtente, this.OggettoAssociato);
        }

        /// <summary>
        /// Reperimento della lista delle note associate ad un documento / fascicolo
        /// </summary>
        /// <param name="filtroRicerca"></param>
        /// <returns></returns>
        public InfoNota[] GetNote(FiltroRicercaNote filtroRicerca)
        {
            return this.WebServiceInstance.GetNote(this.InfoUtente, this.OggettoAssociato, filtroRicerca);
        }

        /// <summary>
        /// 
        /// </summary>
        public int CountNote
        {
            get
            {
                return this.GetNote(null).Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNoteReadOnly 
        {
            get
            {   
                if (!string.IsNullOrEmpty(this.OggettoAssociato.Id))
                    return this.WebServiceInstance.IsNoteSolaLettura(UserManager.getInfoUtente(), this.OggettoAssociato);
                else
                    return false;
            }
        }

        /// <summary>
        /// Rimozione di tutte le note dell'oggetto
        /// </summary>
        public void ClearNote()
        {
        }


        
        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        private DocsPaWR.DocsPaWebService _webServiceInstance = null;

        /// <summary>
        /// 
        /// </summary>
        private AssociazioneNota _oggettoAssociato = null;

        /// <summary>
        /// Istanza webservice corrente
        /// </summary>
        protected DocsPaWR.DocsPaWebService WebServiceInstance
        {
            get
            {
                if (this._webServiceInstance == null)
                    this._webServiceInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
                return this._webServiceInstance;
            }
        }

        /// <summary>
        /// Reperimento oggetto InfoUtente corrente
        /// </
        protected InfoUtente InfoUtente
        {
            get
            {
                return UserManager.getInfoUtente();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected AssociazioneNota OggettoAssociato
        {
            get
            {
                return this._oggettoAssociato;
            }
            set
            {
                this._oggettoAssociato = value;
            }
        }

        #endregion
    }
}