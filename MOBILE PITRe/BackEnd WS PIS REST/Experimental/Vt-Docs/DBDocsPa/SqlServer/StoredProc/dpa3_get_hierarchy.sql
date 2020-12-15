SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



create procedure  [@db_user].[dpa3_get_hierarchy]
@id_amm varchar(64),
@cod varchar(64),
@tipo_ie varchar(2),
@codes varchar(4000) output
as
declare @c_type varchar(2)
declare @p_cod varchar(64)
declare @system_id int
declare @id_parent int
declare @id_uo int
declare @id_utente int

set @codes = ''
select @id_parent = id_parent, @system_id = system_id, @id_uo = id_uo, @id_utente = id_people, @c_type = cha_tipo_urp from dpa_corr_globali where
var_cod_rubrica=@cod and
cha_tipo_ie=@tipo_ie and
id_amm=@id_amm and
dta_fine is null

while (1 > 0)
begin
if @c_type is null
break

if @c_type = 'U'
begin
if (@id_parent is null or @id_parent = 0)
break
select @p_cod = var_cod_rubrica, @system_id = system_id from dpa_corr_globali where system_id = @id_parent and id_amm=@id_amm and dta_fine is null
end
if @c_type = 'R'
begin
if (@id_uo is null or @id_uo = 0)
break
select @p_cod = var_cod_rubrica, @system_id = system_id from dpa_corr_globali where system_id = @id_uo and id_amm=@id_amm and dta_fine is null
end
if @c_type = 'P'
begin
select top 1 @p_cod = var_cod_rubrica from dpa_corr_globali where id_gruppo =
(
case when exists
(select groups_system_id from peoplegroups where people_system_id = @id_utente and cha_preferito = 1)
then
(select groups_system_id from peoplegroups where people_system_id = @id_utente  and cha_preferito = 1)
else
(select max(groups_system_id) from peoplegroups where people_system_id = @id_utente and dta_fine is null)
end
)
and id_amm=@id_amm and dta_fine is null
end
if @p_cod is null
break

select @id_parent = id_parent, @system_id = system_id, @id_uo = id_uo, @c_type = cha_tipo_urp from dpa_corr_globali where var_cod_rubrica=@p_cod and id_amm=@id_amm and dta_fine is null

set @codes = @p_cod + ':' + @codes
end
set @codes = @codes + @cod


GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO