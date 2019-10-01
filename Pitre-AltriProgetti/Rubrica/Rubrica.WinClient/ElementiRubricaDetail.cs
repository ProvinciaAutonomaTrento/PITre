using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Rubrica.WinClient
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ElementiRubricaDetail : UserControl, IDetail
    {
        public ElementiRubricaDetail()
        {
            InitializeComponent();

            this.cmdTipoCorrispondente.Items.AddRange(new object[]
                { 
                    new TipoCorrispondente { Descrizione = "Unità Organizzativa", Value = ClientProxy.RubricaServices.Tipo.UO },
                    new TipoCorrispondente { Descrizione = "RF", Value = ClientProxy.RubricaServices.Tipo.RF }
                });
            this.cmdTipoCorrispondente.SelectedValue = 0;
        }

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
        protected Rubrica.ClientProxy.RubricaServiceProxy Proxy
        {
            get
            {
                return new Rubrica.ClientProxy.RubricaServiceProxy(this.Credentials, Rubrica.WinClient.Properties.Settings.Default.ServiceRoot);
            }
        }

        #region IDetail Members

        /// <summary>
        /// 
        /// </summary>
        public object Current
        {
            get 
            {
                if (this.listView1.SelectedItems.Count > 0)
                    return (Rubrica.ClientProxy.RubricaServices.ElementoRubrica)this.listView1.SelectedItems[0].Tag;
                else
                    return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequestInsert()
        {
            this.InsertMode = true;

            this.SetControlsEnabled();

            this.ClearAll();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadData()
        {
            this.ClearAll();
            
            this.listView1.Items.Clear();
            foreach (Rubrica.ClientProxy.RubricaServices.ElementoRubrica item in this.Proxy.Search(null))
                this.listView1.Items.Add(this.CreateItem(item));
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveCurrent()
        {
            Rubrica.ClientProxy.RubricaServices.ElementoRubrica elemento = null;

            if (this.InsertMode)
            {
                elemento = this.Proxy.Insert(this.CreateNew());

                this.InsertMode = false;
            }
            else
            {
                elemento = this.Proxy.Update(this.GetElementoRubrica());
            }

            this.LoadData();
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteCurrent()
        {
            if (this.Current != null)
            {
                this.Proxy.Delete(((Rubrica.ClientProxy.RubricaServices.ElementoRubrica)this.Current).Id);

                this.LoadData();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
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
        private ListViewItem CreateItem(Rubrica.ClientProxy.RubricaServices.ElementoRubrica elemento)
        {
            ListViewItem item = new ListViewItem();
            item.Tag = elemento;
            item.Text = elemento.Id.ToString();
            item.SubItems.Add(elemento.Codice);
            item.SubItems.Add(elemento.Descrizione);
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Rubrica.ClientProxy.RubricaServices.ElementoRubrica CreateNew()
        {   
            return new Rubrica.ClientProxy.RubricaServices.ElementoRubrica
            {
                Codice = this.txtCodice.Text,
                Descrizione = this.txtDescrizione.Text, 
                Indirizzo = this.txtIndirizzo.Text,
                Telefono = this.txtTelefono.Text,
                Fax = this.txtFax.Text,
                Email = this.txtEmail.Text,
                TipoCorrispondente = (ClientProxy.RubricaServices.Tipo)this.cmdTipoCorrispondente.SelectedValue,
                Url = this.txtUrl.Text
                
            };
        }

        /// <summary>
        /// 
        /// </summary>
        private void Bind(Rubrica.ClientProxy.RubricaServices.ElementoRubrica elemento)
        {
            this.txtId.Text = elemento.Id.ToString();
            this.txtCodice.Text = elemento.Codice;
            this.txtDescrizione.Text = elemento.Descrizione;
            this.txtIndirizzo.Text = elemento.Indirizzo;
            this.txtTelefono.Text = elemento.Telefono;
            this.txtFax.Text = elemento.Fax;
            this.txtEmail.Text = elemento.Email;
            this.txtDataCreazione.Text = elemento.DataCreazione.ToString();
            this.txtDataUltimaModifica.Text = elemento.DataUltimaModifica.ToString();
            this.txtUtenteCreatore.Text = elemento.UtenteCreatore;
            this.txtUrl.Text = elemento.Url;
            this.cmdTipoCorrispondente.SelectedValue = elemento.TipoCorrispondente.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearAll()
        {
            this.txtId.Text = string.Empty;
            this.txtCodice.Text = string.Empty;
            this.txtDescrizione.Text = string.Empty;
            this.txtIndirizzo.Text = string.Empty;
            this.txtTelefono.Text = string.Empty;
            this.txtFax.Text = string.Empty;
            this.txtEmail.Text = string.Empty;
            this.txtCodiceAoo.Text = string.Empty;
            this.txtDataCreazione.Text = string.Empty;
            this.txtDataUltimaModifica.Text = string.Empty;
            this.txtUtenteCreatore.Text = string.Empty;
            this.cmdTipoCorrispondente.SelectedValue = 0;
            this.txtUrl.Text = String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Current != null)
            {
                this.SetControlsEnabled();

                this.Bind((Rubrica.ClientProxy.RubricaServices.ElementoRubrica)this.Current);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Rubrica.ClientProxy.RubricaServices.ElementoRubrica GetElementoRubrica()
        {
            Rubrica.ClientProxy.RubricaServices.ElementoRubrica elemento = new Rubrica.ClientProxy.RubricaServices.ElementoRubrica();

            elemento.Id = Convert.ToInt32(this.txtId.Text);
            elemento.Codice = this.txtCodice.Text;
            elemento.Descrizione = this.txtDescrizione.Text;
            elemento.Indirizzo = this.txtIndirizzo.Text;
            elemento.Telefono = this.txtTelefono.Text;
            elemento.Fax = this.txtFax.Text;
            elemento.Email = this.txtEmail.Text;
            elemento.DataCreazione = Convert.ToDateTime(this.txtDataCreazione.Text);
            elemento.DataUltimaModifica = Convert.ToDateTime(this.txtDataUltimaModifica.Text);
            elemento.TipoCorrispondente = (Rubrica.ClientProxy.RubricaServices.Tipo)this.cmdTipoCorrispondente.SelectedValue;
            elemento.Url = this.txtUrl.Text;

            return elemento;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetControlsEnabled()
        {
            this.txtCodice.Enabled = this.InsertMode;
        }
    }
}