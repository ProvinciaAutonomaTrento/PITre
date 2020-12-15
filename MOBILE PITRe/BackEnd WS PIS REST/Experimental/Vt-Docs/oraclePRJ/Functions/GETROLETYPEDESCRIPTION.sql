--------------------------------------------------------
--  DDL for Function GETROLETYPEDESCRIPTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETROLETYPEDESCRIPTION" 
(
  roleTypeId integer
)
RETURN VARCHAR2 deterministic AS 
  code VARCHAR (128);
  description VARCHAR (256);
BEGIN

  
    /******************************************************************************

    AUTHOR:    Samuele Furnari

    NAME:      GETCORRDESCRIPTION

    PURPOSE:   Funzione per la costruzione della descrizione completa di un tipo ruolo
 
  ******************************************************************************/



  select var_codice, var_desc_ruolo into code, description from dpa_tipo_ruolo where system_id = roleTypeId;
  
  if(description is not null and code is not null) then
    RETURN description || ' (' || code || ')';
  else
    RETURN '';
  end if;
  
END GETROLETYPEDESCRIPTION; 

/
