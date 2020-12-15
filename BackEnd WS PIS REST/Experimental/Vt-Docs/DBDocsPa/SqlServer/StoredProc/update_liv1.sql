

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE         PROCEDURE @db_user.update_liv1 @id_amm int as
DECLARE @system_id varchar(32)
DECLARE @id_parent varchar(32)
DECLARE @num_livello INT
DECLARE @id_parent_2 varchar(32)
DECLARE @cont2 int
DECLARE @var_cod_liv1 varchar(32)


	UPDATE PROJECT SET VAR_COD_LIV1 = '0000' 
	WHERE ID_AMM=@id_amm 
	AND cha_tipo_proj= 'T' 
	AND num_livello = 0


SET @num_livello = 1

WHILE @num_livello < 8 

BEGIN

	DECLARE curr CURSOR local FOR
	SELECT system_id, id_parent 
	from project 
	where num_livello = @num_livello 
	and id_amm=@id_amm  and cha_tipo_proj= 'T'  
	ORDER BY ID_PARENT, system_id	OPEN curr
	FETCH NEXT FROM curr INTO @system_id, @id_parent
	SET @cont2 = 0
	SET @id_parent_2 = @id_parent

	WHILE @@FETCH_STATUS = 0

		BEGIN

			IF (@id_parent_2 <> @id_parent)
				-- per azzerare il contatore e far ripartire la numerazione dei var_cod_liv
				-- quando il record corrente presenta un' id_parent diverso da quello del record precedente
				BEGIN
					SET @cont2 = 0
					SET @id_parent_2 = @id_parent
				END
	
			SET @cont2 = @cont2 + 1
		
		    	IF @num_livello = 1
		
				BEGIN
					IF @cont2 <= 9
						update @db_user.project set var_cod_liv1 = '000' + CONVERT(varchar(32), @cont2) where num_livello = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id=@system_id
					IF @cont2 > 9 AND @cont2 <= 99
						update @db_user.project set var_cod_liv1 = '00' + CONVERT(varchar(32), @cont2) where num_livello = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id=@system_id
					IF @cont2 > 99 and @cont2 <= 999
						update @db_user.project set var_cod_liv1 = '0' + CONVERT(varchar(32), @cont2) where num_livello = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id=@system_id
					IF @cont2 > 999
						update @db_user.project set var_cod_liv1 = CONVERT(varchar(32), @cont2) where num_livello = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id=@system_id
				END
		    	ELSE
				
				BEGIN	
					-- viene  ricavato il var cod_liv_1 del padre
					SET @var_cod_liv1 = (select var_cod_liv1 from @db_user.project where system_id = @id_parent and id_amm=@id_amm and cha_tipo_proj= 'T' and num_livello = @num_livello - 1)
			
					IF @cont2 <= 9 -- questo controllo serve per concatenare due o tre zeri a seconda del valore del contatore
						update @db_user.project set var_cod_liv1 = @var_cod_liv1 + '000' + CONVERT(varchar(32), @cont2) where num_livello = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id = @system_id
					IF @cont2 > 9 AND @cont2 <= 99
						update @db_user.project set var_cod_liv1 = @var_cod_liv1 + '00' + CONVERT(varchar(32), @cont2) where num_livello  = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id = @system_id
					IF @cont2 > 99 and @cont2 <= 999
						update @db_user.project set var_cod_liv1 = @var_cod_liv1 + '0' + CONVERT(varchar(32), @cont2) where num_livello  = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id = @system_id
					IF @cont2 > 999
						update @db_user.project set var_cod_liv1 = @var_cod_liv1  + CONVERT(varchar(32), @cont2) where num_livello  = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id = @system_id
		
				END
			
		    	FETCH NEXT FROM curr INTO @system_id,@id_parent
		
		END

	--rilascio le risorse allocate
	CLOSE curr
	DEALLOCATE curr

	SET @num_livello = @num_livello + 1

END
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO