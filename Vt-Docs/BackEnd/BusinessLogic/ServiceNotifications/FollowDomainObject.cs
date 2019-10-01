using DocsPaDB.Query_DocsPAWS;
using System;
using DocsPaDB;

namespace BusinessLogic.ServiceNotifications
{
    public class FollowDomainObject : DBProvider
    {
        #region CONSTANT

        const string ADD_DOC_FOLDER = "addDocFolder";
        const string REMOVE_DOC_FOLDER = "removeDocFolder";
        const string ADD_FOLDER = "addFolder";
        const string REMOVE_FOLDER = "removeFolder";
        const string ADD_DOC = "addDoc";
        const string REMOVE_DOC = "removeDoc";

        #endregion

        /// <summary>
        ///It was added a document to the folder, then those who follow the file now also follows the document
        /// </summary>
        /// <returns></returns>
        public static bool FollowDocAddFolder(string idFasc, string idDoc)
        {
            return true;
            /*FollowDomainObjectDB dbFollowDomainObj = new FollowDomainObjectDB();
            return dbFollowDomainObj.FollowDomainObjectManager(ADD_DOC_FOLDER, idFasc, idDoc);*/
        }

        /// <summary>
        /// It was deleted a document from a folder, then the document is no longer followed by those who follow the folder
        /// </summary>
        /// <param name="idFasc"></param>
        /// <param name="idDoc"></param>
        /// <returns></returns>
        public static bool UnFollowDocRemoveFromFolder(string idFasc, string idDoc)
        {
            return true;
            /*FollowDomainObjectDB dbFollowDomainObj = new FollowDomainObjectDB();
            return dbFollowDomainObj.FollowDomainObjectManager(REMOVE_DOC_FOLDER, idFasc, idDoc);*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFasc"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGroup"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static bool FollowFolder(string idFasc, string idPeople, string idGroup, string idAmm, string application)
        {
            return true;
            /*FollowDomainObjectDB dbFollowDomainObj = new FollowDomainObjectDB();
            return dbFollowDomainObj.FollowDomainObjectManager(ADD_FOLDER, idFasc, idPeople, idGroup, idAmm, application);*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFasc"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGroup"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static bool UnFollowFolder(string idFasc, string idPeople, string idGroup, string idAmm, string application)
        {
            return true;
            /*FollowDomainObjectDB dbFollowDomainObj = new FollowDomainObjectDB();
            return dbFollowDomainObj.FollowDomainObjectManager(REMOVE_FOLDER, idFasc, idPeople, idGroup, idAmm, application);*/
        }

        /// <summary>
        /// You want to follow the document
        /// </summary>
        /// <param name="idDoc"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGroup"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static bool FollowDocument(string idDoc, string idPeople, string idGroup, string idAmm, string application)
        {
            return true;
            /*FollowDomainObjectDB dbFollowDomainObj = new FollowDomainObjectDB();
            return dbFollowDomainObj.FollowDomainObjectManager(ADD_DOC, idDoc, idPeople, idGroup, idAmm, application);*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDoc"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGroup"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static bool UnFollowDocument(string idDoc, string idPeople, string idGroup, string idAmm, string application)
        {
            return true;
            /*FollowDomainObjectDB dbFollowDomainObj = new FollowDomainObjectDB();
            return dbFollowDomainObj.FollowDomainObjectManager(REMOVE_DOC, idDoc, idPeople, idGroup, idAmm, application);*/
        }
    }
}
