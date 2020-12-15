--------------------------------------------------------
--  DDL for Procedure CREATEDOCSP
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."CREATEDOCSP" (
p_idpeople                 NUMBER,
p_doctype                  VARCHAR,
p_idpeopledelegato         NUMBER,
p_isFirmato                       VARCHAR,
p_systemid           OUT   NUMBER
)
IS
BEGIN
DECLARE
docnum      NUMBER;
verid       NUMBER;
iddoctype   NUMBER;
BEGIN
p_systemid := 0;

<<reperimento_documenttypes>>
BEGIN
SELECT system_id
INTO iddoctype
FROM documenttypes
WHERE type_id = p_doctype;
EXCEPTION
WHEN NO_DATA_FOUND
THEN
p_systemid := 0;
RAISE;
END reperimento_documenttypes;

SELECT seq.NEXTVAL
INTO docnum
FROM DUAL;

p_systemid := docnum;

<<inserimento_in_profile>>
BEGIN
INSERT INTO PROFILE
(system_id, typist, author, documenttype,
creation_date, creation_time, docnumber,
id_people_delegato
)
VALUES (docnum, p_idpeople, p_idpeople, iddoctype,
SYSDATE, SYSDATE, docnum,
p_idpeopledelegato
);
EXCEPTION
WHEN OTHERS
THEN
p_systemid := 0;
RAISE;
END inserimento_in_profile;

<<inserimento_in_versions>>
BEGIN
SELECT seq.NEXTVAL
INTO verid
FROM DUAL;

INSERT INTO VERSIONS
(version_id, docnumber, VERSION, subversion,
version_label, author, typist, dta_creazione,
id_people_delegato
)
VALUES (verid, docnum, 1, '!',
'1', p_idpeople, p_idpeople, SYSDATE,
p_idpeopledelegato
);
EXCEPTION
WHEN OTHERS
THEN
p_systemid := 0;
RAISE;
END inserimento_in_versions;

<<inserimento_in_components>>
BEGIN
INSERT INTO components
(version_id, docnumber, file_size, CHA_FIRMATO
)
VALUES (verid, docnum, 0, p_isFirmato
);
EXCEPTION
WHEN OTHERS
THEN
p_systemid := 0;
RAISE;
END inserimento_in_components;

<<inserimento_security>>
BEGIN
INSERT INTO security
(thing, personorgroup, accessrights, id_gruppo_trasm,
cha_tipo_diritto
)
VALUES (docnum, p_idpeople, 0, NULL,
NULL
);
EXCEPTION
WHEN OTHERS
THEN
p_systemid := 0;
RAISE;
END inserimento_security;
END;
END; 

/
