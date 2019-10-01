using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale_DOCUMENTUM.DocsPaServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.DataModel.Core.Query;
using CustomServices = DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom;
using log4net;
using System.Globalization;

namespace DocsPaDocumentale_DOCUMENTUM.DctmServices
{
    /// <summary>
    ///
    /// </summary>
    public sealed class Dfs4DocsPa
    {
        public static ILog logger = LogManager.GetLogger(typeof(Dfs4DocsPa));
        /// <summary>
        /// Lunghezza massima consentita campo objectname
        /// </summary>
        private const int OBJECT_NAME_MAX_LENGHT = 255;

        /// <summary>
        /// Lunghezza massima consentita campo title
        /// </summary>
        private const int TITLE_MAX_LENGTH = 255;

        /// <summary>
        /// Codice da utilizzare per marcare gli oggetti come modificabili solo da DocsPA
        /// </summary>
        public const string APPLICATION_CODE = "pitre";

        /// <summary>
        /// 
        /// </summary>
        private Dfs4DocsPa()
        { }

        #region Public methods

        /// <summary>
        /// Impostazione dell'ownership sul documento e di tutti gli eventuali allegati
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="ownerUser"></param>
        /// <param name="ownerRole"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static void setOwnershipDocumento(string idProfile, string ownerUser, string ownerRole, IQueryService queryService)
        {
            string docNumber = DocsPaQueryHelper.getDocNumber(idProfile);

            string dql = string.Format("UPDATE {0} OBJECT SET owner_name = '{1}', SET {2} = '{3}' WHERE {4} = '{5}'",
                        ObjectTypes.DOCUMENTO,
                        ownerUser,
                        TypeDocumento.RUOLO_CREATORE,
                        ownerRole,
                        TypeDocumento.DOC_NUMBER,
                        docNumber);

            DfsHelper.executePassThrough(queryService, dql);

            if (DocsPaQueryHelper.getCountAllegati(idProfile) > 0)
            {
                dql = string.Format("UPDATE {0} OBJECT SET owner_name = '{1}' WHERE {2} = '{3}'",
                                            ObjectTypes.DOCUMENTO,
                                            ownerUser,
                                            TypeDocumento.DOC_NUMBER,
                                            docNumber);

                DfsHelper.executePassThrough(queryService, dql);
            }
        }


        /// <summary>
        /// Impostazione dell'ownership sul documento
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="ownerName"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static void setOwnershipFascicolo(string idFascicolo, string ownerName, IQueryService queryService)
        {
            // Modifica del campo "owner_name" per il fascicolo e tutti gli eventuali sottofascicoli
            string dql = string.Format("UPDATE {0} OBJECT SET owner_name = '{1}' WHERE {2} = '{3}'",
                                        ObjectTypes.FASCICOLO,
                                        ownerName,
                                        TypeFascicolo.ID_DOCSPA,
                                        idFascicolo);

            DfsHelper.executePassThrough(queryService, dql);

            // Se nel fascicolo sono presenti sottofascicoli
            if (DocsPaQueryHelper.getCountSottofascicoli(idFascicolo) > 0)
            {
                // Aggiornamento ownership sottofascicoli di appartenenza
                dql = string.Format("UPDATE {0} OBJECT SET owner_name = '{1}' WHERE {2} = '{3}'",
                                        ObjectTypes.SOTTOFASCICOLO,
                                        ownerName,
                                        TypeSottofascicolo.ID_FASCICOLO,
                                        idFascicolo);

                DfsHelper.executePassThrough(queryService, dql);
            }
        }

        /// <summary>
        /// Reperimento oggetto AclDefinition valida per gli oggetti che devono essere visibili
        /// senza distinizioni da tutti gli utenti dell'amministrazione 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static CustomServices.AclDefinition getAclDefinitionAmministrazione(DocsPaVO.utente.InfoUtente infoUtente)
        {
            return Dfs4DocsPa.getAclDefinitionAmministrazione(DocsPaQueryHelper.getCodiceAmministrazione(infoUtente.idAmministrazione));
        }

        /// <summary>
        /// Reperimento oggetto AclDefinition valida per gli oggetti che devono essere visibili
        /// senza distinizioni da tutti gli utenti dell'amministrazione
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static CustomServices.AclDefinition getAclDefinitionAmministrazione(string codiceAmministrazione)
        {
            // Reperimento ACL da associare
            CustomServices.AclDefinition aclData = AclHelper.getAclDefinition(DocsPaAdminCabinet.getCodiceAmministrazione(codiceAmministrazione));

            List<CustomServices.AclEntry> entries = new List<CustomServices.AclEntry>();

            // Entry per "dm_owner"
            AclHelper.addBasicPermit(entries, "dm_owner", Permission.DELETE);

            // Entry per il ruolo di tutti gli utenti dell'amministrazione
            AclHelper.addBasicPermit(entries, DocsPaObjectTypes.TypeGruppo.GetGroupNameForAmministrazione(codiceAmministrazione), Permission.READ);

            // Entry per il ruolo degli amministratori dell'amministrazione
            AclHelper.addBasicPermit(entries, DocsPaObjectTypes.TypeGruppo.GetGroupNameForSysAdminAmministrazione(codiceAmministrazione), Permission.DELETE);

            aclData.entries = entries.ToArray();

            return aclData;
        }

        /// <summary>
        /// Traduzione della codifica DocsPa per DCTM per quanto riguarda i permessi su un oggetto DocsPa
        /// </summary>
        /// <param name="accessRights"></param>
        /// <returns></returns>
        public static string getPermitLevel(string accessRights)
        {
            // Indipendentemente dalla modalità di accesso definita in DocsPa,
            // in documentum l'entry viene impostata come DELETE
            return Permission.DELETE;

            //string permitLevel = string.Empty;

            //if (accessRights.EndsWith(DocsPaQueryHelper.DocsPaSecurityItem.ACCESS_RIGHT_45))
            //{
            //    // Diritto di sola lettura
            //    permitLevel = Permission.READ;
            //}
            //else if (accessRights.EndsWith(DocsPaQueryHelper.DocsPaSecurityItem.ACCESS_RIGHT_63))
            //{
            //    // Diritto di lettura / scrittura (ereditato)
            //    // Per DCTM equivale comunque a DELETE, in quanto in docspa
            //    // è sempre possibile rimuovere l'oggetto.
            //    permitLevel = Permission.DELETE;
            //}
            //else if (accessRights.EndsWith(DocsPaQueryHelper.DocsPaSecurityItem.ACCESS_RIGHT_255) || accessRights.EndsWith(DocsPaQueryHelper.DocsPaSecurityItem.ACCESS_RIGHT_0))
            //{
            //    // Diritto di ruolo creatore dell'oggetto (255) 
            //    // o di utente creatore dell'oggetto (0)
            //    // Ad entrambi si concede diritto di cancellazione
            //    permitLevel = Permission.DELETE;
            //}

            //return permitLevel;
        }

        /// <summary>
        /// Reperimento oggetto AclDefinition aggiornato per l'oggetto nella tabella security di docspa
        /// </summary>
        /// <param name="idDocsPa"></param>
        /// <param name="objectType"></param>
        /// <param name="codiceAmministrazione"></param>        
        /// <returns></returns>
        public static CustomServices.AclDefinition getAclDefinition(string idDocsPa, string objectType, string codiceAmministrazione)
        {
            // Reperimento ACL base per tutti gli oggetti docspa in dctm
            CustomServices.AclDefinition aclData = Dfs4DocsPa.getAclWithBasicPermit(idDocsPa, objectType, codiceAmministrazione);

            List<CustomServices.AclEntry> entries = new List<CustomServices.AclEntry>(aclData.entries);

            // Reperimento metadati in tabella Security di DocsPa
            foreach (DocsPaQueryHelper.DocsPaSecurityItem securityItem in DocsPaQueryHelper.getSecurityItems(idDocsPa))
            {
                string accessor = string.Empty;

                if (securityItem.IsPeople)
                    accessor = TypeUtente.NormalizeUserName(securityItem.CodiceRubrica);
                else
                    accessor = TypeGruppo.NormalizeGroupName(securityItem.CodiceRubrica);

                // Aggiornamento entries per l'oggetto AclDefinition
                AclHelper.addBasicPermit(entries, accessor, Dfs4DocsPa.getPermitLevel(securityItem.AccessRights));
            }

            aclData.entries = entries.ToArray();

            return aclData;
        }

        /// <summary>
        /// Reperimento oggetto AclDefinition aggiornato per l'oggetto nella tabella security di docspa
        /// </summary>
        /// <param name="idDocsPa"></param>
        /// <param name="objectType"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static CustomServices.AclDefinition getAclDefinition(string idDocsPa, string objectType, DocsPaVO.utente.InfoUtente infoUtente)
        {
            return Dfs4DocsPa.getAclDefinition(idDocsPa, objectType, DocsPaQueryHelper.getCodiceAmministrazione(infoUtente.idAmministrazione));
        }

        /// <summary>
        /// Reperimento proprietà documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <param name="insertMode">
        /// Se true, sono richieste le proprietà per il documento in inserimento in documentum
        /// </param>
        /// <returns></returns>
        public static Property[] getDocumentoProperties(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, bool insertMode)
        {
            List<Property> list = new List<Property>();

            list.Add(makeDocsPALockCode());

            bool isPredisposto = ((schedaDocumento.tipoProto == "P" || schedaDocumento.tipoProto == "A" || schedaDocumento.tipoProto == "I") && schedaDocumento.protocollo != null && schedaDocumento.protocollo.daProtocollare != null && schedaDocumento.protocollo.daProtocollare.Equals("1"));
            bool isProtocollo = (!isPredisposto && schedaDocumento.tipoProto != "G" && schedaDocumento.protocollo != null && schedaDocumento.protocollatore != null);

            string objectName = string.Empty;

            if (isProtocollo)
                objectName = schedaDocumento.protocollo.segnatura;
            else
                objectName = string.Format("Documento con ID {0}", schedaDocumento.docNumber.ToString());

            list.Add(new StringProperty("object_name", normalizeStringPropertyValue(objectName, OBJECT_NAME_MAX_LENGHT)));
            list.Add(new StringProperty(TypeDocumento.DOC_NUMBER, schedaDocumento.docNumber));
            list.Add(new StringProperty(TypeDocumento.TIPO_PROTOCOLLO, schedaDocumento.tipoProto));
            list.Add(new BooleanProperty(TypeDocumento.PREDISPONI_PROTOCOLLAZIONE, schedaDocumento.predisponiProtocollazione));
            list.Add(new BooleanProperty(TypeDocumento.DA_PROTOCOLLARE, isPredisposto));

            if (insertMode)
                // Impostazione del numero versione a 1 in modalità di inserimento
                list.Add(new NumberProperty(TypeDocumento.NUMERO_VERSIONE, 1));

            if (schedaDocumento.oggetto != null && !string.IsNullOrEmpty(schedaDocumento.oggetto.descrizione))
            {
                string title = normalizeStringPropertyValue(schedaDocumento.oggetto.descrizione, TITLE_MAX_LENGTH);

                list.Add(new StringProperty("title", title));
                list.Add(new StringProperty(TypeDocumento.OGGETTO, title));
            }

            bool personale = schedaDocumento.personale != null && schedaDocumento.personale.Equals("1");
            bool privato = !personale && schedaDocumento.privato != null && schedaDocumento.privato.Equals("1");

            list.Add(new BooleanProperty(TypeDocumento.PRIVATO, privato));
            list.Add(new BooleanProperty(TypeDocumento.PERSONALE, personale));

            // list.Add(new StringProperty(TypeDocumento.NOTE, schedaDocumento.note));
            list.Add(new StringProperty(TypeDocumento.MEZZO_SPEDIZIONE, schedaDocumento.typeId));

            if (schedaDocumento.paroleChiave != null)
            {
                List<string> items = new List<string>();
                foreach (DocsPaVO.documento.ParolaChiave item in schedaDocumento.paroleChiave)
                    items.Add(item.descrizione);
                list.Add(new StringArrayProperty("keywords", items.ToArray()));
            }

            if (insertMode &&
                    schedaDocumento.creatoreDocumento != null &&
                    schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo != null)
            {
                // Inserimento del ruolo creatore, solo se in modalità di inserimento

                // Reperimento oggetto ruolo da id
                //LULUCIANI 26/06/2014:
                //DocsPaVO.utente.Ruolo ruolo = DocsPaQueryHelper.getRuolo(schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo);
                // sembra che basti l'id quindi evito di chiamare tutto il getruolo..
                DocsPaVO.utente.Ruolo ruolo = null;
                 if(!string.IsNullOrEmpty(schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo))   
                 { ruolo= new DocsPaVO.utente.Ruolo();
                    ruolo.systemId=schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                list.Add(new StringProperty(TypeDocumento.RUOLO_CREATORE, TypeGruppo.GetGroupNameByidCorr(ruolo.systemId)));
                 }
            }

            if (schedaDocumento.tipologiaAtto != null)
                list.Add(new StringProperty(TypeDocumento.TIPO_ATTO, schedaDocumento.tipologiaAtto.descrizione));

            if (isProtocollo)
            {
                list.Add(new StringProperty(TypeDocumento.NUMERO_PROTOCOLLO, schedaDocumento.protocollo.numero));
                list.Add(new StringProperty(TypeDocumento.CODICE_REGISTRO, schedaDocumento.registro.codRegistro));

                
                

                // Reperimento oggetto ruolo da id
                //LULUCIANI 26/06/2014:
                //DocsPaVO.utente.Ruolo ruolo = DocsPaQueryHelper.getRuolo(schedaDocumento.protocollatore.ruolo_idCorrGlobali);
                // list.Add(new StringProperty(TypeDocumento.RUOLO_PROTOCOLLATORE, TypeGruppo.GetGroupName(ruolo)));

                // sembra che basti l'id quindi evito di chiamare tutto il getruolo..
                DocsPaVO.utente.Ruolo ruolo = null;
                if (!string.IsNullOrEmpty(schedaDocumento.protocollatore.ruolo_idCorrGlobali))
                {
                    ruolo = new DocsPaVO.utente.Ruolo();
                    ruolo.systemId = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                }
                list.Add(new StringProperty(TypeDocumento.RUOLO_PROTOCOLLATORE, TypeGruppo.GetGroupNameByidCorr(ruolo.systemId)));

                    
                list.Add(new StringProperty(TypeDocumento.UTENTE_PROTOCOLLATORE, schedaDocumento.protocollatore.utente_idPeople));
                list.Add(new StringProperty(TypeDocumento.DESCRIZIONE_UTENTE_PROTOCOLLATORE, schedaDocumento.protocollatore.uo_codiceCorrGlobali));

                if (!string.IsNullOrEmpty(schedaDocumento.protocollo.daProtocollare))
                    list.Add(new BooleanProperty(TypeDocumento.DA_PROTOCOLLARE, !schedaDocumento.protocollo.daProtocollare.Equals("0")));

                if (!string.IsNullOrEmpty(schedaDocumento.protocollo.anno))
                {
                    int anno;
                    if (Int32.TryParse(schedaDocumento.protocollo.anno, out anno))
                        list.Add(new NumberProperty(TypeDocumento.ANNO_PROTOCOLLO, anno));
                }

                if (!string.IsNullOrEmpty(schedaDocumento.protocollo.dataProtocollazione))
                    list.Add(new DateProperty(TypeDocumento.DATA_PROTOCOLLO, Convert.ToDateTime(schedaDocumento.protocollo.dataProtocollazione)));

                if (schedaDocumento.protocollo.segnatura != null)
                    list.Add(new StringProperty(TypeDocumento.SEGNATURA, schedaDocumento.protocollo.segnatura));

                if (schedaDocumento.protocollo.protocolloAnnullato != null)
                {
                    if (!string.IsNullOrEmpty(schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento))
                        list.Add(new DateProperty(TypeDocumento.DATA_ANNULLAMENTO_PROTOCOLLO, Convert.ToDateTime(schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento)));

                    list.Add(new StringProperty(TypeDocumento.NOTE_ANNULLAMENTO_PROTOCOLLO, schedaDocumento.protocollo.protocolloAnnullato.autorizzazione));
                }

                // Risposta al protocollo precedente, impostazione della segnatura
                if (schedaDocumento.rispostaDocumento != null)
                    list.Add(new StringProperty(TypeDocumento.PROTOCOLLO_PRECEDENTE, schedaDocumento.rispostaDocumento.segnatura));
            }

            // Caricamento mittenti / destintatari
            fetchMittentiDestinatari(schedaDocumento, list);

            if (schedaDocumento.tipologiaAtto != null && !string.IsNullOrEmpty(schedaDocumento.tipologiaAtto.systemId) )
            {
                // Reperimento proprietà profilazione dinamica
                list.AddRange(getProfilazioneDinamicaProperties(schedaDocumento.template,
                    TypeDocumento.DYNAMIC_TYPE, TypeDocumento.DYNAMIC_FIELD_NAME,
                    TypeDocumento.DYNAMIC_FIELD_VALUE,
                    TypeDocumento.DYNAMIC_FIELD_INDEX));
            }

            if (schedaDocumento.documentoPrincipale != null)
            {
                // Il documento rappresenta un allegato ed è legato ad un documento principale
                list.Add(new StringProperty(TypeDocumento.ID_DOCUMENTO_PRINCIPALE, schedaDocumento.documentoPrincipale.docNumber));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento proprietà documentum per l'allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="infoUtente"></param>
        /// <param name="insertMode"></param>
        /// <returns></returns>
        public static Property[] getAllegatoProperties(DocsPaVO.documento.Allegato allegato, DocsPaVO.utente.InfoUtente infoUtente, bool insertMode)
        {
            // Reperimento scheda documento relativa all'allegato
            DocsPaVO.documento.SchedaDocumento schedaDocumento = DocsPaQueryHelper.getSchedaDocumentoNoSecurity(allegato.docNumber, infoUtente);

            // Aggiornamento dell'oggetto del documento
            if (schedaDocumento.oggetto != null)
                schedaDocumento.oggetto.descrizione = allegato.descrizione;

            // Reperimento delle properties per il documento
            return Dfs4DocsPa.getDocumentoProperties(schedaDocumento, infoUtente, insertMode);
        }

        private static Property makeDocsPALockCode()
        {
            return new StringProperty("a_controlling_app", APPLICATION_CODE);
        }

        /// <summary>
        /// Reperimento proprietà per l'oggetto stampa registro
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="insertMode">Se true, modalità di inserimento</param>
        /// <returns></returns>
        public static Property[] getDocumentoStampaRegistroProperties(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool insertMode)
        {
            List<Property> list = new List<Property>();

            list.Add(new StringProperty("object_name", normalizeStringPropertyValue(schedaDocumento.oggetto.descrizione, OBJECT_NAME_MAX_LENGHT)));
            list.Add(new StringProperty(TypeDocumentoStampaRegistro.DOC_NUMBER, schedaDocumento.docNumber));

            if (insertMode)
                // Impostazione del numero versione a 1 in modalità di inserimento
                list.Add(new NumberProperty(TypeDocumentoStampaRegistro.NUMERO_VERSIONE, 1));

            list.Add(makeDocsPALockCode());

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_stamparegistro" a partire da un DocNumber
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoStampaRegistroIdentityByDocNumber(string docNumber)
        {
            string qualString = ObjectTypes.DOCUMENTO_STAMPA_REGISTRO + " where " +
                TypeDocumentoStampaRegistro.DOC_NUMBER + " = '" + docNumber + "'";

            return DfsHelper.createObjectIdentityByQualification(qualString);
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_stamparegistro" a partire da un DocNumber e il numero versione
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoStampaRegistroIdentityByDocNumber(string docNumber, string version)
        {
            string qualString = string.Format("{0} (ALL) WHERE {1} = '{2}' AND {3} = '{4}'",
                    ObjectTypes.DOCUMENTO_STAMPA_REGISTRO,
                    TypeDocumentoStampaRegistro.DOC_NUMBER,
                    docNumber,
                    TypeDocumentoStampaRegistro.NUMERO_VERSIONE,
                    version);

            return DfsHelper.createObjectIdentityByQualification(qualString);

        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_stamparegistro" a partire da un DocNumber
        /// </summary>
        /// <param name="queryService"></param>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoStampaRegistroIdentityByDocNumber(IQueryService queryService, string docNumber, string version)
        {
            string qualString = string.Format("{0} (ALL) WHERE {1} = '{2}' AND {3} = '{4}'",
                        ObjectTypes.DOCUMENTO_STAMPA_REGISTRO,
                        TypeDocumentoStampaRegistro.DOC_NUMBER,
                        docNumber,
                        TypeDocumentoStampaRegistro.NUMERO_VERSIONE,
                        version);

            ObjectId objectId = new ObjectId(DfsHelper.getDctmObjectId(queryService, new Qualification(qualString)));
            ObjectIdentity identity = new ObjectIdentity(objectId, DctmConfigurations.GetRepositoryName());
            identity.ValueType = ObjectIdentityType.OBJECT_ID;

            return identity;
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_stamparegistro" a partire da un DocNumber
        /// </summary>
        /// <param name="queryService"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoStampaRegistroIdentityByDocNumber(IQueryService queryService, string docNumber)
        {
            string qualString = string.Format("{0} WHERE {1} = '{2}'",
                        ObjectTypes.DOCUMENTO_STAMPA_REGISTRO, TypeDocumentoStampaRegistro.DOC_NUMBER, docNumber);

            ObjectId objectId = new ObjectId(DfsHelper.getDctmObjectId(queryService, new Qualification(qualString)));
            ObjectIdentity identity = new ObjectIdentity(objectId, DctmConfigurations.GetRepositoryName());
            identity.ValueType = ObjectIdentityType.OBJECT_ID;

            return identity;
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_document" (currentversion) a partire da un DocNumber
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoIdentityByDocNumber(string docNumber)
        {
            string qualString = string.Empty;
            bool isStampaRegistro = (DocsPaQueryHelper.isStampaRegistro(docNumber));

            if (isStampaRegistro)
                qualString = ObjectTypes.DOCUMENTO_STAMPA_REGISTRO + " where " + TypeDocumentoStampaRegistro.DOC_NUMBER + " = '" + docNumber + "'";
            else
                qualString = ObjectTypes.DOCUMENTO + " where " + TypeDocumento.DOC_NUMBER + " = '" + docNumber + "'";

            return DfsHelper.createObjectIdentityByQualification(qualString);
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_document" a partire da un DocNumber ed una versione
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoIdentityByDocNumber(string docNumber, string version)
        {
            string qualString = string.Format("{0} (ALL) WHERE {1} = '{2}' AND {3} = '{4}'", ObjectTypes.DOCUMENTO, TypeDocumento.DOC_NUMBER, docNumber, TypeDocumento.NUMERO_VERSIONE, version);

            return DfsHelper.createObjectIdentityByQualification(qualString);
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_document" (versione specificata) a partire da un DocNumber
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoIdentityByDocNumber(IQueryService queryService, string docNumber, string version)
        {
            string qualString = string.Format("{0} (ALL) WHERE {1} = '{2}' AND {3} = '{4}'", ObjectTypes.DOCUMENTO, TypeDocumento.DOC_NUMBER, docNumber, TypeDocumento.NUMERO_VERSIONE, version);

            ObjectId objectId = new ObjectId(DfsHelper.getDctmObjectId(queryService, new Qualification(qualString)));
            ObjectIdentity identity = new ObjectIdentity(objectId, DctmConfigurations.GetRepositoryName());
            identity.ValueType = ObjectIdentityType.OBJECT_ID;

            return identity;
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_document" a partire da un DocNumber
        /// </summary>
        /// <param name="queryService"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoIdentityByDocNumber(IQueryService queryService, string docNumber)
        {
            string qualString = string.Format("{0} WHERE {1} = '{2}'", ObjectTypes.DOCUMENTO, TypeDocumento.DOC_NUMBER, docNumber);

            ObjectId objectId = new ObjectId(DfsHelper.getDctmObjectId(queryService, new Qualification(qualString)));
            ObjectIdentity identity = new ObjectIdentity(objectId, DctmConfigurations.GetRepositoryName());
            identity.ValueType = ObjectIdentityType.OBJECT_ID;

            return identity;
        }

        /// <summary>
        /// A partire dal docnumber dell'allegato, reperimento
        /// dell'objectidentity del documento principale
        /// </summary>
        /// <param name="docNumberAllegato"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoPrincipaleIdentity(string docNumberAllegato, IQueryService queryService)
        {
            ObjectIdentity identity = null;

            QueryExecution queryExecution = new QueryExecution();
            queryExecution.CacheStrategyType = CacheStrategyType.DEFAULT_CACHE_STRATEGY;

            PassthroughQuery query = new PassthroughQuery();
            query.AddRepository(DctmConfigurations.GetRepositoryName());

            string innerQueryText = string.Format("select {0} from {1} where {2} = '{3}'", 
                                    TypeDocumento.ID_DOCUMENTO_PRINCIPALE,
                                    ObjectTypes.DOCUMENTO, 
                                    TypeDocumento.DOC_NUMBER, 
                                    docNumberAllegato);

            string queryText = string.Format("select r_object_id from {0} where {1} = ({2})",
                                    ObjectTypes.DOCUMENTO,
                                    TypeDocumento.DOC_NUMBER,
                                    innerQueryText);

            query.QueryString = queryText;

            QueryResult result = queryService.Execute(query, queryExecution, null);

            if (result.DataObjects != null && result.DataObjects.Count > 0)
                identity = result.DataObjects[0].Identity;

            return identity;
        }

        /// <summary>
        /// Reperimento di tutti gli oggetti ObjectIdentity (per object_id) 
        /// relativi ai documenti contenuti in un fascicolo
        /// </summary>
        /// <param name="queryService"></param>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        public static string[] getIdDocumentiInFascicolo(IQueryService queryService, string idFascicolo)
        {
            QueryExecution queryExecution = new QueryExecution();
            queryExecution.CacheStrategyType = CacheStrategyType.DEFAULT_CACHE_STRATEGY;

            PassthroughQuery query = new PassthroughQuery();
            query.AddRepository(DctmConfigurations.GetRepositoryName());

            string innerCommandText = string.Format("select r_object_id from {0} where {1} = '{2}'",
                                    ObjectTypes.FASCICOLO, TypeFascicolo.ID_DOCSPA, idFascicolo);

            string commandText = string.Format("select {0} from {1} where any i_folder_id in ({2})",
                                    TypeDocumento.DOC_NUMBER, ObjectTypes.DOCUMENTO, innerCommandText);

            query.QueryString = commandText;

            QueryResult result = queryService.Execute(query, queryExecution, null);

            List<string> list = new List<string>();
            
            if (result.DataPackage != null)
                foreach (DataObject dataObject in result.DataPackage.DataObjects)
                    list.Add(dataObject.Properties.Get(TypeDocumento.DOC_NUMBER).GetValueAsString());

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento di tutti gli oggetti ObjectIdentity (per object_id) 
        /// relativi ai documenti contenuti in un sottofascicolo 
        /// </summary>
        /// <param name="queryService"></param>
        /// <param name="idSottoFascicolo"></param>
        /// <returns></returns>
        public static string[] getIdDocumentiInSottofascicolo(IQueryService queryService, string idSottoFascicolo)
        {
            QueryExecution queryExecution = new QueryExecution();
            queryExecution.CacheStrategyType = CacheStrategyType.DEFAULT_CACHE_STRATEGY;

            PassthroughQuery query = new PassthroughQuery();
            query.AddRepository(DctmConfigurations.GetRepositoryName());

            string innerCommandText = string.Format("select r_object_id from {0} where {1} = '{2}'",
                                    ObjectTypes.SOTTOFASCICOLO, TypeSottofascicolo.ID_DOCSPA, idSottoFascicolo);

            string commandText = string.Format("select {0} from {1} where any i_folder_id in ({2})",
                                    TypeDocumento.DOC_NUMBER, ObjectTypes.DOCUMENTO, innerCommandText);

            query.QueryString = commandText;

            QueryResult result = queryService.Execute(query, queryExecution, null);

            List<string> list = new List<string>();

            if (result.DataPackage != null)
                foreach (DataObject dataObject in result.DataPackage.DataObjects)
                    list.Add(dataObject.Properties.Get(TypeDocumento.DOC_NUMBER).GetValueAsString());

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento di tutti gli oggetti ObjectIdentity (per object_id) 
        /// di tutti gli allegati di un documento
        /// </summary>
        /// <param name="queryService"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static ObjectIdentity[] getAllegatiDocumentoIdentities(IQueryService queryService, string docNumber)
        {
            List<ObjectIdentity> identities = new List<ObjectIdentity>();

            QueryExecution queryExecution = new QueryExecution();
            queryExecution.CacheStrategyType = CacheStrategyType.DEFAULT_CACHE_STRATEGY;

            PassthroughQuery query = new PassthroughQuery();
            query.AddRepository(DctmConfigurations.GetRepositoryName());

            query.QueryString = string.Format("select r_object_id from {0} where {1} = '{2}'", ObjectTypes.DOCUMENTO, TypeDocumento.ID_DOCUMENTO_PRINCIPALE, docNumber);

            QueryResult result = queryService.Execute(query, queryExecution, null);

            if (result.DataPackage != null)
                foreach (DataObject dataObject in result.DataPackage.DataObjects)
                    identities.Add(dataObject.Identity);

            return identities.ToArray();
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_fascicolo" a partire dall'id del fascicolo
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns></returns>
        public static ObjectIdentity getFascicoloIdentityBySystemId(string idFascicolo)
        {
            string qualString = ObjectTypes.FASCICOLO + " where " + TypeFascicolo.ID_DOCSPA + " = '" + idFascicolo + "'";

            return DfsHelper.createObjectIdentityByQualification(qualString);
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_titolario" a partire da un id
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public static ObjectIdentity getTitolarioIdentity(string idTitolario)
        {
            string qualString = string.Format("{0} where {1} = '{2}'", ObjectTypes.TITOLARIO, TypeTitolario.ID_DOCSPA, idTitolario);

            return DfsHelper.createObjectIdentityByQualification(qualString);
        }

        /// <summary>
        /// Reperimento proprietà del titolario per l'oggetto "p3_titolario"
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public static Property[] getTitolarioProperties(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            List<Property> list = new List<Property>();

            list.Add(makeDocsPALockCode());

            list.Add(new StringProperty("object_name", normalizeStringPropertyValue(titolario.ToString(), OBJECT_NAME_MAX_LENGHT)));
            list.Add(new StringProperty("title", normalizeStringPropertyValue(titolario.Commento, TITLE_MAX_LENGTH)));
            list.Add(new StringProperty(TypeTitolario.STATO, Convert.ToString(titolario.Stato)));
            list.Add(new StringProperty(TypeTitolario.ID_DOCSPA, titolario.ID));

            if (!string.IsNullOrEmpty(titolario.DataAttivazione))
                list.Add(new DateProperty(TypeTitolario.DATA_APERTURA, Convert.ToDateTime(titolario.DataAttivazione)));

            if (!string.IsNullOrEmpty(titolario.DataCessazione))
                list.Add(new DateProperty(TypeTitolario.DATA_CHIUSURA, Convert.ToDateTime(titolario.DataCessazione)));

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_ndc" a partire da un id
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public static ObjectIdentity getNodoTitolarioIdentity(string idNodoTitolario)
        {
            string qualString = string.Format("{0} where {1} = '{2}'", ObjectTypes.NODO_TITOLARIO, TypeNodoTitolario.ID_DOCSPA, idNodoTitolario);

            return DfsHelper.createObjectIdentityByQualification(qualString);
        }

        /// <summary>
        /// Reperimento proprietà del nodo titolario per l'oggetto "p3_ndc"
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public static Property[] getNodoTitolarioProperties(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            List<Property> list = new List<Property>();

            list.Add(makeDocsPALockCode());

            string objectName = nodoTitolario.ToString();

            if (!string.IsNullOrEmpty(nodoTitolario.IDRegistroAssociato))
            {
                // Composizione del nome del nodo titolario, inserisce il codice registro
                // per permettere l'univocitià di più nodi con lo stesso nome ma su diversi registri
                objectName = string.Format("{0} ({1})", objectName,
                    DocsPaQueryHelper.getCodiceRegistro(nodoTitolario.IDRegistroAssociato));
            }

            list.Add(new StringProperty("object_name", normalizeStringPropertyValue(objectName, OBJECT_NAME_MAX_LENGHT)));
            list.Add(new StringProperty("title", normalizeStringPropertyValue(nodoTitolario.Descrizione, TITLE_MAX_LENGTH)));
            list.Add(new StringProperty("subject", nodoTitolario.note));

            // Reperimento del codice del registro dall'id univoco
            string codiceRegistro = string.Empty;
            if (!string.IsNullOrEmpty(nodoTitolario.IDRegistroAssociato))
                codiceRegistro = DocsPaQueryHelper.getCodiceRegistro(nodoTitolario.IDRegistroAssociato);

            list.Add(new StringProperty(TypeNodoTitolario.CODICE_REGISTRO, codiceRegistro));
            list.Add(new NumberProperty(TypeNodoTitolario.LIVELLO, System.Convert.ToInt32(nodoTitolario.Livello)));
            list.Add(new NumberProperty(TypeNodoTitolario.MESI_CONSERVAZIONE, nodoTitolario.NumeroMesiConservazione));
            list.Add(new StringProperty(TypeNodoTitolario.ID_DOCSPA, nodoTitolario.ID));

            // Reperimento dell'id classificazione per il nodo titolario
            string idClassifica = DocsPaQueryHelper.getIdFolderCType(nodoTitolario.ID);
            list.Add(new StringProperty(TypeNodoTitolario.ID_CLASSIFICA, idClassifica));

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento delle proprietà relative al fascicolo generale
        /// per il nodo titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public static Property[] getFascicoloGeneraleProperties(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            List<Property> list = new List<Property>();

            list.Add(makeDocsPALockCode());

            // ObjectName per il fascicolo generale
            list.Add(new StringProperty("object_name", "FASCICOLO GENERALE"));

            // Reperimento dell'id del fascicolo generale in docspa
            string idFascGenerale = DocsPaQueryHelper.getIdFascicoloGenerale(nodoTitolario.ID);
            
            // Impostazione, nel campo "id_class", dell'id del record di docspa di tipo "C"
            string idClassifica = DocsPaQueryHelper.getIdFolderCType(idFascGenerale);

            // Impostazione nel campo "id_docspa" dell'id del titolario di appartenenza (parent)
            list.Add(new StringProperty(TypeFascicoloGenerale.ID_DOCSPA, idFascGenerale));
            list.Add(new StringProperty(TypeFascicoloGenerale.ID_CLASSIFICA, idClassifica));

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento delle proprietà relative ad un fascicolo procedimentale
        /// </summary>
        /// <param name="fascicoloProcedimentale"></param>
        /// <returns></returns>
        public static Property[] getFascicoloProcedimentaleProperties(DocsPaVO.fascicolazione.Fascicolo fascicoloProcedimentale)
        {
            logger.Info("START");
            List<Property> list = new List<Property>();
            logger.Info("Informazioni per P3SBCLib, test con cultureInfo hardcoded");
            CultureInfo cInfo = new CultureInfo("it-IT");
            list.Add(makeDocsPALockCode());
            logger.Info(fascicoloProcedimentale.ToString());
            list.Add(new StringProperty("object_name", normalizeStringPropertyValue(fascicoloProcedimentale.ToString(), OBJECT_NAME_MAX_LENGHT)));
            logger.Info(fascicoloProcedimentale.descrizione);
            list.Add(new StringProperty("title", normalizeStringPropertyValue(fascicoloProcedimentale.descrizione, TITLE_MAX_LENGTH)));

            list.Add(new BooleanProperty(TypeFascicoloProcedimentale.CARTACEO, fascicoloProcedimentale.cartaceo));

            list.Add(new BooleanProperty(TypeFascicoloProcedimentale.PRIVATO, (fascicoloProcedimentale.privato != null && fascicoloProcedimentale.privato.Equals("1"))));
            logger.Info(fascicoloProcedimentale.apertura);
            list.Add(new DateProperty(TypeFascicoloProcedimentale.DATA_APERTURA, Convert.ToDateTime(fascicoloProcedimentale.apertura, cInfo)));
            logger.Info("Data apertura convertita.");
            if (!string.IsNullOrEmpty(fascicoloProcedimentale.chiusura))
            {
                logger.Info(fascicoloProcedimentale.chiusura);
                list.Add(new DateProperty(TypeFascicoloProcedimentale.DATA_CHIUSURA, Convert.ToDateTime(fascicoloProcedimentale.chiusura, cInfo)));
                logger.Info("Data chiusura convertita.");
            }
            list.Add(new StringProperty(TypeFascicoloProcedimentale.STATO, fascicoloProcedimentale.stato));
            list.Add(new StringProperty(TypeFascicoloProcedimentale.ID_DOCSPA, fascicoloProcedimentale.systemID));

            // Reperimento id del record di tipo "C" del fascicolo procedimentale
            string idClassifica = DocsPaQueryHelper.getIdFolderCType(fascicoloProcedimentale.systemID);
            list.Add(new StringProperty(TypeFascicoloProcedimentale.ID_CLASSIFICA, idClassifica));

            if (fascicoloProcedimentale.template != null)
            {
                // Reperimento proprietà profilazione dinamica
                list.AddRange(getProfilazioneDinamicaProperties(fascicoloProcedimentale.template, TypeFascicoloProcedimentale.DYNAMIC_TYPE, TypeFascicoloProcedimentale.DYNAMIC_FIELD_NAME, TypeFascicoloProcedimentale.DYNAMIC_FIELD_VALUE, TypeFascicoloProcedimentale.DYNAMIC_FIELD_INDEX));
            }
            logger.Info("END");
            return list.ToArray();
        }

        /// <summary>
        /// Reperimento properties relative al sottofascicolo
        /// </summary>
        /// <param name="sottofascicolo"></param>
        /// <returns></returns>
        public static Property[] getSottoFascicoloProperties(DocsPaVO.fascicolazione.Folder sottofascicolo)
        {
            List<Property> list = new List<Property>();

            list.Add(makeDocsPALockCode());

            list.Add(new StringProperty("object_name", normalizeStringPropertyValue(sottofascicolo.descrizione, OBJECT_NAME_MAX_LENGHT)));
            list.Add(new StringProperty(TypeSottofascicolo.ID_DOCSPA, sottofascicolo.systemID));
            list.Add(new StringProperty(TypeSottofascicolo.ID_FASCICOLO, sottofascicolo.idFascicolo));

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento oggetto Identity per un oggetto "p3_titolario" con stato attivo
        /// </summary>
        /// <param name="codiceAmm"></param>
        /// <returns></returns>
        public static ObjectIdentity getTitolarioAttivoIdentity(string codiceAmm)
        {
            string qualString = string.Format("{0} where folder('/{1}/{2}') and {3} = '{4}'",
                ObjectTypes.TITOLARIO, codiceAmm.ToLowerInvariant(), DocsPaAdminCabinet.FOLDER_TITOLARIO, TypeTitolario.STATO, DocsPaVO.amministrazione.OrgStatiTitolarioEnum.Attivo);

            return DfsHelper.createObjectIdentityByQualification(qualString);
        }

        /// <summary>
        /// Verifica l'esistenza di un oggetto titolario in documentum
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsTitolario(string idTitolario, IQueryService querySrvc)
        {
            return containsDocsPaObject(TypeTitolario.ID_DOCSPA, idTitolario, ObjectTypes.TITOLARIO, querySrvc);
        }

        /// <summary>
        /// Verifica l'esistenza dell'titolario attivo in documentum per l'amministrazione richiesta
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsTitolarioAttivo(string codiceAmministrazione, IQueryService querySrvc)
        {
            // Identity titolario attivo
            ObjectIdentity identity = Dfs4DocsPa.getTitolarioAttivoIdentity(codiceAmministrazione);

            string objectId = DfsHelper.getDctmObjectId(querySrvc, new Qualification(identity.GetValueAsString()));

            return (!string.IsNullOrEmpty(objectId));
        }

        /// <summary>
        /// Verifica l'esistenza di un oggetto nodo titolario in documentum
        /// </summary>
        /// <param name="idNodoTitolario"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsNodoTitolario(string idNodoTitolario, IQueryService querySrvc)
        {
            return containsDocsPaObject(TypeNodoTitolario.ID_DOCSPA, idNodoTitolario, ObjectTypes.NODO_TITOLARIO, querySrvc);
        }

        /// <summary>
        /// Verifica l'esistenza di un oggetto fascicolo generale in documentum
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsFascicoloGenerale(string idFascicolo, IQueryService querySrvc)
        {
            return containsDocsPaObject(TypeFascicoloGenerale.ID_DOCSPA, idFascicolo, ObjectTypes.FASCICOLO_GENERALE, querySrvc);
        }

        /// <summary>
        /// Verifica l'esistenza di un oggetto fascicolo procedimentale in documentum 
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsFascicoloProcedimentale(string idFascicolo, IQueryService querySrvc)
        {
            return containsDocsPaObject(TypeFascicoloProcedimentale.ID_DOCSPA, idFascicolo, ObjectTypes.FASCICOLO_PROCEDIMENTALE, querySrvc);
        }

        /// <summary>
        /// Verifica l'esistenza di un oggetto sottofascicolo in documentum
        /// </summary>
        /// <param name="idSottofascicolo"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsSottofascicolo(string idSottofascicolo, IQueryService querySrvc)
        {
            return containsDocsPaObject(TypeSottofascicolo.ID_DOCSPA, idSottofascicolo, ObjectTypes.SOTTOFASCICOLO, querySrvc);
        }

        /// <summary>
        /// Verifica l'esistenza di un documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsDocumento(string docNumber, IQueryService querySrvc)
        {
            return containsDocsPaObject(TypeDocumento.DOC_NUMBER, docNumber, ObjectTypes.DOCUMENTO, querySrvc);
        }

        /// <summary>
        /// Verifica l'esistenza di un documento di tipo stampa registro
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsDocumentoStampaRegistro(string docNumber, IQueryService querySrvc)
        {
            return containsDocsPaObject(TypeDocumentoStampaRegistro.DOC_NUMBER, docNumber, ObjectTypes.DOCUMENTO_STAMPA_REGISTRO, querySrvc);
        }

        /// <summary>
        /// Verifica l'esistenza di una versione particolare di un documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsVersion(string docNumber, string version, IQueryService querySrvc)
        {
            string qualification = string.Empty;
            string objectType = string.Empty;

            if (DocsPaQueryHelper.isStampaRegistro(docNumber))
            {
                objectType = ObjectTypes.DOCUMENTO_STAMPA_REGISTRO;

                qualification = string.Format("{0} = '{1}' AND {2} = '{3}'",
                                        TypeDocumentoStampaRegistro.DOC_NUMBER,
                                        docNumber,
                                        TypeDocumentoStampaRegistro.NUMERO_VERSIONE,
                                        version);
            }
            else
            {
                objectType = ObjectTypes.DOCUMENTO;

                qualification = string.Format("{0} = '{1}' AND {2} = '{3}'",
                        TypeDocumento.DOC_NUMBER,
                        docNumber,
                        TypeDocumento.NUMERO_VERSIONE,
                        version);
            }

            if (querySrvc == null)
            {
                // Creazione istanza "IQueryService" con impersonate da superamministratore
                querySrvc = DctmServiceFactory.GetServiceInstance<IQueryService>(DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager.ImpersonateSuperUser());
            }

            // Reperimento id documentum
            string objectId = DfsHelper.getDctmObjectId(querySrvc, objectType, qualification);

            return (!string.IsNullOrEmpty(objectId));
        }

        /// <summary>
        /// Verifica l'esistenza di un'amministrazione
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public static bool containsAmministrazione(string codiceAmministrazione)
        {
            return containsAmministrazione(codiceAmministrazione, null);
        }

        /// <summary>
        /// Verifica l'esistenza di un'amministrazione
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsAmministrazione(string codiceAmministrazione, IQueryService querySrvc)
        {
            codiceAmministrazione = DocsPaAdminCabinet.getCodiceAmministrazione(codiceAmministrazione);

            return containsDocsPaObject("object_name", codiceAmministrazione, "dm_cabinet", querySrvc);
        }

        /// <summary>
        /// Verifica l'esistenza di un utente
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsUser(string userId, IQueryService querySrvc)
        {
            userId = TypeUtente.NormalizeUserName(userId);

            return containsDocsPaObject("user_name", userId, ObjectTypes.UTENTE, querySrvc);
        }


        /// <summary>
        /// Conta il numero di gruppi di appartenenza di un utente documentum
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static int countGroupsByUserId(string userId, IQueryService querySrvc)
        {
            userId = TypeUtente.NormalizeUserName(userId);

            return countDocsPaCountGroupsByUserId(userId, querySrvc);
        }



        /// <summary>
        /// Verifica l'esistenza di un gruppo
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        public static bool containsGroup(string groupName, IQueryService querySrvc)
        {
            groupName = TypeGruppo.NormalizeGroupName(groupName);

            return containsDocsPaObject("group_name", groupName, ObjectTypes.GRUPPO, querySrvc);
        }

        /// <summary>
        /// Reperimento ObjectIdentity per un fascicolo procedimentale
        /// </summary>
        /// <param name="idDocspa"></param>
        /// <returns></returns>
        public static ObjectIdentity getFascicoloProcedimentaleIdentityById(string idDocspa)
        {
            Qualification qualification = getFascicoloProcedimentaleQualificationById(idDocspa);

            return new ObjectIdentity(qualification, DctmConfigurations.GetRepositoryName());
        }

        /// <summary>
        /// Restituisce la qualification di un oggetto di tipo FascicoloProcedimentale
        /// </summary>
        /// <param name="idDocspa"></param>
        /// <returns></returns>
        public static Qualification getFascicoloProcedimentaleQualificationById(string idDocspa)
        {
            string qry = string.Format(ObjectTypes.FASCICOLO_PROCEDIMENTALE + " where {0} = '{1}'", TypeFascicoloProcedimentale.ID_DOCSPA, idDocspa);

            return new Qualification(qry);
        }

        /// <summary>
        /// Restituisce la qualification di un oggetto di tipo FascicoloProcedimentale
        /// </summary>
        /// <param name="idDocspa"></param>
        /// <returns></returns>
        public static Qualification getFascicoloQualificationById(string idDocspa)
        {
            string tipoFascicolo = DocsPaQueryHelper.getTipoFascicolo(idDocspa);

            string qry = string.Empty;

            if (tipoFascicolo == "G")
                qry = string.Format(ObjectTypes.FASCICOLO_GENERALE + " where {0} = '{1}'", TypeFascicolo.ID_DOCSPA, idDocspa);
            else if (tipoFascicolo == "P")
                qry = string.Format(ObjectTypes.FASCICOLO_PROCEDIMENTALE + " where {0} = '{1}'", TypeFascicolo.ID_DOCSPA, idDocspa);

            return new Qualification(qry);
        }

        /// <summary>
        /// Restituzione di un ObjectIdentity per il sottofascicolo richiesto
        /// </summary>
        /// <param name="idDocpsa"></param>
        /// <returns></returns>
        public static ObjectIdentity getSottofascicoloIdentityById(string idDocpsa)
        {
            return new ObjectIdentity(getSottofascicoloQualificationById(idDocpsa), DctmConfigurations.GetRepositoryName());
        }

        /// <summary>
        /// Restituisce la qualification di un oggetto di tipo SottoFascicolo
        /// </summary>
        /// <param name="idDocspa"></param>
        /// <returns></returns>
        public static Qualification getSottofascicoloQualificationById(string idDocspa)
        {
            string qry = string.Format(ObjectTypes.SOTTOFASCICOLO + " where {0} = '{1}'", TypeSottofascicolo.ID_DOCSPA, idDocspa);
            Qualification qual = new Qualification(qry);
            return (qual);
        }

        /// <summary>
        /// Reperimento proprietà gruppo docspa
        /// </summary>
        /// <param name="group"></param>
        /// <param name="forUpdate"></param>
        /// <returns></returns>
        public static Property[] getGroupProperties(DocsPaVO.amministrazione.OrgRuolo group, bool forUpdate)
        {
            List<Property> list = new List<Property>();

            if (!forUpdate)
                list.Add(new StringProperty("group_name", TypeGruppo.NormalizeGroupName(DocsPaQueryHelper.getCodiceRuolo(group.IDCorrGlobale))));
            list.Add(new StringProperty("group_display_name", group.Codice));
            list.Add(new StringProperty("description", group.Descrizione));
            list.Add(new StringProperty("group_class", "group"));

            return list.ToArray();
        }


        /// <summary>
        /// Reperimento del ruolo e utente creatore del documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static void getOwnershipDocumento(string docNumber, out string ownerUser, out string ownerRole, IQueryService queryService)
        {
            ownerUser = string.Empty;
            ownerRole = string.Empty;

            string dql = string.Format("SELECT owner_name, {0} FROM {1} WHERE {2} = '{3}'", TypeDocumento.RUOLO_CREATORE, ObjectTypes.DOCUMENTO, TypeDocumento.DOC_NUMBER, docNumber);

            QueryResult result = DfsHelper.executePassThrough(queryService, dql);

            if (result.DataObjects.Count == 1)
            {
                ownerUser = result.DataObjects[0].Properties.Get("owner_name").GetValueAsString();
                ownerRole = result.DataObjects[0].Properties.Get(TypeDocumento.RUOLO_CREATORE).GetValueAsString();
            }
        }

        /// <summary>
        /// Reperimento dell'utente creatore del fascicolo
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static void getOwnershipFascicolo(string idFascicolo, out string ownerUser, IQueryService queryService)
        {
            ownerUser = string.Empty;

            string dql = string.Format("SELECT owner_name FROM {0} WHERE {1} = '{2}'", ObjectTypes.FASCICOLO, TypeFascicolo.ID_DOCSPA, idFascicolo);

            QueryResult result = DfsHelper.executePassThrough(queryService, dql);

            if (result.DataObjects.Count == 1)
            {
                ownerUser = result.DataObjects[0].Properties.Get("owner_name").GetValueAsString();
            }
        }

        /// <summary>
        /// Reperimento proprietà utente
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Property[] getUserProperties(DocsPaVO.amministrazione.OrgUtente user)
        {
            List<Property> list = new List<Property>();

            string userName = TypeUtente.getUserName(user);
            list.Add(new StringProperty("user_name", userName));
            list.Add(new StringProperty("user_os_name", userName));

            if (!string.IsNullOrEmpty(user.Password))
            {
                list.Add(new StringProperty("user_source", "inline password"));
                list.Add(new StringProperty("user_password", user.Password));
            }

            list.Add(new StringProperty("user_address", user.Email));

            if (user.Abilitato == "1")
                list.Add(new StringProperty("user_state", "0")); // Utente abilitato
            else
                list.Add(new StringProperty("user_state", "1")); // Utente non abilitato

            //eventuale attribuzione privilegi di amministratore
             if (!string.IsNullOrEmpty(user.Amministratore) && Int16.Parse(user.Amministratore) > 0)
                list.Add(new NumberProperty("user_privileges", 8));
            else
                list.Add(new NumberProperty("user_privileges", 0));

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ObjectIdentity getUserIdentityByName(string userId)
        {
            Qualification qual = new Qualification(string.Format(ObjectTypes.UTENTE + " where lower(user_name) = lower('{0}')", userId));

            return DfsHelper.createObjectIdentityByQualification(qual);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static ObjectIdentity getUserIdentityByName(string userId, IQueryService queryService)
        {
            Qualification qual = new Qualification(string.Format(ObjectTypes.UTENTE + " where lower(user_name) = lower('{0}')", userId));

            string id = DfsHelper.getDctmObjectId(queryService, qual);

            ObjectIdentity identity = null;

            if (!string.IsNullOrEmpty(id))
                identity = DfsHelper.createObjectIdentityObjId(id);

            return identity;
        }

        /// <summary>
        /// Reperimento oggetto identity relativo ad un gruppo
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static ObjectIdentity getGroupIdentityByName(string groupName)
        {
            Qualification qual = new Qualification(string.Format(ObjectTypes.GRUPPO + " where lower(group_name) = lower('{0}')", groupName));

            return DfsHelper.createObjectIdentityByQualification(qual);
        }

        /// <summary>
        /// Reperimento oggetto identity relativo ad un gruppo
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static ObjectIdentity getGroupIdentityByName(string groupName, IQueryService queryService)
        {
            Qualification qual = new Qualification(string.Format(ObjectTypes.GRUPPO + " where lower(group_name) = lower('{0}')", groupName));

            string id = DfsHelper.getDctmObjectId(queryService, qual);

            ObjectIdentity identity = null;

            if (!string.IsNullOrEmpty(id))
                identity = DfsHelper.createObjectIdentityObjId(id);

            return identity;
        }

        /// <summary>
        /// Reperimento di tutti gli utenti facenti parte del gruppo di sistema dell'amministrazione
        /// </summary>
        /// <param name="codAmm"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static string[] getUsersSystemGroup(string codAmm, IQueryService queryService)
        {
            // Reperimento nome gruppo di sistema
            string systemGroup = TypeGruppo.GetGroupNameForAmministrazione(codAmm);

            return getUsersGroup(systemGroup, queryService);
        }

        /// <summary>
        /// Reperimento di tutti gli utenti facenti parte di un gruppo
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static string[] getUsersGroup(string groupName, IQueryService queryService)
        {
            List<string> list = new List<string>();

            string dql = string.Format("SELECT users_names FROM dm_group WHERE lower(group_name) = lower('{0}')", groupName);

            QueryResult result = DfsHelper.executePassThrough(queryService, dql);

            foreach (DataObject item in result.DataObjects)
            {
                StringArrayProperty itemArray = (StringArrayProperty)item.Properties.Get("users_names");

                if (itemArray.GetValues().Length > 0)
                    list.Add(itemArray.GetValues()[0]);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento oggetto identity relativo al cabinet predefinito per l'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ObjectIdentity getUserHomeFolderIdentity(string userId)
        {
            string qualification = string.Format("dm_cabinet WHERE lower(object_name) = lower('{0}')", TypeUtente.getHomeFolderName(userId));

            return DfsHelper.createObjectIdentityByQualification(qualification);
        }

        /// <summary>
        /// Reperimento oggetto identity relativo al gruppo di sistema del cabinet / amministrazione
        /// </summary>
        /// <param name="codAmm"></param>
        /// <returns></returns>
        public static ObjectIdentity getSystemGroupIdentity(string codAmm)
        {
            string qualification = string.Format("{0} WHERE lower(group_name) = lower('{1}')", ObjectTypes.GRUPPO, TypeGruppo.GetGroupNameForAmministrazione(codAmm));

            return DfsHelper.createObjectIdentityByQualification(qualification);
        }

        /// <summary>
        /// Reperimento oggetto identity relativo al gruppo di sistema del cabinet / amministrazione
        /// </summary>
        /// <param name="codAmm"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static ObjectIdentity getSystemGroupIdentity(string codAmm, IQueryService queryService)
        {
            string id = DfsHelper.getDctmObjectId(queryService, ObjectTypes.GRUPPO, string.Format("lower(group_name) = lower('{0}')", TypeGruppo.GetGroupNameForAmministrazione(codAmm)));

            ObjectIdentity identity = null;

            if (!string.IsNullOrEmpty(id))
                identity = DfsHelper.createObjectIdentityObjId(id);

            return identity;
        }

        /// <summary>
        /// Reperimento oggetto identity relativo al gruppo di sistema degli amministratori del cabinet / amministrazione
        /// </summary>
        /// <param name="codAmm"></param>
        /// <returns></returns>
        public static ObjectIdentity getAmmSystemGroupIdentity(string codAmm)
        {
            string qualification = string.Format("{0} WHERE lower(group_name) = lower('{1}')", ObjectTypes.GRUPPO, TypeGruppo.GetGroupNameForSysAdminAmministrazione(codAmm));

            return DfsHelper.createObjectIdentityByQualification(qualification);
        }

        /// <summary>
        /// Reperimento oggetto identity relativo al gruppo di sistema degli amministratori del cabinet / amministrazione
        /// </summary>
        /// <param name="codAmm"></param>
        /// <param name="queryService"></param>
        /// <returns></returns>
        public static ObjectIdentity getAmmSystemGroupIdentity(string codAmm, IQueryService queryService)
        {
            string id = DfsHelper.getDctmObjectId(queryService, ObjectTypes.GRUPPO, string.Format("lower(group_name) = lower('{0}')", TypeGruppo.GetGroupNameForSysAdminAmministrazione(codAmm)));

            ObjectIdentity identity = null;

            if (!string.IsNullOrEmpty(id))
                identity = DfsHelper.createObjectIdentityObjId(id);

            return identity;
        }

        /// <summary>
        /// Reperimento oggetto identity relativo al cabinet / amministrazione
        /// </summary>
        /// <param name="codAmm"></param>
        /// <returns></returns>
        public static ObjectIdentity getCabinetIdentity(string codAmm)
        {
            // Reperimento oggetto Identity relativamente al cabinet da rimuovere
            string qualification = string.Format("{0} where lower(object_name) = lower('{1}')", "dm_cabinet", codAmm);

            return DfsHelper.createObjectIdentityByQualification(qualification);
        }

        /// <summary>
        /// Creazione o reperimento identity per il folder in cui inserire il documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ObjectIdentity getDocumentoParentFolderIdentity(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, IObjectService objectService)
        {
            string documentFolder = DocsPaAdminCabinet.getPathDocumento(infoUtente, schedaDocumento.dataCreazione);

            return objectService.CreatePath(new ObjectPath(documentFolder), DctmConfigurations.GetRepositoryName());
        }

        /// <summary>
        /// Creazione o reperimento identity per il folder in cui inserire il documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ObjectIdentity getStampaRegistroParentFolderIdentity(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, IObjectService objectService)
        {
            // Creazione oggetto Identity parent e oggetto relationship,
            // il documento appena creato deve essere inserito nel folder dei documenti stampa registro
            string documentFolder = DocsPaAdminCabinet.getPathStampaRegistro(infoUtente, schedaDocumento.registro.systemId);

            return objectService.CreatePath(new ObjectPath(documentFolder), DctmConfigurations.GetRepositoryName());
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Normalizzazione valore per una proprietà stringa
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLenght"></param>
        /// <returns></returns>
        private static string normalizeStringPropertyValue(string value, int maxLenght)
        {
            const int MAX_LENGHT_DEFUALT = -1;

            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
                value = value.Replace("/", string.Empty);
                value = value.Replace(System.Environment.NewLine, string.Empty);
                value = value.Replace("\n", string.Empty);

                if (maxLenght != MAX_LENGHT_DEFUALT && value.Length >= maxLenght)
                {
                    value = value.Substring(0, maxLenght - 1);
                }
            }

            return value;
        }

        /// <summary>
        /// Verifica l'esistenza di un oggetto docspa in documentum
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="idDocspa"></param>
        /// <param name="objectType"></param>
        /// <param name="querySrvc"></param>
        /// <returns></returns>
        private static bool containsDocsPaObject(string fieldName, string idDocspa, string objectType, IQueryService querySrvc)
        {
            string qualification = string.Format("{0} = '{1}'", fieldName, idDocspa);

            if (querySrvc == null)
            {
                // Creazione istanza "IQueryService" con impersonate da superamministratore
                querySrvc = DctmServiceFactory.GetServiceInstance<IQueryService>(DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager.ImpersonateSuperUser());
            }

            // Reperimento id documentum
            string objectId = DfsHelper.getDctmObjectId(querySrvc, objectType, qualification);

            return (!string.IsNullOrEmpty(objectId));
        }

        /// <summary>
        /// Conta il numero di gruppi di appartenenza di un utente in documentum
        /// </summary>
        private static int countDocsPaCountGroupsByUserId(string userId, IQueryService querySrvc)
        {
            if (querySrvc == null)
            {
                // Creazione istanza "IQueryService" con impersonate da superamministratore
                querySrvc = DctmServiceFactory.GetServiceInstance<IQueryService>(DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager.ImpersonateSuperUser());
            }

            // Reperimento id documentum
            int counter = DfsHelper.getDctmCountGroupsByUserId(querySrvc, userId);

            return counter;
        }



        /// <summary>
        /// Caricamento nella lista delle proprietà documentum
        /// della descrizione dei mittenti / destinatari del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="list"></param>        
        private static void fetchMittentiDestinatari(DocsPaVO.documento.SchedaDocumento schedaDocumento, List<Property> list)
        {
            if (schedaDocumento.tipoProto.Equals("A"))
            {
                // Se il documento è in ingresso, viene reperito solo il mittente
                DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;

                if (pe.mittente != null)
                    list.Add(new StringProperty(TypeDocumento.MITTENTE, pe.mittente.ToString()));
            }
            else if (schedaDocumento.tipoProto.Equals("P"))
            {
                // Se il documento è in ingresso, vengono reperiti sia il mittente che i destinatari (anche per conoscenza)
                DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                if (pu.mittente != null)
                    list.Add(new StringProperty(TypeDocumento.MITTENTE, pu.mittente.ToString()));

                if (pu.destinatari != null)
                {
                    StringArrayProperty propertyList = new StringArrayProperty();
                    propertyList.Name = TypeDocumento.DESTINATARI;

                    foreach (DocsPaVO.utente.Corrispondente item in pu.destinatari)
                        propertyList.AddAppendPropertySequence(truncate(item.ToString(),239));

                    list.Add(propertyList);
                }

                if (pu.destinatariConoscenza != null)
                {
                    StringArrayProperty propertyList = new StringArrayProperty();
                    propertyList.Name = TypeDocumento.DESTINATARI_CC;

                    foreach (DocsPaVO.utente.Corrispondente item in pu.destinatariConoscenza)
                        propertyList.AddAppendPropertySequence(truncate(item.ToString(),239));

                    list.Add(propertyList);
                }
            }
        }

        /// <summary>
        /// Reperimento proprietà per profilazione dinamica
        /// </summary>
        /// <param name="profilo"></param>
        /// <param name="dynType"></param>
        /// <param name="dynFieldName"></param>
        /// <param name="dynFieldValue"></param>
        /// <param name="dynValueIndex"></param>
        /// <returns></returns>
        private static Property[] getProfilazioneDinamicaProperties(
                    DocsPaVO.ProfilazioneDinamica.Templates profilo,
                    string dynType, string dynFieldName,
                    string dynFieldValue, string dynValueIndex)
        {
            List<Property> list = new List<Property>();

            // Nome tipologia fascicolo
            list.Add(new StringProperty(dynType, profilo.DESCRIZIONE));

            List<string> names = new List<string>();
            List<string> values = new List<string>();
            List<int> indexes = new List<int>();

            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom item in profilo.ELENCO_OGGETTI)
            {
                if (item.VALORI_SELEZIONATI != null && item.VALORI_SELEZIONATI.Count > 0)
                {
                    // Campo multivalue
                    for (int i = 0; i < item.VALORI_SELEZIONATI.Count; i++)
                    {
                        string singleValue = string.Empty;
                        if (item.VALORI_SELEZIONATI[i] != null)
                            singleValue = item.VALORI_SELEZIONATI[i].ToString();

                        indexes.Add(i);
                        names.Add(item.DESCRIZIONE);
                        values.Add(truncate(singleValue,249));
                    }
                }
                else
                {
                    // Campo singlevalue
                    names.Add(item.DESCRIZIONE);
                    values.Add(!string.IsNullOrEmpty(item.VALORE_DATABASE) ? truncate(item.VALORE_DATABASE,249) : string.Empty);
                    indexes.Add(0);
                }
            }

            list.Add(new StringArrayProperty(dynFieldName, names.ToArray()));
            list.Add(new StringArrayProperty(dynFieldValue, values.ToArray()));
            list.Add(new NumberArrayProperty(dynValueIndex, indexes.ToArray()));

            return list.ToArray();
        }

        static string truncate(string inString, int len)
        {
            if (inString.Length > len)
                return inString.Remove(len);
            else
                return inString;
        }

        /// <summary>
        /// Creazione di un oggetto AclDefinition 
        /// con l'associazione di una delete permission verso:
        ///     - l'utente "dm_owner"
        ///     - il gruppo degli utenti amministratori dell'amministrazione 
        /// </summary>
        /// <param name="idDocsPa"></param>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        private static CustomServices.AclDefinition getAclWithBasicPermit(string idDocsPa, string objectType, string codiceAmministrazione)
        {
            CustomServices.AclDefinition aclData = AclHelper.getAclDefinition(codiceAmministrazione, idDocsPa, objectType);

            // Entry per "dm_owner"
            List<CustomServices.AclEntry> entries = new List<CustomServices.AclEntry>();

            AclHelper.addBasicPermit(entries, "dm_owner", Permission.DELETE);

            // Entry per gli amministratori dell'amministrazione
            string gruppoAmministratore = DocsPaObjectTypes.TypeGruppo.GetGroupNameForSysAdminAmministrazione(codiceAmministrazione);

            AclHelper.addBasicPermit(entries, gruppoAmministratore, Permission.DELETE);

            aclData.entries = entries.ToArray();

            return aclData;
        }

        #endregion
    }
}
