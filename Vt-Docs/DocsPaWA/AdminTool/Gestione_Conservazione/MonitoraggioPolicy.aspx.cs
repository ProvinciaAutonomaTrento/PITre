using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class MonitoraggioPolicy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                this.InitializePage();
            }
        }

        protected void InitializePage()
        {
            try
            {
                DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
                DocsPAWA.AdminTool.Manager.AmministrazioneManager ammManager = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
                DocsPaWR.InfoUtenteAmministratore infoAmministratore = session.getUserAmmSession();

                if (infoAmministratore.tipoAmministratore == "2" || infoAmministratore.tipoAmministratore == "3")
                {
                    this.ddl_amministrazione.Items.Clear();
                    ammManager.InfoAmmCorrente(infoAmministratore.idAmministrazione);
                    DocsPaWR.InfoAmministrazione infoAmm = ammManager.getCurrentAmm();
                    this.ddl_amministrazione.Items.Add(new ListItem()
                    {
                        Text = string.Format("[{0}] {1}", infoAmm.Codice, infoAmm.Descrizione),
                        Value = infoAmm.IDAmm,
                        Selected = true
                    });

                    this.ddl_amministrazione.Enabled = false;
                }
                else
                {

                    ammManager.ListaAmministrazioni();

                    if (ammManager.getListaAmministrazioni() != null && ammManager.getListaAmministrazioni().Count > 0)
                    {
                        this.ddl_amministrazione.Items.Clear();
                        this.ddl_amministrazione.Items.Add(new ListItem() { Text = string.Empty, Value = "0" });
                        foreach (DocsPaWR.InfoAmministrazione amm in ammManager.getListaAmministrazioni())
                        {
                            ListItem item = new ListItem()
                            {
                                Text = string.Format("[{0}] {1}", amm.Codice, amm.Descrizione),
                                Value = amm.IDAmm
                            };
                            this.ddl_amministrazione.Items.Add(item);
                            this.ddl_amministrazione.Enabled = true;
                        }
                    }
                }

                this.ddl_dataEsecuzione_SelectedIndexChanged(null, null);

            }
            catch (Exception)
            {

            }
        }

        protected void btn_export_Click(object sender, EventArgs e)
        {
            if (this.checkInput())
            {
                DocsPAWA.DocsPaWR.ReportMonitoraggioPolicyRequest request = new DocsPaWR.ReportMonitoraggioPolicyRequest();
                request.IdAmm = this.ddl_amministrazione.SelectedValue;
                request.Codice = this.txtCodPolicy.Text.Trim();
                request.Descrizione = this.txtDescPolicy.Text.Trim();
                request.TipoDataEsecuzione = this.ddl_dataEsecuzione.SelectedValue;
                if (request.TipoDataEsecuzione == "S")
                {
                    request.DataEsecuzioneFrom = this.lbl_dataCreazioneDa.Text;
                }
                if (request.TipoDataEsecuzione == "R")
                {
                    request.DataEsecuzioneFrom = this.lbl_dataCreazioneDa.Text;
                    request.DataEsecuzioneTo = this.lbl_dataCreazioneA.Text;
                }

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                DocsPaWR.FileDocumento doc = ws.ReportMonitoraggioPolicy(request);

                if (doc != null)
                {
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", "attachment; filename=test.xls");
                    Response.BinaryWrite(doc.content);
                    Response.Flush();
                    Response.End();
                }
                else
                {

                }
            }

        }

        protected void ddl_dataEsecuzione_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(this.ddl_dataEsecuzione.SelectedValue)
            {
                case "S":
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lblDa.Text = "Il";
                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;
                case "R":
                    this.lblA.Visible = true;
                    this.lbl_dataCreazioneA.Visible = true;
                    this.lblDa.Text = "Da";
                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;
                case "M":
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    break;
            }
            this.UpdatePanelSelCrit.Update();
        }

        protected bool checkInput()
        {
            
            if(this.ddl_dataEsecuzione.SelectedValue == "S")
            {
                if(string.IsNullOrEmpty(this.lbl_dataCreazioneDa.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Export_err1", "alert('Selezionare una data valida');", true);
                    return false;
                }
            }
            if(this.ddl_dataEsecuzione.SelectedValue == "R")
            {
                bool check = true;
                string dataFrom = this.lbl_dataCreazioneDa.Text;
                string dataTo = this.lbl_dataCreazioneA.Text;
                if(string.IsNullOrEmpty(dataFrom))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Export_err3a", "alert('Selezionare un intervallo valido');", true);
                    return false;
                }
                else
                {
                    if(string.IsNullOrEmpty(dataTo))
                    {
                        try
                        {
                            DateTime dd = Convert.ToDateTime(dataFrom);
                            if(DateTime.Compare(dd, DateTime.Now) > 0)
                            {
                                check = false;
                            }
                            else if((DateTime.Now - dd).TotalDays > 30)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Export_err2a", "alert('L\\'intervallo massimo selezionabile è un mese');", true);
                                return false;
                            }
                        }
                        catch(Exception)
                        {
                            check = false;
                        }

                        if (!check)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Export_err3a", "alert('Selezionare un intervallo valido');", true);
                            return false;
                        }
                    }
                    else
                    {
                        try
                        {
                            DateTime d1 = Convert.ToDateTime(dataFrom);
                            DateTime d2 = Convert.ToDateTime(dataTo);

                            if(DateTime.Compare(d2, d1) < 0)
                            {
                                check = false;
                            }
                            else if((d2-d1).TotalDays > 30)
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Export_err2b", "alert('L\\'intervallo massimo selezionabile è un mese');", true);
                                return false;
                            }
                        }
                        catch(Exception)
                        {
                            check = false;
                        }

                        if(!check)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Export_err3b", "alert('Selezionare un intervallo valido');", true);
                            return false;
                        }
                        
                    }
                }
               
            }

            return true;
        }
    }
}