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
using System.Configuration;
using System.Globalization;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.gestione.registro
{
	/// <summary>
	/// Summary description for regElenco.
	/// </summary>
    public partial class regRepertorioElenco : System.Web.UI.Page
	{
        protected System.Web.UI.WebControls.DataGrid DataGrid2;
        private DocsPAWA.AdminTool.UserControl.ScrollKeeper skDgTemplate;
        protected System.Web.UI.WebControls.Button btn_stampaRepertori;
        protected System.Web.UI.WebControls.Button btn_cambiaStato;
		protected ArrayList Dt_elem;

		private void Page_Load(object sender, System.EventArgs e)
		{
			Utils.startUp(this);
			if(!Page.IsPostBack)
			{
				try
				{

                    BindGrid();
                    //disabilito i pulsanti di stampa e cambia stato
                    if (DataGrid2.Items.Count < 1)
                    {
                        btn_stampaRepertori.Enabled = false;
                        btn_cambiaStato.Enabled = false;
                    }
                    if (!this.IsPostBack)
                    {
                        this.PerformActionSelectFirstElemento();
                    }

				}
				catch (System.Exception ex)
				{
					ErrorManager.redirect(this, ex);
				}
			}
            DocsPAWA.AdminTool.UserControl.ScrollKeeper skDgTemplate = new DocsPAWA.AdminTool.UserControl.ScrollKeeper();
            skDgTemplate.WebControl = "DivDGList";
            this.Form.Controls.Add(skDgTemplate);

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}
        #endregion

        #region datagrid

        public void BindGrid()
        {

            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            DocsPaWR.RegistroRepertorio[] listaRegRepert = ws.GetRegistriesWithAooOrRf(null, UserManager.getInfoUtente().idGruppo);

            if (listaRegRepert != null && listaRegRepert.Length > 0)
            {
                string stato = string.Empty;
                string responsabile = string.Empty;
                string dtaLastPrint = string.Empty;
                Dt_elem = new ArrayList();
                string img_stato = "";
                for (int i = 0; i < listaRegRepert.Length; i++)
                {
                    string descrTipoDocumento = listaRegRepert[i].TipologyDescription;
                    string counterID = listaRegRepert[i].CounterId;
             
                    if (listaRegRepert[i].SingleSettings != null && listaRegRepert[i].SingleSettings.Length > 0)
                    {
                        foreach (DocsPaWR.RegistroRepertorioSingleSettings regRep in listaRegRepert[i].SingleSettings)
                        {
                            stato = regRep.CounterState.ToString().Equals("O") ? "Aperto" : "Chiuso";
                            stato = GetImageStato(stato);
                            responsabile = regRep.RoleAndUserDescription;
                            dtaLastPrint = regRep.DateLastPrint.ToShortDateString();
                            if (!string.IsNullOrEmpty(regRep.RegistryId))
                            {
                                Dt_elem.Add(new ColsRF(descrTipoDocumento, counterID, regRep.RegistryOrRfDescription, "Registro", regRep.RegistryId, "", stato, responsabile, dtaLastPrint));
                            }
                            else if (!string.IsNullOrEmpty(regRep.RFId))
                            {
                                Dt_elem.Add(new ColsRF(descrTipoDocumento, counterID, regRep.RegistryOrRfDescription, "RF", "", regRep.RFId, stato, responsabile, dtaLastPrint));
                            }
                            else
                            {
                                Dt_elem.Add(new ColsRF(descrTipoDocumento, counterID, string.Empty, string.Empty, string.Empty, string.Empty, stato, responsabile, dtaLastPrint));
                            }
                        }
                    }
                    else
                    {
                        Dt_elem.Add(new ColsRF(descrTipoDocumento, counterID, string.Empty, string.Empty, string.Empty, string.Empty, stato, string.Empty, string.Empty));
                    }
                }
                if (Dt_elem.Count > 0)
                {

                    this.DataGrid2.DataSource = Dt_elem;
                    this.DataGrid2.DataBind();
                }
            }

        }

        protected void DataGrid2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.DataGrid2.Items.Count > 0)
            {
                string responsabile = string.Empty;
                string dtaLastPrint = string.Empty;
                string counterId = String.Empty;
                string registryId = String.Empty;
                string rfId = String.Empty;
                responsabile = ((Label)DataGrid2.SelectedItem.Cells[8].Controls[1]).Text;
                dtaLastPrint = ((Label)DataGrid2.SelectedItem.Cells[7].Controls[1]).Text;
                
                counterId = ((Label)DataGrid2.SelectedItem.Cells[0].Controls[1]).Text;
                registryId = ((Label)DataGrid2.SelectedItem.Cells[3].Controls[1]).Text;
                rfId = ((Label)DataGrid2.SelectedItem.Cells[4].Controls[1]).Text;
                
                this.PerformActionSelectElemento(responsabile, dtaLastPrint, counterId, registryId, rfId);
            }
        }

        public class ColsRF
        {
            private string descrTipoDocumento;
            private string contatoreID;
            private string descrRegOrRF;
            private string tipologia;
            private string idReg;
            private string idRf;
            private string stato;
            private string responsabile;
            string dtaLastPrint;
            public ColsRF(string descrTipoDocumento, string contatoreID, string descrRegOrRF, string tipologia, string idReg, string idRf, string stato, string Responsabile, string dtaLastPrint)
            {
                this.descrTipoDocumento = descrTipoDocumento;
                this.contatoreID = contatoreID;
                this.descrRegOrRF = descrRegOrRF;
                this.tipologia = tipologia;
                this.idReg = idReg;
                this.idRf =idRf;
                this.stato = stato;
                this.responsabile = Responsabile;
                this.dtaLastPrint = dtaLastPrint;
            }

            public string DescrizioneTipologia { get { return descrTipoDocumento; } }
            public string ContatoreID { get { return contatoreID; } }
            public string DescrRegOrRF { get { return descrRegOrRF; } }
            public string Tipologia { get { return tipologia; } }
            public string IdReg { get { return idReg; } }
            public string IdRf { get { return idRf; } }
            public string Stato { get { return stato; } }
            public string Responsabile { get { return responsabile; } }
            public string DateLastPrint { get { return dtaLastPrint; } }
        }

        private void PerformActionSelectFirstElemento()
        {
            if (this.DataGrid2.Items.Count > 0)
            {
                string dtaLastPrint = string.Empty;
                string responsabile = string.Empty;
                responsabile = ((Label)this.DataGrid2.Items[0].Cells[8].Controls[1]).Text;
                dtaLastPrint = ((Label)this.DataGrid2.Items[0].Cells[7].Controls[1]).Text;

                String counterId = ((Label)DataGrid2.Items[0].Cells[0].Controls[1]).Text;
                String registryId = ((Label)DataGrid2.Items[0].Cells[3].Controls[1]).Text;
                String rfId = ((Label)DataGrid2.Items[0].Cells[4].Controls[1]).Text;
                
                
                this.PerformActionSelectElemento(responsabile, dtaLastPrint, counterId, registryId, rfId);
                this.DataGrid2.SelectedIndex = 0;
            }
        }

        private void PerformActionSelectElemento(string responsabile, string dtaLastPrint, String counterId, String registryId, String rfId)
        {
            // Se ci sono repertori degli anni passati da stampare, viene visualizzato l'avviso
            RepertorioPrintRange[] ranges = utils.RegistriRepertorioUtils.GetRepertoriPrintRanges(counterId, registryId, rfId);
            if (ranges != null && ranges.Length > 0)
            {
                this.SetSourceForBullettedList(ranges);
                this.pnlAlert.Visible = true;
            }
            else
                this.pnlAlert.Visible = false;

            updateElementoSelezionato(responsabile, dtaLastPrint);
        }

        private void SetSourceForBullettedList(RepertorioPrintRange[] ranges)
        {
            System.Collections.Generic.List<String> docs = new System.Collections.Generic.List<string>();
            foreach (var r in ranges)
                docs.Add(String.Format("Un documento con i repertori dal numero {0} al numero {1} stampati nel {2}",
                    r.FirstNumber, r.LastNumber, r.Year));

            this.blDocList.DataSource = docs;
            this.blDocList.DataBind();
            
        }

        private void updateElementoSelezionato(string responsabile, string dtaLastPrint)
        {
            Response.Write("<script>parent.iFrame_dettaglio.location='regRepertorioDettaglio.aspx?responsabile=" + responsabile + "&dtaLastPrint=" + dtaLastPrint + "';</script>");
        }

        private string GetImageStato(string stato)
        {
            string img = string.Empty;
            if (stato.Equals("Aperto"))
                img = "stato_verde2";
            else if (stato.Equals("Chiuso"))
                img = "stato_rosso2";
            stato = "<img src='../../images/" + img + ".gif' border='0'>";
            return stato;
        }
        #endregion

        #region gestione pulsanti

        protected void btn_stampaRepertori_Click1(object sender, EventArgs e)
        {
            try
            {
                RegistriRepertorioUtils.GeneratePrintRepertorio(UserManager.getRuolo(), 
                    UserManager.getInfoUtente(),
                    ((Label)DataGrid2.SelectedItem.Cells[4].Controls[1]).Text,
                    ((Label)DataGrid2.SelectedItem.Cells[3].Controls[1]).Text,
                    ((Label)DataGrid2.SelectedItem.Cells[0].Controls[1]).Text);
                ClientScript.RegisterStartupScript(this.GetType(), "stampaRepertori", "alert('Stampa repertorio avvenuta con successo.');", true); 
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "stampaRepertori", "alert('" + ex.Message.Replace("'"," ") + "');", true); 
            }
        }

        protected void btn_cambiaStato_Click1(object sender, EventArgs e)
        {
            try
            {
                int selected = DataGrid2.SelectedIndex;
                bool res = RegistriRepertorioUtils.ChangeRepertorioState(((Label)DataGrid2.SelectedItem.Cells[0].Controls[1]).Text,
                    ((Label)DataGrid2.SelectedItem.Cells[3].Controls[1]).Text,
                    ((Label)DataGrid2.SelectedItem.Cells[4].Controls[1]).Text, 
                    UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
                if (res)
                {
                    BindGrid();
                    DataGrid2.SelectedIndex = selected;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
	}
}
