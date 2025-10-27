// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.Amministrazione;
using BusinessLogic.RubricaComune;
using BusinessLogic.Trasmissioni;
using BusinessLogic.Utenti;
using DocsPaVO.addressbook;
using DocsPaVO.rubrica;
using DocsPaVO.trasmissione;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;

namespace BusinessLogic.Rubrica
{
    public class DPA3_RubricaSearchAgent : RubricaSearchAgent
    {
        private static Hashtable h_utenti = null;
        private static Hashtable h_ruoli = null;
        private static Hashtable h_registri = null;
        private static Hashtable h_uo = null;

        private DocsPaVO.utente.InfoUtente _user;

        public static void resetHashTable()
        {
            h_utenti = null;
            h_ruoli = null;
            h_registri = null;
            h_uo = null;
        }

        void init_ht()
        {
            if (h_utenti == null)
                h_utenti = UtenteManager.GetRuoliUtenteSemplice(_user.idAmministrazione);
            if (h_ruoli == null)
                h_ruoli = UOManager.GetRuoliUOSemplice(_user.idAmministrazione);
            if (h_registri == null)
                h_registri = RegistriManager.GetRegistriByRuolo(_user.idAmministrazione);
            if (h_uo == null)
                h_uo = UOManager.GetUORuoloSemplice(_user.idAmministrazione);
        }

        public DPA3_RubricaSearchAgent(DocsPaVO.utente.InfoUtente user)
        {
            SearchFilter = new RubricaSearchFilter(DPA3_SearchFilter);
            _user = user;
            init_ht();
        }

        public override DocsPaVO.rubrica.ElementoRubrica SearchSingle(string codice, DocsPaVO.rubrica.SmistamentoRubrica smistaRubrica, string condRegistri, IBaseRubricaComuneService rubricaComuneService)
        {
            DocsPaVO.rubrica.ElementoRubrica er = new ElementoRubrica();
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            er = r.GetElementoRubrica(codice, condRegistri);

            ArrayList ers = new ArrayList();
            ers.Add(er);

            if (smistaRubrica != null && smistaRubrica.smistamento == "1")
            {
                //caso in cui è abilitato lo smistamento, devo quindi filtrare i corrispondenti
                //ma solo per determinati callType

                if ((smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT)
                   || (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    || (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST)
                    || (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
                    || (smistaRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
                {
                    filtraPrimoSmistamento(smistaRubrica.idRegistro, smistaRubrica.ruoloProt.systemId, smistaRubrica.infoUt, ref ers);
                }
            }

            if (er == null)
            {
                // Ricerca elemento rubrica per codice in rubrica comune
                er = RubricaComune.RubricaServices.GetElementoRubricaComune(this._user, codice, true, rubricaComuneService);
            }

            return er;
        }

        public override DocsPaVO.rubrica.ElementoRubrica SearchSingleSimpleBySystemId(string systemId)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            return r.GetElementoRubricaSimpleBySystemId(systemId);
        }


        public override ArrayList GetHierarchyRange(string[] codici, DocsPaVO.addressbook.TipoUtente[] tipiIE)
        {
            throw new NotImplementedException("GetHierarchyRange");
        }

        public override void CheckChildrenExistence(ref DocsPaVO.rubrica.ElementoRubrica[] ers, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            r.CheckChildrenExistence(ref ers, idAmm);
        }

        public override void CheckChildrenExistence(ref DocsPaVO.rubrica.ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            r.CheckChildrenExistence(ref ers, checkUo, checkRuoli, checkUtenti, idAmm);
        }

        public override DocsPaVO.rubrica.ElementoRubrica SearchSingleSimple(string codice, IBaseRubricaComuneService rubricaComuneService)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            DocsPaVO.rubrica.ElementoRubrica er = r.GetElementoRubricaSimple(codice);

            if (er == null)
            {
                // Ricerca elemento rubrica per codice in rubrica comune
                er = RubricaComune.RubricaServices.GetElementoRubricaComune(this._user, codice, true, rubricaComuneService);
            }

            return er;
        }

        public override ArrayList GetChildrenElement(string elementID, string childrensType)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            return r.GetChildrenElement(elementID, childrensType);
        }

        public override System.Collections.ArrayList Search(DocsPaVO.rubrica.ParametriRicercaRubrica qr, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, IBaseRubricaComuneService rubricaComuneService)
        {
            if (qr.caller == null || qr.caller.IdRuolo == "" || qr.caller.IdUtente == "")
                throw new ArgumentException("Dati mancanti nella definizione dell'utente che ha effettuato la chiamata alla Rubrica");

            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);

            ArrayList ers = r.GetElementiRubrica(qr);

            if (SearchFilter != null)
                SearchFilter(qr, ref ers, smistamentoRubrica);

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

                //2020: RICERCA NELLE RUBRICHE ESTERNE
                if (qr.rubricaEsterna != null && qr.rubricaEsterna.Count > 0)
                {
                    filtroRubricaComune.RubricaEsterna = new List<DocsPaVO.RubricaComune.RubricaEsterna>();
                    foreach (ParametriRicercaRubrica.RubricaEsterna ru in qr.rubricaEsterna)
                    {
                        filtroRubricaComune.RubricaEsterna.Add((DocsPaVO.RubricaComune.RubricaEsterna)Enum.Parse(typeof(DocsPaVO.RubricaComune.RubricaEsterna), ru.ToString(), true));
                    }
                }

                ICollection c = RubricaComune.RubricaServices.GetElementiRubricaComune(this._user, filtroRubricaComune, rubricaComuneService);

                if (c != null && c.Count > 0)
                {
                    ers.AddRange(c);

                    // Ordinamento dell'array, a seguito dell'inserimento dei dati dalla rubrica comune
                    ers.Sort(new DocsPaVO.rubrica.ElementoRubrica.ElementoRubricaComparer());
                }
            }

            return ers;
        }

        public override System.Collections.ArrayList SearchPaging(DocsPaVO.rubrica.ParametriRicercaRubrica qr, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, int firstRowNum, int maxRowForPage, out int totale, IBaseRubricaComuneService rubricaComuneService)
        {
            if (qr.caller == null || qr.caller.IdRuolo == "" || qr.caller.IdUtente == "")
                throw new ArgumentException("Dati mancanti nella definizione dell'utente che ha effettuato la chiamata alla Rubrica");

            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);

            ArrayList ers = r.GetElementiRubricaPaging(qr, firstRowNum, maxRowForPage, out totale);

            if (SearchFilter != null)
                SearchFilter(qr, ref ers, smistamentoRubrica);

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

            if (this.RicercaRubricaComune(qr) || this.RicercaRubricaEsterna(qr))
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
                if (!string.IsNullOrEmpty(qr.noteEmail))
                {
                    filtroRubricaComune.Note = qr.noteEmail.Replace("'", "''");
                }
                else
                {
                    filtroRubricaComune.Note = qr.noteEmail;
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

                if (qr.rubricaEsterna != null && qr.rubricaEsterna.Count > 0)
                {
                    filtroRubricaComune.RubricaEsterna = new List<DocsPaVO.RubricaComune.RubricaEsterna>();
                    foreach (ParametriRicercaRubrica.RubricaEsterna ru in qr.rubricaEsterna)
                    {
                        filtroRubricaComune.RubricaEsterna.Add((DocsPaVO.RubricaComune.RubricaEsterna)Enum.Parse(typeof(DocsPaVO.RubricaComune.RubricaEsterna), ru.ToString(), true));
                    }
                }
                if (!this.RicercaRubricaComune(qr))
                {
                    //Non effettuo la ricerca nella rubrica comune ma solo in quella esterna
                    filtroRubricaComune.SoloRubricaEsterna = true;
                }

                ICollection c = RubricaComune.RubricaServices.GetElementiRubricaComune(this._user, filtroRubricaComune, rubricaComuneService);

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

        public override ArrayList GetRootItems(DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            ArrayList ers = new ArrayList();
            ers = r.GetRootItems(tipoIE);

            if (smistamentoRubrica != null && smistamentoRubrica.smistamento == "1")
            {
                //caso in cui è abilitato lo smistamento, devo quindi filtrare i corrispondenti
                //ma solo per determinati callType
                if ((smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
                {
                    filtraPrimoSmistamento(smistamentoRubrica.idRegistro, smistamentoRubrica.ruoloProt.systemId, smistamentoRubrica.infoUt, ref ers);

                }
            }
            return ers;
        }

        public override ArrayList SearchRange(string[] codici, DocsPaVO.addressbook.TipoUtente tipoIE, IBaseRubricaComuneService baseRubricaComuneService)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);

            ArrayList list = r.SearchRange(codici, tipoIE);

            if (RubricaComune.Configurazioni.GetConfigurazioni(this._user).GestioneAbilitata)
            {
                // Ricerca degli elementi in rubrica comune
                foreach (string codice in codici)
                {
                    DocsPaVO.rubrica.ElementoRubrica item = RubricaComune.RubricaServices.GetElementoRubricaComune(this._user, codice, true, baseRubricaComuneService);

                    if (item != null)
                        list.Add(item);
                }
            }

            return list;
        }

        public override ArrayList GetHierarchy(string codice, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            ArrayList ers = r.GetGerarchiaElemento(codice, tipoIE);
            if (smistamentoRubrica != null && smistamentoRubrica.smistamento == "1")
            {
                //caso in cui è abilitato lo smistamento, devo quindi filtrare i corrispondenti
                //ma solo per determinati callType

                if ((smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT)
                       || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
                {
                    filtraPrimoSmistamento(smistamentoRubrica.idRegistro, smistamentoRubrica.ruoloProt.systemId, smistamentoRubrica.infoUt, ref ers);
                }
            }

            setVisibleCheckBoxElement(smistamentoRubrica, ref ers);

            return ers;
        }

        public override ArrayList SearchRangeSystemID(string[] valoriRicerca, IBaseRubricaComuneService rubricaComuneService)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            DocsPaVO.rubrica.ElementoRubrica item = new DocsPaVO.rubrica.ElementoRubrica();
            Regex reg = new System.Text.RegularExpressions.Regex(@"^\d{0,9}$");
            ArrayList list = new ArrayList();
            foreach (string valoreRicerca in valoriRicerca)
            {
                //if (Char.IsNumber(valoreRicerca, 0))
                if (reg.IsMatch(valoreRicerca))
                {
                    item = r.GetElementoRubricaSimpleBySystemId(valoreRicerca);
                }
                else
                {
                    if (RubricaComune.Configurazioni.GetConfigurazioni(this._user).GestioneAbilitata)
                    {
                        // Ricerca degli elementi in rubrica comune
                        item = RubricaComune.RubricaServices.GetElementoRubricaComune(this._user, valoreRicerca, true, rubricaComuneService);
                    }
                }
                if (item != null)
                    list.Add(item);
            }

            return list;
        }

        private void DPA3_SearchFilter(DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica)
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
                filtra_aoo(qr, ref ers);
                if (smistamentoRubrica != null && smistamentoRubrica.smistamento.Equals("1"))
                {
                    if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST))
                    {
                        //if(!filtraPrimoSmistamentoElenco(qr,this._user,ref ers))
                        if (!filtraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, this._user, ref ers))
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
                filtra_trasmissioni(qr, ref ers);

            if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO))
                if (qr.ObjectType != null)
                    filtra_trasmissioni(qr, ref ers);

            if ((((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
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
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA && qr.caller.IdRegistro != null)
                )
                && ((qr.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO) || (qr.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)))
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE && qr.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE))
            {
                filtra_aoo(qr, ref ers);
                if (smistamentoRubrica != null && smistamentoRubrica.smistamento.Equals("1"))
                {
                    if (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                        || qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI
                        || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO))
                    {
                        //if(!filtraPrimoSmistamentoElenco(qr,this._user,ref ers))
                        if (!filtraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, this._user, ref ers))
                        {
                            ers = null;
                        }
                    }

                    if (smistamentoRubrica != null && smistamentoRubrica.smistamento.Equals("1"))
                    {
                        if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA && qr.caller.IdRegistro != null))
                        {
                            if (!filtraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, this._user, ref ers))
                            {
                                ers = null;
                            }
                        }
                    }
                }

            }
            if (smistamentoRubrica != null && smistamentoRubrica.smistamento.Equals("1"))
            {
                //viene effettuata quando si ricerca (tab elenco) su protocollo uscita nel caso di destinatario globale
                if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT || qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)) && qr.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE)
                {
                    //if(!filtraPrimoSmistamentoElenco(qr,this._user,ref ers))
                    if (!filtraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, this._user, ref ers))
                    {
                        ers = null;
                    }
                }

                if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
                {
                    filtraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, this._user, ref ers);
                }
            }

            //Commentato perchè adesso si filtra con la query direttamente
            //if(qr.descrizione!=null && qr.descrizione!=string.Empty)
            //	filtraListe(qr, ref ers);
        }

        string[] uo_in_aoo;
        string[] uo_in_aoo_with_reg;

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

                if (!smistamento_empty)
                {
                    //prendo la Uo relativa al ruolo che sta protocollando
                    DocsPaVO.utente.Corrispondente ruoloProt = UserManager.getCorrispondenteCompletoBySystemId(idRuolo, DocsPaVO.addressbook.TipoUtente.INTERNO, utente);

                    // listaUoSmistamento in hashtable
                    // codice: codice uo
                    // valore: oggetto uo smistamento
                    Hashtable tableUoSmistamento = new Hashtable();
                    foreach (DocsPaVO.Smistamento.UOSmistamento item in listaUoSmistamento)
                    {
                        if (!tableUoSmistamento.ContainsKey(item.Codice))
                            tableUoSmistamento.Add(item.Codice, item);
                    }

                    string idUoAppartenenza = getIdUoAppartenenza
                        (((DocsPaVO.utente.Ruolo)ruoloProt).uo.codiceRubrica);
                    for (int i = 0; i < ers.Count; i++)
                    {
                        DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica)ers[i];

                        if (er != null)
                        {
                            if (!er.interno)
                            {
                                continue;
                            }
                            switch (er.tipo) //(manca il controllo per le UO sottoposte)
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

                                        if (er.isVisibile && check_ruoli_utenti(er))
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
            catch (Exception ex)
            {
                retvalue = false;
                Console.WriteLine("Errore durante il reperimento delle Uo in Primo Smistamento: " + ex.Message);
            }
            return retvalue;
        }

        private void setVisibleCheckBoxElement(DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers)
        {
            if (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
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

        protected bool RicercaRubricaEsterna(DocsPaVO.rubrica.ParametriRicercaRubrica qr)
        {
            bool rubricaEsterna = false;

            if ((qr.tipoIE == TipoUtente.GLOBALE || qr.tipoIE == TipoUtente.ESTERNO) && qr.rubricaEsterna != null && qr.rubricaEsterna.Count > 0)
                rubricaEsterna = true;

            return rubricaEsterna;
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

        private void filtra_aoo(DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers)
        {
            ArrayList a = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(_user);
            if (qr.caller.IdRegistro != null && qr.caller.IdRegistro != string.Empty)
                uo_in_aoo = r.GetUoInterneAoo(qr.caller.IdRegistro);
            else
            {
                uo_in_aoo = r.GetUoInterneAooNoReg();
            }

            // ricerco le UO che sono contenute nella dpa_uo_reg, ovvero
            //quelle uo che hanno almeno un registro associato
            uo_in_aoo_with_reg = r.GetUoInterneAooWithReg(_user.idAmministrazione);


            //E' NECESSARIO PRIMA ORDINARE L'ARRAY
            if (uo_in_aoo != null)
                Array.Sort(uo_in_aoo, CaseInsensitiveComparer.Default);

            if (uo_in_aoo_with_reg != null)
                Array.Sort(uo_in_aoo_with_reg, CaseInsensitiveComparer.Default);


            switch (qr.tipoIE)
            {
                case DocsPaVO.addressbook.TipoUtente.ESTERNO:

                    for (int i = 0; i < ers.Count; i++)
                    {
                        DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica)ers[i];
                        try
                        {
                            switch (er.tipo)
                            {
                                case "U":
                                    //se la Uo è interna alla AOO e la UO ha almeno un reg associato
                                    //la aggiungo nella lista dei corrispondenti
                                    if ((!er.interno) || (er.interno && !check_uo(er.codice) && check_uo_withReg(er.codice)))
                                        a.Add(er);
                                    break;
                                case "R":
                                case "P":
                                    if (!check_ruoli_utenti(er))
                                        a.Add(er);
                                    break;
                                case "L":
                                    a.Add(er);
                                    break;
                            }


                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    break;

                default:
                    for (int i = 0; i < ers.Count; i++)
                    {
                        DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica)ers[i];
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
                            Console.WriteLine(ex.Message);
                        }
                    }
                    break;
            }

            ers = a;
        }

        private void filtra_trasmissioni(DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref ArrayList ers)
        {
            ArrayList a = new ArrayList();
            ArrayList ruoli_autorizzati = new ArrayList();

            TipoOggetto tipo_oggetto;
            tipo_oggetto = (qr.ObjectType != null && qr.ObjectType.StartsWith("F:")) ? TipoOggetto.FASCICOLO : TipoOggetto.DOCUMENTO;
            string id_nodo_titolario = (tipo_oggetto == TipoOggetto.FASCICOLO) ? qr.ObjectType.Substring(2) : null;

            DocsPaVO.utente.Ruolo r = UserManager.getRuolo(qr.caller.IdRuolo);
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
                    RagioneTrasmissione rTo = RagioniManager.GetRagione("TO", _user.idAmministrazione);
                    TipoGerarchia gTo = rTo.tipoDestinatario;

                    switch (gTo)
                    {
                        case TipoGerarchia.INFERIORE:
                            ruoli_autorizzati = g.getGerarchiaInf(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                            break;

                        case TipoGerarchia.PARILIVELLO:
                            ruoli_autorizzati = g.getGerarchiaPariLiv(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                            break;

                        case TipoGerarchia.SUPERIORE:
                            ruoli_autorizzati = g.getGerarchiaSup(r, qr.caller.IdRegistro, id_nodo_titolario, tipo_oggetto);
                            break;

                        case TipoGerarchia.TUTTI:
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

            /* 2023-07-04: MEV LOTTO3, ANCHE I GRIGI HANNO ASSOCIATO UN REGISTRO DI AOO PER CUI NON è PIù
             * POSSIBILE TRASMETTERLI A RUOLI DI ALTRE AOO
             * (CODICE MODIFICATO IL FOREACH SUCCESSIVO)
             *
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
            */
            foreach (DocsPaVO.rubrica.ElementoRubrica er in ers)
            {
                try
                {
                    switch (er.tipo)
                    {
                        case "U":
                            if (uo_is_autorizzata((er.interno ? "I" : "E") + @"\" + er.codice, r, c_ruoli_aut))
                                a.Add(er);
                            break;

                        case "R":
                            if (ruolo_is_autorizzato(er.codice, c_ruoli_aut))
                                a.Add(er);
                            break;

                        case "P":
                            if (utente_is_autorizzato((er.interno ? "I" : "E") + @"\" + er.codice, c_ruoli_aut))
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

        public static ArrayList GetListaUOSmistamentoRubrica(string idRegistro)
        {
            ArrayList retValue = new ArrayList();

            DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti dbSmistaDoc = new DocsPaDB.Query_DocsPAWS.SmistamentoDocumenti();
            DataSet ds = dbSmistaDoc.GetListaUOSmistamento(idRegistro);
            dbSmistaDoc = null;

            if (ds.Tables["UO"].Rows.Count > 0)
                retValue = CreateListaUOSmistamento(ds);

            return retValue;
        }

        public string getIdUoAppartenenza(string codiceUoAppartenenza)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            return r.GetIdUoAppartenenza(codiceUoAppartenenza);
        }

        public bool verificaDipendenzaCodRubrica(string idUoAppartenenza, string codiceRubrica)
        {
            DocsPaDB.Query_DocsPAWS.Rubrica r = new DocsPaDB.Query_DocsPAWS.Rubrica(this._user);
            return r.verificaDipendenzaCodRubrica(idUoAppartenenza, codiceRubrica);
        }

        private bool check_ruoli_utenti(ElementoRubrica er)
        {
            string cod_uo = null;
            if (er.tipo == "R")
            {
                cod_uo = (string)h_uo[er.codice];
            }
            else
            {
                string[] ruoli = (string[])h_utenti[er.codice];
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
                    if (check_ruoli_utenti(err))
                        return true;
                }
                return false;
            }
            return check_uo(cod_uo);
        }

        private bool check_uo(string cod_uo)
        {
            int i;

            i = Array.BinarySearch(uo_in_aoo, cod_uo);
            return (i >= 0);
        }

        private bool check_uo_withReg(string currentCodUo)
        {
            int i;

            i = Array.BinarySearch(uo_in_aoo_with_reg, currentCodUo);
            return (i >= 0);
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

        private static ArrayList CreateListaUOSmistamento(DataSet ds)
        {
            ArrayList retValue = new ArrayList();

            DocsPaVO.Smistamento.UOSmistamento uo = null;


            foreach (DataRow rowUO in ds.Tables["UO"].Rows)
            {
                uo = new DocsPaVO.Smistamento.UOSmistamento();
                uo.ID = rowUO["ID"].ToString();
                uo.Codice = rowUO["CODICE_UO"].ToString();
                uo.Descrizione = rowUO["DESCRIZIONE_UO"].ToString();

                retValue.Add(uo);
            }

            return retValue;
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
                    arrTaxCode[k] = x.ToString()[0];
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
                    c = listControl.Substring(x, 1)[0];
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

    }
}
