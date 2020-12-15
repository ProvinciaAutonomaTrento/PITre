using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.Mobile;
using DocsPaVO.trasmissione;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.utente;
using System.Collections;
using DocsPaVO.addressbook;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;

namespace DocsPaWS.Mobile.Builders
{
    public class TrasmBuilder
    {
        private Trasmissione _trasm;
        private InfoUtente _infoUtente;
        private Ruolo _ruolo;
        private string _id;
        private ModelloTrasmissione _modello;
        private bool _isDoc;

        public TrasmBuilder(ModelloTrasmissione modello, string id,InfoUtente infoUtente,Ruolo ruolo,bool isDoc)
        {
            this._infoUtente = infoUtente;
            this._ruolo = ruolo;
            this._id = id;
            this._modello=modello;
            this._isDoc = isDoc;
            init();
        }

        public Trasmissione Trasmissione
        {
            get
            {
                return _trasm;
            }
        }

        private void init()
        {
            _trasm = new Trasmissione();
            _trasm.noteGenerali = _modello.VAR_NOTE_GENERALI;
            if (_isDoc)
            {
                _trasm.infoDocumento = BusinessLogic.Documenti.DocManager.GetInfoDocumento(_infoUtente, _id, null);
                _trasm.tipoOggetto = TipoOggetto.DOCUMENTO;
            }
            else
            {
                Fascicolo fasc=BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(_id, _infoUtente);
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
}