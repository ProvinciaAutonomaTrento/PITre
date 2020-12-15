using System;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Documenti
{
	/// <summary>
	/// Summary description for DettaglioDocumento.
	/// </summary>
	public class DettaglioDocumentoUserControl : System.Web.UI.UserControl
	{
		private SchedaDocumento _schedaDocumento=null;

		private bool _readOnlyMode=false;

		private bool _initialized=false;

		private Hashtable _parameters=new Hashtable();

		public DettaglioDocumentoUserControl()
		{
		}

		protected override void OnLoad(EventArgs e)
		{
			this.BindLabelsToFields();

			base.OnLoad (e);

			if (this.IsInitialized() && !this.IsPostBack)
			{
				// Caricamento dati
				this.Fetch();
			}
		}

		public bool IsInitialized()
		{
			return this._initialized;
		}

		/// <summary>
		/// Inizializzazione controllo
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <param name="readOnlyMode"></param>
		public virtual void Initialize(SchedaDocumento schedaDocumento,bool readOnlyMode)
		{
			this._schedaDocumento=schedaDocumento;

			this.SetReadOnlyMode(readOnlyMode);

			this._initialized=true;
		}

		/// <summary>
		/// Impostazione parametro opzionale maschera
		/// </summary>
		/// <param name="parameterKey"></param>
		/// <param name="parameterValue"></param>
		public void SetParameter(object parameterKey,object parameterValue)
		{
			if (this._parameters.ContainsKey(parameterKey))
				this._parameters[parameterKey]=parameterValue;
			else
				this._parameters.Add(parameterKey,parameterValue);
		}

		/// <summary>
		/// Reperimento parametro opzionale maschera
		/// </summary>
		/// <param name="parameterKey"></param>
		/// <returns></returns>
		public object GetParameter(object parameterKey)
		{
			if (this._parameters.ContainsKey(parameterKey))
				return this._parameters[parameterKey];
			else
				return null;
		}

		/// <summary>
		/// Caricamento dati maschera
		/// </summary>
		protected virtual void Fetch()
		{
			throw new ApplicationException("Fetch - Operazione non accessibile");
		}

		/// <summary>
		/// Aggiornamento dei dati presenti nei campi della UI
		/// ad un nuovo oggetto schedaDocumento, che sovrascrive il precedente
		/// </summary>
		/// <param name="schedaDocumento"></param>
		public virtual void Update(SchedaDocumento schedaDocumento)
		{
			this._schedaDocumento=schedaDocumento;

			this.Update();
		}

		/// <summary>
		/// Aggiornamento dei dati presenti nei campi della UI
		/// all'oggetto schedaDocumento
		/// </summary>
		public virtual void Update()
		{
			if (!this.IsReadOnlyMode())
				this.OnUpdate();
		}

		public virtual bool IsValid()
		{
			throw new ApplicationException("IsValid - Operazione non accessibile");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isReadOnly"></param>
		public void SetReadOnlyMode(bool isReadOnly)
		{
			this._readOnlyMode=isReadOnly;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsReadOnlyMode()
		{
			return this._readOnlyMode;
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnUpdate()
		{
			throw new ApplicationException("OnUpdate - Operazione non accessibile");
		}

		/// <summary>
		/// Reperimento scheda documento corrente
		/// </summary>
		/// <returns></returns>
		protected SchedaDocumento GetSchedaDocumento()
		{
			return this._schedaDocumento;
		}

		/// <summary>
		/// Associazione attributo for per i campi label richiesti
		/// </summary>
		protected virtual void BindLabelsToFields()
		{
		}

		/// <summary>
		/// Associazione attributo "for"
		/// </summary>
		/// <param name="labelControl"></param>
		/// <param name="controlToBind"></param>
		protected virtual void BindLabelToField(HtmlGenericControl labelControl,System.Web.UI.Control controlToBind)
		{
			labelControl.Attributes.Add("for",controlToBind.ClientID);
		}
	}
}