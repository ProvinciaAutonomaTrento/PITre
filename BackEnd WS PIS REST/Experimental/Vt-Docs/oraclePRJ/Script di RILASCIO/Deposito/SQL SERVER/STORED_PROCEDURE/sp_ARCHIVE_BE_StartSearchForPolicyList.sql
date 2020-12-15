USE PCM_DEPOSITO_FINGER
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================================
-- Author:		Giovanni Olivari
-- Create date: 20/05/2013
-- Description:	Esegue la stored procedure sp_ARCHIVE_BE_StartSearchForPolicy 
--              per ogni ID presente nella lista passata come parametro
-- ==========================================================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_StartSearchForPolicyList
	@policyList VARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @sql_string nvarchar(MAX)

	-- Create temp table
	--
	IF OBJECT_ID('tempdb..#policyListTable') IS NOT NULL DROP TABLE #policyListTable
	CREATE TABLE #policyListTable
	(
		ID int
	)

	SET @sql_string = CAST(N'
		INSERT INTO #policyListTable (ID)
		SELECT SYSTEM_ID FROM ARCHIVE_TransferPolicy
		WHERE SYSTEM_ID IN (' AS NVARCHAR(MAX)) + CAST(@policyList AS NVARCHAR(MAX)) + CAST(N')' AS NVARCHAR(MAX))
		
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;



	-- Chiude e dealloca eventuali cursori rimasti aperti
	--
	execute sp_ARCHIVE_BE_CleanUpCursor 'policyList_cursor'


	
	DECLARE @PolicyID int
	DECLARE policyList_cursor CURSOR
	FOR SELECT ID FROM #policyListTable
	     
	OPEN policyList_cursor
	FETCH policyList_cursor INTO @PolicyID 
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
	 
		print 'PolicyID: ' + CAST(@PolicyID AS NVARCHAR(MAX))
		execute sp_ARCHIVE_BE_StartSearchForPolicy @PolicyID
	   
	FETCH policyList_cursor INTO @PolicyID     
	END
	  
	CLOSE policyList_cursor
	DEALLOCATE policyList_cursor

END
GO
