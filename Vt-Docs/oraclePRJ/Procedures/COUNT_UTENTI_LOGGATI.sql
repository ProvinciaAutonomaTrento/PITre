--------------------------------------------------------
--  DDL for Procedure COUNT_UTENTI_LOGGATI
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."COUNT_UTENTI_LOGGATI" (val int) IS
tmpVar NUMBER;


BEGIN

begin
   tmpVar := 0;
   
   select count(*) into tmpvar from dpa_login;
  
       
   insert into utenti_loggati (TOT_UTENTI, DATATIME)   values (tmpvar,sysdate);  
   commit;
    
    EXCEPTION    
         WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       raise;
 end;      
       
END count_utenti_loggati; 

/
