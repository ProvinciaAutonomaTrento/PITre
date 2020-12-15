--------------------------------------------------------
--  DDL for Function GETCODREGCORCAT
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODREGCORCAT" (idruolo INT)
RETURN VARCHAR IS risultato VARCHAR(256);
BEGIN
declare item varchar(4000);

CURSOR cur IS
SELECT getcodreg(id_registro) FROM dpa_l_ruolo_reg WHERE id_ruolo_in_uo=idruolo;

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ;
ELSE
risultato := risultato||item;
END IF;

END LOOP;

exception  WHEN OTHers then risultato:='';
end;

RETURN risultato;
end; 

/
