--------------------------------------------------------
--  DDL for Procedure SP_INS_EXT_IN_COMPONENTS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INS_EXT_IN_COMPONENTS" 
IS
profile_docnumber   NUMBER;
estensione          VARCHAR (30);
id_version          NUMBER;
id_last_version     NUMBER;
estensioneappo      VARCHAR (30);
indice number;

CURSOR curr_components
IS
SELECT docnumber, version_id,
SUBSTR (PATH, LENGTH (PATH) - INSTR (REVERSE (PATH), '.') + 2)
FROM components where path is not null;
BEGIN
OPEN curr_components;

LOOP
FETCH curr_components
INTO profile_docnumber, id_version, estensione;

EXIT WHEN curr_components%NOTFOUND;

BEGIN
SELECT MAX (VERSIONS.version_id)
INTO id_last_version
FROM VERSIONS, components
WHERE VERSIONS.docnumber = profile_docnumber
AND VERSIONS.version_id = components.version_id;




IF (UPPER (TRIM (estensione)) = 'P7M')
THEN
SELECT           case
when  instr(f.path, '\')=0
then f.path
else
SUBSTR (f.PATH,
LENGTH (f.PATH) - INSTR (REVERSE (f.PATH), '\') + 2,
LENGTH (f.PATH)
)
end
INTO estensioneappo
FROM components f
WHERE  f.docnumber = profile_docnumber and f.version_id = id_version;

SELECT SUBSTR (estensioneappo,
INSTR (estensioneappo, '.') + 1,
LENGTH (estensioneappo)
)
INTO estensione
FROM DUAL;

IF (UPPER (TRIM (estensione)) = 'P7M')
THEN
UPDATE components
SET ext = UPPER (estensione)
WHERE  docnumber = profile_docnumber and version_id = id_version
;
ELSE
SELECT SUBSTR (estensione,
LENGTH (estensione)
- INSTR (REVERSE (estensione), '.')
+ 2
)
INTO estensioneappo
FROM DUAL;

WHILE (UPPER (TRIM (estensioneappo)) = 'P7M')
LOOP
SELECT SUBSTR (estensione,
0,
LENGTH (estensione)
- INSTR (REVERSE (estensione), '.')
)
INTO estensione
FROM DUAL;

SELECT SUBSTR (estensione,
LENGTH (estensione)
- INSTR (REVERSE (estensione), '.')
+ 2
)
INTO estensioneappo
FROM DUAL;
END LOOP;

select instr(estensione,'.') into indice from dual;
if(indice>0) then

select substr(estensione, instr(estensione,'.')+1, length(estensione)) into estensione from dual;
end if;

IF (LENGTH (estensione) > 7)
THEN
estensione := SUBSTR (estensione, 0, 7);
END IF;

UPDATE components
SET ext = UPPER (estensione)
WHERE  docnumber = profile_docnumber and VERSION_ID=id_version;
END IF;

IF (id_version = id_last_version)
THEN
UPDATE components
SET cha_firmato = '1'
WHERE docnumber = profile_docnumber and version_id = id_version;
END IF;
ELSE
IF (id_version = id_last_version)
THEN
UPDATE components
SET cha_firmato = '0'
WHERE docnumber = profile_docnumber and version_id = id_version;
END IF;

select instr(estensione,'.') into indice from dual;
if(indice>0) then

select substr(estensione, instr(estensione,'.')+1, length(estensione)) into estensione from dual;
end if;


IF (LENGTH (estensione) > 7)
THEN
estensione := SUBSTR (estensione, 0, 7);
END IF;

UPDATE components
SET ext = UPPER (estensione)
WHERE version_id = id_version AND docnumber = profile_docnumber;
END IF;
--UPDATE components SET EXT=upper(estensione) where  docnumber=profile_docnumber and VERSION_ID=id_version;
EXCEPTION
WHEN OTHERS
THEN
NULL;
END;
END LOOP;

CLOSE curr_components;
END sp_ins_ext_in_components; 

/
