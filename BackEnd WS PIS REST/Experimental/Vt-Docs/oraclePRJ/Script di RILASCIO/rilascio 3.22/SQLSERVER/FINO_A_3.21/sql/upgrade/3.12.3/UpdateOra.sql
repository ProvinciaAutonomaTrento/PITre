BEGIN
    DECLARE cnt int;
     --cntdati int;
  BEGIN

    SELECT COUNT(*) INTO cnt FROM all_sequences where sequence_name='SEQ_DISPOSITIVI_STAMPA';
    --select max(nvl(system_id,0)) +1 into cntdati from dpa_chiavi_configurazione;
    IF (cnt = 0) THEN
          execute immediate ' CREATE SEQUENCE @db_user.SEQ_DISPOSITIVI_STAMPA
                  START WITH 1
                  MAXVALUE 999999999999999999999999999
                  MINVALUE 0
                  NOCYCLE
                  NOCACHE
                  NOORDER';
    END IF;
    END;
END;
/



/*
AUTORE:					  ALESSANDRO FIORDI
Data creazione:				  04/04/2011
Scopo della modifica:		CREARE LA TABELLA DPA_DISPOSITIVI_STAMPA
*/
BEGIN
   DECLARE
      istruzione         VARCHAR2 (2000)
         := 'CREATE TABLE @db_user.DPA_DISPOSITIVI_STAMPA (
                ID NUMBER(1,0),
                CODE VARCHAR2(50)        NOT NULL ,
                DESCRIPTION VARCHAR2(50) NOT NULL ,
                PRIMARY KEY (ID)
                ) ';

      tabella_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (tabella_esistente, -955);
   BEGIN
      EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN tabella_esistente
      THEN
         DBMS_OUTPUT.put_line ('tabella già esistente');
      WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_DISPOSITIVI_STAMPA 
       WHERE CODE='ZEBRA';
    IF (cnt = 0) THEN       
		insert into @db_user.DPA_DISPOSITIVI_STAMPA  (Id, Code, Description) values (1,'ZEBRA','ZEBRA');
    END IF;
	END;
END;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_DISPOSITIVI_STAMPA 
       WHERE CODE='DYMO_LABEL_WRITER_400';
    IF (cnt = 0) THEN       
		insert into @db_user.DPA_DISPOSITIVI_STAMPA  (Id, Code, Description) values (2, 'DYMO_LABEL_WRITER_400', 'DYMO LABEL WRITER 400');
    END IF;
	END;
END;
/

-- altrimenti fallisce la successiva creazione della FK da DPA_AMMINISTRA 
commit
/


/*
AUTORE:                      ALESSANDRO FIORDI
Data creazione:                  04/04/2011
Scopo della modifica:        AGGIUNGERE LA COLONNA ID_DISPOSITIVO_STAMPA NEL DPA_AMMINISTRA
                E CHIAVE ESTERNA
*/
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_AMMINISTRA';
      nomecolonna   VARCHAR2 (32)  := 'ID_DISPOSITIVO_STAMPA';
 -- default a dispositivo ZEBRA	! 					  
     tipodato      VARCHAR2 (200) := ' int default 1';

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
                              || ' '|| tipodato;

            EXECUTE IMMEDIATE 'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD CONSTRAINT FK_'|| 'AMM_'|| nomecolonna
                              || ' FOREIGN KEY
                              ('|| nomecolonna
                              || ') REFERENCES '|| nomeutente ||'.'|| 'DPA_DISPOSITIVI_STAMPA'
                              ||'(ID)
							   ON DELETE SET NULL ';

         END IF;
      END IF;
   END;
END;
/

/*
AUTORE:					  ALESSANDRO FIORDI
Data creazione:				  04/04/2011
Scopo della modifica:		AGGIUNGERE LA COLONNA ID_DISPOSITIVO_STAMPA NEL PEOPLE
				E CHIAVE ESTERNA
*/

BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'PEOPLE';
      nomecolonna   VARCHAR2 (32)  := 'ID_DISPOSITIVO_STAMPA';
      tipodato      VARCHAR2 (200) := ' int NULL';

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

            EXECUTE IMMEDIATE 'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD CONSTRAINT FK_'|| nometabella || nomecolonna
                              || ' FOREIGN KEY
                              ('|| nomecolonna
                              || ') REFERENCES '|| nomeutente ||'.'|| 'DPA_DISPOSITIVI_STAMPA'
                              ||'(ID)
							   ON DELETE SET NULL ';

         END IF;
      END IF;
   END;
END;
/


begin
declare cnt int;
begin
	select count(*) into cnt
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l where l.var_codice ='USA_CONNECTBYPRIOR_OR_WITH';

	if (cnt = 0) then
		insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
		(SYSTEM_ID
		,ID_AMM
		,VAR_CODICE
		,VAR_DESCRIZIONE
		,VAR_VALORE
		,CHA_TIPO_CHIAVE
		,CHA_VISIBILE
		,CHA_MODIFICABILE
		,CHA_GLOBALE
		--,VAR_CODICE_OLD_WEBCONFIG
		)
		values
		(@db_user.SEQ_DPA_CHIAVI_CONFIG.nextval
		,0
		,'USA_CONNECTBYPRIOR_OR_WITH'
		,'Chiave utilizzate per decidere se utilizzare le nuove query per la risalita gerarchica'
		,'0'
		,'B'
		,'1'
		,'1'
		,'1');
	end if;
end;
end;
/

begin
Insert into @db_user.DPA_DOCSPA
   (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values
   (seq.nextval, sysdate, '3.12.3');
end;
/
