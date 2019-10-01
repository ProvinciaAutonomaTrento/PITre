using System;
using System.Collections.Generic;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using MText;
using MText.DomainObjects;

namespace NttDataWA.models.mtext
{
    public partial class ModelGenerator : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Reperimento del template in definizione
            Templates template = (Templates)Session["templateSelPerModelli"];

            // Generazione di un dizionario creato a partire dai campi del template
            List<LabelValuePair> sampleValues = Utils.MTextUtils.GetSampleLabelValueCollection(new List<OggettoCustom>(template.ELENCO_OGGETTI));
            MTextModelProvider provider = ModelProviderFactory<MTextModelProvider>.GetInstance();
            Byte[] dataSource = provider.GetXmlExample(template.DESCRIZIONE, sampleValues);

            this.Response.Clear();

            // Scrittura del content nella response
            this.WriteDataSource(dataSource);

        }

        /// <summary>
        /// Funzione per la scrittura, nella response, dell'XML con l'esempio di data source
        /// </summary>
        /// <param name="dataSource">Data source da scrivere nella response</param>
        private void WriteDataSource(Byte[] dataSource)
        {
            // Pulizia e creazione degli header
            Response.Buffer = true;
            Response.ClearHeaders();
            Response.ContentType = "text/xml";
            Response.AppendHeader("Content-Disposition", "attachment;filename=DataSourceSample.xml");
            Response.AddHeader("Content-Length", dataSource.Length.ToString());

            // Scrittura del contenuto dl file
            Response.BinaryWrite(dataSource);

            // Flushcing ed invio della response
            Response.Flush();
            Response.End();
    
        }
    }
}