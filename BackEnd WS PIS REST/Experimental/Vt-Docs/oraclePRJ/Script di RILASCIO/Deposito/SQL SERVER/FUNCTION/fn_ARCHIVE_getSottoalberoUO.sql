USE [PCM_040413]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =======================================================================
-- Author:		Giovanni Olivari
-- Create date: 23/04/2013
-- Description:	Restituisce la UO e il relativo sottoalbero
-- =======================================================================
ALTER FUNCTION DOCSADM.fn_ARCHIVE_getSottoalberoUO
(	
	@idUO int -- Identificativo della UO (campo System_ID)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH GERARCHIA_UO AS
	(
	SELECT CG.SYSTEM_ID, CG.ID_PARENT, CG.VAR_CODICE
	FROM DPA_CORR_GLOBALI CG
	WHERE CG.SYSTEM_ID = @idUO
	UNION ALL
	SELECT CG.SYSTEM_ID, CG.ID_PARENT, CG.VAR_CODICE
	FROM DPA_CORR_GLOBALI CG INNER JOIN GERARCHIA_UO G ON G.SYSTEM_ID = CG.ID_PARENT
	)
	SELECT * FROM GERARCHIA_UO
)
GO
