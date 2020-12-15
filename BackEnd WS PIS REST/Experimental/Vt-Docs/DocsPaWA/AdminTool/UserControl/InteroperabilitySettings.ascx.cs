using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.AdminTool.Manager;
using DocsPAWA.utils;
using Amministrazione.Manager;
using AmmUtils;

namespace DocsPAWA.AdminTool.UserControl
{
    public partial class InteroperabilitySettings : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Il controllo sarà visibile solo se è attiva l'IS
            this.Visible = InteroperabilitaSemplificataManager.IsEnabledSimpInterop;

            // Popolamento della drop down list con i tipi di interoperabilità
            if (this.ddlManagement.Items.Count == 0)
            {
                this.ddlManagement.Items.Add(new ListItem("Automatica", ManagementType.A.ToString()));
                this.ddlManagement.Items.Add(new ListItem("Manuale", ManagementType.M.ToString()));
                this.ddlManagement.SelectedValue = ManagementType.A.ToString();
            }
        }

        /// <summary>
        /// Id del registro o RF cui si riferisce questo controllo
        /// </summary>
        public String RegistryId
        {
            get
            {
                return ViewState["RegistryId"].ToString();
            }

            set
            {
                ViewState["RegistryId"] = value;

                // Caricamento e visualizzazione delle impostazioni
                this.LoadSettings(value);

            }
        }

        /// <summary>
        /// Metodo per il caricamento delle impostazioni per un registro
        /// </summary>
        /// <param name="registryId">Id del registro</param>
        private void LoadSettings(string registryId)
        {
            try
            {
                // Caricamento dati
                DocsPaWR.InteroperabilitySettings interoperabilitySettings =
                    InteroperabilitaSemplificataManager.LoadSimplifiedInteroperabilitySettings(registryId);

                // Visualizzazione dati
                this.txtInteroperabilityUrl.Text = InteroperabilitaSemplificataManager.InteroperabilityServiceUrl;
                if (interoperabilitySettings != null)
                {
                    this.RuoloResponsabile1.RoleSystemId = interoperabilitySettings.RoleId.ToString();
                    this.RuoloResponsabile1.UserSystemId = interoperabilitySettings.UserId.ToString();
                    this.chkEnableInterop.Checked = interoperabilitySettings.IsEnabledInteroperability;
                    this.ddlManagement.SelectedValue = interoperabilitySettings.ManagementMode.ToString();
                    this.chkKeepPrivate.Checked = interoperabilitySettings.KeepPrivate;
                }

                this.pnlSettings.Enabled = this.chkEnableInterop.Checked;
                this.chkKeepPrivate.Enabled = interoperabilitySettings.ManagementMode == ManagementType.M;
            }
            catch (Exception e)
            {
                // Alert ...
            }

        }

        /// <summary>
        /// Metodo per il salvataggio delle impostazioni di interoperabilità semplificata relative a questo
        /// registro
        /// </summary>
        public void SaveSettings()
        {
            DocsPaWR.InteroperabilitySettings interoperabilitySettings = new DocsPaWR.InteroperabilitySettings();
            interoperabilitySettings.IsEnabledInteroperability = this.chkEnableInterop.Checked;
            interoperabilitySettings.KeepPrivate = this.chkKeepPrivate.Checked;
            interoperabilitySettings.ManagementMode = (ManagementType)Enum.Parse(typeof(ManagementType), this.ddlManagement.SelectedValue);
            interoperabilitySettings.RegistryId = this.RegistryId;

            int appVal = 0;
            if (Int32.TryParse(this.RuoloResponsabile1.RoleSystemId, out appVal))
                interoperabilitySettings.RoleId = appVal;
            if (Int32.TryParse(this.RuoloResponsabile1.UserSystemId, out appVal))
                interoperabilitySettings.UserId = appVal;

            try
            {
                // Salvataggio delle impostazioni
                bool saved = InteroperabilitaSemplificataManager.SaveSimplifiedInteroperabilitySettings(interoperabilitySettings);

            }
            catch (Exception e)
            {

                throw new Exception(DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e).Message);

            }

        }

        protected void ddlManagement_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as DropDownList).SelectedValue == ManagementType.A.ToString())
            {
                this.chkKeepPrivate.Enabled = false;
                this.chkKeepPrivate.Checked = false;
            }
            else
                this.chkKeepPrivate.Enabled = true;
        }

        protected void chkEnableInterop_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlSettings.Enabled = (sender as CheckBox).Checked;
        }

    }
}