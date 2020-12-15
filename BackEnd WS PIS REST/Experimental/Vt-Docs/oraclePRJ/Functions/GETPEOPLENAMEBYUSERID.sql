--------------------------------------------------------
--  DDL for Function GETPEOPLENAMEBYUSERID
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETPEOPLENAMEBYUSERID" (userid varchar)
RETURN varchar IS risultato varchar(256);
BEGIN

select full_name into risultato from people where upper(user_id) = upper(userid);

RETURN risultato;
END getPeopleNamebyUserid; 

/
