SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE PROCEDURE [@db_user].[dpa3_get_children]
@id_amm varchar(64),
@cha_tipo_ie varchar(2),
@var_cod_rubrica varchar(64),
@corr_types int = 7
as

declare @tipo varchar(2)
declare @system_id int
declare @id_gruppo int

select
@tipo = cha_tipo_urp, @system_id = system_id, @id_gruppo = id_gruppo
from
dpa_corr_globali
where
id_amm = @id_amm and
cha_tipo_ie=@cha_tipo_ie and
var_cod_rubrica = @var_cod_rubrica

if (@tipo = 'U')
begin
SELECT
var_cod_rubrica, var_desc_corr, (case when CHA_TIPO_IE='I' then 1 else 0 end) AS interno, cha_tipo_urp
FROM
dpa_corr_globali
where
id_parent = @system_id and
dta_fine is null and
((@corr_types & 1) > 0)
union
SELECT
var_cod_rubrica, var_desc_corr, (case when CHA_TIPO_IE='I' then 1 else 0 end) AS interno, cha_tipo_urp
FROM
dpa_corr_globali
where
cha_tipo_urp='R' and id_uo = @system_id and
dta_fine is null and
((@corr_types & 2) > 0)
end
else
begin
SELECT DISTINCT
var_cod_rubrica, var_desc_corr, (case when CHA_TIPO_IE='I' then 1 else 0 end) AS interno, cha_tipo_urp
FROM
dpa_corr_globali
where
id_people in (
select people_system_id from peoplegroups where groups_system_id = @id_gruppo and dta_fine is null)
and dta_fine is null and CHA_TIPO_URP != 'L' and
((@corr_types & 4) > 0)
end

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO