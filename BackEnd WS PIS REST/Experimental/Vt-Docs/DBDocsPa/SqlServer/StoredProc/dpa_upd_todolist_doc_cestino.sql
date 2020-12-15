SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[dpa_upd_todolist_doc_cestino]
@idProfile int,
@returnvalue int out
AS

declare @id_trasmutente int
declare @idTrasm int
declare @idpeopletx int

BEGIN
set @returnvalue = 0

DECLARE trasm CURSOR LOCAL FOR
select
id_trasmissione,id_people_dest,id_trasm_utente
from dpa_todolist where id_profile=@idProfile;

OPEN trasm
FETCH next from trasm into @idTrasm,@idpeopletx,@id_trasmutente
while(@@fetch_status=0)
BEGIN

UPDATE dpa_trasm_utente
SET  CHA_IN_TODOLIST = '0',
CHA_VISTA  = (CASE WHEN DTA_VISTA IS NULL THEN 1 ELSE 0 END)
WHERE SYSTEM_ID = @id_trasmutente


if @@error <> 0 set @returnvalue = -1

FETCH next from trasm into @idTrasm,@idpeopletx,@id_trasmutente

END    -- GENERALE
-- fine codice ciclo cursore
close trasm
deallocate trasm
set @returnvalue = 1


return @returnvalue
END

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO