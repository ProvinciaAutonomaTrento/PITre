using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using DocsPaVO.FascicolazioneCartacea;
using DocsPaVO.utente;
using DocsPaVO.filtri;
using log4net;

namespace BusinessLogic.FascicolazioneCartacea
{
    /// <summary>
    /// Classe che fornisce servizi per la persistenza
    /// e ripristino dei documenti fascicolati
    /// </summary>
    public sealed class FascicolazioneCartaceaSnapshotServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(FascicolazioneCartaceaSnapshotServices));
        private FascicolazioneCartaceaSnapshotServices()
        { }

        #region Public methods

        /// <summary>
        /// Reperimento lista delle snapshot per le ricerche persistite
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static SnapshotDocumentiFascicolazione[] GetSnapshotList(InfoUtente infoUtente)
        {
            List<SnapshotDocumentiFascicolazione> retValue = new List<SnapshotDocumentiFascicolazione>();

            DataSet ds = GetDatasetMaster(infoUtente);

            DataView dv = ds.Tables["SnapshotTable"].DefaultView;
            dv.Sort = "IdSnapshot DESC";
            foreach (DataRowView rv in dv)
                retValue.Add(new SnapshotDocumentiFascicolazione(rv.Row));

            return retValue.ToArray();
        }

        /// <summary>
        /// Repreimento filtri per la snapshot richiesta
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSnapshot"></param>
        /// <returns></returns>
        public static FiltroRicerca[] GetSnapshotFilters(InfoUtente infoUtente, int idSnapshot)
        {
            string filePath = GetSnapshotFiltersFilePath(infoUtente, idSnapshot);

            FiltroRicerca[] retValue = null;
            
            if (File.Exists(filePath))
                retValue = (FiltroRicerca[]) Deserialize(filePath);

            if (retValue == null)
                retValue = new FiltroRicerca[0];

            return retValue;
        }

        /// <summary>
        /// Salvataggio immagine dei dati sui documenti da fascicolare
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtri"></param>
        /// <returns></returns>
        public static SnapshotDocumentiFascicolazione CreateSnapshot(InfoUtente infoUtente, FiltroRicerca[] filtri)
        {
            DocumentoFascicolazione[] documenti = FascicolazioneCartaceaServices.GetDocumentiFascicolazione(infoUtente, filtri);

            return CreateSnapshot(infoUtente, documenti, filtri);
        }

        /// <summary>
        /// Salvataggio immagine dei dati sui documenti da fascicolare
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="documenti"></param>
        /// <param name="filtri"></param>
        public static SnapshotDocumentiFascicolazione CreateSnapshot(InfoUtente infoUtente, DocumentoFascicolazione[] documenti, FiltroRicerca[] filtri)
        {
            SnapshotDocumentiFascicolazione retValue = null;

            int idSnapshot = 1;
            DataSet ds = GetDatasetMaster(infoUtente);
            DataRow newRow = ds.Tables["SnapshotTable"].NewRow();
            newRow["CreationDate"] = DateTime.Now;
            newRow["UserId"] = infoUtente.userId;
            ds.Tables["SnapshotTable"].Rows.Add(newRow);
            idSnapshot = Convert.ToInt32(newRow["IdSnapshot"]);
            ds.WriteXml(GetSnapshotMasterFile(infoUtente), XmlWriteMode.WriteSchema);

            string filePath = GetSnapshotFilePath(infoUtente, idSnapshot);
            Serialize(filePath, documenti);

            filePath = GetSnapshotFiltersFilePath(infoUtente, idSnapshot);
            Serialize(filePath, filtri);

            return new SnapshotDocumentiFascicolazione(newRow);
        }

        /// <summary>
        /// Aggiornamento documenti nella snapshot
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSnapshot"></param>
        /// <param name="documenti"></param>
        public static void UpdateSnapshot(InfoUtente infoUtente, int idSnapshot, DocumentoFascicolazione[] documenti)
        {
            string filePath = GetSnapshotFilePath(infoUtente, idSnapshot);

            Serialize(filePath, documenti);
        }

        /// <summary>
        /// Ripristino smmagine dei dati sui documenti da fascicolare
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSnapshot"></param>
        /// <returns></returns>
        public static DocumentoFascicolazione[] RestoreSnapshot(InfoUtente infoUtente, int idSnapshot)
        {
            DocumentoFascicolazione[] retValue = null;

            string filePath = GetSnapshotFilePath(infoUtente, idSnapshot);

            if (File.Exists(filePath))
                retValue = (DocumentoFascicolazione[]) Deserialize(filePath);

            if (retValue == null)
                retValue = new DocumentoFascicolazione[0];

            return retValue;
        }

        /// <summary>
        /// Rimozione snapshot
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSnapshot"></param>
        public static void RemoveSnapshot(InfoUtente infoUtente, int idSnapshot)
        {
            // Rimozione cartella in cui sono persistiti i file snapshot
            string snapshotFolder = GetSnapshotFileFolder(infoUtente, idSnapshot);
            if (Directory.Exists(snapshotFolder))
                Directory.Delete(snapshotFolder, true);

            // Rimozione dati dal dataset master
            DataSet dsMaster = GetDatasetMaster(infoUtente);

            DataView dv = dsMaster.Tables[0].DefaultView;
            dv.RowFilter = "IdSnapshot = " + idSnapshot.ToString();
            if (dv.Count == 1)
            {
                dsMaster.Tables[0].Rows.Remove(dv[0].Row);
                dsMaster.WriteXml(GetSnapshotMasterFile(infoUtente), XmlWriteMode.WriteSchema);
            }
        }

        /// <summary>
        /// Rimozione di tutte le snapshot persistite
        /// </summary>
        /// <param name="infoUtente"></param>
        public static void ClearSnapshots(InfoUtente infoUtente)
        {
            string rootFolder = GetSnapshotRootFolder(infoUtente);
            if (Directory.Exists(rootFolder))
                Directory.Delete(rootFolder, true);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Serializzazione oggetto
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        private static void Serialize(string filePath, object graph)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, graph);
            }
        }

        /// <summary>
        /// Deserializzazione oggetto
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static object Deserialize(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Reperimento del dataset contenente i metadati sulle snapshot persistite
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static DataSet GetDatasetMaster(InfoUtente infoUtente)
        {
            DataSet ds = new DataSet("Snapshot");

            string masterFile = GetSnapshotMasterFile(infoUtente);

            if (File.Exists(masterFile))
            {
                ds.ReadXml(masterFile);
            }
            else
            {
                DataTable dt = new DataTable("SnapshotTable");

                DataColumn column = new DataColumn("IdSnapshot", typeof(int));
                column.AutoIncrement = true;
                column.AutoIncrementSeed = 1;
                column.AutoIncrementStep = 1;
                column.Unique = true;
                dt.Columns.Add(column);

                column = new DataColumn("CreationDate", typeof(DateTime));
                dt.Columns.Add(column);

                column = new DataColumn("UserId", typeof(string));
                dt.Columns.Add(column);

                ds.Tables.Add(dt);
            }

            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static string GetSnapshotRootFolder(InfoUtente infoUtente)
        {
            return string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\FascicolazioneCartacea\");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static string GetSnapshotMasterFile(InfoUtente infoUtente)
        {
            string folder = GetSnapshotRootFolder(infoUtente);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return string.Concat(folder, "snapshot.config");
        }

        /// <summary>
        /// Reperimento percorso snapshot
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSnapshot"></param>
        /// <returns></returns>
        private static string GetSnapshotFilePath(InfoUtente infoUtente, int idSnapshot)
        {
            // Reperimento percorso cartella file snapshot
            string folder = GetSnapshotFileFolder(infoUtente, idSnapshot);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return string.Concat(folder, "documents");
        }

        /// <summary>
        /// Reperimento percorso cartella in cui sono persistiti i file snapshot
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSnapshot"></param>
        /// <returns></returns>
        private static string GetSnapshotFileFolder(InfoUtente infoUtente, int idSnapshot)
        {
            // Reperimento percorso cartella file snapshot
            return string.Concat(GetSnapshotRootFolder(infoUtente), idSnapshot.ToString(), @"\");
        }

        /// <summary>
        /// Reperimento percorso filtri snapshot
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idSnapshot"></param>
        /// <returns></returns>
        private static string GetSnapshotFiltersFilePath(InfoUtente infoUtente, int idSnapshot)
        {
            // Reperimento percorso cartella file snapshot
            string folder = string.Concat(GetSnapshotRootFolder(infoUtente), idSnapshot.ToString(), @"\");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return string.Concat(folder, "filters");
        }

        #endregion
    }
}
