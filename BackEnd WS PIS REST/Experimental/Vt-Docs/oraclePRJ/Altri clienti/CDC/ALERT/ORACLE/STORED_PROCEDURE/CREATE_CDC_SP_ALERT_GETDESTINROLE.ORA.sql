CREATE OR REPLACE PROCEDURE CDC_SP_ALERT_GETDESTINROLE  (UOString IN varchar2 , 
                                                         FUNC_NAME IN varchar2,                   
                                                         UO_CURSOR OUT SYS_REFCURSOR)
IS

sql_str varchar2(20000);

BEGIN

sql_str :=   '   select '
             ||' p.SYSTEM_ID PEOPLE_SYSTEM_ID,'
             ||' cg.ID_GRUPPO GROUPS_SYSTEM_ID,'
             ||' p.var_cognome VAR_COGNOME,'
             ||' p.var_nome VAR_NOME,'
             ||' p.email_address EMAIL_ADDRESS'
             ||' from '
             ||' dpa_funzioni f, '
             ||' dpa_tipo_funzione tf, '
             ||' dpa_tipo_f_ruolo tfr, '
             ||' dpa_corr_globali cg,'
             ||' peoplegroups pg, '
             ||' people p'
             ||' where '
             ||' tf.SYSTEM_ID=f.ID_TIPO_FUNZIONE'
             ||' and tfr.ID_RUOLO_IN_UO=cg.system_id'
             ||' and tfr.ID_TIPO_FUNZ=tf.SYSTEM_ID'
             ||' and cg.id_uo = '
             ||' (select system_id from dpa_corr_globali where var_cod_rubrica = '''|| UOString ||''')'
             ||' and cg.id_gruppo=pg.GROUPS_SYSTEM_ID'
             ||' and pg.DTA_FINE is null'
             ||' and pg.people_system_id=p.system_id'
             ||' and f.cod_funzione='''|| FUNC_NAME  ||'''';

  open UO_CURSOR  for sql_str;
  
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CDC_SP_ALERT_GETDESTINROLE;
/