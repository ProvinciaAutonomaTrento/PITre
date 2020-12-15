using System;
using System.Globalization;
using System.Collections;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Validations;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Classe per la gestione dei filtri per data
	/// </summary>
	public class RangeDateFilter
	{
		private string _fieldName=string.Empty;
		private bool _searchInitDate=false;

		private string _initDate=string.Empty;
		private string _endDate=string.Empty;

		private RangeDateFilter()
		{
		}

		public static RangeDateFilter Create(string fieldName,string initDate,string endDate)
		{
			RangeDateFilter retValue=new RangeDateFilter();
			
			retValue._fieldName=fieldName;
			retValue._initDate=initDate;
			retValue._endDate=endDate;
			
			if (retValue._endDate==null || retValue._endDate==string.Empty)
				retValue._searchInitDate=true;

			return retValue;

//			RangeDateFilter retValue=new RangeDateFilter();
//			
//			retValue._fieldName=fieldName;
//
//			if (initDate!=null && initDate!=string.Empty)
//				retValue._initDate=ParseDate(initDate);
//			
//			if (endDate!=null && endDate!=string.Empty)
//				retValue._endDate=ParseDate(endDate);
//			else
//				retValue._searchInitDate=true;
//
//			return retValue;
		}

		/// <summary>
		/// 
		/// </summary>
		public string FieldName
		{
			get
			{
				return this._fieldName;
			}
		}

		/// <summary>
		/// Flag che indica se la ricerca viene effettuata solo per data iniziale
		/// </summary>
		public bool SearchInitDate
		{
			get
			{
				return this._searchInitDate;
			}
		}

		public DateTime InitDate
		{
			get
			{
				if (Validator.ValidateDate(this.InitDateString))
					return ParseDate(this.InitDateString);
				else
					return DateTime.MinValue;
			}
		}

		public string InitDateString
		{
			get
			{
				//return this.ToDateString(this._initDate);
				return this._initDate;
			}
		}

		public DateTime EndDate
		{
			get
			{
				if (!this.SearchInitDate && Validator.ValidateDate(this.EndDateString))
					return ParseDate(this.EndDateString);
				else
					return DateTime.MaxValue;
			}
		}

		public string EndDateString
		{
			get
			{
				//return this.ToDateString(this._endDate);				
				return this._endDate;
			}
		}

		private string ToDateString(DateTime date)
		{
			return date.ToString("dd/MM/yyyy");
		}

		/// <summary>
		/// Parsing della data
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		private static DateTime ParseDate(string date)
		{
			string[] formats={"dd/MM/yyyy HH.mm.ss","dd/MM/yyyy H.mm.ss","dd/MM/yyyy"};
			return DateTime.ParseExact(date,formats,new CultureInfo("it-IT").DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
		}

//		/// <summary>
//		/// Validazione range di date
//		/// </summary>
//		public bool ValidateRange()
//		{
//			if (this.SearchInitDate)
//				return true;
//			else
//				return (this.EndDate >= this.InitDate);	
//		}
	}
}