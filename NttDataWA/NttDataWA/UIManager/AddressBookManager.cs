using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Collections;
using System.Data;

namespace NttDataWA.UIManager
{
    public class AddressBookManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static Corrispondente GetCorrespondentBySystemId(string systemID)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteBySystemId(systemID);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.ElementoRubrica[] getElementiRubricaMultipli(string codiceRubrica, DocsPaWR.RubricaCallType callType, bool codiceEsatto)
        {
            try
            {
                DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
                //cerco su tutti i tipi utente:
                qco.calltype = callType;
                setQueryRubricaCaller(ref qco);

                //  qco.caller.filtroRegistroPerRicerca = qco.caller.IdRegistro;

                qco.codice = codiceRubrica;
                qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;

                if (callType == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT || 
                    callType == DocsPaWR.RubricaCallType.CALLTYPE_OWNER_AUTHOR ||
                    callType == DocsPaWR.RubricaCallType.CALLTYPE_CORR_INT_NO_UO ||
                    callType == DocsPaWR.RubricaCallType.CALLTYPE_PROTO_INT_DEST
                    )
                {
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.INTERNO;
                }

                if (callType == DocsPaWR.RubricaCallType.CALLTYPE_CORR_EST)
                {
                    qco.tipoIE = DocsPaWR.AddressbookTipoUtente.ESTERNO;
                }


                //questo serve perchè in questi casi quando si cerca un esterno all'amministrazione
                //si deve ricercare anche tra gli esterni all'amministrazione che sono creati su degli RF
                //associati al registro corrente  (nel caso di protocollo)
                if (callType == RubricaCallType.CALLTYPE_PROTO_IN
                    || callType == RubricaCallType.CALLTYPE_PROTO_IN_INT
                    || callType == RubricaCallType.CALLTYPE_PROTO_OUT
                    || callType == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                    || callType == RubricaCallType.CALLTYPE_CORR_INT
                    || callType == RubricaCallType.CALLTYPE_CORR_EST
                    || callType == RubricaCallType.CALLTYPE_CORR_INT_EST
                    || callType == RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                    || callType == RubricaCallType.CALLTYPE_CORR_INT_CON_DISABILITATI)
                {
                    if (qco.caller.IdRegistro != null && qco.caller.IdRegistro != string.Empty)
                    {
                        //af 07032013
                        DocsPaWR.Registro[] listaReg;
                        if(!UIManager.AdministrationManager.IsEnableRF(UIManager.UserManager.GetUserInSession().idAmministrazione))
                            listaReg = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, string.Empty, qco.caller.IdRegistro);
                        else
                        listaReg = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, "1", qco.caller.IdRegistro);

                        //Ritorna una lista di RF concatenati da una ","
                        string condReg = getCondizioneRegistro(listaReg);

                        if (condReg != null && condReg != string.Empty)
                        {
                            //se cè almeno un RF allora aggancio anche l'id registro
                            // per ricercare tra tutti gli esterni appartenenti
                            //al mio registro e agli RF ad esso associati che posso vedere
                            condReg += ", " + qco.caller.IdRegistro;
                        }
                        else
                        {
                            condReg += qco.caller.IdRegistro;
                        }
                        qco.caller.filtroRegistroPerRicerca = condReg;
                    }
                }

                //in questo caso devo ricercare i corrispondenti esterni all'amministrazione
                //tra tutti i corrispondenti che sono stati creati su registi e rf a cui
                //il ruolo corrente è associato
                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                || callType == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE
                || qco.calltype == RubricaCallType.CALLTYPE_RICERCA_CORRISPONDENTE)
                {

                    //nuova gestione: devo cercare in tutti i registri e RF visibili al ruolo

                    DocsPaWR.Registro[] regRuolo = UIManager.RegistryManager.GetListRegistriesAndRF(qco.caller.IdRuolo, string.Empty, string.Empty);

                    string filtroRegistro = string.Empty;
                    for (int i = 0; i < regRuolo.Length; i++)
                    {
                        filtroRegistro = filtroRegistro + regRuolo[i].systemId;
                        if (i < regRuolo.Length - 1)
                        {
                            filtroRegistro = filtroRegistro + " , ";
                        }
                    }

                    qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                }

                qco.doRuoli = true;
                qco.doUtenti = true;
                qco.doUo = true;
                qco.doListe = false;

                // Abilita la ricerca in rubrica comune, qualora l'utente sia abilitato
                if (callType != RubricaCallType.CALLTYPE_PROTO_INT_DEST)
                {
                    qco.doRubricaComune = (CommonAddressBook.Configurations.GetConfigurations(UIManager.UserManager.GetInfoUser()).GestioneAbilitata);
                }
                qco.queryCodiceEsatta = codiceEsatto;
                DocsPaWR.ElementoRubrica[] elSearch = getElementiRubrica(qco);
                return elSearch;
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica[] getElementiRubrica(ParametriRicercaRubrica qco)
        {
            try
            {
                DocsPaWR.SmistamentoRubrica smistamentoRubrica = new SmistamentoRubrica();
                return docsPaWS.rubricaGetElementiRubrica(qco, UIManager.UserManager.GetInfoUser(), smistamentoRubrica);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setQueryRubricaCaller(ref DocsPaWR.ParametriRicercaRubrica qco)
        {
            try
            {
                qco.caller = new DocsPaWR.RubricaCallerIdentity();
                System.Web.HttpContext ctx = System.Web.HttpContext.Current;
                qco.caller.IdRuolo = UIManager.RoleManager.GetRoleInSession().systemId;

                //se richiamo la rubrica Gestione Rubrica in testata320
                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_DEP_OSITO)
                {
                    qco.caller.IdRegistro = null;
                }
                else
                {
                    qco.caller.IdRegistro = UIManager.RegistryManager.GetRegistryInSession().systemId;
                }

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private static string getCondizioneRegistro(DocsPaWR.Registro[] listaReg)
        {
            string retValue = null;
            try
            {
                if (listaReg != null && listaReg.Length>0)
                {

                    foreach (DocsPaWR.Registro item in listaReg)
                    {
                        retValue += " " + item.systemId + ",";
                    }
                    if (retValue.EndsWith(","))
                    {
                        retValue = retValue.Remove(retValue.LastIndexOf(","));
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return retValue;
        }

        public static DocsPaWR.Corrispondente getCorrispondenteByCodRubricaRubricaComune(string codice)
        {
            try
            {
                return docsPaWS.GetCorrRubricaComune(codice, UIManager.UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ArrayList getCorrispondentiByCodLista(string codiceLista, string idAmm)
        {
            try
            {
                return new ArrayList(docsPaWS.getCorrispondentiByCodLista(codiceLista, idAmm, UIManager.UserManager.GetInfoUser()));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ArrayList getCorrispondentiByCodListaByUtente(string codiceLista, string idAmm)
        {
            try
            {
                return new ArrayList(docsPaWS.getCorrispondentiByCodListaByUtente(codiceLista, idAmm, UIManager.UserManager.GetInfoUser()));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


        public static string getNomeLista(string codiceLista, string idAmm)
        {
            try
            {
                return docsPaWS.getNomeLista(codiceLista, idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ArrayList getCorrispondentiByCodRF(string codiceRF)
        {
            try
            {
                return new ArrayList(docsPaWS.getCorrispondentiByCodRF(codiceRF));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getNomeRF(string codiceRF)
        {
            try
            {
                return docsPaWS.getNomeRF(codiceRF);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Corrispondente getCorrispondenteRubrica(string codiceRubrica, DocsPaWR.RubricaCallType callType)
        {
            try
            {
                DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
                //cerco su tutti i tipi utente:
                qco.calltype = callType;
                setQueryRubricaCaller(ref qco);

                //  qco.caller.filtroRegistroPerRicerca = qco.caller.IdRegistro;

                qco.codice = codiceRubrica;
                qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;


                //questo serve perchè in questi casi quando si cerca un esterno all'amministrazione
                //si deve ricercare anche tra gli esterni all'amministrazione che sono creati su degli RF
                //associati al registro corrente  (nel caso di protocollo)
                if (callType == RubricaCallType.CALLTYPE_PROTO_IN
                    || callType == RubricaCallType.CALLTYPE_PROTO_IN_INT
                    || callType == RubricaCallType.CALLTYPE_PROTO_OUT
                    || callType == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                   || callType == RubricaCallType.CALLTYPE_CORR_INT_EST
                || callType == RubricaCallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI)
                {
                    if (qco.caller.IdRegistro != null && qco.caller.IdRegistro != string.Empty)
                    {

                        DocsPaWR.Registro[] listaReg = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, "1", qco.caller.IdRegistro);

                        //Ritorna una lista di RF concatenati da una ","
                        string condReg = getCondizioneRegistro(listaReg);

                        if (condReg != null && condReg != string.Empty)
                        {
                            //se cè almeno un RF allora aggancio anche l'id registro
                            // per ricercare tra tutti gli esterni appartenenti
                            //al mio registro e agli RF ad esso associati che posso vedere
                            condReg += ", " + qco.caller.IdRegistro;
                        }
                        else
                        {
                            condReg += qco.caller.IdRegistro;
                        }
                        qco.caller.filtroRegistroPerRicerca = condReg;
                    }
                }

                //in questo caso devo ricercare i corrispondenti esterni all'amministrazione
                //tra tutti i corrispondenti che sono stati creati su registi e rf a cui
                //il ruolo corrente è associato
                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                || callType == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                {

                    //nuova gestione: devo cercare in tutti i registri e RF visibili al ruolo

                    DocsPaWR.Registro[] regRuolo = UIManager.RegistryManager.GetListRegistriesAndRF(qco.caller.IdRuolo, "", "");

                    string filtroRegistro = "";
                    for (int i = 0; i < regRuolo.Length; i++)
                    {
                        filtroRegistro = filtroRegistro + regRuolo[i].systemId;
                        if (i < regRuolo.Length - 1)
                        {
                            filtroRegistro = filtroRegistro + " , ";
                        }
                    }

                    qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                }

                qco.doRuoli = true;
                qco.doUtenti = true;
                qco.doUo = true;
                qco.doListe = false;

                // Abilita la ricerca in rubrica comune, qualora l'utente sia abilitato
                qco.doRubricaComune = (CommonAddressBook.Configurations.GetConfigurations(UIManager.UserManager.GetInfoUser()).GestioneAbilitata);

                qco.queryCodiceEsatta = true;
                DocsPaWR.Corrispondente corrRes;
                DocsPaWR.ElementoRubrica[] elSearch = getElementiRubrica(qco);

                if (elSearch != null && elSearch.Length == 1)
                {
                    if (elSearch[0].rubricaComune != null && elSearch[0].rubricaComune.IdRubricaComune != null)
                    {
                        corrRes = getCorrispondenteByCodRubricaRubricaComune(elSearch[0].codice);
                    }
                    else
                    {
                        corrRes = GetCorrespondentBySystemId(elSearch[0].systemId);
                    }

                }
                else
                {
                    corrRes = null;
                }
                return corrRes;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Corrispondente GetCorrispondenteInterno(string codiceRubrica, bool fineValidita, bool destProtoInt)
        {
            try
            {
                Utente utente = UIManager.UserManager.GetUserInSession();
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                //TODO
                qco.idRegistri = new string[1];
                qco.idRegistri[0] = UIManager.RegistryManager.GetRegistryInSession().systemId;
                qco.codiceRubrica = codiceRubrica;
                qco.getChildren = false;
                qco.idAmministrazione = utente.idAmministrazione;
                qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.fineValidita = fineValidita;
                qco.descrizioneRuolo = "";

                DocsPaWR.Corrispondente result;
                //DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti(qco);
                DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti_Aut(qco);

                if (corrispondenti.Length > 0)
                {
                    //result = corrispondenti[0];
                    result = getAllRuoli(corrispondenti, false)[0];
                }
                else
                {
                    result = null;
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Corrispondente[] getAllRuoli(DocsPaWR.Corrispondente[] corrispondenti, bool cameFromRubrica)
        {
            try
            {
                string l_oldSystemId = "";
                System.Object[] l_objects = new System.Object[0];
                System.Object[] l_objects_ruoli = new System.Object[0];
                DocsPaWR.Ruolo[] lruolo = new DocsPaWR.Ruolo[0];
                int i = 0;
                foreach (DocsPaWR.Corrispondente t_corrispondente in corrispondenti)
                {
                    string t_systemId = t_corrispondente.systemId;
                    if (t_systemId != l_oldSystemId)
                    {

                        l_objects = Utils.Common.AddToArray(l_objects, t_corrispondente);
                        l_oldSystemId = t_systemId;
                        i = i + 1;
                        continue;
                    }
                    else
                    {
                        /* il corrispondente non viene aggiunto, in quanto sarebbe un duplicato 
                         * ma viene aggiunto solamente il ruolo */

                        if (t_corrispondente.GetType().Equals(typeof(DocsPaWR.Utente)))
                        {
                            if (!cameFromRubrica)
                            {
                                if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPaWR.Utente)))
                                {
                                    l_objects_ruoli = ((Utils.Common.AddToArray(((DocsPaWR.Utente)(l_objects[i - 1])).ruoli, ((DocsPaWR.Utente)t_corrispondente).ruoli[0])));
                                    DocsPaWR.Ruolo[] l_ruolo = new DocsPaWR.Ruolo[l_objects_ruoli.Length];
                                    ((DocsPaWR.Utente)(l_objects[i - 1])).ruoli = l_ruolo;
                                    l_objects_ruoli.CopyTo(((DocsPaWR.Utente)(l_objects[i - 1])).ruoli, 0);

                                }
                            }
                        }
                    }
                }

                DocsPaWR.Corrispondente[] l_corrSearch = new DocsPaWR.Corrispondente[l_objects.Length];
                l_objects.CopyTo(l_corrSearch, 0);

                return l_corrSearch;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool alradyExistCorrespondent(List<Corrispondente> lista, Corrispondente corr)
        {
            bool retVal = false;
            try
            {
                if (lista != null)
                {
                    int i = lista.FindIndex(o => o.systemId == corr.systemId);
                    if (corr.systemId == null) i = -1;
                    if (i != -1)
                    {
                        retVal = true;
                    }
                    else
                    {
                        int j = lista.FindIndex(o => o.codiceRubrica == corr.codiceRubrica && string.IsNullOrEmpty(o.systemId) && string.IsNullOrEmpty(corr.systemId));
                        if (j != -1)
                        {
                            retVal = true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return retVal;
        }

        public static bool esisteCorrispondente(Corrispondente[] lista, Corrispondente corr)
        {
            try
            {
                if (corr!=null && corr.systemId != null)
                {
                    if (lista != null)
                    {
                        for (int i = 0; i < lista.Length; i++)
                        {
                            if (lista[i] != null && lista[i].systemId != null)
                                if (lista[i].systemId.Equals(corr.systemId))
                                    return true;
                        }
                    }
                }
                else
                {
                    if (lista != null)
                    {
                        for (int i = 0; i < lista.Length; i++)
                        {
                            if (lista[i].descrizione.ToUpper().Equals(corr.descrizione.ToUpper()))
                                return true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return false;
        }

        public static Corrispondente[] addCorrispondenteModificato(Corrispondente[] lista, Corrispondente[] listaCC, Corrispondente corr)
        {
            try
            {
                Corrispondente[] nuovaLista;
                if (lista != null)
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = getCorrispondentiByCodLista(corr.codiceRubrica, idAmm);

                        System.Object[] l_objects = new System.Object[0];


                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];

                            if (!esisteCorrispondente(lista, c) && (!esisteCorrispondente(listaCC, c)))
                            {
                                l_objects = Utils.Common.AddToArray(l_objects, c);
                            }
                        }

                        nuovaLista = new Corrispondente[l_objects.Length + lista.Length];
                        lista.CopyTo(nuovaLista, 0);
                        l_objects.CopyTo(nuovaLista, lista.Length);

                    }
                    else
                    {
                        int len = lista.Length;
                        nuovaLista = new Corrispondente[len + 1];
                        lista.CopyTo(nuovaLista, 0);
                        nuovaLista[len] = corr;
                    }
                }
                else
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = getCorrispondentiByCodLista(corr.codiceRubrica, idAmm);
                        System.Object[] l_objects = new System.Object[0];

                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];

                            if (!esisteCorrispondente(lista, c) && !esisteCorrispondente(listaCC, c))
                            {
                                l_objects = Utils.Common.AddToArray(l_objects, c);
                            }
                        }

                        nuovaLista = new Corrispondente[l_objects.Length];
                        l_objects.CopyTo(nuovaLista, 0);

                    }
                    else
                    {
                        nuovaLista = new Corrispondente[1];
                        nuovaLista[0] = corr;
                    }
                }
                return nuovaLista;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<Corrispondente> AddCorrespondet(List<Corrispondente> lista, List<Corrispondente> listaCC, Corrispondente corr)
        {
            try
            {
                List<Corrispondente> result = lista;
                if (lista != null && lista.Count > 0)
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = getCorrispondentiByCodLista(corr.codiceRubrica, idAmm);
                        
                        System.Object[] l_objects = new System.Object[0];


                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];
                            if (c.tipoIE == "I" || (HttpContext.Current.Session["typeDoc"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["typeDoc"].ToString()) && !HttpContext.Current.Session["typeDoc"].ToString().ToUpper().Equals("I")))
                            {
                                if (c != null && !string.IsNullOrEmpty(c.codiceRubrica))
                                {
                                    if (!esisteCorrispondente(lista.ToArray(), c) && (!esisteCorrispondente(listaCC.ToArray(), c)))
                                    {
                                        result.Insert(0, c);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (corr.tipoCorrispondente == "F" && !corr.inRubricaComune)
                        {
                            string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                            ArrayList lsCorr = getCorrispondentiByCodRF(corr.codiceRubrica);

                            System.Object[] l_objects = new System.Object[0];


                            for (int i = 0; i < lsCorr.Count; i++)
                            {
                                Corrispondente c = (Corrispondente)lsCorr[i];

                                if (c != null && !string.IsNullOrEmpty(c.codiceRubrica))
                                {
                                    if (!esisteCorrispondente(lista.ToArray<Corrispondente>(), c))
                                    {
                                        result.Insert(0, c);
                                    }
                                }
                            }
                        }
                        else
                        {
                            result.Insert(0, corr);
                        }
                    }
                }
                else
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = getCorrispondentiByCodLista(corr.codiceRubrica, idAmm);
                        System.Object[] l_objects = new System.Object[0];

                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];
                            if (c.tipoIE == "I" || (HttpContext.Current.Session["typeDoc"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["typeDoc"].ToString()) && !HttpContext.Current.Session["typeDoc"].ToString().ToUpper().Equals("I")))
                            {
                                if (c != null && !string.IsNullOrEmpty(c.codiceRubrica))
                                {
                                    if (!esisteCorrispondente(lista.ToArray(), c) && !esisteCorrispondente(listaCC.ToArray(), c))
                                    {
                                        result.Insert(0, c);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (corr.tipoCorrispondente == "F" && !corr.inRubricaComune)
                        {
                            string idAmm = UserManager.GetInfoUser().idAmministrazione;
                            ArrayList lsCorr = getCorrispondentiByCodRF(corr.codiceRubrica);
                            System.Object[] l_objects = new System.Object[0];

                            for (int i = 0; i < lsCorr.Count; i++)
                            {
                                Corrispondente c = (Corrispondente)lsCorr[i];

                                if (c != null && !string.IsNullOrEmpty(c.codiceRubrica))
                                {
                                    if (!esisteCorrispondente(lista.ToArray(), c) && !esisteCorrispondente(listaCC.ToArray(), c))
                                    {
                                        result.Insert(0, c);
                                    }
                                }
                            }
                        }
                        else
                        {
                            result.Insert(0, corr);
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// da la descrizione del corr, ma senza dettaglio delle uo.
        /// </summary>
        /// <param name="myCorr"></param>
        /// <returns></returns>
        public static string getDecrizioneCorrispondenteSemplice(Corrispondente myCorr)
        {
            try
            {
                string desc = "";
                if (myCorr == null)
                    return "";
                //			if (myCorr.GetType() == typeof(Ruolo)) 
                //			{
                //				Ruolo corrRuolo = (Ruolo) myCorr;
                //				string descrUO = "";				
                //				UnitaOrganizzativa corrUO;
                //				corrUO = corrRuolo.uo;
                //
                //				while(corrUO!=null) 
                //				{
                //					descrUO = descrUO + " - " + corrUO.descrizione;
                //					corrUO = corrUO.parent;
                //				}
                //					
                //				desc = corrRuolo.descrizione + descrUO;
                //			} 
                //			else 
                desc = myCorr.descrizione;
                return desc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.ElementoRubrica[] getElementiRubricaMultipliSoloInterni(string codiceRubrica, DocsPaWR.RubricaCallType callType, bool codiceEsatto, bool role, bool user)
        {
            try
            {
                DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
                //cerco su tutti i tipi utente:
                qco.calltype = callType;
                setQueryRubricaCaller(ref qco);

                //  qco.caller.filtroRegistroPerRicerca = qco.caller.IdRegistro;

                qco.codice = codiceRubrica;
                qco.tipoIE = DocsPaWR.AddressbookTipoUtente.GLOBALE;


                //questo serve perchè in questi casi quando si cerca un esterno all'amministrazione
                //si deve ricercare anche tra gli esterni all'amministrazione che sono creati su degli RF
                //associati al registro corrente  (nel caso di protocollo)
                if (callType == RubricaCallType.CALLTYPE_PROTO_IN
                    || callType == RubricaCallType.CALLTYPE_PROTO_IN_INT
                    || callType == RubricaCallType.CALLTYPE_PROTO_OUT
                    || callType == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                {
                    if (qco.caller.IdRegistro != null && qco.caller.IdRegistro != string.Empty)
                    {

                        DocsPaWR.Registro[] listaReg = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, "1", qco.caller.IdRegistro);

                        //Ritorna una lista di RF concatenati da una ","
                        string condReg = getCondizioneRegistro(listaReg);

                        if (condReg != null && condReg != string.Empty)
                        {
                            //se cè almeno un RF allora aggancio anche l'id registro
                            // per ricercare tra tutti gli esterni appartenenti
                            //al mio registro e agli RF ad esso associati che posso vedere
                            condReg += ", " + qco.caller.IdRegistro;
                        }
                        else
                        {
                            condReg += qco.caller.IdRegistro;
                        }
                        qco.caller.filtroRegistroPerRicerca = condReg;
                    }
                }

                //in questo caso devo ricercare i corrispondenti esterni all'amministrazione
                //tra tutti i corrispondenti che sono stati creati su registi e rf a cui
                //il ruolo corrente è associato
                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                || callType == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE
                || qco.calltype == RubricaCallType.CALLTYPE_RICERCA_CORRISPONDENTE)
                {
                    //nuova gestione: devo cercare in tutti i registri e RF visibili al ruolo

                    DocsPaWR.Registro[] regRuolo = UIManager.RegistryManager.GetListRegistriesAndRF(qco.caller.IdRuolo, string.Empty, string.Empty);

                    string filtroRegistro = "";
                    for (int i = 0; i < regRuolo.Length; i++)
                    {
                        filtroRegistro = filtroRegistro + regRuolo[i].systemId;
                        if (i < regRuolo.Length - 1)
                        {
                            filtroRegistro = filtroRegistro + " , ";
                        }
                    }
                    qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                }

                qco.doRuoli = role;
                qco.doUtenti = user;
                qco.doUo = false;
                qco.doListe = false;

                // Abilita la ricerca in rubrica comune, qualora l'utente sia abilitato
                qco.doRubricaComune = false;

                qco.queryCodiceEsatta = codiceEsatto;
                DocsPaWR.ElementoRubrica[] elSearch = getElementiRubrica(qco);

                return elSearch;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<Corrispondente> addCorrispondente(List<Corrispondente> lista, Corrispondente corr)
        {
            try
            {
                List<Corrispondente> result = lista;
                if (lista != null && lista.Count > 0)
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = getCorrispondentiByCodLista(corr.codiceRubrica, idAmm);

                        System.Object[] l_objects = new System.Object[0];


                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];

                            if (!esisteCorrispondente(lista.ToArray<Corrispondente>(), c))
                            {
                                result.Insert(0, c);
                            }
                        }
                    }
                    else
                    {
                        if (corr.tipoCorrispondente == "F" && !corr.inRubricaComune)
                        {
                            string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                            ArrayList lsCorr = getCorrispondentiByCodRF(corr.codiceRubrica);

                            System.Object[] l_objects = new System.Object[0];


                            for (int i = 0; i < lsCorr.Count; i++)
                            {
                                Corrispondente c = (Corrispondente)lsCorr[i];

                                if (!esisteCorrispondente(lista.ToArray<Corrispondente>(), c))
                                {
                                    result.Insert(0, c);
                                }
                            }
                        }
                        else
                        {
                            result.Insert(0, corr);
                        }
                    }
                }
                else
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = getCorrispondentiByCodLista(corr.codiceRubrica, idAmm);

                        System.Object[] l_objects = new System.Object[0];

                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];

                            if (!esisteCorrispondente(lista.ToArray<Corrispondente>(), c))
                            {
                                result.Insert(0, c);
                            }
                        }
                    }
                    else
                    {
                        if (corr.tipoCorrispondente == "F" && !corr.inRubricaComune)
                        {
                            string idAmm = UserManager.GetInfoUser().idAmministrazione;
                            ArrayList lsCorr = getCorrispondentiByCodRF(corr.codiceRubrica);
                            System.Object[] l_objects = new System.Object[0];

                            for (int i = 0; i < lsCorr.Count; i++)
                            {
                                Corrispondente c = (Corrispondente)lsCorr[i];

                                if (!esisteCorrispondente(lista.ToArray(), c) && !esisteCorrispondente(lista.ToArray<Corrispondente>(), c))
                                {
                                    result.Insert(0, c);
                                }
                            }
                        }
                        else
                        {
                            result.Insert(0, corr);
                        }
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<Corrispondente> AddCorrespondent(List<Corrispondente> lista, Corrispondente corr)
        {
            try
            {
                List<Corrispondente> result = lista;
                if (lista != null)
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = getCorrispondentiByCodLista(corr.codiceRubrica, idAmm);

                        System.Object[] l_objects = new System.Object[0];


                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];

                            if (!esisteCorrispondente(lista.ToArray(), c))
                            {
                                result.Insert(0, c);
                            }
                        }

                    }
                    else
                    {
                        result.Insert(0, corr);
                    }
                }
                else
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UIManager.UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = getCorrispondentiByCodLista(corr.codiceRubrica, idAmm);

                        System.Object[] l_objects = new System.Object[0];

                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];

                            if (!esisteCorrispondente(lista.ToArray(), c))
                            {
                                result.Insert(0, c);
                            }
                        }

                    }
                    else
                    {
                        result.Insert(0, corr);
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Corrispondente getCorrispondenteBySystemIDDisabled(string systemID)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteBySystemIdDisabled(systemID);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Corrispondente getCorrispondenteByCodRubrica(string codice, bool storicizzato)
        {
            try
            {
                DocsPaWR.Registro[] regAll = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, string.Empty, string.Empty);
                string condRegistri = string.Empty;
                if (regAll != null && regAll.Length > 0)
                {
                    condRegistri = " and (id_registro in (";
                    foreach (DocsPaWR.Registro reg in regAll)
                        condRegistri += reg.systemId + ",";
                    condRegistri = condRegistri.Substring(0, condRegistri.Length - 1);
                    condRegistri += ") OR id_registro is null)";
                }
                return docsPaWS.AddressbookGetCorrispondenteByCodRubrica(codice, UIManager.UserManager.GetInfoUser(), condRegistri, storicizzato);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<Corrispondente> RemoveCorrespondent(List<Corrispondente> lista, int index)
        {
            try
            {
                if (lista == null || lista.Count < index)
                    return lista;

                if (lista.Count == 1)
                    return null;

                List<Corrispondente> result = lista;
                lista.RemoveAt(index);

                return lista;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getSystemIdRF(string codiceRF)
        {
            try
            {
                return docsPaWS.getSystemIdRF(codiceRF);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool VerificaAutorizzazioneRuoloSuRegistro(string idRegistro, string idRuolo)
        { 
            bool result = false;
            try
            {               
                if (idRegistro != null && idRegistro != "")
                {
                    DocsPaWR.Registro[] RegRuoloSel = UIManager.RegistryManager.GetRegistriesByRole(idRuolo);
                    foreach (DocsPaWR.Registro reg in RegRuoloSel)
                    {
                        if (idRegistro == reg.systemId)
                        {
                            result = true;
                            break;
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// ritorna un oggetto null, se il corrispondente non osiste o è disabilitato
        /// </summary>
        /// <param name="page"></param>
        /// <param name="codice"></param>
        /// <param name="tipoIE"></param>
        /// <returns></returns>
        public static DocsPaWR.Corrispondente getCorrispondenteByCodRubricaIENotdisabled(string codice, DocsPaWR.AddressbookTipoUtente tipoIE)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteByCodRubricaIENotDisabled(codice, tipoIE, UIManager.UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool UOHasReferenceRole(string idCorr)
        {
            try
            {
                return docsPaWS.UOHasReferenceRole(idCorr);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            
        }

        public static string getOldDescByCorr(string idCorr)
        {
            try
            {
                return docsPaWS.getOldDescByCorr(idCorr);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }            
        }

        public static string getCheckInteropFromSysIdCorrGlob(string idCorr)
        {
            try
            {
                return docsPaWS.getCheckInteropFromSysIdCorrGlob(idCorr);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DataSet GetCorrByEmail(string mail, string reg)
        {
            try
            {
                return docsPaWS.GetCorrByEmail(mail, reg);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }            
        }

        public static DataSet GetCorrByEmailAndDescr(string mail, string oldDesc, string reg)
        {
            try
            {
                return docsPaWS.GetCorrByEmailAndDescr(mail, oldDesc, reg);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.CorrespondentDetails GetCorrespondentDetails(string idCorr)
        {
            try
            {
                if (idCorr != null)
                {
                    return docsPaWS.GetCorrespondentDetails(idCorr);
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Corrispondente ValorizeInfoCorr(Corrispondente corr
            , string address, string city, string zipCode, string district, string country, string phone, string phone2
            , string fax, string taxId, string note, string place, string title, string birthPlace, string birthDay, string commercialId,string ipacode)
        {
            try
            {
                return docsPaWS.ValorizeInfoCorr(corr, address, city, zipCode, district, country, phone, phone2, fax, taxId, note, place, title, birthPlace, birthDay, commercialId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.MezzoSpedizione[] GetAmmListaMezzoSpedizione(string idAmm, bool showAll)
        {
            try
            {
                return docsPaWS.AmmListaMezzoSpedizione(idAmm, false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }            
        }

        public static string[] GetListaTitoli()
        {
            try
            {
                return docsPaWS.GetListaTitoli();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }           
        }

        public static DataSet isCorrInListaDistr(string idCorrGlobali)
        {
            try
            {
                return docsPaWS.isCorrInListaDistr(idCorrGlobali);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static int ResetCodRubCorrIterop(string corrSystemId, string val)
        {
            try
            {
                return docsPaWS.ResetCodRubCorrIterop(corrSystemId, val);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
        }

        public static bool IsCodRubricaPresente(string codRubrica, string tipoCorr, string idAmm, string idReg, bool inRubricaComune)
        {
            try
            {
                return docsPaWS.IsCodRubricaPresente(codRubrica, tipoCorr, idAmm, idReg, inRubricaComune);
            }
            catch (Exception e)
            {
                UIManager.AdministrationManager.DiagnosticError(e);
                return false;
            }
        }

        /// <summary>
        /// Servizio web per resettare VAR_INSERT_BY_INTEROP
        /// </summary>
        /// <param name="corrSystemId">ID del corrispondente</param>
        /// <param name="val">Valore col quale resettare il campo VAR_INSERT_BY_INTEROP</param>
        public static int ResetCorrVarInsertIterop(string corrSystemId, string val)
        {
            try
            {
                return docsPaWS.ResetCorrVarInsertIterop(corrSystemId, val);
            }
            catch (Exception e)
            {

                UIManager.AdministrationManager.DiagnosticError(e);
                return 0;
            }
        }

        public static DocsPaWR.Corrispondente GetCorrispondenteInterno(string codiceRubrica, bool fineValidita)
        {
            try
            {
                Utente utente = UIManager.UserManager.GetUserInSession();
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                qco.idRegistri = new string[1];
                qco.idRegistri[0] = UIManager.RegistryManager.GetRegistryInSession().systemId;
                qco.codiceRubrica = codiceRubrica;
                qco.getChildren = false;
                qco.idAmministrazione = utente.idAmministrazione;
                qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.fineValidita = fineValidita;
                DocsPaWR.Corrispondente result;
                DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti(qco);

                if (corrispondenti.Length > 0)
                {
                    //result = corrispondenti[0];
                    result = getAllRuoli(corrispondenti, false)[0];
                }
                else
                {
                    result = null;
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool ImportaRubrica(byte[] dati, bool enableDistributionLists, ref int corrInseriti, ref int corrAggiornati, ref int corrRimossi, ref int corrNonInseriti, ref int corrNonAggiornati, ref int corrNonRimossi)
        {
            int flagListe = enableDistributionLists ? 1 : 0;
            return docsPaWS.ImportaRubrica(UserManager.GetInfoUser(), dati, flagListe, "importRubrica.xls", ref corrInseriti, ref corrAggiornati, ref corrRimossi, ref corrNonInseriti, ref corrNonAggiornati, ref corrNonRimossi);
        }

        public static object[] getLogImportRubrica()
        {
            return docsPaWS.getLogImportRubrica();
        }

        public static FileDocumento ExportRubrica(InfoUtente infoUser, bool store, string register)
        {
            return docsPaWS.ExportRubrica(infoUser, store, register);
        }

        public static string[] GetListaCapComuni(string prefixCap, string comune)
        {
            return docsPaWS.GetListaCapComuni(prefixCap, comune);
        }

        public static string[] GetListaComuni(string prefixComune)
        {
            return docsPaWS.GetListaComuni(prefixComune);
        }

        public static DocsPaWR.InfoComune GetCapComuni(string cap, string comune)
        {
            return docsPaWS.GetCapComuni(cap, comune);
        }

        public static DocsPaWR.InfoComune GetProvinciaComune(string comune)
        {
            return docsPaWS.GetProvinciaComune(comune);
        }
    }
}
