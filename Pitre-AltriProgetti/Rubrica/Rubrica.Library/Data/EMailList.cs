using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rubrica.Library.Data;

namespace Rubrica.Library.Data
{
    /// <summary>
    /// Lista di email e relative note associate ad un corrispondete
    /// </summary>
    [Serializable()]
    public class EMailList :IList<EmailInfo>
    {
        private List<EmailInfo> _mails;

        public EMailList()
        {
            this._mails = new List<EmailInfo>();
        }

        public EMailList(ICollection<EmailInfo> collection)
        {
            this._mails = new List<EmailInfo>(collection);
        }

        public int IndexOf(EmailInfo item)
        {
            return this._mails.IndexOf(item);
        }

        public void Insert(int index, EmailInfo item)
        {
            if (!this.Contains(item))
            {
                // Se l'item da inserire rappresenta una mail preferita, tutte le
                // eventuali altre mail non sono più preferite
                if (item.Preferita)
                    this._mails.ForEach(m => m.Preferita = false);

                this._mails.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            this._mails.RemoveAt(index);

            this.SetFirstMailAsPreferred();
            
        }

        public EmailInfo this[int index]
        {
            get
            {
                return this._mails[index];
            }
            set
            {
                this._mails[index] = value;

                this.SetFirstMailAsPreferred();
            }
        }

        public void Add(EmailInfo item)
        {
            if (!this.Contains(item))
            {
                // Se la mail da inserire è segnata come di default, tutte le altre non
                // lo possono essere
                if (item.Preferita)
                    this._mails.ForEach(m => m.Preferita = false);

                this._mails.Add(item);
            }
            else
                // Se la mail che si è tentato di aggiungere è la preferita, viene aggiornato
                // il relativo flag
                if (item.Preferita)
                    this.SetMailAsPreferred(item.Email);
        }

        public void Clear()
        {
            this._mails.Clear();
        }

        public bool Contains(EmailInfo item)
        {
            return this._mails.Contains(item);
        }

        public void CopyTo(EmailInfo[] array, int arrayIndex)
        {
            this._mails.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this._mails.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(EmailInfo item)
        {
            bool retVal = this._mails.Remove(item);

            this.SetFirstMailAsPreferred();

            return retVal;
            
        }

        /// <summary>
        /// Metodo per la rimozione della mail preferita e per l'impostazione della prima mail diponibile come
        /// preferita
        /// </summary>
        /// <returns>Esito della rimozione</returns>
        public bool RemovePreferred()
        {
            EmailInfo mail = this._mails.Where(m => m.Preferita).FirstOrDefault();
            if (mail != null)
            {
                this._mails.Remove(mail);
                this.SetFirstMailAsPreferred();
            }
            return mail != null;
        }

        public IEnumerator<EmailInfo> GetEnumerator()
        {
            return this._mails.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return null;
        }

        /// <summary>
        /// Metodo per il recupero della mail preferita
        /// </summary>
        /// <returns>Informazioni sulla mail preferita</returns>
        public EmailInfo GetPreferredMail()
        {
            return this._mails.Where(m => m.Preferita).FirstOrDefault();
        }

        /// <summary>
        /// Metodo per l'effettuazione di una azione per tutti gli elementi della lista
        /// </summary>
        /// <param name="action">Azione da eseguire</param>
        public void ForEach(Action<EmailInfo> action)
        {
            this._mails.ForEach(action);
        }

        /// <summary>
        /// Metodo per l'impostazione di una specifica mail come mail preferita
        /// </summary>
        /// <param name="email">Mail da impostare come preferita</param>
        public void SetMailAsPreferred(String email)
        {
            EmailInfo mail = this._mails.Where(m => m.Email == email).FirstOrDefault();
            if (mail != null)
            {
                this._mails.ForEach(m => m.Preferita = false);
                mail.Preferita = true;
            }
        }
     

        /// <summary>
        /// Metodo per l'impostazione della prima mail come preferita se non ne è già selezionata una
        /// </summary>
        public void SetFirstMailAsPreferred()
        {
            // Se la mail eliminata è la preferita, viene impostata come preferita 
            // la prima mail presente
            if (this._mails.Count(m => m.Preferita) == 0 && this._mails.Count > 0)
                this._mails[0].Preferita = true;

        }

    }

}
