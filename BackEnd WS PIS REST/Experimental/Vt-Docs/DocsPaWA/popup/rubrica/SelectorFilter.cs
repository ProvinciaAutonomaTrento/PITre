using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using DocsPAWA.popup.RubricaDocsPA.CustomControls;

namespace DocsPAWA.popup.RubricaDocsPA
{
	/// <summary>
	/// Summary description for SelectorFilter.
	/// </summary>
	public class SelectorFilter
	{
		static Hashtable ht_uo_smistamento;
		static Hashtable ht_uo_interne;

		UOSmistamento[] uo_smistamento = null;
		string[] uo_interne = null;
		
		DocsPaWR.ParametriRicercaRubrica sf_qco;
		Page _page;
		readonly RubricaCallType _calltype;

		public RubricaCallType CallType { get { return _calltype; } }

		static SelectorFilter()
		{
			ht_uo_smistamento = new Hashtable();
			ht_uo_interne = new Hashtable();
		}
		
		public SelectorFilter(Page page, RubricaCallType calltype)
		{
			_page = page;
			_calltype = calltype;

			// Inizializza la tabella UO smistamento


			string id_registro = null;
			if (calltype == DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_PROTO_INGRESSO)
				id_registro = ProtocollazioneIngresso.Registro.RegistroMng.GetRegistroInSessione().systemId;
			else
			{
				Registro rreg = UserManager.getRegistroSelezionato(_page);
				if (rreg != null)
					id_registro = rreg.systemId;
				else
				{
					Ruolo rr = UserManager.getRuolo();

                    //se veniamo dall'amministrazione (per esempio nel caso di creazione
                    // di una lista di distribuzione) non sappiamo l'id del registro in
                    // esame
                    if (rr.registri == null)
                    {
 			            string codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string) page.Session["AMMDATASET"],"0");
                        DocsPaWebService ws = new DocsPaWebService();
                        OrgRegistro[] reg = ws.AmmGetRegistri(codAmm, "0");
                        if(reg.Length > 0)
                            id_registro = reg[0].IDRegistro;
                    }
                    else
					id_registro = rr.registri[0].systemId;
					//UserManager.setRegistroSelezionato(_page,rr.registri[0]);
				}
			}

			uo_smistamento = (UOSmistamento[]) ht_uo_smistamento[id_registro];
			uo_interne = (string[]) ht_uo_interne[id_registro];

			if (uo_smistamento == null) 
			{
				DocsPaWebService ws = new DocsPaWebService();
				MittenteSmistamento ms = new MittenteSmistamento();
				ms.IDPeople = "0";
				uo_smistamento = ws.GetUOSmistamento(id_registro, ms);

				if (uo_smistamento != null)
				{
					Array.Sort (uo_smistamento, new UOSmistamentoByCodiceSorter());
				}
				else
					uo_smistamento = new UOSmistamento[0];

				ht_uo_smistamento[id_registro] = uo_smistamento;
			}

			// Inizializza la tabella UO interne alla nostra AOO
			if (uo_interne == null) 
			{
				DocsPaWebService ws = new DocsPaWebService();
				uo_interne = UserManager.GetUoInterneAoo (_page);

				if(uo_interne!=null)
					Array.Sort (uo_interne , CaseInsensitiveComparer.Default);
			
				ht_uo_interne[id_registro] = uo_interne;
			}
		}


		#region gestione filtro organigramma
		/// <summary>
		/// metodo per filtrare gli elementi rubrica che sono selezionabili nel contesto corrente
		/// </summary>
		/// <param name="er">elemento Rubrica corrente</param>
		/// <returns></returns>
		public void filterCorrSelectedAllowed (ref ElementoRubrica[] ers, string tipoIESelected)
		{
			DocsPaWR.ElementoRubrica[] a = null;

			foreach(DocsPAWA.DocsPaWR.ElementoRubrica er in ers)
			{
				if(er!=null)
				{
					switch (er.tipo)
					{
						case "U":
						{
							if (check_uo (er.codice) )
							{
								er.isVisibile = true;
								a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
										
							}
							else
							{	
								if(tipoIESelected.Equals("I"))
								{
									er.isVisibile = false;
									a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
								}
								else
								{
									er.isVisibile = true;
									a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
								}
							}	
						}
							break;

						case "R":
						case "P":	
						{
							filterRuoliUtentiSelectedAllowed(ref a, er,tipoIESelected);
						}
							break;
					}

				}
			}
			ers = a ;
		}

		private bool filterRuoliUtentiSelectedAllowed (ref ElementoRubrica[] a, DocsPAWA.DocsPaWR.ElementoRubrica er, string tipoIESelected)
		{
			string codiceCorr = null;
			bool retValue = true;
			DocsPaWR.Corrispondente corr = UserManager.GetCorrispondenteInternoOrganigramma(_page, er.codice, true);
			if (corr == null)
			{
				retValue = false;
				return retValue;
			}

			if (er.tipo == "R")
			{
				if(((DocsPAWA.DocsPaWR.Ruolo) corr).uo!=null)
				{
					codiceCorr = ((DocsPAWA.DocsPaWR.Ruolo) corr).uo.codiceRubrica;
					if(check_uo (codiceCorr))
					{
						er.isVisibile = true;
						a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
					}
					else
					{
						if(tipoIESelected.Equals("I"))
						{
							er.isVisibile = false;
							a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
						}
						else
						{
							er.isVisibile = true;
							a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
						}
//						
//						er.isVisibile = false;
//						a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);	
					}
				}
				else //se il ruolo non appartiene a nessuna UO, di dafault non lo rendo selezionabile
				{
					er.isVisibile = false;
					DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
				}		
				
			}
			else // caso utente
			{
				DocsPaWR.Ruolo[] ruoli = ((DocsPAWA.DocsPaWR.Utente) corr).ruoli;
				
				if(ruoli != null && ruoli.Length > 0)
				{
					foreach (Ruolo r in ruoli) 
					{
						codiceCorr = (((DocsPAWA.DocsPaWR.Ruolo)r).uo).codiceRubrica;
						if(check_uo (codiceCorr))
						{
							er.isVisibile = true;
							a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
						}
						else
						{
							if(tipoIESelected.Equals("I"))
							{
								er.isVisibile = false;
								a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
							}
							else
							{
								er.isVisibile = true;
								a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
							}
						}
					}
				}
				else //se l'utente non ha ruoli di default mettiamo il check a invisible
				{
					er.isVisibile = false;
					a = DocsPAWA.Utils.addToArrayElementoRubrica(a,er);
				}
				
			}
			return retValue;
		}


		private bool check_uo (string cod_uo)
		{
			int i;
			
			i = Array.BinarySearch(uo_interne, cod_uo);
			return (i >= 0);
		}

		#endregion

		public bool execute (ElementoRubrica er)
		{
			if (!er.interno)
				return true;

			if (er.tipo == "U") 
				return filtra_smistamento(er.codice);
			else
				if (er.tipo == "R" || er.tipo == "P")
					return filtra_ruoli_utenti_smistamento (er);
				else
					return true;
		}

		public bool execute (string _codice, string _tipo, string _tipo_ie)
		{
			ElementoRubrica er = new ElementoRubrica();
			er.codice = _codice;
			er.interno = (_tipo_ie == "I");
			er.tipo = _tipo;
			return execute (er);
		}


		#region Classi di supporto

		class UOSmistamentoByCodiceSorter : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				string cod_x = ((UOSmistamento)x).Codice;
				string cod_y = ((UOSmistamento)y).Codice;
				return String.Compare(cod_x, cod_y, true);
			}

			#endregion

		}

		class UOSmistamentoByCodiceFinder : IComparer
		{
			#region IComparer Members


			public int Compare(object x, object y)
			{
				if (x is UOSmistamento && y is string) 
				{
					string cod_x = ((UOSmistamento)x).Codice;
					return String.Compare (cod_x, (string) y,true);
				}
				else
				{
					string cod_y = ((UOSmistamento)y).Codice;
					return String.Compare ((string) x, cod_y,true);
				}
			}

			#endregion
		}

		#endregion

		#region Metodi di supporto

		private bool filtra_ruoli_utenti_smistamento (DocsPAWA.DocsPaWR.ElementoRubrica er)
		{
			string pcode = null;
			DocsPaWR.Corrispondente rcorr = UserManager.GetCorrispondenteInterno(_page, er.codice, true);
			if (rcorr == null)
				return false;

			if (er.tipo == "R")
			{
				pcode = ((DocsPAWA.DocsPaWR.Ruolo) rcorr).uo.codiceRubrica;
				if (IsInSmistamento (pcode))
					return false;
			}
			else
			{
				DocsPaWR.Ruolo[] ruoli = ((DocsPAWA.DocsPaWR.Utente) rcorr).ruoli;
				if (ruoli == null || ruoli.Length == 0)
					return false;

				foreach (Ruolo r in ruoli) 
				{
					DocsPaWR.ElementoRubrica err = new ElementoRubrica();
					err.codice = r.codiceRubrica;
					err.interno = true;
					err.tipo = "R";
					err.descrizione = "";
					err.has_children = false;
					bool filtra = filtra_ruoli_utenti_smistamento (err);
					if (filtra)
						return true;
				}
				return false;
			}
			bool result = filtra_smistamento (pcode);
			return result;
		}

		private bool IsInSmistamento (string cod_uo)
		{
			return (Array.BinarySearch (uo_smistamento, cod_uo, new UOSmistamentoByCodiceFinder()) >= 0);
		}

		private bool filtra_smistamento(string cod_uo)
		{
			cod_uo = cod_uo.ToUpper();
			string id_amm = UserManager.getInfoUtente(_page).idAmministrazione;
			if (sf_qco == null) 
			{
				sf_qco = new DocsPAWA.DocsPaWR.ParametriRicercaRubrica();
				UserManager.setQueryRubricaCaller (ref sf_qco);
			}

			//12 gennaio 2007

			bool smistamento_empty = (uo_smistamento == null || uo_smistamento.Length == 0);
			if (!smistamento_empty) 
			{
				bool is_in_smistamento = false;
				bool is_on_ext_reg = false;
				if(uo_smistamento !=  null)
				{
					is_in_smistamento = (Array.BinarySearch (uo_smistamento, cod_uo, new UOSmistamentoByCodiceFinder()) >= 0);
				}
				if(uo_interne != null)
				{
					is_on_ext_reg = (Array.BinarySearch (uo_interne, cod_uo) < 0);
				}
				
				if (_calltype == RubricaCallType.CALLTYPE_PROTO_INT_DEST)
					is_on_ext_reg = false;

				return is_in_smistamento || is_on_ext_reg;
			}
			return true;
		}

		#endregion
	}
}
