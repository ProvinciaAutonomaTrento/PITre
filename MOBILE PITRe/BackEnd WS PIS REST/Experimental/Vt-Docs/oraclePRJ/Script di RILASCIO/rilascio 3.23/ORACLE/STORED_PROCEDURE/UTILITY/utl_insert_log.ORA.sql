begin 
	utl_backup_sp ('UTL_INSERT_LOG','3.23');
end;
/

create or replace PROCEDURE @db_user.utl_insert_log (nomeutente VARCHAR2,
 data_eseguito DATE, comando_eseguito VARCHAR2, versione_CD VARCHAR2, esito VARCHAR2)
IS  
Pragma Autonomous_Transaction ;
cnt int;
Myusername Varchar2(200);
line int :=0;
BEGIN
 
SELECT COUNT(*) INTO cnt FROM all_sequences where sequence_name='SEQ_INSTALL_LOG' 
	And Sequence_Owner= Upper(Nomeutente);
  line := 13;
If (Cnt = 0) Then
        Execute Immediate 'CREATE SEQUENCE '||Nomeutente||'.SEQ_INSTALL_LOG START WITH 1   MAXVALUE 99999999999     
					MINVALUE 1  NOCYCLE   NOCACHE   NOORDER';
          line := 17;
end IF;

SELECT COUNT(*) INTO cnt FROM user_tables where table_name='DPA_LOG_INSTALL';
IF (cnt = 0) THEN
	Select Username Into Myusername From User_Users;
    line := 23;
    Raise_Application_Error(-20001,'Missing table '||Myusername||'.DPA_LOG_INSTALL');
    
end IF;

INSERT INTO DPA_LOG_INSTALL (ID,		DATA_OPERAZIONE,
	COMANDO_RICHIESTO,	VERSIONE_CD,	ESITO_OPERAZIONE) 
VALUES ( SEQ_INSTALL_LOG.nextval  ,NVL (data_eseguito, sysdate)
 ,Comando_Eseguito ,Versione_Cd ,Esito ) ;
 line := 32;
commit;  
 -- pls leave this output here, this is intended for tracing, not for debugging
   Dbms_Output.Put_Line (Comando_Eseguito ) ;  
     line := 36;
EXCEPTION  WHEN OTHERS  THEN
 DBMS_OUTPUT.put_line ('errore da SP utl_insert_log: ' || SQLERRM);
 RAISE; --manda errore a sp chiamante
END utl_insert_log;
/

