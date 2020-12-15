using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DocsPaVO.FascicolazioneCartacea
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SnapshotDocumentiFascicolazione
    {
        /// <summary>
        /// ID immagine
        /// </summary>
        public int IdSnapshot = 0;

        /// <summary>
        /// Nome dell'immagine
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// Data di creazione immagine
        /// </summary>
        public DateTime CreationDate = DateTime.MinValue;

        /// <summary>
        /// Utente che ha creato l'immagine
        /// </summary>
        public string UserId = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public SnapshotDocumentiFascicolazione()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public SnapshotDocumentiFascicolazione(DataRow row)
        {
            if (row["IdSnapshot"] != DBNull.Value)
                Int32.TryParse(row["IdSnapshot"].ToString(), out this.IdSnapshot);

            if (row["CreationDate"] != DBNull.Value)
                DateTime.TryParse(row["CreationDate"].ToString(), out this.CreationDate);

            if (row["UserId"] != DBNull.Value)
                this.UserId = row["UserId"].ToString();

            this.Name = string.Format("{0} - Ricerca del {1}", this.UserId, this.CreationDate.ToString());
        }
    }
}
