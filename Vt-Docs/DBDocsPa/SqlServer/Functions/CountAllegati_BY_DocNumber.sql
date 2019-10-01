
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


create  function [@db_user].[CountAllegati_BY_DocNumber](@id int)
returns int
as
begin
declare @risultato int

SELECT @risultato=
COUNT(V.VERSION_ID)
FROM PROFILE P INNER JOIN VERSIONS V ON P.DOCNUMBER = V.DOCNUMBER
WHERE (P.DOCNUMBER = @id AND V.VERSION = 0) OR P.ID_DOCUMENTO_PRINCIPALE = @id

return @risultato
end

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO