using System;

namespace DocsPAWA.SitoAccessibile
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils
	{
		static string[] DATE_FORMATS = {"dd/MM/yyyy", "dd-MM-yyyy"};
		static string DEFAULT_DATE_FORMAT = "dd/MM/yyyy";

		public Utils()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static DateTime StringToDate(string date, DateTime defVal)
		{
			DateTime dt = defVal;
			try
			{
				dt = DateTime.ParseExact(date,DATE_FORMATS,null,System.Globalization.DateTimeStyles.None);
			}
			catch (Exception) {}
			return dt;
		}

		public static DateTime StringToDate(string date)
		{
			return DateTime.ParseExact(date,DATE_FORMATS,null,System.Globalization.DateTimeStyles.None);
		}

		public static string DateToString(DateTime date, string defVal)
		{
			string s = defVal;
			try
			{
				s = date.ToString(DEFAULT_DATE_FORMAT);
			} 
			catch (Exception) {}
			return s;
		}

		public static string DateToString(DateTime date)
		{
			return DateToString(date,null);
		}

		public static bool CompareDates(string da, string a)
		{
			bool check = true;
			if (da!="" && a!="")
			{
				DateTime d_da = StringToDate(da,DateTime.MinValue);
				DateTime d_a = StringToDate(a, DateTime.MaxValue);
				check = CheckDateDifference(d_da, d_a);
			}

			return check;
		}

		public static string UniformDateFormat(string date)
		{
			string s = null;
			try
			{
				DateTime dt = StringToDate(date);
				s = DateToString(dt);
			}
			catch (Exception) {}
			return s;
		}

		public static string UniformDateFormat(DateTime date)
		{
			string s = null;
			try
			{
				s = DateToString(date);
			}
			catch (Exception) {}
			return s;
		}

		public static bool CheckDateDifference(DateTime da, DateTime a)
		{
			return (da.CompareTo(a)<=0);
		}
	}
}
