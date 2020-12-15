CREATE OR REPLACE PROCEDURE CDC_SP_ALERT_INSERT_STATE ( NEW_STATE IN number)

IS


BEGIN

   INSERT INTO CDC_ALERT_STATE VALUES (to_char(sysdate,'dd/MM/yyyy hh24:mm:ss'), NEW_STATE, null);
     
   COMMIT;
   
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CDC_SP_ALERT_INSERT_STATE;
/
