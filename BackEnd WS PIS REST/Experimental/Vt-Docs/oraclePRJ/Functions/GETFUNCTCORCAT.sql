--------------------------------------------------------
--  DDL for Function GETFUNCTCORCAT
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETFUNCTCORCAT" (idruolo INT)
RETURN VARCHAR IS risultato VARCHAR(256);
BEGIN
declare item varchar(4000);

CURSOR cur IS
SELECT getcodtipofunz(id_tipo_funz) FROM dpa_tipo_f_ruolo WHERE id_ruolo_in_uo=idruolo;

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
