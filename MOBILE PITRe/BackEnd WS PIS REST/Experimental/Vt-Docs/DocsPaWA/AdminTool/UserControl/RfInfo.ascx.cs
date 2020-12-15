using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.AdminTool.Manager;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;

namespace DocsPAWA.AdminTool.UserControl
{
    public partial class RfInfo : System.Web.UI.UserControl
    {
        
        #region Methods

        /// <summary>
        /// Metodo per la compilazione della proprietà di un elemento da inviare alla rubrica comune
        /// </summary>
        /// <param name="elementoRubrica">Elemento da compilare</param>
        internal void CompileRFProperty(global::RubricaComune.Proxy.Elementi.ElementoRubrica elementoRubrica)
        {
            SessionManager sm = new SessionManager();
            InfoUtenteAmministratore infoUtente = sm.getUserAmmSession();
            string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];

            elementoRubrica.Cap = this.txtCap.Text;
            elementoRubrica.Citta = this.txtCitta.Text;
            elementoRubrica.Fax = this.txtFax.Text;
            elementoRubrica.Indirizzo = this.txtIndirizzo.Text;
            elementoRubrica.Nazione = this.txtNazione.Text;
            elementoRubrica.Provincia = this.txtProvincia.Text;
            elementoRubrica.Telefono = this.txtTelefono.Text;
            elementoRubrica.CodiceFiscale = this.txtCodiceFiscale.Text;
            elementoRubrica.PartitaIva = this.txtPartitaIva.Text;

        }

        /// <summary>
        /// Metodo per la visualizzazione delle prorietà di un corrispondente presente in rubrica comune
        /// </summary>
        /// <param name="elementoRubrica">Elemento di cui visualizzare le proprietà</param>
        internal void ShowRfProperty(global::RubricaComune.Proxy.Elementi.ElementoRubrica elementoRubrica)
        {
            this.txtCap.Text = elementoRubrica.Cap;
            this.txtCitta.Text = elementoRubrica.Citta;
            this.txtFax.Text = elementoRubrica.Fax;
            this.txtIndirizzo.Text = elementoRubrica.Indirizzo;
            this.txtNazione.Text = elementoRubrica.Nazione;
            this.txtProvincia.Text = elementoRubrica.Provincia;
            this.txtTelefono.Text = elementoRubrica.Telefono;
            this.txtCodiceFiscale.Text = elementoRubrica.CodiceFiscale;
            this.txtPartitaIva.Text = elementoRubrica.PartitaIva;
            
        }

        #endregion

        internal void SaveRfDetails(global::RubricaComune.Proxy.Elementi.ElementoRubrica elementoRubrica, String idRf)
        {
            DocsPaWebService ws = new DocsPaWebService();
            ws.SaveElementoRubricaRF(new RaggruppamentoFunzionale() 
                { 
                    cap = elementoRubrica.Cap,
                    citta = elementoRubrica.Citta,
                    fax = elementoRubrica.Fax,
                    indirizzo = elementoRubrica.Indirizzo,
                    nazionalita = elementoRubrica.Nazione,
                    prov = elementoRubrica.Provincia,
                    telefono1 = elementoRubrica.Telefono,
                    codfisc = elementoRubrica.CodiceFiscale,
                    partitaiva = elementoRubrica.PartitaIva

                }, idRf);
        }
    }
}