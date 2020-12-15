using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BusinessLogic.Amministrazione
{
    public class StruttureSottofascicoliManager
    {
        public static DataTable getStruttureSottofascicoli(string idamm)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().getStruttureSottofascicoli(idamm);
        }

        public static DataTable getStruttureSottofascicoli(string idamm, int idfascicolo, int idtitolario, int idtemplate)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().getStruttureSottofascicoli(idamm, idfascicolo, idtitolario, idtemplate);
        }

        public static void saveStrutturaSottofascicoli(int id, DataTable data, string name, string idAmm)
        {
            new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().saveStrutturaSottofascicoli(id, data, name, idAmm);
        }

        public static void DeleteStrutturaSottofascicoli(int id, string idAmm)
        {
            new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().deleteStrutturaSottofascicoli(id, idAmm);
        }

        public static DataTable getTemplateStrutturaRelation(int idtemplate, int idtipofascicolo, int idtitolario)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().getTemplateStrutturaRelation(idtemplate, idtipofascicolo, idtitolario);
        }

        public static bool saveStrutturaSottofascicoli(int idtipofascicolo, int idtitolario, int idtemplate)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().saveStrutturaSottofascicoli(idtemplate, idtipofascicolo, idtitolario);
        }

        public static bool IsNodoStrutturaInFascicoliEmpty(string strutturaid, string nodename, out int numfascicoli, string idamm)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().IsNodoStrutturaInFascicoliEmpty(strutturaid, nodename, out numfascicoli, idamm);
        }

        public static DocsPaVO.fascicolazione.Folder[] GetFoldersByStrutturaTemplate(string name, string id_template)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().GetFoldersByStrutturaTemplate(name, id_template, null);
        }

        public static DocsPaVO.fascicolazione.Folder[] GetFoldersByStrutturaTemplate(string name, string id_template, string idamm)
        {
            return new DocsPaDB.Query_DocsPAWS.StruttureSottoFascicoli().GetFoldersByStrutturaTemplate(name, id_template, idamm);
        }
    }
}
