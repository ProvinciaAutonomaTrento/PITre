using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class AttachmentsUpload : System.Web.UI.Page
    {
        protected bool PdfConvertServerSide
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["PdfConvertServerSide"] != null)
                {
                    result = (bool)HttpContext.Current.Session["PdfConvertServerSide"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["PdfConvertServerSide"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Web.HttpContext.Current.Session["UploadMassivoConversionePDF"] = "0";
                System.Web.HttpContext.Current.Session["UploadMassivoCartaceo"] = "0";
                string language = UIManager.UserManager.GetUserLanguage();
                HttpContext.Current.Session["FileAcquisitionSizeMax"] = "";
                if (!this.PdfConvertServerSide)
                    this.optPDF.Enabled = false;
                this.optPDF.Text = Utils.Languages.GetLabelFromCode("FileUploadPDFOptionSynchronous", language);
                this.labelLegendaAcquisizioneMassivaAllegati.Text = Utils.Languages.GetLabelFromCode("labelLegendaAcquisizioneMassivaAllegati", language);
            }
        }

        protected void btnClosePopUp_Click(object sender, EventArgs e)
        {
            //UploadMassivoAllegati
            string result = (string)HttpContext.Current.Session["UploadMassivoAllegati"];
            ScriptManager.RegisterStartupScript(this, this.GetType(), "addAtt", "parent.closeAjaxModal('AttachmentsUpload', '" + result + "');", true);

        }
    }
}