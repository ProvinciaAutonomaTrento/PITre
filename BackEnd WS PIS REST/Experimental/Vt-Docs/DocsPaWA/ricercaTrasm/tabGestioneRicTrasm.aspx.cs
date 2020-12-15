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

namespace DocsPAWA.ricercaTrasm
{
	/// <summary>
	/// Summary description for tabGestioneRicTrasm.
	/// </summary>
	/// 
	

	public class tabGestioneRicTrasm : DocsPAWA.CssPage
	{
		
		protected System.Web.UI.WebControls.RadioButtonList RadioRE;
		protected System.Web.UI.WebControls.ImageButton btn_completa;
		protected System.Web.UI.WebControls.ImageButton btn_toDoList;
	
		protected string queryStringPar_Verso;
		protected DocsPaWebCtrlLibrary.IFrameWebControl IframeTabs;
		protected System.Web.UI.WebControls.Label lbl_ruolo;
		protected string queryStringPar_Tab;

		private bool _onBack=false;
        protected string Tema;

		enum typeVersoTrasm
		{
			Ricevute,
			Effettuate
		}	


		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			try
			{
                string TemaAll = GetCssAmministrazione();
                if (TemaAll != null && !TemaAll.Equals(""))
                {
                    string[] realTema = TemaAll.Split('^');
                    this.Tema = realTema[0];
                }
                else
                    this.Tema = "TemaRosso";
                
                Utils.startUp(this);
				if (!IsPostBack)
				{
					//al primo caricamento, imposto i parametri dello
					//stato in base alla query string, o, se 
					//questa è vuota, in base a valori default;
					aggiornaStatoQueryStringParameter();
					if (queryStringPar_Verso=="R")
						this.RadioRE.SelectedIndex=0;
					else
						this.RadioRE.SelectedIndex=1;

                    if (Request.QueryString["verso"] != null)
                        if(Request.QueryString["verso"].Equals("R"))
                            this.RadioRE.SelectedIndex = 0;
                        else
                            this.RadioRE.SelectedIndex = 1;

                    refreshTabByClick(this.GetSearchButton(), false);
				 
					//carica il ruolo scelto
					this.lbl_ruolo.Text = UserManager.getRuolo(this).descrizione;
				}
				else
				{
					reloadStato();
				}

                //deprecato
                this.btn_toDoList.Visible = false;
				
			}
			catch (System.Exception ex)
			{
				ErrorManager.redirect(this, ex);
			}
		}

        private string GetCssAmministrazione()
        {
            string result = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                result = userM.getCssAmministrazione(idAmm);
            }
            else
            {
                if (UserManager.getInfoUtente() != null)
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    result = userM.getCssAmministrazione(idAmm);
                }
            }
            return result;
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
			this.RadioRE.SelectedIndexChanged += new System.EventHandler(this.RadioRE_SelectedIndexChanged);
			this.btn_completa.Click += new System.Web.UI.ImageClickEventHandler(this.btn_completa_Click);
			this.btn_toDoList.Click += new System.Web.UI.ImageClickEventHandler(this.btn_toDoList_Click);
			this.IframeTabs.Navigate += new System.EventHandler(this.IframeTabs_Navigate);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
		
		private void reloadStato()
		{
			queryStringPar_Tab=getSelectedTab();
			queryStringPar_Verso=getVersoTrasmToString(getVersoTrasm());
		}


		private string getSelectedTab()
		{
			string nameButtonSel="";
			string retFunction="";
			if(ViewState["ID_Butt_precedente"]!=null)
			{
				nameButtonSel=(string)ViewState["ID_Butt_precedente"];
				retFunction=nameButtonSel.Substring(nameButtonSel.IndexOf("_")+1);
			}
			return retFunction;
		}


		private void aggiornaStatoQueryStringParameter()
		{
			if (Request.QueryString["tab"]!=null)
			{
				queryStringPar_Tab=(string)Request.QueryString["tab"];
			}
			else
			{
				queryStringPar_Tab=getTabNameByButton(this.btn_completa);
			}
			
			if (Request.QueryString["verso"]!=null)
			{
				queryStringPar_Verso=(string)Request.QueryString["verso"];
			}
			else
			{
				queryStringPar_Verso=getVersoTrasmToString(typeVersoTrasm.Ricevute);
			}
			if (Request.QueryString["back"]!=null)
			{
				this._onBack=(Request.QueryString["back"].ToString().ToLower()=="true");
			}			
		}


		private void refreshDirection()
		{
			typeVersoTrasm versoTrasm=getVersoTrasm();
			refreshDirectionByState(versoTrasm);
		}

		private void aggiornaPaginaNelFrame(string tab,string direction) 
		{
            // Aggiornamento tab nel contesto corrente
            this.RefreshCurrentContextTabName(tab);

			//aggiorna frame di sinistra
			string queryString;
			queryString="tab="+tab+"&verso="+direction;
            if (Request.QueryString["oneTime"] != null)
                queryString += "&oneTime=1";

			if (this._onBack)
			{	 
				queryString+="&back=true&docIndex=" + this.Request.QueryString["docIndex"];
			}
			else
			{
				//aggiorna il frame di destra
				Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='tabRisultatiRicTrasm.aspx?tiporic="+direction+"&home=N';</script>");
			}
			this.IframeTabs.NavigateTo="ricTrasm.aspx?"+queryString;
		}

		private void refreshDirectionByState(typeVersoTrasm versoTrasm)
		{
			string toDoListAndAssPend_AltText;
			string toDoListAndAssPend_ImageUrl;

			if (versoTrasm==typeVersoTrasm.Effettuate)
			{
				toDoListAndAssPend_AltText="Assegnazioni pendenti";
				//toDoListAndAssPend_ImageUrl="../images/ricerca/assegnazioneDipendenti_nonattivo.gif";
                toDoListAndAssPend_ImageUrl = "../App_Themes/ImgComuni/assegnazioneDipendenti_nonattivo.gif";
			}
			else
			{
				toDoListAndAssPend_AltText="To do list";
				//toDoListAndAssPend_ImageUrl="../images/ricerca/todoList_nonattivo.gif";
                toDoListAndAssPend_ImageUrl = "../App_Themes/ImgComuni/todoList_nonattivo.gif";
			}
			queryStringPar_Verso=getVersoTrasmToString(versoTrasm);
			this.btn_toDoList.AlternateText=toDoListAndAssPend_AltText;
			this.btn_toDoList.ImageUrl=toDoListAndAssPend_ImageUrl;

			refreshTabByClick(this.btn_completa,true);
		}


        private ImageButton GetButtonByTabName(string tabName)
        {
            string buttonId = "btn_" + tabName;

            return this.FindControl(buttonId) as ImageButton;
        }

		private string getTabNameByButton(ImageButton btn)
		{
			string strNumTab=btn.ID.Substring(btn.ID.IndexOf("_")+1);
			return strNumTab;
		}

		
		private typeVersoTrasm getVersoTrasm()
		{
			typeVersoTrasm versoTrasm=typeVersoTrasm.Effettuate;
			if (this.RadioRE.SelectedItem.Value.Equals("E")) 
			{
				versoTrasm=typeVersoTrasm.Effettuate;
			}
			else if (this.RadioRE.SelectedItem.Value.Equals("R"))
			{
				versoTrasm=typeVersoTrasm.Ricevute;
			}
			return versoTrasm;
		}
		
		
		private string getVersoTrasmToString(typeVersoTrasm versoTrasm)
		{
			string retFunction="";
			if (versoTrasm==typeVersoTrasm.Effettuate) 
			{
				retFunction="E";
			}
			else if (versoTrasm==typeVersoTrasm.Ricevute)
			{
				retFunction="R";
			}
			return retFunction;
		}

		
		private void refreshTabByClick(ImageButton btn,bool removeTrasmFilters)
		{
			try
			{
				setImageForTabState(btn);
				string tab=getTabNameByButton(btn);
				string direction=getVersoTrasmToString(getVersoTrasm());
				
				aggiornaPaginaNelFrame(tab,direction);	

				CleanSessionRisultatiRicerca();
				
				if (removeTrasmFilters)
				{
					DocumentManager.removeFiltroRicTrasm(this);
					TrasmManager.removeMemoriaFiltriRicTrasm(this);
				}
			}
			catch (System.Exception ex)
			{
				ErrorManager.redirect(this, ex);
			}
        }

        private void setImageForTabState(ImageButton btn)
		{
			string imageAttiva="";
			string imageNonAttiva="";
			typeVersoTrasm versoTrasm=getVersoTrasm();
			queryStringPar_Verso=getVersoTrasmToString(versoTrasm);
			queryStringPar_Tab=getTabNameByButton(btn);
			//solo se già un butt è stato cliccato
			if(ViewState["ID_Butt_precedente"]!=null)
			{
				//img del butt precedentemente cliccato torna a nonattivo.	
				ImageButton btnPrec=((ImageButton)Page.FindControl((string)ViewState["ID_Butt_precedente"]));			
				if (btnPrec.ID==this.btn_completa.ID)
				{
					//imageNonAttiva="../images/ricerca/completa_nonattivo.gif";
                    imageNonAttiva = "../App_Themes/ImgComuni/completa_nonattivo.gif";
					if (queryStringPar_Verso=="R")
					{
						//this.btn_toDoListAndAssPend.ImageUrl="../images/ricerca/todolist_nonattivo.gif";
                        this.btn_toDoList.ImageUrl = "../App_Themes/ImgComuni/todolist_nonattivo.gif";
					}
					else
					{
						//this.btn_toDoListAndAssPend.ImageUrl="../images/ricerca/assegnazionedipendenti_nonattivo.gif";
                        this.btn_toDoList.ImageUrl = "../App_Themes/ImgComuni/assegnazionedipendenti_nonattivo.gif";
					}
				}
				else
				{
					if (versoTrasm==typeVersoTrasm.Effettuate)
					{
						//imageNonAttiva="../images/ricerca/assegnazionedipendenti_nonattivo.gif";
                        imageNonAttiva = "../App_Themes/ImgComuni/assegnazionedipendenti_nonattivo.gif";
					}
					else if (versoTrasm==typeVersoTrasm.Ricevute)
					{
						//imageNonAttiva="../images/ricerca/todolist_nonattivo.gif";
                        imageNonAttiva = "../App_Themes/ImgComuni/todolist_nonattivo.gif";
					}
				}
				btnPrec.ImageUrl=imageNonAttiva;
			}
			else
			{
				if (versoTrasm==typeVersoTrasm.Effettuate)
				{
					//imageNonAttiva="../images/ricerca/assegnazionedipendenti_nonattivo.gif";
                    imageNonAttiva = "../App_Themes/ImgComuni/assegnazionedipendenti_nonattivo.gif";
				}
				else if (versoTrasm==typeVersoTrasm.Ricevute)
				{
					//imageNonAttiva="../images/ricerca/todolist_nonattivo.gif";
                    imageNonAttiva = "../App_Themes/ImgComuni/todolist_nonattivo.gif";
				}
				this.btn_toDoList.ImageUrl=imageNonAttiva;
				//this.btn_completa.ImageUrl=this.btn_completa.ImageUrl.Substring(0, this.btn_completa.ImageUrl.IndexOf("_"))+"_nonattivo.gif";
                this.btn_completa.ImageUrl = this.btn_completa.ID.Substring(this.btn_completa.ImageUrl.IndexOf("_") + 1) + "_nonattivo.gif";
			}

			//serve per segnare la prima volta che si click un bottone.
			if(Page.IsPostBack)
				ViewState.Add("ID_Butt_precedente",btn.ID);
				
			//cambio img del butt cliccato
			
			//imageAttiva=btn.ImageUrl.Substring(0, btn.ImageUrl.IndexOf("_"))+"_attivo.gif";
            imageAttiva = "../App_Themes/" + this.Tema + "/" + btn.ID.Substring(btn.ID.IndexOf("_") + 1) + "_attivo.gif";

			btn.ImageUrl=imageAttiva;
		}

		
		private void btn_completa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			refreshTabByClick(this.btn_completa,true);
			//aggiornaPaginaNelFrame(queryStringPar_Tab,queryStringPar_Verso);
		}

		
		private void btn_toDoList_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			refreshTabByClick(this.btn_toDoList,true);
		}

		private void RadioRE_SelectedIndexChanged(object sender, System.EventArgs e)
		{

            Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY] = null;

			refreshTabByClick(this.btn_completa,true);

			UserManager.removeCorrispondentiSelezionati(this);
		}

		private void CleanSessionRisultatiRicerca()
		{
			TrasmManager.removeDocTrasmSel(this);
			TrasmManager.removeDocTrasmQueryEff(this);
			TrasmManager.removeDataTableRic(this);
			TrasmManager.removeDataTableEff(this);
			TrasmManager.removeDataTableRic(this);
			TrasmManager.removeDocTrasmSel(this);
		}

		private void IframeTabs_Navigate(object sender, System.EventArgs e) 
		{
		
		}

        #region Gestione callcontext

        /// <summary>
        /// Aggiornamento chiave del tab selezionato nel contesto corrente
        /// </summary>
        /// <param name="tabName"></param>
        private void RefreshCurrentContextTabName(string tabName)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI)
                currentContext.QueryStringParameters["tab"] = tabName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ImageButton GetSearchButton()
        {
            ImageButton btn=null;
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI && currentContext.IsBack)
            {
                if (currentContext.QueryStringParameters.ContainsKey("tab"))
                    btn = this.GetButtonByTabName(currentContext.QueryStringParameters["tab"].ToString());
            }
            else
            {
                btn=this.btn_completa;
            }

            return btn;
        }

        #endregion
	}
}