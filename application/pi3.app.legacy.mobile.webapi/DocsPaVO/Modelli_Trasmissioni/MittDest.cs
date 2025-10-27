// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DocsPaVO.Modelli_Trasmissioni
{
    [DataContract]
	public class MittDest
	{
        [DataMember]
        public int SYSTEM_ID;
        [DataMember]
        public int ID_MODELLO;
        [DataMember]
        public string CHA_TIPO_MITT_DEST;
        [DataMember]
        public string VAR_COD_RUBRICA;
        [DataMember]
        public int ID_RAGIONE;
        [DataMember]
        public string CHA_TIPO_TRASM;
        [DataMember]
        public int SCADENZA;
        [DataMember]
        public string VAR_NOTE_SING;
        [DataMember]
        public string DESCRIZIONE;
        [DataMember]
        public string CHA_TIPO_URP;
        [DataMember]
        public int ID_CORR_GLOBALI;

        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm))]
        [DataMember]
        public System.Collections.ArrayList UTENTI_NOTIFICA = new System.Collections.ArrayList();

        /// <summary>
        /// Se true, il modello trasmissione prevede di nascondere le versioni precedenti
        /// a quella corrente di un documento trasmesso
        /// </summary>
        /// <remarks>
        /// Applicabile solo ai modelli trasmissione per i documenti
        /// </remarks>
        [DataMember]
        public bool NASCONDI_VERSIONI_PRECEDENTI;

        /// <summary>
        /// Destinatario storicizzato
        /// </summary>
        [DefaultValue(false)]
        [DataMember]
        public bool Disabled { get; set; }

        /// <summary>
        /// Ruolo inibito alla ricezione di trasmissioni
        /// </summary>
        [DefaultValue(false)]
        [DataMember]
        public bool Inhibited { get; set; }


	}
}
