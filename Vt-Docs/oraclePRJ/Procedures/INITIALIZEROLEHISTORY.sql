--------------------------------------------------------
--  DDL for Procedure INITIALIZEROLEHISTORY
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."INITIALIZEROLEHISTORY" As
idRole number;
idUo number;
idRoleType number;
originalId number;
startTime date;
description varchar (256);
code varchar (128);
BEGIN
    
  /******************************************************************************
  
    AUTHOR:    Samuele Furnari

    NAME:      InitializeRoleHistory

    PURPOSE:   Store procedure per l'inizializzazione della tabella dello storico
               dei ruoli
 
  ******************************************************************************/

    
    Declare Cursor Cursore Is 
      select id_uo, id_tipo_ruolo, original_id, dta_inizio, var_codice, var_desc_corr, System_Id
      from dpa_corr_globali 
      where id_old = '0' and system_id = original_id 
            and id_uo is not null
            and id_tipo_ruolo is not null;
            
    Begin Open Cursore;
    LOOP FETCH cursore INTO idUo, idRoleType, originalId, startTime, code, description, idRole ;
    EXIT WHEN cursore%NOTFOUND;

      insert
      INTO DPA_ROLE_HISTORY
      (
        SYSTEM_ID ,
        ACTION_DATE ,
        UO_ID ,
        ROLE_TYPE_ID ,
        ORIGINAL_CORR_ID ,
        Action ,
        Role_Description,
        ROLE_ID
      )
      VALUES
      (
        seqrolehistory.nextval ,
        startTime ,
        idUo ,
        idroletype ,
        originalid ,
        'C' ,
        Description || ' (' || Code || ')',
        Idrole
      );
    end loop;
    close cursore;
end;    
End Initializerolehistory; 

/
