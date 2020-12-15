--------------------------------------------------------
--  DDL for Procedure SETRIGHTSFORROLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SETRIGHTSFORROLE" (
  objId Number,
  idRole Number,
  rightsToAssign Number,
  returnValue Out Number
) AS 
BEGIN
  /******************************************************************************
  
    AUTHOR:   Samuele Furnari

    NAME:     SetRightsForRole

    PURPOSE:  Store per l'assegnazione dei diritti di tipo A ad un ruolo (solo
              se il ruolo non li possiede gi o se non possiede diritti superiori)

  ******************************************************************************/


  -- Diritti posseduti dal ruolo
  Declare rights VarChar (2000);
  
  Begin
    -- Selezione degli eventuali diritti posseduti da un ruolo
    Select Max(AccessRights) Into rights From  security Where Thing = objId And PersonOrGroup = idRole;
    
    -- Se i diritti sono ci sono, si procede con un inserimento
    IF rights Is Null THEN
      Insert Into security
      (
        THING,
        PERSONORGROUP,
        ACCESSRIGHTS,
        ID_GRUPPO_TRASM,
        CHA_TIPO_DIRITTO,
        HIDE_DOC_VERSIONS,
        TS_INSERIMENTO,
        VAR_NOTE_SEC
      )
      VALUES
      (
        objId,
        idRole,
        rightsToAssign,
        null,
        'A',
        null,
        sysdate,
        null
      );
    Else
      Begin
        -- Se i diritti posseduti dal ruolo sono minori di quelli che si vogliono concedere,
        -- si procede con un aggiornamento del diritto
        IF to_number(rights) != 0 And rightsToAssign > to_number(rights) THEN
          Update security
          Set ACCESSRIGHTS = rightsToAssign,
          CHA_TIPO_DIRITTO = 'A'
          Where Thing = objId;
          
        END IF ;
      End;
      
    END IF ;
  
    returnValue := 1;
  End;
End Setrightsforrole; 

/
