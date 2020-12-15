using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Smistamento;

namespace DocsPaVO.Mobile.Requests
{
    public class EseguiSmistamentoRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public RuoloInfo Ruolo
        {
            get;
            set;
        }

        public string IdDocumento
        {
            get;
            set;
        }

        public string IdTrasmissione
        {
            get;
            set;
        }

        public string IdTrasmissioneUtente
        {
            get;
            set;
        }

        public string IdTrasmissioneSingola
        {
            get;
            set;
        }

        public string NoteGenerali
        {
            get;
            set;
        }

        public List<EseguiSmistamentoElement> Elements
        {
            get;
            set;
        }
    }

    public class EseguiSmistamentoElement
    {
        public string IdUtente
        {
            get;
            set;
        }

        public string IdUO
        {
            get;
            set;
        }

        public string IdRuolo
        {
            get;
            set;
        }

        public bool Competenza
        {
            get;
            set;
        }

        public bool Conoscenza
        {
            get;
            set;
        }

        public string NoteIndividuali
        {
            get;
            set;
        }

        public bool isRicerca
        {
            get;
            set;
        }

        public void fillUOSmistamento(UOSmistamento uo)
        {

            if (!string.IsNullOrEmpty(IdUtente))
                if ((IdUtente.ToLower()) == "null")
                    IdUtente = null;

            if (!string.IsNullOrEmpty(IdRuolo))
                if ((IdRuolo.ToLower()) == "null")
                    IdRuolo = null;
            // MEV MOBILE - modifica per smistamento da ricerca
            #region OLD CODE
            if (!uo.ID.Equals(IdUO)) 
                return;
            #endregion
            // fine MEV MOBILE
            if (!string.IsNullOrEmpty(IdUtente))
            {
                //id utente valorizzato: smistamento ad utente.
                RuoloSmistamento ruolo=uo.Ruoli.OfType<RuoloSmistamento>().Single(e => e.ID.Equals(IdRuolo));
                UtenteSmistamento utente = ruolo.Utenti.OfType<UtenteSmistamento>().Single(e => e.ID.Equals(IdUtente));
                utente.FlagCompetenza = Competenza;
                utente.FlagConoscenza = Conoscenza;
                utente.datiAggiuntiviSmistamento.NoteIndividuali = NoteIndividuali;
            }
            else
            {
                if (!string.IsNullOrEmpty(IdRuolo))
                {
                    //il ruolo è specificato
                    RuoloSmistamento ruolo = uo.Ruoli.OfType<RuoloSmistamento>().Single(e => e.ID.Equals(IdRuolo));
                    ruolo.FlagCompetenza = Competenza;
                    ruolo.FlagConoscenza = Conoscenza;
                    ruolo.datiAggiuntiviSmistamento.NoteIndividuali = NoteIndividuali;
                }
                else {
                    uo.FlagCompetenza = Competenza;
                    uo.FlagConoscenza = Conoscenza;
                }
            }
        }

    }
}
