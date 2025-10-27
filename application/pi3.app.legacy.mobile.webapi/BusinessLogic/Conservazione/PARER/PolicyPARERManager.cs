// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Conservazione.PARER;
using DocsPaVO.utente;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Conservazione.PARER
{
    public class PolicyPARERManager
    {
        public static ArrayList getListaPolicy(string idAmm, string tipo)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetListaPolicyPARER(idAmm, tipo);
        }

        public static bool InsertNewPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.InsertNewPolicyPARER(policy);
        }

        public static bool UpdatePolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.UpdatePolicyPARER(policy);
        }

        public static string GetCountDocumentiFromPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetCountDocumentiFromPolicy(policy, idAmm);
        }

        public static DocsPaVO.Conservazione.PARER.PolicyPARER GetPolicyById(string idPolicy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetPolicyPARERById(idPolicy);
        }

        public static DocsPaVO.Conservazione.PARER.EsecuzionePolicy GetInfoEsecuzionePolicy(string idPolicy, string tipo)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetInfoEsecuzionePolicy(idPolicy, tipo);
        }

        public static bool DeletePolicy(string idPolicy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.DeletePolicyPARER(idPolicy);
        }

        public static bool UpdateStatoPolicy(ArrayList lista, InfoUtente utente)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.UpdateStatoPolicy(lista, utente);
        }

    }
}
