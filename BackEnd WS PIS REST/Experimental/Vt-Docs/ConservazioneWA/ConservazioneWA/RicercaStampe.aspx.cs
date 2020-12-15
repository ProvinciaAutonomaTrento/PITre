using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Drawing;
using System.Globalization;
using ConservazioneWA.Utils;
using ConservazioneWA.DocsPaWR;
using Debugger = ConservazioneWA.Utils.Debugger;

namespace ConservazioneWA
{
    public partial class RicercaStampe : System.Web.UI.Page
    {

        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected WSConservazioneLocale.InfoAmministrazione amm;

        protected List<WSConservazioneLocale.StampaConservazione> listStampe;
        

        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Expires = -1;

            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);


            if (!IsPostBack)
            {

                amm = ConservazioneWA.Utils.ConservazioneManager.GetInfoAmmCorrente(this.infoUtente.idAmministrazione);
                this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;

                this.btnFind.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnFind.Attributes.Add("onmouseout", "this.className='cbtn';");
            }
            else
            {
                //per reload pagina dopo che un'istanza è stata firmata
                //ricarico l'elenco in modo da rimuoverla dall'elenco delle non firmate
                if (this.reloadData.Value == "1")
                {
                    this.reloadData.Value = "0";

                    List<WSConservazioneLocale.FiltroRicerca> filters = this.creaFiltro();
                    //this.dg_StampeCons.CurrentPageIndex = 0;
                    LoadData(filters);
                }
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            if (ValidaFiltri())
            {
                List<WSConservazioneLocale.FiltroRicerca> filters = this.creaFiltro();

                this.dg_StampeCons.CurrentPageIndex = 0;
                LoadData(filters);
            }

        }

        /// <summary>
        /// Produce una lista di filtri su cui eseguire la ricerca
        /// </summary>
        /// <returns></returns>
        protected List<WSConservazioneLocale.FiltroRicerca> creaFiltro()
        {

            List<WSConservazioneLocale.FiltroRicerca> filters = new List<WSConservazioneLocale.FiltroRicerca>();

            //filtro su firma
            switch (this.radFirmate.SelectedItem.Value)
            {
                case "0":
                    filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "FIRMATE", valore ="0" });
                    break;
                case "1":
                    filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "FIRMATE", valore = "1" });
                    break;
            }

  
            //FILTRO SU DATA
            //intervallo
            if(this.ddl_dataStampa.SelectedIndex == 1)
            {
                if (this.dataStampa_da.Text != "")
                    filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "DATA_STAMPA_DA", valore = dataStampa_da.Text });
              
                if (this.dataStampa_a.Text != "") 
                    filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "DATA_STAMPA_A", valore = dataStampa_a.Text });
            }

            //valore singolo
            if(this.ddl_dataStampa.SelectedIndex == 0)
            {
                if (this.dataStampa_da.Text != "")
                    filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "DATA_STAMPA_IL", valore = dataStampa_da.Text });
            }

            //oggi
            if(this.ddl_dataStampa.SelectedIndex == 2) 
            {                
                filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento="DATA_STAMPA_TODAY", valore = "1" });
            }

            //settimana corrente
            if (this.ddl_dataStampa.SelectedIndex == 3)
            {
                filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "DATA_STAMPA_SC", valore = "1" });
            }

            //mese corrente
            if (this.ddl_dataStampa.SelectedIndex == 4)
            {
                filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "DATA_STAMPA_MC", valore = "1" });
            }


            //filtro su amministrazione
            filters.Add(new WSConservazioneLocale.FiltroRicerca { argomento = "ID_AMM", valore = infoUtente.idAmministrazione });

            return filters;
        }

        protected void LoadData(List<WSConservazioneLocale.FiltroRicerca> filters)
        {

            //leggo i dati da db
            listStampe = ConservazioneManager.GetListStampaConservazione(filters, infoUtente);

            //popolo la datagrid
            dg_StampeCons.DataSource = listStampe;
            dg_StampeCons.DataBind();

            this.upStampe.Update();

        }

        protected void ddl_dataStampa_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            this.dataStampa_da.Text = string.Empty;
            this.dataStampa_a.Text = string.Empty;

            switch (this.ddl_dataStampa.SelectedIndex)
            {
                //valore singolo
                case 0:
                    this.labelA.Visible = false;
                    this.labelA.Text = string.Empty;
                    this.dataStampa_a.Visible = false;
                    this.dataStampa_a.Text = string.Empty;

                    this.labelDa.Text = "Il";
                    this.labelDa.Visible = true;
                    this.dataStampa_da.Visible = true;

                    break;

                //intervallo
                case 1:
                    this.labelA.Text = "a";
                    this.labelA.Visible = true;
                    this.dataStampa_a.Visible = true;

                    this.labelDa.Text = "Da";
                    this.labelDa.Visible = true;
                    this.dataStampa_da.Visible = true;

                    break;

                //oggi, settimana corrente, mese corrente
                case 2:
                case 3:
                case 4:
                    this.labelA.Visible = false;
                    this.labelA.Text = string.Empty;
                    this.labelDa.Visible = false;
                    this.labelDa.Text = string.Empty;
                    this.dataStampa_a.Visible = false;
                    this.dataStampa_da.Visible = false;

                    break;
                
            }

            //this.upFiltriRicerca.Update();
        }


        protected void SelectedIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            this.dg_StampeCons.CurrentPageIndex = e.NewPageIndex;
            LoadData(this.creaFiltro());

            this.upStampe.Update();

        }

        
        protected void dg_StampeCons_PreRender(object sender, EventArgs e)
        {
          
            foreach (DataGridItem itm in dg_StampeCons.Items)
            {
                string idDoc = itm.Cells[0].Text;
                bool isFirmato = itm.Cells[1].Text == "1" ? true : false;
                Label lblFirma = (Label)itm.FindControl("lbl_firma");
                if (isFirmato)
                    lblFirma.Text = "firmata";
                else
                    lblFirma.Text = "non firmata";
                ImageButton btnFirma = (ImageButton)itm.FindControl("btn_firma");
                if(btnFirma!=null)
                    btnFirma.OnClientClick = String.Format("return showDialogFirma('{0}','{1}');", idDoc, isFirmato);
            }
        }

        protected void BtnFirmaStampa_Click(object sender, EventArgs e)
        {

        }

        protected static bool isDate(string data)
        {
            try
            {
                data = data.Trim();
                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                DateTime d_ap = DateTime.ParseExact(data, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected bool ValidaFiltri()
        {
            //filtro DA
            if (!this.dataStampa_da.txt_Data.Text.Equals(string.Empty))
            {
                if (!isDate(this.dataStampa_da.txt_Data.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alertDa", "alert('Formato data non valido. \\nIl formato richiesto è gg/mm/aaaa.');", true);
                    return false;
                }
            }

            //filtro A
            if (!this.dataStampa_a.txt_Data.Text.Equals(string.Empty))
            {
                if (!isDate(this.dataStampa_a.txt_Data.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alertA", "alert('Formato data non valido. \\nIl formato richiesto è gg/mm/aaaa.');", true);
                    return false;
                }
            }

            return true;
           
        }
        
    }
}