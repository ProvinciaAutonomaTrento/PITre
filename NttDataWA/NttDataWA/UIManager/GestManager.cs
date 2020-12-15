using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class GestManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        //gestione registri

        public static Registro getRegistroSel(Page page)
        {
            try
            {
                return getRegistroSel();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Registro getRegistroSel()
        {
            try
            {
                return (Registro)HttpContext.Current.Session["regElenco.Registro"];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setRegistroSel(Page page, Registro registro)
        {
            try
            {
                setRegistroSel(registro);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void setRegistroSel(Registro registro)
        {
            try
            {
                HttpContext ctx = HttpContext.Current;
                ctx.Session["regElenco.Registro"] = registro;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void removeRegistroSel(Page page)
        {
            try
            {
                removeRegistroSel();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void removeRegistroSel()
        {
            try
            {
                HttpContext ctx = HttpContext.Current;
                ctx.Session.Remove("regElenco.Registro");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void getCasellaSel(Page page)
        {
            try
            {
                getCasellaSel();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string getCasellaSel()
        {
            try
            {
                if (HttpContext.Current.Session["regElenco.CasellaSel"] != null)
                    return HttpContext.Current.Session["regElenco.CasellaSel"].ToString();
                else
                    return string.Empty;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setCasellaSel(Page page, string casella)
        {
            try
            {
                setCasellaSel(casella);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void setCasellaSel(string casella)
        {
            try
            {
                HttpContext.Current.Session["regElenco.CasellaSel"] = casella;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void removeCasellaSel(Page page)
        {
            try
            {
                removeCasellaSel();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void removeCasellaSel()
        {
            try
            {
                HttpContext.Current.Session.Remove("regElenco.CasellaSel");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static Registro getRegistroById(Page page, string idReg)
        {
            Registro reg = null;
            try
            {
                reg = docsPaWS.GetRegistroBySistemId(idReg);
                if (reg == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return reg;
        }

        public static Registro cambiaStatoRegistro(Registro reg)
        {
            Registro registro = null;

            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                //Celeste
                //registro = docsPaWS.RegistriCambiaStato(infoUtente.idPeople, infoUtente.idCorrGlobali, getRegistroSel(page));
                registro = docsPaWS.RegistriCambiaStato(infoUtente, reg);
                //Fine Celeste

                if (registro == null)
                {
                    throw new Exception();
                }

                //setRegistroSel(page, registro);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            return registro;
        }

        public static Registro modificaRegistro(Page page, Registro registro)
        {
            Registro result = null;

            try
            {
                result = docsPaWS.RegistriModifica(registro, UserManager.GetInfoUser());

                if (result == null)
                {
                    throw new Exception();
                }
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            return result;
        }

        public static DocsPaWR.StampaRegistroResult StampaRegistro(Page page, DocsPaWR.InfoUtente infoUt, Ruolo ruolo, Registro registro)
        {
            try
            {
                DocsPaWR.StampaRegistroResult result = docsPaWS.RegistriStampa(infoUt, ruolo, registro);

                if (result == null)
                {
                    throw new Exception();
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static FileDocumento GetReportRegistroWithFilters(
                                Page page,
                                DocsPaWR.InfoUtente infoUt,
                                Ruolo ruolo,
                                Registro registro,
                                FiltroRicerca[][] filters)
        {
            FileDocumento retValue = null;

            try
            {
                // Chiamata Web Service
                DocsPaWR.StampaRegistroResult result = docsPaWS.RegistriStampaWithFilters(infoUt, ruolo, registro, filters, out retValue);

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

            return retValue;
        }

        public static FileDocumento GetReportBusteWithFilters(
            Page page,
            DocsPaWR.InfoUtente infoUt,
            Ruolo ruolo,
            Registro registro,
            FiltroRicerca[][] filters)
        {
            DocsPaWR.FileDocumento result = null;
            try
            {
                result = docsPaWS.StampaBusteWithFilters(infoUt, ruolo, registro, filters);

            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            return result;

        }

        private static void pause(Double timeToPause)
        {
            try
            {
                bool inPause = true;
                DateTime start = DateTime.Now;
                DateTime stop = start.AddMilliseconds(timeToPause);

                while (inPause)
                {
                    DateTime adesso = DateTime.Now;
                    if (DateTime.Compare(adesso, stop) == 1)
                    {
                        inPause = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static bool startIstitutionalMailboxCheck(Page page, DocsPaWR.Registro registro, out DocsPaWR.MailAccountCheckResponse checkResponse)
        {
            bool retValue = false;
            checkResponse = null;

            try
            {
                string serverName = utils.getHttpFullPath();

                DocsPaWR.Utente ut1 = (DocsPaWR.Utente)page.Session["userData"];
                DocsPaWR.Ruolo ruolo = (DocsPaWR.Ruolo)page.Session["userRuolo"];
                retValue = docsPaWS.InteroperabilitaRicezione(serverName, registro, ut1, ruolo, out checkResponse);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return retValue;
        }

        public static OrgRegistro GetRegisterAmm(string idRegister)
        {
            try
            {
                return docsPaWS.AmmGetRegistro(idRegister);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Aggiornamento registro
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public static ValidationResultInfo UpdateRegistro(OrgRegistro registro)
        {
            try
            {
                return docsPaWS.AmmUpdateRegistro(ref registro);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static CasellaRegistro[] GetMailRegistro(string idRegistro)
        {
            try
            {
                return docsPaWS.AmmGetMailRegistro(idRegistro);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ValidationResultInfo UpdateMailRegistro(string idRegistro, CasellaRegistro[] caselle)
        {
            try
            {
                return docsPaWS.AmmUpdateMailRegistro(idRegistro, caselle);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}