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

namespace SAAdminTool.AdminTool.Gestione_ProfDinamica
{
	/// <summary>
	/// Summary description for AssociaModelliTrasm.
	/// </summary>
	public class AssociaModelliTrasm : System.Web.UI.Page
	{
		private string idAmministrazione;
		protected System.Web.UI.WebControls.Button btn_conferma;
		protected System.Web.UI.WebControls.DataGrid dg_ModelliTrasm;
		private ArrayList modelliTrasmissioneApp;
		private ArrayList modelliTrasmissione;
		private ArrayList modelliTrasmAssociati;
		private string idTipoDoc = "";
		private string idDiagramma = "";
		protected System.Web.UI.WebControls.Label lbl_titolo;
		private string idStato = "";
        protected System.Web.UI.WebControls.DropDownList ddl_ricTipo;
        protected System.Web.UI.WebControls.TextBox txt_ricerca;
        protected System.Web.UI.WebControls.Button btn_find;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_codice;
        private Hashtable HTmodelli;

		private void Page_Load(object sender, System.EventArgs e)
		{
            this.Page.MaintainScrollPositionOnPostBack = true;

			if(Session["idTipoDoc"] != null)
				idTipoDoc = (string) Session["idTipoDoc"];
			
			dg_ModelliTrasm.Columns[3].Visible = false;

			if(Session["idDiagramma"] != null)
			{
				idDiagramma = (string) Session["idDiagramma"];
				if(idDiagramma != "0")
				{
					lbl_titolo.Text = "Modelli di Trasmissione validi per stato non valorizzato";					
				}
				else
				{
                    string descrizioneStato = (string)Session["descrStato"];
                    lbl_titolo.Text = "Stato: " + descrizioneStato;
					//lbl_titolo.Text = "Modelli di Trasmissione ";					
				}
			}

			if(Session["idStato"] != null)
			{
				idStato = (string) Session["idStato"];
                lbl_titolo.Text = "Stato: " + (string)Session["descrStato"];
				dg_ModelliTrasm.Columns[3].Visible = true;
			}

            if (!IsPostBack)
            {
                Session.Add("reloadHT", false);
                this.Inizialize();
            }
            else
            {
                this.impostaSelezione();
            }
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
			this.btn_conferma.Click += new System.EventHandler(this.btn_conferma_Click);
            this.ddl_ricTipo.SelectedIndexChanged += new System.EventHandler(this.ddl_ricTipo_SelectedIndexChanged);
            this.btn_find.Click += new System.EventHandler(this.btn_find_Click);       
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void caricaDgModelliTrasm()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("SYSTEM_ID");
			dt.Columns.Add("DESCRIZIONE");
            for (int i = 0; i < modelliTrasmissione.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione modello = (SAAdminTool.DocsPaWR.ModelloTrasmissione)modelliTrasmissione[i];
                DataRow rw = dt.NewRow();
                rw[0] = modello.SYSTEM_ID;
                rw[1] = modello.NOME;
                dt.Rows.Add(rw);
            }
			dt.AcceptChanges();
			dg_ModelliTrasm.DataSource = dt;
			dg_ModelliTrasm.DataBind();
		}

		private void impostaSelezioneModelliTrasmAssociati()
		{
            //for(int i=0; i<modelliTrasmAssociati.Count; i++)
            //{
            //    //string idModello = (string) idModelliTrasmAssociati[i];				
            //    DocsPaWR.AssDocDiagTrasm obj = (SAAdminTool.DocsPaWR.AssDocDiagTrasm) modelliTrasmAssociati[i];		
            //    for(int j=0; j<dg_ModelliTrasm.Items.Count; j++)
            //    {
            //        //Imposto le selezioni
            //        if(obj.ID_TEMPLATE == dg_ModelliTrasm.Items[j].Cells[0].Text)
            //            ((CheckBox) dg_ModelliTrasm.Items[j].Cells[2].Controls[1]).Checked = true;
            //        if(obj.ID_TEMPLATE == dg_ModelliTrasm.Items[j].Cells[0].Text && obj.TRASM_AUT == "1")
            //            ((CheckBox) dg_ModelliTrasm.Items[j].Cells[3].Controls[1]).Checked = true;					
            //    }
            //}
            //this.HTmodelli = new Hashtable();
            HTmodelli = (Hashtable)Session["hashtableModelli"];
            for (int i = 0; i < dg_ModelliTrasm.Items.Count; i++)
            {
                if (HTmodelli.Count != 0)
                {
                    if (HTmodelli.ContainsKey(dg_ModelliTrasm.Items[i].Cells[0].Text))
                    {
                        ModelliTrasmHT m;
                        m = (ModelliTrasmHT)HTmodelli[dg_ModelliTrasm.Items[i].Cells[0].Text];
                        if (m.Ins == "0" && m.Ric == "0")
                        {
                            ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Checked = false;
                            ((CheckBox)dg_ModelliTrasm.Items[i].Cells[3].Controls[1]).Checked = false;
                            ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Enabled = true;
                        }
                        if (m.Ins == "1" && m.Ric == "1")
                        {
                            ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Checked = true;
                            ((CheckBox)dg_ModelliTrasm.Items[i].Cells[3].Controls[1]).Checked = true;
                            ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Enabled = false;
                        }
                        if (m.Ins == "1" && m.Ric == "0")
                        {
                            ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Checked = true;
                            ((CheckBox)dg_ModelliTrasm.Items[i].Cells[3].Controls[1]).Checked = false;
                            ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Enabled = true;
                        }
                    }
                }
            }


		}

		private void impostaAbilitazioneSelezioneModelli()
		{
			for(int j=0; j<dg_ModelliTrasm.Items.Count; j++)
			{
				//Imposto le abilitazione della checkbox
				if( !((CheckBox) dg_ModelliTrasm.Items[j].Cells[3].Controls[1]).Checked)
					((CheckBox) dg_ModelliTrasm.Items[j].Cells[2].Controls[1]).Enabled = true;

				if( ((CheckBox) dg_ModelliTrasm.Items[j].Cells[3].Controls[1]).Checked)
				{
					((CheckBox) dg_ModelliTrasm.Items[j].Cells[2].Controls[1]).Enabled = false;
					((CheckBox) dg_ModelliTrasm.Items[j].Cells[2].Controls[1]).Checked = true;
				}				
			}
		}

		private void btn_conferma_Click(object sender, System.EventArgs e)
		{
            Hashtable hashModelli = (Hashtable)Session["hashtableModelli"];             
			ArrayList modelliSelezionati = new ArrayList();
            foreach (string chiave in hashModelli.Keys)
            {
                ModelliTrasmHT m = (ModelliTrasmHT)hashModelli[chiave];
                if (m.Ins == "1")
                {
                    DocsPaWR.AssDocDiagTrasm obj = new SAAdminTool.DocsPaWR.AssDocDiagTrasm();
                    obj.ID_DIAGRAMMA = idDiagramma;
                    obj.ID_TIPO_DOC = idTipoDoc;
                    obj.ID_STATO = idStato;

                    obj.ID_TEMPLATE = chiave;
                    if (m.Ric == "1")
                        obj.TRASM_AUT = "1";
                    else
                        obj.TRASM_AUT = "0";
                    modelliSelezionati.Add(obj);
                }
            }
			DocsPaWR.AssDocDiagTrasm [] modelliSelezionati_1 = new SAAdminTool.DocsPaWR.AssDocDiagTrasm[modelliSelezionati.Count];
			modelliSelezionati.CopyTo(modelliSelezionati_1);
            ProfilazioneDocManager.salvaAssociazioneModelli(idTipoDoc,idDiagramma,modelliSelezionati_1,idStato, this);
			RegisterStartupScript("ChiudiAssociazioneModelli","<script>window.close()</script>");
            Session.Remove("reloadHT");
            Session.Remove("modelliTrasmissione");
            Session.Remove("modelliTrasmAssociati");
            Session.Remove("hashtableModelli");
		}

        protected void ddl_ricTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.hd_codice.Value = "0";
            if (this.ddl_ricTipo.SelectedItem.Value.Equals("T"))
            {
                this.txt_ricerca.Enabled = false;
                this.txt_ricerca.Text = "";
                Session["reloadHT"] = true;
                this.Inizialize();

            }
            else
            {
                if (ddl_ricTipo.SelectedItem.Value == "C")
                    this.hd_codice.Value = "1";
                if (this.ddl_ricTipo.SelectedItem.Value.Equals("S") || this.ddl_ricTipo.SelectedItem.Value.Equals("U"))
                {
                    this.txt_ricerca.Enabled = false;
                    this.txt_ricerca.Text = "";
                    this.cercaModelliSelezionati();
                }
                else
                {
                    this.txt_ricerca.Enabled = true;
                }
            }
        }

        protected void btn_find_Click(object sender, EventArgs e)
        {
            if (this.ddl_ricTipo.SelectedItem.Value.Equals("T"))
            {
                Session["reloadHT"] = true;
                this.Inizialize();
            }
            else
            {
                if (!this.ddl_ricTipo.SelectedItem.Value.Equals("S") && !this.ddl_ricTipo.SelectedItem.Value.Equals("U"))
                {
                    string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                    string codiceAmministrazione = amministrazione[0];
                    idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);
                    string codiceRicerca = txt_ricerca.Text;
                    bool isNumber = true;
                    if (this.ddl_ricTipo.SelectedItem.Value == "C")
                    {
                        if (codiceRicerca != string.Empty)
                            codiceRicerca = codiceRicerca.Substring(3);
                    }
                    modelliTrasmissioneApp = new ArrayList(ModelliTrasmManager.getModelliByAmmConRicerca(idAmministrazione, codiceRicerca, this.ddl_ricTipo.SelectedItem.Value, this));
                    modelliTrasmissione = new ArrayList();
                    for (int i = 0; i < modelliTrasmissioneApp.Count; i++)
                    {
                        DocsPaWR.ModelloTrasmissione modello = (SAAdminTool.DocsPaWR.ModelloTrasmissione)modelliTrasmissioneApp[i];
                        if (modello.CHA_TIPO_OGGETTO == "D")
                            modelliTrasmissione.Add(modelliTrasmissioneApp[i]);
                    }
                    Session.Add("modelliTrasmissione", modelliTrasmissione);

                    modelliTrasmAssociati = new ArrayList(ProfilazioneDocManager.getIdModelliTrasmAssociati(idTipoDoc, idDiagramma, idStato, this));
                    Session.Add("modelliTrasmAssociati", modelliTrasmAssociati);

                    caricaDgModelliTrasm();
                    if (modelliTrasmAssociati.Count != 0)
                        impostaSelezioneModelliTrasmAssociati();

                    //impostaAbilitazioneSelezioneModelli();
                    impostaSelezione();
                }
                else
                {
                    cercaModelliSelezionati();
                }
            }
        }

        private void cercaModelliSelezionati()
        {
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);

            if (this.ddl_ricTipo.SelectedItem.Value.Equals("S"))
                modelliTrasmissioneApp = new ArrayList(ModelliTrasmManager.getModelliAssDiagrammi(idTipoDoc, idDiagramma, idStato, idAmministrazione, true, "D", this));
            else
                modelliTrasmissioneApp = new ArrayList(ModelliTrasmManager.getModelliAssDiagrammi(idTipoDoc, idDiagramma, idStato, idAmministrazione, false, "D", this));

            modelliTrasmissione = new ArrayList();
            for (int i = 0; i < modelliTrasmissioneApp.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione modello = (SAAdminTool.DocsPaWR.ModelloTrasmissione)modelliTrasmissioneApp[i];
                if (modello.CHA_TIPO_OGGETTO == "D")
                    modelliTrasmissione.Add(modelliTrasmissioneApp[i]);
            }
            Session.Add("modelliTrasmissione", modelliTrasmissione);

            modelliTrasmAssociati = new ArrayList(ProfilazioneDocManager.getIdModelliTrasmAssociati(idTipoDoc, idDiagramma, idStato, this));
            Session.Add("modelliTrasmAssociati", modelliTrasmAssociati);

            caricaDgModelliTrasm();
            if (modelliTrasmAssociati.Count != 0)
                impostaSelezioneModelliTrasmAssociati();

            //impostaAbilitazioneSelezioneModelli();
            impostaSelezione();
        }


        private void Inizialize()
        {
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);
            modelliTrasmissioneApp = new ArrayList(ModelliTrasmManager.getModelliByAmm(idAmministrazione, this));
            modelliTrasmissione = new ArrayList();
            for (int i = 0; i < modelliTrasmissioneApp.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione modello = (SAAdminTool.DocsPaWR.ModelloTrasmissione)modelliTrasmissioneApp[i];
                if (modello.CHA_TIPO_OGGETTO == "D")
                    modelliTrasmissione.Add(modelliTrasmissioneApp[i]);
            }
            Session.Add("modelliTrasmissione", modelliTrasmissione);

            modelliTrasmAssociati = new ArrayList(ProfilazioneDocManager.getIdModelliTrasmAssociati(idTipoDoc, idDiagramma, idStato, this));
            Session.Add("modelliTrasmAssociati", modelliTrasmAssociati);

            bool reloadHT = (Boolean)Session["reloadHT"];
            if (!reloadHT)
                caricaHTModelliTrasm();

            caricaDgModelliTrasm();

            if (modelliTrasmAssociati.Count != 0)
                impostaSelezioneModelliTrasmAssociati();
            
            //impostaAbilitazioneSelezioneModelli();	
        }

        private void caricaHTModelliTrasm()
        {
            modelliTrasmissione = (ArrayList)Session["modelliTrasmissione"];
            modelliTrasmAssociati = (ArrayList)Session["modelliTrasmAssociati"];

            this.HTmodelli = new Hashtable();
            ModelliTrasmHT m = new ModelliTrasmHT();
            bool modelloSel;

            for (int i = 0; i < modelliTrasmissione.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione modello= (SAAdminTool.DocsPaWR.ModelloTrasmissione)modelliTrasmissione[i];
                modelloSel = false;

                if (modelliTrasmAssociati.Count != 0)
                {
                    for (int j = 0; j < modelliTrasmAssociati.Count; j++)
                    {
                        if (Convert.ToString(modello.SYSTEM_ID) == ((SAAdminTool.DocsPaWR.AssDocDiagTrasm)modelliTrasmAssociati[j]).ID_TEMPLATE)
                        {
                            modelloSel = true;
                            if (((SAAdminTool.DocsPaWR.AssDocDiagTrasm)modelliTrasmAssociati[j]).TRASM_AUT == "1")
                            {
                                m = new ModelliTrasmHT(modello.NOME, "1", "1");
                            }
                            else
                            {
                                m = new ModelliTrasmHT(modello.NOME, "1", "0");
                            }
                            this.HTmodelli.Add(Convert.ToString(modello.SYSTEM_ID), m);
                            break;
                        }
                    }
                }
                if (!modelloSel)
                {
                    m = new ModelliTrasmHT(modello.NOME, "0", "0");
                    this.HTmodelli.Add(Convert.ToString(modello.SYSTEM_ID), m);
                }
            }
            Session.Add("hashtableModelli", HTmodelli);
        }

        private void impostaSelezione()
        {
            ModelliTrasmHT m = new ModelliTrasmHT();
            HTmodelli = (Hashtable)Session["hashtableModelli"];
            for (int i = 0; i < dg_ModelliTrasm.Items.Count; i++)
            {
                if (HTmodelli.Count != 0)
                {
                    if (HTmodelli.ContainsKey(dg_ModelliTrasm.Items[i].Cells[0].Text))
                    {
                        if (((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Checked)
                        {
                            if (((CheckBox)dg_ModelliTrasm.Items[i].Cells[3].Controls[1]).Checked)
                            {
                                m = new ModelliTrasmHT(dg_ModelliTrasm.Items[i].Cells[1].Text, "1", "1");
                                ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Enabled = false;
                            }
                            else
                            {
                                m = new ModelliTrasmHT(dg_ModelliTrasm.Items[i].Cells[1].Text, "1", "0");
                                ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Enabled = true;
                            }
                        }
                        else
                        {
                            if (!((CheckBox)dg_ModelliTrasm.Items[i].Cells[3].Controls[1]).Checked)
                            {
                                m = new ModelliTrasmHT(dg_ModelliTrasm.Items[i].Cells[1].Text, "0", "0");
                                ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Enabled = true;
                            }
                            else
                            {
                                m = new ModelliTrasmHT(dg_ModelliTrasm.Items[i].Cells[1].Text, "1", "1");
                                ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Enabled = false;
                                ((CheckBox)dg_ModelliTrasm.Items[i].Cells[2].Controls[1]).Checked = true;
                            }
                        }
                    }
                }
                HTmodelli.Remove(dg_ModelliTrasm.Items[i].Cells[0].Text);
                HTmodelli.Add(dg_ModelliTrasm.Items[i].Cells[0].Text, m);
            }
            Session.Add("hashtableModelli", HTmodelli);
        }

       

        public class ModelliTrasmHT
        {
            private string descr;
            private string ins;
            private string ric;

            public ModelliTrasmHT()
            {
            }

            public ModelliTrasmHT(string descr, string ins, string ric)
            {
                this.descr = descr;
                this.ins = ins;
                this.ric = ric;
            }

            public string Descr { get { return descr; } }
            public string Ins { get { return ins; } }
            public string Ric { get { return ric; } }
        }
	}
}
