using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DocsPaDB;
using DocsPaVO.areaConservazione;
using DocsPaVO.utente;
using log4net;

namespace DocsPaConservazione
{
    public class SaveFolder
    {
        private ILog logger = LogManager.GetLogger(typeof(SaveFolder));
        public FolderConservazione[] folderTree;

        /// <summary>
        /// Valorizzo l'oggetto FolderConservazione[] che contiene tutte le informazioni dell'albero delle
        /// directory del fascicolo e dei relativi documenti in esso contenuti.
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="codFascicolo"></param>
        public SaveFolder(string idFascicolo, string codFascicolo)
        {
            folderTree = getFoldersConservazione(idFascicolo);
            setFolderPath(ref folderTree, idFascicolo, codFascicolo, true);
        }

        /// <summary>
        /// Restituisce il percorso relativo al fascicolo di uno specifico documento in esso contenuto
        /// </summary>
        /// <param name="id_profile"></param>
        /// <returns></returns>
        public string getFolderDocument(string id_profile)
        {
            string path = string.Empty;
            for (int i = 0; i < folderTree.Length; i++)
            {
                if (folderTree[i].ID_Profile.Contains(id_profile))
                {
                    //rimuovo il documento nel caso sia presente in altre sottocartelle per non farlo scrivere
                    //sempre nella prima che trova!!!
                    folderTree[i].ID_Profile.Remove(id_profile);
                    path = folderTree[i].relativePath;
                    return path;
                }
            }
            return path;
        }

        /// <summary>
        /// Inserisce il percorso relativo di ciascun sotto fascicolo dentro l'oggetto FolderConservazione
        /// </summary>
        /// <param name="folderList"></param>
        /// <param name="idFascicolo"></param>
        /// <param name="pathParent"></param>
        protected void setFolderPath(ref FolderConservazione[] folderList, string idFascicolo, string pathParent, bool isRoot)
        {
            for (int i = 0; i < folderList.Length; i++)
            {
                if (folderList[i].parent == idFascicolo)
                {
                    if (isRoot)
                    {
                        pathParent = replaceInvalidChar(pathParent);
                        folderList[i].relativePath = pathParent;
                    }
                    else
                    {
                        string descrizione = replaceInvalidChar(folderList[i].descrizione  +" ("+folderList[i].systemID +")");
                        folderList[i].relativePath = pathParent + '\u005C'.ToString() + descrizione;
                    }
                    this.setFolderPath(ref folderList, folderList[i].systemID, folderList[i].relativePath, false);
                }
            }
        }

        private string replaceInvalidChar(string path)
        {
            string resultPath = path;
            char[] invalid = System.IO.Path.GetInvalidPathChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                resultPath = resultPath.Replace(invalid[i], '\u005F');
            }
            return resultPath;
        }

        /// <summary>
        /// Restituisce la lista di sotto fascicoli contenuti in un determinato fascicolo
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        protected FolderConservazione[] getFoldersConservazione(string idFascicolo)
        {
            System.Data.DataSet ds = new DataSet();
            System.Collections.ArrayList lista = new System.Collections.ArrayList();
            try
            {
                string queryFolderString = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project14");

                q.setParam("param1", idFascicolo);

                queryFolderString = q.getSQL();
                DBProvider dbProvider = new DBProvider();
                dbProvider.ExecuteQuery(ds, "FOLDER", queryFolderString);

                for (int j = 0; j < ds.Tables["FOLDER"].Rows.Count; j++)
                {
                    FolderConservazione folder = new FolderConservazione();
                    folder.systemID = ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString();
                    folder.parent = ds.Tables["FOLDER"].Rows[j]["ID_PARENT"].ToString();
                    folder.descrizione = ds.Tables["FOLDER"].Rows[j]["DESCRIPTION"].ToString();

                    string queryDocString = "";
                    string DocString = "";
                    DocsPaUtils.Query qSelect = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents3");

                    queryDocString = ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString();
                    qSelect.setParam("param1", queryDocString);
                    DocString = qSelect.getSQL();

                    DataSet dsDoc = new DataSet();
                    dbProvider.ExecuteQuery(dsDoc, "DOC", DocString);
                    ArrayList ID_Profile_List = new ArrayList();
                    for (int k = 0; k < dsDoc.Tables["DOC"].Rows.Count; k++)
                    {
                        ID_Profile_List.Add(dsDoc.Tables["DOC"].Rows[k]["LINK"].ToString());
                    }
                    folder.ID_Profile = ID_Profile_List;
                    
                    lista.Add(folder);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                lista = null;
            }
            return (FolderConservazione[])lista.ToArray(typeof(FolderConservazione));
        }
    }
}
