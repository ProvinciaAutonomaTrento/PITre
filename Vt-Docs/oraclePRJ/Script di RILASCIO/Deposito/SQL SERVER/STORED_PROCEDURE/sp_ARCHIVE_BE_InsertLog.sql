USE [PCM_DEPOSITO_FINGER]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Giovanni Olivari
-- Create date: 29/04/2013
-- Description:	Inserisce una riga nel log
-- =============================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_InsertLog 
	  @Action VARCHAR(2000)
	, @ActionType VARCHAR(10)
	, @ActionObject VARCHAR(50)
	, @ObjectType VARCHAR(50) = NULL
	, @ObjectID INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO ARCHIVE_Log([Timestamp], [Action], [ActionType], [ActionObject], [ObjectType], [ObjectID])
	VALUES (GETDATE(), @Action, @ActionType, @ActionObject, @ObjectType, @ObjectID)
END
GO
