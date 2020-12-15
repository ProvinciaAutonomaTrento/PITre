using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.AlboTelematico
{
        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class RichiestaPubblicazione
        {
            /// <summary>
            /// 
            /// </summary>
            public string UserID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DocumentoDaPubblicare Documento { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class DocumentoDaPubblicare
        {
            /// <summary>
            /// 
            /// </summary>
            public string IdDocumento { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string StatoDocumento { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public AttributoProfilo[] AttributiProfilo { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public FileDaPubblicare DocumentoPrincipale { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public FileDaPubblicare[] Allegati { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class AttributoProfilo
        {
            /// <summary>
            /// 
            /// </summary>
            public string Nome { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Valore { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class FileDaPubblicare
        {
            /// <summary>
            /// 
            /// </summary>
            public string NomeFile { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public byte[] Contenuto { get; set; }
        }
}