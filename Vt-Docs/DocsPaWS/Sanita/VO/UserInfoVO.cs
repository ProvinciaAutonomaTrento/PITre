using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.utente;
using DocsPaVO.amministrazione;

namespace DocsPaWS.Sanita.VO
{
    public class UserInfoVO
    {
        public string Descrizione
        {
            get; 
            set; 
        }

        public string UserId
        {
            get; 
            set;
        }

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

        public string IdPeople
        {
            get; 
            set;
        }

        public string Token
        {
            get;
            set;
        }

        public InfoUtente InfoUtente{
            get
            {
                InfoUtente res = new InfoUtente();
                res.userId = UserId;
                res.idAmministrazione = IdAmministrazione;
                res.dst = Dst;
                res.idPeople = IdPeople;
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