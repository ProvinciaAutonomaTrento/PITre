using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.utente;
using DocsPaVO.amministrazione;

namespace DocsPaVO.Mobile
{
    public class UserInfo :LightUserInfo
    {
       
        public string IdAmministrazione
        {
            get; 
            set;
        }

        public string Dst
        {
            get; 
            set;
        }

        public List<RuoloInfo> Ruoli
        {
            get; 
            set;
        }

        public string Token
        {
            get;
            set;

        }

        public UserInfo DelegatoInfo
        {
            get;
            set;
        }

        public static UserInfo buildInstance(Utente input)
        {
            UserInfo res = new UserInfo();
            res.Descrizione = input.descrizione;
            res.UserId = input.userId;
            res.IdAmministrazione = input.idAmministrazione;
            res.Dst = input.dst;
            res.IdPeople = input.idPeople;
            List<RuoloInfo> ruoliList = new List<RuoloInfo>();
            foreach (Object temp in input.ruoli)
            {
                ruoliList.Add(RuoloInfo.buildInstance((Ruolo)temp));
            }
            res.Ruoli = ruoliList;
            return res;
        }

        public static UserInfo buildInstance(OrgUtente input)
        {
            UserInfo res = new UserInfo();
            res.Descrizione = input.Cognome + " " + input.Nome;
            res.UserId = input.UserId;
            return res;
        }

        public InfoUtente InfoUtente{
            get
            {
                InfoUtente res = new InfoUtente();
                res.userId = UserId;
                res.idAmministrazione = IdAmministrazione;
                res.dst = Dst;
                res.idPeople = IdPeople;
                if (this.DelegatoInfo != null)
                    res.delegato = this.DelegatoInfo.InfoUtente;

                return res;
            }
        }

        public Utente Utente
        {
            get
            {
                Utente res = new Utente();
                res.idAmministrazione = IdAmministrazione;
                res.idPeople = IdPeople;
                res.descrizione = Descrizione;
                return res;
            }
        }

    }
}