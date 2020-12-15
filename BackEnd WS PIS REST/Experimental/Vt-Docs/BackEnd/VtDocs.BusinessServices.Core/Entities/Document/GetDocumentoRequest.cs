using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    [Serializable()]
    //[System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/Entities/Document/GetDocumentoRequest")]
    public class GetDocumentoRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public string IdProfile
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DocNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Ricerca del documento per campo profilato
        /// </summary>
        /// <remarks>
        /// Alternativo a IdProfile e DocNumber
        /// </remarks>
        public FiltroPerTipoDocumento TipoDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool GetStatoDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il servizio dovrà restituire anche la lista dei mezzi di spedizione visibili dall'utente
        /// </summary>
        public bool GetMezzoSpedizione
        {
            get;
            set;
        }

        /// <summary>
        /// Classe per la ricerca di un documento a partire dai valori presenti nei campi profilati
        /// </summary>
        /// <remarks>
        /// Il campo dovrà contenere valori univoci
        /// </remarks>
        [Serializable()]
        public class FiltroPerTipoDocumento
        {
            /// <summary>
            /// 
            /// </summary>
            public FiltroPerTipoDocumento()
            {
                this.CampiProfilati = new List<CampoProfilato>();
            }

            /// <summary>
            /// 
            /// </summary>
            public string DescrizioneTipoDocumento
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string StatoDocumento
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string StatoDocumentoDiversoDa
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public List<CampoProfilato> CampiProfilati
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            [Serializable()]
            public class CampoProfilato
            {
                /// <summary>
                /// 
                /// </summary>
                public CampoProfilato()
                { }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="nomeCampoProfilato"></param>
                /// <param name="valoreCampoProfilato"></param>
                public CampoProfilato(string nomeCampoProfilato, string valoreCampoProfilato) : this()
                {
                    this.NomeCampoProfilato = nomeCampoProfilato;
                    this.ValoreCampoProfilato = valoreCampoProfilato;
                }

                /// <summary>
                /// 
                /// </summary>
                public string NomeCampoProfilato
                {
                    get;
                    set;
                }

                /// <summary>
                /// 
                /// </summary>
                public string ValoreCampoProfilato
                {
                    get;
                    set;
                }
            }
        }
    }
}