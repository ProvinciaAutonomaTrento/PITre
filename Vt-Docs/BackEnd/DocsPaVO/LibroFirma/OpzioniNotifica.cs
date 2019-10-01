using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public class OpzioniNotifica
    {
        private string _notifica_interrotto; //NOTIFICA_INTERROTTO
        private string _notifica_concluso; //NOTIFICA_CONCLUSO
        private string _notifica_errore; //NOTIFICA_ERRORE
        private string _notifica_dest_non_interoperanti; //NOTIFICA_DEST_NON_INTEROPERANTI



        /// <summary>
        /// Richiesta notifica per interruzione
        /// </summary>
        public bool Notifica_interrotto
        {
            get
            {
                bool retVal = false;
                if (!string.IsNullOrEmpty(_notifica_interrotto))
                {
                    bool.TryParse(_notifica_interrotto, out retVal);
                }

                return retVal;
            }
            set
            {
                _notifica_interrotto = value.ToString();
            }
        }

        /// <summary>
        /// Richiesta notifica per conclusione
        /// </summary>
        public bool Notifica_concluso
        {
            get
            {
                bool retVal = false;
                if (!string.IsNullOrEmpty(_notifica_concluso))
                {
                    bool.TryParse(_notifica_concluso, out retVal);
                }

                return retVal;
            }
            set
            {
                _notifica_concluso = value.ToString();
            }
        }

        /// <summary>
        /// Richiesta notifica di errore
        /// </summary>
        public bool NotificaErrore
        {
            get
            {
                bool retVal = false;
                if (!string.IsNullOrEmpty(_notifica_errore))
                {
                    bool.TryParse(_notifica_errore, out retVal);
                }

                return retVal;
            }
            set
            {
                _notifica_errore = value.ToString();
            }
        }

        /// <summary>
        /// Richiesta notifica presenza destinatari non interoperanti
        /// </summary>
        public bool NotificaPresenzaDestNonInterop
        {
            get
            {
                bool retVal = false;
                if (!string.IsNullOrEmpty(_notifica_dest_non_interoperanti))
                {
                    bool.TryParse(_notifica_dest_non_interoperanti, out retVal);
                }

                return retVal;
            }
            set
            {
                _notifica_dest_non_interoperanti = value.ToString();
            }
        }

    }
}
