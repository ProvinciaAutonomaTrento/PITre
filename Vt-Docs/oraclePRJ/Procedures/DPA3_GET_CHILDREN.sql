--------------------------------------------------------
--  DDL for Procedure DPA3_GET_CHILDREN
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."DPA3_GET_CHILDREN" (
p_id_amm in varchar,
p_cha_tipo_ie in varchar,
p_var_cod_rubrica in varchar,
p_res_cursor in out pkgrubrica.r_cursor,
p_corr_types in int default 7)
-- p_corr_types ? una bitmask che indica quali tipi di nodi figli restituire
-- (default: tutti)
-- 1 = UO, 2 = Ruoli, 4 = Utenti
as

begin
declare
v_tipo varchar(2);
v_system_id int;
v_id_gruppo int;

begin

select
cha_tipo_urp, system_id, id_gruppo into v_tipo, v_system_id, v_id_gruppo
from
dpa_corr_globali
where
id_amm = p_id_amm and
cha_tipo_ie = p_cha_tipo_ie and
var_cod_rubrica = p_var_cod_rubrica and
dta_fine is null;

if (v_tipo = 'U') then
open p_res_cursor for
SELECT
var_cod_rubrica, var_desc_corr, (case when CHA_TIPO_IE='I' then 1 else 0 end) AS interno, cha_tipo_urp, system_id
FROM
dpa_corr_globali
where
id_parent = v_system_id and
dta_fine is null and
(bitand(p_corr_types, 1) > 0)
union
SELECT
var_cod_rubrica, var_desc_corr, (case when CHA_TIPO_IE='I' then 1 else 0 end) AS interno, cha_tipo_urp, system_id
FROM
dpa_corr_globali
where
cha_tipo_urp='R' and id_uo = v_system_id and
dta_fine is null and
(bitand(p_corr_types, 2) > 0);

else
open p_res_cursor for
SELECT DISTINCT
var_cod_rubrica, var_desc_corr, (case when CHA_TIPO_IE='I' then 1 else 0 end) AS interno, cha_tipo_urp, system_id
FROM
dpa_corr_globali
where
id_people in (
select people_system_id from peoplegroups where groups_system_id = v_id_gruppo and dta_fine is null)
and dta_fine is null and CHA_TIPO_URP != 'L' and
(bitand(p_corr_types, 4) > 0);
end if;


end;
end;

/
