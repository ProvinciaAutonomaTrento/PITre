namespace DocsPAWA.SitoAccessibile.Validations
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Web.UI;

	/// <summary>
	///		Summary description for ValidationContainer.
	/// </summary>
	public class ValidationContainer : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl validationContainers;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblHeader;
		protected System.Web.UI.HtmlControls.HtmlGenericControl controlsContainer;
		protected System.Web.UI.HtmlControls.HtmlAnchor linkFirstField;
		protected System.Web.UI.HtmlControls.HtmlGenericControl headerContainer;

		private void Page_Load(object sender, System.EventArgs e)
		{
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			// Rendering del contenuto del controllo di validazione
			this.RenderContent();
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

		#region Public methods

		/// <summary>
		/// Verifica validità elementi
		/// </summary>
		public bool IsValid()
		{
			bool retValue=true;

			foreach (ValidationItem item in this.GetValidationItemList())
			{
				retValue=item.IsValid();

				if (!retValue)
					break;
			}

			return retValue;
		}
		
		/// <summary>
		/// Impostazione / reperimento intestazione controllo di validazione
		/// </summary>
		public string HeaderText
		{
			get
			{
				return this.headerContainer.InnerText;
			}
			set
			{
				this.headerContainer.InnerText=value;
			}
		}

		/// <summary>
		/// Impostazione di una regola di validazione legata ad un controllo
		/// </summary>
		/// <param name="descriptionControlID">Identifica l'ID del controllo grafico che descrive il/i campo/i per cui si effettua la validazione</param>
		/// <param name="rule">Descrizione della regola di validazione</param>
		public void SetControlRule(string descriptionControlID,string rule)
		{
			ValidationItem entry=this.GetValidationEntry(descriptionControlID);
			
			if (entry==null)
			{
				entry=new ValidationItem(descriptionControlID);
				this.GetValidationItemList().Add(entry);
			}
		
			entry.RuleDescription=rule;
		}

		/// <summary>
		/// Inserimento di un messaggio di errore
		/// </summary>
		/// <param name="errorMessage"></param>
		public void AddControlErrorMessage(string errorMessage)
		{
			this.AddControlErrorMessage(string.Empty,errorMessage);
		}

		/// <summary>
		/// Inserimento di un messaggio di errore legato ad un controllo
		/// </summary>
		/// <param name="descriptionControlID">Identifica l'ID del controllo grafico che descrive il/i campo/i per cui si effettua la validazione</param>
		/// <param name="errorMessage">Messaggio di errore</param>
		public void AddControlErrorMessage(string descriptionControlID,string errorMessage)
		{
			ValidationItem entry=this.GetValidationEntry(descriptionControlID);
			
			if (entry==null)
			{
				entry=new ValidationItem(descriptionControlID);
				this.GetValidationItemList().Add(entry);
			}

			entry.ErrorMessageItems.Add(errorMessage);
		}

		public void RemoveControl(string descriptionControlID)
		{
			ValidationItem entry=this.GetValidationEntry(descriptionControlID);

			if (entry!=null)
				this.GetValidationItemList().Remove(entry);
		}

		public void ClearControls()
		{
			this.GetValidationItemList().Clear();
		}

		#endregion

		#region Private methods

		private const string VALIDATION_KEY="ValidationContainerList";

		/// <summary>
		/// Reperimento di un elemento di validazione
		/// </summary>
		/// <param name="descriptionControlID"></param>
		/// <returns></returns>
		private ValidationItem GetValidationEntry(string descriptionControlID)
		{
			ValidationItem retValue=null;

			ArrayList list=this.GetValidationItemList();

			foreach (ValidationItem entry in list)
			{	
				if (entry.DescriptionControlID.Equals(descriptionControlID))
				{
					retValue=entry;
					break;
				}
			}

			return retValue;
		}
		
		/// <summary>
		/// Reperimento lista elementi di validazione
		/// </summary>
		/// <returns></returns>
		private ArrayList GetValidationItemList()
		{
			if (this.ViewState[VALIDATION_KEY]==null)
				this.ViewState.Add(VALIDATION_KEY,new ArrayList());

			return (ArrayList) this.ViewState[VALIDATION_KEY];
		}

		/// <summary>
		/// Rendering del contenuto del controllo di validazione
		/// </summary>
		private void RenderContent()
		{
			// Lista elementi di validazione
			ArrayList list=this.GetValidationItemList();

			// Controlli di validazione creati
			ArrayList validationControls=new ArrayList();

			foreach (ValidationItem entry in list)
			{	
				if (entry.DescriptionControlID!=string.Empty)
				{
					// Reperimento controllo che descrive il controllo validato
					Control descriptionControl=this.FindLabelControl(this.Page,entry.DescriptionControlID);

					if (descriptionControl==null)
						throw new ApplicationException("Controllo '" + entry.DescriptionControlID + "' non trovato");
					
					descriptionControl.Controls.Add(this.CreateEmHiddenControl(entry.DescriptionControlID,entry.ToString()));
				}

				// Creazione controlli per i singoli messaggi di errore
				validationControls.AddRange(this.CreateErrorMessageControls(entry));

				// Rimozione di tutti i messaggi di errore
				entry.ErrorMessageItems.Clear();
			}

			this.validationContainers.Visible=(validationControls.Count>0);
			
			if (this.validationContainers.Visible)
			{
				HtmlGenericControl parentCtl=new HtmlGenericControl("ul");
				foreach (HtmlGenericControl childCtl in validationControls)
					parentCtl.Controls.Add(childCtl);
				this.controlsContainer.Controls.Add(parentCtl);

				// Impostazione visibilità controllo di validazione;
				// solo se almeno un elemento è presente
				this.validationContainers.Visible=true;
			}
		}

		/// <summary>
		/// Reperimento controllo che descrive il controllo validato
		/// </summary>
		/// <param name="containerControl"></param>
		/// <param name="labelControlId"></param>
		/// <returns></returns>
		private Control FindLabelControl(Control containerControl,string labelControlId)
		{
			// Reperimento controllo che descrive il controllo validato
			Control descriptionControl=containerControl.FindControl(labelControlId);
			
			if (descriptionControl==null)
			{
				string[] items=labelControlId.Split('_');
				if (items.Length==2)
				{
					Control container=this.Page.FindControl(items[0]);
					if (container!=null)
						descriptionControl=this.FindLabelControl(container,items[1]);
				}
			}

			return descriptionControl;
		}

		/// <summary>
		/// Creazione controllo nascosto con tag "Em" relativo al controllo di validazione
		/// </summary>
		/// <param name="descriptionControlID"></param>
		/// <param name="descriptionText"></param>
		/// <returns></returns>
		private HtmlGenericControl CreateEmHiddenControl(string descriptionControlID,string descriptionText)
		{
			HtmlGenericControl ctl=new HtmlGenericControl("em");
			ctl.ID="em_" + descriptionControlID;
			ctl.Attributes.Add("class","hidden");
			ctl.InnerText=descriptionText;
			return ctl;
		}

		/// <summary>
		/// Creazione controlli relativi a tutti i messaggi di errore 
		/// relativamente ad un singolo oggetto "ValidationItem"
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private HtmlGenericControl[] CreateErrorMessageControls(ValidationItem item)
		{
			ArrayList list=new ArrayList();

			foreach (string errorMessage in item.ErrorMessageItems)
			{
				HtmlGenericControl ctl=new HtmlGenericControl("li");
				ctl.InnerText=errorMessage;
				list.Add(ctl);
			}

			return (HtmlGenericControl[]) list.ToArray(typeof(HtmlGenericControl));
		}
	
		#endregion


		/// <summary>
		/// Oggetto contenente le informazioni di una singola validazione dati
		/// </summary>
		[Serializable()]
		private class ValidationItem
		{
			private string _descriptionControlID=string.Empty;
			private string _ruleDescription=string.Empty;
			private StringCollection _errorMessageItems=new StringCollection();

			public ValidationItem()
			{
			}

			public ValidationItem(string descriptionControlID)
			{
				this._descriptionControlID=descriptionControlID;
			}

			public ValidationItem(string descriptionControlID,string ruleDescription) : this(descriptionControlID)
			{
				this._ruleDescription=ruleDescription;
			}

			/// <summary>
			/// Identifica l'ID del controllo grafico che descrive il/i campo/i per cui si effettua la validazione
			/// </summary>
			public string DescriptionControlID
			{
				get
				{
					return this._descriptionControlID;
				}
				set
				{
					this._descriptionControlID=value;
				}
			}

			/// <summary>
			/// Identifica la regola per la validazione
			/// </summary>
			public string RuleDescription
			{
				get
				{
					return this._ruleDescription;
				}
				set
				{
					this._ruleDescription=value;
				}
			}
      
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public bool IsValid()
			{
				return (this.ErrorMessageItems.Count==0);
			}

			/// <summary>
			/// Collection contenente gli eventuali messaggi di errore di una validazione
			/// </summary>
			public StringCollection ErrorMessageItems
			{
				get
				{
					return this._errorMessageItems;
				}
				set
				{
					this._errorMessageItems=value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				string retValue=string.Empty;

				if (!this.IsValid())
				{
					foreach (string errorItem in this.ErrorMessageItems)
					{
						if (retValue!=string.Empty)
							retValue+="; ";

						retValue+=errorItem;
					}
				}
				else
				{
					retValue=this.RuleDescription;
				}

				return retValue;
			}
		}
	}
}