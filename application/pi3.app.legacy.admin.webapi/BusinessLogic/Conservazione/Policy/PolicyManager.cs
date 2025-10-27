// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Conservazione.Policy
{
    public class PolicyManager
    {
        public static DocsPaVO.Conservazione.Policy GetPolicyById(string idPolicy)
        {
            DocsPaVO.Conservazione.Policy result = null;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.GetPolicyById(idPolicy);

            return result;
        }

        public static bool InsertNewPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.InsertNewPolicy(policy);
            return result;
        }

        public static bool SvuotaCachePolicy(string idAmm, string tipo)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.SvuotaCachePolicy(idAmm, tipo);
            return result;
        }

        public static bool ModifyNewPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.ModifyNewPolicy(policy);
            return result;
        }

        public static bool SavePeriodPolicy(DocsPaVO.Conservazione.Policy policy)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.SavePeriodPolicy(policy);
            return result;
        }

        public static DocsPaVO.Conservazione.Policy[] GetListaPolicy(int idAmm, string tipo)
        {
            DocsPaVO.Conservazione.Policy[] result = null;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.GetListaPolicy(idAmm, tipo);

            return result;
        }

        public static bool DeletePolicy(string idPolicy)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.PolicyConservazione conservazione = new DocsPaDB.Query_DocsPAWS.PolicyConservazione();
            result = conservazione.DeletePolicy(idPolicy);
            return result;
        }

    }
}
