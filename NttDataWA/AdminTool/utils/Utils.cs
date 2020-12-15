using System;
using SAAdminTool.DocsPaWR;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using System.Web;
using log4net;
using System.Drawing;
using  SAAdminTool.AdminTool.Manager;
using System.Linq;

namespace SAAdminTool
{
    /// <summary>
    /// Summary description for Utils.
    /// </summary>
    public class Utils
    {
        private static DocsPaWebService docsPaWS = ProxyManager.getWS();
        private static ILog logger = LogManager.GetLogger(typeof(Utils));

        public static string formatDataDocsPa(System.DateTime dataP)
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

        /// <summary>
        /// prende il testo della label della data locazione fisica dal web.config.
        /// ritorna o il valore o string.empty
        /// </summary>
        public static string label_data_Loc_fisica
        {
            get
            {
                string eme = System.Configuration.ConfigurationManager.AppSettings["LABEL_DTA_LOC_FISICA"];
                return (eme != null && eme != "") ? eme : string.Empty;
            }
        }

        public static string getRequestStringValue(System.Web.HttpRequest request, string parameter)
        {
            string retValue = "";
            if (request[parameter] != null)
            {
                retValue = request[parameter].ToString();
            }
            return retValue;
        }

        public static string getAbilitazioneSmistamento()
        {
            string smistamentoAbilitato = "0";
            if (System.Configuration.ConfigurationManager.AppSettings["RUBRICA_PROTO_USA_SMISTAMENTO"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["RUBRICA_PROTO_USA_SMISTAMENTO"] == "1")
            {
                smistamentoAbilitato = "1";
            }
            return smistamentoAbilitato;
        }

        public static string getMaxDate(string dataA, string dataB)
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

        public static bool isNumeric(string val)
        {
            string appo = val;
            Regex regExp = new Regex("\\D");

            return !regExp.IsMatch(appo);
        }

        public static bool isCorrectProv(string val)
        {
            string appo = val.ToUpper();
            Regex regExp = new Regex("([A-Z]{2})");

            return regExp.IsMatch(appo);
        }

        public static void startUp(Page page)
        {

            ErrorManager.checkError(page);

            //Logger.log(page.GetType().ToString());
            if (page.Session == null)
                ErrorManager.redirectToLoginPage(page);
            //no cache:
            page.Response.CacheControl = "no-cache";
            page.Response.AddHeader("Pragma", "no-cache");
            page.Response.Expires = -1;

        }

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

        public static string dateTimeLength(string date)
        {
            CultureInfo culture = new CultureInfo("it-IT");
            DateTime newDateTime;
            ////*** Correzione del bug sul formato dell'ora su alcuni SO ---
            date = date.Replace(":", ".");
            DateTime dateOut = new DateTime();
            string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
            if (DateTime.TryParseExact(date, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out dateOut))
            {
                newDateTime = DateTime.ParseExact(date, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                ////*** Fine correzione ---
            }
            else
            {
                date = date.Replace(".", ":");
                newDateTime = Convert.ToDateTime(date, culture);
            }
            date = date.Trim();
            if (date.Length <= 10)
            {
                return newDateTime.ToShortDateString();
            }
            else
            {
                string data = newDateTime.ToShortDateString();
                string ora = newDateTime.ToString("HH:mm").Replace(".", ":");
                return data + " " + ora;

            }
        }

        public static string getTime(string time)
        {
            time = time.Trim();
            int length = time.Length;
            if (length > 11)
            {
                time = time.Substring(11,length-11);
                return time;
            }
            else
            {
                return null;
            }
        }

        public static string timeLength(string time)
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

        public static bool isNumericOld(string val)
        {
            bool trovato = false;
            string numeri = "0123456789";
            for (int i = 0; (i < val.Length && !trovato); i++)
            {
                if (numeri.IndexOf(val[i], 0, numeri.Length - 1) < 0)
                    trovato = true;
            }
            if (trovato)
                return false;

            return true;
        }

        public static bool isTime(String time)
        {
            try
            {
                time = time.Trim();
                CultureInfo ci = new CultureInfo("it-IT");
                //Commentato per problema su formato ora ammesso - hh:mm:ss con hh.mm.ss
                //string[] formati = { "HH:mm:ss", "H:mm:ss", "HH.mm.ss", "H.mm.ss" };
                //DateTime t_ap = DateTime.ParseExact(time, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                DateTime t_ap = DateTime.Parse(time, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return true;
            }
            catch (Exception)
            {
                return false;
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
            catch (Exception)
            {
                return false;
            }
        }

        public static string getNullDate()
        {
            return "01/01/0001";
        }

        public static DateTime formatStringToDate(string data)
        {
            data = data.Trim();
            DateTime retValue;
            if (data == null || (data != null && data.Equals("")))
            {
                retValue = DateTime.Parse(Utils.getNullDate());
            }
            else
            {
                retValue = DateTime.Parse(data);
            }
            return retValue;
        }

        public static void checkForDateNullOnItem(DataGridItem item)
        {
            Label objRefer = new Label();
            ListItemType typeItem = item.ItemType;
            if (typeItem == ListItemType.Item || typeItem == ListItemType.AlternatingItem)
            {
                for (int i = 0; i < item.Cells.Count; i++)
                {
                    if (item.Cells[i].Controls[1].GetType() == objRefer.GetType())
                    {
                        Label labelItem = (Label)item.Cells[i].Controls[1];
                        if (Utils.isDate(labelItem.Text))
                        {
                            if (Utils.isNullDate(labelItem.Text))
                            {
                                labelItem.Text = "";
                            }
                            else
                            {
                                labelItem.Text = DateTime.Parse(labelItem.Text).ToShortDateString();
                            }
                        }
                    }
                }
            }
        }

        public static bool isNullDate(string dateToTest)
        {
            bool retValue = false;

            if (isDate(dateToTest))
            {
                if (DateTime.Parse(dateToTest).ToShortDateString() == getNullDate())
                {
                    retValue = true;
                }
            }
            else
            {
                retValue = true;
            }

            return retValue;
        }

        public static string formatFascSetteCifre(string numero)
        {
            string fasc;
            int MAX_LENGTH = 7;
            string zeroes = "";
            string result = "";

            fasc = numero.Trim();
            for (int ind = 1; ind <= MAX_LENGTH - fasc.Length; ind++)
            {
                zeroes = zeroes + "0";
            }

            result = fasc.Insert(0, zeroes);
            return result;
        }

        //Celeste
        public static string formatProtocollo(string numero)
        {
            string protocollo;
            int MAX_LENGTH = 7;
            string zeroes = "";
            string result = "";

            protocollo = numero.Trim();
            for (int ind = 1; ind <= MAX_LENGTH - protocollo.Length; ind++)
            {
                zeroes = zeroes + "0";
            }

            result = protocollo.Insert(0, zeroes);
            return result;
        }

        //Fine Celeste

        public static Object[] addToArray(Object[] array, Object nuovoElemento)
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

        public static String[] addToArrayString(String[] array, String nuovoElemento)
        {
            String[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new String[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new String[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        public static MittDest[] addToArrayMittDest(MittDest[] array, MittDest nuovoElemento)
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

        public static RagioneDest[] addToArrayRagioneDest(RagioneDest[] array, RagioneDest nuovoElemento)
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

        public static InfoDocumento[] addToArrayInfoDoc(InfoDocumento[] array, InfoDocumento nuovoElemento)
        {
            InfoDocumento[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new InfoDocumento[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new InfoDocumento[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        public static DocumentoParolaChiave[] addToArrayParoleChiave(DocumentoParolaChiave[] array, DocumentoParolaChiave nuovoElemento)
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

        public static FiltroRicerca[] addToArrayFiltroRicerca(FiltroRicerca[] array, FiltroRicerca nuovoElemento)
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

        public static TrasmissioneSingola[] addToArrayTrasmissioneSingola(TrasmissioneSingola[] array, TrasmissioneSingola nuovoElemento)
        {
            TrasmissioneSingola[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new TrasmissioneSingola[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new TrasmissioneSingola[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        public static ElementoRubrica[] addToArrayElementoRubrica(ElementoRubrica[] array, ElementoRubrica nuovoElemento)
        {
            ElementoRubrica[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new ElementoRubrica[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new ElementoRubrica[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        public static System.Web.HttpContext getPageContext(System.Web.UI.Page page)
        {
            return new System.Web.HttpContext(page.Request, page.Response);
        }

        /// <summary>
        /// Reperimento url root dell'applicazione
        /// </summary>
        /// <returns></returns>
        private static string GetHttpRootPath()
        {
            string httpRootPath = string.Empty;

            if (HttpContext.Current.Session["useStaticRootPath"] != null)
            {
                // Reperimento root path statico impostato nella configurazione del file web.config
                bool useStaticRoot;

                if (bool.TryParse(HttpContext.Current.Session["useStaticRootPath"].ToString(), out useStaticRoot))
                {
                    if (useStaticRoot)
                    {
                        httpRootPath = System.Configuration.ConfigurationManager.AppSettings["STATIC_ROOT_PATH"];

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

        public static string getHttpFullPath()
        {
            string httpRootPath = GetHttpRootPath();

            httpRootPath += HttpContext.Current.Request.ApplicationPath;

            return httpRootPath;
        }

        public static string getHttpFullPath(System.Web.UI.Page page)
        {
            return getHttpFullPath();
        }

        private static string getProcessedFilterString(string filterToProcess)
        {
            string retValue;

            if (filterToProcess != null)
            {
                if (filterToProcess.ToLower().Equals("stato".ToLower()))
                {
                    retValue = "stato (a-c)".ToUpper();
                }
                else
                {
                    retValue = filterToProcess;
                }
            }
            else
            {
                retValue = null;
            }

            return retValue;
        }

        public static void populateDdlWithEnumValuesANdKeys(System.Web.UI.WebControls.DropDownList ddlControl, string[] arrayFiltri)
        {
            System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
            for (int index = 0; index < arrayFiltri.Length; index++)
            {
                arrayList.Add(arrayFiltri[index]);
            }
            arrayList.Sort();

            for (int i = 0; i < arrayList.Count; i++)
            {
                string valore = (string)arrayList[i];
                string testo = getProcessedFilterString(valore);
                System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem(testo, valore);
                ddlControl.Items.Add(item);
            }
        }

        public static System.Boolean getSpedizioneFax()
        {
            string l_spedisciViaFax = ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_SPEDISCI_VIA_FAX);
            System.Boolean retValue = true;
            if (l_spedisciViaFax != null)
            {
                if (l_spedisciViaFax.Equals("0"))
                { retValue = false; }
                else if (l_spedisciViaFax.Equals("1"))
                { retValue = true; }
            }
            return retValue;
        }
        //		Elisa Mosca 01/12/2004
        /// <summary>
        /// Funzione per settare il focus ad un controllo
        /// </summary>
        /// <param name="page">pagina contenente il controllo</param>
        /// <param name="control">controllo sul quale impostare il focus</param>
        /// <param name="sel">se true seleziona il testo per i textbox</param>
        public static void SetFocus(Page page, object control, bool sel)
        {
            // verifica che il controllo a cui si vuole dare il focus non sia nullo
            if (control == null)
                return;

            // rileva l'ID univoco del controllo
            string ctrlID = ((WebControl)control).UniqueID;

            // implementa scripting client-side per focus
            string script;
            script = "<script language=\"javascript\">";
            script += "var control = document.getElementById(\"" + ctrlID + "\");";
            script += "if(control!=null){control.focus();";
            // se è un TextBox, oltre a dare il focus, seleziona il testo
            if (sel == true)
            {
                if (control.GetType().ToString() == "System.Web.UI.WebControls.TextBox")
                {
                    script += "control.select();";
                }
            }
            script += "}<";
            script += "/script>";

            page.ClientScript.RegisterStartupScript(page.GetType(), "Focus", script);
            //page.RegisterStartupScript("Focus", script);
        }

        //imposta il focus su un controllo
        public static void SetFocus(string controlID, Page page)
        {
            string s = "<SCRIPT language='javascript'>try { document.getElementById('" + controlID + "').focus() } catch (ex) {}</SCRIPT>";
            page.ClientScript.RegisterStartupScript(page.GetType(), "focus", s);
            //page.RegisterStartupScript("focus", s);
        }

        public static void RemoveDataSession(Page page)
        {
            try
            {
                for (int i = 0; i <= page.Session.Keys.Count - 1; i++)
                {
                    switch (page.Session.Keys[i])
                    {
                        // variabili di sessione che non devono essere rimosse
                        case "ESERCITADELEGA":
                        case "APRIDELEGHE":
                        case "userData":
                        case "userRuolo":
                        case "useStaticRootPath":
                        case "AppWA":
                        case "CallContextStack.contextStack":
                        case "CallContextStack.CurrentContext":
                        case "reloadScelta":
                        case "TrasmNonViste":
                        case "TrasmNonAccettate":
                        case "TrasmDocPredisposti":
                        case "newRuolo":
                        case "userDelegato":
                        case "userDataDelegato":
                        case "userRuoloDelegato":
                        case "PredispostiInToDoList":
                        case SAAdminTool.ricercaTrasm.DialogFiltriRicercaTrasmissioni.CURRENT_FILTERS_SESSION_KEY:
                        case SAAdminTool.ricercaTrasm.FiltriRicercaTrasmissioni.CURRENT_UI_FILTERS_SESSION_KEY:
                            // Filtri ricerca trasmissioni in todolist in sessione
                            break;
                        default:
                            page.Session.RemoveAt(i);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Metodo RemoveDataSession - Errore: " + ex.Message);
            }
        }

        public static string DO_AdattaString(string valore)
        {
            valore = valore.Trim();
            valore = valore.Replace("\r", "");
            valore = valore.Replace("\n", "");
            return valore;
        }

        public static string getDataOdiernaDocspa()
        {
            return formattaDD_MM_in_Docspa(DateTime.Now.Day.ToString()) + "/" + formattaDD_MM_in_Docspa(System.DateTime.Now.Month.ToString()) + "/" + System.DateTime.Now.Year.ToString();
        }

        public static string formattaDD_MM_in_Docspa(string val)
        {
            string rtn = val;
            if (val != null && val.Length > 0)
            {
                if (val.Length == 1)
                    rtn = "0" + val;
            }
            return rtn;
        }


        //formatta la data con l'ora in solo formato data
        public static string formattaDataTimeInData(string dataTime)
        {   
            
            if (dataTime != null && dataTime.Contains(" "))
            {
                dataTime=dataTime.Remove(dataTime.IndexOf(" "));
            }

            return dataTime;
        }

        public static bool verificaIntervalloDateSenzaOra(string dataUno, string dataDue)
        {
            if (dataUno != "" && dataDue != "")
            {
                try
                {
                    dataUno = dataUno.Trim();
                    dataDue = dataDue.Trim();
                    CultureInfo ci = new CultureInfo("it-IT");
                    string[] formati = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy", "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss" };
                    DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    DateTime d_Due = DateTime.ParseExact(dataDue, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                    if (d_Uno >= d_Due)
                        return true;
                    else
                        return false;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else
            {
                return false;
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
                    string[] formati = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy", "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss" };
                    DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    DateTime d_Due = DateTime.ParseExact(dataDue, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                    if (d_Uno >= d_Due)
                        return true;
                    else
                        return false;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else
            {
                return false;
            }
        }

        public static bool verificaRangeValori(string init, string end)
        {
            try
            {
                if (init != "" && end != "")
                {
                    init = init.Trim();//Da
                    end = end.Trim();//A

                    int int_init = Convert.ToInt32(init);
                    int int_end = Convert.ToInt32(end);

                    if (int_init >= int_end)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #region METODI PER LA GESTIONE DELLA FASCICOLAZIONE RAPIDA

        /// <summary>
        ///Carica il menù con l'array di registri passati in input
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="userReg"></param>
        public static void CaricaComboRegistri(DropDownList ddl, SAAdminTool.DocsPaWR.Registro[] userReg)
        {
            if (userReg != null)
            {
                for (int i = 0; i < userReg.Length; i++)
                {
                    ddl.Items.Add(userReg[i].codRegistro);
                    ddl.Items[i].Value = userReg[i].systemId;
                }
            }
        }



        #endregion

        #region Metodi per settare le operazioni di default sul tasto invio
        public static void DefaultButton(System.Web.UI.Page Page, ref TextBox objTextControl, ref ImageButton objDefaultButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref System.Web.UI.WebControls.TextBox objTextControl, ref DocsPaWebCtrlLibrary.ImageButton objDefaultButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref TextBox objTextControl, ref System.Web.UI.WebControls.Button objDefaultButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref DropDownList objTextControl, ref ImageButton objDefaultButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref DropDownList objTextControl, ref DocsPaWebCtrlLibrary.ImageButton objDefaultButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref DropDownList objTextControl, ref System.Web.UI.WebControls.Button objDefaultButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref DocsPaWebCtrlLibrary.DateMask objTextControl, ref ImageButton objDefaultButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref DocsPaWebCtrlLibrary.DateMask objTextControl, ref Button objDefaultButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref DocsPaWebCtrlLibrary.DateMask objTextControl, ref DocsPaWebCtrlLibrary.ImageButton objDefaultButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref RadioButton radioButton, ref System.Web.UI.WebControls.ImageButton objDefaultButton)
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

                radioButton.Attributes.Add("onkeydown", "try { fnTrapKD(document.all." + objDefaultButton.ClientID + "); } catch (e) {}");

                //Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "ForceDefaultToScript", sScript.ToString());
            }
            catch
            {
            }
        }



        public static void DefaultButton(System.Web.UI.Page Page, ref TextBox objTextControl, ref System.Web.UI.HtmlControls.HtmlInputButton inputButton)
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

                objTextControl.Attributes.Add("onkeydown", "try { fnTrapKD(document.all." + objTextControl.ClientID + "); } catch (e) {}");

                //Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "ForceDefaultToScript", sScript.ToString());
            }
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref DropDownList objTextControl, ref System.Web.UI.HtmlControls.HtmlInputButton inputButton)
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

                objTextControl.Attributes.Add("onkeydown", "try { fnTrapKD(document.all." + objTextControl.ClientID + "); } catch (e) {}");

                //Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "ForceDefaultToScript", sScript.ToString());
            }
            catch
            {
            }
        }


        public static void DefaultButton(System.Web.UI.Page Page, ref DocsPaWebCtrlLibrary.DateMask objTextControl, ref System.Web.UI.HtmlControls.HtmlInputButton inputButton)
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

                objTextControl.Attributes.Add("onkeydown", "try { fnTrapKD(document.all." + inputButton.ClientID + "); } catch (e) {}");

                //Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "ForceDefaultToScript", sScript.ToString());
            }
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref RadioButton radioButton, ref System.Web.UI.HtmlControls.HtmlInputButton inputButton)
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
            catch
            {
            }
        }

        public static void DefaultButton(System.Web.UI.Page Page, ref TextBox textBox, System.Web.UI.HtmlControls.HtmlInputButton inputButton)
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

                textBox.Attributes.Add("onkeydown", "try { fnTrapKD(document.all." + inputButton.ClientID + "); } catch (e) {}");

                //Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "ForceDefaultToScript", sScript.ToString());
            }
            catch
            {
            }
        }

        /// <summary>
        ///  pulisce oggetti sessione per back fascicoli/documenti.
        /// </summary>
        /// <param name="page"></param>
        public static void CleanSessionMemoria(Page page)
        {

            DocumentManager.removeMemoriaFiltriRicDoc(page);
            DocumentManager.removeMemoriaNumPag(page);
            DocumentManager.removeMemoriaTab(page);
            DocumentManager.RemoveMemoriaVisualizzaBack(page);

            FascicoliManager.removeMemoriaRicFasc(page);
            FascicoliManager.RemoveMemoriaVisualizzaBack(page);
            FascicoliManager.SetFolderViewTracing(page, false);
        }
        public static void DefaultButton(System.Web.UI.Page Page, ref CheckBox checkBox, ref System.Web.UI.HtmlControls.HtmlInputButton inputButton)
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
            catch
            {
            }
        }

        #endregion Metodi per settare le operazioni di default sul tasto invio

        public static string getIdAmmByCod(string codiceAmministrazione, Page page)
        {
            try
            {
                return docsPaWS.getIdAmmByCod(codiceAmministrazione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }


        public static string TruncateString(object t)
        {
            int lung = 0;
            string testo = t.ToString();
            if (System.Configuration.ConfigurationManager.AppSettings["TRUNCATESTRING"] != null)
            {
                lung = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TRUNCATESTRING"]);
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

        public static string TruncateString_MittDest(object t)
        {
            int lung = 0;
            string testo = t.ToString();
            if (System.Configuration.ConfigurationManager.AppSettings["TRUNCATESTRING_MITDEST"] != null)
            {
                lung = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TRUNCATESTRING_MITDEST"]);
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

        public static string isEnableConversionePdfLatoServer()
        {
            if (docsPaWS.isEnableConversionePdfLatoServer())
                return "true";
            else
                return "false";

        }

        public static string IsEbabledConversionePdfLatoServerSincrona()
        {
            if (docsPaWS.IsEnabledConversionePdfLatoServerSincrona())
                return "true";
            else
                return "false";

        }

        #region ricerca_note

        public static string[] splittaStringaRicercaNote(string valore)
        {
            // La stringa da ricercare nelle note, oltre al testo da
            // ricercare contiene anche la tipologia di ricerca 
            // richiesta
            string[] separatore = new string[] { "@-@" };
            string[] ricNote = valore.Split(separatore, StringSplitOptions.None);
            return ricNote;
        }

        #endregion

        public static ArrayList eseguiRicercaCampiComuni(SAAdminTool.DocsPaWR.InfoUtente infoUtente, SAAdminTool.DocsPaWR.FiltroRicerca[][] listaFiltri, int numPage, int pageSize, out int nRec, Page page)
         {
            ArrayList result = null;
            nRec = 0;

            try
            {
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                result = new ArrayList(docsPaWS.eseguiRicercaCampiComuni(infoUtente, listaFiltri, numPage, pageSize, out nRec));
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                result = null;
            }

            return result;
        }


        public static int getIntervalloDate(string dataUno, string dataDue)
        {
            int differenceInDays = 0;
            if (dataUno != "" && dataDue != "")
            {
                try
                {
                    dataUno = dataUno.Trim();
                    dataDue = dataDue.Trim();
                    CultureInfo ci = new CultureInfo("it-IT");
                    string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };
                    DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    DateTime d_Due = DateTime.ParseExact(dataDue, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                    TimeSpan ts = d_Due - d_Uno;

                    differenceInDays = ts.Days;

                    return differenceInDays;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else
            {
                return differenceInDays;
            }
        }

        public static string getDayWeekFromData(string dataUno)
        {
            string result = "";

            dataUno = dataUno.Trim();
            CultureInfo ci = new CultureInfo("it-IT");
            string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };
            DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

            result = d_Uno.ToString("dddd", ci); 

            return result;
        }


        public static string getMonthFromData(string dataUno)
        {
            string result = "";

            dataUno = dataUno.Trim();

            int inizio = dataUno.IndexOf('/');
            int fine = dataUno.LastIndexOf('/');

            result = dataUno.Substring(inizio+1, fine-3);

            int key = Convert.ToInt32(result);

            Hashtable months = new Hashtable();

            months.Add(01, "gennaio");
            months.Add(02, "febbraio");
            months.Add(03, "marzo");
            months.Add(04, "aprile");
            months.Add(05, "maggio");
            months.Add(06, "giugno");
            months.Add(07, "luglio");
            months.Add(08, "agosto");
            months.Add(09, "settembre");
            months.Add(10, "ottobre");
            months.Add(11, "novembre");
            months.Add(12, "dicembre");

            result = (string)months[key];

            return result;
        }
        /// <summary>
        /// controlla se è attivo oracle / swl effettuando una semplice query sulla DPA_AMMINISTRA e poi
        /// se presnete il documentale DTCM/WSPIA effettua una chiamata ad un WS DTCM.
        /// </summary>
        /// <returns></returns>
        public static bool checkSystem(out string msg)
        {
             msg = "";
            return   docsPaWS.CheckConnection(out msg);
             
        }

        /// <summary>
        /// Controlla se un documento / fascicolo è atipico o meno
        /// </summary>
        /// <param name="docOrFasc">Oggetto SchedaDocumento o oggetto Fascicolo</param>
        /// <param name="tipoOggetto">Enumeratore che identifica se si vuole verificare l'atipicita su un documento o fascicolo</param>
        /// <param name="btnVisibilita">Pulsante per la visualizzazione della visibilità che in caso di atipicità viene bordato rosso</param>
        /// <returns></returns>
        public static void verificaAtipicita(Object docOrFasc, DocsPaWR.TipoOggettoAtipico tipoOggetto, ref DocsPaWebCtrlLibrary.ImageButton btnVisibilita)
        {
            if (docOrFasc != null && GetAbilitazioneAtipicita())
            {
                InfoUtente infoUtente = UserManager.getInfoUtente();
                switch (tipoOggetto)
                {
                    case DocsPaWR.TipoOggettoAtipico.DOCUMENTO:
                        SchedaDocumento schedaDocumento = (SchedaDocumento)docOrFasc;
                        
                        if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.systemId) && schedaDocumento.InfoAtipicita != null && infoUtente != null)
                        {
                            schedaDocumento.InfoAtipicita = docsPaWS.GetInfoAtipicita(infoUtente, tipoOggetto, schedaDocumento.docNumber);
                            DocumentManager.setDocumentoSelezionato(schedaDocumento);
                            if (!string.IsNullOrEmpty(schedaDocumento.InfoAtipicita.CodiceAtipicita) && schedaDocumento.InfoAtipicita.CodiceAtipicita != "T")
                            {
                                btnVisibilita.BorderWidth = 1;
                                btnVisibilita.BorderColor = Color.Red;
                            }                            
                        }
                        break;

                    case DocsPaWR.TipoOggettoAtipico.FASCICOLO:
                        Fascicolo fascicolo = (Fascicolo)docOrFasc;
                        if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID) && fascicolo.InfoAtipicita != null)
                        {
                            fascicolo.InfoAtipicita = docsPaWS.GetInfoAtipicita(infoUtente, tipoOggetto, fascicolo.systemID);
                            FascicoliManager.setFascicoloSelezionato(fascicolo);
                         
                            if (!string.IsNullOrEmpty(fascicolo.InfoAtipicita.CodiceAtipicita) && fascicolo.InfoAtipicita.CodiceAtipicita != "T")
                            {
                                btnVisibilita.BorderWidth = 1;
                                btnVisibilita.BorderColor = Color.Red;
                            }                            
                        }                        
                        break;
                }
            }
        }

        public static bool GetAbilitazioneAtipicita()
        {
            bool result = false;
            InfoUtente infoUtente;

            try
            {
                infoUtente = UserManager.getInfoUtente();
            }
            catch (Exception e)
            {
                SessionManager sm = new SessionManager();
                infoUtente = sm.getUserAmmSession();
                string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                infoUtente.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);
 
            }

            string valoreChiaveAtipicita = SAAdminTool.utils.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ATIPICITA_DOC_FASC");
            if (string.IsNullOrEmpty(valoreChiaveAtipicita))
                valoreChiaveAtipicita = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "ATIPICITA_DOC_FASC");

            if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
                result = true;

            return result;
        }

        public static DocsPaWR.InfoAtipicita GetInfoAtipicita(DocsPaWR.TipoOggettoAtipico tipoOggetto, string idDocOrFasc)
        {
            InfoUtente infoUtente = UserManager.getInfoUtente();
            InfoAtipicita infoAtipicita = null;
            if (infoUtente != null && !string.IsNullOrEmpty(idDocOrFasc) && GetAbilitazioneAtipicita())
                infoAtipicita = docsPaWS.GetInfoAtipicita(infoUtente, tipoOggetto, idDocOrFasc);

            return infoAtipicita;
        }

        /// <summary>
        /// Metodo per verificare se un dato documento / fascicolo è stato creato da un ruolo attualmente storicizzato
        /// </summary>
        /// <param name="docOrFasc">Oggetto SchedaDocumento o Fascicolo da analizzare</param>
        /// <param name="btnHistory">Bottone a cui verrà aggiunto un bordo rosso spesso</param>
        public static void CheckCreatorRole(Object docOrFasc, ref DocsPaWebCtrlLibrary.ImageButton btnHistory)
        {
            String idCorrGlobAuthor = String.Empty;

            // Selezione del tipo di oggetto passato per parametro
            if (docOrFasc is SchedaDocumento && ((SchedaDocumento)docOrFasc).creatoreDocumento != null)
                    // Prelevamento dell'id corr globale del documento
                    idCorrGlobAuthor = ((SchedaDocumento)docOrFasc).creatoreDocumento.idCorrGlob_Ruolo;
            else
                if (docOrFasc is Fascicolo && ((Fascicolo)docOrFasc).creatoreFascicolo != null)
                    // Prelevamento dell'id corr globale del fascicolo
                    idCorrGlobAuthor = ((Fascicolo)docOrFasc).creatoreFascicolo.idCorrGlob_Ruolo;
                    
            // Se è stato identificato il tipo di oggetto, viene verificato se il ruolo risulta disabilitato
            if (!String.IsNullOrEmpty(idCorrGlobAuthor) &&
                docsPaWS.CheckIfRoleDisabled(idCorrGlobAuthor))
            {
                btnHistory.BorderWidth = Unit.Parse("2px");
                btnHistory.BorderColor = Color.Red;
            }
        }

        /// <summary>
        /// Metodo per verificare se un dato documento / fascicolo è stato creato da un ruolo attualmente storicizzato
        /// </summary>
        /// <param name="docOrFasc">Oggetto SchedaDocumento o Fascicolo da analizzare</param>
        public static bool CheckIfCreatorRoleIsDisabled(Object docOrFasc)
        {
            String idCorrGlobAuthor = String.Empty;

            // Selezione del tipo di oggetto passato per parametro
            if (docOrFasc is SchedaDocumento)
                // Prelevamento dell'id corr globale del documento
                idCorrGlobAuthor = ((SchedaDocumento)docOrFasc).creatoreDocumento.idCorrGlob_Ruolo;
            else
                if (docOrFasc is Fascicolo)
                    // Prelevamento dell'id corr globale del fascicolo
                    idCorrGlobAuthor = ((Fascicolo)docOrFasc).creatoreFascicolo.idCorrGlob_Ruolo;

            return docsPaWS.CheckIfRoleDisabled(idCorrGlobAuthor);
            
        }

        public static bool isEnableRepertori(string idAmministrazione)
        {
            string valueGestioneRepertori = utils.InitConfigurationKeys.GetValue(idAmministrazione, "GESTIONE_REPERTORI");
            if (string.IsNullOrEmpty(valueGestioneRepertori))
                valueGestioneRepertori = utils.InitConfigurationKeys.GetValue("0", "GESTIONE_REPERTORI");

            if (valueGestioneRepertori != null && valueGestioneRepertori == "1")
                return true;
            else
                return false;            
        }

        public static string ControlloContatoriTipologiaDocFasc(OggettoCustom oggDaInserire, Templates template)
        {
            string msg = string.Empty;
            
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                SAAdminTool.DocsPaWR.OggettoCustom ogg = (SAAdminTool.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];

                if (oggDaInserire.TIPO_CONTATORE.Equals("T") && ogg.TIPO_CONTATORE.Equals("T") && !oggDaInserire.DESCRIZIONE.Equals(ogg.DESCRIZIONE))
                {
                    msg = "Esiste già un contatore di tipologia per questo tipo di documento.";
                }
                else if (oggDaInserire.REPERTORIO.Equals("1") && ogg.REPERTORIO.Equals("1") && !oggDaInserire.DESCRIZIONE.Equals(ogg.DESCRIZIONE))
                {
                    msg = "Esiste già un contatore di repertorio su questo tipo di documento.";
                    template.ELENCO_OGGETTI.Where(o => o.DESCRIZIONE.ToString().Equals(oggDaInserire.DESCRIZIONE)).FirstOrDefault().REPERTORIO = "0";
                }
            }
            
            return msg;
        }

        internal static bool GetAbilitazioneCopiaVisibilita()
        {
            bool result = false;
            InfoUtente infoUtente;

            try
            {
                infoUtente = UserManager.getInfoUtente();
            }
            catch (Exception e)
            {
                SessionManager sm = new SessionManager();
                infoUtente = sm.getUserAmmSession();
                string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                infoUtente.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);

            }

            string valoreChiaveAtipicita = SAAdminTool.utils.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "FE_COPIA_VISIBILITA");
            if (string.IsNullOrEmpty(valoreChiaveAtipicita))
                valoreChiaveAtipicita = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_COPIA_VISIBILITA");

            if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
                result = true;

            return result;
        }

        internal static bool GetAbilitazioneGestioneRuoliAvanzata()
        {
            bool result = false;
            InfoUtente infoUtente;

            try
            {
                infoUtente = UserManager.getInfoUtente();
            }
            catch (Exception e)
            {
                SessionManager sm = new SessionManager();
                infoUtente = sm.getUserAmmSession();
                string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                infoUtente.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);

            }

            string valoreChiaveAtipicita = SAAdminTool.utils.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "FE_GEST_RUOLI_AVANZATA");
            if (string.IsNullOrEmpty(valoreChiaveAtipicita))
                valoreChiaveAtipicita = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_GEST_RUOLI_AVANZATA");

            if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
                result = true;

            return result;
        }


        public static int CheckVatNumber(string vatNum)
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


        //Mev Firma1 < aggiunto function getEnteCertificatore()
        #region Formattazione Ente Certificatore
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

            return string.Format("{0}{1}{2}", enteCertCN, enteCertO, enteCertC);
        }

        #endregion
        //>

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsEnabledSupportedFileTypes()
        {
            try
            {
                return docsPaWS.IsEnabledSupportedFileTypes();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static SupportedFileType[] GetSupportedFileTypes(int idAmministrazione)
        {
            try
            {
                return docsPaWS.GetSupportedFileTypes(idAmministrazione);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Verifica se il formato del file è ammesso per la firma digitale
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public static bool IsFormatSupportedForSign(DocsPaWR.FileRequest fileRequest)
        {
            bool retValue = false;

            if (!SAAdminTool.FormatiDocumento.Configurations.SupportedFileTypesEnabled)
            {
                retValue = true;
            }
            else
            {
                string extension = System.IO.Path.GetExtension(fileRequest.fileName);

                if (!string.IsNullOrEmpty(extension))
                {
                    // Rimozione del primo carattere dell'estensione (punto)
                    extension = extension.Substring(1);

                    DocsPaWR.SupportedFileType[] fileTypes = ProxyManager.getWS().GetSupportedFileTypes(Convert.ToInt32(UserManager.getInfoUtente().idAmministrazione));

                    retValue = fileTypes.Count(e => e.FileExtension.ToLower() == extension.ToLower() &&
                                                e.FileTypeUsed && e.FileTypeSignature) > 0;
                }
            }

            return retValue;
        }
    }

}
