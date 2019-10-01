using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Rubrica.Library.Data
{
    /// <summary>
    /// E-mail associata al corrispondente con relative note
    /// </summary>
    [Serializable]
    public class EmailInfo
    {
        public EmailInfo() { }

        public EmailInfo(String email, bool preferita)
        {
            this.Email = email;
            this.Preferita = preferita;
        }

        /// <summary>
        /// Indirizzo Email
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Note associate alla mail
        /// </summary>
        public String Note { get; set; }

        /// <summary>
        /// True se questa è la mail preferita
        /// </summary>
        public bool Preferita { get; set; }

        /// <summary>
        /// Due oggetti di questoi tipo sono uguali se fanno riferimento alla stessa casella email
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is EmailInfo && (obj as EmailInfo).Email.Equals(this.Email);
        }

        public override int GetHashCode()
        {
            return this.Email.GetHashCode();
        }
    }
}
