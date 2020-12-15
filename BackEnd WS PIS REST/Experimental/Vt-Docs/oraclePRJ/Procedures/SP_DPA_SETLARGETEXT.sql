--------------------------------------------------------
--  DDL for Procedure SP_DPA_SETLARGETEXT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_DPA_SETLARGETEXT" (
   sysid                 NUMBER,
   var_filtriric         VARCHAR2,
   returnvalue     OUT   NUMBER
)
IS
   tmpvar   NUMBER;
   tmpClob  CLOB;
BEGIN
   tmpvar := 0;
   
   select var_filtri_ric into tmpClob
   from dpa_salva_ricerche where system_id = sysid for update;
    
    DBMS_LOB.WRITE(tmpClob, length(var_filtriric),1,var_filtriric);
    
   --UPDATE dpa_salva_ricerche f
     -- SET f.var_filtri_ric = var_filtriric
 --   WHERE system_id = sysid;

   returnvalue := 0;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
     RAISE;
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
    
     RAISE;
      
COMMIT;
      
END sp_dpa_setlargetext; 

/
