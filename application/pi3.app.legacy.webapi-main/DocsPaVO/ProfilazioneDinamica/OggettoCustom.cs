// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
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
    public class OggettoCustom
    {
        public int SYSTEM_ID;

        public string DESCRIZIONE = string.Empty;
        public string INDIRIZZO = string.Empty;
        public string TELEFONO = string.Empty;
        public string INDIRIZZO_TELEFONO = string.Empty;
        //public string NOME = string.Empty;

        public string ORIZZONTALE_VERTICALE = string.Empty;
        public string CAMPO_OBBLIGATORIO = string.Empty;
        public string ASTERISCO_OBBLIGATORIETA = string.Empty;
        public string MULTILINEA = string.Empty;
        public string NUMERO_DI_LINEE = string.Empty;
        public string NUMERO_DI_CARATTERI = string.Empty;
        public string CAMPO_DI_RICERCA = string.Empty;
        public string VALORE_DATABASE = string.Empty;
        public string POSIZIONE = string.Empty;
        public string ID_RUOLO_DEFAULT = string.Empty;
        public string TIPO_RICERCA_CORR = string.Empty;

        //Campo per identificare in fase di archiviazione sul DB l'operazione che deve essere
        //effettuata per questo specifico componente : "DaAggiungere", "DaEliminare" o "DaAggiornare".
        public string TIPO_OPERAZIONE = string.Empty;

        //modifica del 18/09/2009
        //flag per visualizzare la colonna del numero di contatore di protocollo
        public string DA_VISUALIZZARE_RICERCA = string.Empty;
        //fine modifica del 18/09/2009

        public TipoOggetto TIPO;

        public DocsPaVO.ProfilazioneDinamica.ValoreOggetto[] ELENCO_VALORI = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto[0];

        public string[] VALORI_SELEZIONATI = new string[0];

        public string RESETTA_CONTATORE_INIZIO_ANNO;
        public string FORMATO_CONTATORE;
        public string ANNO;
        public string CAMPO_COMUNE;

        //Propriet� aggiunte per la nuova gestione dei contatori
        public string TIPO_CONTATORE = string.Empty;
        public string CONTA_DOPO = string.Empty;
        public string REPERTORIO = string.Empty;
        public string ID_AOO_RF = string.Empty;
        public bool CONTATORE_DA_FAR_SCATTARE = false;
        //Propriet� aggiunte per il contatore custom da C.Fuccia
        public string DATA_INIZIO = string.Empty;
        public string DATA_FINE = string.Empty;
        public string ANNO_ACC = string.Empty;
        //propriet� aggiunta per la gestione manuale della sospensione
        // del contatore custom da C.Fuccia.
        public string SOSPESO = string.Empty;

        public string FORMATO_ORA = string.Empty;

        // Modifica Elaborazione XML da PIS req.2
        public string CAMPO_XML_ASSOC = string.Empty;
        public string OPZIONI_XML_ASSOC = string.Empty;
        /// <summary>
        /// Indica la modalit� di ricerca dei campi di profilazione dinamica di tipo stringa
        /// </summary>
        /// <remarks>
        /// Per mantenere la compatibilit� con il codice esistente, per default la modalit� di ricerca � PARTE_DELLA_PAROLA 
        /// </remarks>
        public TipoRicercaStringaEnum TIPO_RICERCA_STRINGA = TipoRicercaStringaEnum.PARTE_DELLA_PAROLA;

        //propriet� aggiunte per la gestione dei link
        public string TIPO_LINK = string.Empty;
        public string TIPO_OBJ_LINK = string.Empty;

        public string MODULO_SOTTOCONTATORE = string.Empty;
        public string VALORE_SOTTOCONTATORE = string.Empty;
        public string DATA_INSERIMENTO = string.Empty;
        //propriet� aggiunt per la gestione oggetti esterni
        public string CONFIG_OBJ_EST = string.Empty;
        public string CODICE_DB = string.Empty;
        public bool MANUAL_INSERT;

        public string CONSOLIDAMENTO = string.Empty;
        public string CONSERVAZIONE = string.Empty;
        public string CONS_REPERTORIO = string.Empty;

        public string DATA_ANNULLAMENTO = string.Empty;

        public bool ESTENDI_STORICIZZATI = false;

        public string VAR_SEGNATURA = string.Empty;

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
