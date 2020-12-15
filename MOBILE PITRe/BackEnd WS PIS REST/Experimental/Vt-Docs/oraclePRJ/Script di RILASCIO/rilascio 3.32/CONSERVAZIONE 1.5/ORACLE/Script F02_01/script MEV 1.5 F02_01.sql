begin
    declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_VERIFICA_FORMATI_CONS';
        if (cnt = 0) 
        then
          execute immediate  'CREATE TABLE DPA_VERIFICA_FORMATI_CONS
            (
              SYSTEM_ID NUMBER NOT NULL,                                
              ID_ISTANZA NUMBER NOT NULL,                               
              ID_ITEM NUMBER,                                           
              DOCNUMBER NUMBER,                                         
              ID_PROJECT NUMBER,
              ID_DOCPRINCIPALE NUMBER,
              VERSION_ID NUMBER,                                        
              TIPO_FILE CHAR(1 BYTE),  
			  ESTENSIONE VARCHAR2(32 BYTE),			  
              CONSOLIDATO NUMBER(1),
              CONVERTIBILE NUMBER(1),                                      
              MODIFICA NUMBER(1),                                          
              UT_PROP NUMBER,                                           
              RUOLO_PROP NUMBER,    
              VALIDO NUMBER(1),
              AMMESSO NUMBER(1),
              FIRMATA NUMBER(1),
              MARCATA NUMBER(1),
              ERRORE NUMBER(1),
              TIPOERRORE NUMBER(1),
              DACONVERTIRE NUMBER(1),
              CONVERTITO NUMBER(1),
              ESITO VARCHAR2(400),                                      
              CONSTRAINT DPA_VERIFICA_FORMATI_CONS_PK PRIMARY KEY                          
              (                                                         
                SYSTEM_ID                                               
              )                                                         
                                                                  
            )';                                                         
        end if;                                                         
    end;                                                                
end;
/

-- SEQUENCE
--
BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_VERIFICA_FORMATI_CONS';
    IF (cnt = 0) THEN  -- crea la SEQUENCE

		   execute immediate 'CREATE SEQUENCE SEQ_DPA_VERIFICA_FORMATI_CONS 
					Minvalue 1 Maxvalue 999999999999999999999999999 
					Increment By 1 Start With 1 Cache 20 Noorder  Nocycle ';
    END IF;
END;        
END;
/

-- COLONNA DPA_AREA_CONSERVAZIONE
BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_AREA_CONSERVAZIONE' and column_name='ESITO_VERIFICA';

if cntcol = 0 then 
	execute immediate 
	'alter table DPA_AREA_CONSERVAZIONE add (ESITO_VERIFICA NUMBER)';
end if;

end;
END;
/

-- COLONNA policy_conservazione STATO_CONVERSIONE
BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='policy_conservazione' and column_name='STATO_CONVERSIONE';

if cntcol = 0 then 
	execute immediate 
	'alter table policy_conservazione add (STATO_CONVERSIONE VARCHAR2(10))';
end if;

end;
END;
/

-- Tabella di lookup per gli esiti di verifica
begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_ESITO_VERIFICA_CONS';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_ESITO_VERIFICA_CONS
			(
			  SYSTEM_ID NUMBER NOT NULL,
			  VALORE VARCHAR2(200) NOT NULL,
			  CONSTRAINT DPA_ESITO_VERIFICA_CONS_PK PRIMARY KEY
			  (
				SYSTEM_ID
			  )
			  ENABLE
			)';
		end if;
	end;
end;
/

INSERT ALL 
INTO DPA_ESITO_VERIFICA_CONS (SYSTEM_ID, VALORE) VALUES (0,'Non Effettuata')
INTO DPA_ESITO_VERIFICA_CONS (SYSTEM_ID, VALORE) VALUES (1,'Successo')
INTO DPA_ESITO_VERIFICA_CONS (SYSTEM_ID, VALORE) VALUES (2,'Documenti direttamente convertibili dal proprietario')
INTO DPA_ESITO_VERIFICA_CONS (SYSTEM_ID, VALORE) VALUES (3,'Documenti non convertibili direttamente dal proprietario')
INTO DPA_ESITO_VERIFICA_CONS (SYSTEM_ID, VALORE) VALUES (4,'Fallita')
INTO DPA_ESITO_VERIFICA_CONS (SYSTEM_ID, VALORE) VALUES (5,'Errore')
SELECT 1 FROM DUAL;


