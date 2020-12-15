using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Smistamento;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.Mobile
{
    public abstract class SmistamentoNode
    {

        public string Id
        {
            get;
            set;
        }

        public string Descrizione
        {
            get;
            set;
        }

        public abstract SmistamentoNodeType Type
        {
            get;
        }

        public SmistamentoCheckType CheckType
        {
            get;
            set;
        }
    }

    public class SmistamentoUONode : SmistamentoNode
    {
        public SmistamentoUONode()
        {
            Ruoli = new List<SmistamentoRuoloNode>();
        }

        public List<SmistamentoRuoloNode> Ruoli
        {
            get;
            set;
        }

        /*public UOSmistamento UOSmistamento
        {
            get
            {
                UOSmistamento res = new UOSmistamento();
                res.FlagCompetenza = (CheckType == SmistamentoCheckType.COMPETENZA);
                res.FlagConoscenza = (CheckType == SmistamentoCheckType.CONOSCENZA);
                res.ID = Id;
                res.Descrizione = Descrizione;
                res.UoInferiori = new ArrayList();
                res.Ruoli = new ArrayList(Ruoli.Select(e => e.RuoloSmistamento).ToList());
                return res;
            }
        }*/

        public static SmistamentoUONode BuildInstance(UOSmistamento input,UserInfo userInfo,string idRuolo)
        {
            SmistamentoUONode res = new SmistamentoUONode();
            if (input.FlagCompetenza) res.CheckType = SmistamentoCheckType.COMPETENZA;
            if (input.FlagConoscenza) res.CheckType = SmistamentoCheckType.CONOSCENZA;
            res.Descrizione = input.Descrizione;
            res.Id = input.ID;
            //IEnumerable<RuoloSmistamento> ruoli=input.Ruoli.OfType<RuoloSmistamento>().Where(e => !e.ID.Equals(idRuolo));
            IEnumerable<RuoloSmistamento> ruoli=input.Ruoli.OfType<RuoloSmistamento>().Where(e => !e.ID.Equals(idRuolo) || e.Utenti.Count > 1);
            foreach (RuoloSmistamento temp in ruoli) res.Ruoli.Add(SmistamentoRuoloNode.BuildInstance(temp, userInfo));
            //il ruolo dell'utente va messo solo se c'è almeno un'altra persona nel ruolo
            //IEnumerable<RuoloSmistamento> ruoloUtente = input.Ruoli.OfType<RuoloSmistamento>().Where(e => e.ID.Equals(idRuolo) && e.Utenti.Count > 1);
            //foreach (RuoloSmistamento temp in ruoloUtente) res.Ruoli.Add(SmistamentoRuoloNode.BuildInstance(temp, userInfo));
            return res;
        }

        public override SmistamentoNodeType Type
        {
            get
            {
                return SmistamentoNodeType.UO;
            }
        }
    }

    public class SmistamentoRuoloNode : SmistamentoNode
    {

        public SmistamentoRuoloNode()
        {
            Utenti = new List<SmistamentoUtenteNode>();
        }

        public List<SmistamentoUtenteNode> Utenti
        {
            get;
            set;
        }

        /*public RuoloSmistamento RuoloSmistamento
        {
            get
            {
                RuoloSmistamento res = new RuoloSmistamento();
                res.FlagCompetenza = (CheckType == SmistamentoCheckType.COMPETENZA);
                res.FlagConoscenza = (CheckType == SmistamentoCheckType.CONOSCENZA);
                res.ID = Id;
                res.Utenti = new ArrayList(Utenti.Select(e => e.UtenteSmistamento).ToList());
                return res;
            }
        }*/

        public static SmistamentoRuoloNode BuildInstance(RuoloSmistamento input,UserInfo userInfo)
        {
            SmistamentoRuoloNode res = new SmistamentoRuoloNode();
            if (input.FlagCompetenza) res.CheckType = SmistamentoCheckType.COMPETENZA;
            if (input.FlagConoscenza) res.CheckType = SmistamentoCheckType.CONOSCENZA;
            res.Descrizione = input.Descrizione;
            res.Id = input.ID;
            IEnumerable<UtenteSmistamento> utenti = input.Utenti.OfType<UtenteSmistamento>().Where(e => !e.UserID.Equals(userInfo.UserId));
            foreach (UtenteSmistamento temp in utenti) res.Utenti.Add(SmistamentoUtenteNode.BuildInstance(temp));
            return res;
        }

        public override SmistamentoNodeType Type
        {
            get
            {
                return SmistamentoNodeType.RUOLO;
            }
        }
    }

    public class SmistamentoUtenteNode : SmistamentoNode
    {

        /*public UtenteSmistamento UtenteSmistamento
        {
            get
            {
                UtenteSmistamento res = new UtenteSmistamento();
                res.FlagCompetenza = (CheckType == SmistamentoCheckType.COMPETENZA);
                res.FlagConoscenza = (CheckType == SmistamentoCheckType.CONOSCENZA);
                res.Denominazione = Descrizione;
                res.ID = Id;
                return res;
            }
        }*/

        public static SmistamentoUtenteNode BuildInstance(UtenteSmistamento input)
        {
            SmistamentoUtenteNode res = new SmistamentoUtenteNode();
            if (input.FlagCompetenza) res.CheckType = SmistamentoCheckType.COMPETENZA;
            if (input.FlagConoscenza) res.CheckType = SmistamentoCheckType.CONOSCENZA;
            res.Descrizione = input.Denominazione;
            res.Id = input.ID;
            return res;
        }

        public override SmistamentoNodeType Type
        {
            get
            {
                return SmistamentoNodeType.UTENTE;
            }
        }
    }

    public enum SmistamentoNodeType
    {
        UO, RUOLO, UTENTE
    }

    public enum SmistamentoCheckType
    {
        COMPETENZA, CONOSCENZA, UNCHECKED
    }
}
