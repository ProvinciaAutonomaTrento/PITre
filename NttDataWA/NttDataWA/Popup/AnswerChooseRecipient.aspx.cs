using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{

    public partial class AnswerChooseRecipient : System.Web.UI.Page
    {

        #region Fields

        protected DataSet ds;
        protected SchedaDocumento schedadocIngressoNew;
        protected int itemIndex;

        #endregion

        #region Properties

        private Hashtable ht_destinatariTO_CC
        {
            get
            {
                Hashtable result = null;
                if (HttpContext.Current.Session["ht_destinatariTO_CC"] != null)
                {
                    result = (Hashtable)HttpContext.Current.Session["ht_destinatariTO_CC"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ht_destinatariTO_CC"] = value;
            }
        }

        private SchedaDocumento DocumentWIP
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["Answer.DocumentWIP"] != null)
                {
                    result = HttpContext.Current.Session["Answer.DocumentWIP"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Answer.DocumentWIP"] = value;
            }
        }

        private DataTable dt
        {
            get
            {
                DataTable result = null;
                if (HttpContext.Current.Session["dt"] != null)
                {
                    result = (DataTable)HttpContext.Current.Session["dt"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["dt"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitLanguage();

                    if (this.DocumentWIP==null) this.DocumentWIP = DocumentManager.getSelectedRecord();

                    if (this.DocumentWIP.protocollo.GetType() == typeof(DocsPaWR.ProtocolloUscita))
                    {
                        DocsPaWR.Corrispondente[] listaCorrTo = null;
                        DocsPaWR.Corrispondente[] listaCorrCC = null;

                        //prendo i destinatari in To
                        listaCorrTo = ((DocsPaWR.ProtocolloUscita)this.DocumentWIP.protocollo).destinatari;
                        //prendo i destinatari in CC
                        listaCorrCC = ((DocsPaWR.ProtocolloUscita)this.DocumentWIP.protocollo).destinatariConoscenza;

                        FillGrid(listaCorrTo, listaCorrCC);
                    }

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
            this.litTitle.Text = Utils.Languages.GetLabelFromCode("TitleAnswerChoosRecipient", language);
            this.BtnOk.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
        }

        /// <summary>
        /// Caricamento griglia destinatari del protocollo in uscita selezionato
        /// </summary>
        /// <param name="uoApp"></param>
        private void FillGrid(DocsPaWR.Corrispondente[] listaCorrTo, DocsPaWR.Corrispondente[] listaCorrCC)
        {
            ds = this.CreateGridDataSetDestinatari();
            this.CaricaGridDataSetDestinatari(ds, listaCorrTo, listaCorrCC);
            this.grdList.DataSource = ds;
            this.grdList.DataBind();

            // Impostazione corrispondente predefinito
            this.SelectDefaultCorrispondente();
        }

        /// <summary>
        /// In presenza di un solo corrispondente in griglia,
        /// lo seleziona per default
        /// </summary>
        private void SelectDefaultCorrispondente()
        {
            foreach (GridViewRow row in this.grdList.Rows)
            {
                if (!string.IsNullOrEmpty(row.Cells[4].Text) && row.Cells[4].Text.Equals("disable"))
                {
                    RadioButton optCorr = row.Cells[3].FindControl("optCorr") as RadioButton;
                    optCorr.Enabled = false;
                }
            }

            if (this.grdList.Rows.Count == 1)
            {
                GridViewRow dgItem = this.grdList.Rows[0];

                RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
                if (optCorr != null)
                {
                    if (!string.IsNullOrEmpty(dgItem.Cells[4].Text) && dgItem.Cells[4].Text.Equals("disable"))
                    {
                        optCorr.Checked = false;
                        optCorr.Enabled = false;
                    }
                    else
                    {
                        optCorr.Checked = true;
                        optCorr.Enabled = true;
                    }
                }
            }

        }

        private DataSet CreateGridDataSetDestinatari()
        {
            DataSet retValue = new DataSet();

            dt = new DataTable("GRID_TABLE_DESTINATARI");
            dt.Columns.Add("SYSTEM_ID", typeof(string));
            dt.Columns.Add("TIPO_CORR", typeof(string));
            dt.Columns.Add("DESC_CORR", typeof(string));
            dt.Columns.Add("DISABLED", typeof(string));
            retValue.Tables.Add(dt);

            return retValue;
        }

        protected void CheckOne_Click(object sender, EventArgs e)
        {

            RadioButton check = sender as System.Web.UI.WebControls.RadioButton;

            int rowIndex = Convert.ToInt32(check.Attributes["RowIndex"]);


            foreach (GridViewRow dgItem in this.grdList.Rows)
            {
                RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
                if ((optCorr != null))
                {
                    if (dgItem.RowIndex != rowIndex)
                    {
                        optCorr.Checked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Caricamento dataset utilizzato per le griglie
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="uo"></param>
        private void CaricaGridDataSetDestinatari(DataSet ds, DocsPaWR.Corrispondente[] listaCorrTo, DocsPaWR.Corrispondente[] listaCorrCC)
        {
            DataTable dt = ds.Tables["GRID_TABLE_DESTINATARI"];
            ht_destinatariTO_CC = new Hashtable();
            string tipoURP = "";

            if (listaCorrTo != null && listaCorrTo.Length > 0)
            {
                for (int i = 0; i < listaCorrTo.Length; i++)
                {
                    if (listaCorrTo[i].tipoCorrispondente != null && listaCorrTo[i].tipoCorrispondente.Equals("O"))
                    {
                        this.AppendDataRow(dt, listaCorrTo[i].tipoCorrispondente, listaCorrTo[i].systemId, "&nbsp;" + listaCorrTo[i].descrizione, listaCorrTo[i].dta_fine);
                    }
                    else
                    {
                        if (listaCorrTo[i].GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                        {
                            tipoURP = "U";
                        }
                        if (listaCorrTo[i].GetType().Equals(typeof(DocsPaWR.Ruolo)))
                        {
                            tipoURP = "R";
                        }
                        if (listaCorrTo[i].GetType().Equals(typeof(DocsPaWR.Utente)))
                        {
                            tipoURP = "P";
                        }
                        this.AppendDataRow(dt, listaCorrTo[i].tipoIE, listaCorrTo[i].systemId, GetImage(tipoURP) + " - " + listaCorrTo[i].descrizione, listaCorrTo[i].dta_fine);
                    }
                    ht_destinatariTO_CC.Add(listaCorrTo[i].systemId, listaCorrTo[i]);
                }
            }
            if (listaCorrCC != null && listaCorrCC.Length > 0)
            {
                for (int i = 0; i < listaCorrCC.Length; i++)
                {
                    if (listaCorrCC[i].tipoCorrispondente != null && listaCorrCC[i].tipoCorrispondente.Equals("O"))
                    {
                        this.AppendDataRow(dt, listaCorrCC[i].tipoCorrispondente, listaCorrCC[i].systemId, "&nbsp;" + listaCorrCC[i].descrizione + " (CC)", listaCorrCC[i].dta_fine);
                    }
                    else
                    {
                        if (listaCorrCC[i].GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                        {
                            tipoURP = "U";
                        }
                        if (listaCorrCC[i].GetType().Equals(typeof(DocsPaWR.Ruolo)))
                        {
                            tipoURP = "R";
                        }
                        if (listaCorrCC[i].GetType().Equals(typeof(DocsPaWR.Utente)))
                        {
                            tipoURP = "P";
                        }
                        this.AppendDataRow(dt, listaCorrCC[i].tipoIE, listaCorrCC[i].systemId, GetImage(tipoURP) + " - " + listaCorrCC[i].descrizione + " (CC)", listaCorrCC[i].dta_fine);
                    }
                    ht_destinatariTO_CC.Add(listaCorrCC[i].systemId, listaCorrCC[i]);
                }
            }
        }

        private void AppendDataRow(DataTable dt, string tipoCorr, string systemId, string descCorr, string dtaFine)
        {
            DataRow row = dt.NewRow();
            row["SYSTEM_ID"] = systemId;
            row["TIPO_CORR"] = tipoCorr;
            row["DESC_CORR"] = descCorr;
            if (!string.IsNullOrEmpty(dtaFine))
            {
                row["DISABLED"] = "disable";
            }

            dt.Rows.Add(row);
            row = null;
        }

        private string GetImage(string rowType)
        {
            string retValue = string.Empty;

            switch (rowType)
            {
                case "U":
                    retValue = "uo_icon";
                    break;

                case "R":
                    retValue = "role2_icon";
                    break;

                case "P":
                    retValue = "user_icon";
                    break;
            }

            retValue = " <img src=\"../Images/Icons/" + retValue + ".png\" border=\"\" alt=\"\" />";

            return retValue;
        }

        protected virtual void RenderMessage(string message)
        {
            rowMessage.InnerHtml = message;
            rowMessage.Visible = true;
        }

        private void ClearSessionData()
        {
            this.dt = null;
            this.ht_destinatariTO_CC = null;
        }

        protected void CloseMask(bool withReturnValue)
        {
            this.ClearSessionData();

            string returnValue = "";
            if (withReturnValue) returnValue = "true";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('AnswerChooseRecipient', '" + returnValue + "');} else {parent.closeAjaxModal('AnswerChooseRecipient', '" + returnValue + "');};", true);
        }

        private bool verificaSelezione(out int itemIndex)
        {
            bool verificaSelezione = false;
            itemIndex = -1;
            foreach (GridViewRow dgItem in this.grdList.Rows)
            {
                RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
                if ((optCorr != null) && optCorr.Checked == true)
                {
                    itemIndex = dgItem.RowIndex;
                    verificaSelezione = true;
                    break;
                }
            }
            return verificaSelezione;
        }

        protected void BtnOk_Click(object sender, EventArgs e)
        {
            try {
                DocsPaWR.Corrispondente destSelected = null;
                bool avanzaCor = verificaSelezione(out itemIndex);

                if (avanzaCor)
                {
                    string key = this.grdList.Rows[itemIndex].Cells[0].Text;

                    //prendo la hashTable che contiene i corrisp dalla sesisone
                    if (ht_destinatariTO_CC != null)
                    {
                        if (ht_destinatariTO_CC.ContainsKey(key))
                        {
                            //prendo il corrispondente dalla hashTable secondo quanto è stato richiesto dall'utente
                            destSelected = (DocsPaWR.Corrispondente)ht_destinatariTO_CC[key];
                        }
                    }

                    if (string.IsNullOrEmpty(destSelected.dta_fine))
                    {
                        //creo il documento
                        schedadocIngressoNew = DocumentManager.riproponiDatiRispIngresso(this, this.DocumentWIP, destSelected);
                        FileManager.removeSelectedFile();
                        schedadocIngressoNew.predisponiProtocollazione = true;
                        this.DocumentWIP = schedadocIngressoNew;
                    }
                    this.CloseMask(true);
                }
                else
                {
                    //avviso l'utente che non ha selezionato nessun corrispondente
                    string language = UIManager.UserManager.GetUserLanguage();
                    string msg = "ChainsNoCorrespondentSelected";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void grdList_PageIndexChanged(object sender, GridViewPageEventArgs e)
        {
            try {
                this.grdList.SelectedIndex = -1;
                this.grdList.PageIndex = e.NewPageIndex;
                //this.grdList.PageIndex = this.grdList.PageIndex + 1;
                this.grdList.DataSource = this.dt;
                this.grdList.DataBind();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

    }
}