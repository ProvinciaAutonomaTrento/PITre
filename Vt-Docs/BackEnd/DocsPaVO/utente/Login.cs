using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.utente
{
    [Serializable()]
    public class Login
    {
        public string userName = string.Empty;
        public string password = string.Empty;
        public string idAmministrazione = string.Empty;
        public string dominio = string.Empty;
        public bool update = false;
    }
}
