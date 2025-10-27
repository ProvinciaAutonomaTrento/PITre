// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace DocsPaVO.ProfilazioneDinamica
{
    /// <summary>
    /// Enumeration che consente di specificare, per i soli campi di profilazione dinamica di tipo stringa,
    /// la modalit� di ricerca del contenuto
    /// </summary>
    public enum TipoRicercaStringaEnum
    {
        /// <summary>
        /// Effettua la ricerca di parte di un testo su un campo stringa
        /// </summary>
        PARTE_DELLA_PAROLA,

        /// <summary>
        /// Effettua la ricerca di un testo esatto su un campo stringa
        /// </summary>
        PAROLA_INTERA,

        /// <summary>
        /// Effettua la ricerca di un testo sulla parte iniziale di un campo stringa
        /// </summary>
        PAROLA_INIZIA_CON,
    }

    [XmlInclude(typeof(DocsPaVO.ProfilazioneDinamica.TipoOggetto))]
    [XmlInclude(typeof(DocsPaVO.ProfilazioneDinamica.ValoreOggetto))]
    [Serializable()]
    [DataContract]
    public class OggettoCustom
    {
        [DataMember]
        public int SYSTEM_ID { get; set; } 

        [DataMember]
        public string DESCRIZIONE { get; set; } = string.Empty;
        [DataMember]
        public string INDIRIZZO { get; set; } = string.Empty;
        [DataMember]
        public string TELEFONO { get; set; } = string.Empty;
        [DataMember]
        public string INDIRIZZO_TELEFONO { get; set; } = string.Empty;
        //public string NOME = string.Empty;

        [DataMember]
        public string ORIZZONTALE_VERTICALE { get; set; } = string.Empty;
        [DataMember]
        public string CAMPO_OBBLIGATORIO { get; set; } = string.Empty;
        [DataMember]
        public string ASTERISCO_OBBLIGATORIETA { get; set; } = string.Empty;
        [DataMember]
        public string MULTILINEA { get; set; } = string.Empty;
        [DataMember]
        public string NUMERO_DI_LINEE { get; set; } = string.Empty;
        [DataMember]
        public string NUMERO_DI_CARATTERI { get; set; } = string.Empty;
        [DataMember]
        public string CAMPO_DI_RICERCA { get; set; } = string.Empty;
        [DataMember]
        public string VALORE_DATABASE { get; set; } = string.Empty;
        [DataMember]
        public string POSIZIONE { get; set; } = string.Empty;
        [DataMember]
        public string ID_RUOLO_DEFAULT { get; set; } = string.Empty;
        [DataMember]
        public string TIPO_RICERCA_CORR { get; set; } = string.Empty;

        //Campo per identificare in fase di archiviazione sul DB l'operazione che deve essere
        //effettuata per questo specifico componente : "DaAggiungere", "DaEliminare" o "DaAggiornare".
        [DataMember]
        public string TIPO_OPERAZIONE { get; set; } = string.Empty;

        //modifica del 18/09/2009
        //flag per visualizzare la colonna del numero di contatore di protocollo
        [DataMember]
        public string DA_VISUALIZZARE_RICERCA { get; set; } = string.Empty;
        //fine modifica del 18/09/2009

        [DataMember]
        public TipoOggetto TIPO { get; set; }

        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.ProfilazioneDinamica.ValoreOggetto))]
        [DataMember]
        public ArrayList ELENCO_VALORI { get; set; } = new ArrayList();

        [XmlArray()]
        [XmlArrayItem(typeof(string))]
        [DataMember]
        public ArrayList VALORI_SELEZIONATI { get; set; } = new ArrayList();

        [DataMember]
        public string RESETTA_CONTATORE_INIZIO_ANNO { get; set; }
        [DataMember]
        public string FORMATO_CONTATORE { get; set; }
        [DataMember]
        public string ANNO { get; set; }
        [DataMember]
        public string CAMPO_COMUNE { get; set; }

        //Propriet� aggiunte per la nuova gestione dei contatori
        [DataMember]
        public string TIPO_CONTATORE { get; set; } = string.Empty;
        [DataMember]
        public string CONTA_DOPO { get; set; } = string.Empty;
        [DataMember]
        public string REPERTORIO { get; set; } = string.Empty;
        [DataMember]
        public string ID_AOO_RF { get; set; } = string.Empty;
        [DataMember]
        public bool CONTATORE_DA_FAR_SCATTARE { get; set; } = false;
        //Propriet� aggiunte per il contatore custom da C.Fuccia
        [DataMember]
        public string DATA_INIZIO { get; set; } = string.Empty;
        [DataMember]
        public string DATA_FINE { get; set; } = string.Empty;
        [DataMember]
        public string ANNO_ACC { get; set; } = string.Empty;
        //propriet� aggiunta per la gestione manuale della sospensione
        // del contatore custom da C.Fuccia.
        [DataMember]
        public string SOSPESO { get; set; } = string.Empty;

        [DataMember]
        public string FORMATO_ORA { get; set; } = string.Empty;

        // Modifica Elaborazione XML da PIS req.2
        [DataMember]
        public string CAMPO_XML_ASSOC { get; set; } = string.Empty;
        [DataMember]
        public string OPZIONI_XML_ASSOC { get; set; } = string.Empty;
        /// <summary>
        /// Indica la modalit� di ricerca dei campi di profilazione dinamica di tipo stringa
        /// </summary>
        /// <remarks>
        /// Per mantenere la compatibilit� con il codice esistente, per default la modalit� di ricerca � PARTE_DELLA_PAROLA 
        /// </remarks>
        [DataMember]
        public TipoRicercaStringaEnum TIPO_RICERCA_STRINGA { get; set; } = TipoRicercaStringaEnum.PARTE_DELLA_PAROLA;

        //propriet� aggiunte per la gestione dei link
        [DataMember]
        public string TIPO_LINK { get; set; } = string.Empty;
        [DataMember]
        public string TIPO_OBJ_LINK { get; set; } = string.Empty;

        [DataMember]
        public string MODULO_SOTTOCONTATORE { get; set; } = string.Empty;
        [DataMember]
        public string VALORE_SOTTOCONTATORE { get; set; } = string.Empty;
        [DataMember]
        public string DATA_INSERIMENTO { get; set; } = string.Empty;
        //propriet� aggiunt per la gestione oggetti esterni
        [DataMember]
        public string CONFIG_OBJ_EST { get; set; } = string.Empty;
        [DataMember]
        public string CODICE_DB { get; set; } = string.Empty;
        [DataMember]
        public bool MANUAL_INSERT { get; set; }

        [DataMember]
        public string CONSOLIDAMENTO { get; set; } = string.Empty;
        [DataMember]
        public string CONSERVAZIONE { get; set; } = string.Empty;
        [DataMember]
        public string CONS_REPERTORIO { get; set; } = string.Empty;

        [DataMember]
        public string DATA_ANNULLAMENTO { get; set; } = string.Empty;

        [DataMember]
        public bool ESTENDI_STORICIZZATI { get; set; } = false;

        [DataMember]
        public string VAR_SEGNATURA { get; set; } = string.Empty;
        public OggettoCustom() { }

        public void gestisciCaratteriSpeciali()
        {
            DESCRIZIONE = DESCRIZIONE.Replace("'", "''");
            //_nome = _nome.Replace("'","''");
            ORIZZONTALE_VERTICALE = ORIZZONTALE_VERTICALE.Replace("'", "''");
            CAMPO_OBBLIGATORIO = CAMPO_OBBLIGATORIO.Replace("'", "''");
            MULTILINEA = MULTILINEA.Replace("'", "''");
            NUMERO_DI_LINEE = NUMERO_DI_LINEE.Replace("'", "''");
            NUMERO_DI_CARATTERI = NUMERO_DI_CARATTERI.Replace("'", "''");
            CAMPO_DI_RICERCA = CAMPO_DI_RICERCA.Replace("'", "''");
            TIPO_OPERAZIONE = TIPO_OPERAZIONE.Replace("'", "''");
            VALORE_DATABASE = VALORE_DATABASE.Replace("'", "''");
            CONFIG_OBJ_EST = CONFIG_OBJ_EST.Replace("'", "''");
            CODICE_DB=CODICE_DB.Replace("'", "''");
        }

        public void ritornaCaratteriSpeciali()
        {
            DESCRIZIONE = DESCRIZIONE.Replace("''", "'");
            //_nome = _nome.Replace("'","''");
            ORIZZONTALE_VERTICALE = ORIZZONTALE_VERTICALE.Replace("''", "'");
            CAMPO_OBBLIGATORIO = CAMPO_OBBLIGATORIO.Replace("''", "'");
            MULTILINEA = MULTILINEA.Replace("''", "'");
            NUMERO_DI_LINEE = NUMERO_DI_LINEE.Replace("''", "'");
            NUMERO_DI_CARATTERI = NUMERO_DI_CARATTERI.Replace("''", "'");
            CAMPO_DI_RICERCA = CAMPO_DI_RICERCA.Replace("''", "'");
            TIPO_OPERAZIONE = TIPO_OPERAZIONE.Replace("''", "'");
            VALORE_DATABASE = VALORE_DATABASE.Replace("''", "'");
            CONFIG_OBJ_EST = CONFIG_OBJ_EST.Replace("''", "'");
            CODICE_DB=CODICE_DB.Replace("''", "'");
        }
    }
}
