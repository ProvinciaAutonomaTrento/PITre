using System;
using System.Data;
using System.Collections;
using DocsPaDB;
using BusinessLogic;
using BusinessLogic.Utenti;
using BusinessLogic.Trasmissioni;
using BusinessLogic.Amministrazione;
using DocsPaVO.utente;
using DocsPaVO.rubrica;
using DocsPaVO.trasmissione;
using DocsPaVO.addressbook;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Rubrica
{
	/// <summary>
	/// Implementazione delle ricerche in Rubrica per la versione 3.0.x di DocsPA
	/// </summary>
	public class DPA3_RubricaSearchAgent  : RubricaSearchAgent
	{
        private static ILog logger = LogManager.GetLogger(typeof(DPA3_RubricaSearchAgent));
		private DocsPaVO.utente.InfoUtente _user;

		private static Hashtable h_utenti = null;
		private static Hashtable h_ruoli = null;
		private static Hashtable h_registri = null;
		private static Hashtable h_uo = null;

		void init_ht()
		{
			if (h_utenti == null)
				h_utenti = UtenteManager.GetRuoliUtenteSemplice (_user.idAmministrazione);
			if (h_ruoli == null)
				h_ruoli = UOManager.GetRuoliUOSemplice (_user.idAmministrazione);
			if (h_registri == null)
				h_registri = RegistriManager.GetRegistriByRuolo (_user.idAmministrazione);
			if (h_uo == null)
				h_uo = UOManager.GetUORuoloSemplice (_user.idAmministrazione);
		}

		public DPA3_RubricaSearchAgent(DocsPaVO.utente.InfoUtente user)
		{
			SearchFilter = new RubricaSearchFilter (DPA3_SearchFilter);
			_user = user;
			init_ht();
		}

		//Metodo per forzare il caricamento della hashtable
		public static void resetHashTable()
		{
			h_utenti = null;
			h_ruoli = null;
			h_registri =  null;
			h_uo = null;
		}

        public override System.Collections.ArrayList Search(DocsPaVO.rubrica.ParametriRicercaRubrica qr, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
		{
			if (qr.caller == null || qr.caller.IdRuolo == "" || qr.caller.IdUtente == "")
				throw new ArgumentException ("Dati mancanti nella definizione dell'utente che ha effettuato la chiamata alla Rubrica");

			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);

			ArrayList ers = r.GetElementiRubrica (qr);

			if (SearchFilter != null)
				SearchFilter (qr, ref ers, smistamentoRubrica);

            //Imposto la visiblità delle checkbox in funzione della disabilitazione/abilitazione alla ricezione delle trasmissioni
            setVisibleCheckBoxElement(qr, ref ers);

            //Se selezionato il filtro località non devo andare in rubrica comune perchè non esiste la località in rubrica comune
            if (qr.localita == null || qr.localita == "")
            {

            }
            else
            {
                qr.doRubricaComune = false;
            }

            if (this.RicercaRubricaComune(qr)) 
            {
                // Ricerca, in base agli stessi criteri di filtro, nel sistema rubrica comune
                DocsPaVO.RubricaComune.FiltriRubricaComune filtroRubricaComune = new DocsPaVO.RubricaComune.FiltriRubricaComune();
                if (qr.codice != null && qr.codice != "")
                {
                    filtroRubricaComune.Codice = qr.codice.Replace("'", "''").TrimEnd();
                }
                else
                {
                    filtroRubricaComune.Codice = qr.codice;
                }
                if (!string.IsNullOrEmpty(qr.descrizione))
                {
                    filtroRubricaComune.Descrizione = qr.descrizione.Replace("'", "''").TrimEnd();
                }
                else
                {
                    filtroRubricaComune.Descrizione = qr.descrizione;
                }
                if (!string.IsNullOrEmpty(qr.citta))
                {
                    filtroRubricaComune.Citta = qr.citta.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.Citta = qr.citta;
                }

                if (!string.IsNullOrEmpty(qr.email))
                {
                    filtroRubricaComune.Mail = qr.email.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.Mail = qr.email;
                }
                if (!string.IsNullOrEmpty(qr.codiceFiscale))
                {
                    filtroRubricaComune.CodiceFiscale = qr.codiceFiscale.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.CodiceFiscale = qr.codiceFiscale;
                }
                if (!string.IsNullOrEmpty(qr.partitaIva))
                {
                    filtroRubricaComune.PartitaIva = qr.partitaIva.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.PartitaIva = qr.partitaIva;
                }
                //if (!string.IsNullOrEmpty(qr.localita))
                //{
                //    filtroRubricaComune.Localita = qr.localita.Replace("'", "''");
                //}
                //else
                //{
                //    filtroRubricaComune.Localita = qr.localita;
                //}
                
                // Indica se ricercare o meno per parola intera
                filtroRubricaComune.RicercaParolaIntera = qr.queryCodiceEsatta;

                ICollection c = RubricaComune.RubricaServices.GetElementiRubricaComune(this._user, filtroRubricaComune);

                if (c != null && c.Count > 0)
                {
                    ers.AddRange(c);

                    // Ordinamento dell'array, a seguito dell'inserimento dei dati dalla rubrica comune
                    ers.Sort(new DocsPaVO.rubrica.ElementoRubrica.ElementoRubricaComparer());
                }
            }

			return ers;
		}

        public override System.Collections.ArrayList SearchPaging(DocsPaVO.rubrica.ParametriRicercaRubrica qr, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, int firstRowNum, int maxRowForPage, out int totale)
		{
			if (qr.caller == null || qr.caller.IdRuolo == "" || qr.caller.IdUtente == "")
				throw new ArgumentException ("Dati mancanti nella definizione dell'utente che ha effettuato la chiamata alla Rubrica");

			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);

			ArrayList ers = r.GetElementiRubricaPaging (qr, firstRowNum, maxRowForPage, out totale);

			if (SearchFilter != null)
				SearchFilter (qr, ref ers, smistamentoRubrica);

            //Imposto la visiblità delle checkbox in funzione della disabilitazione/abilitazione alla ricezione delle trasmissioni
            setVisibleCheckBoxElement(qr, ref ers);

            //Se selezionato il filtro località non devo andare in rubrica comune perchè non esiste la località in rubrica comune
            if (qr.localita == null || qr.localita == "")
            {

            }
            else
            {
                qr.doRubricaComune = false;
            }

            if (this.RicercaRubricaComune(qr)) 
            {
                // Ricerca, in base agli stessi criteri di filtro, nel sistema rubrica comune
                DocsPaVO.RubricaComune.FiltriRubricaComune filtroRubricaComune = new DocsPaVO.RubricaComune.FiltriRubricaComune();
                if (qr.codice != null && qr.codice != "")
                {
                    filtroRubricaComune.Codice = qr.codice.Replace("'", "''").TrimEnd();
                }
                else
                {
                    filtroRubricaComune.Codice = qr.codice;
                }
                if (!string.IsNullOrEmpty(qr.descrizione))
                {
                    filtroRubricaComune.Descrizione = qr.descrizione.Replace("'", "''").TrimEnd();
                }
                else
                {
                    filtroRubricaComune.Descrizione = qr.descrizione;
                }
                if (!string.IsNullOrEmpty(qr.citta))
                {
                    filtroRubricaComune.Citta = qr.citta.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.Citta = qr.citta;
                }

                if (!string.IsNullOrEmpty(qr.email))
                {
                    filtroRubricaComune.Mail = qr.email.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.Mail = qr.email;
                }
                if (!string.IsNullOrEmpty(qr.codiceFiscale))
                {
                    filtroRubricaComune.CodiceFiscale = qr.codiceFiscale.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.CodiceFiscale = qr.codiceFiscale;
                }
                if (!string.IsNullOrEmpty(qr.partitaIva))
                {
                    filtroRubricaComune.PartitaIva = qr.partitaIva.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.PartitaIva = qr.partitaIva;
                }
                //if (!string.IsNullOrEmpty(qr.localita))
                //{
                //    filtroRubricaComune.Localita = qr.localita.Replace("'", "''");
                //}
                //else
                //{
                //    filtroRubricaComune.Localita = qr.localita;
                //}
                
                // Indica se ricercare o meno per parola intera
                filtroRubricaComune.RicercaParolaIntera = qr.queryCodiceEsatta;

                ICollection c = RubricaComune.RubricaServices.GetElementiRubricaComune(this._user, filtroRubricaComune);

                if (c != null && c.Count > 0)
                {
                    ers.AddRange(c);

                    // Ordinamento dell'array, a seguito dell'inserimento dei dati dalla rubrica comune
                    ers.Sort(new DocsPaVO.rubrica.ElementoRubrica.ElementoRubricaComparer());

                    totale = totale + c.Count;
                }
            }

            //Da eliminare quando sarà effettiva la paginazione
            //il totale record ritornati viene modificato dalla funzione setVisibleCheckBoxElement(qr, ref ers);
            //quando la paginazione sarà implementata, la funzione setVisibleCheckBoxElement 
            //dovrà essere sostituita includendo le esclusioni nella query. 
            totale = ers.Count;

			return ers;
		}

        /// <summary>
        /// Verifica se, in base ai criteri di filtro impostati, è richiesta
        /// la ricerca degli elementi nel sistema rubrica comune
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        protected bool RicercaRubricaComune(DocsPaVO.rubrica.ParametriRicercaRubrica qr)
        {
            return ((qr.tipoIE == TipoUtente.GLOBALE || qr.tipoIE == TipoUtente.ESTERNO) && qr.doRubricaComune);
        }

		public override DocsPaVO.rubrica.ElementoRubrica SearchSingle(string codice, DocsPaVO.rubrica.SmistamentoRubrica smistaRubrica, string condRegistri)
		{
			DocsPaVO.rubrica.ElementoRubrica er = new ElementoRubrica();
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			er = r.GetElementoRubrica (codice, condRegistri);
            
			ArrayList ers = new ArrayList();
			ers.Add(er);
            
			if (smistaRubrica!=null  && smistaRubrica.smistamento=="1")
			{
			 //caso in cui è abilitato lo smistamento, devo quindi filtrare i corrispondenti
			 //ma solo per determinati callType

				if((smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT)
                   || (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
					|| (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST)
					|| (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
					|| (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
				{
					filtraPrimoSmistamento(smistaRubrica.idRegistro,smistaRubrica.ruoloProt.systemId,smistaRubrica.infoUt,ref ers);
				}
			}

            if (er == null)
            {
                // Ricerca elemento rubrica per codice in rubrica comune
                er = RubricaComune.RubricaServices.GetElementoRubricaComune(this._user, codice, true);
            }

			return er;
		}

        //public override ArrayList SearchForCorr(string codice, DocsPaVO.rubrica.SmistamentoRubrica smistaRubrica)
        //{
        //    ArrayList ers = new ArrayList();
        //    DocsPaVO.rubrica.ElementoRubrica er = new ElementoRubrica();
        //    DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
        //    foreach (ElementoRubrica e in r.GetElementiRubricaCorr(codice))
        //    {
        //        ers.Add(e);
        //    }


        //    //ers.Add(er);

        //    if (smistaRubrica != null && smistaRubrica.smistamento == "1")
        //    {
        //        //caso in cui è abilitato lo smistamento, devo quindi filtrare i corrispondenti
        //        //ma solo per determinati callType

        //        if ((smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT)
        //           || (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
        //            || (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST)
        //            || (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
        //            || (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
        //        {
        //            filtraPrimoSmistamento(smistaRubrica.idRegistro, smistaRubrica.ruoloProt.systemId, smistaRubrica.infoUt, ref ers);
        //        }
        //    }

        //    if (ers.Count == 0)
        //    {
        //        // Ricerca elemento rubrica per codice in rubrica comune
        //        er = RubricaComune.RubricaServices.GetElementoRubricaComune(this._user, codice);
        //    }

        //    return ers;
        //}

		public override DocsPaVO.rubrica.ElementoRubrica SearchSingleSimple(string codice)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			DocsPaVO.rubrica.ElementoRubrica er = r.GetElementoRubricaSimple (codice);

            if (er == null)
            {
                // Ricerca elemento rubrica per codice in rubrica comune
                er = RubricaComune.RubricaServices.GetElementoRubricaComune(this._user, codice, true);
            }

            return er;
		}

        public override DocsPaVO.rubrica.ElementoRubrica SearchSingleSimpleBySystemId(string systemId)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            return r.GetElementoRubricaSimpleBySystemId(systemId);
        }

        public override ArrayList GetChildrenElement(string elementID, string childrensType)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            return r.GetChildrenElement(elementID, childrensType);
        }

		public override ArrayList GetHierarchy(string codice, DocsPaVO.addressbook.TipoUtente tipoIE,DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			ArrayList ers = r.GetGerarchiaElemento (codice, tipoIE);
			if (smistamentoRubrica!=null  && smistamentoRubrica.smistamento=="1")
			{
				//caso in cui è abilitato lo smistamento, devo quindi filtrare i corrispondenti
				//ma solo per determinati callType

				if((smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT)
                       || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
					|| (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST)
					|| (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
					|| (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
				{
					filtraPrimoSmistamento(smistamentoRubrica.idRegistro,smistamentoRubrica.ruoloProt.systemId,smistamentoRubrica.infoUt,ref ers);
				}
			}

            setVisibleCheckBoxElement(smistamentoRubrica, ref ers);

            return ers;
		}

		public override void CheckChildrenExistence(ref DocsPaVO.rubrica.ElementoRubrica[] ers, string idAmm)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			r.CheckChildrenExistence (ref ers, idAmm);
		}

		public override void CheckChildrenExistence(ref DocsPaVO.rubrica.ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, string idAmm)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			r.CheckChildrenExistence (ref ers, checkUo, checkRuoli, checkUtenti, idAmm);
		}


		public override ArrayList GetHierarchyRange(string[] codici, DocsPaVO.addressbook.TipoUtente[] tipiIE)
		{
			throw new NotImplementedException("GetHierarchyRange");
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codici"></param>
        /// <param name="tipoIE"></param>
        /// <returns></returns>
		public override ArrayList  SearchRange(string[] codici, DocsPaVO.addressbook.TipoUtente tipoIE)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			
            ArrayList list = r.SearchRange (codici, tipoIE);

            if (RubricaComune.Configurazioni.GetConfigurazioni(this._user).GestioneAbilitata)
            {
                // Ricerca degli elementi in rubrica comune
                foreach (string codice in codici)
                {
                    DocsPaVO.rubrica.ElementoRubrica item = RubricaComune.RubricaServices.GetElementoRubricaComune(this._user, codice, true);

                    if (item != null)
                        list.Add(item);
                }
            }

            return list;
		}

		private void DPA3_SearchFilter (DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers,DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
		{
			if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST) ||
				(qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_MITT) ||
				(qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_DEST_MODELLO_TRASM)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_UFFREF_PROTO)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_UTENTE_REG_NOMAIL)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI))
			{
				filtra_aoo (qr, ref ers);
				if(smistamentoRubrica!= null && smistamentoRubrica.smistamento.Equals("1"))
				{
					if((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST))
					{
                        //if(!filtraPrimoSmistamentoElenco(qr,this._user,ref ers))
                        if(!filtraPrimoSmistamento(qr.caller.IdRegistro,qr.caller.IdRuolo,this._user,ref ers))
						{
							ers = null;
						}
					}
				}
			}

			if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL) ||
				(qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF) ||
				(qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP) ||
				(qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO))
					filtra_trasmissioni (qr, ref ers); 

			if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL) ||
				(qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF) ||
				(qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP) ||
				(qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO))
                if(qr.ObjectType != null)
				    filtra_trasmissioni (qr, ref ers);

			if((((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                || qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI)           
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INGRESSO) 
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN_INT)
        		|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_UFFREF)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_GESTFASC_UFFREF)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_GESTFASC_LOCFISICA)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_FILTRIRICFASC_LOCFIS)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO)
				|| (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA && qr.caller.IdRegistro!=null)
				)
				&& ((qr.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO)|| (qr.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)))
                || (qr.calltype== ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE && qr.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE))
			{
				filtra_aoo (qr, ref ers);
				if(smistamentoRubrica!= null && smistamentoRubrica.smistamento.Equals("1"))
				{
					if(qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                        || qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI 
                        || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO))
					{
						//if(!filtraPrimoSmistamentoElenco(qr,this._user,ref ers))
                        if(!filtraPrimoSmistamento(qr.caller.IdRegistro,qr.caller.IdRuolo,this._user,ref ers))
						{
							ers = null;
						}
					}
				
					if(smistamentoRubrica!= null && smistamentoRubrica.smistamento.Equals("1"))
					{
						if((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA && qr.caller.IdRegistro!=null))
						{
							if(!filtraPrimoSmistamento(qr.caller.IdRegistro,qr.caller.IdRuolo,this._user,ref ers))
							{
								ers = null;
							}
						}
					}
				}

			}
			if(smistamentoRubrica!= null && smistamentoRubrica.smistamento.Equals("1"))
			{
				//viene effettuata quando si ricerca (tab elenco) su protocollo uscita nel caso di destinatario globale
                if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT || qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)) && qr.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE)
				{
					//if(!filtraPrimoSmistamentoElenco(qr,this._user,ref ers))
                    if (!filtraPrimoSmistamento(qr.caller.IdRegistro,qr.caller.IdRuolo,this._user,ref ers))
					{
						ers = null;
					}
				}
				
				if((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
				{
					filtraPrimoSmistamento(qr.caller.IdRegistro,qr.caller.IdRuolo,this._user,ref ers);
				}
			}

			//Commentato perchè adesso si filtra con la query direttamente
			//if(qr.descrizione!=null && qr.descrizione!=string.Empty)
			//	filtraListe(qr, ref ers);
		}

		/*Commentato perchè adesso si filtra con la query direttamente
		private void filtraListe(DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers)
		{
			DataSet ds = null;
			string idUtente = this._user.idPeople;
			
			if(qr.descrizione==null || qr.descrizione=="")
				ds = ListeDistribuzione.getListe(idUtente,this._user.idAmministrazione,"%");			
			else
				ds = ListeDistribuzione.getListe(idUtente,this._user.idAmministrazione,qr.descrizione);			
			
			ArrayList a = new ArrayList();
			
			for(int i=0; i<ers.Count; i++)
			{
				DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica) ers[i];
				if(er.tipo != "L")
				{
					a.Add(er);						
				}
			}
			ers = a;
			
			if(qr.doListe && ds != null)
			{
				for(int i=0; i<ds.Tables[0].Rows.Count; i++)
				{
					DocsPaVO.rubrica.ElementoRubrica er = new DocsPaVO.rubrica.ElementoRubrica();
					er.tipo = "L";
					er.codice = ds.Tables[0].Rows[i][0].ToString();
					er.descrizione = ds.Tables[0].Rows[i][1].ToString();
					er.has_children = false;
					er.interno = false;
					ers.Add(er);				
				}
			}
		}
		*/

		public override ArrayList GetRootItems(DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			ArrayList ers = new ArrayList();
			ers = r.GetRootItems (tipoIE);

			if (smistamentoRubrica!=null  && smistamentoRubrica.smistamento=="1")
			{
				//caso in cui è abilitato lo smistamento, devo quindi filtrare i corrispondenti
				//ma solo per determinati callType
				if((smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
					|| (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST)
					|| (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
					|| (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
				{
					filtraPrimoSmistamento(smistamentoRubrica.idRegistro,smistamentoRubrica.ruoloProt.systemId,smistamentoRubrica.infoUt,ref ers);			
				
				}
			}
			return 	ers;		
		}


		#region Metodi di supporto
		private bool ruolo_is_autorizzato (string codice, string[] ruoli_autorizzati)
		{
			return (Array.BinarySearch (ruoli_autorizzati, codice) >= 0);
		}

		private bool utente_is_autorizzato (string _codice, string[] ruoli_autorizzati)
		{
			string codice = _codice;

			if (_codice.StartsWith (@"E\") || _codice.StartsWith (@"I\"))
				codice = _codice.Substring (_codice.IndexOf (@"\") + 1);

//			if (h_utenti == null)
//				h_utenti = UtenteManager.GetRuoliUtenteSemplice (_user.idAmministrazione);

			string[] ruoli = (string[]) h_utenti[codice];
			if (ruoli != null && ruoli.Length > 0)
				foreach (string cod_ruolo in ruoli)
					if (ruolo_is_autorizzato (cod_ruolo, ruoli_autorizzati))
						return true;

			return false;
		}	

		
 
		private bool uo_is_autorizzata (string _codice, DocsPaVO.utente.Ruolo ruolo_caller, string[] ruoli_autorizzati)
		{
			string codice = _codice;

			if (_codice.StartsWith (@"E\") || _codice.StartsWith (@"I\"))
				codice = _codice.Substring (_codice.IndexOf (@"\") + 1);

//			if (h_ruoli == null)
//				h_ruoli = UOManager.GetRuoliUOSemplice (_user.idAmministrazione);

			string[] ruoli = (string[]) h_ruoli[codice];
			if (ruoli != null && ruoli.Length > 0)
				foreach (string cod_ruolo in ruoli)
					if (ruolo_is_autorizzato (cod_ruolo, ruoli_autorizzati))
						return true;

			return false;
		}	

		private bool ruolo_has_registro (DocsPaVO.utente.Ruolo r, string reg_id)
		{
			try 
			{
				foreach (DocsPaVO.utente.Registro reg in r.registri) 
				{
					if (reg.systemId == reg_id)
						return true;
				}
			}
			catch { return false; }
			return false;
		}

		private void filtra_trasmissioni (DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers)
		{
			ArrayList a = new ArrayList();
			ArrayList ruoli_autorizzati = new ArrayList();

			TipoOggetto tipo_oggetto;
            tipo_oggetto = (qr.ObjectType != null && qr.ObjectType.StartsWith("F:")) ? TipoOggetto.FASCICOLO : TipoOggetto.DOCUMENTO;
			string id_nodo_titolario = (tipo_oggetto == TipoOggetto.FASCICOLO) ? qr.ObjectType.Substring(2) : null;

			DocsPaVO.utente.Ruolo r = UserManager.getRuolo (qr.caller.IdRuolo);
			DocsPaDB.Utils.Gerarchia g = new DocsPaDB.Utils.Gerarchia();

			switch (qr.calltype) 
			{
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL:
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL:
					ruoli_autorizzati =  g.getRuoliAut (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
					break;
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP:
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP:
					ruoli_autorizzati = g.getGerarchiaSup (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
					break;
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF:
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF:
					ruoli_autorizzati = g.getGerarchiaInf (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
					break;
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO:
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO:
					ruoli_autorizzati = g.getGerarchiaPariLiv (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
					break;
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST:
					RagioneTrasmissione rTo = RagioniManager.GetRagione("TO", _user.idAmministrazione);
					TipoGerarchia gTo = rTo.tipoDestinatario;

				switch (gTo) 
				{
					case TipoGerarchia.INFERIORE:
						ruoli_autorizzati = g.getGerarchiaInf (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
						break;

					case TipoGerarchia.PARILIVELLO:
						ruoli_autorizzati = g.getGerarchiaPariLiv (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
						break;

					case TipoGerarchia.SUPERIORE:
						ruoli_autorizzati = g.getGerarchiaSup (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
						break;

					case TipoGerarchia.TUTTI:
						ruoli_autorizzati =  g.getRuoliAut (r, qr.caller.IdRegistro, null, tipo_oggetto);
						break;
				}
					break;

				default:
					return;
			}
			string[] c_ruoli_aut = new string[ruoli_autorizzati.Count];
			for (int i = 0; i < ruoli_autorizzati.Count; i++)
				c_ruoli_aut[i] = ((DocsPaVO.utente.Ruolo) ruoli_autorizzati[i]).codiceRubrica;

			Array.Sort (c_ruoli_aut);

			foreach (DocsPaVO.rubrica.ElementoRubrica er in ers) 
			{
				try 
				{
					switch (er.tipo) 
					{
						case "U":
							if (uo_is_autorizzata ((er.interno ? "I" : "E") + @"\" + er.codice, r, c_ruoli_aut) || (qr.ObjectType == "G"))
								a.Add (er);
							break;

						case "R":
							if (ruolo_is_autorizzato (er.codice, c_ruoli_aut) || (qr.ObjectType == "G"))
								a.Add (er);
							break;

						case "P":
							if (utente_is_autorizzato ((er.interno ? "I" : "E") + @"\" + er.codice, c_ruoli_aut) || (qr.ObjectType == "G"))
								a.Add (er);
							break;
						case "L":
                        case "F":
							//if (utente_is_autorizzato ((er.interno ? "I" : "E") + @"\" + er.codice, c_ruoli_aut) || (qr.ObjectType == "G"))
								a.Add (er);
							break;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.Message);
				}
			}
			ers = a;
		}

		public ArrayList filtra_trasmissioniPerListe (DocsPaVO.rubrica.ParametriRicercaRubrica qr, ElementoRubrica[] ers)
		{
			ArrayList a = new ArrayList();
			ArrayList ruoli_autorizzati = new ArrayList();

			TipoOggetto tipo_oggetto;
			tipo_oggetto = (qr.ObjectType.StartsWith("F:")) ? TipoOggetto.FASCICOLO : TipoOggetto.DOCUMENTO;
			string id_nodo_titolario = (tipo_oggetto == TipoOggetto.FASCICOLO) ? qr.ObjectType.Substring(2) : null;

			DocsPaVO.utente.Ruolo r = UserManager.getRuolo (qr.caller.IdRuolo);
			DocsPaDB.Utils.Gerarchia g = new DocsPaDB.Utils.Gerarchia();

			switch (qr.calltype) 
			{
				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL:
					ruoli_autorizzati =  g.getRuoliAut (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
					break;

				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP:
					ruoli_autorizzati = g.getGerarchiaSup (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
					break;

				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF:
					ruoli_autorizzati = g.getGerarchiaInf (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
					break;

				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO:
					ruoli_autorizzati = g.getGerarchiaPariLiv (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
					break;

				case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST:
					RagioneTrasmissione rTo = RagioniManager.GetRagione("TO", _user.idAmministrazione);
					TipoGerarchia gTo = rTo.tipoDestinatario;

				switch (gTo) 
				{
					case TipoGerarchia.INFERIORE:
						ruoli_autorizzati = g.getGerarchiaInf (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
						break;

					case TipoGerarchia.PARILIVELLO:
						ruoli_autorizzati = g.getGerarchiaPariLiv (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
						break;

					case TipoGerarchia.SUPERIORE:
						ruoli_autorizzati = g.getGerarchiaSup (r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
						break;

					case TipoGerarchia.TUTTI:
						ruoli_autorizzati =  g.getRuoliAut (r, qr.caller.IdRegistro, null, tipo_oggetto);
						break;
				}
					break;

				default:
					return a;
			}
			string[] c_ruoli_aut = new string[ruoli_autorizzati.Count];
			for (int i = 0; i < ruoli_autorizzati.Count; i++)
				c_ruoli_aut[i] = ((DocsPaVO.utente.Ruolo) ruoli_autorizzati[i]).codiceRubrica;

			Array.Sort (c_ruoli_aut);

			foreach (DocsPaVO.rubrica.ElementoRubrica er in ers) 
			{
				try 
				{
					switch (er.tipo) 
					{
						case "U":
							if (uo_is_autorizzata ((er.interno ? "I" : "E") + @"\" + er.codice, r, c_ruoli_aut) || (qr.ObjectType == "G"))
								a.Add (er);
							break;

						case "R":
							if (ruolo_is_autorizzato (er.codice, c_ruoli_aut) || (qr.ObjectType == "G"))
								a.Add (er);
							break;

						case "P":
							if (utente_is_autorizzato ((er.interno ? "I" : "E") + @"\" + er.codice, c_ruoli_aut)
								|| ((qr.ObjectType == "G") && (er.interno==true)))
								a.Add (er);
							break;
						case "L":
                        case "F":
							//if (utente_is_autorizzato ((er.interno ? "I" : "E") + @"\" + er.codice, c_ruoli_aut) || (qr.ObjectType == "G"))
							a.Add (er);
							break;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.Message);
				}
			}
			//ers = a;
			return a;
		}


		// **** Nuovo filtro AOO ********************************************

		string[] uo_in_aoo;
		string[] uo_in_aoo_with_reg;

		private bool check_uo (string cod_uo)
		{
			int i;
			
			i = Array.BinarySearch(uo_in_aoo, cod_uo);
			return (i >= 0);
		}

		private bool check_uo_withReg (string currentCodUo)
		{
			int i;
			
			i = Array.BinarySearch(uo_in_aoo_with_reg, currentCodUo);
			return (i >= 0);
		}

		private bool check_ruoli_utenti (ElementoRubrica er)
		{
			string cod_uo = null;
			if (er.tipo == "R")
			{
				cod_uo = (string) h_uo[er.codice];
			}
			else
			{
				string[] ruoli = (string[]) h_utenti[er.codice];
				if (ruoli == null || ruoli.Length == 0)
					return false;

				foreach (string rcod in ruoli) 
				{
					ElementoRubrica err = new ElementoRubrica();
					err.codice = rcod;
					err.interno = true;
					err.tipo = "R";
					err.descrizione = "";
					err.has_children = false;
					if (check_ruoli_utenti (err))
						return true;
				}
				return false;
			}
			return check_uo (cod_uo);
		}


		/// <summary>
		/// Metodo utilizzato per filtrare i corrispondenti qualora sia abilitata la funzionalità
		/// di smistamento in DocsPA. In particolare viene settata a true o false la proprietà
		/// dell'elemento rubrica 'isVisibile' a seconda che siano verificate o meno le condizioni
		/// relative allo smistamento
		/// </summary>
		/// <param name="idRegistro">systemId del registro corrente</param>
		/// <param name="idRuolo">systemId del ruolo corrente, loggato</param>
		/// <param name="utente">InfoUtente dell'utente loggato</param>
		/// <param name="ers">ArrayList di elementi rubrica da filtrare</param>
		/// <returns></returns>
		private bool filtraPrimoSmistamento(string idRegistro, string idRuolo, DocsPaVO.utente.InfoUtente utente, ref ArrayList ers)
		{
			ArrayList listaUoSmistamento = new ArrayList();
            Hashtable tableUoInterneAOO = new Hashtable();
			bool retvalue = true;
			try
			{
//				DocsPaVO.Smistamento.MittenteSmistamento mittSmistamento = new DocsPaVO.Smistamento.MittenteSmistamento();
//				mittSmistamento.IDPeople = "0";
//				listaUoSmistamento =BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetListUOSmistamento(idRegistro,mittSmistamento);
				
				//string cod_uo = qr.codice;
				//string uoCode=null;
                DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(_user);
                uo_in_aoo = r.GetUoInterneAoo(idRegistro);
                if (uo_in_aoo != null)
                {
                    Array.Sort(uo_in_aoo, CaseInsensitiveComparer.Default);
                    foreach (string item in uo_in_aoo)
                    {
                        if (!tableUoInterneAOO.ContainsKey(item))
                            tableUoInterneAOO.Add(item, item);
                    }
                }

				listaUoSmistamento = GetListaUOSmistamentoRubrica(idRegistro);

				DocsPaVO.utente.Corrispondente corr = null;
				bool smistamento_empty = (listaUoSmistamento == null || listaUoSmistamento.Count == 0);
				
				if(!smistamento_empty)
				{
					//prendo la Uo relativa al ruolo che sta protocollando
					DocsPaVO.utente.Corrispondente ruoloProt =  UserManager.getCorrispondenteCompletoBySystemId(idRuolo,DocsPaVO.addressbook.TipoUtente.INTERNO,utente); 

					// listaUoSmistamento in hashtable
					// codice: codice uo
					// valore: oggetto uo smistamento
					Hashtable tableUoSmistamento=new Hashtable();
					foreach (DocsPaVO.Smistamento.UOSmistamento item in listaUoSmistamento)
					{
						if (!tableUoSmistamento.ContainsKey(item.Codice))
							tableUoSmistamento.Add(item.Codice,item);
					}

                    string idUoAppartenenza = getIdUoAppartenenza
                        (((DocsPaVO.utente.Ruolo)ruoloProt).uo.codiceRubrica);
					for (int i = 0; i < ers.Count; i++)
					{
						DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica) ers[i];
						
						if(er!=null)
						{
							if(!er.interno)
							{
								continue;
							}
							switch(er.tipo) //(manca il controllo per le UO sottoposte)
							{
								case "U":  
								{

                                    if (er.isVisibile && tableUoInterneAOO.Contains(er.codice))
									{
                                        if (!verificaDipendenzaCodRubrica(idUoAppartenenza, er.codice))
										{
											//caso in cui la Uo del ruolo è SUPERIORE a quella del protocollatore
											if (!tableUoSmistamento.ContainsKey(er.codice))
											{
												//vuol dire che l'elemento rubrica NON è nella DPA_UO_SMISTAMENTO
												//quindi NON deve essere selezionabile
												er.isVisibile = false;
											}
			
										}
									}
								}
								
									break;
								
								case "R":
								{
									//corr = UserManager.getCorrispondenteByCodRubrica(er.codice,utente);
									//uoCode = ((DocsPaVO.utente.Ruolo) corr).uo.codiceRubrica;
		
									if(er.isVisibile && check_ruoli_utenti(er))
									{
                                        if (!verificaDipendenzaCodRubrica(idUoAppartenenza, er.codice))
										{
											//caso in cui la Uo del ruolo è SUPERIORE a quella del ruolo che protocolla.
											er.isVisibile = false;
										}
									}
								}

									break;

								case "P":
								{

									bool SelectorVisibility = false;
                                    if (!check_ruoli_utenti(er))
                                    { 
                                        SelectorVisibility = true;
                                    }
                                    else
                                    {
                                        if (er.isVisibile)
                                        {
                                            //se la Uo è sottoposta a quella del protocollista allora rendo visibile l'utente
                                            if (verificaDipendenzaCodRubrica(idUoAppartenenza, er.codice))
                                            {

                                                SelectorVisibility = true;

                                            }
                                            er.isVisibile = SelectorVisibility;

                                        }
									}
									break;
								
								}
							}
							
						}	
						
					}
				
				}
			}
			catch(Exception ex)
			{
				retvalue  = false;
				Console.WriteLine ("Errore durante il reperimento delle Uo in Primo Smistamento: " + ex.Message);
			}
			return retvalue;
		}

		/// <summary>
		/// Metodo utilizzato per il filtraggio degli elementi rubrica su ricerca (TAB ELENCO)
		/// La diversità tra TAB ORGANIGRAMMA e TAB ELENCO dipende dal fatto che nel tab elenco
		/// la verifica se una Uo sta sotto o sopra la Uo del protocollatore viene fatta da FronEnd
		/// 
		/// </summary>
		/// <param name="qr"></param>
		/// <param name="utente"></param>
		/// <param name="ers"></param>
		/// <returns></returns>
		private bool filtraPrimoSmistamentoElenco(DocsPaVO.rubrica.ParametriRicercaRubrica qr, DocsPaVO.utente.InfoUtente utente, ref ArrayList ers)
		{
			ArrayList listaUoSmistamento = new ArrayList();
			bool retvalue = true;
			try
			{
				DocsPaVO.Smistamento.MittenteSmistamento mittSmistamento = new DocsPaVO.Smistamento.MittenteSmistamento();
				mittSmistamento.IDPeople = "0";
				//listaUoSmistamento =BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetListUOSmistamento(qr.caller.IdRegistro,mittSmistamento);
				
				listaUoSmistamento = GetListaUOSmistamentoRubrica(qr.caller.IdRegistro);
				
				string cod_uo = qr.codice;

				bool smistamento_empty = (listaUoSmistamento == null || listaUoSmistamento.Count == 0);
                DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(_user);
				uo_in_aoo = r.GetUoInterneAoo(qr.caller.IdRegistro);
                if (uo_in_aoo!= null)
                    Array.Sort(uo_in_aoo, CaseInsensitiveComparer.Default);

				if(!smistamento_empty)
				{
					//prendo la Uo relativa al ruolo che sta protocollando
					DocsPaVO.utente.Corrispondente ruoloProt =  UserManager.getCorrispondenteCompletoBySystemId(qr.caller.IdRuolo,qr.tipoIE,utente); 

					// listaUoSmistamento in hashtable
					// codice: codice uo
					// valore: oggetto uo smistamento
					Hashtable tableUoSmistamento=new Hashtable();
					foreach (DocsPaVO.Smistamento.UOSmistamento item in listaUoSmistamento)
					{
						if (!tableUoSmistamento.ContainsKey(item.Codice))
							tableUoSmistamento.Add(item.Codice,item);
					}

					for (int i = 0; i < ers.Count; i++)
					{
						DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica) ers[i];
						
						if(er!=null)
						{
                            //if (!er.interno || !check_uo(er.codice))
                            //{
                            //    break;
                            //}
							switch(er.tipo) //(manca il controllo per le UO sottoposte)
							{
								case "U":  
									if(er.isVisibile)
									{
									    if (!er.interno || !check_uo(er.codice))
                                        {
                                            continue;
                                        }
										//caso in cui la Uo del ruolo è SUPERIORE a quella del protocollatore
										if (!tableUoSmistamento.ContainsKey(er.codice))
										{
											//vuol dire che l'elemento rubrica NON è nella DPA_UO_SMISTAMENTO
											//quindi NON deve essere selezionabile
											er.isVisibile = false;
										}
			
										
									}
									
									break;	
							}
							
						}	
						
					}
				
				}
			}
			catch(Exception ex)
			{
				retvalue  = false;
				Console.WriteLine ("Errore durante il reperimento delle Uo in Primo Smistamento: " + ex.Message);
			}
			return retvalue;
		}


		public static DocsPaVO.utente.Corrispondente getAllRuoli(ArrayList corrispondenti)
		{
			ArrayList listaRuoliUtenti = new ArrayList();
		

			ArrayList listaCorr = new ArrayList();
			
			DocsPaVO.utente.Corrispondente initCorr = (DocsPaVO.utente.Corrispondente)corrispondenti[0];

			
			for(int i=1;i<corrispondenti.Count;i++)
			{
				((DocsPaVO.utente.Utente)initCorr).ruoli.AddRange(((DocsPaVO.utente.Utente)corrispondenti[i]).ruoli);
			}

			return initCorr;
		}

//		public static DocsPaVO.utente.Corrispondente getAllRuoli(DocsPaVO.utente.Corrispondente[] corrispondenti)
//		{
//			string l_oldSystemId="";								
//			System.Object[] l_objects=new System.Object[0];
//			System.Object[] l_objects_ruoli = new System.Object[0];
//			DocsPaVO.utente.Ruolo[] lruolo = new DocsPaVO.utente.Ruolo[0];
//			int i = 0;
//			foreach(DocsPaVO.utente.Corrispondente t_corrispondente in corrispondenti)
//			{
//				string t_systemId=t_corrispondente.systemId;						
//				if (t_systemId!=l_oldSystemId)
//				{
//					l_objects=addToArray(l_objects,t_corrispondente); 	
//					l_oldSystemId=t_systemId;
//					i = i + 1 ;
//					continue;
//				}
//				else
//				{
//					/* il corrispondente non viene aggiunto, in quanto sarebbe un duplicato 
//					 * ma viene aggiunto solamente il ruolo */
//					
//					if(t_corrispondente.GetType().Equals(typeof(DocsPaVO.utente.Utente)) )
//					{
//						if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPaVO.utente.Utente)))
//						{					
//							l_objects_ruoli =((addToArray(((DocsPaVO.utente.Ruolo[])(((DocsPaVO.utente.Utente)(l_objects[i -1])).ruoli)), ((DocsPaVO.utente.Utente)t_corrispondente).ruoli[0])));			
//							DocsPaVO.utente.Ruolo[] l_ruolo=new DocsPaVO.utente.Ruolo[l_objects_ruoli.Length];
//							((DocsPaVO.utente.Utente)(l_objects[i -1])).ruoli = l_ruolo;
//							l_objects_ruoli.CopyTo(((DocsPaVO.utente.Utente)(l_objects[i -1])).ruoli, 0);
//						
//						}
//				
//					}
//				}
//				
//			}
//			
//			DocsPaVO.utente.Corrispondente l_corrSearch=new DocsPaVO.utente.Corrispondente[l_objects.Length];	
//			l_objects.CopyTo(l_corrSearch,0);
//
//			return l_corrSearch;
//		}
//
//		public static Object[] addToArray(Object[] array, Object nuovoElemento) 
//		{
//			Object[] nuovaLista;
//			if (array!=null) 
//			{
//				int len = array.Length;
//				nuovaLista = new Object[len +1];
//				array.CopyTo(nuovaLista,0);
//				nuovaLista[len] = nuovoElemento;
//				return nuovaLista;
//			}
//			else 
//			{
//				nuovaLista=new Object[1];
//				nuovaLista[0] = nuovoElemento;
//				return nuovaLista;
//			}
//		}

		private void filtra_aoo (DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers)
		{
			ArrayList a = new ArrayList();
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(_user);
            if (qr.caller.IdRegistro != null && qr.caller.IdRegistro != string.Empty)
                uo_in_aoo = r.GetUoInterneAoo(qr.caller.IdRegistro);
            else {
                uo_in_aoo = r.GetUoInterneAooNoReg();
            }
			
			// ricerco le UO che sono contenute nella dpa_uo_reg, ovvero
			//quelle uo che hanno almeno un registro associato
			uo_in_aoo_with_reg = r.GetUoInterneAooWithReg(_user.idAmministrazione);

			
			//E' NECESSARIO PRIMA ORDINARE L'ARRAY
			if(uo_in_aoo!=null)
				Array.Sort (uo_in_aoo , CaseInsensitiveComparer.Default);
			
			if(uo_in_aoo_with_reg!=null)
				Array.Sort (uo_in_aoo_with_reg , CaseInsensitiveComparer.Default);


			switch (qr.tipoIE) 
			{
				case DocsPaVO.addressbook.TipoUtente.ESTERNO:

					for (int i = 0; i < ers.Count; i++)
					{
						DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica) ers[i];
						try 
						{
							switch (er.tipo) 
							{
								case "U":
									//se la Uo è interna alla AOO e la UO ha almeno un reg associato
									//la aggiungo nella lista dei corrispondenti
									if ((!er.interno) ||(er.interno && !check_uo (er.codice) && check_uo_withReg(er.codice)))
										a.Add(er);
									break;
								case "R":
								case "P":	
									if (!check_ruoli_utenti (er))
										a.Add(er);
									break;
								case "L":
									a.Add(er);
									break;
							}

					
						}
						catch (Exception ex)
						{
							Console.WriteLine (ex.Message);
						}
					}
					break;

				default:
					for (int i = 0; i < ers.Count; i++)
					{
						DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica) ers[i];
						try 
						{
							switch (er.tipo) 
							{
								case "U":
								{
                                    if ((!er.interno && qr.tipoIE.Equals(DocsPaVO.addressbook.TipoUtente.GLOBALE)) || check_uo(er.codice))
									{
										er.isVisibile = true;
										a.Add(er);
										
									}
									else
									{
										if ((qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
											|| (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO))
										{
											er.isVisibile = false;
											a.Add(er);
										}
										if (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO)
										{
											er.isVisibile = true;
											a.Add(er);
										}
									}
									break;
								}
								case "R":
								case "P":
                                if ((!er.interno && qr.tipoIE.Equals(DocsPaVO.addressbook.TipoUtente.GLOBALE)) || check_ruoli_utenti(er))
									{
										er.isVisibile = true;
										a.Add(er);
									}
									else
									{
										if ((qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
										|| (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO))
										{
											er.isVisibile = false;
											a.Add(er);
										}
										if (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO)
										{
											er.isVisibile = true;
											a.Add(er);
										}
									}
									break;
								case "L":
									a.Add(er);
									break;
							}

					
						}
						catch (Exception ex)
						{
							Console.WriteLine (ex.Message);
						}
					}
					break;
			}							
			
			ers = a;
		}

		private void filtra_aoo_esterni (DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers)
		{
			ArrayList a = new ArrayList();
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(_user);
			uo_in_aoo = r.GetUoInterneAoo(qr.caller.IdRegistro);
			
			//bug: E' NECESSARIO PRIMA ORDINARE L'ARRAY
			if(uo_in_aoo!=null)
				Array.Sort (uo_in_aoo , CaseInsensitiveComparer.Default);

			switch (qr.tipoIE) 
			{
				case DocsPaVO.addressbook.TipoUtente.ESTERNO:

					for (int i = 0; i < ers.Count; i++)
					{
						DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica) ers[i];
						try 
						{
							if(er.interno)
							{
								switch (er.tipo) 
								{
									case "U":
										if (!check_uo (er.codice))
											a.Add(er);
										break;
									case "R":
									case "P":	
										if (!check_ruoli_utenti (er))
											a.Add(er);
										break;
									case "L":
										a.Add(er);
										break;
								}
							}
						}
					
						catch (Exception ex)
						{
							Console.WriteLine (ex.Message);
						}
					
					}
					break;	
			}							
			
			ers = a;
		}

        public static ArrayList GetListaCorrispondenti(DocsPaVO.utente.InfoUtente infoUtente, string registri)
        {
            ArrayList listaCorr = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Rubrica corr = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
            try
            {

                listaCorr = corr.GetListaCorrExport(infoUtente, registri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                listaCorr = null;
            }
            return listaCorr;

        }

        public static ArrayList GetCorrespondentsByFilter(DocsPaVO.rubrica.ParametriRicercaRubrica qr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (qr.caller == null || qr.caller.IdRuolo == "" || qr.caller.IdUtente == "")
                throw new ArgumentException("Dati mancanti nella definizione dell'utente che ha effettuato la chiamata alla Rubrica");

            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);

            ArrayList ers = r.GetCorrespondentsByFilter(qr);

            //Se selezionato il filtro località non devo andare in rubrica comune perchè non esiste la località in rubrica comune
            if (qr.localita == null || qr.localita == "")
            {

            }
            else
            {
                qr.doRubricaComune = false;
            }

            //Se è impostato il filtro di ricerca in rubrica comune
            if ((qr.tipoIE == TipoUtente.GLOBALE || qr.tipoIE == TipoUtente.ESTERNO) && qr.doRubricaComune)
            {
                // Ricerca, in base agli stessi criteri di filtro, nel sistema rubrica comune
                DocsPaVO.RubricaComune.FiltriRubricaComune filtroRubricaComune = new DocsPaVO.RubricaComune.FiltriRubricaComune();
                if (qr.codice != null && qr.codice != "")
                {
                    filtroRubricaComune.Codice = qr.codice.Replace("'", "''").TrimEnd();
                }
                else
                {
                    filtroRubricaComune.Codice = qr.codice;
                }
                if (!string.IsNullOrEmpty(qr.descrizione))
                {
                    filtroRubricaComune.Descrizione = qr.descrizione.Replace("'", "''").TrimEnd();
                }
                else
                {
                    filtroRubricaComune.Descrizione = qr.descrizione;
                }
                if (!string.IsNullOrEmpty(qr.citta))
                {
                    filtroRubricaComune.Citta = qr.citta.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.Citta = qr.citta;
                }

                if (!string.IsNullOrEmpty(qr.email))
                {
                    filtroRubricaComune.Mail = qr.email.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.Mail = qr.email;
                }
                if (!string.IsNullOrEmpty(qr.codiceFiscale))
                {
                    filtroRubricaComune.CodiceFiscale = qr.codiceFiscale.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.CodiceFiscale = qr.codiceFiscale;
                }
                if (!string.IsNullOrEmpty(qr.partitaIva))
                {
                    filtroRubricaComune.PartitaIva = qr.partitaIva.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.PartitaIva = qr.partitaIva;
                }

                // Indica se ricercare o meno per parola intera
                filtroRubricaComune.RicercaParolaIntera = qr.queryCodiceEsatta;

                ICollection c = RubricaComune.RubricaServices.GetElementiRubricaComuneForExport(infoUtente, filtroRubricaComune);

                if (c != null && c.Count > 0)
                {
                    ers.AddRange(c);
                }
            }

            return ers;
        }

        #region IMPORTA RUBRICA
        public static bool ImportaRubrica(DocsPaVO.utente.InfoUtente infoUtente, byte[] dati, int flagListe, string nomeFile, string serverPath, ref int corrInseriti, ref int corrAggiornati, ref int corrRimossi, ref int corrNonInseriti, ref int corrNonAggiornati, ref int corrNonRimossi)
        {
            bool result = true;
            string message = "";

            string storicizza;
            string codRegistro;
            string codRegistroNuovo;
            string corr_type;
            string codRubrica;
            string codAmm;
            string codAOO;
            string descrizione;
            string cognome;
            string nome;
            string indirizzo;
            string cap;
            string citta;
            string provincia;
            string nazione;
            string codiceFiscale;
            string partitaIva;
            string telefono1;
            string telefono2;
            string fax;
            string mail;
            string localita;
            string note;
            string canaleP;
            string[] mailCorri = null;

            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;

            DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog();

            try
            {
                //Controllo se esiste la Directory "Import" nel path dove vengono salvati i modelli per la profilazione dinamica.
                //Se esiste copio il file excel li' dentro, altrimenti la creo e ci copio il file.
                //In ogni caso poichè il nome del file è fisso, anche se quest'ultimo esiste viene sovrascritto.
                logger.Debug("Metodo \"ImportaRubrica\" classe \"UserManager\" : inizio scrittura file \"ImportRubrica.xls\"");
                if (Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                {
                    FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(dati, 0, dati.Length);
                    fs1.Close();
                }
                else
                {
                    Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");
                    FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(dati, 0, dati.Length);
                    fs1.Close();
                }
                logger.Debug("Metodo \"ImportaRubrica\" classe \"DPA3_RubricaSearchAgent\" : fine scrittura file \"ImportaRubrica.xls\"");
                logger.Debug("Metodo \"ImportaRubrica\" classe \"DPA3_RubricaSearchAgent\" : inizio lettura file \"ImportaRubrica.xls\"");
                //Comincio la lettura del file appena scritto
                sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportRubrica");
                sl.Log("Inizio importazione rubrica - " + System.DateTime.Now.ToString());
                //xlsConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1'";
                xlsConn.Open();
                OleDbCommand xlsCmd;

                // Verifica se ci sono utenti con il campo storicizza valido
                xlsCmd = new OleDbCommand("select * from [RUBRICA$] where (Storicizza = 'I' OR Storicizza = 'C' OR Storicizza = 'M')", xlsConn);

                xlsReader = xlsCmd.ExecuteReader();
                //string idAmministrazione = string.Empty;


                // Se la query non ha restituito dei valori scrivo
                // un messaggio nel log
                if (!xlsReader.HasRows)
                    sl.Log("\nNessun record contiene valori validi nel campo Storicizza. Valorizzare tale colonna e ripetere l'operazione.");


                while (xlsReader.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReader, 0) == "/")
                        break;

                    storicizza = get_string(xlsReader, 0);
                    //Verifico che i campi obbligatori "STORICIZZA" sia presente
                    //nel foglio excel, altrimenti ignoro la riga
                    if (storicizza != "")
                    {
                        codRegistro = get_string(xlsReader, 1);
                        codRubrica = get_string(xlsReader, 2);
                        codAmm = get_string(xlsReader, 3);
                        codAOO = get_string(xlsReader, 4);
                        corr_type = get_string(xlsReader, 5);
                        descrizione = get_string(xlsReader, 6);
                        cognome = get_string(xlsReader, 7);
                        nome = get_string(xlsReader, 8);
                        indirizzo = get_string(xlsReader, 9);
                        cap = get_string(xlsReader, 10);
                        citta = get_string(xlsReader, 11);
                        provincia = get_string(xlsReader, 12);
                        nazione = get_string(xlsReader, 13);
                        codiceFiscale = get_string(xlsReader, 14);
                        partitaIva = get_string(xlsReader, 15);
                        telefono1 = get_string(xlsReader, 16);
                        telefono2 = get_string(xlsReader, 17);
                        fax = get_string(xlsReader, 18);
                        mail = get_string(xlsReader, 19);
                        localita = get_string(xlsReader, 20);
                        note = get_string(xlsReader, 21);
                        codRegistroNuovo = get_string(xlsReader, 22);
                        //creo l'oggetto canale
                        DocsPaVO.utente.Canale canale = new DocsPaVO.utente.Canale();
                        //Devo recuperare il system-id del tipo "lettera"
                        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                        canaleP = string.Empty;
                        string messaggio = string.Empty;
                        try
                        {
                            canaleP = get_string(xlsReader, 23);
                        }
                        catch {
                            sl.Log("attenzione, nel foglio excel manca la colonna Canale preferenziale, quindi viene impostato il canale LETTERA. La colonna Canale preferenziale deve essere ultima nel foglio excel.");
                            
                        }

                        if(!string.IsNullOrEmpty(canaleP))
                            canale.systemId = utenti.GetSystemIDCanale(canaleP);
                        else
                        {
                            canale.systemId = utenti.GetSystemIDCanale();
                            canaleP = "LETTERA";
                        }
                        
                        // Recupero l'id dell'amministrazione
                        string idAmm = infoUtente.idAmministrazione;

                        //Devo recuperare il registro in base alla descrizione
                        string id_registro = utenti.GetRegistroDaCodice(codRegistro);
                        if ("0".Equals(id_registro)) id_registro = "";
                        //Nuovo registro
                        string id_registroNuovo = string.Empty;
                        if (!string.IsNullOrEmpty(codRegistroNuovo))
                        {
                            id_registroNuovo = utenti.GetRegistroDaCodice(codRegistroNuovo);
                        }

                        //Valore del campo storicizza:
                        //I: nuovo corrispondente da inserire
                        //M: corrispondente da modificare
                        //C: corrispondente da cancellare

                        //Inserimento nuovo corrispondente
                        #region nuovo corrispondente
                        if (storicizza.ToUpper() == "I")
                        {
                            bool inserisci = true;
                            if (codRubrica == "" ||
                               (corr_type.ToUpper() == "U" && descrizione.Trim() == "") ||
                               (corr_type.ToUpper() == "R" && descrizione.Trim() == "") ||
                               (corr_type.ToUpper() == "P" && (cognome.Trim() == "" || nome.Trim() == ""))
                               )
                            {
                                //sl.Log("");
                                //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                //sl.Log("Attenzione, riempire i campi obbligatori prima di effettuare l'inserimento di un nuovo corrispondente.");
                                messaggio += "\nAttenzione, riempire i campi obbligatori prima di effettuare l'inserimento di un nuovo corrispondente.";
                                inserisci = false;
                            }
                            else
                            {
                                if (corr_type == "" || (corr_type.ToUpper() != "P" && corr_type.ToUpper() != "U" && corr_type.ToUpper() != "R"))
                                {
                                    //sl.Log("");
                                    //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                    //sl.Log("Attenzione, il campo TIPO è obbligatorio.");
                                    messaggio += "\nAttenzione, il campo TIPO non è corretto.";
                                    inserisci = false;
                                }

                                if (id_registro == null)
                                {
                                    //sl.Log("");
                                    //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                    //sl.Log("Attenzione, il campo COD. REGISTRO non è corretto.");
                                    messaggio += "\nAttenzione, il campo COD. REGISTRO non è corretto.";
                                    inserisci = false;
                                }
                                if (!codice_rubrica_valido(codRubrica))
                                {
                                    //sl.Log("");
                                    //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                    //sl.Log("Attenzione, il campo CODICE RUBRICA contiene caratteri non validi.");
                                    messaggio += "\nAttenzione, il campo CODICE RUBRICA contiene caratteri non validi.";
                                    inserisci = false;
                                }
                                //if (mail.Trim() != "" && !IsValidEmail(mail.Trim()))
                                //{
                                //    sl.Log("");
                                //    sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                //    sl.Log("Attenzione, inserire una EMAIL valida");
                                //    inserisci = false;
                                //}

                                //Emanuela: aggiungo il seguente codice per l'inserimento di email multiple
                                mailCorri = mail.Split(';');
                                bool primaEmail = true;
                                foreach (string m in mailCorri)
                                {
                                    string[] split = { "##" };
                                    string[] splitEmailNota = m.Split(split, StringSplitOptions.None);
                                    if (splitEmailNota[0].Trim() != "")
                                    {
                                        int presente = (from mc in mailCorri where mc.Split(split, StringSplitOptions.None).ElementAt(0).Trim().Equals(splitEmailNota[0].Trim()) select mc).Count();
                                        if (presente > 1)
                                        {
                                            messaggio += "\nAttenzione, indirizzo EMAIL duplicato";
                                            inserisci = false;
                                            break;
                                        }
                                    }
                                    if (!splitEmailNota[0].Trim().Equals("") && !IsValidEmail(splitEmailNota[0].Trim()))
                                    {
                                        messaggio += "\nAttenzione, inserire una EMAIL valida";
                                        //sl.Log("");
                                        //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                        //sl.Log("Attenzione, inserire una EMAIL valida");
                                        inserisci = false;
                                        break;
                                    }
                                    if (!splitEmailNota[0].Trim().Equals("") && primaEmail)
                                    {
                                        mail = splitEmailNota[0].Trim();
                                        primaEmail = false;
                                    }
                                }

                                if (codAmm != "" && !codice_rubrica_valido(codAmm))
                                {
                                    //sl.Log("");
                                    //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                    //sl.Log("Attenzione, il campo CODICE AMMINISTRAZIONE contiene caratteri non validi");
                                    messaggio += "\nAttenzione, il campo CODICE AMMINISTRAZIONE contiene caratteri non validi";
                                    inserisci = false;
                                }
                                if (codAOO != "" && !codice_rubrica_valido(codAOO))
                                {
                                    //sl.Log("");
                                    //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                    //sl.Log("Attenzione, il campo CODICE AOO contiene caratteri non validi");
                                    messaggio += "\nAttenzione, il campo CODICE AOO contiene caratteri non validi";
                                    inserisci = false;
                                }
                                if (cap != null && !cap.Equals("") && !isNumeric(cap))
                                {
                                    //sl.Log("");
                                    //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                    //sl.Log("Attenzione, il campo CAP deve essere numerico");
                                    messaggio += "\nAttenzione, il campo CAP deve essere numerico";
                                    inserisci = false;
                                }
                                if (provincia != null && !provincia.Equals("") && !isCorrectProv(provincia))
                                {
                                    messaggio += "\nAttenzione, il campo PROVINCIA contiene caratteri non validi!";
                                    inserisci = false;
                                }
                                if (corr_type.ToUpper().Equals("U"))
                                {
                                    if ((codiceFiscale != null && !codiceFiscale.Trim().Equals("")) && ((codiceFiscale.Trim().Length == 11 && CheckVatNumber(codiceFiscale.Trim()) != 0) || (codiceFiscale.Trim().Length == 16 && CheckTaxCode(codiceFiscale.Trim()) != 0) || (codiceFiscale.Trim().Length != 11 && codiceFiscale.Trim().Length != 16)))
                                    {
                                        messaggio += "\nAttenzione, il campo CODICE FISCALE ha caratteri non validi";
                                        inserisci = false;
                                    }
                                }
                                else
                                    if (codiceFiscale != null && !codiceFiscale.Trim().Equals("") && CheckTaxCode(codiceFiscale.Trim()) != 0)
                                    {
                                        messaggio += "\nAttenzione, il campo CODICE FISCALE ha caratteri non validi";
                                        inserisci = false;
                                    }
                                if (partitaIva != null && !partitaIva.Trim().Equals("") && CheckVatNumber(partitaIva.Trim()) != 0)
                                {
                                    //sl.Log("");
                                    //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                    //sl.Log("Attenzione, il campo PARTITA IVA contiene caratteri non validi");
                                    messaggio += "\nAttenzione, il campo PARTITA IVA contiene caratteri non validi";
                                    inserisci = false;
                                }

                                //Emanuela: controlli per la gestione del Canale Preferenziale
                                switch (canaleP.ToUpper())
                                {
                                    case "MAIL":
                                        if ((mail == null || mail.Trim().Equals("")))
                                        {
                                            //sl.Log("");
                                            //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                            //sl.Log("Attenzione, per CANALE PREFERENZIALE di tipo mail occorre specificare il campo MAIL");
                                            messaggio += "\nAttenzione, per CANALE PREFERENZIALE di tipo mail occorre specificare il campo MAIL";
                                            inserisci = false;
                                            break;
                                        }
                                        break;

                                    case "INTEROPERABILITA":
                                        if ((mail == null || mail.Trim().Equals("")) || (codAmm == null || codAmm.Trim().Equals("")) || (codAOO == null || codAOO.Trim().Equals("")))
                                        {
                                            //sl.Log("");
                                            //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                            //sl.Log("Attenzione, per CANALE PREFERENZIALE di tipo INTEROPERABILITA occorre specificare i campi MAIL, CODICE AOO e CODICE AMM");
                                            messaggio += "\nAttenzione, per CANALE PREFERENZIALE di tipo INTEROPERABILITA occorre specificare i campi MAIL, CODICE AOO e CODICE AMM";
                                            inserisci = false;
                                            break;
                                        }
                                        break;

                                    case "INTEROPERABILITA PITRE":
                                        //sl.Log("");
                                        //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                        //sl.Log("Attenzione, il CANALE PREFERENZIALE di tipo INTEROPERABILITA PITRE non è valido");
                                        messaggio += "\nAttenzione, il CANALE PREFERENZIALE di tipo INTEROPERABILITA PITRE non è valido";
                                        inserisci = false;
                                        break;

                                    case "LETTERA":
                                        break;

                                    case "RACCOMANDATA":
                                        break;

                                    case "FAX":
                                        break;

                                    case "CONSEGNA A MANO":
                                        break;

                                    default:
                                        //sl.Log("");
                                        //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                        //sl.Log("Attenzione, il CANALE PREFERENZIALE non è valido");
                                        messaggio += "\nAttenzione, il CANALE PREFERENZIALE non è valido";
                                        inserisci = false;
                                        break;
                                }
                                
                                //if ((telefono1 == null || telefono1.Equals(""))
                                //    && !(telefono2 == null || telefono2.Equals("")))
                                //{
                                //    sl.Log("");
                                //    sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                                //    sl.Log("Attenzione, inserire il campo TELEFONO principale");
                                //    inserisci = false;
                                //}
                                if (!string.IsNullOrEmpty(telefono1) && telefono1.Length > 16)
                                {
                                    messaggio += "\nAttenzione, il valore inserito nel campo telefono non è accettabile";
                                    inserisci = false;
                                }
                                if (!string.IsNullOrEmpty(telefono2) && telefono2.Length > 16)
                                {
                                    messaggio += "\nAttenzione, il valore inserito nel campo telefono2 non è accettabile";
                                    inserisci = false;
                                }
                                if (!string.IsNullOrEmpty(fax) && fax.Length > 16)
                                {
                                    messaggio += "\nAttenzione, il valore inserito nel campo fax non è accettabile";
                                    inserisci = false;
                                }
                            }

                            if (inserisci)
                            {
                                DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                                DocsPaVO.utente.Corrispondente res = null;

                                DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                                if (corr_type == "" || corr_type == null)
                                    corr_type = "U";
                                switch (corr_type.ToUpper())
                                {
                                    case "U":
                                        DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
                                        uo.tipoCorrispondente = "U";
                                        uo.info = new DocsPaVO.addressbook.DettagliCorrispondente();
                                        uo.codiceCorrispondente = codRubrica.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                        uo.codiceRubrica = codRubrica.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                        uo.codiceAmm = codAmm.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                        uo.codiceAOO = codAOO.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                        uo.email = mail.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                        uo.descrizione = descrizione.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                                        uo.idAmministrazione = infoUtente.idAmministrazione;

                                        uo.idRegistro = id_registro;


                                        dettagli.Corrispondente.AddCorrispondenteRow(
                                            indirizzo.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), citta.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), cap.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            provincia.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), nazione.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), telefono1.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()),
                                            telefono2.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), fax, codiceFiscale.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), note.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), localita.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()), string.Empty, string.Empty, string.Empty, partitaIva.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray()));

                                        uo.info = dettagli;
                                        uo.dettagli = true;


                                        uo.canalePref = canale;
                                        uo.note = note;
                                        res = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(uo, null);
                                        break;

                                    case "R":
                                        res = new DocsPaVO.utente.Corrispondente();
                                        DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                                        ruolo.tipoCorrispondente = "R";
                                        ruolo.codiceCorrispondente = codRubrica;
                                        ruolo.codiceRubrica = codRubrica;
                                        ruolo.descrizione = descrizione;


                                        ruolo.idRegistro = id_registro;



                                        ruolo.email = mail;
                                        ruolo.codiceAmm = codAmm;
                                        ruolo.codiceAOO = codAOO;
                                        ruolo.idAmministrazione = infoUtente.idAmministrazione;
                                        DocsPaVO.utente.UnitaOrganizzativa parent_uo = new UnitaOrganizzativa();
                                        parent_uo.descrizione = "";
                                        parent_uo.systemId = "0";

                                        ruolo.canalePref = canale;
                                        ruolo.note = note;
                                        res = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(ruolo, parent_uo);
                                        break;

                                    case "P":
                                        res = new DocsPaVO.utente.Corrispondente();
                                        DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                                        utente.codiceCorrispondente = codRubrica;
                                        utente.codiceRubrica = codRubrica;
                                        utente.cognome = cognome;
                                        utente.nome = nome;
                                        utente.email = mail;
                                        utente.codiceAmm = codAmm;
                                        utente.codiceAOO = codAOO;
                                        utente.descrizione = cognome + nome;
                                        utente.idAmministrazione = infoUtente.idAmministrazione;

                                        utente.idRegistro = id_registro;
                                        // utente.tipoCorrispondente = this.ddl_tipoCorr.SelectedItem.Value;
                                        utente.canalePref = canale;
                                        utente.note = note;
                                        if ((indirizzo != null && !indirizzo.Equals("")) ||
                                                (cap != null && !cap.Equals("")) ||
                                                (citta != null && !citta.Equals("")) ||
                                                (provincia != null && !provincia.Equals("")) ||
                                                (nazione != null && !nazione.Equals("")) ||
                                                (telefono1 != null && !telefono1.Equals("")) ||
                                                (telefono2 != null && !telefono2.Equals("")) ||
                                                (fax != null && !fax.Equals("")) ||
                                                (codiceFiscale != null && !codiceFiscale.Equals("")) ||
                                                (localita != null && !localita.Equals("")) ||
                                                 partitaIva != null && !partitaIva.Equals(""))
                                        {
                                            dettagli.Corrispondente.AddCorrispondenteRow(
                                            indirizzo, citta, cap,
                                            provincia, nazione, telefono1,
                                            telefono2, fax, codiceFiscale.Trim(),note, localita, "", "","",partitaIva.Trim());

                                            utente.info = dettagli;
                                            utente.dettagli = true;
                                        }
                                        res = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(utente, null);
                                        break;

                                }

                                if (res != null && res.errore == null)
                                {
                                    corrInseriti++;
                                    sl.Log("");
                                    sl.Log("Corrispondente  inserito - Codice Rubrica: " + codRubrica);
                                    // Loggo l'avvenuto inserimento del corrispondente
                                    UserLog.UserLog.WriteLog(infoUtente,
                                        "IMPORTARUBRICACREA", res.systemId,
                                        string.Format("Corrispondente inserito correttamente - Codice rubrica: {0}", codRubrica), DocsPaVO.Logger.CodAzione.Esito.OK);
                                    //Inserimento Corrispondente in DPA_MAIL_CORR_ESTERNO
                                    //MailCorrispondente mailCorr = new MailCorrispondente();
                                    //mailCorr.Email = res.email;
                                    //res.Emails.Add(mailCorr);
                                    //bool insMailCorrEst = BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(res.Emails, res.systemId);

                                    //Emanuela: aggiungo la parte di codice seguente per l'inserimento di email multiple
                                    List<MailCorrispondente> listMail = new List<MailCorrispondente>();
                                    bool primaEmail = true;
                                    foreach (string m1 in mailCorri)
                                    {
                                        if (!string.IsNullOrEmpty(m1.Trim()))
                                        {
                                            MailCorrispondente mc = new MailCorrispondente();

                                            string[] split = { "##" };
                                            string[] noteEmail = m1.Split(split, StringSplitOptions.None);
                                            if (noteEmail.Length > 1 && noteEmail[1].Trim() != "" && noteEmail[0].Trim() != "")
                                            {
                                                mc.Note = noteEmail[1].Trim();
                                            }
                                            if (noteEmail[0].Trim() != "")
                                            {
                                                mc.Email = noteEmail[0].Trim();
                                                if (primaEmail)
                                                {
                                                    mc.Principale = "1";
                                                    primaEmail = false;
                                                }
                                                listMail.Add(mc);
                                            }
                                        }
                                    }
                                    bool insMailCorrEst = BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(listMail, res.systemId);

                                    //Emanuela
                                    //if (insMailCorrEst)
                                    //{
                                    //    sl.Log("Corrispondente inserito correttamente in DPA_MAIL_CORR_ESTERNO - Codice corrispondente: " + res.codiceCorrispondente);
                                    //}
                                    //else
                                    //{
                                    //    sl.Log("Corrispondente non inserito correttamente");
                                    //}
                                }
                                else
                                {
                                    // Il corrispondente non è stato inserito
                                    corrNonInseriti++;
                                    sl.Log("");
                                    sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica + " " + res.errore);
                                    // Loggo l'inserimento non riuscito
                                    UserLog.UserLog.WriteLog(infoUtente,
                                        "IMPORTARUBRICACREA", null,
                                        string.Format("Corrispondente non inserito - Codice rubrica: {0}", codRubrica), DocsPaVO.Logger.CodAzione.Esito.KO);
                                }
                            }
                            else
                            {
                                // Loggo l'inserimento non riuscito
                                sl.Log("");
                                sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica + messaggio);
                                UserLog.UserLog.WriteLog(infoUtente,
                                    "IMPORTARUBRICACREA", null,
                                    string.Format("Corrispondente non inserito - Codice rubrica: {0}", codRubrica), DocsPaVO.Logger.CodAzione.Esito.KO);

                                // Il corrispondente non è stato inserito
                                corrNonInseriti++;
                            }
                        }

                        #endregion

                        //Devo recuperare il system-id del corrispondente
                        string idCorrGlobali = utenti.GetSystemIDCorr(codRubrica, idAmm,id_registro);
                        //Modifica di un corrispondente
                        #region modifica corrispondente
                        //string messaggio = "";
                        bool primEmail = true;
                        if (storicizza.ToUpper() == "M")
                        {
                            if (verificaSelezione(corr_type, descrizione, cognome,
                                        nome, telefono1, telefono2, cap, mail, provincia, ref codiceFiscale, ref partitaIva, ref note, ref messaggio, codRubrica, canaleP, codAOO, codAmm))
                            {
                                mailCorri = mail.Split(';');
                                if (mailCorri != null && mailCorri.Length > 0)
                                {

                                    string[] split = { "##" };
                                    string[] splitEmailNota = mailCorri[0].Split(split, StringSplitOptions.None);
                                    if (!splitEmailNota[0].Trim().Equals("") && primEmail)
                                    {
                                        mail = splitEmailNota[0].Trim();
                                        primEmail = false;
                                    }
                                }
                                DocsPaVO.utente.DatiModificaCorr corr = new DocsPaVO.utente.DatiModificaCorr();
                                corr.idCorrGlobali = idCorrGlobali;
                                if (!string.IsNullOrEmpty(id_registroNuovo))
                                {
                                    corr.idRegistro = id_registroNuovo;
                                }
                                else
                                {
                                    corr.idRegistro = id_registro;
                                }
                                corr.codRubrica = codRubrica;
                                corr.codiceAmm = codAmm;
                                corr.codiceAoo = codAOO;
                                corr.tipoCorrispondente = corr_type;
                                if (corr_type.ToUpper() == "P")
                                    corr.descCorr = cognome + " " + nome;
                                else
                                    corr.descCorr = descrizione;
                                corr.cognome = cognome;
                                corr.nome = nome;
                                corr.indirizzo = indirizzo;
                                corr.cap = cap;
                                corr.citta = citta;
                                corr.provincia = provincia;
                                corr.nazione = nazione;
                                corr.codFiscale = codiceFiscale.Trim();
                                corr.partitaIva = partitaIva.Trim();
                                corr.telefono = telefono1;
                                corr.telefono2 = telefono2;
                                corr.fax = fax;
                                corr.email = mail;
                                corr.localita = localita;
                                corr.idCanalePref = canale.systemId;
                                corr.note = note;

                                bool res = false;
                                if (corr.idRegistro != null)
                                {
                                    //res = BusinessLogic.Utenti.UserManager.ModifyCorrispondenteEsterno(corr, infoUtente, out message);
                                    string newIDCorr = string.Empty;
                                    res = BusinessLogic.Utenti.UserManager.ModifyCorrispondenteEsterno(corr, infoUtente, out newIDCorr, out message);
                                    if (!string.IsNullOrEmpty(newIDCorr) && !newIDCorr.Equals("0"))
                                        corr.idCorrGlobali = newIDCorr;
                                }
                                
                                    //bool res = BusinessLogic.Utenti.UserManager.ModifyCorrispondenteEsterno(corr, out message);
                                if (message.Equals("OK") && res)
                                {
                                    corrAggiornati++;
                                    sl.Log("");
                                    sl.Log("Corrispondente aggiornato - Codice Rubrica: " + corr.codRubrica);
                                    // Loggo la modifica riuscita
                                    UserLog.UserLog.WriteLog(infoUtente,
                                        "IMPORTARUBRICAMODIFICA", corr.idCorrGlobali,
                                        string.Format("Corrispondente aggiornato con successo - Codice rubrica: {0}", codRubrica), DocsPaVO.Logger.CodAzione.Esito.OK);
                                    //Update Corrispondente in DPA_MAIL_CORR_ESTERNO
                                    //Emanuela_ commento il codice in basso per la gestione del multicasella
                                    //MailCorrispondente mailCorr = new MailCorrispondente();
                                    //mailCorr.Email = corr.email;
                                    //System.Collections.Generic.List<MailCorrispondente> listaMail = new System.Collections.Generic.List<MailCorrispondente>();
                                    //listaMail.Add(mailCorr);

                                    //Emanuela: aggiungo la parte di codice seguente per l'inserimento di email multiple
                                    List<MailCorrispondente> listMail = new List<MailCorrispondente>();
                                    bool primaEmail = true;

                                    foreach (string m1 in mailCorri)
                                    {
                                        if (!string.IsNullOrEmpty(m1.Trim()))
                                        {
                                            MailCorrispondente mc = new MailCorrispondente();

                                            string[] split = { "##" };
                                            string[] noteEmail = m1.Split(split, StringSplitOptions.None);
                                            if (noteEmail.Length > 1 && noteEmail[1].Trim() != "")
                                            {
                                                mc.Note = noteEmail[1].Trim();
                                            }
                                            if (noteEmail[0].Trim() != "")
                                            {
                                                mc.Email = noteEmail[0].Trim();
                                                if (primaEmail)
                                                {
                                                    mc.Principale = "1";
                                                    primaEmail = false;
                                                }
                                                listMail.Add(mc);
                                            }
                                        }
                                    }
                                    bool insMailCorrEst = BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(listMail, corr.idCorrGlobali);
                                    //if (insMailCorrEst)
                                    //{
                                    //    sl.Log("Corrispondente aggiornato correttamente in DPA_MAIL_CORR_ESTERNO - Codice Rubrica: " + corr.codRubrica);
                                    //}
                                    //else
                                    //{
                                    //    sl.Log("Corrispondente non aggiornato correttamente");
                                    //}
                                }
                                else
                                {
                                    // Un corrispondente non è stato aggiornato
                                    corrNonAggiornati++;
                                    sl.Log("");
                                    sl.Log("Corrispondente non aggiornato - Codice Rubrica: " + corr.codRubrica);
                                    // Loggo la modifica non riuscita
                                    UserLog.UserLog.WriteLog(infoUtente,
                                        "IMPORTARUBRICAMODIFICA", null,
                                        string.Format("Corrispondente non aggiornato - Codice rubrica: {0}", codRubrica), DocsPaVO.Logger.CodAzione.Esito.KO);
                                }
                            }
                            else
                            {
                                // Un corrispondente non è stato aggiornato
                                corrNonAggiornati++;
                                sl.Log("");
                                sl.Log("Corrispondente non aggiornato - Codice Rubrica: " + codRubrica + messaggio);
                                // Loggo la modifica non riuscita
                                UserLog.UserLog.WriteLog(infoUtente,
                                    "IMPORTARUBRICAMODIFICA", null,
                                    string.Format("Corrispondente non aggiornato - Codice rubrica: {0}", codRubrica), DocsPaVO.Logger.CodAzione.Esito.KO);
                            }
                        }
                        #endregion

                        //Cancellazione di un corrispondente
                        #region elimina corrispondente
                        if (storicizza.ToUpper() == "C")
                        {
                            bool res = false;
                            if (id_registro != null)
                            {
                                res = BusinessLogic.Utenti.UserManager.DeleteCorrispondenteEsterno(idCorrGlobali, flagListe, infoUtente, out message);
                                if (res)
                                {
                                    corrRimossi++;
                                    sl.Log("");
                                    sl.Log("Corrispondente rimosso - Codice Rubrica: " + codRubrica);
                                    // Loggo la cancellazione riuscita
                                    UserLog.UserLog.WriteLog(infoUtente,
                                        "IMPORTARUBRICACANCELLA", idCorrGlobali,
                                        string.Format("Corrispondente cancellato - Codice rubrica: {0}", codRubrica), DocsPaVO.Logger.CodAzione.Esito.OK);
                                    //Cancello anche in DPA_MAIL_CORR_ESTERNI
                                    bool delMailCorrEst = BusinessLogic.Utenti.addressBookManager.DeleteMailCorrispondente(idCorrGlobali);
                                    if (delMailCorrEst)
                                    {
                                        sl.Log("Corrispondente rimosso correttamente in DPA_MAIL_CORR_ESTERNO - Codice Rubrica: " + codRubrica);
                                    }
                                    else
                                    {
                                        sl.Log("Corrispondente non rimosso correttamente");
                                    }
                                }
                                else
                                {
                                    /*
                                            VALORI RITORNATI
                                            0: CANCELLAZIONE EFFETTUATA - operazione andata a buon fine
                                            1: DISABILITAZIONE EFFETTUATA - il corrispondente è presente nella DPA_DOC_ARRIVO_PAR, quindi non viene cancellato
                                            2: CORRISPONDENTE NON RIMOSSO - il corrispondente è presente nella lista di distribuzione e non posso rimuoverlo
                                            3: ERRORE: la DELETE sulla dpa_corr_globali NON è andata a buon fine
                                            4: ERRORE: la DELETE sulla dpa_dett_globali NON è andata a buon fine
                                            5: ERRORE: l' UPDATE sulla dpa_corr_globali NON è andata a buon fine
                                            6: ERRORE: la DELETE sulla dpa_liste_distr NON è andata a buon fine	
                                        */
                                    // Un corrispondente non è stato rimosso
                                    corrNonRimossi++;
                                    sl.Log("");
                                    sl.Log("Corrispondente non rimosso - Codice Rubrica: " + codRubrica);
                                    if (message.Equals("NOTOK"))
                                    {
                                        sl.Log("Il corrispondente è presente nella lista di distribuzione e non è possibile rimuoverlo");
                                        // Loggo la cancellazione non riuscita
                                        UserLog.UserLog.WriteLog(infoUtente,
                                            "IMPORTARUBRICACANCELLA", codRubrica,
                                            "Il corrispondente è presente nella lista di distribuzione e non è possibile rimuoverlo", DocsPaVO.Logger.CodAzione.Esito.KO);
                                    }
                                    if (message.StartsWith("ERROR"))
                                    {
                                        // Un corrispondente non è stato rimosso
                                        corrNonRimossi++;
                                        sl.Log("Errore durante l'eliminazione di un corrispondente. codice errore: " + message);
                                        // Loggo la cancellazione non riuscita
                                        UserLog.UserLog.WriteLog(infoUtente,
                                            "IMPORTARUBRICACANCELLA", codRubrica,
                                            "Errore durante l'eliminazione di un corrispondente.", DocsPaVO.Logger.CodAzione.Esito.KO);
                                    }
                                }
                            }
                            else
                            {
                                corrNonRimossi++;
                                sl.Log("");
                                sl.Log("Corrispondente non rimosso - Codice Rubrica: " + codRubrica + ". Il codice registro non e' corretto.");
                            }

                        }
                        #endregion
                    }
                    else
                    {
                        sl.Log("");
                        sl.Log("Nessuna azione specificata: compilare la colonna Storicizza.");
                    }
                }
                sl.Log("");
                sl.Log("Fine importazione utenti - " + System.DateTime.Now.ToString());
                sl.Log("Utenti Inseriti: " + corrInseriti + " - Utenti Aggiornati: " + corrAggiornati + " - Utenti Rimossi: " + corrRimossi + " - Utenti NON Inseriti: " + corrNonInseriti + " - Utenti NON Aggiornati: " + corrNonAggiornati + " - Utenti NON Rimossi: " + corrNonRimossi);
                logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : fine lettura file \"importUtenti.xls\"");
            }
            catch (Exception ex)
            {
                if (sl != null)
                    sl.Log("Errore durante l'importazione della rubrica. Errore: " + ex.Message);
                logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" ERRORE : " + ex.Message);
                UserLog.UserLog.WriteLog(infoUtente,
                    "IMPORTARUBRICAEXCEPTION", null,
                    string.Format("Errore durante l'importazione della rubrica. Errore: ",
                    ex.Message), DocsPaVO.Logger.CodAzione.Esito.KO);
                result = false;
                return result;
            }
            finally
            {
                xlsReader.Close();
                xlsConn.Close();
            }
            return result;
        }

        private static string get_string(OleDbDataReader dr, int field)
        {
            if (field >= dr.FieldCount||  dr[field] == null || dr[field] == System.DBNull.Value)
                return "";
            else
                return dr[field].ToString().Trim();
        }

        private static bool codice_rubrica_valido(string cod)
        {
            if (cod == null || cod.Trim() == "")
                return false;

            Regex rx = new Regex(@"^[0-9A-Za-z_\ \.\-]+$");
            return rx.IsMatch(cod);
        }

        private static bool isNumeric(string val)
        {
            string appo = val;
            Regex regExp = new Regex("\\D");
            return !regExp.IsMatch(appo);
        }

        private static bool isCorrectProv(string val)
        {
            string appo = val.ToUpper();
            Regex regExp = new Regex("([A-Z]{2})");

            return regExp.IsMatch(appo);
        }

        private static bool IsValidEmail(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        private static bool verificaSelezione(string corr_type, string descrizione, string cognome,
                                       string nome, string tel1, string tel2, string cap, string mail, string provincia, ref string codiceFiscale, ref string partitaIva, ref string note, ref string messaggio, string codRubrica, string canaleP, string codAOO, string codAmm)
        {
            bool resultCheck = true;

            //int indxMail = dd_canpref.Items.IndexOf(dd_canpref.Items.FindByText("MAIL"));

            if ((corr_type == "U" && descrizione.Trim() == "") ||
                (corr_type == "R" && descrizione.Trim() == "") ||
                (corr_type == "P" && (cognome.Trim() == "" || nome.Trim() == "")))
            //|| (dd_canpref.SelectedIndex == indxMail && this.txt_EmailAOO.Text.Equals(String.Empty)))
            {
                // RegisterStartupScript("chk_fields",
                //     "<script language=\"javascript\">" +
                //     "alert (\"Attenzione: compilare tutti i campi obbligatori\");" +
                //     "</script>");
                messaggio += "\nAttenzione: compilare tutti i campi obbligatori.";
                resultCheck = false;
            }
            else
            {
                if (corr_type == "" || (corr_type.ToUpper() != "P" && corr_type.ToUpper() != "U" && corr_type.ToUpper() != "R"))
                {
                    //sl.Log("");
                    //sl.Log("Corrispondente non inserito - Codice Rubrica: " + codRubrica);
                    //sl.Log("Attenzione, il campo TIPO è obbligatorio.");
                    messaggio += "\nAttenzione, il campo TIPO non è corretto.";
                    resultCheck = false;
                }

                if ((tel1 == null || tel1.Equals(""))
                    && !(tel2 == null || tel2.Equals("")))
                {
                    messaggio += "\nAttenzione, inserire il campo TELEFONO PRINC.";
                    resultCheck = false;
                }

                if (cap != null && !cap.Equals("") && !isNumeric(cap))
                {
                    messaggio += "\nAttenzione, il campo CAP deve essere numerico!";
                    resultCheck = false;
                }

                if (provincia != null && !provincia.Equals("") && !isCorrectProv(provincia))
                {
                    messaggio += "\nAttenzione, il campo PROVINCIA contiene caratteri non validi!";
                    resultCheck = false;
                }
                //Emanuela: commento il seguente codice per la gestione del multicasella
                //if ((!mail.Equals("") && mail.Trim().Equals(""))
                //        || (!mail.Trim().Equals("") && !IsValidEmail(mail.Trim())))
                //{
                //    messaggio = "Attenzione, inserire una EMAIL valida";
                //    resultCheck = false;
                //}

                //Emanuela: aggiungo il seguente codice per l'inserimento di email multiple
                string[] mailCorri = mail.Split(';');
                foreach (string m in mailCorri)
                {
                    string[] split = { "##" };
                    string[] splitEmailNota = m.Split(split, StringSplitOptions.None);
                    if (splitEmailNota[0].Trim() != "")
                    {
                        int presente = (from mc in mailCorri where mc.Split(split, StringSplitOptions.None).ElementAt(0).Trim().Equals(splitEmailNota[0].Trim()) select mc).Count();
                        if (presente > 1)
                        {
                            messaggio += "\nAttenzione, indirizzo EMAIL duplicato";
                            resultCheck = false;
                            break;
                        }
                    }
                    if (!splitEmailNota[0].Trim().Equals("") && !IsValidEmail(splitEmailNota[0].Trim()))
                    {
                        messaggio += "\nAttenzione, inserire una EMAIL valida";
                        resultCheck = false;
                        break;
                    }
                }

                if (corr_type.ToUpper().Equals("U"))
                {
                    if ((codiceFiscale != null && !codiceFiscale.Trim().Equals("")) && ((codiceFiscale.Trim().Length == 11 && CheckVatNumber(codiceFiscale.Trim()) != 0) || (codiceFiscale.Trim().Length == 16 && CheckTaxCode(codiceFiscale.Trim()) != 0) || (codiceFiscale.Trim().Length != 11 && codiceFiscale.Trim().Length != 16)))
                    {
                        messaggio += "\nAttenzione, il campo CODICE FISCALE ha caratteri non validi";
                        resultCheck = false;
                    }
                }
                else
                    if (codiceFiscale != null && !codiceFiscale.Trim().Equals("") && CheckTaxCode(codiceFiscale.Trim()) != 0)
                    {
                        messaggio += "\nAttenzione, il campo CODICE FISCALE ha caratteri non validi";
                        resultCheck = false;
                    }


                if (partitaIva != null && !partitaIva.Trim().Equals("") && CheckVatNumber(partitaIva.Trim()) != 0)
                {
                    messaggio += "\nAttenzione, il campo PARTITA IVA ha caratteri non validi";
                    resultCheck = false;
                }

                //Emanuela: controlli per la gestione del Canale Preferenziale
                switch (canaleP.ToUpper())
                {
                    case "MAIL":
                        if ((mail == null || mail.Trim().Equals("")))
                        {
                            messaggio += "\nAttenzione, per CANALE PREFERENZIALE di tipo mail occorre specificare il campo MAIL";
                            resultCheck = false;
                            break;
                        }
                        break;

                    case "INTEROPERABILITA":
                        if ((mail == null || mail.Trim().Equals("")) || (codAmm == null || codAmm.Trim().Equals("")) || (codAOO == null || codAOO.Trim().Equals("")))
                        {
                            messaggio += "\nAttenzione, per CANALE PREFERENZIALE di tipo INTEROPERABILITA occorre specificare i campi MAIL, CODICE AOO e CODICE AMM";
                            resultCheck = false;
                            break;
                        }
                        break;

                    case "INTEROPERABILITA PITRE":
                        messaggio += "\nAttenzione, il CANALE PREFERENZIALE di tipo INTEROPERABILITA PITRE non è valido";
                        resultCheck = false;
                        break;

                    case "LETTERA":
                        break;

                    case "RACCOMANDATA":
                        break;

                    case "FAX":
                        break;

                    case "CONSEGNA A MANO":
                        break;

                    default:
                        messaggio += "\nAttenzione, il CANALE PREFERENZIALE non è valido";
                        resultCheck = false;
                        break;
                }

            }
            return resultCheck;
        }

        public static int CheckVatNumber(string vatNum)
        {
            bool result = false;
            const int character = 11;
            string vatNumber = vatNum;
            Regex pregex = new Regex("^\\d{" + character.ToString() + "}$");

            if (string.IsNullOrEmpty(vatNumber) || vatNum.Length != character)
                return -1;
            Match m = pregex.Match(vatNumber);
            result = m.Success;
            if (!result)
                return -2;
            result = (int.Parse(vatNumber.Substring(0, 7)) != 0);
            if (!result)
                return -3;
            result = ((int.Parse(vatNumber.Substring(7, 3)) >= 0) && (int.Parse(vatNumber.Substring(7, 3)) < 201));
            if (!result)
                return -4;

            /*Algoritmo di verifica della correttezza formale del numero di partita IVA 
            ---------------------------------------------------------------------------------------------
                1. si sommano tra loro le cifre di posto dispari
                2. le cifre di posto pari si moltiplicano per 2
                3. se il risultato del punto precedente è maggiore di 9 si sottrae 9 al risultato
                4. si sommano tra loro i risultati dei 2 punti precedenti
                5. si sommano tra loro le due somme ottenute
            ---------------------------------------------------------------------------------------------
             */
            int sum = 0;
            for (int i = 0; i < character - 1; i++)
            {
                int j = int.Parse(vatNumber.Substring(i, 1));
                if ((i + 1) % 2 == 0)
                {
                    j *= 2;
                    char[] c = j.ToString("00").ToCharArray();
                    sum += int.Parse(c[0].ToString());
                    sum += int.Parse(c[1].ToString());
                }
                else
                    sum += j;
            }
            if ((sum.ToString("00").Substring(1, 1).Equals("0")) && (!vatNumber.Substring(10, 1).Equals("0")))
                return -5;
            sum = int.Parse(vatNumber.Substring(10, 1)) + int.Parse(sum.ToString("00").Substring(1, 1));
            if (!sum.ToString("00").Substring(1, 1).Equals("0"))
                return -5;
            return 0;
        }

        /// <summary>
        /// Controllo formale della correttezza del Codice Fiscale
        /// </summary>
        /// <param name="taxCode"></param>
        /// <returns>
        /// -1 : Lunghezza Codice Fiscale errata.
        /// -2 : Il formato del Codice Fiscale non è corretto.
        /// -3 : Verifica della correttezza formale del Codice Fiscale non superata 
        /// 0 :  Codice Fiscale corretto
        /// </returns>
        public static int CheckTaxCode(string taxCode)
        {
            taxCode = taxCode.Replace(" ", "");
            bool result = false;
            const int character = 16;
            // stringa per controllo e calcolo omocodia 
            const string omocode = "LMNPQRSTUV";
            // per il calcolo del check digit e la conversione in numero
            const string listControl = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int[] listEquivalent = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            int[] listaUnequal = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };

            result = (string.IsNullOrEmpty(taxCode) || taxCode.Length != character);
            if (result)
                return -1;
            taxCode = taxCode.ToUpper();
            char[] arrTaxCode = taxCode.ToCharArray();

            // check della correttezza formale del codice fiscale
            // elimino dalla stringa gli eventuali caratteri utilizzati negli 
            // spazi riservati ai 7 che sono diventati carattere in caso di omocodia
            for (int k = 6; k < 15; k++)
            {
                if ((k == 8) || (k == 11))
                    continue;
                int x = (omocode.IndexOf(arrTaxCode[k]));
                if (x != -1)
                    arrTaxCode[k] = x.ToString().ToCharArray()[0];
            }

            Regex rgx = new Regex(@"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$");
            Match m = rgx.Match(new string(arrTaxCode));
            result = m.Success;
            // normalizzato il codice fiscale se la regular non ha buon
            // fine è inutile continuare
            if (!result)
                return -2;
            int somma = 0;
            // ripristino il codice fiscale originario 
            arrTaxCode = taxCode.ToCharArray();
            for (int i = 0; i < 15; i++)
            {
                char c = arrTaxCode[i];
                int x = "0123456789".IndexOf(c);
                if (x != -1)
                    c = listControl.Substring(x, 1).ToCharArray()[0];
                x = listControl.IndexOf(c);
                // i modulo 2 = 0 è dispari perchè iniziamo da 0
                if ((i % 2) == 0)
                    x = listaUnequal[x];
                else
                    x = listEquivalent[x];
                somma += x;
            }
            result = (listControl.Substring(somma % 26, 1) == taxCode.Substring(15, 1));
            if (!result)
                return -3;
            return 0;
        }

        public static ArrayList getLogImportRubrica(string serverPath)
        {
            ArrayList fileLog = new ArrayList();
            string sLine = string.Empty;

            try
            {
                StreamReader objReader = new StreamReader(serverPath + "\\Modelli\\Import\\logImportRubrica.log");
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        fileLog.Add(sLine);
                }
                objReader.Close();

                return fileLog;
            }
            catch (Exception e)
            {
                logger.Debug("Metodo \"getLogImportRubrica\" classe \"DPA3_RubricaSearchAgent\" ERRORE : " + e.Message);
                return fileLog;
            }
        }



        #endregion


		public static ArrayList GetListaUOSmistamentoRubrica(string idRegistro)
		{
			ArrayList retValue=new ArrayList();

			DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc=new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
			DataSet ds=dbSmistaDoc.GetListaUOSmistamento(idRegistro);
			dbSmistaDoc=null;

			if(ds.Tables["UO"].Rows.Count > 0)
				retValue=CreateListaUOSmistamento(ds);
			
			return retValue;			
		}


        public override ArrayList SearchRangeSystemID(string[] valoriRicerca)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            DocsPaVO.rubrica.ElementoRubrica item = new DocsPaVO.rubrica.ElementoRubrica();
            Regex reg = new System.Text.RegularExpressions.Regex(@"^\d{0,9}$");
            ArrayList list = new ArrayList();
            foreach (string valoreRicerca in valoriRicerca)
            {
                //if (Char.IsNumber(valoreRicerca, 0))
                if(reg.IsMatch(valoreRicerca))
                {
                    item = r.GetElementoRubricaSimpleBySystemId(valoreRicerca);
                }
                else
                {
                    if (RubricaComune.Configurazioni.GetConfigurazioni(this._user).GestioneAbilitata)
                    {
                        // Ricerca degli elementi in rubrica comune                
                        item = RubricaComune.RubricaServices.GetElementoRubricaComune(this._user, valoreRicerca, true);
                    }
                }
                if (item != null)
                    list.Add(item);
            }

            return list;
        }

		private static ArrayList CreateListaUOSmistamento(DataSet ds)
		{
			ArrayList retValue=new ArrayList();

			DocsPaVO.Smistamento.UOSmistamento uo=null;
			

			foreach (DataRow rowUO in ds.Tables["UO"].Rows)
			{
				uo=new DocsPaVO.Smistamento.UOSmistamento();
				uo.ID=rowUO["ID"].ToString();
				uo.Codice=rowUO["CODICE_UO"].ToString();
				uo.Descrizione=rowUO["DESCRIZIONE_UO"].ToString();

				retValue.Add(uo);
			}

			return retValue;
		}

		public bool UoIsOnExternalReg (string cod_uo, string caller_id_reg, string caller_id_amm)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			return !r.UoIsOnRegistro (cod_uo, caller_id_reg, caller_id_amm);
		}

		public string[] GetUoInterneAoo(string id_reg)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			return r.GetUoInterneAoo (id_reg);
		}

		public string[] getUtenteInternoAOO(string idPeople, string id_reg)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			return r.getUtenteInternoAOO (idPeople,id_reg);
		}

		public bool verificaDipendezaCodRubrica(string codiceUoAppartenenza, string codiceRubrica)
		{
			DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
			return r.verificaDipendezaCodRubrica(codiceUoAppartenenza,codiceRubrica);
		}
        //Fede new
        public bool verificaDipendenzaCodRubrica(string idUoAppartenenza, string codiceRubrica)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            return r.verificaDipendenzaCodRubrica(idUoAppartenenza, codiceRubrica);
        }

        public string getIdUoAppartenenza(string codiceUoAppartenenza)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            return r.GetIdUoAppartenenza(codiceUoAppartenenza);
        }
		#endregion

        /// <summary>
        /// Metodo usato dalla rubrica veloce ajax
        /// <summary>
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="registri"></param>
        /// <returns></returns>
        public static string[] getElementiRubricaVeloce(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.rubrica.ParametriRicercaRubrica qco)
        {
            string[] listaTemp = null;
            DocsPaDB.Query_DocsPAWS.Rubrica query = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
            ArrayList ers = query.GetElementiRubrica(qco);

            SmistamentoRubrica smistamentoRubrica = new SmistamentoRubrica();
           
            DPA3_RubricaSearchAgent SearchFilter = new DPA3_RubricaSearchAgent(infoUtente);

            if (SearchFilter != null)
            {
                SearchFilter.DPA3_SearchFilter(qco, ref ers, smistamentoRubrica);
            }

            if ((qco.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE || qco.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO) && qco.doRubricaComune)
            {
                 DocsPaVO.RubricaComune.FiltriRubricaComune filtroRubricaComune = new DocsPaVO.RubricaComune.FiltriRubricaComune();

                if (!string.IsNullOrEmpty(qco.descrizione))
                {
                    filtroRubricaComune.Descrizione = qco.descrizione.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.Descrizione = qco.descrizione;
                }

                filtroRubricaComune.RicercaParolaIntera = qco.queryCodiceEsatta;

                ICollection c = RubricaComune.RubricaServices.GetElementiRubricaComune(infoUtente, filtroRubricaComune);

                if (c != null && c.Count > 0)
                {
                    ers.AddRange(c);

                    // Ordinamento dell'array, a seguito dell'inserimento dei dati dalla rubrica comune
                    ers.Sort(new DocsPaVO.rubrica.ElementoRubrica.ElementoRubricaComparer());
                }
            }

            string tempStringElemento = null;

            if (ers.Count > 0 && ers!=null)
            {
                listaTemp = new string[ers.Count];
            }
         
            for(int i=0; i<ers.Count; i++)
            {
                DocsPaVO.rubrica.ElementoRubrica tempElement = (DocsPaVO.rubrica.ElementoRubrica)ers[i];
                string codRegTemp = tempElement.codiceRegistro;
                if (tempElement.isRubricaComune == true)
                {
                    codRegTemp = " [RC]";
                }
                else
                {
                    if (codRegTemp == null || codRegTemp.Equals(""))
                    {
                        if (tempElement.interno == true || tempElement.tipo.Equals("L"))
                        {
                            codRegTemp = "";
                        }
                        else
                        {
                            codRegTemp = " [TUTTI]";
                        }
                    }
                    else
                    {
                       codRegTemp = " [" + tempElement.codiceRegistro + "]";
                    }
                }

                tempStringElemento = tempElement.descrizione + " (" + tempElement.codice + ")"  + codRegTemp;
                listaTemp[i] = tempStringElemento;
            }
            return listaTemp;
        }

        private void setVisibleCheckBoxElement(DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers)
        {
            if  (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_MODELLO_TRASM ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO ||
                //qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO ||
                qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE
                )
            {
                foreach (ElementoRubrica er in ers)
                {
                    er.isVisibile = !er.disabledTrasm;
                }
            }            
        }

        private void setVisibleCheckBoxElement(DocsPaVO.rubrica.SmistamentoRubrica sr, ref ArrayList ers)
        {
            if (sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_MODELLO_TRASM ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO ||
                //sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO ||
                sr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE
                )
            {
                foreach (ElementoRubrica er in ers)
                {
                    er.isVisibile = !er.disabledTrasm;
                }
            }
        }

        

        public static List<string> GetListaCapComuni(string prefixCap, string comune)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                return utenti.GetListaCapComuni(prefixCap, comune);
            }
            catch(Exception e)
            {
                logger.Error("Errore in GetListaCapComuni " + e.Message);
                return null;
            }
        }

        public static List<string> GetListaComuni(string prefixComune)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                return utenti.GetListaComuni(prefixComune);
            }
            catch(Exception e)
            {
                logger.Error("Errore in GetListaComuni " + e.Message);
                return null;
            }
        }

        public static InfoComune GetCapComuni(string cap, string comune)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                return utenti.GetCapComuni(cap, comune);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetCapComuni " + e.Message);
                return null;
            }
        }

        public static InfoComune GetProvinciaComune(string comune)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                return utenti.GetProvinciaComune(comune);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetProvinciaComune " + e.Message);
                return null;
            }
        }
	}
}
