using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    /// <summary>
	/// </summary>
    [Serializable]
	public class MailCorrispondente
    {
        public string systemId;
        public string Email;
        public string Note;
        public string Principale;

        public override bool Equals(object obj)
        {
            return obj is MailCorrispondente && ((MailCorrispondente)obj).Email == this.Email;
        }
        //aggiunto GetHashCode perchè se no equals si arrabbia
        public override int GetHashCode()
        {
            return this.Email.GetHashCode();
        }
    }
}
