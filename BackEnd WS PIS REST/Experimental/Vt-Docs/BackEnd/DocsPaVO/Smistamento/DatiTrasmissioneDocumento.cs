using System;

namespace DocsPaVO.Smistamento
{

	/// <summary>
	/// Oggetto che rappresenta il legame tra la trasmissione ed il documento
	/// </summary>
	public class DatiTrasmissioneDocumento
	{
		public string IDDocumento=string.Empty;

		public bool TrasmissioneConWorkflow=false;

		public string IDTrasmissione=string.Empty;

		public string IDTrasmissioneSingola=string.Empty;

		public string IDTrasmissioneUtente=string.Empty;

		public string NoteGenerali=string.Empty;

        public string NoteIndividualiTrasmSingola = string.Empty;

        public string DescRagioneTrasmissione = string.Empty;

        public string MittenteTrasmissione = string.Empty;
	}
}
