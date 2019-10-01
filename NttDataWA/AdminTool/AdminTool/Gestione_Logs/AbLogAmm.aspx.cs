using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using Microsoft.Web.UI.WebControls;

namespace SAAdminTool.AdminTool.Gestione_Logs
{
    public partial class AbLogAmm : System.Web.UI.Page
    {
        #region WebControls e variabili
        protected System.Web.UI.WebControls.DataGrid dg_AbilitaLogAmm;
        protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.WebControls.Label lbl_tit;
        protected System.Web.UI.WebControls.Button btn_modifica;
        protected System.Web.UI.WebControls.CheckBox chk_all;
        protected DataSet dataSet;
        #endregion

        #region Page_Load

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            if (Session.IsNewSession)
            {
                Response.Redirect("../Exit.aspx?FROM=EXPIRED");
            }

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            if (!ws.CheckSession(Session.SessionID))
            {
                Response.Redirect("../Exit.aspx?FROM=ABORT");
            }
            // ---------------------------------------------------------------

            if (!IsPostBack)
            {
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
                lbl_tit.Text = "Attivazione Log Amministrazione";

                chk_all.Attributes.Add("onclick", "CheckAllDataGridCheckBoxes('Chk',document.forms[0].chk_all.checked)");

                LoadDataGrid();
            }
        }
        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_modifica.Click += new System.EventHandler(this.btn_modifica_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        #region datagrid

        /// <summary>
        /// LoadDataGrid
        /// </summary>
        public void LoadDataGrid()
        {
            XmlDocument xmlDoc = new XmlDocument();
            DataRow row;

            try
            {
                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                string xmlStream = ws.GetXmlLogAmm(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"));

                if (xmlStream != null && xmlStream != "")
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlStream);

                    //XmlNode listaAzioni = doc.SelectSingleNode("AMMINISTRAZIONE");
                    XmlNode listaAzioni = doc.SelectSingleNode("NewDataSet");
                    if (listaAzioni.ChildNodes.Count > 0)
                    {
                        IniDataSet();
                        foreach (XmlNode azione in listaAzioni.ChildNodes)
                        {
                            //carica il dataset
                            row = dataSet.Tables[0].NewRow();
                            row["descrizione"] = azione.ChildNodes[1].InnerText;
                            row["oggetto"] = azione.ChildNodes[2].InnerText;
                            if (azione.ChildNodes[4].InnerText == "1")
                            {
                                row["attivo"] = "true";
                            }
                            else
                            {
                                row["attivo"] = "false";
                            }
                            row["codice"] = azione.ChildNodes[0].InnerText;

                            dataSet.Tables["AZIONIAMM"].Rows.Add(row);
                        }

                        DataView dv = dataSet.Tables["AZIONIAMM"].DefaultView;
                        dv.Sort = "oggetto ASC, descrizione ASC";
                        dg_AbilitaLogAmm.DataSource = dv;
                        dg_AbilitaLogAmm.DataBind();

                        //MEV CONS 1.3
                        //se la chiave PGU_FE_DISABLE_AMM_GEST_CONS è abilitata
                        //i log conservazione sono nascosti
                        for (int i = 0; i < dg_AbilitaLogAmm.Items.Count; i++)
                        {
                            if ((this.DisableAmmGestCons() && dg_AbilitaLogAmm.Items[i].Cells[0].Text == "CONSERVAZIONE"))
                                dg_AbilitaLogAmm.Items[i].Visible = false;
                        }

                    }
                    else
                    {
                        IniDataSet();
                        btn_modifica.Visible = false;
                    }
                }
                else
                {
                    lbl_tit.Text = "ATTENZIONE! il file XML dei log è vuoto!";
                    btn_modifica.Visible = false;
                }
            }
            catch
            {
                lbl_tit.Text = "ATTENZIONE! errore nel caricamento del file XML dei log!";
                btn_modifica.Visible = false;
            }
        }

        //MEV CONS 1.3
        /// <summary>
        /// Funzione per la gestione dell'abilitazione/disabilitazione della visualizzazione
        /// dei log conservazione
        /// </summary>
        /// <returns></returns>
        protected bool DisableAmmGestCons()
        {
            bool result = false;

            string PGU_FE_DISABLE_AMM_GEST_CONS_Value = string.Empty;
            PGU_FE_DISABLE_AMM_GEST_CONS_Value = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "PGU_FE_DISABLE_AMM_GEST_CONS");
            result = ((PGU_FE_DISABLE_AMM_GEST_CONS_Value.Equals("0") || string.IsNullOrEmpty(PGU_FE_DISABLE_AMM_GEST_CONS_Value)) ? false : true);

            return result;

        }

        /// <summary>
        /// Inizializza il dataset
        /// </summary>
        private void IniDataSet()
        {
            dataSet = new DataSet();

            dataSet.Tables.Add("AZIONIAMM");

            DataColumn dc = new DataColumn("descrizione");
            dataSet.Tables["AZIONIAMM"].Columns.Add(dc);

            dc = new DataColumn("oggetto");
            dataSet.Tables["AZIONIAMM"].Columns.Add(dc);

            dc = new DataColumn("attivo");
            dataSet.Tables["AZIONIAMM"].Columns.Add(dc);

            dc = new DataColumn("codice");
            dataSet.Tables["AZIONIAMM"].Columns.Add(dc);
        }

        /// <summary>
        /// ordina il datagrid
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="sortColumn"></param>
        /// <returns></returns>
        private DataView OrdinaGrid(DataView dv, string sortColumn)
        {
            dv.Sort = sortColumn + " ASC";
            return dv;
        }

        #endregion

        #region modifica

        /// <summary>
        /// Tasto modifica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_modifica_Click(object sender, System.EventArgs e)
        {
            DataRow row;
            CheckBox cb;

            IniDataSet();

            try
            {
                for (int i = 0; i < this.dg_AbilitaLogAmm.Items.Count; i++)
                {
                    //carica il dataset
                    row = dataSet.Tables[0].NewRow();
                    row["descrizione"] = dg_AbilitaLogAmm.Items[i].Cells[1].Text;
                    row["oggetto"] = dg_AbilitaLogAmm.Items[i].Cells[0].Text;

                    cb = (CheckBox)dg_AbilitaLogAmm.Items[i].Cells[2].FindControl("Chk");

                    if (cb.Checked)
                    {
                        row["attivo"] = "1";
                    }
                    else
                    {
                        row["attivo"] = "0";
                    }
                    row["codice"] = dg_AbilitaLogAmm.Items[i].Cells[3].Text;

                    dataSet.Tables["AZIONIAMM"].Rows.Add(row);
                }

                if (dataSet.Tables["AZIONIAMM"].Rows.Count > 0)
                {
                    string streamXml = dataSet.GetXml().ToUpper();

                    // stream verso il WS
                    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                    if (!ws.SetXmlLogAmm(streamXml, AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0")))
                    {
                        lbl_tit.Text = "Attivazione Log Amministrazione - <b><font color='#ff0000'>ATTENZIONE! errore nella modifica!</font></b>";
                        btn_modifica.Visible = false;
                    }
                    else
                    {
                        lbl_tit.Text = "Attivazione Log Amministrazione - Stato: <b>Modificato</b>";
                        LoadDataGrid();
                    }
                }
            }
            catch
            {
                lbl_tit.Text = "Attivazione Log Amministrazione - <b><font color='#ff0000'>ATTENZIONE! errore nella modifica!</font></b>";
                btn_modifica.Visible = false;
            }
        }
        #endregion

    }
}
