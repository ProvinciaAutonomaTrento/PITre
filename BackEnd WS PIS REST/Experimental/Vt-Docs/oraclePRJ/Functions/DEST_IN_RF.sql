--------------------------------------------------------
--  DDL for Function DEST_IN_RF
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."DEST_IN_RF" (IdProfile INT)
RETURN varchar IS risultato clob;

item clob;

CURSOR cur IS
SELECT B.VAR_DESC_CORR
FROM DPA_DOC_ARRIVO_PAR L, DPA_L_RUOLO_REG C, DPA_CORR_GLOBALI B, DPA_CORR_GLOBALI D
WHERE
L.ID_PROFILE = IdProfile AND
L.CHA_TIPO_MITT_DEST = 'F' AND
L.ID_MITT_DEST = D.SYSTEM_ID AND
D.ID_RF = C.ID_REGISTRO AND
C.ID_RUOLO_IN_UO = B.SYSTEM_ID;

BEGIN
risultato := NULL;
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

risultato := risultato||item || '(D) ';

END LOOP;

RETURN risultato;

END DEST_IN_RF; 

/
