USE [PCM_DEPOSITO_1]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =========================================================
-- Author:		Giovanni Olivari
-- Create date: 11/06/2013
-- Description:	Restituisce l'ID ruolo per il consolidamento
-- =========================================================
ALTER FUNCTION [DOCSADM].[fn_ARCHIVE_getIDForRuoloConsolidamento]
(
)
RETURNS INT
AS
BEGIN

	RETURN 1

END
