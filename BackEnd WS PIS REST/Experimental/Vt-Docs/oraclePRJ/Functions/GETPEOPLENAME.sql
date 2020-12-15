--------------------------------------------------------
--  DDL for Function GETPEOPLENAME
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETPEOPLENAME" (peopleId INT)
RETURN varchar IS risultato varchar(256);
BEGIN

select full_name into risultato from people where system_id = peopleId;

RETURN risultato;
END getPeopleName; 

/
