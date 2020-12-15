--------------------------------------------------------
--  DDL for Procedure ADDNUOVAVERSIONE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."ADDNUOVAVERSIONE" 
(p_subVersion  VARCHAR2,
p_idPeople NUMBER,
p_docNumber NUMBER,
p_descrizione VARCHAR2,
p_cartaceo INTEGER,
p_versionID OUT NUMBER)

IS

identityVersion NUMBER;
p_version NUMBER;

BEGIN

select VERSION INTO p_version from (
SELECT VERSION
FROM VERSIONS
WHERE DOCNUMBER = p_docNumber
ORDER BY VERSION DESC)
where rownum = 1;

p_version:=p_version+1;

SELECT SEQ.NEXTVAL INTO identityVersion FROM DUAL;

p_versionID := identityVersion ;

BEGIN

INSERT INTO VERSIONS
(
VERSION_ID,
DOCNUMBER,
VERSION,
SUBVERSION,
VERSION_LABEL,
AUTHOR,
TYPIST,
DTA_CREAZIONE,
COMMENTS,
CARTACEO
)
VALUES
(
p_versionID,
p_docNumber,
p_version,
p_subVersion,
p_version,
p_idPeople,
p_idPeople,
sysdate,
p_descrizione,
p_cartaceo
);
EXCEPTION
WHEN OTHERS THEN p_versionID:=0;

END;

BEGIN

INSERT INTO COMPONENTS
(
VERSION_ID,
DOCNUMBER,
FILE_SIZE
)
VALUES
(
p_versionID,
p_docNumber,
0
);
EXCEPTION
WHEN OTHERS THEN p_versionID:=0;


END;

END addNuovaVersione; 

/
