using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.FlussoAutomatico
{
    public class Messaggio
    {
        private string id;
        private string descrizione;
        private bool iniziale;
        private bool finale;

        /// <summary>
        /// Indentificativo del messaggio
        /// </summary>
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// Descrizione del messaggio
        /// </summary>
        public string DESCRIZIONE
        {
            get
            {
                return descrizione;
            }
            set
            {
                descrizione = value;
            }
        }

        /// <summary>
        /// Messagio iniziale
        /// </summary>
        public bool INIZIALE
        {
            get
            {
                return iniziale;
            }
            set
            {
                iniziale = value;
            }
        }

        /// <summary>
        /// Messaggio finale
        /// </summary>
        public bool FINALE
        {
            get
            {
                return finale;
            }
            set
            {
                finale = value;
            }
        }
    }
}
