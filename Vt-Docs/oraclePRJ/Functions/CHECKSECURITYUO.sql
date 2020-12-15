--------------------------------------------------------
--  DDL for Function CHECKSECURITYUO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CHECKSECURITYUO" 
(
thingParam INT,
idUO INT
)
RETURN INT IS retValue INT;

cnt INT := 0;
gruppoId INT := 0;
recordCount INT := 0;

CURSOR cur IS
select id_gruppo from dpa_corr_globali where id_uo = idUo;

BEGIN

OPEN cur;
LOOP
FETCH cur INTO gruppoId;
EXIT WHEN cur%NOTFOUND;

begin
SELECT checkSecurityProprietario(thingParam,0,gruppoId) INTO cnt
FROM dual;
exception when others then cnt:=0;
end;


recordCount := cnt + recordCount;



END LOOP;

IF (recordCount > 0) THEN
retValue := 1;
ELSE
retValue := 0;
END IF;


RETURN retValue;

END checkSecurityUO; 

/
