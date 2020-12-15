-- file sql di update per il CD --
---- CREATE_DPA_LOG_INSTALL.ORA.sql  marcatore per ricerca ----
/*
AUTORE:                     GABRIELE SERPI
Data creazione:                  05/09/2011
Scopo della modifica:        CREARE LA TABELLA DPA_LOG_INSTALL
*/
 
BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'create table @db_user.DPA_LOG_INSTALL
             (ID INTEGER  NOT NULL,
  DATA_OPERAZIONE    DATE                       NOT NULL,
  COMANDO_RICHIESTO  VARCHAR2(200 BYTE)         NOT NULL,
  VERSIONE_CD        VARCHAR2(200 BYTE)         NOT NULL,
  ESITO_OPERAZIONE   VARCHAR2(200 BYTE)         NOT NULL)';

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



              
---- CREATE_UTL_SYSTEM_LOG.ORA.sql  marcatore per ricerca ----
/*
AUTORE:                     Paolo De Luca
Data creazione:                  20/03/2012
Scopo della modifica:        CREARE LA TABELLA UTL_SYSTEM_LOG e la relativa sequence SEQ_UTL_SYSTEM_LOG
							, ad uso del package UTILITA
*/
 
BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'create table @db_user.UTL_SYSTEM_LOG
            (ID INTEGER  NOT NULL,
			DATA_OPERAZIONE    DATE                       NOT NULL,
			COMANDO_RICHIESTO  VARCHAR2(2000 BYTE)         NOT NULL,
			CATEGORIA_COMANDO  VARCHAR2(2000 BYTE)         NOT NULL,
			ESITO_OPERAZIONE   VARCHAR2(2000 BYTE)         NOT NULL)';

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
    DECLARE cnt int;

  BEGIN
    SELECT COUNT(*) INTO cnt FROM user_sequences where sequence_name='SEQ_UTL_SYSTEM_LOG';

    IF (cnt = 0) THEN  
		  execute immediate 'CREATE SEQUENCE SEQ_UTL_SYSTEM_LOG
		  START WITH 1
		  MAXVALUE 99999999999
		  MINVALUE 1
		  NOCYCLE		  NOCACHE		  NOORDER';
	END IF;
	END;
END;
/





              
---- PACKAGE_UTILITA.ORA.sql  marcatore per ricerca ----
/*AUTORE:               Paolo De Luca
Data creazione:         20/03/2012
Scopo :					CREARE un package (=gruppo di procedure) per operazioni non applicative 
                        di modifiche dello schema, o di manutenzione, es. ottimizzazione periodica degli indici di dominio

Nei sistemi estesi, dove ci sono tanti schemi (=utenti DB), e al momento questo  vero solo per il sistema PITRE, 
questo package pu essere creato anche per un solo utente e invocato -previo grant di esecuzione- dagli altri schemi


Quindi, al momento SOLO per PITRE, dopo l'esecuzione del CD, si proceder con questi passi:
1. con utente PAT_PROD proprietario del package:
SQL> connect PAT_PROD/<PWD>@PITRE
     grant execute on UTilita to APSS_PROD;
     grant execute on UTilita to COMUNI_PROD; --> per ogni utente
     ....

2. con ogni utente a cui  stato concesso il privilegio di esecuzione, in quanto il package 
se invocato agisce comunque sui propri oggetti:
SQL> connect COMUNI_PROD/<PWD>@PITRE
     BEGIN 
     drop package UTilita;  --> droppo il mio package, si usa quello "comune" di PAT_PROD
     
     create synonym UTilita for PAT_PROD.UTilita;    --> se si fa il sinonimo, non  necessario anteporre il nome dello schema
     UTilita.UTL_OPTIMIZE_CONTEXT_INDEX (2,'FULL') ;  --> agisce sugli indici CONTEXT dell'utente COMUNI_PROD
     
     END;

*/
CREATE OR REPLACE PACKAGE @db_user.UTilita authid CURRENT_USER As

PROCEDURE utl_insert_log_w_autcommit ( -- autcommit = autonomous commit --
    nomeutente VARCHAR2,
    data_eseguito DATE,
    comando_eseguito  VARCHAR2,
    CATEGORIA_COMANDO VARCHAR2,
    esito             VARCHAR2) ;

Procedure Utl_Storicizza_Log (Interval_Of_Days Int -- will insert into DPA_LOG_STORICO, delete from DPA_LOG
            --  of oldest record for #Interval_Of_Days days
                                ); 

procedure UTL_OPTIMIZE_CONTEXT_INDEX (
          log_level int -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
          , OPTIMIZE_mode VARCHAR2 -- can be 'FULL' or 'FAST'
          );

END UTilita ;
/

CREATE OR REPLACE package body @db_user.UTilita  as 
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

 commit; -- si pu fare perch c' PRAGMA AUTONOMOUS_TRANSACTION prima in DECLARE
EXCEPTION
 WHEN OTHERS THEN
 DBMS_OUTPUT.put_line ('errore generico in insert log' || SQLERRM);
 RAISE; --manda errore a sp chiamante
End Utl_Insert_Log_W_Autcommit ;


PROCEDURE UTL_STORICIZZA_LOG (interval_of_days int) IS
Pragma Autonomous_Transaction ;
idx_esistente   EXCEPTION;
Pragma Exception_Init (Idx_Esistente  , -955);
-- ORA-01408 esiste gi un indice per questa lista di colonne
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
         Dbms_Output.Put_Line ('indice gi esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice gi esistente');
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
         Dbms_Output.Put_Line ('indice gi esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice gi esistente');
      
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
    ,'Bonificare DPA_LOG_STORICO! min(dta_azione) non  maggiore o uguale a Max(Dta_Azione_storico) '    ) ; 

Else  -- ok, Min_Dta_Azione_Log  maggiore o uguale a nvl(Max_Dta_Azione_log_storico, to_date('01-01-1970','dd-mm-yyyy') )

-- since dpa_log_storico has STORAGE COMPRESSION enabled, append hint will now store rows in compressed form
  INSERT /*+ append */ INTO dpa_log_storico (
      SYSTEM_ID,         USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                    ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE)
  SELECT SYSTEM_ID,    USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                   ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE
    From Dpa_Log
        Where Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days);  
  
  Delete From Dpa_Log 
        Where Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days); 
  
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


End Utilita ;
/
              
---- SEQ_INSTALL_LOG.ORA.sql  marcatore per ricerca ----
--
-- SEQ_INSTALL_LOG  (Sequence) 
--
BEGIN
    DECLARE cnt int;
     cntdati int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM user_sequences where sequence_name='SEQ_INSTALL_LOG';
    select max(nvl(id,0)) +1 into cntdati from @db_user.dpa_log_install; 
    IF (cnt = 0) THEN  

  execute immediate ' CREATE SEQUENCE SEQ_INSTALL_LOG
  START WITH '||cntdati||'
  MAXVALUE 99999999999
  MINVALUE 1
  NOCYCLE
  NOCACHE
  NOORDER';
	END IF;
	END;
END;
/
              
---- utl_add_column.ORA.SQL  marcatore per ricerca ----
/*
-- =============================================
-- Author:        Gabriele Serpi
-- Create date: 11/07/2011
-- Description:    
-- =============================================

-- es. invocazione:
Declare
  Versione_Cd Varchar2(200)  := '3.19.1';
  Nomeutente Varchar2(200)   := 'INFOTN_COLL';
  Nome_Tabella Varchar2(200) := 'TEST';
  Nome_Colonna Varchar2(200) := 'TEST_ADD2';
  Tipo_Dato Varchar2(200)    := 'CHAR(1)'; 
  VAL_DEFAULT VARCHAR2(200)  := '1';
  Condizione_Modifica_Pregresso Varchar2(200)
                                ;
  CONDIZIONE_CHECK VARCHAR2(200);
  RFU VARCHAR2(200);
BEGIN
  UTL_ADD_COLUMN(
    VERSIONE_CD => VERSIONE_CD,
    NOMEUTENTE => NOMEUTENTE,
    NOME_TABELLA => NOME_TABELLA,
    NOME_COLONNA => NOME_COLONNA,
    TIPO_DATO => TIPO_DATO,
    VAL_DEFAULT => VAL_DEFAULT,
    CONDIZIONE_MODIFICA_PREGRESSO => CONDIZIONE_MODIFICA_PREGRESSO,
    CONDIZIONE_CHECK => CONDIZIONE_CHECK,
    RFU => RFU
  );
End;

*/

CREATE OR REPLACE PROCEDURE @db_user.utl_add_column (
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                     VARCHAR2,
   tipo_dato                        VARCHAR2,
   val_default                      VARCHAR2,
   condizione_modifica_pregresso    VARCHAR2,
   condizione_check                 VARCHAR2,
   RFU                              VARCHAR2)
IS
   cnt   INT;
   istruzsql varchar2(200); 
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = UPPER(nomeutente);
    
   IF (cnt = 1)    THEN    -- ok la tabella esiste
      SELECT COUNT ( * )
        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna)
             AND UPPER(owner) = nomeutente;
      IF (cnt = 0)       THEN       -- ok la colonna non esiste, la aggiungo
		 if val_default  IS null then 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
						|| ' ADD '      || nome_colonna|| ' '|| tipo_dato ; 
		  ELSE --  val_default  IS NOT null 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
					|| ' ADD '      || nome_colonna|| ' '|| tipo_dato || ' default '|| val_default   ; 

		end if ; 
         EXECUTE IMMEDIATE  istruzsql  ;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- if null, data di oggi
                         ,'Added column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 
                         
        ELSE  -- cnt=1,    la colonna esiste gi 
             utl_modify_column (versione_CD                      
				   ,nomeutente  ,nome_tabella                     ,nome_colonna                     
				   ,tipo_dato                        
				   ,val_default                      
				   ,condizione_check                 
				   ,RFU                              ) ; 
			  
			  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data di oggi
                         ,'modified column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'column modified, was already there' );
			  
      END IF;
   END IF;
   
            
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore

   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      
               dbms_output.put_line ('errore da SP utl_add_column: '||SQLERRM);
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Adding column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                        -- RAISE;
               
END utl_add_column;
/
              
---- utl_add_foreign_key.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_foreign_key('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. DOCSADM
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME COLONNA', --->es. COLONNA_TABELLA_A
--					'NOME TABELLA FOREIGN KEY', --->es. TABELLA_B
--					'NOME COLONNA FOREIGN KEY', --->es. COLONNA_TABELLA_B
--					'CONDIZIONE ADD', ---> LASCIARE VUOTO CON ''
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 11/07/2011
-- Description:	
-- =============================================
-- NOME DEL FOREIGN KEY VIENE NOMINATO AUTOMATICAMENTE 
-- PRELEVANDO NOME TABELLA_A, TABELLA_B E COLONNA_A (FK_'TABELLA'_'COLONNA'_'TABELLAFK')


CREATE OR REPLACE PROCEDURE @db_user.utl_add_foreign_key
(
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                   VARCHAR2,
   nome_tabella_fk                 VARCHAR2,
   nome_colonna_fk                VARCHAR2,
   condizione_add                  VARCHAR2,
   RFU                                  VARCHAR2)
IS

      istruzione varchar2(2000); 
      cnt       INT;
      cntdati  INT; 
      nome_f VARCHAR2(200) := SUBSTR(nome_tabella,1,10);
      nome_cl_f VARCHAR2(200) := SUBSTR(nome_colonna,1,8);
      nome_tf VARCHAR2(200) := SUBSTR(nome_tabella_fk,1,8);
      nome_foreign_key VARCHAR2(2000) :='FK_'||nome_f||'_'||nome_cl_f||'_'||nome_tf||'';
     errore_msg varchar(255);

   BEGIN
     /*SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
-- INSERIRE SEMPRE IL TIPO del VINCOLO:
-- i tipi possibili sono U=unique, R=referential (chiave esterna),
-- P=Primary e C= check    
         and cons.constraint_type  = 'R' 
         and cons.owner            = nomeutente
         and cons.table_name       = nome_tabella
         and cons.constraint_name = nome_foreign_key ; */
         
       istruzione :=' SELECT COUNT (*) FROM user_constraints
       WHERE constraint_name = '''||nome_foreign_key||'''
         AND constraint_type = ''R'' 
         and owner='''||nomeutente||'''';
   
     EXECUTE IMMEDIATE istruzione  INTO cnt ;      


      IF (cnt = 0)  
      -- ok il vincolo non esiste
      THEN
     istruzione :=     'SELECT COUNT (f.'||nome_colonna||')  
                    FROM '||nomeutente||'.'||nome_tabella||' f
                   WHERE f.'||nome_colonna||' IN 
                   (SELECT tf.'||nome_colonna_fk||'
                   FROM '||nomeutente||'.'||nome_tabella_fk||' tf)' ; 

     EXECUTE IMMEDIATE istruzione  INTO cntdati ;
    
     
         IF (cntdati = 0)
         -- non esistono record che violano la chiave
         THEN
            istruzione :=  'ALTER TABLE '
                                            ||nomeutente||'.'||nome_tabella||' 
                                            ADD CONSTRAINT
                                            '||nome_foreign_key||' 
                                            FOREIGN KEY 
                                            ('||nome_colonna||')  
                                            REFERENCES
                                            '||nomeutente||'.'||nome_tabella_fk||'
                                            ('||nome_colonna_fk||')'; 
                                            
            dbms_output.put_line ('istruzione '||istruzione); 
            
            EXECUTE IMMEDIATE istruzione;
                                            
                                            utl_insert_log (nomeutente  -- nome utente
                                                         , NULL -- data
                                                         ,'Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                                                         , versione_CD
                                                         , 'esito positivo' ); 

        ELSE                         
   
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Ci sono i records che violano la chiave' );
        

         END IF;
         ELSE                         
        
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Chiave esterna gi esistente' );
      

         END IF;

      
       dbms_output.put_line ('sqlerrm: '||sqlerrm);
             
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
     
               dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                errore_msg := SUBSTR(SQLERRM,1,255);
                
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo - '||errore_msg||'' ); 
END utl_add_foreign_key;
/

              
---- utl_add_index.ORA.sql  marcatore per ricerca ----
create or replace PROCEDURE @db_user.utl_add_index(
/*
-- =============================================
-- Author:		Gabriele Serpi - De Luca
-- Create date: 25/07/2011 -- riv. mar 2012
-- Description:	permette di aggiungere un indice
				se l'indice esiste con nome differente rispetto a quello passato come argomento, fa la rinomina 

-- =============================================

es. invocazione:
DECLARE
  VERSIONE_CD VARCHAR2(200)             := 'TEST';
  NOMEUTENTE VARCHAR2(200)               := 'PAT_TEST'  ;
  Nome_Tabella Varchar2(200) := 'TEST';
  Nome_Indice Varchar2(200)  := 'IDX_CONTEXT';
  IS_UNIQUE VARCHAR2(200)        := NULL;
  NOME_COLONNA1 VARCHAR2(200) := 'ID';
  NOME_COLONNA2 VARCHAR2(200) := '';
  Nome_Colonna3 Varchar2(200) := '';
  Nome_Colonna4 Varchar2(200) := '';
  Index_Type Varchar2(200) := 'DOMAIN' ;
  Ityp_Name Varchar2(200) := 'CONTEXT';
  Optional_Ityp_Parameters Varchar2(200) := Null ;
  RFU VARCHAR2(200) := NULL;
BEGIN

  UTL_ADD_INDEX(
    VERSIONE_CD => VERSIONE_CD,
    NOMEUTENTE => NOMEUTENTE,
    NOME_TABELLA => NOME_TABELLA,
    NOME_INDICE => NOME_INDICE,
    IS_UNIQUE => IS_UNIQUE,
    NOME_COLONNA1 => NOME_COLONNA1,
    NOME_COLONNA2 => NOME_COLONNA2,
    NOME_COLONNA3 => NOME_COLONNA3,
    Nome_Colonna4 => Nome_Colonna4,
    Index_Type => Index_Type ,
    ITYP_NAME => ITYP_NAME,
    OPTIONAL_ITYP_PARAMETERS => OPTIONAL_ITYP_PARAMETERS,
    RFU => RFU
  );
End;

*/
    
    versione_CD              VARCHAR2,
    nomeutente               VARCHAR2,
    Nome_Tabella             Varchar2,
    Nome_Indice              VARCHAR2,
    is_unique          VARCHAR2, -- passare valore 'UNIQUE' se si vuole creare vincolo di unicit
    nome_colonna1            VARCHAR2,
    nome_colonna2            VARCHAR2,
    Nome_Colonna3            Varchar2,
    Nome_Colonna4            Varchar2,
    Index_Type               Varchar2, -- must be supplied, can be NORMAL or DOMAIN
    Ityp_Name                VARCHAR2, -- if Index_Type == DOMAIN, must be supplied, can be CTXCAT or CONTEXT 
    OPTIONAL_ITYP_PARAMETERS VARCHAR2, -- es sync (on commit) stoplist ctxsys.empty_stoplist
    RFU                      VARCHAR2)
IS
  istruzione          VARCHAR2(2000);
  cnt                 INT;
  cntdati             INT;
  colonne             VARCHAR2(2000);
  Errore_Msg          VARCHAR(255);
  Mygranted_Role      VARCHAR2(20);
  Domain_Index_Clause VARCHAR(2000);
  Rename_Clause       VARCHAR(2000);
  Old_Index_Name      Varchar2(32);
  Old_Index_Type      Varchar2(32) := 'non nullo';
  old_Ityp_Name       Varchar2(32) := 'non nullo';
Begin
  IF (nomeutente IS NULL OR nome_tabella IS NULL OR index_type is null OR Nome_Indice IS NULL OR nome_colonna1 IS NULL) THEN
    Raise_Application_Error (-20010,'in creazione di un indice
    , nessuno tra gli argomenti nomeutente OR nome_tabella OR index_type OR Nome_Indice OR nome_colonna1 pu essere NULL ');
  End If;
  If (Index_Type <> 'DOMAIN' AND Index_Type <> 'NORMAL') Then
    Raise_Application_Error (-20010,'per il parametro Index_Type, sono supportati solo tipi NORMAL o DOMAIN');
  END IF;
  If Length(Nome_Indice) >32 Then
    Raise_Application_Error (-20011,'in creazione di un indice, il nome dell''indice non pu essere pi lungo di 32 caratteri') ;
  End If;
  If Ityp_Name          Is Not Null And (Nome_Colonna2 Is Not Null Or Nome_Colonna3 Is Not Null Or Nome_Colonna4 Is Not Null) Then
    Raise_Application_Error (-20012,'se Ityp_Name valorizzato, perch si vuole creare un indice di dominio
        , non  possibile specificare pi di una colonna, gli argomenti nome_colonna da 2 a 4 devono essere null ')    ;
  END IF; 
  
  SELECT COUNT (*)
  INTO cnt
  FROM all_indexes
  WHERE index_name = upper(nome_indice)
  AND owner        =nomeutente;
  
  IF (Cnt = 0) THEN
    dbms_output.put_line ('ok indice non esiste, o almeno non esiste con il nome passato come parametro ');
    -- verifico se esiste l'indice sulle stesse colonne, con altro nome
    IF Nome_Colonna4 IS NOT NULL AND Nome_Colonna3 IS NOT NULL AND Nome_Colonna2 IS NOT NULL AND Nome_Colonna1 IS NOT NULL THEN
      dbms_output.put_line ('4 colonne');
      SELECT Index_Name
      INTO Old_Index_Name
      FROM
        (SELECT I1.Index_Name
        FROM All_Ind_Columns I1,          all_Ind_Columns I2,          all_Ind_Columns I3,          all_Ind_Columns I4
        WHERE I1.Index_Name    = I2.Index_Name
        AND I2.Index_Name      = I3.Index_Name
        AND I3.Index_Name      = I4.Index_Name
        AND I1.Table_Name      = Upper(Nome_Tabella)
        AND I1.Index_Owner     = Upper(Nomeutente)
        AND I1.Column_Name     =Nome_Colonna1        AND I1.Column_Position = 1
        AND I2.Column_Name     =Nome_Colonna2        AND I2.Column_Position = 2
        AND I3.Column_Name     =Nome_Colonna3        AND I3.Column_Position = 3
        AND I4.Column_Name     =Nome_Colonna4        AND I4.Column_Position = 4
        UNION
        SELECT '00000000n.a.' AS Index_Name FROM Dual ORDER BY 1
        )
      WHERE rownum       =1;
      IF Old_Index_Name <> '00000000n.a.' THEN
        Dbms_Output.Put_Line ('4 colonne e indice esiste gi su quelle colonne');
      ELSE -- non esiste indice su quelle colonne
        Colonne := Nome_Colonna1|| ' ,'||Nome_Colonna2|| ' ,'||Nome_Colonna3|| ' ,'||Nome_Colonna4;
      END IF;
    Elsif Nome_Colonna3 IS NOT NULL AND Nome_Colonna2 IS NOT NULL AND Nome_Colonna1 IS NOT NULL THEN
      SELECT Index_Name
      INTO Old_Index_Name
      FROM
        (SELECT I1.Index_Name
        FROM all_Ind_Columns I1,          all_Ind_Columns I2,          all_Ind_Columns I3
        WHERE I1.Index_Name    = I2.Index_Name
        AND I2.Index_Name      = I3.Index_Name
        AND I1.Table_Name      = Upper(Nome_Tabella)
        AND I1.Index_Owner     = Upper(Nomeutente)
        And I1.Column_Name     =Nome_Colonna1        AND I1.Column_Position = 1
        AND I2.Column_Name     =Nome_Colonna2        AND I2.Column_Position = 2
        AND I3.Column_Name     =Nome_Colonna3        AND I3.Column_Position = 3
        UNION
        SELECT '00000000n.a.' AS Index_Name FROM Dual ORDER BY 1
        )
      WHERE rownum       =1;
      IF Old_Index_Name <> '00000000n.a.' THEN
        Dbms_Output.Put_Line ('3 colonne e indice esiste gi su quelle colonne');
      ELSE -- non esiste indice su quelle colonne
        Colonne := Nome_Colonna1|| ' ,'||Nome_Colonna2|| ' ,'||Nome_Colonna3;
      END IF;
    Elsif Nome_Colonna2 IS NOT NULL AND Nome_Colonna1 IS NOT NULL THEN
      SELECT Index_Name
      INTO Old_Index_Name
      FROM
        (SELECT I1.Index_Name
        From All_Ind_Columns I1,          All_Ind_Columns I2        
        WHERE I1.Index_Name    = I2.Index_Name
        AND I1.Table_Name      = Upper(Nome_Tabella)
        AND I1.Index_Owner     = Upper(Nomeutente)
        AND I1.Column_Name     =Nome_Colonna1        AND I1.Column_Position = 1
        AND I2.Column_Name     =Nome_Colonna2        AND I2.Column_Position = 2
        UNION
        SELECT '00000000n.a.' AS Index_Name FROM Dual ORDER BY 1
        )
      WHERE Rownum       =1;
      IF Old_Index_Name <> '00000000n.a.' THEN
        Dbms_Output.Put_Line ('2 colonne e indice esiste gi su quelle colonne');
      ELSE -- non esiste indice su quelle colonne
        Colonne := Nome_Colonna1||' ,'||Nome_Colonna2;
      END IF;
    Else -- Nome_Colonna1 IS NOT NULL
      Select Index_Name  , Index_Type, Ityp_Name  
      Into Old_Index_Name, old_index_type, old_Ityp_Name
      From
        (Select I1.Index_Name, ind.index_type, ind.Ityp_Name
        From All_Ind_Columns I1, All_Indexes Ind
        Where I1.Index_Name    = Ind.Index_Name
        AND I1.Table_Name    = Upper(Nome_Tabella)
        AND I1.Index_Owner     = Upper(Nomeutente)
        AND I1.Column_Name     =Nome_Colonna1
        And I1.Column_Position = 1
        and ind.index_type <> 'DOMAIN'
        Union
        SELECT '00000000n.a.' AS Index_Name, 'index_type' as index_type,  'Ityp_Name' as Ityp_Name FROM Dual ORDER BY 1
        )
      Where Rownum       =1;
      If (Old_Index_Name <> '00000000n.a.' And Old_Index_Type = Index_Type) Then
        Dbms_Output.Put_Line ('indice, di stesso tipo (dominio o normale), esiste gi sulla colonna');
      ELSE -- non esiste indice su quelle colonne
        Colonne := Nome_Colonna1;
      END IF;
    End If;
    -- fine if che cicla su valori delle colonne
    
    IF index_type          = 'DOMAIN' AND OPTIONAL_ITYP_PARAMETERS IS NOT NULL THEN
      Domain_Index_Clause := ' INDEXTYPE IS CTXSYS.'||Ityp_Name||' PARAMETERS ('''||OPTIONAL_ITYP_PARAMETERS||''')';
    Elsif index_type          = 'DOMAIN' THEN
      Domain_Index_Clause := ' INDEXTYPE IS CTXSYS.'||Ityp_Name ;
    ELSE
      Domain_Index_Clause := ' ';
    End If;
    
    IF Old_Index_Name <> '00000000n.a.' and old_Ityp_Name = Ityp_Name THEN -- esiste gi indice su quelle colonne, devo solo rinominare
      istruzione      := 'alter index '||Old_Index_Name||' rename to '||Nome_Indice ;
    ELSE -- non esiste indice su quelle colonne, proseguo normale esecuzione.....
      Istruzione := 'CREATE '||Is_Unique||' INDEX ' ||Nomeutente||'.'||Nome_Indice ||' 
      ON '||nome_tabella||'  ('||colonne||') ' 
      ||Domain_Index_Clause;
    END IF;
    Dbms_Output.Put_Line ('istruzione '||Istruzione);
    EXECUTE IMMEDIATE istruzione;
    Utl_Insert_Log (Nomeutente -- nome utente
    , NULL                     -- data
    ,'Added index '||Nome_Indice||' on table '||Nome_Tabella , Versione_Cd , 'esito positivo' );
  ELSE                         -- esiste indice con stesso nome
    utl_insert_log (nomeutente -- nome utente
    , NULL                     -- data
    ,'Adding index '||nome_indice||' on table '||nome_tabella , versione_CD , 'Indice gi esistente con stesso nome' );
  END IF;
  -- fine if  count su indici con nome= nome passato come argomento
EXCEPTION
WHEN OTHERS THEN
  dbms_output.put_line ('errore '||SQLERRM);
  errore_msg := SUBSTR(SQLERRM,1,255);
  utl_insert_log (nomeutente -- nome utente
  , NULL                     -- data
  ,'Adding index '||nome_indice||' on table '||nome_tabella , versione_CD , 'esito negativo - '||Errore_Msg||'' );
  RAISE;
END utl_add_index;
/

              
---- utl_add_primary_key.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_add_primary_key('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. DOCSADM
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME COLONNA', --->es. COLONNA_A
--					'NOME PRIMARY KEY', --->es. PK_TABELLA_A
--					'CONDIZIONE ADD', --->es. ''
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 26/07/2011
-- Description:	
-- =============================================

CREATE OR REPLACE PROCEDURE @db_user.utl_add_primary_key 
(
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                   VARCHAR2,
   condizione_add                  VARCHAR2,
   RFU                                  VARCHAR2)
IS

      istruzione varchar2(2000); 
      cnt       INT;
      cntdati  INT; 
      nome_primary_key             VARCHAR2(2000) :='PK_'||nome_tabella;
      errore_msg varchar(255);
      
   BEGIN
     SELECT COUNT (*) INTO cnt  FROM all_constraints cons, all_cons_columns cons_cols
       WHERE cons.CONSTRAINT_NAME = cons_cols.CONSTRAINT_NAME
-- INSERIRE SEMPRE IL TIPO del VINCOLO:
-- i tipi possibili sono U=unique, R=referential (chiave esterna),
-- P=Primary e C= check    
         and cons.constraint_type  = 'P' 
         and cons.owner            = nomeutente
         and cons.table_name       = nome_tabella
         and cons_cols.COLUMN_NAME = nome_colonna ; 
      
      IF (cnt = 0)
      -- ok il vincolo non esiste
      THEN
     istruzione :=     'SELECT COUNT (*) 
                    FROM '||nomeutente||'.'||nome_tabella||'
                    group by '||nome_colonna||'
                    having count(*) > 1
                    UNION
                    select 0 from dual' ; 
     
     dbms_output.put_line ('istruzione per cnt dati '||istruzione); 
    
            
     EXECUTE IMMEDIATE istruzione                    INTO cntdati ;
       dbms_output.put_line ('cntdati: '||cntdati); 
       dbms_output.put_line ('arriviamo dopo questa ? cntdati: '||cntdati);  
     
     
         IF (cntdati = 0)
         -- non esistono record che violano la chiave
         THEN
            istruzione :=  'ALTER TABLE '
                                            ||nomeutente||'
                                            .'||nome_tabella||' 
                                            ADD CONSTRAINT
                                            '||nome_primary_key||' 
                                            PRIMARY KEY 
                                            ('||nome_colonna||') '; 
            dbms_output.put_line ('istruzione '||istruzione); 
            
            EXECUTE IMMEDIATE istruzione;
                                            
                                            utl_insert_log (nomeutente  -- nome utente
                                                         , NULL -- data
                                                         ,'Added Primary Key '||nome_primary_key||' on table '||nome_tabella
                                                         , versione_CD
                                                         , 'esito positivo' ); 

        ELSE                         
         dbms_output.put_line ('errore, esiste gi ');    
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added Primary Key '||nome_primary_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Chiave primaria gi esistente' );
                             dbms_output.put_line ('cln esiste: '||sqlerrm);

         END IF;
          ELSE                         
         dbms_output.put_line ('errore, esiste gi ');    
          utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added Primary Key '||nome_primary_key||' on table '||nome_tabella
                         , versione_CD
                         , 'Chiave primaria gi esistente' );
                             dbms_output.put_line ('cln esiste: '||sqlerrm);
      END IF;
      
       dbms_output.put_line ('sqlerrm: '||sqlerrm);
             
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
     
               dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Added Primary Key '||nome_primary_key||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo - '||errore_msg||'' ); 
END utl_add_primary_key;
/

              
---- utl_drop_column.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_drop_column('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. DOCSADM
--					'NOME TABELLA', --->es. TABELLA_A
--					'NOME COLONNA', --->es. COLONNA_A
--					'CONDIZIONE DROP', --->es. '0' CANCELLA COLONNA SOLO SE E' VUOTO, '1' CANCELLA ANCHE SE NON VUOTO!!
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 18/07/2011
-- Description:	
-- =============================================
CREATE OR REPLACE PROCEDURE @db_user.utl_drop_column    
(
   versione_CD                      VARCHAR2,
   nomeutente VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                     VARCHAR2,
   condizione_drop                   VARCHAR2,
   RFU                                      VARCHAR2)
IS
   cnt   INT; -- per vedere se esiste
   ONLYIF_COLUMN_IS_EMPTY INT; -- per vedere se  vuota
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = nomeutente;  

   IF (cnt = 1)
   -- ok la tabella esiste
   THEN

 --SELECT COUNT ( nome_colonna )     INTO cntdati     FROM table_name = UPPER (nome_tabella) AND owner = nomeutente;
     
     EXECUTE IMMEDIATE 'SELECT COUNT ( '|| nome_colonna|| ' )     FROM  '|| nome_tabella INTO ONLYIF_COLUMN_IS_EMPTY;  

     IF ((condizione_drop = 0 AND ONLYIF_COLUMN_IS_EMPTY = 0) OR condizione_drop = 1) 
     THEN
           EXECUTE IMMEDIATE   'ALTER TABLE '
                          || nomeutente
                          || '.'
                          || nome_tabella
                          || ' DROP COLUMN '
                          || nome_colonna;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Droped column '||nome_colonna||' from table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 

   END IF;
   END IF;
   
   
       dbms_output.put_line ('sqlerrm: '||sqlerrm);
EXCEPTION

   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
     
                 dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Droped column '||nome_colonna||' from table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                         -- RAISE;
END utl_drop_column  ;
/

              
---- Utl_Insert_Chiave_Config.ORA.sql  marcatore per ricerca ----
-- function used by Utl_Insert_Chiave_Config and other 2 Utl_Insert_Chiave%
Create Or Replace Function @db_user.Utl_Isvalore_Lt_Column
(Valore Varchar2,Mytable Varchar2,mycol varchar2)
Return Integer -- returns 0 if OK, that is: lentgh(valore) less then Data_Length of the column mycol
Is Cnt Int;
returnvalue int :=1;
Begin
Select  Data_Length - Length(Valore) Into Cnt   From Cols
  Where Lower(Table_Name)=Lower(Mytable) And Lower(Column_Name) = Lower(Mycol) ;

If Cnt>=0 Then Returnvalue := 0 ; End If;

Return Returnvalue ;
Exception When Others Then Raise;
end;
/


CREATE OR REPLACE Procedure @db_user.Utl_Insert_Chiave_Config(
    Codice               VARCHAR2 ,
    Descrizione          VARCHAR2 ,
    Valore               VARCHAR2 ,
    Tipo_Chiave          Varchar2 ,
    Visibile             VARCHAR2 ,
    Modificabile         VARCHAR2 ,
    Globale              VARCHAR2 ,
    Myversione_Cd          Varchar2 ,
    Codice_Old_Webconfig Varchar2 ,
    Forza_Update  VARCHAR2 , RFU VARCHAR2 )
Is

/* -- esempio blocco di invocazione 
Utl_Insert_Chiave_Config(
	'FE_COPIA_VISIBILITA'								--    Codice               VARCHAR2 ,
	,'Abilita il pulsante per la copia della visibilit' --    Descrizione          VARCHAR2 ,
	,'0' --    Valore               VARCHAR2 ,
	,'F' --    Tipo_Chiave          Varchar2 ,
	,'1' --    Visibile             VARCHAR2 ,
    ,'1' --    Modificabile         VARCHAR2 ,
    ,'1' --	Globale              VARCHAR2 ,
    , '3.20.1'  -- myversione_CD
    , NULL	--Codice_Old_Webconfig Varchar2 ,
    , NULL, NULL --Forza_Update_Valore  VARCHAR2 , RFU VARCHAR2 
	)
*/
  -- Pragma Autonomous_Transaction ;
  Cnt Int;
  Maxid Int;
  Nomeutente Varchar2 (32); 
  stringa_msg Varchar2 (200); 
BEGIN
  -- controlli lunghezza valori passati
  If Utl_IsValore_Lt_Column(Codice, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_CODICE') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro CODICE too large for column VAR_CODICE')  ; 
  End If;
  If Utl_IsValore_Lt_Column(Descrizione, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Descrizione') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Descrizione too large for column VAR_CODICE')  ; 
  End If;
  If Utl_IsValore_Lt_Column(Valore, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Valore') = 1 Then 
    RAISE_APPLICATION_ERROR(-20001,'parametro Valore too large for column VAR_CODICE')  ; 
  END IF;
  -- fine controlli lunghezza valori passati
  
Select Username Into Nomeutente From User_Users   ; 
 UTL_ADD_COLUMN(    myVERSIONE_CD,    Nomeutente ,    'DPA_CHIAVI_CONFIGURAZIONE',
    'DTA_INSERIMENTO',    'DATE',    'SYSDATE',    Null,    Null,   Null  );  
 UTL_ADD_COLUMN(    myVERSIONE_CD,    Nomeutente ,    'DPA_CHIAVI_CONFIGURAZIONE',
    'VERSIONE_CD',    'varchar2(32)',    NULL,    Null,    Null,   Null  );  
  
  -- per successivo controllo che sequence sia pi avanti del max system_id
  Select Max(System_Id) Into Maxid From Dpa_Chiavi_Configurazione; 
  
  SELECT COUNT(*)  INTO cnt
  FROM DPA_CHIAVI_CONFIGURAZIONE
  Where Var_Codice=Codice;
  
  If (Cnt         = 0 And Globale    = 1) Then -- inserisco la chiave globale non esistente
      -- must use exec immediate since column VERSIONE_CD may not exist yet at compile time
      execute immediate 'INSERT  INTO DPA_CHIAVI_CONFIGURAZIONE
        ( System_Id,          Id_Amm,          
          Var_Codice , VAR_DESCRIZIONE ,          VAR_VALORE ,
          Cha_Tipo_Chiave,          Cha_Visibile ,
          CHA_MODIFICABILE,          CHA_GLOBALE, 
          VAR_CODICE_OLD_WEBCONFIG, VERSIONE_CD )
        Values
        ( greatest(SEQ_DPA_CHIAVI_CONFIG.nextval,'||maxid||'),          0,'''
          ||Codice||''' ,'''||Descrizione   ||''', '''||valore  ||''' ,'''
          ||Tipo_Chiave   ||''', '''||Visibile||''' ,'''
          ||Modificabile  ||''', '''||Globale||''' ,  '''
          ||Codice_Old_Webconfig||''', '''||myVERSIONE_CD||'''   )';
  
  stringa_msg := 'inserita nuova chiave locale ' || Codice ; 
  Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ; 
	End if;           
  
  If (Cnt         = 0 And Globale    = 0) Then -- inserisco la chiave non globale non esistente
      -- questa forma evita di dover eseguire la procedura CREA_KEYS_AMMINISTRA di riversamento !
      -- must use exec immediate since column VERSIONE_CD may not exist yet at compile time
      execute immediate 'INSERT      INTO dpa_chiavi_configurazione
        ( System_Id,          Id_Amm,          
          Var_Codice ,          VAR_DESCRIZIONE ,          VAR_VALORE ,
          Cha_Tipo_Chiave,          Cha_Visibile ,
          CHA_MODIFICABILE,          CHA_GLOBALE, 
          VAR_CODICE_OLD_WEBCONFIG , VERSIONE_CD)
      SELECT greatest(SEQ_DPA_CHIAVI_CONFIG.nextval,'||maxid||') AS system_id,
        Amm.System_Id                      As Id_Amm,'''
        ||Codice||''' As Var_Codice,'''        ||Descrizione||''' As Var_Descrizione ,      '''||Valore||''' As Var_Valore ,'''
        ||Tipo_Chiave||''' As Cha_Tipo_Chiave,       '''||Visibile||''' As Cha_Visibile ,'''
        ||Modificabile||''' As Cha_Modificabile,     '''||Globale||''' as CHA_GLOBALE ,'''
        ||CODICE_OLD_WEBCONFIG||''' as VAR_CODICE_OLD_WEBCONFIG , '''||myVERSIONE_CD||''' as VERSIONE_CD
      FROM Dpa_Amministra Amm
      WHERE NOT EXISTS        (SELECT Id_Amm FROM Dpa_Chiavi_Configurazione Where Var_Codice = '''||Codice||'''  )';
  
  stringa_msg := 'inserita nuove '||SQL%ROWCOUNT ||' chiavi locali per le amministrazioni: ' ||Codice ; 
  Utl_Insert_Log (Nomeutente, sysDATE, stringa_msg, Myversione_Cd ,'ok') ; 
	End If;
  
  If (Cnt                  = 1) Then -- chiave gi esistente
    DBMS_OUTPUT.PUT_LINE ('chiave ' || Codice || ' gi esistente'); 
    
	IF Forza_Update = '1' THEN
      UPDATE Dpa_Chiavi_Configurazione
      SET VAR_DESCRIZIONE = descrizione,
        VAR_VALORE        = valore, 
		Cha_Visibile	  = Visibile,
		Cha_Modificabile  = Modificabile,
		cha_Tipo_Chiave	  = Tipo_Chiave	
      Where Var_Codice    = Codice       and modificabile = '1';

    Stringa_Msg := 'AGGIORNATO VALORE, visibilit, modificabilit e tipo, per la CHIAVE: ' ||Codice || ' gi esistente' ; 
    Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ; 
    
	ELSE -- aggiorno solo la descrizione
      UPDATE Dpa_Chiavi_Configurazione
      SET Var_Descrizione = Descrizione -- , Var_Valore = Valore
      WHERE Var_Codice    = Codice       and modificabile = '1';
    END IF;
  END IF;
  
  
  Exception When Others Then 
    Dbms_Output.put_line('errore da SP: Utl_Insert_Chiave_Config'||SQLERRM); 
    Rollback; 
    Raise; 
  
END;
/


              
---- utl_INSERT_LOG.ORA.SQL  marcatore per ricerca ----
CREATE OR REPLACE PROCEDURE @db_user.utl_insert_log (nomeutente VARCHAR2,
 data_eseguito DATE, comando_eseguito VARCHAR2, versione_CD VARCHAR2, esito VARCHAR2)
IS  cnt int;
MYUSERNAME VARCHAR2(200);
BEGIN
 
SELECT COUNT(*) INTO cnt FROM all_sequences where sequence_name='SEQ_INSTALL_LOG' 
	and sequence_owner= upper(nomeutente);
If (Cnt = 0) Then
        Execute Immediate 'CREATE SEQUENCE '||nomeutente||'.SEQ_INSTALL_LOG START WITH 1   MAXVALUE 99999999999     
					MINVALUE 1  NOCYCLE   NOCACHE   NOORDER';
end IF;

SELECT COUNT(*) INTO cnt FROM user_tables where table_name='DPA_LOG_INSTALL';
IF (cnt = 0) THEN
	select USERNAME INTO MYUSERNAME from user_users;
    RAISE_APPLICATION_ERROR(-20001,'Missing table '||MYUSERNAME||'.DPA_LOG_INSTALL');
end IF;

INSERT INTO DPA_LOG_INSTALL (ID,		DATA_OPERAZIONE,
	COMANDO_RICHIESTO,	VERSIONE_CD,	ESITO_OPERAZIONE) 
VALUES ( SEQ_INSTALL_LOG.nextval  ,NVL (data_eseguito, sysdate)
 ,comando_eseguito ,versione_CD ,esito ) ;
 
 -- pls leave this output here, this is intended for tracing, not for debugging
   Dbms_Output.Put_Line (Comando_Eseguito ) ;  
     
EXCEPTION  WHEN OTHERS  THEN
 DBMS_OUTPUT.put_line ('errore da SP utl_insert_log: ' || SQLERRM);
 RAISE; --manda errore a sp chiamante
END utl_insert_log;
/

              
---- utl_modify_column.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec utl_modify_column('VERSIONE CD', --->es. 3.16.1
--					'NOME UTENTE', --->es. DOCSADM
--					'NOME TABELLA', --->es. DPA_LOG
--					'NOME COLONNA', --->es. COLONNA_A
--					'TIPO DATO', ---> es. INT 4, VARCHAR 200, ECC.
--					'DEFAULT', --->es. 'VERO, FALSO, 0, ECC. MA SI PU LASCIARE VUOTO CON ''
--					'CONDIZIONE MODIFY', ES. ''
--					'RFU' ---> per uso futuro")
-- =============================================
-- Author:		Gabriele Serpi
-- Create date: 21/07/2011
-- Description:	
-- =============================================

CREATE OR REPLACE PROCEDURE @db_user.utl_modify_column 
(
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna                     VARCHAR2,
   tipo_dato                        VARCHAR2,
   val_default                      VARCHAR2,
   condizione_check                 VARCHAR2,
   RFU                              VARCHAR2)
IS
   cnt   INT;
      istruzsql varchar2(200); 
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = nomeutente;

   IF (cnt = 1) THEN    -- ok la tabella esiste
   
      SELECT COUNT ( * )        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna)
             AND owner = nomeutente;
  
       IF (cnt = 1) THEN      -- ok la colonna esiste, la modifico
         
		 if val_default  IS null then 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
						|| ' MODIFY '      || nome_colonna|| ' '|| tipo_dato ; 
		  ELSE --  val_default  IS NOT null 
		  istruzsql  := 'ALTER TABLE '  || nomeutente || '.' || nome_tabella
						|| ' MODIFY '      || nome_colonna|| ' '|| tipo_dato || ' default '|| val_default   ; 

			end if ; 
         EXECUTE IMMEDIATE  istruzsql  ;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Modified column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 
      END IF;
   END IF;
   
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore

   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
    
               dbms_output.put_line ('errore '||SQLERRM);
               utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Modified column '||nome_colonna||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                    --   RAISE;
END utl_modify_column;
/

              
---- utl_rename_column.ORA.SQL  marcatore per ricerca ----
CREATE OR REPLACE PROCEDURE @db_user.utl_rename_column 
(
   versione_CD                      VARCHAR2,
   nomeutente                       VARCHAR2,
   nome_tabella                     VARCHAR2,
   nome_colonna_old            VARCHAR2,
   nome_colonna_new           VARCHAR2,
   RFU                                   VARCHAR2)
IS
   cnt   INT;
BEGIN
   SELECT COUNT ( * )
     INTO cnt
     FROM all_tables
    WHERE table_name = UPPER (nome_tabella) AND owner = nomeutente;

   IF (cnt = 1)
   -- ok la tabella esiste
   THEN
   dbms_output.put_line ('ok entro nell''if ');
      SELECT COUNT ( * )
        INTO cnt
        FROM all_tab_columns
       WHERE     table_name = UPPER (nome_tabella)
             AND column_name = UPPER (nome_colonna_old)
             AND owner = nomeutente;
 dbms_output.put_line ('cnt vale: '||cnt);
      IF (cnt = 1)
      -- ok la colonna esiste, la modifico
      THEN
         dbms_output.put_line ('ok entro nel 2 if ');
         EXECUTE IMMEDIATE   'ALTER TABLE '
                          || nomeutente
                          || '.'
                          || nome_tabella
                          || ' RENAME COLUMN '
                          || nome_colonna_old
                          || ' TO '
                          || nome_colonna_new;
                          
                         utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Renamed column '||nome_colonna_old||' to '||nome_colonna_new||' on table '||nome_tabella
                         , versione_CD
                         , 'esito positivo' ); 
      END IF;
   END IF;
   
    dbms_output.put_line ('sqlerrm: '||sqlerrm);
             
EXCEPTION  -- qui cominciano le istruzioni da eseguire se le SP va in errore
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
     
               dbms_output.put_line ('errore '||SQLERRM);
                dbms_output.put_line ('non eseguito  ');
                  utl_insert_log (nomeutente  -- nome utente
                         , NULL -- data
                         ,'Renamed column '||nome_colonna_old||' to '||nome_colonna_new||' on table '||nome_tabella
                         , versione_CD
                         , 'esito negativo' ); 
                         -- RAISE;
END utl_rename_column;
/              
 
-------------------cartella  TABLE -------------------
              
---- ALTER_DPA_A_R_OGG_CUSTOM_DOC.ORA.SQL  marcatore per ricerca ----
BEGIN
 @db_user.utl_add_column ('3.19.1', '@db_user'
 , 'DPA_A_R_OGG_CUSTOM_DOC', 'DEL_REP', 'NUMBER', NULL, NULL, NULL, NULL);
END; 
/
              
----------- FINE -
              
---- ALTER_DPA_ASSOCIAZIONE_TEMPLATES.ORA.SQL  marcatore per ricerca ----
BEGIN

@db_user.utl_add_column ('3.19.1', '@db_user', 'DPA_ASSOCIAZIONE_TEMPLATES', 'DTA_ANNULLAMENTO', 'DATE', NULL, NULL, NULL, NULL);
END; 
/
              
----------- FINE -
              
---- ALTER_DPA_CHIAVI_CONFIG_TEMPLATE.ORA.SQL  marcatore per ricerca ----
BEGIN
@db_user.UTL_ADD_COLUMN('3.19.1', '@db_user', 'DPA_CHIAVI_CONFIG_TEMPLATE', 'CHA_INFASATO', 'VARCHAR(1)', 'N', NULL, NULL, NULL);
END;
/

begin
    declare            cnt int;
    begin
        select count(*) into cnt 
        from user_constraints, user_cons_columns 
        where user_cons_columns.table_name = 'DPA_CHIAVI_CONFIG_TEMPLATE' 
        and column_name = 'VAR_CODICE'
        and constraint_type='U' 
        and user_cons_columns.constraint_name = user_constraints.constraint_name;
        
        if (cnt = 0) then 
        execute immediate 
            'ALTER TABLE @db_user.DPA_CHIAVI_CONFIG_TEMPLATE 
            ADD ( CONSTRAINT DPA_CHIAVI_CONFIG_TEMPLATE_U01 UNIQUE (VAR_CODICE))' ; 
            
        end if;
    end;
end;
/
              
----------- FINE -
              
---- ALTER_DPA_CORR_GLOBALI.ORA.SQL  marcatore per ricerca ----
BEGIN

@db_user.UTL_ADD_COLUMN('3.19.1', '@db_user', 'DPA_CORR_GLOBALI', 'VAR_INSERT_BY_INTEROP', 'CHAR(1)', NULL, NULL, NULL, NULL); 
END; 
/

              
----------- FINE -
              
---- ALTER_DPA_CUSTOM_COMP_FASC.ORA.SQL  marcatore per ricerca ----
BEGIN

@db_user.utl_add_column ('3.19.1', '@db_user'
, 'dpa_ogg_custom_comp_fasc', 'Enabledhistory', 'CHAR(1)'
, '0' -- val_default
, NULL, NULL, NULL);
END; 
/
              
----------- FINE -
              
---- ALTER_DPA_CUSTOM_COMP.ORA.SQL  marcatore per ricerca ----
BEGIN

@db_user.utl_add_column ('3.19.1', '@db_user'
, 'dpa_ogg_custom_comp', 'Enabledhistory', 'CHAR(1)'
, '0' -- val_default
, NULL, NULL, NULL); 
END; 
/

              
----------- FINE -
              
---- ALTER_DPA_FORMATI_DOCUMENTO.ORA.SQL  marcatore per ricerca ----
/*
AUTORE:						FAILLACE
Data creazione:				07/11/2011
Scopo della modifica:		INSERITI CAMPO "FILE_TYPE_VALIDATION" PER LA GESTIONE DELLA VERIFICA TIPO FILE

Indicazione della MEV o dello sviluppo a cui  collegata la modifica:
							CREATA PER LA MEV "CONSERVAZIONE"
*/
BEGIN
 @db_user.utl_add_column ('3.19.1', '@db_user', 'DPA_FORMATI_DOCUMENTO', 'FILE_TYPE_VALIDATION', 'INTEGER', NULL, NULL, NULL, NULL);
END; 
/              
----------- FINE -
              
---- ALTER_DPA_NOTIFICA.ORA.sql  marcatore per ricerca ----
--ALTER TABLE DPA_NOTIFICA ADD COLUMN VERSION_ID Number(10,0) default 0 ; 

-- default 0 deve valere  anche per eventuali record gi presenti
BEGIN

@db_user.utl_add_column ('3.19.1', '@db_user', 'DPA_NOTIFICA', 'VERSION_ID', 'Number(10,0)','0','','','');
END; 
/

              
----------- FINE -
              
---- ALTER_PEOPLE.ORA.SQL  marcatore per ricerca ----
BEGIN

@db_user.UTL_ADD_COLUMN('3.19.1', '@db_user', 'PEOPLE', 'ABILITATO_CENTRO_SERVIZI', 'CHAR(1 BYTE)', NULL, NULL, NULL, NULL);
END; 
/
              
----------- FINE -
              
---- CREATE_DPA_REGISTRI_REPERTORIO.ORA.SQL  marcatore per ricerca ----
--Creazione Tabella DPA_REGISTRI_REPERTORIO
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables where owner=upper('@db_user') and table_name='DPA_REGISTRI_REPERTORIO';
        if (cnt = 0) then
          execute immediate    
        'CREATE TABLE @db_user.DPA_REGISTRI_REPERTORIO  
        (TipologyId            Number      Not Null,
  CounterId             Number      Not Null,
  CounterState          VarChar(1)  Not Null,
  SettingsType          Varchar(1) ,
  RegistryId            Number,
  RfId                  Number,
  RoleRespId            Number,
  PrinterRoleRespId         Number,
  PrinterUserRespId     Number,
  PrintFreq             Varchar2(2) Not Null,
  TipologyKind          Varchar2(2) Not Null,
  DtaStart              Date,
  DtaFinish             Date,
  DtaNextAutomaticPrint Date,
  DtaLastPrint          Date,
  LastPrintedNumber     Number,
  RespRights    Varchar2(2)) ' ;
        end if;
    end;    
end;    
/
              
----------- FINE -
              
---- CREATE_DPA_STAMPA_REPERTORI.ORA.SQL  marcatore per ricerca ----
--Creazione Tabella DPA_STAMPA_REPERTORI
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables where owner='@db_user' and table_name='DPA_STAMPA_REPERTORI';
        if (cnt = 0) then
          execute immediate    
        'CREATE TABLE @db_user.DPA_STAMPA_REPERTORI
  (
    SYSTEM_ID     NUMBER,
    ID_REPERTORIO NUMBER,
    NUM_REP_START NUMBER,
    NUM_REP_END   NUMBER,
    NUM_ANNO      NUMBER,
    DOCNUMBER     NUMBER,
    DTA_STAMPA DATE,
    RegistryId Numeric
  )
' ;
        end if;
    end;    
end;    
/
              
----------- FINE -
              
---- CREATE_INDEX_DPA_FORMATI_DOCUMENTO.ORA.sql  marcatore per ricerca ----
begin
    declare cnt int;
    isSQL varchar2(2000);
    begin
      select  count (*) into cnt 
      from user_ind_columns
      where column_name  = 'ID_AMMINISTRAZIONE' 
      and index_name in (
                        select index_name   
                        from user_indexes 
                        where table_name = 'DPA_FORMATI_DOCUMENTO' );

if (cnt = 0) then
isSQL := 'CREATE INDEX INDEX_DPA_FORMATI ON DPA_FORMATI_DOCUMENTO (ID_AMMINISTRAZIONE)';

execute immediate isSQL;   

end if;
end;
end;
/


              
----------- FINE -
              
---- CREATE_MV_DOCUMENTI_CUSTOM.sql  marcatore per ricerca ----
begin 
declare       
      mvlog_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (mvlog_esistente, -12000);
  
      mv_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (mv_esistente, -12006);
  
	    obj_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (obj_esistente, -955);
  
      createmvlog_on varchar2(100) := 'CREATE MATERIALIZED VIEW LOG ON '; -- completare con nome tabella es. DPA_OGG_CUSTOM_COMP
      opzioni_mvlog  varchar2(100) := ' NOPARALLEL WITH ROWID, PRIMARY KEY EXCLUDING NEW VALUES ';
      istruzione     varchar2(4000) ;
   BEGIN
  
      BEGIN
        istruzione := createmvlog_on  || 'DPA_OGG_CUSTOM_COMP' || opzioni_mvlog ;
        EXECUTE IMMEDIATE istruzione;
       EXCEPTION  
        WHEN mvlog_esistente THEN     
        dbms_output.put_line('mvlog gi esistente');
        when others then  
      RAISE; 
      dbms_output.put_line('errore creazione MV');
      END; 
   
   BEGIN
      istruzione := createmvlog_on  || 'DPA_OGGETTI_CUSTOM' || opzioni_mvlog ;
      EXECUTE IMMEDIATE istruzione;
    EXCEPTION  
    WHEN mvlog_esistente THEN     
      dbms_output.put_line('mvlog gi esistente');
   END; 
      
   BEGIN
      istruzione := createmvlog_on  || 'DPA_TIPO_ATTO' || opzioni_mvlog ;
      EXECUTE IMMEDIATE istruzione;
    EXCEPTION  
    WHEN mvlog_esistente THEN     
      dbms_output.put_line('mvlog gi esistente');
      when others then  
      RAISE; 
      dbms_output.put_line('errore creazione MV');
   END; 
    
   BEGIN
      istruzione := createmvlog_on  || 'DPA_TIPO_OGGETTO' || opzioni_mvlog ;
      EXECUTE IMMEDIATE istruzione;
    EXCEPTION  
    WHEN mvlog_esistente THEN     
      dbms_output.put_line('mvlog gi esistente');
   END; 
      
   BEGIN
      istruzione := createmvlog_on  || 'dpa_associazione_templates' || opzioni_mvlog ;
      EXECUTE IMMEDIATE istruzione;
    EXCEPTION  
    WHEN mvlog_esistente THEN     
      dbms_output.put_line('mvlog gi esistente');
      when others then  
      RAISE; 
      dbms_output.put_line('errore creazione MV');
   END; 
      
      
-- crea MV documenti custom  

/* -- modalit periodica di refresh alle 22 di ogni giorno si fa con questa sintassi:
REFRESH FAST ON DEMAND with rowid 
start with trunc(sysdate) + 22/24
next trunc(sysdate+1) + 22/24					*/ 
    begin
     istruzione := ' CREATE MATERIALIZED VIEW MV_DOCUMENTI_CUSTOM 
    COMPRESS LOGGING  BUILD IMMEDIATE USING INDEX 
	REFRESH FAST ON DEMAND 
	USING DEFAULT LOCAL ROLLBACK SEGMENT 
    ENABLE QUERY REWRITE                                   AS
    SELECT dpa_associazione_templates.ROWID  AS asstrowid,
    dpa_oggetti_custom.ROWID               AS oggcrowid,
    dpa_ogg_custom_comp.ROWID              AS oggccrowid,
    dpa_tipo_atto.ROWID                    AS tarowid,
    dpa_tipo_oggetto.ROWID                 AS torowid,
    dpa_associazione_templates.id_oggetto  AS system_id_ogg_custom,
    dpa_tipo_atto.system_id                AS system_id_tipo_atto,
    dpa_associazione_templates.id_template AS system_id_template,
    dpa_ogg_custom_comp.system_id          AS system_id_ogg_custom_comp,
    dpa_tipo_oggetto.system_id             AS system_id_tipo_oggetto,
    dpa_tipo_atto.var_desc_atto,
    dpa_tipo_atto.abilitato_si_no,
    dpa_tipo_atto.in_esercizio,
    dpa_tipo_atto.path_mod_1,
    dpa_tipo_atto.path_mod_2,
    dpa_tipo_atto.path_mod_su,
    dpa_tipo_atto.path_mod_exc,
    dpa_tipo_atto.path_all_1,
    dpa_tipo_atto.gg_scadenza,
    dpa_tipo_atto.gg_pre_scadenza,
    dpa_tipo_atto.cha_privato,
    dpa_tipo_atto.id_amm,
    dpa_tipo_atto.cod_class,
    dpa_tipo_atto.cod_mod_trasm,
    dpa_tipo_atto.iperdocumento,
    dpa_associazione_templates.doc_number,
    valore_oggetto_db,
    dpa_associazione_templates.codice_db,
    dpa_associazione_templates.manual_insert,
    dpa_associazione_templates.anno,
    dpa_associazione_templates.id_aoo_rf,
    dpa_associazione_templates.valore_sc,
    dta_ins,
    dpa_associazione_templates.DTA_ANNULLAMENTO ,
    dpa_oggetti_custom.campo_di_ricerca,
    dpa_oggetti_custom.campo_obbligatorio,
    dpa_oggetti_custom.descrizione,
    dpa_oggetti_custom.multilinea,
    dpa_oggetti_custom.numero_di_caratteri,
    dpa_oggetti_custom.numero_di_linee,
    dpa_oggetti_custom.orizzontale_verticale,
    dpa_oggetti_custom.reset_anno,
    dpa_oggetti_custom.formato_contatore,
    dpa_oggetti_custom.ricerca_corr,
    dpa_oggetti_custom.id_r_default,
    dpa_oggetti_custom.campo_comune,
    dpa_oggetti_custom.cha_tipo_tar,
    dpa_oggetti_custom.conta_dopo,
    dpa_oggetti_custom.repertorio,
    dpa_oggetti_custom.da_visualizzare_ricerca,
    dpa_oggetti_custom.formato_ora,
    dpa_oggetti_custom.tipo_link,
    dpa_oggetti_custom.tipo_obj_link,
    dpa_oggetti_custom.modulo_sottocontatore,
    dpa_ogg_custom_comp.ENABLEDHISTORY ,
    dpa_ogg_custom_comp.posizione,
    dpa_ogg_custom_comp.ID_TEMPLATE ,
    dpa_tipo_oggetto.descrizione AS descrizione_tipo
  FROM dpa_associazione_templates,
    dpa_oggetti_custom,
    dpa_ogg_custom_comp,
    dpa_tipo_atto,
    dpa_tipo_oggetto
  WHERE dpa_tipo_atto.system_id             = dpa_associazione_templates.id_template
  AND dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
  AND dpa_oggetti_custom.system_id          = dpa_ogg_custom_comp.id_ogg_custom
  AND dpa_oggetti_custom.id_tipo_oggetto    = dpa_tipo_oggetto.system_id ';
       EXECUTE IMMEDIATE istruzione;
         EXCEPTION  
        WHEN mv_esistente THEN     
        dbms_output.put_line('mv gi esistente');
        when others then  
      RAISE; 
      dbms_output.put_line('errore creazione MV');
      end;

-- crea indice
istruzione := ' create index indx_mv_custom_docnum on  mv_documenti_custom(doc_number)'; 
EXECUTE IMMEDIATE istruzione;

 

   EXCEPTION  
    WHEN obj_esistente THEN     
      dbms_output.put_line('mv gi esistente');
	
    when others then  
      RAISE; 
      dbms_output.put_line('errore creazione MV');
end; 
end; 
/


--  COMMENT ON MATERIALIZED VIEW MV_DOCUMENTI_CUSTOM IS   'snapshot table for snapshot MV_DOCUMENTI_CUSTOM';
              
----------- FINE -
              
---- CREATE_PUBBLICAZIONI_DOCUMENTI.ORA.SQL  marcatore per ricerca ----
--Creazione Tabella PUBBLICAZIONI_DOCUMENTI
begin
    declare cnt int;
    begin
        select count(*) into cnt from all_tables 
               where upper(owner)=upper('@db_user')
                and table_name='PUBBLICAZIONI_DOCUMENTI';
        if (cnt = 0) then
          execute immediate  
'CREATE TABLE @db_user.PUBBLICAZIONI_DOCUMENTI'||

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


              
----------- FINE -
              
---- DOMAIN_INDEX__ON_CORR_GLOBALI_VAR_DESC_CORR.ORA.sql  marcatore per ricerca ----
--
-- INDX_DESC_CORR_TEXT   (full Text Index) 

-- requires that role CTXAPP already been granted to ;
-- role must be granted by DBA:
-- SQL> grant CTXAPP to <username> ; 
-- es.  grant CTXAPP to PAT_PROD ; 


-- sync on commit option implies that the index needs to be OPTIMIZED on a regular basis
-- to this end, UTL_OPTIMIZE_CONTEXT_INDEX procedure (inside package UTILITA) had been realized
-- this proc must be scheduled at least once per day, by the DBA of the customer, off working hours.

-- invocation is:
-- Sql> execute utilita.UTL_OPTIMIZE_CONTEXT_INDEX (2,'FULL') ;  -- FULL optimizes ALL CONTEXT indexes


BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'CREATE INDEX INDX_DESC_CORR_TEXT ON DPA_CORR_GLOBALI(VAR_DESC_CORR)
				INDEXTYPE IS CTXSYS.CONTEXT 
				PARAMETERS (''sync (on commit) stoplist ctxsys.empty_stoplist'') ';

      obj_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (obj_esistente   , -955);

	  index_esiste_altroNome  EXCEPTION;
      PRAGMA EXCEPTION_INIT (index_esiste_altroNome , -29879);

	   mygranted_role VARCHAR2(20); 
   BEGIN
      
	  BEGIN -- check if role CTXAPP was granted
	  Select granted_role into mygranted_role 
			From User_role_Privs  where granted_role= 'CTXAPP' ; 
	  IF (nvl(mygranted_role,'null') <> 'CTXAPP' ) THEN  
            RAISE_APPLICATION_ERROR(-20001,'Missing CTXAPP role; CTXAPP role must be granted by DBA'); 
        end IF;
	  exception when NO_DATA_FOUND then 
                    RAISE_APPLICATION_ERROR(-20001,'Missing CTXAPP role; CTXAPP role must be granted by DBA'); 
              when others then RAISE; 
	  END; 
	  
	  EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN obj_esistente   
      THEN
         DBMS_OUTPUT.put_line ('indice gi esistente');
      WHEN index_esiste_altroNome  
      THEN
         DBMS_OUTPUT.put_line ('indice gi esistente con altro nome');
	  WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/
              
----------- FINE -
              
---- DOMAIN_INDEX__ON_PROFILE_VAR_PROF_OGGETTO.ORA.sql  marcatore per ricerca ----
--
-- VAR_PROF_OGGETTO_CTXCAT  (full Text Index) 

-- requires that role CTXAPP already been granted to ;
-- role must be granted by DBA:
-- SQL> grant CTXAPP to <username> ; 
-- es.  grant CTXAPP to PAT_PROD ; 

-- sync on commit option implies that the index needs to be OPTIMIZED on a regular basis
-- to this end, UTL_OPTIMIZE_CONTEXT_INDEX procedure (inside package UTILITA) had been realized
-- this proc must be scheduled at least once per day, by the DBA of the customer, off working hours.

-- Es. Sql> execute utilita.UTL_OPTIMIZE_CONTEXT_INDEX (2,'FULL') ;  -- FULL optimizes ALL CONTEXT indexes


BEGIN
   DECLARE
      istruzione VARCHAR2 (2000)
         := 'CREATE INDEX INDX_OGG_TEXT ON PROFILE(VAR_PROF_OGGETTO)
			INDEXTYPE IS CTXSYS.CONTEXT 
			PARAMETERS (''sync (on commit) stoplist ctxsys.empty_stoplist'') ';

      obj_esistente   EXCEPTION;
      PRAGMA EXCEPTION_INIT (obj_esistente   , -955);

	  index_esiste_altroNome  EXCEPTION;
      PRAGMA EXCEPTION_INIT (index_esiste_altroNome , -29879);
	  
	  mygranted_role VARCHAR2(20); 
	   
   BEGIN
      
	  BEGIN -- check if role CTXAPP was granted
	  Select granted_role into mygranted_role 
			From User_role_Privs  where granted_role= 'CTXAPP' ; 
	  IF (nvl(mygranted_role,'null') <> 'CTXAPP' ) THEN  
            RAISE_APPLICATION_ERROR(-20001,'Missing CTXAPP role; CTXAPP role must be granted by DBA'); 
        end IF;
	  exception when NO_DATA_FOUND then 
                    RAISE_APPLICATION_ERROR(-20001,'Missing CTXAPP role; CTXAPP role must be granted by DBA'); 
              when others then RAISE; 
	  END; 
	  

	  EXECUTE IMMEDIATE istruzione;

   EXCEPTION
      WHEN obj_esistente   
      THEN
         DBMS_OUTPUT.put_line ('tabella gi esistente');
      WHEN index_esiste_altroNome  
      THEN
         DBMS_OUTPUT.put_line ('indice gi esistente con altro nome');
	  WHEN OTHERS
      THEN
         RAISE;
         DBMS_OUTPUT.put_line ('KO');
   END;
END;
/
              
----------- FINE -
              
 
-------------------cartella  SEQUENCE -------------------
              
---- SEQ_DPA_PROFIL_STO.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_PROFIL_STO';
    IF (cnt = 0) THEN        
       execute immediate 'create SEQUENCE SEQ_DPA_PROFIL_STO START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER ';
    END IF;
END;        
END;
/
              
----------- FINE -
              
---- SEQ_DPA_STAMPA_REPERTORI.ORA.SQL  marcatore per ricerca ----
BEGIN
    DECLARE cnt int;
BEGIN    
    SELECT COUNT(*) INTO cnt FROM user_sequences 
                              where sequence_name='SEQ_DPA_STAMPA_REPERTORI';
    IF (cnt = 0) THEN        
       execute immediate 'CREATE SEQUENCE @db_user.SEQ_DPA_STAMPA_REPERTORI 
       MINVALUE 1  MAXVALUE 9999999999999999999999999999 
       INCREMENT BY 1  START WITH 1130 CACHE 20 NOORDER NOCYCLE ';
    END IF;
END;        
END;
/


              
----------- FINE -
              
 
-------------------cartella  TRIGGER -------------------
              
---- ModCountDocInMngrTbl.ORA.SQL  marcatore per ricerca ----
create or replace
TRIGGER @db_user.ModCountDocInMngrTbl 
BEFORE Update ON @db_user.DPA_OGGETTI_CUSTOM 
FOR EACH ROW 
WHEN (new.repertorio != old.repertorio Or new.cha_tipo_tar != old.cha_tipo_tar)
Declare 
  idTipologia number;
BEGIN
  /******************************************************************************
  
    AUTHOR:    Samuele Furnari

    NAME:      ModCountInMngrTbl

    PURPOSE:   Ogni volta che viene modificato il flag repertorio o il valore 
               che indica la tipologia di repertorio, bisogna agire sull'anagrafica
 
  ******************************************************************************/
  
  -- Eliminazione dei riferimenti del repertorio dall'anagrafica
  DeleteRegistroRepertorio(:new.system_id);
  
  Select ta.system_id Into idTipologia 
  From @db_user.dpa_tipo_atto ta 
  Inner Join @db_user.dpa_ogg_custom_comp occ 
  On ta.system_id = occ.id_template
  Where occ.id_ogg_custom = :new.system_id;
  
  -- Se  stato cambiato lo stato del flag repertorio, viene ed  stato passato
  -- ad 1, viene eseguito l'inserimento di un riferimento nell'anagrafica
  If :new.repertorio = '1' Then
    @db_user.InsertRegistroRepertorio(idTipologia, :new.system_id, :new.cha_tipo_tar, 'D');
  End If;
  
End;
/
              
----------- FINE -
              
---- RemoveRegFromRepertoriTable.ORA.SQL  marcatore per ricerca ----
create or replace trigger @db_user.RemoveRegFromRepertoriTable 
after delete on @db_user.dpa_el_registri 
referencing old as old new as new 
for each row 
begin
  /******************************************************************************
      AUTHOR:    Samuele Furnari
      NAME:      RemoveRegFromRepertoriTable
      PURPOSE:   Questo trigger in ascolto sulla dpa_el_registri scatta ogni volta
                 che viene eliminato un record dalla tabella dei registri / rf.
                 
    ******************************************************************************/
    Delete From @db_user.Dpa_Registri_Repertorio Where RegistryId = :old.system_id Or RfId = :old.system_id;
End;
/
              
----------- FINE -
              
---- UpdateRepertoriTable.ORA.SQL  marcatore per ricerca ----
Create Or Replace Trigger @db_user.UpdateRepertoriTable After
  Insert On @db_user.Dpa_El_Registri For Each Row Begin
    /******************************************************************************
      AUTHOR:    Samuele Furnari
      NAME:      UpdateRepertoriTable
      PURPOSE:   Questo trigger in ascolto sulla dpa_el_registri scatta ogni volta
                 che viene aggiunto un registro o un RF e serve per aggiungere ad
                 ogni contatore di repertorio di tipo AOO / RF, un riferimento
                 al nuovo registro / RF
    ******************************************************************************/
    -- Cursore per scorrere tutti i repertori di RF
    Declare Cursor RepertoriCursor (Mycha_Tipo_Tar Varchar) Is
    (Select Oc.System_Id
    From @db_user.Dpa_Oggetti_Custom Oc
    Where Repertorio     = '1'
    And Cha_Tipo_Tar     = Mycha_Tipo_Tar -- 'R' o 'A'
    And Id_Tipo_Oggetto In
      (Select System_Id
      From @db_user.Dpa_Tipo_Oggetto
      Where Lower(Descrizione) = 'contatore'
      )
    );
  -- Id del repertorio
  RepId Number;
  -- Tipo di impostazioni scelte per lo specifico repertorio
  SettType Varchar (1);
  -- Id del registro e dell'RF
  Registry Number;
  Rf       Number;
  -- Sigla identificativa del tipo di repertorio da modificare (A o R)
  RepType Varchar (1);
  Begin
    -- Inizializzazione del cursore per scorrere i repertori a seconda
    -- che si stia inserendo un RF o un Registro
    If :New.Cha_Rf = '1' Then
      RepType     := 'R';
    Else
      RepType := 'A';
    End If ;
    Begin
      Open RepertoriCursor(RepType);
      Loop
        Fetch RepertoriCursor Into RepId;
        Exit
      When RepertoriCursor%NotFound;
        -- Se si sta inserendo un registro viene inizializzato
        -- il parametro registry altrimenti viene valorizzato il
        -- parametro rf
        If :New.Cha_Rf = '0' Then
          Begin
            Registry := :New.System_Id;
            Rf       := Null;
          End;
        Else
          Begin
            Registry := Null;
            Rf       := :New.System_Id;
          End;
        End If ;
        -- Selezione delle impostazioni relative al contatore in esame
        -- (viene prelevata la prima istanza in quanto una qualsiasi istanza
        --  sufficiente per determinare come procedere
        Begin
          Select SettingsType
          Into SettType
          From @db_user.Dpa_Registri_Repertorio
          Where CounterId = RepId
          And Rownum      = 1;
        Exception
        When Others Then
          SettType := '';
        End;
        Begin
          If SettType = 'G' Then
            Begin
              -- Se il tipo di impostazione  G, viene inserita nell'anagrafica una riga uguale
              -- alla prima impostazione del repertorio con la data di ultima stampa impostata a null
              -- e con l'ultimo numero stampato impostato a 0
              Insert
              Into @db_user.Dpa_Registri_Repertorio
                (
                  Tipologyid,
                  Counterid,
                  Counterstate,
                  Settingstype,
                  Registryid,
                  Rfid,
                  Rolerespid,
                  Printerrolerespid,
                  Printeruserrespid,
                  Printfreq,
                  Tipologykind,
                  Dtastart,
                  Dtafinish,
                  Dtanextautomaticprint,
                  Dtalastprint,
                  Lastprintednumber,
                  Resprights
                )
                (Select Tipologyid,
                    CounterId,
                    CounterState,
                    SettingsType,
                    Registry,
                    Rf,
                    RolerespId,
                    PrinterRoleRespId,
                    PrinterUserRespId,
                    PrintFreq,
                    TipologyKind,
                    DtaStart,
                    DtaFinish,
                    DtaNextAutomaticPrint,
                    Null,
                    0,
                    RespRights
                  From @db_user.Dpa_Registri_Repertorio
                  Where CounterId = RepId
                  And Rownum      = 1
                );
            Exception
            When Others Then
              SettType := '';
            End;
          Else
            Begin
              If SettType = 'S' Then
                Begin
                  -- Altrimenti le impostazioni sono singole per ogni repertorio.
                  -- In questo caso, viene inserita una riga uguale alla prima istanta
                  -- di configurazione relativa al repertorio ad esclusione di:
                  --    - stato che viene impostato ad Aperto
                  --    - rfid / registry id che vengono impostati a seconda del registro inserito
                  --    - ultimo numero stampato che viene impostato a 0
                  --    - date che vengono impostate tutte a null
                  --    - respondabili che vengono impostati a null
                  --    - frequenza di stampa automatica che viene impostata ad N
                  Insert
                  Into @db_user.Dpa_Registri_Repertorio
                    (
                      Tipologyid,
                      Counterid,
                      Counterstate,
                      Settingstype,
                      Registryid,
                      Rfid,
                      Rolerespid,
                      Printerrolerespid,
                      Printeruserrespid,
                      Printfreq,
                      Tipologykind,
                      Dtastart,
                      Dtafinish,
                      Dtanextautomaticprint,
                      Dtalastprint,
                      Lastprintednumber,
                      Resprights
                    )
                    (Select Tipologyid,
                        Counterid,
                        Counterstate,
                        Settingstype,
                        Registry,
                        Rf,
                        Null,
                        Null,
                        Null,
                        'N',
                        Tipologykind,
                        Null,
                        Null,
                        Null,
                        Null,
                        0,
                        Resprights
                      From @db_user.Dpa_Registri_Repertorio
                      Where Counterid = Repid
                      And Rownum      = 1
                    );
                Exception
                When Others Then
                  Setttype := '';
                End;
              End If;
            End;
          End If;
        End;
      End Loop;
    End;
    Close Repertoricursor;
  End;
End;
/
              
----------- FINE -
              
 
-------------------cartella  INSERT_UPDATE_DATA -------------------
              
---- DPA_CHIAVI_CONFIG_TEMPLATE.ORA.sql  marcatore per ricerca ----
begin

Utl_Insert_Chiave_Config('VIS_SEGNATURA_REPERTORI'
   , 'Visualizza in to do list la segnatura di repertorio (0 = disattivato; 1 = attivato)'
   , '0'   , 'B'            
   , '1'           , '1'     , '0'
   , '3.19.1'
   , NULL, NULL, NULL); -- Codice_Old_Webconfig ,Forza_Update   , RFU

Utl_Insert_Chiave_Config( 'CHECK_MITT_INTEROPERANTE'
   , 'Chiave utilizzata per abilitare un controllo qualora un corrispondente esterno scriva per la prima volta alla casella PEC di AOO/RF'
   , '0'		, 'N'			
   , '1'           , '1'     , '0'
   , '3.19.1'
   , NULL, NULL, NULL); -- Codice_Old_Webconfig ,Forza_Update   , RFU

Utl_Insert_Chiave_Config('CHECK_MAILBOX_INTEROPERANTE'
   , 'Chiave utilizzata per abilitare un controllo qualora un corrispondente esterno scriva alla casella PEC di AOO/RF con un indirizzo di posta elettronica identico ad un altro corrispondente censito.'
   , '0'		, 'N'			
   , '1'           , '1'     , '0'
   , '3.19.1'
   , NULL, NULL, NULL); -- Codice_Old_Webconfig ,Forza_Update   , RFU

end;
/


              
----------- FINE -
              
---- INSERT_dpa_anagrafica_funzioni.ORA.SQL  marcatore per ricerca ----
begin
declare cnt int;
begin
select count(*) into cnt from @db_user.dpa_anagrafica_funzioni l where l.COD_FUNZIONE ='GEST_REGISTRO_REPERTORIO';

if (cnt = 0) then
	insert into @db_user.dpa_anagrafica_funzioni(COD_FUNZIONE,
		VAR_DESC_FUNZIONE, 	CHA_TIPO_FUNZ, 	DISABLED)
	Values('GEST_REGISTRO_REPERTORIO', 'Permette di abilitare/disabilitare la voce di menu registro di repertorio', Null, 'N');
END IF;
END;
END;
/
              
----------- FINE -
              
---- INSERT_DPA_CHIAVI_CONFIGURAZIONE.ORA.SQL  marcatore per ricerca ----
-- FE_AUTOMATIC_SCAN
begin
declare cnt int;
begin
	select count(*) into cnt 
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='FE_AUTOMATIC_SCAN';

if (cnt = 0) then
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(system_id,
	id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale	)
	values 
	(SEQ_DPA_CHIAVI_CONFIG.nextval,
	0,
	'FE_AUTOMATIC_SCAN',
	'Pu assumere i valori 0 o 1 e serve per abilitare la partenza automatica del driver Twain durante l''acquisizione di un documento',
	'0',
	'F',
	'1',
	'1',
	'1');
end if;
end;
end;
/


-- FE_NEW_RUBRICA_VELOCE Utilizzata per abilitare la rubrica ajax sul campo descrizione del corrispondente

begin
declare cnt int;
begin
	select count(*) into cnt 
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='FE_NEW_RUBRICA_VELOCE';

if (cnt = 0) then
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(system_id,
	id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale	)
	values 
	(SEQ_DPA_CHIAVI_CONFIG.nextval,
	0,
	'FE_NEW_RUBRICA_VELOCE',
	'Utilizzata per abilitare la rubrica ajax sul campo descrizione del corrispondente',
	'0',
	'F',
	'1',
	'1',
	'1');
end if;
end;
end;
/








--Creazione chiave di configurazione 

-- USE_TEXT_INDEX
begin
declare cnt int;
begin
	select count(*) into cnt 
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='USE_TEXT_INDEX';

if (cnt = 1) then
	update @db_user.DPA_CHIAVI_CONFIGURAZIONE
	set var_descrizione = 	'Chiave utilizzata per abilitare uso degli indici testuali su oggetto nelle ricerche documenti.'
	,	var_valore = '0'
	where var_codice ='USE_TEXT_INDEX';
end if;

if (cnt = 0) then
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(system_id,	id_amm,
	var_codice
	,	var_descrizione
	,	var_valore
	,	cha_tipo_chiave,
	cha_visibile,	cha_modificabile,
	cha_globale)
	values 
	(SEQ_DPA_CHIAVI_CONFIG.nextval,	0,
	 'USE_TEXT_INDEX'
	 , 'Chiave utilizzata per abilitare uso degli indici testuali su oggetto nelle ricerche documenti.'
	 , '0'
	 , 'B'
	 , '1', '1'
	 , '1')	;
end if;
end;
end;
/






-- 'FULLTEXT_SPECIAL_CHARACTERS'
begin
declare cnt int;
begin
	select count(*) into cnt 
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='FULLTEXT_SPECIAL_CHARACTERS';

if (cnt = 0) then
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(system_id,	id_amm,
	var_codice
	,	var_descrizione
	,	var_valore
	,	cha_tipo_chiave,
	cha_visibile,	cha_modificabile,
	cha_globale)
	values 
	(SEQ_DPA_CHIAVI_CONFIG.nextval,	0,
	 'FULLTEXT_SPECIAL_CHARACTERS'
	 , 'Elenco dei caratteri speciali per le ricerche'
	 , '- ~ | & ( ) [ ] { } , = > \ * ; $ ! _'
	 , 'B'
	 , '1', '1'
	 , '1')	;
end if;
if (cnt = 1) then
	update @db_user.DPA_CHIAVI_CONFIGURAZIONE
	set var_valore = '- ~ | & ( ) [ ] { } , = > \ * ; $ ! _'
	where var_codice = 'FULLTEXT_SPECIAL_CHARACTERS' ; 
end if;


end;
end;
/

begin
declare cnt int;
begin
	select count(*) into cnt 
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='FULLTEXT_ESCAPE_CHARACTER';

if (cnt = 0) then
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(system_id,	id_amm,
	var_codice
	,	var_descrizione
	,	var_valore
	,	cha_tipo_chiave,	cha_visibile
	,	cha_modificabile,	cha_globale)
	values 
	(SEQ_DPA_CHIAVI_CONFIG.nextval,	0
	, 'FULLTEXT_ESCAPE_CHARACTER'
	, 'Carattere da usare come escape per le ricerche'
	, '\'
	, 'B', '1'
	, '1', '1');
end if;
end;
end;
/


begin
declare cnt int;
begin
	select count(*) into cnt 
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='BE_CHECK_INTEROP_DTD';

if (cnt = 0) then
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(system_id,
	id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale,
	var_codice_old_webconfig )
	values 
	(SEQ_DPA_CHIAVI_CONFIG.nextval,
	0,
	'BE_CHECK_INTEROP_DTD',
	'Verifica DTD online',
	'0',
	'B',
	'1',
	'1',
	'1',
	null);
end if;
end;
end;
/


begin
declare cnt int;
begin
	select count(*) into cnt 
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='GESTIONE_REPERTORI';

if (cnt = 0) then
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	(system_id,
	id_amm,
	var_codice,
	var_descrizione,
	var_valore,
	cha_tipo_chiave,
	cha_visibile,
	cha_modificabile,
	cha_globale,
	var_codice_old_webconfig )
	values 
	(SEQ_DPA_CHIAVI_CONFIG.nextval,
	0,
	'GESTIONE_REPERTORI',
	'La chiave abilita o meno la gestione dei repertori',
	'0',
	'B',
	'1',
	'1',
	'1',
	null);
end if;
end;
end;
/

-- VIS_SEGNATURA_REPERTORI  non  globale e va inserita in DPA_CHIAVI_CONFIG_TEMPLATE!


begin
declare cnt int;
begin
	select count(*) into cnt 
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='FE_COPIA_VISIBILITA';

if (cnt = 0) then
INSERT
INTO @db_user.DPA_CHIAVI_CONFIGURAZIONE
  ( SYSTEM_ID,
    ID_AMM,
    VAR_CODICE,
    VAR_DESCRIZIONE,
    VAR_VALORE,
    CHA_TIPO_CHIAVE,
    CHA_VISIBILE,
    CHA_MODIFICABILE,
    CHA_GLOBALE,
    VAR_CODICE_OLD_WEBCONFIG
  )
  VALUES
  ( SEQ_DPA_CHIAVI_CONFIG.NEXTVAL,
    0,
    'FE_COPIA_VISIBILITA',
    'Abilita il pulsante per la copia della visibilit',
    '0',
    'F',
    '1',
    '1',
    '1',
    null  );
END IF;
end;
END;
/

begin
declare cnt int;
begin
	select count(*) into cnt 
	from @db_user.DPA_CHIAVI_CONFIGURAZIONE l 
	where l.var_codice ='FE_GEST_RUOLI_AVANZATA';

if (cnt = 0) then
INSERT INTO @db_user.DPA_CHIAVI_CONFIGURAZIONE
  ( SYSTEM_ID,
    ID_AMM,
    VAR_CODICE,
    VAR_DESCRIZIONE,
    VAR_VALORE,
    CHA_TIPO_CHIAVE,
    CHA_VISIBILE,
    CHA_MODIFICABILE,
    CHA_GLOBALE,
    VAR_CODICE_OLD_WEBCONFIG
  )
  VALUES
  ( SEQ_DPA_CHIAVI_CONFIG.NEXTVAL,
    0,
    'FE_GEST_RUOLI_AVANZATA',
    'Abilita la gestione avanzata ruoli che consente di modificare il tipo ruolo e di cambiare il tipo ruolo',
    '0',
    'F',
    '1',
    '1',
    '1',
    null  );
  END IF;
end;
END;
/


-- BE_RIC_MITT_INTEROP_BY_MAIL_DESC
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC';
    IF (cnt = 0) THEN       
		insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
		   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
		   ,VAR_VALORE
		   ,CHA_TIPO_CHIAVE,CHA_VISIBILE
		   ,CHA_MODIFICABILE,CHA_GLOBALE)
				 values
		  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,
		'BE_RIC_MITT_INTEROP_BY_MAIL_DESC'
		  ,'ATTIVA LA RICERCA DEL MITTENTE PER INTEROPERABILITA PER DESCRIZIONE E MAIL ANZICHE SOLO MAIL. VALORI POSSIBILI 0 o 1'
		  ,'0' -- SEGNALAZIONE luned 19/03/2012 12:50 di Palumbo Ch. 
		  ,'B','1'
		  ,'1','1');
    END IF;

	IF (cnt = 1) THEN       
  		update @db_user.DPA_CHIAVI_CONFIGURAZIONE
		set VAR_DESCRIZIONE = 'ATTIVA LA RICERCA DEL MITTENTE PER INTEROPERABILITA PER DESCRIZIONE E MAIL (valore 1) ANZICHE SOLO MAIL (valore 0)'
		where VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC' ; 
	END IF;
  
  

    END;
END;
/              
----------- FINE -
              
 
-------------------cartella  FUNCTION -------------------
              
---- getContatoreDoc.ORA.SQL  marcatore per ricerca ----
create or replace FUNCTION             @db_user.getContatoreDoc (docNumber INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
repertorio NUMBER;
-- by P. Buono
valoreSottocontatore VARCHAR(255)		 := '';
tipoContatoreSottocontatore VARCHAR(255) := '';

BEGIN

valoreContatore := '';
annoContatore := '';
codiceRegRf := '';


begin

select valore_oggetto_db, anno, repertorio
-- by P. Buono
, valore_sc, dpa_tipo_oggetto.descrizione
into valoreContatore, annoContatore, repertorio
-- by P. Buono
,valoreSottocontatore, tipoContatoreSottocontatore
from dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
	where dpa_associazione_templates.doc_number = to_char(docNumber)
	and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
	and dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
	and (dpa_tipo_oggetto.descrizione = 'Contatore' OR dpa_tipo_oggetto.descrizione = 'ContatoreSottocontatore')
	and dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';
	--and dpa_oggetti_custom.cha_tipo_tar = 'T';
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
select dpa_el_registri.var_codice into codiceRegRf
from dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri
	where dpa_associazione_templates.doc_number = to_char(docNumber)
	and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
	and dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
	and (dpa_tipo_oggetto.descrizione = 'Contatore' OR dpa_tipo_oggetto.descrizione = 'ContatoreSottocontatore')
	and dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1'
	--and
	--dpa_oggetti_custom.cha_tipo_tar = tipoContatore;
	and dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
exception when others then null;

END;
END IF;

-- by P. Buono
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
End Getcontatoredoc; 
/

              
----------- FINE -
              
---- getValCampoProfDoc.ORA.SQL  marcatore per ricerca ----
create or replace FUNCTION             @db_user.getValCampoProfDoc(DocNumber INT, CustomObjectId INT)
RETURN VARCHAR IS result VARCHAR(255);

tipoOggetto varchar(255);tipoCont varchar(1);repert number:=0; tipologiaDoc number:=0;
BEGIN

-- restituisce 1 se il documento DocNumber associato alla tipologia di documento contenente il contatore di repertorio con
-- id = CustomObjectId
SELECT (case when id_oggetto is not null then 1 else 0 end) as res
into tipologiaDoc
from dpa_associazione_templates
where doc_number=DocNumber and id_oggetto=CustomObjectId and rownum = 1;

select b.descrizione,cha_tipo_Tar, a.repertorio
into tipoOggetto,tipoCont,repert
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
      
--Casella di selezione (Per la casella di selezione serve un caso particolare perch?? i valori sono multipli)
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
    elsif(tipoOggetto = 'Contatore' AND repert = 1 And tipologiadoc = 1)
    then
    begin
    RETURN '#CONTATORE_DI_REPERTORIO#';
    end;
    elsif(tipoOggetto = 'Contatore' OR tipoOggetto = 'ContatoreSottocontatore')  then
    begin
        select getContatoreDoc(DocNumber,tipoCont)  into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber; 

    end;
else 
--Tutti gli altri
    select valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber); 
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

End Getvalcampoprofdoc; 
/

              
----------- FINE -
              
---- HAS_CHILDREN.ORA.SQL  marcatore per ricerca ----

CREATE OR REPLACE FUNCTION @db_user.HAS_CHILDREN (corrId number,tipoURP char)

RETURN number IS risultato number;

BEGIN

DECLARE

rtnUO1 number;

rtnRUO number;

rtnUO2 number;

BEGIN

select case when count(*) > 0 then 1 else 0 end into rtnUO1 from

dpa_corr_globali b where tipoURP='U' and cha_tipo_ie='I' and

b.id_parent = corrId;

--oppure

select case when count(*) > 0 then 1 else 0 end into rtnUO2 from

dpa_corr_globali b where tipoURP='U' and cha_tipo_ie='I' and 

b.id_uo = corrId ;

--oppure

select case when count(*) > 0 then 1 else 0 end into rtnRUO from

dpa_corr_globali b where (tipoURP='R')  and cha_tipo_ie='I' and 

exists (select * from peoplegroups

where groups_system_id in (select id_gruppo from dpa_corr_globali where system_id=corrId ) and dta_fine is null)

;


risultato:=rtnUO1+rtnUO2+rtnRUO;

-- come se fosse un booleano

if(risultato>0)

then

risultato:=1;

else

risultato:=0;

end if;

return risultato;

end;

END has_children;
/
              
----------- FINE -
              
 
-------------------cartella  STORED_PROCEDURE -------------------
              
---- 0.InsertDataInHistoryProf.ORA.SQL  marcatore per ricerca ----
create or replace Procedure         @db_user.InsertDataInHistoryProf(
  objType varchar,
  Idtemplate  Varchar,
  idDocOrFasc Varchar,
  Idoggcustom Varchar,
  Idpeople Varchar,
  Idruoloinuo Varchar,
  Descmodifica Varchar,
  Returnvalue out Number
) As 
Begin
   /******************************************************************************
      AUTHOR:   Samuele Furnari
      NAME:     InsertDataInHistoryProf
      PURPOSE:  Store per l'inserimento di una voce nello storico dei campi 
                profilati di documenti / fascicoli. 
  
    ******************************************************************************/
  Begin
    DECLARE enHis CHAR := '';

    -- Verifica del flag di attivazione storico per il campo
    Begin
      if OBJTYPE = 'D' then
        Select Enabledhistory Into enHis From dpa_ogg_custom_comp Where id_ogg_custom = Idoggcustom And id_template = IdTemplate;
      else
        Select Enabledhistory Into enHis From dpa_ogg_custom_comp_fasc Where id_ogg_custom = Idoggcustom And id_template = IdTemplate;
      End If;
      
    -- Se  attiva la storicizzazione del campo, viene inserita una riga nello storico
    If (enHis = '1') Then
      Begin
        -- Se l'oggetto da storicizzare  un documento, viene inserita una riga 
        -- nello storico dei documenti, altrimenti viene inserita in quella dei
        -- fascicoli
        If objType = 'D' Then
          Insert Into DPA_PROFIL_STO
            (Systemid, 
            Id_Template,
            Dta_Modifica,
            Id_Profile,
            Id_Ogg_Custom,
            Id_People,
            Id_Ruolo_In_Uo,
            Var_Desc_Modifica)
          Values
          ( Seq_Dpa_Profil_Sto.Nextval,
            Idtemplate,
            sysdate,
            idDocOrFasc,
            Idoggcustom,
            Idpeople,
            Idruoloinuo,
            Descmodifica
          );
        Else
          Insert Into Dpa_Profil_Fasc_Sto
            (Systemid, 
            Id_Template,
            Dta_Modifica,
            Id_Project,
            Id_Ogg_Custom,
            Id_People,
            Id_Ruolo_In_Uo,
            Var_Desc_Modifica)
          Values
          ( Seq_Dpa_Profil_Sto.Nextval,
            Idtemplate,
            sysdate,
            idDocOrFasc,
            Idoggcustom,
            Idpeople,
            Idruoloinuo,
            Descmodifica
          );
           
        End If ;
      End;
    End If;  
    End;
    Returnvalue := 0;
End;
End Insertdatainhistoryprof;
/

              
----------- FINE -
              
---- 0.InsertRegistroRepertorio.ORA.SQL  marcatore per ricerca ----
create or replace Procedure @db_user.InsertRegistroRepertorio(
  -- Id della tipologia
  tipologyId  Number,
  -- Id del contatore
  counterId Number,
  -- Tipo di contatore
  counterType char,
  -- Categoria della tipologia documentale cui appartirene il
  -- contatore da inserire
  tipologyKind char
) As
Begin
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     InsertRegistroRepertorio
    
    PURPOSE:  Store per l'inserimento di un registro di repertorio nell'anagrafica            

  ******************************************************************************/

  -- Cursore sui registri
  Declare Cursor cursorRegistries Is (
    Select system_id 
    From dpa_el_registri 
    Where cha_rf Is Null Or cha_rf = '0'
  );
  
  -- Cursore sugli RF
  Cursor cursorRf Is (
    Select system_id
    From @db_user.dpa_el_registri
    Where cha_rf = '1'
  );  
  
  -- Id del registro / Rf
  registryRfId  Number;
  
  Begin
    Case (counterType)
      When 'T' Then
        -- Se il contatore  di tipologia, viene inserita una sola riga nell'anagrafica
        Insert Into @db_user.dpa_registri_repertorio (
          TipologyId,
          CounterId,
          CounterState,
          SettingsType,
          RegistryId,
          RfId,
          RoleRespId,
          PrinterRoleRespId,
          PrinterUserRespId,
          PrintFreq,
          TipologyKind,
          DtaStart,
          DtaFinish,
          DtaNextAutomaticPrint,
          DtaLastPrint,
          LastPrintedNumber,
          Resprights
        )
        Values(
          tipologyId,
          counterId,
          'O',
          'G',
          null,
          null,
          null,
          null,
          null,
          'N',
          tipologyKind,
          null,
          null,
          null,
          null,
          0,
          'R');
      When 'A' Then
        -- Se  di AOO vengono inserite tante voci quanti sono i registri
        Begin
          Begin Open cursorRegistries;
          Loop Fetch cursorRegistries INTO registryRfId;
          EXIT WHEN cursorRegistries%NOTFOUND;
            Insert Into @db_user.dpa_registri_repertorio (
              TipologyId,
              CounterId,
              CounterState,
              SettingsType,
              RegistryId,
              RfId,
              RoleRespId,
              PrinterRoleRespId,
              PrinterUserRespId,
              PrintFreq,
              TipologyKind,
              DtaStart,
              DtaFinish,
              DtaNextAutomaticPrint,
              DtaLastPrint,
              LastPrintedNumber,
              resprights
            )
            Values(
              tipologyId,
              counterId,
              'O',
              'G',
              registryRfId,
              null,
              null,
              null,
              null,
              'N',
              tipologyKind,
              null,
              null,
              null,
              null,
              0,
              'R'
            );
            End loop;  
            Close cursorRegistries;
          End;  
        End;  
      When 'R' Then
        -- Se  di RF vengono inserite tante voci quanti sono gli RF
        Begin
          BEGIN OPEN cursorRf;
            LOOP FETCH cursorRf INTO registryRfId;
            EXIT WHEN cursorRf%NOTFOUND;
            Insert Into @db_user.dpa_registri_repertorio (
              TipologyId,
              CounterId,
              CounterState,
              SettingsType,
              RegistryId,
              RfId,
              RoleRespId,
              PrinterRoleRespId,
              PrinterUserRespId,
              PrintFreq,
              TipologyKind,
              DtaStart,
              DtaFinish,
              DtaNextAutomaticPrint,
              DtaLastPrint,
              LastPrintedNumber,
              resprights
            )
          Values (
              tipologyId,
              counterId,
              'O',
              'G',
              null,
              registryRfId,
              null,
              null,
              null,
              'N',
              tipologyKind,
              null,
              null,
              null,
              null,
              0,
              'R'
            );
          End loop;
          Close cursorRf;
        End;  
        End;  
    End Case;
  End;
  End Insertregistrorepertorio;  
/

              
----------- FINE -
              
---- 0.SaveCounterSettings.ORA.sql  marcatore per ricerca ----
create or replace PROCEDURE @db_user.SaveCounterSettings(
  -- Id del contatore
  countId   Number,
  -- Tipo di impostazioni specificato per un contatore (G o S)
  settingsType Varchar,
  -- Id del ruolo stampatore
  roleIdGroup  Number,
  -- Id dell'utente stampatore
  userIdPeople  Number,
  -- Id del ruolo responsabile
  roleRespIdGroup Number,
  -- Frequenza di stampa
  printFrequency Varchar,
  -- Data di partenza del servizio di stampa automatica
  dateAutomaticPrintStart Date,
  -- Data di stop del servizio di stampa automatica
  dateAutomaticPrintFinish  Date,
  -- Data prevista per la prossima stampa automatica
  dateNextAutomaticPrint  Date,
  -- Id del registro cui si riferiscono le impostazioni da salvare
  reg Number,
  -- Id dell'RF cui si riferscono le impostazioni da salvare
  rf Number,
  -- Sigla identificativa della tipologia in cui  definito il contatore (D, F)
  tipology Varchar,
  -- Stato del contatore di repertorio (O, C)
  state Varchar,
  -- Diritti da concedere al responsabile (R o RW)
  rights Varchar,
  -- Valore di ritorno
  returnValue Out Number
) AS BEGIN
  /******************************************************************************
  
    AUTHOR:   Samuele Furnari

    NAME:     SaveCounterSettings

    PURPOSE:  Store per il salvataggio delle modifiche apportate alle impostazioni
              di stampa per un determinato contatore di repertorio

  ******************************************************************************/
 
  -- Tipologia di impostazioni impostata per il contatore
  Declare actualSettingsType Char;
  
  Begin
    -- Se il tipo di impostazione scelta  G, vengono aggiornate le properiet per tutte le istanze
    -- del contatore counterId
    IF settingsType = 'G' THEN
      Begin
        Update dpa_registri_repertorio
          Set SettingsType = 'G',
          PrinterRoleRespId = roleidgroup,
          PrinterUserRespId = useridPeople,
          RoleRespId = rolerespidgroup,
          PrintFreq = printFrequency,
          DtaStart = dateAutomaticPrintStart,
          DtaFinish = dateAutomaticPrintFinish,
          DtaNextAutomaticPrint = dateNextAutomaticPrint,
          CounterState = state,
          Resprights = rights
        Where CounterId = countId And TipologyKind = tipology;  
      End;
    Else
      -- Altrimenti, se prima il tipo di impostazioni era G, vengono aggiornate tutte
      -- le istanze del contatore ad S ed in seguito vengono salvare le informazioni 
      -- per la specifica istanza specificata
      -- da registro / RF specificato
      Begin
        -- Valorizzazione corretta per l'id gruppo del ruolo responsabile
        Declare decodedRoleRespIdGroup Varchar (100);
        -- Valorizzazione corretta per l'id gruppo dello stampatore
        decodedRoleIdGroup Varchar (100);
        -- Valorizzazione corretta per l'id utente dello stampatore
        decodedUserIdPeople Varchar (100);
        
        Begin
        
          Select SettingsType Into actualSettingsType From dpa_registri_repertorio Where counterId = countId And RowNum = 1;
          
          If actualSettingsType != SettingsType And SettingsType = 'S' Then
            Update dpa_registri_repertorio
              Set SettingsType = 'S'/*,
              RoleRespId = null,
              PrinterRoleRespId = null,
              PrinterUserRespId = null,
              PrintFreq = 'N',
              DtaStart = null,
              DtaFinish = null,
              DtaNextAutomaticPrint = null,
              CounterState = 'O'*/
            Where CounterId = countId And TipologyKind = tipology; 
          End If;  
          
          If roleRespIdGroup is null Then
            decodedRoleRespIdGroup := 'null';
          Else
            decodedRoleRespIdGroup := roleRespIdGroup;
          End If;
          
          If roleIdGroup is null Then
            decodedRoleIdGroup := 'null';
          Else
            decodedRoleIdGroup := roleIdGroup;
          End If;
          
          If userIdPeople is null Then
            decodedUserIdPeople := 'null';
          Else
            decodedUserIdPeople := userIdPeople;
          End If;  
          
          Declare updateQuery varchar (2000) := 
              'Update dpa_registri_repertorio
              Set RoleRespId = ' || decodedRoleRespIdGroup || ',
              PrinterRoleRespId = ' || decodedRoleIdGroup || ',
              PrinterUserRespId = ' || decodedUserIdPeople || ',
              PrintFreq = ''' || printFrequency || ''',
              DtaStart = to_date(''' || to_char(dateAutomaticPrintStart, 'dd/MM/yyyy') || ''', ''dd/MM/yyyy''),
              DtaFinish = to_date(''' || to_char(dateAutomaticPrintFinish, 'dd/MM/yyyy') || ''', ''dd/MM/yyyy''),
              DtaNextAutomaticPrint = to_date(''' || to_char(dateNextAutomaticPrint, 'dd/MM/yyyy') || ''', ''dd/MM/yyyy''),
              CounterState = ''' || state || ''',
              Resprights = ''' || rights || '''
            Where CounterId = ' || countId || ' And TipologyKind = ''' || tipology || '''
              And ';
            
          Begin
            IF reg is not null And to_number(reg) > 0  THEN
              updateQuery := updateQuery || ' RegistryId = ' || reg || ' And ';
            Else
              updateQuery := updateQuery || ' RegistryId is null And';
            END IF ;
          
            IF rf is not null And to_number(rf) > 0  THEN
              updateQuery := updateQuery || ' RfId = ' || rf;
            Else
              updateQuery := updateQuery || ' RfId is null';
            END IF ;
            
            execute immediate updateQuery;
          End;  
        End;
      End;
    END IF ;
    
    -- Impostazione del valore di ritorno
    returnValue := 1;
  End;  
END SaveCounterSettings;
/

              
----------- FINE -
              
---- 0.SetRightsForRole.ORA.SQL  marcatore per ricerca ----
create or replace PROCEDURE @db_user.SetRightsForRole(
  objId Number,
  idRole Number,
  rightsToAssign Number,
  returnValue Out Number
) AS 
BEGIN
  /******************************************************************************
  
    AUTHOR:   Samuele Furnari

    NAME:     SetRightsForRole

    PURPOSE:  Store per l'assegnazione dei diritti di tipo A ad un ruolo (solo
              se il ruolo non li possiede gi o se non possiede diritti superiori)

  ******************************************************************************/


  -- Diritti posseduti dal ruolo
  Declare rights VarChar (2000);
  
  Begin
    -- Selezione degli eventuali diritti posseduti da un ruolo
    Select Max(AccessRights) Into rights From  security Where Thing = objId And PersonOrGroup = idRole;
    
    -- Se i diritti sono ci sono, si procede con un inserimento
    IF rights Is Null THEN
      Insert Into security
      (
        THING,
        PERSONORGROUP,
        ACCESSRIGHTS,
        ID_GRUPPO_TRASM,
        CHA_TIPO_DIRITTO,
        HIDE_DOC_VERSIONS,
        TS_INSERIMENTO,
        VAR_NOTE_SEC
      )
      VALUES
      (
        objId,
        idRole,
        rightsToAssign,
        null,
        'A',
        null,
        sysdate,
        null
      );
    Else
      Begin
        -- Se i diritti posseduti dal ruolo sono minori di quelli che si vogliono concedere,
        -- si procede con un aggiornamento del diritto
        IF to_number(rights) != 0 And rightsToAssign > to_number(rights) THEN
          Update security
          Set ACCESSRIGHTS = rightsToAssign,
          CHA_TIPO_DIRITTO = 'A'
          Where Thing = objId;
          
        END IF ;
      End;
      
    END IF ;
  
    returnValue := 1;
  End;
End Setrightsforrole;
/

              
----------- FINE -
              
---- 0.vis_doc_anomala_custom.ORA.SQL  marcatore per ricerca ----
CREATE OR REPLACE PROCEDURE @db_user.vis_doc_anomala_custom (p_id_amm NUMBER, p_querydoc VARCHAR)

IS

--DICHIARAZIONI

   s_idg_security       NUMBER;

   s_ar_security        NUMBER;

   s_td_security        VARCHAR (2);

   s_vn_security        VARCHAR (255);

   s_idg_r_sup          NUMBER;

   s_doc_number         NUMBER;

   s_id_fascicolo       NUMBER;

   s_cha_cod_t_a_fasc   VARCHAR (1024);

   n_id_gruppo          NUMBER;

   codice_atipicita     VARCHAR (255);

BEGIN

--CURSORE DOCUMENTI

   DECLARE

      TYPE empcurtyp IS REF CURSOR;

 

      documenti   empcurtyp;

   BEGIN

      OPEN documenti FOR p_querydoc;

 

      LOOP

         FETCH documenti

          INTO s_doc_number;

 

         EXIT WHEN documenti%NOTFOUND;

 

         --Cursore sulla security per lo specifico documento

         DECLARE

            CURSOR c_idg_security

            IS

               SELECT personorgroup, accessrights, cha_tipo_diritto,

                      var_note_sec

                 FROM security

                WHERE thing = s_doc_number AND accessrights > 20;

         BEGIN

            OPEN c_idg_security;

 

            LOOP

               FETCH c_idg_security

                INTO s_idg_security, s_ar_security, s_td_security,

                     s_vn_security;

 

               EXIT WHEN c_idg_security%NOTFOUND;

 

               --Gerachia ruolo proprietario documento

               IF (UPPER (s_td_security) = 'P')

               THEN

                  DECLARE

                     CURSOR ruoli_sup

                     IS

                        SELECT dpa_corr_globali.id_gruppo

                          FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo

                               ON dpa_corr_globali.id_tipo_ruolo =

                                                     dpa_tipo_ruolo.system_id

                         WHERE dpa_corr_globali.id_uo IN (

                                  SELECT     dpa_corr_globali.system_id

                                        FROM dpa_corr_globali

                                       WHERE dpa_corr_globali.dta_fine IS NULL

                                  CONNECT BY PRIOR dpa_corr_globali.id_parent =

                                                   dpa_corr_globali.system_id

                                  START WITH dpa_corr_globali.system_id =

                                                (SELECT dpa_corr_globali.id_uo

                                                   FROM dpa_corr_globali

                                                  WHERE dpa_corr_globali.id_gruppo =

                                                               s_idg_security))

                           AND dpa_corr_globali.cha_tipo_urp = 'R'

                           AND dpa_corr_globali.id_amm = p_id_amm

                           AND dpa_corr_globali.dta_fine IS NULL

                           AND dpa_tipo_ruolo.num_livello <

                                  (SELECT dpa_tipo_ruolo.num_livello

                                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo

                                          ON dpa_corr_globali.id_tipo_ruolo =

                                                      dpa_tipo_ruolo.system_id

                                    WHERE dpa_corr_globali.id_gruppo =

                                                                s_idg_security);

                  BEGIN

                     OPEN ruoli_sup;

 

                     LOOP

                        FETCH ruoli_sup

                         INTO s_idg_r_sup;

 

                        EXIT WHEN ruoli_sup%NOTFOUND;

 

                        --DBMS_OUTPUT.PUT_LINE('DOCUMENTO : ' || s_doc_number || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);

                        INSERT INTO dpa_vis_anomala

                                    (id_gruppo

                                    )

                             VALUES (s_idg_r_sup

                                    );

                     END LOOP;

 

                     CLOSE ruoli_sup;

                  END;

 

                  --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security

                  --Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie

                  BEGIN

                     n_id_gruppo := 0;

 

                     SELECT COUNT (*)

                       INTO n_id_gruppo

                       FROM (SELECT id_gruppo

                               FROM dpa_vis_anomala

                             MINUS

                             SELECT personorgroup

                               FROM security

                              WHERE thing = s_doc_number);

 

                     IF (    n_id_gruppo <> 0

                         AND NVL (INSTR (codice_atipicita, 'AGRP'), 0) = 0

                        )

                     THEN

                        codice_atipicita := codice_atipicita || 'AGRP-';

                     END IF;

                  END;

 

                  COMMIT;

               END IF;

 

               --Gerarchia destinatario trasmissione

               IF (UPPER (s_td_security) = 'T')

               THEN

                  DECLARE

                     CURSOR ruoli_sup

                     IS

                        SELECT dpa_corr_globali.id_gruppo

                          FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo

                               ON dpa_corr_globali.id_tipo_ruolo =

                                                     dpa_tipo_ruolo.system_id

                         WHERE dpa_corr_globali.id_uo IN (

                                  SELECT     dpa_corr_globali.system_id

                                        FROM dpa_corr_globali

                                       WHERE dpa_corr_globali.dta_fine IS NULL

                                  CONNECT BY PRIOR dpa_corr_globali.id_parent =

                                                   dpa_corr_globali.system_id

                                  START WITH dpa_corr_globali.system_id =

                                                (SELECT dpa_corr_globali.id_uo

                                                   FROM dpa_corr_globali

                                                  WHERE dpa_corr_globali.id_gruppo =

                                                               s_idg_security))

                           AND dpa_corr_globali.cha_tipo_urp = 'R'

                           AND dpa_corr_globali.id_amm = p_id_amm

                           AND dpa_corr_globali.dta_fine IS NULL

                           AND dpa_tipo_ruolo.num_livello <

                                  (SELECT dpa_tipo_ruolo.num_livello

                                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo

                                          ON dpa_corr_globali.id_tipo_ruolo =

                                                      dpa_tipo_ruolo.system_id

                                    WHERE dpa_corr_globali.id_gruppo =

                                                                s_idg_security);

                  BEGIN

                     OPEN ruoli_sup;

 

                     LOOP

                        FETCH ruoli_sup

                         INTO s_idg_r_sup;

 

                        EXIT WHEN ruoli_sup%NOTFOUND;

 

                        --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || s_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);

                        INSERT INTO dpa_vis_anomala

                                    (id_gruppo

                                    )

                             VALUES (s_idg_r_sup

                                    );

                     END LOOP;

 

                     CLOSE ruoli_sup;

                  END;

 

                  --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security

                  --Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie

                  BEGIN

                     n_id_gruppo := 0;

 

                     SELECT COUNT (*)

                       INTO n_id_gruppo

                       FROM (SELECT id_gruppo

                               FROM dpa_vis_anomala

                             MINUS

                             SELECT personorgroup

                               FROM security

                              WHERE thing = s_doc_number);

 

                     IF (    n_id_gruppo <> 0

                         AND NVL (INSTR (codice_atipicita, 'AGDT'), 0) = 0

                        )

                     THEN

                        codice_atipicita := codice_atipicita || 'AGDT-';

                     END IF;

                  END;

 

                  COMMIT;

               END IF;

 

               --Fascicolazione documento

               IF (UPPER (s_td_security) = 'F')

               THEN

                  DECLARE

                     CURSOR fascicoli

                     IS

                        SELECT system_id

                          FROM project

                         WHERE system_id IN (

                                  SELECT id_fascicolo

                                    FROM project

                                   WHERE system_id IN (

                                                    SELECT project_id

                                                      FROM project_components

                                                     WHERE LINK =

                                                                 s_doc_number))

                           AND cha_tipo_fascicolo = 'P';

                  BEGIN

                     OPEN fascicoli;

 

                     LOOP

                        FETCH fascicoli

                         INTO s_id_fascicolo;

 

                        EXIT WHEN fascicoli%NOTFOUND;

 

                        SELECT cha_cod_t_a

                          INTO s_cha_cod_t_a_fasc

                          FROM project

                         WHERE system_id = s_id_fascicolo;

 

                        IF (    s_cha_cod_t_a_fasc IS NOT NULL

                            AND UPPER (s_cha_cod_t_a_fasc) <> 'T'

                           )

                        THEN

                           IF (NVL (INSTR (codice_atipicita, 'AFCD'), 0) = 0

                              )

                           THEN

                              codice_atipicita := codice_atipicita || 'AFCD-';

                           END IF;

                        END IF;

                     END LOOP;

 

                     CLOSE fascicoli;

                  END;

               END IF;

 

               --Gerarchia ruolo destinatario di copia visibilita

               IF (    UPPER (s_td_security) = 'A'

                   AND UPPER (s_vn_security) =

                                              'ACQUISITO PER COPIA VISIBILITA'

                  )

               THEN

                  DECLARE

                     CURSOR ruoli_sup

                     IS

                        SELECT dpa_corr_globali.id_gruppo

                          FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo

                               ON dpa_corr_globali.id_tipo_ruolo =

                                                     dpa_tipo_ruolo.system_id

                         WHERE dpa_corr_globali.id_uo IN (

                                  SELECT     dpa_corr_globali.system_id

                                        FROM dpa_corr_globali

                                       WHERE dpa_corr_globali.dta_fine IS NULL

                                  CONNECT BY PRIOR dpa_corr_globali.id_parent =

                                                   dpa_corr_globali.system_id

                                  START WITH dpa_corr_globali.system_id =

                                                (SELECT dpa_corr_globali.id_uo

                                                   FROM dpa_corr_globali

                                                  WHERE dpa_corr_globali.id_gruppo =

                                                               s_idg_security))

                           AND dpa_corr_globali.cha_tipo_urp = 'R'

                           AND dpa_corr_globali.id_amm = p_id_amm

                           AND dpa_corr_globali.dta_fine IS NULL

                           AND dpa_tipo_ruolo.num_livello <

                                  (SELECT dpa_tipo_ruolo.num_livello

                                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo

                                          ON dpa_corr_globali.id_tipo_ruolo =

                                                      dpa_tipo_ruolo.system_id

                                    WHERE dpa_corr_globali.id_gruppo =

                                                                s_idg_security);

                  BEGIN

                     OPEN ruoli_sup;

 

                     LOOP

                        FETCH ruoli_sup

                         INTO s_idg_r_sup;

 

                        EXIT WHEN ruoli_sup%NOTFOUND;

 

                        --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || s_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);

                        INSERT INTO dpa_vis_anomala

                                    (id_gruppo

                                    )

                             VALUES (s_idg_r_sup

                                    );

                     END LOOP;

 

                     CLOSE ruoli_sup;

                  END;

 

                  --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security

                  --Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie

                  BEGIN

                     n_id_gruppo := 0;

 

                     SELECT COUNT (*)

                       INTO n_id_gruppo

                       FROM (SELECT id_gruppo

                               FROM dpa_vis_anomala

                             MINUS

                             SELECT personorgroup

                               FROM security

                              WHERE thing = s_doc_number);

 

                     IF (    n_id_gruppo <> 0

                         AND NVL (INSTR (codice_atipicita, 'AGCV'), 0) = 0

                        )

                     THEN

                        codice_atipicita := codice_atipicita || 'AGCV-';

                     END IF;

                  END;

 

                  COMMIT;

               END IF;

            END LOOP;

 

            CLOSE c_idg_security;

         END;

 

         --Restituzione codice di atipicita

         IF (codice_atipicita IS NULL)

         THEN

            codice_atipicita := 'T';

 

            --DBMS_OUTPUT.PUT_LINE('Codici Atipicita Documento ' || s_doc_number || ' - ' || codice_atipicita);

            UPDATE PROFILE

               SET cha_cod_t_a = codice_atipicita

             WHERE docnumber = s_doc_number;

 

            COMMIT;

            codice_atipicita := NULL;

         END IF;

 

         IF (SUBSTR (codice_atipicita, LENGTH (codice_atipicita)) = '-')

         THEN

            codice_atipicita :=

                   SUBSTR (codice_atipicita, 0, LENGTH (codice_atipicita) - 1);

 

            --DBMS_OUTPUT.PUT_LINE('Codici Atipicita Documento ' || s_doc_number || ' - ' || codice_atipicita);

            UPDATE PROFILE

               SET cha_cod_t_a = codice_atipicita

             WHERE docnumber = s_doc_number;

 

            COMMIT;

            codice_atipicita := NULL;

         END IF;

      END LOOP;

 

      CLOSE documenti;

   END;

EXCEPTION

   WHEN OTHERS

   THEN

      DBMS_OUTPUT.put_line ('Errore nell''esecuzione della procedura');

END vis_doc_anomala_custom;

/

              
----------- FINE -
              
---- 1.InitRegRepertorio.ORA.SQL  marcatore per ricerca ----
create or replace PROCEDURE @db_user.InitRegRepertorio AS 
BEGIN
  /******************************************************************************
    AUTHOR:   Samuele Furnari
    NAME:     InitRegRepertorio
    PURPOSE:  Store per l'inizializzazione della tabella dell'anagrafica dei 
              registri di repertorio.
  ******************************************************************************/
  
  -- Preventiva pulizia dell'anagrafica dei registri di repertorio
  Delete From dpa_registri_repertorio;
  
  -- Id della tipologia, id del contatore, tipo di contatore definito e id del registro
  Declare tipologyId int;
          counterId int;
          counterType char;
          registryRfId int;
  
  Begin
      -- Cursore per scrorrere le informazioni sui contatori definiti per le tipologie
      -- documento
      Declare Cursor cursorCounters Is (
        Select 
        ta.system_id TipologyId,
        oc.system_id as CounterId,
        oc.cha_tipo_tar as CounterType
        From dpa_tipo_atto ta
        Inner Join dpa_ogg_custom_comp occ
        On ta.system_id = occ.id_template
        Inner Join dpa_oggetti_custom oc
        On occ.id_ogg_custom = oc.system_id
        Inner Join dpa_tipo_oggetto tobj
        On oc.id_tipo_oggetto = tobj.system_id
        Where lower(tobj.descrizione) = 'contatore'
        And oc.repertorio = 1
      );
      
      BEGIN OPEN cursorCounters;
      LOOP FETCH cursorCounters INTO tipologyId, counterId, counterType;
      EXIT WHEN cursorCounters%NOTFOUND;
        -- Inserimento delle informazioni sul registro nell'anagrafica
        InsertRegistroRepertorio(tipologyId, counterId, counterType, 'D');
    
      END LOOP;
      CLOSE cursorCounters;
    End;
    /*
    Begin
      -- Cursore per scorrere le informazioni sui contatori definiti per le tipologie
      -- fascicoli
      Declare Cursor cursorCountersFasc Is (
        Select 
        tf.system_id TipologyId,
        oc.system_id as CounterId,
        oc.cha_tipo_tar as CounterType
        From dpa_tipo_fasc tf
        Inner Join dpa_ogg_custom_comp_fasc occ
        On tf.system_id = occ.id_template
        Inner Join dpa_oggetti_custom_fasc oc
        On occ.id_ogg_custom = oc.system_id
        Inner Join dpa_tipo_oggetto_fasc tobj
        On oc.id_tipo_oggetto = tobj.system_id
        Where lower(tobj.descrizione) = 'contatore'
        And oc.repertorio = 1
      );
    
      BEGIN OPEN cursorCountersFasc;
      LOOP FETCH cursorCountersFasc INTO tipologyId, counterId, counterType;
      EXIT WHEN cursorCountersFasc%NOTFOUND;
        -- Inserimento delle informazioni sul registro nell'anagrafica
        InsertRegistroRepertorio(tipologyId, counterId, counterType, 'F');

      END LOOP;
      CLOSE cursorCountersFasc;
      End;  */
    End;  
end INITREGREPERTORIO;
/


-- questa istruzione si esegue solo una volta - FUORI CD - con questa istruzione 
-- begin	@db_user.INITREGREPERTORIO; end;


              
----------- FINE -
              
---- DeleteRegistroRepertorio.ORA.SQL  marcatore per ricerca ----
create or replace
Procedure @db_user.DeleteRegistroRepertorio(
  -- Id del contatore
  countId Number
) As
Begin
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     DeleteRegistroRepertorio
    
    PURPOSE:  Store per l'eliminazione di un registro di repertorio nell'anagrafica            

  ******************************************************************************/
  Delete From @db_user.dpa_registri_repertorio Where CounterId = countId;
    
End Deleteregistrorepertorio;
/
              
----------- FINE -
              
---- InsertRepertorioFromCode.ORA.SQL  marcatore per ricerca ----
create or replace
PROCEDURE @db_user.InsertRepertorioFromCode(
  tipologyId In Integer,
  counterId In Integer,
  counterType In Varchar,
  returnValue Out Integer) AS 
BEGIN
  INSERTREGISTROREPERTORIO (tipologyId, counterId, counterType, 'D');
  
  returnValue := 0;

End Insertrepertoriofromcode;
/
              
----------- FINE -
              
---- Sp_Storicizza_Log.ORA.sql  marcatore per ricerca ----
create or replace Procedure          @db_user.Sp_Storicizza_Log Is
Begin
Utilita.Utl_Storicizza_Log(7);  -- where Interval_Of_Days = 7
END;
/


              
----------- FINE -
              
 
-------------------cartella  VERSIONE -------------------
              
---- insert_DPA_DOCSPA.ORA.sql  marcatore per ricerca ----
begin
Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
Values    (seq.nextval, sysdate, '3.19.1');
end;
/              
