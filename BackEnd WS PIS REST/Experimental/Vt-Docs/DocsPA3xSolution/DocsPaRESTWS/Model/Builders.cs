using DocsPaVO.addressbook;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class Builders
    {
    }
    #region Builders
    #region TrasmBuilder
    public class TrasmBuilder
    {
        private DocsPaVO.trasmissione.Trasmissione _trasm;
        private InfoUtente _infoUtente;
        private Ruolo _ruolo;
        private string _id;
        private ModelloTrasmissione _modello;
        private bool _isDoc;

        public TrasmBuilder(ModelloTrasmissione modello, string id, InfoUtente infoUtente, Ruolo ruolo, bool isDoc)
        {
            this._infoUtente = infoUtente;
            this._ruolo = ruolo;
            this._id = id;
            this._modello = modello;
            this._isDoc = isDoc;
            init();
        }

        public DocsPaVO.trasmissione.Trasmissione Trasmissione
        {
            get
            {
                return _trasm;
            }
        }

        private void init()
        {
            _trasm = new DocsPaVO.trasmissione.Trasmissione();
            _trasm.noteGenerali = _modello.VAR_NOTE_GENERALI;
            if (_isDoc)
            {
                _trasm.infoDocumento = BusinessLogic.Documenti.DocManager.GetInfoDocumento(_infoUtente, _id, null);
                _trasm.tipoOggetto = TipoOggetto.DOCUMENTO;
            }
            else
            {
                Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(_id, _infoUtente);
                _trasm.infoFascicolo = this.getInfoFascicoloDaFascicolo(fasc);
                _trasm.tipoOggetto = TipoOggetto.FASCICOLO;
            }
            _trasm.utente = BusinessLogic.Utenti.UserManager.getUtente(_infoUtente.idPeople);
            //aggiunta delegato
            if (_infoUtente.delegato != null)
                _trasm.delegato = _infoUtente.delegato.idPeople;

            _trasm.ruolo = _ruolo;
            _trasm.NO_NOTIFY = _modello.NO_NOTIFY;
            TrasmSingoleBuilder trasmSingoleBuilder = new TrasmSingoleBuilder(_infoUtente, _ruolo, _modello);
            for (int i = 0; i < _modello.RAGIONI_DESTINATARI.Count; i++)
            {
                RagioneDest ragDest = (RagioneDest)_modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    MittDest mittDest = (MittDest)destinatari[j];
                    Corrispondente corr = getCorrispondente(mittDest);
                    if (corr != null)
                    {
                        RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById("" + mittDest.ID_RAGIONE);
                        trasmSingoleBuilder.addTrasmSingole(corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA);
                    }
                }
            }
            _trasm.trasmissioniSingole = trasmSingoleBuilder.TrasmissioniSingole;
        }

        private Corrispondente getCorrispondente(MittDest mittDest)
        {
            Corrispondente corr = new Corrispondente();
            if (mittDest.CHA_TIPO_MITT_DEST == "D")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, TipoUtente.INTERNO, _infoUtente);
            }
            else
            {
                SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(_infoUtente, _id, null);
                CorrispondenteBuilder corrBuilder = new CorrispondenteBuilder(mittDest.CHA_TIPO_MITT_DEST, schedaDoc);
                corr = corrBuilder.Corrispondente;
            }
            return corr;
        }

        private InfoFascicolo getInfoFascicoloDaFascicolo(Fascicolo fasc)
        {
            InfoFascicolo infoFasc = new InfoFascicolo();
            infoFasc.idFascicolo = fasc.systemID;
            infoFasc.descrizione = fasc.descrizione;
            infoFasc.idClassificazione = fasc.idClassificazione;
            infoFasc.codice = fasc.codice;
            if (fasc.stato != null && fasc.stato.Equals("A"))
            {
                infoFasc.apertura = fasc.apertura;
            }
            return infoFasc;
        }
    }
    #endregion
    #region TrasmSingoleBuilder
    public class TrasmSingoleBuilder
    {
        private Dictionary<string, TrasmissioneSingola> _trasmissioniSingole;
        private InfoUtente _infoUtente;
        private Ruolo _ruolo;
        private ModelloTrasmissione _modello;

        public TrasmSingoleBuilder(InfoUtente infoUtente, Ruolo ruolo, ModelloTrasmissione modello)
        {
            this._infoUtente = infoUtente;
            this._ruolo = ruolo;
            this._modello = modello;
            _trasmissioniSingole = new Dictionary<string, TrasmissioneSingola>();
        }

        public void addTrasmSingole(Corrispondente corr, RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza)
        {
            if (_trasmissioniSingole.ContainsKey(corr.systemId))
            {
                if (_trasmissioniSingole[corr.systemId].daEliminare) _trasmissioniSingole[corr.systemId].daEliminare = false;
                return;
            }
            TrasmissioneSingola _trasmissioneSingola = new TrasmissioneSingola();
            buildBasicInfo(_trasmissioneSingola, corr, ragione, note, tipoTrasm, scadenza);
            if (corr is Ruolo)
            {
                _trasmissioneSingola.tipoDest = TipoDestinatario.RUOLO;
                ArrayList listaUtenti = queryUtenti(corr);
                if (listaUtenti.Count == 0) return;
                foreach (Utente temp in listaUtenti)
                {
                    addTrasmissioneUtente(temp, _trasmissioneSingola);
                }
                _trasmissioniSingole.Add(corr.systemId, _trasmissioneSingola);
            }
            if (corr is Utente)
            {
                _trasmissioneSingola.tipoDest = TipoDestinatario.UTENTE;
                addTrasmissioneUtente((Utente)corr, _trasmissioneSingola);
                _trasmissioniSingole.Add(corr.systemId, _trasmissioneSingola);
            }
            if (corr is UnitaOrganizzativa)
            {
                UnitaOrganizzativa theUo = (UnitaOrganizzativa)corr;
                QueryCorrispondenteAutorizzato qca = new QueryCorrispondenteAutorizzato();
                qca.ragione = _trasmissioneSingola.ragione;
                qca.ruolo = _ruolo;
                ArrayList ruoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                foreach (Ruolo r in ruoli)
                    addTrasmSingole(r, ragione, note, tipoTrasm, scadenza);
            }
        }

        public ArrayList TrasmissioniSingole
        {
            get
            {
                ArrayList res = new ArrayList();
                foreach (TrasmissioneSingola ts in _trasmissioniSingole.Values) res.Add(ts);
                return res;
            }
        }

        private void addTrasmissioneUtente(Utente utente, TrasmissioneSingola trasmSingola)
        {
            TrasmissioneUtente trasmissioneUtente = new TrasmissioneUtente();
            trasmissioneUtente.utente = utente;
            trasmissioneUtente.daNotificare = daNotificareSuModello(utente.idPeople, trasmSingola.corrispondenteInterno.systemId, _modello);
            trasmSingola.trasmissioneUtente.Add(trasmissioneUtente);
        }

        private static void buildBasicInfo(TrasmissioneSingola trasmissioneSingola, Corrispondente corr, RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza)
        {
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;
            if (scadenza > 0)
            {
                string dataScadenza = "";
                System.DateTime data = System.DateTime.Now.AddDays(scadenza);
                dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                trasmissioneSingola.dataScadenza = dataScadenza;
            }
            trasmissioneSingola.trasmissioneUtente = new ArrayList();
        }

        private ArrayList queryUtenti(Corrispondente corr)
        {
            QueryCorrispondente qco = new QueryCorrispondente();
            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = _infoUtente.idAmministrazione;
            qco.fineValidita = true;
            qco.tipoUtente = TipoUtente.INTERNO;
            return BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco);
        }

        private static bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo, ModelloTrasmissione modello)
        {
            bool retValue = true;
            foreach (RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                foreach (MittDest mittDest in destinatari)
                {
                    if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                    {
                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Count > 0)
                        {
                            foreach (UtentiConNotificaTrasm temp in mittDest.UTENTI_NOTIFICA)
                            {
                                if (temp.ID_PEOPLE.Equals(currentIDPeople))
                                {
                                    if (temp.FLAG_NOTIFICA.Equals("1"))
                                        retValue = true;
                                    else
                                        retValue = false;
                                    return retValue;
                                }
                            }
                        }
                    }
                }
            }
            return retValue;
        }
    }
    #endregion
    #region CorrispondenteBuilder
    public class CorrispondenteBuilder
    {
        private Corrispondente _corrispondente;
        private delegate Corrispondente getCorrFasc(CorrFascInput input);
        private delegate Corrispondente getCorrDoc(CorrDocInput schedaDoc);
        private static Dictionary<string, getCorrFasc> _corrFascDel;
        private static Dictionary<string, getCorrDoc> _corrDocDel;

        private class CorrFascInput
        {
            public Fascicolo fascicolo;
            public Ruolo ruolo;
            public InfoUtente infoUtente;
        }

        private class CorrDocInput
        {
            public SchedaDocumento documento;
            public Ruolo ruolo;
            public InfoUtente infoUtente;
        }

        static CorrispondenteBuilder()
        {
            _corrFascDel = new Dictionary<string, getCorrFasc>();
            _corrFascDel.Add("UT_P", UTPFasc);
            _corrFascDel.Add("R_P", RPFasc);
            _corrFascDel.Add("UO_P", UOPFasc);
            _corrFascDel.Add("RSP_P", RSPPFasc);
            _corrFascDel.Add("R_S", RSFasc);
            _corrFascDel.Add("RSP_M", RSPMFasc);
            _corrFascDel.Add("S_M", SMFasc);
            _corrDocDel = new Dictionary<string, getCorrDoc>();
            _corrDocDel.Add("UT_P", UTPDoc);
            _corrDocDel.Add("R_P", RPDoc);
            _corrDocDel.Add("UO_P", UOPDoc);
            _corrDocDel.Add("RSP_P", RSPPDoc);
            _corrDocDel.Add("R_S", RSDoc);
            _corrDocDel.Add("RSP_M", RSPMDoc);
            _corrDocDel.Add("S_M", SMDoc);
        }

        public CorrispondenteBuilder(string tipoDestinatario, SchedaDocumento schedaDoc)
        {
            CorrDocInput input = null;
            _corrispondente = _corrDocDel[tipoDestinatario](input);
        }

        //public CorrispondenteBuilder(string tipoDestinatario, Fascicolo fascicolo, Ruolo ruolo, UserInfo userInfo)
        //{
        //    CorrFascInput input = null;
        //    _corrispondente = _corrFascDel[tipoDestinatario](input);
        //}

        public Corrispondente Corrispondente
        {
            get
            {
                return _corrispondente;
            }
        }

        #region Documento

        private static Corrispondente UTPDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string utenteProprietario = string.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    utenteProprietario = input.documento.creatoreDocumento.idPeople;
                }
                else utenteProprietario = input.documento.protocollatore.utente_idPeople;

            }
            else
            {
                utenteProprietario = input.documento.creatoreDocumento.idPeople;
            }
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(utenteProprietario, TipoUtente.INTERNO, input.infoUtente);
            return corr;
        }

        private static Corrispondente RPDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliRuolo = string.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    idCorrGlobaliRuolo = input.documento.creatoreDocumento.idCorrGlob_Ruolo;
                }
                else
                    idCorrGlobaliRuolo = input.documento.protocollatore.ruolo_idCorrGlobali;
            }
            else
            {
                idCorrGlobaliRuolo = input.documento.creatoreDocumento.idCorrGlob_Ruolo;
            }
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuolo, TipoUtente.INTERNO, input.infoUtente);
            return corr;
        }

        private static Corrispondente UOPDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
                }
                else
                    idCorrGlobaliUo = input.documento.protocollatore.uo_idCorrGlobali;
            }
            else
            {
                idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
            }
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliUo, TipoUtente.INTERNO, input.infoUtente);
            return corr;
        }

        private static Corrispondente RSPPDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            string idCorr = string.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
                }
                else
                {
                    idCorrGlobaliUo = input.documento.protocollatore.uo_idCorrGlobali;
                }
            }
            else
            {
                idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
            }
            idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO, input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente RSDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            string idCorr = String.Empty;
            if (input.documento.protocollatore != null && input.documento.protocollo != null && !string.IsNullOrEmpty(input.documento.protocollo.numero))
            {
                if (input.documento.creatoreDocumento != null)
                {
                    idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
                }
                else
                {
                    idCorrGlobaliUo = input.documento.protocollatore.uo_idCorrGlobali;
                }
            }
            else
            {
                idCorrGlobaliUo = input.documento.creatoreDocumento.idCorrGlob_UO;
            }
            idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO, input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente RSPMDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = input.ruolo.uo.systemId;
            string idCorr = input.ruolo.systemId;

            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO, input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente SMDoc(CorrDocInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = input.ruolo.uo.systemId;
            string idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO, input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        #endregion
        #region Fascicolo

        private static Corrispondente UTPFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string utenteProprietario = string.Empty;
            utenteProprietario = input.fascicolo.creatoreFascicolo.idPeople;
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(utenteProprietario, TipoUtente.INTERNO, input.infoUtente);
            return corr;
        }

        private static Corrispondente RPFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            idCorrGlobaliUo = input.fascicolo.creatoreFascicolo.idCorrGlob_UO;
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliUo, TipoUtente.INTERNO, input.infoUtente);
            return corr;
        }

        private static Corrispondente UOPFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            idCorrGlobaliUo = input.fascicolo.creatoreFascicolo.idCorrGlob_UO;
            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliUo, TipoUtente.INTERNO, input.infoUtente);
            return corr;
        }

        private static Corrispondente RSPPFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            string idCorr = string.Empty;

            idCorrGlobaliUo = input.fascicolo.creatoreFascicolo.idCorrGlob_UO;
            idCorr = input.fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO, input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente RSFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = string.Empty;
            string idCorr = string.Empty;

            idCorrGlobaliUo = input.fascicolo.creatoreFascicolo.idCorrGlob_UO;
            idCorr = input.fascicolo.creatoreFascicolo.idCorrGlob_Ruolo;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);
            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO, input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente RSPMFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = input.ruolo.uo.systemId;
            string idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);
            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO, input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }

        private static Corrispondente SMFasc(CorrFascInput input)
        {
            Corrispondente corr = new Corrispondente();
            string idCorrGlobaliUo = input.ruolo.uo.systemId;
            string idCorr = input.ruolo.systemId;
            string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);
            if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, TipoUtente.INTERNO, input.infoUtente);
            }
            else
            {
                corr = null;
            }
            return corr;
        }
        #endregion
    }
    #endregion
    #endregion

}