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
   indiceesiste EXCEPTION;
   PRAGMA EXCEPTION_INIT(indiceesiste, -1408);

   cnt       INT;

   nomeutente    VARCHAR2 (200) := upper('@db_user');
   istruzioneSQL VARCHAR2(2000) ; 
   
   BEGIN
   istruzioneSQL := 'CREATE INDEX '||nomeutente||'.INDX_VAR_SEGN 
            ON PROFILE(VAR_SEGNATURA) ' ; 
   
     SELECT COUNT (*) INTO cnt
        FROM all_indexes
       WHERE index_name = upper('INDX_VAR_SEGN')
       and table_name=upper('PROFILE')
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

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CORR_GLOBALI';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_CORR_GLOBALI' and column_name='CHA_DISABLED_TRASM';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE @db_user.DPA_CORR_GLOBALI ADD CHA_DISABLED_TRASM VARCHAR2(1)';
        end if;
    end if;
    end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_STATI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_STATI' and column_name='NON_RICERCABILE';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE @db_user.DPA_STATI ADD NON_RICERCABILE INTEGER';
		end if;
	end if;
	end;
end;
/ 

CREATE OR REPLACE FUNCTION @db_user.GETcodUO (idUO INT)
RETURN VARCHAR IS risultato VARCHAR(256);

BEGIN
if(idUO is null)
	then risultato:=' ';
else
	begin
	risultato := ' ';
	SELECT VAR_CODICE into risultato FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID=idUO;
	exception  WHEN OTHers then risultato:='';
	end;
end if;
RETURN risultato;

END GETcodUO;
/


CREATE OR REPLACE PROCEDURE @db_user.SP_RIMUOVI_DOCUMENTI(idProfile IN number, ReturnValue OUT NUMBER) IS
BEGIN

DELETE FROM DPA_TRASM_UTENTE WHERE id_trasm_singola in
(SELECT system_id FROM DPA_TRASM_SINGOLA WHERE id_trasmissione in
(select     t.system_id  from dpa_trasmissione t where t.id_profile =idProfile));

DELETE FROM DPA_TRASM_SINGOLA WHERE id_trasmissione in
(select     t.system_id  from dpa_trasmissione t where t.id_profile =idProfile);


DELETE FROM DPA_TRASMISSIONE WHERE id_profile=idProfile;

DELETE FROM project_components where LINK =idProfile ;
DELETE FROM VERSIONS WHERE DOCNUMBER = idProfile;
DELETE FROM COMPONENTS WHERE DOCNUMBER = idProfile;
DELETE FROM DPA_AREA_LAVORO WHERE ID_PROFILE = idProfile;
DELETE FROM DPA_PROF_PAROLE WHERE ID_PROFILE = idProfile;
DELETE FROM PROFILE WHERE DOCNUMBER = idProfile;
DELETE FROM SECURITY WHERE THING = idProfile;
delete from dpa_todolist where id_profile = idProfile;
DELETE FROM DPA_DIAGRAMMI WHERE DOC_NUMBER = idProfile;

DECLARE cnt INT;
BEGIN
SELECT COUNT(*) INTO cnt FROM DPA_ASSOCIAZIONE_TEMPLATES
WHERE DOC_NUMBER = idProfile;
IF (cnt != 0) THEN
DELETE FROM DPA_ASSOCIAZIONE_TEMPLATES
WHERE DOC_NUMBER = idProfile;
END IF;
END;
ReturnValue:=1;
END;
/

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

-- 				   BE_NOTE_IN_SEGNATURA 
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='BE_NOTE_IN_SEGNATURA ';
    IF (cnt = 0) THEN       
		INSERT INTO  @db_user.DPA_CHIAVI_CONFIGURAZIONE
				   (system_id, ID_AMM
				   , VAR_CODICE 
				   , VAR_DESCRIZIONE 
				   , VAR_VALORE 
				   , CHA_TIPO_CHIAVE 
				   , CHA_VISIBILE 
				   , CHA_MODIFICABILE 
				   , CHA_GLOBALE 
				   , VAR_CODICE_OLD_WEBCONFIG )
			 VALUES (SEQ_DPA_CHIAVI_CONFIG.nextval, 0
				   ,'BE_NOTE_IN_SEGNATURA '
				   ,'Aggiunge il campo note tutti nella segnatura xml per interoperabilità'
				   ,'0'
				   ,'B'
				   ,'1'
				   ,'1'
				   ,'1'
				   ,NULL); 
  
    END IF;
    END;
END;
/

-- 				   BE_FASC_TUTTI_TIT
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='BE_FASC_TUTTI_TIT';
    IF (cnt = 0) THEN       
		INSERT INTO  @db_user.DPA_CHIAVI_CONFIGURAZIONE
				   (system_id, ID_AMM
				   , VAR_CODICE 
				   , VAR_DESCRIZIONE 
				   , VAR_VALORE 
				   , CHA_TIPO_CHIAVE 
				   , CHA_VISIBILE
				   , CHA_MODIFICABILE 
				   , CHA_GLOBALE 
				   , VAR_CODICE_OLD_WEBCONFIG )
			 VALUES (SEQ_DPA_CHIAVI_CONFIG.nextval, 0
				   ,'BE_FASC_TUTTI_TIT'
				   ,'Se ad 1 permette la creazione di fascicoli su tutti i titolari anche quelli non attivi'
				   ,'0'
				   ,'B'
				   ,'1'
				   ,'1'
				   ,'1'
				   ,NULL); 
  
    END IF;
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
    WHERE VAR_CODICE='FE_PERMANENT_DISPLAYS_SEGNATURE';
    IF (cnt = 0) THEN       

-- il record serve per abilitare la segnatura permanente sui documenti acquisiti in formato pdf nei protocolli
INSERT INTO @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM
   ,VAR_CODICE
   ,VAR_DESCRIZIONE
   ,VAR_VALORE,CHA_TIPO_CHIAVE
   ,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
VALUES 
	(SEQ_DPA_CHIAVI_CONFIG.nextval,0,
           'FE_PERMANENT_DISPLAYS_SEGNATURE',
           'Abilita la segnatura permanente sui documenti acquisiti in formato pdf nei protocolli',
           '0',        'F',
           '1',		   '1',           '1' ) ;
    END IF;
    END;
END;
/





begin
Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values    (seq.nextval, sysdate, '3.15.1');
 --Values    (seq.nextval, sysdate, 'P3 2.6.0');
end;
/





