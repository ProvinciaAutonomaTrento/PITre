create or replace 
FUNCTION          getImprontaWithAllegati (docnum number)
RETURN CLOB
IS el_impronta CLOB;

impronta   CLOB;
temp INT;
vmaxidgenerica   NUMBER;

CURSOR cur IS
SELECT var_impronta
FROM components c, versions v, profile p
WHERE p.id_documento_principale = docnum
AND c.docnumber = p.docnumber AND v.docnumber = p.docnumber
AND v.version_id = c.version_id
AND v.version = 
(
SELECT MAX(VERSION) FROM versions WHERE docnumber = p.docnumber AND VERSION <> 0
);


BEGIN
/* doc principale */
el_impronta := EMPTY_CLOB();
BEGIN
SELECT MAX (v1.version_id) 			INTO vmaxidgenerica
FROM VERSIONS v1, components c
WHERE v1.docnumber = docnum AND v1.version_id = c.version_id;

EXCEPTION
WHEN NO_DATA_FOUND 	THEN
vmaxidgenerica := 0;
WHEN OTHERS 		THEN RAISE;
END;

BEGIN
SELECT var_impronta 			INTO impronta
FROM components
WHERE docnumber = docnum AND version_id = vmaxidgenerica;

EXCEPTION
WHEN NO_DATA_FOUND 	THEN
impronta := '';
WHEN OTHERS THEN RAISE;
END;

el_impronta := el_impronta || impronta;

/* allegati */
BEGIN

SELECT COUNT(system_id) INTO temp
FROM profile
WHERE id_documento_principale = docnum;

IF temp>0 THEN

	FOR el IN cur
	LOOP
    if el.var_impronta IS NOT NULL THEN
      el_impronta := el_impronta || '; ' || el.var_impronta;
    END IF;
	END LOOP; 
  
END IF;
	
END;

RETURN el_impronta;
END getImprontaWithAllegati;