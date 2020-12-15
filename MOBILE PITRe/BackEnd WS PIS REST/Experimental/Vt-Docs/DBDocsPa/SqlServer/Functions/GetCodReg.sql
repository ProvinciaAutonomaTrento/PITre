SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

 
CREATE function @db_user.GetCodReg(@idREG int)
returns varchar(16)
as
begin
declare @risultato varchar(16)

SELECT @risultato=VAR_CODICE FROM DPA_EL_REGISTRI WHERE SYSTEM_ID = @idREG

return @risultato
end

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO