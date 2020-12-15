namespace DocsPAWA.SitoAccessibile.WebControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	///	TreeView accessibile
	/// </summary>
	[Serializable()]
	public class AccessibleTreeView : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl divContainer;
		protected System.Web.UI.HtmlControls.HtmlGenericControl tvheader;

		private string _headerText=string.Empty;
		private AccessibleTreeNodeCollection _nodes=new AccessibleTreeNodeCollection();

		private void Page_Load(object sender, System.EventArgs e)
		{

		}

		/// <summary>
		/// Impostazione / reperimento intestazione
		/// </summary>
		public string HeaderText
		{
			get
			{
				return this._headerText;
			}
			set
			{
				this._headerText=value;
			}
		}

		protected override void LoadViewState(object savedState)
		{
			if (savedState!=null)
			{
				// Load State from the array of objects that
				// was saved in SaveViewState
				object[] vState = (object[])savedState;

				if (vState[0]!=null)
					base.LoadViewState(vState[0]);

				if (vState[1]!=null)
					this.RestoreNodes((ArrayList) vState[1]);
			}
		}

		protected override object SaveViewState()
		{
			ArrayList retValue=new ArrayList();

			retValue.Add(base.SaveViewState());
			retValue.Add(this.PersistNodes());

			return retValue.ToArray(typeof(object));
		}

		private void RestoreNodes(ArrayList nodes)
		{
			foreach (Hashtable properties in nodes)
			{
				AccessibleTreeNode treeNode=new AccessibleTreeNode();

				treeNode.ID=properties["id"].ToString();
				treeNode.Text=properties["text"].ToString();
				treeNode.Link=properties["link"].ToString();
				
				try
				{
					if (Convert.ToBoolean(properties["expanded"]))
						treeNode.Expand();
				}
				catch
				{
				}

				this.Nodes.Add(treeNode);
			}
		}

		private ArrayList PersistNodes()
		{
			ArrayList nodes=new ArrayList();

			foreach (AccessibleTreeNode treeNode in this.Nodes)
			{	
				Hashtable properties=new Hashtable();

				properties.Add("id",treeNode.ID);
				properties.Add("text",treeNode.Text);
				properties.Add("link",treeNode.Link);
				properties.Add("expanded",treeNode.Expanded);

				nodes.Add(properties);
			}

			return nodes;
		}

		/// <summary>
		/// Collection dei nodi del treeview
		/// </summary>
		public AccessibleTreeNodeCollection Nodes
		{
			get
			{
				return this._nodes;
			}
		}

		/// <summary>
		/// Impostazione visibilità campo intestazione
		/// </summary>
		private void SetHeaderVisibility()
		{
			//this.tvheader.Visible=(HeaderText!=null && HeaderText!=string.Empty);
			this.tvheader.Visible=true; //impostazione temporanea
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.SetHeaderVisibility();

			this.RenderTreeView();
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
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		private void RenderTreeView()
		{
			HtmlGenericControl ul=new HtmlGenericControl("ul");
			// Donato 24-10-2006
			ul.Attributes.Add("id", "treemenu1");
			ul.Attributes.Add("class", "treeview");

			foreach (AccessibleTreeNode treeNode in this.Nodes)
				ul.Controls.Add(this.CreateNode(treeNode));

			this.divContainer.Controls.Add(ul);
		}

		private HtmlGenericControl CreateNode(AccessibleTreeNode treeNode)
		{
			HtmlGenericControl li=new HtmlGenericControl("li");

			li.Controls.Add(treeNode);

			if (treeNode.ChildNodes.Count>0)
				li.Attributes.Add("class","openednode");

			if (treeNode.Expanded && treeNode.ChildNodes.Count>0)
			{
				HtmlGenericControl ulChild=new HtmlGenericControl("ul");

				foreach (AccessibleTreeNode childNode in treeNode.ChildNodes)
					ulChild.Controls.Add(this.CreateNode(childNode));

				li.Controls.Add(ulChild);
			}
			
			return li;
		}

		[Serializable()]
		public class AccessibleTreeNodeCollection : CollectionBase
		{
			private AccessibleTreeNode _parentNode=null;

			internal AccessibleTreeNodeCollection()
			{
			}

			public void Add(AccessibleTreeNode treeNode)
			{
				this.InnerList.Add(treeNode);

				treeNode.SetParentNode(this._parentNode);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="parentNode"></param>
			internal void SetParentNode(AccessibleTreeNode parentNode)
			{
				this._parentNode=parentNode;
			}
		}

		[Serializable()]
		public class AccessibleTreeNode : WebControl,INamingContainer
		{
			private AccessibleTreeNode _parentNode=null;
			private AccessibleTreeNodeCollection _childNodes=new AccessibleTreeNodeCollection();
			
			private HtmlInputButton _buttonOpenFolder=null;
			private HtmlAnchor _anchor=null;

			protected override void CreateChildControls()
			{
				this._buttonOpenFolder=new HtmlInputButton();
				this._buttonOpenFolder.Value="";
				this._buttonOpenFolder.Attributes.Add("class","openFolder");

				this._anchor=new HtmlAnchor();
				this._anchor.InnerText=this.Text;
				this._anchor.HRef=this.Link.Replace("&", "&amp;");
				this.Controls.Add(this._buttonOpenFolder);
				this.Controls.Add(this._anchor);
			}

			protected override void Render(HtmlTextWriter output)
			{
				this._buttonOpenFolder.RenderControl(output);
				this._anchor.RenderControl(output);
			}

			public AccessibleTreeNode()
			{
			}

			public AccessibleTreeNode(string id) : this()
			{	
				this.ID=id;
			}
			
			public AccessibleTreeNode(string id,string text) : this(id)
			{
				this.Text=text;
			}

			public AccessibleTreeNode(string id,string text,string link) : this(id,text)
			{
				this.Link=link;
			}

			public AccessibleTreeNodeCollection ChildNodes
			{
				get
				{
					return this._childNodes;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="parentNode"></param>
			internal void SetParentNode(AccessibleTreeNode parentNode)
			{
				this._parentNode=parentNode;
			}

			/// <summary>
			/// Restituzione nodo padre
			/// </summary>
			public AccessibleTreeNode ParentNode
			{
				get
				{
					return this._parentNode;
				}
			}

			/// <summary>
			/// Verifica se il nodo è espanso
			/// </summary>
			public bool Expanded
			{
				get
				{
					bool expanded=false;

					try
					{
						expanded=Convert.ToBoolean(this.GetProperty("expanded"));
					}
					catch
					{
					}
					
					return expanded;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			private void SetExpanded(bool expanded)
			{
				this.SetProperty("expanded",expanded.ToString());
			}

			/// <summary>
			/// Link del nodo
			/// </summary>
			public string Link
			{
				get
				{
					return this.GetProperty("link");
				}
				set
				{
					this.SetProperty("link",value);
				}
			}

			/// <summary>
			/// Testo del nodo
			/// </summary>
			public string Text
			{
				get
				{
					return this.GetProperty("text");
				}
				set
				{
					this.SetProperty("text",value);
				}
			}

			private void SetProperty(string propertyName,string propertyValue)
			{
				this.ViewState[propertyName]=propertyValue;
			}

			private string GetProperty(string propertyName)
			{
				if (this.ViewState[propertyName]!=null)
					return this.ViewState[propertyName].ToString();
				else
					return string.Empty;
			}
 
			/// <summary>
			/// Espansione del nodo corrente
			/// </summary>
			public void Expand()
			{
				this.SetExpanded(true);
			}

			/// <summary>
			/// Espansione del nodo corrente e di tutti i nodi figlio
			/// </summary>
			public void ExpandAll()
			{
				this.Expand();

				foreach (AccessibleTreeNode childNode in this.ChildNodes)
					childNode.Expand();
			}
		}
	}
}