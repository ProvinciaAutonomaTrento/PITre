using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DocsPaConservazione;

namespace DocsPaWS.Conservazione.Policy
{
    /// <summary>
    /// Servizi web focalizzati all'esecuzione delle policy di conservazione
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/Conservazione/Policy/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class PolicyExecutionServices : System.Web.Services.WebService
    {
        /// <summary>
        /// Esecuzione di tutte le policy di conservazione 
        /// di tutte le amministrazioni dell'istanza attivate nella loro esecuzione temporale
        /// </summary>
        /// <returns></returns>
        [WebMethod()]
        public void ExecutePolicy()
        {
            #region oldCode
            //BusinessLogic.Conservazione.Policy.PolicyManager.ExecutePolicy();
            #endregion

            #region MEV CS 1.5 - F03_01
            BusinessLogic.Conservazione.Policy.PolicyManager.ExecutePolicy_WithConstraint(HttpContext.Current);
            #endregion
        }

        /// <summary>
        /// Esecuzione dei test automatici sulle istanze da mettere in lavorazione
        /// </summary>
        [WebMethod()]
        public void ExecuteAutoTests()
        {
            new DocsPaConservazione.DocsPaConsManager().executeAutoTests();
        }

        [WebMethod()]
        public bool TestExecutePolicy()
        {
            #region oldcode
            //BusinessLogic.Conservazione.Policy.PolicyManager.ExecutePolicy();
            #endregion

            #region MEV CS 1.5 - F03_01
            BusinessLogic.Conservazione.Policy.PolicyManager.ExecutePolicy_WithConstraint(HttpContext.Current);
            #endregion
            return true;
        }

        /// <summary>
        /// Metodo web per la stampa automatica del registro di conservazione.
        /// Da schedulare.
        /// </summary>
        [WebMethod()]
        public void GeneratePrintRegCons()
        {
            new BusinessLogic.Stampe.StampaConservazione().GeneratePrintRegCons();

        }

        /// <summary>
        /// Versamento in conservazione al PARER dei documenti in coda
        /// </summary>
        [WebMethod()]
        public void ExecuteVersamento()
        {
            new BusinessLogic.Conservazione.ConservazioneManager().ExecuteVersamento();
        }

        /// <summary>
        /// Versamento in conservazione dei big files
        /// </summary>
        [WebMethod]
        public void ExecuteVersamentoBigFiles()
        {
            new BusinessLogic.Conservazione.ConservazioneManager().ExecuteVersamentoBigFiles();
        }

        /// <summary>
        /// Esecuzione policy PARER configurate in amministrazione
        /// </summary>
        [WebMethod]
        public void ExecutePolicyPARER()
        {
            BusinessLogic.Conservazione.PARER.PolicyPARERManager.ExecutePolicy();
        }

        /// <summary>
        /// Mette in lavorazione le istanze di conservazione
        /// </summary>
        [WebMethod]
        public void PutInWorkingConservation()
        {
            DocsPaConsManager docsPaConsManager = new DocsPaConsManager();
            docsPaConsManager.PutInWorkingConservation();
        }

        /// <summary>
        /// Lavora le instaze in stato StatoIstanza.IN_LAVORAZIONE
        /// </summary>
        [WebMethod]
        public void WorksConservationInstances()
        {
            DocsPaConsManager docsPaConsManager = new DocsPaConsManager();
            docsPaConsManager.WorksConservationInstances();
        }
    }
}
