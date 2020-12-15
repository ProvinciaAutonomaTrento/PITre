using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.utente;
using DocsPaVO.trasmissione;
using DocsPaVO.addressbook;
using BusinessLogic.Utenti;
using DocsPaVO.documento;
using System.Collections;
using DocsPaWS.Sanita.VO;
using BusinessLogic.Trasmissioni;
using log4net;

namespace DocsPaWS.Sanita.Builders
{
    public class TrasmissioneBuilder
    {
        private static ILog logger = LogManager.GetLogger(typeof(TrasmissioneBuilder));
        private InfoUtente _mittenteIU;
        private Utente _mittente;
        private Ruolo _ruoloMittente;
        private TrasmissioneVO _trasmInfo;
        private RagioneTrasmissione _ragione;
        private TrasmissioneSingolaStrategy _tsStrategy;

        public TrasmissioneBuilder(InfoUtente infoUtente, Ruolo ruoloMittente, TrasmissioneVO trasmInfo)
        {
            this._trasmInfo = trasmInfo;
            this._mittente = UserManager.getUtente(infoUtente.idPeople);
            this._ruoloMittente = ruoloMittente;
            this._mittenteIU = infoUtente;
            this._ragione = RagioniManager.getRagioneByCodice(_mittente.idAmministrazione, _trasmInfo.Ragione);
            logger.Debug("Ricerca corrispondente con codice " + trasmInfo.CodiceCorrispondente);
            Corrispondente destinatario = UserManager.getCorrispondenteByCodRubrica(trasmInfo.CodiceCorrispondente, TipoUtente.INTERNO, infoUtente);
            if (destinatario == null)
            {
                logger.Debug("Corrispondente non trovato");
                throw new Exception();
            }
            _tsStrategy = TrasmissioneSingolaStrategy.GetStrategy(destinatario,this);
        }

        public Trasmissione BuildTrasmissione(InfoDocumento documento)
        {
            Trasmissione trasm = new Trasmissione();
            trasm.infoDocumento = documento;
            trasm.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
            trasm.noteGenerali = _trasmInfo.Note;
            trasm.utente = _mittente;
            trasm.ruolo = _ruoloMittente;
            _tsStrategy.TrasmissioniSingole.ForEach(e => trasm.trasmissioniSingole.Add(e));
            return trasm;
        }

        private TrasmissioneSingola Template
        {
            get
            {
                TrasmissioneSingola res = new TrasmissioneSingola();
                res.tipoTrasm = "S";
                res.ragione = _ragione;
                res.noteSingole = _trasmInfo.NoteSingola;
                if (_trasmInfo.GiorniScadenza > 0)
                {
                    string dataScadenza = "";
                    System.DateTime data = System.DateTime.Now.AddDays(_trasmInfo.GiorniScadenza);
                    dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
                    res.dataScadenza = dataScadenza;
                }
                res.trasmissioneUtente = new ArrayList();
                return res;
            }
        }

        internal abstract class TrasmissioneSingolaStrategy
        {
            protected Corrispondente _corrispondente;
            protected TrasmissioneBuilder _builder;

            public TrasmissioneSingolaStrategy(Corrispondente corr, TrasmissioneBuilder builder)
            {
                _corrispondente = corr;
                _builder = builder;
            }

            public static TrasmissioneSingolaStrategy GetStrategy(Corrispondente corr, TrasmissioneBuilder builder)
            {
                if (corr is Ruolo) return new TrasmissioneSingolaRuoloStrategy(corr, builder);
                if (corr is Utente) return new TrasmissioneSingolaUtenteStrategy(corr, builder);
                if (corr is UnitaOrganizzativa) return new TrasmissioneSingolaUOStrategy(corr, builder);
                return null;
            }

            public abstract List<TrasmissioneSingola> TrasmissioniSingole
            {
                get;
            }

            protected void AddTrasmissioneUtente(Utente utente, TrasmissioneSingola trasm)
            {
                TrasmissioneUtente trasmissioneUtente = new TrasmissioneUtente();
                trasmissioneUtente.utente = utente;
                trasmissioneUtente.daNotificare = true;
                trasm.trasmissioneUtente.Add(trasmissioneUtente);
            }

            protected List<Utente> QueryUtenti(Corrispondente corr)
            {
                QueryCorrispondente qco = new QueryCorrispondente();
                qco.codiceRubrica = corr.codiceRubrica;
                qco.getChildren = true;
                qco.idAmministrazione = _builder._mittente.idAmministrazione;
                qco.fineValidita = true;
                qco.tipoUtente = TipoUtente.INTERNO;
                return BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco).Cast<Utente>().ToList();
            }
        }

        internal class TrasmissioneSingolaUtenteStrategy : TrasmissioneSingolaStrategy
        {
            public TrasmissioneSingolaUtenteStrategy(Corrispondente corr, TrasmissioneBuilder builder)
                : base(corr, builder)
            {
            }

            public override List<TrasmissioneSingola> TrasmissioniSingole
            {
                get
                {
                    List<TrasmissioneSingola> res=new List<TrasmissioneSingola>();
                    TrasmissioneSingola trasmSingola = _builder.Template;
                    trasmSingola.tipoDest = TipoDestinatario.UTENTE;
                    trasmSingola.corrispondenteInterno = _corrispondente;
                    AddTrasmissioneUtente((Utente)_corrispondente, trasmSingola);
                    res.Add(trasmSingola);
                    return res;
                }
            }
        }

        internal class TrasmissioneSingolaRuoloStrategy : TrasmissioneSingolaStrategy
        {
            private List<Utente> _utenti;

            public TrasmissioneSingolaRuoloStrategy(Corrispondente corr, TrasmissioneBuilder builder)
                : base(corr, builder)
            {
                _utenti = QueryUtenti(_corrispondente);
            }

            public override List<TrasmissioneSingola> TrasmissioniSingole
            {
                get {
                    List<TrasmissioneSingola> res = new List<TrasmissioneSingola>();
                    TrasmissioneSingola trasmSingola = _builder.Template;
                    trasmSingola.tipoDest = TipoDestinatario.RUOLO;
                    trasmSingola.corrispondenteInterno = _corrispondente;
                    _utenti.ForEach(e => AddTrasmissioneUtente(e, trasmSingola));
                    res.Add(trasmSingola);
                    return res;
                }
            }
        }

        internal class TrasmissioneSingolaUOStrategy : TrasmissioneSingolaStrategy
        {
            private List<Utente> _utenti;
            public TrasmissioneSingolaUOStrategy(Corrispondente uo, TrasmissioneBuilder builder)
                : base(uo, builder)
            {
                UnitaOrganizzativa theUo = (UnitaOrganizzativa)_corrispondente;
                QueryCorrispondenteAutorizzato qca = new QueryCorrispondenteAutorizzato();
                qca.ragione = _builder.Template.ragione;
                qca.ruolo = _builder._ruoloMittente;
                ArrayList ruoli = addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                foreach (Ruolo ruolo in ruoli)
                {
                    QueryUtenti(ruolo).ForEach(e => _utenti.Add(e));

                }

            }

            public override List<TrasmissioneSingola> TrasmissioniSingole
            {
                get {
                    List<TrasmissioneSingola> res = new List<TrasmissioneSingola>();
                    TrasmissioneSingola trasmSingola = _builder.Template;
                    trasmSingola.corrispondenteInterno = _corrispondente;
                    _utenti.ForEach(e => AddTrasmissioneUtente(e, trasmSingola));
                    res.Add(trasmSingola);
                    return res;
                }
            }
        }
    }

}
