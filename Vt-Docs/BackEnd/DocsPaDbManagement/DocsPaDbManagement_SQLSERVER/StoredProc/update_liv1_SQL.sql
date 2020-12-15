-- LA PROCEDURA update_liv1 prende in input un parametro:
-- @id_amm di tipo intero : sytem_id dell'amministazione di interesse

CREATE PROCEDURE update_liv1 @id_amm int as
DECLARE @system_id varchar(32)
DECLARE curr CURSOR FOR
SELECT system_id from docsadm.project where num_livello = 1 and id_amm=@id_amm and cha_tipo_proj= 'T'  ORDER BY system_id
declare @cont int
OPEN curr

SET @cont = 0
FETCH NEXT FROM curr INTO @system_id

WHILE @@FETCH_STATUS = 0
BEGIN
	set @cont = @cont + 1
	IF @cont <= 9
		update docsadm.project set var_cod_liv1 = '000' + CONVERT(varchar(32), @cont) where num_livello = 1 and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id=@system_id
	IF @cont > 9 AND @cont <= 99
		update docsadm.project set var_cod_liv1 = '00' + CONVERT(varchar(32), @cont) where num_livello = 1 and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id=@system_id
	IF @cont > 99 and @cont <= 999
		update docsadm.project set var_cod_liv1 = '0' + CONVERT(varchar(32), @cont) where num_livello = 1 and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id=@system_id
	IF @cont > 999
		update docsadm.project set var_cod_liv1 = CONVERT(varchar(32), @cont) where num_livello = 1 and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id=@system_id
	
	FETCH NEXT FROM curr INTO @system_id
	END

CLOSE curr
DEALLOCATE curr
GO