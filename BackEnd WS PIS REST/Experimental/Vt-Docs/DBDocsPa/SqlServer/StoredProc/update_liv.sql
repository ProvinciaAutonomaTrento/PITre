SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
CREATE PROCEDURE @db_user.update_liv @num_livello int , @id_amm int as
-- dichiarazione variabili
DECLARE @id_parent varchar(32)
DECLARE @system_id varchar(32)
DECLARE @id_parent_2 varchar(32)
DECLARE @var_cod_liv1 varchar(32)
DECLARE @maxliv int
-- fine dichiarazione
SET @maxliv = (SELECT MAX(num_livello) from @db_user.project where id_amm=@id_amm and cha_tipo_proj= 'T')
IF(@num_livello <= @maxliv)
BEGIN
DECLARE curr2 CURSOR FOR
-- la query ritorna i system_id associati ai nodi di titolario relativi al numero di livello in input alla procedure
SELECT id_parent, system_id from @db_user.project where num_livello = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' ORDER BY ID_PARENT, system_id
DECLARE @cont2 int
OPEN curr2
SET @cont2 = 0
FETCH NEXT FROM curr2 INTO @id_parent, @system_id
SET @id_parent_2 = @id_parent
-- finch?i sono record da prelevare
WHILE @@FETCH_STATUS = 0
BEGIN

IF (@id_parent_2 <> @id_parent)
-- per azzerare il contatore e far ripartire la numerazione dei var_cod_liv
-- quando il record corrente presenta un' id_parent diverso da quello del record precedente
BEGIN
SET @cont2 = 0
SET @id_parent_2 = @id_parent
END
-- viene  ricavato il var cod_liv_1 del padre
SET @var_cod_liv1 = (select var_cod_liv1 from @db_user.project where system_id = @id_parent and id_amm=@id_amm and cha_tipo_proj= 'T' and num_livello = @num_livello - 1)
SET @cont2 = @cont2 + 1
IF @cont2 <= 9 -- questo controllo serve per concatenare due o tre zeri a seconda del valore del contatore
update @db_user.project set var_cod_liv1 = @var_cod_liv1 + '000' + CONVERT(varchar(32), @cont2) where num_livello = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id = @system_id
IF @cont2 > 9 AND @cont2 <= 99
update @db_user.project set var_cod_liv1 = @var_cod_liv1 + '00' + CONVERT(varchar(32), @cont2) where num_livello  = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id = @system_id
IF @cont2 > 99 and @cont2 <= 999
update @db_user.project set var_cod_liv1 = @var_cod_liv1 + '0' + CONVERT(varchar(32), @cont2) where num_livello  = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id = @system_id
IF @cont2 > 999
update @db_user.project set var_cod_liv1 = @var_cod_liv1  + CONVERT(varchar(32), @cont2) where num_livello  = @num_livello and id_amm=@id_amm and cha_tipo_proj= 'T' and system_id = @system_id

FETCH NEXT FROM curr2 INTO @id_parent, @system_id
END

CLOSE curr2
DEALLOCATE curr2
END
else
print 'L''amministrazione ' + CONVERT(varchar(32),@id_amm) + ' non presenta il livello di titolario ' + CONVERT(varchar(32), @num_livello)

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO