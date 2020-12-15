-- Alessandro faillace per Original File name

create or replace
FUNCTION             getNomeOriginale (docnum number)
RETURN VARCHAR2
IS
nomeOriginale   VARCHAR2 (500);
vmaxidgenerica   NUMBER;

BEGIN
                                               BEGIN
                                               SELECT MAX (v1.version_id)                                     INTO vmaxidgenerica
                                               FROM VERSIONS v1, components c
                                               WHERE v1.docnumber = docnum AND v1.version_id = c.version_id;

                                                               EXCEPTION                                        
                                                               WHEN NO_DATA_FOUND          THEN
                                                               vmaxidgenerica := 0;
                                                               WHEN OTHERS                                 THEN RAISE; 
                                               END;

                                               BEGIN
                                               SELECT var_nomeOriginale                                        INTO nomeOriginale
                                               FROM components
                                               WHERE docnumber = docnum AND version_id = vmaxidgenerica;

                                                               EXCEPTION                                        
                                                               WHEN NO_DATA_FOUND          THEN
                                                               nomeOriginale := '';
                                                               WHEN OTHERS THEN RAISE; 
                                               END;

RETURN nomeOriginale;
END getNomeOriginale;
