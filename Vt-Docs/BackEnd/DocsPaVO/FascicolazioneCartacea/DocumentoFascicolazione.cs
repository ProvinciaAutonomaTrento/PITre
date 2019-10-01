using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.FascicolazioneCartacea
{
    /// <summary>
    /// Classe che rappresenta un documento cartaceo
    /// da fascicolare in un fascicolo cartaceo
    /// </summary>
    [Serializable()]
    public class DocumentoFascicolazione
    {
        /// <summary>
        /// Dati del documento
        /// </summary>
        public int IdProfile = 0;
        public int DocNumber = 0;

        /// <summary>
        /// Data creazione protocollo
        /// </summary>
        public DateTime DataCreazione = DateTime.MinValue;

        /// <summary>
        /// Dati del protocollo
        /// </summary>
        public int NumeroProtocollo = 0;
        public DateTime DataProtocollo = DateTime.MinValue;

        /// <summary>
        /// Tipologia del documento (A, P, I, G)
        /// </summary>
        public string TipoDocumento = string.Empty;

        /// <summary>
        /// Dati del registro
        /// </summary>
        public int IdRegistro = 0;
        public string CodiceRegistro = string.Empty;
        
        /// <summary>
        /// Dati della versione (di solito corrisponde
        /// all'ultima versione acquisita del documento)
        /// </summary>
        public int VersionId = 0;
        public string VersionLabel = string.Empty;
        public string VersionNote = string.Empty;

        /// <summary>
        /// Dati del fascicolo in archivio
        /// </summary>
        public FascicoloArchivio Fascicolo = null;

        /// <summary>
        /// Indica se il per documento acquisito vi è
        /// un corrispondente cartaceo da archiviare
        /// </summary>
        public bool Cartaceo = false;

        /// <summary>
        /// 
        /// </summary>
        public bool InsertInFascicoloCartaceo = false;

        /// <summary>
        /// 
        /// </summary>
        public bool IsDirty = false;

        /// <summary>
        /// Id della fascicolazione (legame tra documento e fascicolo)
        /// </summary>
        public int IdFascicolazione = 0;

        /// <summary>
        /// Data di archiviazione del documento nel fascicolo cartaceo
        /// </summary>
        public DateTime DataArchiviazione = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool equals = false;

            DocumentoFascicolazione objB = obj as DocumentoFascicolazione;

            if (objB != null)
            {
                equals = (this.VersionId == objB.VersionId && 
                            this.Fascicolo != null && objB.Fascicolo != null && 
                            this.Fascicolo.IdFascicolo == objB.Fascicolo.IdFascicolo);
            }

            return equals;
        }

        //aggiunto GetHashCode perchè se no equals si arrabbia
        public override int GetHashCode()
        {
            return this.Fascicolo.IdFascicolo.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string retValue = string.Empty;

            if (this.NumeroProtocollo > 0)
            {
                retValue = string.Concat("Prot. ", this.NumeroProtocollo.ToString() + " del " + this.DataProtocollo.ToString("dd/MM/yyyy"));
            }
            else
            {
                retValue = string.Concat("Doc. ", this.DocNumber.ToString() + " del " + this.DataCreazione.ToString("dd/MM/yyyy"));
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public DocumentoFascicolazione()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        public DocumentoFascicolazione(int idProfile, int docNumber)
        {
            this.IdProfile = idProfile;
            this.DocNumber = docNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <param name="cartaceo"></param>
        public DocumentoFascicolazione(int idProfile, int docNumber, bool cartaceo) : this(idProfile, docNumber)
        {
            this.Cartaceo = cartaceo;
        }
    }
}