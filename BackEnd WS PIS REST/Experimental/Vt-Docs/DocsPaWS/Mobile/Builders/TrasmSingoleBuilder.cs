using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using System.Collections;
using DocsPaVO.addressbook;
using DocsPaVO.Modelli_Trasmissioni;

namespace DocsPaWS.Mobile.Builders
{
    public class TrasmSingoleBuilder
    {
        private Dictionary<string,TrasmissioneSingola> _trasmissioniSingole;
        private InfoUtente _infoUtente;
        private Ruolo _ruolo;
        private ModelloTrasmissione _modello;

        public TrasmSingoleBuilder(InfoUtente infoUtente,Ruolo ruolo,ModelloTrasmissione modello)
        {
            this._infoUtente = infoUtente;
            this._ruolo = ruolo;
            this._modello = modello;
            _trasmissioniSingole = new Dictionary<string,TrasmissioneSingola>();
        }

        public void addTrasmSingole(Corrispondente corr,RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza){
            if(_trasmissioniSingole.ContainsKey(corr.systemId)){
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
}