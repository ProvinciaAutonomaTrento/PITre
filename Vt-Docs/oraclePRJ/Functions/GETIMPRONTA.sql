--------------------------------------------------------
--  DDL for Function GETIMPRONTA
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETIMPRONTA" (docnum number)
RETURN VARCHAR2
IS
impronta   VARCHAR2 (500);
vmaxidgenerica   NUMBER;

BEGIN
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

RETURN impronta;
END getImpronta;

/
