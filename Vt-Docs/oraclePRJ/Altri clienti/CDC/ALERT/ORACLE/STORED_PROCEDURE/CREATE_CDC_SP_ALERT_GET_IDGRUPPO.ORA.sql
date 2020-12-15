CREATE OR REPLACE PROCEDURE CDC_SP_ALERT_GET_IDGRUPPO (InUO in varchar2 , UO_CURSOR OUT SYS_REFCURSOR)
IS


sql_str varchar2(2000);

BEGIN 
  sql_str := 'select distinct id_gruppo from dpa_corr_globali where  id_uo ='
          ||' (select system_id from dpa_corr_globali where var_cod_rubrica = '''|| InUO ||''' and id_registro is null )';     
            
           -- dbms_output.put_line(sql_str);
             open UO_CURSOR  for sql_str;
               
EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CDC_SP_ALERT_GET_IDGRUPPO;
/