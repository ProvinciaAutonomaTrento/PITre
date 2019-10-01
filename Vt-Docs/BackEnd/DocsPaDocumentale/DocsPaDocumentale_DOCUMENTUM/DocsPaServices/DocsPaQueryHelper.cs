using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_DOCUMENTUM.DocsPaServices
{
    /// <summary>
    /// Classe di utilità per il reperimento dei dati di DocsPa
    /// necessari per documentum
    /// </summary>
    public sealed class DocsPaQueryHelper
    {
        /// <summary>
        /// Reperimento del docNumber del documento a partire dall'idProfile
        /// NB: In DCTM ci si riferisce al DocNumber e non all'IdProfile
        /// </summary>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        public static string getDocNumber(string idProfile)
        {
            string docNumber = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT docNumber FROM profile WHERE system_id = {0}", idProfile);

                dbProvider.ExecuteScalar(out docNumber, commandText);
            }

            return docNumber;
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
        /// A partire dal docNumber di un allegato, viene reperito
        /// il docNumber del relativo documenti principale
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
        /// Reperimento del numero di sottofascicoli presenti nel fascicolo richiesto
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        public static int getCountSottofascicoli(string idFascicolo)
        {
            int count = 0;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT COUNT(system_id) FROM project WHERE id_fascicolo = {0} AND cha_tipo_proj = 'C' AND id_fascicolo != id_parent", idFascicolo);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    Int32.TryParse(field, out count);
            }

            return count;
        }

        /// <summary>
        /// Reperimento del numero di allegati presenti nel documento
        /// </summary>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        public static int getCountAllegati(string idProfile)
        {
            int count = 0;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT COUNT(system_id) FROM profile WHERE id_documento_principale = {0}", idProfile);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    Int32.TryParse(field, out count);
            }

            return count;
        }

        /// <summary>
        /// Reperimento codice amministrazione dall'id
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static string getCodiceAmministrazione(string idAmministrazione)
        {
            DocsPaDB.Query_Utils.Utils utils = new DocsPaDB.Query_Utils.Utils();
            
            return utils.getCodAmm(idAmministrazione).ToLowerInvariant();
        }

        /// <summary>
        /// Reperimento id amministrazione dal codice
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static string getIdAmministrazione(string codiceAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            return dbAmm.GetIDAmm(codiceAmministrazione);
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
        /// Reperimento utente dall'id
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente getUtente(string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();
            
            return utentiDb.getUtenteById(idPeople);
        }

        /// <summary>
        /// Reperimento nomi gruppi docspa per l'amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static string[] getGroupNames(string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione ammDb = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            return ammDb.GetRuoli(idAmministrazione);
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
        /// Reperimento del codice del ruolo a partire dall'id della tabella GROUPS
        /// </summary>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public static string getCodiceRuoloFromIdGroups(string idGroup)
        {
            string codiceRuolo = string.Empty;

            // Reperimento dell'id della tabella corrglobali a partire dall'id della tabella groups
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT var_original_code FROM dpa_corr_globali cg INNER JOIN groups g ON g.system_id = cg.id_gruppo WHERE g.system_id = {0}", idGroup);

                dbProvider.ExecuteScalar(out codiceRuolo, commandText);
            }

            return codiceRuolo;
        }

        /// <summary>
        /// Reperimento del codice del ruolo a partire dall'id della tabella GROUPS
        /// </summary>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public static string getCodiceRuoloFromIdCorr(string idCorr)
        {
            string codiceRuolo = string.Empty;

            // Reperimento dell'id della tabella corrglobali a partire dall'id della tabella groups
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT var_original_code FROM dpa_corr_globali cg WHERE cg.system_id = {0}", idCorr);

                dbProvider.ExecuteScalar(out codiceRuolo, commandText);
            }

            return codiceRuolo;
        }

        ///// <summary>
        ///// Metodo per il reperimento di un codice ruolo in base all'id.
        ///// </summary>
        ///// <param name="idGroup">Id del gruppo da recuperare</param>
        ///// <returns>Codice del ruolo</returns>
        //public static string GetOriginalCodeRuoloFromIdGroups(string idGroup)
        //{            

        //    string codiceRuolo = string.Empty;

        //    // Reperimento dell'id della tabella corrglobali a partire dall'id della tabella groups
        //    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        //    {
        //        string commandText = string.Format("select var_original_code from dpa_corr_globali where id_gruppo = {0} and (id_old = 0 or id_old is null)", idGroup);

        //        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
        //        {
        //            while (reader.Read())
        //                codiceRuolo = reader.GetValue(0).ToString();
        //        }
        //    }

        //    return codiceRuolo;
        //}

        /// <summary>
        /// Reperimento codice rubrica dell'utente o gruppo nella tabella dpa_corr_globali
        /// </summary>
        /// <param name="idPeopleOrGroup"></param>
        /// <param name="tipo">
        /// Parametro di output: P se persona, R se ruolo
        /// </param>
        /// <returns></returns>
        public static string getCodiceRubrica(string idPeopleOrGroup, out string tipo)
        {
            string codiceRubrica = string.Empty;
            tipo = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("SELECT nvl(var_original_code, var_cod_rubrica) AS codice, cha_tipo_urp as tipo FROM dpa_corr_globali WHERE id_gruppo = {0} OR id_people = {0}", idPeopleOrGroup);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        codiceRubrica = reader.GetString(reader.GetOrdinal("codice"));
                        tipo = reader.GetString(reader.GetOrdinal("tipo"));
                    }
                }


            }

            return codiceRubrica;
        }

        /// <summary>
        /// Reperimento di un array di oggetti "DocsPaSecurityItem",
        /// ciascuno contenente i metadati di security relativi ad un oggetto DocsPa
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static DocsPaSecurityItem[] getSecurityItems(string thing)
        {
            List<DocsPaSecurityItem> securityItems = new List<DocsPaSecurityItem>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT cg.id_people AS id_people, cg.id_gruppo AS id_group, nvl(cg.var_original_code, cg.var_cod_rubrica) AS codice_rubrica, s.accessrights AS accessrights, s.cha_tipo_diritto AS tipodiritto FROM security s, dpa_corr_globali cg WHERE (s.personorgroup = cg.id_gruppo or s.personorgroup = cg.id_people) AND s.thing = {0} AND cg.dta_fine IS NULL", thing);
                
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        securityItems.Add(DocsPaSecurityItem.NewItem(reader));
                }
            }

            return securityItems.ToArray();
        }

        /// <summary>
        /// Reperimento del codice del ruolo a partire dall'id della tabella DPA_CORR_GLOBALI
        /// </summary>
        /// <param name="idCorrGlobali"></param>
        /// <returns></returns>
        public static string getCodiceRuolo(string idCorrGlobali)
        {
            string codiceRuolo = string.Empty;

            // Reperimento dell'id della tabella corrglobali a partire dall'id della tabella groups
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("SELECT var_original_code FROM dpa_corr_globali WHERE system_id = {0}", idCorrGlobali);

                dbProvider.ExecuteScalar(out codiceRuolo, commandText);
            }

            return codiceRuolo;
        }

        /// <summary>
        /// Reperimento ruolo dall'id corrglobali
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo getRuolo(string idCorrGlobali)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();
            
            // Caricamento dei dati del ruolo, escludendo le funzioni
            return utentiDb.GetRuoloEnabledAndDisabled(idCorrGlobali);
        }

        /// <summary>
        /// Reperimento del ruolo creatore di un oggetto docspa
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo getRuoloCreatore(string thing)
        {
            DocsPaVO.utente.Ruolo ruolo = null;

            foreach (DocsPaSecurityItem securityItem in getSecurityItems(thing))
            {
                if (securityItem.AccessRights == DocsPaSecurityItem.ACCESS_RIGHT_255)
                {
                    // Ruolo proprietario dell'oggetto
                    ruolo = DocsPaQueryHelper.getRuoloFromIdGroup(securityItem.IdGroup);
                    break;
                }
            }

            return ruolo;
        }

        /// <summary>
        /// Reperimento del ruolo creatore di un oggetto docspa
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente getUtenteCreatore(string thing)
        {
            DocsPaVO.utente.Utente utente = null;

            foreach (DocsPaSecurityItem securityItem in getSecurityItems(thing))
            {
                if (securityItem.AccessRights == DocsPaSecurityItem.ACCESS_RIGHT_0)
                {
                    // Utente proprietario dell'oggetto
                    utente = DocsPaQueryHelper.getUtente(securityItem.IdPeople);

                    break;
                }
            }

            return utente;
        }

        /// <summary>
        /// Verifica se l'utente è configurato in amministrazione per l'autenticazione di dominio 
        /// </summary>
        /// <param name="idPeople">Id dell'utente da verificare</param>
        /// <param name="networkAlias">
        /// Nome utente di rete corrispondente all'utente applicativo pitre (non è detto che corrispondano)
        /// </param>
        /// <returns></returns>
        public static bool isUtenteDominio(string idPeople, out string networkAlias)
        {
            networkAlias = string.Empty;
            bool retValue = false;

            using (DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti())
                utentiDb.GetDominio(out networkAlias, idPeople);

            retValue = (!string.IsNullOrEmpty(networkAlias));

            return retValue;
        }

        /// <summary>
        /// Verifica se l'utente è configurato in amministrazione per l'autenticazione di dominio 
        /// </summary>
        /// <param name="idPeople">Id dell'utente da verificare</param>
        /// <returns></returns>
        public static bool isUtenteDominio(string idPeople)
        {
            string networkAlias;
            return isUtenteDominio(idPeople, out networkAlias);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static bool isUtenteDominioOrLdap(DocsPaVO.utente.UserLogin userLogin)
        {
            return (isUtenteDominio(userLogin.SystemID) || 
                    DocsPaLdapServices.LdapUserConfigurations.UserCanConnectToLdap(userLogin.UserName));
        }

        /// <summary>
        /// Reperimento di tutti i docnumber dei documenti in un folder
        /// </summary>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public static string[] getDocumentiInFolder(string idFolder)
        {
            List<string> documenti = new List<string>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT pc.link FROM project_components pc WHERE pc.project_id = {0}", idFolder);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        documenti.Add(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "link", false, string.Empty).ToString());
                }
            }

            return documenti.ToArray();
        }

        /// <summary>
        /// Reperimento di tutti i docnumber dei documenti in un fascicolo
        /// <remarks>
        /// Vengono reperiti anche i documenti per tutti gli eventuali sottofascicoli
        /// </remarks>
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        public static string[] getDocumentiInFascicolo(string idFascicolo)
        {
            List<string> documenti = new List<string>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT pc.link FROM project pj INNER JOIN project_components pc on pj.system_id = pc.project_id WHERE pj.cha_tipo_proj = 'C' and pj.id_fascicolo = {0} and pc.type = 'D'", idFascicolo);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        documenti.Add(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "link", false, string.Empty).ToString());
                }
            }

            return documenti.ToArray();
        }

        /// <summary>
        /// Reperimento codice del registro dall'id univoco
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static string getCodiceRegistro(string idRegistro)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione ammQuery = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            return ammQuery.GetCodiceRegistro(idRegistro);
        }
        
        /// <summary>
        /// Reperimento del titolario attivo in docspa
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.OrgTitolario getTitolarioAttivo(string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione ammQuery = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            return ammQuery.getTitolarioAttivo(idAmministrazione);
        }

        /// <summary>
        /// Reperimento di tutti gli id dei fascicoli procedimentali appartenenti ad un nodo di titolario
        /// </summary>
        /// <param name="idNodoTitolario"></param>
        /// <returns></returns>
        public static string[] getFascicoliProcedimentaliNodoTitolario(string idNodoTitolario)
        {
            List<string> list = new List<string>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT system_id FROM project WHERE id_parent = {0} AND cha_tipo_proj = 'F' AND cha_tipo_fascicolo = 'P'", idNodoTitolario);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        list.Add(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "system_id", false).ToString());
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento di tutti gli id dei fascicoli procedimentali appartenenti ad un titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public static string[] getFascicoliProcedimentaliTitolario(string idTitolario)
        {
            List<string> list = new List<string>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT system_id FROM project WHERE id_titolario = {0} AND cha_tipo_proj = 'F' AND cha_tipo_fascicolo = 'P'", idTitolario);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        list.Add(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "system_id", false).ToString());
                }
            }

            return list.ToArray();
        }


        /// <summary>
        /// Reperimento di un oggetto titolario in docspa
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.OrgTitolario getTitolario(string idTitolario)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione ammQuery = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            return ammQuery.getTitolarioById(idTitolario);
        }

        /// <summary>
        /// Reperimento dell'id del fascicolo generale del titolario richiesto
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public static string getIdFascicoloGenerale(string idTitolario)
        {
            string retValue = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("select system_id from project where id_parent = {0} and cha_tipo_proj = 'F' and cha_tipo_fascicolo = 'G'", idTitolario);

                dbProvider.ExecuteScalar(out retValue, commandText);
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento ID da docspa del record di tipo "C" per il fascicolo o per il nodo di titolario (F o T)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idForT"></param>
        /// <returns></returns>
        public static string getIdFolderCType(string idForT)
        {
            string recordC = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("select cha_tipo_proj from project where system_id = {0}", idForT);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                {
                    if (field.Equals("C"))
                    {
                        recordC = idForT;
                    }
                    else
                    {
                        if (field.Equals("T"))
                        {
                            // Il record è di tipo "T", reperimento record di tipo "C" 
                            commandText = string.Format("select system_id from project where id_parent in (select system_id from project where id_parent = {0} and cha_tipo_proj = 'F')", idForT);
                        }
                        else if (field.Equals("F"))
                        {
                            // Il record è di tipo "F", reperimento record di tipo "C" 
                            commandText = string.Format("select system_id from project where id_parent = {0}", idForT);
                        }

                        if (dbProvider.ExecuteScalar(out field, commandText))
                        {
                            recordC = field;
                        }
                    }
                }
            }

            return recordC;
        }

        /// <summary>
        /// Reperimento primo numero di versione disponibile
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static int getDocumentNextVersionId(string idDocument)
        {
            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();
            return checkInOutDb.GetNextVersionId(idDocument);
        }

        /// <summary>
        /// Verifica se la versione corrente del documento è acquisita
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static bool isDocumentAcquisito(string idDocument)
        {
            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();
            return checkInOutDb.IsAcquired(idDocument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static bool isAllegatoProfilato(string docNumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();
            return documentiDb.isAllegatoProfilato(docNumber);
        }

        /// <summary>
        /// Verifica se il documento richiesto è di tipo stampa registro
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static bool isStampaRegistro(string idDocument)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT cha_tipo_proto FROM profile WHERE docnumber = {0}", idDocument);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = (field == "R" || field == "C");
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se il documento richiesto è di tipo stampa registro repertorio
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static bool isStampaRegistroRepertorio(string idDocument)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT cha_tipo_proto FROM profile WHERE docnumber = {0}", idDocument);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = (field == "C");
            }

            return retValue;
        }


        /// <summary>
        /// Reperimento del tipo del fascicolo (generale, procedimentale o sottofascicolo)
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns>Restituisce il tipo fascicolo: "G", "P"</returns>
        public static string getTipoFascicolo(string idFascicolo)
        {
            string retValue = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("select cha_tipo_fascicolo from project where system_id = {0} and cha_tipo_proj = 'F'", idFascicolo);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = field;
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento dell'id del fascicolo a partire dall'id del folder (record con cha_tipo_proj = 'C')
        /// </summary>
        /// <param name="idFolder"></param>
        /// <param name="tipoFascicolo">
        /// Restituisce il tipo fascicolo: "G", "P"
        /// </param>
        /// <returns>
        /// Id del fascicolo (record cha_tipo_proj = 'F')
        /// </returns>
        public static string getIdFascicoloFromFolder(string idFolder, out string tipoFascicolo)
        {
            string retValue = string.Empty;
            tipoFascicolo = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string innerCommandText = string.Format("select id_fascicolo from project where system_id = {0}", idFolder);

                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("select system_id, cha_tipo_fascicolo from project where system_id = ({0}) and cha_tipo_proj = 'F'", innerCommandText);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        retValue = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "system_id", false).ToString();
                        tipoFascicolo = DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "cha_tipo_fascicolo", false).ToString();
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se l'idFolder fornito è relativo ad un fascicolo o a un sottofascicolo
        /// </summary>
        /// <param name="idFolder">
        /// Id del folder (record di tipo "C") per cui verificare se il fascicolo
        /// parent è un fascicolo o un sottofascicolo
        /// </param>
        /// <returns></returns>
        public static bool isSottofascicolo(string idFolder)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string innerCommandText = string.Format("select id_parent from project where system_id = {0}", idFolder);

                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("select cha_tipo_proj from project where system_id = ({0})", innerCommandText);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = (field == "C");
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se l'id fornito è relativo ad un fascicolo generale
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        public static bool isFascicoloGenerale(string idFascicolo)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("select cha_tipo_fascicolo from project where system_id = {0}", idFascicolo);
                
                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = (field == "G");
            }
            
            return retValue;
        }

        /// <summary>
        /// A partire dall'id del fascicolo (procedimentale o generale) o del folder,
        /// viene reperito l'id del titolario di appartenenza
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        public static string getIdNodoTitolario(string idFascicolo)
        {
            string idNodoTitolario = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("select id_parent from project where system_id = {0}", idFascicolo);

                dbProvider.ExecuteScalar(out idNodoTitolario, commandText);
            }

            return idNodoTitolario;
        }

        /// <summary>
        /// Reperimento di un fascicolo a partire dall'id
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Fascicolo getFascicolo(string idFascicolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            // Reperimento oggetto fascicolo di appartenenza del sottofascicolo
            DocsPaDB.Query_DocsPAWS.Fascicoli dbFascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return dbFascicoli.GetFascicoloById(idFascicolo, infoUtente);
        }

        /// Reperimento scheda documento senza security
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento getSchedaDocumentoNoSecurity(string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti();

            return dbDocumenti.GetDettaglioNoSecurity(infoUtente, DocsPaQueryHelper.getIdProfile(docNumber), docNumber);
        }

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
        /// Reperimento oggetti info documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.InfoDocumento getInfoDocumento(string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Documenti dbDocumenti = new DocsPaDB.Query_DocsPAWS.Documenti();

            return dbDocumenti.GetInfoDocumento(infoUtente.idGruppo, infoUtente.idPeople, getIdProfile(docNumber), false);
        }

        /// <summary>
        /// Reperimento di un fascicolo a partire dall'id del record di tipo "C" (root folder del fascicolo)
        /// <param name="idfascicoloC"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Fascicolo getFascicoloFromCType(string idfascicoloC, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string idFascicolo = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT id_fascicolo FROM project WHERE system_id = {0} AND cha_tipo_proj = 'C'", idfascicoloC);

                dbProvider.ExecuteScalar(out idFascicolo, commandText);
            }

            return DocsPaQueryHelper.getFascicolo(idFascicolo, infoUtente);
        }

        /// <summary>
        /// Reperimento dei ruoli che hanno la visibilità sul nodo di titolario
        /// </summary>
        /// <param name="idNodoTitolario"></param>
        /// <returns></returns>
        public static string[] getRuoliNodoTitolario(string idNodoTitolario)
        {
            List<string> list = new List<string>();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("SELECT g.group_id FROM security s, groups g WHERE s.personorgroup = g.system_id AND s.thing = {0}", idNodoTitolario);

                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        list.Add(reader.GetValue(0).ToString());
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Verifica se l'utente è tra gli utenti del ruolo richiesto
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static bool isUtenteInRuolo(string idPeople, string roleName)
        {
            bool retValue = false;
            
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string innerCommandText = string.Format("SELECT groups_system_id FROM groups WHERE UPPER(group_id) = UPPER('{0}')", roleName);

                string commandText = string.Format("SELECT COUNT(*) FROM PeopleGroups WHERE people_system_id = {0} AND system_id = ({1}) AND dta_fine IS NULL", idPeople, innerCommandText);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                {
                    int count;
                    if (Int32.TryParse(field, out count))
                        retValue = (count > 0);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento codice rubrica utente o ruolo proprietario di un oggetto in docspa
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="utente">
        /// Se true, indica di reperire il codice dell'utente proprietario, altrimenti del ruolo
        /// </param>
        /// <returns></returns>
        public static string getCodiceRubricaProprietario(string thing, bool utente)
        {
            string codiceRubrica = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Empty;
                string innerCommandText = string.Empty;

                if (utente)
                {

                    innerCommandText = string.Format("SELECT s.personorgroup FROM SECURITY s, PEOPLE p WHERE s.personorgroup = p.system_id AND s.thing = {0} AND s.cha_tipo_diritto = 'P'", thing);
                    commandText = string.Format("SELECT cg.var_cod_rubrica FROM dpa_corr_globali cg where cg.id_people = ({0})", innerCommandText);
                }
                else
                {
                    innerCommandText = string.Format("SELECT s.personorgroup FROM SECURITY s, GROUPS g WHERE s.personorgroup = g.system_id AND s.thing = {0} AND s.cha_tipo_diritto = 'P'", thing);
                    commandText = string.Format("SELECT cg.var_original_code FROM dpa_corr_globali cg where cg.id_gruppo = ({0})", innerCommandText);
                }

                // Benché la funzione debba reperire il codice del proprietario da rubrica
                // (sia esso utente (accessrights 0) or ruolo (accessirghs 255), viene effettuato
                // il filtro anche per accessrights 45 per il supporto di quelle casistiche
                // in cui il doc si trova in stato finale e non esistono più accessrights a 0 o 255, ma solo a 45.
                // Funziona perché comunque il tipo diritto è sempre 'P'                
                dbProvider.ExecuteScalar(out codiceRubrica, commandText);
            }

            return codiceRubrica;
        }

        /// <summary>
        /// Oggetto contenente i metadati di security relativi ad un oggetto DocsPa
        /// </summary>
        [Serializable()]
        public class DocsPaSecurityItem
        {
            /// <summary>
            /// Diritto proprietario di un oggetto docspa per l'utente
            /// </summary>
            public const string ACCESS_RIGHT_0 = "0";

            /// <summary>
            /// Diritto in sola lettura di un oggetto docspa per l'utente
            /// </summary>
            public const string ACCESS_RIGHT_45 = "45";

            /// <summary>
            /// Diritto in lettura / scrittura di un oggetto docspa per l'utente (acquisito, ad es, per trasmissione)
            /// </summary>
            public const string ACCESS_RIGHT_63 = "63";

            /// <summary>
            /// Diritto proprietario di un oggetto docspa per un ruolo
            /// </summary>
            public const string ACCESS_RIGHT_255 = "255";

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