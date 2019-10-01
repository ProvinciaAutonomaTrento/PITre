using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.Note
{
    /// <summary>
    /// Dati di una nota associata ad un documento o fascicolo
    /// </summary>
    [Serializable()]
    public class InfoNota
    {
        /// <summary>
        /// 
        /// </summary>
        public InfoNota()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testo"></param>
        public InfoNota(string testo)
        {
            this.Testo = testo;
        }

        /// <summary>
        /// Creazione nuova nota con visibilità tutti
        /// </summary>
        /// <param name="testo"></param>
        /// <param name="idUtente"></param>
        /// <param name="idRuolo"></param>
        public InfoNota(string testo, string idUtente, string idRuolo, string idPeopleDelegato) :
            this(testo, idUtente, idRuolo, TipiVisibilitaNotaEnum.Tutti, idPeopleDelegato)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testo"></param>
        /// <param name="idUtente"></param>
        /// <param name="idRuolo"></param>
        /// <param name="tipoVisibilita"></param>
        public InfoNota(string testo, string idUtente, string idRuolo, TipiVisibilitaNotaEnum tipoVisibilita, string idPeopleDelegato)
        {
            this.Testo = testo;
            this.UtenteCreatore = new InfoUtenteCreatoreNota();
            this.UtenteCreatore.IdUtente = idUtente;
            this.UtenteCreatore.IdRuolo = idRuolo;
            this.DaInserire = true;
            this.TipoVisibilita = tipoVisibilita;
            this.IdPeopleDelegato = idPeopleDelegato;
        }

        /// <summary>
        /// Id univoco della nota
        /// </summary>
        public string Id;

        /// <summary>
        /// Tipologia di visibilità della nota
        /// </summary>
        [System.ComponentModel.DefaultValue(TipiVisibilitaNotaEnum.Tutti)]
        public TipiVisibilitaNotaEnum TipoVisibilita = TipiVisibilitaNotaEnum.Tutti;

        /// <summary>
        /// Testo della nota
        /// </summary>
        public string Testo;

        /// <summary>
        /// Data di creazione della nota
        /// </summary>
        public DateTime DataCreazione;

        /// <summary>
        /// Dati dell'utente creatore della nota
        /// </summary>
        public InfoUtenteCreatoreNota UtenteCreatore;

        /// <summary>
        /// Indica che, in base al tipo di visibilità 
        /// e alle impostazioni di security dell'oggetto contenitore,
        /// la nota è in sola lettura o meno
        /// </summary>
        public bool SolaLettura = false;

        /// <summary>
        /// Indica che la nota è stata impostata come da rimuovere.
        /// Il flag è necessario nell'ambito della politica di aggiornamento batch delle note.
        /// </summary>
        public bool DaRimuovere = false;

        /// <summary>
        /// Indica che la nota è stata impostata come da inserire.
        /// Il flag è necessario nell'ambito della politica di aggiornamento batch delle note.
        /// </summary>
        public bool DaInserire = false;

        public string IdPeopleDelegato;
        public string DescrPeopleDelegato;

        public string IdRfAssociato;
    }

    /// <summary>
    /// Dati dell'utente creatore della nota
    /// </summary>
    [Serializable()]
    public class InfoUtenteCreatoreNota
    {
        /// <summary>
        /// Id dell'utente creatore della nota
        /// </summary>
        public string IdUtente;

        /// <summary>
        /// Descrizione dell'utente creatore
        /// </summary>
        public string DescrizioneUtente;

        /// <summary>
        /// Id del ruolo creatore della nota
        /// </summary>
        public string IdRuolo;

        /// <summary>
        /// Descrizione del ruolo creatore
        /// </summary>
        public string DescrizioneRuolo;
    }

    /// <summary>
    /// Identifica i dati dell'oggetto associato alla nota
    /// </summary>
    [Serializable()]
    public class AssociazioneNota
    {
        /// <summary>
        /// 
        /// </summary>
        public AssociazioneNota()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoOggetto"></param>
        /// <param name="id"></param>
        public AssociazioneNota(OggettiAssociazioniNotaEnum tipoOggetto, string id)
        {
            this.TipoOggetto = tipoOggetto;
            this.Id = id;
        }

        /// <summary>
        /// L'enumeration indica le tipologie di 
        /// oggetti associati ad una nota
        /// </summary>
        public enum OggettiAssociazioniNotaEnum
        {
            Documento,
            Fascicolo
        }

        /// <summary>
        /// Tipologia di oggetto associato alla nota
        /// </summary>
        [System.ComponentModel.DefaultValue(OggettiAssociazioniNotaEnum.Documento)]
        public OggettiAssociazioniNotaEnum TipoOggetto = OggettiAssociazioniNotaEnum.Documento;

        /// <summary>
        /// Id univoco dell'oggetto associato alla nota
        /// </summary>
        public string Id;
    }

    /// <summary>
    /// Indentifica i filtri utili per la ricerca delle note
    /// </summary>
    [Serializable()]
    public class FiltroRicercaNote
    {
        /// <summary>
        /// 
        /// </summary>
        private const int NUMERO_MASSIMO_CARATTERI = 30;

        public FiltroRicercaNote()
        { }

        /// <summary>
        /// Tipologia di visibilità della nota
        /// </summary>
        public TipiVisibilitaNotaEnum Visibilita = TipiVisibilitaNotaEnum.Tutti;

        /// <summary>
        /// 
        /// </summary>
        public string Testo = string.Empty;

        /// <summary>
        /// Numero massimo di caratteri del campo "Testo" estratti
        /// </summary>
        [System.ComponentModel.DefaultValue(NUMERO_MASSIMO_CARATTERI)]
        public int NumeroMassimoCaratteriTesto = NUMERO_MASSIMO_CARATTERI;
    }

    /// <summary>
    /// Tipologie di visibilità per una nota
    /// 
    /// Se "Tutte", saranno visualizzate le note inserite
    /// da chiunque abbia dato la visibilità a tutti
    /// 
    /// Se "RF", saranno visualizzate le note inserite dal ruolo appartenente al RF
    /// e quelle inserite da chiunque abbia dato la visibilità a tutti
    /// 
    /// Se "Ruolo", saranno visualizzate le note inserite dal ruolo stesso
    /// e quelle inserite da chiunque abbia dato la visibilità a tutti
    /// 
    /// Se "Personali", saranno visualizzate le note inserite dall'utente stesso,
    /// quelle inserite dagli utenti dello stesso ruolo, 
    /// e quelle inserite da chiunque abbia dato la visibilità a tutti
    /// </summary>
    public enum TipiVisibilitaNotaEnum
    {
        /// <summary>
        /// Nota visibile a tutti ruoli / utenti
        /// </summary>
        Tutti,
        /// <summary>
        /// Nota visibile ai soli ruoli appartenenti al RF
        /// </summary>
        RF,
        /// <summary>
        /// Nota visibile solo al ruolo creatore nota
        /// </summary>
        Ruolo,
        /// <summary>
        /// Nota visibile solo all'utente creatore nota
        /// </summary>
        Personale

    }
}