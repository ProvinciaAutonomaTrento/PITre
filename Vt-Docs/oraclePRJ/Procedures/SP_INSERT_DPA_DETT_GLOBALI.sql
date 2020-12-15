--------------------------------------------------------
--  DDL for Procedure SP_INSERT_DPA_DETT_GLOBALI
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INSERT_DPA_DETT_GLOBALI" 

IS

sysCorrente NUMBER;
IdentityDettGlobali NUMBER;

CURSOR sysCursor IS
select system_id from DPA_CORR_GLOBALI
WHERE cha_tipo_ie = 'E' AND CHA_TIPO_URP IN ('U', 'P') AND DTA_FINE IS NULL
and system_id
not in (select id_corr_globali from dpa_dett_globali);
BEGIN

OPEN sysCursor;
LOOP
FETCH sysCursor INTO sysCorrente;
EXIT WHEN sysCursor%NOTFOUND;

BEGIN
SELECT seq.NEXTVAL INTO IdentityDettGlobali FROM dual;

INSERT INTO DPA_DETT_GLOBALI
(
SYSTEM_ID,
ID_CORR_GLOBALI
)
VALUES
(
IdentityDettGlobali,
sysCorrente
);
END;

END LOOP;
CLOSE sysCursor;

COMMIT;
END; 

/
