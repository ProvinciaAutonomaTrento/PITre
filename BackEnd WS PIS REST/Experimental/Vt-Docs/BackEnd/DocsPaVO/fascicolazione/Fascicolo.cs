using System;
using System.Collections;
using System.Xml.Serialization;


namespace DocsPaVO.fascicolazione
{
    /// <summary>
    /// </summary>
    [XmlInclude(typeof(DocsPaVO.Note.AssociazioneNota))]
    [XmlInclude(typeof(DocsPaVO.Note.FiltroRicercaNote))]
    [Serializable()]
    public class Fascicolo
    {
        public string idClassificazione;
        public string descrizione;
        public string codice;
        public string systemID;
        public string apertura;
        public string chiusura;
        public string stato;
        public string tipo;
        public string codUltimo;
        public string dirittoUtente;
        public string codLegislatura;
        public string privato;
        public string inConservazione;
        public string inArchivio;
        public string inScarto;
        public string numMesiConservazione;
        //modifica
        public string contatore;
        //fine modifica
        //public string numMesiChiusura;
        // idRegistro del Fascicolo
        public string idRegistro;
        /* idRegistroNodoTit: idRegistro del nodo di Titolario a cui il fascicolo appartiene
        se la funzionalità di fascicolazione multi registro non è abilitata
        idRegistro e codiceRegistroNodoTit devono necessariamente coincidere */
        public string idRegistroNodoTit;
        /* codiceRegistroNodoTit: codice del registro del nodo di Titolario a cui il fascicolo appartiene */
        public string codiceRegistroNodoTit;
        //Modifiche per Locazione Fisica
        public string idUoLF;
        public string dtaLF;
        public string descrizioneUOLF;
        public string varCodiceRubricaLF;
        public DocsPaVO.utente.Corrispondente ufficioReferente;
        public CreatoreFascicolo creatoreFascicolo;
        public bool daAggiornareUfficioReferente;
        public string accessRights;
        public string codiceGerarchia;
        public string sicurezzaUtente;

        /// <summary>
        /// Codice univoco dell'applicazione di appartenenza
        /// </summary>
        public string codiceApplicazione = string.Empty;

        /// <summary>
        /// 1 se il fascicolo è in ADL
        /// </summary>
        public string InAreaLavoro;

        // Ultima nota fascicolo in formato stringa
        public string ultimaNota;

        public DocsPaVO.ProfilazioneDinamica.Templates template;

        /// <summary>
        /// Se true, per il fascicolo esiste un corrispondente cartaceo in archivio
        /// </summary>
        public bool cartaceo = false;
        public string idTitolario;
        public string controllato;
        public string isFascPrimaria;

        /// <summary>
        /// Se è true, è consentita la classificazione
        /// </summary>
        public string isFascConsentita;

        /// <summary>
        /// Se è true, è consentita la fascicolazione
        /// </summary>
        public bool isFascicolazioneConsentita = true;

        /// <summary>
        /// Note del fascicolo
        /// </summary>
        public DocsPaVO.Note.InfoNota[] noteFascicolo = new DocsPaVO.Note.InfoNota[0];

        public DocsPaVO.fascicolazione.Folder folderSelezionato;

        /// <summary>
        /// Rappresentazione stringa del fascicolo
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return string.Format("{0} - {1}", this.codice, this.descrizione);
            return string.Format("{0}", this.codice);
        }

        public string dtaScadenza;

        public string numFascicolo = string.Empty;

        public ChiudeFascicolo chiudeFascicolo;

        /// <summary>
        /// Informazioni sulla atipicita del documento
        /// </summary>
        public DocsPaVO.Security.InfoAtipicita InfoAtipicita;

        public string dataCreazione;

        //
        // Mev Ospedale Maggiore Policlinico

        /// <summary>
        /// SystemID del Nodo di titolario selezionato per la Funzionalità di riclassificazione; Non obbligatorio
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        //[System.Xml.Serialization.XmlIgnore()]
        public string NodoRiclassificazione_SystemID = string.Empty;

        /// <summary>
        /// Codice del Nodo di titolario selezionato per la Funzionalità di riclassificazione; Non obbligatorio
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        //[System.Xml.Serialization.XmlIgnore()]
        public string NodoRiclassificazione_Codice = string.Empty;
     
        // End Mev Ospedale Maggiore Policlinico
        //

        public bool HasStrutturaTemplate;

        public bool pubblico = false;
    }
}
