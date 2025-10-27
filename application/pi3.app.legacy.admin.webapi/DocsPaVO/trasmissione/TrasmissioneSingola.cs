// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	public class TrasmissioneSingola 
	{
		public RagioneTrasmissione ragione;
		public Corrispondente corrispondenteInterno;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.trasmissione.TrasmissioneUtente))]
		public ArrayList trasmissioneUtente;
		
		public string noteSingole;
		public string tipoTrasm;
		public TipoDestinatario tipoDest;
		public static Hashtable tipoDestStringa;
		public string dataScadenza;
		public string systemId;
		public string idTrasmUtente;
		public bool daAggiornare = false;
		public bool daEliminare = false;

        /// <summary>
        /// Se true, i destinatari della trasmissione di un documento 
        /// non avranno la visibilit� sulle versioni precedenti a quella corrente
        /// </summary>
        /// <remarks>
        /// Per le trasmissioni fascicolo, � sempre false
        /// </remarks>
        public bool hideDocumentPreviousVersions = false;

		/// <summary>
		/// </summary>
		public TrasmissioneSingola()
		{
			trasmissioneUtente=new System.Collections.ArrayList();

			if(tipoDestStringa==null)
			{
				tipoDestStringa=new System.Collections.Hashtable();
				tipoDestStringa.Add(TipoDestinatario.GRUPPO,"G");
				tipoDestStringa.Add(TipoDestinatario.RUOLO,"R");	
				tipoDestStringa.Add(TipoDestinatario.UTENTE,"U");   
			}
		}
	}
}