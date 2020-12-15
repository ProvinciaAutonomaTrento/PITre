using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Deleghe;
using DocsPaDB.Query_DocsPAWS;
using BusinessLogic.Utenti;
using System.Collections;
using DocsPaVO.utente;
using DocsPaVO.addressbook;
using DocsPaVO.ricerche;

namespace BusinessLogic.Deleghe
{
    public class ModelliDelegaManager
    {
        public static List<ModelloDelega> searchModelliDelega(DocsPaVO.utente.InfoUtente utente, SearchModelloDelegaInfo searchInfo,SearchPagingContext pagingContext)
        {
            List<Ruolo> ruoli = UserManager.getRuoliUtente(utente.idPeople).Cast<Ruolo>().ToList();
            ModDeleghe modDeleghe = new ModDeleghe();
            if (!searchInfo.StatoModelloDelegaSpec)
            {
                return modDeleghe.searchModelliDelegaPaging(utente, ruoli, searchInfo, pagingContext);
            }
            else
            {
                StatoModelloDelegaMatcher matcher = new StatoModelloDelegaMatcher(searchInfo.StatoModelloDelega);
                List<ModelloDelega> temp = modDeleghe.searchModelliDelega(utente, ruoli, searchInfo).FindAll(matcher.Match);
                pagingContext.RecordCount = temp.Count;
                List<ModelloDelega> result = new List<ModelloDelega>(temp.Skip(pagingContext.StartRow-1).Take(pagingContext.PageSize));
                return result;
            }
        }

        public static List<ModelloDelega> searchModelliDelega(DocsPaVO.utente.InfoUtente utente, SearchModelloDelegaInfo searchInfo)
        {
            List<Ruolo> ruoli = UserManager.getRuoliUtente(utente.idPeople).Cast<Ruolo>().ToList();
            ModDeleghe modDeleghe = new ModDeleghe();
            return modDeleghe.searchModelliDelega(utente, ruoli, searchInfo);
        }

        public static ModelloDelega getModelloDelegaById(string idModello)
        {
            ModDeleghe modDeleghe = new ModDeleghe();
            return modDeleghe.getModelloDelegaById(idModello);
        }

        public static List<ModelloDelega> getModelliDelegaByStato(DocsPaVO.utente.InfoUtente utente, StatoModelloDelega stato)
        {
            List<ModelloDelega> res = new List<ModelloDelega>();
            ModDeleghe modDeleghe = new ModDeleghe();
            List<ModelloDelega> tempList = searchModelliDelega(utente, null);
            StatoModelloDelegaMatcher matcher = new StatoModelloDelegaMatcher(stato);
            return tempList.FindAll(matcher.Match);
        }

        public static bool insertModelloDelega(ModelloDelega modelloDelega)
        {
            ModDeleghe modDeleghe = new ModDeleghe();
            return modDeleghe.insertModelloDelega(modelloDelega);
        }

        public static bool updateModelloDelega(ModelloDelega modelloDelega)
        {
            ModDeleghe modDeleghe = new ModDeleghe();
            return modDeleghe.updateModelloDelega(modelloDelega);
        }

        public static bool deleteModelliDelega(List<string> idModelli)
        {
            ModDeleghe modDeleghe = new ModDeleghe();
            return modDeleghe.deleteModelliDelega(idModelli);
        }

        public static InfoDelega buildDelegaFromModello(DocsPaVO.utente.InfoUtente utente, string idModello,DateTime overrideDataInizio,DateTime overrideDataFine)
        {
            ModDeleghe modDeleghe = new ModDeleghe();
            ModelloDelega md = modDeleghe.getModelloDelegaById(idModello);
            InfoDelega delega = new InfoDelega();
            if (string.IsNullOrEmpty(md.IdRuoloDelegante))
            {
                delega.id_ruolo_delegante = "0";
                delega.cod_ruolo_delegante = "TUTTI";
            }
            else
            {
                delega.id_ruolo_delegante = md.IdRuoloDelegante;
                delega.cod_ruolo_delegante = md.DescrRuoloDelegante;
            }
            delega.id_utente_delegante = utente.idPeople;
            Corrispondente delegante = UserManager.getCorrispondenteByIdPeople(utente.idPeople, TipoUtente.INTERNO, utente);
            
            delega.cod_utente_delegante = delegante.codiceRubrica;
            delega.id_ruolo_delegato = md.IdRuoloDelegato;

            Corrispondente delegato = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(md.IdUtenteDelegato);
            delega.cod_utente_delegato = delegato.codiceRubrica;
            md.IdUtenteDelegato = ((DocsPaVO.utente.Utente)(delegato)).idPeople;

            ArrayList ruoliDelegato = BusinessLogic.Utenti.UserManager.getRuoliUtente(md.IdUtenteDelegato);
            ruoliDelegato.Sort(new RuoliComparer());
            delega.id_uo_delegato = ((Ruolo)ruoliDelegato[0]).uo.systemId;
            

            Ruolo ruoloDelegato = UserManager.getRuoloById(md.IdRuoloDelegato);
            delega.cod_ruolo_delegato = ruoloDelegato.codice;
            delega.id_utente_delegato = md.IdUtenteDelegato;
            delega.cod_utente_delegato = null;
            if (overrideDataInizio.CompareTo(DateTime.MinValue) > 0)
            {
                delega.dataDecorrenza = buildDateString(overrideDataInizio);
            }
            else
            {
                delega.dataDecorrenza = buildDateString(md.DataInizioDelega);
            }
            if (overrideDataFine.CompareTo(DateTime.MinValue) > 0)
            {
                delega.dataScadenza = buildDateString(overrideDataFine);
            }
            else
            {
                delega.dataScadenza = buildDateString(md.DataFineDelega);
            }
            return delega;
        }

        private static string buildDateString(DateTime data)
        {
            string ora = ""+data.Hour;
            if (ora.Length == 1) ora = "0" + ora;
            return data.ToShortDateString() + " " + ora + ".00.00";
        }

        private bool filter(ModelloDelega modello,StatoModelloDelega stato){
            return (modello.Stato == stato);
        }

        private class StatoModelloDelegaMatcher
        {
            private StatoModelloDelega _stato;

            public StatoModelloDelegaMatcher(StatoModelloDelega stato)
            {
                this._stato = stato;
            }

            public bool Match(ModelloDelega modelloDelega)
            {
                return modelloDelega.Stato == _stato;
            }
        }
        public class RuoliComparer : IComparer
        {

            public int Compare(object x, object y)
            {
                int xId = Int32.Parse(((Ruolo)x).systemId);
                int yId = Int32.Parse(((Ruolo)y).systemId);
                return xId.CompareTo(yId);
            }
        }
    }

}
