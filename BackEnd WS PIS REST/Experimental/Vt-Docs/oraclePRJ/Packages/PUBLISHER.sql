--------------------------------------------------------
--  DDL for Package PUBLISHER
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE "ITCOLL_6GIU12"."PUBLISHER" AS

    TYPE T_SUB_CURSOR IS REF CURSOR;

    -- Avvio del servizio di pubblicazione
    PROCEDURE StartService(p_Id NUMBER, 
                            p_StartDate DATE,
                            p_MachineName NVARCHAR2,
                            p_PublisherServiceUrl NVARCHAR2);
                            
    -- Avvio del servizio di pubblicazione
    PROCEDURE StopService(p_Id NUMBER,
                            p_EndDate DATE);                            
    
    -- Reperimento log di pubblicazione
    PROCEDURE GetLogs(p_IdAdmin NUMBER,
                    p_FromLogId NUMBER,
                    p_ObjectType NVARCHAR2,
                    p_ObjectTemplateName NVARCHAR2,
                    p_EventName NVARCHAR2,
                    p_res_cursor OUT T_SUB_CURSOR);
    
    -- Reperimento di tutte le istanze di pubblicazione
    PROCEDURE GetPublishInstances(p_res_cursor OUT T_SUB_CURSOR);
    
    -- Reperimento di tutte le istanze di pubblicazione per un'amministrazione 
    PROCEDURE GetAdminPublishInstances(p_IdAdmin NUMBER, p_res_cursor OUT T_SUB_CURSOR);

    -- Reperimento dei dati di un'istanza di pubblicazione 
    PROCEDURE GetPublishInstance(p_Id NUMBER, p_res_cursor OUT T_SUB_CURSOR);

    -- Reperimento degli eventi monitorati da un'istanza di pubblicazione
    PROCEDURE GetPublishInstanceEvents(p_IdInstance NUMBER, p_res_cursor OUT T_SUB_CURSOR);

    -- Reperimento di un evento monitorato da un'istanza di pubblicazione
    PROCEDURE GetPublishInstanceEvent(p_Id NUMBER, p_res_cursor OUT T_SUB_CURSOR);

    -- Aggiornamento dati esecuzione istanza 
    PROCEDURE UpdateExecutionState(p_IdInstance NUMBER,
                                    p_ExecutionCount INTEGER,
                                    p_PublishedObjects INTEGER,
                                    p_TotalExecutionCount INTEGER,
                                    p_TotalPublishedObjects INTEGER,                                    
                                    p_LastExecutionDate DATE,
                                    p_StartLogDate DATE,
                                    p_LastLogId NUMBER);
                                    
    -- Inserimento di un'istanza di pubblicazione                                    
    PROCEDURE InsertPublishInstance(p_Name NVARCHAR2, 
                        p_IdAdmin NUMBER,
                        p_SubscriberServiceUrl NVARCHAR2,
                        p_ExecutionType NVARCHAR2,
                        p_ExecutionTicks NVARCHAR2,
                        p_LastExecutionDate DATE,
                        p_ExecutionCount INTEGER,
                        p_PublishedObjects INTEGER,
                        p_TotalExecutionCount INTEGER,
                        p_TotalPublishedObjects INTEGER,                                    
                        p_StartLogDate DATE,
                        p_Id OUT NUMBER,
                        p_LastLogId OUT NUMBER);
                                            

    -- Aggiornamento di un'istanza di pubblicazione                                    
    PROCEDURE UpdatePublishInstance(
                        p_Id NUMBER, 
                        p_Name NVARCHAR2, 
                        p_SubscriberServiceUrl NVARCHAR2,
                        p_ExecutionType NVARCHAR2,
                        p_ExecutionTicks NVARCHAR2,
                        p_StartLogDate DATE,
                        p_LastLogId OUT NUMBER);
                        
    -- Rimozione di tutti gli eventi associati ad un'istanza di pubblicazione                        
    PROCEDURE ClearInstanceEvents(p_Id NUMBER);                        
                  
    -- Inserimento di un evento nell'istanza di pubblicazione
    PROCEDURE InsertInstanceEvent(p_InstanceId NUMBER,
                                    p_EventName NVARCHAR2,
                                    p_ObjectType NVARCHAR2,
                                    p_ObjectTemplateName NVARCHAR2,
                                    p_DataMapperFullClass NVARCHAR2,
                                    p_LoadFileIfDocType CHAR,
                                    p_Id OUT NUMBER);

    -- Rimozione dell'evento
    PROCEDURE RemoveInstanceEvent(p_Id NUMBER);
    
    -- Aggiornamento evento
    PROCEDURE UpdateInstanceEvent(p_Id NUMBER,
                                  p_EventName NVARCHAR2,
                                  p_ObjectType NVARCHAR2,
                                  p_ObjectTemplateName NVARCHAR2,
                                  p_DataMapperFullClass NVARCHAR2,
                                  p_LoadFileIfDocType CHAR);
    
    -- Rimozione di un'istanza di pubblicazione            
    PROCEDURE RemovePublishInstanceEvents(p_Id NUMBER);
    
    -- Reperimento degli errori verificatisi nell'istanza di pubblicazione
    PROCEDURE GetInstanceErrors(p_InstanceId NUMBER, p_res_cursor OUT T_SUB_CURSOR);
              
    -- Inserimento di un errore verificatisi nell'istanza di pubblicazione
    PROCEDURE InsertInstanceError(p_InstanceId NUMBER,
                                    p_ErrorCode NVARCHAR2,
                                    p_ErrorDescription NVARCHAR2,
                                    p_ErrorStack NVARCHAR2,
                                    p_ErrorDate DATE,
                                    p_Id OUT NUMBER);
    
END PUBLISHER; 

/


--------------------------------------------------------
--  DDL for Package Body PUBLISHER
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY "ITCOLL_6GIU12"."PUBLISHER" AS
   
    -- Avvio del servizio di pubblicazione
    PROCEDURE StartService(p_Id NUMBER, 
                            p_StartDate DATE,
                            p_MachineName NVARCHAR2,
                            p_PublisherServiceUrl NVARCHAR2)
        IS
    BEGIN
        UPDATE  PUBLISHER_INSTANCES
        SET     EXECUTIONCOUNT = 0, -- Ogni volta che avvia il servizio, azzera i contatori
                PUBLISHEDOBJECTS = 0,
                STARTEXECUTIONDATE = p_StartDate,
                ENDEXECUTIONDATE = NULL,
                MACHINENAME = p_MachineName,
                PUBLISHERSERVICEURL = p_PublisherServiceUrl
        WHERE ID = p_Id;
    
    END;                            
                            
    -- Stop del servizio di pubblicazione
    PROCEDURE StopService(p_Id NUMBER,
                            p_EndDate DATE)
        IS
    BEGIN
        UPDATE PUBLISHER_INSTANCES
        SET STARTEXECUTIONDATE = NULL,
            ENDEXECUTIONDATE = p_EndDate,
            MACHINENAME = NULL,
            PUBLISHERSERVICEURL = NULL
        WHERE ID = p_Id;
    END;
 
    -- Reperimento istanze di pubblicazione
    -- Reperimento log di pubblicazione
    PROCEDURE GetLogs(p_IdAdmin NUMBER,
                    p_FromLogId NUMBER,
                    p_ObjectType NVARCHAR2,
                    p_ObjectTemplateName NVARCHAR2,
                    p_EventName NVARCHAR2,
                    p_res_cursor OUT T_SUB_CURSOR)
        IS
    BEGIN
        IF (p_ObjectTemplateName IS NULL OR p_ObjectTemplateName = '') THEN    
            OPEN p_res_cursor FOR
                SELECT   L.SYSTEM_ID AS ID,
                         L.ID_AMM AS ID_ADMIN,
                         L.ID_PEOPLE_OPERATORE AS ID_USER,
                         L.USERID_OPERATORE AS USER_NAME,
                         L.ID_GRUPPO_OPERATORE AS ID_ROLE,
                         G.GROUP_ID AS ROLE_CODE,
                         G.GROUP_NAME AS ROLE_DESCRIPTION,
                         L.DTA_AZIONE AS EVENT_DATE,
                         L.VAR_OGGETTO AS OBJECT_TYPE,
                         L.ID_OGGETTO AS ID_OBJECT,
                         L.VAR_DESC_OGGETTO AS OBJECT_DESCRIPTION,   
                         L.VAR_COD_AZIONE AS EVENT_CODE,
                         L.VAR_DESC_AZIONE AS EVENT_DESCRIPTION
                    FROM DPA_LOG L
                         INNER JOIN PEOPLE P ON L.ID_PEOPLE_OPERATORE = P.SYSTEM_ID
                         INNER JOIN GROUPS G ON L.ID_GRUPPO_OPERATORE = G.SYSTEM_ID
                   WHERE L.CHA_ESITO = '1'
                     AND L.ID_AMM = p_IdAdmin
                     AND L.SYSTEM_ID > NVL(p_FromLogId, 0)
                     AND UPPER(L.VAR_OGGETTO) = UPPER(p_ObjectType)
                     AND UPPER(L.VAR_COD_AZIONE) = UPPER(p_EventName)
                ORDER BY L.SYSTEM_ID ASC;
                
        ELSE
            
            IF (UPPER(p_ObjectType) = 'DOCUMENTO') THEN
                 OPEN p_res_cursor FOR
                 SELECT   L.SYSTEM_ID AS ID,
                         L.ID_AMM AS ID_ADMIN,
                         L.ID_PEOPLE_OPERATORE AS ID_USER,
                         L.USERID_OPERATORE AS USER_NAME,
                         L.ID_GRUPPO_OPERATORE AS ID_ROLE,
                         G.GROUP_ID AS ROLE_CODE,
                         G.GROUP_NAME AS ROLE_DESCRIPTION,
                         L.DTA_AZIONE AS EVENT_DATE,
                         L.VAR_OGGETTO AS OBJECT_TYPE,
                         L.ID_OGGETTO AS ID_OBJECT,
                         L.VAR_DESC_OGGETTO AS OBJECT_DESCRIPTION,   
                         L.VAR_COD_AZIONE AS EVENT_CODE,
                         L.VAR_DESC_AZIONE AS EVENT_DESCRIPTION
                    FROM DPA_LOG L
                         INNER JOIN PEOPLE P ON L.ID_PEOPLE_OPERATORE = P.SYSTEM_ID
                         INNER JOIN GROUPS G ON L.ID_GRUPPO_OPERATORE = G.SYSTEM_ID
                         INNER JOIN PROFILE PR ON L.ID_OGGETTO = PR.SYSTEM_ID
                         INNER JOIN DPA_TIPO_ATTO TA ON PR.ID_TIPO_ATTO = TA.SYSTEM_ID
                   WHERE L.CHA_ESITO = '1'
                      AND L.ID_AMM = p_IdAdmin
                      AND L.SYSTEM_ID > NVL(p_FromLogId, 0)
                     AND UPPER(L.VAR_OGGETTO) = UPPER(p_ObjectType)
                     AND UPPER(L.VAR_COD_AZIONE) = UPPER(p_EventName)
                     AND UPPER(TA.VAR_DESC_ATTO) = UPPER(p_ObjectTemplateName) -- Query per template documento
                ORDER BY L.SYSTEM_ID ASC;           
                
            ELSIF (UPPER(p_ObjectType) = 'FASCICOLO') THEN
                OPEN p_res_cursor FOR
                SELECT   L.SYSTEM_ID AS ID,
                         L.ID_AMM AS ID_ADMIN,
                         L.ID_PEOPLE_OPERATORE AS ID_USER,
                         L.USERID_OPERATORE AS USER_NAME,
                         L.ID_GRUPPO_OPERATORE AS ID_ROLE,
                         G.GROUP_ID AS ROLE_CODE,
                         G.GROUP_NAME AS ROLE_DESCRIPTION,
                         L.DTA_AZIONE AS EVENT_DATE,
                         L.VAR_OGGETTO AS OBJECT_TYPE,
                         L.ID_OGGETTO AS ID_OBJECT,
                         L.VAR_DESC_OGGETTO AS OBJECT_DESCRIPTION,   
                         L.VAR_COD_AZIONE AS EVENT_CODE,
                         L.VAR_DESC_AZIONE AS EVENT_DESCRIPTION
                    FROM DPA_LOG L
                         INNER JOIN PEOPLE P ON L.ID_PEOPLE_OPERATORE = P.SYSTEM_ID
                         INNER JOIN GROUPS G ON L.ID_GRUPPO_OPERATORE = G.SYSTEM_ID
                         INNER JOIN PROJECT PJ ON L.ID_OGGETTO = PJ.SYSTEM_ID
                         INNER JOIN DPA_TIPO_FASC PT ON PJ.ID_TIPO_FASC = PT.SYSTEM_ID
                   WHERE L.CHA_ESITO = '1'
                      AND L.ID_AMM = p_IdAdmin
                      AND L.SYSTEM_ID > NVL(p_FromLogId, 0)
                     AND UPPER(L.VAR_OGGETTO) = UPPER(p_ObjectType)
                     AND UPPER(L.VAR_COD_AZIONE) = UPPER(p_EventName)
                     AND UPPER(PT.VAR_DESC_FASC) = UPPER(p_ObjectTemplateName) -- Query per template fascicolo
                ORDER BY L.SYSTEM_ID ASC;
            
            
            END IF;
                        
        END IF;            
          
    END GetLogs;


    -- Reperimento di tutte le istanze di pubblicazione
    PROCEDURE GetPublishInstances(p_res_cursor OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN p_res_cursor FOR
        SELECT *
        FROM PUBLISHER_INSTANCES
        ORDER BY ID ASC;
        
    END GetPublishInstances;

    -- Reperimento di tutte le istanze di pubblicazione per un'amministrazione 
    PROCEDURE GetAdminPublishInstances(p_IdAdmin NUMBER, p_res_cursor OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN p_res_cursor FOR
        SELECT *
        FROM PUBLISHER_INSTANCES
        WHERE IDADMIN = p_IdAdmin
        ORDER BY ID ASC;
    
    END GetAdminPublishInstances;


    -- Reperimento dei dati di un'istanza di pubblicazione 
    PROCEDURE GetPublishInstance(p_Id NUMBER, p_res_cursor OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN p_res_cursor FOR
        SELECT *
        FROM PUBLISHER_INSTANCES
        WHERE ID = p_Id;
    
    END GetPublishInstance;

    -- Reperimento degli eventi monitorati da un'istanza di pubblicazione
    PROCEDURE GetPublishInstanceEvents(p_IdInstance NUMBER, p_res_cursor OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN p_res_cursor FOR
        SELECT *
        FROM PUBLISHER_EVENTS
        WHERE PUBLISHINSTANCEID = p_IdInstance;
    
    END GetPublishInstanceEvents;

    -- Reperimento di un evento monitorato da un'istanza di pubblicazione
    PROCEDURE GetPublishInstanceEvent(p_Id NUMBER, p_res_cursor OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN p_res_cursor FOR
        SELECT *
        FROM PUBLISHER_EVENTS
        WHERE ID = p_Id;
    END GetPublishInstanceEvent; 

    -- Aggiornamento dati esecuzione istanza 
    PROCEDURE UpdateExecutionState(p_IdInstance NUMBER,
                                    p_ExecutionCount INTEGER,
                                    p_PublishedObjects INTEGER,
                                    p_TotalExecutionCount INTEGER,
                                    p_TotalPublishedObjects INTEGER,                                    
                                    p_LastExecutionDate DATE,
                                    p_StartLogDate DATE,
                                    p_LastLogId NUMBER)
    IS
    BEGIN
        UPDATE PUBLISHER_INSTANCES
        SET EXECUTIONCOUNT = p_ExecutionCount,
            PUBLISHEDOBJECTS = p_PublishedObjects,
            TOTALEXECUTIONCOUNT = p_TotalExecutionCount,
            TOTALPUBLISHEDOBJECTS = p_TotalPublishedObjects,
            LASTEXECUTIONDATE = p_LastExecutionDate,
            STARTLOGDATE = p_StartLogDate,
            LASTLOGID = p_LastLogId
        WHERE ID = p_IdInstance;
        
    END UpdateExecutionState; 
    
    
    -- Inserimento di un'istanza di pubblicazione                                    
    PROCEDURE InsertPublishInstance(p_Name NVARCHAR2, 
                        p_IdAdmin NUMBER,
                        p_SubscriberServiceUrl NVARCHAR2,
                        p_ExecutionType NVARCHAR2,
                        p_ExecutionTicks NVARCHAR2,
                        p_LastExecutionDate DATE,
                        p_ExecutionCount INTEGER,
                        p_PublishedObjects INTEGER,
                        p_TotalExecutionCount INTEGER,
                        p_TotalPublishedObjects INTEGER,                                    
                        p_StartLogDate DATE,
                        p_Id OUT NUMBER,
                        p_LastLogId OUT NUMBER)
    AS
    
    BEGIN
        BEGIN
            SELECT SYSTEM_ID INTO p_LastLogId 
            FROM 
            (
                SELECT SYSTEM_ID
                FROM DPA_LOG
                WHERE DTA_AZIONE >= p_StartLogDate
                ORDER BY SYSTEM_ID ASC
            )
            WHERE ROWNUM = 1;
        EXCEPTION
            WHEN OTHERS THEN
                SELECT SYSTEM_ID INTO p_LastLogId 
                FROM 
                (
                    SELECT SYSTEM_ID
                    FROM DPA_LOG
                    ORDER BY SYSTEM_ID DESC
                )
                WHERE ROWNUM = 1;
                  
        END;            
    
        SELECT SEQ_PUBLISHER.NEXTVAL INTO p_Id FROM dual;

        INSERT INTO PUBLISHER_INSTANCES
        (
         ID,
         INSTANCENAME,
         IDADMIN,
         SUBSCRIBERSERVICEURL,
         EXECUTIONTYPE,
         EXECUTIONTICKS,
         LASTEXECUTIONDATE,
         EXECUTIONCOUNT,
         PUBLISHEDOBJECTS,
         TOTALEXECUTIONCOUNT,
         TOTALPUBLISHEDOBJECTS,
         LASTLOGID,
         STARTLOGDATE
        )
        VALUES
        (
         p_Id,
         p_Name,
         p_IdAdmin,
         p_SubscriberServiceUrl,
         p_ExecutionType,
         p_ExecutionTicks,
         p_LastExecutionDate,
         p_ExecutionCount,
         p_PublishedObjects,
         p_TotalExecutionCount,
         p_TotalPublishedObjects,
         p_LastLogId,
         p_StartLogDate
        );
            
    END InsertPublishInstance;              
    
    
    -- Aggiornamento di un'istanza di pubblicazione                                    
    PROCEDURE UpdatePublishInstance(
                        p_Id NUMBER,
                        p_Name NVARCHAR2, 
                        p_SubscriberServiceUrl NVARCHAR2,
                        p_ExecutionType NVARCHAR2,
                        p_ExecutionTicks NVARCHAR2,
                        p_StartLogDate DATE,
                        p_LastLogId OUT NUMBER)
    IS
    BEGIN
        BEGIN
            SELECT SYSTEM_ID INTO p_LastLogId 
            FROM 
            (
                SELECT SYSTEM_ID
                FROM DPA_LOG
                WHERE DTA_AZIONE >= p_StartLogDate
                ORDER BY SYSTEM_ID ASC
            )
            WHERE ROWNUM = 1;
        EXCEPTION
            WHEN OTHERS THEN
                SELECT SYSTEM_ID INTO p_LastLogId 
                FROM 
                (
                    SELECT SYSTEM_ID
                    FROM DPA_LOG
                    ORDER BY SYSTEM_ID DESC
                )
                WHERE ROWNUM = 1;
        END;      
    
       UPDATE PUBLISHER_INSTANCES
        SET INSTANCENAME = p_Name,
            SUBSCRIBERSERVICEURL = p_SubscriberServiceUrl,
            EXECUTIONTYPE = p_ExecutionType,
            EXECUTIONTICKS = p_ExecutionTicks,
            STARTLOGDATE = p_StartLogDate,
            LASTLOGID = p_LastLogId
        WHERE ID = p_Id;
    
    END UpdatePublishInstance;                              
    
    -- Rimozione di tutti gli eventi associati ad un'istanza di pubblicazione                        
    PROCEDURE ClearInstanceEvents(p_Id NUMBER)
    IS
    BEGIN
        DELETE
        FROM PUBLISHER_EVENTS
        WHERE PUBLISHINSTANCEID = p_Id;
         
    END ClearInstanceEvents;    
    
    -- Inserimento di un evento nell'istanza di pubblicazione
    PROCEDURE InsertInstanceEvent(p_InstanceId NUMBER,
                                    p_EventName NVARCHAR2,
                                    p_ObjectType NVARCHAR2,
                                    p_ObjectTemplateName NVARCHAR2,
                                    p_DataMapperFullClass NVARCHAR2,
                                    p_LoadFileIfDocType CHAR,
                                    p_Id OUT NUMBER)
    IS
    BEGIN
        SELECT SEQ_PUBLISHER.NEXTVAL INTO p_Id FROM dual;
    
        INSERT INTO PUBLISHER_EVENTS
        (
            ID,
            PUBLISHINSTANCEID,
            EVENTNAME,
            OBJECTTYPE,
            OBJECTTEMPLATENAME,
            DATAMAPPERFULLCLASS,
            LOADFILEIFDOCTYPE
        )
        VALUES
        (
            p_Id,
            p_InstanceId,
            p_EventName,
            p_ObjectType,
            p_ObjectTemplateName,
            p_DataMapperFullClass,
            p_LoadFileIfDocType
        );
    
    END InsertInstanceEvent;                                     
    
    -- Aggiornamento evento
    PROCEDURE UpdateInstanceEvent(p_Id NUMBER,
                                  p_EventName NVARCHAR2,
                                  p_ObjectType NVARCHAR2,
                                  p_ObjectTemplateName NVARCHAR2,
                                  p_DataMapperFullClass NVARCHAR2,
                                  p_LoadFileIfDocType CHAR)
    IS
    BEGIN
        UPDATE PUBLISHER_EVENTS
        SET EVENTNAME = p_EventName,
            OBJECTTYPE = p_ObjectType,
            OBJECTTEMPLATENAME = p_ObjectTemplateName,
            DATAMAPPERFULLCLASS = p_DataMapperFullClass,
            LOADFILEIFDOCTYPE = p_LoadFileIfDocType
        WHERE ID = p_Id;
        
    
    END UpdateInstanceEvent;          
                                      
    -- Rimozione di un'istanza di pubblicazione            
    PROCEDURE RemovePublishInstanceEvents(p_Id NUMBER)
    IS
    BEGIN
        DELETE
        FROM PUBLISHER_INSTANCES
        WHERE ID = p_Id; 
    
    END RemovePublishInstanceEvents; 
    
        -- Rimozione dell'evento
    PROCEDURE RemoveInstanceEvent(p_Id NUMBER)
    IS
    BEGIN
        DELETE
        FROM PUBLISHER_EVENTS
        WHERE ID = p_Id; 
    END RemoveInstanceEvent;

    
    -- Reperimento degli errori verificatisi nell'istanza di pubblicazione
    PROCEDURE GetInstanceErrors(p_InstanceId NUMBER, p_res_cursor OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN p_res_cursor FOR
        SELECT *
        FROM PUBLISHER_ERRORS
        WHERE PUBLISHINSTANCEID = p_InstanceId
        ORDER BY ERRORDATE DESC;
        
    END GetInstanceErrors;

-- Inserimento di un errore verificatisi nell'istanza di pubblicazione
    PROCEDURE InsertInstanceError(p_InstanceId NUMBER,
                                    p_ErrorCode NVARCHAR2,
                                    p_ErrorDescription NVARCHAR2,
                                    p_ErrorStack NVARCHAR2,
                                    p_ErrorDate DATE,
                                    p_Id OUT NUMBER)
    IS
    BEGIN
        SELECT SEQ_PUBLISHER.NEXTVAL INTO p_Id FROM dual;
    
        INSERT INTO PUBLISHER_ERRORS
        (
            ID,
            PUBLISHINSTANCEID,
            ERRORCODE,
            ERRORDESCRIPTION,
            ERRORSTACK,
            ERRORDATE
        )
        VALUES
        (
            p_Id,
            p_InstanceId,
            p_ErrorCode,
            p_ErrorDescription,
            p_ErrorStack,
            p_ErrorDate
        );    
    
    END InsertInstanceError;                        
                                        
END PUBLISHER; 

/
