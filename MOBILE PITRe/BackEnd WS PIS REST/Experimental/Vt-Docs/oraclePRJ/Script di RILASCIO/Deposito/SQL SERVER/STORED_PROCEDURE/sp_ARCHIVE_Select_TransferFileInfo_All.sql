SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--///////////////////////////////////////////////////////	
--///	Stefano Limiti								/////	
--///	12/09/2013									/////
--///////////////////////////////////////////////////////

CREATE PROC   [DOCSADM].[sp_ARCHIVE_Select_TransferFileInfo_All] 
AS
BEGIN

	SELECT	[ARCHIVE_TempTransferFile].[System_ID], 
			[ARCHIVE_TempTransferFile].[Transfer_ID],
			[ARCHIVE_TempTransferFile].[DocNumber] ,
			[ARCHIVE_TempTransferFile].[Version_ID],
			[ARCHIVE_TempTransferFile].[OriginalPath],			
			[ARCHIVE_TempTransferFile].[OriginalHash],
			[ARCHIVE_TempTransferFile].[Processed],
			[ARCHIVE_TempTransferFile].[ProcessResult],
			[ARCHIVE_TempTransferFile].[ProcessError]
			
	FROM [DOCSADM].[ARCHIVE_TempTransferFile]

END