using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P3SBCLib.DocsPaServices
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class DocsPaWorkflowServiceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private DocsPaWorkflowServiceHelper()
        { }

        /// <summary>
        /// Lo stato del fascicolo viene impostato al primo passo impostato dall'eventuale diagramma di stato associato
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="infoUtente"></param>
        public static void SetOnFirstWorkflowStep(
                        DocsPaVO.utente.InfoUtente infoUtente, 
                        DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            // Reperimento diagramma 
            DocsPaVO.DiagrammaStato.DiagrammaStato workflow = BusinessLogic.DiagrammiStato.DiagrammiStato.getDgByIdTipoFasc(fascicolo.template.SYSTEM_ID.ToString(), infoUtente.idAmministrazione);

            if (workflow != null && workflow.PASSI != null && workflow.PASSI.Count > 0)
            {
                DocsPaVO.DiagrammaStato.Passo firstStep = (DocsPaVO.DiagrammaStato.Passo)workflow.PASSI[0];

                string idStato = firstStep.STATO_PADRE.SYSTEM_ID.ToString();

                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(fascicolo.systemID,
                                idStato,
                                workflow,
                                infoUtente.userId,
                                infoUtente,
                                fascicolo.template.SCADENZA);

                // Esecuzione delle eventuali trasmissioni associate al primo stato del fascicolo
                DocsPaTxServiceHelper.ExecuteStateTx(infoUtente, fascicolo, idStato);
            }
        }
    }
}
