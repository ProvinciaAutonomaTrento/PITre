using System;
using System.Collections;
using System.Web.SessionState;
using System.Web.UI;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Web;

namespace NttDataWA.UIManager
{

	
	/// <summary>
	/// Summary description for ModelliTrasmManager.
	/// </summary>
	public class ModelliTrasmManager
	{

        private static NttDataWA.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

		public ModelliTrasmManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static NttDataWA.DocsPaWR.RagioneTrasmissione[] getlistRagioniTrasm(string idAmm, bool all, string tipo_trasm)
		{
			try
			{            
    			DocsPaWR.TrasmissioneDiritti diritti = new NttDataWA.DocsPaWR.TrasmissioneDiritti();
				diritti.accessRights = "-1";  //INDICA CHE LE RAGIONI DA PRENDERE SONO TUTTE
				diritti.idAmministrazione = idAmm;
				DocsPaWR.RagioneTrasmissione[] result;
				if(all==true) //sel tutta la AOO -- solo trasmissioni con cha_tipo_dest = 'T','P'
				{
					result = docsPaWS.TrasmissioneGetRagioniATutti(diritti);
				}
				else
				{
					result = docsPaWS.TrasmissioneGetRagioni(diritti,false);
				}
				if(result == null)
				{
					throw new Exception();
				}

                //se non siamo in amministrazione e l'utente corrente non può visualizzare le ragioni con diritto di cessione diritti, le elimino dal result delle rag. trasm.
                if (!string.IsNullOrEmpty(tipo_trasm))
                {
                    DocsPaWR.Ruolo role = RoleManager.GetRoleInSession();
                    DocsPaWR.Funzione[] funz = role.funzioni;
                    bool cedi_diritti = false;
                    string cedi_diritti_str = string.Empty;
                    if (tipo_trasm.Equals("D"))
                        cedi_diritti_str = "ABILITA_CEDI_DIRITTI_DOC";
                    else if (tipo_trasm.Equals("F"))
                        cedi_diritti_str = "ABILITA_CEDI_DIRITTI_FASC";
                    foreach (DocsPaWR.Funzione f in funz)//verifico se attiva la microfunz ABILITA_CEDI_DIRITTI_DOC/FASC
                    {
                        if (f.codice.Equals(cedi_diritti_str))
                        {
                            cedi_diritti = true;
                            break;
                        }
                    }
                    if (!cedi_diritti)//se l'utente non ha attiva la microfunz abilita_cedi_diritti_doc/fasc elimino le rag di trasm con cessione diritti dal result 
                    {
                        ArrayList r = new ArrayList();
                        ArrayList resultNotCessione = new ArrayList();
                        r.InsertRange(0, result);

                        foreach (DocsPaWR.RagioneTrasmissione res in r)
                        {
                            if (!res.prevedeCessione.Equals("R") && (!res.prevedeCessione.Equals("W")))
                                resultNotCessione.Add(res);
                        }
                        result = new DocsPaWR.RagioneTrasmissione[resultNotCessione.Count];
                        resultNotCessione.CopyTo(result);
                    }
                }
				return result;
			} 
			catch
			{
				return null;
			} 
		
			
		}

        /// <summary>
        /// Funzione per ricerca e sostituzione di un ruolo con un altro in determinati ruoli
        /// </summary>
        /// <param name="roleToReplace">Ruolo da restituire</param>
        /// <param name="newRole">Ruolo con cui sostituire</param>
        /// <param name="operation">Operazione da compiere</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <param name="userInfo">Informazioni sull'utente</param>
        /// <param name="isAdmin">True se l'utente richiedente è un amministratore</param>
        /// <param name="models">Collection dei modelli</param>
        /// <param name="copyNotes">True se bisogna copiare le note</param>
        /// <returns>Risultato dell'elaborazione</returns>
        public static FindAndReplaceResponse FindAndReplaceRolesInModelliTrasmissione(ElementoRubrica roleToReplace, ElementoRubrica newRole, FindAndReplaceEnum operation, FiltroRicerca[] searchFilters, InfoUtente userInfo, bool isAdmin, ModelloTrasmissioneSearchResult[] models, bool copyNotes)
        {
            // Costruzione della request
            FindAndReplaceRequest request = new FindAndReplaceRequest()
            {
                NewRole = newRole,
                Operation = operation,
                RoleToReplace = roleToReplace,
                SearchFilters = searchFilters,
                UserInfo = userInfo,
                IsAdministrator = isAdmin,
                Models = models,
                CopyNotes = copyNotes
            };

            // Esecuzione operazione
            FindAndReplaceResponse response = docsPaWS.FindAndReplaceRoleInModelliTrasmissione(request);

            // Restituzione della response
            return response;

        }

        /// <summary>
        /// Filtro di ricerca da utilizzare per la ricerca dei modelli di trasmissione
        /// </summary>
        public static FiltroRicerca[] SearchFilters
        {
            get
            {
                return (FiltroRicerca[])HttpContext.Current.Session["FindAndReplace"] as FiltroRicerca[];
            }

            set
            {
                HttpContext.Current.Session["FindAndReplace"] = value;
            }
        }


        public static bool SalvaCessioneDirittiSuModelliTrasm(ModelloTrasmissione objTrasm, Page page)
        {
            try
            {
                return docsPaWS.SalvaCessioneDirittiSuModelliTrasm(objTrasm);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static ModelloTrasmissione UtentiConNotificaTrasm(ModelloTrasmissione objModTrasm, object[] utentiDaInserire, object[] utentiDaCancellare, string operazione, Page page)
        {
            try
            {
                if (utentiDaInserire != null && utentiDaCancellare != null)
                    return docsPaWS.UtentiConNotificaTrasm(objModTrasm, utentiDaInserire, utentiDaCancellare, operazione);
                else
                    return docsPaWS.UtentiConNotificaTrasm(objModTrasm, null, null, operazione);


            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

	}
}
