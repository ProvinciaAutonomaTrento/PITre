using System;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Documenti
{
	/// <summary>
	/// Tipologie di documenti presenti in docspa
	/// </summary>
	public sealed class TipiDocumento
	{
		public const string GRIGIO="G";
		public const string INGRESSO="A";
		public const string USCITA="P";
		public const string INTERNO="I";
		public const string STAMPA_REGISTRO="R";

		private const string DATE_FORMAT="dd/MM/yyyy";

		/// <summary>
		/// Reperimento descrizione estesa di un documento
		/// </summary>
		/// <param name="infoDocumento"></param>
		/// <returns></returns>
		public static string ToString(InfoDocumento infoDocumento)
		{
			string documento=infoDocumento.docNumber;

			if (infoDocumento.tipoProto!=TipiDocumento.GRIGIO)
				documento="Prot. " + infoDocumento.numProt;
			else
				documento="Num. " + infoDocumento.idProfile;

			try
			{
				DateTime dataApertura=Convert.ToDateTime(infoDocumento.dataApertura);

				if (dataApertura.Hour==0 && dataApertura.Minute==0 && dataApertura.Second==0)
					documento+=" del " + dataApertura.ToString(DATE_FORMAT);
				else
					documento+=" del " + dataApertura.ToString();
			}
			catch
			{
			}
			

//			if (infoDocumento.dataAnnullamento!=null && infoDocumento.dataAnnullamento!=string.Empty)
//				documento+=Environment.NewLine + "Ann.to il " + Convert.ToDateTime(infoDocumento.dataAnnullamento).ToString(DATE_FORMAT);

			return documento;
		}

		/// <summary>
		/// Reperimento descrizione del documento dal tipo
		/// </summary>
		/// <param name="tipoDocumento"></param>
		/// <returns></returns>
		public static string GetDescrizione(string tipoDocumento)
		{
			string retValue=string.Empty;

			if (tipoDocumento.Equals(GRIGIO))
				retValue="Grigio";
			else if (tipoDocumento.Equals(INGRESSO))
				retValue="Arrivo";
			else if (tipoDocumento.Equals(USCITA))
				retValue="Partenza";
			else if (tipoDocumento.Equals(INTERNO))
				retValue="Interno";
			else if (tipoDocumento.Equals(STAMPA_REGISTRO))
				retValue="Stampa Registro";
			else
				retValue=tipoDocumento;

			return retValue;
		}

		private TipiDocumento()
		{
		}
	}
}
