--------------------------------------------------------
--  File creato - martedì-maggio-14-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Package Body UTILITA
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY UTILITA as 

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
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE)
  SELECT SYSTEM_ID,    USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                   ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE
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
