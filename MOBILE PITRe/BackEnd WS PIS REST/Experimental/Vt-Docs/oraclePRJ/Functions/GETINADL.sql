--------------------------------------------------------
--  DDL for Function GETINADL
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETINADL" (sysID int, typeID char,idGruppo INT,idPeople INT)

RETURN INT IS risultato INT;


BEGIN --generale


begin --interno

IF (typeID = 'D') THEN
SELECT DISTINCT(SYSTEM_ID) INTO risultato FROM DPA_AREA_LAVORO WHERE ID_PROFILE = sysID
AND ID_PEOPLE =idPeople and ID_RUOLO_IN_UO = (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = idGruppo);
IF (risultato > 0) THEN
risultato := 1;
END IF;
END IF;
IF (typeID = 'F') THEN
SELECT DISTINCT(SYSTEM_ID) INTO risultato FROM DPA_AREA_LAVORO WHERE ID_PROJECT = sysID
AND ID_PEOPLE =idPeople and ID_RUOLO_IN_UO = (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = idGruppo);
IF (risultato > 0) THEN
risultato := 1;
END IF;
END IF;


EXCEPTION
WHEN NO_DATA_FOUND THEN risultato := 0;
WHEN OTHERS THEN risultato := 0;

end;

RETURN risultato;
END getInADL; 

/