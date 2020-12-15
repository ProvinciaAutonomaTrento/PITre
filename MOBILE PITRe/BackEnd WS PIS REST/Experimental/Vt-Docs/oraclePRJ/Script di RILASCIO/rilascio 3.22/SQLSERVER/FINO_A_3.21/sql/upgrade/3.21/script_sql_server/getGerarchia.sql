
/****** Object:  UserDefinedFunction [DOCSADM].[getGerarchia]    Script Date: 02/21/2013 10:35:25 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[getGerarchia]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DOCSADM].[getGerarchia]
GO


CREATE FUNCTION [DOCSADM].[getGerarchia] (@id_amm varchar(64), @codRubrica varchar(64), @tipoCorr varchar(200), @id_ruolo int, @tipo varchar(200))
RETURNS varchar(4000) AS
BEGIN

declare @codes varchar(4000)
declare @c_type varchar(2)
declare @p_cod varchar(64)
declare @system_id int
declare @id_parent int
declare @id_uo int
declare @id_utente int
declare @mydebug varchar(2)

--select 
--@tipoCorr = CHA_TIPO_URP from dpa_corr_globali
--where
--UPPER(var_cod_rubrica) = UPPER(@codRubrica) and cha_tipo_ie = @tipo  and
--id_amm = @id_amm  and dta_fine is null 

set @codes = ''
select @id_parent = id_parent, @system_id = system_id, @id_uo = id_uo, @id_utente = id_people, @c_type = cha_tipo_urp 
from dpa_corr_globali 
where
var_cod_rubrica=@codRubrica and
cha_tipo_ie=@tipo  and
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
		select top 1 @p_cod = var_cod_rubrica 
		--,    @mydebug = '__' 
		from dpa_corr_globali 
		where id_gruppo = @id_Ruolo 
		and id_amm=@id_amm and dta_fine is null
		--set 
	end

	if @p_cod is null
	break
 
	select @id_parent = id_parent, @system_id = system_id, @id_uo = id_uo, @c_type = cha_tipo_urp 
	from dpa_corr_globali where var_cod_rubrica=@p_cod and id_amm=@id_amm and dta_fine is null
	 
	set @codes = @p_cod + ':'+ @codes 
end

set @codes = @codes + @codRubrica

RETURN  @codes
END

GO


