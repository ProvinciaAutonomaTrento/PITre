using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;
using log4net;

namespace DocsPAWA
{
	/// <summary></summary>
	public class FascicoliManager
	{
		private static DocsPaWebService docsPaWS = ProxyManager.getWS();
        private static ILog logger = LogManager.GetLogger(typeof(FascicoliManager));
		public static InfoFascicolo getInfoFascicoloDaFascicolo (Fascicolo fasc,Page page) 
		{
			InfoFascicolo infoFasc = new InfoFascicolo();
		
			infoFasc.idFascicolo = fasc.systemID;
			infoFasc.descrizione = fasc.descrizione;
			infoFasc.idClassificazione = fasc.idClassificazione;
			infoFasc.codice=fasc.codice;
			// non so dove trova queste info ?
			// infoFasc.codRegistro =
			// infoFasc.idRegistro = 	
			
			if(fasc.stato != null && fasc.stato.Equals("A"))
			{
				infoFasc.apertura=fasc.apertura;
			}
			

			return infoFasc;
		}

		public static DocsPAWA.DocsPaWR.Fascicolo getFascicoloDaCodice(Page page,string codiceFasc,DocsPaWR.InfoUtente infoUtente){
			
			
			try 
			{			
				bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
				&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo result = docsPaWS.FascicolazioneGetFascicoloDaCodice(infoUtente, codiceFasc, UserManager.getRegistroSelezionato(page), enableUfficioRef, enableProfilazione);

				if(result == null)
				{
					//throw new Exception();
				}

				return result;
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

		public static DocsPAWA.DocsPaWR.Fascicolo getListaFascicoliDaCodice(Page page,string codiceFasc,DocsPaWR.InfoUtente infoUtente)
		{		
			try 
			{			
				bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
					&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo result = docsPaWS.FascicolazioneGetFascicoloDaCodice(infoUtente, codiceFasc, UserManager.getRegistroSelezionato(page), enableUfficioRef, enableProfilazione);

				if(result == null)
				{
					//throw new Exception();
				}

				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}


		public static InfoDocumento[] getListaDocumenti(Page page, Folder folder)
		{	
			try 
			{				
				//InfoDocumento[] result = docsPaWS.FascicolazioneGetDocumenti(folder,UserManager.getInfoUtente(page));
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				InfoDocumento[] result = docsPaWS.FascicolazioneGetDocumenti(infoUtente.idGruppo, infoUtente.idPeople, folder);
				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
			
		}


		#region paginazione getListaDocumenti

		public static InfoDocumento[] getListaDocumentiPaging(
						Page page, 
						Folder folder,
						DocsPaWR.FiltroRicerca[][] filtriRicerca,
						int numPage,
						out int numTotPage,
						out int nRec,
                        bool compileIdProfileList,
                        out SearchResultInfo[] idProfiles)
		{
			nRec=0;
			numTotPage=0;

            SearchResultInfo[] idProfileList= null;

			InfoDocumento[] retValue=null;

			try 
			{				
				InfoUtente infoUtente = UserManager.getInfoUtente(page);

				if (filtriRicerca==null)
					retValue=docsPaWS.FascicolazioneGetDocumentiPaging(infoUtente.idGruppo, infoUtente.idPeople, folder,numPage, compileIdProfileList, out numTotPage, out nRec, out idProfileList);
				else
					retValue=docsPaWS.FascicolazioneGetDocumentiPagingWithFilters(infoUtente.idGruppo,infoUtente.idPeople,folder,filtriRicerca,numPage, compileIdProfileList, out numTotPage,out nRec, out idProfileList);
				
				if(retValue==null)
					throw new Exception();
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

            idProfiles = idProfileList;
			return retValue;
		}

		public static InfoDocumento[] getListaDocumentiPaging(Page page, Folder folder,int numPage,out int numTotPage,out int nRec, bool compileIdProfileList, out SearchResultInfo[] idProfiles)
		{
            SearchResultInfo[] idProfileList = null;
            InfoDocumento[] toreturn = getListaDocumentiPaging(page,folder,null,numPage, out numTotPage,out nRec, compileIdProfileList, out idProfileList);
            idProfiles = idProfileList;
            return toreturn;
		}
		#endregion


		public static DocsPAWA.DocsPaWR.FascicolazioneClassificazione[] fascicolazioneGetTitolario(Page page) 
		{
			try  
			{
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				Registro registro = UserManager.getRegistroSelezionato(page);
			
				DocsPaWR.FascicolazioneClassificazione[] result = docsPaWS.FascicolazioneGetTitolario(infoUtente.idAmministrazione,infoUtente.idGruppo,infoUtente.idPeople,registro,null,true);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}


		public static DocsPAWA.DocsPaWR.FascicolazioneClassificazione[] fascicolazioneGetTitolario(Page page,string codClassifica,bool getFigli) 
		{
			try  
			{
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				Registro registro = UserManager.getRegistroSelezionato(page);
			
				DocsPaWR.FascicolazioneClassificazione[] result = docsPaWS.FascicolazioneGetTitolario(infoUtente.idAmministrazione,infoUtente.idGruppo,infoUtente.idPeople,registro,codClassifica,getFigli);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

        public static DocsPAWA.DocsPaWR.FascicolazioneClassificazione[] fascicolazioneGetTitolario2(Page page, string codClassifica, bool getFigli, string idTitolario)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Registro registro = UserManager.getRegistroSelezionato(page);

                DocsPaWR.FascicolazioneClassificazione[] result = docsPaWS.FascicolazioneGetTitolario2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, codClassifica, getFigli, idTitolario);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static DocsPAWA.DocsPaWR.FascicolazioneClassificazione[] fascicolazioneGetTitolario2(Page page, string codClassifica, DocsPAWA.DocsPaWR.Registro regNodo, bool getFigli, string idTitolario)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);

                DocsPaWR.FascicolazioneClassificazione[] result = docsPaWS.FascicolazioneGetTitolario2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, regNodo, codClassifica, getFigli, idTitolario);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

		public static DocsPAWA.DocsPaWR.FascicolazioneClassificazione[] fascicolazioneGetTitolario(Page page,string codClassifica, DocsPAWA.DocsPaWR.Registro regNodo, bool getFigli) 
		{
			try  
			{
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
		
				DocsPaWR.FascicolazioneClassificazione[] result = docsPaWS.FascicolazioneGetTitolario(infoUtente.idAmministrazione,infoUtente.idGruppo,infoUtente.idPeople,regNodo,codClassifica,getFigli);

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



		public static string getCodiceUltimoFascicolo(Page page,FascicolazioneClassificazione classificazione)
		{
			try  
			{
				return classificazione.codUltimo;
			} 
			catch(Exception e) 
			{
				ErrorManager.redirect(page, e);
			}
			return null;
		}
		
		public static Fascicolo getFascicolo(Page page, string idGruppo,string idPeople, InfoFascicolo info)
		{
			try 
			{				
				
				bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
					&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
		
				Fascicolo result = docsPaWS.FascicolazioneGetDettaglioFascicolo(UserManager.getInfoUtente(), info, enableUfficioRef);

				if(result == null)
				{
                    if (page.AppRelativeVirtualPath.EndsWith("toDoList.aspx"))
                        return null;
                    else
                    {
                        //Commentato per gestione rimozioni diritti di visibilita del fascicolo
                        //throw new Exception();	
                        //page.Response.Write("<script>window.showModalDialog('../popup/errorPage.aspx?tipo=F','','dialogWidth:510px;dialogHeight:100px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ 100 +';left:'+100)</script>");
                        //return null;
                        string errorMessage = "Sono stati tolti i diritti di visibilità per questo fascicolo.\\nNon è più possibile visualizzarlo.";
                        page.Response.Write("<script>alert('" + errorMessage + "');</script>");
                        // Redirect alla homepage di docspa
                        SiteNavigation.CallContextStack.Clear();
                        SiteNavigation.NavigationContext.RefreshNavigation();
                        string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
                        page.Response.Write(script);

                    }
				}

				return result;
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			}
			catch(Exception es) 
			{
                return null;
				//ErrorManager.redirect(page, es);
			}

			return null;
		}


        public static Fascicolo[] getListaFascicoli(Page page, FascicolazioneClassificazione classificazione, FiltroRicerca[] filtriRicerca, bool childs, byte[] datiExcel)
        {
            try
            {
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Fascicolo[] result = docsPaWS.FascicolazioneGetListaFascicoli(infoUtente, classificazione, filtriRicerca, enableUfficioRef, enableProfilazione, childs, datiExcel);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			}
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static Fascicolo[] getListaFascicoli(Page page, FascicolazioneClassificazione classificazione, FiltroRicerca[] filtriRicerca, bool childs, DocsPaWR.Registro registro, byte[] datiExcel)
        {
            try
            {
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Fascicolo[] result = docsPaWS.FascicolazioneGetAllListaFasc(infoUtente, classificazione, filtriRicerca, enableUfficioRef, enableProfilazione, childs, registro, datiExcel);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }

            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

		
		public static Fascicolo[] getListaFascicoliPaging(Page page,FascicolazioneClassificazione classificazione, DocsPAWA.DocsPaWR.Registro registro, FiltroRicerca[] filtriRicerca,  bool childs,int requestedPage,out int numTotPage,out int nRec,int pageSize,
            bool getSystemIdList, out SearchResultInfo[] idProjectList, byte[] datiExcel)
		{
			nRec = 0;
			numTotPage = 0;

            // Lista dei system id dei fascicoli
            SearchResultInfo[] idProjects = null;

			try 
			{			
				bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
					&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
		
                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

				InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Fascicolo[] result = docsPaWS.FascicolazioneGetListaFascicoliPaging(infoUtente, classificazione, registro, filtriRicerca,
                                                enableUfficioRef, enableProfilazione, childs, requestedPage, pageSize, getSystemIdList, datiExcel, out numTotPage, out nRec, out idProjects);
              
                if(result == null)
				{
					throw new Exception();
				}

                // Salvataggio della lista dei system id dei fascicoli
                idProjectList = idProjects;

				return result;
			} 
				//			catch(System.Web.Services.Protocols.SoapException es) 
				//			{
				//				ErrorManager.redirect(page, es);
				//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

            idProjectList = null;

			return null;
		}

        public static int getGrdFascicoliPageSize(Page page)
        {
            int num = 10;
            string val = System.Configuration.ConfigurationManager.AppSettings["GRD_FASCICOLI_PAGE_SIZE"];
            return (val != null && Int32.TryParse(val, out num)) ? num : 10;
        }

		public static Fascicolo getFascicolo(Page page,string idFascicolo)
		{
			try 
			{				
				InfoUtente infoUtente = UserManager.getInfoUtente(page);	
				Fascicolo result = docsPaWS.FascicolazioneGetFascicoloById(idFascicolo, infoUtente);

                if (result == null)
                {
                    string errorMessage = string.Empty;

                    //verificata ACL, ritorna semmai un msg in out errorMessage
                    int rtn = docsPaWS.VerificaACL("F", idFascicolo, infoUtente, out errorMessage);
                    if (rtn == -1)
                        errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";
                    
                    if (String.IsNullOrEmpty(errorMessage))
                        errorMessage = "Attenzione, non è stato possibile recuperare i dati del documento richiesto.\\nConsultare il log per maggiori dettagli.";

                    page.Response.Write("<script>alert('" + errorMessage + "');</script>");
                    // Redirect alla homepage di docspa
                    SiteNavigation.CallContextStack.Clear();
                    SiteNavigation.NavigationContext.RefreshNavigation();
                    string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
                    page.Response.Write(script);
                    
                }

				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

        public static DocsPAWA.DocsPaWR.Fascicolo getFascicoloDaCodice2(Page page, string codFascicolo, string idTitolario)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo result = docsPaWS.FascicolazioneGetFascicoloDaCodice2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, codFascicolo, UserManager.getRegistroSelezionato(page), enableUfficioRef, enableProfilazione, idTitolario);

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static ArrayList getFascicoloDaCodice3(Page page, string codFascicolo)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                ArrayList result = new ArrayList(docsPaWS.FascicolazioneGetFascicoloDaCodice3(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, codFascicolo, UserManager.getRegistroSelezionato(page), enableUfficioRef, enableProfilazione));

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

		public static DocsPAWA.DocsPaWR.Fascicolo getFascicoloDaCodice(Page page,string codFascicolo)
		{
			try 
			{				
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
					&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo result = docsPaWS.FascicolazioneGetFascicoloDaCodice(infoUtente, codFascicolo, UserManager.getRegistroSelezionato(page), enableUfficioRef, enableProfilazione);

				if(result == null)
				{
					//throw new Exception();
				}

				return result;
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

		public static DocsPAWA.DocsPaWR.Fascicolo[] getListaFascicoliDaCodice(Page page,string codFascicolo, DocsPAWA.DocsPaWR.Registro reg, string insRic)
		{
			try 
			{				
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
					&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

                DocsPaWR.Fascicolo[] result = docsPaWS.FascicolazioneGetListaFascicoliDaCodice(infoUtente, codFascicolo, reg, enableUfficioRef, enableProfilazione, insRic);

				if(result == null)
				{
					//throw new Exception();
				}

				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

     

      /// <summary>
      /// 
      /// </summary>
      /// <param name="page"></param>
      /// <param name="codFascicolo"></param>
      /// <param name="descrFolder"></param>
      /// <param name="reg"></param>
      /// <returns></returns>
      public static DocsPAWA.DocsPaWR.Folder[] getListaFolderDaCodiceFascicolo(Page page,string codFascicolo,string descrFolder, DocsPAWA.DocsPaWR.Registro reg)
		   {
			   try 
			   {				
				   InfoUtente infoUtente = UserManager.getInfoUtente(page);
				   bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
					   && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                   bool enableProfilazione = false;
                   if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                       enableProfilazione = true;

                   DocsPaWR.Folder[] result = docsPaWS.FascicolazioneGetListaFolderDaCodice(infoUtente, codFascicolo,descrFolder, reg, enableUfficioRef, enableProfilazione);

				   if(result == null)
				   {
					   //throw new Exception();
				   }

				   return result;
			   } 
			   catch(Exception es) 
			   {
				   ErrorManager.redirect(page, es);
			   }

			   return null;
		   }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="page"></param>
      /// <param name="codFascicolo"></param>
      /// <param name="descrFolder"></param>
      /// <param name="reg"></param>
      /// <returns></returns>
      public static DocsPAWA.DocsPaWR.Folder[] getListaFolderDaIdFascicolo(Page page, string idFascicolo, DocsPAWA.DocsPaWR.Registro reg)
      {
         try
         {
            InfoUtente infoUtente = UserManager.getInfoUtente(page);
            bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
               && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

            bool enableProfilazione = false;
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
               enableProfilazione = true;

            DocsPaWR.Folder[] result = docsPaWS.FascicolazioneGetListaFolderDaIdFascicolo(infoUtente, idFascicolo, reg, enableUfficioRef, enableProfilazione);

            if (result == null)
            {
               //throw new Exception();
            }

            return result;
         }
         catch (Exception es)
         {
            ErrorManager.redirect(page, es);
         }

         return null;
      }


      public static Fascicolo getFascicoloById(Page page, string idFascicolo)
      { 
         DocsPaWR.Fascicolo result = null;
			
			try 
			{
            InfoUtente infoUtente = UserManager.getInfoUtente(page);

            result = docsPaWS.FascicolazioneGetFascicoloById(idFascicolo, infoUtente);
         }
         catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 

			return result;

      }


		public static Fascicolo getFascicoloInClassifica(Page page,string codFascicolo, string idRegistro, string idTitolario,string systemId)
		{
			DocsPaWR.Fascicolo result = null;
			
			try 
			{				
				
				InfoUtente infoUtente = UserManager.getInfoUtente(page);

				bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
					&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;

				result = docsPaWS.FascicolazioneGetFascicoloInClassifica(infoUtente, codFascicolo, idRegistro, enableUfficioRef, idTitolario,enableProfilazione, systemId);
				
				if(result == null)
				{
					throw new Exception();
				}
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 

			return result;
		}

		public static InfoDocumento[] getDocFascByFolder(string idGruppo, string idPeople, Page page,Folder fold) 
		{
			try 
			{				
				InfoDocumento[] result = docsPaWS.FascicolazioneGetDocumenti(idGruppo, idPeople, fold);
				
				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

        public static DocsPAWA.DocsPaWR.FascicoloDiritto[] getListaVisibilita(Page page, DocsPaWR.InfoFascicolo infoFasc, bool cercaRimossi, string rootFolder)
		{
			//docWS.getQuery(xmlQueryV,xmlSafe);
			try 
			{
				DocsPaWR.FascicoloDiritto[] FascDir=docsPaWS.FascicolazioneGetVisibilita(infoFasc, cercaRimossi, rootFolder);

				if(FascDir == null)
				{
					throw new Exception();
				}

				return FascDir;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 
			return null;
		}

		public static void sospendiRiattiva(Page page,string idPeople,DocsPaWR.Corrispondente corr,DocsPaWR.Fascicolo fasc){
			try
			{
				bool result = docsPaWS.FascicolazioneSospendiRiattivaUtente(idPeople,corr,fasc);

				if(!result)
				{
					throw new Exception();
				}
			}
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 
		}

		public static string decodeStatoFasc(Page page,string shortStato)
		{
			try
			{
				string str="";
				switch(shortStato)
				{
					case "A":
						str="APERTO";
						break;
					case "C":
						str="CHIUSO";
						break;
				}
				return str;
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			return null;
		}

		public static DocsPAWA.DocsPaWR.Fascicolo[] getFascicoliInAreaLavoro(Page page)
		{
			try
			{
				Utente utente = UserManager.getUtente(page);
				Ruolo ruolo = UserManager.getRuolo(page);
				DocsPaWR.AreaLavoroTipoOggetto tipoObj = DocsPAWA.DocsPaWR.AreaLavoroTipoOggetto.FASCICOLO; 
				DocsPaWR.AreaLavoroTipoFascicolo tipoFasc = DocsPAWA.DocsPaWR.AreaLavoroTipoFascicolo.TUTTI; //.PROCEDIMENTALE;
				DocsPaWR.AreaLavoroTipoDocumento tipoDoc = new DocsPAWA.DocsPaWR.AreaLavoroTipoDocumento();

				//perchè è vuoto ?
				string idRegistro = String.Empty;
				//nuovo
				DocsPaWR.Registro registroSel = (DocsPAWA.DocsPaWR.Registro)UserManager.getRegistroSelezionato(page);
				if(registroSel!=null)
				{
					idRegistro = registroSel.systemId;
				}

				bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
					&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
		
				DocsPaWR.AreaLavoro areaLavoro=docsPaWS.DocumentoGetAreaLavoro(utente, ruolo, tipoObj, tipoDoc, tipoFasc, idRegistro, enableUfficioRef);

				if(areaLavoro == null)
				{
					throw new Exception();
				}

				DocsPaWR.Fascicolo[] retValue=new DocsPAWA.DocsPaWR.Fascicolo[areaLavoro.lista.Length];
				areaLavoro.lista.CopyTo(retValue,0);

				return retValue;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}
		
		public static string encodeStatoFasc(Page page,string longStato)
		{
			try
			{
				string str="";
				switch(longStato)
				{
					case "APERTO":
						str="A";
						break;
					case "CHIUSO":
						str="C";
						break;
				}
				return str;
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			return null;
		}

		
		public static string decodeTipoFasc(Page page,string shortTipo)
		{
			try
			{
				string str="";
				switch(shortTipo)
				{
					case "G":
						str="GENERALE";
						break;
					case "P":
						str="PROCEDIMENTALE";
						break;
				}
				return str;
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			return null;
		}

		public static string encodeTipoFasc(Page page,string longTipo)
		{
			try 
			{				
				string str="";
				switch(longTipo)
				{
					case "GENERALE":
						str="G";
						break;
					case "PROCEDURALE":
						str="P";
						break;
				}
				return str;
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			return null;
		}

		public static string[] getListaTipoFascicolo(Page page)
		{
			try 
			{				
				string[] listaTipoFasc=new string[2];
				listaTipoFasc[0]="G";
				listaTipoFasc[1]="P";
				return listaTipoFasc;
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			return null;
		}

		public static void setFascicolo(Page page, ref Fascicolo fasc) 
		{
			try 
			{				
				bool result = docsPaWS.FascicolazioneSetFascicolo(UserManager.getInfoUtente(), ref fasc);

				if(!result)
				{
					throw new Exception();
				}
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}			
		}

		public static DocsPAWA.DocsPaWR.FascicolazioneClassifica[] getGerarchiaDaCodice(Page page,string codClassificazione,string idAmm)
		{
			try 
			{				
				DocsPaWR.FascicolazioneClassifica[] result = docsPaWS.FascicolazioneGetGerarchiaDaCodice(codClassificazione, UserManager.getRegistroSelezionato(page),idAmm);

				if(result == null)
				{
				//	throw new Exception();
				}

				return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

        public static DocsPAWA.DocsPaWR.FascicolazioneClassifica[] getGerarchiaDaCodice2(Page page, string codClassificazione, string idAmm, string idTitolario)
        {
            try
            {
                DocsPaWR.FascicolazioneClassifica[] result = docsPaWS.FascicolazioneGetGerarchiaDaCodice2(codClassificazione, UserManager.getRegistroSelezionato(page), idAmm, idTitolario);

                if (result == null)
                {
                    //	throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

		public static DocsPAWA.DocsPaWR.FascicolazioneClassifica[] getGerarchia(Page page,string idClassificazione,string idAmm)
		{
			try 
			{				
				DocsPaWR.FascicolazioneClassifica[] result = docsPaWS.FascicolazioneGetGerarchia(idClassificazione,idAmm);

                return result;
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

		public static DocsPAWA.DocsPaWR.FileDocumento reportTitolario(Page page, DocsPAWA.DocsPaWR.Registro registro)
		{
			try 
			{	
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				DocsPaWR.FileDocumento result = docsPaWS.ReportTitolario(infoUtente, registro);

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="schedaDoc"></param>
		/// <returns></returns>
		public static DocsPAWA.DocsPaWR.RagioneTrasmissione TrasmettiFascicoloToUoReferente(Ruolo ruolo, out bool verificaRagioni) 
		{
			DocsPaWR.RagioneTrasmissione ragTrasm = null;
			verificaRagioni = true;

			try 
			{
				ragTrasm = docsPaWS.GetRagioneTrasmissione("REFERENTE",ruolo, out verificaRagioni);					
			} 
			catch(Exception) 
			{
				ragTrasm = null;
			} 
			
			return ragTrasm;
		}


		public static DocsPAWA.DocsPaWR.FileDocumento reportFascette(Page page, string codiceFacicolo, DocsPAWA.DocsPaWR.Registro registro) 
		{
			try 
			{
				if(codiceFacicolo.Equals(""))
					//page.RegisterStartupScript("init", "<script>alert('Inserire il codice del fascicolo');</script>");
                    page.ClientScript.RegisterStartupScript(page.GetType(), "init", "<script>alert('Inserire il codice del fascicolo');</script>");
				else 
				{
					Fascicolo fascicolo = getFascicoloDaCodice(page, codiceFacicolo);
				
					if(fascicolo != null)
					{
						return docsPaWS.reportFascetteFascicolo(fascicolo, UserManager.getInfoUtente(page));
					}
					else
					{
						//page.RegisterStartupScript("init", "<script>alert('Fascicolo non trovato');</script>");
                        page.ClientScript.RegisterStartupScript(page.GetType(), "init", "<script>alert('Fascicolo non trovato');</script>");
					}
				}
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}

			return null;
		}

		public static DocsPAWA.DocsPaWR.FascicolazioneClassifica[] GetFigliClassifica(Page page,DocsPaWR.FascicolazioneClassifica classifica,string idAmm)
		{
			try 
			{				
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				Registro infoRegistro = UserManager.getRegistroSelezionato(page);
				string idRegistro=null;
				if (infoRegistro!=null)
					idRegistro = infoRegistro.systemId;
				DocsPaWR.FascicolazioneClassifica[] result = docsPaWS.FascicolazioneGetFigliClassifica(infoUtente.idGruppo,infoUtente.idPeople,classifica,idRegistro,idAmm);

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

        public static DocsPAWA.DocsPaWR.FascicolazioneClassifica[] GetFigliClassifica2(Page page, DocsPaWR.FascicolazioneClassifica classifica, string idAmm, string idTitolario)
        {
            try
            {
                //modifica per la funzione di riclassificazione in inps
                if (docsPaWS.isFiltroAooEnabled())
                {
                    DocsPaWR.Registro[] reg = UserManager.getListaRegistri(page);
                    DocsPaWR.Registro regCorrente = new DocsPaWR.Registro();
                    if (UserManager.getRegistroSelezionato(page) != null)
                    {
                        string IdRegistroSelezionato = UserManager.getRegistroSelezionato(page).systemId;
                        for (int i = 0; i < reg.Length; i++)
                        {
                            if (reg[i].systemId.Equals(IdRegistroSelezionato))
                                regCorrente = reg[i];

                        }
                        //if (reg[0].systemId != UserManager.getRegistroSelezionato(this.Page).systemId)
                        if (reg[0].systemId != IdRegistroSelezionato)
                            UserManager.setRegistroSelezionato(page, regCorrente);
                        //UserManager.setRegistroSelezionato(this.Page, reg[0]);
                    }
                }

                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Registro infoRegistro = UserManager.getRegistroSelezionato(page);
                string idRegistro = null;
                if (infoRegistro != null)
                    idRegistro = infoRegistro.systemId;
                DocsPaWR.FascicolazioneClassifica[] result = docsPaWS.FascicolazioneGetFigliClassifica2(infoUtente.idGruppo, infoUtente.idPeople, classifica, idRegistro, idAmm, idTitolario);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

		public static void addFascicoloInAreaDiLavoro(Page page,DocsPaWR.Fascicolo fascicolo)
		{
			try 
			{	
				
				bool result = docsPaWS.DocumentoExecAddLavoro(null, null, fascicolo, UserManager.getInfoUtente(page),(fascicolo.idRegistroNodoTit!=null?fascicolo.idRegistroNodoTit:""));

				if(!result)
				{
					throw new Exception();
				}
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			} 
		}

        public static void eliminaFascicoloDaAreaDiLavoro(Page page, DocsPaWR.Fascicolo fascicolo)
        {
            try
            {
                Utente utente = UserManager.getUtente(page);
                Ruolo ruolo = UserManager.getRuolo(page);
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                bool result = docsPaWS.DocumentoCancellaAreaLavoro(infoUtente.idPeople, infoUtente.idCorrGlobali, null, fascicolo);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            } 
        }

        public static DocsPAWA.DocsPaWR.ValidationResultInfo deleteDocFromFolder(Page page, DocsPaWR.Folder folder, string idProfile, string fascRapida, out string msg)
        {
            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;
            msg = string.Empty;
            try
            {
                result = docsPaWS.FascicolazioneDeleteDocFromFolder(UserManager.getInfoUtente(), idProfile, folder, fascRapida, out msg);

            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }

            return result;
        }

        public static DocsPAWA.DocsPaWR.ValidationResultInfo deleteDocFromProject(Page page, DocsPaWR.Folder folder, string idProfile, string fascRapida, DocsPaWR.Fascicolo fasc, out string msg)
        {
            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;
            msg = string.Empty;
            try
            {
                result = docsPaWS.FascicolazioneDeleteDocFromProject(UserManager.getInfoUtente(), idProfile, folder, fascRapida, fasc, out msg);

            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }

            return result;
        }

        #region new,del Fascicolo by titolario
		public static Fascicolo newFascicolo(Page page,FascicolazioneClassificazione classificazione,Fascicolo fascicolo, out DocsPAWA.DocsPaWR.ResultCreazioneFascicolo resultCreazione) 
		{
            logger.Info("BEGIN");
			resultCreazione =DocsPaWR.ResultCreazioneFascicolo.OK;
			try 
			{				
				DocsPaWR.InfoUtente infoutente=UserManager.getInfoUtente(page);
				DocsPaWR.Ruolo ruolo=UserManager.getRuolo(page);
				bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
					&& ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                //ABBATANGELI GIANLUIGI - Se non trovo la chiave di configurazione CODICE_APPLICAZIONE, imposto il codice applicazione di default a ____
                if (string.IsNullOrEmpty(fascicolo.codiceApplicazione))
                    fascicolo.codiceApplicazione = (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]);

                //ABBATANGELI GIANLUIGI - TEST
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                Fascicolo result = docsPaWS.FascicolazioneNewFascicolo(classificazione, fascicolo, infoutente, ruolo, enableUfficioRef, out resultCreazione);

//				if(result == null)
//				{
//					throw new Exception("Impossibile Creare Fascicolo");
//				}
                logger.Info("END");
				return result;
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es, "Fascicolo");
			}

			return null;
		} 


        //public static Fascicolo newFascicoloLF(Page page,FascicolazioneClassificazione classificazione,Fascicolo fascicolo,bool LF, string idUoLF, string dtaLF) 
        //{
        //    try 
        //    {				
        //        DocsPaWR.InfoUtente infoutente=UserManager.getInfoUtente(page);
        //        DocsPaWR.Ruolo ruolo=UserManager.getRuolo(page);
				
        //        bool enableUfficioRef =(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF)!= null 
        //            && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
		
        //        Fascicolo result = docsPaWS.FascicolazioneNewFascicoloLF(classificazione,fascicolo,infoutente,ruolo,LF,idUoLF,dtaLF, enableUfficioRef);

        //        if(result == null)
        //        {
        //            throw new Exception("Impossibile Creare Fascicolo");
        //        }

        //        return result;
        //    } 
        //        //			catch(System.Web.Services.Protocols.SoapException es) 
        //        //			{
        //        //				ErrorManager.redirect(page, es);
        //        //			}
        //    catch(Exception es) 
        //    {
        //        ErrorManager.redirect(page, es);
        //    }

        //    return null;
        //}
		
		public static void delFascicolo(Page page,Fascicolo fascicolo) 
		{
			try 
			{	
				//InfoUtente infoUtente = UserManager.getInfoUtente(page);
				//docsPaWS.fascicolazioneNewFascicolo(fascicolo);
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			
		}
# endregion


# region new,get,del folder by Fascicolo
      public static Folder getFolder(Page page, string idFasc, string idFolder)
      {
         try
         {
            InfoUtente infoUtente = UserManager.getInfoUtente(page);

            Folder result = docsPaWS.FascicolazioneGetPrimoChildFolder(infoUtente.idPeople, infoUtente.idGruppo, idFasc, idFolder);
            if (result == null)
            {
               throw new Exception();
            }

            return result;
         }
         catch (System.Web.Services.Protocols.SoapException es)
         {
            ErrorManager.redirect(page, es);
         }
         return null;
      }



		public static Folder getFolder(Page page,Fascicolo fasc) 
		{
			try 
			{				
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				Folder result = docsPaWS.FascicolazioneGetFolder(infoUtente.idPeople, infoUtente.idGruppo, fasc);

				if(result == null)
				{
					throw new Exception();
				}

				return result;
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			return null;
		}

        /* ABBATANGELI GIANLUIGI
         * Nuova funzione che restituisce la cartella ed i suoi figli */
        public static Folder getFolder(Page page, Folder fold)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Folder result = docsPaWS.FascicolazioneGetFolderAndChild(infoUtente.idPeople, infoUtente.idGruppo, fold);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static Folder getFolder(Page page, string idFolder)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                Folder result = docsPaWS.FascicolazioneGetFolderById(infoUtente.idPeople, infoUtente.idGruppo, idFolder);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

      public static Folder[] getFolderByDescrizione(Page page, string idFascicolo, string descFolder)
      {
         Folder[] result = null;
          try 
			{				
				InfoUtente infoUtente = UserManager.getInfoUtente(page);

            result = docsPaWS.FascicolazioneGetFolderByDescr(infoUtente.idPeople, infoUtente.idGruppo, idFascicolo, descFolder);
           
				

				return result;
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
            return null;
			}
          
      
      }
		/// <summary>
		/// Restituzione di un array di oggetti "Folder",
		/// appartenenti a tutti i fascicoli, 
		/// che contengono il documento fornito in ingresso
		/// </summary>
		/// <param name="systemIdDocumento">DocNumber del documento</param>
		/// <returns></returns>
		public static Folder[] GetFoldersDocument(Page page,string systemIdDocumento)
		{
			Folder[] retValue=null;

			try 
			{				
				retValue=docsPaWS.FascicolazioneGetFoldersDocument(systemIdDocumento);

				if(retValue==null)
				{
					throw new Exception();
				}
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			return retValue;
		}

		/// <summary>
		/// Restituzione di un array di oggetti "Folder",relativamente ad un fascicolo,
		/// che contengono il documento fornito in ingresso 
		/// </summary>
		/// <param name="systemIdDocumento">DocNumber del documento</param>
		/// <param name="systemIdFascicolo">ID del fascicolo contenente i folders</param>
		/// <returns></returns>
		public static Folder[] GetFoldersDocument(Page page,string systemIdDocumento,string systemIdFascicolo)
		{
			Folder[] retValue=null;

			try 
			{				
				retValue=docsPaWS.FascicolazioneGetFoldersDocumentFascicolo(systemIdDocumento,systemIdFascicolo);

				if(retValue==null)
				{
					throw new Exception();
				}
			} 
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(page, es);
			}
			return retValue;
		}
		

		public static void delFolder(Page page,Folder fold) 
		{
			try 
			{	
				InfoUtente infoUtente = UserManager.getInfoUtente(page);
				bool result = docsPaWS.FascicolazioneDelFolder(fold, infoUtente);

				if(!result)
				{
					throw new Exception();
				}
			} 
//			catch(System.Web.Services.Protocols.SoapException es) 
//			{
//				ErrorManager.redirect(page, es);
//			}
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}			
		}

        /// <summary>
        /// Creazione nuovo folder
        /// </summary>
        /// <param name="page"></param>
        /// <param name="folder"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="result"></param>
        /// <returns></returns>
		public static bool newFolder(Page page, ref Folder folder, InfoUtente infoUtente, Ruolo ruolo, out DocsPaWR.ResultCreazioneFolder result) 
		{
            result = ResultCreazioneFolder.GENERIC_ERROR;

			try 
			{
                folder = docsPaWS.FascicolazioneNewFolder(folder, infoUtente, ruolo, out result);
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

            return (result == ResultCreazioneFolder.OK);
		}

		public static void updateFolder(Page page, Folder folder) 
		{
			try 
			{				
				bool result = docsPaWS.FascicolazioneModifyFolder(UserManager.getInfoUtente(), folder);

				if(!result)
				{
					throw new Exception();
				}
			} 
			catch(Exception es) 
			{
				ErrorManager.redirect(page, es);
			}

			return;
		}

# endregion

#region Variabile sessione "FolderSelezionato"
		//Utilizzata per memorizzare l'hashtable contenente
		//le informazioni sui titolari relativi al registro
		//selezionato
		public static void removeFolderSelezionato(Page page) 
		{
            removeFolderSelezionato();
		}

        public static void removeFolderSelezionato()
        {
            HttpContext.Current.Session.Remove("fascDocumenti.FolderSel");
        }

		public static DocsPAWA.DocsPaWR.Folder getFolderSelezionato(Page page)
		{
			return (DocsPAWA.DocsPaWR.Folder)page.Session["fascDocumenti.FolderSel"];
		}

        public static DocsPAWA.DocsPaWR.Folder getFolderSelezionato()
        {
            return (DocsPAWA.DocsPaWR.Folder)GetSessionValue("fascDocumenti.FolderSel");
        }

		public static void setFolderSelezionato(Page page,DocsPaWR.Folder folder)
		{
			page.Session["fascDocumenti.FolderSel"]=folder;
		}		
#endregion

#region Variabile sessione "HashTableListaFolder"
		//Utilizzata per memorizzare l'hashtable contenente
		//le informazioni sui titolari relativi al registro
		//selezionato
		public static void removeHashFolder(Page page) 
		{
			page.Session.Remove("fascDocumenti.HashFolder");
		}

		public static Hashtable getHashFolder(Page page)
		{
			return (Hashtable)page.Session["fascDocumenti.HashFolder"];
		}

		public static void setHashFolder(Page page,Hashtable folders)
		{
			page.Session["fascDocumenti.HashFolder"]=folders;
		}		
#endregion
	
#region Variabile sessione "HashTableListaFascicoli"
		//Utilizzata per memorizzare l'hashtable contenente
		//i Fascicoli caricati in griglia
		public static void removeHashFascicoli(Page page) 
		{
			page.Session.Remove("docClassifica.HashFascicoli");
		}

		public static Hashtable getHashFascicoli(Page page)
		{
			return (Hashtable)page.Session["docClassifica.HashFascicoli"];
		}

		public static void setHashFascicoli(Page page,Hashtable fascicoli)
		{
			page.Session["docClassifica.HashFascicoli"]=fascicoli;
		}

        public static void setHashFascicoliSelezionati(Page page, Hashtable fascicoliSelezionati)
        {
            page.Session["docClassifica.HashFascicoliSelezionati"] = fascicoliSelezionati;
        }

        public static Hashtable getHashFascicoliSelezionati(Page page)
        {

            return page.Session["docClassifica.HashFascicoliSelezionati"] as Hashtable;
        }

        public static void removeHashFascicoliSelezionati(Page page)
        {
            page.Session.Remove("docClassifica.HashFascicoliSelezionati");
        }

        public static void removeHashFascicoliADL(Page page)
        {
            page.Session.Remove("docClassifica.HashFascicoliADL");
        }

        public static Hashtable getHashFascicoliADL(Page page)
        {
            return (Hashtable)page.Session["docClassifica.HashFascicoliADL"];
        }

        public static void setHashFascicoliADL(Page page, Hashtable fascicoli)
        {
            page.Session["docClassifica.HashFascicoliADL"] = fascicoli;
        }		

#endregion

#region Variabile sessione "HashDocProtENonProt"
		//Utilizzata per memorizzare l'hashtable contenente
		//le informazioni sui titolari relativi al registro
		//selezionato
		public static void removeHashDocProtENonProt(Page page) 
		{
			page.Session.Remove("HashDocProtENonProt");
		}

		public static Hashtable getHashDocProtENonProt(Page page)
		{
			return (Hashtable)page.Session["HashDocProtENonProt"];
		}

		public static void setHashDocProtENonProt(Page page,Hashtable hashDocProtENonProt)
		{
			page.Session["HashDocProtENonProt"]=hashDocProtENonProt;
		}		
#endregion

#region Variabile sessione "DataTableDocProt"
		//Utilizzata per memorizzare la datatable contenente
		//le informazioni sui documenti protocollati relativi al fascicolo
		//selezionato
		public static void removeDataTableDocProt(Page page) 
		{
			page.Session.Remove("DataTableDocProt");
		}

		public static DataTable getDataTableDocProt(Page page)
		{
			return (DataTable)page.Session["DataTableDocProt"];
		}

		public static void setDataTableDocProt(Page page,DataTable dataTableDocProt)
		{
			page.Session["DataTableDocProt"]=dataTableDocProt;
		}		
#endregion

        #region Variabile sessione "DataTableDocDaArchiv"
        //Utilizzata per memorizzare la datatable contenente
        //le informazioni sui documenti protocollati relativi al fascicolo
        //selezionato
        public static void removeDataTableDocDaArchiv(Page page)
        {
            page.Session.Remove("DataTableDocDaArchiv");
        }

        public static ArrayList getDataTableDocDaArchiv(Page page)
        {
            return (ArrayList)page.Session["DataTableDocDaArchiv"];
        }

        public static void setDataTableDocDaArchiv(Page page, ArrayList DataTableDocDaArchiv)
        {
            page.Session["DataTableDocDaArchiv"] = DataTableDocDaArchiv;
        }
        #endregion

#region Variabile sessione "DataTableDocNonProt"
		//Utilizzata per memorizzare la datatable contenente
		//le informazioni sui documenti protocollati relativi al fascicolo
		//selezionato
		public static void removeDataTableDocNonProt(Page page) 
		{
			page.Session.Remove("DataTableDocNonProt");
		}

		public static DataTable getDataTableDocNonProt(Page page)
		{
			return (DataTable)page.Session["DataTableDocNonProt"];
		}

		public static void setDataTableDocNonProt(Page page,DataTable dataTableDocNonProt)
		{
			page.Session["DataTableDocNonProt"]=dataTableDocNonProt;
		}		
#endregion

#region Variabile sessione "AllClassValue"
		public static void removeAllClassValue(Page page) 
		{
			RemoveSessionValue("AllClassValue");
		}

		public static bool getAllClassValue(Page page)
		{
			bool retValue;
			try
			{				
				retValue = (bool) GetSessionValue("AllClassValue");
			}
			catch
			{
				retValue = false;
			}
			return retValue;
		}

		public static void setAllClassValue(Page page,bool allClassValue)
		{
			SetSessionValue("AllClassValue",allClassValue);
		}		
#endregion

#region Variabile sessione "DescrizioneClassificazione"
		//Utilizzata per memorizzare l'hashtable contenente
		//le informazioni sui titolari relativi al registro
		//selezionato
		public static void removeDescrizioneClassificazione(Page page) 
		{
			page.Session.Remove("DescrizioneClassificazione");
		}

		public static string getDescrizioneClassificazione(Page page)
		{
			return (string)page.Session["DescrizioneClassificazione"];
		}

		public static void setDescrizioneClassificazione(Page page,string descrizioneClassificazione)
		{
			page.Session["DescrizioneClassificazione"]=descrizioneClassificazione;
		}		
#endregion

#region Variabile sessione "ListaFascicoliInGriglia"
		//Utilizzata per memorizzare l'hashtable contenente
		//le informazioni sui titolari relativi al registro
		//selezionato
		public static void removeListaFascicoliInGriglia(Page page) 
		{
			page.Session.Remove("ListaFascicoliInGriglia");
		}

		public static DocsPAWA.DocsPaWR.Fascicolo[] getListaFascicoliInGriglia(Page page)
		{
			return (DocsPAWA.DocsPaWR.Fascicolo[])page.Session["ListaFascicoliInGriglia"];
		}

		public static void setListaFascicoliInGriglia(Page page,DocsPaWR.Fascicolo[] fascicoliInGriglia)
		{
			page.Session["ListaFascicoliInGriglia"]=fascicoliInGriglia;
		}		
#endregion

#region Variabile sessione "Info Fascicolo Sel"
		//Utilizzata per memorizzare l'hashtable contenente
		//le informazioni sui titolari relativi al registro
		//selezionato

		public static void removeInfoFascicolo(Page page) 
		{
			page.Session.Remove("tabRisultatiRicTrasm.InfoFasc");
		}

		public static DocsPAWA.DocsPaWR.InfoFascicolo getInfoFascicolo(Page page)
		{
			return (DocsPAWA.DocsPaWR.InfoFascicolo)page.Session["tabRisultatiRicTrasm.InfoFasc"];
		}

		public static void setInfoFascicolo(Page page,DocsPaWR.InfoFascicolo infofascicoloSelezionato)
		{
			page.Session["tabRisultatiRicTrasm.InfoFasc"]=infofascicoloSelezionato;
		}		
#endregion

#region Variabile sessione "FascicoloSelezionato"
		//Utilizzata per memorizzare l'hashtable contenente
		//le informazioni sui titolari relativi al registro
		//selezionato
		public static void removeFascicoloSelezionato(Page page) 
		{
            removeFascicoloSelezionato();
		}

        public static void removeFascicoloSelezionato()
        {
            HttpContext.Current.Session.Remove("FascicoloSelezionato");
        }

		public static DocsPAWA.DocsPaWR.Fascicolo getFascicoloSelezionato(Page page)
		{
            return getFascicoloSelezionato();
		}

        public static DocsPAWA.DocsPaWR.DocumentoLogDocumento[] getStoriaLog(Page page, string idOggetto, string varOggetto)
        {
            try
            {
                DocsPAWA.DocsPaWR.DocumentoLogDocumento[] result = docsPaWS.DocumentoGetListaLog(idOggetto, varOggetto);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

		public static void setFascicoloSelezionato(Page page,DocsPaWR.Fascicolo fascicoloSelezionato)
		{
            setFascicoloSelezionato(fascicoloSelezionato);
		}

        public static DocsPAWA.DocsPaWR.Fascicolo getFascicoloSelezionato()
        {
            HttpContext ctx = HttpContext.Current;
            return (DocsPAWA.DocsPaWR.Fascicolo)ctx.Session["FascicoloSelezionato"];
        }

        public static void setFascicoloSelezionato(DocsPAWA.DocsPaWR.Fascicolo fascicoloSelezionato)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["FascicoloSelezionato"] = fascicoloSelezionato;
        }

        public static ArrayList getFascicoliSelezionati(Page page)
        {
            return getFascicoliSelezionati();
        }

        public static void setFascicoliSelezionati(Page page, ArrayList listaFascicoli)
        {
            setFascicoliSelezionati(listaFascicoli);
        }

        public static ArrayList getFascicoliSelezionati()
        {
            HttpContext ctx = HttpContext.Current;
            return (ArrayList)ctx.Session["FascicoliSelezionati"];
        }

        public static void setFascicoliSelezionati(ArrayList listaFascicoli)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["FascicoliSelezionati"] = listaFascicoli;
        }

        public static void removeFascicoliSelezionati()
        {
            HttpContext.Current.Session.Remove("FascicoliSelezionati");
        }		

        public static void removeMemoriaFascicoloSelezionato()
        {
            HttpContext.Current.Session.Remove("MemoriaFascicoloSelezionato");
        }
	
		//memoria fascicolo selezionato
		public static void removeMemoriaFascicoloSelezionato(Page page) 
		{
			page.Session.Remove("MemoriaFascicoloSelezionato");
		}

		public static DocsPAWA.DocsPaWR.Fascicolo getMemoriaFascicoloSelezionato(Page page)
		{
			//return (DocsPAWA.DocsPaWR.Fascicolo)page.Session["MemoriaFascicoloSelezionato"];
            return getMemoriaFascicoloSelezionato();
		}

        public static DocsPAWA.DocsPaWR.Fascicolo getMemoriaFascicoloSelezionato()
        {
            HttpContext ctx = HttpContext.Current;
            return (DocsPAWA.DocsPaWR.Fascicolo)ctx.Session["MemoriaFascicoloSelezionato"];
        }

		public static void setMemoriaFascicoloSelezionato(Page page,DocsPaWR.Fascicolo fascicoloSelezionato)
		{
            setMemoriaFascicoloSelezionato(fascicoloSelezionato);
		}

        public static void setMemoriaFascicoloSelezionato( DocsPaWR.Fascicolo fascicoloSelezionato)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["MemoriaFascicoloSelezionato"] = fascicoloSelezionato;
        }
		//fine memoria fascicolo selezionato

		//memoria folder selezionata
		public static void removeMemoriaFolderSelezionata(Page page) 
		{
            removeMemoriaFolderSelezionata();
		}

        public static void removeMemoriaFolderSelezionata()
        {
            HttpContext.Current.Session.Remove("MemoriaFolderSelezionata");
        }

		public static DocsPAWA.DocsPaWR.Folder getMemoriaFolderSelezionata(Page page)
		{
            return getMemoriaFolderSelezionata();
		}

        public static DocsPAWA.DocsPaWR.Folder getMemoriaFolderSelezionata()
        {
            HttpContext ctx = HttpContext.Current;
            return (DocsPAWA.DocsPaWR.Folder)ctx.Session["MemoriaFolderSelezionata"];
        }

		public static void setMemoriaFolderSelezionata(Page page,DocsPaWR.Folder folder)
		{
            setMemoriaFolderSelezionata(folder);
		}

        public static void setMemoriaFolderSelezionata(DocsPaWR.Folder folder)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["MemoriaFolderSelezionata"] = folder;
        }
        //fine memoria folder selezionata
#endregion

#region Variabile sessione "ricFasc.listaFiltri"
		public static void removeFiltroRicFasc(Page page) 
		{
			//page.Session.Remove("ricFasc.listaFiltri");
			RemoveSessionValue("ricFasc.listaFiltri");
            RemoveSessionValue("classificaSelezionata");
            RemoveSessionValue("filtroProfDinamica");

		}

		public static FiltroRicerca[][] getFiltroRicFasc(Page page)
		{
			//return (FiltroRicerca[][])page.Session["ricFasc.listaFiltri"];
			return (FiltroRicerca[][]) GetSessionValue("ricFasc.listaFiltri");			
		}

		public static void setFiltroRicFasc(Page page,FiltroRicerca[][] filtroRicerca)
		{
			//page.Session["ricFasc.listaFiltri"]=filtroRicerca;
			SetSessionValue("ricFasc.listaFiltri",filtroRicerca);			
		}

        public static void removeFiltroRicFascNew(Page page)
        {
            //page.Session.Remove("ricFasc.listaFiltri");
            RemoveSessionValue("ricFasc.listaFiltriNew");
            RemoveSessionValue("classificaSelezionata");
        }

        public static FiltroRicerca[][] getFiltroRicFascNew(Page page)
        {
            //return (FiltroRicerca[][])page.Session["ricFasc.listaFiltri"];
            return (FiltroRicerca[][])GetSessionValue("ricFasc.listaFiltriNew");
        }

        public static void setFiltroRicFascNew(Page page, FiltroRicerca[][] filtroRicerca)
        {
            //page.Session["ricFasc.listaFiltri"]=filtroRicerca;
            SetSessionValue("ricFasc.listaFiltriNew", filtroRicerca);
        }		
#endregion

#region Variabile sessione "TheHash"
		//Utilizzata per memorizzare l'hashtable contenente
		//le informazioni sui titolari relativi al registro
		//selezionato
		public static void removeTheHash(Page page) 
		{
			page.Session.Remove("TheHash");
		}

		public static Hashtable getTheHash(Page page)
		{
			return (Hashtable)page.Session["TheHash"];
		}

		public static void setTheHash(Page page,Hashtable hashTable)
		{
			page.Session["TheHash"]=hashTable;
		}		
#endregion

#region Variabile sessione "tabRicFasc.RigheDG"
		public static void setDatagridFascicolo(Page page, DataTable docs) 
		{
			page.Session["tabRicFasc.RigheDG"] = docs;
		}
		public static DataTable getDatagridFascicolo(Page page) 
		{
			return (DataTable)page.Session["tabRicFasc.RigheDG"];
		}

		public static void removeDatagridFascicolo(Page page) 
		{
			page.Session.Remove("tabRicFasc.RigheDG");
		}
#endregion 

#region Variabile sessione "newFascicolo"
		public static void setNewFascicolo(Page page, Fascicolo newFascicolo) 
		{
			page.Session["newFascicolo"] = newFascicolo;
		}
		public static Fascicolo getNewFascicolo(Page page) 
		{
			return (Fascicolo)page.Session["newFascicolo"];
		}

		public static void removeNewFascicolo(Page page) 
		{
			page.Session.Remove("newFascicolo");
		}
#endregion 

#region Variabile sessione "classificazioneSelezionata"
		public static void setClassificazioneSelezionata(Page page, FascicolazioneClassificazione classificazioneSelezionata) 
		{
			//page.Session["ClassificazioneSelezionata"] = classificazioneSelezionata;
			SetSessionValue("ClassificazioneSelezionata",classificazioneSelezionata);
		}
		public static FascicolazioneClassificazione getClassificazioneSelezionata(Page page) 
		{
			//return (FascicolazioneClassificazione)page.Session["ClassificazioneSelezionata"];
			return (FascicolazioneClassificazione) GetSessionValue("ClassificazioneSelezionata");
		}

		public static void removeClassificazioneSelezionata(Page page) 
		{
			//page.Session.Remove("ClassificazioneSelezionata");
			RemoveSessionValue("ClassificazioneSelezionata");
		}
#endregion 

	#region Variabile sessione "codice rubrica utente selezionato"


	public static void removeCodRubrica(Page page) 
		{
			page.Session.Remove("codRubricaUtSel");
		}

		public static string getCodRubrica(Page page)
		{
			return (string) page.Session["codRubricaUtSel"];
		}

		public static void setCodRubrica(Page page,string codRubrica)
		{
			page.Session["codRubricaUtSel"]=codRubrica;
		}		
    #endregion

		#region Gestione tasto 'Back' verso visualizzazione documenti fascicolo
		/// <summary>
		/// Salva l'indice della pagina visualizzata nel grid per la visualizzazione
		/// dei documenti protocollati.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageIndex"></param>
		public static void SetProtoDocsGridPaging(Page page, int pageIndex)
		{
			if(page.Session["protoDocsGridPaging"] != null)
			{
				page.Session["protoDocsGridPaging"] = pageIndex;
			}
			else
			{
				page.Session.Add("protoDocsGridPaging", pageIndex);
			}
		}

		/// <summary>
		/// Salva l'indice della pagina visualizzata nel grid per la visualizzazione
		/// dei documenti non protocollati.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageIndex"></param>
		public static void SetNonProtoDocsGridPaging(Page page, int pageIndex)
		{
			if(page.Session["nonProtoDocsGridPaging"] != null)
			{
				page.Session["nonProtoDocsGridPaging"] = pageIndex;
			}
			else
			{
				page.Session.Add("nonProtoDocsGridPaging", pageIndex);
			}
		}

		/// <summary>
		/// Ritorna l'indice della pagina da visualizzare nel grid per la visualizzazione
		/// dei documenti protocollati.
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public static int GetProtoDocsGridPaging(Page page)
		{
			int result = 0;

			if(page.Session["protoDocsGridPaging"] != null)
			{
				result = (int)page.Session["protoDocsGridPaging"];
			}

			return result;
		}

		/// <summary>
		/// Ritorna l'indice della pagina da visualizzare nel grid per la visualizzazione
		/// dei documenti non protocollati.
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public static int GetNonProtoDocsGridPaging(Page page)
		{
			int result = 0;

			if(page.Session["nonProtoDocsGridPaging"] != null)
			{
				result = (int)page.Session["nonProtoDocsGridPaging"];
			}

			return result;
		}
		public static void setMemoriaVisualizzaBack(Page page)
		{
			setMemoriaVisualizzaBack();
		}

		public static string getMemoriaVisualizzaBack(Page page)
		{
			return (string) getMemoriaVisualizzaBack();
		}

		public static void RemoveMemoriaVisualizzaBack(Page page)
		{
            RemoveMemoriaVisualizzaBack();
		}

        public static void setMemoriaVisualizzaBack()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["MemoriaVisualizzaBackF"] = "true";
        }

        public static string getMemoriaVisualizzaBack()
        {
            return (string)HttpContext.Current.Session["MemoriaVisualizzaBackF"];
        }

        public static void RemoveMemoriaVisualizzaBack()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("MemoriaVisualizzaBackF");
        }

		/// <summary>
		/// Abilita la tracciabilita' dal folder in modo che altre webform possano
		/// capire se sono state chiamate in seguito ad una ricerca sui fascicoli.
		/// </summary>
		public static void SetFolderViewTracing(Page page, bool tracingEnabled)
		{
			if(tracingEnabled)
			{
				if(page.Session["folderViewTracing"] != null)
				{
					page.Session["folderViewTracing"] = true;
				}
				else
				{
					page.Session.Add("folderViewTracing", true);
				}

				/* Rimuovi le informazioni relative al tasto back per le ricerche sui documenti. Questo 
				 * e' necessario per esser certi che sia visualizzato sempre il tasto back relativo all'ultima
				 * ricerca effettuata.
				 */
				DocumentManager.removeMemoriaFiltriRicDoc(page);
			}
			else
			{
				page.Session.Remove("folderViewTracing");
			}
		}

        /// <summary>
        /// Abilita la tracciabilita' dal folder in modo che altre webform possano
        /// capire se sono state chiamate in seguito ad una ricerca sui fascicoli.
        /// </summary>
        public static void SetFolderViewTracing(bool tracingEnabled)
        {
            if (tracingEnabled)
            {
                if (HttpContext.Current.Session["folderViewTracing"] != null)
                    HttpContext.Current.Session["folderViewTracing"] = true;
                else
                {
                    HttpContext ctx = HttpContext.Current;
                    ctx.Session["folderViewTracing"] = true;
                }
                /* Rimuovi le informazioni relative al tasto back per le ricerche sui documenti. Questo 
                 * e' necessario per esser certi che sia visualizzato sempre il tasto back relativo all'ultima
                 * ricerca effettuata.
                 */
                DocumentManager.removeMemoriaFiltriRicDoc(null);
            }
            else
                HttpContext.Current.Session.Remove("folderViewTracing");
        }

        /// <summary>
		/// Ritorna lo stato della tracciabilita' dal folder.
		/// </summary>
		public static bool GetFolderViewTracing(Page page)
		{
			bool result = false;

			if(page.Session["folderViewTracing"] != null)
			{
				result = (bool)page.Session["folderViewTracing"];
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		public static string FolderViewReloadScript()
		{
			return "<script language='javascript'>top.principale.document.location='../fascicolo/gestioneFasc.aspx?tab=documenti';</script>";		
		}
		#endregion

		public static bool CanRemoveFascicolo(Page page,string project_Id, out string nFasc)
		{
			nFasc = "";
			bool result = docsPaWS.FascicolazioneCanRemoveFascicolo(project_Id, out nFasc);
			return result;
		}


		#region tasto 'back' su gestione fascicolo by massimo digregorio
		//numPag: numero della pagina dalla quale si è partiti nel datagrid
		public static string getMemoriaNumPag(Page page) 
		{
			return (string) GetSessionValue("MemoriaNumPagRicFasc");
		}

		public static void setMemoriaNumPag(Page page, string numPag) 
		{
			SetSessionValue("MemoriaNumPagRicFasc",numPag);
		}

		public static void removeMemoriaNumPag(Page page) 
		{
			RemoveSessionValue("MemoriaNumPagRicFasc");
		}

		//Classifica RicFasc: classifica ricercata  
		public static DocsPAWA.DocsPaWR.FascicolazioneClassificazione getMemoriaClassificaRicFasc(Page page) 
		{
			return (DocsPAWA.DocsPaWR.FascicolazioneClassificazione) GetSessionValue("MemoriaClassificaRicFasc");
		}

		public static void setMemoriaClassificaRicFasc(Page page, DocsPAWA.DocsPaWR.FascicolazioneClassificazione classifica) 
		{
			SetSessionValue("MemoriaClassificaRicFasc",classifica);
		}

		public static void removeMemoriaClassificaRicFasc(Page page) 
		{
			RemoveSessionValue("MemoriaClassificaRicFasc");
		}

		//Classifica RicFasc: Registro selezionato  
		public static DocsPAWA.DocsPaWR.Registro getMemoriaRegistroRicFasc(Page page) 
		{
			return (DocsPAWA.DocsPaWR.Registro) GetSessionValue("MemoriaRegistroRicFasc");
		}

		public static void setMemoriaRegistroRicFasc(Page page, DocsPAWA.DocsPaWR.Registro registro) 
		{
			SetSessionValue("MemoriaRegistroRicFasc", registro);
		}

		public static void removeMemoriaRegistroRicFasc(Page page) 
		{
			RemoveSessionValue("MemoriaRegistroRicFasc");
		}


		//FiltriRicFasc: filtri di ricerca impostati nel tab di ricerca di riferimento
		public static DocsPAWA.DocsPaWR.FiltroRicerca[][] getMemoriaFiltriRicFasc(Page page) 
		{
			return (DocsPAWA.DocsPaWR.FiltroRicerca[][]) GetSessionValue("MemoriaFiltriRicFasc");
		}

		public static void setMemoriaFiltriRicFasc(Page page, DocsPAWA.DocsPaWR.FiltroRicerca[][] listaFiltri) 
		{
			SetSessionValue("MemoriaFiltriRicFasc",listaFiltri);

			/* Rimuovi le informazioni relative al tasto back per le ricerche sui fascicoli. Questo 
			 * e' necessario per esser certi che sia visualizzato sempre il tasto back relativo all'ultima
			 * ricerca effettuata.
			 */
			FascicoliManager.removeFiltroRicFasc(page);
		}
		public static void removeMemoriaFiltriRicFasc(Page page) 
		{
			RemoveSessionValue("MemoriaFiltriRicFasc");
		}


		public static void removeMemoriaRicFasc(Page page) 
		{
			removeMemoriaClassificaRicFasc(page);
			removeMemoriaFiltriRicFasc(page);
			removeMemoriaNumPag(page);
			removeMemoriaRegistroRicFasc(page);
		}
		


		#endregion


		#region gestione OGGETTI SESSIONE
		/// Impostazione valore in sessione
		/// </summary>
		/// <param name="sessionKey"></param>
		/// <param name="sessionValue"></param>
		private static void SetSessionValue(string sessionKey,object sessionValue)
		{	
			System.Web.HttpContext.Current.Session[sessionKey]=sessionValue;
		}

		/// <summary>
		/// Reperimento valore da sessione
		/// </summary>
		/// <param name="sessionKey"></param>
		/// <returns></returns>
		private static Object GetSessionValue(string sessionKey)
		{
			return System.Web.HttpContext.Current.Session[sessionKey];
		}

		/// <summary>
		/// Rimozione chiave di sessione
		/// </summary>
		/// <param name="sessionKey"></param>
		private static void RemoveSessionValue(string sessionKey)
		{
			System.Web.HttpContext.Current.Session.Remove(sessionKey);
		}
		#endregion gestione OGGETTI SESSIONE

		public static void DO_SetLocazioneFisica(DocsPaVO.LocazioneFisica.LocazioneFisica lf)
		{
			SetSessionValue("LocazioneFisica",lf);
		}

		public static void setUoReferenteSelezionato(Page page, Corrispondente corrispondente) 
		{
			page.Session["fascicolo.corrispondenteReferenteSelezionato"] = corrispondente;
		}

		public static void setUoReferenteSelezionato(Corrispondente corrispondente) 
		{
			HttpContext ctx = HttpContext.Current;
			ctx.Session["fascicolo.corrispondenteReferenteSelezionato"] = corrispondente;
		}

		public static DocsPaVO.LocazioneFisica.LocazioneFisica DO_GetLocazioneFisica()
		{
			DocsPaVO.LocazioneFisica.LocazioneFisica lf = (DocsPaVO.LocazioneFisica.LocazioneFisica)GetSessionValue("LocazioneFisica");
			return lf;
		}

		public static Corrispondente getUoReferenteSelezionato(Page page) 
		{
			return (Corrispondente)page.Session["fascicolo.corrispondenteReferenteSelezionato"];
		}
		
		public static void removeUoReferenteSelezionato(Page page) 
		{
			page.Session.Remove("fascicolo.corrispondenteReferenteSelezionato");
		}

		public static void DO_RemoveLocazioneFisica()
		{
			RemoveSessionValue("LocazioneFisica");
		}

		public static void DO_SetIdUO_LF(string idUoLF)
		{
			SetSessionValue("ID_UO_LF",idUoLF);
		}

		public static string DO_GetIdUO_LF()
		{
			return GetSessionValue("ID_UO_LF").ToString();
		}

		public static void DO_RemoveIdUO_LF()
		{
			RemoveSessionValue("ID_UO_LF");
		}

		public static void DO_SetFlagLF()
		{
			SetSessionValue("FlagLF",true);
		}

		public static void DO_RemoveFlagLF()
		{
			RemoveSessionValue("FlagLF");
		}

		public static bool DO_VerifyFlagLF()
		{
			if(System.Web.HttpContext.Current.Session["FlagLF"] != null)
			{
				return true;
			}
			else
			{
				return  false;
			}
		}
		public static void DO_SetFlagUR()
		{
			SetSessionValue("FlagUR",true);
		}

		public static void DO_RemoveFlagUR()
		{
			RemoveSessionValue("FlagUR");
		}

		public static bool DO_VerifyFlagUR()
		{
			if(System.Web.HttpContext.Current.Session["FlagUR"] != null)
			{
				return true;
			}
			else
			{
				return  false;
			}
		}

		#region FASCICOLAZIONE RAPIDA
		public static void removeFascicoloSelezionatoFascRapida(Page page) 
		{
            removeFascicoloSelezionatoFascRapida();
		}

		public static DocsPAWA.DocsPaWR.Fascicolo getFascicoloSelezionatoFascRapida(Page page)
		{
			return (DocsPAWA.DocsPaWR.Fascicolo)getFascicoloSelezionatoFascRapida();
		}

		public static void setFascicoloSelezionatoFascRapida(Page page,DocsPaWR.Fascicolo fascicoloSelezionato)
		{
			setFascicoloSelezionatoFascRapida(fascicoloSelezionato);
		}

        public static void removeFascicoloSelezionatoFascRapida()
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session.Remove("FascicoloSelezionatoFascRapida");
        }

        public static DocsPAWA.DocsPaWR.Fascicolo getFascicoloSelezionatoFascRapida()
        {
            return (DocsPAWA.DocsPaWR.Fascicolo)HttpContext.Current.Session["FascicoloSelezionatoFascRapida"];
        }

        public static void setFascicoloSelezionatoFascRapida(DocsPaWR.Fascicolo fascicoloSelezionato)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["FascicoloSelezionatoFascRapida"] = fascicoloSelezionato;
        }

      public static void removeCodiceFascRapida(Page page)
      {
         page.Session.Remove("CodiceFascRapida");
      }
      public static string getCodiceFascRapida(Page page)
      {
         return (string)page.Session["CodiceFascRapida"];
      }

      public static void setCodiceFascRapida(Page page, string codiceFascRapida)
      {
         page.Session["CodiceFascRapida"] = codiceFascRapida;
      }

      public static void removeDescrizioneFascRapida(Page page)
      {
         page.Session.Remove("DescrizioneFascRapida");
      }
      public static string getDescrizioneFascRapida(Page page)
      {
         return (string)page.Session["DescrizioneFascRapida"];
      }

      public static void setDescrizioneFascRapida(Page page, string descrizioneFascRapida)
      {
         page.Session["DescrizioneFascRapida"] = descrizioneFascRapida;
      }
		#endregion

      #region gestione fascicolo ricerca
      public static void removeFascicoloSelezionatoRicerca()
      {
          HttpContext ctx = HttpContext.Current;
          ctx.Session.Remove("FascicoloSelezionatoRicerca");
      }

      public static DocsPAWA.DocsPaWR.Fascicolo getFascicoloSelezionatoRicerca()
      {
          return (DocsPAWA.DocsPaWR.Fascicolo)HttpContext.Current.Session["FascicoloSelezionatoRicerca"];
      }

      public static void setFascicoloSelezionatoRicerca(Page page,DocsPaWR.Fascicolo fascicoloSelezionato)
      {
          HttpContext ctx = HttpContext.Current;
          ctx.Session["FascicoloSelezionatoRicerca"] = fascicoloSelezionato;
      }
      #endregion gestione fascicolo ricerca

      #region gestione nuova fascicolazione
      public static string getNumeroFascicolo(Page page,string idTitolario, string idRegistro)
        {
            try
            {
                return docsPaWS.FascicolazioneGetFascNumRif(idTitolario,idRegistro);
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return null;
        }
        #endregion

        #region ACL
        public static bool editingFascACL(DocsPAWA.DocsPaWR.FascicoloDiritto fascDiritto, string personOrGroup, DocsPaWR.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.EditingFascACL(fascDiritto, personOrGroup, infoUtente);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        public static bool ripristinaFascACL(DocsPAWA.DocsPaWR.FascicoloDiritto fascDiritto, string personOrGroup, DocsPaWR.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                result = docsPaWS.RipristinaFascACL(fascDiritto, personOrGroup, infoUtente);
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }


        #endregion

        #region verifica se sul nodo è possibile creare fasc e sottofasc
        public static bool verificaNodoAbilitatoCreaFasc(string idnodo)
        {
            return docsPaWS.FascicolazionCanAddFasc(idnodo);
        }
        #endregion 

        #region trasferimento in deposito
        public static DocsPAWA.DocsPaWR.Fascicolo getFascicoloDaArchiviare(Page page, string codFascicolo)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                DocsPaWR.Fascicolo result = docsPaWS.FascicolazioneGetFascicoloDaArchiviare(infoUtente, codFascicolo);

                if (result == null)
                {
                    //throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static Fascicolo[] getListaFascicoliDaArchiviare(Page page, FascicolazioneClassificazione classificazione, DocsPAWA.DocsPaWR.Registro registro, FiltroRicerca[] filtriRicerca, bool childs, int requestedPage, out int numTotPage, out int nRec, int pageSize)
        {
            nRec = 0;
            numTotPage = 0;
            try
            {
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    enableProfilazione = true;
                InfoUtente infoUtente = UserManager.getInfoUtente(page);


                Fascicolo[] result = docsPaWS.FascicolazioneGetListaFascicoliDaArchiviare(infoUtente, classificazione, registro, filtriRicerca,
                                              enableUfficioRef, enableProfilazione, childs, requestedPage, pageSize, out numTotPage, out nRec);
                if (result == null)
                {
                    throw new Exception();
                }
                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        //Restituisce la lista di documenti classificati in un dato fascicolo generale
        public static InfoDocumento[] getListaDocumentiDaArchiviare(Page page, Fascicolo fascicolo, int numPage, out int numTotPage, out int nRec, string anno)
        //public static InfoDocumento[] getListaDocumentiDaArchiviare(Page page, Fascicolo fascicolo, out int nRec, string anno)
        {
            nRec = 0;
            numTotPage = 0;
            InfoDocumento[] retValue = null;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                retValue = docsPaWS.FascicolazioneGetDocumentiDaArchiviare(anno, infoUtente.idGruppo, infoUtente.idPeople, fascicolo, numPage, out numTotPage, out nRec);
                //retValue = docsPaWS.FascicolazioneGetDocumentiDaArchiviare(anno, infoUtente.idGruppo, infoUtente.idPeople, fascicolo, out nRec);
                if (retValue == null)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return retValue;
        }

        public static int trasfInDepositoALLDocsFascicoloGen(Page page, Fascicolo fascicolo, string anno, InfoUtente utente, string tipoOp)
        {
            int result = 0;
            try
            {
                result = docsPaWS.ExecTrasfInDepositoALLDocsFascicoloGen(fascicolo, anno, utente, tipoOp);
                if (result==-1)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            return result;
        }

        public static void trasfInDepositoFascicoloProc(Page page, string idProject, InfoUtente utente, string tipoOp)
        {
            bool result = false;
            try
            {
                result = docsPaWS.ExecTrasfInDepositoFascicoloProc(idProject, utente, tipoOp);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            
        }

        public static void trasfInDepositoALLFascicoliProc(Page page, DocsPAWA.DocsPaWR.FiltroRicerca[][] filtriRic, InfoUtente utente, string tipoOp)
        {
            bool result = false;
            try
            {
                result = docsPaWS.ExecTrasfInDepositoALLFascicoliProc(filtriRic, utente, tipoOp);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }

        }

        public static bool trasfInDepositoFascProc(Page page, DocsPaWR.Fascicolo fascicolo, InfoUtente utente, string tipoOp)
        {
            bool result = false;
            try
            {
                result = docsPaWS.ExecTrasfInDepositoFascicoloProc(fascicolo.systemID, utente, tipoOp);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            return result;
        }

        #endregion

        #region Area Scarto
        public static Fascicolo[] getListaFascicoliInDeposito(Page page, Fascicolo fascicolo, int numPage, out int numTotPage, out int nRec, string tipoRicerca)
        {
            nRec = 0;
            numTotPage = 0;
            Fascicolo[] retValue = null;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                retValue = docsPaWS.ScartoGetFascicoliInDeposito(infoUtente, fascicolo, numPage, tipoRicerca, out numTotPage, out nRec);
                if (retValue == null)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return retValue;
        }

        public static int isPrimaIstanzaScarto(Page page, string idPeople, string idGruppo)
        {
            int result = 0;
            try
            {
                result = docsPaWS.ScartoIsPrimaIstanzaFascicolo(idPeople, idGruppo);
                if (result == -1)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        public static string addAreaScarto(Page page, string idProfile, string idProject, string docNumber, InfoUtente utente)
        {
            string result = string.Empty;
            try
            {
                result = docsPaWS.ScartoAddFascicolo(idProfile, idProject, docNumber, utente);
                if (result == "-1")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            return result;
        }

        public static void eliminaDaAreaScarto(Page page, Fascicolo fasc, string idIstanza, bool deleteIstanza, string systemId)
        {
            try
            {
                bool result = docsPaWS.ScartoDeleteFascicolo(fasc, idIstanza, deleteIstanza, systemId);
                if (!result)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        

        public static bool addAllFascAreaScarto(Page page, DocsPAWA.DocsPaWR.Fascicolo fascicolo, DocsPAWA.DocsPaWR.InfoUtente infoUtente, string tipoRic)
        {
            bool result = true;
            try
            {
                result = docsPaWS.ScartoAddAllFasc(fascicolo, infoUtente, tipoRic);
                if (!result)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        public static InfoScarto[] getListaScarto(Page page, DocsPAWA.DocsPaWR.InfoUtente infoUtente, int numPage, out int numTotPage, out int nRec)
        {
            nRec = 0;
            numTotPage = 0;
            InfoScarto[] listaScarto = null;
            try
            {
                listaScarto = docsPaWS.ScartoGetListaScarto(infoUtente, numPage, out numTotPage, out nRec);
                if (listaScarto == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return listaScarto;
        }

        public static Fascicolo[] getListaFascicoliInScarto(Page page, InfoScarto infoScarto, int numPage, out int numTotPage, out int nRec)
        {
            nRec = 0;
            numTotPage = 0;
            Fascicolo[] retValue = null;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                retValue = docsPaWS.ScartoGetFascicoliInScarto(infoUtente, infoScarto, numPage, out numTotPage, out nRec);
                if (retValue == null)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return retValue;
        }

        public static bool updateAreaScarto(Page page, DocsPaWR.InfoScarto itemScarto)
        {
            bool retValue = true;
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                retValue = docsPaWS.ScartoUpdateScarto(infoUtente, itemScarto);
                if (!retValue)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return retValue;
        }

        public static bool cambiaStatoScarto(Page page, DocsPaWR.InfoScarto itemScarto, DocsPaWR.InfoUtente infoUtente, string nuovoCampo)
        {
            bool retValue = true;
            try
            {
                retValue = docsPaWS.ScartoCambiaStato(infoUtente, itemScarto, nuovoCampo);
                if (!retValue)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return retValue;
        }


        #region Variabili sessione Scarto
        public static void removeListaScarto(Page page)
        {
            page.Session.Remove("ListaScarto");
        }

        public static DocsPAWA.DocsPaWR.InfoScarto[] getListaScarto(Page page)
        {
            return (DocsPAWA.DocsPaWR.InfoScarto[])page.Session["ListaScarto"];
        }

        public static void setListaScarto(Page page, DocsPaWR.InfoScarto[] listaScarto)
        {
            page.Session["ListaScarto"] = listaScarto;
        }

        public static void removeIstanzaScarto(Page page)
        {
            page.Session.Remove("ItemScarto");
        }

        public static DocsPAWA.DocsPaWR.InfoScarto getIstanzaScarto(Page page)
        {
            return (DocsPAWA.DocsPaWR.InfoScarto)GetSessionValue("ItemScarto");
        }

        public static void setIstanzaScarto(Page page, DocsPaWR.InfoScarto itemScarto)
        {
            page.Session["ItemScarto"] = itemScarto;
        }
        #endregion
        #endregion


        #region Area Conservazione
        public static string[] getIdDocumentiFromFascicolo(string idProject)
        {
            SearchResultInfo[] fasc = docsPaWS.getDocumentiInFascicolo(idProject);
            string[] result = new string[fasc.Length];
            for (int i = 0; i < fasc.Length; i++)
            {
                result[i] = Convert.ToString(fasc[i].Id);
            }
            return result;
        }

        public static SearchResultInfo[] getDocumentiFromFascicolo(string idProject)
        {
            return docsPaWS.getDocumentiInFascicolo(idProject);
        }

        #endregion

        public static void creaRiscontroMittente(DocsPaWR.RiscontroMittente riscontroMittente)
        {
            docsPaWS.creaRiscontroMittente(riscontroMittente);
        }

        public static DocsPaWR.RiscontroMittente cercaRiscontroMittente(DocsPaWR.RiscontroMittente riscontroMittente)
        {
            return docsPaWS.cercaRiscontroMittente(riscontroMittente);
        }

        public static void eliminaRiscontroMittente(DocsPaWR.RiscontroMittente riscontroMittente)
        {
            docsPaWS.eliminaRiscontroMittente(riscontroMittente);
        }

        public static int getNumForlderDoc(Page page, string idProfile)
        {
            int result = 0;

            try
            {
                result = docsPaWS.getNumFolderDoc(idProfile);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return -1;
            }

            return result;
        }

        public static void SetDoFascRapida(Page page, Fascicolo fascicolo)
        {
            page.Session["FascicoloRipr"] = fascicolo;
        }

        public static Fascicolo GetDoFascRapida(Page page)
        {
            return page.Session["FascicoloRipr"] as Fascicolo;
        }

        public static string GetRootFolderFasc(Page page, Fascicolo fasc)
        {
            string result = string.Empty;

            try
            {
                result = docsPaWS.getRootFolderFasc(fasc.systemID);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return result;
        }

        #region Funzioni spostate dalla pagina di ricerca fascicoli

        public static string inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string paramContatore)
        {
            string[] formatoContDaFunzione = paramContatore.Split('-');
            string[] formatoContDaSostituire = new string[] { "A", "A", "A" };

            //for (int i = 0; i < formatoContDaSostituire.Length; i++)
            //    formatoContDaSostituire[i] = string.Empty;

            formatoContDaFunzione.CopyTo(formatoContDaSostituire, 0);

            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return paramContatore;
            }

            //Imposto il contatore in funzione del formato
            string contatore = string.Empty;
            if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
            {
                contatore = oggettoCustom.FORMATO_CONTATORE;
                contatore = contatore.Replace("ANNO", formatoContDaSostituire[1].ToString());
                contatore = contatore.Replace("CONTATORE", formatoContDaSostituire[2].ToString());
                if (!string.IsNullOrEmpty(formatoContDaSostituire[0]))
                {
                    contatore = contatore.Replace("RF", formatoContDaSostituire[0].ToString());
                    contatore = contatore.Replace("AOO", formatoContDaSostituire[0].ToString());
                }
                else
                {
                    contatore = contatore.Replace("RF", "A");
                    contatore = contatore.Replace("AOO", "A");
                }
            }
            else
            {
                contatore = paramContatore;
            }

            return eliminaBlankSegnatura(contatore);

        }

        private static string eliminaBlankSegnatura(string paramSegnatura)
        {
            char separatore = '|';
            string[] temp = paramSegnatura.Split('|');
            string appoggio = string.Empty;

            if (temp.Length == 1)
            {
                temp = paramSegnatura.Split('-');
                separatore = '-';
            }

            for (int i = 0; i < temp.Length; i++)
            {
                if (!temp[i].Equals("A"))
                {
                    appoggio += temp[i];
                    if (i != temp.Length - 1)
                        appoggio += separatore;
                }
            }
            return appoggio;
        }

        #endregion

        public static DocsPAWA.DocsPaWR.Folder getFolderByIdFasc(DocsPaWR.InfoUtente infoUtente, DocsPAWA.DocsPaWR.Fascicolo Fasc)
        {
            DocsPAWA.DocsPaWR.Folder result = null;

            result = docsPaWS.FascicolazioneGetFolder(infoUtente.idPeople, infoUtente.idGruppo, Fasc);

            return result;
        }

        public static DocsPAWA.DocsPaWR.FascicoloDiritto[] getListaVisibilitaSemplificata(Page page, DocsPaWR.InfoFascicolo infoFasc, bool cercaRimossi, string rootFolder)
        {
            //docWS.getQuery(xmlQueryV,xmlSafe);
            try
            {
                //DocsPaWR.FascicoloDiritto[] FascDir = docsPaWS.FascicolazioneGetVisibilitaSemplificata(infoFasc, cercaRimossi, rootFolder);
                DocsPaWR.FascicoloDiritto[] FascDir = docsPaWS.FascGetVisibilityWithFIlter(UserManager.getInfoUtente(), infoFasc.idFascicolo, cercaRimossi, null, rootFolder);

                if (FascDir == null)
                {
                    throw new Exception();
                }

                return FascDir;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static DocsPAWA.DocsPaWR.OrgTitolario[] getTitolariUtilizzabili(string idAmm, Page page)
        {
            DocsPAWA.DocsPaWR.OrgTitolario[] retValue = null;
            try
            {
                retValue = docsPaWS.getTitolariUtilizzabili(idAmm);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return retValue;
        }

        //get storico campi profilati di un tipo documento
        public static DocsPAWA.DocsPaWR.StoricoProfilati[] getStoriaProfilatiFasc(Page page, DocsPAWA.DocsPaWR.Templates template, string idProject)
        {
            try
            {
                DocsPaWR.StoricoProfilati[] result = docsPaWS.FascicoloGetListaStoricoProfilati(template.ID_TIPO_FASC, idProject);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        public static SearchObject[] getListaFascicoliPagingCustom(Page page, FascicolazioneClassificazione classificazione, DocsPAWA.DocsPaWR.Registro registro, FiltroRicerca[] filtriRicerca, bool childs, int requestedPage, out int numTotPage, out int nRec, int pageSize, bool getSystemIdList, out SearchResultInfo[] idProjectList, byte[] datiExcel, bool showGridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId,bool security)
        {
            nRec = 0;
            numTotPage = 0;

            // Lista dei system id dei fascicoli
            SearchResultInfo[] idProjects = null;

            try
            {
                bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

                bool enableProfilazione = false;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    enableProfilazione = true;
                }

                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                SearchObject[] result = docsPaWS.FascicolazioneGetListaFascicoliPagingCustom(infoUtente, classificazione, registro, filtriRicerca, enableUfficioRef, enableProfilazione, childs, requestedPage, pageSize, getSystemIdList, datiExcel, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, security, out numTotPage, out nRec, out idProjects);

                if (result == null)
                {
                    throw new Exception();
                }

                // Salvataggio della lista dei system id dei fascicoli
                idProjectList = idProjects;

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            idProjectList = null;

            return null;
        }

        public static SearchObject GetObjectFascicoloBySystemId(string systemId, InfoUtente infoUtente)
        {
            SearchObject result = null;

            result = docsPaWS.GetObjectFascicoloBySystemId(systemId, infoUtente);

            return result;
        }

        public static SearchObject[] getListaDocumentiPagingCustom(
                        Page page,
                        Folder folder,
                        DocsPaWR.FiltroRicerca[][] filtriRicerca,
                        int numPage,
                        out int numTotPage,
                        out int nRec,
                        bool compileIdProfileList,
                        bool showGridPersonalization, 
                        bool export, 
                        Field[] visibleFieldsTemplate, 
                        String[] documentsSystemId,
                        int pageSize,
                        DocsPaWR.FiltroRicerca[][] filtriRicercaOrdinamento,
                        out SearchResultInfo[] idProfiles)
        {
            nRec = 0;
            numTotPage = 0;

            SearchResultInfo[] idProfileList = null;

            SearchObject[] retValue = null;

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(page);

                retValue = docsPaWS.FascicolazioneGetDocumentiPagingWithFiltersCustom(infoUtente, folder, filtriRicerca, numPage, compileIdProfileList, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId,pageSize,filtriRicercaOrdinamento, out numTotPage, out nRec, out idProfileList);

                if (retValue == null)
                    throw new Exception();
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            idProfiles = idProfileList;
            return retValue;
        }
        /// <summary>
        /// Metodo per verificare se un fascicolo è generale
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="idFascicolo"></param>
        /// <returns>True se il fascicolo è generale.</returns>
        public static bool IsFascicoloGenerale(InfoUtente userInfo, String idFascicolo)
        {
            bool res = false;

            try
            {
                res = docsPaWS.IsFascicoloGenerale(userInfo, idFascicolo);
            }
            catch (Exception e)
            {
                DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);
            }

            return res;
        }

        public static Fascicolo[] GetFascicoloDaCodiceNoSecurity(string codiceFasc, string idAmm, string titolari, bool soloGenerali)
        {
            try
            {
                DocsPaWR.Fascicolo[] result = docsPaWS.GetFascicoloDaCodiceNoSecurity(codiceFasc, idAmm, titolari, soloGenerali);

                return result;
            }
            catch (Exception es)
            {
                return null;
            }
        }

        public static Fascicolo getFascicoloByIdNoSecurity(string idFascicolo)
        {
            DocsPaWR.Fascicolo result = null;

            try
            {

                result = docsPaWS.FascicolazioneGetFascicoloByIdNoSecurity(idFascicolo);
            }
            catch (Exception es)
            {

            }

            return result;

        }

    }
}
