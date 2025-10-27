// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.Note
{
    /// <summary>
    /// Dati di una nota associata ad un documento o fascicolo
    /// </summary>
    [Serializable()]
    [DataContract]
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
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Tipologia di visibilità della nota
        /// </summary>
        [System.ComponentModel.DefaultValue(TipiVisibilitaNotaEnum.Tutti)]
        [DataMember]
        public TipiVisibilitaNotaEnum TipoVisibilita { get; set; } = TipiVisibilitaNotaEnum.Tutti;

        /// <summary>
        /// Testo della nota
        /// </summary>
        [DataMember]
        public string Testo { get; set; }

        /// <summary>
        /// Data di creazione della nota
        /// </summary>
        [DataMember]
        public DateTime DataCreazione { get; set; }

        /// <summary>
        /// Dati dell'utente creatore della nota
        /// </summary>
        [DataMember]
        public InfoUtenteCreatoreNota UtenteCreatore { get; set; }

        /// <summary>
        /// Indica che, in base al tipo di visibilità 
        /// e alle impostazioni di security dell'oggetto contenitore,
        /// la nota è in sola lettura o meno
        /// </summary>
        [DataMember]
        public bool SolaLettura { get; set; } = false;

        /// <summary>
        /// Indica che la nota è stata impostata come da rimuovere.
        /// Il flag è necessario nell'ambito della politica di aggiornamento batch delle note.
        /// </summary>
        [DataMember]
        public bool DaRimuovere { get; set; } = false;

        /// <summary>
        /// Indica che la nota è stata impostata come da inserire.
        /// Il flag è necessario nell'ambito della politica di aggiornamento batch delle note.
        /// </summary>
        [DataMember]
        public bool DaInserire { get; set; } = false;

        [DataMember]
        public string IdPeopleDelegato { get; set; }
        [DataMember]
        public string DescrPeopleDelegato { get; set; }

        [DataMember]
        public string IdRfAssociato { get; set; }
    }

    /// <summary>
    /// Dati dell'utente creatore della nota
    /// </summary>
    [Serializable()]
    [DataContract]
    public class InfoUtenteCreatoreNota
    {
        /// <summary>
        /// Id dell'utente creatore della nota
        /// </summary>
        [DataMember]
        public string IdUtente { get; set; }

        /// <summary>
        /// Descrizione dell'utente creatore
        /// </summary>
        [DataMember]
        public string DescrizioneUtente { get; set; }

        /// <summary>
        /// Id del ruolo creatore della nota
        /// </summary>
        [DataMember]
        public string IdRuolo { get; set; }

        /// <summary>
        /// Descrizione del ruolo creatore
        /// </summary>
        [DataMember]
        public string DescrizioneRuolo { get; set; }
    }

    /// <summary>
    /// Identifica i dati dell'oggetto associato alla nota
    /// </summary>
    [Serializable()]
    [DataContract]
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
        [DataMember]
        [System.ComponentModel.DefaultValue(OggettiAssociazioniNotaEnum.Documento)]
        public OggettiAssociazioniNotaEnum TipoOggetto { get; set; } = OggettiAssociazioniNotaEnum.Documento;

        /// <summary>
        /// Id univoco dell'oggetto associato alla nota
        /// </summary>
        [DataMember]
        public string Id { get; set; }
    }

    /// <summary>
    /// Indentifica i filtri utili per la ricerca delle note
    /// </summary>
    [Serializable()]
    [DataContract]
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
        [DataMember]
        public TipiVisibilitaNotaEnum Visibilita { get; set; } = TipiVisibilitaNotaEnum.Tutti;

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Testo { get; set; } = string.Empty;

        /// <summary>
        /// Numero massimo di caratteri del campo "Testo" estratti
        /// </summary>
        [System.ComponentModel.DefaultValue(NUMERO_MASSIMO_CARATTERI)]
        [DataMember]
        public int NumeroMassimoCaratteriTesto { get; set; } = NUMERO_MASSIMO_CARATTERI;
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