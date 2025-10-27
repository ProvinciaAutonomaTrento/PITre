// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
    [DataContract]
	public class UnitaOrganizzativa : Corrispondente 
	{
        /// <summary>
        /// codice rubrica
        /// </summary>
        [DataMember]
        public string codice { get; set; }
        //public string indirizzo;
        /// <summary>
        /// livello nell'organigramma
        /// </summary>
        /// <remarks>0=radice</remarks>
        [DataMember]
        public string livello { get; set; }
        [DataMember]
        public bool interoperante { get; set; }
        [DataMember]
        public string codiceIstat { get; set; }
        /// <summary>
        /// oggetto UO padre
        /// </summary>
        [DataMember]
        public UnitaOrganizzativa parent { get; set; }

        /// <summary>
        /// arraylist dei Registri (AOO) cui appartiene la UO
        /// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Registro))]
        [DataMember]
        public System.Collections.ArrayList registri { get; set; }

        [DataMember]
        public string classificaUO { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetParentIdPath()
        {
            string path = string.Empty;

            if (parent != null)
            {
                if (parent.parent != null)
                {
                    path = parent.parent.GetParentIdPath();
                }

                path += string.Format("/{0}", this.parent.systemId);
            }

            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        protected string GetPath(UnitaOrganizzativa uo)
        {
            string path = string.Empty;

            if (uo.parent != null)
                path = this.GetPath(uo.parent);

            path += string.Format("/+{0}+{1}+{2}", uo.systemId, (uo.codice ?? uo.codiceRubrica), uo.descrizione);

            return path;
        }

        /// <summary>
        /// Reperimento del path completo dell'unit� organizzativa
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            string path = string.Empty;

            if (parent != null)
                path = this.GetPath(this.parent);

            path += string.Format("/+{0}.eof+{1}.eof+{2}.eof/", this.systemId, (this.codice ?? this.codiceRubrica), this.descrizione);

            return path;
        }
	}
}