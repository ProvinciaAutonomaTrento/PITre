using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DocsPAWA.DocsPaWR;
using DocsPAWA.popup.RubricaDocsPA.CustomControls;

namespace DocsPAWA.popup.RubricaDocsPA 
{
	/// <summary>
	/// Delegato per la gestione dell'evento OpenSubItems
	/// </summary>
	public delegate void OpenSubItemsHandler (object sender, OpenSubItemsArgs e);

	/// <summary>HierarchyElementSelected
	/// Delegato per la gestione dell'evento HierarchyElementSelected
	/// </summary>
	public delegate void HierarchyElementSelectedHandler (object sender, HierarchyElementSelectedArgs e);

	/// <summary>
	/// Delegato per la gestione dell'evento PageIndexChanged
	/// </summary>
	public delegate void PageIndexChangedHandler (object sender, PageIndexChangedArgs e);

	/// <summary>
	/// Delegato per la gestione dell'evento DetailSelected
	/// </summary>
	public delegate void DetailSelectedHandler (object sender, DetailSelectedArgs e);

	/// <summary>
	/// Delegato per la gestione dell'evento RemoveItem
	/// </summary>
	public delegate void RemoveItemHandler (object sender, RemoveItemArgs e);

	/// <summary>
	/// Delegato per la gestione dell'evento VerifyItemSelection
	/// </summary>
	public delegate bool VerifyItemSelectionHandler (object sender, string cod);

	/// <summary>
	/// Componente per la visualizzazione, selezione e gestione dei risultati 
	/// di una ricerca
	/// </summary>
	/// <remarks>
	/// Questo componente estende il DataGrid standard, fornendo di default una
	/// serie di meccanismi per la visualizzazione, selezione e gestione dei risultati 
	/// di una ricerca. Una CorrDataGrid è composta di due parti: una parte superiore
	/// in cui viene visualizzata una gerarchia di elementi ed una inferiore in cui 
	/// appare un elenco di elementi con dei pulsanti che permettono, opzionalmente, 
	/// la selezione, eliminazione, visualizzazione del dettaglio o "esplosione" di 
	/// ciascun elemento. Inizialmente la parte relativa alla gerarchia è vuota.
	/// </remarks>
	public class CorrDataGrid : DataGrid, IPostBackDataHandler
	{
		/// <summary>
		/// Viene generato all'apertura dei sotto-elementi
		/// </summary>
		public event OpenSubItemsHandler OpenSubItems;

		/// <summary>
		/// Viene generato durante la selezione di un elemento della gerarchia
		/// </summary>
		public event HierarchyElementSelectedHandler HierarchyElementSelected;

		/// <summary>
		/// Viene generato quando avviene un cambio di pagina
		/// </summary>
		public new event PageIndexChangedHandler PageIndexChanged;

		/// <summary>
		/// Viene generato quando viene richiesto il dettaglio di un elemento
		/// </summary>
		public event DetailSelectedHandler DetailSelected;

		/// <summary>
		/// Viene generato quando viene richiesta l'eliminazione di un elemento
		/// </summary>
		public event RemoveItemHandler RemoveItem;

		/// <summary>
		/// Viene generato quando la CorrDataGrid ha bisogno di verificare se un elemento
		/// risulta selezionato
		/// </summary>
		public event VerifyItemSelectionHandler VerifyItemSelection;

		/// <summary>
		/// Viene generato quando l'utente chiede la rimozione di tutti gli
		/// elementi presenti nella CorrDataGrid
		/// </summary>
		public event EventHandler RemoveAll;

		/// <summary>
		/// Viene generato quando l'utente chiede la rimozione di tutti gli
		/// elementi presenti nella CorrDataGrid
		/// </summary>
		public event SelectorFilterHandler SelectorFilter;

		private string _name;
		private int _selected_page;
		private bool _display_only;
		private ArrayList hierarchy;
		private string _hierarchy_css_class;
		private bool _show_selectors;
		private bool _show_remove_buttons;
		private bool _show_item_hierarchy_in_list;
		private bool _allow_multiple_selection;
		private bool _show_remove_all;
		private string _callType;
		private string _codUoAppartenenza;
		private DocsPAWA.DocsPaWR.InfoUtente _infoUtente;

		private DictionaryEntry event_data;
		internal Hashtable hierarchy_cache;

		/// <exclude></exclude>
		internal const string EVT_OPENSUBITEMS = "_OpenSubItems";
		/// <exclude></exclude>
		internal const string EVT_PGIDXCHANGED = "_PageIndexChanged";
		/// <exclude></exclude>
		internal const string EVT_HIERITMSELECTED = "_HierarchyItemSelected";
		/// <exclude></exclude>
		internal const string EVT_DETAILSELECTED = "_DetailSelected";
		/// <exclude></exclude>
		internal const string EVT_REMOVEITEM = "_RemoveItem";
		/// <exclude></exclude>
		internal const string EVT_REMOVEALL = "_RemoveAll";
		/// <exclude></exclude>
		internal const string EVT_SELECTORFILTER = "_SelectorFilter";

		/// <summary>
		/// Il Nome associato all'istanza della CorrDataGrid
		/// </summary>
		public string Name 
		{
			set { _name = value; }
			get { return _name;  }
		}

		/// <summary>
		/// La classe CSS contenente lo stile degli elementi della gerarchia
		/// </summary>
		public string HierarchyCssClass 
		{
			set { _hierarchy_css_class = value; }
			get { return _hierarchy_css_class;  }
		}

		/// <summary>
		/// Controlla la possibilità di visualizzazione ed "esplosione" di eventuali 
		/// figli dell'elemento
		/// </summary>
		public bool DisplayOnly
		{
			set { _display_only = value; }
			get { return _display_only;  }
		}

		/// <summary>
		/// Visualizza o nasconde le caselle di selezione (Checkbox o RadioButton a
		/// seconda dell'impostazione <see cref="AllowMultipleSelection"/>)
		/// </summary>
		public bool ShowSelectors
		{
			set { _show_selectors = value; }
			get { return _show_selectors;  }
		}

		/// <summary>
		/// Visualizza o nasconde i pulsanti che consentono la rimozione degli elementi
		/// </summary>
		public bool ShowRemoveButtons
		{
			set { _show_remove_buttons = value; }
			get { return _show_remove_buttons;  }
		}

		/// <summary>
		/// Attiva o disattiva la selezione multipla di più elementi, visualizzando
		/// le caselle di selezione come Checkbox o RadioButton
		/// </summary>
		public bool AllowMultipleSelection 
		{
			set { _allow_multiple_selection = value; }
			get { return _allow_multiple_selection;  }
		}

		/// <summary>
		/// Visualizza la descrizione di un elemento completa del suo percorso
		/// in ordine gerarchico
		/// </summary>
		public bool ShowItemHierarchyInList
		{
			set { _show_item_hierarchy_in_list = value; }
			get { return _show_item_hierarchy_in_list;  }
		}

		/// <summary>
		/// Mostra o nasconde il pulsante che permette l'eliminazione
		/// di tutti i corrispondenti selezionati
		/// </summary>
		public bool ShowRemoveAll
		{
			set { _show_remove_all = value; }
			get { return _show_remove_all;  }
		}

		/// <summary>
		/// Tipo di chiamata della rubrica
		/// </summary>
		public string CallType
		{
			set { _callType = value; }
			get { return _callType;  }
		}

		/// <summary>
		/// Codice rubrica della UO di appartenenza
		/// </summary>
		public string CodUoAppartenenza
		{
			set { _codUoAppartenenza = value; }
			get { return _codUoAppartenenza;  }
		}

		/// <summary>
		/// InfoUtente 
		/// </summary>
		public DocsPAWA.DocsPaWR.InfoUtente InfoUtente
		{
			set { _infoUtente = value; }
			get { return _infoUtente;  }
		}

		/// <summary>
		/// Un elemento della gerarchia
		/// </summary>
		[Serializable]
			public class HierarchyElement 
		{
			/// <summary>
			/// Il codice dell'elemento
			/// </summary>
			public string Codice;

			/// <summary>
			/// La descrizione dell'elemento
			/// </summary>
			public string Descrizione;

			/// <summary>
			/// Il tipo dell'elemento ("U", "R" o "P")
			/// </summary>
			public string Tipo;

			/// <summary>
			/// Indica se l'elemento selezionato è interno (true) o esterno (false)
			/// </summary>
			public bool Interno;

			/// <summary>
			/// Il costruttore per la classe HierarchyElement
			/// </summary>
			/// <param name="c">Codice</param>
			/// <param name="d">Descrizione</param>
			/// <param name="t">Tipo ("U", "R" o "P")</param>
			public HierarchyElement (string c, string d, string t, bool i)
			{
				this.Codice = c;
				this.Descrizione = d;
				this.Tipo = t;
				this.Interno = i;
			}
		}

		/// <summary>
		/// Il costruttore della classe CorrDataGrid
		/// </summary>
		public CorrDataGrid() : base()
		{
			this.Width = new Unit ("100%");
			this.ShowSelectors = true;
			this.ShowRemoveButtons = false;
			this.ShowHeader = true;
			this.ShowFooter = true;
			this.AllowMultipleSelection = true;
			this.ShowRemoveAll = true;
			this.HeaderStyle.CssClass = "menu_1_bianco_dg";
			this.HeaderStyle.BackColor = System.Drawing.Color.FromArgb (75,75,75);
			this.FooterStyle.CssClass = "menu_1_bianco_dg";
			this.FooterStyle.BackColor = System.Drawing.Color.FromArgb (75,75,75);
			this.ItemStyle.CssClass = "bg_grigioN";
			this.ShowItemHierarchyInList = false;
			this.ItemStyle.Height = new Unit("23");
			this.AlternatingItemStyle.CssClass = "bg_grigioA";
			this.PagerStyle.CssClass = "menu_1_bianco_dg";
			this.PagerStyle.BackColor = System.Drawing.Color.FromArgb (75,75,75);
			this.PagerStyle.HorizontalAlign = HorizontalAlign.Center;
			this.PagerStyle.VerticalAlign = VerticalAlign.Middle;
			this.PagerStyle.Visible = false;
			this.ItemCreated += new DataGridItemEventHandler(CorrDataGrid_ItemCreated);
			this.DataBinding += new EventHandler(CorrDataGrid_DataBinding);
			this.PreRender +=new EventHandler(CorrDataGrid_PreRender);
			this.EnableViewState = true;
			this.hierarchy = new ArrayList();
			this.HierarchyCssClass =  "";
			this.PageSize = 6;
			this.DataSource = get_empty_grid_table ();
			this.DataBind();
			this.event_data = new DictionaryEntry("", "");
			this.hierarchy_cache = new Hashtable();
		}

		private BoundColumn new_bound_column (string header, string binding)
		{
			BoundColumn bc = new BoundColumn();
			bc.DataField = binding;
			bc.HeaderText = header;
			return bc;
		}

		/// <summary>
		/// Il numero degli elementi (righe) visualizzati da questa CorrDataGrid
		/// </summary>
		public override int PageSize
		{
			get
			{
				return base.PageSize;
			}
			set
			{
				base.PageSize = value;
				this.DataSource = null;
				this.DataBind();
			}
		}

		private DataTable get_empty_grid_table ()
		{
			DataTable dt = new DataTable("empty");
			dt.Columns.Add (new DataColumn (" "));
			dt.Columns.Add (new DataColumn ("  "));
			dt.Columns.Add (new DataColumn ("tipo"));
			dt.Columns.Add (new DataColumn ("descrizione"));
			dt.Columns.Add (new DataColumn ("dettagli"));
			for (int n = 0; n < this.PageSize; n++)
				dt.Rows.Add (new string[] {"","","","",""});

			return dt;
		}

		/// <summary>
		/// La sorgente dati per la CorrDataGrid
		/// </summary>
		/// <remarks>
		/// Può essere un array di <see cref="DocsPaWR.ElementoRubrica"/>, da
		/// usare per riempire la CorrDataGrid, o un qualsiasi altro valore, anche nullo; 
		/// nel secondo caso il valore viene ignorato e vengono visualizzate 
		/// delle righe vuote
		/// </remarks>
		public override object DataSource
		{
			get
			{
				return base.DataSource;
			}
			set
			{
				if (!(value is DocsPAWA.DocsPaWR.ElementoRubrica[])) 
				{
					this.AutoGenerateColumns = false;

					Columns.Clear();
					Columns.Add (new_bound_column (" ", ""));
					Columns.Add (new_bound_column ("  ", ""));
					Columns.Add (new_bound_column ("tipo", "tipo"));
					Columns.Add (new_bound_column ("descrizione", "descrizione"));
					Columns.Add (new_bound_column (" ", ""));	
					set_column_widths();
					base.DataSource = get_empty_grid_table ();
				}
				else
				{
					Columns.Clear();
					Columns.Add (new TemplateColumn());
					Columns.Add (new TemplateColumn());
					Columns.Add (new TemplateColumn());
					Columns.Add (new TemplateColumn());
					Columns.Add (new TemplateColumn());
                    
					((TemplateColumn) Columns[0]).ItemTemplate = (AllowMultipleSelection) ? (ITemplate) new CorrSelectTemplate() : (ITemplate) new CorrSelectSingleTemplate();
					((TemplateColumn) Columns[1]).ItemTemplate = new CorrDetailBtnTemplate();
					((TemplateColumn) Columns[2]).ItemTemplate = new CorrTypeBtnTemplate();
					((TemplateColumn) Columns[3]).ItemTemplate = new CorrDescTemplate();
					((TemplateColumn) Columns[4]).ItemTemplate = new CorrRemoveBtnTemplate();
					set_column_widths();
					set_column_headers();
					base.DataSource = value;
				}
				
			}

		}

		private void set_column_widths()
		{
			const int item_height = 28;
            
			Columns[0].ItemStyle.Width = new Unit(5, UnitType.Percentage);
			Columns[1].ItemStyle.Width = new Unit(5, UnitType.Percentage);
			Columns[2].ItemStyle.Width = new Unit(5, UnitType.Percentage);
			Columns[3].ItemStyle.Width = new Unit(60, UnitType.Percentage);
			Columns[4].ItemStyle.Width = new Unit(5, UnitType.Percentage);

			Columns[0].ItemStyle.Height = new Unit(item_height, UnitType.Pixel);
			Columns[1].ItemStyle.Height = new Unit(item_height, UnitType.Pixel);
			Columns[2].ItemStyle.Height = new Unit(item_height, UnitType.Pixel);
			Columns[3].ItemStyle.Height = new Unit(item_height, UnitType.Pixel);
			Columns[4].ItemStyle.Height = new Unit(item_height, UnitType.Pixel);

			Columns[0].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			Columns[1].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			Columns[2].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			Columns[3].ItemStyle.HorizontalAlign = HorizontalAlign.Left;
			Columns[4].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
		}

		private void set_column_headers()
		{

			Columns[0].HeaderText = " ";
			Columns[1].HeaderText = " ";
			Columns[2].HeaderText = "tipo";
			Columns[3].HeaderText = "descrizione";
			Columns[4].HeaderText = " ";
		}

		/// <summary>
		/// Il testo del Footer
		/// </summary>
		public string FooterText
		{
			// Qui c'e' un mezzo bug di .Net: dobbiamo piazzare il testo
			// custom del footer nella colonna 3 ma la colonna 3 non dovrebbe più 
			// esistere nel footer...
			set { this.Columns[this.ShowRemoveAll ? 3 : 0].FooterText = value; }
			get { return this.Columns[this.ShowRemoveAll ? 1 : 0].FooterText;  }
		}

		/// <summary>
		/// La pagina selezionata
		/// </summary>
		public int SelectedPage
		{
			get { return _selected_page; }
			set { _selected_page = value; }
		}

		private string build_pager()
		{
			string pager = "";
			string target = String.Format ("{0}:{1}", this.ClientID, CorrDataGrid.EVT_PGIDXCHANGED);

			int pstart = 0, pend = 0;
			if (this.DataSource is DocsPAWA.DocsPaWR.ElementoRubrica[] && 
				((DocsPAWA.DocsPaWR.ElementoRubrica[]) this.DataSource).Length > 0) 
			{
				int nitems = this.VirtualItemCount;
				int npages = ((nitems % this.PageSize) == 0) ? nitems / this.PageSize : nitems / (this.PageSize) + 1;
				int pidx = this.SelectedPage;
				int ellback = 0;

				pstart = pidx;
				pend = (pstart + this.PagerStyle.PageButtonCount) - 1 > npages ? npages : (pstart + this.PagerStyle.PageButtonCount) - 1;

				if (pstart > 1)
				{
					ellback = (pstart - this.PagerStyle.PageButtonCount) > 0 ? (pstart - this.PagerStyle.PageButtonCount) : 1;
					pager += String.Format ("<a href=\"javascript:{0};\" class=\"{1}\">...</a>\n", 
						String.Format ("javascript:doWait(); __doPostBack('{0}','{1}');", target, ellback.ToString()),
						this.FooterStyle.CssClass);

				}

				for (int i = pstart; i <= pend; i++) 
				{
					string arg = i.ToString();
					if (i != pidx)
						pager += String.Format ("<a href=\"{0};\" class=\"{2}\">{1}</a>\n", 
							String.Format ("javascript:doWait(); __doPostBack('{0}','{1}');", target, arg),
							i, 
							this.FooterStyle.CssClass);
					else
						pager += String.Format ("<span class=\"{1}\">{0}</span>\n", 
							i, 
							this.FooterStyle.CssClass);
				}

				if ((pend + 1) <= npages) 
					pager += String.Format ("<a href=\"javascript:{0};\" class=\"{1}\">...</a>\n", 
						String.Format ("javascript:doWait(); __doPostBack('{0}','{1}');", target, (pend + 1).ToString()),
						this.FooterStyle.CssClass);
			}
			//return pager + "&nbsp;" + this.VirtualItemCount.ToString();
			return pager;
		}

		private void removeAllBtn_PreRender(object sender, EventArgs e)
		{
			Image img = (Image) sender;
			string target = String.Format ("{0}:{1}", img.NamingContainer.NamingContainer.ClientID, CorrDataGrid.EVT_REMOVEALL);
			img.Attributes["onClick"] = String.Format ("javascript:doRemoveAll('{0}');", target);
		}

		private void CorrDataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
		{

			if (e.Item.ItemType == ListItemType.Footer) 
			{
				bool _l_show_remove_all = ShowRemoveAll;
				_l_show_remove_all = _l_show_remove_all && 
					(DataSource is DocsPAWA.DocsPaWR.ElementoRubrica[]) &&
					(((DocsPAWA.DocsPaWR.ElementoRubrica[])DataSource).Length > 0);

				int cols_to_remove = 1;
				int pager_column_index = 0;
				if (_l_show_remove_all) 
				{
					cols_to_remove = 3;
					pager_column_index = 1;
				}

				DataGridItem dgi = e.Item;
				int numcols = dgi.Cells.Count;
				for (int x = (numcols - cols_to_remove); x > 0 ; x--)
					dgi.Cells.RemoveAt (x);

				if (_l_show_remove_all) 
				{
					dgi.Cells[0].ColumnSpan = 1;
					dgi.Cells[0].HorizontalAlign = HorizontalAlign.Center;
					dgi.Cells[0].VerticalAlign = VerticalAlign.Middle;
					dgi.Cells[0].Width = new Unit("30px");
					dgi.Cells[0].Text = "&nbsp;";
				}

				dgi.Cells[pager_column_index].ColumnSpan = (_l_show_remove_all ? numcols - 2 : numcols);
				dgi.Cells[pager_column_index].HorizontalAlign = HorizontalAlign.Center;
				dgi.Cells[pager_column_index].VerticalAlign = VerticalAlign.Middle;

				if (_l_show_remove_all) 
				{
					dgi.Cells[2].ColumnSpan = 1;
					dgi.Cells[2].HorizontalAlign = HorizontalAlign.Right;
					dgi.Cells[2].VerticalAlign = VerticalAlign.Middle;
					dgi.Cells[2].Width = new Unit("30px");
					Image img = new Image();
					img.ToolTip = "Elimina tutti i corrispondenti dalla selezione";
					img.ImageUrl = "../../images/rubrica/b_elimina_small.gif";
					img.PreRender += new EventHandler(removeAllBtn_PreRender);
					dgi.Cells[2].Controls.Add (img);
				}
			}
		}

		private void CorrDataGrid_DataBinding(object sender, EventArgs e)
		{
			this.FooterText = build_pager();
		}

		/// <exclude></exclude>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine ("<table style=\"width:100%;border-collapse:collapse;border:1px solid #4b4b4b\">\n<tr>");
			writer.WriteLine ("<td class=\"" + this.HierarchyCssClass + "\" style=\"border-collapse:collapse;border:1px solid #4b4b4b;height:24px\">");
			render_hierarchy (writer);
			writer.WriteLine ("</td>\n</tr>");

			writer.WriteLine("<tr>\n<td style=\"border-collapse:collapse;border:1px solid #4b4b4b\">");
			base.Render (writer);
			writer.WriteLine ("</td>\n</tr>\n</table>\n");
		}

		#region IPostBackDataHandler Members

		/// <exclude></exclude>
		public void RaisePostDataChangedEvent()
		{

			switch ((string) event_data.Key) 
			{
				case CorrDataGrid.EVT_OPENSUBITEMS:
					if (event_data.Value is DocsPAWA.DocsPaWR.ElementoRubrica && OpenSubItems != null) 
					{
						DocsPaWR.ElementoRubrica er = (DocsPAWA.DocsPaWR.ElementoRubrica) event_data.Value;
						OpenSubItems (this, new OpenSubItemsArgs (er.codice, er.descrizione, er.tipo, er.interno));
					}
					break;

				case CorrDataGrid.EVT_PGIDXCHANGED:
					if (PageIndexChanged != null) 
						PageIndexChanged(this, new PageIndexChangedArgs((int) event_data.Value));
					break;

				case CorrDataGrid.EVT_HIERITMSELECTED:
					if (HierarchyElementSelected != null) 
					{
						HierarchyElement he = (HierarchyElement) event_data.Value;
						HierarchyElementSelected (this, new HierarchyElementSelectedArgs (he.Codice, he.Descrizione, he.Tipo, he.Interno));
					}
					break;

				case CorrDataGrid.EVT_DETAILSELECTED:
					if (DetailSelected != null) 
					{
						string cod = (string) event_data.Value;
						DetailSelected (this, new DetailSelectedArgs (cod));
					}
					break;

				case CorrDataGrid.EVT_REMOVEITEM:
					if (RemoveItem != null) 
					{
						string cod = (string) event_data.Value;
						RemoveItem (this, new RemoveItemArgs (cod));
					}
					break;

				case CorrDataGrid.EVT_REMOVEALL:
					if (RemoveAll != null) 
					{
						RemoveAll(this, new EventArgs());
					}
					break;

				case CorrDataGrid.EVT_SELECTORFILTER:
					if (SelectorFilter != null) 
					{
						string[] args = (string[]) event_data.Value;
						SelectorFilter(this, new SelectorFilterArgs(args[0], args[1]));
					}
					break;

				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Collegamento della CorrDataGrid alla sorgente dati
		/// </summary>
		public override void DataBind()
		{
			handle_paging();
			base.DataBind ();

		}

		internal bool IsChecked(string cod) 
		{
			if (VerifyItemSelection != null)
				return VerifyItemSelection (this, cod);
			else 
				return false;
		}

		public bool IsVisibleCheckOrRadio(DocsPAWA.DocsPaWR.ElementoRubrica er) 
		{
			if (er != null)
				return er.isVisibile ;
			else 
				return false;
		}

		internal bool IsSelectable(string tipo, string cod) 
		{
			if (SelectorFilter != null)
				return SelectorFilter (this, new SelectorFilterArgs (tipo, cod));
			else 
				return true;
		}

		private void handle_paging()
		{
			if (this.DataSource is DocsPAWA.DocsPaWR.ElementoRubrica[])
			{
				if (this.SelectedPage == 0)
					this.SelectedPage = 1;

				DocsPaWR.ElementoRubrica[] viewable_items = new DocsPAWA.DocsPaWR.ElementoRubrica[this.PageSize];
				DocsPaWR.ElementoRubrica[] all_items = (DocsPAWA.DocsPaWR.ElementoRubrica[]) this.DataSource;
				int first_item = (this.SelectedPage - 1) * this.PageSize;

				int item_count = 0;
				int num_pages = ((all_items.Length % this.PageSize) == 0) ? all_items.Length / this.PageSize : (all_items.Length / this.PageSize) + 1;
				if (all_items.Length > this.PageSize) 
				{
					if (this.SelectedPage == num_pages)
						if((all_items.Length % this.PageSize)==0)
						{
							item_count = this.PageSize;
						}
						else
						{
							item_count = all_items.Length % this.PageSize;
						}
					else
						item_count = this.PageSize;
				}
				else
				{
					item_count = all_items.Length;
				}
				
				Array.Copy (all_items, first_item, viewable_items, 0, item_count);

				for (int n = item_count; n < viewable_items.Length; n++) 
				{
					DocsPaWR.ElementoRubrica er  = new DocsPAWA.DocsPaWR.ElementoRubrica();
					er.codice = "";
					er.descrizione = "&nbsp;";
					er.tipo = "";
					viewable_items[n] = er;
				}
				if (!this.DisplayOnly) // HO NOTATO CHE VIENE FATTO 2 VOLTE..SARA' DA OTTIMIZZARE (23 GENNAIO 2007)
					check_children_existence (ref viewable_items);

				this.DataSource = viewable_items;
				this.VirtualItemCount = all_items.Length;

#if USE_HIERARCHY_CACHE
				string[] codici = new string[viewable_items.Length];
				AddressbookTipoUtente[] tipiIE = new AddressbookTipoUtente[viewable_items.Length];

				for (int i = 0; i < viewable_items.Length; i++) 
				{
					codici[i] = viewable_items[i].codice;
					tipiIE[i] = viewable_items[i].interno ? AddressbookTipoUtente.INTERNO : AddressbookTipoUtente.ESTERNO ;
				}
				foreach (ElementoRubrica[] eg in UserManager.getGerarchiaElementoRange (codici, tipiIE, this.Page)) 
				{
					ElementoRubrica er = eg[eg.Length - 1];
					string key = (er.interno ? @"E\" : @"I\") + er.codice;
					hierarchy_cache[key] = eg;
				}
#endif				
			}
		}

		private void check_children_existence (ref DocsPAWA.DocsPaWR.ElementoRubrica[] items)
		{
			UserManager.check_children_existence (this.Page, ref items);
		}

		/// <exclude></exclude>
		public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			HttpContext ctx = HttpContext.Current;
			string tgt = ctx.Request.Form["__EVENTTARGET"];
			string arg = ctx.Request.Form["__EVENTARGUMENT"];
			if (tgt == null || arg == null)
				return false;

			string[] a_tgt = tgt.Split(new char[] {':'});
			if (a_tgt[0].Equals (this.UniqueID)) 
			{
				// Ok, l'evento è per noi
				string[] a_arg = arg.Split(new char[] {':'});
				switch (a_tgt[1]) 
				{
					case CorrDataGrid.EVT_OPENSUBITEMS:
						DocsPaWR.ElementoRubrica er = new DocsPAWA.DocsPaWR.ElementoRubrica();
						er.codice = a_arg[0];
						er.tipo = a_arg[1];
						er.descrizione = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String (a_arg[2])); 
						er.interno = (a_arg[3] == "I");
						this.event_data.Key = CorrDataGrid.EVT_OPENSUBITEMS;
						this.event_data.Value = er;
						break;

					case CorrDataGrid.EVT_PGIDXCHANGED:
						this.event_data.Key = CorrDataGrid.EVT_PGIDXCHANGED;
						this.event_data.Value = Convert.ToInt32 (a_arg[0]);
						break;

					case CorrDataGrid.EVT_HIERITMSELECTED:
						HierarchyElement he = new HierarchyElement(a_arg[0], System.Text.Encoding.ASCII.GetString(Convert.FromBase64String (a_arg[2])), a_arg[1], a_arg[3] == "1");
						this.event_data.Key = CorrDataGrid.EVT_HIERITMSELECTED;
						this.event_data.Value = he;
						break;

					case CorrDataGrid.EVT_DETAILSELECTED:
						this.event_data.Key = CorrDataGrid.EVT_DETAILSELECTED;
						this.event_data.Value = a_arg[0];
						break;

					case CorrDataGrid.EVT_REMOVEITEM:
						this.event_data.Key = CorrDataGrid.EVT_REMOVEITEM;
						this.event_data.Value = a_arg[0];
						break;

					case CorrDataGrid.EVT_REMOVEALL:
						this.event_data.Key = CorrDataGrid.EVT_REMOVEALL;
						this.event_data.Value = null;
						break;

					case CorrDataGrid.EVT_SELECTORFILTER:
						this.event_data.Key = CorrDataGrid.EVT_SELECTORFILTER;
						this.event_data.Value = new string[] { a_arg[0], a_arg[1] };
						break;

					default:
						throw new NotImplementedException();
				}
				return true;
			}
			return false;
		}

		#endregion

		private void render_hierarchy_element (int cur_depth, TableCell container)
		{
			Table t = new Table();
			t.BorderWidth = new Unit("0");
			t.CellPadding = 0;
			t.CellSpacing = 0;
			TableRow tr = new TableRow();
			t.Rows.Add (tr);
			
			TableCell td = new TableCell();
			td.Width = new Unit (10 * cur_depth, UnitType.Pixel);
			tr.Cells.Add(td);
			td.Text = "";

			td = new TableCell();
			
			tr.Cells.Add(td);
			HtmlAnchor lnk = new HtmlAnchor();
			lnk.Attributes["class"] = this.HierarchyCssClass;
			HierarchyElement he = ((HierarchyElement) hierarchy[cur_depth]);

			string target = String.Format ("{0}:{1}", this.ClientID, CorrDataGrid.EVT_HIERITMSELECTED);
			string arg = String.Format ("{0}:{1}:{2}:{3}", he.Codice, he.Tipo, Convert.ToBase64String (System.Text.Encoding.ASCII.GetBytes(he.Descrizione)), he.Interno ? "1" : "0");

			lnk.HRef = String.Format ("javascript:__doPostBack('{0}','{1}');", target, arg);
			lnk.InnerText = he.Descrizione;
			td.Controls.Add (lnk);

			container.Controls.Add (t);
		}

		private void render_hierarchy (HtmlTextWriter txw)
		{
			int depth;
			if ((depth = hierarchy.Count) > 0) 
			{

#if RENDER_HIERARCHY_AS_PSEUDO_TREE
				Table t = new Table();
				t.BorderWidth = new Unit ("0");
				t.CellPadding = 0;
				t.CellSpacing = 0;
				for (int n = 0; n < depth; n++) 
				{
					TableRow tr = new TableRow();
					t.Rows.Add (tr);
					
					TableCell td = new TableCell();
					tr.Cells.Add(td);
					render_hierarchy_element (n, td);
					
				}
				t.RenderControl (txw);
#else
				for (int i = 0; i < hierarchy.Count; i++)
				{
					HierarchyElement he = (HierarchyElement) hierarchy[i];
					string target = String.Format ("{0}:{1}", this.ClientID, CorrDataGrid.EVT_HIERITMSELECTED);
					string arg = String.Format ("{0}:{1}:{2}:{3}", he.Codice, he.Tipo, Convert.ToBase64String (System.Text.Encoding.ASCII.GetBytes(he.Descrizione)), he.Interno ? "1" : "0");

					HtmlAnchor lnk = new HtmlAnchor();
					lnk.Attributes["class"] = this.HierarchyCssClass;
					lnk.HRef = String.Format ("javascript:__doPostBack('{0}','{1}');", target, arg);
					lnk.InnerText = he.Descrizione;
					lnk.RenderControl (txw);

					if (i != (hierarchy.Count - 1))
						txw.Write ("<span class=\"" + this.HierarchyCssClass + "\">&nbsp;/&nbsp;</span>");
				}
#endif
			}
		}

		/// <exclude></exclude>
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState (savedState);
			if (this.ViewState["hierarchy_state"] != null) 
				this.hierarchy = (ArrayList) this.ViewState["hierarchy_state"];
			if (this.ViewState["current_page"] != null) 
				this.SelectedPage = (int) this.ViewState["current_page"];
		}


		/// <summary>
		/// Aggiunge un elemento alla gerarchia visualizzata dalla CorrDataGrid
		/// </summary>
		/// <param name="elements">Gli elementi figli dell'elemento da aggiungere, che verranno
		/// visualizzati nella parte bassa della CorrDataGrid</param>
		/// <param name="ParentId">Il codice dell'elemento da aggiungere</param>
		/// <param name="ParentText">La descrizione dell'elemento da aggiungere</param>
		/// <param name="ParentType">Il tipo dell'elemento da aggiungere ("U", "R" o "P")</param>
		public void AddHierarchyElement (DocsPAWA.DocsPaWR.ElementoRubrica[] elements, string ParentId, string ParentText, string ParentType, bool ParentIsInterno)
		{
			this.DataSource = elements;
			this.DataBind();

			hierarchy.Add (new HierarchyElement (ParentId, ParentText, ParentType, ParentIsInterno));
		}

		/// <summary>
		/// Svuota la gerarchia della CorrDataGrid
		/// </summary>
		public void ClearHierarchy()
		{
			hierarchy.Clear();
		}

		/// <summary>
		/// Elimina dalla gerarchia un elemento e tutti quelli gerarchicamente 
		/// inferiori ad esso collegati
		/// </summary>
		/// <param name="cod">Il codice dell'elemento da eliminare</param>
		public void DeleteHierarchyElement(string cod)
		{
			for (int n = 0; n < hierarchy.Count; n++)
				if (((HierarchyElement)hierarchy[n]).Codice == cod) 
				{
					hierarchy.RemoveRange (n, hierarchy.Count - n);
					return;
				}
		}

		public void DeleteHierarchyElementChildren(string cod)
		{
			for (int n = 0; n < hierarchy.Count; n++)
				if (((HierarchyElement)hierarchy[n]).Codice == cod) 
				{
					if ((n+1) < hierarchy.Count) 
						hierarchy.RemoveRange (n+1, hierarchy.Count - (n + 1));
					return;
				}
		}

		private void CorrDataGrid_PreRender(object sender, EventArgs e)
		{
			this.ViewState.Add("hierarchy_state", hierarchy);
			this.ViewState.Add("current_page", SelectedPage);

			if (!this.Page.IsClientScriptBlockRegistered("apriDettagli")) 
			{
				string script = "<script language=\"javascript\">" +
                    "function apriDettagli(c, i, tp, rc, sysid)" +
					"{" +
                    "var w_width = 680; " +
					"var w_height = 700; " +
					"var t = 50; " +
					"var l = 540; " +
					"var opts = 'dialogHeight:' + w_height + 'px;dialogWidth:' + w_width + 'px;dialogLeft:' + l + 'px;dialogTop:' + t + 'px;center:yes;help:no;resizable:no;scroll:auto;status:no;unadorned:yes';" +
                    "var ret=window.showModalDialog (\"Dettagli.aspx?cod=\" + c + \"&ie=\" + i + \"&t=\" + tp + \"&rc=\" + rc + \"&sysid=\" + sysid, window, opts);" +
					"if (ret!=null) " +
					"{ " + 
					"window.document.forms['frmRubrica'].txtLoadCommand.value=ret; " +
					"window.document.forms['frmRubrica'].submit(); " + 
					"}" +
					"}" +
					"</script>";

				this.Page.RegisterClientScriptBlock ("apriDettagli", script);
			}

			if (!this.Page.IsClientScriptBlockRegistered ("imposta_cursore")) 
			{
				this.Page.RegisterClientScriptBlock ("imposta_cursore",
					"<script language=\"javascript\">\n" +
					"function ImpostaCursore (t, ctl)\n{\n" +
					"document.getElementById(ctl).style.cursor = (t == 0) ? 'default' : 'hand';\n" +
					"}\n</script>\n");
			}

			if (!this.Page.IsClientScriptBlockRegistered ("remove_all")) 
			{
				this.Page.RegisterClientScriptBlock ("remove_all",
					"<script language=\"javascript\">\n" +
					"function doRemoveAll (t)\n{\n" +
					"if (confirm(\"Sei sicuro di voler rimuovere dall'elenco i corrispondenti selezionati?\")) \n" +
					"__doPostBack(t,'');" +
					"}\n</script>\n");
			}
		}

		/// <summary>
		/// Restituisce un array di <see cref=DocsPaWR.ElementoRubrica""/> 
		/// contenente gli elementi selezionati
		/// </summary>
		/// <returns>L' array di <see cref=DocsPaWR.ElementoRubrica""/> 
		/// contenente gli elementi selezionati</returns>
		public DocsPAWA.DocsPaWR.ElementoRubrica[] GetSelected()
		{
            DocsPAWA.DocsPaWR.ElementoRubrica[] elementi;
            DocsPAWA.DocsPaWR.ElementoRubrica el = null;
			ArrayList a = new ArrayList();
			HttpContext ctx = HttpContext.Current;
			string s_codici = "";
			foreach (string the_key in ctx.Request.Form.AllKeys) 
			{
				string[] a_key = the_key.Split (new char[] {':'});
				string key = a_key[a_key.Length - 1];
				//if (key.StartsWith ("_CBX_[") && key.EndsWith ("]_CBX_")) 
                if (key.Contains("_CBX_[") && key.EndsWith("]_CBX_")) 
				{
					key = key.Substring (key.IndexOf ("_CBX_[") + "_CBX_[".Length);
					key = key.Substring (0, key.IndexOf ("]_CBX_"));

					s_codici += (key + "|");
				}
				if (key == "_CBX__CBX_") 
				{

					//a.Add (UserManager.getElementoRubrica (this.Page, ctx.Request.Form[key]));
					s_codici += (ctx.Request.Form[key] + "|");
				}
			}

			if (s_codici.EndsWith ("|"))
				s_codici = s_codici.Substring (0,s_codici.Length - 1);

			if (s_codici != "") 
			{
				string[] codici = System.Text.RegularExpressions.Regex.Split (s_codici, @"\|");
				//return UserManager.getElementiRubricaRange (codici, ((RubricaDocsPA) this.Page).TipoIE_Corrente, this.Page);
                //return UserManager.getElementiRubricaRange(codici, ((RubricaDocsPA)this.Page).TipoIE_Corrente, this.Page);
                if (codici.Length == 1)
                {

                    ElementoRubrica[] appoggioElemSingolo;
                    if (Char.IsNumber(codici[0], 0))
                        el = UserManager.getElementoRubricaSimpleBySystemId(this.Page, codici[0]);
                    if (el == null)
                    {
                        appoggioElemSingolo = UserManager.getElementiRubricaRange(codici, DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE, this.Page);
                        foreach (ElementoRubrica er in appoggioElemSingolo)
                        {
                            if (er.rubricaComune != null)
                            {
                                el = er;
                            }
                        }

                    }
                    elementi = new DocsPAWA.DocsPaWR.ElementoRubrica[1];
                    elementi[0] = el;
                    return elementi;
                }
                else if (codici.Length > 0)
                {
                    elementi = new DocsPAWA.DocsPaWR.ElementoRubrica[codici.Length];
                    ElementoRubrica[] appoggioElem;
                    for (int i = 0; i < codici.Length; i++)
                    {
                        string[] codiciRR = System.Text.RegularExpressions.Regex.Split(codici[i].ToString(), @"\|");
                        if (Char.IsNumber(codici[i], 0))
                            el = UserManager.getElementoRubricaSimpleBySystemId(this.Page, codici[i]);
                        if (el == null)
                        {
                            //el = RubricaComune.RubricaServices.g.GetElementoRubricaComune(this._user, codici[i]);
                            appoggioElem = UserManager.getElementiRubricaRange(codiciRR, DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE, this.Page);
                            foreach (ElementoRubrica er in appoggioElem)
                            {
                                if (er.rubricaComune != null)
                                {
                                    el = er;
                                }
                            }
                        }
                        elementi[i] = el;
                        el = null;
                    }
                    return elementi;
                }
                //return UserManager.getElementiRubricaRange (codici, DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE, this.Page);
                return new DocsPAWA.DocsPaWR.ElementoRubrica[0];
			}
			else
				return new DocsPAWA.DocsPaWR.ElementoRubrica[0];

		}
	}

	/// <exclude></exclude>
	public class CorrDescTemplate : ITemplate
	{
        /// <summary>
        /// Funzione per la costruzione di un elemento span in cui inserire codice e descrizione del corrispondente
        /// </summary>
        /// <param name="dest">Destinatario da verificare</param>
        /// <param name="text">Testo da inserire nell'elemento span</param>
        /// <returns>Elemento span con stile e testo impostati</returns>
        private String GetSpanElement(MittDest dest, String text)
        {
            // Span da utilizzare per la decorazione di codice e descrizione del destinatario
            String retVal = "<span style=\"color: {0}; {1}\">{2}</span>";

            // Se il corrispondente è disabilitato, viene colorato in nero e barrato
            if (dest.Disabled)
                retVal = String.Format(retVal, "Black", "text-decoration: line-through;", text);

            // Se il destinatario è inibito, viene visualizzato in rosso
            if (dest.Inhibited)
                retVal = String.Format(retVal, "Red", String.Empty, text);

            if (!dest.Inhibited && !dest.Disabled)
                retVal = String.Format(retVal, "Black", String.Empty, text);

            return retVal;
        }

		public void InstantiateIn (Control container)
		{
			LiteralControl l = new LiteralControl();
			l.DataBinding += new EventHandler(this.OnDataBinding);
			container.Controls.Add(l);
		}

		public void OnDataBinding (object sender, EventArgs e)
		{
			LiteralControl l = (LiteralControl) sender;
			DataGridItem container = (DataGridItem) l.NamingContainer;
			CorrDataGrid dg = (CorrDataGrid) container.NamingContainer;
			DocsPaWR.ElementoRubrica er = (DocsPAWA.DocsPaWR.ElementoRubrica) container.DataItem;
            if (er != null)
            {
                
                if ((!dg.ShowItemHierarchyInList) || (er.codice == ""))
                {
                    string codRegTemp = er.codiceRegistro;
                    if (er.isRubricaComune == true)
                    {
                        codRegTemp = " [RC]";
                    }
                    else
                    {
                        if (codRegTemp == null || codRegTemp.Equals(""))
                        {
                            if (er.interno==true)
                            {
                                codRegTemp = "";
                            }
                            else
                            {
                                codRegTemp = " [TUTTI]";
                            }
                        }
                        else
                        {
                            codRegTemp = " [" + er.codiceRegistro + "]";
                        }
                    }
                    l.Text = (er.codice != "" ? er.descrizione + " (" + er.codice + ")" + codRegTemp : "");
                }
                else
                {
                    l.Text = "";
                    string id_amm = UserManager.getInfoUtente(l.Page).idAmministrazione;
#if USE_HIERARCHY_CACHE
					string key = (er.interno ? @"E\" : @"I\") + er.codice;
					DocsPaWR.ElementoRubrica[] ers = (DocsPAWA.DocsPaWR.ElementoRubrica[]) dg.hierarchy_cache[key];
#else
                    DocsPaWR.ElementoRubrica[] ers = UserManager.getGerarchiaElemento(er.codice,
                        er.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO,
                        dg.Page);
#endif

                    foreach (DocsPAWA.DocsPaWR.ElementoRubrica ee in ers)
                        l.Text += (ee.descrizione + " / ");

                    l.Text = l.Text.Substring(0, l.Text.Length - " / ".Length);
                }

                // Se il ruolo è inibito, viene colorato di rosso
                if (er.disabledTrasm)
                    l.Text = String.Format("<span style=\"color:Red;\">{0}</span>", l.Text);

                // Se il ruolo è disabilitato, viene visualizzato nero barrato
                if (er.disabled)
                    l.Text = String.Format("<span style=\"text-decoration: line-through;\">{0}</span>", l.Text);


            }
            else
                l.Text = "&nbsp;";
		}
	}

	/// <exclude></exclude>
	public class CorrSelectTemplate : ITemplate
	{
		private SmarterCheckBox cb = null;

		public void InstantiateIn (Control container)
		{
			cb = new SmarterCheckBox();
			cb.DataBinding += new EventHandler(this.OnDataBinding);
			container.Controls.Add(cb);
		}

		public void OnDataBinding (object sender, EventArgs e)
		{
			HttpContext ctx = HttpContext.Current;

			SmarterCheckBox cb = (SmarterCheckBox) sender;
			DataGridItem container = (DataGridItem) cb.NamingContainer;
			DocsPaWR.ElementoRubrica er = (DocsPAWA.DocsPaWR.ElementoRubrica) container.DataItem;
			CorrDataGrid dg = (CorrDataGrid) container.NamingContainer;
			DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

            if (er.systemId != null && er.systemId != "")
            {
                cb.ID = "_CBX_[" + er.systemId + "]_CBX_";
                cb.Name = "_CBX_[" + er.systemId + "]_CBX_";
                cb.Checked = dg.IsChecked(er.systemId);
            }
            else
            {
                cb.ID = "_CBX_[" + er.codice + "]_CBX_";
                cb.Name = "_CBX_[" + er.codice + "]_CBX_";
                cb.Checked = dg.IsChecked(er.codice);
            }
			
			bool visibilita = dg.IsVisibleCheckOrRadio(er);

			cb.Visible = visibilita;

			
			//				if (!dg.ShowSelectors || (er.codice == "" && cb != null) || !dg.IsSelectable (er.interno ? "I" : "E", er.codice))
			//				{

			//cb.Visible = false;
				
			//Questo controllo viene effettuato in quanto se è atttiva la chiave "RUBRICA_PROTO_USA_SMISTAMENTO", 
			//una eventuale ricerca in rubrica delle UO-Ruoli-Utenti che appartengono a UO sottoposte quella 
			//dell'utente loggato, devono essere comunque selezionabili - PER ANAS

				if(!dg.ShowSelectors)
				{
					cb.Visible = false;
				}
                //if(dg.ShowSelectors && DocsPAWA.Utils.getAbilitazioneSmistamento().Equals("1") &&
                //    ( (RubricaCallType)Convert.ToInt32 (dg.CallType) == RubricaCallType.CALLTYPE_PROTO_OUT || 
                //    (RubricaCallType)Convert.ToInt32 (dg.CallType) == RubricaCallType.CALLTYPE_PROTO_INT_DEST) &&
                //    er.codice != "") 
                //{
				
                //    if(((er.tipo=="U" && !cb.Visible))&& er.interno == true)
                //    {
                //        if(wws.verificaDipendezaCodRubrica(dg.CodUoAppartenenza,er.codice,dg.InfoUtente))
                //        {
                //            cb.Visible = true;
                //        }
                //    }
                //    if((er.tipo!="U")&& er.interno == true)
                //    {
                //        if(wws.verificaDipendezaCodRubrica(dg.CodUoAppartenenza,er.codice,dg.InfoUtente))
                //        {
                //            cb.Visible = true;
                //        }
                //        else
                //        {
                //            cb.Visible = false;
                //        }
                //    }
                //}
			//}
		}
				
	}

		/// <exclude></exclude>
		public class SmarterRadioButton : HtmlInputRadioButton 
		{
			protected override void Render(HtmlTextWriter writer)
			{
				string ctl = "<input id=\"" + this.ID + "\" type=\"radio\" name=\"" + this.Name + "\" value=\"" + this.Value + "\">";
				writer.WriteLine (ctl);
			}

		}

		/// <exclude></exclude>
		public class SmarterCheckBox : HtmlInputCheckBox
		{
			protected override void Render(HtmlTextWriter writer)
			{
				string ctl = "<input id=\"" + this.ID + "\" type=\"checkbox\" name=\"" + this.Name + "\" value=\"" + this.Value + "\"" + (this.Checked ? " checked" : "") + ">";
				writer.WriteLine (ctl);
			}

		}

		/// <exclude></exclude>
		public class CorrSelectSingleTemplate : ITemplate
		{
			private SmarterRadioButton rb = null;

			public void InstantiateIn (Control container)
			{
				rb = new SmarterRadioButton();
				rb.DataBinding += new EventHandler(this.OnDataBinding);
				container.Controls.Add(rb);
			}

			public void OnDataBinding (object sender, EventArgs e)
			{
				SmarterRadioButton rb = (SmarterRadioButton) sender;
				DataGridItem container = (DataGridItem) rb.NamingContainer;
				DocsPaWR.ElementoRubrica er = (DocsPAWA.DocsPaWR.ElementoRubrica) container.DataItem;
			

				CorrDataGrid dg = (CorrDataGrid) container.NamingContainer;
			
				rb.Visible = dg.IsVisibleCheckOrRadio(er);

				if (!dg.ShowSelectors || (er.codice == "" && rb != null) || !dg.IsSelectable (er.interno ? "I" : "E", er.codice))
				{
					rb.Visible = false;
				}

                rb.Name = "_CBX__CBX_";
                if (er.systemId != null && er.systemId != "")
                {
                    rb.ID = "_CBX_[" + er.systemId + "]_CBX_";
                    rb.Value = er.systemId + "";
                }
                else
                {
                    rb.ID = "_CBX_[" + er.codice + "]_CBX_";
                    rb.Value = er.codice + "";
                }
			}
		}

		/// <exclude></exclude>
		public class CorrDetailBtnTemplate : ITemplate
		{
			public void InstantiateIn (Control container)
			{
				Image img = new Image();
				img.ImageUrl = "../../images/rubrica/dettaglio_attivo.gif";
				img.DataBinding += new EventHandler (this.OnDataBinding);
				container.Controls.Add(img);
			}

			public void OnDataBinding (object sender, EventArgs e)
			{
				Image img = (Image) sender;
		
				DataGridItem container = (DataGridItem) img.NamingContainer;
				DocsPaWR.ElementoRubrica er = (DocsPAWA.DocsPaWR.ElementoRubrica) container.DataItem;
                
				HttpContext  svu = HttpContext.Current;
				string target = String.Format ("{0}:{1}", container.NamingContainer.ClientID, CorrDataGrid.EVT_DETAILSELECTED);
				
                string arg = String.Format ("{0}:{1}", 
                                        svu.Server.UrlEncode(er.codice.Replace("'","|@ap@|")), 
                                                            er.interno ? "1" : "0");
				img.ToolTip = "Visualizza i dettagli per questo corrispondente";

                img.Attributes["onClick"] = String.Format("javascript:apriDettagli('{0}', '{1}', '{2}', '{3}', '{4}');",
                            svu.Server.UrlEncode(er.codice.Replace("'", "|@ap@|")),
                                                er.interno ? "I" : "E",
                                                er.tipo,
                                                (er.rubricaComune != null ? er.rubricaComune.IdRubricaComune.ToString() : ""), er.systemId);

				img.ID = "_btnDtl_" + this.GetHashCode().ToString() + "_" + svu.Server.UrlEncode(er.codice.Replace("'","|@ap@|")) + "_";
				img.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + img.ClientID + "');";
				img.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + img.ClientID + "');";
			
				if (er.codice == "")
					img.Visible = false;
			}
		}

		/// <exclude></exclude>
		public class CorrRemoveBtnTemplate : ITemplate
		{
			public void InstantiateIn (Control container)
			{
				Image img = new Image();
				img.ImageUrl = "../../images/rubrica/b_elimina.gif";
				img.DataBinding += new EventHandler (this.OnDataBinding);
			
				container.Controls.Add(img);
			}

			public void OnDataBinding (object sender, EventArgs e)
			{
				Image img = (Image) sender;
				HttpContext  svu = HttpContext.Current;
				DataGridItem container = (DataGridItem) img.NamingContainer;
				DocsPaWR.ElementoRubrica er = (DocsPAWA.DocsPaWR.ElementoRubrica) container.DataItem;
                if (er.systemId == null)
                {
                    er.systemId = "";
                }
                string arg = "";

				string target = String.Format ("{0}:{1}", container.NamingContainer.ClientID, CorrDataGrid.EVT_REMOVEITEM);
                if (er.systemId != "")
                {
                    arg = String.Format("{0}", svu.Server.UrlEncode(er.systemId.Replace("'", "|@ap@|")));
                }
                else
                {
                    arg = String.Format("{0}", svu.Server.UrlEncode(er.codice.Replace("'", "|@ap@|")));
                }
                //				 = String.Format ("{0}", svu.Server.UrlEncode(er.systemId.Replace("'","|@ap@|")));
				img.Attributes["onClick"] = String.Format ("javascript:__doPostBack('{0}','{1}');", target, arg);
				img.ToolTip = "Rimuovi questo corrispondente dall'elenco di quelli selezionati";

                if (er.systemId != "")
                {
                    img.ID = "_btnRem_" + this.GetHashCode().ToString() + "_" + svu.Server.UrlEncode(er.systemId.Replace("'", "|@ap@|")) + "_";
                }
                else
                {
                    img.ID = "_btnRem_" + this.GetHashCode().ToString() + "_" + svu.Server.UrlEncode(er.codice.Replace("'", "|@ap@|")) + "_";
                }
				img.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + img.ClientID + "');";
				img.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + img.ClientID + "');";

				CorrDataGrid dg = (CorrDataGrid) container.NamingContainer;
				if (!dg.ShowRemoveButtons || (er.codice == ""))
					img.Visible = false;
			}
		}

		/// <exclude></exclude>
		public class CorrTypeBtnTemplate : ITemplate
		{
			public void InstantiateIn (Control container)
			{
				Image img = new Image();
				img.DataBinding += new EventHandler(this.OnDataBinding);
				container.Controls.Add(img);
			}

			public void OnDataBinding (object sender, EventArgs e)
			{
				HttpContext  svu = HttpContext.Current;
				Image img = (Image) sender;
				DataGridItem container = (DataGridItem) img.NamingContainer;
				DocsPaWR.ElementoRubrica er = (DocsPAWA.DocsPaWR.ElementoRubrica) container.DataItem;

				string target = String.Format ("{0}:{1}", container.NamingContainer.ClientID, CorrDataGrid.EVT_OPENSUBITEMS);
				string arg = String.Format ("{0}:{1}:{2}:{3}", svu.Server.UrlEncode(er.codice.Replace("'","|@ap@|")), er.tipo, Convert.ToBase64String (System.Text.Encoding.ASCII.GetBytes(er.descrizione)), er.interno ? "I" : "E");
			
                img.ImageUrl = String.Format("../../images/rubrica/{0}", er.tipo.ToLower());

				switch (er.tipo.ToUpper()) 
				{
					case "U":
						img.ToolTip = "Tipo corrispondente: Unità Organizzativa";
						break;

					case "R":
						img.ToolTip = "Tipo corrispondente: Ruolo";
						break;

					case "P":
						img.ToolTip = "Tipo corrispondente: Utente";
						break;

				}

				SimpleTabStrip tsMode = (container).Page.FindControl("tsMode") as SimpleTabStrip;
				DropDownList ddlTipoIE = (container).Page.FindControl("ddlIE") as DropDownList;
				bool enableOrg = true;
				if(tsMode!= null)
				{
					if((tsMode.Tabs!=null)&& tsMode.Tabs.Count==1)
					{
						enableOrg = false;
					}
				}

				if (((CorrDataGrid) container.NamingContainer).DisplayOnly || !enableOrg || !er.interno || (ddlTipoIE!=null && ddlTipoIE.SelectedValue.Equals("E")) ) 
				{
					img.Attributes["onClick"] = "";
					img.ImageUrl += "_noexp.gif";
				}
				else
				{
				
					img.Attributes["onClick"] = String.Format ("javascript:doWait(); __doPostBack('{0}','{1}');", target, arg);
					img.ImageUrl += "_exp.gif";
					img.ID = "_btnOpen_" + this.GetHashCode().ToString() + "_" +  svu.Server.UrlEncode(er.codice.Replace("'","|@ap@|")) + "_";
					img.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + img.ClientID + "');";
					img.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + img.ClientID + "');";
				}

				if (er.codice == "" && img != null) 
					img.Visible = false;
			}
		}

		/// <summary>
		/// Un'istanza di questa classe viene passata come argomento all'evento
		/// <see cref="OpenSubItems"/>
		/// </summary>
		public class OpenSubItemsArgs 
		{
			/// <summary>
			/// La descrizione dell'elemento selezionato
			/// </summary>
			public string Descrizione;

			/// <summary>
			/// Il codice dell'elemento selezionato
			/// </summary>
			public string Codice;

			/// <summary>
			/// Il tipo ("U", "R" o "P") dell'elemento selezionato
			/// </summary>
			public string Tipo;

			/// <summary>
			/// Indica se l'elemento selezionato è interno (true) o esterno (false)
			/// </summary>
			public bool Interno;

			public OpenSubItemsArgs (string c, string d,  string t, bool i)
			{
				this.Codice = c;
				this.Descrizione = d;
				this.Tipo = t;
				this.Interno = i;
			}
		}

		/// <summary>
		/// Un'istanza di questa classe viene passata come argomento all'evento
		/// <see cref="HierarchyElementSelected"/>
		/// </summary>
		public class HierarchyElementSelectedArgs 
		{
			/// <summary>
			/// L'elemento della gerarchia selezionato
			/// </summary>
			public CorrDataGrid.HierarchyElement Element;

			public HierarchyElementSelectedArgs (string c, string d, string t, bool i)
			{
				this.Element = new CorrDataGrid.HierarchyElement(c, d, t, i);
			}
		}

		/// <summary>
		/// Un'istanza di questa classe viene passata come argomento all'evento
		/// <see cref="PageIndexChanged"/>
		/// </summary>
		public class PageIndexChangedArgs 
		{
			/// <summary>
			/// La pagina appena selezionata
			/// </summary>
			public int NewPage;

			public PageIndexChangedArgs (int p)
			{
				this.NewPage = p;
			}
		}

		/// <summary>
		/// Un'istanza di questa classe viene passata come argomento all'evento
		/// <see cref="DetailSelected"/>
		/// </summary>
		public class DetailSelectedArgs 
		{
			/// <summary>
			/// Il codice dell'elemento di cui l'utente ha richiesto la
			/// visualizzazione del dettaglio
			/// </summary>
			public string Codice;

			public DetailSelectedArgs (string c)
			{
				this.Codice = c;
			}
		}

		/// <summary>
		/// Un'istanza di questa classe viene passata come argomento all'evento
		/// <see cref="RemoveItem"/>
		/// </summary>
		public class RemoveItemArgs 
		{		
			/// <summary>
			/// Il codice dell'elemento di cui l'utente ha richiesto la rimozione
			/// </summary>
			public string Codice;
            public string SystemID;
            public string DaElim;

			public RemoveItemArgs (string c)
			{
				//this.Codice = c;
                //this.SystemID = c;
                this.DaElim = c;
			}
		}

	
	
}
