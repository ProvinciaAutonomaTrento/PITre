--------------------------------------------------------
--  DDL for Function UTENTEHASQUALIFICA
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."UTENTEHASQUALIFICA" 
(
	codiceQualifica varchar,
	idPeople int
) 
RETURN int 
IS
mycount int ; 
retValue int ; 
BEGIN
	
	if codiceQualifica  is null 
	THEN
		retValue :=  0 ; 	return retValue ; 
	ELSE 
		select COUNT(pgq.SYSTEM_ID) into mycount
		from DPA_PEOPLEGROUPS_QUALIFICHE pgq
		inner join DPA_QUALIFICHE q on pgq.ID_QUALIFICA = q.SYSTEM_ID
		where q.CHA_COD = codiceQualifica and pgq.ID_PEOPLE = idPeople ; 
	END	IF;

	if (mycount > 0)
	THEN	retValue := 1 ; 
	ELSE
			retValue :=  0 ; 
	END IF; 
		
	return retValue ; 
END;

/
