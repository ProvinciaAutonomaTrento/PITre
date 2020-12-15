using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    [Serializable()]
    public class HSMParameters
    {
      #region private fields

        private string _id_amm;
        private string _dominio;
        private string _tsaurl;
        private string _tsauser;
        private string _tsapassword;
        private string _userapplicativa;
        private string _passwordapplicativa;
        private string _serverurl;

      #endregion

      #region public property
        /// <summary>
        /// System id del passo
        /// </summary>
        public string id_amm
        {
            get
            {
                return _id_amm;
            }

            set
            {
                _id_amm = value;
            }
        }

        public string dominio
        {
            get
            {
                return _dominio;
            }

            set
            {
                _dominio = value;
            }
        }

        /// <summary>
        /// System id del passo
        /// </summary>
        public string tsaurl
        {
            get
            {
                return _tsaurl;
            }

            set
            {
                _tsaurl = value;
            }
        }

        /// <summary>
        /// System id del passo
        /// </summary>
        public string tsauser
        {
            get
            {
                return _tsauser;
            }

            set
            {
                _tsauser = value;
            }
        }

        /// <summary>
        /// System id del passo
        /// </summary>
        public string tsapassword
        {
            get
            {
                return _tsapassword;
            }

            set
            {
                _tsapassword = value;
            }
        }

        /// <summary>
        /// System id del passo
        /// </summary>
        public string userapplicativa
        {
            get
            {
                return _userapplicativa;
            }

            set
            {
                _userapplicativa = value;
            }
        }

        /// <summary>
        /// System id del passo
        /// </summary>
        public string passwordapplicativa
        {
            get
            {
                return _passwordapplicativa;
            }

            set
            {
                _passwordapplicativa = value;
            }
        }

        /// <summary>
        /// System id del passo
        /// </summary>
        public string serverurl
        {
            get
            {
                return _serverurl;
            }

            set
            {
                _serverurl = value;
            }
        }

    #endregion

    }
}
