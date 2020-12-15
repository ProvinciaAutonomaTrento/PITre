--------------------------------------------------------
--  DDL for Trigger ROLEHISTORYMODIFY
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "ITCOLL_6GIU12"."ROLEHISTORYMODIFY" 
BEFORE UPDATE ON DPA_CORR_GLOBALI 
FOR EACH ROW 
 WHEN (new.cha_tipo_urp = 'R' and
        new.dta_fine is null and
        ( new.id_old != old.id_old or 
          new.var_codice != old.var_codice or 
          new.var_desc_corr != old.var_desc_corr or
          new.id_uo != old.id_uo or
          new.id_tipo_ruolo != old.id_tipo_ruolo)) DECLARE
  idLastInsert integer;
BEGIN
  /******************************************************************************

    AUTHOR:    Samuele Furnari

    NAME:      ROLEHISTORYMODIFY

    PURPOSE:   Ogni volta che viene modificato un record nella dpa_corr_globali,
               se il record e relativo ad un Ruolo, se non e stata inserita
               la dta_fine e se e stato modificato almeno uno dei campi 
               monitorati, viene inserita una riga di tipo M nella tabella dello
               storico. Se invece e stato impostato l'id_old, significa che e
               stato storicizzato un ruolo, quindi viene inserito un record di
               tipo S nella tabella dello storico
 
  ******************************************************************************/

  -- Verifica eventuale cambiamento su codice del ruolo
  if :old.id_old != :new.id_old then
    -- Nella dpa_role_history bisogna preventivamente aggiornare il campo
    -- role_id con il nuovo system_id ed in seguito inserire una nuova riga
    -- con id_old uguale a quella del record appena inserito
    UPDATE dpa_role_history
    SET role_id = :new.id_old
    WHERE role_id = :old.system_id;
    
    -- Il ruolo id_old e stato storicizzato (inserimento di un record 
    -- di storicizzazione)
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
          'S',
          :new.var_desc_corr || ' (' || :new.var_codice || ')',
          :new.system_id
        );
    
  else
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
          'M',
          :new.var_desc_corr || ' (' || :new.var_codice || ')',
          :new.system_id
        );
    
  end if;

END;
/
ALTER TRIGGER "ITCOLL_6GIU12"."ROLEHISTORYMODIFY" ENABLE;
