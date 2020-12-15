using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace DocsPaDocumentale_HERMES.Migrazione
{
    /// <summary>
    /// Classe per la gestione dello stato di migrazione dati per un'amministrazione
    /// </summary>
    public sealed class StatoMigrazione
    {
        /// <summary>
        /// 
        /// </summary>
        private StatoMigrazione()
        { }

        /// <summary>
        /// Reperimento metadati di migrazione
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <returns></returns>
        public static InfoStatoMigrazione Get(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            InfoStatoMigrazione retValue = null;

            string filePath = AppDataFolder.GetStatoMigrazioneFilePath(amministrazione);

            if (File.Exists(filePath))
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(InfoStatoMigrazione));

                    retValue = (InfoStatoMigrazione)serializer.Deserialize(stream);
                }
            }

            if (retValue == null)
                retValue = new InfoStatoMigrazione(amministrazione.IDAmm, amministrazione.Codice);

            return retValue;
        }

        /// <summary>
        /// Salva metadati di migrazione
        /// </summary>
        /// <param name="stato"></param>
        /// <param name="amministrazione"></param>
        public static void Save(InfoStatoMigrazione stato, DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            string filePath = AppDataFolder.GetStatoMigrazioneFilePath(amministrazione);

            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(InfoStatoMigrazione));

                serializer.Serialize(stream, stato);
            }
        }

        /// <summary>
        /// Rimozione metadati di migrazione
        /// </summary>
        /// <param name="amministrazione"></param>
        public static void Delete(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            AppDataFolder.ClearFolder(amministrazione);
        }
    }

    /// <summary>
    /// Metadati relativi allo stato della migrazione di un'amministrazione
    /// </summary>
    [Serializable()]
    public class InfoStatoMigrazione
    {
        /// <summary>
        /// 
        /// </summary>
        public InfoStatoMigrazione()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="amministrazione"></param>
        public InfoStatoMigrazione(string idAmministrazione, string amministrazione)
        {
            this.IdAmministrazione = idAmministrazione;
            this.Amministrazione = amministrazione;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("IdAmministrazione")]
        public string IdAmministrazione;
        
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("Amministrazione")]
        public string Amministrazione;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("DatiAmministrazioneMigrati")]
        public bool DatiAmministrazioneMigrati;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fascicolo"></param>
        public void SetFascicoloMigrazione(InfoFascicoloMigrazione fascicolo)
        {
            fascicolo.DataMigrazione = DateTime.Now.ToString();

            int indexOf = this.FascicoliMigrazione.IndexOf(fascicolo);
            
            if (indexOf == -1)
                this.FascicoliMigrazione.Insert(0, fascicolo);
            else
                this.FascicoliMigrazione[indexOf] = fascicolo;
        }

        /// <summary>
        /// Lista dei fascicoli considerati nella migrazione
        /// </summary>
        public List<InfoFascicoloMigrazione> FascicoliMigrazione = new List<InfoFascicoloMigrazione>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documento"></param>
        public void SetDocumentoMigrazione(InfoDocumentoMigrazione documento)
        {
            documento.DataMigrazione = DateTime.Now.ToString();

            int indexOf = this.DocumentiMigrazione.IndexOf(documento);

            if (indexOf == -1)
                this.DocumentiMigrazione.Insert(0, documento);
            else
                this.DocumentiMigrazione[indexOf] = documento;
        }

        /// <summary>
        /// Lista dei documenti considerati nella migrazione
        /// </summary>
        public List<InfoDocumentoMigrazione> DocumentiMigrazione = new List<InfoDocumentoMigrazione>();
    }

    /// <summary>
    /// Metadati relaviti ad un fascicolo oggetto di migrazione
    /// </summary>
    [Serializable()]
    public class InfoFascicoloMigrazione
    {
        /// <summary>
        /// 
        /// </summary>
        public InfoFascicoloMigrazione()
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fascicolo"></param>
        public InfoFascicoloMigrazione(DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            this.Id = fascicolo.systemID;
            this.Codice = fascicolo.codice;
            this.Descrizione = fascicolo.descrizione;
            this.Tipo = fascicolo.tipo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="folder"></param>
        public InfoFascicoloMigrazione(DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.fascicolazione.Folder folder)
        {
            this.Id = folder.systemID;
            this.Codice = string.Format("{0}.{1}", fascicolo.codice, folder.descrizione);
            this.Descrizione = folder.descrizione;
            this.Tipo = "S";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is InfoFascicoloMigrazione)
                return ((InfoFascicoloMigrazione)obj).Id.Equals(this.Id);
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        [XmlAttribute("Id")]
        public string Id;

        [XmlAttribute("Codice")]
        public string Codice;

        [XmlAttribute("Descrizione")]
        public string Descrizione;

        [XmlAttribute("Tipo")]
        public string Tipo;

        [XmlAttribute("EsitoMigrazione")]
        public bool EsitoMigrazione;

        [XmlAttribute("DataMigrazione")]
        public string DataMigrazione;

        [XmlAttribute("ErroreMigrazione")]
        public string ErroreMigrazione;

        [XmlAttribute("HashFascicolo")]
        public string HashFascicolo;
    }

    /// <summary>
    /// Metadati relaviti ad un documento oggetto di migrazione
    /// </summary>
    [Serializable()]
    public class InfoDocumentoMigrazione
    {
        /// <summary>
        /// 
        /// </summary>
        public InfoDocumentoMigrazione()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is InfoDocumentoMigrazione)
                return ((InfoDocumentoMigrazione)obj).DocNumber.Equals(this.DocNumber);
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.DocNumber.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        public InfoDocumentoMigrazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {   
            this.Tipo = schedaDocumento.tipoProto;
            
            this.DocNumber = schedaDocumento.docNumber;
            
            this.DataCreazione = schedaDocumento.dataCreazione;

            if (schedaDocumento.protocollo != null)
                this.Segnatura = schedaDocumento.protocollo.segnatura;

            if (schedaDocumento.oggetto != null)
                this.Oggetto = schedaDocumento.oggetto.descrizione;

            if (schedaDocumento.creatoreDocumento != null)
            {
                this.IdUtenteCreatore = schedaDocumento.creatoreDocumento.idPeople;
                this.IdRuoloCreatore = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
            }
        }

        [XmlAttribute("Tipo")]
        public string Tipo;
        [XmlAttribute("DocNumber")]
        public string DocNumber;
        [XmlAttribute("DataCreazione")]
        public string DataCreazione;
        [XmlAttribute("Segnatura")]
        public string Segnatura;
        [XmlAttribute("Oggetto")]
        public string Oggetto;
        [XmlAttribute("IdUtenteCreatore")]
        public string IdUtenteCreatore;
        [XmlAttribute("IdRuoloCreatore")]
        public string IdRuoloCreatore;
        [XmlAttribute("EsitoMigrazione")]
        public bool EsitoMigrazione;
        [XmlAttribute("DataMigrazione")]
        public string DataMigrazione;
        [XmlAttribute("ErroreMigrazione")]
        public string ErroreMigrazione;
        [XmlAttribute("HashDocumento")]
        public string HashDocumento;
    }
}