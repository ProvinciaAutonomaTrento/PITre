using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using System.Drawing;

namespace DocsPAWA.UserControls
{

    public partial class LinkDocFasc : System.Web.UI.UserControl
    {
        public delegate void HandleInternalLink(string idObj);
        private HandleInternalLink _handleInternalDoc;
        private HandleInternalLink _handleInternalFasc;

        private void DefaultInternalLink(string idObj){
            //non fa nulla
        }

        public HandleInternalLink HandleInternalDoc
        {
            get
            {
                if (this._handleInternalDoc == null)
                {
                    return DefaultInternalLink;
                }
                return this._handleInternalDoc;
            }
            set
            {
                this._handleInternalDoc = value;
            }
        }

        public HandleInternalLink HandleInternalFasc
        {
            get
            {
                if (this._handleInternalFasc == null)
                {
                    return DefaultInternalLink;
                }
                return this._handleInternalFasc;
            }
            set
            {
                this._handleInternalFasc = value;
            }
        }

        public bool IsInsertModify
        {
            get;set;
        }

        public bool HideLink
        {
            get;
            set;
        }

        public bool IsAnteprima
        {
            get;
            set;
        }

        public bool IsEsterno
        {
            get;
            set;
        }

        public bool IsFascicolo
        {
            get;
            set;
        }

        public string TextCssClass
        {
            get;set;
        }

        public string Value
        {
            get
            {
                if (IsEsterno)
                {
                    if (string.IsNullOrEmpty(txt_Maschera.Text) || string.IsNullOrEmpty(this.txt_Link.Text)) return "";
                    return this.txt_Maschera.Text + "||||" + this.txt_Link.Text;
                }
                else
                {
                    if (string.IsNullOrEmpty(txt_Maschera.Text) || string.IsNullOrEmpty(this.hf_Id.Value)) return "";
                    return this.txt_Maschera.Text + "||||" + this.hf_Id.Value;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string[] values = value.Split(new string[]{"||||"}, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length == 2)
                    {
                        this.txt_Maschera.Text = values[0];
                        if (IsEsterno)
                        {
                            this.txt_Link.Text = values[1];
                        }
                        else
                        {
                            string id = values[1];
                            if (IsFascicolo)
                            {
                                Fascicolo fasc = FascicoliManager.getFascicoloById(this.Page, id);
                                this.hf_Id.Value = id;
                                if (fasc != null)
                                {
                                    this.txt_NomeObj.Text = fasc.descrizione;
                                }
                                else
                                {
                                    this.txt_NomeObj.Text = "Fascicolo non visibile";
                                }
                            }
                            else
                            {
                                InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(id, null, this.Page);
                                this.hf_Id.Value = id;
                                if (infoDoc != null)
                                {
                                    this.txt_NomeObj.Text = infoDoc.oggetto;
                                }
                                else
                                {
                                    this.txt_NomeObj.Text = "Documento non visibile";
                                }
                            }
                        }
                    }
                }
            }
        }

        private string LinkText
        {
            get
            {
                if (string.IsNullOrEmpty(this.txt_Maschera.Text))
                {
                    return "Link non inserito";
                }
                else
                {
                    return this.txt_Maschera.Text;
                }
            }
        }

        protected string RicercaFascicoliLink
        {
            get
            {
                return ResolveUrl("~/popup/ricercaFascicoli.aspx?codClassifica=&caller=profilo&NodoMultiReg=N&ricerca=true");
            }
        }

        protected string RicercaDocumentiLink
        {
            get
            {
                return ResolveUrl("~/popup/ricercaDocumenti.aspx");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
                this.hpl_Link.Click += new EventHandler(hpl_Link_Click);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.IsPostBack && hf_Reset.Value.Equals("1"))
            {
                this.hf_Id.Value = "";
                this.txt_NomeObj.Text = "";
                hf_Reset.Value = "0";
            }
            if (this.IsPostBack && hf_SelectedObject.Value.Equals("1"))
            {
                if (IsFascicolo)
                {
                    Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoRicerca();
                    if (fasc != null)
                    {
                        this.hf_Id.Value = fasc.systemID;
                        this.txt_NomeObj.Text = fasc.descrizione;
                        this.txt_Maschera.Text = fasc.codice + " " + CutValue(fasc.descrizione);
                    }
                }
                else
                {
                    InfoDocumento infoDoc = DocumentManager.getInfoDocumentoRicerca(this.Page);
                    if (infoDoc != null)
                    {
                        this.hf_Id.Value = infoDoc.idProfile;
                        this.txt_NomeObj.Text = infoDoc.oggetto;
                        if (!string.IsNullOrEmpty(infoDoc.segnatura))
                        {
                            this.txt_Maschera.Text = infoDoc.segnatura + " " + CutValue(infoDoc.oggetto);
                        }
                        else
                        {
                            this.txt_Maschera.Text = infoDoc.idProfile + " " + CutValue(infoDoc.oggetto);
                        }
                    }
                }
                this.hf_SelectedObject.Value = "0";
            }
            this.txt_NomeObj.CssClass = TextCssClass;
            this.txt_Link.CssClass = this.TextCssClass;
            this.txt_Maschera.CssClass = TextCssClass;
            this.hpl_Link.Text = this.LinkText;
            this.btn_Reset.OnClientClick = "_" + ClientID + "_reset();return false;";
            if (IsFascicolo)
            {
                this.lbl_oggetto.Text = "Fascicolo: ";
                this.btn_Cerca.OnClientClick = "_" + ClientID + "_apriRicercaFascicoli()";
            }
            else
            {
                this.lbl_oggetto.Text = "Documento: ";
                this.btn_Cerca.OnClientClick = "_" + ClientID + "_apriRicercaDocumenti()";
            }
            if (IsEsterno)
            {
                this.hpl_Link.OnClientClick="_"+ClientID+"_apriLinkEsterno()";
            }
            this.pnlLink_Link.Visible = !HideLink;
            if (IsInsertModify)
            {
                this.pnlLink_InsertModify.Visible = true;
                if (IsAnteprima)
                {
                    this.btn_Cerca.Enabled = false;
                    this.btn_Reset.Enabled = false;
                    this.hpl_Link.Enabled = false;
                }
                if (IsEsterno)
                {
                    this.tr_interno.Visible = false;
                    this.tr_esterno.Visible = true;
                }
                else
                {
                    this.tr_interno.Visible = true;
                    this.tr_esterno.Visible = false;
                }
            }
            else
            {
                this.pnlLink_InsertModify.Visible = false;
            }
        }

        protected void hpl_Link_Click(Object sender, EventArgs e)
        {
                string idDocFasc = this.hf_Id.Value;
                if (!string.IsNullOrEmpty(idDocFasc))
                {
                    if (IsFascicolo)
                    {
                        HandleInternalFasc(idDocFasc);
                    }
                    else
                    {
                        HandleInternalDoc(idDocFasc);
                    }
                }
        }

        private string CutValue(string value)
        {
            if (value.Length < 20) return value;
            int firstSpacePos = value.IndexOf(' ', 20);
            if (firstSpacePos == -1) firstSpacePos = 20;
            return value.Substring(0, firstSpacePos) + "...";
        }

    }
}