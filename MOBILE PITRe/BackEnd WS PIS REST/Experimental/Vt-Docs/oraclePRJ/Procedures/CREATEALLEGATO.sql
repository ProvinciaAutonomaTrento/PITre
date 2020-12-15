--------------------------------------------------------
--  DDL for Procedure CREATEALLEGATO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."CREATEALLEGATO" (
p_iddocumentoprincipale         INT,
p_idpeople                      INT,
p_comments                      NVARCHAR2,
p_numeropagine                  INT,
p_idpeopledelegato              INT,
p_idprofile               OUT   INT,
p_versionid               OUT   INT
)
IS
returnvalue   NUMBER;
/******************************************************************************

NAME:       createAllegato

PURPOSE:    Creazione di un nuovo documento di tipo allegato

******************************************************************************/
iddoctype     INT    := 0;
BEGIN
returnvalue := 0;

-- Reperimento tipologia atto del documento principale
SELECT documenttype
INTO iddoctype
FROM PROFILE
WHERE system_id = p_iddocumentoprincipale;

-- Reperimento identity
SELECT seq.NEXTVAL
INTO p_idprofile
FROM DUAL;

INSERT INTO PROFILE
(system_id, docnumber, typist, author, cha_tipo_proto,
cha_da_proto, documenttype, creation_date, creation_time,
id_documento_principale, id_people_delegato,var_chiave_proto,var_prof_oggetto
)
VALUES (p_idprofile, p_idprofile, p_idpeople, p_idpeople, 'G',
'0', iddoctype, SYSDATE, SYSDATE,
p_iddocumentoprincipale, p_idpeopledelegato,to_char(p_idprofile),p_comments
);

returnvalue := SQL%ROWCOUNT;

IF (returnvalue > 0)
THEN
-- Reperimento identity
SELECT seq.NEXTVAL
INTO p_versionid
FROM DUAL;

-- Inserimento record in tabella VERSIONS
INSERT INTO VERSIONS
(version_id, docnumber, VERSION, subversion, version_label,
author, typist, comments, num_pag_allegati,
dta_creazione, cha_da_inviare, id_people_delegato
)
VALUES (p_versionid, p_idprofile, 1, '!', '1',
p_idpeople, p_idpeople, p_comments, p_numeropagine,
SYSDATE, '1', p_idpeopledelegato
);

-- Inserimento record in tabella COMPONENTS
INSERT INTO components
(version_id, docnumber, file_size
)
VALUES (p_versionid, p_idprofile, 0
);
END IF;
END createallegato; 

/
