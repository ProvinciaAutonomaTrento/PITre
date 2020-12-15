USE [PCM_DEPOSITO_1]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Giovanni Olivari
-- Create date: 29/04/2013
-- Description:	Pulizia per il cursore indicato
-- =============================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_CleanUpCursor @cursorName varchar(255) AS
BEGIN

    DECLARE @cursorStatus int
    SET @cursorStatus =  (SELECT cursor_status('global',@cursorName))

    DECLARE @sql varchar(255)
    SET @sql = ''

    IF @cursorStatus > 0
    	SET @sql = 'CLOSE '+@cursorName

    IF @cursorStatus > -3
    	SET @sql = @sql+' DEALLOCATE '+@cursorName

    IF @sql <> ''
    	exec(@sql)

END