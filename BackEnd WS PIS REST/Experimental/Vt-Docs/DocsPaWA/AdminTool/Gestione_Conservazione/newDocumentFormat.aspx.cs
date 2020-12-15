using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class newDocumentFormat : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService _wsInstance = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            if (!IsPostBack)
            {
                FetchData();
            }
        }

        protected void FetchData()
        {
            this.TemporarySelectedFile = new Dictionary<String, String>();
            if (this.SelectedFile == null)
            {
                this.SelectedFile = new Dictionary<String, String>();
                if (this.Policy != null && this.Policy.FormatiDocumento != null && this.Policy.FormatiDocumento.Length > 0)
                {
                    for (int i = 0; i < this.Policy.FormatiDocumento.Length; i++)
                    {
                        this.SelectedFile.Add(this.Policy.FormatiDocumento.ElementAt(i).SystemId.ToString(), this.Policy.FormatiDocumento.ElementAt(i).FileExtension);
                    }
                }
            }

            SupportedFileType[] fileTypes = this.GetSupportedFileTypes();

            for (int i = 0; i < this.SelectedFile.Count; i++)
            {
                this.TemporarySelectedFile.Add(this.SelectedFile.ElementAt(i).Key.ToString(), this.SelectedFile.ElementAt(i).Value.ToString());
            }

            this.grvFileType.DataSource = fileTypes;
            this.grvFileType.CurrentPageIndex = 0;
            this.grvFileType.DataBind();

        }

        protected SupportedFileType[] GetSupportedFileTypes()
        {
            return this.WsInstance.GetSupportedFileTypesPreservation(this.IdAmministrazione);
        }

     /*   protected string[] LoadSelectedFileForPolicy()
        {
           // return this.WsInstance.GetSupportedFileTypesPreservation(this.IdAmministrazione);
        }
        */
        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }

        protected DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (this._wsInstance == null)
                    this._wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
                return this._wsInstance;
            }
        }

        protected String GetTypeID(SupportedFileType temp)
        {
            return (temp.SystemId).ToString();
        }

        protected String GetTypeName(SupportedFileType temp)
        {
            return temp.FileExtension;
        }

        protected String GetTypeDescription(SupportedFileType temp)
        {
            return temp.Description;
        }


        protected bool GetChecked(SupportedFileType temp)
        {
            if(this.SelectedFile!=null && this.SelectedFile.Count>=0 && this.SelectedFile.ContainsKey((temp.SystemId).ToString()))
            {
                return true;
            }
            return false;;
        }

        /// Al clic viene salvata la lista dei formato documenti ammessi
        /// </summary>
        protected void BtnSaveDocumentFormat_Click(object sender, EventArgs e)
        {
            this.SelectedFile = this.TemporarySelectedFile;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save();", true);
        }

        /// Al clic viene salvata la lista dei formato documenti ammessi
        /// </summary>
        protected void BtnCloseDocument_Click(object sender, EventArgs e)
        {
            this.TemporarySelectedFile = null;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "window.close();", true);
        }

        protected void ChangeCheckClick(object sender, EventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            TableCell cell = (TableCell)box.Parent;
            DataGridItem dgItem = (DataGridItem)cell.Parent;
            Label systemIdLabel = (Label)this.grvFileType.Items[dgItem.ItemIndex].FindControl("SYSTEM_ID");
            Label ValueLabel = (Label)this.grvFileType.Items[dgItem.ItemIndex].FindControl("FILENAME");
            if (box.Checked)
            {
                if (this.TemporarySelectedFile != null && !this.TemporarySelectedFile.ContainsKey(systemIdLabel.Text))
                {
                    TemporarySelectedFile.Add(systemIdLabel.Text, ValueLabel.Text);
                }
            }
            else
            {
                if (this.TemporarySelectedFile != null && this.TemporarySelectedFile.ContainsKey(systemIdLabel.Text))
                {
                    TemporarySelectedFile.Remove(systemIdLabel.Text);
                }
            }
        }

      


        /// <summary>
        /// Policy selezionata
        /// </summary>
        protected Policy Policy
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["policy"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["policy"] as Policy;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["policy"] = value;
            }
        }


        /// <summary>
        /// TemporarySelectedFile
        /// </summary>
        public Dictionary<String, String> TemporarySelectedFile
        {
            get
            {
                return HttpContext.Current.Session["TemporarySelectedFile"] as Dictionary<String, String>;
            }
            set
            {
                HttpContext.Current.Session["TemporarySelectedFile"] = value;
            }
        }

        /// <summary>
        /// TemporarySelectedFile
        /// </summary>
        public Dictionary<String, String> SelectedFile
        {
            get
            {
                return HttpContext.Current.Session["SelectedFile"] as Dictionary<String, String>;
            }
            set
            {
                HttpContext.Current.Session["SelectedFile"] = value;
            }
        }
       
    }
}