using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    public class Utente
    {
       #region private fields

        private string _idPeople;
        private string _userid;
        private string _nome;
        private string _cognome;
        private string _idruolo;
        private string _codRuolo;
        private string _codAmm;
        private string _authenticationToken;

       #endregion


       #region public property

        public string CodAmm
        {
            get { return _codAmm; }
            set { _codAmm = value; }
        }

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public string Userid
        {
            get { return _userid; }
            set { _userid = value; }
        }

        public string Cognome
        {
            get { return _cognome; }
            set { _cognome = value; }
        }

        public string Idruolo
        {
            get { return _idruolo; }
            set { _idruolo = value; }
        }

        public string CodRuolo
        {
            get { return _codRuolo; }
            set { _codRuolo = value; }
        }

        public string IdPeople
        {
            get { return _idPeople; }
            set { _idPeople = value; }
        }

        public string AuthenticationToken
        {
            get { return _authenticationToken; }
            set { _authenticationToken = value; }
        }

       #endregion
    }
}

