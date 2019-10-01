using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubrica.ClientProxy
{
    /// <summary>
    /// Credenziali per l'accesso al sistema rubrica
    /// </summary>
    [Serializable()]
    public class SecurityCredentials
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
