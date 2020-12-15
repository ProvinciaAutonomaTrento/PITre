--------------------------------------------------------
--  DDL for Function POPDPAASSRUOLOOGGCUSTOMFASC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."POPDPAASSRUOLOOGGCUSTOMFASC" 
RETURN INT IS retValue INT;

cnt INT := 0;
systemId INT := 0;
recordCount INT := 0;
id_oggetto INT := 0;
id_ruolo INT := 0;

CURSOR cur IS
select  distinct B.SYSTEM_ID, D.ID_RUOLO into id_oggetto, id_ruolo from
DPA_ASS_TEMPLATES_FASC A, DPA_OGGETTI_CUSTOM_FASC B, DPA_TIPO_OGGETTO_FASC C, DPA_VIS_TIPO_FASC D
where
A.ID_OGGETTO = B.SYSTEM_ID
AND
B.ID_TIPO_OGGETTO = C.SYSTEM_ID
AND
A.ID_TEMPLATE = D.ID_TIPO_FASC
AND
D.DIRITTI > 0
AND
A.ID_PROJECT is null
AND
upper(C.TIPO) = 'CONTATORE'
AND
B.CONTA_DOPO = 1
ORDER BY B.SYSTEM_ID ASC;

BEGIN

OPEN cur;
LOOP
FETCH cur INTO id_oggetto,id_ruolo;
EXIT WHEN cur%NOTFOUND;

begin
insert into DPA_ASS_RUOLO_OGG_CUSTOM (SYSTEM_ID, ID_OGGETTO_CUSTOM, ID_RUOLO, INSERIMENTO)
values(seq.nextval, id_oggetto, id_ruolo, 1);
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

END popDpaAssRuoloOggCustomFasc;

/
