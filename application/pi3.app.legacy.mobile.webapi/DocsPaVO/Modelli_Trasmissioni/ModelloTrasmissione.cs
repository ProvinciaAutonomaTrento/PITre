// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.Modelli_Trasmissioni
{
    [DataContract]
	public class ModelloTrasmissione
	{
        [DataMember]
		public int SYSTEM_ID { get; set; }
        [DataMember]
        public string ID_AMM { get; set; }
        [DataMember]
        public string NOME { get; set; }
        [DataMember]
        public string CHA_TIPO_OGGETTO { get; set; }
        [DataMember]
        public string ID_REGISTRO { get; set; }
        [DataMember]
        public string VAR_NOTE_GENERALI { get; set; }
        [DataMember]
        public string SINGLE { get; set; }
        [DataMember]
        public string ID_PEOPLE { get; set; }
        [DataMember]
        public string CEDE_DIRITTI { get; set; }
        [DataMember]
        public string ID_PEOPLE_NEW_OWNER { get; set; }
        [DataMember]
        public string ID_GROUP_NEW_OWNER { get; set; }
        [DataMember]
        public string CODICE { get; set; }
        [DataMember]
        public string NO_NOTIFY { get; set; } //il modello non attiva le notifiche in tdl e  non invia le notifiche vi mail
        [DataMember]
        public string MANTIENI_LETTURA { get; set; }
        [DataMember]
        public string MANTIENI_SCRITTURA { get; set; } // Aggiunto Per MEV Cessione Diritti - Mantieni Scrittura
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.Modelli_Trasmissioni.MittDest))]
        [DataMember]
        public System.Collections.ArrayList MITTENTE { get; set; } = new System.Collections.ArrayList();
		//public DocsPaVO.Modelli_Trasmissioni.MittDest MITTENTE;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.Modelli_Trasmissioni.RagioneDest))]
        [DataMember]
        public System.Collections.ArrayList RAGIONI_DESTINATARI { get; set; } = new System.Collections.ArrayList();

        /// <summary>
        /// Se true, il modello trasmissione prevede di nascondere le versioni precedenti
        /// a quella corrente di un documento trasmesso
        /// </summary>
        /// <remarks>
        /// Applicabile solo ai modelli trasmissione per i documenti
        /// </remarks>
        //public bool NASCONDI_VERSIONI_PRECEDENTI;

        /// <summary>
        /// True se il modello � valido
        /// </summary>
        [DataMember]
        public bool Valid { get; set; }

        /// <summary>
        /// Numero dei mittenti del modello. Attualmente viene utilizzata durante il caricamento
        /// delle informazioni sui modelli di trasmissione utente
        /// </summary>
        [DataMember]
        public int NumMittenti { get; set; }
	
    }
}