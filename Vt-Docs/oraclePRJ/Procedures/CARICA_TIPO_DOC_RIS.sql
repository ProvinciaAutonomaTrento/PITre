--------------------------------------------------------
--  DDL for Procedure CARICA_TIPO_DOC_RIS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."CARICA_TIPO_DOC_RIS" IS

CURSOR currAmm IS
select system_id
from dpa_amministra ORDER BY system_id DESC;


BEGIN
FOR currentAmm in currAmm
LOOP
BEGIN
DECLARE cnt INT;
BEGIN
SELECT COUNT(*) INTO cnt FROM DPA_TIPO_FUNZIONE WHERE VAR_COD_TIPO='DOC_RIS';
IF (cnt != 0) THEN
INSERT INTO DPA_TIPO_FUNZIONE ( SYSTEM_ID, VAR_COD_TIPO, VAR_DESC_TIPO_FUN, CHA_VIS, ID_AMM)
VALUES (seq.nextval, 'DOC_RIS', 'DOCUMENTAZIONE RISERVATA', '1', currentAmm.system_id);
END IF;
END;
END;
END LOOP;
commit;

END; 

/
