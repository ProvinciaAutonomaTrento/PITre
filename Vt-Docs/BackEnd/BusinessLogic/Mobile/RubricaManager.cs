using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Mobile;
using DocsPaVO.utente;
using DocsPaDB.Query_DocsPAWS.Mobile;
using DocsPaVO.Mobile.Responses;
using BusinessLogic.Rubrica;
using log4net;

namespace BusinessLogic.Mobile
{
    public class RubricaManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(RubricaManager));

        public static List<LightUserInfo> GetListaUtentiInterni(string descrizione, InfoUtente infoUtente, int numMaxResults, string idRegistro)
        {
            RubricaMobile rubrica = new RubricaMobile();
            return rubrica.GetListaUtentiInterni(descrizione, infoUtente, numMaxResults,idRegistro);
        }

        public static List<RicercaSmistamentoElement> GetListaElementiInterni(string descrizione, InfoUtente infoUtente, int numMaxResults, int numMaxResultsForCategory, string idRegistro)
        {
            RubricaMobile rubrica = new RubricaMobile();
            return rubrica.GetListaElementiInterni(descrizione.ToUpper(), infoUtente, numMaxResults, numMaxResultsForCategory, idRegistro);
        }

        public static RicercaSmistamentoElement AggiungiElementoRicerca(string idElemento, string tipo, InfoUtente infoUtente, string idRegistro)
        {

            //List<RicercaSmistamentoElement> elements = new List<RicercaSmistamentoElement>();

            //string tipo = idElemento.Split('§')[0];
            //string id = idElemento.Split('§')[1];

            try
            {

                RicercaSmistamentoElement el = new RicercaSmistamentoElement();

                // RUOLO
                if (tipo.Equals("R"))
                {
                    string idGruppo = BusinessLogic.Utenti.UserManager.getRuoloById(idElemento).idGruppo;
                    DocsPaVO.utente.Ruolo role = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(idGruppo);

                    el.IdUO = role.uo.systemId;

                    el.IdRuolo = idElemento;
                    el.DescrizioneRuolo = role.descrizione;

                    el.Type = SmistamentoNodeType.RUOLO;
                }
                // UTENTE
                else
                {
                    DocsPaVO.utente.Utente ut = BusinessLogic.Utenti.UserManager.getUtenteById(idElemento);
                    ut.ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(idElemento);
                    Ruolo role = ((Ruolo[])ut.ruoli.ToArray(typeof(Ruolo))).FirstOrDefault();

                    el.IdUO = role.uo.systemId;

                    el.IdRuolo = role.systemId;
                    //el.DescrizioneRuolo = role.descrizione;

                    el.IdUtente = ut.idPeople;
                    el.DescrizioneUtente = ut.cognome + " " + ut.nome;

                    el.Type = SmistamentoNodeType.UTENTE;
                }

                return el;
            }
            catch (Exception ex)
            {
                logger.Debug("Elemento non trovato - tipo=" + tipo + ", id=" + idElemento);
                return null;
            }
        }

        /// <summary>
        /// MEV SMISTAMENTO
        /// Metodo per l'esecuzione della ricerca da mobile
        /// E' seguita la stessa logica della versione desktop
        /// </summary>
        /// <param name="descrizione"></param>
        /// <param name="infoUtente"></param>
        /// <param name="idRegistro"></param>
        /// <param name="idRuolo"></param>
        /// <param name="ragione"></param>
        /// <param name="numMaxResults"></param>
        /// <returns></returns>
        public static List<RicercaSmistamentoElement> GetListaCorrispondentiVeloce(string descrizione, InfoUtente infoUtente, string idRegistro, string idRuolo, string ragione, int numMaxResults)
        {
            
          
            DocsPaVO.rubrica.ParametriRicercaRubrica qco = new DocsPaVO.rubrica.ParametriRicercaRubrica();

            List<RicercaSmistamentoElement> elements = new List<RicercaSmistamentoElement>();
           
            qco.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
            qco.parent = "";
            qco.caller.IdRuolo = idRuolo;
            qco.caller.IdRegistro = idRegistro;
            qco.descrizione = descrizione;

            #region RAGIONE DI TRASMISSIONE
            
            DocsPaVO.amministrazione.InfoAmministrazione amm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
            string idRagione = string.Empty;
            switch (ragione)
            {
                case "comp":
                    idRagione = amm.IDRagioneCompetenza;
                    break;

                case "con":
                    idRagione = amm.IDRagioneConoscenza;
                    break;
            }
            DocsPaVO.trasmissione.RagioneTrasmissione ragTrasm = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(idRagione);
            switch (ragTrasm.tipoDestinatario.ToString("g").Substring(0, 1))
            {
                case "T":
                    qco.calltype = DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL;
                    break;

                case "I":
                    qco.calltype = DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF;
                    break;

                case "S":
                    qco.calltype = DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP;
                    break;

                case "P":
                    qco.calltype = DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO;
                    break;
            }
            #endregion

            bool abilitazioneRubricaComune = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(infoUtente).GestioneAbilitata;
            Registro[] regTemp = (Registro[])BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(idRuolo, "", "").ToArray(typeof(Registro));

            //Prendo soltanto i corrispondenti del mio registro e di tutti i miei rf se presenti
            #region registro
            Registro[] regOnliyTemp = null;
            if (regTemp != null && regTemp.Length > 0)
            {
                int countReg = 0;
                regOnliyTemp = new Registro[regTemp.Length];
                for (int y = 0; y < regTemp.Length; y++)
                {
                    if ((!string.IsNullOrEmpty(regTemp[y].chaRF) && regTemp[y].chaRF.Equals("1")) || regTemp[y].systemId.Equals(qco.caller.IdRegistro))
                    {
                        regOnliyTemp[countReg] = regTemp[y];
                        countReg++;
                    }
                }
            }
            string retValue = string.Empty;

            foreach (Registro item in regOnliyTemp)
            {
                if (item != null)
                {
                    retValue += " " + item.systemId + ",";
                }
            }
            if (retValue.EndsWith(","))
            {
                retValue = retValue.Remove(retValue.LastIndexOf(","));
            }

            qco.caller.filtroRegistroPerRicerca = retValue;
            #endregion

            qco.tipoIE = DocsPaVO.addressbook.TipoUtente.INTERNO;
            qco.doRuoli = true;
            qco.doUo = true;
            qco.doUtenti = true;
            qco.doListe = true;
            qco.doRF = false;
            qco.doRubricaComune = false;

            qco.ObjectType = string.Empty;

            string[] res = GetElementiRubricaVeloce(infoUtente, qco);
            int i = 0;

            //filtro i risultati cone nel frontend
   
            //response
            if (res != null && res.Count() > 0)
            {
                foreach (string s in res)
                {
                    RicercaSmistamentoElement el = new RicercaSmistamentoElement();
                    string desc = s.Split('§')[0];
                    string sysId = s.Split('§')[1];
                    string tipo = s.Split('§')[2];
                    string idPeople = s.Split('§')[3];

                    // UTENTE
                    if (tipo.Equals("P"))
                    {
                        el = AggiungiElementoRicerca(idPeople, tipo, infoUtente, idRegistro);

                        if (el != null)
                        {
                            el.DescrizioneUtente = desc;
                            //el.IdUtente = sysId;
                            el.Type = SmistamentoNodeType.UTENTE;
                        }
                    }
                    // RUOLO
                    else if (tipo.Equals("R"))
                    {
                        el = AggiungiElementoRicerca(sysId, tipo, infoUtente, idRegistro);

                        if (el != null)
                        {
                            el.DescrizioneRuolo = desc;
                            //el.IdRuolo = sysId;
                            el.Type = SmistamentoNodeType.RUOLO;
                        }
                    }
                    // UO
                    else
                    {
                        if (el != null)
                        {
                            el.DescrizioneUO = desc;
                            el.IdUO = sysId;
                            el.Type = SmistamentoNodeType.UO;
                        }
                    }
                    if (i < numMaxResults)
                    {
                        if (el != null)
                        {
                            elements.Add(el);
                            i++;
                        }
                    }
                    else
                        break;
                }
            }

            return elements;
        }

        private static string[] GetElementiRubricaVeloce(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.rubrica.ParametriRicercaRubrica qco)
        {

            string[] listaTemp = null;
            DocsPaDB.Query_DocsPAWS.Rubrica query = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
            ArrayList ers = query.GetElementiRubrica(qco);

            Mobile_RubricaSearchAgent SearchFilter = new Mobile_RubricaSearchAgent(infoUtente);

            if (SearchFilter != null)
            {
                SearchFilter.filtra_trasmissioni(qco, ref ers);
            }

            string tempStringElemento = null;
            if (ers.Count > 0 && ers != null)
            {
                listaTemp = new string[ers.Count];
            }

            for (int i = 0; i < ers.Count; i++)
            {
                DocsPaVO.rubrica.ElementoRubrica tempElement = (DocsPaVO.rubrica.ElementoRubrica)ers[i];

                //tempStringElemento = tempElement.descrizione + " (" + tempElement.codice + ")" + codRegTemp;
                tempStringElemento = tempElement.descrizione + "§" + tempElement.systemId + "§" + tempElement.tipo + "§" + tempElement.idPeople;
                listaTemp[i] = tempStringElemento;
            }

            return listaTemp;

        }

    }


    /// <summary>
    /// Classe per l'implementazione delle ricerche in rubrica per il mobile
    /// </summary>
    public class Mobile_RubricaSearchAgent {

        private DocsPaVO.utente.InfoUtente _user;

        private static Hashtable h_utenti = null;
        private static Hashtable h_ruoli = null;
        private static Hashtable h_registri = null;
        private static Hashtable h_uo = null;

        void init_ht()
        {
            if (h_utenti == null)
                h_utenti = BusinessLogic.Amministrazione.UtenteManager.GetRuoliUtenteSemplice(_user.idAmministrazione);
            if (h_ruoli == null)
                h_ruoli = BusinessLogic.Amministrazione.UOManager.GetRuoliUOSemplice(_user.idAmministrazione);
            if (h_registri == null)
                h_registri = BusinessLogic.Utenti.RegistriManager.GetRegistriByRuolo(_user.idAmministrazione);
            if (h_uo == null)
                h_uo = BusinessLogic.Amministrazione.UOManager.GetUORuoloSemplice(_user.idAmministrazione);
        }

        //Metodo per forzare il caricamento della hashtable
        public static void resetHashTable()
        {
            h_utenti = null;
            h_ruoli = null;
            h_registri = null;
            h_uo = null;
        }

        public Mobile_RubricaSearchAgent(DocsPaVO.utente.InfoUtente user)
        {
            _user = user;
            init_ht();
        }

        public void filtra_trasmissioni(DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers)
        {
            ArrayList a = new ArrayList();
            ArrayList ruoli_autorizzati = new ArrayList();

            DocsPaVO.trasmissione.TipoOggetto tipo_oggetto;
            tipo_oggetto = (qr.ObjectType.StartsWith("F:")) ? DocsPaVO.trasmissione.TipoOggetto.FASCICOLO : DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
            string id_nodo_titolario = (tipo_oggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO) ? qr.ObjectType.Substring(2) : null;

            DocsPaVO.utente.Ruolo r = BusinessLogic.Utenti.UserManager.getRuolo(qr.caller.IdRuolo);
            DocsPaDB.Utils.Gerarchia g = new DocsPaDB.Utils.Gerarchia();

            switch (qr.calltype)
            {
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL:
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL:
                    ruoli_autorizzati = g.getRuoliAut(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                    break;
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP:
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP:
                    ruoli_autorizzati = g.getGerarchiaSup(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                    break;
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF:
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF:
                    ruoli_autorizzati = g.getGerarchiaInf(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                    break;
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO:
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO:
                    ruoli_autorizzati = g.getGerarchiaPariLiv(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                    break;
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST:
                    DocsPaVO.trasmissione.RagioneTrasmissione rTo = BusinessLogic.Trasmissioni.RagioniManager.GetRagione("TO", _user.idAmministrazione);
                    DocsPaVO.trasmissione.TipoGerarchia gTo = rTo.tipoDestinatario;

                    switch (gTo)
                    {
                        case DocsPaVO.trasmissione.TipoGerarchia.INFERIORE:
                            ruoli_autorizzati = g.getGerarchiaInf(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                            break;

                        case DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO:
                            ruoli_autorizzati = g.getGerarchiaPariLiv(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                            break;

                        case DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE:
                            ruoli_autorizzati = g.getGerarchiaSup(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                            break;

                        case DocsPaVO.trasmissione.TipoGerarchia.TUTTI:
                            ruoli_autorizzati = g.getRuoliAut(r, qr.caller.IdRegistro, null, tipo_oggetto);
                            break;
                    }
                    break;

                default:
                    return;
            }
            string[] c_ruoli_aut = new string[ruoli_autorizzati.Count];
            for (int i = 0; i < ruoli_autorizzati.Count; i++)
                c_ruoli_aut[i] = ((DocsPaVO.utente.Ruolo)ruoli_autorizzati[i]).codiceRubrica;

            Array.Sort(c_ruoli_aut);

            foreach (DocsPaVO.rubrica.ElementoRubrica er in ers)
            {
                try
                {
                    switch (er.tipo)
                    {
                        case "U":
                            if (uo_is_autorizzata((er.interno ? "I" : "E") + @"\" + er.codice, r, c_ruoli_aut) || (qr.ObjectType == "G"))
                                a.Add(er);
                            break;

                        case "R":
                            if (ruolo_is_autorizzato(er.codice, c_ruoli_aut) || (qr.ObjectType == "G"))
                                a.Add(er);
                            break;

                        case "P":
                            if (utente_is_autorizzato((er.interno ? "I" : "E") + @"\" + er.codice, c_ruoli_aut) || (qr.ObjectType == "G"))
                                a.Add(er);
                            break;
                        case "L":
                        case "F":
                            //if (utente_is_autorizzato ((er.interno ? "I" : "E") + @"\" + er.codice, c_ruoli_aut) || (qr.ObjectType == "G"))
                            a.Add(er);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            ers = a;
        }

        private bool ruolo_is_autorizzato(string codice, string[] ruoli_autorizzati)
        {
            return (Array.BinarySearch(ruoli_autorizzati, codice) >= 0);
        }

        private bool utente_is_autorizzato(string _codice, string[] ruoli_autorizzati)
        {
            string codice = _codice;

            if (_codice.StartsWith(@"E\") || _codice.StartsWith(@"I\"))
                codice = _codice.Substring(_codice.IndexOf(@"\") + 1);

            //			if (h_utenti == null)
            //				h_utenti = UtenteManager.GetRuoliUtenteSemplice (_user.idAmministrazione);

            string[] ruoli = (string[])h_utenti[codice];
            if (ruoli != null && ruoli.Length > 0)
                foreach (string cod_ruolo in ruoli)
                    if (ruolo_is_autorizzato(cod_ruolo, ruoli_autorizzati))
                        return true;

            return false;
        }

        private bool uo_is_autorizzata(string _codice, DocsPaVO.utente.Ruolo ruolo_caller, string[] ruoli_autorizzati)
        {
            string codice = _codice;

            if (_codice.StartsWith(@"E\") || _codice.StartsWith(@"I\"))
                codice = _codice.Substring(_codice.IndexOf(@"\") + 1);

            //			if (h_ruoli == null)
            //				h_ruoli = UOManager.GetRuoliUOSemplice (_user.idAmministrazione);

            string[] ruoli = (string[])h_ruoli[codice];
            if (ruoli != null && ruoli.Length > 0)
                foreach (string cod_ruolo in ruoli)
                    if (ruolo_is_autorizzato(cod_ruolo, ruoli_autorizzati))
                        return true;

            return false;
        }
    }
}
