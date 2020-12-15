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
using System.Collections.Generic;
using DocsPAWA.DocsPaWR;
using System.Linq;
using DocsPAWA.utils;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for sceltaTipoSpedizione.
	/// </summary>
    public class sceltaTipoSpedizione : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Label LabelCodice;
        //protected System.Web.UI.WebControls.DropDownList ddl_tipoSped;
        protected System.Web.UI.WebControls.DropDownList ddlM;
        protected System.Web.UI.WebControls.DropDownList ddlTutti;
		//protected System.Web.UI.WebControls.Label Label;
        protected System.Web.UI.WebControls.DataGrid dataGridMezzi;
        List<Mezzi> listaMezzoSpedizione = new List<Mezzi>();
		//DocsPaWR.Corrispondente corrCorrente;
		DocsPaWR.SchedaDocumento schedaDoc;
        protected string editMode = string.Empty; 

		private void Page_Load(object sender, System.EventArgs e)
		{
            //verifico se il documento è in edit
            editMode = Request.QueryString["editMode"];
            if (Session["abilitaModificaSpedizione"] != null && (bool)Session["abilitaModificaSpedizione"])
                editMode = "true";

            //preparo il datasource con i mezzi trasmissione
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            string idAmm = UserManager.getInfoUtente().idAmministrazione;
            DocsPAWA.DocsPaWR.MezzoSpedizione[] m_sped = ws.AmmListaMezzoSpedizione(idAmm, false);
            foreach (DocsPAWA.DocsPaWR.MezzoSpedizione m in m_sped)
            {
                // Il mezzo per l'interoperabilità semplificata può essere inserito solo se è attiva
                if (m.chaTipoCanale == "S")
                {
                    if (InteroperabilitaSemplificataManager.IsEnabledSimpInterop)
                        listaMezzoSpedizione.Add(new Mezzi(m.Descrizione, m.IDSystem));
                }
                else
                    listaMezzoSpedizione.Add(new Mezzi(m.Descrizione, m.IDSystem));

            }
            Response.Expires = -1;
			if (!this.IsPostBack)
			{
                //setto il mezzo di spedizione per tutti
                ddlTutti.Items.Add(new ListItem("", "0"));
                if (listaMezzoSpedizione != null)
                {
                    foreach (Mezzi m in listaMezzoSpedizione)
                        ddlTutti.Items.Add(new ListItem(m.Descrizione, m.Valore));
                    if (editMode.ToUpper().Equals("FALSE"))
                        ddlTutti.Enabled = false;
                }

                BindGrid();

                if (editMode.ToUpper().Equals("FALSE"))
                    btn_ok.Enabled = false;
			}
            (FindControl("div1") as HtmlGenericControl).Style.Remove("overflow-y");
            if (dataGridMezzi != null && dataGridMezzi.Items != null && dataGridMezzi.Items.Count > 7)
                (FindControl("div1") as HtmlGenericControl).Style.Add("overflow-y", "scroll");
        }

        public void BindGrid()
        {
            //preparo i destinatari/destinatari cc
            List<Cols> columns = new List<Cols>();
            schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
            DocsPaWR.ProtocolloUscita protocollo = (DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo;
            DocsPaWR.Corrispondente[] listaCorrispondenti;
            DocsPaWR.Corrispondente[] listaCorrispondentiCc;
            listaCorrispondenti = protocollo.destinatari;
            listaCorrispondentiCc = protocollo.destinatariConoscenza;
            if (listaCorrispondenti != null)
            {
                foreach (DocsPaWR.Corrispondente c in listaCorrispondenti)
                {
                    if (!string.IsNullOrEmpty(c.systemId))
                    {
                        Canale canalePreferenz = UserManager.getCorrispondenteBySystemID(this.Page, c.systemId).canalePref;
                        if (canalePreferenz != null && (!string.IsNullOrEmpty(canalePreferenz.descrizione)))
                            columns.Add(new Cols("(" + canalePreferenz.descrizione + ") " + c.descrizione, c.systemId));
                        else
                            columns.Add(new Cols(c.descrizione, c.systemId));
                    }
                }
            }
            if (listaCorrispondentiCc != null)
            {
                foreach (DocsPaWR.Corrispondente c in listaCorrispondentiCc)
                {
                    if (!string.IsNullOrEmpty(c.systemId))
                    {
                        Canale canalePreferenz = UserManager.getCorrispondenteBySystemID(this.Page, c.systemId).canalePref;
                        if (canalePreferenz != null && (!string.IsNullOrEmpty(canalePreferenz.descrizione)))

                            columns.Add(new Cols("(Cc)(" + c.canalePref.descrizione + ") " + c.descrizione, c.systemId));
                        else
                            columns.Add(new Cols("(Cc)  " + c.descrizione, c.systemId));
                    }
                }

            }
            dataGridMezzi.DataSource = columns;
            dataGridMezzi.DataBind();
        }

        protected void dataGridMezzi_RowDataBound(object sender, DataGridItemEventArgs e)
        {
                DocsPaWR.Corrispondente[] dest = ((DocsPAWA.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).destinatari;
			    DocsPaWR.Corrispondente[] destCc = ((DocsPAWA.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).destinatariConoscenza;
                //id del corrispondente
                Label idCorr = (Label)e.Item.FindControl("lblId");
                //popola la dropddownlist  
                DropDownList ddl1 = (DropDownList)e.Item.FindControl("ddlM");
                Canale canale= null, canaleOrig = null;
                string tipoDest = string.Empty;

                if (ddl1 != null)
                {
                    //leggo il canale in sessione associato al corrispondente
                    if (dest != null && dest.Length > 0)
                    {
                        foreach (Corrispondente d in dest)
                        {
                            if (!string.IsNullOrEmpty(d.systemId) && d.systemId.Equals(idCorr.Text))
                            {
                                canale = d.canalePref;
                                tipoDest = "d";
                                break;
                            }
                        }
                    }
                    if (destCc != null && destCc.Length > 0 && (!tipoDest.Equals("d")))
                    {
                        foreach (Corrispondente d in destCc)
                        {
                            if (!string.IsNullOrEmpty(d.systemId) && d.systemId.Equals(idCorr.Text))
                            {
                                canale = d.canalePref;
                                break;
                            }
                        }
                    }

                    ddl1.Items.Add(new ListItem("", "0"));
                    if (listaMezzoSpedizione != null)
                    {
                        if (UserManager.getCorrispondenteBySystemID(this.Page, idCorr.Text).tipoIE == null ||
                            UserManager.getCorrispondenteBySystemID(this.Page, idCorr.Text).tipoIE.Equals("I"))
                            ddl1.Enabled = false;
                        else
                        {
                            canaleOrig = UserManager.getCorrispondenteBySystemID(this.Page, idCorr.Text).canalePref;
                            List<Mezzi> filteredMeans = GetMeansDeliveryFiltered(canaleOrig, idCorr.Text);
                            if (filteredMeans != null && filteredMeans.Count > 0)
                            {
                                foreach (Mezzi m in filteredMeans)

                                {
                                    ddl1.Items.Add(new ListItem(m.Descrizione, m.Valore));
                                    if (canale != null && canale.systemId != null)
                                    {
                                        if (canaleOrig == null || (canaleOrig != null && canaleOrig.systemId != null && canaleOrig.systemId != canale.systemId))
                                        {
                                            if (m.Valore.Equals(canale.systemId))
                                            {
                                                ddl1.SelectedValue = canale.systemId;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (editMode.ToUpper().Equals("FALSE"))
                            ddl1.Enabled = false;
                    }
                }
        }

        private void dataGridMezzi_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {

            this.dataGridMezzi.CurrentPageIndex = e.NewPageIndex;
            ArrayList ColNew = new ArrayList();
            ColNew = (ArrayList)DocumentManager.getDataGridNotifiche(this);

            dataGridMezzi.DataSource = ColNew;
            dataGridMezzi.DataBind();
        }

        protected void dataGridMezzi_SelectedPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {

            this.dataGridMezzi.CurrentPageIndex = e.NewPageIndex;
            ArrayList ColNew = new ArrayList();
            ColNew = (ArrayList)DocumentManager.getDataGridNotifiche(this);

            dataGridMezzi.DataSource = ColNew;
            dataGridMezzi.DataBind();
        }

        public class Mezzi
        {
            private string descrizione;
            private string valore;
            public Mezzi(string descrizione, string valore)
            {
                this.descrizione = descrizione;
                this.valore = valore;
            }
            public string Descrizione { get { return descrizione; } }
            public string Valore { get { return valore; } }
        }

        public class Cols
        {
            private string descrizione;
            private string corrId;
            public Cols(string descrizione, string corrId)
            {
                this.descrizione = descrizione;
                this.corrId = corrId;
            }
            public string Descrizione { get { return descrizione; } }
            public string Id { get { return corrId; } }
        }

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
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);
            this.dataGridMezzi.ItemDataBound+= new DataGridItemEventHandler(this.dataGridMezzi_RowDataBound);
		}
		#endregion

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Response.Write("<script>window.close();</script>");	
		}

        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            string idCorr = string.Empty;
            string idCanale = string.Empty;
            bool acceptCanaleTutti;
            DocsPaWR.Canale canale = new DocsPAWA.DocsPaWR.Canale();
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
            DocsPaWR.Corrispondente[] dest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari;
            DocsPaWR.Corrispondente[] destCc = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza;

            if (dataGridMezzi != null)
            {
                Canale canaleTutti = null;
                if (!ddlTutti.SelectedValue.Equals("0"))
                    canaleTutti = ws.getCanaleBySystemId(ddlTutti.SelectedValue);
                DataGridItemCollection items = dataGridMezzi.Items;
                if (items != null)
                {
                    //itero sui singoli item della griglia
                    foreach (DataGridItem item in items)
                    {
                        string tipoDest = string.Empty;
                        TableCellCollection cells = item.Cells;
                        foreach (TableCell cell in cells)
                        {
                            ControlCollection controls = cell.Controls;
                            foreach (Control control in controls)
                            {
                                if (control.GetType() == typeof(Label))
                                {
                                    idCorr = ((Label)control).Text;
                                    break;
                                }

                                if (control.GetType() == typeof(DropDownList))
                                {
                                    idCanale = ((DropDownList)control).SelectedValue;
                                    break;

                                }
                            }
                        }

                        if (dest != null && dest.Length > 0)
                        {
                            acceptCanaleTutti = false;
                            foreach (DocsPaWR.Corrispondente corr in dest)
                            {
                                // se è impostato il canale tutti ed il corrispondente corrente ha visibilità del mezzo di spedizione tutti allora lo imposto
                                if (corr.systemId.Equals(idCorr) && canaleTutti != null)
                                {
                                    System.Collections.Generic.List<Mezzi> listMezziVisCorr = GetMeansDeliveryFiltered(UserManager.getCorrispondenteBySystemID(this.Page, idCorr).canalePref, idCorr);
                                    foreach (Mezzi m in listMezziVisCorr)
                                    {
                                        if (m.Descrizione.Equals(canaleTutti.descrizione) && m.Valore.Equals(canaleTutti.systemId))
                                        {
                                            acceptCanaleTutti = true;
                                            corr.canalePref = canaleTutti;
                                            tipoDest = "d";
                                            break;
                                        }
                                    }
                                }

                                //l'utente a selezionata blank quindi reimposto il canale di default
                                if (!acceptCanaleTutti && corr.systemId.Equals(idCorr) && (idCanale.Equals("0")))
                                {
                                    if (corr.canalePref != null && corr.canalePref.systemId != null)
                                    {
                                        canale = UserManager.getCorrispondenteBySystemID(this.Page, idCorr).canalePref;
                                        corr.canalePref = canale;
                                        tipoDest = "d";
                                        break;
                                    }
                                }
                                //imposto il canale selezionato dall'utente
                                else if (!acceptCanaleTutti && corr.systemId.Equals(idCorr))
                                {
                                    canale = ws.getCanaleBySystemId(idCanale);
                                    corr.canalePref = canale;
                                    tipoDest = "d";
                                    break;
                                }
                            }
                        }
                        if (destCc != null && destCc.Length > 0 && (!tipoDest.Equals("d")))
                        {
                            acceptCanaleTutti = false;
                            foreach (DocsPaWR.Corrispondente corr in destCc)
                            {
                                // se è impostato il canale tutti ed il corrispondente corrente ha visibilità del mezzo di spedizione tutti allora lo imposto
                                if (corr.systemId.Equals(idCorr) && canaleTutti != null)
                                {
                                    System.Collections.Generic.List<Mezzi> listMezziVisCorr = GetMeansDeliveryFiltered(UserManager.getCorrispondenteBySystemID(this.Page, idCorr).canalePref, idCorr);
                                    foreach (Mezzi m in listMezziVisCorr)
                                    {
                                        if (m.Descrizione.Equals(canaleTutti.descrizione) && m.Valore.Equals(canaleTutti.systemId))
                                        {
                                            acceptCanaleTutti = true;
                                            corr.canalePref = canaleTutti;
                                            tipoDest = "d";
                                            break;
                                        }
                                    }
                                }

                                //l'utente a selezionata blank quindi reimposto il canale di default
                                if (!acceptCanaleTutti && corr.systemId.Equals(idCorr) && (idCanale.Equals("0")))
                                {
                                    if (corr.canalePref != null && corr.canalePref.systemId != null)
                                    {
                                        canale = UserManager.getCorrispondenteBySystemID(this.Page, idCorr).canalePref;
                                        corr.canalePref = canale;
                                        break;
                                    }
                                }
                                //imposto il canale selezionato dall'utente
                                else if (!acceptCanaleTutti && corr.systemId.Equals(idCorr))
                                {
                                    canale = ws.getCanaleBySystemId(idCanale);
                                    corr.canalePref = canale;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //Salvo le informazioni aggiornate nella sessione
            ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatari = dest;
            ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = destCc;
            DocumentManager.setDocumentoInLavorazione(this, schedaDoc);
            DocumentManager.setDocumentoSelezionato(this, schedaDoc);

            //richiama la funzione javascript che aggiorna il form chiamante
            string funct = " window.open('../documento/docProtocollo.aspx?editMode=true','IframeTabs'); ";
            funct = funct + " window.close(); ";
            Response.Write("<script> " + funct + "</script>");
        }

        private List<Mezzi> GetMeansDeliveryFiltered(Canale canaleOrig, string idDest)
        {
            DocsPaWR.DocsPaWebService wws = new DocsPaWebService();
            DocsPAWA.DocsPaWR.MezzoSpedizione[] m_sped = wws.AmmListaMezzoSpedizione(UserManager.getInfoUtente().idAmministrazione, false);
            List<Mezzi> filteredMeans = new List<Mezzi>();
            Corrispondente corr = UserManager.getCorrispondenteBySystemID(this.Page, idDest);
            if (canaleOrig != null)
            {
                switch (canaleOrig.typeId.ToUpper())
                {

                    case "INTEROPERABILITA":
                        foreach (MezzoSpedizione m in m_sped)
                        {
                            // Il canale interoperabilità semplificata non può essere inserito se il corrispondente non
                            // ha URL mentre il canale mail non deve essere inserito
                            if((m.chaTipoCanale == "S" && InteroperabilitaSemplificataManager.IsEnabledSimpInterop &&
                                corr.Url != null && corr.Url.Length > 0 &&
                                Uri.IsWellFormedUriString(corr.Url[0].Url, UriKind.Absolute)) ||
                                (m.chaTipoCanale != "S" && !m.Descrizione.ToUpper().Equals("MAIL")))
                                filteredMeans.Add(new Mezzi(m.Descrizione, m.IDSystem));
                        }
                        break;

                    case "MAIL":
                        foreach (MezzoSpedizione m in m_sped)
                        {
                            if (!m.Descrizione.ToUpper().Equals("INTEROPERABILITA") && m.chaTipoCanale != "S")
                            {
                                filteredMeans.Add(new Mezzi(m.Descrizione, m.IDSystem));
                            }
                            else
                            {
                                if (m.chaTipoCanale == "S")
                                { 
                                    if(InteroperabilitaSemplificataManager.IsEnabledSimpInterop &&
                                        corr.Url != null && corr.Url.Length > 0 &&
                                        Uri.IsWellFormedUriString(corr.Url[0].Url, UriKind.Absolute))
                                        filteredMeans.Add(new Mezzi(m.Descrizione, m.IDSystem));
                                    
                                }
                                else
                                    //Verifico che siano presenti i campi obbligatori per l'utente con canale preferito INTEROPERABILITA
                                    if (!string.IsNullOrEmpty(corr.codiceAmm) && (!string.IsNullOrEmpty(corr.codiceAOO)) && (!string.IsNullOrEmpty(corr.email)))
                                    {
                                        filteredMeans.Add(new Mezzi(m.Descrizione, m.IDSystem));
                                    }
                            }
                        }
                        break;

                    default:
                        this.FilterInteroperability(m_sped, corr, ref filteredMeans);

                        break;
                }
            }
            return filteredMeans;
        }

        /// <summary>
        /// Metodo per il filtraggio dei mezzi di spedizione relativi all'interoperabilità. Questo metodo
        /// non contente di inserire nella combo dei mezzi di spedizione, Interoperabilità e 
        /// Interoperabilità Semplificata se non sono soddisfatti i requisiti
        /// </summary>
        /// <param name="m_sped">Lista dei mezzi di spedizione</param>
        /// <param name="corr">Corrispondente selezionato</param>
        private void FilterInteroperability(MezzoSpedizione[] m_sped, Corrispondente corr, ref List<Mezzi> filteredMeans)
        {
            
            // Prelevamento mezzi di interoperabilità e di tutti quelli che non riguardano l'interoperabilità
            var interop = m_sped.Where(m => m.Descrizione == "INTEROPERABILITA" || m.Descrizione == "MAIL" || m.chaTipoCanale == "S");
            var other = m_sped.Except(interop);

            // Tutti gli altri elementi vengono aggiunti alla lista dei mezzi filtrati
            foreach (var mezzo in other)
                filteredMeans.Add(new Mezzi(mezzo.Descrizione, mezzo.IDSystem));

            // L'interoperabilità classica può essere utilizzata solo se sono valorizzati tutti i campi obbligatori
            var classic = interop.Where(m => m.Descrizione == "INTEROPERABILITA").FirstOrDefault();
            if (classic != null && !string.IsNullOrEmpty(corr.codiceAmm) &&
                (!string.IsNullOrEmpty(corr.codiceAOO)) && (!string.IsNullOrEmpty(corr.email)))
                filteredMeans.Add(new Mezzi(classic.Descrizione, classic.IDSystem));

            var mail = interop.Where(m => m.Descrizione == "MAIL").FirstOrDefault();
            if (mail != null && !String.IsNullOrEmpty(corr.email))
                filteredMeans.Add(new Mezzi(mail.Descrizione, mail.IDSystem));

            var simpInterop = interop.Where(m => m.chaTipoCanale == "S").FirstOrDefault();
            if (simpInterop != null && InteroperabilitaSemplificataManager.IsEnabledSimpInterop &&
                corr.Url != null && corr.Url.Length > 0 &&
                Uri.IsWellFormedUriString(corr.Url[0].Url, UriKind.Absolute))
                filteredMeans.Add(new Mezzi(simpInterop.Descrizione, simpInterop.IDSystem));

        }
	}
}
