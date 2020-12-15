using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.amministrazione;

namespace DocsPaDocumentale_OCS.DocsPaServices
{
    public sealed class DocsPaQueryHelper
    {
        /// <summary>
        /// Reperimento scheda documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento getSchedaDocumento(string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti();

            return dbDocumenti.GetDettaglio(infoUtente, DocsPaQueryHelper.getIdProfile(docNumber), docNumber, false);
        }

        /// <summary>
        /// Reperimento dell'idprofile del documento a partire dal docNumber
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static string getIdProfile(string docNumber)
        {
            string idProfile = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT system_id FROM profile WHERE docnumber = {0}", docNumber);

                dbProvider.ExecuteScalar(out idProfile, commandText);
            }

            return idProfile;
        }

        /// <summary>
        /// Reperimento oggetto ruolo a partire dall'id della tabella GROUPS
        /// </summary>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo getRuoloFromIdGroup(string idGroup)
        {
            string idCorrGlobali = string.Empty;

            // Reperimento dell'id della tabella corrglobali a partire dall'id della tabella groups
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("select system_id from dpa_corr_globali where var_cod_rubrica = (select group_id from groups where system_id = {0})", idGroup);
                dbProvider.ExecuteScalar(out idCorrGlobali, commandText);
            }

            return getRuolo(idCorrGlobali);
        }

        /// <summary>
        /// Reperimento utente dall'id
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente getUtente(string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utentiDb.getUtenteById(idPeople);
        }

        /// Reperimento ruolo dall'id corrglobali
        /// </summary>
        /// <param name="idCorrGlobali"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo getRuolo(string idCorrGlobali)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();

            // Caricamento dei dati del ruolo, escludendo le funzioni
            return utentiDb.GetRuolo(idCorrGlobali, false);
        }

        /// Reperimento del codice del ruolo a partire dall'id della tabella GROUPS
        /// </summary>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public static string getCodiceRuoloFromIdGroup(string idGroup)
        {
            string codiceRuolo = string.Empty;

            // Reperimento dell'id della tabella corrglobali a partire dall'id della tabella groups
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT group_id FROM groups WHERE system_id = {0}", idGroup);

                dbProvider.ExecuteScalar(out codiceRuolo, commandText);
            }

            return codiceRuolo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static string getCodiceRegistroFromId(string idRegistro)
        {
            string codiceRegistro = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT var_codice FROM dpa_el_registri WHERE system_id = {0}", idRegistro);
                dbProvider.ExecuteScalar(out codiceRegistro, commandText);
            }

            return codiceRegistro;
        }

        /// <summary>
        /// Reperimento codice utente a partire dall'id della tabella PEOPLE
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static string getCodiceUtente(string idPeople)
        {
            string codiceUtente = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT user_id FROM people WHERE system_id = {0}", idPeople);

                dbProvider.ExecuteScalar(out codiceUtente, commandText);
            }

            return codiceUtente;
        }

        /// <summary>
        /// Reperimento path del documento
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static string getDocumentPath(string docNumber)
        {
            string pathDoc = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT path FROM profile WHERE docNumber = {0}", docNumber);

                dbProvider.ExecuteScalar(out pathDoc, commandText);
            }

            return pathDoc;
        }

        /// <summary>
        /// Reperimento name del documento
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static string getDocumentName(string docNumber, string versionId)
        {
            string nameDoc = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT path FROM components WHERE docNumber = {0} " +  
                " and version_id in ( " + 
                " SELECT MAX(version_id) " + 
                " FROM versions " +
                " WHERE docNumber = {0} ", docNumber);
                if (versionId != null && !(versionId.Equals("")))
                {
                    commandText = commandText + " and version_id != " + versionId;
                }
                commandText = commandText + ")";
                dbProvider.ExecuteScalar(out nameDoc, commandText);
                if (nameDoc == null || nameDoc.Equals(""))
                    nameDoc = docNumber;
                else
                {
                    //prendo solo la parte del nome dall'intero path
                    string fileName = "";
                    int pos = nameDoc.LastIndexOf("/");
                    if (pos > 0)
                    {
                        fileName = nameDoc.Substring(pos + 1);
                        nameDoc = fileName;
                    }
                }
            }

            return nameDoc;
        }

        /// <summary>
        /// Reperimento del codice del ruolo a partire dall'id della tabella DPA_CORR_GLOBALI
        /// </summary>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public static string getCodiceRuoloFromIdCorrGlobali(string idCorrGlobali)
        {
            string codiceRuolo = string.Empty;

            if (idCorrGlobali == null || idCorrGlobali.Equals(""))
                return codiceRuolo;
            // Reperimento dell'id della tabella corrglobali a partire dall'id della tabella groups
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT var_cod_rubrica FROM dpa_corr_globali WHERE system_id = {0}", idCorrGlobali);

                dbProvider.ExecuteScalar(out codiceRuolo, commandText);
            }

            return codiceRuolo;
        }

        /// <summary>
        /// A partire dal docNumber di un allegato, viene reperito
        /// il docNumber del relativo documento principale
        /// </summary>
        /// <param name="docNumberAllegato"></param>
        /// <returns></returns>
        public static string getDocNumberDocumentoPrincipale(string docNumberAllegato)
        {
            string docNumber = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string innerCommandText =
                    string.Format("SELECT id_documento_principale FROM profile WHERE docnumber = {0}", docNumberAllegato);

                string commandText =
                    string.Format("SELECT docNumber FROM profile WHERE system_id = ({0})", innerCommandText);

                dbProvider.ExecuteScalar(out docNumber, commandText);
            }

            return docNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static int getVersionNumberById(string docNumber, string versionId)
        {
            // Reperimento del numero di versione 
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per reperire le versioni
                string commandText = string.Format("SELECT version_id FROM versions WHERE docNumber = {0} order by version_id DESC", docNumber);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    bool trovato = false;
                    int numVersione = 0;
                    while (reader.Read() || trovato)
                    {
                        numVersione++;
                        if (reader.GetString(reader.GetOrdinal("version_id")).Equals(versionId))
                        {
                            trovato = true;
                        }
                        return numVersione;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Reperimento codice uo creatore a partire dall'id della tabella PROFILE
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static string getCodiceUO(string docNumber)
        {
            string codiceUtente = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT a.var_cod_rubrica FROM dpa_corr_globali a, profile b WHERE b.docNumber = {0} AND a.system_id = b.id_uo_creatore", docNumber);

                dbProvider.ExecuteScalar(out codiceUtente, commandText);
            }

            return codiceUtente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="docNumber"></param>
        public static void updateLocation(string path, string docNumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
            documenti.O_UpdatePathProfile(path, docNumber);
        }

        /// <summary>
        /// Reperimento del versionId relativo all'ultima versione di un documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static string getDocumentLatestVersionId(string docNumber)
        {
            string versionId = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText =string.Format("SELECT MAX(version_id) FROM versions WHERE docnumber = {0}", docNumber);

                dbProvider.ExecuteScalar(out versionId, commandText);
            }

            return versionId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static DocsPaSecurityItem[] getSecurityItems(string thing)
        {
            List<DocsPaSecurityItem> securityItems = new List<DocsPaSecurityItem>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT cg.id_people AS id_people, cg.id_gruppo AS id_group, cg.var_cod_rubrica AS codice_rubrica, s.accessrights AS accessrights, s.cha_tipo_diritto AS tipodiritto FROM security s, dpa_corr_globali cg WHERE (s.personorgroup = cg.id_gruppo or s.personorgroup = cg.id_people) AND s.thing = {0}", thing);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        securityItems.Add(DocsPaSecurityItem.NewItem(reader));
                }
            }

            return securityItems.ToArray();
        }

        /// <summary>
        /// Reperimento dei gruppi docspa per l'amministrazione richiesta
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static OrgRuolo[] getAdminGroups(string idAmministrazione)
        {
            List<OrgRuolo> roles = new List<OrgRuolo>();

            DocsPaDB.Query_DocsPAWS.Amministrazione ammDb = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            foreach (string item in ammDb.GetRuoli(idAmministrazione))
            {
                OrgRuolo role = new OrgRuolo();
                role.Codice = item;
                roles.Add(role);
            }

            return roles.ToArray();
        }

         /// <summary>
        /// Oggetto contenente i metadati di security relativi ad un oggetto DocsPa
        /// </summary>
        public class DocsPaSecurityItem
        {
            /// <summary>
            /// 
            /// </summary>
            public DocsPaSecurityItem()
            { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            public static DocsPaSecurityItem NewItem(System.Data.IDataReader reader)
            {
                DocsPaSecurityItem item = new DocsPaSecurityItem();
                item.IdPeople = reader.GetValue(reader.GetOrdinal("id_people")).ToString();
                item.IdGroup = reader.GetValue(reader.GetOrdinal("id_group")).ToString();
                item.CodiceRubrica = reader.GetValue(reader.GetOrdinal("codice_rubrica")).ToString();
                item.AccessRights = reader.GetValue(reader.GetOrdinal("accessrights")).ToString();
                item.TipoDiritto = reader.GetValue(reader.GetOrdinal("tipodiritto")).ToString();
                return item;
            }

            
            /// <summary>
            /// Se valorizzato, indica l'utente che ha il diritto sull'oggetto in security
            /// </summary>
            public string IdPeople;

            /// <summary>
            /// Se valorizzato, indica il ruolo dell'utente che ha il diritto sull'oggetto in security
            /// </summary>
            public string IdGroup;

            /// <summary>
            /// Codice rubrica dell'utente o ruolo
            /// </summary>
            public string CodiceRubrica;

            /// <summary>
            /// Indica la permission sull'oggetto
            /// </summary>
            public string AccessRights;

            /// <summary>
            /// Indica il tipo di ownersship sull'oggetto 
            /// </summary>
            public string TipoDiritto;

            /// <summary>
            /// Indica se l'istanza si riferisce ad un diritto a persona o ruolo
            /// </summary>
            public bool IsPeople
            {
                get
                {
                    return (string.IsNullOrEmpty(this.IdGroup) &&
                            !string.IsNullOrEmpty(this.IdPeople));
                }
            }

        }
    }
}
