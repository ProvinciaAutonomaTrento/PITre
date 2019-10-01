using System;

namespace DocsPaVO.documento
{
	/// <summary>
	/// Contiene i dati necessari per effettuare l'export dei risultati della ricerca dei documenti.
	/// gennaio 2007 - gadamo
	/// </summary>
    [Serializable()]
	public class InfoDocumentoExport
	{
		/*		campi richiesti dall'utente per l'export dei documenti:
	
		Numero_protocollo,
		Data_di_protocollo,
		Tipologia_di_documento,
		Data_annullamento,
		Oggetto,
		Mittenti_o_destinatari,
		Codice_del_fascicolo_che_contiene_il_documento,
		Codice_registro (richiesto da Bruno)
        Immagine (richiesta nuova e successiva)
		*/

		public string idOrNumProt = string.Empty;
		public string data = string.Empty;
		public string tipologiaDocumento = string.Empty;
		public string dataAnnullamento = string.Empty;
		public string oggetto = string.Empty;
		public string mittentiDestinatari = string.Empty;
		public string codiceFascicolo = string.Empty;	
		public string codiceRegistro = string.Empty;
        public string acquisitaImmagine = string.Empty;
        public string idTipoAtto = string.Empty;
        public string docNumber = string.Empty;
        public string systemId = string.Empty;
        public string idAmm = string.Empty;
        public string noteCestino = string.Empty;
        public string inConservazione = string.Empty;
        // L'ultima nota inserita con visibilità generale
        public string ultimaNota = string.Empty;
	}
}
