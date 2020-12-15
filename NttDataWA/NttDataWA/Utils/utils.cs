using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using log4net;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Utils
{
    public class utils
    {

        private static DocsPaWebService docsPaWS = ProxyManager.GetWS();
        private static ILog logger = LogManager.GetLogger(typeof(Utils.utils));


        /// <summary>
        /// Reperimento url root dell'applicazione
        /// </summary>
        /// <returns></returns>
        public static string GetHttpRootPath()
        {
            try
            {
                string httpRootPath = string.Empty;

                if (System.Configuration.ConfigurationManager.AppSettings["STATIC_ROOT_PATH"] != null && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["STATIC_ROOT_PATH"].ToString()))
                {
                    HttpContext.Current.Session["useStaticRootPath"] = true;
                }

                if (HttpContext.Current.Session["useStaticRootPath"] != null)
                {
                    // Reperimento root path statico impostato nella configurazione del file web.config
                    bool useStaticRoot;

                    if (bool.TryParse(HttpContext.Current.Session["useStaticRootPath"].ToString(), out useStaticRoot))
                    {
                        if (useStaticRoot)
                        {
                            httpRootPath = System.Configuration.ConfigurationManager.AppSettings["STATIC_ROOT_PATH"];

                            HttpRequest request = HttpContext.Current.Request;

                            httpRootPath = httpRootPath + "://" + request.Url.Host;

                            if (!request.Url.Port.Equals(80))
                                httpRootPath += ":" + request.Url.Port;

                            logger.Debug("GetHttpRootPath - useStaticRootPath");
                        }
                    }
                }

                if (string.IsNullOrEmpty(httpRootPath))
                {
                    // Se non è stata impostata alcuna configurazione da web.config relativamente al path statico
                    // o se l'applicazione è stata avviata senza l'utilizzo di quest'ultimo (ossia in modalità standard),
                    // viene effettuato il reperimento del path in maniera dinamica
                    HttpRequest request = HttpContext.Current.Request;

                    httpRootPath = request.Url.Scheme + "://" + request.Url.Host;

                    if (!request.Url.Port.Equals(80))
                        httpRootPath += ":" + request.Url.Port;
                }

                return httpRootPath;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getHttpFullPath()
        {
            try
            {
                string httpRootPath = GetHttpRootPath();

                httpRootPath += HttpContext.Current.Request.ApplicationPath;

                return httpRootPath;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static MittDest[] addToArrayMittDest(MittDest[] array, MittDest nuovoElemento)
        {
            try
            {
                MittDest[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new MittDest[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new MittDest[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static RagioneDest[] addToArrayRagioneDest(RagioneDest[] array, RagioneDest nuovoElemento)
        {
            try
            {
                RagioneDest[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new RagioneDest[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new RagioneDest[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string isEnableConversionePdfLatoServer()
        {
            try
            {
                if (docsPaWS.isEnableConversionePdfLatoServer())
                    return "true";
                else
                    return "false";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

		public static void DefaultButton(System.Web.UI.Page Page, ref System.Web.UI.WebControls.TextBox objTextControl, ref System.Web.UI.WebControls.Button objDefaultButton)
		{
			try
			{
				System.Text.StringBuilder sScript = new System.Text.StringBuilder();

				sScript.Append(Environment.NewLine + "<SCRIPT language=\"javascript\">" + Environment.NewLine);
				sScript.Append("function fnTrapKD(btn) {" + Environment.NewLine);
				sScript.Append(" if (document.all){" + Environment.NewLine);
				sScript.Append("   if (event.keyCode == 13)" + Environment.NewLine);
				sScript.Append("   { " + Environment.NewLine);
				sScript.Append("     event.returnValue=false;" + Environment.NewLine);
				sScript.Append("     event.cancel = true;" + Environment.NewLine);
				sScript.Append("     btn.click();" + Environment.NewLine);
				sScript.Append("   } " + Environment.NewLine);
				sScript.Append(" } " + Environment.NewLine);
				sScript.Append("}" + Environment.NewLine);
				sScript.Append("</SCRIPT>" + Environment.NewLine);

				objTextControl.Attributes.Add("onkeydown", "try { fnTrapKD(document.all." + objDefaultButton.ClientID + "); } catch (e) {}");

				//Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
				Page.ClientScript.RegisterStartupScript(Page.GetType(), "ForceDefaultToScript", sScript.ToString());
			}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
		}

        public static void DefaultButton(System.Web.UI.Page Page, ref System.Web.UI.WebControls.RadioButton radioButton, ref System.Web.UI.HtmlControls.HtmlInputButton inputButton)
        {
            try
            {
                System.Text.StringBuilder sScript = new System.Text.StringBuilder();

                sScript.Append(Environment.NewLine + "<SCRIPT language=\"javascript\">" + Environment.NewLine);
                sScript.Append("function fnTrapKD(btn) {" + Environment.NewLine);
                sScript.Append(" if (document.all){" + Environment.NewLine);
                sScript.Append("   if (event.keyCode == 13)" + Environment.NewLine);
                sScript.Append("   { " + Environment.NewLine);
                sScript.Append("     event.returnValue=false;" + Environment.NewLine);
                sScript.Append("     event.cancel = true;" + Environment.NewLine);
                sScript.Append("     btn.click();" + Environment.NewLine);
                sScript.Append("   } " + Environment.NewLine);
                sScript.Append(" } " + Environment.NewLine);
                sScript.Append("}" + Environment.NewLine);
                sScript.Append("</SCRIPT>" + Environment.NewLine);

                radioButton.Attributes.Add("onkeydown", "try { fnTrapKD(document.all." + inputButton.ClientID + "); } catch (e) {}");

                //Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "ForceDefaultToScript", sScript.ToString());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref System.Web.UI.WebControls.CheckBox checkBox, ref System.Web.UI.HtmlControls.HtmlInputButton inputButton)
        {
            try
            {
                System.Text.StringBuilder sScript = new System.Text.StringBuilder();

                sScript.Append(Environment.NewLine + "<SCRIPT language=\"javascript\">" + Environment.NewLine);
                sScript.Append("function fnTrapKD(btn) {" + Environment.NewLine);
                sScript.Append(" if (document.all){" + Environment.NewLine);
                sScript.Append("   if (event.keyCode == 13)" + Environment.NewLine);
                sScript.Append("   { " + Environment.NewLine);
                sScript.Append("     event.returnValue=false;" + Environment.NewLine);
                sScript.Append("     event.cancel = true;" + Environment.NewLine);
                sScript.Append("     btn.click();" + Environment.NewLine);
                sScript.Append("   } " + Environment.NewLine);
                sScript.Append(" } " + Environment.NewLine);
                sScript.Append("}" + Environment.NewLine);
                sScript.Append("</SCRIPT>" + Environment.NewLine);

                checkBox.Attributes.Add("onkeydown", "fnTrapKD(document.all." + inputButton.ClientID + ")");

                //Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "ForceDefaultToScript", sScript.ToString());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static Object[] addToArray(Object[] array, Object nuovoElemento)
        {
            try
            {
                Object[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new Object[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new Object[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static FiltroRicerca[] addToArrayFiltroRicerca(FiltroRicerca[] array, FiltroRicerca nuovoElemento)
        {
            try
            {
                FiltroRicerca[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new FiltroRicerca[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new FiltroRicerca[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string TruncateString(object t)
        {
            try
            {
                int lung = 0;
                string testo = t.ToString();
                if (System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.TRUNCATESTRING.ToString()] != null)
                {
                    lung = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.TRUNCATESTRING.ToString()]);
                }
                if (lung > 0)
                {
                    if (testo.Length > lung)
                    {
                        testo = testo.Substring(0, lung) + "...";
                    }
                }
                return testo;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string TruncateString_MittDest(object t)
        {
            try
            {
                int lung = 0;
                string testo = t.ToString();
                if (System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.TRUNCATESTRING_MITDEST.ToString()] != null)
                {
                    lung = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.TRUNCATESTRING_MITDEST.ToString()]);
                }
                if (lung > 0)
                {
                    if (testo.Length > lung)
                    {
                        //Interrompo la stringa se supera in lunghezza la variabile lung.
                        //Dopo averla troncata, cerco l'ultimo - che individua il penultimo mittente-dest e elimino tutto ciò che trovo a destra
                        //del -, così facendo vedo la stringa del mittente-destinatario + pulita!
                        testo = testo.Substring(0, lung);
                        int pos = testo.LastIndexOf('-');
                        if (pos >= 1)
                        {
                            testo = testo.Substring(0, pos - 1) + "...";
                        }
                        else
                        {
                            testo = testo + "...";
                        }
                    }
                }
                return testo;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool isDate(string data)
        {
            try
            {
                data = data.Trim();
                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy", "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss" };
                DateTime d_ap = DateTime.ParseExact(data, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return true;
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static string dateLength(string date)
        {
            try
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getTime(string time)
        {
            try
            {
                time = time.Trim();
                int length = time.Length;
                if (length > 11)
                {
                    time = time.Substring(11, length - 11).Replace(":", ".");
                    return time;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool isTime(String time)
        {
            try
            {
                time = time.Trim();

                try
                {
                    DateTime dt1 = DateTime.Parse(time, new CultureInfo("it-IT"));
                }
                catch (Exception)
                {
                    DateTime dt2 = DateTime.Parse(time.Replace(".", ":"), new CultureInfo("it-IT"));
                    dt2.AddDays(1);
                }

                return true;
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static string getMaxDate(string dataA, string dataB)
        {
            try
            {
                //questa funziona ritorna il max tra due date nel formato dd/MM/yyyy			
                dataA = dataA.Trim();
                dataB = dataB.Trim();
                if ((dataA == null || dataA.Equals("")) && (dataB == null || dataB.Equals("")))
                    return null;
                else if ((dataA != null && !dataA.Equals("")) && (dataB == null || dataB.Equals("")))
                    return dataA.Substring(0, 10);
                else if ((dataA == null || dataA.Equals("")) && (dataB != null && !dataB.Equals("")))
                    return dataB.Substring(0, 10);


                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };
                dataA = dataA.Substring(0, 10);
                dataB = dataB.Substring(0, 10);

                DateTime d_a = DateTime.ParseExact(dataA, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                DateTime d_b = DateTime.ParseExact(dataB, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                if (d_a.CompareTo(d_b) > 0)
                    return dataA;
                else
                    return dataB;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool verificaIntervalloDateSenzaOra(string dataUno, string dataDue)
        {
            if (!string.IsNullOrEmpty(dataUno) && !string.IsNullOrEmpty(dataDue))
            {
                try
                {
                    dataUno = dataUno.Trim();
                    dataDue = dataDue.Trim();
                    CultureInfo ci = new CultureInfo("it-IT");
                    string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };
                    DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    DateTime d_Due = DateTime.ParseExact(dataDue, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                    if (d_Uno >= d_Due)
                        return true;
                    else
                        return false;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static int GetYearsFromInterval(string dataUno, string dataDue)
        {
            if (dataUno != "" && dataDue != "")
            {
                try
                {
                    dataUno = dataUno.Trim();
                    dataDue = dataDue.Trim();
                    CultureInfo ci = new CultureInfo("it-IT");
                    string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                    DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    DateTime d_Due = DateTime.ParseExact(dataDue, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    TimeSpan diff = d_Due.Subtract(d_Uno);
                    DateTime date0 = new DateTime().Add(diff);
                    int years = date0.Year - 1;
                    return years;
                }
                catch (Exception e)
                {
                    logger.Info("Errore in verificaIntervalloDate");
                    logger.Debug(e.Message);
                    throw new Exception(e.Message);
                }
            }
            else
            {
                return 0;
            }
        }


        public static bool verificaIntervalloDate(string dataUno, string dataDue)
        {
            if (dataUno != "" && dataDue != "")
            {
                try
                {
                    dataUno = dataUno.Trim();
                    dataDue = dataDue.Trim();
                    CultureInfo ci = new CultureInfo("it-IT");
                    string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                    DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    DateTime d_Due = DateTime.ParseExact(dataDue, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                    if (d_Uno >= d_Due)
                        return true;
                    else
                        return false;
                }
                catch (Exception e)
                {
                    logger.Info("Errore in verificaIntervalloDate");
                    logger.Debug(e.Message);
                    throw new Exception(e.Message);
                }
            }
            else
            {
                return false;
            }
        }

        public static bool isNumeric(string val)
        {
            try
            {
                string appo = val;
                Regex regExp = new Regex("\\D");

                return !regExp.IsMatch(appo);
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool isCorrectProv(string val)
        {
            try
            {
                string appo = val.ToUpper();
                Regex regExp = new Regex("([A-Z]{2})");

                return regExp.IsMatch(appo);
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static int CheckVatNumber(string vatNum)
        {
            try
            {
                bool result = false;
                const int character = 11;
                string vatNumber = vatNum;
                Regex pregex = new Regex("^\\d{" + character.ToString() + "}$");

                if (string.IsNullOrEmpty(vatNumber) || vatNum.Length != character)
                    return -1;
                Match m = pregex.Match(vatNumber);
                result = m.Success;
                if (!result)
                    return -2;
                result = (int.Parse(vatNumber.Substring(0, 7)) != 0);
                if (!result)
                    return -3;
                result = ((int.Parse(vatNumber.Substring(7, 3)) >= 0) && (int.Parse(vatNumber.Substring(7, 3)) < 201));
                if (!result)
                    return -4;

                /*Algoritmo di verifica della correttezza formale del numero di partita IVA 
                ---------------------------------------------------------------------------------------------
                    1. si sommano tra loro le cifre di posto dispari
                    2. le cifre di posto pari si moltiplicano per 2
                    3. se il risultato del punto precedente è maggiore di 9 si sottrae 9 al risultato
                    4. si sommano tra loro i risultati dei 2 punti precedenti
                    5. si sommano tra loro le due somme ottenute
                ---------------------------------------------------------------------------------------------
                 */
                int sum = 0;
                for (int i = 0; i < character - 1; i++)
                {
                    int j = int.Parse(vatNumber.Substring(i, 1));
                    if ((i + 1) % 2 == 0)
                    {
                        j *= 2;
                        char[] c = j.ToString("00").ToCharArray();
                        sum += int.Parse(c[0].ToString());
                        sum += int.Parse(c[1].ToString());
                    }
                    else
                        sum += j;
                }
                if ((sum.ToString("00").Substring(1, 1).Equals("0")) && (!vatNumber.Substring(10, 1).Equals("0")))
                    return -5;
                sum = int.Parse(vatNumber.Substring(10, 1)) + int.Parse(sum.ToString("00").Substring(1, 1));
                if (!sum.ToString("00").Substring(1, 1).Equals("0"))
                    return -5;
                return 0;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }

        /// <summary>
        /// Controllo formale della correttezza del Codice Fiscale
        /// </summary>
        /// <param name="taxCode"></param>
        /// <returns>
        /// -1 : Lunghezza Codice Fiscale errata.
        /// -2 : Il formato del Codice Fiscale non è corretto.
        /// -3 : Verifica della correttezza formale del Codice Fiscale non superata 
        /// 0 :  Codice Fiscale corretto
        /// </returns>
        public static int CheckTaxCode(string taxCode)
        {
            try
            {
                taxCode = taxCode.Replace(" ", "");
                bool result = false;
                const int character = 16;
                // stringa per controllo e calcolo omocodia 
                const string omocode = "LMNPQRSTUV";
                // per il calcolo del check digit e la conversione in numero
                const string listControl = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                int[] listEquivalent = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
                int[] listaUnequal = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };

                result = (string.IsNullOrEmpty(taxCode) || taxCode.Length != character);
                if (result)
                    return -1;
                taxCode = taxCode.ToUpper();
                char[] arrTaxCode = taxCode.ToCharArray();

                // check della correttezza formale del codice fiscale
                // elimino dalla stringa gli eventuali caratteri utilizzati negli 
                // spazi riservati ai 7 che sono diventati carattere in caso di omocodia
                for (int k = 6; k < 15; k++)
                {
                    if ((k == 8) || (k == 11))
                        continue;
                    int x = (omocode.IndexOf(arrTaxCode[k]));
                    if (x != -1)
                        arrTaxCode[k] = x.ToString().ToCharArray()[0];
                }

                Regex rgx = new Regex(@"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$");
                Match m = rgx.Match(new string(arrTaxCode));
                result = m.Success;
                // normalizzato il codice fiscale se la regular non ha buon
                // fine è inutile continuare
                if (!result)
                    return -2;
                int somma = 0;
                // ripristino il codice fiscale originario 
                arrTaxCode = taxCode.ToCharArray();
                for (int i = 0; i < 15; i++)
                {
                    char c = arrTaxCode[i];
                    int x = "0123456789".IndexOf(c);
                    if (x != -1)
                        c = listControl.Substring(x, 1).ToCharArray()[0];
                    x = listControl.IndexOf(c);
                    // i modulo 2 = 0 è dispari perchè iniziamo da 0
                    if ((i % 2) == 0)
                        x = listaUnequal[x];
                    else
                        x = listEquivalent[x];
                    somma += x;
                }
                result = (listControl.Substring(somma % 26, 1) == taxCode.Substring(15, 1));
                if (!result)
                    return -3;
                return 0;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }
        }

        public static string FormatUrl(string text)
        {
            text = text.Replace("\n", "<br />");
            text = HttpUtility.UrlEncode(text);
            return text;
        }

        public static string FormatHtml(string text)
        {
            //text = text.Replace("\n", "<br />");
            text = HttpUtility.HtmlEncode(text);
            return text;
        }

        public static string FormatJs(string text)
        {
            try
            {
                string result = "";
                if (!string.IsNullOrEmpty(text)) result = text.Replace("\"", "\\\"").Replace("'", "\\'").Replace("\r\n", "\\n").Replace("\n", "\\n");
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocumentoParolaChiave[] addToArrayParoleChiave(DocumentoParolaChiave[] array, DocumentoParolaChiave nuovoElemento)
        {
            try
            {
                DocumentoParolaChiave[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new DocumentoParolaChiave[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new DocumentoParolaChiave[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
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

        public static bool IsValidEmail(string strToCheck)
        {
            try
            {
                return Regex.IsMatch(strToCheck, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static string DO_AdattaString(string valore)
        {
            valore = valore.Trim();
            valore = valore.Replace("\r", "");
            valore = valore.Replace("\n", "");
            return valore;
        }

        public static string[] splittaStringaRicercaNote(string valore)
        {
            // La stringa da ricercare nelle note, oltre al testo da
            // ricercare contiene anche la tipologia di ricerca 
            // richiesta
            string[] separatore = new string[] { "@-@" };
            string[] ricNote = valore.Split(separatore, StringSplitOptions.None);
            return ricNote;
        }

        public static bool GetAbilitazioneAtipicita()
        {
            bool result = false;
            InfoUtente infoUtente;

            try
            {
                infoUtente = UserManager.GetInfoUser();
            }
            catch (Exception e)
            {
                SessionManager sm = new SessionManager();
                infoUtente = sm.getUserAmmSession();
                string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                infoUtente.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);

            }

            string valoreChiaveAtipicita = Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.ATIPICITA_DOC_FASC.ToString());
            if (string.IsNullOrEmpty(valoreChiaveAtipicita))
                valoreChiaveAtipicita = Utils.InitConfigurationKeys.GetValue("0", DBKeys.ATIPICITA_DOC_FASC.ToString());

            if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
                result = true;

            return result;
        }

        public static string ReplaceApexes(string sourceString)
        {
            string result = null;

            if (sourceString != null)
            {
                result = sourceString.Replace("'", "''");
            }

            return result;
        }

        public static string FWithoutHtml(string text)
        {
            return Regex.Replace(text, @"<[^>]*>", "");
        }

        public static DateTime formatStringToDate(string data)
        {
            data = data.Trim();
            DateTime retValue;
            if (data == null || (data != null && data.Equals("")))
            {
                retValue = DateTime.Parse(utils.getNullDate());
            }
            else
            {
                retValue = DateTime.Parse(data);
            }
            return retValue;
        }

        public static string getNullDate()
        {
            return "01/01/0001";
        }

        /// <summary>
        /// Formatta i dati relativi all'ente certificatore
        /// </summary>
        /// <param name="issuesName">IssuesName con formato generico CN={0},CN={1}, OU={2}, O={3}, C={4}</param>
        /// <returns>Ente certificatore contenete le info di CN, O, C  </returns>
        public static string getEnteCertificatore(string issuerName)
        {
            string enteCertCN = string.Empty;
            string enteCertO = string.Empty;
            string enteCertC = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(issuerName)) return string.Empty; //no issuesName
                string[] issuerNamePars = issuerName.Split(',');
                // recupera CN
                if (issuerName.Contains("CN="))
                    enteCertCN = issuerNamePars.Where(a => a.Contains("CN=")).SingleOrDefault().Split('=')[1];
                // recupera O
                if (issuerName.Contains("O="))
                    enteCertO = string.Format("{0}{1}", (string.IsNullOrEmpty(enteCertCN) ? string.Empty : ", "), issuerNamePars.Where(a => a.Contains("O=")).SingleOrDefault().Split('=')[1]);
                if (issuerName.Contains("C="))
                    enteCertC = string.Format(", {0}", issuerNamePars.Where(a => a.Contains("C=")).SingleOrDefault().Split('=')[1]);
            }
            catch (Exception e)
            {

            }
            return string.Format("{0}{1}{2}", enteCertCN, enteCertO, enteCertC);
        }

        /// <summary>
        /// controlla se è attivo oracle / swl effettuando una semplice query sulla DPA_AMMINISTRA e poi
        /// se presnete il documentale DTCM/WSPIA effettua una chiamata ad un WS DTCM.
        /// </summary>
        /// <returns></returns>
        public static bool checkSystem(out string msg)
        {
            msg = "";
            return docsPaWS.CheckConnection(out msg);

        }

    }
}
