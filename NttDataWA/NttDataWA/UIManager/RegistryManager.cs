using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using System.Web.UI;
using NttDataWA.DocsPaWR;
using System.Globalization;

namespace NttDataWA.UIManager
{
    public class RegistryManager
    {       
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static void SetRegistryInSession(DocsPaWR.Registro registry)
        {
            try
            {
                HttpContext.Current.Session["userRegistry"] = registry;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Return the registry object from session
        /// </summary>
        /// <returns>Registro</returns>
        public static DocsPaWR.Registro GetRegistryInSession()
        {
            try
            {
                return HttpContext.Current.Session["userRegistry"] as DocsPaWR.Registro;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetRFListInSession(DocsPaWR.Registro[] lista)
        {
            try
            {
                HttpContext.Current.Session["userRFList"] = lista;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.Registro[] GetRFListInSession()
        {
            try
            {
                return HttpContext.Current.Session["userRFList"] as DocsPaWR.Registro[];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetRegAndRFListInSession(DocsPaWR.Registro[] lista)
        {
            try
            {
                HttpContext.Current.Session["userRegAndRFList"] = lista;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.Registro[] GetRegAndRFListInSession()
        {
            try
            {
                return HttpContext.Current.Session["userRegAndRFList"] as DocsPaWR.Registro[];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        /// <summary>
        /// Dato un idRuolo ritorna Registri e RF o solo RF.
        /// Se all="" allora anche idAooColl deve essere = "" e il metodo ritorna sia Reg che RF
        /// Se all="0" (e idAooColl deve essere = "" ) ritorna tutti gli RF assocaiati a un determinato ruolo
        /// Se all = "1" (idAooColl deve essere popolato con una systemId di un registro) e in
        /// tal caso il metoto ritorna tutti gli Rf associati a quel registro e visibili dal ruolo
        /// corrente
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Registro[] getListaRegistriWithRF(string all, string idAooColl)
        {
            Registro[] result = null;
            try
            {
                string idcorrglobal = UserManager.GetInfoUser().idCorrGlobali;

                //result = docsPaWS.UtenteGetRegistriWithRf(GetRegistryInSession().systemId, all, idAooColl);
                result = docsPaWS.UtenteGetRegistriWithRf(idcorrglobal, all, idAooColl);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        /// <summary>
        /// Dato un idRuolo ritorna Registri e RF o solo RF.
        /// Se all="" allora anche idAooColl deve essere = "" e il metodo ritorna sia Reg che RF
        /// Se all="0" (e idAooColl deve essere = "" ) ritorna tutti gli RF assocaiati a un determinato ruolo
        /// Se all = "1" (idAooColl deve essere popolato con una systemId di un registro) e in
        /// tal caso il metoto ritorna tutti gli Rf associati a quel registro e visibili dal ruolo
        /// corrente
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <param name="all"></param>
        /// <param name="idAooColl"></param>
        /// <returns></returns>
        public static Registro[] GetListRegistriesAndRF(string idRuolo, string all, string idAooColl)
        {
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistriWithRf(idRuolo, all, idAooColl);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            return result;
        }

        public static void setListaIdRegistri(string[] idRegistri)
        {
            try
            {
                HttpContext.Current.Session["rubrica.listaIdRegistri"] = idRegistri;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void setListaIdRegistri(string idRegistro)
        {
            try
            {
                string[] idRegistri = { idRegistro };
                setListaIdRegistri(idRegistri);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private static string[] getListaIdRegistri(Registro[] registri)
        {
            try
            {
                int numRegistri = registri.Length;
                string[] id = new string[numRegistri];
                for (int i = 0; i < numRegistri; i++)
                    id[i] = registri[i].systemId;
                return id;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string[] getListaIdRegistriRuolo(NttDataWA.DocsPaWR.Ruolo ruolo)
        {
            try
            {
                return getListaIdRegistri(ruolo.registri);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static String[] AddRfToRegistry(String[] array, String nuovoElemento)
        {
            try
            {
                String[] nuovaLista;
                if (array != null)
                {
                    int len = array.Length;
                    nuovaLista = new String[len + 1];
                    array.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = nuovoElemento;
                    return nuovaLista;
                }
                else
                {
                    nuovaLista = new String[1];
                    nuovaLista[0] = nuovoElemento;
                    return nuovaLista;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Registro[] GetRegistriesByRole(string idRole)
        {
            try
            {
                return docsPaWS.GetListaRegistriByRuolo(idRole);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Registro getRegistroBySistemId(string idRegistro)
        {
            try
            {
                DocsPaWR.Registro result = null;
                result = docsPaWS.GetRegistroBySistemId(idRegistro);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool isEnableRepertori(string idAmministrazione)
        {
            try
            {
                string valueGestioneRepertori = Utils.InitConfigurationKeys.GetValue(idAmministrazione, DBKeys.GESTIONE_REPERTORI.ToString());
                if (string.IsNullOrEmpty(valueGestioneRepertori))
                    valueGestioneRepertori = Utils.InitConfigurationKeys.GetValue("0", DBKeys.GESTIONE_REPERTORI.ToString());

                if (valueGestioneRepertori != null && valueGestioneRepertori == "1")
                    return true;
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Metodo per reperire registri/RF in amministrazione
        /// </summary>
        /// <param name="codAmm"></param>
        /// <param name="codChaRF">0, se voglio solo i registri, 1 se voglio solo gli RF, "" se voglio entrambi</param>
        /// <returns></returns>
        public static OrgRegistro[] getRegistriByCodAmm(string codAmm, string codChaRF)
        {
            OrgRegistro[] result = null;
            try
            {
                result = docsPaWS.AmmGetRegistri(codAmm, codChaRF);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        ///// <summary>
        ///// Metodo per reperire registri/RF in amministrazione
        ///// </summary>
        ///// <param name="codAmm"></param>
        ///// <param name="codChaRF">0, se voglio solo i registri, 1 se voglio solo gli RF, "" se voglio entrambi</param>
        ///// <returns></returns>
        //public static OrgRegistro[] getRegistriByCodAmm(string codAmm, string codChaRF)
        //{
        //    OrgRegistro[] result = null;
        //    try
        //    {
        //        result = docsPaWS.AmmGetRegistri(codAmm, codChaRF);

        //        if (result == null)
        //        {
        //            throw new Exception();
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        UIManager.AdministrationManager.DiagnosticError(ex);
        //        return null;
        //    }
        //    return result;
        //}

        public static OrgRegistro[] AmmGetRegistri()
        {
            DocsPaWR.InfoAmministrazione infoAmm = new DocsPaWR.InfoAmministrazione();
            infoAmm = docsPaWS.AmmGetInfoAmmCorrente(UserManager.GetInfoUser().idAmministrazione);

            return docsPaWS.AmmGetRegistri(infoAmm.Codice, "1");
        }

        public static string getStatoRegistro(DocsPaWR.Registro reg)
        {
            // R = Rosso -  CHIUSO
            // V = Verde -  APERTO
            // G = Giallo - APERTO IN GIALLO

            string dataApertura = reg.dataApertura;

            if (!dataApertura.Equals(""))
            {

                DateTime dt_cor = DateTime.Now;

                CultureInfo ci = new CultureInfo("it-IT");

                string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };

                DateTime d_ap = DateTime.ParseExact(dataApertura, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                //aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
                d_ap = d_ap.AddDays(1);

                string mydate = dt_cor.ToString(ci);

                //DateTime dt = DateTime.ParseExact(mydate,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);

                string StatoAperto = "A";
                if (reg.stato.Equals(StatoAperto))
                {
                    if (dt_cor.CompareTo(d_ap) > 0)
                    {
                        //data odierna maggiore della data di apertura del registro
                        return "G";
                    }
                    else
                        return "V";
                }
            }
            return "R";

        }

        public static string getRegTitolarioById(string idTitolario)
        {
            try
            {
                return docsPaWS.getRegTitolarioById(idTitolario);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
        }

        #region Session functions
        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static System.Object GetSessionValue(string sessionKey)
        {
            try
            {
                return System.Web.HttpContext.Current.Session[sessionKey];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            try
            {
                System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove(sessionKey);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion
    }
}