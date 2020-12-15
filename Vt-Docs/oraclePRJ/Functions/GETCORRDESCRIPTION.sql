--------------------------------------------------------
--  DDL for Function GETCORRDESCRIPTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCORRDESCRIPTION" 
    (  idCorrGlobali integer)
RETURN VARCHAR2 deterministic AS 
  code VARCHAR (128);
  description VARCHAR (256);
BEGIN
    /******************************************************************************
    AUTHOR:    Samuele Furnari
    NAME:      GETCORRDESCRIPTION
    PURPOSE:   Funzione per la costruzione 
                della descrizione completa di un ruolo
   ******************************************************************************/
  
select var_codice, var_desc_corr into code, description 
  from dpa_corr_globali
  where system_id = idCorrGlobali;
  
  if(code is not null and code is not null) then
    RETURN description || ' (' || code || ')';
  else     RETURN '';
  end if;  
  
END GETCORRDESCRIPTION; 

/
