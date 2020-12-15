namespace DocsPAWA.SitoAccessibile.Ricerca
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using DocsPAWA.DocsPaWR;
	using DocsPAWA.SitoAccessibile.Paging;

	/// <summary>
	///	Esito ricerca dei documenti protocollati
	/// </summary>
	public class ListaDocumentiProtocollati : ListaDocumenti
	{
		protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdDocumentiProtocollati;

		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;

		private const int GRID_COL_ID_DOCUMENTO=0;
		private const int GRID_COL_DOC_NUMBER=1;
		private const int GRID_COL_PROTOCOLLO=2;
		private const int GRID_COL_DATA_ANNULLAMENTO=8;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		private void GridProtocolli_PreRender(object sender, System.EventArgs e)
		{
			// Aggiornamento righe relative ai protocolli annullati
			this.UpdateRowsProtocolliAnnullati();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grdDocumentiProtocollati.PreRender += new System.EventHandler(this.GridProtocolli_PreRender);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Reperimento filtri ricerca documenti protocollati
		/// </summary>
		/// <returns></returns>
		protected override FiltroRicerca[] GetFiltriRicerca()
		{
			ArrayList filtri=new ArrayList(base.GetFiltriRicerca());

			SearchProperties searchProperties=RicercaDocumentiHandler.CurrentFilter;

			DocsPaWR.FiltroRicerca condizione = null;
			string aux = null;

			aux = "";
			if (searchProperties.ProtocolliArrivo && searchProperties.ProtocolliPartenza) //&& Mask.ProtocolliInterni )
				aux = "T";
			else if (searchProperties.ProtocolliArrivo)
				aux = "A";
			else if (searchProperties.ProtocolliPartenza)
				aux = "P";
			else if (searchProperties.ProtocolliInterni)
				aux = "I";

			if (aux!=null && aux!="")
			{
				condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
				condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.TIPO.ToString();
				condizione.valore = aux;
				filtri.Add(condizione);
			}

			#region Anno protocollo

			if (searchProperties.Protocollo.AnnoProtocollo!=null && 
				searchProperties.Protocollo.AnnoProtocollo!="") 
			{
				condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
				condizione.argomento=DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
				condizione.valore=searchProperties.Protocollo.AnnoProtocollo;
				filtri.Add(condizione);
			}
					
			#endregion

			#region Registri
			if (searchProperties.Protocollo.Registri.Length>0)
			{
				aux=string.Empty;

				foreach (SitoAccessibile.Ricerca.SearchRegistry item in searchProperties.Protocollo.Registri)
				{
					if (item.Selezionato)
					{
						if (aux!=string.Empty)
							aux+=",";
						aux+=item.Id;
					}
				}

				if (aux!=null && aux!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento=DocsPaWR.FiltriDocumento.REGISTRO.ToString();
					condizione.valore=aux;
					filtri.Add(condizione);
				}
			}
			#endregion Registri

			#region Mittente/Destinatario
			if (searchProperties.Protocollo.Corrispondente!=null)
			{
				condizione= new DocsPAWA.DocsPaWR.FiltroRicerca();
				if (searchProperties.Protocollo.Corrispondente.systemId!=null &&
					searchProperties.Protocollo.Corrispondente.systemId!="")
				{
					aux = searchProperties.Protocollo.Corrispondente.systemId;
					condizione.argomento=DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
					condizione.valore=aux;
					filtri.Add(condizione);
				}
				else if (searchProperties.Protocollo.Corrispondente.descrizione!=null &&
					searchProperties.Protocollo.Corrispondente.descrizione!="")
				{
					aux = searchProperties.Protocollo.Corrispondente.descrizione;
					condizione.argomento=DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
					condizione.valore=aux;
					filtri.Add(condizione);
				}
			}
			#endregion Mittente/Destinatario

			#region Segnatura

			if (searchProperties.Protocollo.Segnatura!=null && searchProperties.Protocollo.Segnatura!=string.Empty)
			{
				condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
				condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.SEGNATURA.ToString();
				condizione.valore=searchProperties.Protocollo.Segnatura=searchProperties.Protocollo.Segnatura;
				filtri.Add(condizione);
			}

			#endregion

			#region Data Arrivo
			aux = Utils.UniformDateFormat(searchProperties.Protocollo.DataArrivoDa);
			if (aux!=null && aux!="")
			{
				condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
				condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_ARRIVO_SUCCESSIVA_AL.ToString();
				condizione.valore = aux;
				filtri.Add(condizione);
			}

			aux = Utils.UniformDateFormat(searchProperties.Protocollo.DataArrivoA);
			if (aux!=null && aux!="")
			{
				condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
				condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_ARRIVO_PRECEDENTE_IL.ToString();
				condizione.valore = aux;
				filtri.Add(condizione);
			}
			#endregion Data Arrivo

			if (searchProperties.AdvancedProtProperties)
			{
				#region Segnatura Mittente
				aux = searchProperties.Protocollo.SegnaturaMittente;
				if (aux!=null && aux!="")
				{
					condizione= new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento=DocsPaWR.FiltriDocumento.PROTOCOLLO_MITTENTE.ToString();
					condizione.valore=aux;
					filtri.Add(condizione);
				}
				#endregion Segnatura Mittente

				#region Data Protocollo Mittente
				aux = Utils.UniformDateFormat(searchProperties.Protocollo.DataProtMittenteDa);
				if (aux!=null && aux!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SUCCESSIVA_AL.ToString();
					condizione.valore = aux;
					filtri.Add(condizione);
				}

				aux = Utils.UniformDateFormat(searchProperties.Protocollo.DataProtMittenteA);
				if (aux!=null && aux!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_PRECEDENTE_IL.ToString();
					condizione.valore = aux;
					filtri.Add(condizione);
				}
				#endregion Data Protocollo Mittente

				#region Stato
				if (searchProperties.Protocollo.Stato!=null)
				{
					switch (searchProperties.Protocollo.Stato)
					{
						case "valid": 
							aux = "0";
							break;
						case "invalid": 
							aux = "1";
							break;
						default: 
							aux = "T";
							break;
					}
					if (aux!="T")
					{
						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.ANNULLATO.ToString();
						condizione.valore = aux;
						filtri.Add(condizione);
					}
				}
				#endregion Stato
	
				#region Mittente Intermedio
				if (searchProperties.Protocollo.MittenteIntermedio!=null)
				{
					condizione= new DocsPAWA.DocsPaWR.FiltroRicerca();
					if (searchProperties.Protocollo.MittenteIntermedio.systemId!=null &&
						searchProperties.Protocollo.MittenteIntermedio.systemId!="")
					{
						aux = searchProperties.Protocollo.MittenteIntermedio.systemId;
						condizione.argomento=DocsPaWR.FiltriDocumento.ID_MITTENTE_INTERMEDIO.ToString();
						condizione.valore=aux;
						filtri.Add(condizione);
					}
					else
					{
						aux = searchProperties.Protocollo.MittenteIntermedio.descrizione;
						condizione.argomento=DocsPaWR.FiltriDocumento.MITTENTE_INTERMEDIO.ToString();
						condizione.valore=aux;
						filtri.Add(condizione);
					}
				}
				#endregion Mittente Intermedio

				#region Data Protocollo Emergenza
				aux = Utils.UniformDateFormat(searchProperties.Protocollo.DataProtEmergenza);
				if (aux!=null && aux!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_PROTO_EMERGENZA_IL.ToString();
					condizione.valore = aux;
					filtri.Add(condizione);
				}
				#endregion Data Protocollo Emergenza

				#region Segnatura Protocollo Emergenza
				aux = searchProperties.Protocollo.ProtocolloEmergenza;
				if (aux!=null && aux!="")
				{
					condizione= new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento=DocsPaWR.FiltriDocumento.NUM_PROTO_EMERGENZA.ToString();
					condizione.valore=aux;
					filtri.Add(condizione);
				}
				#endregion Segnatura protocollo Emergenza
			}


			DocsPaWR.FiltroRicerca[] outcome = new DocsPAWA.DocsPaWR.FiltroRicerca[filtri.Count];
			filtri.CopyTo(outcome);
			return outcome;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="documenti"></param>
		/// <returns></returns>
		protected override DataSet DocumentiToDataset(InfoDocumento[] documenti)
		{
			DataSet ds=new DataSet("DatasetDocumentiProtocollati");
			DataTable dt=new DataTable("TableDocumentiProtocollati");

			dt.Columns.Add("ID",typeof(string));
			dt.Columns.Add("DocNumber",typeof(string));
			dt.Columns.Add("Protocollo",typeof(string));
			dt.Columns.Add("Registro",typeof(string));
			dt.Columns.Add("DirezioneProtocollo",typeof(string));
			dt.Columns.Add("Oggetto",typeof(string));
			dt.Columns.Add("MittenteDestinatario",typeof(string));
			dt.Columns.Add("DataAnnullamento",typeof(string));

			foreach (DocsPAWA.DocsPaWR.InfoDocumento infoDocumento in documenti)
			{
				DataRow row=dt.NewRow();
				row["ID"]=infoDocumento.idProfile;
				row["DocNumber"]=infoDocumento.docNumber;
				
				string protocollo=SitoAccessibile.Documenti.TipiDocumento.ToString(infoDocumento);
				if (infoDocumento.dataAnnullamento!=null && infoDocumento.dataAnnullamento!=string.Empty)
					protocollo+="<BR />Ann.to il " + infoDocumento.dataAnnullamento;
				row["Protocollo"]=protocollo;

				row["Registro"]=infoDocumento.codRegistro;
				row["DirezioneProtocollo"]=SitoAccessibile.Documenti.TipiDocumento.GetDescrizione(infoDocumento.tipoProto);
				row["Oggetto"]=infoDocumento.oggetto;
				row["MittenteDestinatario"]=this.GetMittenteDestinatari(infoDocumento.mittDest);
				row["DataAnnullamento"]=infoDocumento.dataAnnullamento;
				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		private string GetMittenteDestinatari(string[] corrispondenti)
		{
			string outcome = "";
			foreach (string s in corrispondenti)
				outcome += s+", ";

			if (outcome.EndsWith(", "))
				outcome = outcome.Substring(0, outcome.Length - ", ".Length);

			return outcome;
		}

		/// <summary>
		/// Impostazione messaggio
		/// </summary>
		/// <param name="message"></param>
		protected override void RefreshMessage()
		{
			PagingContext pagingContext=this.GetControlPaging().GetPagingContext();

			string message=string.Empty;

			if (pagingContext.RecordCount==0)
				message="Nessun protocollo trovato";
			else if (pagingContext.RecordCount==1)
				message="Trovato 1 protocollo";
			else
				message="Trovati " + pagingContext.RecordCount.ToString() + " protocolli";

			this.pnlMessage.InnerText=message;
		}

		/// <summary>
		/// Aggiornamento righe relative ai protocolli annullati
		/// </summary>
		private void UpdateRowsProtocolliAnnullati()
		{
			foreach (DataGridItem item in this.grdDocumentiProtocollati.Items)
			{
				string dataAnnullamento=item.Cells[GRID_COL_DATA_ANNULLAMENTO].Text;

				if (!dataAnnullamento.Equals(string.Empty) && 
					!dataAnnullamento.Equals("&nbsp;"))
				{
					foreach (TableCell cell in item.Cells)
						// Modifica del colore del font
						cell.CssClass="invalidItem";
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override DataGrid GetControlDatagrid()
		{
			return this.grdDocumentiProtocollati;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override ListPagingNavigationControls GetControlPaging()
		{
			return this.FindControl("listPagingNavigation") as ListPagingNavigationControls;
		}

		protected override void OnGridItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName.Equals("SHOW_DOCUMENT"))
				this.ShowDocument(e.Item.Cells[GRID_COL_ID_DOCUMENTO].Text,e.Item.Cells[GRID_COL_DOC_NUMBER].Text);
		}
	}
}
