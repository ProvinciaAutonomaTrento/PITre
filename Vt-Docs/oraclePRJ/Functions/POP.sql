--------------------------------------------------------
--  DDL for Function POP
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."POP" 
RETURN INT IS retValue INT;

cnt INT := 0;
amm INT := 0;
codice VARCHAR2(128);
descrizione VARCHAR2(256);
aoo INT := 0;
systemId INT := 0;
recordCount INT := 0;

CURSOR cur IS
select distinct SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESC_REGISTRO, ID_AOO_COLLEGATA
from DPA_EL_REGISTRI where CHA_RF = 1;


BEGIN

OPEN cur;
LOOP
FETCH cur INTO systemId, amm, codice, descrizione, aoo;
EXIT WHEN cur%NOTFOUND;

begin
cnt := 0;
select count(*) into cnt from dpa_corr_globali where id_rf = systemId;
if (cnt = 0)
then
insert into dpa_corr_globali(system_id,ID_AMM,ID_REGISTRO,VAR_COD_RUBRICA,VAR_DESC_CORR,DTA_INIZIO,CHA_TIPO_URP,ID_RF)
values(seq.nextval,amm,aoo,codice,descrizione,sysdate,'F',systemId);
end if;
end;


recordCount := cnt + recordCount;


END LOOP;

IF (recordCount > 0) THEN
retValue := 1;
ELSE
retValue := 0;
END IF;

RETURN retValue;

END pop;

/
