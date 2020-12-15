using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.ExternalServices
{
    [Serializable()]
    public class Servizio
    {
        private string idServizio;
        private string descrizione;
        private string codiceEsecutore;
        private string percorso;
        private Dictionary<string, string> parametro;
        private List<Dictionary<string, string>> parametri;

        public string getId()
        {
            return this.idServizio;
        }

        public void setId(string database_SystemId)
        {
            if (!string.IsNullOrEmpty(database_SystemId))
                this.idServizio = database_SystemId;
        }

        public string getDescrizione()
        {
            return this.descrizione;
        }

        public void setDescrizione(string strDescrizione)
        {
            if (!string.IsNullOrEmpty(strDescrizione))
                this.descrizione = strDescrizione;
        }

        public string getCodiceEsecutore()
        {
            return this.codiceEsecutore;
        }

        public void setCodiceEsecutore(string idEsecutore)
        {
            if (!string.IsNullOrEmpty(idEsecutore))
                this.codiceEsecutore = idEsecutore;
        }

        public string getUrl()
        {
            return this.percorso;
        }

        public void setUrl(string urlServizio)
        {
            if (!string.IsNullOrEmpty(urlServizio))
                this.percorso = urlServizio;
        }

        public List<Dictionary<string, string>> getParametri()
        {
            if (this.parametri == null)
                this.parametri = new List<Dictionary<string, string>>();

            return parametri;
        }

        public void addParametro(string descrizione, string tipo)
        {
            if (!string.IsNullOrEmpty(descrizione))
            {
                this.parametro = new Dictionary<string, string>();
                parametro.Add(descrizione.ToUpper(), tipo);

                if (!this.getParametri().Contains(parametro))
                {
                    this.parametri.Add(parametro);
                }
            }
        }

        public void removeParametro(string descrizione, string tipo)
        {
            if (!string.IsNullOrEmpty(descrizione))
            {
                this.parametro = new Dictionary<string, string>();
                parametro.Add(descrizione.ToUpper(), tipo);

                if (this.getParametri().Contains(parametro))
                {
                    this.parametri.Remove(parametro);
                }
            }
        }

    }
}
