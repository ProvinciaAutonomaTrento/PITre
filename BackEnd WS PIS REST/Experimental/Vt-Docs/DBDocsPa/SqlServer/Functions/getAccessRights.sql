

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE function @db_user.getAccessRights(@id_ruolo INT,@id_people INT,@systemid INT)
returns INT
as
begin
declare @risultato INT

SET @risultato = -1

declare @idDocumentoPrincipale int
set @idDocumentoPrincipale = (select id_documento_principale from profile where system_id = @systemid)

if (not @idDocumentoPrincipale is null)
set @systemid = @idDocumentoPrincipale

SELECT @risultato =  max(accessrights)
from security
where thing = @systemid and
PERSONORGROUP in (@id_ruolo,@id_people)

return @risultato
end
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO