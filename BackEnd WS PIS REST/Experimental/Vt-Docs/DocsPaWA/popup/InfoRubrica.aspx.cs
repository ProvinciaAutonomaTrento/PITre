
	
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

		namespace DocsPAWA.popup
				  {
		/// <summary>
		/// Summary description for dettagliCorrispondenti.
		/// </summary>
            public class InfoRubrica : DocsPAWA.CssPage
		{
			protected System.Web.UI.WebControls.Label lbl_nomeCorr;
			protected System.Web.UI.WebControls.Label lbl_DettCorr;
				
		
	
			private void Page_Load(object sender, System.EventArgs e)
			{
				//carica_Info
				try
				{
					string codCor = Request.QueryString["codCor"];
					if ((codCor == null) || (codCor == ""))
						return;		
					getInfoCor(codCor);
				}
				catch (System.Exception ex)
				{
					ErrorManager.redirect(this, ex);
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
				this.Load += new System.EventHandler(this.Page_Load);

			}
		#endregion

		
			private void getInfoCor(string codiceRubrica) 
			{
				try
				{
					DocsPaWR.Corrispondente corr = UserManager.getCorrispondente(this, codiceRubrica,false); //?? 
					if(corr!=null)
					{
						drawInfoCor(corr);
						drawDettagliCorr(corr);
					}
					else
						this.lbl_DettCorr.Text="Corrispondente Inesistente";
				}
				catch (Exception ex) 
				{
					ErrorManager.redirect(this, ex);
				}
			}
		
			private void drawInfoCor(DocsPAWA.DocsPaWR.Corrispondente myCorr) 
			{			
				try
				{
					if (myCorr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo)) 
					{
						DocsPaWR.Ruolo corrRuolo = (DocsPAWA.DocsPaWR.Ruolo) myCorr;
						string descrUO = "";
				
						DocsPaWR.UnitaOrganizzativa corrUO;
						corrUO = corrRuolo.uo;

						while(corrUO!=null) 
						{
							descrUO = descrUO+"&nbsp;-&nbsp;"+ corrUO.descrizione;
							corrUO=corrUO.parent;
						}
					
						this.lbl_nomeCorr.Text = corrRuolo.descrizione + descrUO;
					} 
					else 
						if (myCorr.GetType() == typeof(DocsPAWA.DocsPaWR.Utente)) 
					{
						DocsPaWR.Utente corrUtente = (DocsPAWA.DocsPaWR.Utente) myCorr;
						DocsPaWR.Ruolo corrRuolo;
						if(corrUtente.ruoli!=null && corrUtente.ruoli.Length>0)
						{
							corrRuolo = (DocsPAWA.DocsPaWR.Ruolo) corrUtente.ruoli[0];
						}

						lbl_nomeCorr.Text= corrUtente.descrizione;
					}
					else
						if (myCorr.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)) 
					{
						DocsPaWR.UnitaOrganizzativa corrUnitOrg = (DocsPAWA.DocsPaWR.UnitaOrganizzativa) myCorr;
						string descrUO = "";
				
						DocsPaWR.UnitaOrganizzativa corrUO;
						corrUO = corrUnitOrg.parent;

						while(corrUO!=null) 
						{
							descrUO = descrUO+"&nbsp;-&nbsp;"+ corrUO.descrizione;
							corrUO=corrUO.parent;
						}
					
						this.lbl_nomeCorr.Text = corrUnitOrg.descrizione + descrUO;
					} 
				}
				catch (Exception ex) 
				{
					ErrorManager.redirect(this, ex);
				}							
			}

			private void drawDettagliCorr(DocsPAWA.DocsPaWR.Corrispondente corr) 
			{
				try
				{
					corr.info = UserManager.getDettagliCorrispondente(this,corr.systemId);
					if (corr.info == null && (corr.email == null || corr.email.Equals("")))
						return;
					string txtTable = "";
					txtTable += "<table width='95%' align='center' border='0' cellpadding='1' cellspacing='1'>";
					//txtTable += addRowVuota();
					txtTable += addHeaderRow();
					txtTable += addRow(corr);
					//txtTable += addRowVuota();
					txtTable += "<tr><td height='5'></td></tr></table>";

					this.lbl_DettCorr.Text = txtTable;
				}
				catch (Exception ex) 
				{
					ErrorManager.redirect(this, ex);
				}
			}

			private string addRow(DocsPAWA.DocsPaWR.Corrispondente corr) 
			{
				DocsPaVO.addressbook.DettagliCorrispondente dettagliCorr = new DocsPaVO.addressbook.DettagliCorrispondente();
				DocsPaUtils.Data.TypedDataSetManager.MakeTyped(corr.info, dettagliCorr.Corrispondente.DataSet);

				return "<TR height='100%' bgcolor=#d9d9d9><TD class='testo_grigio'>&nbsp;" + dettagliCorr.Corrispondente[0].indirizzo+ "&nbsp;</TD><TD class='testo_grigio' >&nbsp;" + dettagliCorr.Corrispondente[0].citta+ "&nbsp;</TD><TD class='testo_grigio' >&nbsp;" + dettagliCorr.Corrispondente[0].cap+ "&nbsp;</TD><TD class='testo_grigio' >&nbsp;" + dettagliCorr.Corrispondente[0].provincia+ "&nbsp;</TD><TD class='testo_grigio' >&nbsp;" + dettagliCorr.Corrispondente[0].fax+ "&nbsp;</TD><TD class='testo_grigio'>&nbsp;" + dettagliCorr.Corrispondente[0].telefono+ "&nbsp;</TD><td class='testo_grigio' width='30'>&nbsp;"+ corr.email +"&nbsp;</td></TR>";
			}
			private string addHeaderRow() 
			{
				return "<TR class='testo_bianco' bgcolor='#4b4b4b'><TD>&nbsp;Indirizzo&nbsp;</TD><TD>&nbsp;Citta&nbsp;</TD><TD>&nbsp;Cap&nbsp;</TD><TD>&nbsp;Prov.&nbsp;</TD><TD>&nbsp;Fax&nbsp;</TD><TD>&nbsp;Tel.&nbsp;</TD><TD>&nbsp;Email&nbsp;</TD></tr>";
			}
			
			/*private string addRowRossa()
			{
				return "<TR class='menu_1_bianco' height='10'  bgcolor='#c08682'><TD colspan='9'>&nbsp;</TD></tr>";
			}*/

			private string addRowVuota()
			{
				return "<TR><TD height='5'>&nbsp;</TD></tr>";			
			}
		}

	}


		

