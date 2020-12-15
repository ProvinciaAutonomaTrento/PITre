create procedure [DOCSADM].[sp_ARCHIVE_CheckMoveFile]

as
begin

	select [ARCHIVE_Configuration].[Value] 
	from [DOCSADM].[ARCHIVE_Configuration] 
	where [ARCHIVE_Configuration].[Key] = 'SPOSTA_FILE'

end