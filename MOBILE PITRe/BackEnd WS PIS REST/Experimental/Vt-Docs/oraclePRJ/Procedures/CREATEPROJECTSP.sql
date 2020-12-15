--------------------------------------------------------
--  DDL for Procedure CREATEPROJECTSP
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."CREATEPROJECTSP" (
p_idpeople                 NUMBER,
p_description              VARCHAR,
p_idpeopledelegato         NUMBER,
p_projectid          OUT   NUMBER
)
IS
BEGIN
DECLARE
projid   NUMBER;
BEGIN
p_projectid := 0;

SELECT seq.NEXTVAL
INTO projid
FROM DUAL;

p_projectid := projid;

<<inserimento_in_project>>
BEGIN
INSERT INTO project
(system_id, description, iconized, id_people_delegato
)
VALUES (p_projectid, p_description, 'Y', p_idpeopledelegato
);
EXCEPTION
WHEN OTHERS
THEN
p_projectid := 0;
RETURN;
END inserimento_in_project;

<<inserimento_security>>
BEGIN
INSERT INTO security
(thing, personorgroup, accessrights, id_gruppo_trasm,
cha_tipo_diritto
)
VALUES (p_projectid, p_idpeople, 0, NULL,
NULL
);
EXCEPTION
WHEN OTHERS
THEN
p_projectid := 0;
RETURN;
END inserimento_security;
END;
END;

/
