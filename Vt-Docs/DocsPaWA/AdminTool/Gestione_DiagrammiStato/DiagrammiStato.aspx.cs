using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Linq;

namespace DocsPAWA.AdminTool.Gestione_DiagrammiStato
{
	/// <summary>
	/// Summary description for DiagrammiStato.
	/// </summary>
	public class DiagrammiStato : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.Label lbl_avviso;
		protected System.Web.UI.WebControls.Panel Panel_ListaDiagrammi;
		protected System.Web.UI.WebControls.Panel Panel_GestioneStati;
		protected System.Web.UI.WebControls.Panel Panel_ListaPassi;
		protected System.Web.UI.WebControls.Button btn_addStato;
		protected System.Web.UI.WebControls.Button btn_delStato;
		protected System.Web.UI.WebControls.DropDownList ddl_stati;
		protected System.Web.UI.WebControls.ListBox lbox_stati1;
		protected System.Web.UI.WebControls.ListBox lbox_stati2;
		protected System.Web.UI.WebControls.Button btn_moveStato1;
		protected System.Web.UI.WebControls.Button btn_moveStato2;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.Button btn_addStep;
		protected System.Web.UI.WebControls.Button btn_nuovoDiagr;
		protected System.Web.UI.WebControls.DataGrid dg_listaPassi;
		protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGListaPassi;
		protected System.Web.UI.WebControls.Button btn_salva;
		protected System.Web.UI.WebControls.Button btn_listaDiagrammi;
		protected System.Web.UI.WebControls.Label lbl_statiFinali;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Label lbl_statiIniziali;
		protected System.Web.UI.WebControls.Button btn_modStato;
		protected System.Web.UI.WebControls.Button btn_modPasso;
        protected System.Web.UI.WebControls.Button btn_vis;
		protected System.Web.UI.WebControls.Label lbl_titolo;
		private string idAmministrazione;
		protected System.Web.UI.WebControls.DataGrid dg_listaDiagrammi;
		protected System.Web.UI.HtmlControls.HtmlGenericControl DivGgListaDiagrammi;
		protected System.Web.UI.WebControls.Label lbl_statoAuto;
		protected System.Web.UI.WebControls.DropDownList ddl_statiAutomatici;
        protected System.Web.UI.WebControls.DropDownList ddl_statiAutomaticiProcessoFirma;
        protected System.Web.UI.WebControls.Panel PnlStatoAutoProcesso;
        private ArrayList listaDiagrammi;
			
		private void Page_Load(object sender, System.EventArgs e)
		{

			//----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            Session["AdminBookmark"] = "DiagrammiStato";
            if (Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}
			
			AmmUtils.WebServiceLink  ws=new AmmUtils.WebServiceLink();
			if(!ws.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}

			if(Session["AMMDATASET"] == null)
			{
				RegisterStartupScript("NoProfilazione","<script>alert('Attenzione selezionare un\\'amministrazione !'); document.location = '../Gestione_Homepage/Home.aspx';</script>");
				return;
			}
			// ---------------------------------------------------------------
			
			string[] amministrazione = ((string) Session["AMMDATASET"]).Split('@');
			string codiceAmministrazione  = amministrazione[0];
            idAmministrazione = DocsPAWA.Utils.getIdAmmByCod(codiceAmministrazione,this);

            listaDiagrammi = DiagrammiManager.getDiagrammi(idAmministrazione, this);

            caricaDgListaDiagrammi();
						
			if(!IsPostBack)
			{
				Panel_ListaDiagrammi.Visible = true;
				Panel_GestioneStati.Visible = false;
				Panel_ListaPassi.Visible = false;
				btn_salva.Visible = false;	
		
				lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");                
			}

			if (Session["DiagrammaStato"] != null && Session["statoModificatoSalvato"] != null)
            {
                caricaDdlStati();
                settaStatiInizialiNew();
                settaStatiFinali(trovaStatiFinali());
                if (dg_listaPassi.Items.Count != 0)
                {
                    caricaDgListaPassi();
                    if (dg_listaPassi.Items.Count > 0)
                    {
                        dg_listaPassi.Columns[2].Visible = true;
                        dg_listaPassi.Columns[3].Visible = true;
                    }
                }
                Session.Remove("statoModificatoSalvato");
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
			this.btn_nuovoDiagr.Click += new System.EventHandler(this.btn_nuovoDiagr_Click);
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.btn_listaDiagrammi.Click += new System.EventHandler(this.btn_listaDiagrammi_Click);
			this.dg_listaDiagrammi.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_listaDiagrammi_PageIndexChanged);
			this.dg_listaDiagrammi.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_listaDiagrammi_DeleteCommand);
			this.dg_listaDiagrammi.SelectedIndexChanged += new System.EventHandler(this.dg_listaDiagrammi_SelectedIndexChanged);
            this.dg_listaDiagrammi.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_listaDiagrammi_ItemCommand);
			this.dg_listaPassi.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_listaPassi_PageIndexChanged);
			this.dg_listaPassi.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_listaPassi_DeleteCommand);
			this.dg_listaPassi.SelectedIndexChanged += new System.EventHandler(this.dg_listaPassi_SelectedIndexChanged);
			this.btn_addStato.Click += new System.EventHandler(this.btn_addStato_Click);
			this.btn_delStato.Click += new System.EventHandler(this.btn_delStato_Click);
			this.btn_modStato.Click += new System.EventHandler(this.btn_modStato_Click);
			this.ddl_stati.SelectedIndexChanged += new System.EventHandler(this.ddl_stati_SelectedIndexChanged);
			this.btn_moveStato1.Click += new System.EventHandler(this.btn_moveStato1_Click);
			this.btn_moveStato2.Click += new System.EventHandler(this.btn_moveStato2_Click);
			this.btn_addStep.Click += new System.EventHandler(this.btn_addStep_Click);
			this.btn_modPasso.Click += new System.EventHandler(this.btn_modPasso_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Metodi di Utilità

        private bool verificaEliminazioneStato(string nomeStato)
		{
			if(  ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI != null)
			{
				ArrayList steps = new ArrayList( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI);
				for(int i=0; i<steps.Count; i++)
				{
					DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo) steps[i];
					if(nomeStato == step.STATO_PADRE.DESCRIZIONE)
						return true;
					for(int j=0; j<step.SUCCESSIVI.Length; j++)
					{
						if(nomeStato == ((DocsPAWA.DocsPaWR.Stato) step.SUCCESSIVI[j]).DESCRIZIONE)
							return true;
					}			
				}
			}
			return false;			
		}

		private bool verificaStatoPadre(string statoPadre)
		{
			DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"];
			if(dg.PASSI != null)
			{
				for(int i=0; i<dg.PASSI.Length; i++)
				{
					DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo) dg.PASSI[i];	
					if(step.STATO_PADRE.DESCRIZIONE == statoPadre)
						return true;
				}
			}
			return false;
		}

		public void settaStatiIniziali(ArrayList statiIniziali)
		{
			lbl_statiIniziali.Text = "";
			for(int i=0; i<statiIniziali.Count; i++)
			{
				lbl_statiIniziali.Text += ((DocsPAWA.DocsPaWR.Stato) statiIniziali[i]).DESCRIZIONE + " - ";
			}
			if(lbl_statiIniziali.Text != "")
				lbl_statiIniziali.Text = lbl_statiIniziali.Text.Substring(0,lbl_statiIniziali.Text.Length-2);
		}

		public void settaStatiFinali(ArrayList statiFinali)
		{
			lbl_statiFinali.Text = "";
			for(int i=0; i<statiFinali.Count; i++)
			{
				lbl_statiFinali.Text += ((DocsPAWA.DocsPaWR.Stato) statiFinali[i]).DESCRIZIONE + " - ";
			}
			if(lbl_statiFinali.Text != "")
				lbl_statiFinali.Text = lbl_statiFinali.Text.Substring(0,lbl_statiFinali.Text.Length-2);
		}

		private ArrayList trovaStatiIniziali()
		{
			//E' uno stato Iniziale, uno stato che non è mai usato come successivo
			ArrayList statiIniziali = new ArrayList();
			DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"];
			
			if(dg.STATI != null)
			{
				for(int i=0; i<dg.STATI.Length; i++)
				{	
					DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato) dg.STATI[i];
					bool resultIniziale = true;
					
					if(dg.PASSI != null)
					{
						for(int j=0; j<dg.PASSI.Length; j++)
						{
							DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo) dg.PASSI[j];
							if(step.SUCCESSIVI != null)
							{
								for(int k=0; k<step.SUCCESSIVI.Length; k++)
								{
									if(st.DESCRIZIONE == ((DocsPAWA.DocsPaWR.Stato) step.SUCCESSIVI[k]).DESCRIZIONE)
										resultIniziale = false;
								}
							}
						}
					}

					if(resultIniziale)
					{
						statiIniziali.Add(st);
						((DocsPAWA.DocsPaWR.Stato) dg.STATI[i]).STATO_INIZIALE = true;
					}
					else
					{
						((DocsPAWA.DocsPaWR.Stato) dg.STATI[i]).STATO_INIZIALE = false;				
					}					
				}
			}
			return statiIniziali;
		}

		private ArrayList trovaStatiFinali()
		{
			//E' uno stato Finale, uno stato senza stati successivi
			ArrayList statiFinali= new ArrayList();
			DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"];
			
			if(dg.STATI != null)
			{
				for(int i=0; i<dg.STATI.Length; i++)
				{
					DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato) dg.STATI[i];
					bool resultFinale = true;
					
					if(dg.PASSI != null)
					{
						for(int j=0; j<dg.PASSI.Length; j++)
						{
							DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo) dg.PASSI[j];
							if(st.DESCRIZIONE == step.STATO_PADRE.DESCRIZIONE)
								resultFinale = false;
						}
					}
					if(resultFinale)
					{
						statiFinali.Add(st);
						((DocsPAWA.DocsPaWR.Stato) dg.STATI[i]).STATO_FINALE = true;
					}
					else
					{
						((DocsPAWA.DocsPaWR.Stato) dg.STATI[i]).STATO_FINALE = false;				
					}
				}
			}
			return statiFinali;
		}

        private void setDdlStatiAutomatici(DocsPAWA.DocsPaWR.Passo step)
		{
			for(int i=0; i<ddl_statiAutomatici.Items.Count; i++)
			{
				if(step.DESCRIZIONE_STATO_AUTOMATICO == ddl_statiAutomatici.Items[i].Text)
					ddl_statiAutomatici.SelectedIndex = i;
			}
		}

        private void setDdlStatiAutomaticiProcessoFirma(DocsPAWA.DocsPaWR.Passo step)
        {
            PnlStatoAutoProcesso.Visible = false;

            DocsPAWA.DocsPaWR.DiagrammaStato diagramma = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
            string idProcessoFirma = (from s in diagramma.STATI
                                        where s.DESCRIZIONE.Equals(ddl_stati.SelectedItem.Text)
                                        select s.ID_PROCESSO_FIRMA).FirstOrDefault();
            if (!string.IsNullOrEmpty(idProcessoFirma) && !DiagrammiManager.IsProcessoFirmaConCambioStato(idProcessoFirma, this))
            {
                PnlStatoAutoProcesso.Visible = true;
                if (!string.IsNullOrEmpty(step.ID_STATO_AUTOMATICO_LF))
                {
                    DocsPAWA.DocsPaWR.Stato statoAutomaticoLf = (from s in diagramma.STATI
                                                                 where s.SYSTEM_ID.ToString().Equals(step.ID_STATO_AUTOMATICO_LF)
                                                                 select s).FirstOrDefault();
                    string descrizioneStatoLf = statoAutomaticoLf.DESCRIZIONE;
                    for (int i = 0; i < ddl_statiAutomaticiProcessoFirma.Items.Count; i++)
                    {
                        if (descrizioneStatoLf == ddl_statiAutomaticiProcessoFirma.Items[i].Text)
                            ddl_statiAutomaticiProcessoFirma.SelectedIndex = i;
                    }
                }
            }
        }

        private void setDllStati(DocsPAWA.DocsPaWR.Stato st)
		{
			for(int i=0; i<ddl_stati.Items.Count; i++)
			{
				if(ddl_stati.Items[i].Text == st.DESCRIZIONE)
				{
					ddl_stati.SelectedIndex = i;
					return;
				}
			}
		}

		private void setLbxStati1(DocsPAWA.DocsPaWR.Stato st, DocsPAWA.DocsPaWR.Stato[] successivi/*ArrayList successivi*/)
		{
			lbox_stati1.Items.Clear();
			DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"];
			
			if(dg.STATI != null)
			{
				for(int i=0; i<dg.STATI.Length; i++)
				{
					lbox_stati1.Items.Add( ((DocsPAWA.DocsPaWR.Stato) dg.STATI[i]).DESCRIZIONE);
				}
			}

			for(int i=0; i<lbox_stati1.Items.Count; i++)
			{
				if(lbox_stati1.Items[i].Text == st.DESCRIZIONE)
					lbox_stati1.Items.RemoveAt(i);
			}

			if(successivi != null)
			{
				for(int i=0; i<successivi.Length; i++)
				{
					DocsPaWR.Stato stApp = (DocsPAWA.DocsPaWR.Stato) successivi[i];
					for(int j=0; j<lbox_stati1.Items.Count; j++)
					{
						if(stApp.DESCRIZIONE == lbox_stati1.Items[j].Text)
							lbox_stati1.Items.RemoveAt(j);
					}
				}
			}
		}

		private void setLbxStati2(DocsPAWA.DocsPaWR.Stato[] successivi /*ArrayList successivi*/)
		{
			lbox_stati2.Items.Clear();
			ddl_statiAutomatici.Items.Clear();
            ddl_statiAutomaticiProcessoFirma.Items.Clear();
			if(successivi != null)
			{
				ddl_statiAutomatici.Items.Add("");
                ddl_statiAutomaticiProcessoFirma.Items.Add("");
				for(int i=0; i<successivi.Length; i++)
				{				
					lbox_stati2.Items.Add( ((DocsPAWA.DocsPaWR.Stato) successivi[i]).DESCRIZIONE);
					ddl_statiAutomatici.Items.Add( ((DocsPAWA.DocsPaWR.Stato) successivi[i]).DESCRIZIONE);
                    ddl_statiAutomaticiProcessoFirma.Items.Add(((DocsPAWA.DocsPaWR.Stato)successivi[i]).DESCRIZIONE);

                }
			}
		}

		private void rimuoviPasso(int stepDaRimuovere)
		{
			if( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI != null)
			{
				ArrayList passi = new ArrayList( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI);
				passi.RemoveAt(stepDaRimuovere);
				((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI = new DocsPAWA.DocsPaWR.Passo[passi.Count];
				passi.CopyTo(((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI);
			}
		}

		private void aggiungiPasso(DocsPAWA.DocsPaWR.Passo step)
		{
			ArrayList passi;
			if( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI != null)
			{
				passi = new ArrayList( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI);
				passi.Add(step);
			}
			else
			{
				passi = new ArrayList();
				passi.Add(step);
			}
			((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI = new DocsPAWA.DocsPaWR.Passo[passi.Count];
			passi.CopyTo(((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI);
		}

		private void rimuoviStato(int stDaRimuovere)
		{
			if( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).STATI != null)
			{
				ArrayList stati = new ArrayList( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).STATI);
				DocsPaWR.Stato statoDaEliminare = (DocsPAWA.DocsPaWR.Stato) stati[stDaRimuovere];
				if(statoDaEliminare.STATO_INIZIALE)
					lbl_statiIniziali.Text = "";
				if(statoDaEliminare.STATO_FINALE)
					lbl_statiFinali.Text = "";
				stati.RemoveAt(stDaRimuovere);
				((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).STATI = new DocsPAWA.DocsPaWR.Stato[stati.Count];
				stati.CopyTo(((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).STATI);
			}
		}

        private void settaStatiInizialiNew()
        {
            DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
            if(dg.STATI != null)
            {
                lbl_statiIniziali.Text = "";
                for (int i = 0; i < dg.STATI.Length; i++)
                {
                    if (dg.STATI[i].STATO_INIZIALE)
                        lbl_statiIniziali.Text += dg.STATI[i].DESCRIZIONE + " - ";
                }
                if (lbl_statiIniziali.Text != "")
                    lbl_statiIniziali.Text = lbl_statiIniziali.Text.Substring(0, lbl_statiIniziali.Text.Length - 2);
                lbox_stati1.SelectedIndex = -1;
            }
        }

        private bool controllaStatiInizialiFinali(DocsPaWR.DiagrammaStato diagramma)
        {
            if (lbl_statiIniziali.Text != "" && lbl_statiFinali.Text != "" && diagramma != null)
            {
                ArrayList stati_Iniziali = new ArrayList();
                ArrayList statiFinali = new ArrayList();

                for(int i=0; i<diagramma.STATI.Length; i++)
                {
                    DocsPaWR.Stato st = (DocsPaWR.Stato) diagramma.STATI[i];
                    if(st.STATO_INIZIALE)
                        stati_Iniziali.Add(st);
                    if(st.STATO_FINALE)
                        statiFinali.Add(st);
                }

                for (int j = 0; j < stati_Iniziali.Count; j++)
                {
                    DocsPaWR.Stato stApp_1 = (DocsPaWR.Stato)stati_Iniziali[j];
                    for (int k = 0; k < statiFinali.Count; k++)
                    {
                        DocsPaWR.Stato stApp_2 = (DocsPaWR.Stato)statiFinali[k];
                        if (stApp_2.DESCRIZIONE.Trim().ToUpper() == stApp_1.DESCRIZIONE.Trim().ToUpper())
                            return true;
                    }
                }    
            }
            return false;
        }

        private bool controllaStatiEProcessiFirma(DocsPaWR.DiagrammaStato diagramma, out string msg)
        {
            bool result = false;
            msg = string.Empty;
            //Controllo che non ci sia un solo processo di firma associato al diagramma di stato
            //MEV 2020: Posso avviare più processi di firma per uno stesso diagramma
            /*
            if ((from s in diagramma.STATI where !string.IsNullOrEmpty(s.ID_PROCESSO_FIRMA) select s).ToList().Count > 1)
            {
                msg = "Attenzione, non è possibile associare più di un processo di firma all'interno di un diagramma";
                result = true;
                return result;
            }
            */
            for (int i = 0; i < diagramma.STATI.Length; i++)
            {
                DocsPaWR.Stato st = (DocsPaWR.Stato)diagramma.STATI[i];
                if(!string.IsNullOrEmpty(st.ID_PROCESSO_FIRMA))
                {
                    if (st.STATO_FINALE)
                    {
                        msg = "Attenzione, non è possibile associare un processo di firma ad uno stato finale";
                        result = true;
                        break;
                    }

                    //Controllo che lo stato con processo non sia stato automatico per qualche passo
                    if ((from p in diagramma.PASSI where p.ID_STATO_AUTOMATICO != null &&
                         p.ID_STATO_AUTOMATICO.Equals(st.SYSTEM_ID.ToString()) select p).FirstOrDefault() != null)
                    {
                        msg = "Attenzione, non è possibile associare un processo di firma ad uno stato automatico";
                        result = true;
                        break;
                    }

                    //Controllo che lo stato con processo non sia stato automatico di processo per qualche passo
                    if ((from p in diagramma.PASSI
                         where p.ID_STATO_AUTOMATICO_LF != null && p.ID_STATO_AUTOMATICO_LF.Equals(st.SYSTEM_ID.ToString())
                         select p).FirstOrDefault() != null)
                    {
                        if (DiagrammiManager.IsModelloDiFirma(st.ID_PROCESSO_FIRMA, this))
                        {
                            msg = "Attenzione, non è possibile associare un modello di firma ad uno stato automatico di un altro processo di firma";
                            result = true;
                            break;
                        }
                    }

                    //Controllo stati precedenti, un passo con processo di firma non può essere preceduto da uno di consolidamento
                    DocsPaWR.Stato statoSel = (from s1 in diagramma.STATI where s1.SYSTEM_ID.Equals(st.SYSTEM_ID) select s1).FirstOrDefault();
                    if (!statoSel.STATO_CONSOLIDAMENTO.Equals(DocsPaWR.DocumentConsolidationStateEnum.None))
                    {
                        msg = "Attenzione, un stato con avvio processo di firma non può essere uno stato con consolidamento.";
                        return true;
                        break;
                    }
                    Session.Add("StatiPrecendenti", new List<string>());
                    if (ControllaEsistenzaStatoPrecedenteConCosolidamento(st, diagramma))
                    {
                        msg = "Attenzione, un stato con avvio processo di firma non può segure uno di consolidamento.";
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public bool ControllaPresensaStato(DocsPaWR.Stato[] successivi, DocsPaWR.Stato stato)
        {
            bool result = true;

            result = (from s in successivi where s.SYSTEM_ID.Equals(stato.SYSTEM_ID) select s).FirstOrDefault() != null;

            return result;
        }

        public bool ControllaEsistenzaStatoPrecedenteConCosolidamento(DocsPaWR.Stato stato, DocsPaWR.DiagrammaStato diagramma)
        {
            bool result = false;
            (Session["StatiPrecendenti"] as List<string>).Add(stato.SYSTEM_ID.ToString());
            
            //Estraggo tutti gli stati per cui lo stato in input è il successivo e controllo che non contenga il consolidamento
            DocsPaWR.Stato[] statoPrec = (from p in diagramma.PASSI where ControllaPresensaStato(p.SUCCESSIVI, stato)
                                          select p.STATO_PADRE).ToArray();
            foreach (DocsPaWR.Stato s in statoPrec)
            {
                if (!(Session["StatiPrecendenti"] as List<string>).Contains(s.SYSTEM_ID.ToString()))
                {
                    DocsPaWR.Stato statoSel = (from s1 in diagramma.STATI where s1.SYSTEM_ID.Equals(s.SYSTEM_ID) select s1).FirstOrDefault();
                    if (!statoSel.STATO_CONSOLIDAMENTO.Equals(DocsPaWR.DocumentConsolidationStateEnum.None))
                    {
                        return true;
                    }
                    else if (!statoSel.STATO_INIZIALE)
                    {
                        if (ControllaEsistenzaStatoPrecedenteConCosolidamento(statoSel, diagramma))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private void caricaDdlStati()
        {
            if (Session["DiagrammaStato"] != null)
            {
                DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
                if (dg.STATI != null)
                {
                    int elSelezionato = ddl_stati.SelectedIndex;
                    ddl_stati.Items.Clear();
                    for (int i = 0; i < dg.STATI.Length; i++)
                    {
                        DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)dg.STATI[i];
                        ListItem li = new ListItem();
                        if(st.SYSTEM_ID != 0)
                            li.Value = Convert.ToString(st.SYSTEM_ID);
                        li.Text = st.DESCRIZIONE;
                        ddl_stati.Items.Add(li);
                    }
                    ddl_stati.SelectedIndex = elSelezionato;
                }
            }
        }

		#endregion

		#region Azioni Pulsanti
		private void btn_addStato_Click(object sender, System.EventArgs e)
		{
            Session.Remove("statoDaModificare");
            ClientScript.RegisterStartupScript(this.GetType(), "gestioneStato", "apriPopupGestioneStato();", true);
            
            dg_listaPassi.SelectedIndex = -1;
            btn_modPasso.Visible = false;
            btn_addStep.Visible = true;            
		}
		
		private void btn_delStato_Click(object sender, System.EventArgs e)
		{
			dg_listaPassi.SelectedIndex = -1;
			btn_modPasso.Visible = false;
			btn_addStep.Visible = true;

            if (ddl_stati.SelectedItem == null || ddl_stati.SelectedItem.Text == "")
                return;

			int stSelezionato = ddl_stati.SelectedIndex;

            if (DiagrammiManager.getDocOrFascInStato(ddl_stati.SelectedValue, this))
			{
				RegisterStartupScript("statiSuccessivi","<script>alert('Eliminazione impossibile, esistono documenti o fascicoli nello stato selezionato !');</script>");
				return;
			}

			if(verificaEliminazioneStato(ddl_stati.SelectedItem.Text))
			{
				RegisterStartupScript("statiSuccessivi","<script>alert('Eliminazione impossibile, lo stato è coinvolto in uno o più passi !');</script>");
				return;
			}
			
			if(ddl_stati.Items.Count != 0)
			{
				ddl_stati.Items.RemoveAt(stSelezionato);

				rimuoviStato(stSelezionato);
				
				lbox_stati1.Items.Clear();
				lbox_stati2.Items.Clear();
				ddl_statiAutomatici.Items.Clear();
                ddl_statiAutomaticiProcessoFirma.Items.Clear();
			}

			//Imposta le label che indicano istantaneamente quali sono gli stati iniziali e finali
			//del diagramma che si sta costruendo, avvalendosi delle funzioni:
			//"trovaStatoIniziale" - "trovaStatoFinale"
			
			settaStatiFinali(trovaStatiFinali());
			btn_modPasso.Visible = false;
			btn_addStep.Visible = true;
		}

		private void btn_moveStato1_Click(object sender, System.EventArgs e)
		{
            if (ddl_statiAutomatici.Items.Count == 0 || ddl_statiAutomatici.Items[0].Text != "")
            {
                ddl_statiAutomatici.Items.Add("");
                ddl_statiAutomaticiProcessoFirma.Items.Add("");
            }


			for(int i=0; i<lbox_stati1.Items.Count; i++)
			{
				if(lbox_stati1.Items[i].Selected)
				{
					lbox_stati2.Items.Add(lbox_stati1.Items[i]);
					ddl_statiAutomatici.Items.Add(lbox_stati1.Items[i]);
                    ddl_statiAutomaticiProcessoFirma.Items.Add(lbox_stati1.Items[i]);
                }
			}
			
			for(int i=0; i<lbox_stati1.Items.Count; i++)
			{
				if(lbox_stati1.Items[i].Selected)
				{
					lbox_stati1.Items.RemoveAt(i);
					i--;
				}
			}

			lbox_stati2.SelectedIndex = -1;			
		}

		private void btn_moveStato2_Click(object sender, System.EventArgs e)
		{
			for(int i=0; i<lbox_stati2.Items.Count; i++)
			{
				if(lbox_stati2.Items[i].Selected)
				{
					lbox_stati1.Items.Add(lbox_stati2.Items[i]);
				}
			}
			
			for(int i=0; i<lbox_stati2.Items.Count; i++)
			{
				if(lbox_stati2.Items[i].Selected)
				{
					lbox_stati2.Items.RemoveAt(i);
					ddl_statiAutomatici.Items.RemoveAt(i+1);
                    ddl_statiAutomaticiProcessoFirma.Items.RemoveAt(i+1);
					i--;
				}
			}

			lbox_stati1.SelectedIndex = -1;				
		}

		private void btn_nuovoDiagr_Click(object sender, System.EventArgs e)
		{
			lbl_titolo.Text = "Gestione Diagramma di stato";
			Panel_ListaDiagrammi.Visible = false;
			Panel_GestioneStati.Visible = true;
			Panel_ListaPassi.Visible = false;
			
			txt_descrizione.Text = "";
			lbox_stati1.Items.Clear();
			lbox_stati2.Items.Clear();
			ddl_stati.Items.Clear();

			lbl_statiIniziali.Text = "";
			lbl_statiFinali.Text = "";

			//DocsPaWR.DiagrammaStato diagrammaStato = new DocsPAWA.DocsPaWR.DiagrammaStato();
			DocsPaWR.DiagrammaStato diagrammaStato = new DocsPAWA.DocsPaWR.DiagrammaStato();
			Session.Add("DiagrammaStato",diagrammaStato);

			//Imposta le label che indicano istantaneamente quali sono gli stati iniziali e finali
			//del diagramma che si sta costruendo, avvalendosi delle funzioni:
			//"trovaStatoIniziale" - "trovaStatoFinale"
			
			settaStatiFinali(trovaStatiFinali());
			
			dg_listaPassi.SelectedIndex = -1;
			btn_modPasso.Visible = false;
			btn_addStep.Visible = true;
			btn_salva.Visible = true;
			ddl_statiAutomatici.Items.Clear();
            ddl_statiAutomaticiProcessoFirma.Items.Clear();
			SetFocus(txt_descrizione);			
		}

		private void btn_addStep_Click(object sender, System.EventArgs e)
		{
			if(lbox_stati2.Items.Count == 0)
			{
				RegisterStartupScript("statiSuccessivi","<script>alert('Selezionare uno stato iniziale e uno o più stati successivi !');</script>");
				return;
			}

			if(verificaStatoPadre(ddl_stati.SelectedItem.Text))
			{
				RegisterStartupScript("statiSuccessivi","<script>alert('Spiacente, esiste già un passo con questo stato iniziale !');</script>");
				return;
			}

			DocsPaWR.Stato st_1	= new DocsPAWA.DocsPaWR.Stato();
			DocsPaWR.Passo step	= new DocsPAWA.DocsPaWR.Passo();
			st_1.DESCRIZIONE = ddl_stati.SelectedItem.Text;
			step.STATO_PADRE = st_1;
            DocsPAWA.DocsPaWR.DiagrammaStato diagramma = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
            if (ddl_statiAutomatici.SelectedItem.Text != "")
			{
				step.DESCRIZIONE_STATO_AUTOMATICO = ddl_statiAutomatici.SelectedItem.Text;	
			}
			else
			{
				step.DESCRIZIONE_STATO_AUTOMATICO = "";
				step.ID_STATO_AUTOMATICO = "";
			}
            if (ddl_statiAutomaticiProcessoFirma.SelectedItem.Text != "")
            {
                string idStatoAutomaticoSel = (from s in diagramma.STATI
                                               where s.DESCRIZIONE.Equals(ddl_statiAutomaticiProcessoFirma.SelectedItem.Text)
                                               select s.SYSTEM_ID.ToString()).FirstOrDefault();

                step.ID_STATO_AUTOMATICO_LF = idStatoAutomaticoSel;
            }
            else
            {
                step.ID_STATO_AUTOMATICO_LF = string.Empty;
            }

			ArrayList successivi = new ArrayList();
			for(int i=0; i<lbox_stati2.Items.Count; i++)
			{
				DocsPaWR.Stato st_2 = new DocsPAWA.DocsPaWR.Stato();
				st_2.DESCRIZIONE = lbox_stati2.Items[i].Text;
	
				successivi.Add(st_2);
				step.SUCCESSIVI = new DocsPAWA.DocsPaWR.Stato[successivi.Count];
				successivi.CopyTo(step.SUCCESSIVI);
			}

			aggiungiPasso(step);
			
			lbox_stati1.Items.Clear();
			lbox_stati2.Items.Clear();

            ddl_stati.SelectedIndex = -1;
            PnlStatoAutoProcesso.Visible = false;
            string idProcessoFirma = (from s in diagramma.STATI
                                      where s.DESCRIZIONE.Equals(ddl_stati.SelectedItem.Text)
                                      select s.ID_PROCESSO_FIRMA).FirstOrDefault();
            if (!string.IsNullOrEmpty(idProcessoFirma) && !DiagrammiManager.IsProcessoFirmaConCambioStato(idProcessoFirma, this))
                PnlStatoAutoProcesso.Visible = true;

            ddl_statiAutomatici.Items.Clear();
            ddl_statiAutomaticiProcessoFirma.Items.Clear();

			caricaDgListaPassi();
			dg_listaPassi.Columns[2].Visible = true;
			dg_listaPassi.Columns[3].Visible = true;
			
			settaStatiFinali(trovaStatiFinali());
		}

		private void btn_modStato_Click(object sender, System.EventArgs e)
		{
            if (Session["DiagrammaStato"] != null)
            {
                DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
                foreach (DocsPAWA.DocsPaWR.Stato st in dg.STATI)
                {
                    if ( (st.SYSTEM_ID.ToString() == ddl_stati.SelectedValue) || (st.DESCRIZIONE == ddl_stati.SelectedItem.Text) )
                    {
                        Session.Add("statoDaModificare", st);
                        ClientScript.RegisterStartupScript(this.GetType(), "gestioneStato", "apriPopupGestioneStato();", true);
                    }
                }
            }

            dg_listaPassi.SelectedIndex = -1;
            btn_modPasso.Visible = false;
            btn_addStep.Visible = true;            
		}

        private void btn_modPasso_Click(object sender, System.EventArgs e)
		{
			if(lbox_stati2.Items.Count == 0)
			{
				RegisterStartupScript("statiSuccessivi","<script>alert('Selezionare uno stato iniziale e uno o più stati successivi !');</script>");
				return;
			}

			int elSelezionato = (dg_listaPassi.CurrentPageIndex * dg_listaPassi.PageSize) + dg_listaPassi.SelectedIndex;
			
			rimuoviPasso(elSelezionato);
			
			if(verificaStatoPadre(ddl_stati.SelectedItem.Text))
			{
				RegisterStartupScript("statiSuccessivi","<script>alert('Spiacente, esiste già un passo con questo stato iniziale !');</script>");
				return;
			}

			DocsPaWR.Passo newStep = new DocsPAWA.DocsPaWR.Passo();
			DocsPaWR.Stato st_1 = new DocsPAWA.DocsPaWR.Stato();
			st_1.DESCRIZIONE = ddl_stati.SelectedItem.Text;
			newStep.STATO_PADRE = st_1;

            DocsPAWA.DocsPaWR.DiagrammaStato diagramma = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
            if (ddl_statiAutomatici.SelectedItem.Text != "")
			{
				newStep.DESCRIZIONE_STATO_AUTOMATICO = ddl_statiAutomatici.SelectedItem.Text;	
			}
			else
			{
				newStep.DESCRIZIONE_STATO_AUTOMATICO = "";
				newStep.ID_STATO_AUTOMATICO = "";
			}
            if (ddl_statiAutomaticiProcessoFirma.SelectedItem.Text != "")
            {
                string idStatoAutomaticoSel = (from s in diagramma.STATI
                                                            where s.DESCRIZIONE.Equals(ddl_statiAutomaticiProcessoFirma.SelectedItem.Text)
                                                            select s.SYSTEM_ID.ToString()).FirstOrDefault();

                newStep.ID_STATO_AUTOMATICO_LF = idStatoAutomaticoSel;
            }
            else
            {
                newStep.ID_STATO_AUTOMATICO_LF = string.Empty;
            }

			ArrayList successivi = new ArrayList();
			for(int i=0; i<lbox_stati2.Items.Count; i++)
			{
				DocsPaWR.Stato st_2 = new DocsPAWA.DocsPaWR.Stato();
				st_2.DESCRIZIONE = lbox_stati2.Items[i].Text;
				
				successivi.Add(st_2);
				newStep.SUCCESSIVI = new DocsPAWA.DocsPaWR.Stato[successivi.Count];
				successivi.CopyTo(newStep.SUCCESSIVI);
			}

			ArrayList passi_1 = new ArrayList( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI);
			passi_1.Insert(elSelezionato,newStep);
			((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI = new DocsPAWA.DocsPaWR.Passo[passi_1.Count];
			passi_1.CopyTo(((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI);
		
			caricaDgListaPassi();
			dg_listaPassi.Columns[2].Visible = true;
			dg_listaPassi.Columns[3].Visible = true;
			btn_modPasso.Visible = false;
			btn_addStep.Visible = true;
			lbox_stati1.Items.Clear();
			lbox_stati2.Items.Clear();
			ddl_statiAutomatici.Items.Clear();
            ddl_statiAutomaticiProcessoFirma.Items.Clear();
			ddl_stati.SelectedIndex = -1;

            PnlStatoAutoProcesso.Visible = false;
            string idProcessoFirma = (from s in diagramma.STATI
                                      where s.DESCRIZIONE.Equals(ddl_stati.SelectedItem.Text)
                                      select s.ID_PROCESSO_FIRMA).FirstOrDefault();
            if (!string.IsNullOrEmpty(idProcessoFirma) && !DiagrammiManager.IsProcessoFirmaConCambioStato(idProcessoFirma, this))
                PnlStatoAutoProcesso.Visible = true;
            //Imposta le label che indicano istantaneamente quali sono gli stati iniziali e finali
            //del diagramma che si sta costruendo, avvalendosi delle funzioni:
            //"trovaStatoIniziale" - "trovaStatoFinale"

            settaStatiFinali(trovaStatiFinali());
		}

		private void btn_salva_Click(object sender, System.EventArgs e)
		{
			if(txt_descrizione.Text == "")
			{
				//RegisterStartupScript("nomeDiagramma","<script>alert('Descrizione Diagramma vuota !');</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "nomeDiagramma", "<script>alert('Descrizione Diagramma vuota !');</script>");
				return;
			}

			if(lbl_statiIniziali.Text == "")
			{
				//RegisterStartupScript("statiIniziali","<script>alert('Il Diagramma non può non avere STATI INIZIALI !');</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "statiIniziali", "<script>alert('Il Diagramma non può non avere STATI INIZIALI !');</script>");
				return;			
			}

            if ( ((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).PASSI == null || ((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"]).PASSI.Length == 0)
            {
                //RegisterStartupScript("passi", "<script>alert('Il Diagramma non può non avere PASSI !');</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "passi", "<script>alert('Il Diagramma non può non avere PASSI !');</script>");
                return;						
            }

            if (controllaStatiInizialiFinali( (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"] ) )
            {
                //RegisterStartupScript("passi", "<script>alert('Stati INIZIALI e stati FINALI non possono coincidere !');</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "statiInizialiFinali", "<script>alert('Stati INIZIALI e stati FINALI non possono coincidere !');</script>");
                return;						
            }
            string msg = string.Empty;
            if (controllaStatiEProcessiFirma((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"], out msg))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "statiInizialiFinali", "<script>alert('" + msg.Replace("'", "\\'") + "');</script>");
                return;
            }

			//Verifico se è da effettuare una modifica di diagramma o un salvatagggio di un nuovo diagramma
			//Salvataggio nuovo diagramma
			if( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).SYSTEM_ID == 0)
			{
                if (!DiagrammiManager.isUniqueNameDiagramma(txt_descrizione.Text,this))
				{
					RegisterStartupScript("statiSuccessivi","<script>alert('Descrizione Diagramma già esistente !');</script>");
					return;			
				}	
				((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).ID_AMM = Convert.ToInt32(idAmministrazione);
				((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).DESCRIZIONE = txt_descrizione.Text;
                DiagrammiManager.salvaDiagramma((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"], idAmministrazione,this);

                listaDiagrammi = DiagrammiManager.getDiagrammi(idAmministrazione, this);

                Panel_GestioneStati.Visible = false;
				Panel_ListaPassi.Visible = false;
				caricaDgListaDiagrammi();
				dg_listaDiagrammi.SelectedIndex = -1;
				Panel_ListaDiagrammi.Visible = true;
				btn_salva.Visible = false;
				btn_nuovoDiagr.Visible = true;
			}
			//Modifica diagramma esistente
			else
			{
				if( ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).DESCRIZIONE != txt_descrizione.Text)
				{
                    if (!DiagrammiManager.isUniqueNameDiagramma(txt_descrizione.Text,this))
					{
						RegisterStartupScript("statiSuccessivi","<script>alert('Descrizione Diagramma già esistente !');</script>");
						return;			
					}	
				}
				((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).DESCRIZIONE = txt_descrizione.Text;
                DiagrammiManager.updateDiagramma((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"],this);

                listaDiagrammi = DiagrammiManager.getDiagrammi(idAmministrazione, this);

                Panel_GestioneStati.Visible = false;
				Panel_ListaPassi.Visible = false;
				caricaDgListaDiagrammi();
				dg_listaDiagrammi.SelectedIndex = -1;
				Panel_ListaDiagrammi.Visible = true;
				btn_salva.Visible = false;
				btn_nuovoDiagr.Visible = true;
			}
			lbl_titolo.Text = "Lista Diagrammi di stato";
		}

		private void btn_listaDiagrammi_Click(object sender, System.EventArgs e)
		{
			Panel_GestioneStati.Visible = false;
			Panel_ListaPassi.Visible = false;

            listaDiagrammi = DiagrammiManager.getDiagrammi(idAmministrazione, this);

            caricaDgListaDiagrammi();
			Panel_ListaDiagrammi.Visible = true;
			btn_salva.Visible = false;
			btn_nuovoDiagr.Visible = true;
			dg_listaDiagrammi.SelectedIndex = -1;
			lbl_titolo.Text = "Lista Diagrammi di stato";

		}

        private void ddl_stati_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            lbox_stati1.Items.Clear();
            lbox_stati2.Items.Clear();
            ddl_statiAutomatici.Items.Clear();
            ddl_statiAutomatici.Items.Add("");

            ddl_statiAutomaticiProcessoFirma.Items.Clear();
            ddl_statiAutomaticiProcessoFirma.Items.Add("");

            int elSelezionato = ddl_stati.SelectedIndex;

            DocsPAWA.DocsPaWR.DiagrammaStato diagramma = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaStato"];
            DocsPAWA.DocsPaWR.Stato statoSelezionato = (from s in diagramma.STATI where s.DESCRIZIONE.Equals(ddl_stati.SelectedItem.Text)
                                                        select s).FirstOrDefault();
            if(!string.IsNullOrEmpty(statoSelezionato.ID_PROCESSO_FIRMA) && !DiagrammiManager.IsProcessoFirmaConCambioStato(statoSelezionato.ID_PROCESSO_FIRMA, this))
            {
                PnlStatoAutoProcesso.Visible = true;
            }
            else
            {
                //Se il processo procede passi di tipo cambio stato non mostro la combo per cambio stato in caso di conclusione del processo
                PnlStatoAutoProcesso.Visible = false;
            }

            for (int i = 0; i < ddl_stati.Items.Count; i++)
            {
                if (i != elSelezionato)
                {
                    lbox_stati1.Items.Add(ddl_stati.Items[i].Text);
                }
            }

            btn_delStato.Visible = true;
            btn_modStato.Visible = true;
            dg_listaPassi.SelectedIndex = -1;
            btn_modPasso.Visible = false;
            btn_addStep.Visible = true;
        }
        
		#endregion

		#region DataGrid ListaPassi
		public void caricaDgListaPassi()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Stato");
			dt.Columns.Add("Stati Successivi");

			DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"];
			
			if(dg.PASSI != null)
			{
				for(int i=0; i<dg.PASSI.Length; i++)
				{
					DataRow dr = dt.NewRow();
					DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo) dg.PASSI[i];
					dr["Stato"] = step.STATO_PADRE.DESCRIZIONE;
					string statiSuccessivi = "";
					
					if(step.SUCCESSIVI != null)
					{
						for(int j=0; j<step.SUCCESSIVI.Length; j++)
						{
							statiSuccessivi += ((DocsPAWA.DocsPaWR.Stato) step.SUCCESSIVI[j]).DESCRIZIONE + " - ";
						}
						statiSuccessivi = statiSuccessivi.Substring(0,statiSuccessivi.Length-2);
						dr["Stati Successivi"] = statiSuccessivi;
						dt.Rows.Add(dr);
					}
				}
			}

			dg_listaPassi.DataSource = dt;
			
			dg_listaPassi.DataBind();
			dg_listaPassi.SelectedIndex = -1;
			Panel_ListaPassi.Visible = true;
		}

		private void dg_listaPassi_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int oggettoSelezionato = (dg_listaPassi.CurrentPageIndex * dg_listaPassi.PageSize) + e.Item.ItemIndex;
			DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"];
			string statoIniziale = ((DocsPAWA.DocsPaWR.Passo) dg.PASSI[oggettoSelezionato]).STATO_PADRE.DESCRIZIONE;
			//string statoIniziale = dg_listaPassi.Items[oggettoSelezionato].Cells[0].Text;
			
			if(dg.PASSI != null)
			{
				for(int i=0; i<dg.PASSI.Length; i++)
				{
					DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo) dg.PASSI[i];
					if(step.STATO_PADRE.DESCRIZIONE == statoIniziale)
					{
						rimuoviPasso(i);
					}
				}
			}

			Session.Add("DiagrammaStato",dg);
			
			if(dg_listaPassi.Items.Count == 0)
				Panel_ListaPassi.Visible = false;
			else
				dg_listaPassi.CurrentPageIndex = 0;
			
			caricaDgListaPassi();
			dg_listaPassi.Columns[2].Visible = true;
			dg_listaPassi.Columns[3].Visible = true;

			//Imposta le label che indicano istantaneamente quali sono gli stati iniziali e finali
			//del diagramma che si sta costruendo, avvalendosi delle funzioni:
			//"trovaStatoIniziale" - "trovaStatoFinale"
			
			//settaStatiIniziali(trovaStatiIniziali());
			settaStatiFinali(trovaStatiFinali());
			btn_modPasso.Visible = false;
			btn_addStep.Visible = true;
		}

		private void dg_listaPassi_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dg_listaPassi.CurrentPageIndex = e.NewPageIndex;
			caricaDgListaPassi();
			dg_listaPassi.Columns[2].Visible = true;
			dg_listaPassi.Columns[3].Visible = true;
			btn_modPasso.Visible = false;
			btn_addStep.Visible = true;
			dg_listaPassi.SelectedIndex = -1;
		}
				
		private void dg_listaPassi_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btn_modPasso.Visible = true;
			btn_addStep.Visible = false;

			int elSelezionato = (dg_listaPassi.CurrentPageIndex * dg_listaPassi.PageSize) + dg_listaPassi.SelectedIndex;
			
			DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo) ((DocsPAWA.DocsPaWR.DiagrammaStato) Session["DiagrammaStato"]).PASSI[elSelezionato];
			setDllStati(step.STATO_PADRE);
			setLbxStati1(step.STATO_PADRE,step.SUCCESSIVI);		
			setLbxStati2(step.SUCCESSIVI);
			setDdlStatiAutomatici(step);
            setDdlStatiAutomaticiProcessoFirma(step);

        }
		#endregion

		#region DataGrid ListaDiagrammi
		public void caricaDgListaDiagrammi()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("System_id");
			dt.Columns.Add("Nome diagramma");

			for(int i=0; i<listaDiagrammi.Count; i++)
			{
				DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato) listaDiagrammi[i];
				DataRow dr = dt.NewRow();
				dr["System_id"] = dg.SYSTEM_ID;
				dr["Nome diagramma"] = dg.DESCRIZIONE;
				dt.Rows.Add(dr);
			}
			dg_listaDiagrammi.DataSource = dt;			
			dg_listaDiagrammi.DataBind();
		}

		private void dg_listaDiagrammi_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int elSelezionato = (dg_listaDiagrammi.CurrentPageIndex * dg_listaDiagrammi.PageSize) + dg_listaDiagrammi.SelectedIndex;
			
			//Controllo se il diagramma puo' essere modificato o meno
			//a seconda se associato o no ad un tipo documento.
			//Verifico quindi la valorizzazione della proprietà "TIPO_DOCUMENTO".
			DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato) listaDiagrammi[elSelezionato];
			Session.Add("DiagrammaStato",dg);

			Panel_ListaDiagrammi.Visible = false;
			dg_listaDiagrammi.SelectedIndex = -1;
			Panel_GestioneStati.Visible = true;
			btn_nuovoDiagr.Visible = false;
			btn_salva.Visible = true;
			txt_descrizione.Text = dg.DESCRIZIONE;
			lbox_stati1.Items.Clear();
			lbox_stati2.Items.Clear();
			ddl_statiAutomatici.Items.Clear();
            ddl_statiAutomaticiProcessoFirma.Items.Clear();
            /*
            ddl_stati.Items.Clear();
			for(int i=0; i<dg.STATI.Length; i++)
			{
				DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato) dg.STATI[i];
				ListItem li = new ListItem();
				li.Value = Convert.ToString(st.SYSTEM_ID);
				li.Text = st.DESCRIZIONE;
				ddl_stati.Items.Add(li);
				//ddl_stati.Items.Add( ((DocsPAWA.DocsPaWR.Stato) dg.STATI[i]).DESCRIZIONE);
			}
			*/
            caricaDdlStati();

			//settaStatiIniziali(trovaStatiIniziali());
			settaStatiInizialiNew();
			settaStatiFinali(trovaStatiFinali());
			caricaDgListaPassi();
			dg_listaPassi.Columns[2].Visible = true;
			dg_listaPassi.Columns[3].Visible = true;
			Panel_ListaPassi.Visible = true;
			lbl_titolo.Text = "Gestione Diagramma di stato";
		}

        private void dg_listaDiagrammi_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName.ToUpper().Equals("VISIBILITA"))
            {
                int elSelezionato = (dg_listaDiagrammi.CurrentPageIndex * dg_listaDiagrammi.PageSize) + e.Item.ItemIndex;
                DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)listaDiagrammi[elSelezionato];
                Session.Add("DiagrammaStato", dg);
                string s = "<SCRIPT language='javascript'>ApriPopupVisibilita(); </SCRIPT>";
                RegisterStartupScript("popupVis", s);
            }
        }

		private void dg_listaDiagrammi_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int elSelezionato = (dg_listaDiagrammi.CurrentPageIndex * dg_listaDiagrammi.PageSize) + e.Item.ItemIndex;
			DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato) listaDiagrammi[elSelezionato];
			Session.Add("DiagrammaStato",dg);

			//Diagramma non cacellabile
			//if(dg.ID_TIPO_ATTO != 0)
            if (!DiagrammiManager.isModificabile(dg.SYSTEM_ID,this))
			{
				RegisterStartupScript("scriptModDiagramma","<script>alert('Il diagramma non può essere eliminato, è necessario disassociarlo dalla tipologia di documento !'); </script>");
				dg_listaDiagrammi.SelectedIndex = -1;
				Panel_GestioneStati.Visible = false;
				Panel_ListaPassi.Visible = false;
				btn_salva.Visible = false;
				return;
			}
			//Diagramma cancellabile
			else
			{
                DiagrammiManager.delDiagramma(dg,this);
                listaDiagrammi = DiagrammiManager.getDiagrammi(idAmministrazione, this);

                dg_listaDiagrammi.CurrentPageIndex = 0;
				caricaDgListaDiagrammi();
				Panel_GestioneStati.Visible = false;
				Panel_ListaPassi.Visible = false;
				btn_salva.Visible = false;
				dg_listaDiagrammi.SelectedIndex = -1;
			}            
		
		}	
		
		private void dg_listaDiagrammi_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dg_listaDiagrammi.CurrentPageIndex = e.NewPageIndex;
			caricaDgListaDiagrammi();
			dg_listaDiagrammi.SelectedIndex = -1;
			Panel_ListaPassi.Visible = false;
		}		
		#endregion

		#region SetFocus
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}
		#endregion SetFocus		

		#region ItemCreateDatagrid
        protected void dg_listaDiagrammi_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }

        protected void dg_listaPassi_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }
        #endregion
    }
}
