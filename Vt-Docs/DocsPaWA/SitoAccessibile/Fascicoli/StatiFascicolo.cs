using System;

namespace DocsPAWA.SitoAccessibile.Fascicoli
{
	/// <summary>
	/// Stati del fascicolo
	/// </summary>
	public class StatiFascicolo
	{
		public const string APERTO="A";
		public const string CHIUSO="C";

		/// <summary>
		/// Reperimento descrizione del fascicolo dallo stato
		/// </summary>
		/// <param name="statoFascicolo"></param>
		/// <returns></returns>
		public static string GetDescrizione(string statoFascicolo)
		{
			string retValue=string.Empty;

			if (statoFascicolo.Equals(APERTO))
				retValue="Aperto";
			else if (statoFascicolo.Equals(CHIUSO))
				retValue="Chiuso";
			else
				retValue=statoFascicolo;

			return retValue;
		}

		private StatiFascicolo()
		{
		}
	}
}
