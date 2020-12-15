using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using log4net;

namespace NttDataWA.Popup
{
    public partial class PrintLabel : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(PrintLabel));

        private SchedaDocumento documento;
        private Fascicolo project;

        private string strToZebra;
        private bool isProject;

        public static string componentType = Constans.TYPE_ACTIVEX;
        public static string printer = "DYMO_LABEL_WRITER_400";

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                
                componentType = UserManager.getComponentType(Request.UserAgent);
                printer = UserManager.getDispositivoStampaUtente(UserManager.GetUserInSession().idPeople);
                isProject = (!string.IsNullOrEmpty(Request.QueryString.Get("type")));

                if (componentType == Constans.TYPE_APPLET && printer != "DYMO_LABEL_WRITER_400")
                    this.plcApplet.Visible = true;
                else
                    this.plcApplet.Visible = false;

                if (!Page.IsPostBack)
                {
                    
                    this.documento = DocumentManager.getSelectedRecord();
                    this.project = ProjectManager.getProjectInSession();
                   
                    if (this.documento != null || isProject)
                    {
                        caricaCampiEtichetta();
                   
                        // DYMO - Ignoriamo i componenti e usiamo il Dymo Label Framework
                        if (!string.IsNullOrWhiteSpace(printer) && printer.Equals("DYMO_LABEL_WRITER_400"))
                        {
                            if (!PrintLabel_alreadyPrinted)
                            {
                                printPen.printPen pp = new  printPen.printPen();

                                // Volani: Path configurato via chiave webconfig, permette di avere un path dinamico su amm/reg/uo                          
                                pp.PathDymo = GetLabelsPath();                               

                                //pp.PathDymo = System.Web.HttpContext.Current.Server.MapPath(null);
                                //string urlDymo = pp.getDymoBaseUrl();
                                //pp.init_PrintPen();  // Non serve x dymo, questo medoto cerca il file ini della zebra
                             
                                // Volani: Aggiungiamo un prefisso all'ID del documento necessario x il funzionamento della massiva
                                // in quanto non configurabile da etichetta. 
                                // Questo parametro sarebbe da mettere su chiave DB x adm oppure passare un parametro dedicato x il barcode allo script
                                // pp.NumeroDocumento = this.hd_num_doc.Value;// "unescape(document.forms[0].hd_num_doc.value)";
                                string dymoPrefix = "" + System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DYMO_DOCNUMBER_PREFIX.ToString()];
                                if (String.IsNullOrEmpty(dymoPrefix))
                                    pp.NumeroDocumento = this.hd_num_doc.Value;
                                else
                                    pp.NumeroDocumento = dymoPrefix + "$" + this.hd_num_doc.Value;

                                pp.Classifica = pp.ClassificazioneFascicolo = this.hd_classifica.Value; // "unescape(document.forms[0].hd_classifica.value)";
                                pp.Fascicolo = pp.CodiceFascicolo = this.hd_fascicolo.Value;// "unescape(document.forms[0].hd_fascicolo.value)";
                                pp.DescrizioneFascicolo = this.hd_fascicoloDesc.Value;
                                pp.AperturaFascicolo = this.hd_dataCreazione.Value;
                                pp.Amministrazione_Etichetta =this.hd_amministrazioneEtichetta.Value; // "unescape(document.forms[0].hd_amministrazioneEtichetta.value)";
                                pp.CodiceUoProtocollatore = this.hd_coduo_proto.Value;// "unescape(document.forms[0].hd_coduo_proto.value)";
                                pp.CodiceRegistroProtocollo =  this.hd_codreg_proto.Value;//"unescape(document.forms[0].hd_codreg_proto.value)";
                                pp.TipoProtocollo = this.hd_tipo_proto.Value;//"unescape(document.forms[0].hd_tipo_proto.value)"; 
                                pp.NumeroProtocollo = this.hd_num_proto.Value;//"unescape(document.forms[0].hd_num_proto.value)"; 
                                pp.AnnoProtocollo = this.hd_anno_proto.Value;//"unescape(document.forms[0].hd_anno_proto.value)"; 
                                pp.DataProtocollo = this.hd_data_proto.Value;//"unescape(document.forms[0].hd_data_proto.value)"; 
                                pp.NumeroAllegati = this.hd_numero_allegati.Value;
                                pp.DataCreazione = this.hd_dataCreazione.Value;
                                
                                pp.CodiceUoCreatore = this.hd_codiceUoCreatore.Value;
                                pp.DescrizioneRegistroProtocollo = this.hd_descreg_proto.Value;
                                pp.UrlFileIni = this.hd_UrlIniFileDispositivo.Value;//"unescape(document.forms[0].hd_UrlIniFileDispositivo.value)"; 
                                pp.Amministrazione = this.hd_descrizioneAmministrazione.Value;//"unescape(document.forms[0].hd_descrizioneAmministrazione.value)"; 
                                pp.DataArrivo = this.hd_dataArrivo.Value;
                                pp.DataArrivoEstesa = this.hd_dataArrivoEstesa.Value;
                                pp.Dispositivo = this.hd_dispositivo.Value;//"unescape(document.forms[0].hd_dispositivo.value)"; 
                                pp.ModelloDispositivo = this.hd_modello_dispositivo.Value; //"unescape(document.forms[0].hd_modello_dispositivo.value)"; 
                                pp.NumeroStampe = EnumLabel;//this.hd_num_stampe.Value;
                                pp.NumeroStampeEffettuate = this.hd_num_stampe_effettuate.Value;
                                pp.NumeroStampaCorrente = this.hd_num_stampe_effettuate.Value;
                                pp.OraCreazione = this.hd_ora_creazione.Value;
                                pp.Text = this.hd_num_proto.Value;// "unescape(document.forms[0].hd_num_proto.value)";
                            
                                string dymoPrintScript="";

                                if (isProject)
                                {
                                    pp.DymoXML = pp.getDymoFile("ET_DYMO_FASC.label");
                                    dymoPrintScript = pp.getDymoFascPrintScript(false);
                                }
                                else
                                {
                                    pp.DymoXML = pp.getDymoFile("ET_DYMO.label");
                                    dymoPrintScript = pp.getDymoPrintScript(false);
                                }
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "IncludeDymoPrint", dymoPrintScript, true);
                                                        
                                if (!PrintLabel_alreadyPrinted2)//if (!PrintLabel_alreadyPrinted)
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "dymoprint", "dymoPrint();parent.closeAjaxModal('PrintLabel','');", true);
                                  
                                    PrintLabel_alreadyPrinted = true;
                                    
                                }
                                PrintLabel_alreadyPrinted2 = true;
                            }
                        }
                        // ActiveX + Zebra
                        else if (componentType != Constans.TYPE_APPLET && componentType != Constans.TYPE_SOCKET)
                        {
                            if (!PrintLabel_alreadyPrinted)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "print_labels", "start_stampe();", true);
                                PrintLabel_alreadyPrinted = true;
                            }
                        }        
                        // applet + Zebra
                        else if (!string.IsNullOrWhiteSpace(printer) && printer.Equals("ZEBRA"))
                        {
                            //if (!PrintLabel_alreadyPrinted)
                            //{
                            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiuditi", "alert('Non implementato.');parent.closeAjaxModal('PrintLabel','');", true);
                            
                            //    if (PrintLabel_alreadyPrinted2)
                            //        PrintLabel_alreadyPrinted = true;
                            
                            //    PrintLabel_alreadyPrinted2 = true;
                            //}

                            //Applet
                            if (componentType == Constans.TYPE_APPLET || componentType == Constans.TYPE_SOCKET)
                            {
                                printPen.printPen pp = new printPen.printPen();
                                //pp.PathZebra = System.Web.HttpContext.Current.Server.MapPath(null);         
                                pp.PathZebra = GetLabelsPath();                     
                                pp.FILEINI_NAME = "DOCSPA.INI";
                                pp.init_PrintPen();

                                pp.Classifica = pp.ClassificazioneFascicolo = this.hd_classifica.Value; // "unescape(document.forms[0].hd_classifica.value)";
                                pp.Fascicolo = pp.CodiceFascicolo = this.hd_fascicolo.Value;// "unescape(document.forms[0].hd_fascicolo.value)";
                                pp.DescrizioneFascicolo = this.hd_fascicoloDesc.Value;
                                pp.AperturaFascicolo = this.hd_dataCreazione.Value;
                                pp.Amministrazione_Etichetta = this.hd_amministrazioneEtichetta.Value; // "unescape(document.forms[0].hd_amministrazioneEtichetta.value)";
                                pp.NumeroDocumento = this.hd_num_doc.Value;// "unescape(document.forms[0].hd_num_doc.value)";
                                pp.CodiceUoProtocollatore = this.hd_coduo_proto.Value;// "unescape(document.forms[0].hd_coduo_proto.value)";
                                pp.CodiceRegistroProtocollo = this.hd_codreg_proto.Value;//"unescape(document.forms[0].hd_codreg_proto.value)";
                                pp.TipoProtocollo = this.hd_tipo_proto.Value;//"unescape(document.forms[0].hd_tipo_proto.value)"; 
                                pp.NumeroProtocollo = this.hd_num_proto.Value;//"unescape(document.forms[0].hd_num_proto.value)"; 
                                pp.AnnoProtocollo = this.hd_anno_proto.Value;//"unescape(document.forms[0].hd_anno_proto.value)"; 
                                pp.DataProtocollo = this.hd_data_proto.Value;//"unescape(document.forms[0].hd_data_proto.value)"; 
                                pp.NumeroAllegati = this.hd_numero_allegati.Value;
                                pp.DataCreazione = this.hd_dataCreazione.Value;
                                pp.CodiceUoCreatore = this.hd_codiceUoCreatore.Value;
                                pp.DescrizioneRegistroProtocollo = this.hd_descreg_proto.Value;
                                pp.UrlFileIni = this.hd_UrlIniFileDispositivo.Value;//"unescape(document.forms[0].hd_UrlIniFileDispositivo.value)"; 
                                pp.Amministrazione = this.hd_descrizioneAmministrazione.Value;//"unescape(document.forms[0].hd_descrizioneAmministrazione.value)"; 
                                pp.DataArrivo = this.hd_dataArrivo.Value;
                                pp.DataArrivoEstesa = this.hd_dataArrivoEstesa.Value;
                                pp.Dispositivo = this.hd_dispositivo.Value;//"unescape(document.forms[0].hd_dispositivo.value)"; 
                                pp.ModelloDispositivo = this.hd_modello_dispositivo.Value; //"unescape(document.forms[0].hd_modello_dispositivo.value)"; 
                                pp.NumeroStampe = EnumLabel;//this.hd_num_stampe.Value;
                                pp.NumeroStampeEffettuate = this.hd_num_stampe_effettuate.Value;
                                pp.NumeroStampaCorrente = this.hd_num_stampe_effettuate.Value;
                                pp.OraCreazione = this.hd_ora_creazione.Value;
                                pp.Text = this.hd_num_proto.Value;// "unescape(document.forms[0].hd_num_proto.value)";

                                //per ora è hard-coded, bisogna implementare la gestione per associare le amministrazioni
                                //pp.DymoXML = pp.getDymoFile("ET_DYMO.label");

                                string zebraString = string.Empty;
                                if (isProject)
                                {
                                    if (pp.StampaFascicolo(ref zebraString))
                                        this.strToZebra = zebraString.Replace("\0", "{}");
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(pp.NumeroProtocollo))
                                    {
                                        if (pp.StampaGrigio(ref zebraString))
                                            this.strToZebra = zebraString.Replace("\0", "{}");
                                    }
                                    else
                                    {
                                        if (pp.Stampa(ref zebraString))
                                            this.strToZebra = zebraString.Replace("\0", "{}");
                                    } 
                                }
                                
                                if (!PrintLabel_alreadyPrinted)
                                {
                                    if(componentType == Constans.TYPE_SOCKET)
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "applet_labels", "PrintWithSocket('" + this.strToZebra + "','" + pp.GetZebraPrinterName() + "');", true);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "applet_labels", "PrintWithApplet('" + this.strToZebra + "','" + pp.GetZebraPrinterName() + "');", true);
                                    }
                                    PrintLabel_alreadyPrinted = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        /// <summary>
        /// Utilizzato da stampa dymo e applet. Ritorna il path locale contenente i file di configurazione etichetta
        /// </summary>
        /// <returns>Percorso lato server contenente i file x la stampa</returns>        
        private string GetLabelsPath() {

            // carica path e sostituisci eventuali parametri dinamici
            string labelsPath = "" + System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LABELS_PATH.ToString()];

            if (string.IsNullOrEmpty(labelsPath))
            {
                // se la chiave di config non esiste ritorna il path corrente del server come default
                labelsPath = System.Web.HttpContext.Current.Server.MapPath(null);
            }
            else
            {
                labelsPath = labelsPath.Replace("%COD_AMM%", getCodiceAmministrazione(UserManager.GetUserInSession().idAmministrazione));
                labelsPath = labelsPath.Replace("%COD_REG%", this.hd_codreg_proto.Value);
                labelsPath = labelsPath.Replace("%COD_UO%", this.hd_coduo_proto.Value);
            }

            logger.DebugFormat("Path etichette: {0}", labelsPath);

            return labelsPath;
        }

        /// <summary>
        /// utilizzato da ActiveX di stampa. estrae la descrizione dell' Amministrazione Attuale.
        /// </summary>
        /// <param name="IdAmministrazione"></param>
        /// <returns>restituisce la descrizione dell'amministrazione passata con il parametro di input IdAmministrazione</returns>        
        private string getDescAmministrazione(string IdAmministrazione)
        {
            string descAmm = string.Empty;
            string returnMsg = string.Empty;

            DocsPaWR.Amministrazione[] amministrazioni = UserManager.getListaAmministrazioni(this, out returnMsg);

            if (amministrazioni.Length == 1)
            {
                descAmm = amministrazioni[0].descrizione;
            }
            else
            {
                bool found = false;
                int i = 0;

                while ((!found) && (i < amministrazioni.Length))
                {
                    if (amministrazioni[i].systemId == IdAmministrazione)
                    {
                        found = true;
                        descAmm = amministrazioni[i].descrizione;
                    }

                    i++;
                }
            }

            return descAmm;
        }

        /// <summary>
        /// utilizzato da ActiveX di stampa. estrae il codice dell' Amministrazione Attuale.
        /// </summary>
        /// <param name="IdAmministrazione"></param>
        /// <returns>restituisce il codice dell'amministrazione passata con il parametro di input IdAmministrazione</returns>

        private string getCodiceAmministrazione(string IdAmministrazione)
        {
            string codAmm = string.Empty;
            string returnMsg = string.Empty;

            DocsPaWR.Amministrazione[] amministrazioni = UserManager.getListaAmministrazioni(this, out returnMsg);

            if (amministrazioni.Length == 1)
            {
                codAmm = amministrazioni[0].codice;
            }
            else
            {
                bool found = false;
                int i = 0;

                while ((!found) && (i < amministrazioni.Length))
                {
                    if (amministrazioni[i].systemId == IdAmministrazione)
                    {
                        found = true;
                        codAmm= amministrazioni[i].codice;
                    }

                    i++;
                }
            }

            return codAmm;
        }

        /// <summary>
        /// utilizzato da ActiveX di stampa. Estrae il codice classifica a cui la scheda documento è associata.
        /// </summary>
        /// <returns>codice classifica</returns>
        private string getClassificaPrimaria()
        {
            string codClassifica = "";
            if (documento != null)
            {
                //RECUPERARE VALORE PER infoDocumento
                DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(documento);
                codClassifica = DocumentManager.GetClassificaDoc(this, infoDocumento.idProfile);
            }
            return codClassifica;
        }

        /// <summary>
        /// utilizzato da ActiveX di stampa. Estrae il codice fascicolo a cui il documento è associato
        /// </summary>
        /// <returns>codice Fascicolo</returns>
        private string getCodiceFascicolo()
        {
            string codFascicolo = "";

            if (documento != null)
            {
                //RECUPERARE VALORE PER infoDocumento
                DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord());
                codFascicolo = DocumentManager.getFascicoloDoc(this, infoDocumento);
            }
            return codFascicolo;
        }


        private string formatProtocollo(string numero)
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

        private void caricaCampiEtichetta()
        {
            #region parametro Dispositivo Di Stampa
            
            var dispositivoStampaUtente = UserManager.getDispositivoStampaUtente(UserManager.GetUserInSession().idPeople);
            if (dispositivoStampaUtente != null)
            {
                this.hd_dispositivo.Value = "Etichette";
                this.hd_modello_dispositivo.Value = dispositivoStampaUtente.ToString();
            }
            else
                this.hd_dispositivo.Value = "Penna";


            #endregion parametro Dispositivo Di Stampa

            #region parametro Descrizione Amministrazione
            string descAmm = getDescAmministrazione(UserManager.GetUserInSession().idAmministrazione);
            #endregion parametro Descrizione Amministrazione

            #region parametro Classifica Primaria
            string classificaPrimaria = String.Empty;

            string classificazioneInEtichetta = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.StampaClassificazioneInEtichetta.ToString()];
            if (classificazioneInEtichetta != null)
            {
                switch (classificazioneInEtichetta)
                {
                    case "1": // stampa il codice classifica In Etichetta
                        classificaPrimaria = getClassificaPrimaria();
                        break;
                    default:
                        //massimo digregorio, non necessario se l'assegnazione avviene in dichiarazione. old: classificaPrimaria = String.Empty;
                        break;
                }
            }
            this.hd_classifica.Value = classificaPrimaria;
            #endregion parametro Classifica Primaria

            #region parametro Fascicolo primario
            string fascicoloInEtichetta = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.StampaFascicoloInEtichetta.ToString()];
            if (fascicoloInEtichetta != null)
            {
                switch (fascicoloInEtichetta)
                {
                    case "1": // stampa il codice fascicolo In Etichetta
                        this.hd_fascicolo.Value = getCodiceFascicolo();
                        break;
                    default:
                        this.hd_fascicolo.Value = String.Empty;
                        break;
                }
            }
            #endregion parametro Fascicolo primario

            #region patch per cuneo
            string descAmministrInEtichetta = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.StampaDescrizioneAmministrazioneInEtichetta.ToString()];
            if (descAmministrInEtichetta != null)
            {
                switch (descAmministrInEtichetta)
                {
                    case "1": // Stampa Descrizione Amministrazione In Etichetta
                        this.hd_amministrazioneEtichetta.Value = descAmm;
                        break;
                    default:
                        this.hd_amministrazioneEtichetta.Value = String.Empty;
                        break;
                }
            }

            //aggiuto tag Hidden "hd_desAmministrazione" per ActiveX di stampa
            /* se parametro esiste ed a 0, a hd_desAmministrazione viene assegnata la classifica
                     * se parametro non esiste o esiste <> 0, a hd_desAmministrazione viene assegnata la descrizione dell'amministrazione
                     */
            bool BarCodeConAmministrazione = true;
            DocsPaWR.Configurazione visualizzaClassificaSopraBarCode = UserManager.getParametroConfigurazione(this.Page);

            if (visualizzaClassificaSopraBarCode != null)
            {
                if (visualizzaClassificaSopraBarCode.valore.Equals("0")) BarCodeConAmministrazione = false;
            }

            if (BarCodeConAmministrazione)
            {
                this.hd_descrizioneAmministrazione.Value = descAmm;
            }
            else
            {
                this.hd_descrizioneAmministrazione.Value = classificaPrimaria;
            }

            #endregion patch per cuneo

            #region parametro URL File di configurazione Dispositivo di Stampa
          
            this.hd_UrlIniFileDispositivo.Value = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.URL_INIFILE_DISPOSITIVO_STAMPA.ToString()];            
            // Sostituzione campi %COD_AMM% %COD_REG% e %COD_UO% nell'url dell'etichetta in modo da renderlo dinamico x le multi amm
            this.hd_UrlIniFileDispositivo.Value = this.hd_UrlIniFileDispositivo.Value.Replace("%COD_AMM%", getCodiceAmministrazione(UserManager.GetUserInSession().idAmministrazione));        
            this.hd_UrlIniFileDispositivo.Value = this.hd_UrlIniFileDispositivo.Value.Replace("%COD_REG%", this.hd_codreg_proto.Value);
            this.hd_UrlIniFileDispositivo.Value = this.hd_UrlIniFileDispositivo.Value.Replace("%COD_UO%", this.hd_coduo_proto.Value);

            #endregion parametro URL File di configurazione Dispositivo di Stampa

            #region stampa multipla etichetta
            string abilita_multi_stampa_etichetta = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_MULTI_STAMPA_ETICHETTA.ToString());//utils.InitConfigurationKeys.GetValue("0", "FE_MULTI_STAMPA_ETICHETTA");

            if (this.documento != null)
            {
                if (!string.IsNullOrEmpty(abilita_multi_stampa_etichetta) && abilita_multi_stampa_etichetta.Equals("1"))
                {

                    // recupero il numero di stampe del documento da effettuare
                    if (!string.IsNullOrWhiteSpace(EnumLabel))
                        this.hd_num_stampe.Value = EnumLabel;
                    else
                        this.hd_num_stampe.Value = "1";
                    // recupero il valore di stampa corrente da inserire nella  successiva etichetta da stampare
                    int num_stampe_eff;
                    if (this.documento.protocollo != null && !String.IsNullOrEmpty(this.documento.protocollo.stampeEffettuate))
                    {
                        num_stampe_eff = Convert.ToInt32(this.documento.protocollo.stampeEffettuate) + 1;
                        this.hd_num_stampe_effettuate.Value = num_stampe_eff.ToString();
                    }
                    else
                        this.hd_num_stampe_effettuate.Value = "1";

                }
                else
                {
                    this.hd_num_stampe_effettuate.Value = "1";
                    this.hd_num_stampe.Value = "1";
                }

            #endregion

                #region parametri scheda Documento
                this.hd_num_doc.Value = documento.docNumber;
                this.hd_dataCreazione.Value = this.documento.dataCreazione;
                this.hd_codiceUoCreatore.Value = documento.creatoreDocumento.uo_codiceCorrGlobali;
                if (!string.IsNullOrEmpty(this.documento.oraCreazione))
                {
                    this.hd_ora_creazione.Value = this.documento.oraCreazione;
                    this.hd_ora_creazione.Value = this.hd_ora_creazione.Value.Substring(0, 5);
                }

                this.hd_tipo_proto.Value = DocumentManager.GetCodeLabel(documento.tipoProto);

                this.hd_coduo_proto.Value = String.Empty;//è gestito sul db e sull'oggetto ruolo utente attuale, ma non nell'oggetto schedaDocumento;

                if (documento.registro != null)
                {
                    this.hd_codreg_proto.Value = documento.registro.codRegistro;
                    this.hd_descreg_proto.Value = documento.registro.descrizione;
                }

                if (documento.protocollo != null && documento.protocollo.numero != null)
                {
                    //Celeste
                    //this.hd_num_proto.Value = schedaDocumento.protocollo.numero;
                    this.hd_num_proto.Value = formatProtocollo(documento.protocollo.numero);
                    //Fine Celeste
                    this.hd_anno_proto.Value = documento.protocollo.anno;
                    this.hd_data_proto.Value = documento.protocollo.dataProtocollazione;

                    //massimo digregorio new:
                    if (documento.protocollatore != null)
                        this.hd_coduo_proto.Value = documento.protocollatore.uo_codiceCorrGlobali;
                }

                #endregion parametri scheda Documento

                #region Parametri allegati

                // Impostazione del numero degli allegati
                this.hd_numero_allegati.Value = documento.allegati.Length.ToString();

                #endregion

                #region parametro data arrivo
                if (documento != null && documento.documenti != null)
                {
                    int firstDoc = (documento.documenti.Length > 0) ? documento.documenti.Length - 1 : 0;
                    if (string.IsNullOrEmpty(documento.documenti[firstDoc].dataArrivo))
                    {
                        this.hd_dataArrivo.Value = string.Empty;
                        this.hd_dataArrivoEstesa.Value = string.Empty;
                    }
                    else
                    {
                        DateTime dataArrivo;
                        DateTime.TryParse(documento.documenti[firstDoc].dataArrivo, out dataArrivo);
                        this.hd_dataArrivo.Value = dataArrivo.ToString("d");
                        this.hd_dataArrivoEstesa.Value = documento.documenti[firstDoc].dataArrivo;
                    }
                }
                else
                {
                    this.hd_dataArrivo.Value = string.Empty;
                    this.hd_dataArrivoEstesa.Value = string.Empty;
                }
                #endregion
            }
            else
            {
                if (this.project != null) 
                {
                    InfoFascicolo infoPrj = ProjectManager.getInfoFascicoloDaFascicolo(project);
                    this.hd_fascicolo.Value = infoPrj.codice;
                    this.hd_fascicoloDesc.Value = infoPrj.descrizione;
                    this.hd_classifica.Value = infoPrj.descClassificazione;
                    this.hd_dataCreazione.Value = infoPrj.apertura;
                }
            }
        }

        public string EnumLabel
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["enumLabel"] != null)
                {
                    result = HttpContext.Current.Session["enumLabel"].ToString();
                }
                return result;

            }
            set
            {
                HttpContext.Current.Session["enumLabel"] = value;
            }
        }

        public bool PrintLabel_alreadyPrinted
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["printlabel_alreadyprinted"] != null)
                {
                    result = Boolean.Parse(HttpContext.Current.Session["printlabel_alreadyprinted"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["printlabel_alreadyprinted"] = value.ToString();
            }
        }

        public bool PrintLabel_alreadyPrinted2
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["printlabel_alreadyprinted2"] != null)
                {
                    result = Boolean.Parse(HttpContext.Current.Session["printlabel_alreadyprinted2"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["printlabel_alreadyprinted2"] = value.ToString();
            }
        }

        public string getZebraString()
        {
            return this.strToZebra;
        }
    }
}