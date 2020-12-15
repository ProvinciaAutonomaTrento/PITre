
--------------------------------------------------------
--  DDL for Package PGU
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE "ITCOLL_6GIU12"."PGU" AS

    TYPE T_SUB_CURSOR IS REF CURSOR; 
    
    
    -- Reperimento utenti
    PROCEDURE GetUtenti(cur_OUT OUT T_SUB_CURSOR);
    
    -- Reperimento utente
    PROCEDURE GetUtente(pId NUMBER, cur_OUT OUT T_SUB_CURSOR);       
    
    -- Reperimento associazioni
    PROCEDURE GetAssociazioniUtente(pId NUMBER, cur_OUT OUT T_SUB_CURSOR);
    
    -- Verifica esistenza utente
    PROCEDURE ContainsUtente(pNome NVARCHAR2, pRet OUT NUMBER); 
    
    -- Inserimento di un utente
    PROCEDURE InsertUtente( pNome NVARCHAR2,
                            pDescrizione NVARCHAR2, 
                            pPassword NVARCHAR2, 
                            pSecretKey NVARCHAR2,
                            pSecretIV NVARCHAR2,
                            pAmministratore CHAR,
                            pId OUT NUMBER);
                            
    -- Modifica password utente                            
    PROCEDURE ModificaPasswordUtente(pId NUMBER,
                                     pOldPassword NVARCHAR2,
                                     pPassword NVARCHAR2, 
                                     pSecretKey NVARCHAR2,
                                     pSecretIV NVARCHAR2);    
    
    -- Rimozione associazioni utente
    PROCEDURE ClearAssociazioniUtenteEnte(pIdUtente NUMBER);
    
    -- Inserimento associazione utente / ente                            
    PROCEDURE InsertAssociazioneUtenteEnte(pIdEnte NUMBER,
                                           pIdUtente NUMBER,
                                           pId OUT NUMBER);
    
    -- Modifica di un utente
    PROCEDURE UpdateUtente( pId NUMBER,
                            pDescrizione NVARCHAR2, 
                            pAmministratore CHAR);    
    
    -- Rimozione utente
    PROCEDURE DeleteUtente(pId NUMBER);
    
    -- Reperimento degli enti
    PROCEDURE GetEnti(cur_OUT OUT T_SUB_CURSOR);
    
      -- Reperimento chiavi private 
    PROCEDURE GetPasswordPrivateKeys(pNome NVARCHAR2, cur_OUT OUT T_SUB_CURSOR);

    -- LoginUtente
    PROCEDURE Login(pNome NVARCHAR2, pPassword NVARCHAR2, cur_OUT OUT T_SUB_CURSOR);
    
    -- Reperimento degli enti cui ha visibilit l'utente
    PROCEDURE GetEntiUtente(pIdUtente NUMBER, cur_OUT OUT T_SUB_CURSOR);
   
    
    -- Reperimento ente
    PROCEDURE GetEnte(pId NUMBER, cur_OUT OUT T_SUB_CURSOR);

    -- Verifica esistenza ente
    PROCEDURE ContainsEnte(pNome NVARCHAR2, pRet OUT NUMBER);
    
    -- Inserimento ente    
    PROCEDURE InsertEnte(pNome NVARCHAR2,
                        pDescrizione NVARCHAR2,
                        pUrlAppConsProtocollo NVARCHAR2,
                        pUrlAppConsAltreApp NVARCHAR2,
                        pUrlWsConsProtocollo NVARCHAR2,
                        pUrlWsConsAltreApp NVARCHAR2,
                        pId OUT NUMBER);
                        
          
    -- Aggiornamento ente    
    PROCEDURE UpdateEnte(pId NUMBER,
                        pNome NVARCHAR2,
                        pDescrizione NVARCHAR2,
                        pUrlAppConsProtocollo NVARCHAR2,
                        pUrlAppConsAltreApp NVARCHAR2,
                        pUrlWsConsProtocollo NVARCHAR2,
                        pUrlWsConsAltreApp NVARCHAR2);

    -- Rimozione ente
    PROCEDURE DeleteEnte(pId NUMBER);
                                
END PGU;

/


--------------------------------------------------------
--  DDL for Package Body PGU
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY "ITCOLL_6GIU12"."PGU" AS

   

    PROCEDURE GetUtenti(cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
           SELECT *         
           FROM PGU_UTENTI
           ORDER BY Id ASC;       
          
    END GetUtenti;
  

    -- Reperimento utente
    PROCEDURE GetUtente(pId NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
           SELECT *         
           FROM PGU_UTENTI
           WHERE Id = pId
           ORDER BY Nome ASC;  
               
    END GetUtente;
    
        -- Reperimento associazioni
    PROCEDURE GetAssociazioniUtente(pId NUMBER, cur_OUT OUT T_SUB_CURSOR)
    AS
    
    BEGIN
        DECLARE pCount NUMBER;    
        
        BEGIN
            BEGIN
             OPEN cur_OUT FOR
                SELECT EU.ID,
                        E.ID AS IDENTE,
                        E.NOME AS ENTE,
                        U.ID AS IDUTENTE,
                        U.NOME AS UTENTE
                from pgu_enti e
                    left join pgu_enti_utenti eu on e.id = eu.idente
                    left join pgu_utenti u on eu.idutente = u.id
                where u.id = pId
                order by e.Nome asc;
            END;
          
        END;            
         
    END GetAssociazioniUtente; 

-- Verifica esistenza utente
    PROCEDURE ContainsUtente(pNome NVARCHAR2, pRet OUT NUMBER)
    IS
    BEGIN
        SELECT COUNT(*) INTO pRet
        FROM PGU_UTENTI
        WHERE UPPER(NOME) = UPPER(pNome);
         
    END ContainsUtente; 
    
    -- Inserimento di un utente
    PROCEDURE InsertUtente( pNome NVARCHAR2,
                            pDescrizione NVARCHAR2, 
                            pPassword NVARCHAR2, 
                            pSecretKey NVARCHAR2,
                            pSecretIV NVARCHAR2,
                            pAmministratore CHAR,
                            pId OUT NUMBER)
    IS
    BEGIN
        SELECT SEQ_PGU.NEXTVAL INTO pId FROM dual;

        INSERT INTO PGU_UTENTI
        (
         ID,
         NOME,
         DESCRIZIONE,
         PASSWORD, 
         SECRETKEY,
         SECRETIV,
         AMMINISTRATORE
        )
        VALUES
        (
         pId,
         pNome,
         pDescrizione,
         pPassword,
         pSecretKey,
         pSecretIV,
         pAmministratore
        );
            
    END InsertUtente;          
    
        -- Modifica password utente                            
    PROCEDURE ModificaPasswordUtente(pId NUMBER,
                                     pOldPassword NVARCHAR2,
                                     pPassword NVARCHAR2, 
                                     pSecretKey NVARCHAR2,
                                     pSecretIV NVARCHAR2)
    IS
    BEGIN
        UPDATE  PGU_UTENTI
        SET     PASSWORD = pPassword,
                SECRETKEY = pSecretKey,
                SECRETIV = pSecretIV
        WHERE   ID = pId
                AND PASSWORD = pOldPassword;
        
    END ModificaPasswordUtente;                               

    
    -- Rimozione associazioni utente
    PROCEDURE ClearAssociazioniUtenteEnte(pIdUtente NUMBER)
    IS
    BEGIN
        DELETE FROM PGU_ENTI_UTENTI WHERE IDUTENTE = pIdUtente;
        
    END ClearAssociazioniUtenteEnte; 
    
    
    -- Inserimento associazione utente / ente                            
    PROCEDURE InsertAssociazioneUtenteEnte(pIdEnte NUMBER,
                                           pIdUtente NUMBER,
                                           pId OUT NUMBER)
    IS
    BEGIN
        SELECT SEQ_PGU.NEXTVAL INTO pId FROM dual;
        
        INSERT INTO PGU_ENTI_UTENTI
        (
            ID,
            IDENTE,
            IDUTENTE
        )
        VALUES
        (   
            pId,
            pIdEnte,
            pIdUtente
        );
        
    END InsertAssociazioneUtenteEnte;          
    
    -- Modifica di un utente
    PROCEDURE UpdateUtente( pId NUMBER,
                            pDescrizione NVARCHAR2, 
                            pAmministratore CHAR)
    IS
    BEGIN
        UPDATE  PGU_UTENTI
        SET     DESCRIZIONE = pDescrizione,
                AMMINISTRATORE = pAmministratore
        WHERE   ID = pId;
    
    END UpdateUtente;
    
    -- Rimozione utente
    PROCEDURE DeleteUtente(pId NUMBER)
    IS
    BEGIN
        DELETE FROM PGU_UTENTI WHERE ID = pId;
        
    END DeleteUtente;

                             
    -- Reperimento degli enti
    PROCEDURE GetEnti(cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        
        OPEN cur_OUT FOR
        SELECT *
        FROM PGU_ENTI;
    
    END GetEnti;
    
     -- Reperimento chiavi private 
    PROCEDURE GetPasswordPrivateKeys(pNome NVARCHAR2, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
           SELECT SECRETKEY, SECRETIV
           FROM PGU_UTENTI
           WHERE UPPER(NOME) = UPPER(pNome);      
        
    
    END GetPasswordPrivateKeys;

    -- LoginUtente
    PROCEDURE Login(pNome NVARCHAR2, pPassword NVARCHAR2, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
        OPEN cur_OUT FOR
           SELECT *
           FROM PGU_UTENTI
           WHERE UPPER(NOME) = UPPER(pNome)
            AND PASSWORD = pPassword;
        
    END Login;
    
    -- Reperimento degli enti cui ha visibilit l'utente
    PROCEDURE GetEntiUtente(pIdUtente NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
          OPEN cur_OUT FOR
           SELECT *
           FROM PGU_ENTI E
                INNER JOIN PGU_ENTI_UTENTI EU ON E.ID = EU.IDENTE
           WHERE EU.IDUTENTE = pIdUtente
           ORDER BY E.ID ASC;
    
    END GetEntiUtente;
  
       
    -- Reperimento ente
    PROCEDURE GetEnte(pId NUMBER, cur_OUT OUT T_SUB_CURSOR)
    IS
    BEGIN
      OPEN cur_OUT FOR
           SELECT *
           FROM PGU_ENTI
           WHERE ID = pId
           ORDER BY ID ASC;
    
    END GetEnte; 
     
    -- Verifica esistenza ente
    PROCEDURE ContainsEnte(pNome NVARCHAR2, pRet OUT NUMBER)
    IS
    BEGIN
        SELECT COUNT(*) INTO pRet
            FROM PGU_ENTI
            WHERE UPPER(NOME) = UPPER(pNome);    
    END ContainsEnte;

    -- Inserimento ente    
    PROCEDURE InsertEnte(pNome NVARCHAR2,
                        pDescrizione NVARCHAR2,
                        pUrlAppConsProtocollo NVARCHAR2,
                        pUrlAppConsAltreApp NVARCHAR2,
                        pUrlWsConsProtocollo NVARCHAR2,
                        pUrlWsConsAltreApp NVARCHAR2,
                        pId OUT NUMBER)
    IS
    BEGIN
    
        SELECT SEQ_PGU.NEXTVAL INTO pId FROM dual;

        INSERT INTO PGU_ENTI
        (
         ID,
         NOME,
         DESCRIZIONE,
         URLAPPCONSPROTOCOLLO,
         URLAPPCONSALTREAPP,
         URLWSCONSPROTOCOLLO,
         URLWSCONSALTREAPP
        )
        VALUES
        (
         pId,
         pNome,
         pDescrizione,         
         pUrlAppConsProtocollo,
         pUrlAppConsAltreApp,
         pUrlWsConsProtocollo,
         pUrlWsConsAltreApp
        );
        
        -- Inserimento record associativi
        INSERT INTO PGU_ENTI_UTENTI
        SELECT SEQ_PGU.NEXTVAL, pId, U.ID 
        FROM PGU_UTENTI U;
    
    END InsertEnte;                     


    -- Aggiornamento ente    
    PROCEDURE UpdateEnte(pId NUMBER,
                        pNome NVARCHAR2,
                        pDescrizione NVARCHAR2,
                        pUrlAppConsProtocollo NVARCHAR2,
                        pUrlAppConsAltreApp NVARCHAR2,
                        pUrlWsConsProtocollo NVARCHAR2,
                        pUrlWsConsAltreApp NVARCHAR2)
    IS
    BEGIN
        
        UPDATE PGU_ENTI
        SET NOME = pNome,
            DESCRIZIONE = pDescrizione,
            URLAPPCONSPROTOCOLLO = pUrlAppConsProtocollo,
            URLAPPCONSALTREAPP = pUrlAppConsAltreApp,
            URLWSCONSPROTOCOLLO = pUrlWsConsProtocollo,
            URLWSCONSALTREAPP = pUrlWsConsAltreApp
        WHERE ID = pId; 
    
    
    END UpdateEnte;                     
                        
    
    -- Rimozione ente
    PROCEDURE DeleteEnte(pId NUMBER)
    IS
    BEGIN
        DELETE FROM PGU_ENTI WHERE ID = pId;
         
    END DeleteEnte;     
           
END PGU;

/
