--------------------------------------------------------
--  DDL for Trigger REMOVEREGFROMREPERTORITABLE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "ITCOLL_6GIU12"."REMOVEREGFROMREPERTORITABLE" 
after delete on dpa_el_registri 
referencing old as old new as new 
for each row 
begin
  /******************************************************************************
      AUTHOR:    Samuele Furnari
      NAME:      RemoveRegFromRepertoriTable
      PURPOSE:   Questo trigger in ascolto sulla dpa_el_registri scatta ogni volta
                 che viene eliminato un record dalla tabella dei registri / rf.
                 
    ******************************************************************************/
    Delete From Dpa_Registri_Repertorio Where RegistryId = :old.system_id Or RfId = :old.system_id;
End;
/
ALTER TRIGGER "ITCOLL_6GIU12"."REMOVEREGFROMREPERTORITABLE" ENABLE;
