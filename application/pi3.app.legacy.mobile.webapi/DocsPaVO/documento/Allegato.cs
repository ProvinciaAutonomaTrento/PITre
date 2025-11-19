// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
    [DataContract]
	public class Allegato : FileRequest 
	{
        [DataMember]
		public int numeroPagine { get; set; }

        /// <summary>
        /// Posizione di inserimento dell'allegato nell'ambito del suo documento principale
        /// </summary>
        [DataMember]
        public int position { get; set; }

        /// <summary>
        /// Identificativo del documento da cui � stato ottenuto questo documento tramite la funzionalit� di Inoltro
        /// </summary>
        [DataMember]
        public String ForwardingSource { get; set; }

        /// <summary>
        /// private parameter for the type attachment
        /// </summary>
        private int _typeAttchament;


        /// <summary>
        /// Public Properties associated with the type attachment
        /// </summary>
        [DataMember]
        public int TypeAttachment
        {
            get 
            {
                if (_typeAttchament == null)
                    return -1;
                else
                    return _typeAttchament;
            }
            set
            {
                _typeAttchament = value;
            }
        }
	}
}
