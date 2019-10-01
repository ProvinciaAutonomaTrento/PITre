using System;
using NttDataWA.UIManager;
using MText;
using NttDataWA.DocsPaWR;
using NttDataWA.CheckInOut;

namespace NttDataWA.models.mtext
{
    public partial class Delete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Nel query string deve essere presente l'id del documento da cancellare
            if (!String.IsNullOrEmpty(Request["idDocument"]))
                this.ProcessDelete(Request["idDocument"]);
            else
                // Viene cancellato il documento salvato nel contesto di checkout
                this.ProcessDeleteFromCheckOutContext();
                
            // Redirezionamento alla pagina di UndoCheckout in modo da aggiornare lo stato del documento
            string checkinOutFolderPath = "/CheckInOut/";
            NttDataWA.DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.SmartClientConfigurations smcConf = ws.GetSmartClientConfigurationsPerUser(UserManager.GetInfoUser());
            string componentsType = smcConf.ComponentsType;
            if (componentsType == "3")
                checkinOutFolderPath = "/CheckInOutApplet/";

            Response.Redirect(Utils.utils.getHttpFullPath() + checkinOutFolderPath + "UndoCheckOutPage.aspx");

        }

        /// <summary>
        /// Funzione per la cancellazione di un documento M/Text dal server M/Text
        /// </summary>
        /// <param name="idDocument">Id del documento da cancellare</param>
        private void ProcessDelete(String idDocument)
        {
            try
            {
                // Eliminazione del documento da M/Text
                MTextModelProvider mTextProvider = ModelProviderFactory<MTextModelProvider>.GetInstance();
                mTextProvider.DeleteDocument(Utils.MTextUtils.Id2FullQualifiedName(idDocument));

            }
            catch (Exception e)
            {
                // Non viene intrapresa alcuna azione
            }
        }

        /// <summary>
        /// Funzione per l'eliminazione di un documento salvato in un contesto di checkout
        /// </summary>
        private void ProcessDeleteFromCheckOutContext()
        {
            if(CheckOutContext.Current != null && !String.IsNullOrEmpty(CheckOutContext.Current.Status.DocumentLocation))
            {
                MTextModelProvider mTextProvider = ModelProviderFactory<MTextModelProvider>.GetInstance();

                try
                {
                    mTextProvider.DeleteDocument(CheckOutContext.Current.Status.DocumentLocation.Substring(8));
                }
                catch (Exception e)
                { 
                    // Non viene intrapresa alcuna azione
                }
            }
        }


    }
}