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
    /// <summary>
    /// 
    /// </summary>
    public partial class Login : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public Rubrica.ClientProxy.SecurityCredentials Credentials
        {
            get
            {
                return new Rubrica.ClientProxy.SecurityCredentials
                {
                    UserName = this.txtUserName.Text,
                    Password = this.txtPassword.Text
                };
            }
        }

        public bool Amministratore
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            Rubrica.ClientProxy.UtentiServiceProxy proxy = new Rubrica.ClientProxy.UtentiServiceProxy(this.Credentials, Rubrica.WinClient.Properties.Settings.Default.ServiceRoot);

            bool amministratore;

            Rubrica.ClientProxy.UtentiServices.SecurityCredentialsResult result = proxy.ValidateCredentials();

            if (result != null)
            {
                this.Amministratore = result.Amministratore;

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(Properties.Resources.AuthenticationFailedException);
            }
        }
    }
}
