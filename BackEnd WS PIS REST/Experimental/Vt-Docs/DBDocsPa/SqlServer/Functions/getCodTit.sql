


SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE function @db_user.getCodTit(@idParent int)
returns varchar(32)
as
begin
declare @risultato varchar(32)
SET @risultato =''

SELECT @risultato = upper(var_codice) from project where system_id = @idParent

return @risultato
end

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO