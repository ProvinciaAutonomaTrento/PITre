using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Paging;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Classe astratta per la gestione della visualizzazione 
	/// dei documenti dalla ricerca documenti
	/// </summary>
	public abstract class ListaDocumenti : UserControl
	{
		public ListaDocumenti()
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			this.GetControlDatagrid().ItemCommand += new DataGridCommandEventHandler(this.OnGridItemCommand);

			ListPagingNavigationControls ctl=this.GetControlPaging();
			ctl.OnPageChanged+=new ListPagingNavigationControls.PageChangedDelegate(this.GridPaging_OnPageChanged);

			base.OnLoad (e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.SetFieldsVisibility();

			this.RefreshMessage();

			base.OnPreRender (e);
		}

		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		private void SetFieldsVisibility()
		{
			PagingContext pagingContext=this.GetControlPaging().GetPagingContext();
			
			this.GetControlDatagrid().Visible=(pagingContext.RecordCount>0);

			this.GetControlPaging().Visible=(pagingContext.RecordCount>0);
		}

		/// <summary>
		/// Impostazione messaggio
		/// </summary>
		/// <param name="message"></param>
		protected abstract void RefreshMessage();

		/// <summary>
		/// Caricamento dati
		/// </summary>
		public void Fetch()
		{
			this.Fetch(new PagingContext(1));
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		/// <param name="searchProperties">Oggetto "SearchProperties" contenente i parametri di filtro</param>
		public void Fetch(SearchProperties searchProperties)
		{
			RicercaDocumentiHandler.CurrentFilter=searchProperties;

			this.Fetch();
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		/// <param name="pagingContext"></param>
		private void Fetch(PagingContext pagingContext)
		{
			InfoDocumento[] documenti=this.GetDocumenti(pagingContext);

			this.BindGrid(documenti);

			this.GetControlPaging().RefreshPaging(pagingContext);
		}

		/// <summary>
		/// Reperimento dei documenti per il contesto di paginazione richiesto
		/// </summary>
		/// <returns></returns>
		protected virtual InfoDocumento[] GetDocumenti(PagingContext pagingContext)
		{
			RicercaDocumentiHandler handler=new RicercaDocumentiHandler();
			return handler.GetDocumenti(this.GetFiltriRicerca(),pagingContext);
		}

		
		/// <summary>
		/// Visualizzazione del documento selezionato
		/// </summary>
		/// <param name="idProfile"></param>
		/// <param name="docNumber"></param>
		protected void ShowDocument(string idProfile,string docNumber)
		{
			// Impostazione contesto chiamante
			this.SetCallerContext();

			string url=EnvironmentContext.RootPath + "Documenti/DettagliDocumento.aspx?iddoc=" + idProfile + "&docnum=" + docNumber;

			Response.Redirect(url);
		}

		/// <summary>
		/// Impostazione contesto chiamante
		/// </summary>
		private void SetCallerContext()
		{
			// Impostazione del contesto del chiamante in sessione
			CallerContext callerContext=CallerContext.NewContext(this.Page.Request.Url.AbsoluteUri);
			callerContext.PageNumber=this.GetControlPaging().GetPagingContext().PageNumber;
		}

		/// <summary>
		/// Creazione oggetto dataset contenente i dati dei documenti da visualizzare
		/// </summary>
		/// <param name="documenti"></param>
		/// <returns></returns>
		protected abstract DataSet DocumentiToDataset(InfoDocumento[] documenti);

		/// <summary>
		/// Associazione dati alla griglia dei documenti
		/// </summary>
		/// <param name="documenti"></param>
		private void BindGrid(InfoDocumento[] documenti)
		{
			DataGrid dataGrid=this.GetControlDatagrid();
			dataGrid.DataSource=this.DocumentiToDataset(documenti);
			dataGrid.DataBind();
		}

		/// <summary>
		/// Reperimento filtri di ricerca
		/// </summary>
		/// <returns></returns>
		protected virtual DocsPAWA.DocsPaWR.FiltroRicerca[] GetFiltriRicerca()
		{
			SearchProperties searchProperties=RicercaDocumentiHandler.CurrentFilter;

			ArrayList filtri = new ArrayList();

			if (searchProperties!=null)
			{
				DocsPaWR.FiltroRicerca condizione = null;
				string aux = null;

				#region Profilo Ridotto

				#region Oggetto
				aux = DocsPAWA.Utils.DO_AdattaString(searchProperties.Documento.Oggetto);
				if (aux!=null && aux!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento=DocsPaWR.FiltriDocumento.OGGETTO.ToString();
					condizione.valore=aux;
					filtri.Add(condizione);
				}
				#endregion Oggetto

				#region Note
				aux = searchProperties.Documento.Note;
				if (aux!=null && aux!="")
				{
					condizione= new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento=DocsPaWR.FiltriDocumento.NOTE.ToString();
					condizione.valore=aux;
					filtri.Add(condizione);
				}
				#endregion Note

				#region Tipologia Documento
				if (searchProperties.Documento.Tipologia != "-1")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
					condizione.valore = searchProperties.Documento.Tipologia;
					filtri.Add(condizione);
				}
				#endregion Tipologia Documento

				#region Evidenza
				if (searchProperties.Documento.Evidenza!=null)
				{
					switch (searchProperties.Documento.Evidenza)
					{
						case "yes": 
							aux = "1";
							break;
						case "no": 
							aux = "0";
							break;
						default: 
							aux = "T";
							break;
					}
					if (aux!="T")
					{
						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.EVIDENZA.ToString();
						condizione.valore = aux;
						filtri.Add(condizione);
					}
				}
				#endregion Evidenza

				#region Data Creazione
				//				aux = (Mask.Documento.DataDocumentoDa!="") ? Utils.UniformDateFormat(Mask.Documento.DataDocumentoDa) : Utils.UniformDateFormat(DateTime.MinValue);
				aux = Utils.UniformDateFormat(searchProperties.Documento.DataDocumentoDa);
				if (aux!=null && aux!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
					condizione.valore = aux;
					filtri.Add(condizione);
				}

				aux = Utils.UniformDateFormat(searchProperties.Documento.DataDocumentoA);
				if (aux!=null && aux!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
					condizione.valore = aux;
					filtri.Add(condizione);
				}
				#endregion Data Creazione

				#region Id Documento
				/*
								aux = (Mask.Documento.IdDocumentoDa!=null && Mask.Documento.IdDocumentoDa!="")
									? Mask.Documento.IdDocumentoDa
									: "0";
				*/				
				aux = searchProperties.Documento.IdDocumentoDa;
				if (aux!=null && aux!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
					condizione.valore = aux;
					filtri.Add(condizione);
				}
	
				aux = searchProperties.Documento.IdDocumentoA;
				if (aux!=null && aux!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
					condizione.valore = aux;
					filtri.Add(condizione);
				}
				#endregion

				# region  Num protocollo DA A

				string auxA =searchProperties.Protocollo.NumProtocolloA;

				if (auxA!=null && auxA!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
					condizione.valore = auxA;
					filtri.Add(condizione);
				}

				aux = searchProperties.Protocollo.NumProtocolloDa;
				if (aux!=null && aux!="")
				{
					if (auxA!=null && auxA!="") //ricerca per intervallo
					{
						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
						condizione.valore = aux;
						filtri.Add(condizione);
					}
					else  //ricerca per numero protocollo singolo valore
					{
						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
						condizione.valore = aux;
						filtri.Add(condizione);
					}
				}
				#endregion 

				#region Data protocollo Da A

				auxA = Utils.UniformDateFormat(searchProperties.Protocollo.DataProtocolloDa);
				if (auxA!=null && auxA!="")
				{
					condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
					condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
					condizione.valore = auxA;
					filtri.Add(condizione);
				}
	
				aux = Utils.UniformDateFormat(searchProperties.Protocollo.DataProtocolloA);
				if (aux!=null && aux!="")
				{
					if (auxA!=null && auxA!="") //ricerca intervallo
					{
						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
						condizione.valore = aux;
						filtri.Add(condizione);
					}
//					else //ricerca valore singolo
//					{
//						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
//						condizione.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
//						condizione.valore = aux;
//						filtri.Add(condizione);
//					}
				}

				#endregion

				#endregion Profilo 

				#region Profilo Avanzato
				if (searchProperties.AdvancedDocProperties)
				{
					#region Parole Chiave

					foreach (DocsPAWA.DocsPaWR.DocumentoParolaChiave parolaChiave in searchProperties.Documento.ParoleChiavi)
					{
						condizione=new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento=DocsPaWR.FiltriDocumento.PAROLE_CHIAVE.ToString();
						condizione.valore=parolaChiave.systemId;
						filtri.Add(condizione);
					}

					#endregion Parole Chiave

					#region Privi Di

					if (searchProperties.Documento.PriviDiAssegnatario)
					{
						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento=DocsPaWR.FiltriDocumento.MANCANZA_ASSEGNAZIONE.ToString();
						condizione.valore = "0";
						filtri.Add(condizione);
					}

					if (searchProperties.Documento.PriviDiImmagine)
					{
						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento=DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
						condizione.valore = "0";
						filtri.Add(condizione);
					}

					if (searchProperties.Documento.PriviDiFascicolazione)
					{
						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento=DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
						condizione.valore = "0";
						filtri.Add(condizione);
					}

					#endregion Privi Di

					#region Fascicolo
					aux = searchProperties.Documento.CodiceFascicolo;
					if (aux!=null && aux!="")
					{
						DocsPaWR.Folder folder=FascicoliManager.getFolder(new Page(),FascicoliManager.getFascicoloDaCodice(new Page(),aux));
						string inSubFolder= "IN (" + folder.systemID;
						if(folder.childs != null && folder.childs.Length > 0) 
						{
							for (int i=0; i < folder.childs.Length; i++) 
							{
								inSubFolder += ", " + folder.childs[i].systemID;
								inSubFolder= this.GetInStringChild(folder.childs[i],inSubFolder);
							}					
						}
						inSubFolder += ")";

						condizione = new DocsPAWA.DocsPaWR.FiltroRicerca();
						condizione.argomento = DocsPAWA.DocsPaWR.FiltriFascicolazione.IN_CHILD_RIC_ESTESA.ToString();
						condizione.valore = inSubFolder;
						filtri.Add(condizione);
					}
					#endregion Fascicolo

				}
				#endregion Profilo Avanzato

				#region Da realizzare...
				#region Profilazione Dinamica
				#endregion Profilazione Dinamica

				#region Diagrammi di Stato
				#endregion Diagrammi di Stato
				#endregion Da realizzare...
			}

			DocsPaWR.FiltroRicerca[] outcome = new DocsPAWA.DocsPaWR.FiltroRicerca[filtri.Count];
			filtri.CopyTo(outcome);
			return outcome;
		}
		

		private string GetInStringChild(DocsPAWA.DocsPaWR.Folder folder, string inSubFolder)
		{
			if(folder.childs != null && folder.childs.Length > 0) 
			{
				for (int i=0; i < folder.childs.Length; i++) 
				{
					inSubFolder += ", " + folder.childs[i].systemID;
					inSubFolder= this.GetInStringChild(folder.childs[i],inSubFolder);
				}
			}
			return inSubFolder;
		}

		/// <summary>
		/// Reperimento controllo per la gestione della paginazione
		/// </summary>
		/// <returns></returns>
		protected abstract ListPagingNavigationControls GetControlPaging();

		/// <summary>
		/// Reperimento controllo DataGrid contenente i documenti visualizzati
		/// </summary>
		/// <returns></returns>
		protected abstract DataGrid GetControlDatagrid();

		/// <summary>
		/// Evento relativo al cambio pagina della griglia
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GridPaging_OnPageChanged(Object sender,ListPagingNavigationControls.PageChangedEventArgs e)
		{
			this.Fetch(e.PagingContext);
		}

		/// <summary>
		/// Evento "ItemCommand" della griglia documenti
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected abstract void OnGridItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e);
	}
}
