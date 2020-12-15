using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Manager
{
    public class SistemiEsterniManager
    {
        public static DocsPaWR.SistemaEsterno[] getSistemiEsterni(string idAmm)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.getSistemiEsterni(idAmm);

        }

        public static DocsPaWR.MetodoPIS[] getPISMethods()
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.getPISMethods();
        }

        public static bool modPIS(string metodi, string idSysExt)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.ModificaMetodiPermessiSistemaEsterno(metodi, idSysExt);
        }

        public static bool modDescTknPIS(string desc, string tknTime, string idSysExt)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.ModificaDescTknPerSysExt(desc, tknTime, idSysExt);
        }

        public static TipoRuolo getTipoRuoloByCode(string idAmm, string codice)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.getTipoRuoloByCode(idAmm, codice);
        }

        public static bool insSysExtAfterAssoc(string idAmm, string codUtente, string codRuolo, string descrizione)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.InsSysExtAfterAssoc(idAmm, codUtente, codRuolo, descrizione);
        }

        public static UnitaOrganizzativa getHubSistemiEsterni(string codice, string idAmm)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.GetHubSistemiEsterni(codice, idAmm);
        }

        public static string ctrlInserimentoSistemaEsterno(string idAmm, string codUtente, string codRuolo)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.ctrlInserimentoSistemaEsterno(idAmm, codUtente, codRuolo);
        }

        public static bool setVisibilityHubSysExt(string idHub)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.setVisibilityHubSysExt(idHub);
        }

        public static bool delExtSys(DocsPAWA.DocsPaWR.SistemaEsterno sysExt, DocsPAWA.DocsPaWR.InfoUtente infoUt)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.delExtSys(sysExt, infoUt);
        }
    }
}