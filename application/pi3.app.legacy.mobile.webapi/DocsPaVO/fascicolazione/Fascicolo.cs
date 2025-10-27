// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace DocsPaVO.fascicolazione
{
    /// <summary>
    /// </summary>
    [XmlInclude(typeof(DocsPaVO.Note.AssociazioneNota))]
    [XmlInclude(typeof(DocsPaVO.Note.FiltroRicercaNote))]
    [Serializable()]
    [DataContract]
    public class Fascicolo
    {

        [DataMember]
        public string idClassificazione { get; set; }

        [DataMember]
        public string descrizione { get; set; }

        [DataMember]
        public string codice { get; set; }

        [DataMember]
        public string systemID { get; set; }

        [DataMember]
        public string apertura { get; set; }

        [DataMember]
        public string chiusura { get; set; }

        [DataMember]
        public string stato { get; set; }

        [DataMember]
        public string tipo { get; set; }

        [DataMember]
        public string codUltimo { get; set; }

        [DataMember]
        public string dirittoUtente { get; set; }

        [DataMember]
        public string codLegislatura { get; set; }

        [DataMember]
        public string privato { get; set; }

        [DataMember]
        public string inConservazione { get; set; }

        [DataMember]
        public string inArchivio { get; set; }

        [DataMember]
        public string inScarto { get; set; }

        [DataMember]
        public string numMesiConservazione { get; set; }

        [DataMember]
        //modifica
        public string contatore { get; set; }
        //fine modifica
        //public string numMesiChiusura;
        // idRegistro del Fascicolo

        [DataMember]
        public string idRegistro { get; set; }
        /* idRegistroNodoTit: idRegistro del nodo di Titolario a cui il fascicolo appartiene
        se la funzionalit� di fascicolazione multi registro non � abilitata
        idRegistro e codiceRegistroNodoTit devono necessariamente coincidere */

        [DataMember]
        public string idRegistroNodoTit { get; set; }
        /* codiceRegistroNodoTit: codice del registro del nodo di Titolario a cui il fascicolo appartiene */

        [DataMember]
        public string codiceRegistroNodoTit { get; set; }
        //Modifiche per Locazione Fisica

        [DataMember]
        public string idUoLF { get; set; }

        [DataMember]
        public string dtaLF { get; set; }

        [DataMember]
        public string descrizioneUOLF { get; set; }

        [DataMember]
        public string varCodiceRubricaLF { get; set; }

        [DataMember]
        public DocsPaVO.utente.Corrispondente ufficioReferente { get; set; }

        [DataMember]
        public CreatoreFascicolo creatoreFascicolo { get; set; }

        [DataMember]
        public bool daAggiornareUfficioReferente { get; set; }

        [DataMember]
        public string accessRights { get; set; }

        [DataMember]
        public string codiceGerarchia { get; set; }

        [DataMember]
        public string sicurezzaUtente { get; set; }

        /// <summary>
        /// Codice univoco dell'applicazione di appartenenza
        /// </summary>

        [DataMember]
        public string codiceApplicazione { get; set; } = string.Empty;

        /// <summary>
        /// 1 se il fascicolo � in ADL
        /// </summary>

        [DataMember]
        public string InAreaLavoro { get; set; }

        // Ultima nota fascicolo in formato stringa

        [DataMember]
        public string ultimaNota { get; set; }


        [DataMember]
        public DocsPaVO.ProfilazioneDinamica.Templates template { get; set; }

        /// <summary>
        /// Se true, per il fascicolo esiste un corrispondente cartaceo in archivio
        /// </summary>

        [DataMember]
        public bool cartaceo { get; set; } = false;

        [DataMember]
        public string idTitolario { get; set; }

        [DataMember]
        public string controllato { get; set; }

        [DataMember]
        public string isFascPrimaria { get; set; }

        /// <summary>
        /// Se � true, � consentita la classificazione
        /// </summary>

        [DataMember]
        public string isFascConsentita { get; set; }

        /// <summary>
        /// Se � true, � consentita la fascicolazione
        /// </summary>

        [DataMember]
        public bool isFascicolazioneConsentita { get; set; } = true;

        /// <summary>
        /// Note del fascicolo
        /// </summary>

        [DataMember]
        public DocsPaVO.Note.InfoNota[] noteFascicolo { get; set; } = new DocsPaVO.Note.InfoNota[0];


        [DataMember]
        public DocsPaVO.fascicolazione.Folder folderSelezionato { get; set; }

        /// <summary>
        /// Rappresentazione stringa del fascicolo
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return string.Format("{0} - {1}", this.codice, this.descrizione);
            return string.Format("{0}", this.codice);
        }


        [DataMember]
        public string dtaScadenza { get; set; }


        [DataMember]
        public string numFascicolo { get; set; } = string.Empty;


        [DataMember]
        public ChiudeFascicolo chiudeFascicolo { get; set; }

        /// <summary>
        /// Informazioni sulla atipicita del documento
        /// </summary>

        [DataMember]
        public DocsPaVO.Security.InfoAtipicita InfoAtipicita { get; set; }


        [DataMember]
        public string dataCreazione { get; set; }

        //
        // Mev Ospedale Maggiore Policlinico

        /// <summary>
        /// SystemID del Nodo di titolario selezionato per la Funzionalit� di riclassificazione; Non obbligatorio
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        //[System.Xml.Serialization.XmlIgnore()]

        [DataMember]
        public string NodoRiclassificazione_SystemID { get; set; } = string.Empty;

        /// <summary>
        /// Codice del Nodo di titolario selezionato per la Funzionalit� di riclassificazione; Non obbligatorio
        /// </summary>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        //[System.Xml.Serialization.XmlIgnore()]

        [DataMember]
        public string NodoRiclassificazione_Codice { get; set; } = string.Empty;

        // End Mev Ospedale Maggiore Policlinico
        //


        [DataMember]
        public bool HasStrutturaTemplate { get; set; }


        [DataMember]
        public bool pubblico { get; set; } = false;


        [DataMember]
        public PianoConservazione pianoConservazione { get; set; } = null;
    }
}
