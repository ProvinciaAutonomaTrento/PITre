CREATE OR REPLACE PROCEDURE @db_user.createAllegato
(
p_idDocumentoPrincipale int,
p_idPeople int,
p_comments nvarchar2,
p_numeroPagine int,
p_idProfile out int,
p_versionId out int
)
IS returnvalue NUMBER;
idDocType INT := 0;

BEGIN
returnvalue := 0;
SELECT DOCUMENTTYPE INTO idDocType
FROM PROFILE WHERE SYSTEM_ID = p_idDocumentoPrincipale;

SELECT SEQ.nextval INTO p_idProfile FROM dual;

INSERT INTO Profile
(
SYSTEM_ID,
DOCNUMBER,
TYPIST,
AUTHOR,
CHA_TIPO_PROTO,
CHA_DA_PROTO,
DOCUMENTTYPE,
CREATION_DATE,
CREATION_TIME,
ID_DOCUMENTO_PRINCIPALE
)
VALUES
(
p_idProfile,
p_idProfile,
p_idpeople,
p_idpeople,
'G',
'0',
idDocType,
SYSDATE,
SYSDATE,
p_idDocumentoPrincipale
);

returnvalue := SQL%ROWCOUNT;

IF (returnvalue > 0) THEN

SELECT SEQ.nextval INTO p_versionId FROM dual;

INSERT INTO VERSIONS
(
VERSION_ID,
DOCNUMBER,
VERSION,
SUBVERSION,
VERSION_LABEL,
AUTHOR,
TYPIST,
COMMENTS,
NUM_PAG_ALLEGATI,
DTA_CREAZIONE,
CHA_DA_INVIARE
)
VALUES
(
p_versionId,
p_idProfile,
1,
'!',
'1',
p_idPeople,
p_idPeople,
p_comments,
p_numeroPagine,
SYSDATE,
'1'
);

INSERT INTO COMPONENTS
(
VERSION_ID,
DOCNUMBER,
FILE_SIZE
)
VALUES
(
p_versionId,
p_idProfile,
0
);

END IF;

END createAllegato;
/