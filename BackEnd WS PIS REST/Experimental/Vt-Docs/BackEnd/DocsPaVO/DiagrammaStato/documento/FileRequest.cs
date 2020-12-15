using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class FileRequest : System.ICloneable
	{
		public string dataInserimento;
		public string descrizione;
		public string docNumber;
		public string docServerLoc;
		public string path;
		public string fileName;
		public string idPeople;
		public string versionId;
		public string fNversionId; //campo per FILENET  = v_e_name+" "+v_name;
		public string version;
		public string subVersion;
		public string versionLabel;
		public string fileSize;
		public string autore;
        public string firmato=string.Empty;
        public string idPeopleDelegato;
		public bool daAggiornareFirmatari;
		public Applicazione applicazione;
        //messaggio di errore in caso di non corretta memorizzazzione del file.
        public string msgErr = string.Empty;

        /// <summary>
        /// Se true, il documento ha un originale cartaceo
        /// </summary>
        public bool cartaceo = false;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.Firmatario))]
		public ArrayList firmatari;

        /// <summary>
        /// Indica il contesto di esecuzione del repository temporaneo per l'inserimento dei file
        /// riservato al documento finché i metatati non vengono persistiti
        /// </summary>
        /// <remarks>
        /// L'oggetto è valorizzato solamente in fase di creazione di un nuovo documento
        /// </remarks>
        public SessionRepositoryContext repositoryContext = null;

		#region ICloneable Members

		public object Clone()
		{
			FileRequest fr;
			if(this.GetType().Equals(typeof(Documento)))
				fr = new Documento();
			else if(this.GetType().Equals(typeof(Allegato)))
				fr = new Allegato();
			else
				fr = new FileRequest();
			fr.dataInserimento = dataInserimento;
			fr.descrizione = descrizione;
			fr.docNumber = docNumber;
			fr.docServerLoc = docServerLoc;
			fr.path = path;
			fr.fileName = fileName;
			fr.idPeople = idPeople;
			fr.versionId = versionId;
			fr.version = version;
			fr.subVersion = subVersion;
			fr.versionLabel = versionLabel;
			fr.fileSize = fileSize;
			fr.applicazione = applicazione;
			fr.firmatari = firmatari;
			return fr;
		}

		#endregion
	}
}
