// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
	public class FileRequest : System.ICloneable
	{
		[DataMember]
        public string dataInserimento { get; set; }
        [DataMember]
        public string descrizione { get; set; }
        [DataMember]
        public string docNumber { get; set; }
        [DataMember]
        public string docServerLoc { get; set; }
        [DataMember]
        public string path { get; set; }
        [DataMember]
        public string fileName { get; set; }
        [DataMember]
        public string idPeople { get; set; }
        [DataMember]
        public string versionId { get; set; }
        [DataMember]
        public string fNversionId { get; set; } //campo per FILENET  = v_e_name+" "+v_name;
        [DataMember]
        public string version { get; set; }
        [DataMember]
        public string subVersion { get; set; }
        [DataMember]
        public string versionLabel { get; set; }
        [DataMember]
        public string fileSize { get; set; }
        [DataMember]
        public string impronta { get; set; }
        [DataMember]
        public string autore { get; set; }
        [DataMember]
        public string autoreFile { get; set; } //utente che ha effettuato l'acquisizione
        [DataMember]
        public string dataAcquisizione { get; set; } //data in cui � stato acquisito il file
        [DataMember]
        public string firmato { get; set; } = string.Empty;
        [DataMember]
        public string idPeopleDelegato { get; set; }
        [DataMember]
        public bool daAggiornareFirmatari { get; set; }
        [DataMember]
        public Applicazione applicazione { get; set; }
        [DataMember]
        public string tipoFirma { get; set; } = DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA.ToString();
        //messaggio di errore in caso di non corretta memorizzazzione del file.
        [DataMember]
        public string msgErr { get; set; } = string.Empty;

        [DataMember]
        public bool inLibroFirma { get; set; }

        [DataMember]
        public bool conSegnaturaPermanente { get; set; } = false;

        /// <summary>
        /// Se true, il documento ha un originale cartaceo
        /// </summary>
        [DataMember]
        public bool cartaceo { get; set; } = false;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.Firmatario))]
        [DataMember]
        public ArrayList firmatari { get; set; }

        /// <summary>
        /// Indica il contesto di esecuzione del repository temporaneo per l'inserimento dei file
        /// riservato al documento finch� i metatati non vengono persistiti
        /// </summary>
        /// <remarks>
        /// L'oggetto � valorizzato solamente in fase di creazione di un nuovo documento
        /// </remarks>
		[DataMember]
        public SessionRepositoryContext repositoryContext { get; set; } = null;

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
