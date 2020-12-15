using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class InformazioniFile : System.Web.UI.Page
    {
        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            InitializeLanguage();
            string docnumber = DocumentManager.getSelectedRecord().docNumber;
            List<InfoFile> infoFile = DocumentManager.GetListInfoFileDocument(docnumber);
            BindGridInfoFile(infoFile);
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.BtnInformationiFileClose.Text = Utils.Languages.GetLabelFromCode("BtnInformationFileClose", language);
        }

        #endregion

        private void BindGridInfoFile(List<InfoFile> infoFile)
        {
            if(infoFile != null  && infoFile.Count > 0)
            {
                this.GridInfoFile.DataSource = infoFile;
                this.GridInfoFile.DataBind();
            }
        }

        protected void GridInfoFile_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //CONFORME
                Image imgConforme = e.Row.FindControl("imgConforme") as Image;
                if ((e.Row.DataItem as InfoFile).Conforme)
                    imgConforme.ImageUrl = @"~/Images/Common/messager_check.png";
                else
                    imgConforme.ImageUrl = @"~/Images/Common/messager_error.png";

                /*
                //ESTENSIONE CONFORME
                Image imgEstConforme = e.Row.FindControl("imgEstConforme") as Image;
                if ((e.Row.DataItem as InfoFile).EstensioneConforme)
                    imgEstConforme.ImageUrl = @"~/Images/Common/messager_check.png";
                else
                    imgEstConforme.ImageUrl = @"~/Images/Common/messager_error.png";

                //MACRO
                Image imgMacro = e.Row.FindControl("imgMacro") as Image;
                if (!(e.Row.DataItem as InfoFile).ContieneMacro)
                    imgMacro.ImageUrl = @"~/Images/Common/messager_check.png";
                else
                    imgMacro.ImageUrl = @"~/Images/Common/messager_error.png";

                //FORMS
                Image imgForms = e.Row.FindControl("imgForms") as Image;
                if (!(e.Row.DataItem as InfoFile).ContieneForms)
                    imgForms.ImageUrl = @"~/Images/Common/messager_check.png";
                else
                    imgForms.ImageUrl = @"~/Images/Common/messager_error.png";

                //JAVASCRIPT
                Image imgJavascript = e.Row.FindControl("imgJavascript") as Image;
                if (!(e.Row.DataItem as InfoFile).ContieneJavascript)
                    imgJavascript.ImageUrl = @"~/Images/Common/messager_check.png";
                else
                    imgJavascript.ImageUrl = @"~/Images/Common/messager_error.png";
               */
            }
        }
        protected string GetDataAcquisizione(InfoFile info)
        {
            string result = string.Empty;
            if(!string.IsNullOrEmpty(info.DataAcquisizione))
                result = Utils.dateformat.ConvertToDate(info.DataAcquisizione).ToString("dd/MM/yyyy");
            return result;
        }

        protected string GetDescrizioneNonConforme(InfoFile info)
        {
            string retValue = string.Empty;
            string language = UserManager.GetUserLanguage();
            if (!string.IsNullOrEmpty(info.DescrizioneInfoFile))
            {
                string[] split = info.DescrizioneInfoFile.Split(',');
                foreach(string s in split)
                {
                    retValue += string.IsNullOrEmpty(retValue) ? "Presenza di: " + Utils.Languages.GetLabelFromCode("InformazioniFile" + s, language) : "<br />" + Utils.Languages.GetLabelFromCode("InformazioniFile" + s, language); 
                }
            }

            return retValue;
        }

        protected string GetCodice(InfoFile info)
        {
            string retValue = string.Empty;

            if (string.IsNullOrEmpty(info.IdDocumentoPrincipale))
                retValue = "D";
            else
            {
                Allegato[] allegati = DocumentManager.getSelectedRecord().allegati;
                if(allegati != null && allegati.Length > 0)
                {
                    retValue = (from a in allegati where a.docNumber.Equals(info.IdProfile) select a.versionLabel).FirstOrDefault();
                }
            }

            return retValue;
        }



        #region Event 

        protected void BtnInformationFileClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('InformazioniFile','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

    }
}