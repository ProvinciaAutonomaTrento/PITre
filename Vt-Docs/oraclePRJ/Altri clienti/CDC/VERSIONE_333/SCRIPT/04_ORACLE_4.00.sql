

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='PGU_FORMATI_DOCUMENTO';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE PGU_FORMATI_DOCUMENTO
			(
			  SYSTEM_ID                 INTEGER             NOT NULL,
			  FILE_TYPE_USED            INTEGER             DEFAULT 1                     NOT NULL,
			  DESCRIPTION               VARCHAR2(255 BYTE)  NOT NULL,
			  FILE_EXTENSION            VARCHAR2(10 BYTE)   NOT NULL,
			  MAX_FILE_SIZE             INTEGER             DEFAULT 0                     NOT NULL,
			  MAX_FILE_SIZE_ALERT_MODE  INTEGER             DEFAULT 2                     NOT NULL,
			  CONTAINS_FILE_MODEL       INTEGER             DEFAULT 0                     NOT NULL,
			  DOCUMENT_TYPE             INTEGER             DEFAULT 0                     NOT NULL,
			  FILE_TYPE_SIGNATURE       INTEGER,
			  FILE_TYPE_PRESERVATION    INTEGER,
			  FILE_TYPE_VALIDATION      INTEGER
			)';
		end if;
	end;
end;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_STATO_INVIO' and column_name='STATUS_C_MASK';

if cntcol = 0 then 
	execute immediate 
	'alter table DPA_STATO_INVIO add (STATUS_C_MASK VARCHAR2(20))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='PROJECT' and column_name='CHA_in_CESTINO';

if cntcol = 0 then 
	execute immediate 
	'Alter TABLE PROJECT ADD CHA_in_CESTINO VARCHAR(1 BYTE) null';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from DPA_CHIAVI_CONFIG_TEMPLATE 
	where var_codice='FE_DETTAGLI_FIRMA';

if cntcol = 0 then 
	execute immediate 
	'INSERT INTO DPA_CHIAVI_CONFIG_TEMPLATE VALUES (
      seq_dpa_chiavi_config_template.nextval,
      ''FE_DETTAGLI_FIRMA'',
      ''Attiva/Disattiva Dettagli Firma Completi/Sintetici'',
      ''1'',
      ''F'',
      ''1'',
      ''1'',
      ''Y''
    )';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ITEMS_CONSERVAZIONE' and column_name='MASK_VALIDAZIONE_POLICY';

if cntcol = 0 then 
	execute immediate 
	'Alter TABLE DPA_ITEMS_CONSERVAZIONE ADD MASK_VALIDAZIONE_POLICY VARCHAR2(20 BYTE)';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='COMPONENTS' and column_name='FILE_INFO';

if cntcol = 0 then 
	execute immediate 
	'Alter TABLE COMPONENTS ADD FILE_INFO VARCHAR2(64 BYTE)';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from PGU_FORMATI_DOCUMENTO 
    where upper(FILE_EXTENSION) = 'PDF';

if cntcol = 0 then 
    execute immediate 
    'insert into PGU_FORMATI_DOCUMENTO
    (
    SYSTEM_ID,
    DESCRIPTION,
    FILE_EXTENSION,
    FILE_TYPE_SIGNATURE,
    FILE_TYPE_PRESERVATION,
    FILE_TYPE_VALIDATION
    )
    values
    (seq.nextval,''Adobe Acrobat'',''PDF'',1,1,1)';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from PGU_FORMATI_DOCUMENTO 
    where upper(FILE_EXTENSION) = 'DOC';

if cntcol = 0 then 
    execute immediate 
    'insert into PGU_FORMATI_DOCUMENTO
    (
    SYSTEM_ID,
    DESCRIPTION,
    FILE_EXTENSION,
    FILE_TYPE_SIGNATURE,
    FILE_TYPE_PRESERVATION,
    FILE_TYPE_VALIDATION
    )
    values
    (seq.nextval,''Microsoft Word'',''DOC'',1,1,1)';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG' and column_name='VAR_COD_AZIONE';

if cntcol = 1 then 
	execute immediate 
	'ALTER TABLE DPA_LOG MODIFY (VAR_COD_AZIONE       			VARCHAR2(256 CHAR) )';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_LOG_STORICO' and column_name='DESC_PRODUCER';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_LOG_STORICO ADD (DESC_PRODUCER VARCHAR2(500 CHAR) DEFAULT '''' )';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_AREA_CONSERVAZIONE' and column_name='DATA_RIFIUTO';

if cntcol = 0 then 
	execute immediate 
	'Alter TABLE DPA_AREA_CONSERVAZIONE ADD DATA_RIFIUTO DATE';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_CONS_VERIFICA' and column_name='CHA_TIPO_VER';

if cntcol = 0 then 
	execute immediate 
	'Alter TABLE DPA_CONS_VERIFICA ADD CHA_TIPO_VER VARCHAR2(1 BYTE)';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ITEMS_CONSERVAZIONE' and column_name='ESITO_FIRMA';

if cntcol = 0 then 
	execute immediate 
	'Alter TABLE DPA_ITEMS_CONSERVAZIONE ADD ESITO_FIRMA CHAR(1 BYTE)';
end if;

end;
END;
/

CREATE OR REPLACE Procedure Utl_insert_anagrafica_log(
    Codice               VARCHAR2 ,
    Descrizione          VARCHAR2 ,
    oggetto              VARCHAR2 ,
    Metodo               Varchar2 ,
    Forza_Aggiornamento  Varchar2 ,
    Myversione_cd        Varchar2 ,
    RFU                  VARCHAR2,
    Multi                VARCHAR2,
    IdAmm                int,
    Notific              char,
    config               char,
    Recipients           varchar2, 
    Colo                 varchar2)
Is

  Pragma Autonomous_Transaction ;
  Cnt Int;
  Maxid Int;
  Nomeutente Varchar2 (32); 
  Stringa_Msg Varchar2 (200); 
  
Begin
 -- controlli lunghezza valori passati
  
 If Utl_IsValore_Lt_Column(Codice, 'dpa_anagrafica_log', 'VAR_CODICE') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro CODICE too large for column VAR_CODICE')  ; 
  End If;
  If Utl_IsValore_Lt_Column(Descrizione, 'dpa_anagrafica_log', 'VAR_Descrizione') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Descrizione too large for column VAR_Descrizione')  ; 
  End If;
  If Utl_IsValore_Lt_Column(OGGETTO, 'dpa_anagrafica_log', 'VAR_OGGETTO') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Valore too large for column VAR_OGGETTO')  ; 
  END IF;
  If Utl_IsValore_Lt_Column(metodo, 'dpa_anagrafica_log', 'VAR_METODO') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Valore too large for column VAR_METODO')  ; 
  END IF;
  -- fine controlli lunghezza valori passati
 
  Select Username Into Nomeutente From User_Users   ; 
 
  SELECT COUNT(*)  INTO cnt
  FROM dpa_anagrafica_log
  Where Var_Codice=Codice;
  Dbms_Output.Put_Line('Sql%Rowcount dopo:' ||Sql%Rowcount); 
  
  If (Cnt         = 0 ) Then -- inserisco la chiave globale non esistente
  -- per successivo inserimento
       Insert  Into Dpa_Anagrafica_Log
        ( System_Id,     var_Codice,      Var_Descrizione
        , Var_Oggetto,Var_Metodo, MULTIPLICITY, ID_AMM, NOTIFICATION, CONFIGURABLE, NOTIFICATION_RECIPIENTS, COLOR) 
      Select Max(System_Id) +1  As System_Id       ,Codice, Descrizione        
      , OGGETTO, METODO, Multi, IdAmm, Notific, config, Recipients, Colo
      From dpa_anagrafica_log; 
      
  Stringa_Msg := 'inserita nuova chiave log ' || Codice ;  
  End if;           
  
  
  If (Cnt                  = 1 and Forza_aggiornamento = '1') Then -- chiave gi esistente
    UPDATE dpa_anagrafica_log
      SET Var_Descrizione = Descrizione, Var_Oggetto = oggetto, Var_Metodo = metodo
      Where Var_Codice    =Codice;
    Stringa_Msg := 'Aggiornati Descrizione, Oggetto, Metodo per chiave log: ' || Codice ;  
  End If;
  
  If Sql%Rowcount = 1 and Stringa_Msg is not null Then
    Dbms_Output.put_line('try to commit with stringa_msg:'||stringa_msg); 
      commit; 
      Utl_Insert_Log (Nomeutente, sysDATE, stringa_msg, Myversione_Cd ,'ok') ; 
   end if; 
  
  Exception When Others Then 
    Dbms_Output.put_line('errore da SP Utl_insert_anagrafica_log: '||SQLERRM); 
    Rollback; 
    Raise; 
  
END;
/

commit;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_SEND_STO';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_SEND_STO
(
  SYSTEM_ID NUMBER NOT NULL,
  ID_CORR_GLOBALE NUMBER,
  ID_PROFILE NUMBER,
  DTA_SPEDIZIONE DATE,
  ESITO VARCHAR2(400),
  MAIL VARCHAR2(400),
  ID_DOCUMENTTYPE NUMBER,
  ID_GROUP_SENDER NUMBER, 
  MAIL_MITTENTE VARCHAR2(400),
  ID_REG_MAIL_MITTENTE NUMBER,
  CONSTRAINT TABLE1_PK PRIMARY KEY
  (
    SYSTEM_ID
  )
  ENABLE
)';
		end if;
	end;
end;
/


BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_SEND_STO';
    IF (cnt = 0) THEN  -- crea la SEQUENCE

		   execute immediate 'CREATE SEQUENCE SEQ_DPA_SEND_STO  
					Minvalue 1 Maxvalue 999999999999999999999999999 
					Increment By 1 Start With 1 Cache 20 Noorder  Nocycle ';
    END IF;
END;        
END;
/

BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences where sequence_name='SEQ_DPA_REGISTRO_CONSERVAZIONE';
    IF (cnt = 0) THEN  -- crea la SEQUENCE

           execute immediate 'CREATE SEQUENCE SEQ_DPA_REGISTRO_CONSERVAZIONE  
                    Minvalue 1 Maxvalue 999999999999999999999999999 
                    Increment By 1 Start With 1 Cache 20 Noorder  Nocycle ';
    END IF;
END;        
END;
/


BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences where sequence_name='SEQ_DPA_STAMPA_CONSERVAZIONE';
    IF (cnt = 0) THEN  -- crea la SEQUENCE

           execute immediate 'CREATE SEQUENCE SEQ_DPA_STAMPA_CONSERVAZIONE  
                    Minvalue 1 Maxvalue 999999999999999999999999999 
                    Increment By 1 Start With 1 Cache 20 Noorder  Nocycle ';
    END IF;
END;        
END;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_ASS_POLICY_TYPE';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_ASS_POLICY_TYPE
(
  ID_POLICY  INTEGER,
  ID_TYPE    INTEGER
)';
		end if;
	end;
end;
/

begin
  Utl_Insert_Chiave_Config('FE_TRANS_FORW','Consente il sollecito da ricerca trasmissioni'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_ENABLE_AMMRF','Abilita il filtro EFFETTUATE DAI RUOLI IN RF nel tab trasmissioni'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_DETTAGLI_FIRMA','Attiva/Disattiva Dettagli Firma Completi/Sintetici'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_CONFIG_STAMPA_CONS';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_CONFIG_STAMPA_CONS
(
  SYSTEM_ID              NUMBER                 NOT NULL,
  ID_AMM                 NUMBER                 NOT NULL,
  DISABLED               NUMBER,
  PRINTER_ROLE_ID        NUMBER,
  PRINTER_USER_ID        NUMBER,
  PRINT_FREQ             NUMBER,
  DTA_LAST_PRINT         DATE,
  DTA_NEXT_PRINT         DATE,
  ID_LAST_PRINTED_EVENT  NUMBER
, CONSTRAINT DPA_CONFIG_STAMPA_CONSERVAZ_PK PRIMARY KEY
  (
    SYSTEM_ID
  )
  ENABLE
)';
		end if;
	end;
end;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
    where table_name='dpa_config_stampa_cons' and column_name='print_hour';

if cntcol = 0 then 
    execute immediate 
    'alter table dpa_config_stampa_cons add (print_hour number)';
end if;

end;
END;
/


begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_REGISTRO_CONSERVAZIONE';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_REGISTRO_CONSERVAZIONE
(
  SYSTEM_ID         INTEGER                     NOT NULL,
  VAR_COD_AZIONE    VARCHAR2(64 BYTE)           NOT NULL,
  VAR_DESC_AZIONE   VARCHAR2(256 BYTE),
  ID_ISTANZA        INTEGER                     NOT NULL,
  ID_OGGETTO        INTEGER,
  CHA_TIPO_OGGETTO  VARCHAR2(1 BYTE)            NOT NULL,
  USERID_OPERATORE  VARCHAR2(64 BYTE)           NOT NULL,
  CHA_ESITO         VARCHAR2(1 BYTE),
  ID_AMM            INTEGER                     NOT NULL,
  DTA_OPERAZIONE    DATE                        NOT NULL,
  CHA_TIPO_AZIONE   VARCHAR2(1 BYTE),
  CHA_PRINTED       VARCHAR2(1 BYTE)            DEFAULT 0
, CONSTRAINT DPA_REGISTRO_CONSERVAZIONE_PK PRIMARY KEY
  (
    SYSTEM_ID
  )
  ENABLE
)';
		end if;
	end;
end;
/


begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_STAMPA_CONSERVAZIONE';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_STAMPA_CONSERVAZIONE
(
  SYSTEM_ID               NUMBER                NOT NULL,
  ID_AMM                  NUMBER,
  PRINT_DATE              DATE,
  ID_PROFILE              NUMBER,
  ID_LAST_PRINTED_EVENT   NUMBER,
  ID_FIRST_PRINTED_EVENT  NUMBER
, CONSTRAINT DPA_STAMPA_CONSERVAZIONE_PK PRIMARY KEY
  (
    SYSTEM_ID
  )
  ENABLE
)';
		end if;
	end;
end;
/



BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ITEMS_CONSERVAZIONE' and column_name='VALIDAZIONE_MARCA';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ITEMS_CONSERVAZIONE ADD (VALIDAZIONE_MARCA CHAR(1 BYTE))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ITEMS_CONSERVAZIONE' and column_name='VALIDAZIONE_FORMATO';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ITEMS_CONSERVAZIONE ADD (VALIDAZIONE_FORMATO CHAR(1 BYTE))';
end if;

end;
END;
/

--ALTER TABLE DPA_ITEMS_CONSERVAZIONE
--ADD (VALIDAZIONE_MARCA CHAR(1 BYTE));

--ALTER TABLE DPA_ITEMS_CONSERVAZIONE
--ADD (VALIDAZIONE_FORMATO CHAR(1 BYTE));


Begin 
	Utl_Backup_Plsql_Code    ( 'FUNCTION', 'GetDateInADL');
end;
/


CREATE OR REPLACE FUNCTION GetDateInADL (sysID int, typeID char,idGruppo INT,idPeople INT)

RETURN date IS risultato date;


BEGIN --generale


begin --interno

IF (typeID = 'D') THEN
SELECT DISTINCT(DTA_INS) INTO risultato FROM DPA_AREA_LAVORO WHERE ID_PROFILE = sysID
AND (ID_PEOPLE =idPeople or ID_PEOPLE=0) and ID_RUOLO_IN_UO = (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = idGruppo);

END IF;
IF (typeID = 'F') THEN
SELECT DISTINCT(DTA_INS) INTO risultato FROM DPA_AREA_LAVORO WHERE ID_PROJECT = sysID
AND (ID_PEOPLE =idPeople or ID_PEOPLE=0) and ID_RUOLO_IN_UO = (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = idGruppo);

END IF;


EXCEPTION
WHEN NO_DATA_FOUND THEN risultato := '';
WHEN OTHERS THEN risultato := '';

end;

RETURN risultato;
END GetDateInADL;
/


create or replace
FUNCTION GETCOUNTRICEVUTEINTEROP(IDDOCUMENT VARCHAR, TIPORICEVUTA VARCHAR) RETURN VARCHAR IS
COUNTRAPPORTO VARCHAR(30);
BEGIN
DECLARE
numeratore NUMBER;
denominatore NUMBER;
begin
SELECT count(1) into denominatore 
from dpa_stato_invio a, documenttypes b 
where a.id_documenttype= b.system_id 
and a.id_profile= iddocument 
and b.type_id in ('INTEROPERABILITA','SIMPLIFIEDINTEROPERABILITY');
IF(tiporicevuta='1') THEN
select count (1) into numeratore FROM dpa_stato_invio a where a.id_profile= iddocument and var_proto_dest is not null;
end if;
if (tiporicevuta='2') THEN
select count (1) into numeratore FROM dpa_stato_invio a where a.id_profile= iddocument and cha_annullato='1';
end if;
if(tiporicevuta='3') THEN
select count (1) into numeratore FROM dpa_stato_invio a where a.id_profile= iddocument and status_c_mask like '_____X%';
end if;
if(tiporicevuta='1' or tiporicevuta='2' or tiporicevuta='3') THEN
countrapporto := (numeratore||' su '|| denominatore);
else
countrapporto := '';
end if;

end;
  RETURN countrapporto;
END GETCOUNTRICEVUTEINTEROP;


/


create or replace
FUNCTION GETESITOSPEDIZIONE(IDDOCUMENT in VARCHAR2)
RETURN VARCHAR2 IS ESITOSPEDIZIONE VARCHAR2(200);


BEGIN
declare
num_righeDSI number;
num_consegne number;
num_attese number;
num_ko NUMBER;

begin
select count(1) into num_righeDSI from dpa_stato_invio where id_profile= iddocument;
select count(1) into num_consegne from dpa_stato_invio where id_profile= iddocument and (status_c_mask like 'VV%' or status_c_mask like '_VV%');
SELECT COUNT(1) into num_attese FROM dpa_stato_invio WHERE ID_PROFILE = iddocument and (status_c_mask like 'AA%' or status_c_mask like '__A%');
SELECT COUNT(1) into num_ko FROM dpa_stato_invio WHERE ID_PROFILE = iddocument and (status_c_mask like 'XX%' or status_c_mask like '__X%');
if(num_righeDSI>0) THEN
  IF(num_righeDSI = num_consegne)
  THEN esitospedizione := 'Tutti i destinatari raggiunti';
  end if;
  IF((num_attese>0) and (num_ko=0)) THEN
  esitospedizione :='Almeno un destinatario in attesa di ricezione';
  end if;
  if(num_ko>0) THEN
  esitospedizione:='Almeno un destinatario non raggiunto';
  end if;
  end if;
  if(num_righeDSI <= 0 ) then
  esitospedizione := 'Documento non spedito';
  end if;
  end;
  return esitospedizione;
END GETESITOSPEDIZIONE;


/


create or replace FUNCTION GETSTATOINVIOMASKBYTYPEID
( DOCUMENTTYPEID IN VARCHAR2
) RETURN VARCHAR2 IS MASK VARCHAR2(16);
BEGIN
DECLARE 
DOCTYPE VARCHAR2(40);
BEGIN
SELECT TYPE_ID
INTO DOCTYPE
FROM DOCUMENTTYPES
WHERE system_id= documenttypeid;

IF(DOCTYPE='MAIL') THEN
MASK := 'AANNNNA';
END IF;

IF(DOCTYPE='INTEROPERABILITA') THEN
MASK := 'AAAAAAA';
END IF ;

IF(DOCTYPE='SIMPLIFIEDINTEROPERABILITY')THEN 
MASK := 'ANAAAAN';
END IF ;
  END;
  RETURN MASK;
END GETSTATOINVIOMASKBYTYPEID;

/

Begin 
Utl_Insert_Anagrafica_Log(
    'DIMENSIONE_INSTANZA' , --Codice               
    'Controlla che l''istanza rispetti i valori massimi', --Descrizione          
    'CONSERVAZIONE', --oggetto              
    'DIMENSIONE_INSTANZA', --Metodo               
    Null,     --Forza_Aggiornamento  
    '4.00',   -- Myversione_Cd         
    Null,      --RFU                  
	NULL, 		--multiplicity
	NULL,		--Id_amm
	'0',		--Notification 
	'0',		--configurable 
	NULL,		--notification_recipients 
    'BLUE');	--color
End;    
/

commit;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'PASS_ISTANZA_CHIUSA', 'Passaggio istanza in stato chiusa', 'CONSERVAZIONE', 'PASS_ISTANZA_CHIUSA', Null, '4.00', Null,	NULL, NULL,	'0', '0', NULL,	'BLUE');
End;    
/

commit;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'PASS_ISTANZA_CONSERVATA', 'Passaggio istanza in stato conservata', 'CONSERVAZIONE', 'PASS_ISTANZA_CONSERVATA', Null, '4.00', Null,	NULL, NULL,	'0', '0', NULL,	'BLUE');
End;    
/

commit;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'GENERAZIONE_FILE_CHIUSURA', 'Generazione del file di chiusura', 'CONSERVAZIONE', 'GENERAZIONE_FILE_CHIUSURA', Null, '4.00', Null,	NULL, NULL,	'0', '0', NULL,	'BLUE');
End;    
/

commit;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'VALIDAZIONE_ISTANZA_POLICY', 'Validazione conformità alla Policy', 'CONSERVAZIONE', 'VALIDAZIONE_ISTANZA_POLICY', Null, '4.00', Null,	NULL, NULL,	'0', '0', NULL,	'BLUE');
End;    
/

commit;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'LEGGIBILITA', 'Leggibilita verificata dell''istanza', 'CONSERVAZIONE', 'LEGGIBILITA', Null, '4.00', Null,	NULL, NULL,	'0', '0', NULL,	'BLUE');
End;    
/

commit;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'INTEGRITA_STORAGE', 'Verifica Integrità storage', 'CONSERVAZIONE', 'INTEGRITA_STORAGE', Null, '4.00', Null,	NULL, NULL,	'0', '0', NULL,	'BLUE');
End;    
/

commit;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'DOWNLOAD_ISTANZA', 'Esecuzione Download Istanza', 'CONSERVAZIONE', 'DOWNLOAD_ISTANZA', Null, '4.00', Null,	NULL, NULL,	'0', '0', NULL,	'BLUE');
End;    
/

commit;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'SFOGLIA_ISTANZA', 'Visualizzazione contenuti Istanza', 'CONSERVAZIONE', 'SFOGLIA_ISTANZA', Null, '4.00', Null,	NULL, NULL,	'0', '0', NULL,	'BLUE');
End;    
/

commit;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'LOGIN_CONSERVAZIONE', 'Login in Conservazione', 'CONSERVAZIONE', 'LOGIN_CONSERVAZIONE', Null, '4.00', Null,	NULL, NULL,	'0', '0', NULL,	'BLUE');
End;    
/

commit;
/


-- VELTRI - inserimento chiavi configurazione per nuova grafica

BEGIN
   utl_insert_chiave_microfunz
      ('DO_ADL_ROLE',
       'Abilita adl ruolo',
       NULL,
       'N',
       NULL,
       '4.00',
       NULL
      );
END;
/

BEGIN
   utl_insert_chiave_microfunz
      ('HOME_NOTES',
       'Abilita gli appunti nel centro notifiche',
       NULL,
       'N',
       NULL,
       '4.00',
       NULL
      );
END;
/ 

BEGIN
   utl_insert_chiave_microfunz
      ('HOME_EXPAND_ALL',
       'Notifiche espanse in automatico',
       NULL,
       'N',
       NULL,
       '4.00',
       NULL
      );
END;
/

begin
  Utl_Insert_Chiave_Config('FE_IDENTITY_CARD','La chiave abilita o meno la gestione della carta di identità'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end; 

  /
  
  
begin
  Utl_Insert_Chiave_Config('FE_RENDER_PDF','Permette la creazione di un file in pdf da posizione segnatura'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_REQ_CONV_PDF','Obbliga la conversione in pdf alla firma'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

BEGIN
    Utl_Insert_Chiave_Config('ENABLE_GEST_EXT_APPS'
    ,'Abilita o disabilita la gestione delle applicazioni esterne'
    ,'0','B','1','1','1','4.0',NULL,NULL,NULL);
end;
/
    
BEGIN
    Utl_Insert_Chiave_Config('BE_WEB_URL_DEPOSITO_SERVICE'
    ,'Indica la weburl esterna al db corrente'
    ,'http://localhost/DepositoService/Deposito.asmx','B','1','1','1','4.0',NULL,NULL,NULL);
    
end;
/

BEGIN
    Utl_Insert_Chiave_Config('BE_HAS_ARCHIVE'
    ,'Indica se esiste il modulo di deposito per questo Docspa'
    ,'0','B','1','1','1','4.0',NULL,NULL,NULL);
end;
/

BEGIN
    Utl_Insert_Chiave_Config('ARCHIVIO_DEPOSITO'
    ,'Archivio Deposito'
    ,'0','B','1','1','1','4.0',NULL,NULL,NULL);
end;
/

BEGIN
   utl_insert_chiave_microfunz
      ('FASC_STRUCTURE',
       'Abilita il tab struttura nel fascicolo',
       NULL,
       'N',
       NULL,
       '4.00',
       NULL
      );
END;
/


Begin 
Utl_Insert_Anagrafica_Log(
    'NO_DELIVERY_SEND_PEC', 'Notifica di non avvenuta consegna per spedizioni via PEC', 'DOCUMENTO', 'NO_DELIVERY_SEND_PEC', Null, '4.00', Null, 'ONE', NULL, '1', '1', 'NOTIFIABLE_ROLES_PEC4', 'RED');
End;    
/

Begin 
Utl_Insert_Anagrafica_Log(
    'EXCEPTION_INTEROPERABILITY_PEC', 'Eccezione interoperabilità PEC', 'DOCUMENTO', 'EXCEPTION_INTEROPERABILITY_PEC', Null, '4.00', Null, 'ONE', NULL, '1', '1', 'NOTIFIABLE_ROLES_PEC4', 'RED');
End;    
/

--insert into DPA_ANAGRAFICA_LOG
--(system_id, var_codice, var_descrizione, var_oggetto, var_metodo, multiplicity, id_amm, notification, configurable, notification_recipients, color)
--values (
--seq.nextval,
--'NO_DELIVERY_SEND_PEC',
--'Notifica di non avvenuta consegna per spedizioni via PEC',
--'DOCUMENTO',
--'NO_DELIVERY_SEND_PEC',
--'ONE',
--null,
--'1',
--'1',
--'NOTIFIABLE_ROLES_PEC4',
--'RED');

--/

--insert into DPA_ANAGRAFICA_LOG
--(system_id, var_codice, var_descrizione, var_oggetto, var_metodo, multiplicity, id_amm, notification, configurable, notification_recipients, color)
--values (
--seq.nextval,
--'EXCEPTION_INTEROPERABILITY_PEC',
--'Eccezione interoperabilità PEC',
--'DOCUMENTO',
--'EXCEPTION_INTEROPERABILITY_PEC',
--'ONE',
--null,
--'1',
--'1',
--'NOTIFIABLE_ROLES_PEC4',
--'RED');



 BEGIN
    utl_insert_chiave_microfunz
      ('GEST_REP_SPED',
       'Abilita il sottomenu'' Report Spedizione del menu'' Gestione',
       NULL,
       'N',
       NULL,
       '4.00',
       NULL
      );
      END;

/


CREATE OR REPLACE PROCEDURE i_smistamento_smistadoc (
   idpeoplemittente               IN     NUMBER,
   idcorrglobaleruolomittente     IN     NUMBER,
   idgruppomittente               IN     NUMBER,
   idamministrazionemittente      IN     NUMBER,
   idpeopledestinatario           IN     NUMBER,
   idcorrglobaledestinatario      IN     NUMBER,
   iddocumento                    IN     NUMBER,
   idtrasmissione                 IN     NUMBER,
   idtrasmissioneutentemittente   IN     NUMBER,
   trasmissioneconworkflow        IN     CHAR,
   notegeneralidocumento          IN     VARCHAR2,
   noteindividuali                IN     VARCHAR2,
   datascadenza                   IN     DATE,
   tipodiritto                    IN     CHAR,
   rights                         IN     NUMBER,
   originalrights                 IN     NUMBER,
   idragionetrasm                 IN     NUMBER,
   idpeopledelegato               IN     NUMBER,
    nonotify                     IN NUMBER,
   returnvalue                       OUT NUMBER)
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
   identitytrasm         NUMBER := NULL;
   identitytrasmsing     NUMBER := NULL;
   existaccessrights     CHAR (1) := 'Y';
   accessrights          NUMBER := NULL;
   accessrightsvalue     NUMBER := NULL;
   tipotrasmsingola      CHAR (1) := NULL;
   isaccettata           VARCHAR2 (1) := '0';
   isaccettatadelegato   VARCHAR2 (1) := '0';
   isvista               VARCHAR2 (1) := '0';
   isvistadelegato       VARCHAR2 (1) := '0';
   meth                VARchar2(256);
   descr                  VARchar2(2000);
   descrOgg                  VARchar2(2000);
   producer              VARchar2(2000);
   ragione               varchar2(32);
   idorSegn                 varchar2(256);
   idtrasmsingolamitt   NUMBER := 0;
   resultvalue           NUMBER;
BEGIN
   BEGIN
      SELECT seq.NEXTVAL INTO identitytrasm FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL INTO identitytrasmsing FROM DUAL;
   END;

   BEGIN
      /*Inserimento in tabella DPA_TRASMISSIONE */
      IF (idpeopledelegato > 0)
      THEN
         INSERT INTO dpa_trasmissione (system_id,
                                       id_ruolo_in_uo,
                                       id_people,
                                       cha_tipo_oggetto,
                                       id_profile,
                                       id_project,
                                       dta_invio,
                                       var_note_generali,
                                       id_people_delegato)
              VALUES (identitytrasm,
                      idcorrglobaleruolomittente,
                      idpeoplemittente,
                      'D',
                      iddocumento,
                      NULL,
                      SYSDATE (),
                      notegeneralidocumento,
                      idpeopledelegato);
      ELSE
         INSERT INTO dpa_trasmissione (system_id,
                                       id_ruolo_in_uo,
                                       id_people,
                                       cha_tipo_oggetto,
                                       id_profile,
                                       id_project,
                                       dta_invio,
                                       var_note_generali)
              VALUES (identitytrasm,
                      idcorrglobaleruolomittente,
                      idpeoplemittente,
                      'D',
                      iddocumento,
                      NULL,
                      SYSDATE (),
                      notegeneralidocumento);
      END IF;
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
      /* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola (system_id,
                                     id_ragione,
                                     id_trasmissione,
                                     cha_tipo_dest,
                                     id_corr_globale,
                                     var_note_sing,
                                     cha_tipo_trasm,
                                     dta_scadenza,
                                     id_trasm_utente)
           VALUES (identitytrasmsing,
                   idragionetrasm,
                   identitytrasm,
                   'U',
                   idcorrglobaledestinatario,
                   noteindividuali,
                   'S',
                   datascadenza,
                   NULL);
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

   BEGIN
      /* Inserimento in tabella DPA_TRASM_UTENTE */
      INSERT INTO dpa_trasm_utente (system_id,
                                    id_trasm_singola,
                                    id_people,
                                    dta_vista,
                                    dta_accettata,
                                    dta_rifiutata,
                                    dta_risposta,
                                    cha_vista,
                                    cha_accettata,
                                    cha_rifiutata,
                                    var_note_acc,
                                    var_note_rif,
                                    cha_valida,
                                    id_trasm_risp_sing)
           VALUES (seq.NEXTVAL,
                   identitytrasmsing,
                   idpeopledestinatario,
                   NULL,
                   NULL,
                   NULL,
                   NULL,
                   '0',
                   '0',
                   '0',
                   NULL,
                   NULL,
                   '1',
                   NULL);
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -4;
         RETURN;
   END;

BEGIN
    IF nonotify < 1 THEN
        BEGIN
            --per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio
            UPDATE dpa_trasmissione
            SET dta_invio   = SYSDATE ()
            WHERE system_id = identitytrasm;
            SELECT accessrights
            INTO accessrights
            FROM
            (SELECT accessrights
            FROM security
            WHERE thing       = iddocumento
            AND personorgroup = idpeopledestinatario
            )
            WHERE ROWNUM = 1;
        EXCEPTION
        WHEN NO_DATA_FOUND THEN
            existaccessrights := 'N';
        END;
    END IF;
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
               WHERE     thing = iddocumento
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
               SET accessrights = rights, cha_tipo_diritto = 'T'
             WHERE     thing = iddocumento
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
         INSERT INTO security (thing,
                               personorgroup,
                               accessrights,
                               id_gruppo_trasm,
                               cha_tipo_diritto)
              VALUES (iddocumento,
                      idpeopledestinatario,
                      rights,
                      idgruppomittente,
                      tipodiritto);
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

--  CENTRO_NOTIFICHE_1 
/* Formatted on 21/06/2013 15:32:27 (QP5 v5.215.12089.38647) */
BEGIN

   SELECT NVL (var_desc_ragione, ' ')
     INTO ragione
     FROM dpa_ragione_trasm
    WHERE system_id = idragionetrasm;

   SELECT NVL (docname, ' ')
     INTO idorSegn
     FROM profile
    WHERE system_id = iddocumento;

   meth := 'TRASM_DOC_' || REPLACE (UPPER (ragione), ' ', '_');

   descrOgg := 'Trasmesso Documento ID: ' || idorSegn;
   
  descr:= 'Effettuata trasmissione documento con ragione '|| REPLACE (UPPER (ragione), ' ', '_');

   producer := getpeoplename (idpeoplemittente);

   IF (getdesccorr (idcorrglobaleruolomittente) IS NOT NULL)
   THEN
      producer :=
            producer
         || ' ('
         || REPLACE (getdesccorr (idcorrglobaleruolomittente), ''',''''')
         || ')';
   END IF;


   INSERT INTO dpa_log (SYSTEM_ID,
                        USERID_OPERATORE,
                        ID_PEOPLE_OPERATORE,
                        ID_GRUPPO_OPERATORE,
                        ID_AMM,
                        DTA_AZIONE,
                        VAR_OGGETTO,
                        ID_OGGETTO,
                        VAR_DESC_OGGETTO,
                        VAR_COD_AZIONE,
                        CHA_ESITO,
                        VAR_DESC_AZIONE,
                        VAR_COD_WORKING_APPLICATION,
                        ID_TRASM_SINGOLA,
                        CHECK_NOTIFY,
                        DESC_PRODUCER)
        VALUES (seq.nextval,
                getpeopleuserid(idpeoplemittente),
                idpeoplemittente,
                idcorrglobaleruolomittente,
                idamministrazionemittente,
                SYSDATE,
                'DOCUMENTO',
                iddocumento,
                descrOgg, 
                meth,               
                '1',
               descr,
                '',
                identitytrasmsing,
                '1',
                producer);
EXCEPTION
   WHEN OTHERS
   THEN
      returnvalue := -4;
END;
   
 
   /* END Gestione Centro Notifiche */

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
                                   WHEN dta_vista IS NULL THEN SYSDATE
                                   ELSE dta_vista
                                END),
                            cha_vista =
                               (CASE WHEN dta_vista IS NULL THEN 1 ELSE 0 END),
                            cha_vista_delegato = isvistadelegato,
                            cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE     tu.id_people =
                                                  idpeoplemittente
                                           AND tx.system_id =
                                                  ts.id_trasmissione
                                           AND tx.system_id = idtrasmissione
                                           AND ts.system_id =
                                                  tu.id_trasm_singola
                                           AND ts.cha_tipo_dest = 'U'));
-- devo impostare come vista la notifica.. come si fa ? Chiedo a Manu.
  UPDATE dpa_notify a
                        SET a.read_notification = '1'
                      WHERE id_specialized_object = idtrasmsingolamitt
                        AND id_people_receiver = idpeoplemittente
                        AND (   id_group_receiver = idcorrglobaleruolomittente
                             OR id_group_receiver = 0
                            );
                  END;
               ELSE
                  BEGIN
                     -- l'oggetto trasmesso visto,
                     -- pertanto la trasmissione viene solo rimossa dalla todolist
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0', cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE     tu.id_people =
                                                  idpeoplemittente
                                           AND tx.system_id =
                                                  ts.id_trasmissione
                                           AND tx.system_id = idtrasmissione
                                           AND ts.system_id =
                                                  tu.id_trasm_singola
                                           AND ts.cha_tipo_dest = 'U'));

                                           
-- START GESTIONE CENTRO NOTIFICHE
select tuu.id_trasm_singola into idtrasmsingolamitt from dpa_trasm_utente tuu where system_id=idtrasmissioneutentemittente;

  -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
           
-- END GESTIONE CENTRO NOTIFICHE
                  END;
               END IF;
            END;
         ELSE
            -- la trasmissione ancora non risulta accettata, pertanto:
            -- 1) viene accettata implicitamente,
            -- 2) l'oggetto trasmesso impostato come visto,
            -- 3) la trasmissione rimossa la trasmissione da todolist
            BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL THEN SYSDATE
                             ELSE dta_vista
                          END),
                      cha_vista =
                         (CASE WHEN dta_vista IS NULL THEN 1 ELSE 0 END),
                      cha_vista_delegato = isvistadelegato,
                      dta_accettata = SYSDATE (),
                      cha_accettata = '1',
                      cha_accettata_delegato = isaccettatadelegato,
                      var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0',
                      id_people_delegato = idpeopledelegato
                WHERE     (   system_id = idtrasmissioneutentemittente
                           OR system_id =
                                 (SELECT tu.system_id
                                    FROM dpa_trasm_utente tu,
                                         dpa_trasmissione tx,
                                         dpa_trasm_singola ts
                                   WHERE     tu.id_people = idpeoplemittente
                                         AND tx.system_id =
                                                ts.id_trasmissione
                                         AND tx.system_id = idtrasmissione
                                         AND ts.system_id =
                                                tu.id_trasm_singola
                                         AND ts.cha_tipo_dest = 'U'))
                      AND cha_valida = '1';
-- START GESTIONE CENTRO NOTIFICHE
select tuu.id_trasm_singola into idtrasmsingolamitt from dpa_trasm_utente tuu where system_id=idtrasmissioneutentemittente;


  -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
           


-- END GESTIONE CENTRO NOTIFICHE
            END;
         END IF;

         --update security se diritti  trasmssione in accettazione =20
         BEGIN
            UPDATE security s
               SET s.accessrights = originalrights, s.cha_tipo_diritto = 'T'
             WHERE     s.thing = iddocumento
                   AND s.personorgroup IN
                          (idpeoplemittente, idgruppomittente)
                   AND s.accessrights = 20;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue);

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
        FROM (  SELECT a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE     a.system_id = b.id_trasm_singola
                       AND b.system_id IN
                              (SELECT tu.system_id
                                 FROM dpa_trasm_utente tu,
                                      dpa_trasmissione tx,
                                      dpa_trasm_singola ts
                                WHERE     tu.id_people = idpeoplemittente
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
            SET cha_valida = '0', cha_in_todolist = '0'
          WHERE     id_trasm_singola IN
                       (SELECT a.system_id
                          FROM dpa_trasm_singola a, dpa_trasm_utente b
                        WHERE     a.system_id = b.id_trasm_singola
                               AND b.system_id IN
                                      (SELECT tu.system_id
                                         FROM dpa_trasm_utente tu,
                                              dpa_trasmissione tx,
                                              dpa_trasm_singola ts
                                        WHERE     tu.id_people =
                                                     idpeoplemittente
                                              AND tx.system_id =
                                                     ts.id_trasmissione
                                              AND tx.system_id =
                                                     idtrasmissione
                                              AND ts.system_id =
                                                     tu.id_trasm_singola
                                              AND ts.system_id =
                                                     (SELECT id_trasm_singola
                                                        FROM dpa_trasm_utente
                                                       WHERE system_id =
                                                                idtrasmissioneutentemittente)))
                AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;

   
   --ovvero solo se io sono tra i notificati!!!
                        -- elimino la notifica a tutti gli utenti del ruolo
                        --copio la notifica nello storico
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt;
                     
   
   returnvalue := 0;
END;
/


/* Formatted on 2013/06/24 15:04 (Formatter Plus v4.8.8) */
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
   identitytrasm          NUMBER          := NULL;
   systrasmsing           NUMBER          := NULL;
   existaccessrights      CHAR (1)        := 'Y';
   accessrights           NUMBER          := NULL;
   accessrightsvalue      NUMBER          := NULL;
   idutente               NUMBER;
   recordcorrente         NUMBER;
   idgroups               NUMBER          := NULL;
   idgruppo               NUMBER;
   resultvalue            NUMBER;
   tipotrasmsingola       CHAR (1)        := NULL;
   isaccettata            VARCHAR2 (1)    := '0';
   isaccettatadelegato    VARCHAR2 (1)    := '0';
   isvista                VARCHAR2 (1)    := '0';
   isvistadelegato        VARCHAR2 (1)    := '0';
   val_idpeopledelegato   NUMBER;
   tiporag                VARCHAR2 (1);
   meth                   VARCHAR2 (256);
   descr                  VARCHAR2 (2000);
   descrogg               VARCHAR2 (2000);
   producer               VARCHAR2 (2000);
   ragione                VARCHAR2 (32);
   idorsegn               VARCHAR2 (256);
   idtrasmsingolamitt     NUMBER          := 0;
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
-- inizio modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega
/* Inserimento in tabella DPA_TRASMISSIONE */

      -- la procedura riceve in input il valore = 0 per il campo idpeopledelegato, in caso di senza delega
-- e il valore = 1 in caso di delega
-- nel primo caso si vuole avere comunque il valore NULL nel campo idpeopledelegato
      IF (idpeopledelegato > 0)
      THEN
         val_idpeopledelegato := idpeopledelegato;
      ELSE
         val_idpeopledelegato := NULL;
      END IF;

      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali, id_people_delegato
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE (), notegeneralidocumento, val_idpeopledelegato
                  );
-- precedente era
/*
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE , notegeneralidocumento
                  );
*/
-- fine modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega
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
               SET accessrights = rights,
                   cha_tipo_diritto = 'T'
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

--  CENTRO_NOTIFICHE_1
   SELECT tuu.id_trasm_singola
     INTO idtrasmsingolamitt
     FROM dpa_trasm_utente tuu
    WHERE system_id = idtrasmissioneutentemittente;

   BEGIN
      SELECT NVL (var_desc_ragione, ' ')
        INTO ragione
        FROM dpa_ragione_trasm
       WHERE system_id = idragionetrasm;

      SELECT NVL (docname, ' ')
        INTO idorsegn
        FROM PROFILE
       WHERE system_id = iddocumento;

      meth := 'TRASM_DOC_' || REPLACE (UPPER (ragione), ' ', '_');
      descrogg := 'Trasmesso Documento ID: ' || idorsegn;
      descr :=
            'Effettuata trasmissione documento con ragione '
         || REPLACE (UPPER (ragione), ' ', '_');
      producer := getpeoplename (idpeoplemittente);

      IF (getdesccorr (idcorrglobaleruolomittente) IS NOT NULL)
      THEN
         producer :=
               producer
            || ' ('
            || REPLACE (getdesccorr (idcorrglobaleruolomittente), ''',''''')
            || ')';
      END IF;

      INSERT INTO dpa_log
                  (system_id, userid_operatore,
                   id_people_operatore, id_gruppo_operatore,
                   id_amm, dta_azione, var_oggetto,
                   id_oggetto, var_desc_oggetto, var_cod_azione, cha_esito,
                   var_desc_azione, var_cod_working_application,
                   id_trasm_singola, check_notify, desc_producer
                  )
           VALUES (seq.NEXTVAL, getpeopleuserid (idpeoplemittente),
                   idpeoplemittente, idcorrglobaleruolomittente,
                   idamministrazionemittente, SYSDATE, 'DOCUMENTO',
                   iddocumento, descrogg, meth, '1',
                   descr, '',
                   systrasmsing, '1', producer
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -4;
   END;

   /* END Gestione Centro Notifiche */
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

         SELECT cha_tipo_ragione
           INTO tiporag
           FROM dpa_ragione_trasm rs,
                dpa_trasm_singola ts,
                dpa_trasm_utente tsu
          WHERE tsu.system_id = idtrasmissioneutentemittente
            AND ts.system_id = tsu.id_trasm_singola
            AND rs.system_id = ts.id_ragione;

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

                     -- devo impostare come vista la notifica.. come si fa ? Chiedo a Manu.
                     UPDATE dpa_notify a
                        SET a.read_notification = '1'
                      WHERE id_specialized_object = idtrasmsingolamitt
                        AND id_people_receiver = idpeoplemittente
                        AND (   id_group_receiver = idcorrglobaleruolomittente
                             OR id_group_receiver = 0
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

                               -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                     --copio la notifica nello storico
                     INSERT INTO dpa_notify_history
                                 (system_id, id_notify, id_event,
                                  desc_producer, id_people_receiver,
                                  id_group_receiver, type_notify, dta_notify,
                                  field_1, field_2, field_3, field_4,
                                  multiplicity, specialized_field, type_event,
                                  domainobject, id_object,
                                  id_specialized_object, dta_event,
                                  read_notification)
                        SELECT seq.NEXTVAL, system_id, id_event,
                               desc_producer, id_people_receiver,
                               id_group_receiver, type_notify, dta_notify,
                               field_1, field_2, field_3, field_4,
                               multiplicity, specialized_field, type_event,
                               domainobject, id_object, id_specialized_object,
                               dta_event, read_notification
                          FROM dpa_notify
                         WHERE id_specialized_object = idtrasmsingolamitt
                           AND id_people_receiver = idpeoplemittente
                           AND (   id_group_receiver =
                                                    idcorrglobaleruolomittente
                                OR id_group_receiver = 0
                               );

                     -- elimino la ntoifica
                     DELETE FROM dpa_notify
                           WHERE id_specialized_object = idtrasmsingolamitt
                             AND id_people_receiver = idpeoplemittente
                             AND (   id_group_receiver =
                                                    idcorrglobaleruolomittente
                                  OR id_group_receiver = 0
                                 );
-- END GESTIONE CENTRO NOTIFICHE
                  END;
               END IF;
            END;
         ELSE
            BEGIN
-- la trasmissione ancora non risulta accettata, pertanto:
-- 1) viene accettata implicitamente,
-- 2) l'oggetto trasmesso impostato come visto,
-- 3) la trasmissione rimossa la trasmissione da todolist
               IF (tiporag = 'W')
               THEN
                  BEGIN
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
               ELSE                                              --no workflow
                  BEGIN
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
                                       AND ts.cha_tipo_dest = 'U'
                                       AND cha_valida = '1')
                            );
                  END;

-- START GESTIONE CENTRO NOTIFICHE

                  -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                                    --copio la notifica nello storico
                  INSERT INTO dpa_notify_history
                              (system_id, id_notify, id_event, desc_producer,
                               id_people_receiver, id_group_receiver,
                               type_notify, dta_notify, field_1, field_2,
                               field_3, field_4, multiplicity,
                               specialized_field, type_event, domainobject,
                               id_object, id_specialized_object, dta_event,
                               read_notification)
                     SELECT seq.NEXTVAL, system_id, id_event, desc_producer,
                            id_people_receiver, id_group_receiver,
                            type_notify, dta_notify, field_1, field_2,
                            field_3, field_4, multiplicity, specialized_field,
                            type_event, domainobject, id_object,
                            id_specialized_object, dta_event,
                            read_notification
                       FROM dpa_notify
                      WHERE id_specialized_object = idtrasmsingolamitt
                        AND id_people_receiver = idpeoplemittente
                        AND (   id_group_receiver = idcorrglobaleruolomittente
                             OR id_group_receiver = 0
                            );

                  -- elimino la ntoifica
                  DELETE FROM dpa_notify
                        WHERE id_specialized_object = idtrasmsingolamitt
                          AND id_people_receiver = idpeoplemittente
                          AND (   id_group_receiver =
                                                    idcorrglobaleruolomittente
                               OR id_group_receiver = 0
                              );
-- END GESTIONE CENTRO NOTIFICHE
               END IF;
            END;
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
            -- visibilit gi esistente, ignora e continua con eventuali altri inserimenti
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

   --ovvero solo se io sono tra i notificati!!!
                        -- elimino la notifica a tutti gli utenti del ruolo
                        --copio la notifica nello storico
   INSERT INTO dpa_notify_history
               (system_id, id_notify, id_event, desc_producer,
                id_people_receiver, id_group_receiver, type_notify,
                dta_notify, field_1, field_2, field_3, field_4, multiplicity,
                specialized_field, type_event, domainobject, id_object,
                id_specialized_object, dta_event, read_notification)
      SELECT seq.NEXTVAL, system_id, id_event, desc_producer,
             id_people_receiver, id_group_receiver, type_notify, dta_notify,
             field_1, field_2, field_3, field_4, multiplicity,
             specialized_field, type_event, domainobject, id_object,
             id_specialized_object, dta_event, read_notification
        FROM dpa_notify
       WHERE id_specialized_object = idtrasmsingolamitt;

   -- elimino la notifica
   DELETE FROM dpa_notify
         WHERE id_specialized_object = idtrasmsingolamitt;
END;
/

begin
	execute immediate 
	'update dpa_chiavi_configurazione set cha_conservazione = 1 where upper(var_codice) like ''%CONSERVAZIONE%''';
end;
/


CREATE OR REPLACE PROCEDURE dpa3_get_hierarchy (
p_id_amm in varchar,
p_cod in varchar,
p_tipo_ie in varchar,
p_id_Ruolo in varchar,
p_codes out varchar)

as

begin
declare
v_c_type varchar(2);
v_p_cod varchar(64);
v_system_id int;
v_id_parent int;
v_id_uo int;
v_id_utente int;

begin

p_codes := '';
v_p_cod := p_cod;

select
id_parent, system_id, id_uo, id_people, cha_tipo_urp
into
v_id_parent, v_system_id, v_id_uo, v_id_utente, v_c_type
from
dpa_corr_globali
where
var_cod_rubrica = v_p_cod and
cha_tipo_ie = p_tipo_ie and
id_amm = p_id_amm and
dta_fine is null and cha_tipo_ie='I' ;


while (1 > 0) loop
if v_c_type = 'U' then
if (v_id_parent is null or v_id_parent = 0) then
exit;
end if;

select var_cod_rubrica, system_id into v_p_cod, v_system_id from dpa_corr_globali where system_id = v_id_parent and id_amm = p_id_amm and dta_fine is null;
end if;

if v_c_type = 'R' then
if (v_id_uo is null or v_id_uo = 0) then
exit;
end if;

select var_cod_rubrica, system_id into v_p_cod, v_system_id from dpa_corr_globali where system_id = v_id_uo and id_amm = p_id_amm and dta_fine is null ;
end if;

if v_c_type = 'P' then
select var_cod_rubrica into v_p_cod from dpa_corr_globali where id_gruppo = p_id_Ruolo
and id_amm = p_id_amm and dta_fine is null;
end if;
if v_p_cod is null then
exit;
end if;

select
id_parent, system_id, id_uo, cha_tipo_urp
into
v_id_parent, v_system_id, v_id_uo, v_c_type
from
dpa_corr_globali
where
var_cod_rubrica = v_p_cod and
id_amm = p_id_amm and
dta_fine is null and cha_tipo_ie='I';

p_codes := v_p_cod || ':' || p_codes;
end loop;

p_codes := p_codes || p_cod;

end;
end;
/

create or replace
FUNCTION getContatoreDoc2 (docNumber INT, tipoContatore CHAR, oggettoCustomId INT)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
repertorio NUMBER;
valoreSottocontatore VARCHAR(255);
tipoContatoreSottocontatore VARCHAR(255);

BEGIN

valoreContatore := '';
annoContatore := '';
codiceRegRf := '';
valoreSottocontatore := '';
tipoContatoreSottocontatore := '';


begin

select
valore_oggetto_db, anno, repertorio, valore_sc, dpa_tipo_oggetto.descrizione
into valoreContatore, annoContatore, repertorio, valoreSottocontatore, tipoContatoreSottocontatore
from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
where
dpa_associazione_templates.doc_number = to_char(docNumber)
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
(dpa_tipo_oggetto.descrizione = 'Contatore' OR dpa_tipo_oggetto.descrizione = 'ContatoreSottocontatore')
and
dpa_oggetti_custom.system_id= oggettoCustomId;
--dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';
--and
--dpa_oggetti_custom.cha_tipo_tar = 'T';
exception when others then null;

end;

IF(repertorio = 1) THEN
BEGIN
risultato := '#CONTATORE_DI_REPERTORIO#';
RETURN risultato;
END;
END IF;

IF(tipoContatore<>'T') THEN
BEGIN
select
dpa_el_registri.var_codice
into codiceRegRf
from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri
where
dpa_associazione_templates.doc_number = to_char(docNumber)
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
(dpa_tipo_oggetto.descrizione = 'Contatore' OR dpa_tipo_oggetto.descrizione = 'ContatoreSottocontatore')
and
dpa_oggetti_custom.system_id= oggettoCustomId
--dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1'
--and
--dpa_oggetti_custom.cha_tipo_tar = tipoContatore;
and
dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
exception when others then null;

END;
END IF;

if(tipoContatoreSottocontatore = 'Contatore') then
  if(codiceRegRf is  null) then 
    risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
  else  
    risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
  end if; 
end if;

if(tipoContatoreSottocontatore = 'ContatoreSottocontatore') then
  if(valoreContatore is not null AND valoreSottocontatore is not null) then
    risultato :=    nvl(valoreContatore,'')||'-'||nvl(valoreSottocontatore,'') ;
  end if;
end if;


RETURN risultato;
End Getcontatoredoc2;
/

Begin 
Utl_Insert_Anagrafica_Log(
    'VERIFICA_INT_CONTENUTO_FILE_AC', 'Verifica di conformità del contenuto dei file rispetto ai formati in area conservazione', 'AREA_CONSERVAZIONE', 'VERIFICA_INT_CONTENUTO_FILE_AC', Null, '4.00', Null, 'ONE', NULL, '0', '0', '', 'BLUE');
End;    
/

 Begin 
Utl_Insert_Anagrafica_Log(
    'VERIFICA_INT_FORMATO_FILE_AC', 'Verifica di conformità del formato dei file rispetto ai formati in area conservazione', 'AREA_CONSERVAZIONE', 'VERIFICA_INT_FORMATO_FILE_AC', Null, '4.00', Null, 'ONE', NULL, '0', '0', '', 'BLUE');
End;    
/

BEGIN
--create index idx_PROJECT_K39 on PROJECT(ID_TIPO_FASC)

 utl_add_index('4.00','DOCSPACOLL30','DPA_CORR_GLOBALI',
    'indx_corr_glob_uo',null,
    '(ID_UO, CHA_RIFERIMENTO)',null,null,null,
    'NORMAL', null,null,null);
END;
/


--------------------------------------------------------
--  File creato - martedì-maggio-14-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Package Body UTILITA
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY "UTILITA" as 

Procedure             Utl_Insert_Log_W_Autcommit (Nomeutente Varchar2,
 data_eseguito DATE,
 comando_eseguito VARCHAR2,
 CATEGORIA_COMANDO VARCHAR2,
 esito VARCHAR2)

Is
cnt int;
MYUSERNAME VARCHAR2(200);
UTL_LOG_VARCHAR_DATA_LENGTH INT; 
PRAGMA AUTONOMOUS_TRANSACTION ;

BEGIN
select USERNAME INTO MYUSERNAME from user_users;

SELECT COUNT(*) INTO cnt FROM user_sequences where sequence_name='SEQ_UTL_SYSTEM_LOG';
If (Cnt = 0) Then
        Execute Immediate 'CREATE SEQUENCE SEQ_UTL_SYSTEM_LOG '||
                        'START WITH 1                       '||
                        ' MAXVALUE 99999999999     MINVALUE 1  '||
                        ' NOCYCLE   NOCACHE   NOORDER';
end IF;

SELECT COUNT(*) INTO cnt FROM user_tables where table_name='UTL_SYSTEM_LOG';
IF (cnt = 0) THEN
    RAISE_APPLICATION_ERROR(-20001,'Missing table '||MYUSERNAME||'.UTL_SYSTEM_LOG');
end IF;

select min(data_length) into UTL_LOG_VARCHAR_DATA_LENGTH 
    from cols 
    where table_name = 'UTL_SYSTEM_LOG'
    and column_name in ('COMANDO_RICHIESTO', 'CATEGORIA_COMANDO', 'ESITO_OPERAZIONE') ;  

IF UTL_LOG_VARCHAR_DATA_LENGTH < 2000 THEN 
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY COMANDO_RICHIESTO VARCHAR2(2000) '; 
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY CATEGORIA_COMANDO VARCHAR2(2000) ';
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY ESITO_OPERAZIONE  VARCHAR2(2000) ';
end if; 

 INSERT INTO UTL_SYSTEM_LOG
 ( ID , DATA_OPERAZIONE
  , COMANDO_RICHIESTO
  , CATEGORIA_COMANDO
  , ESITO_OPERAZIONE   )
 VALUES ( SEQ_UTL_SYSTEM_LOG.nextval, nvl(data_eseguito, SYSTIMESTAMP)
 , substr(comando_eseguito  ,1,2000)
 , substr(CATEGORIA_COMANDO ,1,2000)
 , substr(esito             ,1,2000)  ) ;

 commit; -- si può fare perché c'è PRAGMA AUTONOMOUS_TRANSACTION prima in DECLARE
EXCEPTION
 WHEN OTHERS THEN
 DBMS_OUTPUT.put_line ('errore generico in insert log' || SQLERRM);
 RAISE; --manda errore a sp chiamante
End Utl_Insert_Log_W_Autcommit ;


PROCEDURE UTL_STORICIZZA_LOG (interval_of_days int) IS
Pragma Autonomous_Transaction ;
idx_esistente   EXCEPTION;
Pragma Exception_Init (Idx_Esistente  , -955);
-- ORA-01408 esiste già un indice per questa lista di colonne
Idx_Esistente_su_colonne   Exception;
Pragma Exception_Init (Idx_Esistente_su_colonne  , -01408);

Myusername Varchar2(200);
istruz_sql Varchar2(200);
Is_Compression_Enabled Varchar2(200);
Stringa_Msg   Varchar2(2000);
Min_Dta_Azione_Log         Date; 
Max_Dta_Azione_log_Storico Date; 
dummy varchar2(32); 
Btimestamp Timestamp; secs_elapsed number(8,1);
BEGIN
Select Username Into Myusername From User_Users;
--Select Systimestamp Into Btimestamp From Dual ;

Select 'Prima di aver iniziato esecuzione della procedura, alle '||Systimestamp||', la tabella STORICO ha min dta_azione:'
    ||To_Char(Min_Dta_Azione_S,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||To_Char(Max_Dta_Azione_S,'dd/mm/yyyy hh24:mi:ss')     
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_S From Dpa_Log_Storico)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_S From Dpa_Log_Storico)   ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

Select 'Prima di aver iniziato esecuzione della procedura, alle '||Systimestamp||', la tabella DPA_LOG ha min dta_azione:'
    ||to_char(Min_Dta_Azione_L,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione_L,'dd/mm/yyyy hh24:mi:ss') 
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_L From Dpa_Log)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_L From Dpa_Log) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

select compression into is_compression_enabled
    from user_tables 
    where table_name = 'DPA_LOG_STORICO' ;

if is_compression_enabled = 'DISABLED' THEN 
-- il comando abilita solo la compressione per i nuovi record, non la effettua su record esistenti
-- se si volesse comprimere i record esistenti, eseguire alter table DPA_LOG_STORICO move compress; 
    Istruz_Sql := 'alter table '||MYUSERNAME||'.DPA_LOG_STORICO compress storage (buffer_pool recycle)'  ;
    Execute Immediate  Istruz_Sql ;
    utl_insert_log_w_autcommit (      MYUSERNAME        ,Sysdate    
      ,Istruz_Sql --comando_eseguito VARCHAR2,
      ,'procedura UTL_STORICIZZA_LOG, package UTILITA'         --CATEGORIA_COMANDO VARCHAR2,
      ,'abilitata compressione per tabella '||MYUSERNAME||'.DPA_LOG_STORICO' 
      ) ; 
End If; 
  
-- create indexes INDX_DTA_AZIONE on DPA_LOG(DTA_AZIONE) and DPA_LOG_STORICO(DTA_AZIONE) if do not exist
   Begin
      Istruz_Sql := 'create index INDX_DTA_AZIONE on DPA_LOG (DTA_AZIONE) compress storage (buffer_pool recycle)';
      EXECUTE IMMEDIATE  istruz_sql ;
      utl_insert_log_w_autcommit (
            Myusername        --nomeutente VARCHAR2,
            ,Sysdate          --data_eseguito DATE,
            ,Istruz_Sql       --comando_eseguito VARCHAR2,
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'         --CATEGORIA_COMANDO VARCHAR2,
            ,'OK'  --esito VARCHAR2
            ) ;
   EXCEPTION
      WHEN idx_esistente       THEN
         Dbms_Output.Put_Line ('indice già esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice già esistente');
      When others then 
            utl_insert_log_w_autcommit ( Myusername,Sysdate ,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'            ,'KO!'              ) ;
            RAISE;
   End;
-- now with DPA_LOG_STORICO   
   Begin
      Istruz_Sql := 'create index INDX_DTA_AZIONE_STORICO on DPA_LOG_STORICO(DTA_AZIONE) compress storage (buffer_pool recycle)';
      EXECUTE IMMEDIATE  istruz_sql ;
      utl_insert_log_w_autcommit (Myusername,Sysdate,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'       ,'OK') ;
   EXCEPTION
      WHEN idx_esistente       THEN
         Dbms_Output.Put_Line ('indice già esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice già esistente');
      
      When others then 
           utl_insert_log_w_autcommit ( Myusername,Sysdate ,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'            ,'KO!'              ) ;
             Raise;
   End;
-- end of create indexes on DPA_LOG(DTA_AZIONE) and DPA_LOG_STORICO(DTA_AZIONE) if do not exist

select min(dta_azione) into min_dta_azione_log
    From Dpa_Log;  
Select Max(Dta_Azione) Into Max_Dta_Azione_log_storico
    From Dpa_Log_storico;  

If Min_Dta_Azione_Log < Nvl(Max_Dta_Azione_Log_Storico, To_Date('01-01-1970','dd-mm-yyyy') ) Then
  -- N8OT OK !
  Utl_Insert_Log_W_Autcommit (Myusername  ,Sysdate    , 'procedura interrotta '        
    ,'invocata procedura UTL_STORICIZZA_LOG, in package UTILITA'
    ,'Bonificare DPA_LOG_STORICO! min(dta_azione) non è maggiore o uguale a Max(Dta_Azione_storico) '    ) ; 

Else  -- ok, Min_Dta_Azione_Log è maggiore o uguale a nvl(Max_Dta_Azione_log_storico, to_date('01-01-1970','dd-mm-yyyy') )

-- since dpa_log_storico has STORAGE COMPRESSION enabled, append hint will now store rows in compressed form
  INSERT /*+ append */ INTO dpa_log_storico (
      SYSTEM_ID,         USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                    ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE, DESC_PRODUCER)
  SELECT SYSTEM_ID,    USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                   ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE, DESC_PRODUCER
    From Dpa_Log
        Where (Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days)) and check_notify = '0';  
  
  Delete From Dpa_Log 
        Where (Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days)) and check_notify = '0'; 
  
  Commit;
End If; 
-- END IF Min_Dta_Azione_Log < Max_Dta_Azione_log_storico 


Select 'A fine esecuzione della procedura, alle '||Systimestamp||',                 la tabella '||Tabelladi||' ha min dta_azione:'
    ||to_char(Min_Dta_Azione,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione,'dd/mm/yyyy hh24:mi:ss')     into stringa_msg
From (Select Min(Dta_Azione) As Min_Dta_Azione From Dpa_Log_Storico)
   , (Select Max(Dta_Azione) As Max_Dta_Azione From Dpa_Log_Storico)
   , (Select 'STORICO' As Tabelladi From Dual) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

Select 'A fine esecuzione della procedura, alle '||Systimestamp||',                 la tabella '||Tabelladi||' ha min dta_azione:'
    ||to_char(Min_Dta_Azione_LEnd,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione_LEnd,'dd/mm/yyyy hh24:mi:ss') 
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_Lend From Dpa_Log)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_LEnd From Dpa_Log)
   , (Select 'DPA_LOG' As Tabelladi From Dual) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,Stringa_Msg     ) ; 


EXCEPTION
    When Others  Then
    utl_insert_log_w_autcommit (
    MYUSERNAME  --nomeutente VARCHAR2,
    ,Sysdate    --data_eseguito DATE,
    ,'invocata procedura UTL_STORICIZZA_LOG; in package UTILITA'         --comando_eseguito VARCHAR2,
    ,'categoria STORICIZZA LOG'         --CATEGORIA_COMANDO VARCHAR2,
    ,'eseguito rollback. Rilevato errore: '||SQLERRM           --esito VARCHAR2
    ) ; 
    ROLLBACK;
End UTL_STORICIZZA_LOG;


procedure UTL_OPTIMIZE_CONTEXT_INDEX (log_level int 
-- 0 = disabled, 1= only error messages
--, 2 = gives total elapsed time, 3 gives elapsed per single index
, Optimize_Mode Varchar2 -- can be 'FAST' or 'FULL'  or 'FULL_PARALLEL8' or 'REBUILD_INDEX'
 -- in 'REBUILD_INDEX' mode, index is not available, but elapsed time can be much less than with FULL mode, even with 'FULL_PARALLEL8' mode
) as

Cursor C_Text Is
   Select Index_Name, Parameters As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And (Lower(Parameters) Like '%replace%' and Parameters is not null)
   Union Select Index_Name, 'replace '||Parameters As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And (Lower(Parameters) Not Like '%replace%' And Parameters Is Not Null)
   union Select Index_Name, NULL As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And Parameters Is Null;
    
Myusername Varchar2(200); Myindex Varchar2(200); myparam Varchar2(200);
Btimestamp Timestamp; Stringa_Msg Varchar2(2000);
--minutes_elapsed number(8,1);   seconds_elapsed  number(8,1);  
privilege_granted  varchar2(20);
begin
select USERNAME INTO MYUSERNAME from user_users;

    BEGIN
        select privilege into privilege_granted from  user_tab_privs where table_name= 'CTX_DDL' ;
        IF (nvl(privilege_granted,'null') <> 'EXECUTE' ) THEN
            RAISE_APPLICATION_ERROR(-20002,'Missing EXECUTE privilege on CTX_DDL; EXECUTE privilege must be given by DBA to '||MYUSERNAME);
        end IF;
    exception when NO_DATA_FOUND then
                    RAISE_APPLICATION_ERROR(-20002,'Missing EXECUTE privilege on CTX_DDL; EXECUTE privilege must be given by DBA to '||MYUSERNAME);
              when others then RAISE;
    END;

select systimestamp into Btimestamp from dual ;

For Text In C_Text
loop
    Myindex := Text.Index_Name ;
    myparam := Text.myidxparam ;
    
--OPTIMIZE_mode can be 'FULL_PARALLEL8' or 'REBUILD_INDEX' or 'FAST' or 'FULL'  
    IF OPTIMIZE_mode = 'FULL_PARALLEL8' THEN 
    -- maxtime specify maximum optimization time, in minutes, for FULL optimize.
      -- for PAT, with optimize degree 8 elapsed 36 min approx 
      Ctx_Ddl.Optimize_Index(Myindex,Optimize_Mode, Maxtime => 59, Parallel_Degree => 8 ) ;
     elsif  (OPTIMIZE_mode = 'REBUILD_INDEX' AND myparam IS NULL) THEN 
        Execute Immediate  'alter index '||Myindex||' rebuild ' ; 
      Elsif  (Optimize_Mode = 'REBUILD_INDEX' And Myparam Is Not Null) Then 
        Execute Immediate  'Alter Index '||Myindex||' Rebuild Parameters('' '|| Myparam||''')' ;  
    Else  -- 'FAST' or other mode
      Ctx_Ddl.Optimize_Index(Myindex,Optimize_Mode) ;
    END IF; 
    
 
 Select  'Context index '||Myindex||' '||Optimize_Mode||' optimized. Elapsed '
 ||Extract(Minute From Systimestamp - Btimestamp) ||' minutes and '
 ||round(Extract(Second From Systimestamp - Btimestamp))|| ' seconds since start ' 
      into stringa_msg from dual ;

 -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
    if log_level > 2 then    -- scrive nella tabella utl_system_log
        Utl_Insert_Log_W_Autcommit (Myusername           ,Sysdate  
        ,stringa_msg        ,'OPTIMIZE_CONTEXT_INDEX'               ,'ok'   ) ; 
    end if;

End Loop;
      
      
Select Optimize_Mode||' optimized all '||Myusername||
      ' context indexes via UTL_OPTIMIZE_CONTEXT_INDEX in '
      ||Extract(Minute From Systimestamp - Btimestamp) ||' minutes and '
      ||round(Extract(Second From Systimestamp - Btimestamp))|| ' seconds'       
      into stringa_msg from dual ;

 If Log_Level > 1 Then -- scrive nella tabella utl_system_log
 Utl_Insert_Log_W_Autcommit (Myusername   ,Sysdate     , stringa_msg
    ,'OPTIMIZE_CONTEXT_INDEX'       ,'ok'   ) ; 
 end if;

exception when others then
 -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
 if log_level > 0 then -- scrive nella tabella utl_system_log
 utl_insert_log_w_autcommit (
    MYUSERNAME   --VARCHAR2,
    ,sysdate  --DATE,
    ,'KO WHILE '||OPTIMIZE_mode||' OPTIMIZING '||MYINDEX||' VIA THE sp UTL_OPTIMIZE_CONTEXT_INDEX ' -- VARCHAR2,
    ,'OPTIMIZE_CONTEXT_INDEX'       --VARCHAR2,
    ,SUBSTR('ko! '||sqlerrm,200)
              ) ; --           VARCHAR2)
 end if ;

End;

Procedure Utl_Setsecurityruoloreg Is 
Cursor C_Ruolo Is 
  Select Id_Registro, New_Aooruoloresp    
  From Utl_Track_Aooruoloresp 
    Where Cha_Changed_Notyet_Processed = 'y'; 
Myusername Varchar2(32);
Grantotale Int := 0; 
myflag boolean := true; 

Begin
Select Username Into  Myusername   From User_Users; 
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
        ,Sysdate  --DATE,
        ,'Starting SP ....' -- VARCHAR2,
        ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
        ,'ok'         ) ;     
        
For Ruolo In C_Ruolo Loop 
exit when C_Ruolo%NOTFOUND;

If Myflag Then
    myflag := false; 
    Lock Table Utl_Track_Aooruoloresp In Exclusive Mode; 
     utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
        ,Sysdate  --DATE,
        ,'Started SP; locked Table Utl_Track_Aooruoloresp In Exclusive Mode ....' -- VARCHAR2,
        ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
        ,'ok'         ) ; 
end if;

 Insert /*+ append (Security) */ Into Security (  Thing     
                        , Personorgroup
                        , Accessrights
                        , Id_Gruppo_Trasm
                        , Cha_Tipo_Diritto )  
    Select P.System_Id        As Thing               
      ,Ruolo.New_Aooruoloresp --,Cg.Id_Gruppo           
      As Personorgroup
      ,Elr.Diritto_Ruolo_Aoo  As Accessrights
      ,Null                   As Id_Gruppo_Trasm 
      ,'A'                    As Cha_Tipo_Diritto
    From Profile P,  Dpa_El_Registri Elr --, Dpa_Corr_Globali Cg
    Where P.Id_Registro=Elr.System_Id --Elr.Id_Ruolo_Resp= Cg.System_Id And 
    And Elr.Id_Ruolo_Resp Is Not Null 
    and Elr.Cha_Rf= '0' 
    and p.num_proto is not null
    and not exists (select 'x' from SECURITY s1 
      Where S1.Thing=P.System_Id 
      And S1.Personorgroup = Ruolo.New_Aooruoloresp -- Cg.Id_Gruppo --idGruppo 
      And S1.Accessrights >= Elr.Diritto_Ruolo_Aoo --Diritto 
      );
grantotale := grantotale + SQL%ROWCOUNT; 
End Loop;      

if grantotale > 0 then
     update Utl_Track_Aooruoloresp 
      Set Cha_Changed_Notyet_Processed = 'n'        ;
     commit;   
        
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'inseriti '||grantotale|| ' record in SECURITY a seguito della modifica del Ruolo responsabile AOO' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,'ok'         ) ; 
else 
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'trovati zero record da inserire in SECURITY ' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,'ok'         ) ; 

end if; 


Exception
  When No_Data_Found Then Null; 
  When Others Then 
  rollback; 
   utl_insert_log_w_autcommit (
    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'ko and rollback while running SP Utl_Setsecurityruoloreg ' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,SUBSTR('ko! '||sqlerrm,200)         ) ; 
    Raise; 
end Utl_Setsecurityruoloreg;

End Utilita ;

/

--------------------------------------------------------
--  File creato - mercoledì-settembre-18-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure I_SMISTAMENTO_SMISTADOC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE I_SMISTAMENTO_SMISTADOC (
   idpeoplemittente               IN     NUMBER,
   idcorrglobaleruolomittente     IN     NUMBER,
   idgruppomittente               IN     NUMBER,
   idamministrazionemittente      IN     NUMBER,
   idpeopledestinatario           IN     NUMBER,
   idcorrglobaledestinatario      IN     NUMBER,
   iddocumento                    IN     NUMBER,
   idtrasmissione                 IN     NUMBER,
   idtrasmissioneutentemittente   IN     NUMBER,
   trasmissioneconworkflow        IN     CHAR,
   notegeneralidocumento          IN     VARCHAR2,
   noteindividuali                IN     VARCHAR2,
   datascadenza                   IN     DATE,
   tipodiritto                    IN     CHAR,
   rights                         IN     NUMBER,
   originalrights                 IN     NUMBER,
   idragionetrasm                 IN     NUMBER,
   idpeopledelegato               IN     NUMBER,
    nonotify                     IN NUMBER,
   returnvalue                       OUT NUMBER)
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
   identitytrasm         NUMBER := NULL;
   identitytrasmsing     NUMBER := NULL;
   existaccessrights     CHAR (1) := 'Y';
   accessrights          NUMBER := NULL;
   accessrightsvalue     NUMBER := NULL;
   tipotrasmsingola      CHAR (1) := NULL;
   isaccettata           VARCHAR2 (1) := '0';
   isaccettatadelegato   VARCHAR2 (1) := '0';
   isvista               VARCHAR2 (1) := '0';
   isvistadelegato       VARCHAR2 (1) := '0';
   meth                VARchar2(256);
   descr                  VARchar2(2000);
   descrOgg                  VARchar2(2000);
   producer              VARchar2(2000);
   ragione               varchar2(32);
   idorSegn                 varchar2(256);
   idtrasmsingolamitt   NUMBER := 0;
   resultvalue           NUMBER;
BEGIN
   BEGIN
      SELECT seq.NEXTVAL INTO identitytrasm FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL INTO identitytrasmsing FROM DUAL;
   END;

   BEGIN
      /*Inserimento in tabella DPA_TRASMISSIONE */
      IF (idpeopledelegato > 0)
      THEN
         INSERT INTO dpa_trasmissione (system_id,
                                       id_ruolo_in_uo,
                                       id_people,
                                       cha_tipo_oggetto,
                                       id_profile,
                                       id_project,
                                       dta_invio,
                                       var_note_generali,
                                       id_people_delegato)
              VALUES (identitytrasm,
                      idcorrglobaleruolomittente,
                      idpeoplemittente,
                      'D',
                      iddocumento,
                      NULL,
                      SYSDATE (),
                      notegeneralidocumento,
                      idpeopledelegato);
      ELSE
         INSERT INTO dpa_trasmissione (system_id,
                                       id_ruolo_in_uo,
                                       id_people,
                                       cha_tipo_oggetto,
                                       id_profile,
                                       id_project,
                                       dta_invio,
                                       var_note_generali)
              VALUES (identitytrasm,
                      idcorrglobaleruolomittente,
                      idpeoplemittente,
                      'D',
                      iddocumento,
                      NULL,
                      SYSDATE (),
                      notegeneralidocumento);
      END IF;
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
      /* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola (system_id,
                                     id_ragione,
                                     id_trasmissione,
                                     cha_tipo_dest,
                                     id_corr_globale,
                                     var_note_sing,
                                     cha_tipo_trasm,
                                     dta_scadenza,
                                     id_trasm_utente)
           VALUES (identitytrasmsing,
                   idragionetrasm,
                   identitytrasm,
                   'U',
                   idcorrglobaledestinatario,
                   noteindividuali,
                   'S',
                   datascadenza,
                   NULL);
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

   BEGIN
      /* Inserimento in tabella DPA_TRASM_UTENTE */
      INSERT INTO dpa_trasm_utente (system_id,
                                    id_trasm_singola,
                                    id_people,
                                    dta_vista,
                                    dta_accettata,
                                    dta_rifiutata,
                                    dta_risposta,
                                    cha_vista,
                                    cha_accettata,
                                    cha_rifiutata,
                                    var_note_acc,
                                    var_note_rif,
                                    cha_valida,
                                    id_trasm_risp_sing)
           VALUES (seq.NEXTVAL,
                   identitytrasmsing,
                   idpeopledestinatario,
                   NULL,
                   NULL,
                   NULL,
                   NULL,
                   '0',
                   '0',
                   '0',
                   NULL,
                   NULL,
                   '1',
                   NULL);
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -4;
         RETURN;
   END;

BEGIN
    IF nonotify < 1 THEN
        BEGIN
            --per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio
            UPDATE dpa_trasmissione
            SET dta_invio   = SYSDATE ()
            WHERE system_id = identitytrasm;
            SELECT accessrights
            INTO accessrights
            FROM
            (SELECT accessrights
            FROM security
            WHERE thing       = iddocumento
            AND personorgroup = idpeopledestinatario
            )
            WHERE ROWNUM = 1;
        EXCEPTION
        WHEN NO_DATA_FOUND THEN
            existaccessrights := 'N';
        END;
    END IF;
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
               WHERE     thing = iddocumento
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
               SET accessrights = rights, cha_tipo_diritto = 'T'
             WHERE     thing = iddocumento
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
         INSERT INTO security (thing,
                               personorgroup,
                               accessrights,
                               id_gruppo_trasm,
                               cha_tipo_diritto)
              VALUES (iddocumento,
                      idpeopledestinatario,
                      rights,
                      idgruppomittente,
                      tipodiritto);
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

 --  CENTRO_NOTIFICHE_1 
/* Formatted on 21/06/2013 15:32:27 (QP5 v5.215.12089.38647) */
BEGIN

   SELECT NVL (var_desc_ragione, ' ')
     INTO ragione
     FROM dpa_ragione_trasm
    WHERE system_id = idragionetrasm;

   SELECT NVL (docname, ' ')
     INTO idorSegn
     FROM profile
    WHERE system_id = iddocumento;

   meth := 'TRASM_DOC_' || REPLACE (UPPER (ragione), ' ', '_');

   descrOgg := 'Trasmesso Documento ID: ' || idorSegn;
   
  descr:= 'Effettuata trasmissione documento con ragione '|| REPLACE (UPPER (ragione), ' ', '_');

   producer := getpeoplename (idpeoplemittente);

   IF (getdesccorr (idcorrglobaleruolomittente) IS NOT NULL)
   THEN
      producer :=
            producer
         || ' ('
         || REPLACE (getdesccorr (idcorrglobaleruolomittente), ''',''''')
         || ')';
   END IF;
  
   IF nonotify < 1
   THEN
       INSERT INTO dpa_log (SYSTEM_ID,
                            USERID_OPERATORE,
                            ID_PEOPLE_OPERATORE,
                            ID_GRUPPO_OPERATORE,
                            ID_AMM,
                            DTA_AZIONE,
                            VAR_OGGETTO,
                            ID_OGGETTO,
                            VAR_DESC_OGGETTO,
                            VAR_COD_AZIONE,
                            CHA_ESITO,
                            VAR_DESC_AZIONE,
                            VAR_COD_WORKING_APPLICATION,
                            ID_TRASM_SINGOLA,
                            CHECK_NOTIFY,
                            DESC_PRODUCER)
            VALUES (seq.nextval,
                    getpeopleuserid(idpeoplemittente),
                    idpeoplemittente,
                    idcorrglobaleruolomittente,
                    idamministrazionemittente,
                    SYSDATE,
                    'DOCUMENTO',
                    iddocumento,
                    descrOgg, 
                    meth,               
                    '1',
                    descr,
                    '',
                    identitytrasmsing,
                    '0',
                    producer);
        ELSE
           INSERT INTO dpa_log (SYSTEM_ID,
                  USERID_OPERATORE,
                  ID_PEOPLE_OPERATORE,
                  ID_GRUPPO_OPERATORE,
                  ID_AMM,
                  DTA_AZIONE,
                  VAR_OGGETTO,
                  ID_OGGETTO,
                  VAR_DESC_OGGETTO,
                  VAR_COD_AZIONE,
                  CHA_ESITO,
                  VAR_DESC_AZIONE,
                  VAR_COD_WORKING_APPLICATION,
                  ID_TRASM_SINGOLA,
                  CHECK_NOTIFY,
                  DESC_PRODUCER)
            VALUES (seq.nextval,
                    getpeopleuserid(idpeoplemittente),
                    idpeoplemittente,
                    idcorrglobaleruolomittente,
                    idamministrazionemittente,
                    SYSDATE,
                    'DOCUMENTO',
                    iddocumento,
                    descrOgg, 
                    meth,               
                    '1',
                    descr,
                    '',
                    identitytrasmsing,
                    '1',
                    producer);
      END IF;            

EXCEPTION
   WHEN OTHERS
   THEN
      returnvalue := -4;
END;
   
 
   /* END Gestione Centro Notifiche */

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
                                   WHEN dta_vista IS NULL THEN SYSDATE
                                   ELSE dta_vista
                                END),
                            cha_vista =
                               (CASE WHEN dta_vista IS NULL THEN 1 ELSE 0 END),
                            cha_vista_delegato = isvistadelegato,
                            cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE     tu.id_people =
                                                  idpeoplemittente
                                           AND tx.system_id =
                                                  ts.id_trasmissione
                                           AND tx.system_id = idtrasmissione
                                           AND ts.system_id =
                                                  tu.id_trasm_singola
                                           AND ts.cha_tipo_dest = 'U'));
-- devo impostare come vista la notifica.. come si fa ? Chiedo a Manu.
  UPDATE dpa_notify a
                        SET a.read_notification = '1'
                      WHERE id_specialized_object = idtrasmsingolamitt
                        AND id_people_receiver = idpeoplemittente
                        AND (   id_group_receiver = idcorrglobaleruolomittente
                             OR id_group_receiver = 0
                            );
                  END;
               ELSE
                  BEGIN
                     -- l'oggetto trasmesso visto,
                     -- pertanto la trasmissione viene solo rimossa dalla todolist
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0', cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE     tu.id_people =
                                                  idpeoplemittente
                                           AND tx.system_id =
                                                  ts.id_trasmissione
                                           AND tx.system_id = idtrasmissione
                                           AND ts.system_id =
                                                  tu.id_trasm_singola
                                           AND ts.cha_tipo_dest = 'U'));

                                           
-- START GESTIONE CENTRO NOTIFICHE
select tuu.id_trasm_singola into idtrasmsingolamitt from dpa_trasm_utente tuu where system_id=idtrasmissioneutentemittente;

  -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
           
-- END GESTIONE CENTRO NOTIFICHE
                  END;
               END IF;
            END;
         ELSE
            -- la trasmissione ancora non risulta accettata, pertanto:
            -- 1) viene accettata implicitamente,
            -- 2) l'oggetto trasmesso impostato come visto,
            -- 3) la trasmissione rimossa la trasmissione da todolist
            BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL THEN SYSDATE
                             ELSE dta_vista
                          END),
                      cha_vista =
                         (CASE WHEN dta_vista IS NULL THEN 1 ELSE 0 END),
                      cha_vista_delegato = isvistadelegato,
                      dta_accettata = SYSDATE (),
                      cha_accettata = '1',
                      cha_accettata_delegato = isaccettatadelegato,
                      var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0',
                      id_people_delegato = idpeopledelegato
                WHERE     (   system_id = idtrasmissioneutentemittente
                           OR system_id =
                                 (SELECT tu.system_id
                                    FROM dpa_trasm_utente tu,
                                         dpa_trasmissione tx,
                                         dpa_trasm_singola ts
                                   WHERE     tu.id_people = idpeoplemittente
                                         AND tx.system_id =
                                                ts.id_trasmissione
                                         AND tx.system_id = idtrasmissione
                                         AND ts.system_id =
                                                tu.id_trasm_singola
                                         AND ts.cha_tipo_dest = 'U'))
                      AND cha_valida = '1';
-- START GESTIONE CENTRO NOTIFICHE
select tuu.id_trasm_singola into idtrasmsingolamitt from dpa_trasm_utente tuu where system_id=idtrasmissioneutentemittente;


  -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
           


-- END GESTIONE CENTRO NOTIFICHE
            END;
         END IF;

         --update security se diritti  trasmssione in accettazione =20
         BEGIN
            UPDATE security s
               SET s.accessrights = originalrights, s.cha_tipo_diritto = 'T'
             WHERE     s.thing = iddocumento
                   AND s.personorgroup IN
                          (idpeoplemittente, idgruppomittente)
                   AND s.accessrights = 20;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue);

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
        FROM (  SELECT a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE     a.system_id = b.id_trasm_singola
                       AND b.system_id IN
                              (SELECT tu.system_id
                                 FROM dpa_trasm_utente tu,
                                      dpa_trasmissione tx,
                                      dpa_trasm_singola ts
                                WHERE     tu.id_people = idpeoplemittente
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
            SET cha_valida = '0', cha_in_todolist = '0'
          WHERE     id_trasm_singola IN
                       (SELECT a.system_id
                          FROM dpa_trasm_singola a, dpa_trasm_utente b
                         WHERE     a.system_id = b.id_trasm_singola
                               AND b.system_id IN
                                      (SELECT tu.system_id
                                         FROM dpa_trasm_utente tu,
                                              dpa_trasmissione tx,
                                              dpa_trasm_singola ts
                                        WHERE     tu.id_people =
                                                     idpeoplemittente
                                              AND tx.system_id =
                                                     ts.id_trasmissione
                                              AND tx.system_id =
                                                     idtrasmissione
                                              AND ts.system_id =
                                                     tu.id_trasm_singola
                                              AND ts.system_id =
                                                     (SELECT id_trasm_singola
                                                        FROM dpa_trasm_utente
                                                       WHERE system_id =
                                                                idtrasmissioneutentemittente)))
                AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;

   
   --ovvero solo se io sono tra i notificati!!!
                        -- elimino la notifica a tutti gli utenti del ruolo
                        --copio la notifica nello storico
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt;
                     
   
   returnvalue := 0;
END;

/

--------------------------------------------------------
--  File creato - giovedì-settembre-19-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Trigger ROLEHISTORYMODIFY
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER ROLEHISTORYMODIFY 
BEFORE UPDATE ON DPA_CORR_GLOBALI 
FOR EACH ROW 
 WHEN (new.cha_tipo_urp = 'R' and
      new.cha_tipo_ie = 'I' and
        new.dta_fine is null and
        ( new.id_old != old.id_old or 
          new.var_codice != old.var_codice or 
          new.var_desc_corr != old.var_desc_corr or
          new.id_uo != old.id_uo or
          new.id_tipo_ruolo != old.id_tipo_ruolo)) DECLARE
  idLastInsert integer;
BEGIN
  /******************************************************************************

    AUTHOR:    Samuele Furnari

    NAME:      ROLEHISTORYMODIFY

    PURPOSE:   Ogni volta che viene modificato un record nella dpa_corr_globali,
               se il record è relativo ad un Ruolo, se non è stata inserita
               la dta_fine e se è stato modificato almeno uno dei campi 
               monitorati, viene inserita una riga di tipo M nella tabella dello
               storico. Se invece è stato impostato l'id_old, significa che è
               stato storicizzato un ruolo, quindi viene inserito un record di
               tipo S nella tabella dello storico
 
  ******************************************************************************/

  -- Verifica eventuale cambiamento su codice del ruolo
  if :old.id_old != :new.id_old then
    -- Nella dpa_role_history bisogna preventivamente aggiornare il campo
    -- role_id con il nuovo system_id ed in seguito inserire una nuova riga
    -- con id_old uguale a quella del record appena inserito
    UPDATE dpa_role_history
    SET role_id = :new.id_old
    WHERE role_id = :old.system_id;
    
    -- Il ruolo id_old è stato storicizzato (inserimento di un record 
    -- di storicizzazione)
    INSERT
      INTO DPA_ROLE_HISTORY
        (
          SYSTEM_ID ,
          ACTION_DATE ,
          UO_ID ,
          ROLE_TYPE_ID ,
          ORIGINAL_CORR_ID ,
          ACTION ,
          ROLE_DESCRIPTION,
          ROLE_ID
        )
        VALUES
        (
          seqrolehistory.nextval,
          sysdate,
          :new.id_uo,
          :new.id_tipo_ruolo,
          :new.original_id,
          'S',
          :new.var_desc_corr || ' (' || :new.var_codice || ')',
          :new.system_id
        );
    
  else
    INSERT
      INTO DPA_ROLE_HISTORY
        (
          SYSTEM_ID ,
          ACTION_DATE ,
          UO_ID ,
          ROLE_TYPE_ID ,
          ORIGINAL_CORR_ID ,
          ACTION ,
          ROLE_DESCRIPTION,
          ROLE_ID
        )
        VALUES
        (
          seqrolehistory.nextval,
          sysdate,
          :new.id_uo,
          :new.id_tipo_ruolo,
          :new.original_id,
          'M',
          :new.var_desc_corr || ' (' || :new.var_codice || ')',
          :new.system_id
        );
    
  end if;

END;
/

begin
    begin
          execute immediate    
			'ALTER TRIGGER ROLEHISTORYMODIFY ENABLE';
	end;
end;
/

BEGIN
 utl_add_index('4.00','@db_user','DPA_CORR_GLOBALI',
    'COD_RUBRICA',null,
    '(VAR_COD_RUBRICA, ID_REGISTRO,id_Amm)',null,null,null,
    'NORMAL', null,null,null     );
END;
/

CREATE OR REPLACE Function Getdocnameorcodfasc (Id Integer)

return varchar IS 
Returnvalue Varchar2(2000); 
Mytipo_Proj Varchar2(20);  
Myid_Fascicolo Varchar2(20);  
myId_Parent Varchar2(20);  
Begin

/* dato un ID, o è un documento oppure un fascicolo 
INFATTI QUESTA QUERY TORNA ZERO RECORD: 
Select System_Id From ProJECT
  Intersect
Select System_Id From Profile */

Select nvl(Docname,system_id) into returnvalue From Profile 
  Where System_Id = Id; 

return  Returnvalue; 
Exception 
When No_Data_Found Then  -- SE LA QUERY NON HA TORNATO VALORI, dovrebbe essere un fascicolo

Select  cha_tipo_Proj , id_Fascicolo , Id_Parent into Mytipo_Proj , Myid_Fascicolo , myId_Parent 
From Project   Where System_Id = Id; 

If Mytipo_Proj = 'F' Then 
  Select    'Fascicolo\' || Nvl(Var_Codice,Description)  into Returnvalue
  From Project   Where System_Id = Id; 
end if;  

If MyTipo_Proj = 'C' Then 
  If Myid_Fascicolo = Myid_Parent Then 
    Select   'CartellaPrincipale\'|| Nvl(Var_Codice,Description) Into Returnvalue
    From Project   Where System_Id = Id; 
  End If;   
  If Myid_Fascicolo <> Myid_Parent Then 
    Select   'SottoFascicolo\'|| Nvl(Var_Codice,Description)  into Returnvalue
    From Project   Where System_Id = Id; 
  end if; 
End If;  


return  Returnvalue; 
When Others Then Returnvalue := Null; -- richiesta esplicita che non si intercetti l'eccezione
return  Returnvalue; 
End;
/

--------------------------------------------------------
--  File creato - venerdì-settembre-20-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure I_SMISTAMENTO_SMISTADOC_U
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE I_SMISTAMENTO_SMISTADOC_U (
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
   nonotify                       IN       CHAR,
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
   identitytrasm          NUMBER          := NULL;
   systrasmsing           NUMBER          := NULL;
   existaccessrights      CHAR (1)        := 'Y';
   accessrights           NUMBER          := NULL;
   accessrightsvalue      NUMBER          := NULL;
   idutente               NUMBER;
   recordcorrente         NUMBER;
   idgroups               NUMBER          := NULL;
   idgruppo               NUMBER;
   resultvalue            NUMBER;
   tipotrasmsingola       CHAR (1)        := NULL;
   isaccettata            VARCHAR2 (1)    := '0';
   isaccettatadelegato    VARCHAR2 (1)    := '0';
   isvista                VARCHAR2 (1)    := '0';
   isvistadelegato        VARCHAR2 (1)    := '0';
   val_idpeopledelegato   NUMBER;
   tiporag                VARCHAR2 (1);
   meth                   VARCHAR2 (256);
   descr                  VARCHAR2 (2000);
   descrogg               VARCHAR2 (2000);
   producer               VARCHAR2 (2000);
   ragione                VARCHAR2 (32);
   idorsegn               VARCHAR2 (256);
   idtrasmsingolamitt     NUMBER          := 0;
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
-- inizio modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega
/* Inserimento in tabella DPA_TRASMISSIONE */

      -- la procedura riceve in input il valore = 0 per il campo idpeopledelegato, in caso di senza delega
-- e il valore = 1 in caso di delega
-- nel primo caso si vuole avere comunque il valore NULL nel campo idpeopledelegato
      IF (idpeopledelegato > 0)
      THEN
         val_idpeopledelegato := idpeopledelegato;
      ELSE
         val_idpeopledelegato := NULL;
      END IF;

      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali, id_people_delegato
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE (), notegeneralidocumento, val_idpeopledelegato
                  );
-- precedente era
/*
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE , notegeneralidocumento
                  );
*/
-- fine modifica Giugno 2011    -- Correzione bug in caso di smistamento con delega
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
               SET accessrights = rights,
                   cha_tipo_diritto = 'T'
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

--  CENTRO_NOTIFICHE_1
   SELECT tuu.id_trasm_singola
     INTO idtrasmsingolamitt
     FROM dpa_trasm_utente tuu
    WHERE system_id = idtrasmissioneutentemittente;

   BEGIN
      SELECT NVL (var_desc_ragione, ' ')
        INTO ragione
        FROM dpa_ragione_trasm
       WHERE system_id = idragionetrasm;

      SELECT NVL (docname, ' ')
        INTO idorsegn
        FROM PROFILE
       WHERE system_id = iddocumento;

      meth := 'TRASM_DOC_' || REPLACE (UPPER (ragione), ' ', '_');
      descrogg := 'Trasmesso Documento ID: ' || idorsegn;
      descr :=
            'Effettuata trasmissione documento con ragione '
         || REPLACE (UPPER (ragione), ' ', '_');
      producer := getpeoplename (idpeoplemittente);

      IF (getdesccorr (idcorrglobaleruolomittente) IS NOT NULL)
      THEN
         producer :=
               producer
            || ' ('
            || REPLACE (getdesccorr (idcorrglobaleruolomittente), ''',''''')
            || ')';
      END IF;

      INSERT INTO dpa_log
                  (system_id, userid_operatore,
                   id_people_operatore, id_gruppo_operatore,
                   id_amm, dta_azione, var_oggetto,
                   id_oggetto, var_desc_oggetto, var_cod_azione, cha_esito,
                   var_desc_azione, var_cod_working_application,
                   id_trasm_singola, check_notify, desc_producer
                  )
           VALUES (seq.NEXTVAL, getpeopleuserid (idpeoplemittente),
                   idpeoplemittente, idcorrglobaleruolomittente,
                   idamministrazionemittente, SYSDATE, 'DOCUMENTO',
                   iddocumento, descrogg, meth, '1',
                   descr, '',
                   systrasmsing, nonotify, producer
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -4;
   END;

   /* END Gestione Centro Notifiche */
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

         SELECT cha_tipo_ragione
           INTO tiporag
           FROM dpa_ragione_trasm rs,
                dpa_trasm_singola ts,
                dpa_trasm_utente tsu
          WHERE tsu.system_id = idtrasmissioneutentemittente
            AND ts.system_id = tsu.id_trasm_singola
            AND rs.system_id = ts.id_ragione;

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

                     -- devo impostare come vista la notifica.. come si fa ? Chiedo a Manu.
                     UPDATE dpa_notify a
                        SET a.read_notification = '1'
                      WHERE id_specialized_object = idtrasmsingolamitt
                        AND id_people_receiver = idpeoplemittente
                        AND (   id_group_receiver = idcorrglobaleruolomittente
                             OR id_group_receiver = 0
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

                               -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                     --copio la notifica nello storico
                     INSERT INTO dpa_notify_history
                                 (system_id, id_notify, id_event,
                                  desc_producer, id_people_receiver,
                                  id_group_receiver, type_notify, dta_notify,
                                  field_1, field_2, field_3, field_4,
                                  multiplicity, specialized_field, type_event,
                                  domainobject, id_object,
                                  id_specialized_object, dta_event,
                                  read_notification)
                        SELECT seq.NEXTVAL, system_id, id_event,
                               desc_producer, id_people_receiver,
                               id_group_receiver, type_notify, dta_notify,
                               field_1, field_2, field_3, field_4,
                               multiplicity, specialized_field, type_event,
                               domainobject, id_object, id_specialized_object,
                               dta_event, read_notification
                          FROM dpa_notify
                         WHERE id_specialized_object = idtrasmsingolamitt
                           AND id_people_receiver = idpeoplemittente
                           AND (   id_group_receiver =
                                                    idcorrglobaleruolomittente
                                OR id_group_receiver = 0
                               );

                     -- elimino la ntoifica
                     DELETE FROM dpa_notify
                           WHERE id_specialized_object = idtrasmsingolamitt
                             AND id_people_receiver = idpeoplemittente
                             AND (   id_group_receiver =
                                                    idcorrglobaleruolomittente
                                  OR id_group_receiver = 0
                                 );
-- END GESTIONE CENTRO NOTIFICHE
                  END;
               END IF;
            END;
         ELSE
            BEGIN
-- la trasmissione ancora non risulta accettata, pertanto:
-- 1) viene accettata implicitamente,
-- 2) l'oggetto trasmesso impostato come visto,
-- 3) la trasmissione rimossa la trasmissione da todolist
               IF (tiporag = 'W')
               THEN
                  BEGIN
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
               ELSE                                              --no workflow
                  BEGIN
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
                                       AND ts.cha_tipo_dest = 'U'
                                       AND cha_valida = '1')
                            );
                  END;

-- START GESTIONE CENTRO NOTIFICHE

                  -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                                    --copio la notifica nello storico
                  INSERT INTO dpa_notify_history
                              (system_id, id_notify, id_event, desc_producer,
                               id_people_receiver, id_group_receiver,
                               type_notify, dta_notify, field_1, field_2,
                               field_3, field_4, multiplicity,
                               specialized_field, type_event, domainobject,
                               id_object, id_specialized_object, dta_event,
                               read_notification)
                     SELECT seq.NEXTVAL, system_id, id_event, desc_producer,
                            id_people_receiver, id_group_receiver,
                            type_notify, dta_notify, field_1, field_2,
                            field_3, field_4, multiplicity, specialized_field,
                            type_event, domainobject, id_object,
                            id_specialized_object, dta_event,
                            read_notification
                       FROM dpa_notify
                      WHERE id_specialized_object = idtrasmsingolamitt
                        AND id_people_receiver = idpeoplemittente
                        AND (   id_group_receiver = idcorrglobaleruolomittente
                             OR id_group_receiver = 0
                            );

                  -- elimino la ntoifica
                  DELETE FROM dpa_notify
                        WHERE id_specialized_object = idtrasmsingolamitt
                          AND id_people_receiver = idpeoplemittente
                          AND (   id_group_receiver =
                                                    idcorrglobaleruolomittente
                               OR id_group_receiver = 0
                              );
-- END GESTIONE CENTRO NOTIFICHE
               END IF;
            END;
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
            -- visibilit gi esistente, ignora e continua con eventuali altri inserimenti
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

   --ovvero solo se io sono tra i notificati!!!
                        -- elimino la notifica a tutti gli utenti del ruolo
                        --copio la notifica nello storico
   INSERT INTO dpa_notify_history
               (system_id, id_notify, id_event, desc_producer,
                id_people_receiver, id_group_receiver, type_notify,
                dta_notify, field_1, field_2, field_3, field_4, multiplicity,
                specialized_field, type_event, domainobject, id_object,
                id_specialized_object, dta_event, read_notification)
      SELECT seq.NEXTVAL, system_id, id_event, desc_producer,
             id_people_receiver, id_group_receiver, type_notify, dta_notify,
             field_1, field_2, field_3, field_4, multiplicity,
             specialized_field, type_event, domainobject, id_object,
             id_specialized_object, dta_event, read_notification
        FROM dpa_notify
       WHERE id_specialized_object = idtrasmsingolamitt;

   -- elimino la notifica
   DELETE FROM dpa_notify
         WHERE id_specialized_object = idtrasmsingolamitt;
END;

/


  CREATE OR REPLACE PROCEDURE I_SMISTAMENTO_SMISTADOC (
   idpeoplemittente               IN     NUMBER,
   idcorrglobaleruolomittente     IN     NUMBER,
   idgruppomittente               IN     NUMBER,
   idamministrazionemittente      IN     NUMBER,
   idpeopledestinatario           IN     NUMBER,
   idcorrglobaledestinatario      IN     NUMBER,
   iddocumento                    IN     NUMBER,
   idtrasmissione                 IN     NUMBER,
   idtrasmissioneutentemittente   IN     NUMBER,
   trasmissioneconworkflow        IN     CHAR,
   notegeneralidocumento          IN     VARCHAR2,
   noteindividuali                IN     VARCHAR2,
   datascadenza                   IN     DATE,
   tipodiritto                    IN     CHAR,
   rights                         IN     NUMBER,
   originalrights                 IN     NUMBER,
   idragionetrasm                 IN     NUMBER,
   idpeopledelegato               IN     NUMBER,
    nonotify                     IN NUMBER,
   returnvalue                       OUT NUMBER)
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
   identitytrasm         NUMBER := NULL;
   identitytrasmsing     NUMBER := NULL;
   existaccessrights     CHAR (1) := 'Y';
   accessrights          NUMBER := NULL;
   accessrightsvalue     NUMBER := NULL;
   tipotrasmsingola      CHAR (1) := NULL;
   isaccettata           VARCHAR2 (1) := '0';
   isaccettatadelegato   VARCHAR2 (1) := '0';
   isvista               VARCHAR2 (1) := '0';
   isvistadelegato       VARCHAR2 (1) := '0';
   meth                VARchar2(256);
   descr                  VARchar2(2000);
   descrOgg                  VARchar2(2000);
   producer              VARchar2(2000);
   ragione               varchar2(32);
   idorSegn                 varchar2(256);
   idtrasmsingolamitt   NUMBER := 0;
   resultvalue           NUMBER;
BEGIN
   BEGIN
      SELECT seq.NEXTVAL INTO identitytrasm FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL INTO identitytrasmsing FROM DUAL;
   END;

   BEGIN
      /*Inserimento in tabella DPA_TRASMISSIONE */
      IF (idpeopledelegato > 0)
      THEN
         INSERT INTO dpa_trasmissione (system_id,
                                       id_ruolo_in_uo,
                                       id_people,
                                       cha_tipo_oggetto,
                                       id_profile,
                                       id_project,
                                       dta_invio,
                                       var_note_generali,
                                       id_people_delegato)
              VALUES (identitytrasm,
                      idcorrglobaleruolomittente,
                      idpeoplemittente,
                      'D',
                      iddocumento,
                      NULL,
                      SYSDATE (),
                      notegeneralidocumento,
                      idpeopledelegato);
      ELSE
         INSERT INTO dpa_trasmissione (system_id,
                                       id_ruolo_in_uo,
                                       id_people,
                                       cha_tipo_oggetto,
                                       id_profile,
                                       id_project,
                                       dta_invio,
                                       var_note_generali)
              VALUES (identitytrasm,
                      idcorrglobaleruolomittente,
                      idpeoplemittente,
                      'D',
                      iddocumento,
                      NULL,
                      SYSDATE (),
                      notegeneralidocumento);
      END IF;
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
      /* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola (system_id,
                                     id_ragione,
                                     id_trasmissione,
                                     cha_tipo_dest,
                                     id_corr_globale,
                                     var_note_sing,
                                     cha_tipo_trasm,
                                     dta_scadenza,
                                     id_trasm_utente)
           VALUES (identitytrasmsing,
                   idragionetrasm,
                   identitytrasm,
                   'U',
                   idcorrglobaledestinatario,
                   noteindividuali,
                   'S',
                   datascadenza,
                   NULL);
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

   BEGIN
      /* Inserimento in tabella DPA_TRASM_UTENTE */
      INSERT INTO dpa_trasm_utente (system_id,
                                    id_trasm_singola,
                                    id_people,
                                    dta_vista,
                                    dta_accettata,
                                    dta_rifiutata,
                                    dta_risposta,
                                    cha_vista,
                                    cha_accettata,
                                    cha_rifiutata,
                                    var_note_acc,
                                    var_note_rif,
                                    cha_valida,
                                    id_trasm_risp_sing)
           VALUES (seq.NEXTVAL,
                   identitytrasmsing,
                   idpeopledestinatario,
                   NULL,
                   NULL,
                   NULL,
                   NULL,
                   '0',
                   '0',
                   '0',
                   NULL,
                   NULL,
                   '1',
                   NULL);
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -4;
         RETURN;
   END;

BEGIN
    IF nonotify < 1 THEN
        BEGIN
            --per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio
            UPDATE dpa_trasmissione
            SET dta_invio   = SYSDATE ()
            WHERE system_id = identitytrasm;
            SELECT accessrights
            INTO accessrights
            FROM
            (SELECT accessrights
            FROM security
            WHERE thing       = iddocumento
            AND personorgroup = idpeopledestinatario
            )
            WHERE ROWNUM = 1;
        EXCEPTION
        WHEN NO_DATA_FOUND THEN
            existaccessrights := 'N';
        END;
    END IF;
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
               WHERE     thing = iddocumento
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
               SET accessrights = rights, cha_tipo_diritto = 'T'
             WHERE     thing = iddocumento
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
         INSERT INTO security (thing,
                               personorgroup,
                               accessrights,
                               id_gruppo_trasm,
                               cha_tipo_diritto)
              VALUES (iddocumento,
                      idpeopledestinatario,
                      rights,
                      idgruppomittente,
                      tipodiritto);
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

 --  CENTRO_NOTIFICHE_1 
/* Formatted on 21/06/2013 15:32:27 (QP5 v5.215.12089.38647) */
BEGIN

   SELECT NVL (var_desc_ragione, ' ')
     INTO ragione
     FROM dpa_ragione_trasm
    WHERE system_id = idragionetrasm;

   SELECT NVL (docname, ' ')
     INTO idorSegn
     FROM profile
    WHERE system_id = iddocumento;

   meth := 'TRASM_DOC_' || REPLACE (UPPER (ragione), ' ', '_');

   descrOgg := 'Trasmesso Documento ID: ' || idorSegn;
   
  descr:= 'Effettuata trasmissione documento con ragione '|| REPLACE (UPPER (ragione), ' ', '_');

   producer := getpeoplename (idpeoplemittente);

   IF (getdesccorr (idcorrglobaleruolomittente) IS NOT NULL)
   THEN
      producer :=
            producer
         || ' ('
         || REPLACE (getdesccorr (idcorrglobaleruolomittente), ''',''''')
         || ')';
   END IF;
  
   IF nonotify > 1
   THEN
       INSERT INTO dpa_log (SYSTEM_ID,
                            USERID_OPERATORE,
                            ID_PEOPLE_OPERATORE,
                            ID_GRUPPO_OPERATORE,
                            ID_AMM,
                            DTA_AZIONE,
                            VAR_OGGETTO,
                            ID_OGGETTO,
                            VAR_DESC_OGGETTO,
                            VAR_COD_AZIONE,
                            CHA_ESITO,
                            VAR_DESC_AZIONE,
                            VAR_COD_WORKING_APPLICATION,
                            ID_TRASM_SINGOLA,
                            CHECK_NOTIFY,
                            DESC_PRODUCER)
            VALUES (seq.nextval,
                    getpeopleuserid(idpeoplemittente),
                    idpeoplemittente,
                    idcorrglobaleruolomittente,
                    idamministrazionemittente,
                    SYSDATE,
                    'DOCUMENTO',
                    iddocumento,
                    descrOgg, 
                    meth,               
                    '1',
                    descr,
                    '',
                    identitytrasmsing,
                    '0',
                    producer);
        ELSE
           INSERT INTO dpa_log (SYSTEM_ID,
                  USERID_OPERATORE,
                  ID_PEOPLE_OPERATORE,
                  ID_GRUPPO_OPERATORE,
                  ID_AMM,
                  DTA_AZIONE,
                  VAR_OGGETTO,
                  ID_OGGETTO,
                  VAR_DESC_OGGETTO,
                  VAR_COD_AZIONE,
                  CHA_ESITO,
                  VAR_DESC_AZIONE,
                  VAR_COD_WORKING_APPLICATION,
                  ID_TRASM_SINGOLA,
                  CHECK_NOTIFY,
                  DESC_PRODUCER)
            VALUES (seq.nextval,
                    getpeopleuserid(idpeoplemittente),
                    idpeoplemittente,
                    idcorrglobaleruolomittente,
                    idamministrazionemittente,
                    SYSDATE,
                    'DOCUMENTO',
                    iddocumento,
                    descrOgg, 
                    meth,               
                    '1',
                    descr,
                    '',
                    identitytrasmsing,
                    '1',
                    producer);
      END IF;            

EXCEPTION
   WHEN OTHERS
   THEN
      returnvalue := -4;
END;
   
 
   /* END Gestione Centro Notifiche */

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
                                   WHEN dta_vista IS NULL THEN SYSDATE
                                   ELSE dta_vista
                                END),
                            cha_vista =
                               (CASE WHEN dta_vista IS NULL THEN 1 ELSE 0 END),
                            cha_vista_delegato = isvistadelegato,
                            cha_in_todolist = '0',
                            cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE     tu.id_people =
                                                  idpeoplemittente
                                           AND tx.system_id =
                                                  ts.id_trasmissione
                                           AND tx.system_id = idtrasmissione
                                           AND ts.system_id =
                                                  tu.id_trasm_singola
                                           AND ts.cha_tipo_dest = 'U'));
-- devo impostare come vista la notifica.. come si fa ? Chiedo a Manu.
  UPDATE dpa_notify a
                        SET a.read_notification = '1'
                      WHERE id_specialized_object = idtrasmsingolamitt
                        AND id_people_receiver = idpeoplemittente
                        AND (   id_group_receiver = idcorrglobaleruolomittente
                             OR id_group_receiver = 0
                            );
                  END;
               ELSE
                  BEGIN
                     -- l'oggetto trasmesso visto,
                     -- pertanto la trasmissione viene solo rimossa dalla todolist
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0', cha_valida = '0'
                      WHERE (   system_id = idtrasmissioneutentemittente
                             OR system_id =
                                   (SELECT tu.system_id
                                      FROM dpa_trasm_utente tu,
                                           dpa_trasmissione tx,
                                           dpa_trasm_singola ts
                                     WHERE     tu.id_people =
                                                  idpeoplemittente
                                           AND tx.system_id =
                                                  ts.id_trasmissione
                                           AND tx.system_id = idtrasmissione
                                           AND ts.system_id =
                                                  tu.id_trasm_singola
                                           AND ts.cha_tipo_dest = 'U'));

                                           
-- START GESTIONE CENTRO NOTIFICHE
select tuu.id_trasm_singola into idtrasmsingolamitt from dpa_trasm_utente tuu where system_id=idtrasmissioneutentemittente;

  -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
           
-- END GESTIONE CENTRO NOTIFICHE
                  END;
               END IF;
            END;
         ELSE
            -- la trasmissione ancora non risulta accettata, pertanto:
            -- 1) viene accettata implicitamente,
            -- 2) l'oggetto trasmesso impostato come visto,
            -- 3) la trasmissione rimossa la trasmissione da todolist
            BEGIN
               UPDATE dpa_trasm_utente
                  SET dta_vista =
                         (CASE
                             WHEN dta_vista IS NULL THEN SYSDATE
                             ELSE dta_vista
                          END),
                      cha_vista =
                         (CASE WHEN dta_vista IS NULL THEN 1 ELSE 0 END),
                      cha_vista_delegato = isvistadelegato,
                      dta_accettata = SYSDATE (),
                      cha_accettata = '1',
                      cha_accettata_delegato = isaccettatadelegato,
                      var_note_acc = 'Documento accettato e smistato',
                      cha_in_todolist = '0',
                      cha_valida = '0',
                      id_people_delegato = idpeopledelegato
                WHERE     (   system_id = idtrasmissioneutentemittente
                           OR system_id =
                                 (SELECT tu.system_id
                                    FROM dpa_trasm_utente tu,
                                         dpa_trasmissione tx,
                                         dpa_trasm_singola ts
                                   WHERE     tu.id_people = idpeoplemittente
                                         AND tx.system_id =
                                                ts.id_trasmissione
                                         AND tx.system_id = idtrasmissione
                                         AND ts.system_id =
                                                tu.id_trasm_singola
                                         AND ts.cha_tipo_dest = 'U'))
                      AND cha_valida = '1';
-- START GESTIONE CENTRO NOTIFICHE
select tuu.id_trasm_singola into idtrasmsingolamitt from dpa_trasm_utente tuu where system_id=idtrasmissioneutentemittente;


  -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt
                    AND ID_PEOPLE_RECEIVER      = idpeoplemittente
                    AND (ID_GROUP_RECEIVER      = idcorrglobaleruolomittente
                    OR ID_GROUP_RECEIVER        = 0);
           


-- END GESTIONE CENTRO NOTIFICHE
            END;
         END IF;

         --update security se diritti  trasmssione in accettazione =20
         BEGIN
            UPDATE security s
               SET s.accessrights = originalrights, s.cha_tipo_diritto = 'T'
             WHERE     s.thing = iddocumento
                   AND s.personorgroup IN
                          (idpeoplemittente, idgruppomittente)
                   AND s.accessrights = 20;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue);

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
        FROM (  SELECT a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE     a.system_id = b.id_trasm_singola
                       AND b.system_id IN
                              (SELECT tu.system_id
                                 FROM dpa_trasm_utente tu,
                                      dpa_trasmissione tx,
                                      dpa_trasm_singola ts
                                WHERE     tu.id_people = idpeoplemittente
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
            SET cha_valida = '0', cha_in_todolist = '0'
          WHERE     id_trasm_singola IN
                       (SELECT a.system_id
                          FROM dpa_trasm_singola a, dpa_trasm_utente b
                         WHERE     a.system_id = b.id_trasm_singola
                               AND b.system_id IN
                                      (SELECT tu.system_id
                                         FROM dpa_trasm_utente tu,
                                              dpa_trasmissione tx,
                                              dpa_trasm_singola ts
                                        WHERE     tu.id_people =
                                                     idpeoplemittente
                                              AND tx.system_id =
                                                     ts.id_trasmissione
                                              AND tx.system_id =
                                                     idtrasmissione
                                              AND ts.system_id =
                                                     tu.id_trasm_singola
                                              AND ts.system_id =
                                                     (SELECT id_trasm_singola
                                                        FROM dpa_trasm_utente
                                                       WHERE system_id =
                                                                idtrasmissioneutentemittente)))
                AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;

   select tuu.id_trasm_singola into idtrasmsingolamitt from dpa_trasm_utente tuu where system_id=idtrasmissioneutentemittente;
   --ovvero solo se io sono tra i notificati!!!
                        -- elimino la notifica a tutti gli utenti del ruolo
                        --copio la notifica nello storico
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = idtrasmsingolamitt;
                     
   
   returnvalue := 0;
END;

/

  
begin
Insert into DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
select max(system_id) +1 , sysdate,  '4.0' from Dpa_Docspa;
end;
/

