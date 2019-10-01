using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Web.UI;

namespace NttDataWA.UIManager
{
	public class ObjectManager
	{		
		private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

		public static Oggetto[] getListaOggettiByCod(Page page, List<string> idRegistri, string queryDescrizione, string queryCodice)
		{
			Oggetto[] result = null;

			//Tolgo i caratteri speciali dal campo descrizione oggetto
			queryDescrizione = queryDescrizione.Replace(System.Environment.NewLine, "");

			try
			{
				result = docsPaWS.DocumentoGetListaOggetti(getQueryOggetto(page, idRegistri.ToArray(), queryDescrizione, queryCodice));

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

		public static Oggetto[] getListaOggetti(Page page, List<string> idRegistri, string queryDescrizione)
		{
			Oggetto[] result = null;

			//Tolgo i caratteri speciali dal campo descrizione oggetto
			queryDescrizione = queryDescrizione.Replace(System.Environment.NewLine, "");

			try
			{
				result = docsPaWS.DocumentoGetListaOggetti(getQueryOggetto(page, idRegistri.ToArray(), queryDescrizione, ""));

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

		public static bool DeleteObject(Page page, Oggetto Object)
		{
			//Tolgo i caratteri speciali dal campo descrizione oggetto
			Object.descrizione = Object.descrizione.Replace(System.Environment.NewLine, "");
			try
			{
				InfoUtente infoUtente = UserManager.GetInfoUser();
				bool result = docsPaWS.UpdateOggetto(infoUtente, Object);
				return result;
			}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
		}

		public static Oggetto AddObject(Page page, Oggetto oggetto, Registro registro, ref string errMsg)
		{
			//Tolgo i caratteri speciali dal campo descrizione oggetto
			oggetto.descrizione = oggetto.descrizione.Replace(System.Environment.NewLine, "");

			try
			{
				InfoUtente infoUtente = UserManager.GetInfoUser();
				Oggetto result = docsPaWS.DocumentoAddOggetto(infoUtente, oggetto, registro, ref errMsg);

				if (result == null)
				{
					//throw new Exception();
				}

				return result;
			}
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
		}

        protected static DocumentoQueryOggetto getQueryOggetto(Page page, string[] registri, string queryDescrizione, string queryCodice)
        {
            try
            {
                DocumentoQueryOggetto query = new DocumentoQueryOggetto();
                //Utente utente = UserManager.GetInfoUser();
                Utente utente = HttpContext.Current.Session["userData"] as Utente;
                if (registri != null && registri.Length >= 0)
                {
                    query.idRegistri = new string[registri.Length];
                    for (int i = 0; i < registri.Length; i++)
                        query.idRegistri[i] = registri[i];
                }
                query.idAmministrazione = utente.idAmministrazione;
                query.queryDescrizione = queryDescrizione;
                query.queryCodice = queryCodice;

                return query;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
	}
}