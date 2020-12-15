--------------------------------------------------------
--  DDL for Function VARDESCRIBE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."VARDESCRIBE" 
(sysid INT, typetable VARCHAR)
RETURN VARCHAR2
IS
risultato   VARCHAR2 (8000);
tmpvar      VARCHAR2 (8000);
tipo        CHAR;
num_proto   NUMBER;
doc_number  NUMBER;
BEGIN
BEGIN
tmpvar := NULL;

--MAIN
IF (typetable = 'PEOPLENAME')
THEN
SELECT var_desc_corr
INTO risultato
FROM dpa_corr_globali
WHERE id_people = sysid AND cha_tipo_urp = 'P' AND cha_tipo_ie = 'I';
END IF;

IF (typetable = 'GROUPNAME')
THEN
SELECT var_desc_corr
INTO risultato
FROM dpa_corr_globali
WHERE system_id = sysid AND cha_tipo_urp = 'R';
END IF;

IF (typetable = 'DESC_RUOLO')
THEN
SELECT var_desc_corr
INTO risultato
FROM dpa_corr_globali
WHERE id_gruppo = sysid AND cha_tipo_urp = 'R';
END IF;

IF (typetable = 'RAGIONETRASM')
THEN
SELECT var_desc_ragione
INTO risultato
FROM dpa_ragione_trasm
WHERE system_id = sysid;
END IF;

IF (typetable = 'TIPO_RAGIONE')
THEN
SELECT cha_tipo_ragione
INTO risultato
FROM dpa_ragione_trasm
WHERE system_id = sysid;
END IF;

IF (typetable = 'DATADOC')
THEN
BEGIN
SELECT cha_tipo_proto, NVL (num_proto, 0)
INTO tipo, num_proto
FROM PROFILE
WHERE system_id = sysid;

IF (    tipo IS NOT NULL
AND (tipo IN ('A', 'P', 'I') AND num_proto != 0)
)
THEN
SELECT TO_CHAR (dta_proto, 'dd/mm/yyyy')
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
ELSE
SELECT TO_CHAR (creation_date, 'dd/mm/yyyy')
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
END IF;
END;
END IF;

IF (typetable = 'CHA_TIPO_PROTO')
THEN
SELECT cha_tipo_proto
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
END IF;

IF (typetable = 'NUMPROTO')
THEN
SELECT num_proto
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
END IF;

IF (typetable = 'CODFASC')
THEN
SELECT var_codice
INTO risultato
FROM project
WHERE system_id = sysid;
END IF;

IF (typetable = 'DTA_CREAZ')
THEN
SELECT TO_CHAR (dta_creazione, 'yyyy')
INTO risultato
FROM project
WHERE system_id = sysid;
END IF;

IF (typetable = 'NUM_FASC')
THEN
SELECT num_fascicolo
INTO risultato
FROM project
WHERE system_id = sysid;
END IF;

IF (typetable = 'DESC_OGGETTO')
THEN
SELECT var_prof_oggetto
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
END IF;

IF (typetable = 'DESC_FASC')
THEN
BEGIN
SELECT description
INTO risultato
FROM project
WHERE system_id = sysid;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
risultato := '';
END;
END IF;

IF (typetable = 'PROF_IDREG')
THEN
BEGIN
IF sysid IS NOT NULL
THEN
BEGIN
SELECT id_registro
INTO risultato
FROM PROFILE
WHERE system_id = sysid;

IF (risultato IS NULL)
THEN
risultato := '0';
END IF;
END;
ELSE
risultato := '0';
END IF;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
risultato := '0';
END;
END IF;

IF (typetable = 'ID_GRUPPO')
THEN
BEGIN
IF sysid IS NOT NULL
THEN
BEGIN
SELECT id_gruppo
INTO risultato
FROM dpa_corr_globali
WHERE system_id = sysid;

IF (risultato IS NULL)
THEN
risultato := '0';
END IF;
END;
ELSE
risultato := '0';
END IF;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
risultato := '0';
END;
END IF;

IF (typetable = 'SEGNATURA_DOCNUMBER')
THEN
BEGIN
SELECT var_segnatura
INTO risultato
FROM PROFILE
WHERE system_id = sysid;

IF (risultato IS NULL)
THEN
SELECT docnumber
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
END IF;
END;
END IF;

IF (typetable = 'SEGNATURA_CODFASC')
THEN
BEGIN
SELECT num_proto
INTO risultato
FROM PROFILE
WHERE system_id = sysid;

IF (risultato IS NULL)
THEN
SELECT docnumber
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
END IF;
END;
END IF;

IF (typetable = 'OGGETTO_MITTENTE')
THEN
BEGIN
-- OGGETTO
SELECT var_prof_oggetto
INTO risultato
FROM PROFILE
WHERE system_id = sysid;

--MITTENTE
BEGIN
SELECT var_desc_corr
INTO tmpvar
FROM (SELECT var_desc_corr
FROM dpa_corr_globali a, dpa_doc_arrivo_par b
WHERE b.id_mitt_dest = a.system_id
AND b.cha_tipo_mitt_dest = 'M'
AND b.id_profile = sysid)
WHERE ROWNUM = 1;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
tmpvar := '';
END;

IF (tmpvar IS NOT NULL)
THEN
risultato := risultato || '@@' || tmpvar;
END IF;
END;
END IF;

IF (typetable = 'PROFILE_CHA_IMG')
THEN
SELECT getchaimg (docnumber)
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
END IF;

IF (typetable = 'PROFILE_CHA_FIRMATO')
THEN
SELECT CHA_FIRMATO
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
END IF;

IF (typetable = 'COMPONENTS_CHA_FIRMATO')
THEN
SELECT getchafirmato(docnumber)
INTO risultato
FROM PROFILE
WHERE system_id = sysid;
END IF;

--ENDMAIN
EXCEPTION
WHEN NO_DATA_FOUND
THEN
NULL;
WHEN OTHERS
THEN
RAISE;
END;
RETURN risultato;
end;

/
