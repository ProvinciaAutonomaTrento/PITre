
--------------------------------------------------------
--  DDL for Function DEST_IN_LISTA
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."DEST_IN_LISTA" (IdProfile INT)
RETURN varchar IS risultato varchar(4000);

item varchar(4000);

CURSOR cur IS
SELECT L.VAR_DESC_CORR
FROM DPA_LISTE_DISTR B, DPA_CORR_GLOBALI L, DPA_DOC_ARRIVO_PAR C
WHERE
C.ID_PROFILE = IdProfile AND
C.CHA_TIPO_MITT_DEST = 'L' AND
B.ID_LISTA_DPA_CORR = C.ID_MITT_DEST
AND L.SYSTEM_ID=B.ID_DPA_CORR
AND L.DTA_FINE IS NULL;

BEGIN
risultato := NULL;
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

risultato := risultato||item || '(D) ';

END LOOP;

RETURN risultato;

END DEST_IN_LISTA;

/
