// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Deleghe
{
    public class DelegheManager
    {
        public static ArrayList getListaDeleghe(DocsPaVO.utente.InfoUtente infoUtente, string tipoDelega, string statoDelega, string idAmm, out int numDeleghe)
        {
            ArrayList deleghe = new ArrayList();
            using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
            {

                DocsPaDB.Query_DocsPAWS.Deleghe d = new DocsPaDB.Query_DocsPAWS.Deleghe();

                //Recupero la lista delle deleghe assegnate da un dato utente
                deleghe = d.getListaDeleghe(infoUtente, tipoDelega, statoDelega, idAmm, out numDeleghe);

            }
            return deleghe;
        }

        public static bool verificaUnicaDelegaAmm(DocsPaVO.Deleghe.InfoDelega infoDelega)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = delega.verificaUnicaDelegaAmm(infoDelega);
            return result;
        }

        public static bool creaNuovaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega infoDelega)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = delega.creaNuovaDelega(infoDelega);
            return result;
        }

        public static bool revocaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega[] listaDeleghe, out string msg)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Deleghe deleghe = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = deleghe.revocaDelega(infoUtente, listaDeleghe, out msg);
            return result;
        }

        public static bool modificaDelegaAmm(DocsPaVO.Deleghe.InfoDelega delegaOld, DocsPaVO.Deleghe.InfoDelega delegaNew, string tipoDelega, string dataScadenzaOld, string dataDecorrenzaOld)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = delega.modificaDelegaAmm(delegaOld, delegaNew, tipoDelega, dataScadenzaOld, dataDecorrenzaOld);
            return result;
        }
    }
}
