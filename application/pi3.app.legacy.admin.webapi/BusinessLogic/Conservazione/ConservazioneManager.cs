// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Conservazione
{
    public class ConservazioneManager
    {
        public string GetIdRuoloRespConservazione(string idAmm, string idAOO)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione manager = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return manager.GetIdRoleResponsabileConservazione(idAmm, idAOO);
        }

        public string GetIdUtenteRespConservazione(string idAmm, string idAOO)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione manager = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return manager.GetIdUtenteResponsabileConservazione(idAmm, idAOO);
        }

        public bool GetStatoAttivazione(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.GetStatoAttivazione(idAmm);
        }

        public bool SetStatoAttivazione(string idAmm, string stato)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.SetStatoAttivazione(idAmm, stato);
        }

        public bool SetMailStruttura(string idAmm, DocsPaVO.Conservazione.PARER.Mailbox mailbox)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            DocsPaVO.Conservazione.PARER.ReportConfiguration reportConfig = cons.GetReportConfiguration(idAmm);

            if (reportConfig != null && !string.IsNullOrEmpty(reportConfig.idAmm))
            {
                result = cons.SaveMailStruttura(idAmm, mailbox, false);
            }
            else
            {
                result = cons.SaveMailStruttura(idAmm, mailbox, true);
            }

            return result;
        }

        public DocsPaVO.Conservazione.PARER.Mailbox GetMailStruttura(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.GetMailStruttura(idAmm);
        }

        public ArrayList getResponsabiliCons(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return cons.getResponsabiliCons(idAmm);
        }

    }
}
