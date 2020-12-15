FUNCTION               classcat (docId INT)
RETURN VARCHAR IS risultato VARCHAR(4000);

item VARCHAR(4000);

CURSOR cur IS
	SELECT DISTINCT A.VAR_CODICE
	FROM PROJECT A
	WHERE A.CHA_TIPO_PROJ = 'F'
	AND A.SYSTEM_ID IN
	(SELECT A.ID_FASCICOLO FROM PROJECT A, PROJECT_COMPONENTS B WHERE A.SYSTEM_ID=B.PROJECT_ID AND B.LINK=docId);

BEGIN
	risultato := '';
	OPEN cur;
	LOOP
		FETCH cur INTO item;
		EXIT WHEN cur%NOTFOUND;

		IF(risultato IS NOT NULL) THEN
			risultato := risultato||'; '||item;
		ELSE
			risultato := risultato||item;
		END IF;

	END LOOP;

	RETURN risultato;

END classcat;
