using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	[XmlInclude(typeof(DocsPaVO.utente.Utente))]
	public class TrasmissioneUtente 
	{
        public Utente utente ;
        public string dataVista = string.Empty;
        public string dataAccettata = string.Empty;
        public string dataRifiutata = string.Empty;
        public string noteRifiuto = string.Empty;
        public string noteAccettazione = string.Empty;
        public string systemId = string.Empty;
		public TipoRisposta tipoRisposta;
		public static Hashtable tipoRispostaStringa;
        public string dataRisposta = string.Empty;
        public string idTrasmRispSing = string.Empty;
        public string valida = string.Empty;
		public bool daAggiornare = false;
        public string dataRimossaTDL = string.Empty;
        public string idPeopleDelegato = string.Empty;
        public string cha_accettata_delegato = string.Empty;
        public string cha_vista_delegato = string.Empty;
        public string cha_rifiutata_delegato = string.Empty;
        public string cha_rimossa_delegato = string.Empty;

        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool daNotificare = true;

		/// <summary>
		/// </summary>
		public TrasmissioneUtente()
		{
			if(tipoRispostaStringa == null)
			{
				tipoRispostaStringa = new Hashtable();
				tipoRispostaStringa.Add(TipoRisposta.ACCETTAZIONE, "A");
				tipoRispostaStringa.Add(TipoRisposta.RIFIUTO, "R");
			}            
		}
	}
}