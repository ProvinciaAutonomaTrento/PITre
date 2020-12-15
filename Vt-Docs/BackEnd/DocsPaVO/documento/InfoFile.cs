using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.documento
{
    public class InfoFile
    {
        private string systemId;
        private string idProfile;
        private string idDocumentoPrincipale;
        private string versionId;
        private string dataAcquisizione = string.Empty;
        private string estensione = string.Empty;
        private string nomeFile = string.Empty;
        private string oggetto;
        private string descrizioneInfoFile = string.Empty;
        private bool conforme = true;
        private bool estensioneConforme = true;
        private bool contieneMacro = false;
        private bool contieneForms = false;
        private bool contieneJavascript = false;

        public string SystemId
        {
            get
            {
                return systemId;
            }

            set
            {
                systemId = value;
            }
        }

        /// <summary>
        /// Id del documento
        /// </summary>
        public string IdProfile
        {
            get
            {
                return idProfile;
            }

            set
            {
                idProfile = value;
            }
        }

        /// <summary>
        /// Se allegato, id del documento principale
        /// </summary>
        public string IdDocumentoPrincipale
        {
            get
            {
                return idDocumentoPrincipale;
            }

            set
            {
                idDocumentoPrincipale = value;
            }
        }

        /// <summary>
        /// Id dell'ultima versione
        /// </summary>
        public string VersionId
        {
            get
            {
                return versionId;
            }

            set
            {
                versionId = value;
            }
        }

        /// <summary>
        /// Data acquisizione del file
        /// </summary>
        public string DataAcquisizione
        {
            get
            {
                return dataAcquisizione;
            }

            set
            {
                dataAcquisizione = value;
            }
        }

        /// <summary>
        /// Estensione del file
        /// </summary>
        public string Estensione
        {
            get
            {
                return estensione;
            }

            set
            {
                estensione = value;
            }
        }

        /// <summary>
        /// Nome del file
        /// </summary>
        public string NomeFile
        {
            get
            {
                return nomeFile;
            }

            set
            {
                nomeFile = value;
            }
        }

        /// <summary>
        /// Oggetto del documento
        /// </summary>
        public string Oggetto
        {
            get
            {
                return oggetto;
            }

            set
            {
                oggetto = value;
            }
        }

        /// <summary>
        /// Descrizione sulla conformità del file
        /// </summary>
        public string DescrizioneInfoFile
        {
            get
            {
                return descrizioneInfoFile;
            }

            set
            {
                descrizioneInfoFile = value;
            }
        }

        /// <summary>
        /// Indica se il file è conforme
        /// </summary>
        public bool Conforme
        {
            get
            {
                return conforme;
            }

            set
            {
                conforme = value;
            }
        }

        /// <summary>
        /// Indica se l'estensione è conforme
        /// </summary>
        public bool EstensioneConforme
        {
            get
            {
                return estensioneConforme;
            }

            set
            {
                estensioneConforme = value;
            }
        }

        /// <summary>
        /// Indica se il file contiene macro
        /// </summary>
        public bool ContieneMacro
        {
            get
            {
                return contieneMacro;
            }

            set
            {
                contieneMacro = value;
            }
        }

        /// <summary>
        /// Indica se il file contiene forms
        /// </summary>
        public bool ContieneForms
        {
            get
            {
                return contieneForms;
            }

            set
            {
                contieneForms = value;
            }
        }

        /// <summary>
        /// Indica se il file contiene codice javascript
        /// </summary>
        public bool ContieneJavascript
        {
            get
            {
                return contieneJavascript;
            }

            set
            {
                contieneJavascript = value;
            }
        }
    }
}
