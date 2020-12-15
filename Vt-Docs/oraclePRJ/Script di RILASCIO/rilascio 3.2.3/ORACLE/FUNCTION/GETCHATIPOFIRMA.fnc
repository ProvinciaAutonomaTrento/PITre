CREATE OR REPLACE FUNCTION getchatipofirma (docnum number)
RETURN VARCHAR2
IS
tipoFirma   VARCHAR2 (16);
BEGIN

DECLARE
vmaxidgenerica   NUMBER;

BEGIN
BEGIN
SELECT MAX (v1.version_id)
INTO vmaxidgenerica
FROM VERSIONS v1, components c
WHERE v1.docnumber = docnum AND v1.version_id = c.version_id;
EXCEPTION
WHEN OTHERS
THEN
vmaxidgenerica := 0;
END;

BEGIN
SELECT cha_tipo_firma
INTO tipoFirma
FROM components
WHERE docnumber = docnum AND version_id = vmaxidgenerica;
EXCEPTION
WHEN OTHERS
THEN
tipoFirma := 'N';
END;

END;

RETURN tipoFirma;
END getchatipofirma;
/