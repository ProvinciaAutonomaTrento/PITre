using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.UploadFiles
{
    public class FileInUpload
    {
        private string strIdentity;
        private string idUtente;
        private string idRuolo;
        private string machineName;
        private string fileHash;
        private string fileName;
        private string fileDescription;
        private string fileSenderPath;
        private string repositoryPath;
        private long fileSize;
        private int totalChunkNumber;
        private int chunkNumber;
        private int order;
        private byte[] fileContent;

        /// <summary>
        /// Identificativo univoco (descrizione e nome file)
        /// </summary>
        public string StrIdentity
        {
            get
            {
                return strIdentity;
            }
            set
            {
                strIdentity = value;
            }
        }

        /// <summary>
        /// Utente che ha richiesto l'upload
        /// </summary>
        public string IdUtente
        {
            get
            {
                return idUtente;
            }
            set
            {
                idUtente = value;
            }
        }
        
        /// <summary>
        /// Ruolo con visibilità sul file
        /// </summary>
        public string IdRuolo
        {
            get
            {
                return idRuolo;
            }
            set
            {
                idRuolo = value;
            }
        }
        /// <summary>
        /// Nome macchina su cui è iniziato l'upload
        /// </summary>
        public string MachineName
        {
            get
            {
                return machineName;
            }
            set
            {
                machineName = value;
            }
        }

        /// <summary>
        /// Hash del file (completo) in dwnload
        /// </summary>
        public string FileHash
        {
            get
            {
                return fileHash;
            }
            set
            {
                fileHash = value;
            }
        }

        /// <summary>
        /// Nome del file in download
        /// </summary>
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }

        /// <summary>
        /// Descrizione aggiuntiva associata al file
        /// </summary>
        public string FileDescription
        {
            get
            {
                return fileDescription;
            }
            set
            {
                fileDescription = value;
            }
        }

        /// <summary>
        /// Percorso del file sulla macchina di destinazione
        /// </summary>
        public string RepositoryPath
        {
            get
            {
                return repositoryPath;
            }
            set
            {
                repositoryPath = value;
            }
        }
        
        /// <summary>
        /// Percorso del file sulla macchina di provenienza
        /// </summary>
        public string FileSenderPath
        {
            get
            {
                return fileSenderPath;
            }
            set
            {
                fileSenderPath = value;
            }
        }

        /// <summary>
        /// Dimensioni del file completo
        /// </summary>
        public long FileSize
        {
            get
            {
                return fileSize;
            }
            set
            {
                fileSize = value;
            }
        }

        /// <summary>
        /// Numero totale di parti
        /// </summary>
        public int TotalChunkNumber
        {
            get
            {
                return totalChunkNumber;
            }
            set
            {
                totalChunkNumber = value;
            }
        }

        /// <summary>
        /// Numero di parte file in download
        /// </summary>
        public int ChunkNumber
        {
            get
            {
                return chunkNumber;
            }
            set
            {
                chunkNumber = value;
            }
        }

        /// <summary>
        /// Ordine nella lista dei download
        /// </summary>
        public int Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
            }

        }

        /// <summary>
        /// Content della parte in download
        /// </summary>
        public byte[] FileContent
        {
            get
            {
                return fileContent;
            }
            set
            {
                fileContent = value;
            }
        }
    }
}
