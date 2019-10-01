--------------------------------------------------------
--  DDL for Procedure INS_OCC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."INS_OCC" 
(p_ID_REG in integer,
p_IDAMM in integer,
p_Prefix_cod_rub in VARCHAR,
p_DESC_CORR in VARCHAR,
p_CHA_DETTAGLI in VARCHAR,
p_RESULT OUT integer)
as
BEGIN
declare sysid integer;

BEGIN
-- verifica preesistenza dell'occ corrente
-- per sviluppi futuri .....
/*

select system_id into p_RESULT
from ( select system_id  from DPA_CORR_GLOBALI
where UPPER (var_desc_corr) = UPPER (p_DESC_CORR) and  CHA_TIPO_CORR = 'O' AND ID_AMM=P_IDAMM)
where rownum =1;


EXCEPTION WHEN NO_DATA_FOUND THEN
*/
--inserisco il nuovo occ_
begin
select seq.nextval into sysid from dual;
if(p_id_reg=0) then

INSERT INTO
DPA_CORR_GLOBALI (system_id,ID_REGISTRO,ID_AMM,VAR_COD_RUBRICA,VAR_DESC_CORR,ID_OLD,DTA_INIZIO,ID_PARENT,
VAR_CODICE,CHA_TIPO_CORR,CHA_DETTAGLI)
VALUES (sysid,null,p_idamm,p_Prefix_cod_rub||to_char(sysid),p_desc_corr,0,sysdate,0,p_Prefix_cod_rub||to_char(sysid),'O',0)
returning system_id into p_RESULT;
else
INSERT INTO
DPA_CORR_GLOBALI (system_id,id_registro,ID_AMM,VAR_COD_RUBRICA,VAR_DESC_CORR,ID_OLD,DTA_INIZIO,ID_PARENT,
VAR_CODICE,CHA_TIPO_CORR,CHA_DETTAGLI)
VALUES (sysid,p_ID_REG,p_idamm,p_Prefix_cod_rub||to_char(sysid),p_desc_corr,0,sysdate,0,p_Prefix_cod_rub||to_char(sysid),'O',0)
returning system_id into p_RESULT;
end if;

END;
END;

end; 

/
