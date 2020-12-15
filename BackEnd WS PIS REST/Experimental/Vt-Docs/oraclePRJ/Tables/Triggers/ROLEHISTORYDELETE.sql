--------------------------------------------------------
--  DDL for Trigger ROLEHISTORYDELETE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "ITCOLL_6GIU12"."ROLEHISTORYDELETE" 
   BEFORE DELETE
   ON dpa_corr_globali
   REFERENCING NEW AS NEW OLD AS OLD
   FOR EACH ROW
    WHEN (OLD.cha_tipo_urp = 'R') BEGIN
  /******************************************************************************
  
    AUTHOR:    Samuele Furnari

    NAME:      RoleHistoryDelete

    PURPOSE:   Ogni volta che viene eliminato un record dalla dpa_corr_globali
               vengono cancellati dallo storico tutti i record relativi alla 
               storia del ruolo eliminati
 
  ******************************************************************************/

   DELETE FROM dpa_role_hisTory 
         WHERE original_corr_id = :OLD.original_id;
EXCEPTION
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      RAISE;
END;
/
ALTER TRIGGER "ITCOLL_6GIU12"."ROLEHISTORYDELETE" ENABLE;
