CREATE OR REPLACE PROCEDURE CDC_SP_ALERT_GET_DOCSMPS (system_id IN number,
                                                            UO_CURSOR OUT SYS_REFCURSOR)
IS

sql_str varchar2(2000);

BEGIN

    sql_str := ' select DPA_CORR_GLOBALI.ID_PEOPLE PEOPLE_SYSTEM_ID, '
            ||' DPA_CORR_GLOBALI.ID_GRUPPO GROUPS_SYSTEM_ID, '
            ||' DPA_CORR_GLOBALI.var_cognome VAR_COGNOME, '
            ||' DPA_CORR_GLOBALI.var_nome VAR_NOME, '
            ||' DPA_CORR_GLOBALI.VAR_EMAIL EMAIL_ADDRESS'
            ||' from  '
            ||' DPA_CORR_GLOBALI '
            ||' where '
            ||' DPA_CORR_GLOBALI.SYSTEM_ID = ' || system_id;
            
       
   open UO_CURSOR  for sql_str;

   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CDC_SP_ALERT_GET_DOCSMPS;
/