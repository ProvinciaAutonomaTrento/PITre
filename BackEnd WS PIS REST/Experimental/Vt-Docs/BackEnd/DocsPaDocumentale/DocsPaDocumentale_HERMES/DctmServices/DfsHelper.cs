using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.Runtime.Context;

namespace DocsPaDocumentale_DOCUMENTUM.DctmServices
{
    /// <summary>

    /// </summary>
    public class DfsHelper
    {
        /// <summary>
        /// Costruttore di ObjecyIdentity[ObjectId].        
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="repositoryName"></param>
        /// <returns></returns>
        public static ObjectIdentity createObjectIdentityObjId(string objectId)
        {
            return createObjectIdentityObjId(new ObjectId(objectId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static ObjectIdentity createObjectIdentityObjId(ObjectId objectId)
        {
            ObjectIdentity objectIdentity = new ObjectIdentity(objectId, DctmConfigurations.GetRepositoryName());
            objectIdentity.ValueType = ObjectIdentityType.OBJECT_ID;
            objectIdentity.valueTypeSpecified = true;
            return objectIdentity;
        }

        /// <summary>
        /// Costruttore di ObjecyIdentity[ObjectPath]
        /// </summary>
        /// <param name="repositoryName"></param>
        /// <param name="pathString"></param>
        /// <returns></returns>
        public static ObjectIdentity createObjectIdentityByPath(string pathString)
        {
            return createObjectIdentityByPath(new ObjectPath(pathString));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ObjectIdentity createObjectIdentityByPath(ObjectPath path)
        {
            ObjectIdentity objectIdentity = new ObjectIdentity(path, DctmConfigurations.GetRepositoryName());
            objectIdentity.ValueType = ObjectIdentityType.OBJECT_PATH;
            objectIdentity.valueTypeSpecified = true;
            return objectIdentity;
        }

        /// <summary>
        /// Costruttore di ObjecyIdentity[Qualification]
        /// </summary>
        /// <param name="qualString"></param>
        /// <param name="repositoryName"></param>
        /// <returns></returns>
        public static ObjectIdentity createObjectIdentityByQualification(string qualString)
        {
            return createObjectIdentityByQualification(new Qualification(qualString));
        }

        /// <summary>
        /// Costruttore di ObjecyIdentity[Qualification]
        /// </summary>
        /// <param name="qual"></param>
        /// <returns></returns>
        public static ObjectIdentity createObjectIdentityByQualification(Qualification qual)
        {
            ObjectIdentity objectIdentity = new ObjectIdentity(qual, DctmConfigurations.GetRepositoryName());
            objectIdentity.ValueType = ObjectIdentityType.QUALIFICATION;
            objectIdentity.valueTypeSpecified = true;
            return objectIdentity;
        }
        
        /// <summary>
        /// Restituisce un DataObject con tutte le proprietà di un oggetto DCTM e
        /// l'insieme dei folder a cui è linkato (quest'ultimi solo come identity però)
        /// </summary>
        /// <param name="objSrvc"></param>
        /// <param name="documentIdentity"></param>
        /// <param name="propertyNames">null == TUTTE; altrimenti quelle indicate. OKKIO: in nessun caso restituisce r_object_id !!</param>
        /// <returns></returns>
        public static DataObject getAllPropsAndFolders(IObjectService objSrvc,
                                                        ObjectIdentity objectIdentity,
                                                        List<string> propertyNames,
                                                        bool includeFolders)
        {

            DataPackage pkg = new DataPackage();

            ObjectIdentitySet objectIdSet = new ObjectIdentitySet(objectIdentity);

            PropertyProfile propertyProfile = null;

            //propertyProfile.filterModeSpecified = true;
            if (propertyNames == null)
                propertyProfile = new PropertyProfile(PropertyFilterMode.ALL);
            else
            {
                propertyProfile = new PropertyProfile(PropertyFilterMode.SPECIFIED_BY_INCLUDE);
                propertyProfile.IncludeProperties = propertyNames;
            }

            RelationshipProfile relationProfile = new RelationshipProfile();
            if (includeFolders)
            {
                //TODO: bug di DFS60sp1:
                // se specifico che voglio solo le relazioni parent non funziona
                // devo chiedere tutte le relazioni

                //relationProfile.ResultDataMode = ResultDataMode.REFERENCE;
                //relationProfile.TargetRoleFilter = TargetRoleFilter.ANY;
                //relationProfile.NameFilter = RelationshipNameFilter.ANY;
                //relationProfile.DepthFilter = DepthFilter.UNLIMITED;

                // proposta di Azzalini per velocizzare eliminazione documento in Fascicolo con DCTM 6.7
                relationProfile.ResultDataMode = ResultDataMode.REFERENCE;
                relationProfile.TargetRoleFilter = TargetRoleFilter.SPECIFIED;
                relationProfile.TargetRole = Relationship.ROLE_PARENT;
                relationProfile.NameFilter = RelationshipNameFilter.SPECIFIED;
                relationProfile.RelationName = Relationship.RELATIONSHIP_FOLDER;
                relationProfile.DepthFilter = DepthFilter.SPECIFIED;
                relationProfile.Depth = 1; 
            }

            OperationOptions operationOptions = new OperationOptions();
            //operationOptions.Profiles = new Profile[profilesCount];
            operationOptions.PropertyProfile = propertyProfile;
            if (includeFolders)
                operationOptions.RelationshipProfile = relationProfile;

            pkg = objSrvc.Get(objectIdSet, operationOptions);
            return pkg.DataObjects[0];
        }

        /// <summary>
        /// Restituisce il campo r_object_id di documentum usando una query.
        /// </summary>
        /// <param name="querySvc"></param>
        /// <param name="repositoryName"></param>
        /// <param name="objectType"></param>
        /// <param name="qualification">Una condizione per ricercare l'oggetto</param>
        /// <returns></returns>
        public static string getDctmObjectId(IQueryService querySvc, string objectType, string qualification)
        {
            string result = string.Empty;

            PassthroughQuery passthroughQuery = new PassthroughQuery();
            passthroughQuery.QueryString = "SELECT r_object_id FROM " + objectType + " WHERE " + qualification;
            passthroughQuery.Repositories = new List<string>();
            passthroughQuery.Repositories.Add(DctmConfigurations.GetRepositoryName());

            QueryExecution queryExecution = new QueryExecution();
            queryExecution.CacheStrategyType = CacheStrategyType.NO_CACHE_STRATEGY;
            queryExecution.MaxResultCount = 1;

            QueryResult queryResult = querySvc.Execute(passthroughQuery, queryExecution, null);
            if (queryResult.DataPackage.DataObjects != null && queryResult.DataPackage.DataObjects.Count > 0)
            {
                DataObject dataObj = queryResult.DataPackage.DataObjects[0];
                result = ((ObjectId)dataObj.Identity.Value).Id;
            }

            return result;
        }

        public static string getDctmObjectId(IQueryService querySvc, Qualification qualification)
        {
            string result = string.Empty;

            PassthroughQuery passthroughQuery = new PassthroughQuery();
            passthroughQuery.QueryString = "SELECT r_object_id  FROM " + qualification.GetValueAsString();
            passthroughQuery.Repositories = new List<string>();
            passthroughQuery.Repositories.Add(DctmConfigurations.GetRepositoryName());

            QueryExecution queryExecution = new QueryExecution();
            queryExecution.CacheStrategyType = CacheStrategyType.NO_CACHE_STRATEGY;
            queryExecution.MaxResultCount = 1;

            QueryResult queryResult = querySvc.Execute(passthroughQuery, queryExecution, null);
            if (queryResult.DataPackage.DataObjects != null && queryResult.DataPackage.DataObjects.Count > 0)
            {
                DataObject dataObj = queryResult.DataPackage.DataObjects[0];
                result = ((ObjectId)dataObj.Identity.Value).Id;
            }

            return result;
        }

        /// <summary>
        /// Crea un oggetto ReferenceRelationship adatto per rimuovere un link tra un oggetto ed il folder specificato
        /// </summary>
        /// <param name="folderIdentity">identifica il folder parent</param>
        /// <returns></returns>
        public static ReferenceRelationship createRemoveParentFolder(ObjectIdentity folderIdentity)
        {
            ReferenceRelationship removeRelationship = new ReferenceRelationship();
            removeRelationship.TargetRole = Relationship.ROLE_PARENT;
            //removeRelationship.intentModifierSpecified = true;
            removeRelationship.Name = Relationship.RELATIONSHIP_FOLDER;
            removeRelationship.Target = folderIdentity;
            removeRelationship.IntentModifier = RelationshipIntentModifier.REMOVE;
            return removeRelationship;
        }


        /// <summary>
        /// Crea un oggetto ReferenceRelationship adatto per creare un link tra un oggetto ed il folder specificat
        /// </summary>
        /// <param name="folderIdentity">identifica il folder parent</param>
        /// <returns></returns>
        public static ReferenceRelationship createParentFolderRelationship(ObjectIdentity folderIdentity)
        {
            ReferenceRelationship addRelationship = new ReferenceRelationship();
            addRelationship.TargetRole = Relationship.ROLE_PARENT;
            addRelationship.Name = Relationship.RELATIONSHIP_FOLDER;

            // i due seguenti secondo la doc. non sarebbero necessari, ma...
            //addRelationship.intentModifierSpecified = true;
            addRelationship.IntentModifier = RelationshipIntentModifier.ADD;

            addRelationship.Target = folderIdentity;
            return addRelationship;
        }

        /// <summary>
        /// Restituisce i folders ai quali l'oggetto è linkato (direttamente).
        /// In realtà sembra che DFS60sp1 abbia un bug: non posso imporre nessuna limitazione
        /// nella query per le relationship, quindi per il momento vengono resituite tutte
        /// TODO: cercare solo le relazioni dirette e di tipo "folder"
        /// </summary>
        /// <param name="objSrvc"></param>
        /// <param name="documentIdentity"></param>
        /// <returns></returns>
        public static DataObject getParentFolders(IObjectService objSrvc, ObjectIdentity documentIdentity)
        {
            //TODO: bug di DFS60sp1:
            // se specifico che voglio solo le relazioni parent non funziona
            // devo chiedere tutte le relazioni

            RelationshipProfile relationProfile = new RelationshipProfile();
            relationProfile.ResultDataMode = ResultDataMode.REFERENCE;
            relationProfile.TargetRoleFilter = TargetRoleFilter.ANY;
            relationProfile.NameFilter = RelationshipNameFilter.ANY;
            relationProfile.DepthFilter = DepthFilter.UNLIMITED;
            OperationOptions operationOptions = new OperationOptions();
            operationOptions.RelationshipProfile = relationProfile;

            ObjectIdentitySet objectIdSet = new ObjectIdentitySet(documentIdentity);
            DataPackage pkg = objSrvc.Get(objectIdSet, operationOptions);

            return pkg.DataObjects[0];

        }



        /// <summary>
        /// Reperimento formato file documentum
        /// </summary>
        /// <param name="queryService"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static string getDctmFileFormat(IQueryService queryService, string fileExtension)
        {
            string fileFormat = string.Empty;

            PassthroughQuery passthroughQuery = new PassthroughQuery();
            passthroughQuery.QueryString = string.Format("SELECT name FROM dm_format WHERE dos_extension = '{0}'", replaceInvalidFileExtension(fileExtension).ToLower());
            passthroughQuery.Repositories = new List<string>();
            passthroughQuery.Repositories.Add(DctmConfigurations.GetRepositoryName());

            QueryExecution queryExecution = new QueryExecution();
            queryExecution.CacheStrategyType = CacheStrategyType.NO_CACHE_STRATEGY;
            queryExecution.MaxResultCount = 1;

            QueryResult queryResult = queryService.Execute(passthroughQuery, queryExecution, null);

            if (queryResult.DataPackage.DataObjects != null &&
                queryResult.DataPackage.DataObjects.Count > 0)
            {
                DataObject dataObj = queryResult.DataPackage.DataObjects[0];

                fileFormat = ((StringProperty)dataObj.Properties.Properties[0]).Value;
            }

            return fileFormat;
        }

        /// <summary>
        /// Esegue una DQL di tipo UPDATE o DDL (ALTER...)
        /// </summary>
        /// <param name="qrySrvc"></param>
        /// <param name="dql"></param>
        /// <returns></returns>
        public static QueryResult executePassThrough(IQueryService qrySrvc, string dql)
        {
            string repositoryName = DctmConfigurations.GetRepositoryName();
            PassthroughQuery query = new PassthroughQuery();
            query.QueryString = dql;
            query.AddRepository(repositoryName);
            QueryExecution queryEx = new QueryExecution();
            queryEx.MaxResultCount = -1; // Nessun limite di risultati restituiti
            queryEx.MaxResultPerSource = -1;
            queryEx.CacheStrategyType = CacheStrategyType.NO_CACHE_STRATEGY;
            OperationOptions operationOptions = null;
            return qrySrvc.Execute(query, queryEx, operationOptions);
        }

        public static bool isUserMemberOf(String userName, string groupName, IQueryService qrySrvc)
        {
            string query = string.Format(
                "SELECT i_all_users_names FROM dm_group WHERE group_name = '{0}' AND ANY i_all_users_names = '{1}'", 
                groupName, userName);
            QueryResult queryResult = DfsHelper.executePassThrough(qrySrvc, query);
            if (queryResult.DataObjects.Count > 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        /// <param name="qrySvc"></param>
        public static void insertUserInGroup(string userName, string groupName, IQueryService qrySvc)
        {
            string query = string.Format("ALTER GROUP '{0}' ADD '{1}'", groupName, userName);
            QueryResult queryResult = DfsHelper.executePassThrough(qrySvc, query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        /// <param name="qrySvc"></param>
        public static void removeUserFromGroup(string userName, string groupName, IQueryService qrySvc)
        {
            string query = string.Format("ALTER GROUP '{0}' DROP '{1}'", groupName, userName);
            QueryResult queryResult = DfsHelper.executePassThrough(qrySvc, query);
        }

        /// <summary>
        /// Workaround per risolvere un bug documentum.
        /// Sembra che associare al documento un file con il formato xml crei problemi 
        /// nel successivo reperimento del contenuto. Pertanto un file xml 
        /// verrà considerato in dctm come un normale file txt.
        /// Il workaround può essere applicato a tutti gli eventuali formati non accettati.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        private static string replaceInvalidFileExtension(string fileExtension)
        {
            fileExtension = fileExtension.Replace(".", string.Empty);

            switch (fileExtension.ToLower())
            {
                case "xml":
                case "p7m":
                    fileExtension = "txt";
                    break;
            }

            return fileExtension;
        }
    }
}
