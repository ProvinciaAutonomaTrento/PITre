using System;

namespace DocsPaVO.Smistamento
{
	/// <summary>
	/// Classe che riporta l'esito dello smistamento
	/// di un documento ad un utente
	/// </summary>
	public class EsitoSmistamentoDocumento
	{
		public string IDPeopleDestinatario=string.Empty;

		public string IDCorrGlobaleDestinatario=string.Empty;

		public string DenominazioneDestinatario=string.Empty;

		public int CodiceEsitoSmistamento=0;

		public string DescrizioneEsitoSmistamento=string.Empty;
	}
}
