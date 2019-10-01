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
using System.Globalization;

namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for docProtocolloSemplice.
	/// </summary>
	public class docProtocolloSemplice : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.RadioButtonList rbl_InOut_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_stampaSegn_P;
		protected System.Web.UI.WebControls.TextBox txt_dataSegn;
		protected System.Web.UI.WebControls.TextBox lbl_segnatura;
		protected DocsPaWebCtrlLibrary.ImageButton btn_RubrOgget_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_modificaOgget_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_storiaOgg_P;
		protected System.Web.UI.WebControls.TextBox txt_oggetto_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_RubrMit_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_DetMit_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_ModMit_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_storiaMit_P;
		protected System.Web.UI.WebControls.TextBox txt_CodMit_P;
		protected System.Web.UI.WebControls.TextBox txt_DescMit_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_RubrMitInt_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_DetMitInt_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_ModMitInt_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_storiaMitInt_P;
		protected System.Web.UI.WebControls.TextBox txt_CodMitInt_P;
		protected System.Web.UI.WebControls.TextBox txt_DescMitInt_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_verificaPrec_P;
		protected System.Web.UI.WebControls.TextBox txt_DataProtMit_P;
		protected System.Web.UI.WebControls.TextBox txt_NumProtMit_P;
		protected System.Web.UI.WebControls.Panel panel_Mit;
		protected DocsPaWebCtrlLibrary.ImageButton btn_RubrDest_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_ModDest_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungiDest_P;
		protected System.Web.UI.WebControls.TextBox txt_CodDest_P;
		protected System.Web.UI.WebControls.TextBox txt_DescDest_P;
		protected System.Web.UI.WebControls.ListBox lbx_dest;
		protected DocsPaWebCtrlLibrary.ImageButton btn_dettDest;
		protected DocsPaWebCtrlLibrary.ImageButton btn_cancDest;
		protected DocsPaWebCtrlLibrary.ImageButton btn_insDestCC;
		protected DocsPaWebCtrlLibrary.ImageButton btn_insDest;
		protected System.Web.UI.WebControls.ListBox lbx_destCC;
		protected DocsPaWebCtrlLibrary.ImageButton btn_dettDestCC;
		protected DocsPaWebCtrlLibrary.ImageButton btn_cancDestCC;
		protected System.Web.UI.WebControls.Panel panel_Dest;
		protected DocsPaWebCtrlLibrary.ImageButton btn_adl_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_catenaProt_P;
		protected System.Web.UI.WebControls.TextBox txt_RispProtSegn_P;
		protected System.Web.UI.WebControls.TextBox txt_dataAnnul_P;
		protected System.Web.UI.WebControls.TextBox txt_numAnnul_P;
		protected System.Web.UI.WebControls.Panel panel_Annul;
		protected System.Web.UI.WebControls.Label lbl_ADL_SPED;
		protected DocsPaWebCtrlLibrary.ImageButton btn_salva_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_protocolla_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_protocollaGiallo_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungi_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_spedisci_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_riproponiDati_P;
		protected DocsPaWebCtrlLibrary.ImageButton btn_annulla_P;
		protected System.Web.UI.HtmlControls.HtmlTable tblMitt;
		protected System.Web.UI.HtmlControls.HtmlTable Table1;
		protected System.Web.UI.WebControls.Panel activeX;
		protected System.Web.UI.WebControls.Panel panel_mitInt;
		protected System.Web.UI.WebControls.TextBox txt_DataArrivo_P;
		
		private DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;

	
		private void Page_Load(object sender, System.EventArgs e)
		{
			try 
			{
				Utils.startUp(this);
				schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
				
				if (schedaDocumento == null)
					schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

				if (schedaDocumento!=null)
				{
					if (schedaDocumento.registro == null)
						schedaDocumento.registro = UserManager.getRegistroSelezionato(this);

					if (schedaDocumento.protocollo == null)
						setProtoArrivo();

					setDataProtocollo();
					enableEditableFields();
				}

				

			} 
			catch (Exception ex) 
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
			this.rbl_InOut_P.SelectedIndexChanged += new System.EventHandler(this.rbl_InOut_P_SelectedIndexChanged);
			this.txt_dataSegn.TextChanged += new System.EventHandler(this.txt_dataSegn_TextChanged);
			this.btn_modificaOgget_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modificaOgget_P_Click);
			this.btn_storiaOgg_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_storiaOgg_P_Click);
			this.txt_oggetto_P.TextChanged += new System.EventHandler(this.txt_oggetto_P_TextChanged);
			this.btn_DetMit_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_DetMit_P_Click);
			this.btn_ModMit_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ModMit_P_Click);
			this.txt_CodMit_P.TextChanged += new System.EventHandler(this.txt_CodMit_P_TextChanged);
			this.txt_DescMit_P.TextChanged += new System.EventHandler(this.txt_DescMit_P_TextChanged);
			this.btn_ModMitInt_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ModMitInt_P_Click);
			this.txt_CodMitInt_P.TextChanged += new System.EventHandler(this.txt_CodMitInt_P_TextChanged);
			this.txt_DataProtMit_P.TextChanged += new System.EventHandler(this.txt_DataProtMit_P_TextChanged);
			this.txt_NumProtMit_P.TextChanged += new System.EventHandler(this.txt_NumProtMit_P_TextChanged);
			this.btn_ModDest_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ModDest_P_Click);
			this.btn_aggiungiDest_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungiDest_P_Click);
			this.txt_CodDest_P.TextChanged += new System.EventHandler(this.txt_CodDest_P_TextChanged);
			this.btn_dettDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_dettDest_Click);
			this.btn_cancDest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDest_Click);
			this.btn_cancDestCC.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancDestCC_Click);
			this.btn_adl_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_adl_P_Click);
			this.btn_salva_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_salva_P_Click);
			this.btn_protocolla_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocolla_P_Click);
			this.btn_protocollaGiallo_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocollaGiallo_P_Click);
			this.btn_aggiungi_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungi_P_Click);
			this.btn_spedisci_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_spedisci_P_Click);
			this.btn_riproponiDati_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_riproponiDati_P_Click);
			this.btn_annulla_P.Click += new System.Web.UI.ImageClickEventHandler(this.btn_annulla_P_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.docProtocolloSemplice_PreRender);

		}
		#endregion

		private void docProtocolloSemplice_PreRender(object sender, System.EventArgs e) 
		{
			try 
			{
			
				//se il documento esiste, riempie i campi con i valori
				if (schedaDocumento != null) 
				{
		
					//oggetto
					this.txt_oggetto_P.Text = schedaDocumento.oggetto.descrizione;

					//protocollo in entrata o in uscita
					

					switch(schedaDocumento.tipoProto)
					{
						case "A":
							this.rbl_InOut_P.SelectedIndex = 0;
						
							DocsPaWR.Corrispondente mittInt;
							DocsPaWR.Corrispondente mitt;
							mittInt = ((DocsPAWA.DocsPaWR.ProtocolloEntrata) schedaDocumento.protocollo).mittenteIntermedio;
							mitt = ((DocsPAWA.DocsPaWR.ProtocolloEntrata) schedaDocumento.protocollo).mittente;

							//mittente 
							if (mitt != null) 
							{
								if (mitt.tipoCorrispondente == null || !mitt.tipoCorrispondente.Equals("O"))
									this.txt_CodMit_P.Text = mitt.codiceRubrica;
								else
									this.txt_CodMit_P.Text = "";
								this.txt_DescMit_P.Text = UserManager.getDecrizioneCorrispondente(this,mitt);
							} 
							else 
							{
								this.txt_CodMit_P.Text = "";
								this.txt_DescMit_P.Text = "";
							}
					
							//mittente intermedio per proto entrata
							if (mittInt != null) 
							{
								if (mittInt.tipoCorrispondente == null || !mittInt.tipoCorrispondente.Equals("O"))
									this.txt_CodMitInt_P.Text = mittInt.codiceRubrica;
								else
									this.txt_CodMitInt_P.Text = "";
								this.txt_DescMitInt_P.Text =  UserManager.getDecrizioneCorrispondente(this,mittInt);
							} 
							else 
							{
								this.txt_CodMitInt_P.Text = "";
								this.txt_DescMitInt_P.Text = "";
							}

							//nascondo le tabelle con i dati sui destinatari
							this.panel_Dest.Visible = false;
							this.panel_Mit.Visible = true;

							//cambio la label del pulsante della bottoniera
							this.lbl_ADL_SPED.Text = "ADL";
							this.btn_aggiungi_P.Visible = true;
							this.btn_spedisci_P.Visible = false;
						
							break;
						case "P":					
							this.lbx_dest.Items.Clear();
							this.lbx_destCC.Items.Clear();
							this.rbl_InOut_P.SelectedIndex = 1;					
							DocsPaWR.Corrispondente[] destUscita;
							destUscita = ((DocsPAWA.DocsPaWR.ProtocolloUscita) schedaDocumento.protocollo).destinatari;
							//devo costruire la tabella con l'elenco dei destinatari
							setListBoxDestinatari();
								
							//nascondo le tabelle con i dati sui mittenti
							this.panel_Dest.Visible = true;
							this.panel_Mit.Visible = false;

							//cambio la label del pulsante della bottoniera
							this.lbl_ADL_SPED.Text = "SPEDISCI";
							this.btn_aggiungi_P.Visible = false;
							this.btn_spedisci_P.Visible = true;

							break;
						case "I":					
							this.lbx_dest.Items.Clear();
							this.lbx_destCC.Items.Clear();
							this.rbl_InOut_P.SelectedIndex = 1;					
							DocsPaWR.Corrispondente[] destInterno;
							destInterno = ((DocsPAWA.DocsPaWR.ProtocolloInterno) schedaDocumento.protocollo).destinatari;
							//devo costruire la tabella con l'elenco dei destinatari
							setListBoxDestinatari();
								
							//nascondo le tabelle con i dati sui mittenti
							this.panel_Dest.Visible = true;
							this.panel_Mit.Visible = false;

							//cambio la label del pulsante della bottoniera
							this.lbl_ADL_SPED.Text = "SPEDISCI";
							this.btn_aggiungi_P.Visible = false;
							this.btn_spedisci_P.Visible = true;

							break;
					}

					//segnatura e data
					this.lbl_segnatura.Text = schedaDocumento.protocollo.segnatura;
					if (schedaDocumento.protocollo.dataProtocollazione != null && !schedaDocumento.protocollo.dataProtocollazione.Equals(""))
						this.txt_dataSegn.Text = schedaDocumento.protocollo.dataProtocollazione.Substring(0,10);
					if(!(schedaDocumento.protocollo.segnatura != null && !schedaDocumento.protocollo.segnatura.Equals("")))
					{
						//visualizzo uno dei due pulsanti per la protocollazione (normale o in giallo)
						string tipoReg = UserManager.getStatoRegistro(UserManager.getRegistroSelezionato(this));
						if (tipoReg.Equals("G"))
						{
							this.btn_protocollaGiallo_P.Visible = true;
							this.btn_protocolla_P.Visible = false;
						}
						else
							if (tipoReg.Equals("V"))
						{
							this.btn_protocollaGiallo_P.Visible = false;
							this.btn_protocolla_P.Visible = true;	
						}
						else 
						{
							this.btn_protocollaGiallo_P.Visible = false;
							this.btn_protocolla_P.Visible = true;
							this.btn_protocolla_P.Enabled = false;
							this.btn_protocollaGiallo_P.Enabled = false;
						}
						
					}
			
					//protocollo mittente 
					if(schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata)) 
					{
						this.txt_NumProtMit_P.Text = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).descrizioneProtocolloMittente;
						this.txt_DataProtMit_P.Text = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).dataProtocolloMittente;
					}
				
					//risposta al protocollo 
					if(schedaDocumento.rispostaDocumento != null)
                  this.txt_RispProtSegn_P.Text = schedaDocumento.rispostaDocumento.segnatura;
				
					//annullamento
					DocsPaWR.ProtocolloAnnullato protAnnull;
					protAnnull = schedaDocumento.protocollo.protocolloAnnullato;
					if (protAnnull != null) 
					{
						this.panel_Annul.Visible = true;
						this.txt_dataAnnul_P.Text = schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento;
						this.txt_numAnnul_P.Text  = schedaDocumento.protocollo.protocolloAnnullato.autorizzazione;
					}

					// abilita/disabilita bottoni e campi
					enableFormProtocolloFields();
					setFormProperties();			

					DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
					if(schedaDocumento.documenti != null && schedaDocumento.documenti.Length>0)
						FileManager.setSelectedFile(this, schedaDocumento.documenti[0]);
			
					//abilitazione delle funzioni in base al ruolo
					UserManager.disabilitaFunzNonAutorizzate(this);

				
				}
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void enableFormProtocolloFields() 
		{
			// dettagli mittente
			if (this.txt_CodMit_P.Text.Equals(""))
				this.btn_DetMit_P.Enabled = false;
			else
				this.btn_DetMit_P.Enabled = true;
			
			// dettagli mittente intermedio
			if( this.txt_CodMitInt_P.Text.Equals(""))
				this.btn_DetMitInt_P.Enabled = false;
			else
				this.btn_DetMitInt_P.Enabled = true;

			// protocollo mittente
			if (this.txt_DataProtMit_P.Text.Equals("") && this.txt_NumProtMit_P.Text.Equals(""))
				this.btn_verificaPrec_P.Enabled = false;
			else
				this.btn_verificaPrec_P.Enabled = true;

			//pulsante protocollo (disabilito se il documento non è nuovo o se non è un documento grigio predisposto alla protocollazione)
			/* old codice 131202
			
			////			if ((schedaDocumento.systemId != null) && (!schedaDocumento.predisponiProtocollazione)))
			//			if ((schedaDocumento.systemId != null) && (!(schedaDocumento.protocollo.daProtocollare != null && schedaDocumento.protocollo.daProtocollare.Equals("1"))))
			//			{
			//				this.btn_protocolla_P.Enabled = false;
			//				this.btn_protocollaGiallo_P.Enabled = false;
			//			}
			//			else
			//				this.btn_salva_P.Enabled = false;
			*/
			/* nuovo codice */
			if ((schedaDocumento.protocollo.numero != null && !schedaDocumento.protocollo.numero.Equals("")))
			{
				this.btn_protocolla_P.Enabled = false;
				this.btn_protocollaGiallo_P.Enabled = false;
			}

			if (schedaDocumento.systemId == null)
				this.btn_salva_P.Enabled = false;
			else
				this.btn_salva_P.Enabled = true;
			
			/* -- fine nuovo codice */

			if (!(schedaDocumento.protocollo.numero != null && !schedaDocumento.protocollo.numero.Equals(""))) 
			{
				this.btn_annulla_P.Enabled = false; 
				this.btn_riproponiDati_P.Enabled = false;
				this.btn_aggiungi_P.Enabled = false;
			}	
			
			if (schedaDocumento.protocollo.protocolloAnnullato != null) 
			{
				this.btn_annulla_P.Enabled = false;
				this.btn_salva_P.Enabled = false;
			}
		
			/* old codice 131202
			//			if (schedaDocumento.systemId != null && (schedaDocumento.predisponiProtocollazione)) 
						if ((schedaDocumento.systemId != null && (schedaDocumento.protocollo.daProtocollare != null && schedaDocumento.protocollo.daProtocollare.Equals("1"))))
						{
							this.btn_salva_P.Enabled = true;
						}
			  fine old codice */		
			if ((schedaDocumento.systemId != null) && (!schedaDocumento.predisponiProtocollazione))
				this.rbl_InOut_P.Enabled = false;
			else
				this.rbl_InOut_P.Enabled = true;


			//disabilito catena (per ora)
			this.btn_catenaProt_P.Enabled = false;

			//disabilito il campo descrizione di mittente intermedio
			this.txt_DescMitInt_P.Enabled = false;

			//abilito disabilito pulsante SPEDISCI se il doc è in partenza

			if (schedaDocumento.tipoProto.Equals("P"))
			{
				if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.numero != null && !(schedaDocumento.protocollo.numero.Equals("")) && (schedaDocumento.protocollo.protocolloAnnullato == null || schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento == null || schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento.Equals("")))
					this.btn_spedisci_P.Enabled = true;
				else
					this.btn_spedisci_P.Enabled = false;
				
			}

			//ultimo controllo sul documento annullato: disabilitare tutto
			//annullamento
			DocsPaWR.ProtocolloAnnullato protAnnull;
			protAnnull = schedaDocumento.protocollo.protocolloAnnullato;
			if (protAnnull != null && protAnnull.dataAnnullamento != null && !protAnnull.Equals(""))
			{
				this.btn_protocolla_P.Enabled = false;
				this.btn_aggiungi_P.Enabled = false;
				this.btn_salva_P.Enabled = false;
			}


		}
	
		protected void enableEditableFields()
		{
			//abilito il pulsante modifica e rendo i campi read only
			if ((schedaDocumento.systemId != null) && (!schedaDocumento.predisponiProtocollazione))
			{
				this.btn_modificaOgget_P.Enabled = true;
				this.btn_ModMit_P.Enabled = true;
				this.btn_ModMitInt_P.Enabled = true;
				this.btn_ModDest_P.Enabled = true;
					
				this.txt_CodMit_P.ReadOnly = true;
				this.txt_DescMit_P.ReadOnly = true;
				this.txt_CodMitInt_P.ReadOnly = true;
				//this.txt_DescMitInt_P.ReadOnly = true;
				this.txt_oggetto_P.ReadOnly = true;
				this.txt_CodDest_P.ReadOnly = true;
				this.txt_DescDest_P.ReadOnly = true;

				this.btn_RubrMit_P.Enabled = false;
				this.btn_RubrMitInt_P.Enabled = false;
				this.btn_RubrOgget_P.Enabled = false;
				this.btn_RubrDest_P.Enabled = false;
				this.btn_cancDest.Enabled = false;
				this.btn_cancDestCC.Enabled = false;
				this.btn_insDest.Enabled = false;
				this.btn_insDestCC.Enabled = false;
			} 
			else
			{
				this.btn_modificaOgget_P.Enabled = false;
				this.btn_ModMit_P.Enabled = false;
				this.btn_ModMitInt_P.Enabled = false;
				this.btn_ModDest_P.Enabled = false;
					
				this.txt_CodMit_P.ReadOnly = false;
				this.txt_DescMit_P.ReadOnly = false;
				this.txt_CodMitInt_P.ReadOnly = false;
				//this.txt_DescMitInt_P.ReadOnly = false;
				this.txt_oggetto_P.ReadOnly = false;
				this.txt_CodDest_P.ReadOnly = false;
				this.txt_DescDest_P.ReadOnly = false;

				this.btn_RubrMit_P.Enabled = true;
				this.btn_RubrMitInt_P.Enabled = true;
				this.btn_RubrOgget_P.Enabled = true;
				this.btn_RubrDest_P.Enabled = true;
				this.btn_cancDest.Enabled = true;
				this.btn_cancDestCC.Enabled = true;
				this.btn_insDest.Enabled = true;
				this.btn_insDestCC.Enabled = true;

			}

		}

		protected void setFormProperties() 
		{
			//per aprire il popup di area lavoro devo passare un parametro che è l'opposto della tipologia del documento
			// doc arrivo   tipoADL = P
			// doc partenza tipoADL = A
			string tipoADL = "A";
			if(schedaDocumento.tipoProto.Equals("A"))
				tipoADL = "P";
			this.btn_RubrOgget_P.Attributes.Add("onclick","ApriOggettario('proto');");
			
			this.btn_RubrMit_P.Attributes.Add("onclick","ApriRubrica('protoS','mitt');");
			this.btn_RubrMitInt_P.Attributes.Add("onclick","ApriRubrica('protoS','mittInt');");
			this.btn_RubrDest_P.Attributes.Add("onclick","ApriRubrica('protoS','dest');");
			this.btn_adl_P.Attributes.Add("onclick","ApriFinestraADL('../popup/areaDiLavoro.aspx?tipoDoc=" + tipoADL + "');");
			this.btn_annulla_P.Attributes.Add("onclick","ApriFinestraAnnullaProto();");
			this.btn_DetMit_P.Attributes.Add("onclick","ApriFinestraCor('../popup/dettagliCorrispondenti.aspx?',document.docProtocolloSemplice.txt_CodMit_P.value);");
			this.btn_DetMitInt_P.Attributes.Add("onclick","ApriFinestraCor('../popup/dettagliCorrispondenti.aspx?',document.docProtocolloSemplice.txt_CodMitInt_P.value);");
			this.btn_dettDest.Attributes.Add("onclick","ApriFinestraCor('../popup/dettagliCorrispondenti.aspx?',document.docProtocolloSemplice.lbx_dest.value);");
			this.btn_dettDestCC.Attributes.Add("onclick","ApriFinestraCor('../popup/dettagliCorrispondenti.aspx?',document.docProtocolloSemplice.lbx_destCC.value);");
			this.btn_stampaSegn_P.Attributes.Add("onclick","stampaSegnatura();");

			//			this.btn_aggiungiDest_P.Attributes.Add("onclick","ApriFinestra(document.form_rubrica,'rubricaCorrInterni.aspx?wnd=proto&target=dest','Rubrica');");
		}

		private void rbl_InOut_P_SelectedIndexChanged(object sender, System.EventArgs e) 
		{
			try 
			{
				this.txt_RispProtSegn_P.Text="";
				if (this.rbl_InOut_P.SelectedItem.Value.Equals("In")) 
				{
					this.setProtoArrivo();
					this.btn_aggiungi_P.Visible = true;
					this.btn_spedisci_P.Visible = false;
				}
				else 
				{
					this.setProtoPartenza();
					this.btn_aggiungi_P.Visible = false;
					this.btn_spedisci_P.Visible = true;
				}
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void setProtoArrivo() 
		{
			schedaDocumento.protocollo = new DocsPAWA.DocsPaWR.ProtocolloEntrata();
			schedaDocumento.tipoProto = "A";
		}

		private void setProtoPartenza() 
		{
			schedaDocumento.protocollo = new DocsPAWA.DocsPaWR.ProtocolloUscita();
			schedaDocumento.tipoProto = "P";
		}
		
		private void setProtoInterno() 
		{
			schedaDocumento.protocollo = new DocsPAWA.DocsPaWR.ProtocolloInterno();
			schedaDocumento.tipoProto = "I";
		}
		
		protected void txt_CodMit_P_TextChanged(object sender, System.EventArgs e) 
		{
			try 
			{
				setDescCorrispondente(this.txt_CodMit_P.Text, "Mit");
				if(schedaDocumento.systemId != null)
					((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void txt_oggetto_P_TextChanged(object sender, System.EventArgs e) 
		{	
			try 
			{
				DocsPaWR.Oggetto ProtoObj = new DocsPAWA.DocsPaWR.Oggetto(); 					
				ProtoObj.descrizione = this.txt_oggetto_P.Text;
				schedaDocumento.oggetto = ProtoObj;
				if(schedaDocumento.systemId != null)
					schedaDocumento.oggetto.daAggiornare = true;
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void txt_DescMit_P_TextChanged(object sender, System.EventArgs e) 
		{	
			try 
			{
				DocsPaWR.Corrispondente corr = new DocsPAWA.DocsPaWR.Corrispondente();
				corr.descrizione = this.txt_DescMit_P.Text;
				corr.tipoCorrispondente = "O";
				((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = corr;
				if(schedaDocumento.systemId != null)
					((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittente = true;
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void txt_DescMitInt_P_TextChanged(object sender, System.EventArgs e) 
		{		
			try 
			{
				DocsPaWR.Corrispondente corr = new DocsPAWA.DocsPaWR.Corrispondente();
				corr.descrizione = this.txt_DescMitInt_P.Text;
				corr.tipoCorrispondente = "O";
				((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio = corr;
				if(schedaDocumento.systemId != null)
					((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittenteIntermedio = true;
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}

		}
		
		private void txt_DataProtMit_P_TextChanged(object sender, System.EventArgs e) 
		{		
			try 
			{
				((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).dataProtocolloMittente = this.txt_DataProtMit_P.Text;
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		
		private void txt_NumProtMit_P_TextChanged(object sender, System.EventArgs e) 
		{
			try 
			{
				((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).descrizioneProtocolloMittente = this.txt_NumProtMit_P.Text;
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void txt_CodMitInt_P_TextChanged(object sender, System.EventArgs e)	
		{
			try 
			{
				setDescCorrispondente(this.txt_CodMitInt_P.Text, "MitInt");	
				((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).daAggiornareMittenteIntermedio = true;
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}


	#region gestione Corrispondenti

		private void setDescCorrispondente(string codiceRubrica, string tipoCor) 
		{						
			
			DocsPaWR.Corrispondente corr = null;
			if(!codiceRubrica.Equals(""))
			{
				corr = UserManager.getCorrispondente(this, codiceRubrica,true);
				if (tipoCor == "Mit") 
					((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = corr;
				else if (tipoCor == "MitInt") 
					((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio = corr;
				else if (tipoCor == "Dest")
				{
					//aggiorna solo il campo descrizione ma non aggiunge tra i destinatari
					if(corr != null)
						this.txt_DescDest_P.Text = corr.descrizione;
				}
			}
		}


		private void addDestinatari(int index, string tipoDest) 
		{
			//controlo se esiste già il corrispondente selezionato
			DocsPaWR.Corrispondente[] listaDest;
			DocsPaWR.Corrispondente[] listaDestCC;
			DocsPaWR.Corrispondente corr;

			listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
			listaDestCC = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
			

			//aggiungo il corrispondente
			if (tipoDest.Equals("P"))
			{
				corr = listaDestCC[index];
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = UserManager.addCorrispondente(listaDest, corr);
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = true;
			}
			else
				if(tipoDest.Equals("C"))
			{
				corr = listaDest[index];
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = UserManager.addCorrispondente(listaDestCC, corr);
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = true;
			}
			
		}


		private void removeDestinatari(int index, string tipoDest) 
		{
			DocsPaWR.Corrispondente[] listaDest;
			if (tipoDest.Equals("P"))
			{
				listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = UserManager.removeCorrispondente(listaDest, index);
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = true;		
			}
			else
				if(tipoDest.Equals("C"))
			{
				listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = UserManager.removeCorrispondente(listaDest, index);
				((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatariConoscenza = true;
			}

		}


		private void setListBoxDestinatari()
		{
			//Valido per i documenti in Partenza
			DocsPaWR.Corrispondente destinatario;
			
			if(((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari != null)
			{
				for (int i=0; i< ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Length; i++)
				{
					destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari[i]);
					this.lbx_dest.Items.Add(UserManager.getDecrizioneCorrispondente(this,destinatario));
					this.lbx_dest.Items[i].Value = destinatario.codiceRubrica;
				
				}
			}

			if(((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza != null)
			{
				for (int i=0; i< ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Length; i++)
				{
					destinatario = (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza[i]);
					this.lbx_destCC.Items.Add(UserManager.getDecrizioneCorrispondente(this,destinatario));
					this.lbx_destCC.Items[i].Value = destinatario.codiceRubrica;
				}			
			}

		}


	#endregion

		private string CheckDate()
		{
			//Se il registro è giallo e il documento è nuovo
			//controllo sul range: la data del protocollo deve essere > data ultimo protocollo e < data odierna
			
			string d_reg = "";
			
			//reperisco la data secondo il registro
			DocsPaWR.Registro reg = schedaDocumento.registro;
			if (reg!=null)
				d_reg = Utils.getMaxDate(reg.dataApertura, reg.dataUltimoProtocollo);
			else
			{
				string msg = "Si è verificato un errore nel reperimento delle informazioni";
				return msg;
			}
			
			//reperisco la data odierna
			DateTime dt_cor = DateTime.Now;
			CultureInfo ci = new CultureInfo("it-IT");
			string[] formati={"dd/MM/yyyy"};			
			
			DateTime dt_reg = DateTime.ParseExact(d_reg,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);	
		
			DateTime dt_segn = DateTime.ParseExact(this.txt_dataSegn.Text,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);

			if (dt_segn.CompareTo(dt_reg) < 0 || dt_segn.CompareTo(dt_cor) > 0)
			{
				return "Data segnatura non valida";
			}
			return "";
		}

		private string CheckFields(string action) 
		{
			string msg ="";

			// action:  S = salva   -  P = protocolla

			//controllo la data solo se sto protocollando
			if (action.Equals("P"))
			{
				//data segnatura obbligatoria 
				if (this.txt_dataSegn.Text.Equals(""))
				{
					msg = "Data di segnatura non presente";
					return msg;
				}
				//controllo sulla data
				if (this.txt_dataSegn.Text.Length > 0)
				{
					if(!Utils.isDate(this.txt_dataSegn.Text))
					{
						msg = "Errore nel formato della data";
						return msg;
					}
				}

				//controllo sul range di date
				if (!this.txt_dataSegn.ReadOnly)
				{
					msg = this.CheckDate();
					if (!msg.Equals(""))
						return msg;
				}
			}

			//controllo sull'inserimento dell'oggetto
			if (this.txt_oggetto_P.Text.Equals("") || this.txt_oggetto_P.Text == null ) 
			{
				msg = "Inserire il valore: oggetto";
				return msg;
			}
			
			if (this.rbl_InOut_P.SelectedItem.Value.Equals("In"))
			{			
				//controllo sull'inserimento del mittente
				if (this.txt_CodMit_P.Text.Equals("") || this.txt_CodMit_P.Text == null ) 
				{
					if (this.txt_DescMit_P.Text.Equals("") || this.txt_DescMit_P.Text== null ) 
					{
						msg = "Inserire il valore: mittente";
						return msg;
					}
				}
			}
			else
			{
				//protocollo in uscita - controllo sui destinatari
				if (this.lbx_dest.Items.Count<=0)
				{
					msg = "Inserire il valore: destinatario";
					return msg;
				}
			}
			return msg;
				
		}




		private void btn_add_ADL_Click(object sender, System.EventArgs e) 
		{
			//aggiunde il documento all'area di lavoro
			DocumentManager.addAreaLavoro(this,schedaDocumento);
		}


		private void protocollaDoc()
		{
			try 
			{
				DocsPaWR.ResultProtocollazione resProto;
				string msg = CheckFields("P");
				if (msg.Equals(""))	
				{
					string systemID = schedaDocumento.systemId;
					schedaDocumento = DocumentManager.creaProtocollo(this, schedaDocumento, null, out  resProto);
					
					DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
					//se il documento è nuovo riarico il frame di destra
					if (systemID == null || systemID.Equals(""))
					{ 
						//ricarica il frame destro
						string  funct_dx = "top.principale.iFrame_dx.document.location='tabDoc.aspx';";
						Response.Write("<script language='javascript'> " + funct_dx + "</script>");
					}
					Response.Write("<script language='javascript' > top.principale.document.location = 'gestioneDocSemplice.aspx';</script>");

					//aggiunge script per il waiting
					if (!Page.IsStartupScriptRegistered("wait"))
					{
						Page.RegisterStartupScript("wait","<script>DocsPa_FuncJS_WaitWindows();</script>");
					}
					
				}
				else
					Response.Write("<script>alert('" + msg + "');</script>");	
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void btn_protocolla_P_Click(object sender, System.Web.UI.ImageClickEventArgs e) 
		{
			protocollaDoc();
		}		

		private void btn_protocollaGiallo_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			protocollaDoc();
		}


		private void btn_salva_P_Click(object sender, System.Web.UI.ImageClickEventArgs e) 
		{
			try 
			{
				string msg = CheckFields("S");
				bool daAggiornareUffRef;
				if (msg.Equals(""))	
				{
					schedaDocumento = DocumentManager.salva(this, schedaDocumento,false, out daAggiornareUffRef);
					DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
				}
				else
					Response.Write("<script>alert('" + msg + "');</script>");	
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void btn_aggiungi_P_Click(object sender, System.Web.UI.ImageClickEventArgs e) 
		{
			try 
			{
				DocumentManager.addAreaLavoro(this, schedaDocumento);
				string msg = "Documento aggiunto all'area di lavoro";
				Response.Write("<script>alert(\"" + msg + "\");</script>");
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void btn_riproponiDati_P_Click(object sender, System.Web.UI.ImageClickEventArgs e) 
		{
			schedaDocumento = DocumentManager.riproponiDati(this,schedaDocumento,false);
			//predispongo il nuovo documento alla protocollazione
			DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
			FileManager.removeSelectedFile(this);
			//schedaDocumento.predisponiProtocollazione = true;
			DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
			this.rbl_InOut_P.Enabled = true;
		//	Response.Write("<script>parent.document.location.href = 'gestioneDocSemplice.aspx';</script>");
			Response.Write("<script language='javascript'>top.principale.document.location = 'gestioneDocSemplice.aspx';</script>");
		}



		private void txt_CodDest_P_TextChanged(object sender, System.EventArgs e) 
		{
			try 
			{
				setDescCorrispondente(this.txt_CodDest_P.Text, "Dest");
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void txt_DescDest_P_TextChanged(object sender, System.EventArgs e) 
		{
		
		}
	

		private void btn_aggiungiDest_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (this.txt_CodDest_P != null && !this.txt_CodDest_P.Equals(""))
			{
				//cerca il corrispondente e lo aggiunge tra i destinatari principali
				DocsPaWR.Corrispondente corr = null;
				corr = UserManager.getCorrispondente(this, this.txt_CodDest_P.Text,true);
				if (corr != null)
				{
					
					DocsPaWR.Corrispondente[] listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
					if (!UserManager.esisteCorrispondente(listaDest,corr)) 
					{
						listaDest = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
						if (!UserManager.esisteCorrispondente(listaDest,corr))
						{
							((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = UserManager.addCorrispondente(listaDest, corr);
							((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).daAggiornareDestinatari = true;
						}
					}

				}

			}
		}

		private void btn_insDestCC_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			//inserisco il destinatario selezionato tra i destinatari per conoscenza e lo rimuovo dai destinatari
			if (this.lbx_dest.SelectedIndex >=0)
			{
				//cerca il corrispondente e lo aggiunge tra i destinatari principali
				addDestinatari(this.lbx_dest.SelectedIndex, "C");
				removeDestinatari(this.lbx_dest.SelectedIndex, "P");

			}
		}

		private void btn_insDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			//inserisco il destinatario per conoscenza selezionato tra i destinatari e lo rimuovo dai destinatari per conoscenza
			if (this.lbx_destCC.SelectedIndex >= 0)
			{
				//cerca il corrispondente e lo aggiunge tra i destinatari principali
				addDestinatari(this.lbx_destCC.SelectedIndex, "P");	
				removeDestinatari(this.lbx_destCC.SelectedIndex, "C");
				
			}
		
		}


		private void btn_dettDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void btn_cancDest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (this.lbx_dest.SelectedIndex >=0)
				removeDestinatari(this.lbx_dest.SelectedIndex, "P");
		}

		private void btn_cancDestCC_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (this.lbx_destCC.SelectedIndex >= 0)
				removeDestinatari(this.lbx_destCC.SelectedIndex, "C");	
		}

	

	
		private void setDataProtocollo()
		{
			//Da richiamare solo se il documento non è protocollato
			if (schedaDocumento.protocollo != null)
			{
				if((schedaDocumento.protocollo.segnatura != null && !schedaDocumento.protocollo.segnatura.Equals("")))
				{
					this.txt_dataSegn.ReadOnly = true;
					return;
				}
			}

			DocsPaWR.Registro reg = schedaDocumento.registro;
			if (reg!=null)
			{
				string stato = UserManager.getStatoRegistro(reg);
				if (stato.Equals("V"))
				{
					if(schedaDocumento.protocollo.dataProtocollazione == null || schedaDocumento.protocollo.dataProtocollazione.Equals("")) 
						schedaDocumento.protocollo.dataProtocollazione = reg.dataApertura;
					this.txt_dataSegn.ReadOnly = true;
				}
				else
					if (stato.Equals("G"))
				{
					if(schedaDocumento.protocollo.dataProtocollazione == null || schedaDocumento.protocollo.dataProtocollazione.Equals("")) 
						schedaDocumento.protocollo.dataProtocollazione = Utils.getMaxDate(reg.dataApertura, reg.dataUltimoProtocollo);
					this.txt_dataSegn.ReadOnly = false;
				}
			}
		}

		private void txt_dataSegn_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				schedaDocumento.protocollo.dataProtocollazione = this.txt_dataSegn.Text;
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void btn_adl_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void btn_modificaOgget_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			this.txt_oggetto_P.ReadOnly = false;
			this.btn_RubrOgget_P.Enabled = true;
		}

		private void btn_ModMit_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			this.txt_CodMit_P.ReadOnly = false;
			this.txt_DescMit_P.ReadOnly = false;
			this.btn_RubrMit_P.Enabled = true;

		}

		private void btn_ModMitInt_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			this.txt_CodMitInt_P.ReadOnly = false;
			this.btn_RubrMitInt_P.Enabled = true;
		}

		private void btn_RubrMit_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void btn_DetMitInt_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void btn_spedisci_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			DocumentManager.spedisciDoc(this, schedaDocumento);
		}

		private void btn_ModDest_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			this.btn_RubrDest_P.Enabled = true;
			this.btn_cancDest.Enabled = true;
			this.btn_cancDestCC.Enabled = true;
			this.btn_insDest.Enabled = true;
			this.btn_insDestCC.Enabled = true;
			this.txt_CodDest_P.ReadOnly = false;
			this.txt_DescDest_P.ReadOnly = false;
		}

		private void btn_DetMit_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void btn_annulla_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void btn_storiaOgg_P_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}


		
	}
}
