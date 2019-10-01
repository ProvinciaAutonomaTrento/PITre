CREATE OR REPLACE PROCEDURE CDC_SP_ALERT_GET_DOC_B (Giorni IN number,
                                                            UOString IN varchar2 , 
                                                            UO_CURSOR OUT SYS_REFCURSOR)
IS

sql_str varchar2(20000);

BEGIN

sql_str := 'select *  from V_CDC_CONTROLLO_PREVENTIVO'
            ||' where '
            ||' DATA_REGISTRAZIONE is null AND'
            ||' DATA_RESTITUZ_AMMINISTRAZIONE is null AND'
            ||' DATA_INVIO_DEFERIMENTO is null '
            ||' and ( sysdate - DATA_SCADENZA_CONTROLLO ) <='||Giorni
            ||' and personorgroup in '||UOString
            ||'order by system_id asc';
           
   --DBMS_OUTPUT.PUT_LINE (sql_str);
   
   open UO_CURSOR  for sql_str;


   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CDC_SP_ALERT_GET_DOC_B;
/