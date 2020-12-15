-- Alessandro faillace per Original File name

Begin 
	Utl_Backup_Plsql_Code    ( 'FUNCTION', 'getNomeOriginale');
end;
/


create or replace
FUNCTION             getNomeOriginale (docnum number)
RETURN VARCHAR2
IS
nomeOriginale   VARCHAR2 (500);
vmaxidgenerica   NUMBER;

BEGIN
                                               BEGIN
                                               SELECT MAX (v1.version_id)                                     INTO vmaxidgenerica
                                               FROM VERSIONS v1, components c
                                               WHERE v1.docnumber = docnum AND v1.version_id = c.version_id;

                                                               EXCEPTION                                        
                                                               WHEN NO_DATA_FOUND          THEN
                                                               vmaxidgenerica := 0;
                                                               WHEN OTHERS                                 THEN RAISE; 
                                               END;

                                               BEGIN
                                               SELECT var_nomeOriginale                                        INTO nomeOriginale
                                               FROM components
                                               WHERE docnumber = docnum AND version_id = vmaxidgenerica;

                                                               EXCEPTION                                        
                                                               WHEN NO_DATA_FOUND          THEN
                                                               nomeOriginale := '';
                                                               WHEN OTHERS THEN RAISE; 
                                               END;

RETURN nomeOriginale;
END getNomeOriginale;
/



-- VELTRI - inserimento Nuova microfunzione per abilitare il tab archivio in ricerca fascicoli


/* Formatted on 2012/07/16 10:38 (Formatter Plus v4.8.8) */
BEGIN
   utl_insert_chiave_microfunz
      ('DO_TAB_ARCHIVIO',
       'Abilita il tab archivio nelle ricerche fascicoli',
       NULL,
       'Y',
       NULL,
       '3.29',
       NULL
      );
END;
/

/* Formatted on 2012/07/16 10:38 (Formatter Plus v4.8.8) */
BEGIN
   utl_insert_chiave_microfunz
      ('CERCA_DOC_ADV',
       'Ricerca documenti avanzata selezionata di default',
       NULL,
       'Y',
       NULL,
       '3.29',
       NULL
      );
END;
/

-- Andrea De Marco - MEV per Ospedale Maggiore
BEGIN
                               utl_insert_chiave_microfunz
                               ('FASC_RICLASS',
                               'Abilita il pulsante riclassifica nei dettagli del fascicolo',
                               NULL,
                               'Y',
                               NULL,
                               '3.29',
                               NULL
                               );
                END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='VAR_CODICE';

if cntcol > 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG MODIFY (VAR_CODICE VARCHAR2(256 CHAR))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='MULTIPLICITY';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (MULTIPLICITY CHAR(3 CHAR))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='ID_AMM';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (ID_AMM  NUMBER(10,0) DEFAULT NULL)';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='NOTIFICATION';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (NOTIFICATION  CHAR(1 CHAR) DEFAULT ''0'')';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='CONFIGURABLE';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (CONFIGURABLE  CHAR(1 CHAR) DEFAULT ''0'')';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='NOTIFICATION_RECIPIENTS';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (NOTIFICATION_RECIPIENTS VARCHAR2(200 CHAR) DEFAULT NULL)';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ANAGRAFICA_LOG' and column_name='COLOR';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ANAGRAFICA_LOG ADD (COLOR VARCHAR2(200 CHAR) DEFAULT ''BLUE'')';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG_ATTIVATI' and column_name='NOTIFY';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_LOG_ATTIVATI ADD (NOTIFY VARCHAR2(3 CHAR) DEFAULT ''NN'' )';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG' and column_name='ID_TRASM_SINGOLA';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_LOG ADD (ID_TRASM_SINGOLA NUMBER(10, 0) )';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG' and column_name='CHECK_NOTIFY';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_LOG ADD (CHECK_NOTIFY  CHAR(1 CHAR) DEFAULT 0 )';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG' and column_name='DESC_PRODUCER';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_LOG ADD (DESC_PRODUCER VARCHAR2(500 CHAR) DEFAULT '''' )';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_EL_REGISTRI' and column_name='VAR_CODICE_AOO_IPA';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_EL_REGISTRI ADD (VAR_CODICE_AOO_IPA  VARCHAR2(16 BYTE))';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_EL_REGISTRI' and column_name='CODICE_UAC';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_EL_REGISTRI ADD (CODICE_UAC  VARCHAR2(128 BYTE))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_EL_REGISTRI' and column_name='CODICE_CLASSIFICAZIONE';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_EL_REGISTRI ADD (CODICE_CLASSIFICAZIONE  VARCHAR2(20 BYTE))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_AMMINISTRA' and column_name='VAR_EMAIL_RES_IPA';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_AMMINISTRA ADD (VAR_EMAIL_RES_IPA  VARCHAR2(128 BYTE))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_AMMINISTRA' and column_name='VAR_CODICE_AMM_IPA';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_AMMINISTRA ADD (VAR_CODICE_AMM_IPA  VARCHAR2(16 BYTE))';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='ENABLEDHISTORY';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD (ENABLEDHISTORY  CHAR(1 BYTE))';
end if;

end;
END;
/


begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_UA_INFO';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_UA_INFO
(
  USER_ID          VARCHAR2(32),
  ID_PEOPLE        NUMBER,
  DTA_LOGIN        DATE,
  IP               VARCHAR2(20),
  BROWSER_TYPE     VARCHAR2(20),
  BROWSER_VERSION  VARCHAR2(10),
  ACTIVEX          CHAR(1),
  JAVASCRIPT       CHAR(1),
  JAVAAPPLET       CHAR(1),
  USER_AGENT       VARCHAR2(256)
)';
		end if;
	end;
end;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_DOCSPA' and column_name='ID_VERSIONS_U';

if cntcol > 0 then 
	execute immediate 
	'ALTER TABLE DPA_DOCSPA MODIFY (ID_VERSIONS_U  VARCHAR2(30 BYTE))';
end if;

end;
END;
/


begin
Insert into DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
select max(system_id) +1 , sysdate,  '3.29' from Dpa_Docspa;
end;
/





