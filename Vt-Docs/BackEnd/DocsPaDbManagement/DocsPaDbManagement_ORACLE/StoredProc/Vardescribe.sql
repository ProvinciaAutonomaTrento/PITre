CREATE OR REPLACE FUNCTION Vardescribe (sysid INT, typeTable VARCHAR)
RETURN VARCHAR2 IS risultato VARCHAR2(8000);

TMPVAR VARCHAR2(8000);
TIPO CHAR;
num_proto number;
BEGIN

begin
tmpVar:= null;
--MAIN
IF(typeTable = 'PEOPLENAME') THEN SELECT VAR_DESC_CORR INTO risultato FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = sysid AND CHA_TIPO_URP='P' AND CHA_TIPO_IE = 'I';
END IF;
IF(typeTable = 'GROUPNAME') THEN SELECT VAR_DESC_CORR INTO risultato FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = sysid AND CHA_TIPO_URP='R';
END IF;
IF(typeTable = 'RAGIONETRASM') THEN SELECT VAR_DESC_RAGIONE INTO risultato FROM DPA_RAGIONE_TRASM WHERE SYSTEM_ID= sysid;
END IF;
IF(typeTable = 'TIPO_RAGIONE') THEN SELECT CHA_TIPO_RAGIONE INTO risultato FROM DPA_RAGIONE_TRASM WHERE SYSTEM_ID= sysid;
END IF;
IF(typeTable = 'DATADOC') THEN
BEGIN
SELECT CHA_TIPO_PROTO,nvl(num_proto,0) INTO TIPO,num_proto FROM PROFILE WHERE SYSTEM_ID=sysid;
IF(TIPO IS NOT NULL AND (TIPO  IN ('A','P','I') and num_proto !=0 ))
THEN
SELECT TO_CHAR(DTA_PROTO,'dd/mm/yyyy')  INTO risultato FROM PROFILE WHERE SYSTEM_ID = sysid;
ELSE
SELECT TO_CHAR(CREATION_DATE,'dd/mm/yyyy') INTO risultato FROM PROFILE WHERE SYSTEM_ID = sysid;
END IF;
END;
END IF;
IF(typeTable = 'CHA_TIPO_PROTO') THEN SELECT CHA_TIPO_PROTO INTO risultato FROM PROFILE WHERE SYSTEM_ID= sysid;
END IF;
IF(typeTable = 'NUMPROTO') THEN SELECT NUM_PROTO INTO risultato FROM PROFILE WHERE SYSTEM_ID= sysid;
END IF;
IF(typeTable = 'CODFASC') THEN SELECT VAR_CODICE INTO risultato FROM PROJECT WHERE SYSTEM_ID= sysid;
END IF;
IF(typeTable = 'DESC_OGGETTO') THEN SELECT VAR_PROF_OGGETTO INTO risultato FROM PROFILE WHERE SYSTEM_ID= sysid;
END IF;
IF(typeTable = 'DESC_FASC') THEN
BEGIN
SELECT DESCRIPTION INTO risultato FROM PROJECT WHERE SYSTEM_ID= sysid;
EXCEPTION WHEN NO_DATA_FOUND THEN risultato := '';
END;

END IF;

IF(typeTable = 'PROF_IDREG') THEN
BEGIN
IF sysid IS NOT NULL THEN
BEGIN
SELECT ID_REGISTRO INTO risultato FROM PROFILE WHERE SYSTEM_ID= sysid;
IF (risultato IS NULL) THEN risultato := '0';
END IF;
END;
ELSE risultato := '0';
END IF;
EXCEPTION WHEN NO_DATA_FOUND THEN risultato := '0';
END;
END IF;

IF(typeTable = 'ID_GRUPPO') THEN
BEGIN
IF sysid IS NOT NULL THEN
BEGIN
SELECT ID_GRUPPO INTO risultato FROM DPA_CORR_GLOBALI WHERE system_id = sysid;
IF (risultato IS NULL) THEN risultato := '0';
END IF;
END;
ELSE risultato := '0';
END IF;
EXCEPTION WHEN NO_DATA_FOUND THEN risultato := '0';
END;
END IF;

IF(typeTable = 'SEGNATURA_DOCNUMBER') THEN
BEGIN
SELECT VAR_SEGNATURA INTO risultato FROM PROFILE WHERE SYSTEM_ID= sysid;
IF (risultato IS NULL) THEN
SELECT DOCNUMBER INTO risultato FROM PROFILE WHERE SYSTEM_ID= sysid;
END IF;
END;
END IF;
IF(typeTable = 'OGGETTO_MITTENTE') THEN
BEGIN
-- OGGETTO
SELECT VAR_PROF_OGGETTO INTO risultato FROM PROFILE WHERE SYSTEM_ID= sysid;

--MITTENTE
BEGIN
SELECT var_desc_corr INTO TMPVAR FROM DPA_CORR_GLOBALI a, DPA_DOC_ARRIVO_PAR b
WHERE b.id_mitt_dest=a.system_id AND b.cha_tipo_mitt_dest='M'
AND b.id_profile=sysid;
EXCEPTION WHEN NO_DATA_FOUND THEN TMPVAR := '';
END;

IF (TMPVAR IS NOT NULL) THEN  risultato := risultato || '@@' || TMPVAR;
END IF;
END;

END IF;

--ENDMAIN

EXCEPTION
WHEN NO_DATA_FOUND THEN
null;
WHEN OTHERS THEN
raise;
end;
RETURN risultato;
END Vardescribe;
/