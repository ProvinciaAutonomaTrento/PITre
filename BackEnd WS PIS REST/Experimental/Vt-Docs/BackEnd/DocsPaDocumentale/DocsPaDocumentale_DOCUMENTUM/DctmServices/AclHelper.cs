using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Emc.Documentum.FS.Services.Core;
using DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom;
using DataModel = Emc.Documentum.FS.DataModel.Core;

namespace DocsPaDocumentale_DOCUMENTUM.DctmServices
{
    /// <summary>
    /// HelperClass per l'utilizzo del servizio custom per la gestione delle ACL
    /// </summary>
    public class AclHelper
    {
        /// <summary>
        /// Inserimento, nella lista delle entries, di una permission basic
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="accessor"></param>
        /// <param name="permitLevel"></param>
        public static void addBasicPermit(List<AclEntry> entries, string accessor, string permitLevel)
        {   
            bool contains = false;

            // Verifica se la lista delle entries già contiene l'accessor fornito
            foreach (AclEntry entry in entries)
            {
                contains = (entry.accessor.Equals(accessor));
                if (contains)
                    break;
            }

            if (!contains)
            {
                AclEntry entry1 = new AclEntry();
                entry1.accessor = accessor;

                // Creazione permission
                Permission p1 = new Permission();
                p1.name = permitLevel;
                p1.type = PermissionType.BASIC;
                entry1.permission = p1;

                entries.Add(entry1);
            }
        }

        /// <summary>
        /// Impostazione proprietà oggetto per associazione con un'ACL
        /// </summary>
        /// <param name="propertySet"></param>
        /// <param name="aclDefinition"></param>
        public static void setAclObjectProperties(DataModel.Properties.PropertySet propertySet, AclDefinition aclDefinition)
        {
            propertySet.Set<string>("acl_name", aclDefinition.name);
            if (aclDefinition.domain != null)
                propertySet.Set<string>("acl_domain", aclDefinition.domain);
            else
                // "dm_dbo"
                // NB: sembra che (bug dctm??) non sempre "dm_dbo" viene risolto correttamente
                // nel nome del repository owner. Per questo, utilizziamo il nome del repository
                // configurato con l'istanza docspa, che DEVE coincidere (ma in minuscolo)
                propertySet.Set<string>("acl_domain", DctmConfigurations.GetRepositoryName().ToLowerInvariant());
        }
        
        /// <summary>
        /// Creazione di un nuovo oggetto AclDefinition necessario 
        /// per gestire quei casi in cui un oggetto deve essere 
        /// visibile a tutti i gruppi di un'amministrazione.
        /// NB: La creazione dell'acl globale deve essere effettuata
        /// all'atto della creazione di una nuova amministrazione
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static AclDefinition getAclDefinition(string codiceAmministrazione)
        {
            AclDefinition aclData = new AclDefinition();
            aclData.repository = DctmConfigurations.GetRepositoryName();
            aclData.name = getAclName(codiceAmministrazione);
            aclData.description = aclData.name;
            aclData.entries = new AclEntry[0];
            return aclData;
        }

        /// <summary>
        /// Creazione di un nuovo oggetto AclDefinition
        /// associato ad un oggetto DocsPa in DCTM
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <param name="idDocsPa"></param>
        /// <param name="objectType"></param>
        /// <remarks>
        /// Restituisce solo la parte di "intestazione", non le entries
        /// </remarks>
        /// <returns></returns>
        public static AclDefinition getAclDefinition(string codiceAmministrazione, string idDocsPa, string objectType)
        {   
            AclDefinition aclData = new AclDefinition();
            aclData.repository = DctmConfigurations.GetRepositoryName();
            aclData.name = getAclName(codiceAmministrazione, idDocsPa, objectType);
            aclData.description = aclData.name;

            const int MAX_ACL_DESCRIPTION = 128;
            if (aclData.description.Length > MAX_ACL_DESCRIPTION)
                aclData.description = aclData.description.Substring(0, MAX_ACL_DESCRIPTION);

            aclData.entries = new AclEntry[0];
            return aclData;
        }

        /// <summary>
        /// Creazione del nome univoco con cui viene creata l'acl per un oggetto docspa in dctm
        /// </summary>
        /// <remarks>
        /// In Documentum, il limite per il nome dell'acl è 32 caratteri.
        /// Il metodo crea un nome acl univoco che include
        /// il codice dell'amministrazione (massimo 16 caratteri),
        /// il codice abbreviato dell'oggetto (massimo 3 caratteri),
        /// l'id dell'oggetto
        /// </remarks>
        /// <param name="codiceAmministrazione"></param>
        /// <param name="idDocsPa"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static string getAclName(string codiceAmministrazione, string idDocsPa, string objectType)
        {
            return string.Format("{0}.{1}.{2}", 
                    getAclName(codiceAmministrazione),
                    getObjectTypeNameForAcl(objectType), 
                    idDocsPa);
        }

        /// <summary>
        /// Creazione del nome univoco con cui viene creata l'acl per l'amministrazione
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static string getAclName(string codiceAmministrazione)
        {
            return codiceAmministrazione.ToLowerInvariant();
        }

        /// <summary>
        /// 
        /// </summary>
        private const string ACL_DOCUMENTO = "doc";
        private const string ACL_DOCUMENTO_STAMPA_REGISTRO = "dsr";
        private const string ACL_ALLEGATO = "all";
        private const string ACL_TITOLARIO = "tit";
        private const string ACL_NODO_TITOLARIO = "ndc";
        private const string ACL_FASCICOLO = "fas";
        private const string ACL_FASCICOLO_GENERALE = "fge";
        private const string ACL_FASCICOLO_PROCEDIMENTALE = "fpr";
        private const string ACL_SOTTOFASCICOLO = "fld";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private static string getObjectTypeNameForAcl(string objectType)
        {
            string aclObjectTypeName = string.Empty;

            switch (objectType)
            {
                case DocsPaObjectTypes.ObjectTypes.DOCUMENTO:
                    aclObjectTypeName = ACL_DOCUMENTO;
                    break;
                case DocsPaObjectTypes.ObjectTypes.DOCUMENTO_STAMPA_REGISTRO:
                    aclObjectTypeName = ACL_DOCUMENTO_STAMPA_REGISTRO;
                    break;
                case DocsPaObjectTypes.ObjectTypes.TITOLARIO:
                    aclObjectTypeName = ACL_TITOLARIO;
                    break;
                case DocsPaObjectTypes.ObjectTypes.NODO_TITOLARIO:
                    aclObjectTypeName = ACL_NODO_TITOLARIO;
                    break;
                case DocsPaObjectTypes.ObjectTypes.FASCICOLO:
                    aclObjectTypeName = ACL_FASCICOLO;
                    break;
                case DocsPaObjectTypes.ObjectTypes.FASCICOLO_GENERALE:
                    aclObjectTypeName = ACL_FASCICOLO_GENERALE;
                    break;
                case DocsPaObjectTypes.ObjectTypes.FASCICOLO_PROCEDIMENTALE:
                    aclObjectTypeName = ACL_FASCICOLO_PROCEDIMENTALE;
                    break;
            }

            return aclObjectTypeName;
        }
    }
}