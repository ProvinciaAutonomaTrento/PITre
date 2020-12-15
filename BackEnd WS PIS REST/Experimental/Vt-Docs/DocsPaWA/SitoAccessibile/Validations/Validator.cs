using System;
using System.Globalization;
using System.Collections;

namespace DocsPAWA.SitoAccessibile.Validations
{
	/// <summary>
	/// Classe per la gestione delle validazioni di dati
	/// </summary>
	public sealed class Validator
	{
		private Validator()
		{
		}

		/// <summary>
		/// Validazione di un dato numerico
		/// </summary>
		/// <param name="numericValue"></param>
		/// <returns></returns>
		public static bool ValidateNumeric(string numericValue)
		{
			bool retValue=true;

			if (numericValue!=null && numericValue!=string.Empty)
			{
				try
				{
					Int32.Parse(numericValue);
				}
				catch
				{ 
					retValue=false;
				}
			}

			return retValue;
		}

		/// <summary>
		/// Validazione di una data
		/// </summary>
		/// <param name="dateValue"></param>
		/// <returns></returns>
		public static bool ValidateDate(string dateValue)
		{
			bool retValue=true;

			if (dateValue!=null && dateValue!=string.Empty)
			{
				try
				{
					ParseDate(dateValue);
				}
				catch
				{
					retValue=false;
				}
			}

			return retValue;
		}

		/// <summary>
		/// Validazione range di date
		/// </summary>
		/// <param name="initValue"></param>
		/// <param name="endValue"></param>
		/// <param name="initValid"></param>
		/// <param name="endValid"></param>
		/// <param name="rangeValid"></param>
		public static void ValidateDateRange(string initValue,string endValue,out bool initValid,out bool endValid,out bool rangeValid)
		{
			initValid=true;
			endValid=true;
			rangeValid=true;

			DateTime dateInitValue=DateTime.MinValue,dateEndValue=DateTime.MaxValue;

			if (initValue!=string.Empty)
			{
				initValid=Validator.ValidateDate(initValue);
				if (initValid)
					dateInitValue=ParseDate(initValue);
			}

			if (endValue!=string.Empty)
			{
				endValid=Validator.ValidateDate(endValue);
				if (endValid)
					dateEndValue=ParseDate(endValue);
			}

			if (initValid && initValue!=string.Empty &&
				endValid && endValue!=string.Empty)
				rangeValid=(dateInitValue <= dateEndValue);
		}

		/// <summary>
		/// Validazione range di dati numerico
		/// </summary>
		/// <param name="initValue"></param>
		/// <param name="endValue"></param>
		/// <returns></returns>
		public static void ValidateNumericRange(string initValue,string endValue,out bool initValid,out bool endValid,out bool rangeValid)
		{
			initValid=true;
			endValid=true;
			rangeValid=true;

			int numInitValue=0,numEndValue=0;

			if (initValue!=string.Empty)
			{
				initValid=Validator.ValidateNumeric(initValue);
				if (initValid)
					numInitValue=Convert.ToInt32(initValue);
			}

			if (endValue!=string.Empty)
			{
				endValid=Validator.ValidateNumeric(endValue);
				if (endValid)
					numEndValue=Convert.ToInt32(endValue);
			}

			if (initValid && initValue!=string.Empty &&
				endValid && endValue!=string.Empty)
				rangeValid=(numInitValue <= numEndValue);
		}


		/// <summary>
		/// Conversione di una data
		/// </summary>
		/// <param name="dateValue"></param>
		/// <returns></returns>
		private static DateTime ParseDate(string dateValue)
		{
			ArrayList formats=new ArrayList();
			formats.Add("dd/MM/yyyy HH.mm.ss");
			formats.Add("dd/M/yyyy HH.mm.ss");
			formats.Add("dd/MM/yyyy H.mm.ss");
			formats.Add("dd/M/yyyy H.mm.ss");
			formats.Add("dd/MM/yyyy");
			formats.Add("dd/M/yyyy");

			return DateTime.ParseExact(dateValue,(string[]) formats.ToArray(typeof(string)),new CultureInfo("it-IT").DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
		}
	}
}