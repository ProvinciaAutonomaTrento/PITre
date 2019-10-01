--------------------------------------------------------
--  DDL for Trigger MODCOUNTDOCINMNGRTBL
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "ITCOLL_6GIU12"."MODCOUNTDOCINMNGRTBL" 
BEFORE Update ON DPA_OGGETTI_CUSTOM 
FOR EACH ROW 
 WHEN (new.repertorio != old.repertorio Or new.cha_tipo_tar != old.cha_tipo_tar) Declare 
  idTipologia number;
BEGIN
  /******************************************************************************
  
    AUTHOR:    Samuele Furnari

    NAME:      ModCountInMngrTbl

    PURPOSE:   Ogni volta che viene modificato il flag repertorio o il valore 
               che indica la tipologia di repertorio, bisogna agire sull'anagrafica
 
  ******************************************************************************/
  
  -- Eliminazione dei riferimenti del repertorio dall'anagrafica
  DeleteRegistroRepertorio(:new.system_id);
  
  Select ta.system_id Into idTipologia 
  From dpa_tipo_atto ta 
  Inner Join dpa_ogg_custom_comp occ 
  On ta.system_id = occ.id_template
  Where occ.id_ogg_custom = :new.system_id;
  
  -- Se e stato cambiato lo stato del flag repertorio, viene ed e stato passato
  -- ad 1, viene eseguito l'inserimento di un riferimento nell'anagrafica
  If :new.repertorio = '1' Then
    InsertRegistroRepertorio(idTipologia, :new.system_id, :new.cha_tipo_tar, 'D');
  End If;
  
End;
/
ALTER TRIGGER "ITCOLL_6GIU12"."MODCOUNTDOCINMNGRTBL" ENABLE;
