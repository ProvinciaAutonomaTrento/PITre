--------------------------------------------------------
--  DDL for Procedure DPA_SETDIRITTORUOLOPROP
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."DPA_SETDIRITTORUOLOPROP" (
p_idprofile     IN       NUMBER,
p_idcorrGlobRuolo  IN       NUMBER,
p_returnvalue   OUT      NUMBER
)
IS
BEGIN
DECLARE
rtn      NUMBER;
gruppo   NUMBER;
BEGIN
SELECT id_gruppo
INTO gruppo
FROM dpa_corr_globali
WHERE system_id = p_idcorrGlobRuolo;

SELECT COUNT (*)
INTO rtn
FROM security s
WHERE s.personorgroup = gruppo AND s.thing = p_idprofile ;

IF (rtn = 0)
THEN
INSERT INTO security s
(s.accessrights, s.cha_tipo_diritto, s.personorgroup,
s.thing
)
VALUES (63, 'A', gruppo,
p_idprofile
);
ELSE
IF (rtn > 0)
THEN
UPDATE security s
SET s.accessrights = 63,
s.cha_tipo_diritto = 'A'
WHERE s.thing = p_idprofile AND s.personorgroup = gruppo and s.accessrights != 255;
END IF;
END IF;
EXCEPTION
WHEN OTHERS
THEN
p_returnvalue := -1;
END;

p_returnvalue := 1;

END dpa_setdirittoruoloprop; 

/
