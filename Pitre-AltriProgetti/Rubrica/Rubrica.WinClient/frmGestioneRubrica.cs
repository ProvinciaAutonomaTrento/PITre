using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Rubrica.WinClient
{
    public partial class frmGestioneRubrica : Form
    {
        private Rubrica.ClientProxy.SecurityCredentials _credentials = null;
        private bool _amministratore = false;

        public frmGestioneRubrica(Rubrica.ClientProxy.SecurityCredentials credentials, bool amministratore)
        {
            InitializeComponent();

            this._credentials = credentials;
            this._amministratore = amministratore;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadDetail()
        {
            IDetail detail = this.GetCurrentDetail();

            if (detail != null)
            {
                detail.SetCredentials(this._credentials);
                detail.LoadData();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            Rubrica.ClientProxy.UtentiServiceProxy proxy = new Rubrica.ClientProxy.UtentiServiceProxy(this._credentials, Properties.Settings.Default.ServiceRoot);

            proxy.ChangePassword(
                                        new Rubrica.ClientProxy.UtentiServices.ChangePwdSecurityCredentials
                                        {
                                            UserName = this._credentials.UserName, 
                                            Password = this._credentials.Password,
                                            NewPassword = this.txtNewPassword.Text
                                        }
                                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmGestioneRubrica_Load(object sender, EventArgs e)
        {
            this.LoadDetail();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbRubrica_Selected(object sender, TabControlEventArgs e)
        {
            this.LoadDetail();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IDetail GetCurrentDetail()
        {
            if (this._amministratore && this.tbRubrica.SelectedTab == this.tabUtenti)
                return this.detailUtentiRubrica;
            else if (this.tbRubrica.SelectedTab == this.tabElementi)
                return this.detailElementiRubrica;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            this.GetCurrentDetail().RequestInsert();

            this.btnNew.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.GetCurrentDetail().SaveCurrent();

            this.btnNew.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.GetCurrentDetail().DeleteCurrent();
        }


    }
}
