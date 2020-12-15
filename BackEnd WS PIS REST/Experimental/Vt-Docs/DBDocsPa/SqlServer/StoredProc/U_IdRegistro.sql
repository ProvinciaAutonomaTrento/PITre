SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE  PROCEDURE [@db_user].[U_IdRegistro] AS
DECLARE @id_parent varchar(32)
DECLARE @system_id varchar(32)
DECLARE @idRegistro int
DECLARE curr CURSOR FOR
select distinct id_parent from project where (cha_tipo_proj = 'F' and cha_tipo_fascicolo = 'P') order by id_parent
OPEN curr
FETCH NEXT FROM curr INTO @id_parent
WHILE @@FETCH_STATUS = 0
BEGIN
SET @system_id = @id_parent
SET @idRegistro = (select id_registro from project where system_id = @system_id)
UPDATE project set id_registro = @idRegistro where (cha_tipo_proj = 'F' and cha_tipo_fascicolo = 'P') and id_parent =@id_parent
FETCH NEXT FROM curr INTO @id_parent
END
CLOSE curr
DEALLOCATE curr
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO