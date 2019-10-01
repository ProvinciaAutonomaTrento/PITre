CREATE OR REPLACE PROCEDURE CDC_SP_ALERT_GET_UO (UO_CURSOR OUT SYS_REFCURSOR)
IS


BEGIN
   

    OPEN UO_CURSOR FOR  
        select *  from CDC_ALERT_UO;
          
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CDC_SP_ALERT_GET_UO;
/