using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{	
	/// <summary>
	/// </summary>
	[XmlType("FascicoloDiritto")]
    [Serializable()]
	public class DirittoOggetto
	{
		public string idObj;
		public TipoDiritto tipoDiritto;
		public DocsPaVO.utente.Corrispondente soggetto;
        public int accessRights;
        public bool deleted;
        public string note;
        public string personorgroup;
        public string rootFolder;
        public string dtaInsSecurity;
        public string noteSecurity;
        //
        // Mev Editing ACL Massivo
        public bool Checked = false;
        public string removed;
        // End Mev
        //

        //copia_visibilita: identifica se i diritti sono acquisiti per copia visibilita
        public string CopiaVisibilita;
	}
}
