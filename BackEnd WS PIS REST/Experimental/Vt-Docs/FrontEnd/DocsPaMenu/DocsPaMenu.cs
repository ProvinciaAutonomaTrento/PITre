using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Specialized;
using log4net;


/// <summary></summary>
namespace docsPaMenu
{
	public class myLink
	{
		private string _target;
		private string _type;
		private string _href;
		private string _text;
		private bool _visible;
		private int _widthCell;
		private string _WndOpenProprities;
		private string _clientScriptAction;
		private string _key;

		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
		public string Target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
			}
		}
		public string Href
		{
			get
			{
				return _href;
			}
			set
			{
				_href = value;
			}
		}
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
			}
		}
		public int WidthCell
		{
			get
			{
				return _widthCell;
			}
			set
			{
				_widthCell = value;
			}
		}
		public string WndOpenProprities
		{
			get
			{
				return _WndOpenProprities;
			}
			set
			{
				_WndOpenProprities = value;
			}
		}

		/// <summary>
		/// Funzione o codice script client da
		/// eseguire al click della voce di menù
		/// </summary>
		public string ClientScriptAction
		{
			get
			{
				return this._clientScriptAction;
			}
			set
			{
				this._clientScriptAction=value;
			}
		}

		public string Key
		{
			get
			{
				return this._key;
			}
			set
			{
				this._key=value;
			}
		}
	}

	
	public class myLinkCollection : System.Collections.CollectionBase 
	{
        private ILog logger = LogManager.GetLogger(typeof(myLinkCollection));
		public void Add(myLink l)
		{
			this.List.Add(l);
		}


		public void Remove(int index)
		{
			// Check to see if there is a widget at the supplied index.
			if (index > this.Count - 1 || index < 0)
				// If no widget exists, a messagebox is shown and the operation 
				// is cancelled.
			{
				logger.Error("DocsPaMenu - Indice fuori range!");
				throw new Exception("index is out of the range") ;
			}
			else
			{
				this.List.RemoveAt(index); 
			}
		}


		public myLink this[int index]
		{
			get
			{
				return (myLink) List[index];
			}
		}


	}





	/// <summary>
	/// Summary description for WebCustomControl1.
	/// </summary>
	[ParseChildren(true, "Links")]
	[DefaultProperty("CssHLC"),Description("Menu"),
	ToolboxData("<{0}:DocsPaMenuWC runat=server></{0}:DocsPaMenuWC>")]
	public class DocsPaMenuWC : System.Web.UI.WebControls.WebControl,INamingContainer
	{
        private ILog logger = LogManager.GetLogger(typeof(DocsPaMenuWC));
		public event EventHandler Click;

		Table _table;
		private myLinkCollection _links = new myLinkCollection();

		public  DocsPaMenuWC()
		{
			ViewState["CssHLC"]=" ";
			ViewState["MenuPosLeft"]="0";
			ViewState["widthTable"]="0";
		}



		[Bindable(true),Browsable(true),Category("Data")]
		[PersistenceMode(System.Web.UI.PersistenceMode.InnerProperty)]
		public myLinkCollection Links
		{
			get
			{
				return _links;
			}
		}

		[Bindable(true),Browsable(true),
		Category("Appearance"),
		DefaultValue(" "),PersistenceMode(System.Web.UI.PersistenceMode.Attribute)]
		public string CssHLC
		
		{
			get { return ViewState["CssHLC"].ToString(); }
			set{ViewState["CssHLC"]=value.ToString();}
		}
		[Bindable(true),Browsable(true),
		Category("Appearance"),PersistenceMode(System.Web.UI.PersistenceMode.Attribute),
		DefaultValue("100")]
		public int WidthTable
		
		{
			get { return Int32.Parse(ViewState["widthTable"].ToString()); }
			set{ViewState["widthTable"]=value.ToString();}
		}

		
		[Bindable(true),Browsable(true),
		Category("Appearance"),
		DefaultValue("0"),PersistenceMode(System.Web.UI.PersistenceMode.Attribute)]
		public int MenuPosLeft
		
		{
			get { return Int32.Parse(ViewState["MenuPosLeft"].ToString()); }
			set{ViewState["MenuPosLeft"]=value.ToString();}
		}
	

	
		[Bindable(true),Browsable(true),
		Category("Appearance"),PersistenceMode(System.Web.UI.PersistenceMode.Attribute)
		]
		Style _tableStyle=new Style();
		
		public Style MyTableStyle
		{
			get{return _tableStyle;}
		}
		Style _cellStyle=new Style();
		[Bindable(true),Browsable(true),
		Category("Appearance"),PersistenceMode(System.Web.UI.PersistenceMode.Attribute)
		]
		public Style MyCellStyle
		{
			get {return _cellStyle;}
		}
		protected override void CreateChildControls()
		{
			LiteralControl text;
			LiteralControl text2;
			//LiteralControl script1;
			
			
			text=new LiteralControl("<!--Menu inzio--><DIV id=\"box\" style=\"Z-INDEX: 102; LEFT: "+MenuPosLeft+"\"><div align=\"left\"><table  cellSpacing=\"0\" cellPadding=\"0\" bgColor=\"#ffffff\" border=\"0\"><tr><td>");
			Controls.Add(text);
			TableRow row;
			TableCell cell;

			_table=new Table();
			_table.CellPadding=3;
			_table.CellSpacing=1;
			_table.Width=Unit.Percentage(100);//Unit.Pixel(this.WidthTable);
			_table.ApplyStyle(this._tableStyle);
			
			Controls.Add(_table);

            row = new TableRow();
            _table.Rows.Add(row);
			
			if(this._links==null)
			{
				logger.Error("MenuWC requires a datasource for Href Urls");
				throw new Exception("MenuWC requires a datasource for Href Urls");
			}
			if(this._links.Count==0)
			{
				logger.Error("MenuWC requires a datasource for Href Targets");
				throw new Exception("MenuWC requires a datasource for Href Targets");
			}
						
			int t=1;
			for(int i=0;i<this._links.Count;i++)
			{

             
				
				cell=new TableCell();
				cell.Attributes.Add("onmouseover","this.className='cellMenuOver';");
				cell.Attributes.Add("onmouseover","this.className='cellMenuOut';");
				cell.HorizontalAlign=HorizontalAlign.Center;
				cell.CssClass="cellMenuOut";
				cell.BorderWidth=Unit.Pixel(1);
				cell.ID="mnubar"+t.ToString();
				//cell.Width=Unit.Pixel(this._links[i].WidthCell);
				cell.ApplyStyle(this._cellStyle);
                cell.Visible=this._links[i].Visible;
				LinkButton lb=new LinkButton();
				lb.CssClass=this.CssHLC;
				lb.ID=lb.ClientID;
				lb.CommandArgument=this._links[i].Type;
				lb.CommandName = "Click";
				cell.Wrap=false;

				if (this.Links[i].ClientScriptAction!=null &&
					this.Links[i].ClientScriptAction!=string.Empty)
				{
					lb.Attributes.Add("OnClick",this.Links[i].ClientScriptAction);
				}
				else if(this._links[i].Target != "_new" && this._links[i].Target != "_blank")
				{
					if(this.Links[i].WndOpenProprities != null && !this.Links[i].WndOpenProprities.Trim().Equals("")) //per attivare rubrica: || this.Links[i].Text=="Rubrica"
						lb.Attributes.Add("OnClick", "window.open('"+this._links[i].Href+"','"+this._links[i].Target+"','"+this.Links[i].WndOpenProprities+"');");
					else
						lb.Attributes.Add("OnClick", "top."+this._links[i].Target+".location='"+this._links[i].Href+"';");

//					lb.Text=this._links[i].Text;
//					lb.Visible=this._links[i].Visible;
//				
//					//	cell.Text="<A runat='server' id="+this.UniqueID+i.ToString()+" onserverclick='btn_doc_Click()' href=\""+this._links[i].Href+"\" class=\""+this.CssHLC+"\" onclick='closeIt();' target=\""+this._links[i].Target+"\">"+this._links[i].Text+"</A>";
//					t++;
//					cell.Controls.Add(lb);
//					row.Cells.Add(cell);
				}
//				else
//				{
//					/*Oss:  prospettiRiepilogativi() è una funzione javascript
//					inserita nell'html della pagina testata2.aspx, che serve ad 
//					aprire a tutto schermo la pagina dei prospetti riepilogativi*/
//					lb.Attributes.Add("OnClick", "prospettiRiepilogativi();");	
//				}

				lb.Text=this._links[i].Text;
				lb.Visible=this._links[i].Visible;

				t++;
				cell.Controls.Add(lb);
				row.Cells.Add(cell);

				if(this.Links[i].WndOpenProprities == "JS") 
				{
					lb.Attributes.Add("OnClick", this._links[i].Href);
				}
			}
	
			text2=new LiteralControl("</td></tr></table></div></DIV><!--Fine menu-->");	
			Controls.Add(text2);
			

			
			
		}
		
		protected override bool OnBubbleEvent(object source, EventArgs e) 
		{   
			bool handled = false;
			if (e is CommandEventArgs)
			{
				CommandEventArgs ce = (CommandEventArgs)e;
				if (ce.CommandName == "Click")
				{
					OnClick(ce);
					handled = true;   
				}  
			}
			return handled;            
		}
		protected virtual void OnClick (EventArgs e)
		{
			if (Click != null)
			{
				Click(this,e);
			}
		}


	

		
	}
}
