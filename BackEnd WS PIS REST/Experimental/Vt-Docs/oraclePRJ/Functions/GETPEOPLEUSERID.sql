--------------------------------------------------------
--  DDL for Function GETPEOPLEUSERID
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETPEOPLEUSERID" (peopleId INT)
RETURN varchar IS risultato varchar(256);
BEGIN

select user_id into risultato from people where system_id = peopleId;

RETURN risultato;
END getPeopleUserId; 

/
