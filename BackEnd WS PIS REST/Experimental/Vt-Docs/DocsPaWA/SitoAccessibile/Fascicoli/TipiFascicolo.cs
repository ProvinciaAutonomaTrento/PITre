using System;

namespace DocsPAWA.SitoAccessibile.Fascicoli
{
	/// <summary>
	/// Tipologie di fascicolo disponibili
	/// </summary>
	public class TipiFascicolo
	{
		public const string GENERALE="G";
		public const string PROCEDIMENTALE="P";

		/// <summary>
		/// Reperimento descrizione del fascicolo dal tipo
		/// </summary>
		/// <param name="tipoFascicolo"></param>
		/// <returns></returns>
		public static string GetDescrizione(string tipoFascicolo)
		{
			string retValue=string.Empty;

			if (tipoFascicolo.Equals(GENERALE))
				retValue="Generale";
			else if (tipoFascicolo.Equals(PROCEDIMENTALE))
				retValue="Procedimentale";
			else
				retValue=tipoFascicolo;

			return retValue;
		}


		private TipiFascicolo()
		{
		}
	}
}
