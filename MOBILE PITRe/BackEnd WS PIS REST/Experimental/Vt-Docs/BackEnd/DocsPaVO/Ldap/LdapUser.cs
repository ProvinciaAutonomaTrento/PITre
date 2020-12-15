using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Ldap
{
    /// <summary>
    /// Rappresenta un utente in LDAP
    /// </summary>
    [Serializable()]
    public class LdapUser
    {
        /// <summary>
        /// Campo USERPRINCIPALNAME (obbligatorio univoco)
        /// </summary>
        public string UserID = string.Empty;

        /// <summary>
        /// Campo EMPLOYEEID (obbligatorio univoco)
        /// </summary>
        public string Matricola = string.Empty;

        /// <summary>
        /// Campo MAIL (obbligatorio univoco)
        /// </summary>
        public string Email = string.Empty;

        /// <summary>
        /// Campo GIVENNAME
        /// </summary>
        public string Nome = string.Empty;

        /// <summary>
        /// Campo SN
        /// </summary>
        public string Cognome = string.Empty;

        /// <summary>
        /// Campo STREETADDRESS
        /// </summary>
        public string Sede = string.Empty;

        /// <summary>
        /// Distinguished name dell'utente in LDAP
        /// </summary>
        public string DN = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore()]
        public string ErrorMessage = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore()]
        public string Key
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Matricola))
                    return this.Matricola.ToUpper();
                else
                    return this.DN;
            }
        }
    }
}