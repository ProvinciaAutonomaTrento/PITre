--------------------------------------------------------
--  DDL for Package SUBSCRIBER
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE "ITCOLL_6GIU12"."SUBSCRIBER" AS

    TYPE T_SUB_CURSOR IS REF CURSOR; 
  
    -- Reperimento istanze di pubblicazione
    PROCEDURE GetInstances(cur_OUT OUT T_SUB_CURSOR);

    -- Reperimento di un'istanza di pubblicazione
    PROCEDURE GetInstance(pId NUMBER, cur_OUT OUT T_SUB_CURSOR);

    -- Inserimento di un'istanza di pubblicazione
    PROCEDURE InsertInstance(pName NVARCHAR2, 
                        pDescription NVARCHAR2, 
                        pSmtpHost NVARCHAR2,
                        pSmtpPort INTEGER,
                        pSmtpSsl CHAR,
                        pSmtpUserName NVARCHAR2,
                        pSmtpPassword NVARCHAR2, 
                        pSmtpMail NVARCHAR2,
                        pId OUT NUMBER);
    
    -- Aggiornamento di un'istanza di pubblicazione
    PROCEDURE UpdateInstance(pId NUMBER, 
                    pName NVARCHAR2, 
                    pDescription NVARCHAR2,
                    pSmtpHost NVARCHAR2,
                    pSmtpPort INTEGER,
                    pSmtpSsl CHAR,
                    pSmtpUserName NVARCHAR2,
                    pSmtpPassword NVARCHAR2,
                    pSmtpMail NVARCHAR2);
                    
                     
    -- Rimozione di n'istanza di pubblicazione
    PROCEDURE DeleteInstance(pId NUMBER);
    
    -- Reperimento regole di pubblicazione
    PROCEDURE GetRules(pIdInstance NUMBER, cur_OUT OUT T_SUB_CURSOR);
    
    -- Reperimento regola di pubblicazione
    PROCEDURE GetRule(pId NUMBER, cur_OUT OUT T_SUB_CURSOR);    

    -- Inserimento di una regola di pubblicazione
    PROCEDURE InsertRule(pInstanceId NUMBER, 
                pName NVARCHAR2, 
                pDescription NVARCHAR2, 
                pEnabled CHAR,
                pOptions NVARCHAR2,
                pParentRuleId NUMBER,
                pSubName NVARCHAR2,
                pClassId NVARCHAR2,
                pOrdinal OUT INTEGER,
                pId OUT NUMBER);
                
                
    -- Aggiornamento di una regola di pubblicazione
    PROCEDURE UpdateRule(
                pId NUMBER,
                pDescription NVARCHAR2, 
                pEnabled CHAR,
                pOptions NVARCHAR2,
                pClassId NVARCHAR2);                

    -- Rimozione di una regola di pubblicazione
    PROCEDURE DeleteRule(pId NUMBER);
    
    -- Reperimento regole di pubblicazione
    PROCEDURE GetSubRules(pIdRule NUMBER, cur_OUT OUT T_SUB_CURSOR);

    -- Reperimento regola di pubblicazione
    PROCEDURE GetSubRule(pId NUMBER, cur_OUT OUT T_SUB_CURSOR);
    
    -- Inserimento di una sottoregola di pubblicazione
    PROCEDURE InsertSubRule(pInstanceId NUMBER, 
                pDescription NVARCHAR2, 
                pEnabled CHAR,
                pOptions NVARCHAR2,
                pParentRuleId NUMBER,
                pSubName NVARCHAR2,
                pOrdinal OUT INTEGER,
                pId OUT NUMBER);
                
                
-- Inserimento di una regola di pubblicazione
    PROCEDURE InsertHistory(
                pRuleId NUMBER, 
                pIdObject NVARCHAR2, 
                pObjectType NVARCHAR2,
                pObjectTemplateName NVARCHAR2,
                pObjectDescription NVARCHAR2,
                pAuthorName NVARCHAR2,
                pAuthorId NVARCHAR2, 
                pRoleName NVARCHAR2,
                pRoleId NVARCHAR2,
                pObjectSnapshot CLOB,
                pMailMessageSnapshot CLOB,
                pComputed CHAR,
                pComputeDate DATE,
                pErrorId NVARCHAR2,    
                pErrorDescription NVARCHAR2,
                pErrorStack NVARCHAR2,
                pId OUT NUMBER);
                
    -- Reperimento dati pubblicati per una regola                
    PROCEDURE GetRuleHistory(pIdRule NUMBER, cur_OUT OUT T_SUB_CURSOR);
                
    -- Reperimento dati pubblicati per una regola                
    PROCEDURE GetRuleHistoryPaging(pIdRule NUMBER, 
            pObjectDescription NVARCHAR2,
            pAuthorName NVARCHAR2,
            pRoleName NVARCHAR2,
            pPage INT, 
            pObjectsPerPage INT, 
            pObjectsCount OUT INT, 
            cur_OUT OUT T_SUB_CURSOR);
        
    -- Reperimento dati ultima pubblicazione per una regola
    PROCEDURE GetLastHistory(pIdRule NUMBER, pIdObject NVARCHAR2, cur_OUT OUT T_SUB_CURSOR);

    -- Reperimento dati di un elemento pubblicato                
    PROCEDURE GetHistory(pId NUMBER, cur_OUT OUT T_SUB_CURSOR);
        
END SUBSCRIBER; 

/


--------------------------------------------------------
--  DDL for Package Body SUBSCRIBER
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY "ITCOLL_6GIU12"."SUBSCRIBER" AS

    PROCEDURE GetInstances(cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
           SELECT *         
           FROM SUBSCRIBER_INSTANCES
           ORDER BY ID ASC;       
          
    END GetInstances;
  
  
    -- Reperimento di un'istanza di pubblicazione
    PROCEDURE GetInstance(pId NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
           SELECT *          
           FROM SUBSCRIBER_INSTANCES
           WHERE ID = pId;    
    
    END;
    
    -- Inserimento di un'istanza di pubblicazione
    PROCEDURE InsertInstance(pName NVARCHAR2, 
                pDescription NVARCHAR2, 
                pSmtpHost NVARCHAR2,
                pSmtpPort INTEGER,
                pSmtpSsl CHAR,
                pSmtpUserName NVARCHAR2,
                pSmtpPassword NVARCHAR2,
                pSmtpMail NVARCHAR2,                 
                pId OUT NUMBER)
    IS
    BEGIN
    
        SELECT SEQ_SUBSCRIBER.NEXTVAL INTO pId FROM dual;

        INSERT INTO SUBSCRIBER_INSTANCES
        (
         Id,
         NAME,
         DESCRIPTION,
         SMTPHOST, 
         SMTPPORT,
         SMTPSSL,
         SMTPUSERNAME,
         SMTPPASSWORD,
         SMTPMAIL
        )
        VALUES
        (
         pId,
         pName,
         pDescription,
         pSmtpHost,
         pSmtpPort,
         pSmtpSsl,
         pSmtpUserName,
         pSmtpPassword,
         pSmtpMail   
        );
            
        END;
    
    -- Aggiornamento di un'istanza di pubblicazione
    PROCEDURE UpdateInstance(pId NUMBER, 
                pName NVARCHAR2, 
                pDescription NVARCHAR2,
                pSmtpHost NVARCHAR2,
                pSmtpPort INTEGER,
                pSmtpSsl CHAR,
                pSmtpUserName NVARCHAR2,
                pSmtpPassword NVARCHAR2,
                pSmtpMail NVARCHAR2)
    IS
    BEGIN
        UPDATE SUBSCRIBER_INSTANCES
        SET NAME = pName, 
            DESCRIPTION = pDescription,
            SMTPHOST = pSmtpHost,
            SMTPPORT = pSmtpPort,
            SMTPSSL = pSmtpSsl,
            SMTPUSERNAME = pSmtpUserName,
            SMTPPASSWORD =  pSmtpPassword,
            SMTPMAIL = pSmtpMail             
        WHERE ID = pId;
    END;
    
    -- Rimozione di n'istanza di pubblicazione
    PROCEDURE DeleteInstance(pId NUMBER)
    IS
    BEGIN
        DELETE FROM SUBSCRIBER_INSTANCES WHERE ID = pId;
    END;
    
    -- Reperimento regole di pubblicazioe
    PROCEDURE GetRules(pIdInstance NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
            SELECT * 
            FROM SUBSCRIBER_RULES 
            WHERE INSTANCEID = pIdInstance
            ORDER BY INSTANCEID ASC, NVL(PARENTRULEID, 0) ASC, ORDINAL ASC;        
    END;
    
    -- Reperimento regola di pubblicazione
    PROCEDURE GetRule(pId NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
            SELECT * 
            FROM SUBSCRIBER_RULES 
            WHERE ID = pId OR PARENTRULEID = pId 
            ORDER BY NVL(PARENTRULEID, 0) ASC, ORDINAL ASC;
    
    END;
    
    -- Reperimento regole di pubblicazione
    PROCEDURE GetSubRules(pIdRule NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
            SELECT * 
            FROM SUBSCRIBER_RULES 
            WHERE PARENTRULEID = pIdRule
            ORDER BY ORDINAL ASC;  
    END;
    
    -- Reperimento regola di pubblicazione
    PROCEDURE GetSubRule(pId NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
            SELECT * 
            FROM SUBSCRIBER_RULES 
            WHERE ID = pId;
    
    END;
  
-- Inserimento di una regola di pubblicazione
    PROCEDURE InsertRule(pInstanceId NUMBER, 
                pName NVARCHAR2, 
                pDescription NVARCHAR2, 
                pEnabled CHAR,
                pOptions NVARCHAR2,
                pParentRuleId NUMBER,
                pSubName NVARCHAR2,
                pClassId NVARCHAR2,
                pOrdinal OUT INTEGER,                                
                pId OUT NUMBER)
    IS
    
    BEGIN
        SELECT SEQ_SUBSCRIBER.NEXTVAL INTO pId FROM dual;
        
        SELECT NVL(MAX(ORDINAL), 0) + 1 INTO pOrdinal
        FROM SUBSCRIBER_RULES 
        WHERE INSTANCEID = pInstanceId;
        
        INSERT INTO SUBSCRIBER_RULES
        (
            ID,
            INSTANCEID,
            NAME,
            DESCRIPTION,
            ENABLED,
            ORDINAL,
            OPTIONS,
            PARENTRULEID,
            SUBNAME,
            CLASS_ID
        )
        VALUES
        (   
            pId,
            pInstanceId,
            pName,
            pDescription,
            pEnabled,
            pOrdinal,
            pOptions,
            pParentRuleId,
            pSubName,
            pClassId
        );
    
    END;
    
        -- Aggiornamento di una regola di pubblicazione
    PROCEDURE UpdateRule(
                pId NUMBER,
                pDescription NVARCHAR2, 
                pEnabled CHAR,
                pOptions NVARCHAR2,
                pClassId NVARCHAR2)
    IS
    BEGIN
        UPDATE SUBSCRIBER_RULES
        SET DESCRIPTION = pDescription, 
            ENABLED = pEnabled,
            OPTIONS = pOptions,
            CLASS_ID = pClassId
        WHERE ID = pId;
    END;                
               
-- Rimozione di una regola di pubblicazione
    PROCEDURE DeleteRule(pId NUMBER)
    IS
    BEGIN
        DELETE FROM SUBSCRIBER_RULES WHERE ID = pId;
    END;
    

    -- Inserimento di una sottoregola di pubblicazione
    PROCEDURE InsertSubRule(
                pInstanceId NUMBER, 
                pDescription NVARCHAR2, 
                pEnabled CHAR,
                pOptions NVARCHAR2,
                pParentRuleId NUMBER,
                pSubName NVARCHAR2,
                pOrdinal OUT INTEGER,
                pId OUT NUMBER)
    IS
    pName NVARCHAR2(50) := 0;
    BEGIN
    
        SELECT SEQ_SUBSCRIBER.NEXTVAL INTO pId FROM dual;
        
        SELECT NVL(MAX(ORDINAL), 0) + 1 INTO pOrdinal
        FROM SUBSCRIBER_RULES 
        WHERE PARENTRULEID = pParentRuleId;
        
        SELECT NAME INTO pName
        FROM SUBSCRIBER_RULES
        WHERE ID = pParentRuleId;
        
        INSERT INTO SUBSCRIBER_RULES
        (
            ID,
            INSTANCEID,
            NAME,
            DESCRIPTION,
            ENABLED,
            ORDINAL,
            OPTIONS,
            PARENTRULEID,
            SUBNAME
        )
        VALUES
        (   
            pId,
            pInstanceId,
            pName,
            pDescription,
            pEnabled,
            pOrdinal,
            pOptions,
            pParentRuleId,
            pSubName
        );    
    
    END;    
             
-- Inserimento di una regola di pubblicazione
    PROCEDURE InsertHistory(
                pRuleId NUMBER, 
                pIdObject NVARCHAR2, 
                pObjectType NVARCHAR2,
                pObjectTemplateName NVARCHAR2,
                pObjectDescription NVARCHAR2,
                pAuthorName NVARCHAR2,
                pAuthorId NVARCHAR2, 
                pRoleName NVARCHAR2,
                pRoleId NVARCHAR2,
                pObjectSnapshot CLOB,
                pMailMessageSnapshot CLOB,
                pComputed CHAR,
                pComputeDate DATE,
                pErrorId NVARCHAR2,    
                pErrorDescription NVARCHAR2,
                pErrorStack NVARCHAR2,
                pId OUT NUMBER)
    IS
    BEGIN
    
            SELECT SEQ_SUBSCRIBER.NEXTVAL INTO pId FROM dual;
            
            INSERT INTO SUBSCRIBER_HISTORY
            (
                ID,
                RULEID,    
                IDOBJECT,
                OBJECTTYPE,    
                OBJECTTEMPLATENAME,
                OBJECDESCRIPTION,
                AUTHORNAME,
                AUTHORID,    
                ROLENAME,    
                ROLEID,
                OBJECTSNAPSHOT,    
                MAILMESSAGESNAPSHOT,
                COMPUTED,
                COMPUTEDATE,    
                ERRORID,
                ERRORDESCRIPTION,
                ERRORSTACK                    
            )
            VALUES
            (
                pId,
                pRuleId, 
                pIdObject, 
                pObjectType,
                pObjectTemplateName,
                pObjectDescription,
                pAuthorName,
                pAuthorId, 
                pRoleName,
                pRoleId,
                empty_clob(),
                empty_clob(),
                pComputed,
                pComputeDate,
                pErrorId,    
                pErrorDescription,
                pErrorStack             
            );
            
            update SUBSCRIBER_HISTORY
            set OBJECTSNAPSHOT = pObjectSnapshot,
            MAILMESSAGESNAPSHOT = pMailMessageSnapshot
            where id = pId;
            
    END;
    
    -- Reperimento dati pubblicati per una regola                
    PROCEDURE GetRuleHistory(pIdRule NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
                SELECT H.*
                FROM SUBSCRIBER_HISTORY H
                        INNER JOIN SUBSCRIBER_RULES R ON H.RULEID = R.ID
                WHERE R.ID = pIdRule OR R.PARENTRULEID = pIdRule 
                ORDER BY H.ID DESC;
                
    
    END;    
    
    -- Reperimento dati pubblicati per una regola                
    PROCEDURE GetRuleHistoryPaging(pIdRule NUMBER, 
                pObjectDescription NVARCHAR2,
                pAuthorName NVARCHAR2,
                pRoleName NVARCHAR2,
                pPage INT, 
                pObjectsPerPage INT, 
                pObjectsCount OUT INT, 
                cur_OUT OUT T_SUB_CURSOR)
    IS
         sqlInnerText VARCHAR2(2000) := NULL;
         selectStatement NVARCHAR2(2000) := NULL;
         fromClausole NVARCHAR2(2000) := NULL;
         whereClausole NVARCHAR2(2000) := NULL;
         orderByStatement NVARCHAR2(2000) := 'H.ID DESC';
         startRow INT := 0;
         endRow INT := 0;
    
    BEGIN
    
        IF (pPage != 0 AND pObjectsPerPage != 0) THEN
            startRow := ((pPage * pObjectsPerPage) - pObjectsPerPage) + 1;
            endRow := (startRow - 1) + pObjectsPerPage;
        END IF;
        
        
        --selectStatement := 'ROWNUM RN, H.*';
        selectStatement := 'H.*';

        fromClausole := 'SUBSCRIBER_HISTORY H INNER JOIN SUBSCRIBER_RULES R ON H.RULEID = R.ID';
             
        whereClausole := '(R.ID = ' || pIdRule || ' OR R.PARENTRULEID = ' || pIdRule || ')';                        
        
        if (pObjectDescription is not null) then
            whereClausole := whereClausole || ' AND UPPER(H.OBJECDESCRIPTION) LIKE UPPER(''%' || trim(pObjectDescription) || '%'')';
        end if;
        
        if (pAuthorName is not null) then
            whereClausole := whereClausole || ' AND UPPER(H.AUTHORNAME) LIKE UPPER(''%' || trim(pAuthorName) || '%'')';
        end if;        

        if (pRoleName is not null) then
            whereClausole := whereClausole || ' AND UPPER(H.ROLENAME) LIKE UPPER(''%' || trim(pRoleName) || '%'')';
        end if;
        
                
        sqlInnerText :=  'SELECT ' || selectStatement ||
                         ' FROM ' || fromClausole ||
                         ' WHERE ' || whereClausole ||
                         ' ORDER BY ' || orderByStatement;
                         
        
        IF (pPage != 0 AND pObjectsPerPage != 0) THEN
            EXECUTE IMMEDIATE  'SELECT COUNT(*) FROM (' || sqlInnerText || ')' INTO pObjectsCount;
        
            OPEN cur_OUT FOR 'SELECT * FROM (SELECT ROWNUM RN, H.* FROM (' || sqlInnerText || ') H) H2 WHERE H2.RN BETWEEN  ' || startRow || ' AND ' || endRow;
        ELSE
            OPEN cur_OUT FOR sqlInnerText;
        END IF;            
    END;    
    
    
    -- Reperimento dati ultima pubblicazione per una regola
    PROCEDURE GetLastHistory(pIdRule NUMBER, pIdObject NVARCHAR2, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
                SELECT *
                FROM 
                (
                    SELECT *
                    FROM SUBSCRIBER_HISTORY
                    WHERE RULEID = pIdRule 
                    AND IDOBJECT = pIdObject
                    ORDER BY ID DESC
                )
                WHERE ROWNUM = 1;
    
    END;
                  
    -- Reperimento dati di un elemento pubblicato                
    PROCEDURE GetHistory(pId NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
                SELECT *
                FROM SUBSCRIBER_HISTORY
                WHERE ID = pId;
    
    END;
        
END SUBSCRIBER; 

/
