using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeUtente
    {
        /// <summary>
        /// 
        /// </summary>
        protected TypeUtente()
        { }

        /// <summary>
        /// 
        /// </summary>
        public const string HOME_FOLDER_PREFIX = "Utente_";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getHomeFolderName(string userId)
        {
            return HOME_FOLDER_PREFIX + NormalizeUserName(userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static string getUserName(DocsPaVO.amministrazione.OrgUtente utente)
        {
            return NormalizeUserName(utente.UserId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static string getUserName(DocsPaVO.utente.Utente utente)
        {
            return NormalizeUserName(utente.userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static string getUserName(DocsPaVO.utente.InfoUtente utente)
        {
            return NormalizeUserName(utente.userId);
        }

        /// <summary>
        /// Normalizzazione username per documentum
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string NormalizeUserName(string userName)
        {
            return userName.ToLowerInvariant().Replace("'", "''");
        }
    }
}
