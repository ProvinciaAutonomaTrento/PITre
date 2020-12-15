




SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


CREATE FUNCTION @db_user.getInADL (@sysID int, @typeID char,@idGruppo INT,@idPeople INT)
RETURNS INT
AS
BEGIN --generale
DECLARE @risultato int
set @risultato = 0

begin --interno

IF (@typeID = 'D')
BEGIN
set @risultato = (SELECT DISTINCT(SYSTEM_ID) FROM DPA_AREA_LAVORO WHERE ID_PROFILE = @sysID
AND ID_PEOPLE =@idPeople and ID_RUOLO_IN_UO = (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = @idGruppo))
IF (@risultato > 0)
BEGIN
set @risultato = 1
END
END
IF (@typeID = 'F')
BEGIN
set @risultato = (SELECT DISTINCT(SYSTEM_ID) FROM DPA_AREA_LAVORO WHERE ID_PROJECT = @sysID
AND ID_PEOPLE =@idPeople and ID_RUOLO_IN_UO = (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = @idGruppo))
IF (@risultato > 0)
BEGIN
set @risultato = 1
END
END

end

RETURN @risultato;
END

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO