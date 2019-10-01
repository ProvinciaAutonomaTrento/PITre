--------------------------------------------------------
--  DDL for Procedure SP_INS_DOCUMENTYPE_IN_PROFILE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INS_DOCUMENTYPE_IN_PROFILE" 
AS
BEGIN
DECLARE
profileId NUMBER;
idMezzoSped number;


CURSOR curr_profile
IS
SELECT SYSTEM_ID
FROM PROFILE WHERE cha_invio_conferma='1';

BEGIN

select system_id INTO idMezzoSped from documenttypes where type_id='INTEROPERABILITA';

OPEN curr_profile;

LOOP

FETCH curr_profile
INTO profileId;

EXIT WHEN curr_profile%NOTFOUND;
BEGIN

UPDATE profile SET DOCUMENTTYPE=idMezzoSped WHERE system_id=profileId;

END;

END LOOP;

CLOSE curr_profile;
END;
END;

/
