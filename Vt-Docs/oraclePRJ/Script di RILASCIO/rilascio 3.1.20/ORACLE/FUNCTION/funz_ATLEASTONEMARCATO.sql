create or replace 
FUNCTION AtLeastOneMarcato (iddoc number)
RETURN VARCHAR2
IS
isMarcato   VARCHAR2 (16);

BEGIN

DECLARE
vmaxidgenerica   NUMBER;
docnum			 NUMBER;

BEGIN
    SELECT MAX (v1.version_id)
    INTO vmaxidgenerica
    FROM VERSIONS v1, components c
    WHERE v1.docnumber = iddoc AND v1.version_id = c.version_id;
    
    BEGIN
      SELECT MAX(system_id)
      INTO docnum
      FROM dpa_timestamp_doc
      WHERE doc_number=iddoc AND version_id = vmaxidgenerica;
    EXCEPTION
      WHEN NO_DATA_FOUND THEN
       docnum := NULL;
       isMarcato := '0';
    END;
	
	IF(docnum IS NULL) THEN
    isMarcato := '0';
		BEGIN
			DECLARE 
			item VARCHAR(255);
			maxVersion NUMBER;
			ddocnum NUMBER;
			CURSOR curAllegato IS select system_id from profile where id_documento_principale = iddoc;
			BEGIN
				OPEN curAllegato;
				LOOP
				FETCH curAllegato INTO item;
				EXIT WHEN (curAllegato%NOTFOUND or isMarcato='1');
					SELECT MAX (v1.version_id)
					INTO maxVersion
					FROM VERSIONS v1, components c
					WHERE v1.docnumber = item AND v1.version_id = c.version_id;
					
          BEGIN
            SELECT MAX(system_id)
            INTO ddocnum
            FROM dpa_timestamp_doc
            WHERE doc_number=item AND version_id = maxVersion;
          EXCEPTION
            WHEN NO_DATA_FOUND THEN
              ddocnum := NULL;
          END;
					
					IF(ddocnum IS NULL) THEN
						isMarcato := '0';
					ELSE
						isMarcato := '1';
					END IF;
				END LOOP;
				CLOSE curAllegato;
			END;
		END;
	ELSE
		isMarcato := '1' ;
	END IF;
	
END;

RETURN isMarcato;

END AtLeastOneMarcato;