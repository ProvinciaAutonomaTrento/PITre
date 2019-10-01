--------------------------------------------------------
--  DDL for Trigger ROLEHISTORYCREATE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "ITCOLL_6GIU12"."ROLEHISTORYCREATE" 
BEFORE INSERT ON dpa_corr_globali 
FOR EACH ROW 
 WHEN (new.cha_tipo_urp = 'R' and new.original_id = new.system_id) DECLARE
    uoDescription varchar (256);
    uoCode varchar (128);
    roleTypeDescription varchar (64);
    roleTypeCode varchar (16);
BEGIN
  /******************************************************************************

    AUTHOR:    Samuele Furnari

    NAME:      ROLEHISTORYCREATE

    PURPOSE:   All'inserimento di un ruolo viene inserita una riga nella
               tabella dello storico
 
  ******************************************************************************/
  
  -- Inserimento di una tupla nella tabella della storia del ruolo
  INSERT
  INTO DPA_ROLE_HISTORY
    (
      SYSTEM_ID ,
      ACTION_DATE ,
      UO_ID ,
      ROLE_TYPE_ID ,
      ORIGINAL_CORR_ID ,
      ACTION ,
      ROLE_DESCRIPTION,
      ROLE_ID
    )
    VALUES
    (
      seqrolehistory.nextval,
      sysdate,
      :new.id_uo,
      :new.id_tipo_ruolo,
      :new.original_id,
      'C',
      :new.var_desc_corr || ' (' || :new.var_codice || ')',
      :new.system_id
    );
END;
/
ALTER TRIGGER "ITCOLL_6GIU12"."ROLEHISTORYCREATE" ENABLE;
