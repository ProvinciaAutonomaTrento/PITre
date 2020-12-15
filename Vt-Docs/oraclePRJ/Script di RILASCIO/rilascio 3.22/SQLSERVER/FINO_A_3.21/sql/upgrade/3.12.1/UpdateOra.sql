
-- il campo deve avere lunghezza identica a quella di PROFILE.VAR_PROF_OGGETTO

BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_NOTIFICA';
      nomecolonna   VARCHAR2 (32)  := 'VAR_OGGETTO';
      tipodato      VARCHAR2 (200) := ' VARCHAR2(2000) ';

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

         IF (cnt = 1)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/


BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_LOG_STORICO';
      nomecolonna   VARCHAR2 (32)  := 'USERID_OPERATORE';
      tipodato      VARCHAR2 (200) := '   VARCHAR2(32) ';

      cnt           INT;
      nomeutente    VARCHAR2 (200) := upper('@db_user');

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

         IF (cnt = 1)
         -- ok la colonna esiste
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/




BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_LOG';    -- 'DPA_LOG_STORICO';
      nomecolonna   VARCHAR2 (32)  := 'USERID_OPERATORE';
      tipodato      VARCHAR2 (200) := '   VARCHAR2(32) ';

      cnt           INT;
      nomeutente    VARCHAR2 (200) := upper('@db_user');

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

         IF (cnt = 1)
         -- ok la colonna esiste
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/

-- Inizio script P. De Luca 22-10-2010
BEGIN
   DECLARE
      cnt       INT;
      cntdati   INT;
   BEGIN
      SELECT COUNT (*) INTO cnt FROM user_tables ut
       WHERE ut.table_name = 'DPA_CHIAVI_CONFIGURAZIONE';

      IF (cnt = 1)                                     -- ok esiste la tabella
      THEN
         SELECT COUNT (*) INTO cnt FROM user_constraints uc
          WHERE uc.table_name = 'DPA_CHIAVI_CONFIGURAZIONE'
            AND uc.constraint_name = 'DPA_CHIAVI_CONFIGURAZIONE_C01';

         SELECT COUNT (*) INTO cntdati
           FROM dpa_chiavi_configurazione
          WHERE id_amm = 0 AND cha_globale = '0';

         IF (cnt = 0 AND cntdati = 0)
         THEN
            EXECUTE IMMEDIATE 'ALTER TABLE DPA_CHIAVI_CONFIGURAZIONE ADD
CONSTRAINT DPA_CHIAVI_CONFIGURAZIONE_C01
 CHECK (nvl(cha_globale,1) + nvl(id_amm,1) > 0)
 ENABLE
 VALIDATE ';
         END IF;

         IF (cnt = 0 AND cntdati > 0)
         THEN
            EXECUTE IMMEDIATE 'ALTER TABLE DPA_CHIAVI_CONFIGURAZIONE ADD
CONSTRAINT DPA_CHIAVI_CONFIGURAZIONE_C01
 CHECK (nvl(cha_globale,1) + nvl(id_amm,1) > 0)
 ENABLE
 NOVALIDATE ';
         END IF;
      END IF;
   END;
END;
/


begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='LAST_FORWARD';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD LAST_FORWARD NUMBER(10) DEFAULT -1';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='FORWARDING_SOURCE';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD FORWARDING_SOURCE NUMBER(10) DEFAULT -1';
        end if;
    end;
end;
/

-- creazione tabelle per modulo cache Ferro G.
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
-- fine creazione tabelle per modulo cache Ferro G.



-- STAMPA REPORT NO SECURITY -- F. Veltri 
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='STAMPA_REG_NO_SEC';
    IF (cnt = 0) THEN        
       INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) VALUES ('STAMPA_REG_NO_SEC' , 'Abilita utente a stampare registri senza controllo sulla sicurezza', NULL, 'N');
    END IF;
    END;
END;
/


-- modifica #1 colonna COD_PEOPLE_DELEGANTE
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_DELEGHE';
      nomecolonna   VARCHAR2 (32)  := 'COD_PEOPLE_DELEGANTE';
      tipodato      VARCHAR2 (200) := ' VARCHAR2(256) ';

      cnt           INT;
	  nomeutente	VARCHAR2 (200) := upper('@db_user');

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

         IF (cnt = 1)
		 -- ok la colonna esiste
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/

-- modifica #2, colonna COD_RUOLO_DELEGANTE
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_DELEGHE';
      nomecolonna   VARCHAR2 (32)  := 'COD_RUOLO_DELEGANTE';
      tipodato      VARCHAR2 (200) := ' VARCHAR2(256) ';

      cnt           INT;
	  nomeutente	VARCHAR2 (200) := upper('@db_user');

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

         IF (cnt = 1)
		 -- ok la colonna esiste
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/

-- modifica #3, colonna COD_PEOPLE_DELEGATO
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_DELEGHE';
      nomecolonna   VARCHAR2 (32)  := 'COD_PEOPLE_DELEGATO';
      tipodato      VARCHAR2 (200) := ' VARCHAR2(256) ';

      cnt           INT;
	  nomeutente	VARCHAR2 (200) := upper('@db_user');

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

         IF (cnt = 1)
		 -- ok la colonna esiste
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/


-- modifica #4, colonna COD_RUOLO_DELEGATO
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_DELEGHE';
      nomecolonna   VARCHAR2 (32)  := 'COD_RUOLO_DELEGATO';
      tipodato      VARCHAR2 (200) := ' VARCHAR2(256) ';

      cnt           INT;
	  nomeutente	VARCHAR2 (200) := upper('@db_user');

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

         IF (cnt = 1)
		 -- ok la colonna esiste
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/




BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := UPPER('DPA_EL_REGISTRI');
      nomecolonna   VARCHAR2 (32)  := UPPER('VAR_INBOX_IMAP');
      tipodato      VARCHAR2 (200) := ' VARCHAR2(128) NULL';

        cnt           INT;

        nomeutente  VARCHAR2 (200) := upper('@db_user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = nometabella and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
            and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
         END IF;

            IF (CNT = 1)
            then 
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente||'.'|| nometabella
                              || ' MODIFY '|| nomecolonna 
                              || '  VARCHAR2(128)';
            END IF;

      END IF;
   END;
END;
/

-- CREATE INDEX DPA_ASSOCIAZIONE_TEMPL_INDX01 ON DPA_ASSOCIAZIONE_TEMPLATES(ID_OGGETTO)
BEGIN
   DECLARE
     istruzioneSQL VARCHAR2(2000) := 'CREATE INDEX 
			@db_user.DPA_ASSOCIAZIONE_TEMPL_INDX01 
			ON DPA_ASSOCIAZIONE_TEMPLATES(ID_OGGETTO) ' ; 
      
      cnt       INT;
	  nomeutente		VARCHAR2 (200) := upper('@db_user');
   BEGIN
     SELECT COUNT (*) INTO cnt
        FROM all_indexes
       WHERE index_name = upper('DPA_ASSOCIAZIONE_TEMPL_INDX01')
       and table_name=upper('DPA_ASSOCIAZIONE_TEMPLATES')
	   and owner=nomeutente;

      IF (cnt = 0)
      THEN
            EXECUTE IMMEDIATE istruzioneSQL;
      END IF;
   END;
END;
/


BEGIN   
   DECLARE
   indiceesiste EXCEPTION;
   PRAGMA EXCEPTION_INIT(indiceesiste, -1408);
	
	nomeutente        VARCHAR2 (200) := upper('@db_user');
   istruzioneSQL VARCHAR2(2000) := 'CREATE INDEX '||
            nomeutente||'.DPA_ASS_TEMPL_FASC_INDX01 
            ON dpa_ass_templates_fasc(ID_OGGETTO) ' ; 
      
   cnt       INT;
   
   BEGIN
     SELECT COUNT (*) INTO cnt
        FROM all_indexes
       WHERE index_name = upper('DPA_ASS_TEMPL_FASC_INDX01')
       and table_name=upper('dpa_ass_templates_fasc')
       and owner=nomeutente;

      IF (cnt = 0)
      THEN
            EXECUTE IMMEDIATE istruzioneSQL;
      END IF;

EXCEPTION 
WHEN indiceesiste
THEN DBMS_OUTPUT.PUT_LINE('esiste già un indice per questa lista di colonne'); 

   END;
END;
/


BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '' ;

      cnt       INT;
      cntdati   INT;
      nomeutente		VARCHAR2(200) := upper('@db_user');
   BEGIN
      SELECT COUNT (*) INTO cnt FROM user_constraints
       WHERE table_name = 'PEOPLE'
         AND constraint_type = 'P'
		 and owner=nomeutente;

istruzioneSQL := 'ALTER TABLE '||nomeutente||'.PEOPLE ADD CONSTRAINT PEOPLE_PK PRIMARY KEY (SYSTEM_ID) ' ;

      IF (cnt = 0)
      THEN
                  EXECUTE IMMEDIATE istruzioneSQL;
            END IF;
   END;
END;
/



BEGIN
   DECLARE
      istruzioneSQL VARCHAR2(2000) := '' ;

      cnt       INT;
      cntdati   INT;
      nomeutente		VARCHAR2(200) := upper('@db_user');
   BEGIN
      SELECT COUNT (*) INTO cnt FROM user_constraints
       WHERE table_name = 'PROFILE'
         AND constraint_type = 'P'
		 and owner=nomeutente;

		istruzioneSQL := 'ALTER TABLE '||nomeutente||'.PROFILE ADD CONSTRAINT PROFILE_PK PRIMARY KEY (SYSTEM_ID) ' ;

      IF (cnt = 0)
      THEN
                  EXECUTE IMMEDIATE istruzioneSQL;
      END IF;
   END;
END;
/





BEGIN
    DECLARE cnt int;
     cntdati int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM user_sequences where sequence_name='SEQ_DPA_CHIAVI_CONFIG';
    select max(nvl(system_id,0)) +1 into cntdati from dpa_chiavi_configurazione; 
    IF (cnt = 0) THEN        
		  execute immediate ' CREATE SEQUENCE SEQ_DPA_CHIAVI_CONFIG
		  START WITH '||cntdati||'
		  MAXVALUE 999999999999999999999999999
		  MINVALUE 1
		  NOCYCLE
		  CACHE 20
		  NOORDER';
    END IF;
    END;
END;
/





begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_ATTO';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' and column_name='PATH_MOD_SU';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_TIPO_ATTO ADD PATH_MOD_SU VARCHAR(255)';
        end if;
    end if;
    end;
end;
/

BEGIN
   DECLARE
      cnt       INT;
      cntdati   INT;
   BEGIN
      SELECT COUNT (*) INTO cnt
        FROM user_indexes
       WHERE index_name = 'DPA_CHIAVI_CONFIG_IDX';

      SELECT COUNT (*) INTO cntdati
        FROM DUAL
       WHERE EXISTS (SELECT   id_amm, var_codice, COUNT (*)
                         FROM dpa_chiavi_configurazione
                     GROUP BY id_amm, var_codice
                       HAVING COUNT (*) > 1);

      IF (cnt = 0 AND cntdati = 0)
      THEN
         EXECUTE IMMEDIATE '    
                    CREATE UNIQUE INDEX DPA_CHIAVI_CONFIG_IDX 
                    ON DPA_CHIAVI_CONFIGURAZIONE
                    (ID_AMM, VAR_CODICE) ';
      END IF;
   END;
END;
/


begin
    declare   cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_TIMESTAMP_DOC';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_TIMESTAMP_DOC START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_TIMESTAMP_DOC';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_TIMESTAMP_DOC ' ||
            '( ' ||
            'SYSTEM_ID INT NOT NULL, ' ||
            'DOC_NUMBER INT, ' ||
			'VERSION_ID INT, ' ||
			'ID_PEOPLE INT, ' ||
			'DTA_CREAZIONE DATE, ' ||
			'DTA_SCADENZA DATE, ' ||
			'NUM_SERIE VARCHAR2(64), ' ||
			'S_N_CERTIFICATO VARCHAR2(64), ' ||
			'ALG_HASH VARCHAR2(64), ' ||
			'SOGGETTO VARCHAR2(64), ' ||
			'PAESE VARCHAR2(64), ' ||
			'TSR_FILE CLOB, ' ||
			'CONSTRAINT PK_DPA_TIMESTAMP_DOC PRIMARY KEY (SYSTEM_ID) ' ||    
            ')';
        end if;
    end;    
end;    
/ 




begin
    declare   cnt int;
    begin
    select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_LETTERE_DOCUMENTI';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_LETTERE_DOCUMENTI START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;   
	select count(*) into cnt from user_tables where table_name='DPA_LETTERE_DOCUMENTI';
		if (cnt = 0) then
		  execute immediate    
				'CREATE TABLE DPA_LETTERE_DOCUMENTI ' ||
				'( ' ||
				'SYSTEM_ID NUMBER PRIMARY KEY NOT NULL, ' ||
				'CODICE VARCHAR(100) NOT NULL, ' ||
				'DESCRIZIONE VARCHAR(255) NOT NULL, ' ||  
				'ETICHETTA VARCHAR(255) NOT NULL ' ||  
				') ';
		end if;
    end;    
end;    
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_LETTERE_DOCUMENTI;
	if (cnt = 0) then	
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'A','A','Arrivo');
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'P','P','Partenza');
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'I','I','Interno');
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'G','NP','NP');
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'ALL','ALL','Allegato');	
	end if;
	end;
end;
/

begin
    declare   cnt int;
    begin
	select count(*) into cnt from user_tables where table_name='DPA_ASS_LETTERE_DOCUMENTI';
		if (cnt = 0) then
		  execute immediate    
				'CREATE TABLE DPA_ASS_LETTERE_DOCUMENTI ' ||
				'( ' ||
				'ID_AMM NUMBER NOT NULL, ' ||
				'ID_LETTERADOC NUMBER NOT NULL, ' ||
				'DESCRIZIONE VARCHAR(255) NOT NULL, ' ||  
				'ETICHETTA VARCHAR(255) NOT NULL ' || 
				') ';
		end if;
    end;    
end;   
/

/* Inserimento voci per definizione funzioni di importazione fascicoli / documenti / RDE */
begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='IMP_FASC';
    if (cnt = 0) then        
       insert into @db_user.DPA_ANAGRAFICA_FUNZIONI VALUES('IMP_FASC','Abilita la voce di menu'' Import Fascicoli sotto Ricerca', '', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='IMP_DOCS';
    if (cnt = 0) then        
       insert into @db_user.DPA_ANAGRAFICA_FUNZIONI VALUES('IMP_DOCS','Abilita la voce di menu'' Import Documenti sotto Documenti', '', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='IMP_RDE';
    if (cnt = 0) then        
       insert into @db_user.DPA_ANAGRAFICA_FUNZIONI VALUES('IMP_RDE','Abilita la voce di menu'' Import Emergenza sotto Gestione', '', 'N');
    end if;
    end;
end;
/

--Elenco note
begin
    declare   cnt int;
    begin
    select count(*) into cnt from user_tables where table_name='DPA_ELENCO_NOTE';
        if (cnt = 0) then
          execute immediate    
                'CREATE TABLE DPA_ELENCO_NOTE ' ||
                '( ' ||
                'SYSTEM_ID NUMBER NOT NULL, ' ||
                'ID_REG_RF NUMBER, ' ||
                'VAR_DESC_NOTA VARCHAR2(256 BYTE), ' ||
				'COD_REG_RF VARCHAR2(128 BYTE), ' || 
                'CONSTRAINT PK_DPA_ELENCO_NOTE PRIMARY KEY (SYSTEM_ID) ' ||  
                ') ';
        end if;
    end;    
end;    
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='ELENCO_NOTE';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('ELENCO_NOTE','Abilita il sottomenu'' Elenco note dal menu'' Gestione','', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='IMPORT_ELENCO_NOTE';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('IMPORT_ELENCO_NOTE','Abilita l''import di un elenco di note da foglio excel','', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='RICERCA_NOTE_ELENCO';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('RICERCA_NOTE_ELENCO','Abilita la ricerca delle note da un elenco','', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='INSERIMENTO_NOTERF';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('INSERIMENTO_NOTERF','Abilita l''inserimento delle note associate ad un dato rf','', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='DO_TIMESTAMP';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI  (COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED)	
			values ('DO_TIMESTAMP', 'Abilita l''utente all''utilizzo della funzione del timestamp', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_NOTE' and column_name='IDRFASSOCIATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_NOTE ADD IDRFASSOCIATO INTEGER';
    end if;
    end;
end;
/


/*INIZIO VISIBILITA CAMPI PROFILAZIONE DINAMICA*/

--Creazione Tabella DPA_A_R_OGG_CUSTOM_DOC
begin
    declare cnt int;
    begin
      select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_A_R_OGG_CUSTOM_DOC';
      if (cnt = 0) then
         execute immediate 'CREATE SEQUENCE SEQ_DPA_A_R_OGG_CUSTOM_DOC START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
      end if;     
  
      select count(*) into cnt from user_tables where table_name='DPA_A_R_OGG_CUSTOM_DOC';
        if (cnt = 0) then
        execute immediate     
        'CREATE TABLE DPA_A_R_OGG_CUSTOM_DOC ' ||
            '( ' ||
            'SYSTEM_ID INT NOT NULL, ' ||
            'ID_TEMPLATE INT, ' ||
      'ID_OGGETTO_CUSTOM INT, ' ||
        'ID_RUOLO INT, ' ||
            'INS_MOD INT, ' ||
            'VIS INT, ' ||
        'CONSTRAINT PK_DPA_A_R_OGG_CUSTOM_DOC PRIMARY KEY (SYSTEM_ID) ' ||  
            ')';
        end if;
        
        select count(*) into cnt from user_indexes where table_name='DPA_A_R_OGG_CUSTOM_DOC' and index_name='INDX_DPA_A_R_DOC1';
        if (cnt = 0) then
            execute immediate 'CREATE INDEX INDX_DPA_A_R_DOC1 ON DPA_A_R_OGG_CUSTOM_DOC (ID_TEMPLATE, ID_OGGETTO_CUSTOM, ID_RUOLO)';
        end if;
    end;    
end;  
/
 
--Creazione Tabella DPA_A_R_OGG_CUSTOM_FASC
begin
    declare cnt int;
    begin
      select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_A_R_OGG_CUSTOM_FASC';
      if (cnt = 0) then
         execute immediate 'CREATE SEQUENCE SEQ_DPA_A_R_OGG_CUSTOM_FASC START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
      end if;     
  
      select count(*) into cnt from user_tables where table_name='DPA_A_R_OGG_CUSTOM_FASC';
        if (cnt = 0) then
        execute immediate     
        'CREATE TABLE DPA_A_R_OGG_CUSTOM_FASC ' ||
            '( ' ||
            'SYSTEM_ID INT NOT NULL, ' ||
            'ID_TEMPLATE INT, ' ||
      'ID_OGGETTO_CUSTOM INT, ' ||
        'ID_RUOLO INT, ' ||
            'INS_MOD INT, ' ||
            'VIS INT, ' ||
        'CONSTRAINT PK_DPA_A_R_OGG_CUSTOM_FASC PRIMARY KEY (SYSTEM_ID) ' || 
            ')';
        end if;
        
        select count(*) into cnt from user_indexes where table_name='DPA_A_R_OGG_CUSTOM_FASC' and index_name='INDX_DPA_A_R_FASC1';
        if (cnt = 0) then
            execute immediate 'CREATE INDEX INDX_DPA_A_R_FASC1 ON DPA_A_R_OGG_CUSTOM_FASC (ID_TEMPLATE, ID_OGGETTO_CUSTOM, ID_RUOLO)';
        end if;
    end;    
end;  
/
 
--Buono Paolo 18/02/2011

--1)Per ogni amministrazione vengono estratte le tipologie di documento con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che hanno diritti di inserimento e ricerca su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede già dei diritti, altrimenti gli vengono assegnati pieni diritti sul campo (Inserimento,Modifica,Visibilità)
--DIRITTI 2(Inserimento/Ricerca) SULLA DPA_VIS_TIPO_DOC, INSERIMENTO NELLA DPA_A_R_OGG_CUSTOM_DOC CON DIRITTO INS_MOD(Inserimento/Modifica)=1 e VIS(Visiblità)=1
BEGIN
    DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_atto where id_amm is not null;
    par_id_template int;
    par_id_oggetto_Custom int;
    par_id_ruolo int;
    par_id_amm int;
    cnt int;
    BEGIN  OPEN c_idTemplate;
        LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
        EXIT WHEN c_idTemplate%NOTFOUND;
        
        DECLARE CURSOR c_oggCustom IS select dpa_associazione_templates.id_oggetto from dpa_associazione_templates, dpa_oggetti_custom where dpa_associazione_templates.id_template = par_id_template and dpa_associazione_templates.doc_number IS NULL and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id;
        BEGIN  OPEN c_oggCustom;
            LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
            EXIT WHEN c_oggCustom%NOTFOUND;
            
			--Inserimento e Ricerca Tipologia
			DECLARE CURSOR c_idRuoli IS select id_ruolo from dpa_vis_tipo_doc where id_tipo_doc = par_id_template and diritti = 2;
			
			BEGIN OPEN c_idRuoli;
                LOOP FETCH c_idRuoli INTO par_id_ruolo;
                EXIT WHEN c_idRuoli%NOTFOUND;  
                
                    select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_DOC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                    if(cnt = 0) then
                        --Inserimento/Modifica e Visibilità sul campo
						insert into DPA_A_R_OGG_CUSTOM_DOC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_DOC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 1, 1);
			        end if;
                    
                END LOOP;
                CLOSE c_idRuoli;
            END;
            
            END LOOP;
            CLOSE c_oggCustom;
        END;        
        
        END LOOP;
        CLOSE c_idTemplate;
    END;
END;
/


--1)Per ogni amministrazione vengono estratte le tipologie di documento con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che hanno diritti di ricerca su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede già dei diritti, altrimenti gli viene assegnato il diritto di visibilità sul campo
--DIRITTI 1(Ricerca) SULLA DPA_VIS_TIPO_DOC, INSERIMENTO NELLA DPA_A_R_OGG_CUSTOM_DOC CON DIRITTO INS_MOD(Inserimento/Modifica)=0 e VIS(Visiblità)=1
BEGIN
    DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_atto where id_amm is not null;
    par_id_template int;
    par_id_oggetto_Custom int;
    par_id_ruolo int;
    par_id_amm int;
    cnt int;
    BEGIN  OPEN c_idTemplate;
        LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
        EXIT WHEN c_idTemplate%NOTFOUND;
        
        DECLARE CURSOR c_oggCustom IS select dpa_associazione_templates.id_oggetto from dpa_associazione_templates, dpa_oggetti_custom where dpa_associazione_templates.id_template = par_id_template and dpa_associazione_templates.doc_number IS NULL and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id;
        BEGIN  OPEN c_oggCustom;
            LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
            EXIT WHEN c_oggCustom%NOTFOUND;
            
			--Ricerca tipologia
            DECLARE CURSOR c_idRuoli IS select id_ruolo from dpa_vis_tipo_doc where id_tipo_doc = par_id_template and diritti = 1;
			
			BEGIN OPEN c_idRuoli;
                LOOP FETCH c_idRuoli INTO par_id_ruolo;
                EXIT WHEN c_idRuoli%NOTFOUND;  
                
                    select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_DOC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                    if(cnt = 0) then
            			--Visibilità sul campo
						insert into DPA_A_R_OGG_CUSTOM_DOC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_DOC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 0, 1);
					end if;
                    
                END LOOP;
                CLOSE c_idRuoli;
            END;
            
            END LOOP;
            CLOSE c_oggCustom;
        END;        
        
        END LOOP;
        CLOSE c_idTemplate;
    END;
END;
/


--1)Per ogni amministrazione vengono estratte le tipologie di documento con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che non hanno diritti su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede già dei diritti, altrimenti non gli viene assegnato nessun diritto sul campo
--DIRITTI 0(Nessun diritto) SULLA DPA_VIS_TIPO_DOC, INSERIMENTO NELLA DPA_A_R_OGG_CUSTOM_DOC CON DIRITTO INS_MOD(Inserimento/Modifica)=0 e VIS(Visiblità)=0
BEGIN
    DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_atto where id_amm is not null;
    par_id_template int;
    par_id_oggetto_Custom int;
    par_id_ruolo int;
    par_id_amm int;
    cnt int;
    BEGIN  OPEN c_idTemplate;
        LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
        EXIT WHEN c_idTemplate%NOTFOUND;
        
        DECLARE CURSOR c_oggCustom IS select dpa_associazione_templates.id_oggetto from dpa_associazione_templates, dpa_oggetti_custom where dpa_associazione_templates.id_template = par_id_template and dpa_associazione_templates.doc_number IS NULL and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id;
        BEGIN  OPEN c_oggCustom;
            LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
            EXIT WHEN c_oggCustom%NOTFOUND;
            
			--Nessun diritto sulla tipologia
			DECLARE CURSOR c_idRuoli IS select id_ruolo from dpa_vis_tipo_doc where id_tipo_doc = par_id_template and diritti = 0;
			
			BEGIN OPEN c_idRuoli;
                LOOP FETCH c_idRuoli INTO par_id_ruolo;
                EXIT WHEN c_idRuoli%NOTFOUND;  
                
                    select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_DOC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                    if(cnt = 0) then
            			--Nessun diritto sul campo
						insert into DPA_A_R_OGG_CUSTOM_DOC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_DOC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 0, 0);
                    end if;
                    
                END LOOP;
                CLOSE c_idRuoli;
            END;
            
            END LOOP;
            CLOSE c_oggCustom;
        END;        
        
        END LOOP;
        CLOSE c_idTemplate;
    END;
END;
/


--1)Per ogni amministrazione vengono estratte le tipologie di documento con i loro campi di profilazione
--2)Vengono selezionati tutti i ruoli dell'amministrazione
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede già dei diritti, altrimenti non gli viene assegnato nessun diritto sul campo
--PER TUTTI I RUOLI NON CENSITI NELLA DPA_A_R_OGG_CUSTOM_DOC, INSERIMENTO NELLA DPA_A_R_OGG_CUSTOM_DOC CON DIRITTO INS_MOD(Inserimento/Modifica)=1 e VIS(Visiblità)=1
--Quest'ultimo script serve a dare comunque visibilità sui campi, anche ai ruoli che non possono inserire o ricercare la specifica tipologia
--Questo per evitare che un documento trasmesso ad un utente senza diritti di inserimento e ricerca su una tipologia, si presenti senza campi di profilazione
BEGIN
    DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_atto where id_amm is not null;
    par_id_template int;
    par_id_oggetto_Custom int;
    par_id_ruolo int;
    par_id_amm int;
    cnt int;
    BEGIN  OPEN c_idTemplate;
        LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
        EXIT WHEN c_idTemplate%NOTFOUND;
        
        DECLARE CURSOR c_oggCustom IS select dpa_associazione_templates.id_oggetto from dpa_associazione_templates, dpa_oggetti_custom where dpa_associazione_templates.id_template = par_id_template and dpa_associazione_templates.doc_number IS NULL and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id;
        BEGIN  OPEN c_oggCustom;
            LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
            EXIT WHEN c_oggCustom%NOTFOUND;
            
			--TUTTI I RUOLI
			DECLARE CURSOR c_idRuoli IS select id_gruppo from dpa_corr_globali where id_amm = par_id_amm and id_gruppo is not null and cha_tipo_urp = 'R';
			
            BEGIN OPEN c_idRuoli;
                LOOP FETCH c_idRuoli INTO par_id_ruolo;
                EXIT WHEN c_idRuoli%NOTFOUND;  
                
                    select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_DOC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                    if(cnt = 0) then
                        --Visibilità sul campo
						insert into DPA_A_R_OGG_CUSTOM_DOC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_DOC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 0, 1);
			        end if;
                    
                END LOOP;
                CLOSE c_idRuoli;
            END;
            
            END LOOP;
            CLOSE c_oggCustom;
        END;        
        
        END LOOP;
        CLOSE c_idTemplate;
    END;
END;
/

--Buono Paolo 18/02/2011

--1)Per ogni amministrazione vengono estratte le tipologie di fascicolo con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che hanno diritti di inserimento e ricerca su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede già dei diritti, altrimenti gli vengono assegnati pieni diritti sul campo (Inserimento,Modifica,Visibilità)
--DIRITTI 2(Inserimento/Ricerca) SULLA DPA_VIS_TIPO_FASC, INSERIMENTO NELLA DPA_A_R_OGG_CUSTOM_FASC CON DIRITTO INS_MOD(Inserimento/Modifica)=1 e VIS(Visiblità)=1
BEGIN
    DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_fasc where id_amm is not null;
    par_id_template int;
    par_id_oggetto_Custom int;
    par_id_ruolo int;
    par_id_amm int;
    cnt int;
    BEGIN  OPEN c_idTemplate;
        LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
        EXIT WHEN c_idTemplate%NOTFOUND;
        
        DECLARE CURSOR c_oggCustom IS select dpa_ass_templates_fasc.id_oggetto from dpa_ass_templates_fasc, dpa_oggetti_custom_fasc where dpa_ass_templates_fasc.id_template = par_id_template and dpa_ass_templates_fasc.id_project IS NULL and dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id;
        BEGIN  OPEN c_oggCustom;
            LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
            EXIT WHEN c_oggCustom%NOTFOUND;
            
            --Inserimento e Ricerca Tipologia
            DECLARE CURSOR c_idRuoli IS select id_ruolo from dpa_vis_tipo_fasc where id_tipo_fasc = par_id_template and diritti = 2;
        
			BEGIN OPEN c_idRuoli;
                LOOP FETCH c_idRuoli INTO par_id_ruolo;
                EXIT WHEN c_idRuoli%NOTFOUND;  
                
                    select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_FASC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                    if(cnt = 0) then
                        --Inserimento/Modifica e Visibilità sul campo
                        insert into DPA_A_R_OGG_CUSTOM_FASC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_FASC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 1, 1);
                    end if;
                    
                END LOOP;
                CLOSE c_idRuoli;
            END;
            
            END LOOP;
            CLOSE c_oggCustom;
        END;        
        
        END LOOP;
        CLOSE c_idTemplate;
    END;
END;
/

--1)Per ogni amministrazione vengono estratte le tipologie di fascicolo con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che hanno diritti di ricerca su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede già dei diritti, altrimenti gli viene assegnato il diritto di visibilità sul campo
--DIRITTI 1(Ricerca) SULLA DPA_VIS_TIPO_FASC, INSERIMENTO NELLA DPA_A_R_OGG_CUSTOM_FASC CON DIRITTO INS_MOD(Inserimento/Modifica)=0 e VIS(Visiblità)=1
BEGIN
    DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_fasc where id_amm is not null;
    par_id_template int;
    par_id_oggetto_Custom int;
    par_id_ruolo int;
    par_id_amm int;
    cnt int;
    BEGIN  OPEN c_idTemplate;
        LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
        EXIT WHEN c_idTemplate%NOTFOUND;
        
        DECLARE CURSOR c_oggCustom IS select dpa_ass_templates_fasc.id_oggetto from dpa_ass_templates_fasc, dpa_oggetti_custom_fasc where dpa_ass_templates_fasc.id_template = par_id_template and dpa_ass_templates_fasc.id_project IS NULL and dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id;
        BEGIN  OPEN c_oggCustom;
            LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
            EXIT WHEN c_oggCustom%NOTFOUND;
            
            --Ricerca tipologia
            DECLARE CURSOR c_idRuoli IS select id_ruolo from dpa_vis_tipo_fasc where id_tipo_fasc = par_id_template and diritti = 1;
            
			BEGIN OPEN c_idRuoli;
                LOOP FETCH c_idRuoli INTO par_id_ruolo;
                EXIT WHEN c_idRuoli%NOTFOUND;  
                
                    select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_FASC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                    if(cnt = 0) then
                        --Visibilità sul campo
                        insert into DPA_A_R_OGG_CUSTOM_FASC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_FASC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 0, 1);
                    end if;
                    
                END LOOP;
                CLOSE c_idRuoli;
            END;
            
            END LOOP;
            CLOSE c_oggCustom;
        END;        
        
        END LOOP;
        CLOSE c_idTemplate;
    END;
END;
/

--1)Per ogni amministrazione vengono estratte le tipologie di fascicolo con i loro campi di profilazione
--2)Per ogni tipologia vengono selezionati i ruoli che non hanno diritti su di essa
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede già dei diritti, altrimenti non gli viene assegnato nessun diritto sul campo
--DIRITTI 0(Nessun diritto) SULLA DPA_VIS_TIPO_FASC, INSERIMENTO NELLA DPA_A_R_OGG_CUSTOM_FASC CON DIRITTO INS_MOD(Inserimento/Modifica)=0 e VIS(Visiblità)=0
BEGIN
    DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_fasc where id_amm is not null;
    par_id_template int;
    par_id_oggetto_Custom int;
    par_id_ruolo int;
    par_id_amm int;
    cnt int;
    BEGIN  OPEN c_idTemplate;
        LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
        EXIT WHEN c_idTemplate%NOTFOUND;
        
        DECLARE CURSOR c_oggCustom IS select dpa_ass_templates_fasc.id_oggetto from dpa_ass_templates_fasc, dpa_oggetti_custom_fasc where dpa_ass_templates_fasc.id_template = par_id_template and dpa_ass_templates_fasc.id_project IS NULL and dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id;
        BEGIN  OPEN c_oggCustom;
            LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
            EXIT WHEN c_oggCustom%NOTFOUND;
            
            --Nessun diritto sulla tipologia
            DECLARE CURSOR c_idRuoli IS select id_ruolo from dpa_vis_tipo_fasc where id_tipo_fasc = par_id_template and diritti = 0;
            
			BEGIN OPEN c_idRuoli;
                LOOP FETCH c_idRuoli INTO par_id_ruolo;
                EXIT WHEN c_idRuoli%NOTFOUND;  
                
                    select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_FASC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                    if(cnt = 0) then
                        --Nessun diritto sul campo
                        insert into DPA_A_R_OGG_CUSTOM_FASC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_FASC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 0, 0);
                    end if;
                    
                END LOOP;
                CLOSE c_idRuoli;
            END;
            
            END LOOP;
            CLOSE c_oggCustom;
        END;        
        
        END LOOP;
        CLOSE c_idTemplate;
    END;
END;
/

--1)Per ogni amministrazione vengono estratte le tipologie di fascicolo con i loro campi di profilazione
--2)Vengono selezionati tutti i ruoli dell'amministrazione
--3)Per ogni campo di una tipologia, si verifica se il ruolo possiede già dei diritti, altrimenti non gli viene assegnato nessun diritto sul campo
--PER TUTTI I RUOLI NON CENSITI NELLA DPA_A_R_OGG_CUSTOM_FASC, INSERIMENTO NELLA DPA_A_R_OGG_CUSTOM_FASC CON DIRITTO INS_MOD(Inserimento/Modifica)=1 e VIS(Visiblità)=1
--Quest'ultimo script serve a dare comunque visibilità sui campi, anche ai ruoli che non possono inserire o ricercare la specifica tipologia
--Questo per evitare che un fascicolo trasmesso ad un utente senza diritti di inserimento e ricerca su una tipologia, si presenti senza campi di profilazione
BEGIN
    DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_fasc where id_amm is not null;
    par_id_template int;
    par_id_oggetto_Custom int;
    par_id_ruolo int;
    par_id_amm int;
    cnt int;
    BEGIN  OPEN c_idTemplate;
        LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
        EXIT WHEN c_idTemplate%NOTFOUND;
        
        DECLARE CURSOR c_oggCustom IS select dpa_ass_templates_fasc.id_oggetto from dpa_ass_templates_fasc, dpa_oggetti_custom_fasc where dpa_ass_templates_fasc.id_template = par_id_template and dpa_ass_templates_fasc.id_project IS NULL and dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id;
        BEGIN  OPEN c_oggCustom;
            LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
            EXIT WHEN c_oggCustom%NOTFOUND;
            
            --TUTTI I RUOLI
            DECLARE CURSOR c_idRuoli IS select id_gruppo from dpa_corr_globali where id_amm = par_id_amm and id_gruppo is not null and cha_tipo_urp = 'R';
			
            BEGIN OPEN c_idRuoli;
                LOOP FETCH c_idRuoli INTO par_id_ruolo;
                EXIT WHEN c_idRuoli%NOTFOUND;  
                
                    select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_FASC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                    if(cnt = 0) then
                        --Visibilità sul campo
                        insert into DPA_A_R_OGG_CUSTOM_FASC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_FASC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 0, 1);
                    end if;
                    
                END LOOP;
                CLOSE c_idRuoli;
            END;
            
            END LOOP;
            CLOSE c_oggCustom;
        END;        
        
        END LOOP;
        CLOSE c_idTemplate;
    END;
END;
/

-- Fine VISIBILITA CAMPI PROFILAZIONE DINAMICA

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' AND column_name='IPERFASCICOLO';
        if (cnt != 0) then            
		    execute immediate 'ALTER TABLE DPA_TIPO_ATTO drop COLUMN IPERFASCICOLO '; --TO IPERDOCUMENTO;
		end if; 
	   
	   select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' AND column_name='IPERDOCUMENTO';
	   if (cnt = 0) then            
	   -- la colonna non esiste, la creo
			execute immediate 'ALTER TABLE DPA_TIPO_ATTO add IPERDOCUMENTO NUMBER ';
        end if;   
 end;
end;
/


begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='FILTRO_FASC_EXCEL';
    if (cnt = 0) then        
       insert into @db_user.DPA_ANAGRAFICA_FUNZIONI VALUES('FILTRO_FASC_EXCEL','Abilita la gestione filtro su foglio excel in ricerca fascicoli', '', 'N');
    end if;
    end;
end;
/


begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' and column_name='COD_MOD_TRASM';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TIPO_ATTO ADD COD_MOD_TRASM VARCHAR2(128)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' and column_name='COD_CLASS';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TIPO_ATTO ADD COD_CLASS VARCHAR2(128)';
	end if;
	end;
end;
/

CREATE OR REPLACE PROCEDURE sp_dpa_count_todolist_no_reg (
   id_people_p   IN   NUMBER,
   id_gruppo     IN   NUMBER,
   ts            IN   VARCHAR
)
IS
   trasmdoctot             NUMBER;
   trasmdocnonletti        NUMBER;
   trasmdocnonaccettati    NUMBER;
   trasmfasctot            NUMBER;
   trasmfascnonletti       NUMBER;
   trasmfascnonaccettati   NUMBER;
   docpredisposti          NUMBER;
   ts_stampa_p             DATE;
BEGIN
--numero documenti presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdoctot
     FROM dpa_todolist
    WHERE id_profile > 0 
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );
        
--numero documenti non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonletti
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');
      
--numero documenti non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonaccettati
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
--numero fascicoli presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfasctot
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );
--numero fascicoli non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonletti
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');
--numero fascicoli non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonaccettati
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
--numero documenti predisposti
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO docpredisposti
     FROM dpa_todolist
    WHERE id_profile > 0
         AND id_profile IN (SELECT system_id
                           FROM PROFILE
                          WHERE cha_da_proto = '1')
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
   BEGIN                                                               -- MAIN
      ts_stampa_p := TO_DATE (ts, 'dd/mm/yyyy hh24:mi:ss');
-- SVUOTO LA TABELLA DEI DATI
      DELETE      dpa_count_todolist
            WHERE (id_people = id_people_p);

      INSERT INTO dpa_count_todolist
                  (id_people, ts_stampa, tot_doc, tot_doc_no_letti,
                   tot_doc_no_accettati, tot_fasc, tot_fasc_no_letti,
                   tot_fasc_no_accettati, tot_doc_predisposti
                  )
           VALUES (id_people_p, ts_stampa_p, trasmdoctot, trasmdocnonletti,
                   trasmdocnonaccettati, trasmfasctot, trasmfascnonletti,
                   trasmfascnonaccettati, docpredisposti
                  );
   END;                                                                -- MAIN
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN;
END;
/

--Inizio FASCICOLI CONTROLLATI
begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='PROJECT' and column_name='CHA_CONTROLLATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE PROJECT ADD CHA_CONTROLLATO VARCHAR(2) default ''0'' NOT NULL ';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='FASC_CONTROLLATO';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_LOG (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
       VALUES (SEQ.NEXTVAL,'FASC_CONTROLLATO', 'Modificata proprietà controllato del fascicolo', 'FASCICOLO', 'FASCCONTROLLATO');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_anagrafica_funzioni where cod_funzione='FASC_CONTROLLATO';
    if (cnt = 0) then
		begin
            INSERT INTO dpa_anagrafica_funzioni values ('FASC_CONTROLLATO','Permette la creazione/modifica di un fascicolo controllato', '', 'N');
		end;
    end if;
    end;
end;
/
-- FINE FASCICOLI CONTROLLATI

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='GROUPS';
	if (cnt != 0) then		
        	execute immediate 'ALTER TABLE GROUPS MODIFY(GROUP_NAME VARCHAR2(256 BYTE))';
		end if;
	end;
end;
/ 

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_AMMINISTRA';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' and column_name='TIPO_DOC_OBBL';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD (TIPO_DOC_OBBL  CHAR(1 BYTE) DEFAULT 0)';
		end if;
	end if;
	end;
end;
/ 

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_MODELLI_TRASM' and column_name='NO_NOTIFY';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_MODELLI_TRASM ADD NO_NOTIFY VARCHAR(1)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_CORR_GLOBALI' and column_name='COD_DESC_INTEROP';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_CORR_GLOBALI ADD COD_DESC_INTEROP VARCHAR(516)';
    end if;
    end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_DOC_ARRIVO_PAR';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_DOC_ARRIVO_PAR' and column_name='CHA_TIPO_MITT_DEST';
  	if (cnt != 0) then
        	execute immediate 'ALTER TABLE DPA_DOC_ARRIVO_PAR MODIFY CHA_TIPO_MITT_DEST VARCHAR(2)';
		end if;
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_CORR_STO';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_CORR_STO' and column_name='CHA_TIPO_MITT_DES';
  	if (cnt != 0) then
        	execute immediate 'ALTER TABLE DPA_CORR_STO MODIFY CHA_TIPO_MITT_DES VARCHAR(2)';
		end if;
	end if;
	end;
end;
/

CREATE OR REPLACE PROCEDURE         spsetdatavista_tv (
   p_idpeople      IN       NUMBER,
   p_idoggetto     IN       NUMBER,
   p_idgruppo      IN       NUMBER,
   p_tipooggetto   IN       CHAR,
   p_iddelegato    IN       NUMBER,
   p_resultvalue   OUT      NUMBER
)
IS
/*
----------------------------------------------------------------------------------------

RICHIAMATA SOLO DAL TASTO VISTO, agisce solo sulle trasmissioni NO WKFL. TOGLIENDOLE DALLA TDL 
NON SETTA DATA VISTA PERCHé LO FA la SP_SET_DATAVISTA_V2


dpa_trasm_singola.cha_tipo_trasm = 'S''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

dpa_trasm_singola.cha_tipo_trasm = 'T''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

*/
   p_cha_tipo_trasm   CHAR (1) := NULL;
   p_chatipodest      NUMBER;
BEGIN
   p_resultvalue := 0;

   DECLARE
      CURSOR cursortrasmsingoladocumento
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_profile = p_idoggetto
            AND b.id_ragione = c.system_id;

      CURSOR cursortrasmsingolafascicolo
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_project = p_idoggetto
            AND b.id_ragione = c.system_id;
   BEGIN
      IF (p_tipooggetto = 'D')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingoladocumento
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET --dpa_trasm_utente.cha_vista = '1',
                              -- dpa_trasm_utente.dta_vista =
                               --   (CASE
                               --       WHEN dta_vista IS NULL
                               --          THEN SYSDATE
                               --       ELSE dta_vista
                               --    END
                               --   ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE -- dpa_trasm_utente.dta_vista IS NULL AND
                          id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET --dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                              -- dpa_trasm_utente.dta_vista =
                              --    (CASE
                              --        WHEN dta_vista IS NULL
                             -- --           THEN SYSDATE
                              --        ELSE dta_vista
                             --      END
                             --     ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE-- dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                 /* BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END; */

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        IF (p_iddelegato = 0)
                        THEN
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET --dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                  id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET -- dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                  id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1');

--AND dpa_trasm_utente.id_people = p_idpeople;
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;

      IF (p_tipooggetto = 'F')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingolafascicolo
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET --dpa_trasm_utente.cha_vista = '1',
                               --dpa_trasm_utente.dta_vista =
                              --    (CASE
                              --        WHEN dta_vista IS NULL
                              --           THEN SYSDATE
                              --        ELSE dta_vista
                              --     END
                              --    ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--caso in cui si sta esercitando una delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET  --dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               --dpa_trasm_utente.dta_vista =
                               --   (CASE
                              --        WHEN dta_vista IS NULL
                              --           THEN SYSDATE
                              --        ELSE dta_vista
                             --      END
                             --     ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                /*  BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END; */

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
                        IF (p_iddelegato = 0)
                        THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET --dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                  id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET -- dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                                                id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- se la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                              dpa_trasm_utente.dta_vista =
                                  (CASE
                                     WHEN dta_vista IS NULL
                                        THEN SYSDATE
                                     ELSE dta_vista
                                  END
                                 )
                         WHERE dpa_trasm_utente.dta_vista IS NULL and
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
-- caso in cui si sta esercitando una delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET -- dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato--,
                              -- dpa_trasm_utente.dta_vista =
                              --    (CASE
                               --       WHEN dta_vista IS NULL
                               ---          THEN SYSDATE
                               --       ELSE dta_vista
                               --    END
                               --   )
                         WHERE-- dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1')
                        AND dpa_trasm_utente.id_people = p_idpeople;

                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;
   END;
END spsetdatavista_tv;
/


CREATE OR REPLACE PROCEDURE         spsetdatavista_v2 (
   p_idpeople      IN       NUMBER,
   p_idoggetto     IN       NUMBER,
   p_idgruppo      IN       NUMBER,
   p_tipooggetto   IN       CHAR,
   p_iddelegato    IN       NUMBER,
   p_resultvalue   OUT      NUMBER
)
IS
/*
----------------------------------------------------------------------------------------
dpa_trasm_singola.cha_tipo_trasm = 'S''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

dpa_trasm_singola.cha_tipo_trasm = 'T''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

*/
   p_cha_tipo_trasm   CHAR (1) := NULL;
   p_chatipodest      NUMBER;
BEGIN
   p_resultvalue := 0;

   DECLARE
      CURSOR cursortrasmsingoladocumento
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_profile = p_idoggetto
            AND b.id_ragione = c.system_id;

      CURSOR cursortrasmsingolafascicolo
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_project = p_idoggetto
            AND b.id_ragione = c.system_id;
   BEGIN
      IF (p_tipooggetto = 'D')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingoladocumento
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )--,
                             --  dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )--,
                              -- dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        IF (p_iddelegato = 0)
                        THEN
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_vista = '1'--,
                                   --  dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE dpa_trasm_utente.dta_vista IS NULL
                                 AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato--,
                                    -- dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE dpa_trasm_utente.dta_vista IS NULL
                                 AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1');

--AND dpa_trasm_utente.id_people = p_idpeople;
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;

      IF (p_tipooggetto = 'F')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingolafascicolo
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )--,
                               --dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--caso in cui si sta esercitando una delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )--,
                              -- dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
                        IF (p_iddelegato = 0)
                        THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_vista = '1'--,
                                    -- dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE dpa_trasm_utente.dta_vista IS NULL
                                 AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato--,
                                    -- dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE dpa_trasm_utente.dta_vista IS NULL
                                 AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- se la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
-- caso in cui si sta esercitando una delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1')
                        AND dpa_trasm_utente.id_people = p_idpeople;

                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;
   END;
END spsetdatavista_v2;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_DETT_GLOBALI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='VAR_LOCALITA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD VAR_LOCALITA VARCHAR2(128)';
		end if;
	end if;
	end;
end;
/


begin
declare cnt int;
 begin
select count(*) into cnt from user_tables where table_name='DPA_TIPO_NOTIFICA';
        if (cnt = 0) then
            execute immediate
                        'CREATE TABLE DPA_TIPO_NOTIFICA' ||
                        '(' ||
                        'SYSTEM_ID            NUMBER                   NOT NULL,' ||
                        'VAR_CODICE_NOTIFICA  VARCHAR2(50 BYTE)        NOT NULL,' ||
                        'VAR_DESCRIZIONE      VARCHAR2(255 BYTE)' ||
                        ') ';
            end if;
  end;
end;
/

                  
                  begin
declare cnt int;
begin
select count(*) into cnt from user_tables where table_name='DPA_NOTIFICA';
        if (cnt = 0) then
            execute immediate
                        'CREATE TABLE DPA_NOTIFICA' ||
                        '(' ||
                        'SYSTEM_ID               NUMBER,' ||
                        'ID_TIPO_NOTIFICA        NUMBER,' ||
                        'DOCNUMBER               NUMBER,' ||
                        'VAR_MITTENTE            VARCHAR2(255 BYTE),' ||
                        'VAR_TIPO_DESTINATARIO   VARCHAR2(100 BYTE),' ||
                        'VAR_DESTINATARIO        VARCHAR2(255 BYTE),' ||
                        'VAR_RISPOSTE            VARCHAR2(255 BYTE),' ||
                        'VAR_OGGETTO             VARCHAR2(516 BYTE),' ||
                        'VAR_GESTIONE_EMITTENTE  VARCHAR2(255 BYTE),' ||
                        'VAR_ZONA                VARCHAR2(10 BYTE),' ||
                        'VAR_GIORNO_ORA          DATE,' ||
                        'VAR_IDENTIFICATIVO      VARCHAR2(516 BYTE),' ||
                        'VAR_MSGID               VARCHAR2(516 BYTE),' ||
                        'VAR_TIPO_RICEVUTA       VARCHAR2(516 BYTE),' ||
                        'VAR_CONSEGNA            VARCHAR2(516 BYTE),' ||
                        'VAR_RICEZIONE           VARCHAR2(516 BYTE),' ||
                        'VAR_ERRORE_ESTESO       CLOB,' ||
                        ' VAR_ERRORE_RICEVUTA     VARCHAR2(50 BYTE)' ||
                        ') ';
            end if;
   end;
end;
/

                  begin
    declare
         cnt int;
    begin    
    
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_NOTIFICA';
        if (cnt = 0) then
         begin
           select max(system_id)+1 into cnt from DPA_NOTIFICA;
           execute immediate 'CREATE SEQUENCE SEQ_DPA_NOTIFICA START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
         end;
        end if;    
    end;
end;
/

begin
    declare
         cnt int;
    begin    
    
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_TIPO_NOTIFICA';
        if (cnt = 0) then
         begin
           select max(system_id)+1 into cnt from DPA_TIPO_NOTIFICA;
           execute immediate 'CREATE SEQUENCE SEQ_DPA_TIPO_NOTIFICA START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
         end;
        end if;    
    end;
end;
/


begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='PROFILE' and column_name='CHA_DOCUMENTO_DA_PEC';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE PROFILE ADD CHA_DOCUMENTO_DA_PEC VARCHAR(1)';
    end if;
    end;
end;
/

CREATE OR REPLACE FUNCTION @db_user.getIfModelloAutorizzato
(
id_ruolo  number,
id_people  number,
system_id  number,
id_modelloTrasm number,
accesssRigth number
)
RETURN NUMBER IS retVal NUMBER;
--accesssRigth number;
idRagione  number;
tipo_diritto  char;
cursor cur is
select distinct ID_RAGIONE FROM dpa_modelli_mitt_dest WHERE id_modello=id_modelloTrasm and CHA_TIPO_MITT_DEST <> 'M';
BEGIN
retVal:=1;
--accesssRigth:= getaccessrights(id_ruolo, id_people, system_id);
if (accesssRigth = 45)
then
begin
OPEN cur;
LOOP
FETCH cur INTO idRagione;
EXIT WHEN cur%NOTFOUND;
select CHA_TIPO_DIRITTI into tipo_diritto from DPA_RAGIONE_TRASM where system_id=idRagione;
if(tipo_diritto <> 'R' and tipo_diritto <> 'C')
then
begin
retVal:=0;
end;
end if;
exit when  retVal=0;
end loop;
close cur;
end;
end if;
RETURN retVal;
END getIfModelloAutorizzato;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='DO_RIC_CAMPI_COMUNI';
    if (cnt = 0) then        
       insert into @db_user.DPA_ANAGRAFICA_FUNZIONI VALUES('DO_RIC_CAMPI_COMUNI','Abilita la ricerca per campi comuni', '', 'N');
    end if;
    end;
end;
/

begin
	declare
		   cnt int;
	begin
		select count(*) into cnt from user_constraints, user_cons_columns where user_cons_columns.table_name = 'DPA_VOCI_MENU_ADMIN' and  constraint_type='U' and user_cons_columns.constraint_name = user_constraints.constraint_name;
		if (cnt = 0) then  	  
			execute immediate 'ALTER TABLE DPA_VOCI_MENU_ADMIN ADD (CONSTRAINT DPA_VOCI_MENU_ADMIN_U01 UNIQUE (VAR_CODICE))';
		end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_VOCI_MENU_ADMIN where VAR_CODICE='Tipi fascicolo';
	if (cnt = 0) then		
	   insert into DPA_VOCI_MENU_ADMIN (system_id, VAR_CODICE, VAR_DESCRIZIONE, VAR_VISIBILITA_MENU ) values (20, 'Tipi fascicolo','Tipi fascicolo','ProfilazioneDinamicaFasc');
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_VOCI_MENU_ADMIN where VAR_CODICE='Gestione Deleghe';
	if (cnt = 0) then		
	   insert into DPA_VOCI_MENU_ADMIN (system_id, VAR_CODICE, VAR_DESCRIZIONE, VAR_VISIBILITA_MENU ) values (21, 'Gestione Deleghe','Gestione Deleghe',NULL);
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_VOCI_MENU_ADMIN where VAR_CODICE='Gestione RF';
	if (cnt = 0) then		
	   insert into DPA_VOCI_MENU_ADMIN (system_id, VAR_CODICE, VAR_DESCRIZIONE, VAR_VISIBILITA_MENU ) values (22,'Gestione RF','Gestione RF', NULL);
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_VOCI_MENU_ADMIN where VAR_CODICE='Gestione News';
	if (cnt = 0) then		
	   insert into DPA_VOCI_MENU_ADMIN (system_id, VAR_CODICE, VAR_DESCRIZIONE, VAR_VISIBILITA_MENU ) values (23, 'Gestione News','Gestione News', NULL);
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_VOCI_MENU_ADMIN where VAR_CODICE='Gestione Chiavi Config';
	if (cnt = 0) then		
	   insert into DPA_VOCI_MENU_ADMIN (system_id, VAR_CODICE, VAR_DESCRIZIONE, VAR_VISIBILITA_MENU ) values (24, 'Gestione Chiavi Config','Gestione Chiavi Config', NULL);
	end if;
	end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' AND column_name='IS_ENABLED_SMART_CLIENT';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD IS_ENABLED_SMART_CLIENT CHAR(1)';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PEOPLE' AND column_name='IS_ENABLED_SMART_CLIENT';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PEOPLE ADD IS_ENABLED_SMART_CLIENT CHAR(1)';
        end if;
    end;
end;
/

CREATE OR REPLACE FUNCTION CORRCAT (docId INT, tipo_proto VARCHAR)
RETURN varchar IS risultato varchar(4000);

item varchar(4000);
tipo_mitt_dest VARCHAR(10);
LNG INT;

CURSOR cur IS
SELECT c.var_desc_corr, dap.cha_tipo_mitt_dest
FROM DPA_CORR_GLOBALI c , DPA_DOC_ARRIVO_PAR dap
WHERE dap.id_profile=docId
AND dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc;

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item,tipo_mitt_dest;
EXIT WHEN cur%NOTFOUND;

LNG:=LENGTh(risultato);

IF(risultato IS NOT NULL anD LNG>=(3900-128))
tHEN RETURN RISULTATO||'...';
ELSE
BEGIN

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'MD') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (MM)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'I') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (MI)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'L') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '|| DEST_IN_LISTA(docId);
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'F') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '|| DEST_IN_RF(docId);
ELSE
risultato := risultato||item;
END IF;
END IF;


END;
END IF;

END LOOP;

RETURN risultato;

END Corrcat;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' AND column_name='SMART_CLIENT_PDF_CONV_ON_SCAN';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD SMART_CLIENT_PDF_CONV_ON_SCAN CHAR(1)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PEOPLE' AND column_name='SMART_CLIENT_PDF_CONV_ON_SCAN';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PEOPLE ADD SMART_CLIENT_PDF_CONV_ON_SCAN CHAR(1)';
        end if;
    end;
end;
/

-- AZIONI MASSIVE

-- FIRMA MASSIVA
BEGIN
     DECLARE cnt int;
   BEGIN
     SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='MASSIVE_SIGN';
     IF (cnt = 0) THEN
        INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_SIGN' , 'Abilita l''utente a compiere firme massive', NULL, 'N');
     END IF;
     END;
END;
/

-- FASCICOLAZIONE MASSIVA
BEGIN
     DECLARE cnt int;
   BEGIN
     SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='MASSIVE_CLASSIFICATION';
     IF (cnt = 0) THEN
        INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_CLASSIFICATION' , 'Abilita l''utente a compiere fascicolazioni massive', NULL, 'N');
     END IF;
     END;
END;
/

-- TRASMISSIONE MASSIVA
BEGIN
     DECLARE cnt int;
   BEGIN
     SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='MASSIVE_TRANSMISSION';
     IF (cnt = 0) THEN
        INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_TRANSMISSION' , 'Abilita l''utente a compiere trasmissioni massive', NULL, 'N');
     END IF;
     END;
END;
/

-- TIMESTAMP MASSIVO
BEGIN
     DECLARE cnt int;
   BEGIN
     SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='MASSIVE_TIMESTAMP';
     IF (cnt = 0) THEN
        INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_TIMESTAMP' , 'Abilita l''utente a compiere applicazioni massive di timestamp', NULL, 'N');
     END IF;
     END;
END;
/

-- CONVERSIONE MASSIVA
BEGIN
     DECLARE cnt int;
   BEGIN
     SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='MASSIVE_CONVERSION';
     IF (cnt = 0) THEN
        INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED)
         VALUES ('MASSIVE_CONVERSION' , 'Abilita l''utente a compiere conversioni massive', NULL, 'N');
     END IF;
     END;
END;
/

-- FINE AZIONI MASSIVE

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PEOPLE' AND column_name='LDAP_AUTHENTICATED';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PEOPLE ADD LDAP_AUTHENTICATED CHAR(1)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_LDAP_CONFIG' AND column_name='ID_USER';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_LDAP_CONFIG ADD ID_USER CHAR(1)';
        end if;
    end;
end;
/
begin
    declare nlb char(1);
    begin
        select nullable into nlb from user_tab_columns where table_name = 'DPA_LDAP_CONFIG' and column_name = 'USERID_ATTRIBUTE';
        if (nlb = 'N') then
               execute immediate 'ALTER TABLE DPA_LDAP_CONFIG MODIFY USERID_ATTRIBUTE NULL';
        end if;
    end;
end;    
/
begin
    declare nlb char(1);
    begin
        select nullable into nlb from user_tab_columns where table_name = 'DPA_LDAP_CONFIG' and column_name = 'EMAIL_ATTRIBUTE';
        if (nlb = 'N') then
               execute immediate 'ALTER TABLE DPA_LDAP_CONFIG MODIFY EMAIL_ATTRIBUTE NULL';
        end if;
    end;
end;    
/
begin
    declare nlb char(1);
    begin
        select nullable into nlb from user_tab_columns where table_name = 'DPA_LDAP_CONFIG' and column_name = 'MATRICOLA_ATTRIBUTE';
        if (nlb = 'N') then
               execute immediate 'ALTER TABLE DPA_LDAP_CONFIG MODIFY MATRICOLA_ATTRIBUTE NULL';
        end if;
    end;
end; 
/
begin
    declare nlb char(1);
    begin
        select nullable into nlb from user_tab_columns where table_name = 'DPA_LDAP_CONFIG' and column_name = 'NOME_ATTRIBUTE';
        if (nlb = 'N') then
               execute immediate 'ALTER TABLE DPA_LDAP_CONFIG MODIFY NOME_ATTRIBUTE NULL';
        end if;
    end;
end; 
/
begin
    declare nlb char(1);
    begin
        select nullable into nlb from user_tab_columns where table_name = 'DPA_LDAP_CONFIG' and column_name = 'COGNOME_ATTRIBUTE';
        if (nlb = 'N') then
               execute immediate 'ALTER TABLE DPA_LDAP_CONFIG MODIFY COGNOME_ATTRIBUTE NULL';
        end if;
    end;
end; 
/
begin
    declare nlb char(1);
    begin
        select nullable into nlb from user_tab_columns where table_name = 'DPA_LDAP_CONFIG' and column_name = 'SEDE_ATTRIBUTE';
        if (nlb = 'N') then
               execute immediate 'ALTER TABLE DPA_LDAP_CONFIG MODIFY SEDE_ATTRIBUTE NULL';
        end if;
    end;
end; 
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM' and column_name='FORMATO_ORA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM ADD FORMATO_ORA VARCHAR(10)';
		end if;
	end if;
	end;
end;
/ 

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='FORMATO_ORA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD FORMATO_ORA VARCHAR(10)';
		end if;
	end if;
	end;
end;
/ 

begin
    declare   cnt int;
    begin
select count(*) into cnt from user_tables where table_name='DPA_MODELLI_DELEGA';
if (cnt = 0) then
  execute immediate
    'CREATE TABLE DPA_MODELLI_DELEGA ' ||
    '( ' ||
      'SYSTEM_ID            NUMBER                   NOT NULL, ' ||
      'ID_PEOPLE_DELEGANTE  NUMBER                   NOT NULL, ' ||
      'ID_RUOLO_DELEGANTE   NUMBER,                            ' ||
      'ID_PEOPLE_DELEGATO   NUMBER                   NOT NULL, ' ||
      'ID_RUOLO_DELEGATO    NUMBER                   NOT NULL, ' ||
      'INTERVALLO           INTEGER,                           ' ||
      'DTA_INIZIO           DATE,                              ' ||
      'DTA_FINE             DATE,                              ' ||
      'NOME                 VARCHAR2(100 BYTE)       NOT NULL  ' ||
    ')                                                         ';
     
    execute immediate
     'CREATE UNIQUE INDEX DPA_MODELLI_DELEGA_PK ON DPA_MODELLI_DELEGA     ' ||
    '(SYSTEM_ID)                                               ' ||
    'LOGGING                                                   ';
    
    execute immediate
    'ALTER TABLE DPA_MODELLI_DELEGA ADD (                      ' ||
      'CONSTRAINT DPA_MODELLI_DELEGA_PK                        ' ||
     'PRIMARY KEY                                              ' ||
     '(SYSTEM_ID)                                              ' ||
        'USING INDEX  )                                         ';

    
    end if;
    end;    
end;    
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM' and column_name='TIPO_LINK';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM ADD TIPO_LINK varchar2(50 byte)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM' and column_name='TIPO_OBJ_LINK';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM ADD TIPO_OBJ_LINK varchar2(50 byte)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='TIPO_LINK';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD TIPO_LINK varchar2(50 byte)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='TIPO_OBJ_LINK';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD TIPO_OBJ_LINK varchar2(50 byte)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_OGGETTO';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_TIPO_OGGETTO where upper(descrizione)='LINK';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO (SYSTEM_ID,TIPO,DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO.NEXTVAL,''Link'',''Link'')';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_OGGETTO_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_TIPO_OGGETTO_FASC where upper(descrizione)='LINK';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO_FASC (SYSTEM_ID,TIPO,DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO_FASC.NEXTVAL,''Link'',''Link'')';
        end if;
    end if;
    end;
end;
/

-- INOLTRA MASSIVO
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='MASSIVE_INOLTRA';
    IF (cnt = 0) THEN        
       INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
        VALUES ('MASSIVE_INOLTRA' , 'Abilita l''utente a compiere inoltra massivi', NULL, 'N');
    END IF;
    END;
END;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_MODELLI_TRASM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_MODELLI_TRASM' and column_name='CHA_MANTIENI_LETTURA';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_MODELLI_TRASM ADD CHA_MANTIENI_LETTURA char(1) NULL';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_RAGIONE_TRASM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_RAGIONE_TRASM' and column_name='CHA_MANTIENI_LETT';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_RAGIONE_TRASM ADD CHA_MANTIENI_LETT char(1) NULL';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
    begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='GET_FIRMA';
        if (cnt > 0) then
            execute immediate 'UPDATE DPA_ANAGRAFICA_LOG SET VAR_CODICE = ''PUT_FILE'' WHERE VAR_CODICE = ''GET_FIRMA''';            
        end if;
    end;    
end;   
/

begin
    declare cnt int;
    begin
    select count(*) into cnt from DPA_LOG where VAR_COD_AZIONE='GET_FIRMA';
        if (cnt > 0) then
            execute immediate 'UPDATE DPA_LOG SET VAR_COD_AZIONE = ''PUT_FILE'' WHERE VAR_COD_AZIONE = ''GET_FIRMA''';            
        end if;
    end;    
end;   
/

begin
    declare cnt int;
    begin
    select count(*) into cnt from DPA_LOG_STORICO where VAR_COD_AZIONE='GET_FIRMA';
        if (cnt > 0) then
            execute immediate 'UPDATE DPA_LOG_STORICO SET VAR_COD_AZIONE = ''PUT_FILE'' WHERE VAR_COD_AZIONE = ''GET_FIRMA''';            
        end if;
    end;    
end;   
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='PROJECT_COMPONENTS';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='PROJECT_COMPONENTS' and column_name='CHA_FASC_PRIMARIA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE PROJECT_COMPONENTS ADD CHA_FASC_PRIMARIA Varchar(1) DEFAULT (''0'') NOT NULL';
		end if;
	end if;
	end;
end;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_GESTIONE_DISSERVIZIO';
    IF (cnt = 0) THEN        
       insert into DPA_CHIAVI_CONFIGURAZIONE 
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_GESTIONE_DISSERVIZIO','Gestione Disservizio', '0','F','0','1','1');
    END IF;
    END;
END;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_NEW_RUBRICA_VELOCE';
    IF (cnt = 0) THEN        
       insert into DPA_CHIAVI_CONFIGURAZIONE 
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_NEW_RUBRICA_VELOCE','Abilita Rubrica Ajax', '0','F','0','1','1');
    END IF;
    END;
END;
/

--BE_SALVA_EMAIL_IN_LOCALE
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='BE_SALVA_EMAIL_IN_LOCALE';
    IF (cnt = 0) THEN        
       insert into DPA_CHIAVI_CONFIGURAZIONE 
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'BE_SALVA_EMAIL_IN_LOCALE','Salva le email delle ricevute PEC come allegati al documento inviato', '0','B','0','1','1');
    END IF;
    END;
END;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_FASC_PRIMARIA';
    IF (cnt = 0) THEN        
       insert into DPA_CHIAVI_CONFIGURAZIONE 
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_FASC_PRIMARIA','Attivazione fascicolazione primaria', '0','F','0','1','1');
    END IF;
    END;
END;
/

CREATE OR REPLACE FUNCTION getFascPrimaria(iProfile number, idFascicolo number) RETURN varchar2 IS
fascPrimaria varchar2(1);
begin

     SELECT nvl(B.CHA_FASC_PRIMARIA,'0') into fascPrimaria
       FROM PROJECT A, PROJECT_COMPONENTS B
       WHERE A.SYSTEM_ID=B.PROJECT_ID 
       AND B.LINK=iProfile and ID_FASCICOLO=idFascicolo;
	   return fascPrimaria;

 EXCEPTION
WHEN OTHERS
THEN
fascPrimaria := '0';

return fascPrimaria;
end getFascPrimaria;
/


begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='PROJECT';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='PROJECT' and column_name='CHA_CONSENTI_CLASS';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD CHA_CONSENTI_CLASS Varchar(1) DEFAULT (''1'') NOT NULL';
        end if;
	if (cnt = 1) then
            execute immediate 'ALTER TABLE PROJECT MODIFY CHA_CONSENTI_CLASS Varchar(1) DEFAULT (''1'')';
			-- gestisce casi di record con CHA_CONSENTI_CLASS a NULL
			execute immediate 'update PROJECT set CHA_CONSENTI_CLASS = ''1'' where CHA_CONSENTI_CLASS  is NULL'; 
			commit; 
        end if;
    end if;
    end;
end;
/

CREATE OR REPLACE PROCEDURE @db_user.CREATE_NEW_NODO_TITOLARIO(p_idAmm number, p_livelloNodo number,
p_description varchar2, p_codiceNodo varchar2, p_idRegistroNodo number, p_idParent number,
p_varCodLiv1 varchar2, p_mesiConservazione number, p_chaRW char, p_idTipoFascicolo number, p_bloccaFascicolo varchar2, p_sysIdTitolario number, p_noteNodo varchar, p_bloccaFigli varchar2, p_contatoreAttivo varchar2, p_numProtTit number,
p_consentiClassificazione VARCHAR2, p_bloccaClass varchar2, p_idTitolario OUT number) IS
BEGIN
DECLARE CURSOR currReg IS
select system_id
from DPA_EL_REGISTRI
WHERE ID_AMM = p_idAmm and cha_rf = '0';

secProj NUMBER;
secFasc NUMBER;
secRoot NUMBER;
varChiaveTit varchar2(64);
varChiaveFasc varchar2(64);
varChiaveRoot varchar2(64);
BEGIN
p_idTitolario:=0;

SELECT SEQ.NEXTVAL INTO secProj FROM DUAL;
p_idTitolario:= secProj;

if(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') then
varChiaveTit:= p_idamm ||'_'|| p_codiceNodo || '_' || p_idParent || '_0' ;
else
varChiaveTit:= p_codiceNodo || '_' || p_idParent || '_'  || p_idRegistroNodo;
end if;

if (p_bloccaClass = '1')
then
update PROJECT set CHA_CONSENTI_CLASS = '0', CHA_RW = 'R' where SYSTEM_ID=p_idParent;
update PROJECT set CHA_CONSENTI_CLASS = '0', CHA_RW = 'R' where SYSTEM_ID=p_idParent and CHA_TIPO_PROJ = 'F';
end if;

BEGIN

INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT,
CHA_CONSENTI_CLASS
)
VALUES
(
secProj,
p_description,
'Y',
'T',
p_codiceNodo,
p_idAmm,
p_idRegistroNodo,
p_livelloNodo,
NULL,
p_idParent,
p_varCodLiv1,
sysdate ,
NULL,
NULL,
p_chaRW,
p_mesiConservazione,
varChiaveTit,
p_idTipoFascicolo,
p_bloccaFascicolo,
p_sysIdTitolario,
sysdate,
p_noteNodo,
p_bloccaFigli,
p_contatoreAttivo,
p_numProtTit,
p_consentiClassificazione
);


EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;

END;

BEGIN

SELECT SEQ.NEXTVAL INTO secFasc FROM DUAL;

if(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') then
varChiaveFasc:= p_codiceNodo || '_' || p_idTitolario || '_0' ;
else
varChiaveFasc:= p_codiceNodo || '_' || p_idTitolario || '_'  || p_idRegistroNodo;
end if;

INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT,
CHA_CONSENTI_CLASS
)
VALUES
(
secFasc,
p_description,
'Y',
'F',
p_codiceNodo,
P_idAmm,
p_idRegistroNodo,
NULL,
'G',
p_idTitolario,
NULL,
sysdate ,
'A',
NULL,
p_chaRW,
p_mesiConservazione,
varChiaveFasc,
p_idTipoFascicolo,
p_bloccaFascicolo,
p_sysIdTitolario,
sysdate,
p_noteNodo,
p_bloccaFigli,
p_contatoreAttivo,
p_numProtTit,
p_consentiClassificazione
);

EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;


BEGIN


if(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') then
varChiaveRoot:= p_codiceNodo || '_' || secFasc || '_0' ;
else
varChiaveRoot:= p_codiceNodo || '_' || secFasc || '_'  || p_idRegistroNodo;
end if;

SELECT SEQ.NEXTVAL INTO secRoot FROM DUAL;

INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT,
CHA_CONSENTI_CLASS
)
VALUES
(
secRoot,
'Root Folder',
'Y',
'C',
NULL,
p_idAmm,
NULL,
NULL,
NULL,
secFasc,
NULL,
sysDate,
NULL,
secFasc,
p_chaRW,
p_mesiConservazione,
varChiaveRoot,
p_idTipoFascicolo,
p_bloccaFascicolo,
p_sysIdTitolario,
sysdate,
p_noteNodo,
p_bloccaFigli,
p_contatoreAttivo,
p_numProtTit,
p_consentiClassificazione
);
EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;

-- SE IL NODO HA REGISTRO NULL ALLORA DEVONO ESSERE CREATI TANTI RECORD NELLA
-- DPA_REG_FASC QUANTI SONO I REGISTRI INTERNI ALL'AMMINISTRAZIONE
IF(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') THEN
FOR currentReg IN currReg
LOOP
BEGIN
INSERT INTO DPA_REG_FASC
(
system_id,
id_Titolario,
num_rif,
id_registro
)
VALUES
(
seq.nextval,
p_idTitolario,
1,
currentReg.system_id
);
EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;
END LOOP;

-- inoltre bisogna inserire un record nella dpa_reg_Fasc relativo al registro null
-- per tutte quelle amministrazioni che non hanno abilitata la funzione di fascicolazione
--multi registro
insert into dpa_reg_fasc
(
system_id,
id_Titolario,
num_rif,
id_registro
)
values
(
seq.nextval,
p_idTitolario,
1,
NULL    -- SE IL NODO ¿ COMUNE A TUTTI p_idRegistro = NULL
);

ELSE -- il nodo creato ¿  associato  a uno solo registro

BEGIN
insert into dpa_reg_fasc
(
system_id,
id_Titolario,
num_rif,
id_registro
)
values
(
seq.nextval,
p_idTitolario,
1,
p_idRegistroNodo    -- REGISTRO SU CUI  CREO IL NODO
);
EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;
END IF;
end;

END CREATE_NEW_NODO_TITOLARIO;
/

 BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_BLOCCA_CLASS';
    IF (cnt = 0) THEN        
       insert into DPA_CHIAVI_CONFIGURAZIONE 
   (system_id,ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
  values 
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_BLOCCA_CLASS','Blocca la classificazione sui nodi che hanno nodi figli', '0','F','0','1','1');
    END IF;
    END;
END;
/

-- INIZIO BATTISTA 18.10.2010
-- campo luogo nascita
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_DETT_GLOBALI';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='VAR_LUOGO_NASCITA';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD VAR_LUOGO_NASCITA VARCHAR2(64)';
        end if;
    end if;
    end;
end;
/
 
-- campo data di nascita
begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='DTA_NASCITA';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD DTA_NASCITA VARCHAR2(64)';
    end if;
    end;
end;
/
 
-- campo titolo
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_DETT_GLOBALI';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='VAR_TITOLO';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD VAR_TITOLO VARCHAR2(64)';
        end if;
    end if;
    end;
end;
/

-- TABELLA DELLE QUALIFICHE
begin
    declare   cnt int;
      begin 
        select count(*) into cnt from user_tables where table_name='DPA_QUALIFICA_CORRISPONDENTE';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_QUALIFICA_CORRISPONDENTE ' ||
            '( ' ||
            'SYSTEM_ID INT PRIMARY KEY NOT NULL, ' ||
            'VAR_TITOLO VARCHAR2(64), ' ||
            'DTA_FINE_VALIDITA DATE) ';    
        end if;
    end;    
end;   
/

-- chiave esterna da fare solo dopo aver eseguito la creazione della tabella DPA_QUALIFICA_CORRISPONDENTE
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_DETT_GLOBALI';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='ID_QUALIFICA_CORR';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD ID_QUALIFICA_CORR INT';
            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD FOREIGN KEY (ID_QUALIFICA_CORR) REFERENCES DPA_QUALIFICA_CORRISPONDENTE(SYSTEM_ID)';
        end if;
    end if;
    end;
end;
/


-- popolamento tabella titoli corrispondenti

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Arch.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (1, 'Arch.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Avv.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (2, 'Avv.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Dott.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (3, 'Dott.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Dott.ssa';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (4, 'Dott.ssa', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Dr.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (5, 'Dr.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Geom.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (6, 'Geom.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Ing.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (7, 'Ing.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Mo.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (8, 'Mo.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Mons.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (9, 'Mons.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'on.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (10, 'on.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Prof.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (11, 'Prof.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Prof.ssa';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (12, 'Prof.ssa', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Rag.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (13, 'Rag.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Rev.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (14, 'Rev.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Sig.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (15, 'Sig.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Sig.na.';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (16, 'Sig.na.', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Sig.ra';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (17, 'Sig.ra', NULL);
         END IF;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM cols c
       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'
         AND c.column_name = 'VAR_TITOLO';

      IF (cnt = 1)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_qualifica_corrispondente qc
          WHERE qc.var_titolo = 'Sig.ra/sig.na';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_qualifica_corrispondente
                 VALUES (18, 'Sig.ra/sig.na', NULL);
         END IF;
      END IF;
   END;
END;
/

-- fine popolamento tabella titoli

-- FINE BATTISTA 18.10.2010

/*<TOAD_FILE_CHUNK>*/
begin
    declare
         cnt int;
	begin
		select count(*) into cnt from user_tables where table_name='DPA_DISSERVIZI';
		if (cnt = 0) then
			execute immediate
                'CREATE TABLE DPA_DISSERVIZI ' ||
                '( ' ||
                '  SYSTEM_ID             NUMBER, ' ||
                '  STATO                 VARCHAR2(32 BYTE), ' ||
                '  TESTO_NOTIFICA        VARCHAR2(1024 BYTE), ' ||
                '  TESTO_EMAIL_NOTIFICA  VARCHAR2(1024 BYTE), ' ||
                '  TESTO_PAG_CORTESIA    VARCHAR2(1024 BYTE), ' ||
                '  TESTO_EMAIL_RIPRESA   VARCHAR2(4 BYTE), ' ||
                '  NOTIFICATO      NUMBER(10) ' ||
                ') ';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin    
    
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_DISSERVIZI';
        if (cnt = 0) then
         begin
           select max(system_id)+1 into cnt from DPA_DISSERVIZI;
           execute immediate 'CREATE SEQUENCE SEQ_DPA_DISSERVIZI START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
         end;
        end if;    
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PEOPLE' AND column_name='ACCETTAZIONE_DISSERV';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PEOPLE ADD ACCETTAZIONE_DISSERV VARCHAR(1)';
        end if;
    end;
end;
/
  
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM' and column_name='CONFIG_OBJ_EST';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM ADD CONFIG_OBJ_EST clob';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='CONFIG_OBJ_EST';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD CONFIG_OBJ_EST clob';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_OGGETTO';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_TIPO_OGGETTO where upper(descrizione)='OGGETTOESTERNO';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO (SYSTEM_ID,TIPO,DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO.NEXTVAL,''Oggetto esterno'',''OggettoEsterno'')';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_OGGETTO_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_TIPO_OGGETTO_FASC where upper(descrizione)='OGGETTOESTERNO';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO_FASC (SYSTEM_ID,TIPO,DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO_FASC.NEXTVAL,''Oggetto esterno'',''OggettoEsterno'')';
        end if;
    end if;
    end;
end;
/

begin
    declare   cnt int;
    begin
	select count(*) into cnt from user_tables where table_name='PUBBLICAZIONI_DOCUMENTI';
		if (cnt = 0) then
		  execute immediate
'CREATE TABLE PUBBLICAZIONI_DOCUMENTI'||
'('||
'  SYSTEM_ID                     NUMBER, ' ||
'  ID_TIPO_DOCUMENTO             NUMBER,' ||
'  ID_PROFILE                    NUMBER,' ||
'  ID_USER                       NUMBER,' ||
'  ID_RUOLO                      NUMBER,' ||
'  DATA_DOC_PUBBLICATO           DATE,' ||
'  DATA_PUBBLICAZIONE_DOCUMENTO  DATE,' ||
'  ESITO_PUBBLICAZIONE           VARCHAR2(1 BYTE),' ||
'  ERRORE_PUBBLICAZIONE          VARCHAR2(255 BYTE)' ||
')';

end if;
    end;    
end;    
/ 

begin
    declare
         cnt int;
    begin    
    
        select count(*) into cnt from user_sequences where sequence_name='SEQ_PUBBLICAZIONI_DOCUMENTI';
        if (cnt = 0) then
         begin
           select max(system_id)+1 into cnt from PUBBLICAZIONI_DOCUMENTI;
           execute immediate 'CREATE SEQUENCE SEQ_PUBBLICAZIONI_DOCUMENTI START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
         end;
        end if;    
    end;
end;
/

--inizio chiavi di configurazione

--aggiunge la colonna “VAR_CODICE_OLD_WEBCONFIG” alla tabella
--“DPA_CHIAVI_CONFIGURAZIONE” per tenere traccia del vecchio codice della chiave
-- nella vecchia gestione da Web.config
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_CHIAVI_CONFIGURAZIONE' and column_name='VAR_CODICE_OLD_WEBCONFIG';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_CHIAVI_CONFIGURAZIONE ADD VAR_CODICE_OLD_WEBCONFIG  varchar(64)';
        end if;
    end if;
    end;
end;
/

--aumenta la lunghezza della colonna VAR_DESCRIZIONE da varchar(256) a 
--varchar(512)
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_CHIAVI_CONFIGURAZIONE' and column_name='VAR_DESCRIZIONE';
      if (cnt != 0) then
            execute immediate 'ALTER TABLE DPA_CHIAVI_CONFIGURAZIONE modify VAR_DESCRIZIONE varchar(512)';
        end if;
    end if;
    end;
end;
/


CREATE OR REPLACE PROCEDURE @db_user.setsecurityRuoloReg
(idCorrGlobali IN NUMBER, idProfile IN NUMBER,diritto IN NUMBER,Idreg in NUMBER, ReturnValue OUT NUMBER) IS

idGruppo dpa_corr_globali.id_gruppo%TYPE;

BEGIN

SELECT ID_GRUPPO INTO idGruppo FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID =
idCorrGlobali;

IF (idGruppo IS NOT NULL) THEN
BEGIN
SELECT MAX(accessrights) INTO ReturnValue from security  where thing =
idProfile and personorgroup = idGruppo;
END;
--
IF (ReturnValue < diritto ) THEN
BEGIN
update security set accessrights = diritto where thing = idProfile and
personorgroup = idGruppo;
END;
END IF;

IF (ReturnValue IS NULL) THEN
BEGIN
--insert into security values(idProfile,idGruppo,diritto,null,'A');

insert into security  (thing    ,personorgroup, ACCESSRIGHTS, CHA_TIPO_DIRITTO) 
                values(idProfile,idGruppo     ,diritto      ,'A');


END;
END IF;

END IF;



insert into security ( THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO )  
select SYSTEM_ID,idGruppo,diritto,null,'A' from PROFILE p where ID_REGISTRO=Idreg
and num_proto is not null
and not exists (select 'x' from SECURITY s1 where s1.THING=p.system_id and
s1.PERSONORGROUP=idGruppo and s1.ACCESSRIGHTS=diritto );

ReturnValue := diritto;
END setsecurityRuoloReg;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_AMMINISTRA';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' and column_name='CHA_SMTP_STA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD (CHA_SMTP_STA  CHAR(1 BYTE)';
		end if;
	end if;
	end;
end;
/ 


-- INIZIO SCRIPT DEL 18.10.2010 ALESSIO
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASSOCIAZIONE_TEMPLATES';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASSOCIAZIONE_TEMPLATES' and column_name='CODICE_DB';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASSOCIAZIONE_TEMPLATES ADD CODICE_DB VARCHAR(50)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASSOCIAZIONE_TEMPLATES';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASSOCIAZIONE_TEMPLATES' and column_name='MANUAL_INSERT';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASSOCIAZIONE_TEMPLATES ADD MANUAL_INSERT INT';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASS_TEMPLATES_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASS_TEMPLATES_FASC' and column_name='CODICE_DB';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASS_TEMPLATES_FASC ADD CODICE_DB VARCHAR(50)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASS_TEMPLATES_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASS_TEMPLATES_FASC' and column_name='MANUAL_INSERT';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASS_TEMPLATES_FASC ADD MANUAL_INSERT INT';
        end if;
    end if;
    end;
end;
/

-- FINE SCRIPT DEL 18.10.2010 ALESSIO

-- INIZIO SCRIPT DEL 18.10.2010 RICCIUTI
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_TIMER_DISSERVIZIO';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_CHIAVI_CONFIGURAZIONE'||
            '(SYSTEM_ID'||
            ',ID_AMM'||
           ',VAR_CODICE'||
           ',VAR_DESCRIZIONE'||
           ',VAR_VALORE' ||
           ',CHA_TIPO_CHIAVE' ||
           ',CHA_VISIBILE' ||
           ',CHA_MODIFICABILE' ||
           ',CHA_GLOBALE' ||
           ')' ||
     'VALUES' ||
           '(SEQ_DPA_CHIAVI_CONFIG.nextval' ||
           ',0' ||
           ', ''FE_TIMER_DISSERVIZIO'' '||
           ', ''Configurazione Timer DIsservizio, valore corrispondente a intervallo del Timer in millisecondi'''||
           ',''60000''' ||
           ',''F''' ||
           ',''1''' ||
           ',''1''' ||
           ',''1''' || 
           ')';
        end if;
    end if;
    end;
end;
/
-- FINE SCRIPT DEL 18.10.2010 RICCIUTI

-- INIZIO SCRIPT DEL 18.10.2010 VELTRI
-- REPORT FASCICOLI
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='EXP_FASC_COUNT';
    IF (cnt = 0) THEN        
       INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
        VALUES ('EXP_FASC_COUNT' , 'Abilita l''utente ai report dei fascicoli', NULL, 'N');
    END IF;
    END;
END;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='PROJECT';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='PROJECT' and column_name='ID_RUOLO_CHIUSURA';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD ID_RUOLO_CHIUSURA NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='PROJECT';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='PROJECT' and column_name='ID_UO_CHIUSURA';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD ID_UO_CHIUSURA NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='PROJECT';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='PROJECT' and column_name='ID_AUTHOR_CHIUSURA';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD ID_AUTHOR_CHIUSURA NUMBER';
        end if;
    end if;
    end;
end;
/
-- FINE SCRIPT DEL 18.10.2010 VELTRI


-- INIZIO SCRIPT DEL 18.20.2010 BUONO
begin
    declare cnt int;  
  begin
      select count(*) into cnt from DPA_TIPO_OGGETTO where DESCRIZIONE = 'ContatoreSottocontatore';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO (SYSTEM_ID, TIPO, DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO.NEXTVAL, ''ContatoreSottocontatore'', ''ContatoreSottocontatore'')';
      end if;
  end;
end;
/

begin
    declare cnt int;
  begin
      select count(*) into cnt from DPA_TIPO_OGGETTO_FASC where DESCRIZIONE = 'ContatoreSottocontatore';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO_FASC (SYSTEM_ID, TIPO, DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO_FASC.NEXTVAL, ''ContatoreSottocontatore'', ''ContatoreSottocontatore'')';
      end if;
  end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='MODULO_SOTTOCONTATORE';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD MODULO_SOTTOCONTATORE NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM' and column_name='MODULO_SOTTOCONTATORE';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM ADD MODULO_SOTTOCONTATORE NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CONTATORI_DOC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_CONTATORI_DOC' and column_name='VALORE_SC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_CONTATORI_DOC ADD VALORE_SC INTEGER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CONTATORI_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_CONTATORI_FASC' and column_name='VALORE_SC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_CONTATORI_FASC ADD VALORE_SC INTEGER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASSOCIAZIONE_TEMPLATES';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASSOCIAZIONE_TEMPLATES' and column_name='VALORE_SC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASSOCIAZIONE_TEMPLATES ADD VALORE_SC NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASS_TEMPLATES_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASS_TEMPLATES_FASC' and column_name='VALORE_SC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASS_TEMPLATES_FASC ADD VALORE_SC NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASSOCIAZIONE_TEMPLATES';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASSOCIAZIONE_TEMPLATES' and column_name='DTA_INS';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASSOCIAZIONE_TEMPLATES ADD DTA_INS DATE';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASS_TEMPLATES_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASS_TEMPLATES_FASC' and column_name='DTA_INS';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASS_TEMPLATES_FASC ADD DTA_INS DATE';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;  
  begin
      select count(*) into cnt from DPA_TIPO_OGGETTO where DESCRIZIONE = 'Separatore';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO (SYSTEM_ID, TIPO, DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO.NEXTVAL, ''Separatore'', ''Separatore'')';
      end if;
  end;
end;
/

begin
    declare cnt int;
  begin
      select count(*) into cnt from DPA_TIPO_OGGETTO_FASC where DESCRIZIONE = 'Separatore';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO_FASC (SYSTEM_ID, TIPO, DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO_FASC.NEXTVAL, ''Separatore'', ''Separatore'')';
      end if;
  end;
end;
/
-- FINE SCRIPT DEL 18.10.2010 BUONO


-- INIZIO SCRIPT DEL 19.10.2010 RICCIUTI
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_ATTO';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' and column_name='PATH_MOD_EXC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_TIPO_ATTO ADD Path_Mod_Exc VARCHAR(255)';
        end if;
    end if;
    end;
end;
/
-- FINE SCRIPT DEL 19.10.2010 RICCIUTI

-- INIZIO SCRIPT VELTRI DEL 20.10.2010
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_DETTAGLIO_TRASM_TODOLIST';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_CHIAVI_CONFIGURAZIONE'||
            '(SYSTEM_ID'||
            ',ID_AMM'||
           ',VAR_CODICE'||
           ',VAR_DESCRIZIONE'||
           ',VAR_VALORE' ||
           ',CHA_TIPO_CHIAVE' ||
           ',CHA_VISIBILE' ||
           ',CHA_MODIFICABILE' ||
           ',CHA_GLOBALE' ||
           ')' ||
     'VALUES' ||
           '(SEQ_DPA_CHIAVI_CONFIG.nextval' ||
           ',0' ||
           ', ''FE_DETTAGLIO_TRASM_TODOLIST'' '||
           ', ''Utilizzata per visualizzare l intero dettaglio della trasmissione dalla todolist'''||
           ',''1''' ||
           ',''F''' ||
           ',''1''' ||
           ',''1''' ||
           ',''1''' || 
           ')';
        end if;
    end if;
    end;
end;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM user_tables
       WHERE table_name = 'DPA_CHIAVI_CONFIGURAZIONE';

      IF (cnt != 0)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_chiavi_configurazione
          WHERE var_codice = 'FE_ESTRAZIONE_LOG';

         IF (cnt = 0)
         THEN
            INSERT INTO DPA_CHIAVI_CONFIGURAZIONE
                               (SYSTEM_ID
                               ,ID_AMM
                               ,VAR_CODICE
                               ,VAR_DESCRIZIONE
                               ,VAR_VALORE
                               ,CHA_TIPO_CHIAVE
                               ,CHA_VISIBILE
                               ,CHA_MODIFICABILE
                               ,CHA_GLOBALE
                               )
                               VALUES
                               (SEQ_DPA_CHIAVI_CONFIG.nextval
                               ,0
                               , 'FE_ESTRAZIONE_LOG'
                               , 'Utilizzata per abilitare estrazione dei log da amministrazione'
                               ,'0'
                               ,'F'
                               ,'0'
                               ,'1'
                               ,'1'
                               );
         END IF;
      END IF;
   END;
END;
/

-- FINE SCRIPT VELTRI DEL 20.10.2010



-- INIZIO SCRIPT FREZZA-PALUMBO DEL 13-10-2010
BEGIN
   DECLARE
      cnt          INT;
 
      CURSOR c_fd
      IS SELECT fd.system_id, fd.file_type_used
           FROM dpa_formati_documento fd;
 
      stringasql   varCHAR(200);

   BEGIN
      SELECT COUNT (*) INTO cnt
        FROM cols
       WHERE table_name = 'DPA_FORMATI_DOCUMENTO'
         AND column_name = 'FILE_TYPE_SIGNATURE';

      IF (cnt = 0)
      THEN
         EXECUTE IMMEDIATE 'ALTER TABLE DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_SIGNATURE INTEGER NULL';

-- valorizzo la colonna appena creata
         FOR my_c_fd IN c_fd
         LOOP
            stringasql := 'UPDATE dpa_formati_documento SET FILE_TYPE_SIGNATURE = '
               || my_c_fd.file_type_used
               || ' WHERE system_id = '
               || my_c_fd.system_id;

--            dbms_output.put_line(stringasql);

            EXECUTE IMMEDIATE stringasql;
         END LOOP;
      END IF;
   END;
END;
/

 
BEGIN
   DECLARE
      cnt          INT;
 
      CURSOR c_fd
      IS SELECT fd.system_id, fd.file_type_used
           FROM dpa_formati_documento fd;
 
      stringasql   varCHAR(200);

   BEGIN
      SELECT COUNT (*) INTO cnt
        FROM cols
       WHERE table_name = 'DPA_FORMATI_DOCUMENTO'
         AND column_name = 'FILE_TYPE_PRESERVATION';
 
      IF (cnt = 0)
      THEN
         EXECUTE IMMEDIATE 'ALTER TABLE DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_PRESERVATION INTEGER NULL';

-- valorizzo la colonna appena creata
         FOR my_c_fd IN c_fd
         LOOP
            stringasql := 'UPDATE dpa_formati_documento SET FILE_TYPE_PRESERVATION = '
               || my_c_fd.file_type_used
               || ' WHERE system_id = '
               || my_c_fd.system_id;
 
--           dbms_output.put_line(stringasql);
            EXECUTE IMMEDIATE stringasql;
         END LOOP;
      END IF;
   END;
END;
/

-- fine SCRIPT FREZZA-PALUMBO DEL 13-10-2010


-- INIZIO SCRIPT RICCIUTI 20.10.2010
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_EXPORT_DA_MODELLO';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_CHIAVI_CONFIGURAZIONE'||
            '(SYSTEM_ID'||
            ',ID_AMM'||
           ',VAR_CODICE'||
           ',VAR_DESCRIZIONE'||
           ',VAR_VALORE' ||
           ',CHA_TIPO_CHIAVE' ||
           ',CHA_VISIBILE' ||
           ',CHA_MODIFICABILE' ||
           ',CHA_GLOBALE' ||
           ')' ||
     'VALUES' ||
           '(SEQ_DPA_CHIAVI_CONFIG.nextval' ||
           ',0' ||
           ', ''FE_EXPORT_DA_MODELLO'' '||
           ', ''Abilitazione export ricerca da modello (0=disabilitato, 1= abilitato)'''||
           ',''0''' ||
           ',''F''' ||
           ',''0''' ||
           ',''1''' ||
           ',''1''' || 
           ')';
        end if;
    end if;
    end;
end;
/
-- FINE SCRIPT RICCIUTI 20.10.2010


begin
declare cnt int; 
begin
select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_VOCI_MENU_ADMIN' ;  

if (cnt=0) 
then
execute immediate 'CREATE SEQUENCE SEQ_DPA_VOCI_MENU_ADMIN
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER';
 end if;
 end; 
end;
/

-- Fine script P. De Luca 22-10-2010

--inizio script Dimitri de filippo 22/10/2010
BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM user_sequences
       WHERE sequence_name = 'SEQ_DPA_CHIAVI_CONFIG_TEMPLATE';

      IF (cnt = 0)
      THEN
         EXECUTE IMMEDIATE 'CREATE SEQUENCE SEQ_DPA_CHIAVI_CONFIG_TEMPLATE START WITH 1 INCREMENT BY 1 MINVALUE 1';
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM dpa_anagrafica_log l
       WHERE l.var_codice = 'MODIFICADOCSTATOFINALE';

      IF (cnt = 0)
      THEN
         INSERT INTO dpa_anagrafica_log
                     (system_id, var_codice,
                      var_descrizione,
                      var_oggetto, var_metodo
                     )
              VALUES (seq.NEXTVAL, 'MODIFICADOCSTATOFINALE',
                      'Azione di modifica dei diritti di lettura / scrittura sul documento in stato finale',
                      'DOCUMENTO', 'MODIFICADOCSTATOFINALE'
                     );
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM user_tables
       WHERE table_name = 'PROFILE';

      IF (cnt != 0)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM cols
          WHERE table_name = 'PROFILE'
            AND column_name = 'CHA_UNLOCKED_FINAL_STATE';

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE 'ALTER TABLE PROFILE ADD CHA_UNLOCKED_FINAL_STATE varchar(1) NULL';
         END IF;
      END IF;
   END;
END;
/

--inizio chiavi di configurazione

--Dimitri de filippo 22/10/2010

--creazione tabella template per le chiavi di configurazione non globali

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM user_tables
       WHERE table_name = 'DPA_CHIAVI_CONFIG_TEMPLATE';

      IF (cnt = 0)
      THEN
         EXECUTE IMMEDIATE    'CREATE TABLE DPA_CHIAVI_CONFIG_TEMPLATE '
                           || '( '
                           || 'SYSTEM_ID NUMBER NOT NULL, '
                           || 'VAR_CODICE varchar(32) NOT NULL, '
                           || 'VAR_DESCRIZIONE varchar(512), '
                           || 'VAR_VALORE varchar(32) NOT NULL, '
                           || 'CHA_TIPO_CHIAVE char(1), '
                           || 'CHA_VISIBILE char(1) DEFAULT 1, '
                           || 'CHA_MODIFICABILE char(1) DEFAULT 1 '
                           || ')';

         EXECUTE IMMEDIATE    'CREATE UNIQUE INDEX DPA_CHIAVI_CONFIG_TEMPLATE_PK ON DPA_CHIAVI_CONFIG_TEMPLATE '
                           || '(SYSTEM_ID) '
                           || 'LOGGING ';

         EXECUTE IMMEDIATE    'ALTER TABLE DPA_CHIAVI_CONFIG_TEMPLATE ADD ( '
                           || 'CONSTRAINT DPA_CHIAVI_CONFIG_TEMPLATE_PK '
                           || 'PRIMARY KEY '
                           || '(SYSTEM_ID) '
                           || 'USING INDEX ) ';
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM dpa_chiavi_config_template
       WHERE var_codice = 'FE_MAX_LENGTH_NOTE';

      IF (cnt = 0)
      THEN
         INSERT INTO dpa_chiavi_config_template
                     (system_id, var_codice, var_descrizione, var_valore,
                      cha_tipo_chiave, cha_visibile, cha_modificabile)
            SELECT seq_dpa_chiavi_config_template.NEXTVAL,
                   'FE_MAX_LENGTH_NOTE',
                   'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Note',
                   '2000', 'F', '1', '1'
              FROM DUAL;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM dpa_chiavi_config_template
       WHERE var_codice = 'FE_MAX_LENGTH_OGGETTO';

      IF (cnt = 0)
      THEN
         INSERT INTO dpa_chiavi_config_template
                     (system_id, var_codice, var_descrizione, var_valore,
                      cha_tipo_chiave, cha_visibile, cha_modificabile)
            SELECT seq_dpa_chiavi_config_template.NEXTVAL,
                   'FE_MAX_LENGTH_OGGETTO',
                   'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Oggetto',
                   '2000', 'F', '1', '1'
              FROM DUAL;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM dpa_chiavi_config_template
       WHERE var_codice = 'FE_MAX_LENGTH_DESC_FASC';

      IF (cnt = 0)
      THEN
         INSERT INTO dpa_chiavi_config_template
                     (system_id, var_codice, var_descrizione, var_valore,
                      cha_tipo_chiave, cha_visibile, cha_modificabile)
            SELECT seq_dpa_chiavi_config_template.NEXTVAL,
                   'FE_MAX_LENGTH_DESC_FASC',
                   'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Fascicolo',
                   '2000', 'F', '1', '1'
              FROM DUAL;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM dpa_chiavi_config_template
       WHERE var_codice = 'FE_MAX_LENGTH_DESC_TRASM';

      IF (cnt = 0)
      THEN
         INSERT INTO dpa_chiavi_config_template
                     (system_id, var_codice, var_descrizione, var_valore,
                      cha_tipo_chiave, cha_visibile, cha_modificabile)
            SELECT seq_dpa_chiavi_config_template.NEXTVAL,
                   'FE_MAX_LENGTH_DESC_TRASM',
                   'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Trasmissione',
                   '2000', 'F', '1', '1'
              FROM DUAL;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM dpa_chiavi_config_template
       WHERE var_codice = 'FE_MAX_LENGTH_DESC_ALLEGATO';

      IF (cnt = 0)
      THEN
         INSERT INTO dpa_chiavi_config_template
                     (system_id, var_codice, var_descrizione, var_valore,
                      cha_tipo_chiave, cha_visibile, cha_modificabile)
            SELECT seq_dpa_chiavi_config_template.NEXTVAL,
                   'FE_MAX_LENGTH_DESC_ALLEGATO',
                   'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Allegato ',
                   '200', 'F', '1', '1'
              FROM DUAL;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM dpa_chiavi_config_template
       WHERE var_codice = 'FE_RICEVUTA_PROTOCOLLO_PDF';

      IF (cnt = 0)
      THEN
         INSERT INTO dpa_chiavi_config_template
                     (system_id, var_codice, var_descrizione, var_valore,
                      cha_tipo_chiave, cha_visibile, cha_modificabile)
            SELECT seq_dpa_chiavi_config_template.NEXTVAL,
                   'FE_RICEVUTA_PROTOCOLLO_PDF',
                   'Chiave per l''impostazione del formato di stampa della ricevuta di un protocollo. Valori: 1= PDF; 0=stampa tramite activex ',
                   '0', 'F', '0', '1'
              FROM DUAL;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM dpa_voci_menu_admin
       WHERE var_codice = 'FE_ABILITA_GEST_DOCS_ST_FINALE';

      IF (cnt = 0)
      THEN
         INSERT INTO dpa_voci_menu_admin
                     (system_id, var_codice, var_descrizione,
                      var_visibilita_menu)
            SELECT (select max(m.system_id) +1 from  dpa_voci_menu_admin m),
                   'FE_ABILITA_GEST_DOCS_ST_FINALE',
                   'GESTIONE DOCS STATO FINALE', ''
              FROM DUAL;
      END IF;
   END;
END;
/

BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM dpa_chiavi_config_template
       WHERE var_codice = 'FE_ABILITA_GEST_DOCS_ST_FINALE';

      IF (cnt = 0)
      THEN
         INSERT INTO dpa_chiavi_config_template
                     (system_id, var_codice, var_descrizione, var_valore,
                      cha_tipo_chiave, cha_visibile, cha_modificabile)
            SELECT seq_dpa_chiavi_config_template.NEXTVAL,
                   'FE_ABILITA_GEST_DOCS_ST_FINALE',
                   'Chiave per consentire lo sblocco dei documenti in stato finale. Valori: 1= CONSENTI; 0=NON CONSENTIRE ',
                   '0', 'F', '0', '0'
              FROM DUAL;
      END IF;
   END;
END;
/

--Dimitri de Filippo

--Stored che scorre la tabella DPA_CHIAVI_CONFIG_TEMPLATE e
--inserisce le corrispondenti chiavi non globali nella tabella DPA_CHIAVI_CONFIGURAZIONE per ciascuna amministrazione

CREATE OR REPLACE PROCEDURE crea_keys_amministra
IS
   syscurramm   INT;
   syscurrkey   VARCHAR (32);
   cnt          INT;

   CURSOR curramm                    -- CURSORE CHE SCORRE LE AMMINISTRAZIONI
   IS
      SELECT system_id
        FROM dpa_amministra;

   CURSOR currkey
     -- CURSORE CHE SCORRE LE CHIAVI della tabella DPA_CHIAVI_CONFIG_TEMPLATE
   IS
      SELECT var_codice
        FROM dpa_chiavi_config_template;
BEGIN
   OPEN curramm;

   LOOP
      FETCH curramm
       INTO syscurramm;

      EXIT WHEN curramm%NOTFOUND;

      BEGIN
---cursore annidato x le chiavi di configurazione
         BEGIN
            OPEN currkey;

            LOOP
               FETCH currkey
                INTO syscurrkey;

               EXIT WHEN currkey%NOTFOUND;

               BEGIN
                  SELECT COUNT (*)
                    INTO cnt
                    FROM dpa_chiavi_configurazione
                   WHERE var_codice = syscurrkey AND id_amm = syscurramm;
               END;

               BEGIN
                  IF (cnt = 0)
                  THEN
                     INSERT INTO dpa_chiavi_configurazione
                                 (system_id, id_amm, var_codice,
                                  var_descrizione, var_valore,
                                  cha_tipo_chiave, cha_visibile,
                                  cha_modificabile, cha_globale)
                        (SELECT 
						-- modifica di P. De Luca a seguito ticket ETT000000016485 di I. Giovacchini 
						SEQ_DPA_CHIAVI_CONFIG.nextval,
						-- (SELECT MAX (system_id) + 1 FROM dpa_chiavi_configurazione),
                                syscurramm AS id_amm, var_codice,
                                var_descrizione, var_valore, cha_tipo_chiave,
                                cha_visibile, cha_modificabile, '0'
                           FROM dpa_chiavi_config_template
                          WHERE var_codice = syscurrkey AND ROWNUM = 1);
                  END IF;
               END;
            END LOOP;

            CLOSE currkey;
         END;
--- fine cursore annidato per chiavi di configurazione
      END;
   END LOOP;

   CLOSE curramm;

   COMMIT;
END;
/

/
----------------------------------------fine gestione chiavi configurazione
--fine script Dimitri de filippo 22/10/2010


-- script Ferro 23-10.2010
BEGIN
   DECLARE cnt   INT;
   BEGIN
        SELECT COUNT (*) INTO cnt FROM user_tables WHERE table_name = 'DPA_CHIAVI_CONFIGURAZIONE';
             IF (cnt != 0) THEN
                    SELECT COUNT (*) INTO cnt FROM dpa_chiavi_configurazione WHERE var_codice = 'BE_URL_WA';
              IF (cnt = 0) THEN
                    INSERT INTO DPA_CHIAVI_CONFIGURAZIONE
                           (SYSTEM_ID, ID_AMM,VAR_CODICE,
                           VAR_DESCRIZIONE,VAR_VALORE,
                           CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,
                           CHA_GLOBALE
                           )
                           VALUES
                           (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'BE_URL_WA', 
						   'Utilizzata per identificare il path del Front end per esportare il link tramite web service',
                           '1',
                           'B','1','1',
                           '1'
                           );
         END IF;
      END IF;
   END;
END;
/
-- fine script Ferro 23-10.2010





-- script Veltri 23-10-2010
BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (*)
        INTO cnt
        FROM user_tables
       WHERE table_name = 'DPA_CHIAVI_CONFIGURAZIONE';

      IF (cnt != 0)
      THEN
         SELECT COUNT (*)
           INTO cnt
           FROM dpa_chiavi_configurazione
          WHERE var_codice = 'FE_TAB_TRASM_ALL';

         IF (cnt = 0)
         THEN
            INSERT INTO dpa_chiavi_configurazione
                        (system_id, id_amm, var_codice,
                         var_descrizione,
                         var_valore, cha_tipo_chiave, cha_visibile,
                         cha_modificabile, cha_globale
                        )
                 VALUES (SEQ_DPA_CHIAVI_CONFIG.nextval, 0, 'FE_TAB_TRASM_ALL',
                         'Utilizzata per vedere dal tab classifica tutti i fascicoli',
                         '0', 'F', '0',
                         '1', '1'
                        );
         END IF;
      END IF;
   END;
END;
/
-- fine script Veltri 23-10-2010



-- script ALessio Di Bartolo 25-10-2010
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI f where f.COD_FUNZIONE ='DO_AMM_CONSERVAZIONE_WA';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('DO_AMM_CONSERVAZIONE_WA', 'Permette l''amministrazione della WA di Conservazione', '', 'N');
    end if;
    end;
end;
/

-- chiavi configurazione by Alessio DiMauro Bartolo 
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_MODELLI_DELEGA';
    IF (cnt = 0) THEN        
       insert into DPA_CHIAVI_CONFIGURAZIONE
         (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
         ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE
         ,CHA_MODIFICABILE,CHA_GLOBALE)
        values 
        (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_MODELLI_DELEGA', 'Abilitazione dei modelli di delega'
        , '0','F','0'
        ,'1','1');
    END IF;
    END;
END;
/
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_STAMPA_UNIONE';
    IF (cnt = 0) THEN        
       insert into DPA_CHIAVI_CONFIGURAZIONE
         (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
         ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE
         ,CHA_MODIFICABILE,CHA_GLOBALE)
        values 
        (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_STAMPA_UNIONE', 'Chiave utilizzata per la gestione della stampa unione'
        , '0','F','0'
        ,'1','1');
    END IF;
    END;
END;
/

-- fine script ALessio DiMauro Bartolo 25-10-2010



begin
    declare cnt int;
  begin
      select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE = 'MASSIVE_REMOVE_VERSIONS';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE,CHA_TIPO_FUNZ, DISABLED) VALUES (''MASSIVE_REMOVE_VERSIONS'', ''Abilita la rimozione massiva delle versioni dei doc grigi'', null,''N'')';
      end if;
  end;
end;
/

-- script Frezza 
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE l where l.var_codice ='BE_CONSOLIDAMENTO';
    
    if (cnt = 0) then       
    insert into DPA_CHIAVI_CONFIGURAZIONE
    (
    system_id,
    id_amm,
    var_codice,
    var_descrizione,
    var_valore,
    cha_tipo_chiave,
    cha_visibile,
    cha_modificabile,
    cha_globale,
    var_codice_old_webconfig
    )
    values 
    (
    SEQ_DPA_CHIAVI_CONFIG.nextval,
    0,
    'BE_CONSOLIDAMENTO',
    'Abilitazione della funzione di consolidamento dello stato di un documento',
    '0',
    'B',
    '0',
    '1',
    '1',
    null
    );
    end if;
  end;
end;
/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l where l.var_codice ='CONSOLIDADOCUMENTO';
    if (cnt = 0) then       
    insert into dpa_anagrafica_log
    (
    system_id,
    var_codice,
    var_descrizione,
    var_oggetto,
    var_metodo
    )
    values 
    (
    seq.nextval,
    'CONSOLIDADOCUMENTO',
    'Azione di consolidamento dello stato di un documento',
    'DOCUMENTO',
    'CONSOLIDADOCUMENTO'
    );
    end if;
  end;
end;
/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI f where f.COD_FUNZIONE ='DO_CONSOLIDAMENTO';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('DO_CONSOLIDAMENTO', 'Abilita il consolidamento di un documento', '', 'N');
    end if;
    end;
end;
/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI f where f.COD_FUNZIONE ='DO_CONSOLIDAMENTO_METADATI';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('DO_CONSOLIDAMENTO_METADATI', 'Abilita il consolidamento dei metadati di un documento', '', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_STATI' AND column_name='STATO_CONSOLIDAMENTO';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_STATI ADD STATO_CONSOLIDAMENTO CHAR(1) NULL';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from user_tables where table_name='DPA_CONSOLIDATED_DOCS';
        if (cnt = 0) then
            execute immediate 
            'CREATE TABLE DPA_CONSOLIDATED_DOCS
                (
                  ID                       NUMBER(10),
                  DOCNAME                  VARCHAR2(240 BYTE),
                  CREATION_DATE            DATE,
                  DOCUMENTTYPE             NUMBER(10),
                  AUTHOR                   NUMBER(10),
                  AUTHOR_NAME              VARCHAR2(4000 BYTE),
                  ID_RUOLO_CREATORE        NUMBER,
                  RUOLO_CREATORE           VARCHAR2(4000 BYTE),
                  NUM_PROTO                NUMBER(10),
                  NUM_ANNO_PROTO           NUMBER(10),
                  DTA_PROTO                DATE,
                  ID_PEOPLE_PROT           NUMBER,
                  PEOPLE_PROT              VARCHAR2(4000 BYTE),
                  ID_RUOLO_PROT            NUMBER,
                  RUOLO_PROT               VARCHAR2(4000 BYTE),
                  ID_REGISTRO              NUMBER(10),
                  REGISTRO                 VARCHAR2(4000 BYTE),
                  CHA_TIPO_PROTO           VARCHAR2(1 BYTE),
                  VAR_PROTO_IN             VARCHAR2(128 BYTE),
                  DTA_PROTO_IN             DATE,
                  DTA_ANNULLA              DATE,
                  ID_OGGETTO               NUMBER(10),
                  VAR_PROF_OGGETTO         VARCHAR2(2000 BYTE),
                  MITT_DEST                VARCHAR2(4000 BYTE),
                  ID_DOCUMENTO_PRINCIPALE  INTEGER
                )';            
        end if;
    end;
end;
/
-- fine script Frezza 

-- chiavi di anagrafica funzioni
begin
    declare cnt int;

  begin
      select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE = 'GEST_SU';

      if (cnt = 0) then
  INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE,CHA_TIPO_FUNZ, DISABLED)
  VALUES ('GEST_SU', 'Abilita il sottomenu Stampa unione del menu Gestione', null,'N');

      end if;
  end;
end;
/
 

-- fine chiavi di anagrafica funzioni

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_NUOVO_FASC_DIRECT' ;
	if (cnt = 0) then		
	     INSERT INTO DPA_CHIAVI_CONFIGURAZIONE 
   (SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
      VALUES (
    SEQ_DPA_CHIAVI_CONFIG.NEXTVAL
	, 0
	, 'FE_NUOVO_FASC_DIRECT'
	, 'flag attivazione creazione nuovo fasc da maschera di protocollo/documento (0=disattivato; 1= attivato)'
	, '0'
	, 'F'
	, '0'
	, '1'
	, '1')
;
    end if;
    end;
end;
/ 



/*
AUTORE:						FREZZA s.
Data creazione:				29/10/2010
Scopo della modifica:		INSERITO FLAG "HIDE_DOC_VERSIONS" PER GESTIRE LA VISIBILITA' DELLE VERSIONI SU UN DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/


begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DELETED_SECURITY' AND column_name='HIDE_DOC_VERSIONS';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DELETED_SECURITY ADD HIDE_DOC_VERSIONS CHAR(1)';
        end if;
    end;
end;
/

-- ALTER TABLE SECURITY  ADD (TS_INSERIMENTO   TIMESTAMP(6) );
-- ALTER TABLE SECURITY  MODIFY TS_INSERIMENTO DEFAULT systimestamp; 
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'SECURITY';
      nomecolonna   VARCHAR2 (32)  := 'TS_INSERIMENTO';
      tipodato      VARCHAR2 (200) := ' TIMESTAMP(6) ';
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
                              || ' modify '|| nomecolonna
                              || ' DEFAULT systimestamp';
         END IF;
      END IF;
   END;
END;
/



/*
AUTORE:						FREZZA s.
Data creazione:				29/10/2010
Scopo della modifica:		INSERITO FLAG "HIDE_DOC_VERSIONS" PER GESTIRE LA VISIBILITA' DELLE VERSIONI SU UN DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/


begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DELETED_SECURITY' AND column_name='HIDE_DOC_VERSIONS';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DELETED_SECURITY ADD HIDE_DOC_VERSIONS CHAR(1)';
        end if;
    end;
end;
/




/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITO FLAG "HIDE_DOC_VERSIONS" PER GESTIRE LA VISIBILITA' DELLE VERSIONI SU UN DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/


begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_MODELLI_MITT_DEST' AND column_name='HIDE_DOC_VERSIONS';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_MODELLI_MITT_DEST ADD HIDE_DOC_VERSIONS CHAR(1)';
        end if;
    end;
end;
/
/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITI CAMPO "STATO_CONSOLIDAMENTO" PER LA GESTIONE DEL CONSOLIDAMENTO DEL DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSOLIDAMENTO DOCUMENTI"
*/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_STATI' AND column_name='STATO_CONSOLIDAMENTO';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_STATI ADD STATO_CONSOLIDAMENTO CHAR(1) NULL';
        end if;
    end;
end;
/

/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITO FLAG "HIDE_DOC_VERSIONS" PER GESTIRE LA VISIBILITA' DELLE VERSIONI SU UN DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_TRASM_SINGOLA' AND column_name='HIDE_DOC_VERSIONS';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_TRASM_SINGOLA ADD HIDE_DOC_VERSIONS CHAR(1)';
        end if;
    end;
end;
/
/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITI CAMPI PER LA GESTIONE DEL CONSOLIDAMENTO DEL DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSOLIDAMENTO DOCUMENTI"
*/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='CONSOLIDATION_STATE';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CONSOLIDATION_STATE CHAR(1)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='CONSOLIDATION_AUTHOR';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CONSOLIDATION_AUTHOR NUMBER(10)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='CONSOLIDATION_ROLE';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CONSOLIDATION_ROLE NUMBER(10)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='CONSOLIDATION_DATE';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CONSOLIDATION_DATE DATE';
        end if;
    end;
end;
/
/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITO FLAG "HIDE_DOC_VERSIONS" PER GESTIRE LA VISIBILITA' DELLE VERSIONI SU UN DOCUMENTO

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='SECURITY' AND column_name='HIDE_DOC_VERSIONS';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE SECURITY ADD HIDE_DOC_VERSIONS CHAR(1)';
        end if;
    end;
end;
/
/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della modifica:		INSERITA NUOVA TABELLA "DPA_CONSOLIDATED_DOCS" IN CUI VENGONO MEMORIZZATI
							I METADATI PRINCIPALI DEI DOCUMENTI CONSOLIDATI IN STATO 2

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSOLIDAMENTO DOCUMENTI"
*/
begin
    declare cnt int;
    begin
        select count(*) into cnt from user_tables where table_name='DPA_CONSOLIDATED_DOCS';
        if (cnt = 0) then
            execute immediate 
            'CREATE TABLE DPA_CONSOLIDATED_DOCS
                (
                  ID                       NUMBER(10),
                  DOCNAME                  VARCHAR2(240 BYTE),
                  CREATION_DATE            DATE,
                  DOCUMENTTYPE             NUMBER(10),
                  AUTHOR                   NUMBER(10),
                  AUTHOR_NAME              VARCHAR2(4000 BYTE),
                  ID_RUOLO_CREATORE        NUMBER,
                  RUOLO_CREATORE           VARCHAR2(4000 BYTE),
                  NUM_PROTO                NUMBER(10),
                  NUM_ANNO_PROTO           NUMBER(10),
                  DTA_PROTO                DATE,
                  ID_PEOPLE_PROT           NUMBER,
                  PEOPLE_PROT              VARCHAR2(4000 BYTE),
                  ID_RUOLO_PROT            NUMBER,
                  RUOLO_PROT               VARCHAR2(4000 BYTE),
                  ID_REGISTRO              NUMBER(10),
                  REGISTRO                 VARCHAR2(4000 BYTE),
                  CHA_TIPO_PROTO           VARCHAR2(1 BYTE),
                  VAR_PROTO_IN             VARCHAR2(128 BYTE),
                  DTA_PROTO_IN             DATE,
                  DTA_ANNULLA              DATE,
                  ID_OGGETTO               NUMBER(10),
                  VAR_PROF_OGGETTO         VARCHAR2(2000 BYTE),
                  MITT_DEST                VARCHAR2(4000 BYTE),
                  ID_DOCUMENTO_PRINCIPALE  INTEGER
                )';            
        end if;
    end;
end;
/


-- script Furnari x  griglie custom 29-10-2010
begin
    declare cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_GRIDS';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_GRIDS START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_GRIDS';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_GRIDS           (
            SYSTEM_ID	INT NOT NULL,
            USER_ID		INT,
            ROLE_ID		INT,
            ADMINISTRATION_ID INT,
            SEARCH_ID	INT,
            SERIALIZED_GRID CLOB,
			TYPE_GRID          VARCHAR2(30 BYTE),
            CONSTRAINT PK_DPA_GRIDS PRIMARY KEY (SYSTEM_ID)             )';
        end if;
    end;    
end;    
/     
-- fine script Furnari 

/*
AUTORE:			Federico Ricciuti			
Data creazione:		04-11-10		
Scopo della modifica:    Creazione tabella per lo storico della data e ora arrivo della protocollazione, più relative 
                            microfunzioni per l'abilitazione della modifica e dello storico delle modifiche.
*/  

-- Creazione tabella DPA_DATA_ARRIVO_STO
begin
declare cnt int;
begin
select count(*) into cnt from user_tables where table_name='DPA_DATA_ARRIVO_STO';
        if (cnt = 0) then
            execute immediate
                'CREATE TABLE DPA_DATA_ARRIVO_STO' ||
                '('||
                      'SYSTEM_ID     INTEGER NOT NULL primary key ,' ||
                      'DOCNUMBER     INTEGER NOT NULL references PROFILE (SYSTEM_ID),' ||
                      'DTA_ARRIVO    DATE,' ||
                      'ID_GROUP      INTEGER NOT NULL,' ||
                      'ID_PEOPLE     INTEGER NOT NULL references PEOPLE (SYSTEM_ID),' ||
                      'DTA_MODIFICA  DATE' ||
                ') ';
                
            end if;
            end;
            end;
            /

-- Sequence per la tabella DPA_DATA_ARRIVO_STO
begin
	declare
		   cnt int;
	begin	
		select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_DATA_ARRIVO_STO';
		if (cnt = 0) then
		   execute immediate 'CREATE SEQUENCE SEQ_DPA_DATA_ARRIVO_STO START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE NOCACHE NOORDER';
		end if;	
	end;
end;
/
---fine creazione tabelle

--Inserimento Microfunzione per abilitare la modifica di ora pervenuto (DO_PROT_DATA_ORA_MODIFICA)
begin
    declare cnt int;
  begin
    select count(*) into cnt from  DPA_ANAGRAFICA_FUNZIONI where Cod_funzione='DO_PROT_DATA_ORA_MODIFICA';
    if (cnt = 0) then
		begin
           execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI(COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED) Values (''DO_PROT_DATA_ORA_MODIFICA'', ''Abilito la modifica di data arrivo e ora pervenuto nella protocollazione'', ''N'')';
		end;
	end if;
	end;
end;
/
--fine inserimento Microfunzione DO_PROT_DATA_ORA_MODIFICA

--inserimento Microfunzione per abilitare il tasto dello storico delle modifiche (DO_PROT_DATA_ORA_STORIA)

begin
    declare cnt int;
  begin
    select count(*) into cnt from  DPA_ANAGRAFICA_FUNZIONI where Cod_funzione='DO_PROT_DATA_ORA_STORIA';
    if (cnt = 0) then
		begin
           execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI(COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED) Values (''DO_PROT_DATA_ORA_STORIA'', ''Abilita il pulsante per lo storico della data arrivo e ora pervenuto nella creazione del protocollo'', ''N'')';
		end;
	end if;
	end;
end;
/
---fine inserimento Microfunzione DO_PROT_DATA_ORA_STORIA
----fine script Ricciuti

-- Microfunzione personalizzazione griglia

/*
 * Autore:					FURNARI
 * Data creazione:			18/11/2010
 * Scopo dell'inserimento:	Microfunzione per abilitare un ruolo alla personalizzazione delle griglie di ricerca documenti / fascicolo.
 *
*/
begin
    declare cnt int;
  begin
    select count(*) into cnt from  DPA_ANAGRAFICA_FUNZIONI where Cod_funzione='GRID_PERSONALIZATION';
    if (cnt = 0) then
		begin
           INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) VALUES ('GRID_PERSONALIZATION', 'Abilita ruolo alla personalizzazione delle griglie di ricerca.', '', 'N');
		end;
	end if;
	end;
end;
/
-- Fine microfunzione personalizza griglia

    /*
AUTORE:						BUONO
Data creazione:				15/11/2010
Scopo dell'inserimento:		INSERIMENTO CHIAVE DI CONFIGURAZIONE PER ATTIVARE LA GESTIONE DEI CONTATORI CON SOTTOCONTATORE DEI DOCUMENTI
Sviluppi CDC
*/
begin
declare cnt int;
begin
select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE l where l.var_codice ='GEST_CONTATORI';

if (cnt = 0) then
insert into DPA_CHIAVI_CONFIGURAZIONE
(
system_id,
id_amm,
var_codice,
var_descrizione,
var_valore,
cha_tipo_chiave,
cha_visibile,
cha_modificabile,
cha_globale,
var_codice_old_webconfig
)
values
(
SEQ_DPA_CHIAVI_CONFIG.nextval,
0,
'GEST_CONTATORI',
'Abilitazione della funzione di gestione contatori',
'0',
'F',
'0',
'1',
'1',
null
);
end if;
end;
end;
/
 
/*

Convenzione per il nome del file: 0.getChaConsentiClass.sql

----------
Inserire SEMPRE i seguenti COMMENTI nel CORPO della funzione (non dello script): 
AUTORE:		GIAMBOI VERONICA
Data creazione: 18/11/2010
Scopo della funzione: Recuperare il campo consenti classificazione per un dato fascicolo o sottofascicolo

Indicazione della MEV o dello sviluppo a cui è collegata la funzione: INPS - MEV 3491
*/


CREATE OR REPLACE FUNCTION getChaConsentiClass (p_id_parent number, p_cha_tipo_proj varchar2, p_id_fascicolo number)
RETURN VARCHAR2
IS
risultato   VARCHAR2 (16);
BEGIN

BEGIN

BEGIN
risultato := '0';

if (p_cha_tipo_proj = 'F') 
then select cha_consenti_class into risultato from project where system_id = p_id_parent;
end if;
    
if (p_cha_tipo_proj = 'C')
then select cha_consenti_class into risultato from project 
        where system_id in (select id_parent from project where system_id = p_id_fascicolo);
end if;

END;

END;

RETURN risultato;
END getChaConsentiClass; 
/

/*
AUTORE:						LORUSSO
Data creazione:				12/11/2010
Scopo dell'inserimento:		INSERIMENTO RECORD IN ANAGRAFICA LOG PER MONITORARE IL RIMUOVI DALLA TODOLIST.

Indicazione della MEV o dello sviluppo a cui è collegata la modifica:
							
*/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l where l.var_codice ='SVUOTA_TDL';
    if (cnt = 0) then       
	insert into dpa_anagrafica_log
	(
	system_id,
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	seq.nextval,
	'SVUOTA_TDL',
	'Rimozione documenti o fascicoli dalla todolist',
	'TRASMISSIONE',
	'SVUOTA_TDL'
	);
    end if;
  end;
end;
/ 

/*

Convenzione per il nome del file: 0_SP_MODIFY_CORR_ESTERNO.SQL

----
Inserire SEMPRE i seguenti COMMENTI nel CORPO della Procedura (non dello script): 
AUTORE:	LORUSSO	
Data creazione: 19/11/2010
Scopo della Procedura: modifica della SP già esistente per inserimento dati in dpa_dett_globali nel caso in cui manchi il record relativo

Principali funzionalità che attivano la chiamata alla Procedura:

Indicazione della MEV o dello sviluppo a cui è collegata la Procedura:
*/


CREATE OR REPLACE PROCEDURE sp_modify_corr_esterno (
   idcorrglobale     IN       NUMBER,
   desc_corr         IN       VARCHAR2,
   nome              IN       VARCHAR2,
   cognome           IN       VARCHAR2,
   codice_aoo        IN       VARCHAR2,
   codice_amm        IN       VARCHAR2,
   email             IN       VARCHAR2,
   indirizzo         IN       VARCHAR2,
   cap               IN       VARCHAR2,
   provincia         IN       VARCHAR2,
   nazione           IN       VARCHAR2,
   citta             IN       VARCHAR2,
   cod_fiscale       IN       VARCHAR2,
   telefono          IN       VARCHAR2,
   telefono2         IN       VARCHAR2,
   note              IN       VARCHAR2,
   fax               IN       VARCHAR2,
   var_iddoctype     IN       NUMBER,
   inrubricacomune   IN       CHAR,
   tipourp           IN       CHAR,
   localita             IN          VARCHAR2,
   luogoNascita      IN       VARCHAR2,
   dataNascita       IN       VARCHAR2,
   titolo            IN       VARCHAR2,
   newid             OUT      NUMBER,  
   returnvalue       OUT      NUMBER
)
IS
BEGIN
   DECLARE
      cnt   integer; 
      cod_rubrica              VARCHAR2 (128);
      id_reg                   NUMBER;
      idamm                    NUMBER;
      new_var_cod_rubrica      VARCHAR2 (128);
      cha_dettaglio            CHAR (1):= '0';
      cha_tipourp             CHAR (1);
      myprofile                NUMBER;
      new_idcorrglobale        NUMBER;
      identitydettglobali      NUMBER;
      outvalue                 NUMBER         := 1;
      rtn                      NUMBER;
      v_id_doctype             NUMBER;
      identitydpatcanalecorr   NUMBER;
      chaTipoIE                CHAR (1);
      numLivello               NUMBER          := 0;
      idParent                 NUMBER;
      idPesoOrg                NUMBER;
      idUO                     NUMBER;
      idGruppo                 NUMBER;
      idTipoRuolo              NUMBER;
      cha_tipo_corr            CHAR (1);
       chapa            CHAR (1);
   BEGIN
 
      <<reperimento_dati>>
      BEGIN
         SELECT var_cod_rubrica, cha_tipo_urp, id_registro, id_amm,cha_pa, cha_tipo_ie, num_livello, id_parent, id_peso_org, id_uo, id_tipo_ruolo, id_gruppo
           INTO cod_rubrica, cha_tipourp, id_reg, idamm, chapa, chaTipoIE, numLivello, idParent, idPesoOrg, IdUO, idTipoRuolo, idGruppo
           FROM dpa_corr_globali
          WHERE system_id = idcorrglobale;
          dbms_output.put_line('select effettuata') ; 
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
         dbms_output.put_line('primo blocco eccezione') ; 
            outvalue := 0;
            RETURN;
      END reperimento_dati;
      
      
    if(tipourp is not null and cha_tipourp is not null and cha_tipourp!=tipourp) then
        cha_tipourp := tipourp;
      
    end if;
      
 
      <<dati_canale_utente>>
      if cha_tipourp = 'P' 
      THEN
        BEGIN
         SELECT id_documenttype
           INTO v_id_doctype
           FROM dpa_t_canale_corr
          WHERE id_corr_globale = idcorrglobale;
        EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            dbms_output.put_line('2do blocco eccezione') ; 
            outvalue := 2;
        END dati_canale_utente;
      end if;
      
      IF /* 0 */ outvalue = 1
      THEN
         IF /* 1 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
         THEN
            cha_dettaglio := '1';
         END IF;                                                       /* 1 */
 
--VERIFICO se il corrisp Ã¨ stato utilizzato come dest/mitt di protocolli
         SELECT COUNT (id_profile)
           INTO myprofile
           FROM dpa_doc_arrivo_par
          WHERE id_mitt_dest = idcorrglobale;
          
-- 1) non Ã¨ stato mai utilizzato come corrisp in un protocollo
         IF /* 2 */ (myprofile = 0)
         THEN
            BEGIN
               UPDATE dpa_corr_globali
                  SET var_codice_aoo = codice_aoo,
                      var_codice_amm = codice_amm,
                      var_email = email,
                      var_desc_corr = desc_corr,
                      var_nome = nome,
                      var_cognome = cognome,
                      cha_pa=chapa,
                      cha_tipo_urp=cha_tipourp
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
               dbms_output.put_line('3o blocco eccezione') ; 
                  outvalue := 3;
                  RETURN;
            END;
 
/* SE L'UPDATE SU DPA_CORR_GLOBALI Ã¨ ANDTATA A BUON FINE
PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
*/
            IF /* 3 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
            THEN
 
               <<update_dpa_dett_globali>>
               BEGIN
                      
               
               
                     SELECT count(*) into cnt
                          FROM dpa_dett_globali
                          WHERE id_corr_globali = idcorrglobale ; 
                      
                      IF (cnt = 0)
                      THEN 
                       dbms_output.put_line('sono nella INSERT,id_corr_globali =  '||idcorrglobale) ; 
                      INSERT INTO dpa_dett_globali (
                          system_id,
                          id_corr_globali, 
                          var_indirizzo ,
                                 var_cap ,
                                 var_provincia ,
                                 var_nazione ,
                                 var_cod_fiscale ,
                                 var_telefono ,
                                 var_telefono2 ,
                                 var_note ,
                                 var_citta ,
                                 var_fax ,
                                 var_localita,
                                  var_luogo_nascita,
                                 dta_nascita,
                                 var_titolo)
                          VALUES (
                          seq.nextval,
                          idcorrglobale,
                          indirizzo,
                                 cap,
                                 provincia,
                                 nazione,
                                 cod_fiscale,
                                 telefono,
                                 telefono2,
                                 note,
                                 citta,
                                 fax,
                                 localita,
                                 luogoNascita,
                                 dataNascita,
                                 titolo);
                           END IF;
                           
                        IF (cnt = 1)
                      THEN    
                        dbms_output.put_line('sono nella UPDATE') ; 
                     UPDATE dpa_dett_globali SET
                             var_indirizzo = indirizzo,
                             var_cap = cap,
                             var_provincia = provincia,
                             var_nazione = nazione,
                             var_cod_fiscale = cod_fiscale,
                             var_telefono = telefono,
                             var_telefono2 = telefono2,
                             var_note = note,
                             var_citta = citta,
                             var_fax = fax,
                             var_localita = localita,
                          var_luogo_nascita = luogoNascita,
                         dta_nascita = dataNascita,
                         var_titolo = titolo
                          WHERE (id_corr_globali = idcorrglobale) ; 
                                              END IF;
                                               
                      /*
                      
                      MERGE INTO dpa_dett_globali
                        USING (
                          SELECT system_id as id_interno
                          FROM dpa_dett_globali
                          WHERE id_corr_globali = idcorrglobale) select_interna
                        ON (system_id = select_interna.id_interno)
                        WHEN MATCHED THEN
                          UPDATE SET
                             var_indirizzo = indirizzo,
                             var_cap = cap,
                             var_provincia = provincia,
                             var_nazione = nazione,
                             var_cod_fiscale = cod_fiscale,
                             var_telefono = telefono,
                             var_telefono2 = telefono2,
                             var_note = note,
                             var_citta = citta,
                             var_fax = fax,
                             var_localita = localita,
                          var_luogo_nascita = luogoNascita,
                         dta_nascita = dataNascita,
                         var_titolo = titolo
                         WHERE (id_corr_globali = idcorrglobale) 
                        WHEN NOT MATCHED THEN
                       INSERT (
                          system_id,
                          id_corr_globali, 
                          var_indirizzo ,
                                 var_cap ,
                                 var_provincia ,
                                 var_nazione ,
                                 var_cod_fiscale ,
                                 var_telefono ,
                                 var_telefono2 ,
                                 var_note ,
                                 var_citta ,
                                 var_fax ,
                                 var_localita,
                                  var_luogo_nascita,
                                 dta_nascita,
                                 var_titolo)
                         VALUES (
                          seq.nextval,
                          idcorrglobale,
                          indirizzo,
                                 cap,
                                 provincia,
                                 nazione,
                                 cod_fiscale,
                                 telefono,
                                 telefono2,
                                 note,
                                 citta,
                                 fax,
                                 localita,
                                 luogoNascita,
                                 dataNascita,
                                 titolo);

                          
                        */
                         commit; 
                                  dbms_output.put_line('sono nella merge') ; 
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('4o blocco eccezione'||SQLERRM) ; 
                     outvalue := 4;
                     RETURN;
               END update_dpa_dett_globali;
            END IF;                                                    /* 3 */
 
--METTI QUI UPDATE SU DPA_T_CANALE_CORR
--AGGIORNO LA DPA_T_CANALE_CORR
               BEGIN
                  UPDATE dpa_t_canale_corr
                     SET id_documenttype = var_iddoctype
                   WHERE id_corr_globale = idcorrglobale;
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('5o blocco eccezione') ; 
                     outvalue := 5;
                     RETURN;
               END;
         ELSE
-- caso 2) Il corrisp Ã¨ stato utilizzato come corrisp in un protocollo
-- NUOVO CODICE RUBRICA
            new_var_cod_rubrica :=
                                cod_rubrica || '_' || TO_CHAR (idcorrglobale);
 
            <<storicizzazione_corrisp>>
            BEGIN
               UPDATE dpa_corr_globali
                  SET dta_fine = SYSDATE,
                      var_cod_rubrica = new_var_cod_rubrica,
                      var_codice = new_var_cod_rubrica,
                      id_parent = NULL
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
                 dbms_output.put_line('6o blocco eccezione') ; 
                  outvalue := 6;
                  RETURN;
            END storicizzazione_corrisp;
 
            SELECT seq.NEXTVAL
              INTO newid
              FROM DUAL;
 
/* DOPO LA STORICIZZAZIONE DEL VECCHIO CORRISPONDENTE POSSO
INSERIRE IL NUOVO CORRISPONDENTE NELLA DPA_CORR_GLOBALI */
            <<inserimento_nuovo_corrisp>>
            BEGIN
               IF (inrubricacomune = '1')
               THEN
                  cha_tipo_corr := 'C';
               ELSE
                  cha_tipo_corr := 'S';
               END IF;
 
               INSERT INTO dpa_corr_globali
                           (system_id, num_livello, cha_tipo_ie, id_registro,
                            id_amm, var_desc_corr, var_nome, var_cognome,
                            id_old, dta_inizio, id_parent, var_codice,
                            cha_tipo_corr, cha_tipo_urp,
                            var_codice_aoo, var_cod_rubrica, cha_dettagli,
                            var_email, var_codice_amm, cha_pa, id_peso_org,
                            id_gruppo, id_tipo_ruolo, id_uo 
                           )
                    VALUES (newid, numLivello, chaTipoIE, id_reg,
                            idamm, desc_corr, nome, cognome,
                            idcorrglobale, SYSDATE, idParent, cod_rubrica,
                            cha_tipo_corr, cha_tipourp,
                            codice_aoo, cod_rubrica, cha_dettaglio,
                            email, codice_amm, chapa, idPesoOrg,
                            idGruppo, idTipoRuolo, idUO
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  dbms_output.put_line('7o blocco eccezione') ; 
                  outvalue := 7;
                  RETURN;
            END inserimento_nuovo_corrisp;
 
 
/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
E UNITA' ORGANIZZATIVE */
            IF /* 4 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
            THEN
--PRENDO LA SYSTEM_ID APPENA INSERITA
               SELECT seq.NEXTVAL
                 INTO identitydettglobali
                 FROM DUAL;
 
               <<inserimento_dettaglio_corrisp>>
               BEGIN
                  INSERT INTO dpa_dett_globali
                              (system_id, id_corr_globali, var_indirizzo,
                               var_cap, var_provincia, var_nazione,
                               var_cod_fiscale, var_telefono, var_telefono2,
                               var_note, var_citta, var_fax, var_localita, var_luogo_nascita, dta_nascita, var_titolo
                              )
                       VALUES (identitydettglobali, newid, indirizzo,
                               cap, provincia, nazione,
                               cod_fiscale, telefono, telefono2,
                               note, citta, fax, localita, luogoNascita, dataNascita, titolo
                              );
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('8o blocco eccezione') ; 
                     outvalue := 8;
                     RETURN;
               END inserimento_dettaglio_corrisp;
            END IF;                                                    /* 4 */
 
--INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
            <<inserimento_dpa_t_canale_corr>>
            BEGIN
               SELECT seq.NEXTVAL
                 INTO identitydpatcanalecorr
                 FROM DUAL;
 
               INSERT INTO dpa_t_canale_corr
                           (system_id, id_corr_globale, id_documenttype,
                            cha_preferito
                           )
                    VALUES (identitydpatcanalecorr, newid, var_iddoctype,
                            '1'
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  dbms_output.put_line('9o blocco eccezione') ; 
                  outvalue := 9;
                  RETURN;
            END inserimento_dpa_t_canale_corr;
         END IF;                                                       /* 2 */
      END IF /* 0 */;
 
      returnvalue := outvalue;
   END;
END;
/

/*
AUTORE:						FURNARI
Data creazione:				23/11/2010
Scopo della funzione:		Funzione per il recupero del nome di una ragione di trasmissione a partire dal suo id

Principali funzionalità che attivano la chiamata alla funzione:
							Odinamento per ragione di trasmissione delle trasmissioni ricevute ed effettuate

*/
CREATE OR REPLACE FUNCTION @db_user.GetTransReasonDesc(idReason NUMBER)
RETURN VARCHAR2 IS result VARCHAR2(16);

BEGIN
begin

SELECT R.VAR_DESC_RAGIONE INTO result FROM DPA_RAGIONE_TRASM R WHERE R.SYSTEM_ID = idReason;

EXCEPTION
WHEN OTHERS THEN
result:='';
end;
RETURN result;
END GetTransReasonDesc;
/



/*
AUTORE:						FREZZA
Data creazione:				29/10/2010
Scopo della funzione:		UTILIZZATA PER LA VERIFICA CHE UN UTENTE / RUOLO DISPONE DELLA VISIBILITA' 
							SULLE VERSIONI PRECEDENTI DI UN DOCUMENTO CONSOLIDATO

Principali funzionalità che attivano la chiamata alla funzione:
							CREAZIONE TRASMISSIONE, VERIFICA SE L'UTENTE / RUOLO 
							DISPONE DELLA VISIBILITA' COMPLETA SULLE VERSIONI DI UN DOCUMENTO CONSOLIDATO

Indicazione della MEV o dello sviluppo a cui è collegata la funzione:
							CREATA PER LA MEV "VISIBILITA' VERSIONI"
*/
CREATE OR REPLACE FUNCTION hasVersionsFullVisibility
(
idProfile NUMBER,
idPeople NUMBER,
idGroup NUMBER
)
RETURN INT IS retValue INT;

idDocument NUMBER(10) := 0;
idDocumentoPrincipale NUMBER(10) := 0;
hasSecurity INT := 0;

BEGIN

idDocument := idProfile;
retValue := 0;

-- 1a) Determina se il documento è un allegato
select p.id_documento_principale into idDocumentoPrincipale
from profile p
where p.system_id = idDocument;

if (not idDocumentoPrincipale is null and idDocumentoPrincipale > 0) then
    -- L'allegato non ha security, pertanto viene impostato l'id documento principale nell'id profile 
    idDocument := idDocumentoPrincipale;
end if;

-- 2) Verifica se l'utente dispone della visibilità sul documento
select count(*) into hasSecurity
from security s 
where s.thing = idDocument
        and s.personorgroup in (idPeople, idGroup);

if (hasSecurity > 0) then
            
    -- 4) verifica in sercurity se sul doc dispongo di diritti  
    -- superiori a quelli inviati per tramsmissione
                
    select count(*) into retValue
    from security s 
    where s.thing = idDocument
            and s.personorgroup in (idPeople, idGroup)
            and (s.hide_doc_versions is null or s.hide_doc_versions = '0');
else
    -- 2a) L'utente non dispone di alcun diritto di visibilità sul documento, versione non visibile a prescindere 
    retValue := 0;
end if;   

RETURN retValue;
EXCEPTION
WHEN OTHERS
THEN
return -1;

END hasVersionsFullVisibility;
/



CREATE OR REPLACE FUNCTION @db_user.getValCampoProfDoc(DocNumber INT, CustomObjectId INT)
RETURN VARCHAR IS result VARCHAR(255);

tipoOggetto varchar(255);

BEGIN

select b.descrizione
into tipoOggetto
from 
dpa_oggetti_custom a, dpa_tipo_oggetto b 
where 
a.system_id = CustomObjectId
and
a.id_tipo_oggetto = b.system_id;

--Casella di selezione (Per la casella di selezione serve un caso particolare perchè i valori sono multipli)
if(tipoOggetto = 'CasellaDiSelezione') then
    BEGIN
        declare item varchar(255);
        CURSOR curCasellaDiSelezione IS select valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber; 
        BEGIN
            OPEN curCasellaDiSelezione;
            LOOP
            FETCH curCasellaDiSelezione INTO item;
            EXIT WHEN curCasellaDiSelezione%NOTFOUND;
                IF(result IS NOT NULL) THEN
                result := result||'; '||item ;
                ELSE
                result := result||item;
                END IF;        
            END LOOP;
            CLOSE curCasellaDiSelezione;
        END;    
    END;
else
--Tutti gli altri
    select valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber; 
end if;

RETURN result;

END getValCampoProfDoc;
/





CREATE OR REPLACE FUNCTION @db_user.GetValProfObjPrj (
   PrjId            INT,
   CustomObjectId   INT
)
   RETURN VARCHAR
IS
   RESULT       VARCHAR (255);
   
-- Tipo di oggetto su cui compiere la selezione
   objectType   VARCHAR (255);

BEGIN
-- Selezione del tipo di oggetto
   SELECT b.DESCRIZIONE
     INTO objectType
     FROM dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b
    WHERE a.SYSTEM_ID = customobjectid AND a.id_tipo_oggetto = b.system_id;

--Casella di selezione (Per la casella di selezione serve un caso particolare 
-- perchè i valori sono multipli)
   IF (objectType = 'CasellaDiSelezione')
   THEN
      BEGIN
         DECLARE
            item   VARCHAR (255);

            -- Concatenazione dei valori definiti per la casella di selezione
            CURSOR curcaselladiselezione
            IS
               SELECT valore_oggetto_db
                 INTO RESULT
                 FROM dpa_ass_templates_fasc
                WHERE id_oggetto = CustomObjectId AND ID_PROJECT = PrjId;
         BEGIN
            OPEN curcaselladiselezione;

            LOOP
               FETCH curcaselladiselezione
                INTO item;

               EXIT WHEN curcaselladiselezione%NOTFOUND;

               IF (RESULT IS NOT NULL)
               THEN
                  RESULT := RESULT || '; ' || item;
               ELSE
                  RESULT := RESULT || item;
               END IF;
            END LOOP;

            CLOSE curcaselladiselezione;
         END;
      END;
   ELSE
--Tutti gli altri
      SELECT valore_oggetto_db
        INTO RESULT
        FROM dpa_ass_templates_fasc
       WHERE id_oggetto = customobjectid AND id_project = PrjId;
   END IF;

   RETURN RESULT;
END GetValProfObjPrj;
/


CREATE OR REPLACE FUNCTION GetCountNote(tipoOggetto CHAR, idOggetto NUMBER, note NVARCHAR2, idUtente NUMBER, idGruppo NUMBER, tipoRic char, idRegistro NUMBER)
RETURN NUMBER IS retValue NUMBER;

BEGIN

IF tipoRic = 'Q' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
(N.TIPOVISIBILITA = 'T' OR
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente) OR
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo)OR 
(n.tipovisibilita = 'F' AND n.idrfassociato in 
(select id_registro from dpa_l_ruolo_Reg r,dpa_el_registri lr where lr.CHA_RF='1' and r.ID_REGISTRO=lr.SYSTEM_ID
and r.ID_RUOLO_IN_UO in (select system_id from dpa_corr_globali where id_gruppo=idGruppo ))
)

);

ELSIF tipoRic = 'T' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
N.TIPOVISIBILITA = 'T';

ELSIF tipoRic = 'P' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente);

ELSIF tipoRic = 'R' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo);

ELSIF tipoRic = 'F' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
upper(N.TESTO) LIKE upper('%'||note||'%') AND
(N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO = idRegistro);


END IF;

RETURN retValue;
END GetCountNote;
/

-- script Frezza 23-10-2010

CREATE OR REPLACE FUNCTION isVersionVisible
(
versionId NUMBER,
idPeople NUMBER,
idGroup NUMBER
)
RETURN INT IS retValue INT;

idProfile NUMBER(10) := 0;
maxVersionId NUMBER(10) := 0;
hideVersions NUMBER(10) := 0;
ownership NUMBER(10) := 0;

BEGIN
retValue := 0;

-- 1) Reperimento IdProfile e DocNumber del documento
select  p.system_id into idProfile
from    versions v 
        inner join profile p on v.docnumber = p.docnumber
where   v.version_id = versionId;

-- 2) verifica se la versione richiesta è l'ultima
select max(v.version_id) into maxVersionId
from versions v
where v.docnumber = (select docnumber from profile where system_id = idProfile);

if (maxVersionId = versionId) then
    -- 2a.) Il versionId si riferisce all'ultima versione del documento, è sempre visibile
    retValue := 1;
else

    -- 3) verifica se il documento è stato trasmesso a me o al mio ruolo e 
        -- se tale trasmissione prevede le versioni precedenti nascoste 
    select count(*) into hideVersions
    from dpa_trasmissione t
          inner join dpa_trasm_singola ts on t.system_id = ts.id_trasmissione
        where t.id_profile = idProfile
        and ts.id_corr_globale in 
            (
                (select system_id from dpa_corr_globali where id_people = idPeople), 
                (select system_id from dpa_corr_globali where id_gruppo = idGroup)
            )
        and ts.hide_doc_versions = '1';

    if (hideVersions > 0) then
        -- 4) verifica in sercurity se sul doc non dispongo dei diritti di ownership 
        -- (trasmissione a me stesso) oppure abbia già acquisito i diritti di visibilità
        -- (es. superiore gerarchico)
        select count(*) into ownership
        from security s 
        where thing = idProfile
                and personorgroup in (idPeople, idGroup)
                and (cha_tipo_diritto = 'P' or cha_tipo_diritto = 'A');
        
        
        if (ownership = 0) then
            -- Sul documento non si dispongono i diritti di ownership,
            -- pertanto la versione deve essere nascosta 
            retValue := 0;
        else
            -- Sul documento si dispongono già dei diritti di ownership,
            -- pertanto la versione non deve essere nascosta        
            retValue := 1;
        end if;
        
    else
        -- 3a) la tx non prevede di nascondere le versioni, quindi la versione è sempre visibile
        retValue := 1;    
    end if;
end if;

RETURN retValue;
EXCEPTION
WHEN OTHERS
THEN
return -1;

END isVersionVisible;
/

-- fine script Frezza 23-10-2010

/*
AUTORE:						LORUSSO
Data creazione:				07/12/2010
Scopo dell'inserimento:		INSERIMENTO RECORD IN ANAGRAFICA LOG PER CONSERVAZIONE DOCUMENTO.

Indicazione della MEV o dello sviluppo a cui è collegata la modifica:
							
*/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l where l.var_codice ='DOCUMENTOCONSERVAZIONE';
    if (cnt = 0) then       
	insert into dpa_anagrafica_log
	(
	system_id,
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	seq.nextval,
	'DOCUMENTOCONSERVAZIONE',
	'Invio in conservazione del documento',
	'DOCUMENTO',
	'DOCUMENTOCONSERVAZIONE'
	);
    end if;
  end;
end;
/

/*
AUTORE:						LORUSSO
Data creazione:				07/12/2010
Scopo dell'inserimento:		INSERIMENTO RECORD IN ANAGRAFICA LOG PER CONVERSIONE PDF DOCUMENTO.

Indicazione della MEV o dello sviluppo a cui è collegata la modifica:
							
*/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l where l.var_codice ='DOCUMENTOCONVERSIONEPDF';
    if (cnt = 0) then       
	insert into dpa_anagrafica_log
	(
	system_id,
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	seq.nextval,
	'DOCUMENTOCONVERSIONEPDF',
	'Conversione in pdf del documento',
	'DOCUMENTO',
	'DOCUMENTOCONVERSIONEPDF'
	);
    end if;
  end;
end;
/

/*
AUTORE:						LORUSSO
Data creazione:				07/12/2010
Scopo dell'inserimento:		INSERIMENTO RECORD IN ANAGRAFICA LOG PER TIMESTAMP DOCUMENTO.

Indicazione della MEV o dello sviluppo a cui è collegata la modifica:
							
*/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l where l.var_codice ='DOCUMENTOTIMESTAMP';
    if (cnt = 0) then       
	insert into dpa_anagrafica_log
	(
	system_id,
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	seq.nextval,
	'DOCUMENTOTIMESTAMP',
	'Marca Temporale del documento',
	'DOCUMENTO',
	'DOCUMENTOTIMESTAMP'
	);
    end if;
  end;
end;
/

/*

Convenzione per il nome del file: 0_I_SMISTAMENTO_SMISTADOC_U.SQL

----
Inserire SEMPRE i seguenti COMMENTI nel CORPO della Procedura (non dello script): 
AUTORE:	VELTRI	
Data creazione: 13/01/2011
Scopo della Procedura: modifica della SP già esistente per accettazione nello smistamento

Principali funzionalità che attivano la chiamata alla Procedura:

Indicazione della MEV o dello sviluppo a cui è collegata la Procedura:
*/


/*

Convenzione per il nome del file: 0_I_SMISTAMENTO_SMISTADOC_U.SQL

----
Inserire SEMPRE i seguenti COMMENTI nel CORPO della Procedura (non dello script): 
AUTORE:	VELTRI	
Data creazione: 13/01/2010
Scopo della Procedura: modifica della SP già esistente per accettazione nello smistamento

Principali funzionalità che attivano la chiamata alla Procedura:

Indicazione della MEV o dello sviluppo a cui è collegata la Procedura:
*/


CREATE OR REPLACE PROCEDURE i_smistamento_smistadoc_u (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipotrasmissione               IN       CHAR,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm         NUMBER       := NULL;
   systrasmsing          NUMBER       := NULL;
   existaccessrights     CHAR (1)     := 'Y';
   accessrights          NUMBER       := NULL;
   accessrightsvalue     NUMBER       := NULL;
   idutente              NUMBER;
   recordcorrente        NUMBER;
   idgroups              NUMBER       := NULL;
   idgruppo              NUMBER;
   resultvalue           NUMBER;
   tipotrasmsingola      CHAR (1)     := NULL;
   isaccettata           VARCHAR2 (1) := '0';
   isaccettatadelegato   VARCHAR2 (1) := '0';
   isvista               VARCHAR2 (1) := '0';
   isvistadelegato       VARCHAR2 (1) := '0';
   TipoRag VARCHAR2 (1);
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL
        INTO systrasmsing
        FROM DUAL;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE , notegeneralidocumento
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing,
                   cha_tipo_trasm, dta_scadenza, id_trasm_utente
                  )
           VALUES (systrasmsing, idragionetrasm, identitytrasm, 'R',
                   idcorrglobaledestinatario, noteindividuali,
                   tipotrasmissione, datascadenza, NULL
                  );

      returnvalue := systrasmsing;
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

-- Verifica se non vi sia gi una trasmissione per il documento:
-- - se presente, si distinguono 2 casi:
-- 1) se ACCESSRIGHT < Rights
--    viene fatto un'aggiornamento impostandone il valore a Rights
-- 2) altrimenti non fa nulla
-- - se non presente viene fatta in ogni caso la insert con
--   valore di ACCESSRIGHT = Rights
   BEGIN
      SELECT a.id_gruppo
        INTO idgroups
        FROM dpa_corr_globali a
       WHERE a.system_id = idcorrglobaledestinatario;
   END;

   idgruppo := idgroups;

   BEGIN
      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento AND personorgroup = idgruppo)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;

   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;

      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET accessrights = rights
             WHERE thing = iddocumento
               AND personorgroup = idgruppo
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto
                     )
              VALUES (iddocumento, idgruppo, rights, idgruppomittente,
                      tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1')
   THEN
      BEGIN
-- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
         SELECT cha_accettata
           INTO isaccettata
           FROM dpa_trasm_utente
          WHERE system_id = idtrasmissioneutentemittente;

         SELECT cha_vista
           INTO isvista
           FROM dpa_trasm_utente
          WHERE system_id = idtrasmissioneutentemittente;

          SELECT cha_tipo_ragione into TipoRag from dpa_ragione_trasm rs, dpa_trasm_singola ts,dpa_trasm_utente tsu
           where tsu.system_id=idtrasmissioneutentemittente and ts.system_id=tsu.ID_TRASM_SINGOLA and rs.system_id=ts.ID_RAGIONE ;

         IF (idpeopledelegato > 0)
         THEN
            BEGIN
-- Impostazione dei flag per la gestione del delegato
               isvistadelegato := '1';
               isaccettatadelegato := '1';
            END;
         END IF;

         IF (isaccettata = '1')
         THEN
            BEGIN
-- caso in cui la trasmissione risulta gi? accettata
               IF (isvista = '0')
               THEN
                  BEGIN
-- l'oggetto trasmesso non risulta ancora visto,
-- pertanto vengono impostati i dati di visualizzazione
-- e viene rimossa la trasmissione dalla todolist
                     UPDATE dpa_trasm_utente
                        SET dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               ),
                            cha_vista =
                                       (CASE
                                           WHEN dta_vista IS NULL
                                              THEN 1
                                           ELSE 0
                                        END
                                       ),
                            cha_vista_delegato = isvistadelegato,
                            cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE tu.id_people = idpeoplemittente
                                       AND tx.system_id = ts.id_trasmissione
                                       AND tx.system_id = idtrasmissione
                                       AND ts.system_id = tu.id_trasm_singola
                                       AND ts.cha_tipo_dest = 'U')
                            );
                  END;
               ELSE
                  BEGIN
-- l'oggetto trasmesso risulta visto,
-- pertanto la trasmissione viene solo rimossa dalla todolist
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE tu.id_people = idpeoplemittente
                                       AND tx.system_id = ts.id_trasmissione
                                       AND tx.system_id = idtrasmissione
                                       AND ts.system_id = tu.id_trasm_singola
                                       AND ts.cha_tipo_dest = 'U')
                            );
                  END;
               END IF;
            END;
         ELSE

         begin
-- la trasmissione ancora non risulta accettata, pertanto:
-- 1) viene accettata implicitamente,
-- 2) l'oggetto trasmesso impostato come visto,
-- 3) la trasmissione rimossa la trasmissione da todolist

if(TipoRAG='W') then
            BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL
                                THEN SYSDATE
                             ELSE dta_vista
                          END
                         ),
                      cha_vista = (CASE
                                      WHEN dta_vista IS NULL
                                         THEN 1
                                      ELSE 0
                                   END),
                      cha_vista_delegato = isvistadelegato,
                      dta_accettata = SYSDATE,
                      cha_accettata = '1',
                      cha_accettata_delegato = isaccettatadelegato,
                      var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0'
                WHERE (   system_id = idtrasmissioneutentemittente
                       OR system_id =
                             (SELECT tu.system_id
                                FROM dpa_trasm_utente tu,
                                     dpa_trasmissione tx,
                                     dpa_trasm_singola ts
                               WHERE tu.id_people = idpeoplemittente
                                 AND tx.system_id = ts.id_trasmissione
                                 AND tx.system_id = idtrasmissione
                                 AND ts.system_id = tu.id_trasm_singola
                                 AND ts.cha_tipo_dest = 'U')
                      )
                  AND cha_valida = '1';
            END;
         else --no workflow
             BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL
                                THEN SYSDATE
                             ELSE dta_vista
                          END
                         ),
                      cha_vista = (CASE
                                      WHEN dta_vista IS NULL
                                         THEN 1
                                      ELSE 0
                                   END),
                      cha_vista_delegato = isvistadelegato,
                     -- dta_accettata = SYSDATE (),
                    --  cha_accettata = '1',
                     -- cha_accettata_delegato = isaccettatadelegato,
                     -- var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0'
                WHERE (   system_id = idtrasmissioneutentemittente
                       OR system_id =
                             (SELECT tu.system_id
                                FROM dpa_trasm_utente tu,
                                     dpa_trasmissione tx,
                                     dpa_trasm_singola ts
                               WHERE tu.id_people = idpeoplemittente
                                 AND tx.system_id = ts.id_trasmissione
                                 AND tx.system_id = idtrasmissione
                                 AND ts.system_id = tu.id_trasm_singola
                                 AND ts.cha_tipo_dest = 'U'  AND cha_valida = '1')
                      )
                 ;
            END;



         end if;
         end;

         END IF;



--update security se diritti  trasmssione in accettazione =20
         UPDATE security s
            SET s.accessrights = originalrights,
                s.cha_tipo_diritto = 'T'
          WHERE s.thing = iddocumento
            AND s.personorgroup IN (idpeoplemittente, idgruppomittente)
            AND s.accessrights = 20;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );

         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;

   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;
END;
/

CREATE OR REPLACE PROCEDURE create_new_nodo_titolario (
   p_idamm                           NUMBER,
   p_livellonodo                     NUMBER,
   p_description                     VARCHAR2,
   p_codicenodo                      VARCHAR2,
   p_idregistronodo                  NUMBER,
   p_idparent                        NUMBER,
   p_varcodliv1                      VARCHAR2,
   p_mesiconservazione               NUMBER,
   p_charw                           CHAR,
   p_idtipofascicolo                 NUMBER,
   p_bloccafascicolo                 VARCHAR2,
   p_sysidtitolario                  NUMBER,
   p_notenodo                        VARCHAR,
   p_bloccafigli                     VARCHAR2,
   p_contatoreattivo                 VARCHAR2,
   p_numprottit                      NUMBER,
   p_consenticlassificazione         VARCHAR,
   p_bloccaclass                     VARCHAR,
   p_idtitolario               OUT   NUMBER
)
IS
BEGIN
   DBMS_OUTPUT.put_line ('inizio stored');

   DECLARE
      CURSOR currreg
      IS
         SELECT system_id
           FROM dpa_el_registri
          WHERE id_amm = p_idamm AND cha_rf = '0';

      secproj         NUMBER;
      secfasc         NUMBER;
      secroot         NUMBER;
      varchiavetit    VARCHAR2 (64);
      varchiavefasc   VARCHAR2 (64);
      varchiaveroot   VARCHAR2 (64);
      consenticlass   VARCHAR2 (1 BYTE);
   BEGIN
      p_idtitolario := 0;

      SELECT seq.NEXTVAL
        INTO secproj
        FROM DUAL;

      p_idtitolario := secproj;
      DBMS_OUTPUT.put_line ('controllo id registro');

      IF (p_idregistronodo IS NULL OR p_idregistronodo = '')
      THEN
         varchiavetit :=
                  p_idamm || '_' || p_codicenodo || '_' || p_idparent || '_0';
      ELSE
         varchiavetit :=
                 p_codicenodo || '_' || p_idparent || '_' || p_idregistronodo;
      END IF;

      DBMS_OUTPUT.put_line ('controllo blocca class');

      IF (p_bloccaclass = '1')
      THEN
         UPDATE project
            SET cha_consenti_class = '0',
                cha_rw = 'R'
          WHERE system_id = p_idparent;
      END IF;

      BEGIN
         DBMS_OUTPUT.put_line ('inizio primo inserimento');

         IF (p_consenticlassificazione IS NULL)
         THEN
            consenticlass := '0';
         ELSE
            consenticlass := p_consenticlassificazione;
         END IF;

         INSERT INTO project
                     (system_id, description, iconized, cha_tipo_proj,
                      var_codice, id_amm, id_registro, num_livello,
                      cha_tipo_fascicolo, id_parent, var_cod_liv1,
                      dta_apertura, cha_stato, id_fascicolo, cha_rw,
                      num_mesi_conservazione, var_chiave_fasc, id_tipo_fasc,
                      cha_blocca_fasc, id_titolario, dta_creazione,
                      var_note, cha_blocca_figli, cha_conta_prot_tit,
                      num_prot_tit, cha_consenti_class
                     )
              VALUES (secproj, p_description, 'Y', 'T',
                      p_codicenodo, p_idamm, p_idregistronodo, p_livellonodo,
                      NULL, p_idparent, p_varcodliv1,
                      SYSDATE, NULL, NULL, p_charw,
                      p_mesiconservazione, varchiavetit, p_idtipofascicolo,
                      p_bloccafascicolo, p_sysidtitolario, SYSDATE,
                      p_notenodo, p_bloccafigli, p_contatoreattivo,
                      p_numprottit, consenticlass
                     );

         DBMS_OUTPUT.put_line ('fine primo inserimento');
      EXCEPTION
         WHEN OTHERS
         THEN raise;
           -- p_idtitolario := 0;
           -- RETURN;
      END;

      BEGIN
         SELECT seq.NEXTVAL
           INTO secfasc
           FROM DUAL;

         IF (p_idregistronodo IS NULL OR p_idregistronodo = '')
         THEN
            varchiavefasc := p_codicenodo || '_' || p_idtitolario || '_0';
         ELSE
            varchiavefasc :=
               p_codicenodo || '_' || p_idtitolario || '_'
               || p_idregistronodo;
         END IF;

         DBMS_OUTPUT.put_line ('inizio secondo inserimento');

         INSERT INTO project
                     (system_id, description, iconized, cha_tipo_proj,
                      var_codice, id_amm, id_registro, num_livello,
                      cha_tipo_fascicolo, id_parent, var_cod_liv1,
                      dta_apertura, cha_stato, id_fascicolo, cha_rw,
                      num_mesi_conservazione, var_chiave_fasc, id_tipo_fasc,
                      cha_blocca_fasc, id_titolario, dta_creazione,
                      var_note, cha_blocca_figli, cha_conta_prot_tit,
                      num_prot_tit, cha_consenti_class
                     )
              VALUES (secfasc, p_description, 'Y', 'F',
                      p_codicenodo, p_idamm, p_idregistronodo, NULL,
                      'G', p_idtitolario, NULL,
                      SYSDATE, 'A', NULL, p_charw,
                      p_mesiconservazione, varchiavefasc, p_idtipofascicolo,
                      p_bloccafascicolo, p_sysidtitolario, SYSDATE,
                      p_notenodo, p_bloccafigli, p_contatoreattivo,
                      p_numprottit, consenticlass
                     );

         DBMS_OUTPUT.put_line ('fine secondo inserimento');
      EXCEPTION
         WHEN OTHERS
         THEN
            p_idtitolario := 0;
            RETURN;
      END;

      BEGIN
         IF (p_idregistronodo IS NULL OR p_idregistronodo = '')
         THEN
            varchiaveroot := p_codicenodo || '_' || secfasc || '_0';
         ELSE
            varchiaveroot :=
                    p_codicenodo || '_' || secfasc || '_' || p_idregistronodo;
         END IF;

         SELECT seq.NEXTVAL
           INTO secroot
           FROM DUAL;

         DBMS_OUTPUT.put_line ('inizio terzo inserimento');

         INSERT INTO project
                     (system_id, description, iconized, cha_tipo_proj,
                      var_codice, id_amm, id_registro, num_livello,
                      cha_tipo_fascicolo, id_parent, var_cod_liv1,
                      dta_apertura, cha_stato, id_fascicolo, cha_rw,
                      num_mesi_conservazione, var_chiave_fasc, id_tipo_fasc,
                      cha_blocca_fasc, id_titolario, dta_creazione,
                      var_note, cha_blocca_figli, cha_conta_prot_tit,
                      num_prot_tit, cha_consenti_class
                     )
              VALUES (secroot, 'Root Folder', 'Y', 'C',
                      NULL, p_idamm, NULL, NULL,
                      NULL, secfasc, NULL,
                      SYSDATE, NULL, secfasc, p_charw,
                      p_mesiconservazione, varchiaveroot, p_idtipofascicolo,
                      p_bloccafascicolo, p_sysidtitolario, SYSDATE,
                      p_notenodo, p_bloccafigli, p_contatoreattivo,
                      p_numprottit, consenticlass
                     );

         DBMS_OUTPUT.put_line ('fine terzo inserimento');
      EXCEPTION
         WHEN OTHERS
         THEN
            p_idtitolario := 0;
            RETURN;
      END;

-- SE IL NODO HA REGISTRO NULL ALLORA DEVONO ESSERE CREATI TANTI RECORD NELLA
-- DPA_REG_FASC QUANTI SONO I REGISTRI INTERNI ALL'AMMINISTRAZIONE
      IF (p_idregistronodo IS NULL OR p_idregistronodo = '')
      THEN
         FOR currentreg IN currreg
         LOOP
            BEGIN
               DBMS_OUTPUT.put_line ('inizio primo inserimento dpa_reg_fasc');

               INSERT INTO dpa_reg_fasc
                           (system_id, id_titolario, num_rif,
                            id_registro
                           )
                    VALUES (seq.NEXTVAL, p_idtitolario, 1,
                            currentreg.system_id
                           );

               DBMS_OUTPUT.put_line ('fine primo inserimento dpa_reg_fasc');
            EXCEPTION
               WHEN OTHERS
               THEN
                  p_idtitolario := 0;
                  RETURN;
            END;
         END LOOP;

-- inoltre bisogna inserire un record nella dpa_reg_Fasc relativo al registro null
-- per tutte quelle amministrazioni che non hanno abilitata la funzione di fascicolazione
--multi registro
         DBMS_OUTPUT.put_line ('inizio secondo inserimento dpa_reg_fasc');

         INSERT INTO dpa_reg_fasc
                     (system_id, id_titolario, num_rif,
                      id_registro
                     )
              VALUES (seq.NEXTVAL, p_idtitolario, 1,
                      NULL  -- SE IL NODO ¿ COMUNE A TUTTI p_idRegistro = NULL
                     );

         DBMS_OUTPUT.put_line ('fine secondo inserimento dpa_reg_fasc');
      ELSE                 -- il nodo creato ¿  associato  a uno solo registro
         BEGIN
            DBMS_OUTPUT.put_line ('inizio terzo inserimento dpa_reg_fasc');

            INSERT INTO dpa_reg_fasc
                        (system_id, id_titolario, num_rif,
                         id_registro
                        )
                 VALUES (seq.NEXTVAL, p_idtitolario, 1,
                         p_idregistronodo     -- REGISTRO SU CUI  CREO IL NODO
                        );

            DBMS_OUTPUT.put_line ('fine terzo inserimento dpa_reg_fasc');
         EXCEPTION
            WHEN OTHERS
            THEN
               p_idtitolario := 0;
               RETURN;
         END;
      END IF;
   END;
END create_new_nodo_titolario;
/

 
CREATE OR REPLACE FUNCTION docclasstablefunction (
id_amm       NUMBER,
id_registr   NUMBER,
id_anno      NUMBER,
sede         VARCHAR,
titolario    NUMBER
)
RETURN docclasstablerow PIPELINED
IS
outrec           docclasstabletype
:= docclasstabletype (NULL, NULL, NULL, NULL, NULL);
totdocclass      FLOAT;
codclass         VARCHAR (255);
descclass        VARCHAR (255);
totdocclassvt    NUMBER;
percdocclassvt   FLOAT;
contatore        FLOAT;
tmpcontatore     FLOAT;
v_var_sede       VARCHAR (100);
system_id_vt     NUMBER;
description_vt   VARCHAR (255);
var_codice_vt    VARCHAR (255);
system_id_fasc   NUMBER;
system_id_fold   NUMBER;

CURSOR c_vocitit (amm NUMBER, id_reg NUMBER)
IS
SELECT   system_id, description, var_codice
FROM project
WHERE var_codice IS NOT NULL
--AND id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T' and id_amm=amm)
AND id_titolario = titolario
AND id_amm = amm
AND cha_tipo_proj = 'T'
AND (id_registro = id_reg OR id_registro IS NULL)
ORDER BY var_cod_liv1;


CURSOR c_fascicoli (amm NUMBER, reg NUMBER, parentid NUMBER)
IS
SELECT system_id
FROM project
WHERE cha_tipo_proj = 'F'
AND id_amm = amm
AND (id_registro = reg OR id_registro IS NULL)
AND id_parent = parentid;

CURSOR c_folder (amm NUMBER, reg NUMBER, parentid NUMBER)
IS
SELECT system_id
FROM project
WHERE cha_tipo_proj = 'C'
AND id_amm = amm
AND id_parent = parentid
AND (id_registro = reg OR id_registro IS NULL);
BEGIN

percdocclassvt := 0;
totdocclass := 0;
contatore := 0;

IF (sede = '')
THEN
v_var_sede := NULL;
ELSE
v_var_sede := sede;
END IF;

IF ((v_var_sede <> ' ') AND (v_var_sede IS NOT NULL))
THEN
SELECT COUNT (PROFILE.system_id)
INTO totdocclass
FROM PROFILE
WHERE cha_fascicolato = '1'
AND ((id_registro = id_registr) OR (id_registro IS NULL))
AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = id_anno
AND (   (v_var_sede IS NULL AND PROFILE.var_sede IS NULL)
OR (var_sede IS NOT NULL AND PROFILE.var_sede = v_var_sede)
);
ELSE
SELECT COUNT (PROFILE.system_id)
INTO totdocclass
FROM PROFILE
WHERE cha_fascicolato = '1'
AND (id_registro = id_registr OR id_registro IS NULL)
AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = id_anno;
END IF;

OPEN c_vocitit (id_amm, id_registr);

LOOP
FETCH c_vocitit
INTO system_id_vt, description_vt, var_codice_vt;

EXIT WHEN c_vocitit%NOTFOUND;

OPEN c_fascicoli (id_amm, id_registr, system_id_vt);

LOOP
FETCH c_fascicoli
INTO system_id_fasc;

EXIT WHEN c_fascicoli%NOTFOUND;

OPEN c_folder (id_amm, id_registr, system_id_fasc);

LOOP
FETCH c_folder
INTO system_id_fold;

EXIT WHEN c_folder%NOTFOUND;
tmpcontatore := contatore;

IF ((v_var_sede <> ' ') AND (v_var_sede IS NOT NULL))
THEN
SELECT COUNT (PROFILE.system_id)
INTO contatore
FROM project_components, PROFILE
WHERE project_components.project_id = system_id_fold
AND project_components.LINK = PROFILE.system_id
AND (TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) =
id_anno
)
AND (   (PROFILE.id_registro = id_registr)
OR (PROFILE.id_registro IS NULL)
)
AND (   (v_var_sede IS NULL AND PROFILE.var_sede IS NULL)
OR (    var_sede IS NOT NULL
AND PROFILE.var_sede = v_var_sede
)
);
ELSE
SELECT COUNT (PROFILE.system_id)
INTO contatore
FROM project_components, PROFILE
WHERE project_components.project_id = system_id_fold
AND project_components.LINK = PROFILE.system_id
AND (TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) =
id_anno
)
AND (   (PROFILE.id_registro = id_registr)
OR (PROFILE.id_registro IS NULL)
);
END IF;

contatore := contatore + tmpcontatore;
END LOOP;

CLOSE c_folder;
END LOOP;

CLOSE c_fascicoli;

IF ((contatore <> 0) AND (totdocclass <> 0))
THEN
percdocclassvt := ROUND (((contatore / totdocclass) * 100), 2);
outrec.tot_doc_class := totdocclass;
outrec.cod_class := var_codice_vt;
outrec.desc_class := description_vt;
outrec.tot_doc_class_vt := contatore;
outrec.perc_doc_class_vt := percdocclassvt;
PIPE ROW (outrec);
END IF;

contatore := 0;
percdocclassvt := 0;
END LOOP;

CLOSE c_vocitit;

RETURN;
EXCEPTION
WHEN OTHERS
THEN
RETURN;
END docclasstablefunction;
/



CREATE OR REPLACE FUNCTION docclasscomptablefunction (
id_amm       NUMBER,
id_registr   NUMBER,
id_anno      NUMBER,
sede         VARCHAR,
titolario    NUMBER
)
RETURN docclasscomptablerow PIPELINED
IS
outrec                  docclasscomptabletype
:= docclasscomptabletype (NULL, NULL, NULL, NULL, NULL, NULL);


totdocclass             FLOAT;
codclass                VARCHAR (255);
descclass               VARCHAR (255);
totdocclassvt           NUMBER;
percdocclassvt          FLOAT;
contatore               FLOAT;
tmpcontatore            FLOAT;
v_var_sede              VARCHAR (100);
num_livello1            VARCHAR (255);
tot_primo_livello       NUMBER;
var_codice_livello1     VARCHAR (255);
description__livello1   VARCHAR (255);
system_id_vt            NUMBER;
description_vt          VARCHAR (255);
var_codice_vt           VARCHAR (255);
system_id_fasc          NUMBER;
system_id_fold          NUMBER;

CURSOR c_vocitit (amm NUMBER, id_reg NUMBER)
IS
SELECT   system_id, description, var_codice, num_livello
FROM project
WHERE var_codice IS NOT NULL
--AND id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T' and id_amm=amm
AND id_titolario = titolario

--)
AND id_amm = amm
AND cha_tipo_proj = 'T'
AND (id_registro = id_reg OR id_registro IS NULL)
ORDER BY var_cod_liv1;

CURSOR c_fascicoli (amm NUMBER, reg NUMBER, parentid NUMBER)
IS
SELECT system_id
FROM project
WHERE cha_tipo_proj = 'F'
AND id_amm = amm
AND (id_registro = reg OR id_registro IS NULL)
AND id_parent = parentid;

CURSOR c_folder (amm NUMBER, reg NUMBER, parentid NUMBER)
IS
SELECT system_id
FROM project
WHERE cha_tipo_proj = 'C'
AND id_amm = amm
AND id_parent = parentid
AND (id_registro = reg OR id_registro IS NULL);
BEGIN

percdocclassvt := 0;
totdocclass := 0;
contatore := 0;
tot_primo_livello := 0;


IF (sede = ' ')
THEN
v_var_sede := NULL;
ELSE
v_var_sede := sede;
END IF;

IF ((v_var_sede <> '') AND (v_var_sede IS NOT NULL))
THEN
SELECT COUNT (DISTINCT (PROFILE.system_id))
INTO totdocclass
FROM PROFILE
WHERE cha_fascicolato = '1'
AND ((id_registro = id_registr) OR (id_registro IS NULL))
AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = id_anno
AND (   (v_var_sede IS NULL AND PROFILE.var_sede IS NULL)
OR (var_sede IS NOT NULL AND PROFILE.var_sede = v_var_sede)
);
ELSE
SELECT COUNT (DISTINCT (PROFILE.system_id))
INTO totdocclass
FROM PROFILE
WHERE cha_fascicolato = '1'
AND (id_registro = id_registr OR id_registro IS NULL)
AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = id_anno;
END IF;

OPEN c_vocitit (id_amm, id_registr);

LOOP
FETCH c_vocitit
INTO system_id_vt, description_vt, var_codice_vt, num_livello1;

EXIT WHEN c_vocitit%NOTFOUND;

IF (num_livello1 = 1)
THEN
var_codice_livello1 := var_codice_vt;
description__livello1 := description_vt;
END IF;


OPEN c_fascicoli (id_amm, id_registr, system_id_vt);

LOOP
FETCH c_fascicoli
INTO system_id_fasc;

EXIT WHEN c_fascicoli%NOTFOUND;


OPEN c_folder (id_amm, id_registr, system_id_fasc);

LOOP
FETCH c_folder
INTO system_id_fold;

EXIT WHEN c_folder%NOTFOUND;

IF ((v_var_sede <> '') AND (v_var_sede IS NOT NULL))
THEN
SELECT COUNT (DISTINCT (PROFILE.system_id))
INTO tmpcontatore
FROM project_components, PROFILE
WHERE project_components.project_id = system_id_fold
AND project_components.LINK = PROFILE.system_id
AND (TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) =
id_anno
)
AND (   (PROFILE.id_registro = id_registr)
OR (PROFILE.id_registro IS NULL)
)
AND (   (v_var_sede IS NULL AND PROFILE.var_sede IS NULL)
OR (    var_sede IS NOT NULL
AND PROFILE.var_sede = v_var_sede
)
);
ELSE
SELECT COUNT (DISTINCT (PROFILE.system_id))
INTO tmpcontatore
FROM project_components, PROFILE
WHERE project_components.project_id = system_id_fold
AND project_components.LINK = PROFILE.system_id
AND (TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) =
id_anno
)
AND (   (PROFILE.id_registro = id_registr)
OR (PROFILE.id_registro IS NULL)
);
END IF;

contatore := contatore + tmpcontatore;
END LOOP;


CLOSE c_folder;
END LOOP;


CLOSE c_fascicoli;


tot_primo_livello := tot_primo_livello + contatore;

contatore := 0;

percdocclassvt := 0;


IF (num_livello1 = 1 OR c_vocitit%NOTFOUND)
THEN
IF ((tot_primo_livello <> 0) AND (totdocclass <> 0))
THEN
percdocclassvt :=
ROUND (((tot_primo_livello / totdocclass) * 100), 2);
END IF;

outrec.tot_doc_class := totdocclass;
outrec.cod_class := var_codice_livello1;
outrec.desc_class := description__livello1;
outrec.tot_doc_class_vt := tot_primo_livello;
outrec.perc_doc_class_vt := percdocclassvt;
outrec.num_livello := '1';
PIPE ROW (outrec);
tot_primo_livello := 0;
percdocclassvt := 0;
END IF;
END LOOP;

CLOSE c_vocitit;

RETURN;
EXCEPTION
WHEN OTHERS
THEN
return;
END docclasscomptablefunction;
/

-- a valle di tutti gli inserimenti nella DPA_CHIAVI_CONFIG_TEMPLATE eseguiti prima,
-- si valorizza la DPA_CHIAVI_CONFIGURAZIONE
BEGIN 
	crea_keys_amministra;
commit;
END;
/

CREATE OR REPLACE PROCEDURE spsetdatavistasmistamento (
p_idpeople         IN       NUMBER,
p_idoggetto        IN       NUMBER,
p_idgruppo         IN       NUMBER,
p_tipooggetto      IN       CHAR,
p_idtrasmissione   IN       NUMBER,
p_iddelegato       IN       NUMBER,
p_resultvalue      OUT      NUMBER
)
IS
/*
----------------------------------------------------------------------------------------
dpa_trasm_singola.cha_tipo_trasm = 'S''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

dpa_trasm_singola.cha_tipo_trasm = 'T''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

*/
p_cha_tipo_trasm   CHAR (1) := NULL;
p_chatipodest      NUMBER;
tipodiritti        CHAR (1) := NULL;
diritti            NUMBER;
BEGIN
p_resultvalue := 0;

DECLARE
CURSOR cursortrasmsingoladocumento
IS
SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
WHERE a.system_id = p_idtrasmissione
AND a.dta_invio IS NOT NULL
AND a.system_id = b.id_trasmissione
AND (   b.id_corr_globale = (SELECT system_id
FROM dpa_corr_globali
WHERE id_gruppo = p_idgruppo)
OR b.id_corr_globale = (SELECT system_id
FROM dpa_corr_globali
WHERE id_people = p_idpeople)
)
AND a.id_profile = p_idoggetto
AND b.id_ragione = c.system_id;
BEGIN
IF (p_tipooggetto = 'D')
THEN
FOR currenttrasmsingola IN cursortrasmsingoladocumento
LOOP
BEGIN
IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
OR currenttrasmsingola.cha_tipo_ragione = 'I'
)
THEN
-- SE ? una trasmissione senza workFlow
IF (p_iddelegato = 0)
THEN
BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento setto la data di vista
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
dpa_trasm_utente.cha_in_todolist = '0'
WHERE            --dpa_trasm_utente.dta_vista IS NULL
--AND
id_trasm_singola =
(currenttrasmsingola.system_id
)
AND dpa_trasm_utente.id_people = p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
ELSE
--in caso di delega
BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.cha_vista_delegato = '1',
dpa_trasm_utente.id_people_delegato =
p_iddelegato,
dpa_trasm_utente.dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
dpa_trasm_utente.cha_in_todolist = '0'
WHERE            --dpa_trasm_utente.dta_vista IS NULL
-- AND
id_trasm_singola =
(currenttrasmsingola.system_id
)
AND dpa_trasm_utente.id_people = p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
END IF;

-- Impostazione data vista nella trasmissione in todolist
BEGIN
UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola = currenttrasmsingola.system_id
AND id_people_dest = p_idpeople
AND id_profile = p_idoggetto;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;

BEGIN
IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
AND currenttrasmsingola.cha_tipo_dest = 'R'
)
THEN
IF (p_iddelegato = 0)
THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.cha_in_todolist = '0'
WHERE      --dpa_trasm_utente.dta_vista IS NULL
--AND
id_trasm_singola =
(currenttrasmsingola.system_id
)
AND dpa_trasm_utente.id_people != p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
ELSE
BEGIN
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.cha_vista_delegato = '1',
dpa_trasm_utente.id_people_delegato =
p_iddelegato,
dpa_trasm_utente.cha_in_todolist = '0'
WHERE
--dpa_trasm_utente.dta_vista IS NULL
--AND
id_trasm_singola =
(currenttrasmsingola.system_id
)
AND dpa_trasm_utente.id_people != p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
END IF;
END IF;
END;
ELSE
BEGIN
-- la ragione di trasmissione prevede workflow
IF (p_iddelegato = 0)
THEN
BEGIN
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
dta_accettata =
(CASE
WHEN dta_accettata IS NULL
THEN SYSDATE
ELSE dta_accettata
END
),
cha_accettata = '1',
cha_valida = '0'                         --,
--cha_in_todolist = '0'
WHERE          --dpa_trasm_utente.dta_vista IS NULL
--AND
id_trasm_singola =
currenttrasmsingola.system_id
AND dpa_trasm_utente.id_people = p_idpeople;

UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola =
currenttrasmsingola.system_id
AND id_people_dest = p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
ELSE
BEGIN
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_vista = '1',
dpa_trasm_utente.cha_vista_delegato = '1',
dpa_trasm_utente.id_people_delegato =
p_iddelegato,
dpa_trasm_utente.dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
dta_accettata =
(CASE
WHEN dta_accettata IS NULL
THEN SYSDATE
ELSE dta_accettata
END
),
cha_accettata = '1',
cha_accettata_delegato = '1',
cha_valida = '0'                         --,
--cha_in_todolist = '0'
WHERE          --dpa_trasm_utente.dta_vista IS NULL
--   AND
id_trasm_singola =
currenttrasmsingola.system_id
AND dpa_trasm_utente.id_people = p_idpeople;

UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola =
currenttrasmsingola.system_id
AND id_people_dest = p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
END IF;

BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
UPDATE dpa_trasm_utente
SET cha_in_todolist = '0'
WHERE id_trasm_singola =
currenttrasmsingola.system_id
AND NOT dpa_trasm_utente.dta_vista IS NULL
AND (cha_accettata = '1' OR cha_rifiutata = '1');

--AND dpa_trasm_utente.id_people = p_idpeople;
UPDATE dpa_todolist
SET dta_vista = SYSDATE
WHERE id_trasm_singola =
currenttrasmsingola.system_id
AND id_people_dest = p_idpeople
AND id_profile = p_idoggetto;
begin
SELECT r.cha_tipo_diritti
INTO tipodiritti
FROM dpa_trasm_singola w,dpa_ragione_trasm r
WHERE w.system_id = currenttrasmsingola.system_id and r.SYSTEM_ID=w.id_ragione;
exception when others then tipodiritti:=0;
end;
IF (tipodiritti IS NOT NULL)
THEN
IF (tipodiritti = 'W')
THEN
diritti := 63;

IF (tipodiritti = 'N')
THEN
diritti := 0;

IF (tipodiritti = 'R')
THEN
diritti := 45;
END IF;
END IF;
END IF;
END IF;

IF (diritti != 0)
THEN
BEGIN
UPDATE security s
SET s.accessrights = diritti,
s.cha_tipo_diritto = 'T'
WHERE s.thing = p_idoggetto
AND s.personorgroup IN (p_idpeople, p_idgruppo)
AND s.accessrights = 20;
EXCEPTION
WHEN DUP_VAL_ON_INDEX THEN
NULL;
END;
END IF;
END;

BEGIN
-- se la trasm ? con WorkFlow ed ? di tipo UNO e il dest ? Ruolo allora levo la validit? della
-- trasmissione a tutti gli altri utenti del ruolo
IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
AND currenttrasmsingola.cha_tipo_dest = 'R'
)
THEN
BEGIN
UPDATE dpa_trasm_utente
SET dpa_trasm_utente.cha_valida = '0',
dpa_trasm_utente.cha_in_todolist = '0'
WHERE
-- DPA_TRASM_UTENTE.DTA_VISTA IS NULL
id_trasm_singola =
currenttrasmsingola.system_id
AND dpa_trasm_utente.id_people != p_idpeople;
EXCEPTION
WHEN OTHERS
THEN
p_resultvalue := 1;
RETURN;
END;
END IF;
END;
END;
END IF;
END;
END LOOP;
END IF;
END;
END spsetdatavistasmistamento;
/



CREATE OR REPLACE PROCEDURE i_smistamento_smistadoc_u (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipotrasmissione               IN       CHAR,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm         NUMBER       := NULL;
   systrasmsing          NUMBER       := NULL;
   existaccessrights     CHAR (1)     := 'Y';
   accessrights          NUMBER       := NULL;
   accessrightsvalue     NUMBER       := NULL;
   idutente              NUMBER;
   recordcorrente        NUMBER;
   idgroups              NUMBER       := NULL;
   idgruppo              NUMBER;
   resultvalue           NUMBER;
   tipotrasmsingola      CHAR (1)     := NULL;
   isaccettata           VARCHAR2 (1) := '0';
   isaccettatadelegato   VARCHAR2 (1) := '0';
   isvista               VARCHAR2 (1) := '0';
   isvistadelegato       VARCHAR2 (1) := '0';
   TipoRag VARCHAR2 (1);
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL
        INTO systrasmsing
        FROM DUAL;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE, notegeneralidocumento
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing,
                   cha_tipo_trasm, dta_scadenza, id_trasm_utente
                  )
           VALUES (systrasmsing, idragionetrasm, identitytrasm, 'R',
                   idcorrglobaledestinatario, noteindividuali,
                   tipotrasmissione, datascadenza, NULL
                  );

      returnvalue := systrasmsing;
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

-- Verifica se non vi sia gi una trasmissione per il documento:
-- - se presente, si distinguono 2 casi:
-- 1) se ACCESSRIGHT < Rights
--    viene fatto un'aggiornamento impostandone il valore a Rights
-- 2) altrimenti non fa nulla
-- - se non presente viene fatta in ogni caso la insert con
--   valore di ACCESSRIGHT = Rights
   BEGIN
      SELECT a.id_gruppo
        INTO idgroups
        FROM dpa_corr_globali a
       WHERE a.system_id = idcorrglobaledestinatario;
   END;

   idgruppo := idgroups;

   BEGIN
      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento AND personorgroup = idgruppo)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;

   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;

      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET accessrights = rights
             WHERE thing = iddocumento
               AND personorgroup = idgruppo
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto
                     )
              VALUES (iddocumento, idgruppo, rights, idgruppomittente,
                      tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1')
   THEN
      BEGIN
-- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
         SELECT cha_accettata
           INTO isaccettata
           FROM dpa_trasm_utente
          WHERE system_id = idtrasmissioneutentemittente;

         SELECT cha_vista
           INTO isvista
           FROM dpa_trasm_utente
          WHERE system_id = idtrasmissioneutentemittente;

          SELECT cha_tipo_ragione into TipoRag from dpa_ragione_trasm rs, dpa_trasm_singola ts,dpa_trasm_utente tsu
           where tsu.system_id=idtrasmissioneutentemittente and ts.system_id=tsu.ID_TRASM_SINGOLA and rs.system_id=ts.ID_RAGIONE ;

         IF (idpeopledelegato > 0)
         THEN
            BEGIN
-- Impostazione dei flag per la gestione del delegato
               isvistadelegato := '1';
               isaccettatadelegato := '1';
            END;
         END IF;

         IF (isaccettata = '1')
         THEN
            BEGIN
-- caso in cui la trasmissione risulta gi? accettata
               IF (isvista = '0')
               THEN
                  BEGIN
-- l'oggetto trasmesso non risulta ancora visto,
-- pertanto vengono impostati i dati di visualizzazione
-- e viene rimossa la trasmissione dalla todolist
                     UPDATE dpa_trasm_utente
                        SET dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               ),
                            cha_vista =
                                       (CASE
                                           WHEN dta_vista IS NULL
                                              THEN 1
                                           ELSE 0
                                        END
                                       ),
                            cha_vista_delegato = isvistadelegato,
                            cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE tu.id_people = idpeoplemittente
                                       AND tx.system_id = ts.id_trasmissione
                                       AND tx.system_id = idtrasmissione
                                       AND ts.system_id = tu.id_trasm_singola
                                       AND ts.cha_tipo_dest = 'U')
                            );
                  END;
               ELSE
                  BEGIN
-- l'oggetto trasmesso risulta visto,
-- pertanto la trasmissione viene solo rimossa dalla todolist
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE tu.id_people = idpeoplemittente
                                       AND tx.system_id = ts.id_trasmissione
                                       AND tx.system_id = idtrasmissione
                                       AND ts.system_id = tu.id_trasm_singola
                                       AND ts.cha_tipo_dest = 'U')
                            );
                  END;
               END IF;
            END;
         ELSE

         begin
-- la trasmissione ancora non risulta accettata, pertanto:
-- 1) viene accettata implicitamente,
-- 2) l'oggetto trasmesso impostato come visto,
-- 3) la trasmissione rimossa la trasmissione da todolist

if(TipoRAG='W') then
            BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL
                                THEN SYSDATE
                             ELSE dta_vista
                          END
                         ),
                      cha_vista = (CASE
                                      WHEN dta_vista IS NULL
                                         THEN 1
                                      ELSE 0
                                   END),
                      cha_vista_delegato = isvistadelegato,
                      dta_accettata = SYSDATE (),
                      cha_accettata = '1',
                      cha_accettata_delegato = isaccettatadelegato,
                      var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0'
                WHERE (   system_id = idtrasmissioneutentemittente
                       OR system_id =
                             (SELECT tu.system_id
                                FROM dpa_trasm_utente tu,
                                     dpa_trasmissione tx,
                                     dpa_trasm_singola ts
                               WHERE tu.id_people = idpeoplemittente
                                 AND tx.system_id = ts.id_trasmissione
                                 AND tx.system_id = idtrasmissione
                                 AND ts.system_id = tu.id_trasm_singola
                                 AND ts.cha_tipo_dest = 'U')
                      )
                  AND cha_valida = '1';
            END;
         else --no workflow
             BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL
                                THEN SYSDATE
                             ELSE dta_vista
                          END
                         ),
                      cha_vista = (CASE
                                      WHEN dta_vista IS NULL
                                         THEN 1
                                      ELSE 0
                                   END),
                      cha_vista_delegato = isvistadelegato,
                     -- dta_accettata = SYSDATE (),
                    --  cha_accettata = '1',
                     -- cha_accettata_delegato = isaccettatadelegato,
                     -- var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0'
                WHERE (   system_id = idtrasmissioneutentemittente
                       OR system_id =
                             (SELECT tu.system_id
                                FROM dpa_trasm_utente tu,
                                     dpa_trasmissione tx,
                                     dpa_trasm_singola ts
                               WHERE tu.id_people = idpeoplemittente
                                 AND tx.system_id = ts.id_trasmissione
                                 AND tx.system_id = idtrasmissione
                                 AND ts.system_id = tu.id_trasm_singola
                                 AND ts.cha_tipo_dest = 'U')
                      )
                  AND cha_valida = '1';
            END;



         end if;
         end;

         END IF;



--update security se diritti  trasmssione in accettazione =20
         UPDATE security s
            SET s.accessrights = originalrights,
                s.cha_tipo_diritto = 'T'
          WHERE s.thing = iddocumento
            AND s.personorgroup IN (idpeoplemittente, idgruppomittente)
            AND s.accessrights = 20;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );

         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;

   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;
END;
/

CREATE OR REPLACE PROCEDURE @db_user.i_smistamento_smistadoc (
idpeoplemittente               IN       NUMBER,
idcorrglobaleruolomittente     IN       NUMBER,
idgruppomittente               IN       NUMBER,
idamministrazionemittente      IN       NUMBER,
idpeopledestinatario           IN       NUMBER,
idcorrglobaledestinatario      IN       NUMBER,
iddocumento                    IN       NUMBER,
idtrasmissione                 IN       NUMBER,
idtrasmissioneutentemittente   IN       NUMBER,
trasmissioneconworkflow        IN       CHAR,
notegeneralidocumento          IN       VARCHAR2,
noteindividuali                IN       VARCHAR2,
datascadenza                   IN       DATE,
tipodiritto                    IN       CHAR,
rights                         IN       NUMBER,
originalrights                 IN       NUMBER,
idragionetrasm                 IN       NUMBER,
idpeopledelegato               IN       NUMBER,
returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-- -5: Errore in SPsetDataVistaSmistamento
-------------------------------------------------------------------------------------------------------
*/
identitytrasm       NUMBER   := NULL;
identitytrasmsing   NUMBER   := NULL;
existaccessrights   CHAR (1) := 'Y';
accessrights        NUMBER   := NULL;
accessrightsvalue   NUMBER   := NULL;
tipotrasmsingola    CHAR (1) := NULL;
isAccettata         VARCHAR2(1)  := '0';
isAccettataDelegato VARCHAR2(1)  := '0';
isVista             VARCHAR2(1)  := '0';
isVistaDelegato     VARCHAR2(1)  := '0';
resultvalue         NUMBER;

BEGIN
BEGIN
SELECT seq.NEXTVAL
INTO identitytrasm
FROM DUAL;
END;

BEGIN
SELECT seq.NEXTVAL
INTO identitytrasmsing
FROM DUAL;
END;

BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
INSERT INTO dpa_trasmissione
(system_id, id_ruolo_in_uo,
id_people, cha_tipo_oggetto, id_profile, id_project,
dta_invio, var_note_generali
)
VALUES (identitytrasm, idcorrglobaleruolomittente,
idpeoplemittente, 'D', iddocumento, NULL,
SYSDATE (), notegeneralidocumento
);
EXCEPTION
WHEN OTHERS
THEN
returnvalue := -2;
RETURN;
END;

BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
INSERT INTO dpa_trasm_singola
(system_id, id_ragione, id_trasmissione, cha_tipo_dest,
id_corr_globale, var_note_sing, cha_tipo_trasm,
dta_scadenza, id_trasm_utente
)
VALUES (identitytrasmsing, idragionetrasm, identitytrasm, 'U',
idcorrglobaledestinatario, noteindividuali, 'S',
datascadenza, NULL
);
EXCEPTION
WHEN OTHERS
THEN
returnvalue := -3;
RETURN;
END;

BEGIN
/* Inserimento in tabella DPA_TRASM_UTENTE */
INSERT INTO dpa_trasm_utente
(system_id, id_trasm_singola, id_people,
dta_vista, dta_accettata, dta_rifiutata, dta_risposta,
cha_vista, cha_accettata, cha_rifiutata, var_note_acc,
var_note_rif, cha_valida, id_trasm_risp_sing
)
VALUES (seq.NEXTVAL, identitytrasmsing, idpeopledestinatario,
NULL, NULL, NULL, NULL,
'0', '0', '0', NULL,
NULL, '1', NULL
);
EXCEPTION
WHEN OTHERS
THEN
returnvalue := -4;
RETURN;
END;

BEGIN
--per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio
UPDATE dpa_trasmissione
SET dta_invio = SYSDATE ()
WHERE system_id = identitytrasm;

SELECT accessrights
INTO accessrights
FROM (SELECT accessrights
FROM security
WHERE thing = iddocumento
AND personorgroup = idpeopledestinatario)
WHERE ROWNUM = 1;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
existaccessrights := 'N';
END;

IF existaccessrights = 'Y'
THEN
accessrightsvalue := accessrights;

IF accessrightsvalue < rights
THEN
BEGIN
/* aggiornamento a Rights */
UPDATE security
SET accessrights = rights
WHERE thing = iddocumento
AND personorgroup = idpeopledestinatario
AND accessrights = accessrightsvalue;
EXCEPTION
WHEN DUP_VAL_ON_INDEX
THEN
NULL;
END;
END IF;
ELSE
BEGIN
/* inserimento a Rights */
INSERT INTO security
(thing, personorgroup, accessrights,
id_gruppo_trasm, cha_tipo_diritto
)
VALUES (iddocumento, idpeopledestinatario, rights,
idgruppomittente, tipodiritto
);
EXCEPTION
WHEN DUP_VAL_ON_INDEX
THEN
NULL;
END;
END IF;

/* Aggiornamento trasmissione del mittente */
IF (trasmissioneconworkflow = '1') THEN
BEGIN
-- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
select  cha_accettata into isAccettata
from    dpa_trasm_utente
where   system_id = idtrasmissioneutentemittente;

select  cha_vista into isVista
from    dpa_trasm_utente
where   system_id = idtrasmissioneutentemittente;

if (idPeopleDelegato > 0) then
begin
-- Impostazione dei flag per la gestione del delegato
isVistaDelegato := '1';
isAccettataDelegato := '1';
end;
end if;

if (isAccettata = '1') then
begin
-- caso in cui la trasmissione risulta gi? accettata
if (isVista = '0') then
begin
-- l'oggetto trasmesso non risulta ancora visto,
-- pertanto vengono impostati i dati di visualizzazione
-- e viene rimossa la trasmissione dalla todolist
UPDATE dpa_trasm_utente
SET dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
cha_vista = (CASE
WHEN dta_vista IS NULL
THEN 1
ELSE 0
END),
cha_vista_delegato = isVistaDelegato,
cha_in_todolist = '0',
cha_valida = '0'
WHERE (   system_id = idtrasmissioneutentemittente
OR system_id =
(SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.cha_tipo_dest = 'U')
);
end;
else
begin
-- l'oggetto trasmesso visto,
-- pertanto la trasmissione viene solo rimossa dalla todolist
UPDATE dpa_trasm_utente
SET cha_in_todolist = '0',
cha_valida = '0'
WHERE (   system_id = idtrasmissioneutentemittente
OR system_id =
(SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.cha_tipo_dest = 'U')
);
end;
end if;
end;
else
-- la trasmissione ancora non risulta accettata, pertanto:
-- 1) viene accettata implicitamente,
-- 2) l'oggetto trasmesso impostato come visto,
-- 3) la trasmissione rimossa la trasmissione da todolist
begin
UPDATE dpa_trasm_utente
SET dta_vista =
(CASE
WHEN dta_vista IS NULL
THEN SYSDATE
ELSE dta_vista
END
),
cha_vista = (CASE
WHEN dta_vista IS NULL
THEN 1
ELSE 0
END),
cha_vista_delegato = isVistaDelegato,
dta_accettata = SYSDATE (),
cha_accettata = '1',
cha_accettata_delegato = isAccettataDelegato,
var_note_acc = 'Documento accettato e smistato',
cha_in_todolist = '0',
cha_valida = '0'
WHERE (   system_id = idtrasmissioneutentemittente
OR system_id =
(SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.cha_tipo_dest = 'U')
)
AND cha_valida = '1';
end;
end if;

--update security se diritti  trasmssione in accettazione =20
UPDATE security s
SET     s.accessrights = originalrights,
s.cha_tipo_diritto = 'T'
WHERE s.thing=iddocumento and s.personorgroup IN (idpeoplemittente, idgruppomittente)
AND s.accessrights = 20;
END;
ELSE
BEGIN
spsetdatavistasmistamento (idpeoplemittente,
iddocumento,
idgruppomittente,
'D',
idtrasmissione,
idpeopledelegato,
resultvalue
);

IF (resultvalue = 1)
THEN
returnvalue := -4;
RETURN;
END IF;
END;
END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
BEGIN
SELECT *
INTO tipotrasmsingola
FROM (SELECT   a.cha_tipo_trasm
FROM dpa_trasm_singola a, dpa_trasm_utente b
WHERE a.system_id = b.id_trasm_singola
AND b.system_id IN (
SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.system_id =
(SELECT id_trasm_singola
FROM dpa_trasm_utente
WHERE system_id =
idtrasmissioneutentemittente))
ORDER BY cha_tipo_dest)
WHERE ROWNUM = 1;
END;

IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
BEGIN
UPDATE dpa_trasm_utente
SET cha_valida = '0',
cha_in_todolist = '0'
WHERE id_trasm_singola IN (
SELECT a.system_id
FROM dpa_trasm_singola a, dpa_trasm_utente b
WHERE a.system_id = b.id_trasm_singola
AND b.system_id IN (
SELECT tu.system_id
FROM dpa_trasm_utente tu,
dpa_trasmissione tx,
dpa_trasm_singola ts
WHERE tu.id_people = idpeoplemittente
AND tx.system_id = ts.id_trasmissione
AND tx.system_id = idtrasmissione
AND ts.system_id = tu.id_trasm_singola
AND ts.system_id =
(SELECT id_trasm_singola
FROM dpa_trasm_utente
WHERE system_id =
idtrasmissioneutentemittente)))
AND system_id NOT IN (idtrasmissioneutentemittente);
END;
END IF;

returnvalue := 0;
END;
/

CREATE OR REPLACE PROCEDURE i_smistamento_smistadoc_u (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipotrasmissione               IN       CHAR,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm         NUMBER       := NULL;
   systrasmsing          NUMBER       := NULL;
   existaccessrights     CHAR (1)     := 'Y';
   accessrights          NUMBER       := NULL;
   accessrightsvalue     NUMBER       := NULL;
   idutente              NUMBER;
   recordcorrente        NUMBER;
   idgroups              NUMBER       := NULL;
   idgruppo              NUMBER;
   resultvalue           NUMBER;
   tipotrasmsingola      CHAR (1)     := NULL;
   isaccettata           VARCHAR2 (1) := '0';
   isaccettatadelegato   VARCHAR2 (1) := '0';
   isvista               VARCHAR2 (1) := '0';
   isvistadelegato       VARCHAR2 (1) := '0';
   TipoRag VARCHAR2 (1);
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL
        INTO systrasmsing
        FROM DUAL;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE , notegeneralidocumento
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing,
                   cha_tipo_trasm, dta_scadenza, id_trasm_utente
                  )
           VALUES (systrasmsing, idragionetrasm, identitytrasm, 'R',
                   idcorrglobaledestinatario, noteindividuali,
                   tipotrasmissione, datascadenza, NULL
                  );

      returnvalue := systrasmsing;
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

-- Verifica se non vi sia gi una trasmissione per il documento:
-- - se presente, si distinguono 2 casi:
-- 1) se ACCESSRIGHT < Rights
--    viene fatto un'aggiornamento impostandone il valore a Rights
-- 2) altrimenti non fa nulla
-- - se non presente viene fatta in ogni caso la insert con
--   valore di ACCESSRIGHT = Rights
   BEGIN
      SELECT a.id_gruppo
        INTO idgroups
        FROM dpa_corr_globali a
       WHERE a.system_id = idcorrglobaledestinatario;
   END;

   idgruppo := idgroups;

   BEGIN
      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento AND personorgroup = idgruppo)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;

   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;

      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET accessrights = rights
             WHERE thing = iddocumento
               AND personorgroup = idgruppo
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto
                     )
              VALUES (iddocumento, idgruppo, rights, idgruppomittente,
                      tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1')
   THEN
      BEGIN
-- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
         SELECT cha_accettata
           INTO isaccettata
           FROM dpa_trasm_utente
          WHERE system_id = idtrasmissioneutentemittente;

         SELECT cha_vista
           INTO isvista
           FROM dpa_trasm_utente
          WHERE system_id = idtrasmissioneutentemittente;

          SELECT cha_tipo_ragione into TipoRag from dpa_ragione_trasm rs, dpa_trasm_singola ts,dpa_trasm_utente tsu
           where tsu.system_id=idtrasmissioneutentemittente and ts.system_id=tsu.ID_TRASM_SINGOLA and rs.system_id=ts.ID_RAGIONE ;

         IF (idpeopledelegato > 0)
         THEN
            BEGIN
-- Impostazione dei flag per la gestione del delegato
               isvistadelegato := '1';
               isaccettatadelegato := '1';
            END;
         END IF;

         IF (isaccettata = '1')
         THEN
            BEGIN
-- caso in cui la trasmissione risulta gi? accettata
               IF (isvista = '0')
               THEN
                  BEGIN
-- l'oggetto trasmesso non risulta ancora visto,
-- pertanto vengono impostati i dati di visualizzazione
-- e viene rimossa la trasmissione dalla todolist
                     UPDATE dpa_trasm_utente
                        SET dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               ),
                            cha_vista =
                                       (CASE
                                           WHEN dta_vista IS NULL
                                              THEN 1
                                           ELSE 0
                                        END
                                       ),
                            cha_vista_delegato = isvistadelegato,
                            cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE tu.id_people = idpeoplemittente
                                       AND tx.system_id = ts.id_trasmissione
                                       AND tx.system_id = idtrasmissione
                                       AND ts.system_id = tu.id_trasm_singola
                                       AND ts.cha_tipo_dest = 'U')
                            );
                  END;
               ELSE
                  BEGIN
-- l'oggetto trasmesso risulta visto,
-- pertanto la trasmissione viene solo rimossa dalla todolist
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE tu.id_people = idpeoplemittente
                                       AND tx.system_id = ts.id_trasmissione
                                       AND tx.system_id = idtrasmissione
                                       AND ts.system_id = tu.id_trasm_singola
                                       AND ts.cha_tipo_dest = 'U')
                            );
                  END;
               END IF;
            END;
         ELSE

         begin
-- la trasmissione ancora non risulta accettata, pertanto:
-- 1) viene accettata implicitamente,
-- 2) l'oggetto trasmesso impostato come visto,
-- 3) la trasmissione rimossa la trasmissione da todolist

if(TipoRAG='W') then
            BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL
                                THEN SYSDATE
                             ELSE dta_vista
                          END
                         ),
                      cha_vista = (CASE
                                      WHEN dta_vista IS NULL
                                         THEN 1
                                      ELSE 0
                                   END),
                      cha_vista_delegato = isvistadelegato,
                      dta_accettata = SYSDATE,
                      cha_accettata = '1',
                      cha_accettata_delegato = isaccettatadelegato,
                      var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0'
                WHERE (   system_id = idtrasmissioneutentemittente
                       OR system_id =
                             (SELECT tu.system_id
                                FROM dpa_trasm_utente tu,
                                     dpa_trasmissione tx,
                                     dpa_trasm_singola ts
                               WHERE tu.id_people = idpeoplemittente
                                 AND tx.system_id = ts.id_trasmissione
                                 AND tx.system_id = idtrasmissione
                                 AND ts.system_id = tu.id_trasm_singola
                                 AND ts.cha_tipo_dest = 'U')
                      )
                  AND cha_valida = '1';
            END;
         else --no workflow
             BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL
                                THEN SYSDATE
                             ELSE dta_vista
                          END
                         ),
                      cha_vista = (CASE
                                      WHEN dta_vista IS NULL
                                         THEN 1
                                      ELSE 0
                                   END),
                      cha_vista_delegato = isvistadelegato,
                     -- dta_accettata = SYSDATE (),
                    --  cha_accettata = '1',
                     -- cha_accettata_delegato = isaccettatadelegato,
                     -- var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0'
                WHERE (   system_id = idtrasmissioneutentemittente
                       OR system_id =
                             (SELECT tu.system_id
                                FROM dpa_trasm_utente tu,
                                     dpa_trasmissione tx,
                                     dpa_trasm_singola ts
                               WHERE tu.id_people = idpeoplemittente
                                 AND tx.system_id = ts.id_trasmissione
                                 AND tx.system_id = idtrasmissione
                                 AND ts.system_id = tu.id_trasm_singola
                                 AND ts.cha_tipo_dest = 'U'  AND cha_valida = '1')
                      )
                 ;
            END;



         end if;
         end;

         END IF;



--update security se diritti  trasmssione in accettazione =20
         UPDATE security s
            SET s.accessrights = originalrights,
                s.cha_tipo_diritto = 'T'
          WHERE s.thing = iddocumento
            AND s.personorgroup IN (idpeoplemittente, idgruppomittente)
            AND s.accessrights = 20;

        EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
      -- visibilità già esistente, ignora e continua con eventuali altri inserimenti 
		      NULL;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );

         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;

   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;
END;
/
  
begin
Insert into @db_user.DPA_DOCSPA
   (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values
   (seq.nextval, sysdate, '3.12');
end;
/