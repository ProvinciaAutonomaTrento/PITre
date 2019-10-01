using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Configuration;
using DocsPaVO.Caching;
using log4net;


namespace SAAdminTool.AdminTool.Gestione_Cache
{
    public partial class Cache : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(Cache));
        /// <summary>
        /// varibile privat aper ricercare il nome del server
        /// </summary>
       
        private const int kbyte = 1024;
        private const int mbyte = 1048576;
        SAAdminTool.DocsPaWR.DocsPaWebService ws = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                hd_idAmm.Value= ws.getIdAmmByCod(codiceAmministrazione);
                Initialize();
            }


        }



        #region gestione degli eventi dei tasti
        /// <summary>
        /// gestione del tasto modifica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_annulla_Click(object sender, EventArgs e)
        {
            if (btn_salva.Text == "Salva")
                clearCampi();
            else
                Initialize();
        }
        /// <summary>
        /// gestione del tasto salva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_salva_Click(object sender, EventArgs e)
        {
            //controllo se ci sono tutti i dati da settare
            if (txt_dim_cache.Text != string.Empty
               && txt_dim_file.Text != string.Empty
               && txt_doc_root.Text != string.Empty
               && txt_ora_fine.Text != string.Empty
               && txt_ora_start.Text != string.Empty
               && txt_ws_generale.Text != string.Empty
               && txt_wsLocale.Text != string.Empty
               && txt_docLocale.Text != string.Empty
               && txt_min_start.Text != string.Empty
               && txt_min_fine.Text != string.Empty
               && txt_ora_fine.Text.Length == 2
               && txt_ora_start.Text.Length == 2
               && txt_min_fine.Text.Length == 2
               && txt_min_start.Text.Length == 2)
            {

                if (IsNumeric(txt_min_start.Text)
                && IsNumeric(txt_ora_start.Text)
                && IsNumeric(txt_min_fine.Text)
                && IsNumeric(txt_ora_fine.Text))
                {

                    SAAdminTool.DocsPaWR.CacheConfig info = new SAAdminTool.DocsPaWR.CacheConfig();

                    info.doc_root_server = txt_doc_root.Text;
                    info.url_ws_caching_locale = txt_wsLocale.Text;
                    info.doc_root_server_locale = txt_docLocale.Text;

                    if (ck_dim_cache_infinito.Checked)
                        info.massima_dimensione_caching = -1;
                    else
                    {
                        info.massima_dimensione_caching = double.Parse(txt_dim_cache.Text);
                        //ridimensionamento da MByte a byte ---> MByte * 1048576
                        info.massima_dimensione_caching = info.massima_dimensione_caching * mbyte;
                    }

                    if (ck_dim_file_infinito.Checked)
                        info.massima_dimensione_file = -1;
                    else
                    {
                        info.massima_dimensione_file = double.Parse(txt_dim_file.Text);
                        //ridimesionamento da kb a byte ---> kbyte * 1024
                        info.massima_dimensione_file = info.massima_dimensione_file * kbyte;
                    }

                    info.ora_fine_cache = controllaOrario(txt_ora_fine.Text + ":" + txt_min_fine.Text);
                    info.ora_inizio_cache = controllaOrario(txt_ora_start.Text + ":" + txt_min_start.Text);
                    info.caching = cb_cache.Checked;
                    info.idAmministrazione = hd_idAmm.Value;
                    info.urlwscaching = txt_ws_generale.Text;
                    //eseguo la query
                    if(ws==null)
                        ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                    
                    if (btn_salva.Text.ToUpper() == "Salva".ToUpper())
                    {
                        if (ws.setConfigurazioneCache(info))
                            modifica_effettuata.Alert("Salvataggio effettuata con successo");
                        else
                            modifica_effettuata.Alert("Salvataggio non effettuata");
                    }
                        
                        else
                        {
                            if (ws.updateConfigurazioneCache(info))
                                modifica_effettuata.Alert("Modifica effettuata con successo");
                            else
                                modifica_effettuata.Alert("Modifica non effettuata");
                        }
                }
                else
                    RegisterStartupScript("disabled", "<SCRIPT>alert('Formato ora o minuti non corretto');</SCRIPT>");
            }
            else
                RegisterStartupScript("disabled", "<SCRIPT>alert('Riempire tutti i campi obbligatori (*)');</SCRIPT>");
        }

        /// <summary>
        /// gestione del tasto elimina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_elimina_Click(object sender, EventArgs e)
        {
            msg_elimina.Confirm("Sei sicuro di voler cancellare la configurazione?");
        }


        /// <summary>
        ///  gestione dell'evento del messaggio di conferma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void msg_elimina_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                ws.deleteConfigurazioneCache(hd_idAmm.Value);
                clearCampi();
            }
        }




        #endregion

        #region funzioni per la gestione dei campi della webform
        /// <summary>
        /// pulizia dei campi della webfrom
        /// </summary>
        private void clearCampi()
        {
            txt_dim_cache.Text = string.Empty;
            txt_dim_file.Text = string.Empty;
            txt_ora_fine.Text = string.Empty;
            txt_ora_start.Text = string.Empty;
            txt_min_fine.Text = string.Empty;
            txt_min_start.Text = string.Empty;
            txt_doc_root.Text = string.Empty;
            cb_cache.Checked = false;
            btn_salva.Text = "Salva";
            btn_elimina.Visible = false;
            ck_dim_file_infinito.Checked = false;
            ck_dim_cache_infinito.Checked = false;
            txt_ws_generale.Text = string.Empty;
            txt_wsLocale.Text = string.Empty;
            txt_docLocale.Text = string.Empty;
        }
        /// <summary>
        /// funzione che inizializza la web form
        /// </summary>
        protected void Initialize()
        {

            if(ws== null)
                ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            string idAmm = hd_idAmm.Value;
            SAAdminTool.DocsPaWR.CacheConfig info = ws.getConfigurazioneCache(idAmm);
            if (info != null)
            {
                if (info.massima_dimensione_caching != -1.0)
                {
                    txt_dim_cache.Text = (info.massima_dimensione_caching / mbyte).ToString();
                    txt_dim_cache.Style["text-align"] = "right";
                    ck_dim_cache_infinito.Checked = false;
                }
                else
                {
                    txt_dim_cache.Text = " ";
                    ck_dim_cache_infinito.Checked = true;
                }

                if (info.massima_dimensione_file != -1.0)
                {
                    txt_dim_file.Text = (info.massima_dimensione_file / kbyte).ToString();
                    txt_dim_file.Style["text-align"] = "right";
                    ck_dim_file_infinito.Checked = false;
                }
                else
                {
                    txt_dim_file.Text = " ";
                    ck_dim_file_infinito.Checked = true;
                }
                txt_ora_fine.Text = info.ora_fine_cache.Substring(0, 2);
                txt_ora_start.Text = info.ora_inizio_cache.Substring(0, 2);
                txt_min_fine.Text = info.ora_fine_cache.Substring(3, 2);
                txt_min_start.Text = info.ora_inizio_cache.Substring(3, 2);
                txt_doc_root.Text = info.doc_root_server;
                cb_cache.Checked = info.caching ;
                btn_salva.Text = "Modifica";
                btn_elimina.Visible = true;
                btn_annulla.Visible = false;
                hd_idAmm.Value = info.idAmministrazione;
                txt_ws_generale.Text = info.urlwscaching;
                txt_docLocale.Text = info.doc_root_server_locale;
                txt_wsLocale.Text = info.url_ws_caching_locale;
            }
            else
            {
                clearCampi();
                btn_salva.Text = "Salva";
            }
        }
        static public bool IsNumeric(string valore)
        {
            try
            {
                Int32.Parse(valore);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        //controlla il formato dell'orario
        private string controllaOrario(string orarioStringa)
        {
            int ora = int.Parse(orarioStringa.Substring(0, 2));
            int minuti = int.Parse(orarioStringa.Substring(3, 2));
            string orario = string.Empty;
            string mm = string.Empty;
            string hh = string.Empty;
            try
            {


                if (minuti > 0 && minuti < 10)
                    mm = "0" + minuti.ToString();
                else
                    if (minuti < 1 || minuti > 59)
                        mm = "00";
                    else
                        mm = minuti.ToString();

                if (ora > 0 && ora < 10)
                    hh = "0" + ora.ToString();
                else
                    if (ora < 1 || ora > 23)
                        hh = "00";
                    else
                        hh = ora.ToString();

                return hh + orarioStringa.Substring(2, 1) + mm;
            }
            catch (Exception e)
            {
                logger.Debug("errore nella codifica del tempo: " + e.StackTrace);
                return "00" + orarioStringa.Substring(2, 1) + "00";
            }


        }
        #endregion

        protected void ck_dim_cache_infinito_PreRender(object sender, EventArgs e)
        {
            if (ck_dim_cache_infinito.Checked)
            {
                txt_dim_cache.Text = " ";
                txt_dim_cache.Enabled = false;
            }
            else
            {
                txt_dim_cache.Enabled = true;
                if (txt_dim_cache.Text == " ")
                    txt_dim_cache.Text = string.Empty;
            }

        }

        protected void ck_dim_file_infinito_PreRender(object sender, EventArgs e)
        {
            if (ck_dim_file_infinito.Checked)
            {
                txt_dim_file.Text = " ";
                txt_dim_file.Enabled = false;
            }
            else
            {
                txt_dim_file.Enabled = true;
                if (txt_dim_file.Text == " ")
                    txt_dim_file.Text = string.Empty;
            }

        }
    }
}
