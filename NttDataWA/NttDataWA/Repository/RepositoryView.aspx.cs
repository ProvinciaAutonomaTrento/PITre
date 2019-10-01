using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;
using NttDataWA.UIManager;


namespace NttDataWA.Repository
{
    public partial class RepositoryView : System.Web.UI.Page
    {
        private DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
        private string language;
        private bool deleteFile = false;

        public List<FileInUpload> ListaFileDisponibili
        {
            get
            {
                List<FileInUpload> result = new List<FileInUpload>();
                if (HttpContext.Current.Session["listFilesUploaded"] != null)
                {
                    result = HttpContext.Current.Session["listFilesUploaded"] as List<FileInUpload>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listFilesUploaded"] = value;
            }
        }

        public List<FileInUpload> ListaFileFiltrata
        {
            get
            {
                List<FileInUpload> result;
                
                if (HttpContext.Current.Session["listaFileFiltrata"] != null)
                {
                    result = HttpContext.Current.Session["listaFileFiltrata"] as List<FileInUpload>;
                }
                else
                {
                    result = new List<FileInUpload>();
                    HttpContext.Current.Session["listaFileFiltrata"] = result;
                }

                return result;
            }
            set
            {
                HttpContext.Current.Session["listaFileFiltrata"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["selectedPageRepository"] != null) Int32.TryParse(HttpContext.Current.Session["selectedPageRepository"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["selectedPageRepository"] = value;
            }
        }

        private string SelectedRow
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["selectedRowElement"] != null)
                {
                    result = HttpContext.Current.Session["selectedRowElement"] as String;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedRowElement"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ListaFileDisponibili == null || ListaFileDisponibili.Count == 0)
            {
                //ListaFileDisponibili = ws.GetUploadedFiles(UserManager.GetInfoUser(), false).OfType<FileInUpload>().ToList();
            }

            if (!this.Page.IsPostBack)
            {
                this.InitLanguage();
                GridViewResult_Bind();
                //GridViewInProgressResult_Bind();
                HttpContext.Current.Session.Remove("selectedPageRepository");
            }
        }

        private void InitLanguage()
        {
            language = UIManager.UserManager.GetUserLanguage();

            this.lblSearch.Text = Utils.Languages.GetLabelFromCode("PersonalFileView_SearchTitle", language);
            this.optSearch_All.Text = Utils.Languages.GetLabelFromCode("PersonalFileView_OptAll", language);
            this.optSearch_InProgress.Text = Utils.Languages.GetLabelFromCode("PersonalFileView_OptIncomplete", language);
            this.optSearch_Complete.Text = Utils.Languages.GetLabelFromCode("PersonalFileView_OptComplete", language);
            this.lblSearch_FileName.Text = Utils.Languages.GetLabelFromCode("PersonalFileView_FileName", language);
            this.lblSearch_Description.Text = Utils.Languages.GetLabelFromCode("PersonalFileView_FileDescription", language);

            this.grdfileList.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("PersonalFileView_FileHash", language);
            this.grdfileList.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("PersonalFileView_FileName", language);
            this.grdfileList.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("PersonalFileView_FileDescription", language);
            this.grdfileList.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("PersonalFileView_FileSize", language);
            this.grdfileList.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("PersonalFileView_FileType", language);
            this.grdfileList.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("PersonalFileView_Percentage", language);
        }

        private void GridViewResult_Bind(bool filtered=false)
        {
            try
            {
                if (filtered)
                {
                    this.grdfileList.DataSource = ListaFileFiltrata;
                }
                else
                {
                    ListaFileDisponibili = ws.GetUploadedFiles(UserManager.GetInfoUser()).OfType<FileInUpload>().ToList();

                    this.grdfileList.DataSource = ListaFileDisponibili;
                }
                this.grdfileList.DataBind();
                this.UpGrid.Update();
            }
            catch (Exception e)
            {
                //logger.Error("Errore in fase di creazione della griglia: " + e.Message);
            }
        }

        protected void gridViewResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FileInUpload elemento = e.Row.DataItem as FileInUpload;
                if (!string.IsNullOrEmpty(elemento.StrIdentity))
                {
                    bool isIncomplete = (elemento.TotalChunkNumber != elemento.ChunkNumber);
                    e.Row.CssClass += " NormalRow";
                    string descriptionFile = "";

                    if (!string.IsNullOrEmpty(elemento.FileDescription))
                    {
                        descriptionFile = " [" + elemento.FileDescription + "]";
                    }

                    if (isIncomplete)
                    {
                        int percentage = (elemento.TotalChunkNumber>0?(elemento.ChunkNumber * 100) / elemento.TotalChunkNumber:0);

                        string[] splitName = elemento.FileSenderPath.Split('\\');
                        string fileNameUncompressed = splitName[splitName.Length - 1];

                        string reformatFileName = FileNameReconfigurated(fileNameUncompressed);

                        string pathImgFileType = FileManager.getFileIconSmall(this, FileManager.getEstensioneIntoSignedFile(reformatFileName));
                        (e.Row.FindControl("lblPercentage") as Label).Text = percentage.ToString() + " %";
                        (e.Row.FindControl("imgPercentage") as Image).ImageUrl = ProgressImgPath(percentage);
                        (e.Row.FindControl("lblNomeFile") as LinkButton).Text = reformatFileName;// +descriptionFile;
                        (e.Row.FindControl("lblNomeFile") as LinkButton).CommandName = "UpdateGrid";
                        (e.Row.FindControl("lblNomeFile") as LinkButton).ToolTip = Utils.Languages.GetLabelFromCode("RepositoryUpdate", language);
                        if (descriptionFile.Length < 70)
                            (e.Row.FindControl("lblDescrizione") as Label).Text = descriptionFile;
                        else
                        {
                            (e.Row.FindControl("lblDescrizione") as Label).Text = descriptionFile.Substring(0, 67) + "...";
                            (e.Row.FindControl("lblDescrizione") as Label).ToolTip = descriptionFile;
                        }
                        
                        //if (descriptionFile.Length < 30)
                        //{
                        //    (e.Row.FindControl("lkbDescrizione") as LinkButton).Visible = false;
                        //    (e.Row.FindControl("lblDescrizione") as Label).Text = descriptionFile;
                        //}
                        //else
                        //{
                        //    (e.Row.FindControl("lblDescrizione") as Label).Visible = false;
                        //    (e.Row.FindControl("lkbDescrizione") as LinkButton).Visible = true;
                        //    (e.Row.FindControl("lkbDescrizione") as LinkButton).CommandName = "Description";
                        //    (e.Row.FindControl("lkbDescrizione") as LinkButton).Text = descriptionFile.Substring(0, 17) + "...";                          
                        //}
                        (e.Row.FindControl("imgFile") as Image).ImageUrl = pathImgFileType;
                        ((CustomImageButton)e.Row.FindControl("imgDeleteFile")).Visible = false;
                        //(e.Row.FindControl("imgDeleteFile") as Image).CommandName = false;
                    }
                    else
                    {
                        (e.Row.FindControl("lblNomeFile") as LinkButton).Text = FileNameReconfigurated(elemento.FileName);// +descriptionFile; //Text='<%# Bind("FileName") %>'
                        (e.Row.FindControl("lblNomeFile") as LinkButton).CommandName = "AddFile";
                        (e.Row.FindControl("lblNomeFile") as LinkButton).ToolTip = Utils.Languages.GetLabelFromCode("RepositoryAdd", language);
                        //(e.Row.FindControl("lblDescrizione") as Label).Text = descriptionFile;
                        (e.Row.FindControl("lkbDescrizione") as LinkButton).Visible = false;
                        if (descriptionFile.Length < 70)
                            (e.Row.FindControl("lblDescrizione") as Label).Text = descriptionFile;
                        else
                        {
                            (e.Row.FindControl("lblDescrizione") as Label).Text = descriptionFile.Substring(0, 67) + "...";
                            (e.Row.FindControl("lblDescrizione") as Label).ToolTip = descriptionFile;
                        }
                        
                        //if (descriptionFile.Length < 30)
                        //{
                        //    (e.Row.FindControl("lkbDescrizione") as LinkButton).Visible = false;
                        //    (e.Row.FindControl("lblDescrizione") as Label).Text = descriptionFile;
                        //}
                        //else
                        //{
                        //    (e.Row.FindControl("lblDescrizione") as Label).Visible = false;
                        //    (e.Row.FindControl("lkbDescrizione") as LinkButton).Visible = true;
                        //    (e.Row.FindControl("lkbDescrizione") as LinkButton).CommandName = "Description";
                        //    (e.Row.FindControl("lkbDescrizione") as LinkButton).Text = descriptionFile.Substring(0, 17) + "...";
                        //}
                        string pathImgFileType = FileManager.getFileIconSmall(this, FileManager.getEstensioneIntoSignedFile(elemento.FileName));
                        (e.Row.FindControl("imgFile") as Image).ImageUrl = pathImgFileType;
                        (e.Row.FindControl("lblPercentage") as Label).Text = Utils.Languages.GetLabelFromCode("PersonalFileView_Complete", language);
                        (e.Row.FindControl("imgPercentage") as Image).ImageUrl = ProgressImgPath(-1);
                        ((CustomImageButton)e.Row.FindControl("imgDeleteFile")).ImageUrl = "../Images/Icons/delete2.png";
                        ((CustomImageButton)e.Row.FindControl("imgDeleteFile")).OnMouseOverImage = "../Images/Icons/delete2.png";
                        ((CustomImageButton)e.Row.FindControl("imgDeleteFile")).OnMouseOutImage = "../Images/Icons/delete2.png";
                        ((CustomImageButton)e.Row.FindControl("imgDeleteFile")).ToolTip = Utils.Languages.GetLabelFromCode("RepositoryDelete", language);
                        //for (int i = 0; i < (e.Row.Cells.Count - 1); i++)
                        //{
                        //    if (i == 0)
                            //{
                        //        e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(this.grdfileList, "Select$" + e.Row.RowIndex);
                        //        e.Row.Attributes["style"] = "cursor:pointer";
                            //}

                            //e.Row.Cells[i].Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');__doPostBack('UpGrid');disallowOp('ContentPlaceHolderContent');return false;";
                        //}
                    }
                }
            }
        }

        private bool ApplyFilters()
        {
            List<FileInUpload> tmpListElemetsFilter = new List<FileInUpload>();
            ListaFileFiltrata.Clear();

            string srcfileName = this.txtSearch_FileName.Text.ToUpper().Trim();
            string srcfileDesc = this.txtSearch_Description.Text.ToUpper().Trim();
            bool serchModulePresent = (optSearch_Complete.Selected || optSearch_InProgress.Selected || !string.IsNullOrEmpty(srcfileName) || !string.IsNullOrEmpty(srcfileDesc));

            if (serchModulePresent)
            {
                if (!string.IsNullOrEmpty(srcfileName))
                {
                    tmpListElemetsFilter = ((from e in this.ListaFileDisponibili where e.FileName.ToUpper().Trim().Contains(srcfileName) select e).ToList());
                    this.ListaFileFiltrata.Clear();
                    this.ListaFileFiltrata.AddRange(tmpListElemetsFilter);
                }

                if (!string.IsNullOrEmpty(srcfileDesc))
                {
                    if (ListaFileFiltrata.Count > 0)
                        tmpListElemetsFilter = ((from e in this.ListaFileFiltrata where e.FileDescription.ToUpper().Trim().Contains(srcfileDesc) select e).ToList());
                    else
                        tmpListElemetsFilter = ((from e in this.ListaFileDisponibili where e.FileDescription.ToUpper().Trim().Contains(srcfileDesc) select e).ToList());
                    
                    this.ListaFileFiltrata.Clear();
                    this.ListaFileFiltrata.AddRange(tmpListElemetsFilter);
                }

                if (optSearch_Complete.Selected)
                {
                    if (ListaFileFiltrata.Count > 0)
                        tmpListElemetsFilter = ((from e in this.ListaFileFiltrata where e.TotalChunkNumber == e.ChunkNumber select e).ToList());
                    else
                        tmpListElemetsFilter = ((from e in this.ListaFileDisponibili where e.TotalChunkNumber == e.ChunkNumber select e).ToList());

                    this.ListaFileFiltrata.Clear();
                    this.ListaFileFiltrata.AddRange(tmpListElemetsFilter);
                }

                if (optSearch_InProgress.Selected)
                {
                    if (ListaFileFiltrata.Count > 0)
                        tmpListElemetsFilter = ((from e in this.ListaFileFiltrata where e.TotalChunkNumber != e.ChunkNumber select e).ToList());
                    else
                        tmpListElemetsFilter = ((from e in this.ListaFileDisponibili where e.TotalChunkNumber != e.ChunkNumber select e).ToList());

                    this.ListaFileFiltrata.Clear();
                    this.ListaFileFiltrata.AddRange(tmpListElemetsFilter);
                }
            }
            else
                ListaFileFiltrata.AddRange(ListaFileDisponibili);

            return (ListaFileFiltrata.Count > 0);
        }

        private void ViewFilterResult()
        {
            GridViewResult_Bind(true);
        }

        private string FileNameReconfigurated(string filename)
        {
            if (filename.Contains('/'))
            {
                string[] splitFileName = filename.Split('/');
                filename = splitFileName[splitFileName.Length - 1];
            }

            string retVal = string.Empty;

            //if (filename.Length > 36)
            //{
            //    int lung = 35;

            //    int charPrecedenti = 0;
            //    while (charPrecedenti < filename.Length)
            //    {

            //        if ((charPrecedenti + lung) > filename.Length)
            //            lung = filename.Length - charPrecedenti;

            //        retVal = retVal + filename.Substring(charPrecedenti, lung) + " ";
            //        charPrecedenti = charPrecedenti + 35;
            //    }
            //}
            //else
                retVal = filename;

            return retVal;

        }

        protected void gridViewResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.grdfileList.PageIndex = e.NewPageIndex;
                this.SelectedRow = "-1";
                //BindGrid();
                this.UpGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gridViewResult_PreRender(object sender, EventArgs e)
        {

        }

        protected void gridViewResult_ItemCreated(Object sender, GridViewRowEventArgs e)
        {

        }
        
        protected void gridViewResult_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            string msg = string.Empty;
            string msgError = string.Empty;
            FileInUpload fileSelected = null;
            NttDataWA.DocsPaWR.FileDocumento fileDoc = null;
            //int indexSelected = grdfileList.SelectedRow.RowIndex;
            string rowIndex = string.Empty;

            switch (e.CommandName)
            {
                //Redirect al grigio contenente l'allegato selezionato
                case "DeleteFile":
                    rowIndex = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("systemIdElemento") as Label).Text;                    
                    fileSelected = (from v in ListaFileDisponibili where v.StrIdentity.Equals(rowIndex) select v).FirstOrDefault();
                    fileDoc = NewFileDoc(fileSelected);
                    //fileSelected = (FileInUpload)GetSessionValue("personalFileSelected");

                    msgError = FileManager.DeletePersonalFile(this, fileDoc, fileSelected.FileName, fileSelected.FileDescription);

                    if (string.IsNullOrEmpty(msgError))
                    {
                        GridViewResult_Bind();
                    }
                    else
                    {
                        msg = "ErrorFileUpload_custom";
                        msgError = msgError.Equals("ErrorAcquiredDocument") ? Utils.Languages.GetMessageFromCode(msgError, UserManager.GetUserLanguage()) : msgError;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + Utils.utils.FormatJs(msg) + "', 'error', '', '" + Utils.utils.FormatJs(msgError) + "');} else {parent.ajaxDialogModal('" + Utils.utils.FormatJs(msg) + "', 'error', '', '" + Utils.utils.FormatJs(msgError) + "');}; reallowOp();", true);
                    }

                    return;
                case "AddFile":
                    rowIndex = (((e.CommandSource as LinkButton).Parent.Parent as GridViewRow).FindControl("systemIdElemento") as Label).Text;
                    fileSelected = (from v in ListaFileDisponibili where v.StrIdentity.Equals(rowIndex) select v).FirstOrDefault();
                    fileDoc = NewFileDoc(fileSelected);

                    //fileDoc = (NttDataWA.DocsPaWR.FileDocumento)GetSessionValue("fileDoc");
                    //fileSelected = (FileInUpload)GetSessionValue("personalFileSelected");

                    msgError = FileManager.uploadPersonalFile(this, fileDoc, fileSelected.FileName, fileSelected.FileDescription);

                    if (string.IsNullOrEmpty(msgError))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                        ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('RepositoryView','selected');", true);
                    }
                    else
                    {
                        msg = "ErrorFileUpload_custom";
                        msgError = msgError.Equals("ErrorAcquiredDocument") ? Utils.Languages.GetMessageFromCode(msgError, UserManager.GetUserLanguage()) : msgError;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + Utils.utils.FormatJs(msg) + "', 'error', '', '" + Utils.utils.FormatJs(msgError) + "');} else {parent.ajaxDialogModal('" + Utils.utils.FormatJs(msg) + "', 'error', '', '" + Utils.utils.FormatJs(msgError) + "');}; reallowOp();", true);
                    }
                    return;
                case "Description":
                    rowIndex = (((e.CommandSource as LinkButton).Parent.Parent as GridViewRow).FindControl("systemIdElemento") as Label).Text;
                    fileSelected = (from v in ListaFileDisponibili where v.StrIdentity.Equals(rowIndex) select v).FirstOrDefault();
                    fileDoc = NewFileDoc(fileSelected);

                    msg = "PersonalFileDetails";
                    msgError = fileSelected.FileDescription;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + Utils.utils.FormatJs(msg) + "', 'check', '', '" + Utils.utils.FormatJs(msgError) + "');} else {parent.ajaxDialogModal('" + Utils.utils.FormatJs(msg) + "', 'check', '', '" + Utils.utils.FormatJs(msgError) + "');}; reallowOp();", true);
                    return;
                case "UpdateGrid":
                    GridViewResult_Bind();
                    return;
                default:
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                    break;
            }
        }

        private NttDataWA.DocsPaWR.FileDocumento NewFileDoc(FileInUpload fileSelected)
        {
            NttDataWA.DocsPaWR.FileDocumento fileDoc = new NttDataWA.DocsPaWR.FileDocumento();

            fileDoc.name = fileSelected.FileName;
            fileDoc.fullName = System.IO.Path.Combine(fileSelected.RepositoryPath, fileSelected.FileName);
            fileDoc.contentType = NttDataWA.UIManager.FileManager.GetMimeType(fileSelected.FileName);
            fileDoc.length = int.Parse(fileSelected.FileSize.ToString());//this.fileUpload.PostedFile.ContentLength;// ContentLength;// .FileSize;
            fileDoc.nomeOriginale = fileSelected.StrIdentity;


            SetSessionValue("fileDoc", fileDoc);
            SetSessionValue("personalFileSelected", fileSelected);

            return fileDoc;
        }

        protected void gridViewResult_SelectedIndexChanged(Object sender, EventArgs e)
        {
            //GridViewRow row = grdfileList.SelectedRow;
            int indexSelected = grdfileList.SelectedRow.RowIndex;
            FileInUpload fileSelected = ListaFileDisponibili[indexSelected];
            
            //InfoUtente infoUtente = UserManager.GetInfoUser();

            NttDataWA.DocsPaWR.FileDocumento fileDoc = new NttDataWA.DocsPaWR.FileDocumento();

            fileDoc.name = fileSelected.FileName;
            fileDoc.fullName = System.IO.Path.Combine(fileSelected.RepositoryPath, fileSelected.FileName);
            fileDoc.contentType = NttDataWA.UIManager.FileManager.GetMimeType(fileSelected.FileName);
            fileDoc.length = int.Parse(fileSelected.FileSize.ToString());//this.fileUpload.PostedFile.ContentLength;// ContentLength;// .FileSize;
            fileDoc.nomeOriginale = fileSelected.StrIdentity;

            
            SetSessionValue("fileDoc",fileDoc);
            SetSessionValue("personalFileSelected", fileSelected);
        }

        protected void ImgSearch_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                if (ApplyFilters())
                {
                    ViewFilterResult();
                }
                else
                {
                    string message = "CICCIOLO";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningPersonalFileView_NoFile', 'warning', '', '" + message + "');} else {parent.ajaxDialogModal('WarningPersonalFileView_NoFile', 'warning', '', '" + message + "');}", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RepositoryRefresh_Click(object sender, EventArgs e)
        {
            GridViewResult_Bind();
        }

        protected void RepositoryClose_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('RepositoryView','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private string ProgressImgPath (int numPercentage)
        {
            string result = string.Empty;

            if (numPercentage == -1) result = "identity_valid.png";
            if (numPercentage >= 0 && numPercentage < 10) result = "00_perc.jpg";
            if (numPercentage >= 10 && numPercentage < 20) result = "10_perc.jpg";
            if (numPercentage >= 20 && numPercentage < 30) result = "20_perc.jpg";
            if (numPercentage >= 30 && numPercentage < 40) result = "30_perc.jpg";
            if (numPercentage >= 40 && numPercentage < 50) result = "40_perc.jpg";
            if (numPercentage >= 50 && numPercentage < 60) result = "50_perc.jpg";
            if (numPercentage >= 60 && numPercentage < 70) result = "60_perc.jpg";
            if (numPercentage >= 70 && numPercentage < 80) result = "70_perc.jpg";
            if (numPercentage >= 80 && numPercentage < 90) result = "80_perc.jpg";
            if (numPercentage >= 90 && numPercentage < 100) result = "90_perc.jpg";
            if (numPercentage >= 100) result = "100_perc.jpg";

            return (numPercentage == -1 ? this.ResolveClientUrl("~/Images/Icons/") : this.ResolveClientUrl("~/Images/progressBar/")) + result;
        }

        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static object GetSessionValue(string sessionKey)
        {
            try
            {
                return System.Web.HttpContext.Current.Session[sessionKey];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            try
            {
                System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove(sessionKey);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
    }
}