create or replace
FUNCTION          getaccessrights (
id_ruolo    NUMBER,
id_people   NUMBER,
id_ruolo_pubblico   NUMBER,
systemid    NUMBER
)
RETURN NUMBER
IS
risultato               NUMBER;
iddocumentoprincipale   NUMBER := NULL;
thingvar                NUMBER := NULL;
BEGIN
thingvar := systemid;

BEGIN
risultato :=0;--checkgestionearchivio ( id_ruolo );

IF (risultato = 0)
THEN
BEGIN
BEGIN
SELECT id_documento_principale
INTO iddocumentoprincipale
FROM PROFILE
WHERE system_id = thingvar;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
iddocumentoprincipale := NULL;
END;

IF (NOT iddocumentoprincipale IS NULL)
THEN
thingvar := iddocumentoprincipale;
END IF;

SELECT MAX (accessrights)
INTO risultato
FROM security
WHERE thing = thingvar AND personorgroup IN
(id_ruolo, id_people, id_ruolo_pubblico);
EXCEPTION
WHEN OTHERS
THEN
risultato := -1;
END;
ELSE
--per archivisti tutto il solo lettura
risultato := 45;
END IF;
END;

RETURN risultato;
END getaccessrights; 
 