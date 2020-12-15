declare 

c_seq number;

c_seq2 number;

cnt1 int;

cnt2 int;


begin


select @db_user.SEQ.nextval into c_seq from dual;

select @db_user.SEQ.nextval into c_seq2 from dual;

 
 begin
select count(*) into cnt1 from @db_user.DPA_ANAGRAFICA_LOG l where l.var_codice ='AMM_LOGIN';

if (cnt1 = 0) then
Insert into DPA_ANAGRAFICA_LOG

   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)

Values

   (c_seq, 'AMM_LOGIN', 'Accesso Admin all''applicazione', 'UTENTE', 'AMM_LOGIN');

Insert into @db_user.DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (c_seq, 0);


 end if;
 end;
 
  begin
select count(*) into cnt2 from @db_user.DPA_ANAGRAFICA_LOG l where l.var_codice ='AMM_LOGOFF';

if (cnt2 = 0) then

Insert into DPA_ANAGRAFICA_LOG

   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)

Values

   (c_seq2, 'AMM_LOGOFF', 'Uscita Admin dall''applicazione', 'UTENTE', 'AMM_LOGOFF');
Insert into @db_user.DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (c_seq2, 0);

end if;
end;
 
END;
/
BEGIN
   DECLARE
     nomeutente		VARCHAR2 (200) := upper('@db_user');

      nometabellaPK VARCHAR2(2000) :='DPA_RAGIONE_TRASM'   ;
      nomecolonnaPK VARCHAR2(2000) :='SYSTEM_ID' ;
     -- fine variabili da modificare

      -- il nome NON  usato per verificare se la PK esiste gi, ma si vede se esiste un vincolo tipo 'P' su quella colonna
      nomePrimaryKey VARCHAR2(2000) :='PK_'||nometabellaPK ; 
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaPK||' ADD CONSTRAINT
                '||nomePrimaryKey||' PRIMARY KEY 
                ('||nomecolonnaPK||') ' ; 
      
      istruzioneCheck VARCHAR2(2000) := 'SELECT COUNT (*)
                    FROM '||nomeutente||'.'||nometabellaPK||'
                    group by '||nomecolonnaPK||'
                    having count(*) > 1
                -- devo fare la UNION e aggiungere una riga per evitare eccezione di NO_DATA_FOUND
                -- , che in questo caso corrisponde a esito positivo, perch nessun record viola il vincolo!
                UNION
                        select 0 from dual';
      cnt       INT;
      cntdati   INT;
   BEGIN
     SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
-- INSERIRE SEMPRE IL TIPO del VINCOLO:
-- i tipi possibili sono U=unique, R=referential (chiave esterna),
-- P=Primary e C= check	
         and cons.constraint_type  = 'P'
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaPK
         and cons_cols.COLUMN_NAME = nomecolonnaPK ; 

      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN

       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ; 
         IF (cntdati = 0)
         -- non esistono record che violano la chiave
         THEN
            EXECUTE IMMEDIATE istruzioneSQL; 
        ELSE
            DBMS_OUTPUT.PUT_line('impossibile creare vincolo di primary key su colonna 
                '||nomecolonnaPK||' della tabella '||nometabellaPK||'. Trovati record che violano il vincolo'  ); 
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
     nomeutente		VARCHAR2 (200) := upper('@db_user');

      nometabellaPK VARCHAR2(2000) :='DPA_TRASMISSIONE'   ; 
      nomecolonnaPK VARCHAR2(2000) :='SYSTEM_ID' ;
     -- fine variabili da modificare
     
      -- il nome NON  usato per verificare se la PK esiste gi, ma si vede se esiste un vincolo tipo 'P' su quella colonna 
      nomePrimaryKey VARCHAR2(2000) :='PK_'||nometabellaPK ;
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaPK||' ADD CONSTRAINT
                '||nomePrimaryKey||' PRIMARY KEY 
                ('||nomecolonnaPK||') ' ;
      
      istruzioneCheck VARCHAR2(2000) := 'SELECT COUNT (*)
                    FROM '||nomeutente||'.'||nometabellaPK||'
                    group by '||nomecolonnaPK||'
                    having count(*) > 1
                -- devo fare la UNION e aggiungere una riga per evitare eccezione di NO_DATA_FOUND
                -- , che in questo caso corrisponde a esito positivo, perch nessun record viola il vincolo!
                UNION
                        select 0 from dual';
      cnt       INT;
      cntdati   INT;
   BEGIN
     SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
-- INSERIRE SEMPRE IL TIPO del VINCOLO:
-- i tipi possibili sono U=unique, R=referential (chiave esterna),
-- P=Primary e C= check	
         and cons.constraint_type  = 'P' 
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaPK
         and cons_cols.COLUMN_NAME = nomecolonnaPK ;
      
      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
         
       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ; 
         IF (cntdati = 0)
         -- non esistono record che violano la chiave
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
        ELSE
            DBMS_OUTPUT.PUT_line('impossibile creare vincolo di primary key su colonna
                '||nomecolonnaPK||' della tabella '||nometabellaPK||'. Trovati record che violano il vincolo'  );
         END IF;
      END IF;
   END;
END;
/

/*
AUTORE:							Stefano Frezza
Data creazione:					18/02/2011
Scopo della modifica:			Rilascio pubblicazioni
Indicazione della MEV o dello sviluppo a cui  collegata la modifica:	INFOTN-AVVOCATURA

CREATE UNIQUE INDEX PUBLISHER_INSTANCES_PK ON PUBLISHER_INSTANCES(ID);

ALTER TABLE PUBLISHER_INSTANCES ADD (  CONSTRAINT PUBLISHER_INSTANCES_PK
 PRIMARY KEY (ID));

*/
BEGIN
   DECLARE

      istruzioneSQL VARCHAR2 (2000)
         := 'CREATE TABLE PUBLISHER_INSTANCES
			( ID                     NUMBER  PRIMARY KEY,
			  INSTANCENAME           NVARCHAR2(50)			NOT NULL,
			  IDADMIN                NUMBER					NOT NULL,
			  SUBSCRIBERSERVICEURL   NVARCHAR2(1000)        NOT NULL,
			  LASTEXECUTIONDATE      DATE,
			  EXECUTIONCOUNT         INTEGER,
			  PUBLISHEDOBJECTS       INTEGER,
			  TOTALEXECUTIONCOUNT    INTEGER,
			  TOTALPUBLISHEDOBJECTS  INTEGER,
			  STARTEXECUTIONDATE     DATE,
			  ENDEXECUTIONDATE       DATE,
			  LASTLOGID              NUMBER,
			  STARTLOGDATE           DATE,
			  EXECUTIONTYPE          VARCHAR2(50 BYTE)      NOT NULL,
			  EXECUTIONTICKS         VARCHAR2(100 BYTE)     NOT NULL,
			  MACHINENAME            NVARCHAR2(50)
			)';
			tabelle_esistenti EXCEPTION;
    PRAGMA EXCEPTION_INIT (tabelle_esistenti, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzioneSQL;

   EXCEPTION
      WHEN tabelle_esistenti
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/





BEGIN
--ALTER TABLE PUBLISHER_INSTANCES ADD (  CONSTRAINT FK_IDADMIN 
-- FOREIGN KEY (IDADMIN)  REFERENCES DPA_AMMINISTRA (SYSTEM_ID)
--    ON DELETE CASCADE);

   DECLARE
      nomeutente        VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='PUBLISHER_INSTANCES' ;
      nomecolonnaFK VARCHAR2(2000) :='IDADMIN' ; 

      nometabellaPK VARCHAR2(2000) :='DPA_AMMINISTRA'   ; 
      nomecolonnaPK VARCHAR2(2000) :='SYSTEM_ID' ;

      -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
      nomeForeignKey VARCHAR2(2000) :='FK_PUBL_INSTANCES_IDADMIN' ; 

     -- fine variabili da modifica
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ON DELETE CASCADE ' ; 
      
           istruzioneCheck VARCHAR2(2000) := 'SELECT   (SELECT COUNT (*)
            FROM '||nomeutente||'.'||nometabellaFK||') -- meno le righe che soddisfano la condizione             
       - (SELECT COUNT (*)  FROM '||nomeutente||'.'||nometabellaFK||
            ' WHERE '||nomecolonnaFK||' IN (SELECT '||nomecolonnaPK
                                    ||' FROM '||nomeutente||'.'||nometabellaPK||')
                                             )  FROM DUAL';
    
      cnt       INT;
      cntdati   INT;
   
  BEGIN
      
      SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
         and cons.constraint_type  = 'R' 
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaFK
         and cons_cols.COLUMN_NAME = nomecolonnaFK ; 
      
      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
         
       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ;

         IF (cntdati <> 0)
         -- esistono record che violano la chiave
         THEN
             istruzioneSQL := istruzioneSQL||' novalidate';
         END IF;

            EXECUTE IMMEDIATE istruzioneSQL; 
         
      END IF;
   END;

END;
/



/*
AUTORE:							Stefano Frezza
Data creazione:					18/02/2011
Scopo della modifica:			Rilascio pubblicazioni
Indicazione della MEV o dello sviluppo a cui  collegata la modifica:	INFOTN-AVVOCATURA
CREATE UNIQUE INDEX PUBLISH_INSTANCES_PK ON SUBSCRIBER_INSTANCES (ID);

ALTER TABLE SUBSCRIBER_INSTANCES ADD (  CONSTRAINT PUBLISH_INSTANCES_PK
 PRIMARY KEY (ID));

*/
BEGIN
   DECLARE

      istruzioneSQL VARCHAR2 (2000)
         := 'CREATE TABLE SUBSCRIBER_INSTANCES
			( ID            NUMBER(10)  PRIMARY KEY,
			  NAME          NVARCHAR2(50)                   NOT NULL,
			  DESCRIPTION   NVARCHAR2(255),
			  SMTPHOST      NVARCHAR2(50),
			  SMTPPORT      INTEGER,
			  SMTPSSL       CHAR(1 BYTE),
			  SMTPUSERNAME  NVARCHAR2(50),
			  SMTPPASSWORD  NVARCHAR2(50),
			  SMTPMAIL      NVARCHAR2(255) )';
			tabelle_esistenti EXCEPTION;
    PRAGMA EXCEPTION_INIT (tabelle_esistenti, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzioneSQL;

   EXCEPTION
      WHEN tabelle_esistenti
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/




BEGIN
-- ALTER table DPA_TRASM_SINGOLA ADD CONSTRAINT PK_DPA_TRASM_SINGOLA  PRIMARY KEY SYSTEM_ID
   DECLARE
     nomeutente		VARCHAR2 (200) := upper('@db_user');

      nometabellaPK VARCHAR2(2000) :='DPA_TRASM_SINGOLA'   ;
      nomecolonnaPK VARCHAR2(2000) :='SYSTEM_ID' ;
     -- fine variabili da modificare

      -- il nome NON  usato per verificare se la PK esiste gi, ma si vede se esiste un vincolo tipo 'P' su quella colonna
      nomePrimaryKey VARCHAR2(2000) :='PK_'||nometabellaPK ;

      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaPK||' ADD CONSTRAINT
                '||nomePrimaryKey||' PRIMARY KEY
                ('||nomecolonnaPK||') ' ;

      istruzioneCheck VARCHAR2(2000) := 'SELECT COUNT (*)
                    FROM '||nomeutente||'.'||nometabellaPK||'
                    group by '||nomecolonnaPK||'
                    having count(*) > 1
                -- devo fare la UNION e aggiungere una riga per evitare eccezione di NO_DATA_FOUND
                -- , che in questo caso corrisponde a esito positivo, perch nessun record viola il vincolo!
                UNION
                        select 0 from dual';
      cnt       INT;
      cntdati   INT;
   BEGIN
     SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
-- INSERIRE SEMPRE IL TIPO del VINCOLO:
-- i tipi possibili sono U=unique, R=referential (chiave esterna),
-- P=Primary e C= check
         and cons.constraint_type  = 'P'
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaPK
         and cons_cols.COLUMN_NAME = nomecolonnaPK ;

      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN

       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ;
         IF (cntdati = 0)
         -- non esistono record che violano la chiave
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
        ELSE
            DBMS_OUTPUT.PUT_line('impossibile creare vincolo di primary key su colonna
                '||nomecolonnaPK||' della tabella '||nometabellaPK||'. Trovati record che violano il vincolo'  );
         END IF;
      END IF;
   END;
END;
/

BEGIN
--ALTER TABLE DPA_TRASM_SINGOLA ADD (CONSTRAINT FK_DPA_TRASM_SINGOLA_R01 FOREIGN KEY (ID_TRASMISSIONE) REFERENCES DPA_TRASMISSIONE (SYSTEM_ID) )

   DECLARE
      nomeutente        VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='DPA_TRASM_SINGOLA' ; 
      nomecolonnaFK VARCHAR2(2000) :='ID_TRASMISSIONE' ; 

      nometabellaPK VARCHAR2(2000) :='DPA_TRASMISSIONE'   ; 
      nomecolonnaPK VARCHAR2(2000) :='SYSTEM_ID' ; 

      -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
      nomeForeignKey VARCHAR2(2000) :='FK_DPA_TRASM_SINGOLA_R01' ;

     -- fine variabili da modifica
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ' ; 
      
     istruzioneCheck VARCHAR2(2000) := 'SELECT   (SELECT COUNT (*)
            FROM '||nomeutente||'.'||nometabellaFK||') -- meno le righe che soddisfano la condizione             
       - (SELECT COUNT (*)  FROM '||nomeutente||'.'||nometabellaFK||
            ' WHERE '||nomecolonnaFK||' IN (SELECT '||nomecolonnaPK
                                    ||' FROM '||nomeutente||'.'||nometabellaPK||')
                                             )  FROM DUAL';
    
      cnt       INT;
      cntdati   INT;
   
  BEGIN
      
      SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
         and cons.constraint_type  = 'R' 
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaFK
         and cons_cols.COLUMN_NAME = nomecolonnaFK ; 
      
      /*-- precedente era:
      SELECT COUNT (*) INTO cnt FROM all_constraints
       WHERE constraint_name = 'FK_DPA_FUNZIONI_DPA_TIPO_FUNZ'
         AND constraint_type = 'R' 
         and owner=nomeutente;
         */

      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
         
       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ; 
    /* precedente era select esplicita:     
         SELECT COUNT (f.id_tipo_funzione)  FROM @db_user.DPA_FUNZIONI f
          WHERE f.id_tipo_funzione NOT IN (SELECT tf.system_id FROM @db_user.DPA_TIPO_FUNZIONE tf);
          */

         IF (cntdati <> 0)
         -- esistono record che violano la chiave
         THEN
             istruzioneSQL := istruzioneSQL||' novalidate';
         END IF;

            EXECUTE IMMEDIATE istruzioneSQL; 
         
      END IF;
   END;

END;
/


BEGIN
--ALTER TABLE DPA_TRASM_SINGOLA ADD (CONSTRAINT FK_DPA_TRASM_SINGOLA_R02 FOREIGN KEY (ID_RAGIONE) REFERENCES DPA_RAGIONE_TRASM (SYSTEM_ID) )
   DECLARE
      nomeutente        VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='DPA_TRASM_SINGOLA' ; 
      nomecolonnaFK VARCHAR2(2000) :='ID_RAGIONE' ; 

      nometabellaPK VARCHAR2(2000) :='DPA_RAGIONE_TRASM'   ; 
      nomecolonnaPK VARCHAR2(2000) :='SYSTEM_ID' ; 

      nomeForeignKey VARCHAR2(2000) :='FK_DPA_TRASM_SINGOLA_R02' ; 
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ' ; 
      
           istruzioneCheck VARCHAR2(2000) := 'SELECT   (SELECT COUNT (*)
            FROM '||nomeutente||'.'||nometabellaFK||') -- meno le righe che soddisfano la condizione             
       - (SELECT COUNT (*)  FROM '||nomeutente||'.'||nometabellaFK||
            ' WHERE '||nomecolonnaFK||' IN (SELECT '||nomecolonnaPK
                                    ||' FROM '||nomeutente||'.'||nometabellaPK||')
                                             )  FROM DUAL';
                                             
        cnt int;
        cntdati int;

 BEGIN
      SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
         and cons.constraint_type  = 'R'
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaFK
         and cons_cols.COLUMN_NAME = nomecolonnaFK ; 

      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
         
       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ; 
   
         IF (cntdati <> 0)
         -- esistono record che violano la chiave
         THEN
             istruzioneSQL := istruzioneSQL||' novalidate';
         END IF;

            EXECUTE IMMEDIATE istruzioneSQL; 
         
      END IF;
   END;

END;
/
/*
AUTORE:							Stefano Frezza
Data creazione:					18/02/2011
Scopo della modifica:			Rilascio pubblicazioni
Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
INFOTN-AVVOCATURA
*/
BEGIN
   DECLARE

      check_DPA_PROFIL_STO VARCHAR2 (2000)
         := 'CREATE TABLE PUBLISHER_ERRORS
				(
				  ID                 NUMBER  PRIMARY KEY ,
				  PUBLISHINSTANCEID  NUMBER,
				  ERRORCODE          NVARCHAR2(255),
				  ERRORDESCRIPTION   NVARCHAR2(2000),
				  ERRORSTACK         NVARCHAR2(2000),
				  ERRORDATE          DATE
				) ' ;
--ALTER TABLE PUBLISHER_ERRORS ADD ( CONSTRAINT PUBLISH_ERRORS_PK  PRIMARY KEY (ID));
--CREATE UNIQUE INDEX PUBLISH_ERRORS_PK ON PUBLISHER_ERRORS (ID);     

	tabelle_esistenti EXCEPTION;
    PRAGMA EXCEPTION_INIT (tabelle_esistenti, -955);
   BEGIN
      EXECUTE IMMEDIATE check_DPA_PROFIL_STO;

   EXCEPTION
      WHEN tabelle_esistenti
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/



BEGIN
--ALTER TABLE PUBLISHER_ERRORS ADD (  CONSTRAINT FK_PUBLISHER_ERRORS 
-- FOREIGN KEY (PUBLISHINSTANCEID)  REFERENCES PUBLISHER_INSTANCES (ID)
--    ON DELETE CASCADE);
   DECLARE
      nomeutente        VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='PUBLISHER_ERRORS' ; 
      nomecolonnaFK VARCHAR2(2000) :='PUBLISHINSTANCEID' ; 

      nometabellaPK VARCHAR2(2000) :='PUBLISHER_INSTANCES'   ; 
      nomecolonnaPK VARCHAR2(2000) :='ID' ; 

      -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
      nomeForeignKey VARCHAR2(2000) :='FK_PUBLISHER_ERRORS' ; 

     -- fine variabili da modifica
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ON DELETE CASCADE ' ; 
      
           istruzioneCheck VARCHAR2(2000) := 'SELECT   (SELECT COUNT (*)
            FROM '||nomeutente||'.'||nometabellaFK||') -- meno le righe che soddisfano la condizione             
       - (SELECT COUNT (*)  FROM '||nomeutente||'.'||nometabellaFK||
            ' WHERE '||nomecolonnaFK||' IN (SELECT '||nomecolonnaPK
                                    ||' FROM '||nomeutente||'.'||nometabellaPK||')
                                             )  FROM DUAL';
    
      cnt       INT;
      cntdati   INT;
   
  BEGIN
      
      SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
         and cons.constraint_type  = 'R' 
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaFK
         and cons_cols.COLUMN_NAME = nomecolonnaFK ; 
      
      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
         
       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ; 

         IF (cntdati <> 0)
         -- esistono record che violano la chiave
         THEN
             istruzioneSQL := istruzioneSQL||' novalidate';
         END IF;

            EXECUTE IMMEDIATE istruzioneSQL; 
         
      END IF;
   END;

END;
/


/*
AUTORE:							Stefano Frezza
Data creazione:					18/02/2011
Scopo della modifica:			Rilascio pubblicazioni
Indicazione della MEV o dello sviluppo a cui  collegata la modifica:	
INFOTN-AVVOCATURA

CREATE UNIQUE INDEX PUBLISHER_EVENTS_PK ON PUBLISHER_EVENTS (ID);

ALTER TABLE PUBLISHER_EVENTS ADD (  CONSTRAINT PUBLISHER_EVENTS_PK
 PRIMARY KEY (ID));

*/

BEGIN
   DECLARE

      istruzioneSQL VARCHAR2 (2000)
         := 'CREATE TABLE PUBLISHER_EVENTS
			( ID                   NUMBER  PRIMARY KEY,
			  PUBLISHINSTANCEID    NUMBER,
			  EVENTNAME            NVARCHAR2(255),
			  OBJECTTYPE           NVARCHAR2(255),
			  OBJECTTEMPLATENAME   NVARCHAR2(255),
			  DATAMAPPERFULLCLASS  NVARCHAR2(2000),
			  LOADFILEIFDOCTYPE    CHAR(1 BYTE) ) ';
			tabelle_esistenti EXCEPTION;
    PRAGMA EXCEPTION_INIT (tabelle_esistenti, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzioneSQL;

   EXCEPTION
      WHEN tabelle_esistenti
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/


BEGIN
--ALTER TABLE PUBLISHER_EVENTS ADD (  CONSTRAINT FK_PUBLISHERINSTANCEID 
-- FOREIGN KEY (PUBLISHINSTANCEID)  REFERENCES PUBLISHER_INSTANCES (ID)
--    ON DELETE CASCADE);
   DECLARE
      nomeutente        VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='PUBLISHER_EVENTS' ; 
      nomecolonnaFK VARCHAR2(2000) :='PUBLISHINSTANCEID' ; 

      nometabellaPK VARCHAR2(2000) :='PUBLISHER_INSTANCES'   ; 
      nomecolonnaPK VARCHAR2(2000) :='ID' ; 

      -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
      nomeForeignKey VARCHAR2(2000) :='FK_PUBLISHERINSTANCEID' ; 

     -- fine variabili da modifica
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ON DELETE CASCADE ' ; 
      
           istruzioneCheck VARCHAR2(2000) := 'SELECT   (SELECT COUNT (*)
            FROM '||nomeutente||'.'||nometabellaFK||') -- meno le righe che soddisfano la condizione             
       - (SELECT COUNT (*)  FROM '||nomeutente||'.'||nometabellaFK||
            ' WHERE '||nomecolonnaFK||' IN (SELECT '||nomecolonnaPK
                                    ||' FROM '||nomeutente||'.'||nometabellaPK||')
                                             )  FROM DUAL';

      cnt       INT;
      cntdati   INT;
   
  BEGIN
      
      SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
         and cons.constraint_type  = 'R' 
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaFK
         and cons_cols.COLUMN_NAME = nomecolonnaFK ; 
      
      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
         
       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ; 

         IF (cntdati <> 0)
         -- esistono record che violano la chiave
         THEN
             istruzioneSQL := istruzioneSQL||' novalidate';
         END IF;

            EXECUTE IMMEDIATE istruzioneSQL; 
         
      END IF;
   END;

END;
/



/*
AUTORE:							Stefano Frezza
Data creazione:					18/02/2011
Scopo della modifica:			Rilascio pubblicazioni
Indicazione della MEV o dello sviluppo a cui  collegata la modifica:	INFOTN-AVVOCATURA

CREATE UNIQUE INDEX PUBLISH_RULES_PK ON SUBSCRIBER_RULES (ID);

ALTER TABLE SUBSCRIBER_RULES ADD (  CONSTRAINT PUBLISH_RULES_PK
 PRIMARY KEY (ID));

*/
BEGIN
   DECLARE

      istruzioneSQL VARCHAR2 (2000)
         := 'CREATE TABLE SUBSCRIBER_RULES
			( ID            NUMBER(10) PRIMARY KEY,
			  INSTANCEID    NUMBER(10),
			  NAME          NVARCHAR2(255)                  NOT NULL,
			  DESCRIPTION   NVARCHAR2(2000),
			  ENABLED       CHAR(1 BYTE)                    NOT NULL,
			  ORDINAL       INTEGER                         NOT NULL,
			  OPTIONS       NVARCHAR2(2000),
			  PARENTRULEID  NUMBER(10),
			  CLASS_ID      NVARCHAR2(2000),
			  SUBNAME       NVARCHAR2(255)
			)';
			tabelle_esistenti EXCEPTION;
    PRAGMA EXCEPTION_INIT (tabelle_esistenti, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzioneSQL;

   EXCEPTION
      WHEN tabelle_esistenti
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/




BEGIN
--ALTER TABLE SUBSCRIBER_RULES ADD (  CONSTRAINT FK_RULE_SUBRULES
-- FOREIGN KEY (PARENTRULEID)  REFERENCES SUBSCRIBER_RULES (ID)
--    ON DELETE CASCADE;
   DECLARE
      nomeutente        VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='SUBSCRIBER_RULES' ; 
      nomecolonnaFK VARCHAR2(2000) :='PARENTRULEID' ; 

      nometabellaPK VARCHAR2(2000) :='SUBSCRIBER_RULES'   ; 
      nomecolonnaPK VARCHAR2(2000) :='ID' ; 

      -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
      nomeForeignKey VARCHAR2(2000) :='FK_RULE_SUBRULES' ; 

     -- fine variabili da modifica
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ON DELETE CASCADE ' ; 
      
           istruzioneCheck VARCHAR2(2000) := 'SELECT   (SELECT COUNT (*)
            FROM '||nomeutente||'.'||nometabellaFK||') -- meno le righe che soddisfano la condizione             
       - (SELECT COUNT (*)  FROM '||nomeutente||'.'||nometabellaFK||
            ' WHERE '||nomecolonnaFK||' IN (SELECT '||nomecolonnaPK
                                    ||' FROM '||nomeutente||'.'||nometabellaPK||')
                                             )  FROM DUAL';
    
      cnt       INT;
      cntdati   INT;

  BEGIN
      
      SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
         and cons.constraint_type  = 'R' 
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaFK
         and cons_cols.COLUMN_NAME = nomecolonnaFK ; 
      
      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
         
       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ; 

         IF (cntdati <> 0)
         -- esistono record che violano la chiave
         THEN
             istruzioneSQL := istruzioneSQL||' novalidate';
         END IF;

            EXECUTE IMMEDIATE istruzioneSQL; 
         
      END IF;
   END;

END;
/




BEGIN
--ALTER TABLE SUBSCRIBER_RULES ADD (  CONSTRAINT FK_PUBLISH_INSTANCEID 
-- FOREIGN KEY (INSTANCEID)  REFERENCES SUBSCRIBER_INSTANCES (ID)
--    ON DELETE CASCADE);

   DECLARE
      nomeutente        VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='SUBSCRIBER_RULES' ; 
      nomecolonnaFK VARCHAR2(2000) :='INSTANCEID' ; 

      nometabellaPK VARCHAR2(2000) :='SUBSCRIBER_INSTANCES'   ; 
      nomecolonnaPK VARCHAR2(2000) :='ID' ; 

      -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
      nomeForeignKey VARCHAR2(2000) :='FK_PUBLISH_INSTANCEID' ; 

     -- fine variabili da modifica
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ON DELETE CASCADE ' ; 
      
      istruzioneCheck VARCHAR2(2000) := 'SELECT COUNT (*)
           FROM '||nomeutente||'.'||nometabellaFK||'
          WHERE '||nomecolonnaFK||' NOT IN (SELECT '||nomecolonnaPK||'
                                             FROM '||nomeutente||'.'||nometabellaPK||')';

      cnt       INT;
      cntdati   INT;
   
  BEGIN
      
      SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
         and cons.constraint_type  = 'R' 
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaFK
         and cons_cols.COLUMN_NAME = nomecolonnaFK ; 
      
      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
         
       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ; 

         IF (cntdati <> 0)
         -- esistono record che violano la chiave
         THEN
             istruzioneSQL := istruzioneSQL||' novalidate';
         END IF;

            EXECUTE IMMEDIATE istruzioneSQL; 
         
      END IF;
   END;

END;
/




/*
AUTORE:							Stefano Frezza
Data creazione:					18/02/2011
Scopo della modifica:			Rilascio pubblicazioni
Indicazione della MEV o dello sviluppo a cui  collegata la modifica:	INFOTN-AVVOCATURA

CREATE UNIQUE INDEX PUBLISH_HISTORY_PK ON SUBSCRIBER_HISTORY(ID);

ALTER TABLE SUBSCRIBER_HISTORY ADD (  CONSTRAINT PUBLISH_HISTORY_PK
 PRIMARY KEY (ID));

*/
BEGIN
   DECLARE

      istruzioneSQL VARCHAR2 (2000)
         := 'CREATE TABLE SUBSCRIBER_HISTORY
			( ID                   NUMBER(10)  PRIMARY KEY ,
			  RULEID               NUMBER(10),
			  IDOBJECT             NVARCHAR2(255)           NOT NULL,
			  OBJECTTEMPLATENAME   NVARCHAR2(255),
			  OBJECDESCRIPTION     NVARCHAR2(2000),
			  AUTHORNAME           NVARCHAR2(255),
			  AUTHORID             NVARCHAR2(50),
			  ROLENAME             NVARCHAR2(255),
			  ROLEID               NVARCHAR2(50),
			  OBJECTSNAPSHOT       CLOB,
			  COMPUTED             CHAR(1 BYTE)             NOT NULL,
			  COMPUTEDATE          DATE,
			  ERRORID              NVARCHAR2(255),
			  ERRORDESCRIPTION     NVARCHAR2(2000),
			  MAILMESSAGESNAPSHOT  CLOB,
			  OBJECTTYPE           NVARCHAR2(50),
			  ERRORSTACK           NVARCHAR2(2000)			)';
			tabelle_esistenti EXCEPTION;
    PRAGMA EXCEPTION_INIT (tabelle_esistenti, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzioneSQL;

   EXCEPTION
      WHEN tabelle_esistenti
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/


BEGIN
--ALTER TABLE SUBSCRIBER_HISTORY ADD (  CONSTRAINT FK_PUBLISH_RULEID 
-- FOREIGN KEY (RULEID)  REFERENCES SUBSCRIBER_RULES (ID)
--    ON DELETE CASCADE);

   DECLARE
      nomeutente        VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='SUBSCRIBER_HISTORY' ; 
      nomecolonnaFK VARCHAR2(2000) :='RULEID' ; 

      nometabellaPK VARCHAR2(2000) :='SUBSCRIBER_RULES'   ;
      nomecolonnaPK VARCHAR2(2000) :='ID' ; 

      -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
      nomeForeignKey VARCHAR2(2000) :='FK_PUBLISH_RULEID' ;

     -- fine variabili da modifica
      
      istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ON DELETE CASCADE ' ; 
      
      istruzioneCheck VARCHAR2(2000) := 'SELECT COUNT (*)
           FROM '||nomeutente||'.'||nometabellaFK||'
          WHERE '||nomecolonnaFK||' NOT IN (SELECT '||nomecolonnaPK||'
                                             FROM '||nomeutente||'.'||nometabellaPK||')';
    
      cnt       INT;
      cntdati   INT;
   
  BEGIN
      
      SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
         and cons.constraint_type  = 'R' 
         and cons.owner            = nomeutente
         and cons.table_name       = nometabellaFK
         and cons_cols.COLUMN_NAME = nomecolonnaFK ; 

      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN

       EXECUTE IMMEDIATE istruzioneCheck    INTO cntdati ; 

         IF (cntdati <> 0)
         -- esistono record che violano la chiave
         THEN
             istruzioneSQL := istruzioneSQL||' novalidate';
         END IF;

            EXECUTE IMMEDIATE istruzioneSQL; 

      END IF;
   END;

END;
/

/*
AUTORE:                      SAMUELE ALFREDO FURNARI
Data creazione:                  12/04/2011
Scopo della modifica:         AGGIUNGERE LA COLONNA MTEXT_FQN  NEL COMPONENTS
                
*/
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'COMPONENTS';
      nomecolonna   VARCHAR2 (32)  := 'MTEXT_FQN';
      tipodato      VARCHAR2 (200) := 'VARCHAR2(500)';
      cnt           INT;
      nomeutente        VARCHAR2 (200) := upper('@user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
            and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/

/*
AUTORE:                      P. De Luca
Data creazione:                  Luglio 2011
Scopo della modifica: Aggiungere la colonna "cha_infasato" per determinare 
	se il record  stato gi infasato o meno nella  DPA_CHIAVI_CONFIGURAZIONE
                
*/
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_CHIAVI_CONFIG_TEMPLATE';
      nomecolonna   VARCHAR2 (32)  := 'CHA_INFASATO';
      tipodato      VARCHAR2 (200) := 'VARCHAR2(1)';
	  tipodefault   VARCHAR2 (200) := ' DEFAULT ''Y'' ';
      
	  cnt           INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
            and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato
							  ||tipodefault;
         END IF;
      END IF;
   END;
END;
/

/*
AUTORE:					  AURORA ANDREACCHIO
Data creazione:				  05/04/2011
Scopo della modifica:	     AGGIUNGERE LA COLONNA VAR_CHIAVE_ARCA NEL DPA_CORR_GLOBALI
				
*/
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_CORR_GLOBALI';
      nomecolonna   VARCHAR2 (32)  := 'VAR_CHIAVE_AE';
      tipodato      VARCHAR2 (200) := ' VARCHAR2(12) default ''0'' ';
      cnt           INT;
      nomeutente		VARCHAR2 (200) := upper('@db_user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
			and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/


/*
AUTORE:					  P. Buono 
Data creazione:				  20/05/2011
Scopo della modifica:	     Disabilitazione alla ricezione trasmissione dei ruoli -
colonna CHA_DISABLED_TRASM VARCHAR2(1) NELLA DPA_CORR_GLOBALI
				
*/
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_CORR_GLOBALI';
      nomecolonna   VARCHAR2 (32)  := 'CHA_DISABLED_TRASM';
      tipodato      VARCHAR2 (200) := '  VARCHAR2(1)  ';
      cnt           INT;
      nomeutente		VARCHAR2 (200) := upper('@db_user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
			and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/

/*
AUTORE:					  AURORA ANDREACCHIO
Data creazione:				  05/04/2011
Scopo della modifica:		AGGIUNGERE DUE COLONNE CHA_SESSO E CHA_PROVINCIA_NASCITA NEL DPA_DETT_GLOBALI
				MODIFICHE TRE COLONNE VAR_INDIRIZZO,*CAP,*PROVINCIA

*/
BEGIN
   DECLARE
      nometabella    VARCHAR2 (32)  := 'DPA_DETT_GLOBALI';
      nomecolonna    VARCHAR2 (32)  := 'CHA_SESSO';
      nomecolonna1   VARCHAR2 (32)  := 'CHAR_PROVINCIA_NASCITA';
      nomecolonna2   VARCHAR2 (32)  := 'VAR_INDIRIZZO';
      nomecolonna3   VARCHAR2 (32)  := 'VAR_CAP';
      nomecolonna4   VARCHAR2 (32)  := 'VAR_PROVINCIA';
      tipodato       VARCHAR2 (200) := ' CHAR(1)';
      tipodato1      VARCHAR2 (200) := ' VARCHAR2(3)';
      tipodato2      VARCHAR2 (200) := ' VARCHAR2(256)';
      tipodato3      VARCHAR2 (200) := ' VARCHAR2(9)';
      tipodato4      VARCHAR2 (200) := ' VARCHAR2(3)';
      cnt            INT;
      nomeutente		VARCHAR2 (200) := upper('PAT_PRE');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
			and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna1
                              || ' '|| tipodato1;
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY COLUMN '|| nomecolonna2
                              || ' '|| tipodato2;
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY COLUMN '|| nomecolonna3
                              || ' '|| tipodato3;
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY COLUMN '|| nomecolonna4
                              || ' '|| tipodato4;
                             
         END IF;
      END IF;
   END;
END;
/

-- FLAG_WSPIA in DPA_EL_REGISTRI

BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_EL_REGISTRI';
      nomecolonna   VARCHAR2 (32)  := 'FLAG_WSPIA';
      tipodato      VARCHAR2 (200) := ' CHAR(1) ';
      cnt           INT;
	nomeutente		VARCHAR2 (200) := upper('@db_user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
			and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/
/*
AUTORE:						SPERILLI
Data creazione:				28/02/2011
Scopo della modifica:		INSERITO CAMPO PER TENERE MEMORIA DELLE 
							STAMPE EFFETTUATE DI OGNI PROTOCOLLO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV 3817
*/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='PRINTS_NUM';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD PRINTS_NUM INT';
        end if;
    end;
end;
/


-- ALTER TABLE SECURITY  ADD (TS_INSERIMENTO  TIMESTAMP(6) DEFAULT systimestamp); -- questa  la versione per Oracle
-- Autore: P. De Luca -- storico inserimento

BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'SECURITY';
      nomecolonna   VARCHAR2 (32)  := 'TS_INSERIMENTO';
      tipodato      VARCHAR2 (200) := ' TIMESTAMP(6) ';
	  tipodatodefault      VARCHAR2 (200) := ' DEFAULT systimestamp';
      cnt           INT;
	nomeutente		VARCHAR2 (200) := upper('@db_user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
			and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
		 EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY '|| nomecolonna
                              || ' '|| tipodatodefault;
         END IF;
      END IF;
   END;
END;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from user_tables where table_name='DPA_CACHE';
        if (cnt = 0) then
            execute immediate 

'CREATE TABLE DPA_CACHE
(
  DOCNUMBER          INTEGER,
  PATHCACHE          VARCHAR2(500 BYTE),
  IDAMMINISTRAZIONE  VARCHAR2(100 BYTE),
  AGGIORNATO         INTEGER,
  VERSION_ID         INTEGER,
  LOCKED             VARCHAR2(1 BYTE),
  COMPTYPE           VARCHAR2(3 BYTE),
  FILE_SIZE          INTEGER,
  ALTERNATE_PATH     VARCHAR2(128 BYTE),
  VAR_IMPRONTA       VARCHAR2(64 BYTE),
  EXT                VARCHAR2(7 BYTE),
  LAST_ACCESS        VARCHAR2(50 BYTE)
)';            
        end if;
    end;
end;
/
/*
AUTORE:					 AURORA ANDREACCHIO
Data creazione:				  05/04/2011
Scopo della modifica:		CREARE LA TABELLA DPA_DISPOSITIVO_STAMPA

data modifica:				12/04/2011
modifica:					aggiunta colonna "ENABLED" char(1) NOT NULL default 1 
*/
 
BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'create table @db_user.DPA_CONFIG_ANAGRAFICA_ESTERNA
             (SYSTEM_ID INTEGER  PRIMARY KEY,
              NOME_ANAGRAFICA_ESTERNA VARCHAR2(16)  NOT NULL,
              WSURL_ANAGRAFICA_ESTERNA VARCHAR2(256)  NOT NULL,
              INTEGRATION_ADAPTER_ID VARCHAR2(50)  NOT NULL,
              INTEGRATION_ADAPTER_VERSION VARCHAR2(8)  NOT NULL,
              CONFIG_PROVENIENZA VARCHAR2(2) ,
              CONFIG_MATRICOLA VARCHAR2(8) ,
              CONFIG_PASSWORD VARCHAR2(8) ,
              CONFIG_APPLICAZIONE VARCHAR2(8) ,
              CONFIG_RUOLO VARCHAR2(6) ,
              NOTE VARCHAR2(516), 
			  ENABLED char(1) default 1 NOT NULL  )';

        tabella_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (tabella_esistente, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN tabella_esistente
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from user_tables where table_name='DPA_CONFIG_CACHE';
        if (cnt = 0) then
            execute immediate 

'CREATE TABLE DPA_CONFIG_CACHE
(
  IDAMMINISTRAZIONE           VARCHAR2(100 BYTE),
  CACHING                     INTEGER,
  MASSIMA_DIMENSIONE_CACHING  FLOAT(126),
  MASSIMA_DIMENSIONE_FILE     FLOAT(126),
  DOC_ROOT_SERVER             VARCHAR2(255 BYTE),
  ORA_INIZIO_CACHE            VARCHAR2(10 BYTE),
  ORA_FINE_CACHE              VARCHAR2(10 BYTE),
  URLWSCACHING                VARCHAR2(500 BYTE),
  URL_WS_CACHING_LOCALE       VARCHAR2(500 BYTE),
  DOC_ROOT_SERVER_LOCALE      VARCHAR2(255 BYTE)
)';            
        end if;
    end;
end;
/

/*
AUTORE:				C. Ferlito
Data creazione:			01/04/2011
Scopo della modifica:           storicizzare le modifiche
*/
BEGIN
   DECLARE
      check_DPA_PROFIL_FASC_STO VARCHAR2 (2000)
         := 'CREATE TABLE @db_user.DPA_PROFIL_FASC_STO
      (
         systemId NUMBER(10,0)  NOT NULL,
         id_template NUMBER(10,0) not null,
         dta_modifica TIMESTAMP(3) not null,
         id_project NUMBER(10,0) not null,
         id_ogg_custom NUMBER(10,0) not null,
         id_people NUMBER(10,0),
         id_ruolo_in_uo NUMBER(10,0) not null,
         var_desc_modifica VARCHAR2(2000)
      )';

      tabella_esistente EXCEPTION;
      PRAGMA EXCEPTION_INIT (tabella_esistente, -955);

   BEGIN
      EXECUTE IMMEDIATE check_DPA_PROFIL_FASC_STO;

   EXCEPTION
      WHEN tabella_esistente
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/

/*
AUTORE:						C. Ferlito
Data creazione:				      01/04/2011
Scopo della modifica:		storicizzare modifiche

*/
BEGIN
   DECLARE
      check_DPA_PROFIL_STO VARCHAR2 (2000)
         := 'CREATE TABLE DPA_PROFIL_STO (
               systemId NUMBER(10,0)  NOT NULL,
               id_template NUMBER(10,0) not null,
               dta_modifica TIMESTAMP(3) not null,
               id_profile NUMBER(10,0) not null,
               id_ogg_custom NUMBER(10,0) not null,
               id_people NUMBER(10,0) not null,
               id_ruolo_in_uo NUMBER(10,0) not null,
               var_desc_modifica VARCHAR2(2000))';

      tabelle_esistenti EXCEPTION;
      PRAGMA EXCEPTION_INIT (tabelle_esistenti, -955);
   BEGIN
      EXECUTE IMMEDIATE check_DPA_PROFIL_STO;

   EXCEPTION
      WHEN tabelle_esistenti
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/




-- FE_CHECK_TITOLARIO_ATTIVO
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='FE_CHECK_TITOLARIO_ATTIVO';
    IF (cnt = 0) THEN

insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
   ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE
   ,CHA_MODIFICABILE,CHA_GLOBALE)
         values
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_CHECK_TITOLARIO_ATTIVO'
  ,'Chiave per l''attivazione di default del  titolario attivo nelle maschere di ricerca fascicoli'
  ,'0','F','1'
  ,'1','1');

    END IF;
    END;
END;
/

--FE_MULTI_STAMPA_ETICHETTA
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='FE_MULTI_STAMPA_ETICHETTA';
    IF (cnt = 0) THEN
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
	   ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE
	   ,CHA_MODIFICABILE,CHA_GLOBALE)
			 values
	  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_MULTI_STAMPA_ETICHETTA'
	  ,'Chiave di attivazione della stampa multipla delle etichette nei protocolli in entrata/uscita'
	  ,'0','F','1'
	  ,'1','1');

    END IF;
    END;
END;
/



BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='BE_SALVA_EMAIL_IN_LOCALE';
    IF (cnt = 0) THEN

-- il record serve per ....  <---- inserire commento
        insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
         values
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'BE_SALVA_EMAIL_IN_LOCALE','Abilita il salvatggio delle mail in locale ( 1 attivo 0 no)', '1','B','1','1','1');
    END IF;
    END;
END;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='FE_VISUAL_DOC_SMISTAMENTO';
    IF (cnt = 0) THEN

  insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
         values
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_VISUAL_DOC_SMISTAMENTO','Determina il default per la checkbox su Visualizza Documento su smistamento (1=checked, 0=not checked)', '1','F','1','1','1');
END IF;
    END;
END;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='FE_VISUAL_DOC_SMISTAMENTO';
    IF (cnt = 0) THEN

-- il record serve per la mev inps 3831
        insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
         values
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_VISUAL_DOC_SMISTAMENTO','Determina il default per la checkbox su Visualizza Documento su smistamento (1=checked, 0=not checked)', '1','F','1','1','1');
    END IF;
    END;
END;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE l where l.var_codice ='BE_PUBBLICAZIONI';

    if (cnt = 0) then
        Insert into DPA_CHIAVI_CONFIGURAZIONE
        (
            SYSTEM_ID,
            ID_AMM,
            VAR_CODICE,
            VAR_DESCRIZIONE,
            VAR_VALORE,
            CHA_TIPO_CHIAVE,
            CHA_VISIBILE,
            CHA_MODIFICABILE,
            CHA_GLOBALE
        )
        Values
        (
            SEQ.NEXTVAL,
            0,
            'BE_PUBBLICAZIONI',
            'Abilitazione dei servizi di pubblicazione dei contenuti (1 = abilitata, 0 = disabilitata)',
            '0',
            'B',
            '0',
            '1',
            '1'
         );
    end if;
  end;
end;
/



CREATE OR REPLACE FUNCTION @db_user.getFascPrimaria(iProfile number, idFascicolo number)
RETURN varchar2 IS fascPrimaria varchar2(1);
begin

     SELECT nvl(B.CHA_FASC_PRIMARIA,'0') into fascPrimaria
       FROM PROJECT A, PROJECT_COMPONENTS B
       WHERE A.SYSTEM_ID=B.PROJECT_ID
       AND B.LINK=iProfile and ID_FASCICOLO=idFascicolo
-- modifica by F. Veltri , deve tornare sempre un solo risultato anche in caso di sottofascicolo
	   and rownum<2;

	   return fascPrimaria;

 EXCEPTION
	WHEN OTHERS
	THEN
	fascPrimaria := '0';

return fascPrimaria;
end getFascPrimaria;
/

CREATE OR REPLACE FUNCTION @db_user.iscorrispondenteinterno (
   idcorrglobali   INT,
   idregistro      INT
)
   RETURN NUMBER
IS
   RESULT   NUMBER;
BEGIN
   DECLARE
      tipourp                 VARCHAR (1);
      tipoie                  VARCHAR (1);
      idpeople                INT;
      numero_corrispondenti   INT;
   BEGIN
      tipoie := 'E';
      RESULT := 0;

      SELECT a.cha_tipo_ie
        INTO tipoie
        FROM dpa_corr_globali a
       WHERE a.system_id = idcorrglobali;

      IF (tipoie = 'I')
      THEN
         SELECT a.cha_tipo_urp
           INTO tipourp
           FROM dpa_corr_globali a
          WHERE a.system_id = idcorrglobali;

         IF (tipourp = 'U')
         THEN
            SELECT COUNT (*)
              INTO numero_corrispondenti
              FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
             WHERE cha_tipo_ie = 'I'
               AND cha_tipo_urp = 'R'
               AND id_uo = idcorrglobali
               AND f.id_ruolo_in_uo = a.system_id
               AND f.id_registro = r.system_id
               AND r.system_id = idregistro
               AND r.cha_rf = '0';
         END IF;

         IF (tipourp = 'R')
         THEN
            SELECT COUNT (*)
              INTO numero_corrispondenti
              FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
             WHERE cha_tipo_ie = 'I'
               AND cha_tipo_urp = 'R'
               AND a.system_id = idcorrglobali
               AND f.id_ruolo_in_uo = a.system_id
               AND f.id_registro = r.system_id
               AND r.system_id = idregistro
               AND r.cha_rf = '0';
         END IF;

         IF (tipourp = 'P')
         THEN
            BEGIN
               SELECT a.id_people
                 INTO idpeople
                 FROM dpa_corr_globali a
                WHERE a.system_id = idcorrglobali;

               SELECT   COUNT (a.system_id)
                   INTO numero_corrispondenti
                   FROM dpa_corr_globali a,
                        peoplegroups b,
                        dpa_l_ruolo_reg f,
                        dpa_el_registri r
                  WHERE a.id_gruppo = b.groups_system_id
                    AND b.dta_fine IS NULL
                    AND b.people_system_id = idpeople
                    AND f.id_ruolo_in_uo = a.system_id
                    AND f.id_registro = r.system_id
                    AND r.system_id = idregistro
                    AND r.cha_rf = '0'
               ORDER BY a.system_id DESC;
            END;
         END IF;
      END IF;

      IF (numero_corrispondenti > 0)
      THEN
         RESULT := 1;
      END IF;

      RETURN RESULT;
   END;
END iscorrispondenteinterno;
/
CREATE OR REPLACE PACKAGE PUBLISHER AS

    TYPE T_SUB_CURSOR IS REF CURSOR;

    -- Avvio del servizio di pubblicazione
    PROCEDURE StartService(p_Id NUMBER, 
                            p_StartDate DATE,
                            p_MachineName NVARCHAR2);
                            
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
CREATE OR REPLACE PACKAGE BODY PUBLISHER AS
   
    -- Avvio del servizio di pubblicazione
    PROCEDURE StartService(p_Id NUMBER, 
                            p_StartDate DATE,
                            p_MachineName NVARCHAR2)
        IS
    BEGIN
        UPDATE  PUBLISHER_INSTANCES
        SET     EXECUTIONCOUNT = 0, -- Ogni volta che avvia il servizio, azzera i contatori
                PUBLISHEDOBJECTS = 0,
                STARTEXECUTIONDATE = p_StartDate,
                ENDEXECUTIONDATE = NULL,
                MACHINENAME = p_MachineName
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
            MACHINENAME = NULL
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
CREATE OR REPLACE PACKAGE SUBSCRIBER AS

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
CREATE OR REPLACE PACKAGE BODY SUBSCRIBER AS

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


begin
Insert into @db_user.DPA_DOCSPA
   (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values
   (seq.nextval, sysdate, '3.14');
end;
/

