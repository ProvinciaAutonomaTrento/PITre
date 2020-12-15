using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// Summary description for infoSecurity.
	/// </summary>
	public class infoSecurity
	{
		//system_id del documento o fascicolo in security
		public string thing;

		//accessrights
		public string accessRights;

		//tipo diritto dell'oggetto da trasmettere
		public string chaTipoDiritto;

		//system_id del ruolo o della persona
		public string personOrGroup;

		//systemId del gruppo che ha effettuato la trasmissione
		public string idGruppoTrasm;

        /// <summary>
        /// Se true, indica all'utente / ruolo dell'acl non saranno visibili per un documento le versioni precedenti a quella corrente
        /// </summary>
        /// <remarks>
        /// L'acl con "hideDocVersions" a true può essere solamente relativa ad un documento
        /// </remarks>
        public bool hideDocPreviousVersions;

		//indica il tipo di query che viene effettuata (I=INSERT , U=UPDATE)
		public string tipoQuery;
	}
}
