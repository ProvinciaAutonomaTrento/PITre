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
using DocsPAWA.SitoAccessibile.Ricerca;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile
{
	/// <summary>
	/// Summary description for Home.
	/// </summary>
	public class Home : SessionWebPage
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl todoListContainer;
		protected System.Web.UI.WebControls.DropDownList cboRuoli;
		protected System.Web.UI.WebControls.DropDownList cboSearchType;
		protected System.Web.UI.WebControls.Button btnSearch;
		protected System.Web.UI.WebControls.Button btnSetRole;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{
				this.Fetch();

				this.RefreshSelectedRoleDescription();
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
			this.btnSetRole.Click += new System.EventHandler(this.btnSetRole_Click);
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnSearch_Click(object sender,EventArgs e)
		{
			TrasmissioniRicevute trasmRicevute=this.GetControlTrasmissioniDocumenti();
			trasmRicevute.Visible=true;
			trasmRicevute.Fetch(this.TipoRicercaTrasmissione,true);
		}

		private void btnSetRole_Click(object sender, System.EventArgs e)
		{
			this.SetSelectedRole(this.cboRuoli.SelectedItem.Value);
		}

		/// <summary>
		/// Impostazione del ruolo selezionato
		/// </summary>
		/// <param name="systemID"></param>
		private void SetSelectedRole(string systemID)
		{
			Utente currentUser=this.GetCurrentUser();

			Ruolo selectedRole=null;

			foreach (Ruolo role in currentUser.ruoli)
			{
				role.selezionato=false;

				if (role.systemId.Equals(systemID))
				{
					selectedRole=role;
					selectedRole.selezionato=true;
				}
			}

			// Impostazione del ruolo corrente in sessione
			UserManager.setRuolo(this,selectedRole);

			this.RefreshSelectedRoleDescription();
		}

		/// <summary>
		/// Aggiornamento ruolo correntemente selezionato
		/// </summary>
		private void RefreshSelectedRoleDescription()
		{	
			UserContext userContext=this.FindControl("UserContext") as UserContext;
			if (userContext!=null)
				userContext.RefreshUserContext();
		}

		/// <summary>
		/// Gestione caricamento dati
		/// </summary>
		private void Fetch()
		{
			this.FetchRoles();
			
			this.FetchTipiRicerca();

			// Caricamento todolist trasmissioni ricevute di documenti
			if (Documenti.Trasmissioni.Configurations.AutomaticSearchTodoList)
			{
				TrasmissioniRicevute trasmRicevute=this.GetControlTrasmissioniDocumenti();
				trasmRicevute.Visible=true;
				trasmRicevute.Fetch(this.TipoRicercaTrasmissione,true);
			}
		}

		/// <summary>
		/// Verifica se ricercare in todolist i documenti o i fascicoli
		/// </summary>
		private TipiRicercaTrasmissioniEnum TipoRicercaTrasmissione
		{
			get
			{
				return (TipiRicercaTrasmissioniEnum) 
					Enum.Parse(typeof(TipiRicercaTrasmissioniEnum),this.cboSearchType.SelectedItem.Value,true);
			}
		}

		/// <summary>
		/// Caricamento ruoli disponibili
		/// </summary>
		private void FetchRoles()
		{
			Utente currentUser=this.GetCurrentUser();

			foreach (Ruolo role in currentUser.ruoli)
			{
				ListItem item=new ListItem(role.descrizione,role.systemId);
				item.Selected=role.selezionato;
				this.cboRuoli.Items.Add(item);
			}
		}

		/// <summary>
		/// Caricamento combo tipi ricerca
		/// </summary>
		private void FetchTipiRicerca()
		{
			this.cboSearchType.Items.Add(new ListItem("Documenti",TipiRicercaTrasmissioniEnum.Documenti.ToString()));
			this.cboSearchType.Items.Add(new ListItem("Fascicoli",TipiRicercaTrasmissioniEnum.Fascicoli.ToString()));
		}

		private TrasmissioniRicevute GetControlTrasmissioniDocumenti()
		{
			return this.FindControl("trasmissioniRicevute") as TrasmissioniRicevute;
		}
        
        public bool existsLogoAmm
        {
            get
            {
                return fileExist("logo.gif", "LoginFE");
            }
            // set { login.IdAmministrazione = value; }
        }

        private bool fileExist(string fileName, string type)
        {
            return FileManager.fileExist(fileName, type);
        }
    }
}