using System;
using System.Configuration;
using System.Web.UI;
using SAAdminTool.DocsPaWR;
using System.Collections;
using System.Data;
using System.Web;

namespace SAAdminTool
{
	/// <summary>
	/// Summary description for GestManager.
	/// </summary>
	public class GestManager
	{
		private static SAAdminTool.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();

//gestione registri


		public static Registro getRegistroSel(Page page) 
		{
            return getRegistroSel();
		}

        public static Registro getRegistroSel()
        {
            return (Registro)HttpContext.Current.Session["regElenco.Registro"];
        }

		public static void setRegistroSel(Page page, Registro registro) 
		{
			setRegistroSel(registro);
		}

        public static void setRegistroSel(Registro registro)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["regElenco.Registro"] = registro;
        }

		public static void removeRegistroSel(Page page) 
		{
            removeRegistroSel();
		}

        public static void removeRegistroSel()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("regElenco.Registro");
        }

        public static void getCasellaSel(Page page)
        { 
            getCasellaSel();
        }
        
        public static string getCasellaSel()
        { 
            if( HttpContext.Current.Session["regElenco.CasellaSel"] != null)
                return HttpContext.Current.Session["regElenco.CasellaSel"].ToString();
            else
                return string.Empty;
        }

        public static void setCasellaSel(Page page, string casella)
        {
            setCasellaSel(casella);
        }

        public static void setCasellaSel(string casella)
        { 
            HttpContext.Current.Session["regElenco.CasellaSel"] = casella;
        }

        public static void removeCasellaSel(Page page)
        {
            removeCasellaSel();
        }

        public static void removeCasellaSel()
        {
            HttpContext.Current.Session.Remove("regElenco.CasellaSel");
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
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return reg;
        }
        
        public static Registro cambiaStatoRegistro(Page page) 
		{		
			Registro registro = null;

			try 
			{
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				//Celeste
				//registro = docsPaWS.RegistriCambiaStato(infoUtente.idPeople, infoUtente.idCorrGlobali, getRegistroSel(page));
				registro = docsPaWS.RegistriCambiaStato(infoUtente, getRegistroSel(page));
				//Fine Celeste

				if(registro == null)
				{
					throw new Exception();
				}

				setRegistroSel(page, registro);
			} 
			catch (Exception exception) 
			{
				ErrorManager.redirect(page, exception);
			}

			return registro;
		}

		public static Registro modificaRegistro(Page page, Registro registro) 
		{			
			Registro result = null;

			try 
			{
				result = docsPaWS.RegistriModifica(registro, UserManager.getInfoUtente(page));

				if(result == null)
				{
					throw new Exception();
				}
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			} 
			catch (Exception exception) 
			{
				ErrorManager.redirect(page, exception);
			}

			return result;
		}

		public static SAAdminTool.DocsPaWR.StampaRegistroResult StampaRegistro(Page page,DocsPaWR.InfoUtente infoUt,Ruolo ruolo, Registro registro) 
		{			
			try 
			{
				DocsPaWR.StampaRegistroResult result = docsPaWS.RegistriStampa(infoUt,ruolo,registro);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 

			return null;
		}

		public static FileDocumento GetReportRegistroWithFilters(
								Page page,
								DocsPaWR.InfoUtente infoUt,
								Ruolo ruolo, 
								Registro registro,
								FiltroRicerca[][] filters)
		{
			FileDocumento retValue=null;

			try 
			{
				// Chiamata Web Service
				DocsPaWR.StampaRegistroResult result=docsPaWS.RegistriStampaWithFilters(infoUt,ruolo,registro,filters,out retValue);

				if(result == null)
				{
					throw new Exception();
				}

			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
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
				result=docsPaWS.StampaBusteWithFilters(infoUt,ruolo,registro,filters);

			} 

			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 

			return result;
		
		}

		private static void pause(Double timeToPause)
		{	
			bool inPause=true;
			DateTime start=DateTime.Now;
			DateTime stop=start.AddMilliseconds(timeToPause);

			while (inPause)
			{
				DateTime adesso=DateTime.Now; 
				if (DateTime.Compare(adesso,stop)==1)
				{
					inPause=false;
				}
			}
		}


		public static bool startIstitutionalMailboxCheck(Page page,DocsPaWR.Registro registro,out SAAdminTool.DocsPaWR.MailAccountCheckResponse checkResponse)
		{
			bool retValue=false;
			checkResponse=null;

			try 
			{
				string serverName=Utils.getHttpFullPath(page);

				DocsPaWR.Utente ut1 = (SAAdminTool.DocsPaWR.Utente)page.Session["userData"] ;
                DocsPaWR.Ruolo ruolo = (SAAdminTool.DocsPaWR.Ruolo )page.Session["userRuolo"];

                retValue = docsPaWS.InteroperabilitaRicezione(serverName, registro, ut1, ruolo, out checkResponse);
			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 

			return retValue;
		}

	}
}
