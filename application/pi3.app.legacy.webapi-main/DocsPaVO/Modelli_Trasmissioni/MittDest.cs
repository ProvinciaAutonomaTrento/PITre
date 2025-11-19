// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace DocsPaVO.Modelli_Trasmissioni
{
	public class MittDest
	{
		public int SYSTEM_ID;
		public int ID_MODELLO;
		public string CHA_TIPO_MITT_DEST;
		public string VAR_COD_RUBRICA;
		public int ID_RAGIONE;
		public string CHA_TIPO_TRASM;
        public int SCADENZA;
		public string VAR_NOTE_SING;
		public string DESCRIZIONE;
		public string CHA_TIPO_URP;
		public int ID_CORR_GLOBALI;

        public DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm[] UTENTI_NOTIFICA = new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm[0];

        /// <summary>
        /// Se true, il modello trasmissione prevede di nascondere le versioni precedenti
        /// a quella corrente di un documento trasmesso
        /// </summary>
        /// <remarks>
        /// Applicabile solo ai modelli trasmissione per i documenti
        /// </remarks>
        public bool NASCONDI_VERSIONI_PRECEDENTI;

        /// <summary>
        /// Destinatario storicizzato
        /// </summary>
        [DefaultValue(false)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Ruolo inibito alla ricezione di trasmissioni
        /// </summary>
        [DefaultValue(false)]
        public bool Inhibited { get; set; }


	}
}
