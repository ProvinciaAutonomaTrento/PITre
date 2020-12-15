using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SAAdminTool.documento
{
    public partial class Oggetto : System.Web.UI.UserControl
    {
        protected bool OggPostBack;
        protected bool CodOggPostBack;
        protected bool UsaCodiceOggetto;
        protected string blankSpace;
        protected string wnd;
        protected int caratteriDisponibili = 2000;
        protected void Page_Init(object sender, EventArgs e)
        {
            if (blankSpace == null)
            {
                blankSpace = "";
            }
            //setto la variabile globale dello user control relativa all'uso del codice oggetto
            UsaCodiceOggetto = UseCodiceOggetto;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txt_cod_oggetto.AutoPostBack = CodOggPostBack;
            txt_oggetto.AutoPostBack = OggPostBack;

            SAAdminTool.DocsPaWR.InfoUtente info = new SAAdminTool.DocsPaWR.InfoUtente();
            info = UserManager.getInfoUtente(this.Page);


            string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_OGGETTO");
            if (!string.IsNullOrEmpty(valoreChiave))
                caratteriDisponibili = int.Parse(valoreChiave);

            txt_oggetto.MaxLength = caratteriDisponibili;
            clTesto.Value = caratteriDisponibili.ToString();
            txt_oggetto.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Oggetto'," + clTesto.ClientID + ")");
            txt_oggetto.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Oggetto'," + clTesto.ClientID + ")");

        }

        /// <summary>
        /// 
        /// </summary>
        public System.Drawing.Color BackColor
        {
            set
            {
                this.txt_cod_oggetto.BackColor = value;
                this.txt_oggetto.BackColor = value;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.clTesto.Value = (caratteriDisponibili - txt_oggetto.Text.Length).ToString();
        }

        /// <summary>
        /// Configura la proprietà visible della textBox oggetto
        /// </summary>
        public bool oggetto_isVisible
        {
            set
            {
                txt_oggetto.Visible = value;
            }
        }

        /// <summary>
        /// Configura la proprietà ReadOnly della textBox oggetto
        /// </summary>
        public bool oggetto_isReadOnly
        {
            set
            {
                txt_oggetto.ReadOnly = value;
            }
        }

        /// <summary>
        /// Configura la proprietà isEnable della textBox oggetto
        /// </summary>
        public bool oggetto_isEnable
        {
            set
            {
                txt_oggetto.Enabled = value;
            }
        }

        /// <summary>
        /// Imposta oppure legge il testo della textBox oggetto
        /// </summary>
        public string oggetto_text
        {
            get
            {
                if (txt_oggetto.Text.Trim() != String.Empty)
                {
                    return txt_oggetto.Text;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                txt_oggetto.Text = value;
            }
        }

        /// <summary>
        /// Gestisce il focus sulla textBox oggetto
        /// </summary>
        public void oggetto_SetControlFocus()
        {
            string s = "<SCRIPT language='javascript'>if(document.getElementById('" + txt_oggetto.ClientID + "')!=null){document.getElementById('" + txt_oggetto.ClientID + "').focus()} </SCRIPT>";
            Page.RegisterStartupScript("focus", s);
        }

        /// <summary>
        /// Abilita o disabilita l'auto postback sulla textBox oggetto
        /// </summary>
        public bool oggetto_postback
        {
            set
            {
                OggPostBack = value;
            }
        }

        /// <summary>
        /// Imposto il toolTip per la textBox oggetto
        /// </summary>
        public string OggToolTip
        {
            set
            {
                txt_oggetto.ToolTip = value;
            }
        }

        /// <summary>
        /// Aggiunge un attributo alla textBox dell'oggetto
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Ogg_Add_Attribute(string key, string value)
        {
            txt_oggetto.Attributes.Add(key, value);
        }

        /// <summary>
        /// Restituisce il ClientID della textBox Oggetto
        /// </summary>
        public string Ogg_ClientID
        {
            get
            {
                return txt_oggetto.ClientID;
            }
        }

        /// <summary>
        /// Funzione che attiva l'activeX SpellCheck sul campo oggetto
        /// </summary>
        public void SpellCheck()
        {
            if (!txt_oggetto.ReadOnly && txt_oggetto.Enabled)
            {
                string testo = this.txt_oggetto.Text.Trim();
                if (testo != String.Empty)
                {
                    RegisterClientScript("Spell", "SpellCheck('" + testo + "');");
                }
                else
                {
                    RegisterClientScript("Spell_error", "alert('inserire il testo nel campo oggetto!');");
                }
            }
        }

        /// <summary>
        /// Incorporo la lagica della chiamata a DefaultButton dentro il controllo per poter passare i campi
        /// contenuti nel controllo stesso.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="btn"></param>
        public void DefButton_Ogg(ref DocsPaWebCtrlLibrary.ImageButton btn)
        {
            Utils.DefaultButton(this.Page, ref txt_oggetto, ref btn);
        }

        /// <summary>
        /// Incorporo la lagica della chiamata a DefaultButton dentro il controllo per poter passare i campi
        /// contenuti nel controllo stesso.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="btn"></param>
        public void DefButton_Ogg(ref ImageButton btn)
        {
            Utils.DefaultButton(this.Page, ref txt_oggetto, ref btn);
        }

        /// <summary>
        /// Configura la proprietà visible della textBox codice oggetto
        /// </summary>
        public bool cod_oggetto_isVisible
        {
            set
            {
                txt_cod_oggetto.Visible = value;
            }

            get
            {
                return txt_cod_oggetto.Visible;
            }
        }

        /// <summary>
        /// Configura la proprietà ReadOnly della textBox codice oggetto
        /// </summary>
        public bool cod_oggetto_isReadOnly
        {
            set
            {
                txt_cod_oggetto.ReadOnly = value;
            }
        }

        /// <summary>
        /// Configura la proprietà isEnable della textBox codice oggetto
        /// </summary>
        public bool cod_oggetto_isEnable
        {
            set
            {
                txt_cod_oggetto.Enabled = value;
            }
        }

        /// <summary>
        /// Imposta oppure legge il testo della textBox codice oggetto
        /// </summary>
        public string cod_oggetto_text
        {
            get
            {
                if (txt_cod_oggetto.Text.Trim() != String.Empty)
                {
                    return txt_cod_oggetto.Text;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                txt_cod_oggetto.Text = value;
            }
        }

        /// <summary>
        /// Abilita o disabilita l'auto postback sulla textBox codice oggetto
        /// </summary>
        public bool cod_oggetto_postback
        {
            set
            {
                CodOggPostBack = value;
            }
        }

        /// <summary>
        /// Imposto il toolTip per la textBox codice oggetto
        /// </summary>
        public string CodOggToolTip
        {
            set
            {
                txt_cod_oggetto.ToolTip = value;
            }
        }


        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        //*********** GESTIONE DEGLI EVENTI ******************************

        //dichiaro l'evento delegato che verrà trattato dalla classe padre
        public event OggettoChangedDelegate OggettoChangedEvent = null;

        //l'evento catturato nel controllo sarà delegato per essere gestito dalla pagina contenente il controllo
        public delegate void OggettoChangedDelegate(object sender, OggTextEvent e);

        /// <summary>
        /// Estensione dell'event args per ottenere anche il contenuto della textBox Oggetto
        /// </summary>
        public class OggTextEvent : EventArgs
        {
            private string _testo = string.Empty;

            public OggTextEvent(string testo)
            {

                this._testo = testo;
            }

            public string Testo
            {
                get
                {
                    return this._testo;
                }
            }
        }

        //Devo comunque catturare l'evento del text changed ed all'interno attivo il delegate
        protected void txt_oggetto_TextChanged(object sender, EventArgs e)
        {
            if (this.OggettoChangedEvent != null)
                this.OggettoChangedEvent(this, new OggTextEvent(this.txt_oggetto.Text));
        }

        //Valorizzo la descrizione oggetto in base al codice se esiste!
        protected void txt_cod_oggetto_TextChanged(object sender, EventArgs e)
        {

            SAAdminTool.DocsPaWR.Registro[] listaRF;
            if (CodOggPostBack)
            {
                //Recupero la scheda corrente
                DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione(this.Page);
                if (schedaDoc == null)
                {
                    //Valorizzazione della schedaDoc nel caso di protocollazione semplice
                    ProtocollazioneIngresso.Protocollo.ProtocolloMng protoMng = new ProtocollazioneIngresso.Protocollo.ProtocolloMng(this.Page);
                    schedaDoc = protoMng.GetDocumentoCorrente();
                }

                ////Valorizzo il registro corrente
                string[] listaReg = { "" };
                try
                {
                    //recupero la lista di registri per passarli allo user control oggetto ai fini della ricerca
                    if (wnd == "proto")
                    {
                        //registro in sessione
                        listaReg = UserManager.getListaIdRegistri(this.Page);
                    }
                    else
                    {
                        //se vengo dal protocollo Semplificato
                        if (wnd == "protoSempl")
                        {
                            ProtocollazioneIngresso.Registro.RegistroMng regMng = new ProtocollazioneIngresso.Registro.RegistroMng(this.Page);
                            listaReg = new String[1];
                            listaReg[0] = regMng.GetRegistroCorrente().systemId;
                        }
                        else
                        {
                            listaReg = null; // ricerche e profilo
                        }
                    }

                    ArrayList aL = new ArrayList();
                    if (listaReg != null)
                    {
                        for (int i = 0; i < listaReg.Length; i++)
                        {
                            aL.Add(listaReg[i]);
                            listaRF = UserManager.getListaRegistriWithRF(this.Page, "1", listaReg[i]);
                            for (int j = 0; j < listaRF.Length; j++)
                            {
                                aL.Add(listaRF[j].systemId);
                            }
                        }

                        listaReg = new string[aL.Count];
                        aL.CopyTo(listaReg);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Registro non settato!");
                }
                DocsPaWR.Oggetto[] listaObj;

                // E' inutile finire nel backend se la casella di testo è vuota (a parte il fatto che 
                // la funzione, in questo caso, restituisce tutto l'oggettario)
                if (!String.IsNullOrEmpty(this.txt_cod_oggetto.Text.Trim()))
                    //In questo momento tralascio la descrizione oggetto che metto come stringa vuota
                    listaObj = DocumentManager.getListaOggettiByCod(this.Page, listaReg, "", this.txt_cod_oggetto.Text);
                else
                    listaObj = new DocsPaWR.Oggetto[] { 
                        new DocsPaWR.Oggetto()
                        {
                            descrizione = String.Empty,
                            codOggetto = String.Empty
                        }};

                if (listaObj.Length > 0)
                {
                    this.txt_oggetto.Text = listaObj[0].descrizione;
                    //this.txt_cod_oggetto.Text = listaObj[0].codOggetto;
                    schedaDoc.oggetto.codOggetto = listaObj[0].codOggetto;
                    schedaDoc.oggetto.descrizione = listaObj[0].descrizione;
                    schedaDoc.oggetto.daAggiornare = true;
                    DocumentManager.setDocumentoInLavorazione(schedaDoc);
                }
                else
                {
                    RegisterClientScript("Codice_error", "alert('codice oggetto inesistente!');");
                    this.txt_oggetto.Text = "";
                    this.txt_cod_oggetto.Text = "";
                    //Azzero anche i dati nella scheda corrente in modo che non rimanga in memoria il vecchio
                    //campo e codice oggetto!!!
                    schedaDoc.oggetto.codOggetto = "";
                    schedaDoc.oggetto.descrizione = "";
                    DocumentManager.setDocumentoInLavorazione(schedaDoc);
                }
            }
        }

        /// <summary>
        /// Restituisce il valore di configurazione relativo all'abilitazione del codice oggetto
        /// </summary>
        public bool UseCodiceOggetto
        {
            get
            {
                bool result = false;

                if (ConfigurationManager.AppSettings["USE_CODICE_OGGETTO"] != null)
                {
                    bool.TryParse(ConfigurationManager.AppSettings["USE_CODICE_OGGETTO"], out result);
                }

                return result;
            }
        }

        /// <summary>
        /// Modifica l'aspetto del controllo a seconda della pagina chiamante passata come parametro
        /// </summary>
        /// <param name="layout">layout della pagina chiamante può essere default o estesa</param>
        public void DimensioneOggetto(string layout, string PageName)
        {
            wnd = PageName;
            txt_cod_oggetto.Visible = true;

            if (layout.ToLower() == "default")
            {
                blankSpace = "&nbsp;";
                if (UsaCodiceOggetto)
                {
                    txt_cod_oggetto.Style.Value = "width=75px";
                    txt_oggetto.Style.Value = "width=285px";
                }
                else
                {
                    txt_cod_oggetto.Style.Value = "width=0px";
                    txt_cod_oggetto.Visible = false;
                    txt_oggetto.Style.Value = "width=367px";
                }
            }

            if (layout.ToLower() == "estesa")
            {
                blankSpace = "";
                if (UsaCodiceOggetto)
                {
                    txt_cod_oggetto.Style.Value = "width=15%";
                    txt_oggetto.Style.Value = "width=82%";
                }
                else
                {
                    txt_cod_oggetto.Style.Value = "width=0px";
                    txt_cod_oggetto.Visible = false;
                    txt_oggetto.Style.Value = "width=97%";
                }
            }
        }



    }
}
