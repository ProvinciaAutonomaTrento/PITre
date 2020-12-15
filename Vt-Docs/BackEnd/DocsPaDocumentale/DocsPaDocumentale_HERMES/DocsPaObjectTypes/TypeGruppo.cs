using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale_DOCUMENTUM.DocsPaServices;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeGruppo
    {
        protected TypeGruppo()
        { }

        public const string PREFISSO_GRUPPO_AMMINISTRAZIONE = "sys_";
        public const string PREFISSO_GRUPPO_SYSADMIN_AMMINISTRAZIONE = "sysadm_";

        public static string GetGroupNameForAmministrazione(string codiceAmministrazione)
        {
            return PREFISSO_GRUPPO_AMMINISTRAZIONE + codiceAmministrazione.ToLowerInvariant().Replace("'", "_");
        }

        public static string GetGroupNameForSysAdminAmministrazione(string codiceAmministrazione)
        {
            return PREFISSO_GRUPPO_SYSADMIN_AMMINISTRAZIONE + codiceAmministrazione.ToLowerInvariant().Replace("'", "_");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public static string GetGroupName(DocsPaVO.utente.Ruolo ruolo)
        {
            return NormalizeGroupName(DocsPaQueryHelper.getCodiceRuoloFromIdGroups(ruolo.idGruppo));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public static string GetGroupName(DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            return NormalizeGroupName(DocsPaQueryHelper.getCodiceRuoloFromIdGroups(ruolo.IDGruppo));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static string NormalizeGroupName(string groupName)
        {
            return groupName.ToLowerInvariant().Replace("'", "''");
        }

    }
}
