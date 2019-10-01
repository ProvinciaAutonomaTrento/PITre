using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class AvviseAnswerProtocol : System.Web.UI.Page
    {

        private string AnswerProtocolSender
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["AnswerProtocolSender"] != null) toReturn = (string)HttpContext.Current.Session["AnswerProtocolSender"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["AnswerProtocolSender"] = value;
            }
        }

        private string AnswerProtocolSubject
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["AnswerProtocolSubject"] != null) toReturn = (string)HttpContext.Current.Session["AnswerProtocolSubject"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["AnswerProtocolSubject"] = value;
            }
        }

        private string AnswerProtocolIsProtocol
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["AnswerProtocolIsProtocol"] != null) toReturn = (string)HttpContext.Current.Session["AnswerProtocolIsProtocol"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["AnswerProtocolIsProtocol"] = value;
            }
        }

        private string AnswerProtocolIsOccasional
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["AnswerProtocolIsOccasional"] != null) toReturn = (string)HttpContext.Current.Session["AnswerProtocolIsOccasional"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["AnswerProtocolIsOccasional"] = value;
            }
        }

        private int ActivatedPanel
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["AnswerProtocolActivatedPanel"] != null) toReturn = (int)HttpContext.Current.Session["AnswerProtocolActivatedPanel"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["AnswerProtocolActivatedPanel"] = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitLanguage();

                    //gestione visualizzazione msg di errore
                    string req_corrisp = this.AnswerProtocolSender;
                    string req_oggetto = this.AnswerProtocolSubject;
                    string req_isProto = this.AnswerProtocolIsProtocol;
                    string req_diventaOccasionale = this.AnswerProtocolIsOccasional;

                    if (req_corrisp != null && req_corrisp.Equals("True"))
                    {
                        this.opt_mitt.Visible = false;
                    }
                    if (req_oggetto != null && req_oggetto.Equals("True"))
                    {
                        this.opt_ogg.Visible = false;
                    }
                    if (req_isProto != null && req_isProto.Equals("True"))
                    {
                        //se la risposta al protocollo è PROTOCOLLATA
                        this.ActivatedPanel = 2;
                        this.pnl_rispProto.Visible = true;
                        this.pnl_rispNoProto.Visible = false;
                        this.rbl_scelta2.Items[0].Selected = true;
                    }
                    else
                    {
                        //se la risposta al protocollo NON E' PROTOCOLLATA
                        this.ActivatedPanel = 1;
                        this.pnl_rispProto.Visible = false;
                        this.pnl_rispNoProto.Visible = true;
                        this.rbl_scelta.Items[0].Selected = true;
                    }

                    if (req_diventaOccasionale != null && req_diventaOccasionale.Equals("True"))
                    {
                        this.opt_occ.Visible = false;
                        this.opt_occ_proto.Visible = false;
                    }
                    else
                    {
                        this.ActivatedPanel = 3;
                        this.pnl_rispProto.Visible = false;
                        this.pnl_rispNoProto.Visible = false;
                        this.pnl_sovrascriviCorr.Visible = true;

                        if (req_isProto != null && req_isProto.Equals("True"))
                        {
                            this.opt_occ_proto.Visible = true;
                            this.opt_occ.Visible = false;
                        }
                        else
                        {
                            this.opt_occ_proto.Visible = false;
                            this.opt_occ.Visible = true;
                        }
                    }

                    if (this.opt_mitt.Visible) this.messager.Text += this.opt_mitt.InnerHtml + "<br />\n";
                    if (this.opt_ogg.Visible) this.messager.Text += this.opt_ogg.InnerHtml + "<br />\n";
                    if (this.opt_occ.Visible) this.messager.Text += this.opt_occ.InnerHtml + "<br />\n";
                    if (this.opt_occ_proto.Visible) this.messager.Text += this.opt_occ_proto.InnerHtml + "<br />\n";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.litTitle.Text = Utils.Languages.GetLabelFromCode("AnswerAvviseProtocolTitle", language);
            this.opt_mitt.InnerHtml = Utils.Languages.GetLabelFromCode("AnswerAvviseOptMitt", language);
            this.opt_ogg.InnerHtml = Utils.Languages.GetLabelFromCode("AnswerAvviseOptOgg", language);
            this.opt_occ.InnerHtml = Utils.Languages.GetLabelFromCode("AnswerAvviseOptOcc", language);
            this.opt_occ_proto.InnerHtml = Utils.Languages.GetLabelFromCode("AnswerAvviseOptOccProto", language);
            this.rbl_scelta.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerAvviseContinueAndOverwrite", language), "N"));
            this.rbl_scelta.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerAvviseContinueUsingData", language), "Y"));
            this.rbl_scelta.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerAvviseSelectAnotherDoc", language), "c"));
            this.rbl_scelta.Items[0].Selected = true;
            this.rbl_scelta2.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerAvviseContinue", language), "S"));
            this.rbl_scelta2.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerAvviseSelectAnotherDoc", language), "c"));
            this.rbl_scelta2.Items[0].Selected = true;
            this.rbl_scelta3.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerAvviseContinue", language), "Y"));
            this.rbl_scelta3.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerAvviseSelectAnotherDoc", language), "c"));
            this.rbl_scelta3.Items[0].Selected = true;
            this.BtnOk.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocBtnClose", language);
        }

        protected void BtnOk_Click(object sender, System.EventArgs e)
        {
            /*  ----------------------------------------------------
                  Possibili valori di ritorno dell'avviso modale
                ----------------------------------------------------
			  
                - Pannello pnl_rispNoProto -
                Questo pannello viene mostrato nella pagina di avviso quando si sta rispondendo a un
                protocollo in uscita con un protocollo in ingresso che ancora non è stato protocollato.
			  
                Y : viene ritornato quando l'utente clicca sull' optione 'Continua utilizzando i dati immessi';
                N : viene ritornato quando l'utente clicca sull' optione 'Continua e sovrascrivi i dati'
                C : viene ritornato quando l'utente clicca sull' optione 'Seleziona un altro documento' 
			  
                - Pannello pnl_rispProto -
                Questo pannello viene mostrato nella pagina di avviso quando si sta rispondendo a un
                protocollo in uscita con un protocollo in ingresso protocollato in precedenza.
			  
                V : viene ritornato quando l'utente clicca sull' optione 'continua'
                C : viene ritornato quando l'utente clicca sull' optione 'Seleziona un altro documento' 		
             */
            try {
                if (this.ActivatedPanel==1 && rbl_scelta.SelectedItem != null && rbl_scelta.SelectedItem.Value != String.Empty)
                {
                    switch (rbl_scelta.SelectedItem.Value.ToUpper())
                    {
                        case "Y":
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "returnY", "AnswerProtocol('Y');", true);
                            break;
                        case "N":
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "returnN", "AnswerProtocol('N');", true);
                            break;
                        case "C":
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "returnC", "AnswerProtocol('C');", true);
                            break;
                    }
                }

                if (this.ActivatedPanel == 2 && rbl_scelta2.SelectedItem != null && rbl_scelta2.SelectedItem.Value != String.Empty)
                {
                    switch (rbl_scelta2.SelectedItem.Value.ToUpper())
                    {
                        case "C":
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "returnC", "AnswerProtocol('C');", true);
                            break;
                        case "S":
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "returnS", "AnswerProtocol('S');", true);
                            break;
                    }
                }

                if (this.ActivatedPanel == 3 && rbl_scelta3.SelectedItem != null && rbl_scelta3.SelectedItem.Value != String.Empty)
                {
                    switch (rbl_scelta3.SelectedItem.Value.ToUpper())
                    {
                        case "Y":
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "returnY", "AnswerProtocol('Y');", true);
                            break;
                        case "C":
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "returnC", "AnswerProtocol('C');", true);
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnClose_Click(object sender, System.EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "returnC", "AnswerProtocol('C');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

    }
}