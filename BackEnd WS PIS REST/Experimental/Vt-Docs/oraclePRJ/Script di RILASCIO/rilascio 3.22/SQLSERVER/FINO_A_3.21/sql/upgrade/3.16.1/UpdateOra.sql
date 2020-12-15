-- file sql di update per il CD --
              
---- ALTER_DPA_GRIDS.ORA.sql  marcatore per ricerca ----
-- ALTER TABLE @db_user.DPA_GRIDS MODIFY(GRID_NAME VARCHAR2(100 BYTE));
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_GRIDS';
      nomecolonna   VARCHAR2 (32)  := 'GRID_NAME';
      tipodato      VARCHAR2 (200) := ' VARCHAR2(100) ';
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
		 -- ok la colonna esiste
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' MODIFY ('|| nomecolonna
                              || ' '|| tipodato || ' )';
		ELSE
			EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/



--ALTER TABLE @db_user.DPA_GRIDS RENAME COLUMN USER_ID TO USER_ID_creatore;
-- o
-- ALTER TABLE @db_user.DPA_GRIDS DROP COLUMN USER_ID ;
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_GRIDS';
      nomecolonna   VARCHAR2 (32)  := 'USER_ID';
      newnomecolonna   VARCHAR2 (32)  := 'USER_ID_creatore';
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
		 -- ok la vecchia colonna esiste
         THEN

			SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (newnomecolonna)
			and owner=nomeutente;

			IF (cnt = 0)
		 -- ok la nuova colonna non esiste
			THEN

				EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' RENAME column '|| nomecolonna
                              || ' TO '|| newnomecolonna ;
			END IF; 
			IF (cnt = 1)
		 -- la nuova colonna esiste gi
			THEN

				EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' DROP column '|| nomecolonna   ;
			
			END IF;
		 END IF;
      END IF;
   END;
END;
/

-- ALTER TABLE @db_user.DPA_GRIDS RENAME COLUMN ROLE_ID TO ROLE_ID_creatore;
-- o 
-- ALTER TABLE @db_user.DPA_GRIDS drop COLUMN ROLE_ID ;

BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_GRIDS';
      nomecolonna   VARCHAR2 (32)  := 'ROLE_ID';
      newnomecolonna   VARCHAR2 (32)  := 'ROLE_ID_creatore';
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
		 -- ok la vecchia colonna esiste
         THEN

			SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (newnomecolonna)
			and owner=nomeutente;

			IF (cnt = 0)
		 -- ok la nuova colonna non esiste
			THEN


            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' RENAME column '|| nomecolonna
                              || ' TO '|| newnomecolonna ;
			END IF; 
			IF (cnt = 1)
		 -- la nuova colonna esiste gi
			THEN


            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' DROP column '|| nomecolonna   ;
			
			END IF;
		 END IF;
      END IF;
   END;
END;
/


--ALTER TABLE @db_user.DPA_GRIDS DROP COLUMN IS_TEMPORARY;
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_GRIDS';
      nomecolonna   VARCHAR2 (32)  := 'IS_TEMPORARY';
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
                              || ' DROP column '|| nomecolonna        ;
         END IF;
      END IF;
   END;
END;
/

--ALTER TABLE @db_user.DPA_GRIDS DROP COLUMN IS_ACTIVE;
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_GRIDS';
      nomecolonna   VARCHAR2 (32)  := 'IS_ACTIVE';
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
                              || ' DROP column '|| nomecolonna        ;
         END IF;
      END IF;
   END;
END;
/

--ALTER TABLE @db_user.DPA_GRIDS ADD (IS_SEARCH_GRID CHAR(1 BYTE) DEFAULT 'Y' CHECK (IS_SEARCH_GRID in ('Y','N')));
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_GRIDS';
      nomecolonna   VARCHAR2 (32)  := 'IS_SEARCH_GRID';
      tipodato      VARCHAR2 (200) := ' CHAR(1) DEFAULT ''Y'' CHECK (IS_SEARCH_GRID in (''Y'',''N''))  ';
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

--AGGIUNGE LA COLONNA CHA_VISIBILE_A_UTENTE_O_RUOLO
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_GRIDS';
      nomecolonna   VARCHAR2 (32)  := 'CHA_VISIBILE_A_UTENTE_O_RUOLO';
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
              
------------------
              
---- ALTER_DPA_SALVA_RICERCHE.ORA.sql  marcatore per ricerca ----
-- ALTER TABLE @db_user.DPA_SALVA_RICERCHE 	ADD GRID_ID INT;
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_SALVA_RICERCHE';
      nomecolonna   VARCHAR2 (32)  := 'GRID_ID';
      tipodato      VARCHAR2 (200) := ' INT ';
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

              
------------------
              
---- ALTER_INDX_MODELLI_MITT_DEST_1.ORA.sql  marcatore per ricerca ----
BEGIN
declare 
 cnt INT;
begin 
 SELECT COUNT (*) INTO cnt
 FROM all_indexes
 WHERE index_name = upper('INDX_MODELLI_MITT_DEST_1')
 and owner=upper('@db_user');
 
 IF (cnt = 1) then
 execute immediate 'DROP INDEX INDX_MODELLI_MITT_DEST_1';
 end if;
 
 execute immediate 'CREATE INDEX INDX_MODELLI_MITT_DEST_1 
    ON DPA_MODELLI_MITT_DEST(ID_MODELLO, HIDE_DOC_VERSIONS )';
end;
end; 
/              
------------------

---- ALTER_DPA_COLL_MSPEDIZ_DOCUMENTO.ORA.sql  marcatore per ricerca ----
BEGIN
declare 
 cnt INT;
begin 
 SELECT COUNT (*) INTO cnt
 FROM all_indexes
 WHERE index_name = upper('IDX_COLL_MSPEDIZ_DOC_1')
 and owner=upper('@db_user');
 
 IF (cnt = 0) then
 
 execute immediate 'CREATE INDEX IDX_COLL_MSPEDIZ_DOC_1 
ON DPA_COLL_MSPEDIZ_DOCUMENTO (ID_DOCUMENTTYPES, ID_PROFILE)';
end IF;
end;
end;
/ 
------------------
              
---- ALTER_PROFILE.ORA.sql  marcatore per ricerca ----
-- AGGIUNGE LA COLONNA CHA_COD_T_A NELLA TABELLA PROFILE
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'PROFILE';
      nomecolonna   VARCHAR2 (32)  := 'CHA_COD_T_A';
      tipodato      VARCHAR2 (200) := ' VARCHAR2(1024 BYTE) ';
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
              
------------------
              
---- ALTER_PROJECT.ORA.SQL  marcatore per ricerca ----
-- AGGIUNGE LA COLONNA CHA_COD_T_A NELLA TABELLA PROJECT
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'PROJECT';
      nomecolonna   VARCHAR2 (32)  := 'CHA_COD_T_A';
      tipodato      VARCHAR2 (200) := ' VARCHAR2(1024 BYTE) ';
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
              
------------------
              
---- CREATE_DPA_PROFIL_STO.ORA.sql  marcatore per ricerca ----
/*
AUTORE:                     GABRIELE SERPI
Data creazione:                  25/07/2011
Scopo della modifica:        CREARE LA TABELLA DPA_PROFIL_STO
*/
 
BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'create table @db_user.DPA_PROFIL_STO
             (systemId INT PRIMARY KEY NOT NULL,
                id_template int not null,
                dta_modifica date not null,
                id_profile int not null,
                id_ogg_custom int not null,
                id_people int not null,
                id_ruolo_in_uo int not null,
                var_desc_modifica varchar(2000))';

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

/* add FK id_template */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_template
                                          FOREIGN KEY (id_template) REFERENCES   @db_user.DPA_TIPO_ATTO(SYSTEM_ID)
                                          ENABLE' ;

      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');


   BEGIN
      SELECT COUNT (*) INTO cnt
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_TEMPLATE'
         AND constraint_type = 'R'
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_template)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_template NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.DPA_TIPO_ATTO tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL;
         END IF;
      END IF;
   END;
END;
/

/* add FK id_profile */
BEGIN
   DECLARE

      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_profile
                                          FOREIGN KEY (id_profile) REFERENCES                       @db_user.PROFILE(SYSTEM_ID)
                                          ENABLE' ;

      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');


   BEGIN
      SELECT COUNT (*) INTO cnt
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_PROFILE'
         AND constraint_type = 'R'
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_profile)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_profile NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.PROFILE tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL;
         END IF;
      END IF;
   END;
END;
/

/* add FK id_ogg_custom */
BEGIN
   DECLARE

      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_ogg_custom
                                          FOREIGN KEY (id_ogg_custom) REFERENCES    @db_user.DPA_OGGETTI_CUSTOM(SYSTEM_ID)
                                          ENABLE' ;

      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');


   BEGIN
      SELECT COUNT (*) INTO cnt
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_OGG_CUSTOM'
         AND constraint_type = 'R'
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_ogg_custom)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_ogg_custom NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.DPA_OGGETTI_CUSTOM tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

/* add FK id_people */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_people
                                          FOREIGN KEY (id_people) REFERENCES                       @db_user.PEOPLE(SYSTEM_ID)
                                          ENABLE' ; 
      
      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');
      
      
   BEGIN
      SELECT COUNT (*) INTO cnt 
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_PEOPLE'
         AND constraint_type = 'R' 
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_people)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_people NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.PEOPLE tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

/* add FK id_ruolo_in_uo */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_ruolo_in_uo
                                          FOREIGN KEY (id_ruolo_in_uo) REFERENCES                       @db_user.DPA_CORR_GLOBALI(SYSTEM_ID)
                                          ENABLE' ; 
      
      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');
      
      
   BEGIN
      SELECT COUNT (*) INTO cnt 
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_RUOLO_IN_UO'
         AND constraint_type = 'R' 
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_ruolo_in_uo)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_ruolo_in_uo NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.DPA_CORR_GLOBALI tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

              
------------------
              
---- DPA_ASS_GRIDS.ORA.sql  marcatore per ricerca ----
BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'CREATE TABLE @db_user.DPA_ASS_GRIDS
				( GRID_ID            INTEGER                    NOT NULL,
				  USER_ID            INTEGER,
				  ROLE_ID            INTEGER,
				  ADMINISTRATION_ID  INTEGER,
				  TYPE_GRID          VARCHAR2(30 BYTE)
				)';

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


BEGIN
   DECLARE

	indiceesiste EXCEPTION;
      PRAGMA EXCEPTION_INIT(indiceesiste, -1408);
	     
      cnt       INT;
      cntdati   INT;
	  
	  nomeutente	VARCHAR2 (200) := upper('@db_user');
	  istruzioneSQL VARCHAR2(2000) ; 
    
   BEGIN

	  istruzioneSQL  := 'CREATE UNIQUE INDEX '||nomeutente||'.DPA_ASS_GRIDS_PK 
                    ON DPA_ASS_GRIDS
                    (GRID_ID, USER_ID, ROLE_ID) ' ; 
    
	 SELECT COUNT (*) INTO cnt
        FROM all_indexes
       WHERE index_name = upper('DPA_ASS_GRIDS_PK')
	   and owner=nomeutente;

      IF (cnt = 0)
      THEN
           SELECT COUNT (*) INTO cntdati
        FROM DUAL
       WHERE EXISTS (SELECT   GRID_ID, USER_ID, ROLE_ID, COUNT (*)
                         FROM @db_user.DPA_ASS_GRIDS
                     GROUP BY GRID_ID, USER_ID, ROLE_ID
                       HAVING COUNT (*) > 1);

         IF (cntdati = 0)
         -- ok non esistono record che violano l'indice univoco
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- non posso creare l'indice 
            DBMS_OUTPUT.PUT_LINE ('non posso creare l''indice per chiavi duplicate '); 
         END IF;
      END IF;
   
    EXCEPTION 
WHEN indiceesiste
THEN DBMS_OUTPUT.PUT_LINE('esiste gi un indice per questa lista di colonne'); 

   END;
END;
/

 BEGIN
   DECLARE
     nomeutente		VARCHAR2 (200) := upper('@db_user');

      nometabellaPK VARCHAR2(2000) :='DPA_ASS_GRIDS'   ; 
      nomecolonnaPK VARCHAR2(2000) :='GRID_ID, USER_ID, ROLE_ID' ; 
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
         --and cons_cols.COLUMN_NAME = nomecolonnaPK 
		 ; 
      
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

      nometabellaFK VARCHAR2(2000) :='DPA_ASS_GRIDS' ; 
	  nomecolonnaFK VARCHAR2(2000) :='GRID_ID' ; 

	  nometabellaPK VARCHAR2(2000) :='DPA_GRIDS'   ; 
	  nomecolonnaPK VARCHAR2(2000) :='SYSTEM_ID' ; 

	  -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
	  nomeForeignKey VARCHAR2(2000) :='DPA_ASS_GRIDS_R03' ; 
	 
	 -- fine variabili da modifica
	  
	  istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ' ; 
      
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


BEGIN
   DECLARE
     nomeutente		VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='DPA_ASS_GRIDS' ; 
	  nomecolonnaFK VARCHAR2(2000) :='USER_ID' ; 

	  nometabellaPK VARCHAR2(2000) :='PEOPLE'   ; 
	  nomecolonnaPK VARCHAR2(2000) :='SYSTEM_ID' ; 

	  -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
	  nomeForeignKey VARCHAR2(2000) :='DPA_ASS_GRIDS_R01' ; 
	 
	 -- fine variabili da modifica
	  
	  istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ' ; 
      
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

BEGIN
   DECLARE
     nomeutente		VARCHAR2 (200) := upper('@db_user');

      nometabellaFK VARCHAR2(2000) :='DPA_ASS_GRIDS' ; 
	  nomecolonnaFK VARCHAR2(2000) :='ROLE_ID' ; 

	  nometabellaPK VARCHAR2(2000) :='GROUPS'   ; 
	  nomecolonnaPK VARCHAR2(2000) :='SYSTEM_ID' ; 

	  -- il nome NON  usato per verificare se la FK esiste gi, ma si vede se esiste un vincolo tipo 'R' su quella colonna 
	  nomeForeignKey VARCHAR2(2000) :='DPA_ASS_GRIDS_R02' ; 
	 
	 -- fine variabili da modifica
	  
	  istruzioneSQL VARCHAR2(2000) := ' ALTER TABLE '||nomeutente||'.'||nometabellaFK||' ADD CONSTRAINT
         '||nomeForeignKey||' FOREIGN KEY 
         ('||nomecolonnaFK||') REFERENCES '||nomeutente||'.'||nometabellaPK||' ('||nomecolonnaPK||') ' ; 
      
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

              
------------------
              
---- DPA_PROFIL_FASC_STO.ORA.sql  marcatore per ricerca ----
/*
AUTORE:                     GABRIELE SERPI
Data creazione:                  25/07/2011
Scopo della modifica:        CREARE LA TABELLA DPA_PROFIL_FASC_STO
*/
 
BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'create table @db_user.DPA_PROFIL_FASC_STO
             (systemId INT PRIMARY KEY NOT NULL,
                id_template int not null,
                dta_modifica date not null,
                id_project int not null,
                id_ogg_custom int not null,
                id_people int not null,
                id_ruolo_in_uo int not null,
                var_desc_modifica varchar(2000))';

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

/* add FK id_template */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_FASC_STO ADD
                                          CONSTRAINT FK_DP_PRF_FSC_ST_id_template
                                          FOREIGN KEY (id_template) REFERENCES                       @db_user.DPA_TIPO_FASC(SYSTEM_ID)
                                          ENABLE' ; 
      
      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');
      
      
   BEGIN
      SELECT COUNT (*) INTO cnt 
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_FSC_ST_ID_TEMPLATE'
         AND constraint_type = 'R' 
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_template)        INTO cntdati
           FROM @db_user.DPA_PROFIL_FASC_STO f
          WHERE f.id_template NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.DPA_TIPO_FASC tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

/* add FK id_profile */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_FASC_STO ADD
                                          CONSTRAINT FK_DP_PRF_FSC_ST_id_project
                                          FOREIGN KEY (id_project) REFERENCES                       @db_user.PROJECT(SYSTEM_ID)
                                          ENABLE' ; 
      
      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');
      
      
   BEGIN
      SELECT COUNT (*) INTO cnt 
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_FSC_ST_ID_PROJECT'
         AND constraint_type = 'R' 
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_project)        INTO cntdati
           FROM @db_user.DPA_PROFIL_FASC_STO f
          WHERE f.id_project NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.PROJECT tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

/* add FK id_ogg_custom */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_FASC_STO ADD
                                          CONSTRAINT FK_DP_PRF_FSC_ST_id_ogg_custom
                                          FOREIGN KEY (id_ogg_custom) REFERENCES                       @db_user.DPA_OGGETTI_CUSTOM_FASC(SYSTEM_ID)
                                          ENABLE' ; 
      
      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');
      
      
   BEGIN
      SELECT COUNT (*) INTO cnt 
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_FSC_ST_ID_OGG_CUSTOM'
         AND constraint_type = 'R' 
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_ogg_custom)        INTO cntdati
           FROM @db_user.DPA_PROFIL_FASC_STO f
          WHERE f.id_ogg_custom NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.DPA_OGGETTI_CUSTOM tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

/* add FK id_people */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_FASC_STO ADD
                                          CONSTRAINT FK_DP_PRF_FSC_ST_id_people
                                          FOREIGN KEY (id_people) REFERENCES                       @db_user.PEOPLE(SYSTEM_ID)
                                          ENABLE' ; 
      
      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');
      
      
   BEGIN
      SELECT COUNT (*) INTO cnt 
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_FSC_ST_ID_PEOPLE'
         AND constraint_type = 'R' 
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_people)        INTO cntdati
           FROM @db_user.DPA_PROFIL_FASC_STO f
          WHERE f.id_people NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.PEOPLE tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

/* add FK id_ruolo_in_uo */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_FASC_STO ADD
                                          CONSTRAINT FK_DP_PRF_FSC_ST_id_ru_uo
                                          FOREIGN KEY (id_ruolo_in_uo) REFERENCES                       @db_user.DPA_CORR_GLOBALI(SYSTEM_ID)
                                          ENABLE' ; 
      
      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');
      
      
   BEGIN
      SELECT COUNT (*) INTO cnt 
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_FSC_ST_ID_RU_UO'
         AND constraint_type = 'R' 
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_ruolo_in_uo)        INTO cntdati
           FROM @db_user.DPA_PROFIL_FASC_STO f
          WHERE f.id_ruolo_in_uo NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.DPA_CORR_GLOBALI tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

              
------------------
              
---- DPA_PROFIL_STO.ORA.sql  marcatore per ricerca ----
/*
AUTORE:                     GABRIELE SERPI
Data creazione:                  25/07/2011
Scopo della modifica:        CREARE LA TABELLA DPA_PROFIL_STO
*/
 
BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'create table @db_user.DPA_PROFIL_STO
             (systemId INT PRIMARY KEY NOT NULL,
                id_template int not null,
                dta_modifica date not null,
                id_profile int not null,
                id_ogg_custom int not null,
                id_people int not null,
                id_ruolo_in_uo int not null,
                var_desc_modifica varchar(2000))';

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

/* add FK id_template */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_template
                                          FOREIGN KEY (id_template) REFERENCES   @db_user.DPA_TIPO_ATTO(SYSTEM_ID)
                                          ENABLE' ;

      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');


   BEGIN
      SELECT COUNT (*) INTO cnt
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_TEMPLATE'
         AND constraint_type = 'R'
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_template)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_template NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.DPA_TIPO_ATTO tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL;
         END IF;
      END IF;
   END;
END;
/

/* add FK id_profile */
BEGIN
   DECLARE

      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_profile
                                          FOREIGN KEY (id_profile) REFERENCES                       @db_user.PROFILE(SYSTEM_ID)
                                          ENABLE' ;

      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');


   BEGIN
      SELECT COUNT (*) INTO cnt
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_PROFILE'
         AND constraint_type = 'R'
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_profile)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_profile NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.PROFILE tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL;
         END IF;
      END IF;
   END;
END;
/

/* add FK id_ogg_custom */
BEGIN
   DECLARE

      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_ogg_custom
                                          FOREIGN KEY (id_ogg_custom) REFERENCES    @db_user.DPA_OGGETTI_CUSTOM(SYSTEM_ID)
                                          ENABLE' ;

      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');


   BEGIN
      SELECT COUNT (*) INTO cnt
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_OGG_CUSTOM'
         AND constraint_type = 'R'
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_ogg_custom)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_ogg_custom NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.DPA_OGGETTI_CUSTOM tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

/* add FK id_people */
BEGIN
   DECLARE
     
      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_people
                                          FOREIGN KEY (id_people) REFERENCES                       @db_user.PEOPLE(SYSTEM_ID)
                                          ENABLE' ; 
      
      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');
      
      
   BEGIN
      SELECT COUNT (*) INTO cnt 
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_PEOPLE'
         AND constraint_type = 'R' 
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_people)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_people NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.PEOPLE tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati 
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL; 
         END IF;
      END IF;
   END;
END;
/

/* add FK id_ruolo_in_uo */
BEGIN
   DECLARE

      istruzioneSQL VARCHAR2(2000) := '   ALTER TABLE @db_user.DPA_PROFIL_STO ADD
                                          CONSTRAINT FK_DP_PRF_STO_id_ruolo_in_uo
                                          FOREIGN KEY (id_ruolo_in_uo) REFERENCES                       @db_user.DPA_CORR_GLOBALI(SYSTEM_ID)
                                          ENABLE' ;

      cnt       INT;
      cntdati   INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');


   BEGIN
      SELECT COUNT (*) INTO cnt
      FROM all_constraints
       WHERE constraint_name = 'FK_DP_PRF_STO_ID_RUOLO_IN_UO'
         AND constraint_type = 'R'
         and owner=nomeutente;

      IF (cnt = 0)
      THEN
         SELECT COUNT (f.id_ruolo_in_uo)        INTO cntdati
           FROM @db_user.DPA_PROFIL_STO f
          WHERE f.id_ruolo_in_uo NOT IN (SELECT tf.SYSTEM_ID
                                             FROM @db_user.DPA_CORR_GLOBALI tf);

         IF (cntdati = 0)
         THEN
            EXECUTE IMMEDIATE istruzioneSQL;
         ELSE
         -- la clausolo novalidate la mettiamo solo se la select di prima ritorna dati
         istruzioneSQL := istruzioneSQL||' novalidate';
            EXECUTE IMMEDIATE istruzioneSQL;
         END IF;
      END IF;
   END;
END;
/


------------------

---- SEQ_DPA_PROFIL_FASC_STO.sql  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
BEGIN
    SELECT COUNT(*) INTO cnt FROM user_sequences
							  where sequence_name='SEQ_DPA_PROFIL_FASC_STO';
    IF (cnt = 0) THEN
       execute immediate 'CREATE SEQUENCE @db_user.SEQ_DPA_PROFIL_FASC_STO
			START WITH 1
			MAXVALUE 999999999999999999999999999
			MINVALUE 1
			NOCYCLE
			CACHE 20
			NOORDER';
    END IF;
END;
END;
/

------------------

---- SEQ_DPA_PROFIL_STO.sql  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
BEGIN
    SELECT COUNT(*) INTO cnt FROM user_sequences
							  where sequence_name='SEQ_DPA_PROFIL_STO';
    IF (cnt = 0) THEN
       execute immediate 'CREATE SEQUENCE @db_user.SEQ_DPA_PROFIL_STO
			START WITH 1
			MAXVALUE 999999999999999999999999999
			MINVALUE 1
			NOCYCLE
			CACHE 20
			NOORDER';
    END IF;
END;
END;
/

------------------


---- dpa_anagrafica_funzioni.ORA.sql  marcatore per ricerca ----
--Creazione nuova funzione DPA_ANAGRAFICA_FUNZIONI
begin
declare cnt int;
begin
select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI l where l.COD_FUNZIONE ='ELIMINA_TIPOLOGIA_DOC';

if (cnt = 0) then
insert into @db_user.DPA_ANAGRAFICA_FUNZIONI
(
COD_FUNZIONE,
VAR_DESC_FUNZIONE,
CHA_TIPO_FUNZ,
DISABLED
)
values
(
'ELIMINA_TIPOLOGIA_DOC',
'Consente l''eliminazione di una tipologia di un documento.',
null,
'N'
);
end if;
end;
end;
/              
------------------
              
---- dpa_anagrafica_log.ORA.sql  marcatore per ricerca ----
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l 
	where l.var_codice ='DOCUMENTOSPEDISCI';
    if (cnt = 0) then       
		insert into dpa_anagrafica_log(system_id, var_codice, var_descrizione
		, var_oggetto, var_metodo)
		values (seq.nextval, 'DOCUMENTOSPEDISCI', 'Spedizione documento'
		, 'DOCUMENTO', 'DOCUMENTOSPEDISCI') ; 
    end if;
  end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l 
	where l.var_codice ='RUB_IMP_NEWCORR_WS';
    if (cnt = 0) then       
		insert into dpa_anagrafica_log(system_id, var_codice, var_descrizione
		, var_oggetto, var_metodo)
		values (SEQ_DPA_CHIAVI_CONFIG.nextval, 'RUB_IMP_NEWCORR_WS', 
		'WS rubrica, creazione nuovo corrispondente', 
		'RUBRICA', 'IMPORTARUBRICACREAWS');
    end if;
  end;
end;
/

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
------------------
              
---- dpa_chiavi_config_template.sql  marcatore per ricerca ----

--dopo l'inserimento nella tabella dpa_chiavi_config_template, 
-- occorre eseguire la procedura CREA_KEYS_AMMINISTRA per infasare in DPA_CHIAVI_CONFIGURAZIONE
-- quindi, verificare i record in dpa_chiavi_config_template, prima di eseguire la procedura

-- FE_FASC_RAPIDA_REQUIRED	FALSE
begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_chiavi_config_template  	l
	where l.var_codice ='FE_FASC_RAPIDA_REQUIRED';
    if (cnt = 0) then       
	Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
		(SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
		, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_INFASATO)
	Values
		(SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_FASC_RAPIDA_REQUIRED'
		, 'Obbligatorieta della classificazione o fascicolazione rapida'
		, 'false', 'F', '1', '1', 'N');
    end if;
  end;
end;
/



-- BE_SESSION_REPOSITORY_DISABLED	false
begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_chiavi_config_template l 
	where l.var_codice ='BE_SESSION_REPOSITORY_DISABLED';
    if (cnt = 0) then       
	Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
	(SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
	, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_INFASATO)
	Values
	(SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'BE_SESSION_REPOSITORY_DISABLED'
	, 'Disabilita (TRUE) o Abilita (FALSE) acquisisci file immagine prima di salva/protocolla'
	, 'false', 'B', '1', '1', 'N');

    end if;
  end;
end;
/




-- BE_ELIMINA_MAIL_ELABORATE	0
begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_chiavi_config_template l 
	where l.var_codice ='BE_ELIMINA_MAIL_ELABORATE';
    if (cnt = 0) then    
	Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
		(SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
		, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_INFASATO)
 Values
		(SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'BE_ELIMINA_MAIL_ELABORATE'
		, 'Eliminazione automatica delle mail pervenute su casella PEC processate'
		, '0', 'B', '1', '1', 'N');

    end if;
  end;
end;
/

-- FE_TIPO_ATTO_REQUIRED	0
-- TIPO_ATTO_REQUIRED  stato sostituito da: getTipoDocObbl
--Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_INFASATO)
--Values (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_TIPO_ATTO_REQUIRED', 'Obbligatorieta della tipologia documento', '0', 'F', '1', '1');


-- FE_PROTOIN_ACQ_DOC_OBBLIGATORIA 	false
begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_chiavi_config_template l 
	where l.var_codice ='FE_PROTOIN_ACQ_DOC_OBBLIGATORIA';
    if (cnt = 0) then    Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_INFASATO)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_PROTOIN_ACQ_DOC_OBBLIGATORIA'
   , 'Acquisizione file obbligatoria sulla protocollazione semplificata'
   , 'false', 'F', '1', '1', 'N');

    end if;
  end;
end;
/

--FE_SMISTA_ABILITA_TRASM_RAPIDA	1
begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_chiavi_config_template l 
	where l.var_codice ='FE_SMISTA_ABILITA_TRASM_RAPIDA';
    if (cnt = 0) then  
	Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_INFASATO)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_SMISTA_ABILITA_TRASM_RAPIDA'
   , 'Trasmissione rapida obligatoria  sulla protocollazione semplificata e sullo smistamento'
   , '1', 'F', '1', '1', 'N');

    end if;
  end;
end;
/

-- FE_ENABLE_PROT_RAPIDA_NO_UO	true
 begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_chiavi_config_template l 
	where l.var_codice ='FE_ENABLE_PROT_RAPIDA_NO_UO';
    if (cnt = 0) then    Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_INFASATO)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_ENABLE_PROT_RAPIDA_NO_UO'
   , 'Trasmissione obbligatoria sulla protocollazione semplificata'
   , 'true', 'F', '1', '1', 'N');

    end if;
  end;
end;
/

-- BE_ELIMINA_RICEVUTE_PEC	0
begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_chiavi_config_template l 
	where l.var_codice ='BE_ELIMINA_RICEVUTE_PEC';
    if (cnt = 0) then     
	Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
	(SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
	, VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_INFASATO)
 Values
	   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'BE_ELIMINA_RICEVUTE_PEC'
	, 'Eliminazione automatica delle RICEVUTE PEC pervenute su casella PEC processate'
	, '0', 'B', '1', '1', 'N');

    end if;
  end;
end;
/

BEGIN
   DECLARE
      istruzione VARCHAR2 (2000);
	  data_inst  VARCHAR2 (200);
      tabella_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (tabella_esistente, -955);
   BEGIN
      select to_char(sysdate,'yyyymmdd') 
	  into data_inst  	  from dual ;
	  
	  istruzione 
         := 'CREATE TABLE @db_user.DPA_CHIAVI_CONF_BKP'||data_inst 
		  ||'	as select * from @db_user.DPA_CHIAVI_CONFIGURAZIONE';

	  EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN tabella_esistente
      THEN      DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN OTHERS
      THEN      RAISE;
				DBMS_OUTPUT.put_line ('KO');
   END;
END;
/
              
------------------
              
---- dpa_chiavi_configurazione.ORA.sql  marcatore per ricerca ----
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


--                FE_PAGING_ROW_DOC              Numero di documenti visualizzati nelle ricerche              
begin
    declare cnt int;
  begin
    select count(*) into cnt from	@db_user.dpa_chiavi_configurazione  l 
	where l.var_codice ='FE_PAGING_ROW_DOC';
    if (cnt = 0) then       
	INSERT INTO dpa_chiavi_configurazione
            (system_id, id_amm, var_valore, cha_tipo_chiave, cha_visibile,
             cha_modificabile, cha_globale, var_codice,
             var_descrizione)
     VALUES (seq_dpa_chiavi_config.NEXTVAL, 0, '15', 'F', '1',
             '1', '1', 'FE_PAGING_ROW_DOC',
             'Numero di documenti visualizzati nelle ricerche'
            ); 
    end if;
  end;
end;
/

-- FE_PAGING_ROW_DOCINFASC              Numero di documenti in fascicoli visualizzati nelle ricerche         
begin
    declare cnt int;
  begin
    select count(*) into cnt from	@db_user.dpa_chiavi_configurazione  l 
	where l.var_codice ='FE_PAGING_ROW_DOCINFASC';
    if (cnt = 0) then       
	INSERT INTO dpa_chiavi_configurazione
            (system_id, id_amm, var_valore, cha_tipo_chiave, cha_visibile,
             cha_modificabile, cha_globale, var_codice,
             var_descrizione)
     VALUES (seq_dpa_chiavi_config.NEXTVAL, 0, '10', 'F', '1',
             '1', '1', 'FE_PAGING_ROW_DOCINFASC',
             'Numero di documenti in fascicoli visualizzati nelle ricerche'
            ); 
    end if;
  end;
end;
/

--FE_PAGING_ROW_PROJECT     Numero di fascicoli visualizzati nelle ricerche                     
begin
    declare cnt int;
  begin
    select count(*) into cnt from	@db_user.dpa_chiavi_configurazione  l 
	where l.var_codice ='FE_PAGING_ROW_PROJECT';
    if (cnt = 0) then       
	INSERT INTO dpa_chiavi_configurazione
            (system_id, id_amm, var_valore, cha_tipo_chiave, cha_visibile,
             cha_modificabile, cha_globale, var_codice,
             var_descrizione)
     VALUES (seq_dpa_chiavi_config.NEXTVAL, 0, '10', 'F', '1',
             '1', '1', 'FE_PAGING_ROW_PROJECT',
             'Numero di fascicoli visualizzati nelle ricerche '
            ); 
    end if;
  end;
end;
/

-- FE_IS_PRESENT_NOTE Imposta un valore Si/No se presente o no una nota nelle ricerche e export documenti e fascicoli
begin
    declare cnt int;
  begin
    select count(*) into cnt from	@db_user.dpa_chiavi_configurazione  l 
	where l.var_codice ='FE_IS_PRESENT_NOTE';
    if (cnt = 0) then       
	INSERT INTO dpa_chiavi_configurazione
            (system_id, id_amm, var_valore, cha_tipo_chiave, cha_visibile,
             cha_modificabile, cha_globale, var_codice,
             var_descrizione)
     VALUES (seq_dpa_chiavi_config.NEXTVAL, 0, '0', 'F', '1',
             '1', '1', 'FE_IS_PRESENT_NOTE',
             'Imposta un valore Si/No se presente o no una nota nelle ricerche e export documenti e fascicoli'
            ); 
    end if;
  end;
end;
/


--FE_PAGING_ROW_TRASM Imposta il numero di risultati della griglia di ricerca trasmissioni
begin
    declare cnt int;
  begin
    select count(*) into cnt from	@db_user.dpa_chiavi_configurazione  l 
	where l.var_codice ='FE_PAGING_ROW_TRASM';
    if (cnt = 0) then       
	INSERT INTO dpa_chiavi_configurazione
            (system_id, id_amm, var_valore, cha_tipo_chiave, cha_visibile,
             cha_modificabile, cha_globale, var_codice,
             var_descrizione)
     VALUES (seq_dpa_chiavi_config.NEXTVAL, 0, '10', 'F', '1',
             '1', '1', 'FE_PAGING_ROW_TRASM',
             'Numero di fascicoli visualizzati nelle ricerche '
            ); 
    end if;
  end;
end;
/

-- BE_RIC_MITT_INTEROP_BY_MAIL_DESC
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC';
    IF (cnt = 0) THEN       
  
insert into DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
   ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE
   ,CHA_MODIFICABILE,CHA_GLOBALE)
         values
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,
'BE_RIC_MITT_INTEROP_BY_MAIL_DESC'
  ,'ATTIVA LA RICERCA DEL MITTENTE PER INTEROPERABILITA PER DESCRIZIONE E MAIL ANZICHE SOLO MAIL. VALORI POSSIBILI 0 o 1'
  ,'1','B','1'
  ,'1','1');

  
    END IF;
    END;
END;
/



              
------------------
              
---- classcat.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.classcat (docId INT)
RETURN varchar IS risultato varchar(4000);

item varchar(4000);

CURSOR cur IS
SELECT DISTINCT A.VAR_CODICE
FROM PROJECT A
WHERE A.CHA_TIPO_PROJ = 'F'
AND A.SYSTEM_ID IN
(SELECT A.ID_FASCICOLO FROM PROJECT A, PROJECT_COMPONENTS B WHERE A.SYSTEM_ID=B.PROJECT_ID AND B.LINK=docId);

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

IF(risultato IS NOT NULL   and length(risultato) < 3000  -- aggiunta per blindare da errori di buffer troppo piccolo  
) THEN
risultato := risultato||'; '||item;
ELSE
risultato := risultato||item;
END IF;

END LOOP;

RETURN risultato;

END classcat;
/


              
------------------
              
---- corrcatbytipo.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.corrcatbytipo (
   docid        INT,
   tipo_proto   VARCHAR,
   tipocorr     VARCHAR
)
   RETURN varchar IS risultato clob;
item clob;
tipo_mitt_dest VARCHAR(10);
LNG INT;
   CURSOR cur
   IS
      SELECT   c.var_desc_corr, dap.cha_tipo_mitt_dest
          FROM dpa_corr_globali c, dpa_doc_arrivo_par dap
         WHERE dap.id_profile = docid AND dap.id_mitt_dest = c.system_id
      ORDER BY dap.cha_tipo_mitt_dest DESC;
BEGIN
   risultato := '';

   OPEN cur;

   LOOP
      FETCH cur
       INTO item, tipo_mitt_dest;

      EXIT WHEN cur%NOTFOUND;
      lng := LENGTH (risultato);

      IF (risultato IS NOT NULL AND lng >= (3900 - 128))
      THEN
         RETURN risultato || '...';
      ELSE
         BEGIN
            IF (tipocorr = 'M')
            THEN
               IF (tipo_proto = 'P' AND tipo_mitt_dest = 'M')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (M)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'A' AND tipo_mitt_dest = 'M')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (M)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'A' AND tipo_mitt_dest = 'MD')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (MM)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'A' AND tipo_mitt_dest = 'I')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (MI)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'I' AND tipo_mitt_dest = 'M')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (M)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;
            END IF;

            IF (tipocorr = 'D')
            THEN
               IF (tipo_proto = 'P' AND tipo_mitt_dest = 'D')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (D)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'P' AND tipo_mitt_dest = 'C')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (CC)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'I' AND tipo_mitt_dest = 'D')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (D)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'I' AND tipo_mitt_dest = 'C')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (CC)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'P' AND tipo_mitt_dest = 'L')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || dest_in_lista (docid);
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'P' AND tipo_mitt_dest = 'F')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || dest_in_rf (docid);
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;
            END IF;
         END;
      END IF;
   END LOOP;

   RETURN risultato;
END corrcatbytipo;
/




              
------------------
              
---- CORRCAT.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.CORRCAT (docId INT, tipo_proto VARCHAR)
RETURN varchar IS risultato clob;

item clob;
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


              
------------------
              
---- esisteNotaVisibile.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE function @db_user.esisteNotaVisibile
(p_TIPOOGGETTOASSOCIATO varchar 
, p_IDOGGETTOASSOCIATO int
, p_ID_RUOLO_IN_UO int
, p_IDUTENTECREATORE int
, p_IDRUOLOCREATORE int
) 
RETURN varchar IS ultimanota varchar(2000);

BEGIN 

IF (p_TIPOOGGETTOASSOCIATO  <> 'F' AND p_TIPOOGGETTOASSOCIATO  <> 'D') THEN 
            ultimanota := '-1'; 
    RETURN ultimanota ;

END IF;          


IF p_TIPOOGGETTOASSOCIATO = 'F' THEN 
SELECT TESTO into ultimanota 
      FROM
      (
      SELECT /*+ FIRST_ROWS(1) */ 
      -- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
      -- N.SYSTEM_ID,
      nvl(N.TESTO,'null') as testo
     FROM    DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN PROJECT PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID 
      LEFT JOIN DPA_CORR_GLOBALI DP ON N.IDRUOLOCREATORE = DP.ID_GRUPPO
      WHERE   
      N.TIPOOGGETTOASSOCIATO = p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO = p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' OR
      (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
      (select id_registro from dpa_l_ruolo_reg rr where  DP.ID_GRUPPO = p_ID_RUOLO_IN_UO)) OR
      (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = p_IDUTENTECREATORE) OR
      (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = p_IDRUOLOCREATORE)
      )
      ORDER BY N.DATACREAZIONE DESC
      )
      WHERE   ROWNUM = 1 ; 
END IF; 

IF p_TIPOOGGETTOASSOCIATO = 'D' THEN  --join con la profile invece della project
SELECT TESTO into ultimanota 
      FROM
      (
      SELECT /*+ FIRST_ROWS(1) */ 
      -- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
      -- N.SYSTEM_ID,
      nvl(N.TESTO,'null') as testo
     FROM    DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN profile PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID 
      LEFT JOIN DPA_CORR_GLOBALI DP ON N.IDRUOLOCREATORE = DP.ID_GRUPPO
      WHERE   
      N.TIPOOGGETTOASSOCIATO = p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO = p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' OR
      (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
      (select id_registro from dpa_l_ruolo_reg rr where  DP.ID_GRUPPO = p_ID_RUOLO_IN_UO)) OR
      (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = p_IDUTENTECREATORE) OR
      (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = p_IDRUOLOCREATORE)
      )
      ORDER BY N.DATACREAZIONE DESC
      )
      WHERE   ROWNUM = 1 ; 
      
END IF; 

if (ultimanota is not null)
then
    ultimanota := 'Si';
else
    ultimanota := 'No';
end if;

return ultimanota;


EXCEPTION
when no_data_found  then
ultimanota := 'No'; 
 return ultimanota;

when others  then
ultimanota := '-1'; 
 return ultimanota;

 END;
/


              
------------------
              
---- getchaimg.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getchaimg (docnum NUMBER)
RETURN VARCHAR
IS
tmpVar varchar(30);
BEGIN
declare
v_path varchar(500);
vMaxIdGenerica number;
begin

begin
SELECT
MAX (v1.version_id)
INTO vMaxIdGenerica
FROM VERSIONS v1, components c
WHERE v1.docnumber = docNum
AND v1.version_id = c.version_id;
EXCEPTION
WHEN OTHERS THEN
vMaxIdGenerica:=0;
end;

begin
select ext into v_path from components where docnumber=docNum and version_id=vMaxIdGenerica;

EXCEPTION
WHEN OTHERS THEN
tmpVar:='0';
end;

if(v_path <> '' OR v_path is  not null)
then tmpVar:= trim(v_path);
else tmpVar:='0';
end if;

end;
RETURN tmpVar;
END getChaImg;
/




------------------

---- getcodruolobyidcorr.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getcodruolobyidcorr (idcorr INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (256);
BEGIN
   SELECT var_cod_rubrica
     INTO risultato
     FROM dpa_corr_globali
    WHERE system_id = idcorr;

   RETURN risultato;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      risultato := NULL;
      RETURN risultato;
END getcodruolobyidcorr;
/
------------------

---- getContatoreDoc.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION getContatoreDoc
	(docNumber INT, tipoContatore CHAR)
RETURN VARCHAR IS
	risultato VARCHAR(255);
	valoreContatore VARCHAR(255) := '';
	annoContatore VARCHAR(255) := '';
	codiceRegRf VARCHAR(255) := '';

BEGIN

select valore_oggetto_db, anno into valoreContatore, annoContatore
from dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
	where dpa_associazione_templates.doc_number = to_char(docNumber)
	and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
	and dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
	and dpa_tipo_oggetto.descrizione = 'Contatore'
	and dpa_oggetti_custom.cha_tipo_tar = tipoContatore;

IF(tipoContatore<>'T') THEN
	select dpa_el_registri.var_codice into codiceRegRf
	from dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri
		where dpa_associazione_templates.doc_number = to_char(docNumber)
		and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
		and dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
		and dpa_tipo_oggetto.descrizione = 'Contatore'
		and dpa_oggetti_custom.cha_tipo_tar = tipoContatore
		and dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
END IF;

risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');

RETURN risultato;
END getContatoreDoc;
/


------------------

---- getContatoreDocOrdinamento.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getContatoreDocOrdinamento 
       (docNumber INT, tipoContatore CHAR)
RETURN int IS risultato VARCHAR(255);

BEGIN

select valore_oggetto_db into risultato
from dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
  where dpa_associazione_templates.doc_number = to_char(docNumber)
  and   dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
  and   dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
  and   dpa_tipo_oggetto.descrizione = 'Contatore'
  and   dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';

RETURN to_number(risultato);

END getContatoreDocOrdinamento;
/
------------------

---- getContatoreFascContatore.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getContatoreFascContatore (systemId INT, tipoContatore CHAR)
RETURN int IS risultato VARCHAR(255);

BEGIN

select
dpa_ass_templates_fasc.valore_oggetto_db
into risultato
from
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc
where
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.DESCRIZIONE = 'Contatore'
and
dpa_oggetti_custom_fasc.DA_VISUALIZZARE_RICERCA='1';
RETURN to_number(risultato);

END getContatoreFascContatore;
/



------------------

---- getContatoreFasc.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getContatoreFasc (systemId INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);

BEGIN

select
valore_oggetto_db, anno
into valoreContatore, annoContatore
from
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc
where
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore;


if (tipocontatore <> 'T') then
begin

select
dpa_el_registri.var_codice
into codiceRegRf
from
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc, dpa_el_registri
where
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore
and
dpa_ass_templates_fasc.id_aoo_rf = dpa_el_registri.system_id;
risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
END;
else
risultato :=   nvl(valoreContatore,'');
END IF;

RETURN risultato;
END getContatoreFasc;
/


              
------------------
              
---- getdataarrivodoc.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getdataarrivodoc (p_docnumber INT)
   RETURN DATE
IS
   risultato   DATE;
BEGIN
   select dta_arrivo  INTO risultato from (
   SELECT  /*+FIRST_ROWS(1) */ dta_arrivo
    
       FROM VERSIONS
      WHERE docnumber = p_docnumber
   ORDER BY version_id DESC)   
       where ROWNUM = 1
   ;

   RETURN risultato;
   exCePtion
   when no_data_FOUND
   THEN RISULTATO := null;
   RETURN RISULTATO;
END getdataarrivodoc;
/


              
------------------
              
---- getDescTitolario.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getDescTitolario (IdTit INT)
RETURN varchar IS risultato varchar2(2000);

varStato varchar2(1);
dataInizio varchar2(20);
dataFine varchar2(20);

CURSOR cur IS
select cha_stato, 
    to_char(dtA_ATTIVAZIONE, 'DD/MM/YYYY') as dataInizio2,
    to_char(dta_cessazione, 'DD/MM/YYYY') as dataFine2
    from project where system_id=IdTit;


BEGIN

    varStato := '0';

OPEN cur;
LOOP
FETCH cur INTO varStato, dataInizio, dataFine;
EXIT WHEN cur%NOTFOUND;

begin
    if(varStato != '0')
    then
        begin
        if(varStato = 'A')
        then
            risultato := 'Titolario Attivo';
        else
            risultato := 'Titolario valido dal ' || dataInizio || ' al ' || dataFine;
        end if;
        end;
    end if;
end;

END LOOP;

    RETURN risultato;
END getDescTitolario;
/


              
------------------
              
---- getDiagrammiStato.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getDiagrammiStato (p_docOrId INT, p_tipo varchar)
   RETURN VARCHAR
IS
   risultato   VARCHAR (256);
BEGIN
if(p_tipo='D')
then
   SELECT var_descrizione
     INTO risultato
     FROM dpa_stati f, dpa_diagrammi b
    WHERE b.DOC_NUMBER = p_docOrId
    AND f.system_id = b.ID_STATO ;
    RETURN risultato;

END IF;      

if(p_tipo='F')
then
 SELECT var_descrizione
     INTO risultato
     FROM dpa_stati f, dpa_diagrammi b
    WHERE b.ID_PROJECT = p_docOrId
    AND f.system_id = b.ID_STATO ;
    RETURN risultato ;

END IF;

   
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      risultato := NULL;
      RETURN risultato;
END getDiagrammiStato;
/


              
------------------
              
---- getEsitoPubblicazione.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getEsitoPubblicazione (p_systemId INT)
   RETURN varchar IS risultato varchar(256);
   esito   VARCHAR (256);
   errore VARCHAR(256);
BEGIN
   SELECT ESITO_PUBBLICAZIONE, ERRORE_PUBBLICAZIONE
     INTO esito, errore
     FROM PUBBLICAZIONI_DOCUMENTI
    WHERE ID_PROFILE = p_systemId;

if(errore is NOT null)
then
    risultato := 'Il documento non  stato pubblicato ' || errore;
else
if(esito = '1' and errore is null)
then
risultato := 'Il documento  stato pubblicato';
else
if(esito = '0' and errore is null)
then
risultato := 'Il documento non  stato pubblicato';
end if;
end if;
end if;
return risultato;

   
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      risultato := NULL;
      RETURN risultato;
END getEsitoPubblicazione;
/


              
------------------
              
---- getValCampoProfDoc.ORA.sql  marcatore per ricerca ----

CREATE OR REPLACE FUNCTION getValCampoProfDoc(DocNumber INT, CustomObjectId INT)

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

 

--Casella di selezione (Per la casella di selezione serve un caso particolare perch i valori sono multipli)

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

   

    elsif(tipoOggetto = 'Contatore') then

    begin

        select getContatoreDoc(DocNumber,'R')  into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber;

 

    end;

else

 

 

--Tutti gli altri

    select valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber;

end if;

 

RETURN result;

 

END getValCampoProfDoc;

/ 


              
------------------
              
---- getValCampoProfDocOrder.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION  getValCampoProfDocOrder(DocNumber INT, CustomObjectId INT)
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

if (tipoOggetto = 'Corrispondente') then
      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 
      from dpa_CORR_globali cg where cg.SYSTEM_ID = (
      select valore_oggetto_db from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber));
     else
      
--Casella di selezione (Per la casella di selezione serve un caso particolare perch i valori sono multipli)
if(tipoOggetto = 'CasellaDiSelezione') then
    BEGIN
        declare item varchar(255);
        CURSOR curCasellaDiSelezione IS select  valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber) and valore_oggetto_db is not null; 
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
end if;

RETURN result;

exception
when no_data_found
then
result := null; --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 
RETURN result;
when others
then
result := SQLERRM; 
RETURN result;

END getValCampoProfDocOrder;
/              
------------------
              
---- GetValProfObjPrj.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION GetValProfObjPrj(PrjId INT, CustomObjectId INT)
RETURN VARCHAR IS result VARCHAR(255);

tipoOggetto varchar(255);tipoCont varchar(1);

BEGIN

select b.descrizione,cha_tipo_Tar
into tipoOggetto,tipoCont
from 
dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b 
where 
a.system_id = CustomObjectId
and
a.id_tipo_oggetto = b.system_id;

if (tipoOggetto = 'Corrispondente') then
      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 
      from dpa_CORR_globali cg where cg.SYSTEM_ID = (
      select valore_oggetto_db from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = PrjId);
     else
      
--Casella di selezione (Per la casella di selezione serve un caso particolare perch i valori sono multipli)
if(tipoOggetto = 'CasellaDiSelezione') then
    BEGIN
        declare item varchar(255);
        CURSOR curCasellaDiSelezione IS select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = to_char(PrjId) and valore_oggetto_db is not null; 
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
    
    elsif(tipoOggetto = 'Contatore') then
    begin
        select getContatoreFasc(PrjId,tipoCont)  into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = PrjId; 

    end;
    
else 
--Tutti gli altri
    select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = PrjId; 
end if;
end if;

RETURN result;

exception
when no_data_found
then
result := null; --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 
RETURN result;
when others
then
result := SQLERRM; 
RETURN result;

END GetValProfObjPrj;
/
              
------------------
              
---- GetValProfObjPrjOrder.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION             GetValProfObjPrjOrder(PrjId INT, CustomObjectId INT)
RETURN VARCHAR IS result VARCHAR(255);

tipoOggetto varchar(255);

BEGIN

select b.descrizione
into tipoOggetto
from 
dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b 
where 
a.system_id = CustomObjectId
and
a.id_tipo_oggetto = b.system_id;

if (tipoOggetto = 'Corrispondente') then
      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 
      from dpa_CORR_globali cg where cg.SYSTEM_ID = (
      select valore_oggetto_db from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = PrjId);
     else
      
--Casella di selezione (Per la casella di selezione serve un caso particolare perch i valori sono multipli)
if(tipoOggetto = 'CasellaDiSelezione') then
    BEGIN
        declare item varchar(255);
        CURSOR curCasellaDiSelezione IS select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = to_char(PrjId) and valore_oggetto_db is not null; 
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
    select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = PrjId; 
end if;
end if;

RETURN result;

exception
when no_data_found
then
result := null; --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 
RETURN result;
when others
then
result := SQLERRM; 
RETURN result;

END GetValProfObjPrjOrder;
/
------------------

---- IsValidModelloTrasmissione.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.isvalidmodellotrasmissione (
   -- Id del template da analizzare
   templateid   NUMBER
)
   RETURN NUMBER
IS
-- 0 se il modello  valido, -1 se si  verificato un errore, un numero maggiore
-- di 0 se ci sono ruoli inibiti
   retval   NUMBER;
/******************************************************************************
   AUTHOR:    Samuele Furnari
	NAME:       IsValidModelloTrasmissione
   PURPOSE:
        Funzione per la verifica di validit di un modello di trasmissione.
        Per essere valido, il modello di cui viene passato l'id non deve
        contenere destinatari inibiti alla ricezione di trasmissioni

******************************************************************************/
BEGIN
   retval := 0;

   -- Conteggio dei destinatari inibiti alla ricezione di trasmissioni
   SELECT COUNT ('x')
     INTO retval
     FROM dpa_modelli_mitt_dest, dpa_corr_globali
    WHERE id_modello = templateid
      AND dpa_modelli_mitt_dest.cha_tipo_urp = 'R'
      AND dpa_modelli_mitt_dest.cha_tipo_mitt_dest = 'D'
      AND dpa_modelli_mitt_dest.id_corr_globali = dpa_corr_globali.system_id
      AND (   dpa_corr_globali.cha_disabled_trasm = '1'
           OR dpa_corr_globali.dta_fine IS NOT NULL
          );

   RETURN retval;
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN -1;
END isvalidmodellotrasmissione;
/
------------------

---- insert_DPA_DOCSPA.ORA.sql  marcatore per ricerca ----
begin
Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values    (seq.nextval, sysdate, '3.16.1');
end;
/                      
