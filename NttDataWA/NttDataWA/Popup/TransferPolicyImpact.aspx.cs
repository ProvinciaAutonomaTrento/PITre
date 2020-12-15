using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Data;
using NttDataWA.Utils;
using NttDatalLibrary;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Web.UI.DataVisualization.Charting;
using System.Globalization;


namespace NttDataWA.Popup
{
    public partial class TransferPolicyImpact : System.Web.UI.Page
    {

        #region fields

        private ARCHIVE_TransferPolicy policySelected
        {
            get
            {
                ARCHIVE_TransferPolicy result = null;
                if (HttpContext.Current.Session["SelectedPolicy"] != null)
                    result = (ARCHIVE_TransferPolicy)HttpContext.Current.Session["SelectedPolicy"];

                return result;
            }
            set
            {
                HttpContext.Current.Session["SelectedPolicy"] = value;
            }
        }

        private List<ARCHIVE_Profile_TransferPolicy> listaProfileTransferPolicy
        {
            get
            {
                List<ARCHIVE_Profile_TransferPolicy> result = null;
                if (HttpContext.Current.Session["listaProfileTransferPolicy"] != null)
                    result = (List<ARCHIVE_Profile_TransferPolicy>)HttpContext.Current.Session["listaProfileTransferPolicy"];

                return result;
            }
            set
            {
                HttpContext.Current.Session["listaProfileTransferPolicy"] = value;
            }
        }

        public List<ARCHIVE_Profile_TransferPolicy> grdDocImpactSource
        {
            get
            {
                List<ARCHIVE_Profile_TransferPolicy> result = null;
                if (HttpContext.Current.Session["grdDocImpactSourcePolicy"] != null)
                {
                    result = HttpContext.Current.Session["grdDocImpactSourcePolicy"] as List<ARCHIVE_Profile_TransferPolicy>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["grdDocImpactSourcePolicy"] = value;

            }
        }
        #endregion

        #region struct

        public struct VincoloCopia
        {
            private string Vincolo;

            public string vincolo
            {
                get { return Vincolo; }
                set { Vincolo = value; }
            }
            private double Percentuale;

            public double percentuale
            {
                get { return Percentuale; }
                set { Percentuale = value; }
            }

            public VincoloCopia(string vincolo, double percent)
            {
                Vincolo = vincolo;
                Percentuale = percent;
            }


        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.initializePage();
            }

            this.RefreshScript();
        }

        private void initializePage()
        {
            this.InitLanguage();
            this.GetDataPage();
            this.LoadDataInControl();

        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.btnPolicyImpactChiudi.Text = Utils.Languages.GetLabelFromCode("AddDocInProjectBtnClose", language);
            this.litPolicyId.Text = Utils.Languages.GetLabelFromCode("LitPolicyId", language);
            this.LitDescPolicy.Text = Utils.Languages.GetLabelFromCode("LitDescPolicy", language);
            this.ChartTransfer.Titles[0].Text = Utils.Languages.GetLabelFromCode("ChartTransferTitle", language);

            this.LitTipoAnalisi.Text = Utils.Languages.GetLabelFromCode("LitTipoAnalisi", language);
            this.lblTitoloGrdDocPolicy.Text = Utils.Languages.GetLabelFromCode("impactTitoloGrdDoc", language);
            
            //sezione grafici
            ChartAcc5Anni.Titles[0].Text = Utils.Languages.GetLabelFromCode("ChartAcc5AnniTitle", language);
            ChartAcc12Mesi.Titles[0].Text = Utils.Languages.GetLabelFromCode("ChartAcc12MesiTitle", language);
        }

        private void GetDataPage()
        {
            if (this.policySelected != null)
            {

                this.TxtPolicyId.Text = this.policySelected.System_ID.ToString();
                this.TxtDescPolicy.Text = this.policySelected.Description;

                this.listaProfileTransferPolicy = UIManager.ArchiveManager.GetARCHIVE_Profile_TransferPolicyByTransferPolicy_ID(this.policySelected.System_ID);

                DateTime startDate = new DateTime(DateTime.Now.AddYears(-1).Year, DateTime.Now.Month, 1);
                //lista di dati della griglia grdDoc documenti 
                this.grdDocImpactSource = this.listaProfileTransferPolicy.Where(x => x.TipoTransferimento_Policy == "TRASFERIMENTO"
                                                            && x.DataUltimoAccesso > startDate).Select(x => x).ToList();

            }
        }

        private void LoadDataInControl()
        {
            try
            {
                this.LoadDataInRblTransferImpact();

                this.LoadDataInPieChart();
                this.loadDataInGrdVincolo();
                this.upPnlChartPieGrid.Update();

                this.loadDataInBarChartAnni();
                this.loadDataInBarChartMesi();

                this.loadDataInGrdDoc();
                this.upPnlGrdDoc12mesi.Update();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }

        }

        private void LoadDataInRblTransferImpact()
        {
            // creazione dinamica della rbl
            //this.rblTransferImpact.Items.Add(new ListItem("", "1"));
            //this.rblTransferImpact.Items.Add(new ListItem("", "2"));
            //this.rblTransferImpact.Items.Add(new ListItem("", "3"));
            //this.rblTransferImpact.Items.Add(new ListItem("", "4"));
        }

        protected void GrdDoc_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.GrdDoc.DataSource = this.listaProfileTransferPolicy;
            this.GrdDoc.PageIndex = e.NewPageIndex;
            this.GrdDoc.DataBind();
            this.upPnlGrdDoc12mesi.Update();

        }

        private void loadDataInGrdDoc()
        {
            if (this.grdDocImpactSource == null || this.grdDocImpactSource.Count == 0)
                this.grdDocImpactSource = this.getGrdDocDummy();

            this.GrdDoc.DataSource = this.grdDocImpactSource;
            this.GrdDoc.DataBind();
        }

        private List<ARCHIVE_Profile_TransferPolicy> getGrdDocDummy()
        {
            List<ARCHIVE_Profile_TransferPolicy> lista = new List<ARCHIVE_Profile_TransferPolicy>();
            ARCHIVE_Profile_TransferPolicy item = new ARCHIVE_Profile_TransferPolicy();
            item.Profile_ID = 0;
            item.TransferPolicy_ID = 0;
            item.Registro = string.Empty;
            item.DataCreazione = null;
            item.CopiaPerCatenaDoc_Versamento = 0;
            lista.Add(item);

            return lista;
        }

        private void loadDataInBarChartMesi()
        {
            if (this.listaProfileTransferPolicy != null && this.listaProfileTransferPolicy.Count > 0)
            {
                var list = from x in listaProfileTransferPolicy
                           where x.DataUltimoAccesso != null && x.DataUltimoAccesso > DateTime.Now.AddMonths(-12)
                           select x;


                var groupResult = from p in list
                                  group p by p.DataUltimoAccesso.Value.Month into counts
                                  select new { Numero = counts.Count(), Mese = counts.Key };

                if (groupResult.Count() > 0)
                {
                    this.pnlChart12mesi.Visible = true;
                    this.pnlErrore12mesi.Visible = false;

                    List<int> Mese = groupResult.Select(x => x.Mese).Distinct().ToList();
                    List<int> numAccessi = groupResult.Select(x => x.Numero).ToList();

                    //se i dati sono inferiori di quelli sufficenti per creare il report
                    //aggiungo valori nulli per rendere il grafico leggibile
                    if (Mese.Count < 12)
                    {

                        List<int> templateMesi = Enumerable.Range(DateTime.Now.AddMonths(-1).Month, 12).Select(x => (x % 12) + 1)
                                                       .ToList();

                        List<int> mesiRes = new List<int>();
                        List<int> numAccessiRes = new List<int>();

                        int index = 0;

                        for (int i = 0; i < templateMesi.Count; i++)
                        {
                            if (Mese.Contains(templateMesi[i]))
                            {
                                index = Mese.IndexOf(templateMesi[i]);
                                mesiRes.Add(templateMesi[i]);
                                numAccessiRes.Add(numAccessi[index]);
                            }
                            else
                            {
                                mesiRes.Add(templateMesi[i]);
                                numAccessiRes.Add(0);
                            }
                        }

                        Mese = mesiRes;
                        numAccessi = numAccessiRes;
                    }

                    Color[] colorSet = { Color.LightSeaGreen ,
                                                Color.SteelBlue,
                                                Color.LightSlateGray,
                                                Color.MediumSlateBlue,
                                                Color.MediumSeaGreen,
                                                Color.LightGoldenrodYellow,
                                                Color.MediumPurple,
                                                Color.MediumTurquoise,
                                                Color.MidnightBlue,
                                                Color.LightSkyBlue,
                                                Color.MediumOrchid,
                                                Color.MediumVioletRed};

                    //gestione delle etichette dell'asse x
                    ChartAcc12Mesi.ChartAreas["ChartArea3"].AxisX.Interval = 1;
                    ChartAcc12Mesi.ChartAreas["ChartArea3"].AxisX.LabelStyle.Angle = -45;
                    ChartAcc12Mesi.ChartAreas["ChartArea3"].AxisX.LabelStyle.Font = new Font("Verdana", 8);
                    //ChartAcc12Mesi.Series["SerieAcc12Mesi"]["DrawingStyle"] = "Cylinder";

                    string nomeMese;
                    DataPoint point;
                    Legend item = new Legend("Legend");


                    for (int i = 0; i < Mese.Count(); i++)
                    {

                        nomeMese = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Mese[i]);
                        point = new DataPoint();
                        point.SetValueXY(nomeMese, numAccessi[i]);


                        point.Color = colorSet[i];
                        item.CustomItems.Add(point.Color, nomeMese + " = " + point.YValues[0].ToString() + " accessi");
                        ChartAcc12Mesi.Series["SerieAcc12Mesi"].Points.Add(point);
                    }


                    //ChartAcc12Mesi.ApplyPaletteColors();

                    //asscio il valore di ogni grafico come tooltip
                    ChartAcc12Mesi.Series["SerieAcc12Mesi"].ToolTip = "#VALY";

                    ChartAcc12Mesi.Series["SerieAcc12Mesi"].IsVisibleInLegend = false;

                    //gestione della legenda
                    ChartAcc12Mesi.Legends.Add(item);

                    ChartAcc12Mesi.Legends[0].Enabled = true;
                    //ChartAcc12Mesi.Titles[0].Text = "Numero accessi negli ultimi 12 mesi";
                    ChartAcc12Mesi.Titles[0].Font = new Font("Verdana", 11, FontStyle.Bold);
                    ChartAcc12Mesi.Legends[0].Title = "Mesi";


                    ChartAcc12Mesi.Legends[0].Docking = Docking.Right;
                    ChartAcc12Mesi.Legends[0].Alignment = System.Drawing.StringAlignment.Center;
                    ChartAcc12Mesi.Legends[0].Font = new Font("Verdana", 8);
                    ChartAcc12Mesi.Legends[0].TitleFont = new Font("Verdana", 10, FontStyle.Bold);

                }
                else
                {
                    //non ci sono dati per questo report
                    this.pnlChart12mesi.Visible = false;
                    this.pnlErrore12mesi.Visible = true;

                }

            }
        }

        private void loadDataInBarChartAnni()
        {

            if (this.listaProfileTransferPolicy != null && this.listaProfileTransferPolicy.Count > 0)
            {

                int anno = DateTime.Now.Year;
                var list = from p in listaProfileTransferPolicy
                           where p.DataUltimoAccesso != null && p.DataUltimoAccesso.Value.Year > (anno - 5)
                           select p;

                //risultati raggruppati per anno
                var groupResult = from p in list
                                  group p by p.DataUltimoAccesso.Value.Year into counts
                                  select new { Numero = counts.Count(), Anno = counts.Key };


                if (groupResult.Count() > 0)
                {
                    this.pnlChart5Anni.Visible = true;
                    this.pnlErrore5Anni.Visible = false;

                    List<int> anni = groupResult.Select(x => x.Anno).Distinct().ToList();
                    List<int> numAccessi = groupResult.Select(x => x.Numero).ToList();

                    //se non ci sono abbastanza dati per creare il report ne aggiungo di nulli
                    //per renderlo più leggibile
                    if (anni.Count < 5)
                    {

                        List<int> templateAnni = Enumerable.Range(anno - 4, 5)
                                                           .ToList();
                        List<int> anniRes = new List<int>();
                        List<int> numAccessiRes = new List<int>();

                        int index = 0;

                        for (int i = 0; i < templateAnni.Count; i++)
                        {
                            if (anni.Contains(templateAnni[i]))
                            {
                                index = anni.IndexOf(templateAnni[i]);
                                anniRes.Add(templateAnni[i]);
                                numAccessiRes.Add(numAccessi[index]);
                            }
                            else
                            {
                                anniRes.Add(templateAnni[i]);
                                numAccessiRes.Add(0);
                            }
                        }

                        anni = anniRes;
                        numAccessi = numAccessiRes;
                    }

                    Color[] colorSet = {Color.LightSeaGreen,
                                         Color.SteelBlue,
                                         Color.LightSlateGray ,
                                         Color.MediumSlateBlue,
                                         Color.MediumSeaGreen};


                    Legend item = new Legend("Legend");
                    ChartAcc5Anni.Series["SerieAcc5Anni"].IsVisibleInLegend = false;

                    DataPoint point;

                    for (int i = 0; i < anni.Count(); i++)
                    {
                        point = new DataPoint();

                        point.SetValueXY(anni[i], numAccessi[i]);
                        point.Color = colorSet[i];
                        ChartAcc5Anni.Series["SerieAcc5Anni"].Points.Add(point);

                        item.CustomItems.Add(point.Color, point.XValue.ToString() + " = " + point.YValues[0].ToString() + " documenti");

                    }

                    ChartAcc5Anni.Series["SerieAcc5Anni"].ToolTip = "#VALY";

                    ChartAcc5Anni.Titles[0].Font = new Font("Verdana", 11, FontStyle.Bold);

                    ChartAcc5Anni.Legends.Add(item);
                    ChartAcc5Anni.Legends[0].Enabled = true;
                    ChartAcc5Anni.Legends[0].Title = "Anni";
                    ChartAcc5Anni.Legends[0].TitleFont = new Font("Verdana", 11, FontStyle.Bold);

                    ChartAcc5Anni.Legends[0].Alignment = System.Drawing.StringAlignment.Center;
                    ChartAcc5Anni.Legends[0].Font = new Font("Verdana", 9);
                    ChartAcc5Anni.Legends[0].TitleFont = new Font("Verdana", 11, FontStyle.Bold);

                }
                else
                {
                    //non ci sono risultati per lo specifico report
                    this.pnlChart5Anni.Visible = false;
                    this.pnlErrore5Anni.Visible = true;

                }
            }



        }



        private void LoadDataInPieChart()
        {
            if (this.listaProfileTransferPolicy != null && this.listaProfileTransferPolicy.Count > 0)
            {
                double totpProfile = this.listaProfileTransferPolicy.Count;
                double nTrans = this.listaProfileTransferPolicy.Where(x => x.TipoTransferimento_Policy.Equals("TRASFERIMENTO")).Count();
                double nCopia = this.listaProfileTransferPolicy.Where(x => x.TipoTransferimento_Policy.Equals("COPIA")).Count();

                double percentCopie = nCopia / totpProfile;
                double percentTransfer = nTrans / totpProfile;




                double[] yValues = { percentTransfer, percentCopie };
                string[] xValues = { "Documenti trasferiti in modo effetivo ", "Documenti trasferiti come copia " };

                ChartTransfer.Series["SerieTransfer"].Points.DataBindXY(xValues, yValues);

                ChartTransfer.Series["SerieTransfer"].Points[0].Color = Color.MediumSeaGreen;
                ChartTransfer.Series["SerieTransfer"].Points[1].Color = Color.FromArgb(200, Color.Blue);
                ChartTransfer.Series["SerieTransfer"].Points[1]["Exploded"] = (nCopia > 0).ToString(); ;

                //ChartTransfer.Series["SerieTransfer"].Points[2].Color = Color.LawnGreen;

                ChartTransfer.Series["SerieTransfer"].ChartType = SeriesChartType.Pie;

                ChartTransfer.Series["SerieTransfer"].Points[0].ToolTip = (Math.Round(yValues[0], 2) * 100).ToString() + "%";
                ChartTransfer.Series["SerieTransfer"].Points[1].ToolTip = (Math.Round(yValues[1], 2) * 100).ToString() + "%";
                ChartTransfer.Series["SerieTransfer"].Points[0].Label = xValues[0] + "(" + (Math.Round(yValues[0], 2) * 100).ToString() + "%)";
                ChartTransfer.Series["SerieTransfer"].Points[1].Label = xValues[1] + "(" + (Math.Round(yValues[1], 2) * 100).ToString() + "%)";

                ChartTransfer.Series["SerieTransfer"]["PieLabelStyle"] = "Disabled";


                ChartTransfer.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                ChartTransfer.ChartAreas["ChartArea1"].Area3DStyle.LightStyle =
                                            System.Web.UI.DataVisualization.Charting.LightStyle.Realistic;


                ChartTransfer.Legends[0].Enabled = true;
                ChartTransfer.Legends[0].Title = "Tipo trasferimento";
                ChartTransfer.Legends[0].Font = new Font("Verdana", 10, FontStyle.Bold);



            }
            else
            {

            }
        }

        private void loadDataInGrdVincolo()
        {
            if (this.listaProfileTransferPolicy != null && this.listaProfileTransferPolicy.Count > 0)
            {
                double nCopia = this.listaProfileTransferPolicy.Where(x => x.TipoTransferimento_Policy.Equals("COPIA")).Count();
                double percFascDoc = 0;
                double percCatDoc = 0;
                double percCons = 0;
                if (nCopia > 0)
                {
                    double nFascProc = this.listaProfileTransferPolicy.Where(x => x.CopiaPerFascicolo_Policy == 1).Count();
                    double nCatDoc = this.listaProfileTransferPolicy.Where(x => x.CopiaPerCatenaDoc_Policy == 1).Count();
                    double nCons = this.listaProfileTransferPolicy.Where(x => x.CopiaPerConservazione_Policy == 1).Count();

                    percFascDoc = Math.Round(nFascProc / nCopia, 2) * 100;
                    percCatDoc = Math.Round(nCatDoc / nCopia, 2) * 100;
                    percCons = Math.Round(nCons / nCopia, 2) * 100;
                }

                List<VincoloCopia> grdSource = new List<VincoloCopia>();
                grdSource.Add(new VincoloCopia("Documenti inclusi in altri fascicoli procidementali in archivio corrente", percFascDoc));
                grdSource.Add(new VincoloCopia("Documenti concatenati ad altri documenti in archivio corrente", percCatDoc));
                grdSource.Add(new VincoloCopia("Documenti in conservazione in archivio corrente", percCons));

                this.GrdVincoli.DataSource = grdSource;
                this.GrdVincoli.DataBind();

            }

        }

        protected void GrdDoc_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //int i = e.Row.RowIndex;
                string idProfile = this.GrdDoc.DataKeys[e.Row.RowIndex].Value.ToString();
                if (idProfile.Equals("0"))
                {
                    ((Label)e.Row.FindControl("LblUtentiValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblAOOValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblIdValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblOggettoValue")).Visible = false;
                    ((Label)e.Row.FindControl("LbltipoValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblTipoDocValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblDataCreazioneValue")).Visible = false;
                    ((Label)e.Row.FindControl("LblAccValue")).Visible = false;

                }
            }
        }

        private void RefreshScript()
        {
            ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this.ChartTransfer);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);

        }

        protected void rblPolicyImpact_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.showSelectedPanel(this.rblPolicyImpact.SelectedValue);

        }

        private void showSelectedPanel(string value)
        {
            switch (value)
            {
                case "1":
                    this.pnlChartPieGrid.Visible = true;

                    this.upPnlChartPieGrid.Update();

                    this.pnlNDocAcc5anni.Visible = false;
                    this.upPnlNDocAcc5anni.Update();

                    this.pnlNDocAcc12mesi.Visible = false;
                    this.upPnlNDocAcc12mesi.Update();

                    this.pnlGrdDoc12mesi.Visible = false;
                    this.upPnlGrdDoc12mesi.Update();
                    break;

                case "2":
                    this.pnlChartPieGrid.Visible = false;
                    this.upPnlChartPieGrid.Update();

                    this.pnlNDocAcc5anni.Visible = true;
                    this.upPnlNDocAcc5anni.Update();

                    this.pnlNDocAcc12mesi.Visible = false;
                    this.upPnlNDocAcc12mesi.Update();

                    this.pnlGrdDoc12mesi.Visible = false;
                    this.upPnlGrdDoc12mesi.Update();
                    break;

                case "3":
                    this.pnlChartPieGrid.Visible = false;
                    this.upPnlChartPieGrid.Update();

                    this.pnlNDocAcc5anni.Visible = false;
                    this.upPnlNDocAcc5anni.Update();

                    this.pnlNDocAcc12mesi.Visible = true;
                    this.upPnlNDocAcc12mesi.Update();

                    this.pnlGrdDoc12mesi.Visible = false;
                    this.upPnlGrdDoc12mesi.Update();
                    break;

                case "4":
                    this.pnlChartPieGrid.Visible = false;
                    this.upPnlChartPieGrid.Update();

                    this.pnlNDocAcc5anni.Visible = false;
                    this.upPnlNDocAcc5anni.Update();

                    this.pnlNDocAcc12mesi.Visible = false;
                    this.upPnlNDocAcc12mesi.Update();

                    this.pnlGrdDoc12mesi.Visible = true;
                    this.upPnlGrdDoc12mesi.Update();
                    break;
            }
        }

        protected void btnPolicyImpactChiudi_Click(object sender, EventArgs e)
        {
            try
            {
                this.ClearSessionData();
                this.ClosePage();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ClosePage()
        {


            //ScriptManager.RegisterClientScriptBlock(this.upPnlButtons, this.upPnlButtons.GetType(), "closeAJM", "closeAjaxModal('TransferPolicyImpact','');", true);
            ScriptManager.RegisterClientScriptBlock(this.upPnlButtons, this.upPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('TransferPolicyImpact','',parent);", true);
        }

        private void ClearSessionData()
        {
            this.policySelected = null;
            this.listaProfileTransferPolicy = null;
        }

    }



}