using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class SelectNextMessage : System.Web.UI.Page
    {
        private List<DocsPaWR.Messaggio> NextMessages
        {
            get
            {
                if ((HttpContext.Current.Session["NextMessages"]) != null)
                    return HttpContext.Current.Session["NextMessages"] as List<DocsPaWR.Messaggio>;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["NextMessages"] = value;
            }
        }

        private DocsPaWR.SpedizioneDocumento InfoSpedizioneSelectMessage
        {
            get
            {
                if ((HttpContext.Current.Session["InfoSpedizioneSelectMessage"]) != null)
                    return HttpContext.Current.Session["InfoSpedizioneSelectMessage"] as DocsPaWR.SpedizioneDocumento;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["InfoSpedizioneSelectMessage"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeLanguage();
                InitializePage();
            }
            else
            {
                RefreshScript();
            }
        }

        private void InitializePage()
        {
            this.BuildRblSelectMessage();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SelectNextMessageOk.Text = Utils.Languages.GetLabelFromCode("SelectNextMessageOk", language);
            this.SelectNextMessageClose.Text = Utils.Languages.GetLabelFromCode("SelectNextMessageClose", language);
            this.messager.Text = Utils.Languages.GetLabelFromCode("SelectNextMessageLblSelectMessage", language);
            //this.LblSelectMessage.Text = Utils.Languages.GetLabelFromCode("SelectNextMessageLblSelectMessage", language);
        }

        private void BuildRblSelectMessage()
        {
            if (this.NextMessages != null && this.NextMessages.Count > 0)
            {
                foreach (Messaggio m in this.NextMessages)
                {
                    this.RblSelectMessage.Items.Add(new ListItem() { Value = m.ID, Text = m.DESCRIZIONE });
                }

                this.RblSelectMessage.SelectedValue = this.InfoSpedizioneSelectMessage.tipoMessaggio.ID;
            }
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeIframe", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }


        protected void SelectNextMessageClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('SelectNextMessage', '');} else {parent.closeAjaxModal('SelectNextMessage', '', parent);};", true);
            }
            catch(Exception ex)
            {
            
            }
        }

        protected void SelectNextMessageOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                Messaggio messaggioSelezionato = (from m in this.NextMessages where m.ID.Equals(this.RblSelectMessage.SelectedValue) select m).FirstOrDefault();
                this.InfoSpedizioneSelectMessage.tipoMessaggio = messaggioSelezionato;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('SelectNextMessage', 'up');} else {parent.closeAjaxModal('SelectNextMessage', 'up', parent);};", true);
            }
            catch (Exception ex)
            {
            }
        }
    }
}