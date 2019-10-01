using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace NttDataWA.Utils
{
    public class dateformat
    {
		protected static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static string dateLength(string date)
        {
            date = date.Trim();
            if (date.Length < 10)
            {
                return date;
            }
            else
            {
                return date.Substring(0, 10);
            }
        }

        public static string TimeLength(string time)
        {
            try
            {
                CultureInfo culture = new CultureInfo("it-IT");
                time = time.Replace(".", ":");
                DateTime newTime = Convert.ToDateTime(time, culture);
                string nTime = "";
                if (!(newTime.Hour.ToString().Equals("0") && newTime.Minute.ToString().Equals("0")))
                {
                    nTime = newTime.ToString("HH:mm:ss").Replace(".", ":");
                }
                return nTime;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string formatDataDocsPa(System.DateTime dataP)
        {
            try
            {
                string data = string.Empty;
                string giorni = dataP.Day.ToString();
                string mesi = dataP.Month.ToString();
                if (giorni.Length < 2)
                    giorni = "0" + giorni;
                if (mesi.Length < 2)
                    mesi = "0" + mesi;

                data = giorni + "/" + mesi + "/" + dataP.Year;

                return data;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getDataOdiernaDocspa()
        {
            try
            {
                return formattaDD_MM_in_Docspa(DateTime.Now.Day.ToString()) + "/" + formattaDD_MM_in_Docspa(System.DateTime.Now.Month.ToString()) + "/" + System.DateTime.Now.Year.ToString();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string formattaDD_MM_in_Docspa(string val)
        {
            try
            {
                string rtn = val;
                if (val != null && val.Length > 0)
                {
                    if (val.Length == 1)
                        rtn = "0" + val;
                }
                return rtn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getFirstDayOfWeek()
        {
            try
            {
                return docsPaWS.getFirstDayOfWeek();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getLastDayOfWeek()
        {
            try
            {
                return docsPaWS.getLastDayOfWeek();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getFirstDayOfMonth()
        {
            try
            {
                return docsPaWS.getFirstDayOfMonth();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getLastDayOfMonth()
        {
            try
            {
                return docsPaWS.getLastDayOfMonth();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string toDay()
        {
            try
            {
                return docsPaWS.toDay();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string GetYesterday()
        {
            try
            {
                return docsPaWS.GetYesterday();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string GetLastSevenDay()
        {
            try
            {
                return docsPaWS.GetLastSevenDay();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string GetLastThirtyOneDay()
        {
            try
            {
                return docsPaWS.GetLastThirtyOneDay();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DateTime ConvertToDate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return DateTime.Parse(value.Replace(".", ":"), new System.Globalization.CultureInfo("it-IT", true));
                }
                catch (Exception ex1)
                {
                    try
                    {
                        return DateTime.Parse(value.Replace(".", ":"), new System.Globalization.CultureInfo("en-US", true));
                    }
                    catch (Exception ex2)
                    {
                        try
                        {
                            return DateTime.Parse(value);
                        }
                        catch (Exception ex3)
                        {
                            return new DateTime();
                        }
                    }
                }
            }
            else
            {
                return new DateTime();
            }
        }

    }
}