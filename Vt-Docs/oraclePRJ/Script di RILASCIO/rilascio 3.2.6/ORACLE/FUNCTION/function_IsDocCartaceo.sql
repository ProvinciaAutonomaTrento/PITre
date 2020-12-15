-- Restituisce 1 se sia il documento principale che eventuali allegati utente sono cartacei
-- Restituisce 0 se almeno uno tra documento principale o allegati utente non è cartaceo, o se il documento proviene da spedizione
CREATE OR REPLACE
FUNCTION             IsDocCartaceo (docnum number)
RETURN VARCHAR2
IS
isCartaceo   VARCHAR2 (16);
BEGIN

DECLARE
vmaxidgenerica   NUMBER;

BEGIN
   SELECT MAX (v1.version_id)
   INTO vmaxidgenerica
   FROM VERSIONS v1, components c
   WHERE v1.docnumber = docnum AND v1.version_id = c.version_id;
  
-- CONTROLLO DOC PRINCIPALE
   SELECT NVL(cartaceo,'0')
   INTO isCartaceo
   FROM VERSIONS
   WHERE docnumber = docnum AND version_id = vmaxidgenerica;
END;

   IF(isCartaceo = '1') THEN
    -- CONTROLLO ALLEGATI UTENTE
    BEGIN
        declare item varchar(255);
        maxVersion number;
        typeAtt varchar2(16);
        CURSOR curAllegato IS select system_id from profile where id_documento_principale = docnum; 
        BEGIN
            OPEN curAllegato;
            LOOP
            FETCH curAllegato INTO item;
            EXIT WHEN (curAllegato%NOTFOUND or isCartaceo='0');
               SELECT MAX (v1.version_id)
               INTO maxVersion
               FROM VERSIONS v1, components c
               WHERE v1.docnumber = item AND v1.version_id = c.version_id;
               
               SELECT CHA_ALLEGATI_ESTERNO
               INTO typeAtt
               FROM VERSIONS WHERE version_id = maxVersion;
               
               IF(typeAtt = '0') THEN
                BEGIN
                  SELECT NVL(cartaceo,'0')
                  INTO isCartaceo
                  FROM VERSIONS
                  WHERE docnumber = item AND version_id = maxVersion;
                END;
               END IF;
            END LOOP;
            CLOSE curAllegato;
        END;
    END;    
    end if; -- CONTROLLO ALLEGATI UTENTE
	
	IF(isCartaceo = '1') THEN
		-- Se il documento e tutti gli allegati sono marcati come cartacei verifico se il documento risulta
		-- essere stato spedito da mail/interoperabilità o IS
		BEGIN			
      DECLARE checkMailInt VARCHAR2(16);
       BEGIN
        -- MAIL/PEC
        SELECT COUNT(A.ID_PROFILE)
        INTO checkMailInt
        FROM DPA_DOC_ARRIVO_PAR A, DOCUMENTTYPES B, PROFILE C
        WHERE A.ID_DOCUMENTTYPES = B.SYSTEM_ID AND A.ID_PROFILE = C.SYSTEM_ID AND A.ID_PROFILE = docnum
        AND (B.TYPE_ID = 'MAIL' OR B.TYPE_ID = 'INTEROPERABILITA' OR B.TYPE_ID = 'SIMPLIFIEDINTEROPERABILITY' OR B.TYPE_ID = 'SERVIZI ONLINE') 
        AND C.CHA_TIPO_PROTO = 'A';
			
        IF(checkMailInt = '0') THEN
          isCartaceo := '1';
        ELSE
  				isCartaceo := '0';		
        END IF; -- isMailPec
      END;
		END;
	END IF;
	
RETURN isCartaceo;

exception
when no_data_found
then
isCartaceo :=  '0';
RETURN isCartaceo;
when others
then
isCartaceo := '0'; 
RETURN isCartaceo;

End IsDocCartaceo; 
 