using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class UnitaOrganizzativa : Corrispondente 
	{
        /// <summary>
        /// codice rubrica
        /// </summary>
		public string codice;
		//public string indirizzo;
        /// <summary>
        /// livello nell'organigramma
        /// </summary>
        /// <remarks>0=radice</remarks>
		public string livello;
		public bool interoperante;
		public string codiceIstat;
        /// <summary>
        /// oggetto UO padre
        /// </summary>
		public UnitaOrganizzativa parent;

        /// <summary>
        /// arraylist dei Registri (AOO) cui appartiene la UO
        /// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Registro))]
		public System.Collections.ArrayList registri;

        public string classificaUO;


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
        /// Reperimento del path completo dell'unità organizzativa
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