// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Amministrazione
{
    public class StruttureSottofascicoliManager
    {
        public static bool saveStrutturaSottofascicoli(int idtipofascicolo, int idtitolario, int idtemplate)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().saveStrutturaSottofascicoli(idtemplate, idtipofascicolo, idtitolario);
        }

        public static DataTable getStruttureSottofascicoli(string idamm)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().getStruttureSottofascicoli(idamm);
        }

        public static DataTable getTemplateStrutturaRelation(int idtemplate, int idtipofascicolo, int idtitolario)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().getTemplateStrutturaRelation(idtemplate, idtipofascicolo, idtitolario);
        }

        public static void DeleteStrutturaSottofascicoli(int id, string idAmm)
        {
            new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().deleteStrutturaSottofascicoli(id, idAmm);
        }

        public static DataTable getStruttureSottofascicoli(string idamm, int idfascicolo, int idtitolario, int idtemplate)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().getStruttureSottofascicoli(idamm, idfascicolo, idtitolario, idtemplate);
        }

        public static void saveStrutturaSottofascicoli(int id, DataTable data, string name, string idAmm)
        {
            new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().saveStrutturaSottofascicoli(id, data, name, idAmm);
        }

        public static bool IsNodoStrutturaInFascicoliEmpty(string strutturaid, string nodename, out int numfascicoli, string idamm)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().IsNodoStrutturaInFascicoliEmpty(strutturaid, nodename, out numfascicoli, idamm);
        }


    }
}
