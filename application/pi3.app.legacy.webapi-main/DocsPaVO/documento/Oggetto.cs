// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Oggetto 
	{
		public string systemId;
		public string descrizione;
		public bool daAggiornare = false;
        public string idRegistro;
        public string codRegistro;
        public string codOggetto = string.Empty;
        public bool isOggettoModificato = false;
        
		/// <summary>
		/// </summary>
		public Oggetto()
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="descrizione"></param>
        public Oggetto(string systemId, string descrizione) 
		{
			this.systemId=systemId;
			this.descrizione=descrizione;
            
		}

        public Oggetto(string systemId, string descrizione, string codRegistro, string idRegistro)
        {
            this.systemId = systemId;
            this.descrizione = descrizione;
            this.codRegistro = codRegistro;
            this.idRegistro = idRegistro;

        }

        public Oggetto(string systemId, string descrizione, string codRegistro, string idRegistro, string codOggetto)
        {
            this.systemId = systemId;
            this.descrizione = descrizione;
            this.codRegistro = codRegistro;
            this.idRegistro = idRegistro;
            this.codOggetto = codOggetto;

        }
	}
}
