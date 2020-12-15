
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

create  PROCEDURE [@db_user].[setsecurityRuoloReg]
@idCorrGlobali int,
@idProfile int,
@diritto int,
@Idreg int

AS
DECLARE @esito int
DECLARE @idGruppo int

BEGIN
SET @esito = -1
SET @idGruppo = (select id_Gruppo from dpa_corr_globali where system_id = @idCorrGlobali)

IF (@idGruppo IS NOT NULL)
BEGIN
SET @esito = (select max(accessrights) from security where thing = @idProfile and personorgroup = @idGruppo)
IF (@esito < @diritto )
BEGIN
update security set accessrights = @diritto where thing = @idProfile and personorgroup = @idGruppo
END
ELSE
BEGIN
if (@esito is null)
BEGIN
insert into security values(@idProfile,@idGruppo,@diritto,null,'A')
SET  @esito = @diritto
END
END
END
END


insert into security ( THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO )  
select SYSTEM_ID,@idGruppo,@diritto,null,'A' from PROFILE p where ID_REGISTRO=@Idreg
and num_proto is not null
and not exists (select 'x' from SECURITY s1 where s1.THING=p.system_id and
s1.PERSONORGROUP=@idGruppo and s1.ACCESSRIGHTS=@diritto )

RETURN @esito

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO