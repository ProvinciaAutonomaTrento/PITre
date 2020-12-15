CREATE OR REPLACE PROCEDURE CDC_SP_ALERT_UPDATE_STATE (START_DATE IN Varchar2,
                                                       NEW_STATE IN number)

IS


BEGIN

   UPDATE CDC_ALERT_STATE SET STATE=NEW_STATE, DATEEND = to_char(sysdate,'dd/MM/yyyy hh24:mm:ss') 
       WHERE SUBSTR(DATESTART,0,10) = START_DATE;      

   COMMIT;

   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CDC_SP_ALERT_UPDATE_STATE;
/