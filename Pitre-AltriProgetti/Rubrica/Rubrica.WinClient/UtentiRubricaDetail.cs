using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rubrica.ClientProxy.UtentiServices;

namespace Rubrica.WinClient
{
    public partial class UtentiRubricaDetail : UserControl, IDetail
    {
        public UtentiRubricaDetail()
        {
            InitializeComponent();
        }

        #region IDetail Members


        /// <summary>
        /// 
        /// </summary>
        private ClientProxy.SecurityCredentials Credentials
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        public void SetCredentials(ClientProxy.SecurityCredentials credentials)
        {
            this.Credentials = credentials;
        }

        /// <summary>
        /// 
        /// </summary>
        protected Rubrica.ClientProxy.UtentiServiceProxy Proxy
        {
            get
            {
                return new Rubrica.ClientProxy.UtentiServiceProxy(this.Credentials, Rubrica.WinClient.Properties.Settings.Default.ServiceRoot);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Current
        {
            get
            {
                if (this.listView1.SelectedItems.Count > 0)
                    return (Rubrica.ClientProxy.UtentiServices.Utente)this.listView1.SelectedItems[0].Tag;
                else
                    return null;
            }
        }

        public void RequestInsert()
        {
            this.InsertMode = true;

            this.SetControlsEnabled();

            this.ClearAll();
        }

        public void LoadData()
        {
            this.listView1.Items.Clear();
            foreach (Utente utente in this.Proxy.Search(null))
                this.listView1.Items.Add(this.CreateItem(utente));
        }

        public void SaveCurrent()
        {
            Rubrica.ClientProxy.UtentiServices.Utente utente = null;

            if (this.InsertMode)
            {
                utente = this.Proxy.Insert(this.CreateNew());

                this.InsertMode = false;
            }
            else
            {
                utente = this.Proxy.Update(this.GetUtenteRubrica());
            }

            this.LoadData();
        }

        public void DeleteCurrent()
        {
            if (this.Current != null)
            {
                this.Proxy.Delete(((Rubrica.ClientProxy.UtentiServices.Utente)this.Current).Id);

                this.LoadData();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        private bool InsertMode
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        private ListViewItem CreateItem(Rubrica.ClientProxy.UtentiServices.Utente utente)
        {
            ListViewItem item = new ListViewItem();
            item.Tag = utente;
            item.Text = utente.Id.ToString();
            item.SubItems.Add(utente.Nome);
            item.SubItems.Add(utente.Amministratore.ToString());
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Rubrica.ClientProxy.UtentiServices.Utente CreateNew()
        {
            return new Rubrica.ClientProxy.UtentiServices.Utente
            {
                 Nome = this.txtNome.Text,
                 Password = this.txtPassword.Text,
                 Amministratore = this.chkAmministratore.Checked
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void Bind(Rubrica.ClientProxy.UtentiServices.Utente utente)
        {
            this.txtId.Text = utente.Id.ToString();
            this.txtNome.Text = utente.Nome;
            this.txtPassword.Text = string.Empty;
            this.chkAmministratore.Checked = utente.Amministratore;
            this.txtDataCreazione.Text = utente.DataCreazione.ToString();
            this.txtDataUltimaModifica.Text = utente.DataUltimaModifica.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearAll()
        {
            this.txtId.Text = string.Empty;
            this.txtNome.Text = string.Empty;
            this.txtPassword.Text = string.Empty;
            this.chkAmministratore.Checked = false;
            this.txtDataCreazione.Text = string.Empty;
            this.txtDataUltimaModifica.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Rubrica.ClientProxy.UtentiServices.Utente GetUtenteRubrica()
        {
            Utente utente = new Utente();

            utente.Id = Convert.ToInt32(this.txtId.Text);
            utente.Nome = this.txtNome.Text;
            utente.Amministratore = this.chkAmministratore.Checked;
            utente.DataCreazione = Convert.ToDateTime(this.txtDataCreazione.Text);
            utente.DataUltimaModifica = Convert.ToDateTime(this.txtDataUltimaModifica.Text);

            return utente;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Current != null)
            {
                this.SetControlsEnabled();

                this.Bind((Rubrica.ClientProxy.UtentiServices.Utente)this.Current);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetControlsEnabled()
        {
            this.txtNome.Enabled = this.InsertMode;
        }
    }
}
