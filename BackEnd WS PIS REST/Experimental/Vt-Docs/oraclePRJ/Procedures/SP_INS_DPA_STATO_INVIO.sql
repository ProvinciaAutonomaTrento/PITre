--------------------------------------------------------
--  DDL for Procedure SP_INS_DPA_STATO_INVIO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INS_DPA_STATO_INVIO" 
AS
BEGIN
DECLARE
profileId NUMBER;
idMezzoSped number;


CURSOR curr_dpa_stato_invio
IS
SELECT ID_PROFILE
FROM dpa_stato_invio;

BEGIN

select system_id INTO idMezzoSped from documenttypes where type_id='INTEROPERABILITA';

OPEN curr_dpa_stato_invio;

LOOP

FETCH curr_dpa_stato_invio
INTO profileId;

EXIT WHEN curr_dpa_stato_invio%NOTFOUND;
BEGIN

UPDATE dpa_stato_invio SET ID_DOCUMENTTYPE=idMezzoSped WHERE ID_PROFILE=profileId;

END;

END LOOP;

CLOSE curr_dpa_stato_invio;
END;
END;

/
