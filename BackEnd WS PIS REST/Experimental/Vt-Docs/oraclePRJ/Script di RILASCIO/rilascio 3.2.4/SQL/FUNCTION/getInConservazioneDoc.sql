USE [NTTDOCS]
GO

/****** Object:  UserDefinedFunction [DOCSADM].[getInConservazione]    Script Date: 01/17/2017 16:29:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [DOCSADM].[getInConservazioneDoc]
(
@IDPROFILE int
)

RETURNS int
AS

BEGIN

DECLARE   @risultato   int

SET @risultato=0

BEGIN


SELECT @risultato = COUNT(B.SYSTEM_ID)
FROM DPA_AREA_CONSERVAZIONE A, DPA_ITEMS_CONSERVAZIONE B
WHERE A.SYSTEM_ID=B.ID_CONSERVAZIONE
AND B.ID_PROFILE = @IDPROFILE
AND (A.CHA_STATO = 'C' OR A.CHA_STATO = 'V')

IF (@risultato > 0)
SET @risultato = 1
ELSE
SET @risultato = 0
END


RETURN @risultato
END

GO

