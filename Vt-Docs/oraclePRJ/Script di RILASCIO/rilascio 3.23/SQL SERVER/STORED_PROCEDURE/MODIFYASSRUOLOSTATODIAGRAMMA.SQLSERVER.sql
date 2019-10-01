-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 22/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- MODIFYASSRUOLOSTATODIAGRAMMA
-- =============================================
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[MODIFYASSRUOLOSTATODIAGRAMMA]') AND type in (N'P', N'PC'))
DROP PROCEDURE [DOCSADM].[MODIFYASSRUOLOSTATODIAGRAMMA]
GO

CREATE PROCEDURE [DOCSADM].[MODIFYASSRUOLOSTATODIAGRAMMA]
(
    @idGruppo		INT,
    @idStato		INT,
    @idDiagramma	INT,
    @chaNotVis		VARCHAR(1),
    @result			INT OUTPUT
)
AS

DECLARE @counterRecord INT

BEGIN
	
	SELECT @counterRecord = COUNT(*)
		FROM DOCSADM.DPA_ASS_RUOLO_STATI_DIAGRAMMA
		WHERE id_diagramma = @idDiagramma
			AND id_stato = @idStato
			AND id_gruppo = @idGruppo
			
  -- E' cambiata la visibilità del ruolo sullo stato da non visibile a visibile, quindi elimino il record associato
  
	IF (@chaNotVis = 0 AND @counterRecord = 1) 
	BEGIN
  
		DELETE
		FROM DOCSADM.DPA_ASS_RUOLO_STATI_DIAGRAMMA
		WHERE id_diagramma = @idDiagramma
			AND id_stato = @idStato
			AND id_gruppo = @idGruppo
		
		SET @result = 0
	END 
  
	-- Esiste già l'associazione tra ruolo e stato del diagramma. In realtà questa situazione non dovrebbe verificarsi.
	-- Inserita per prevenire eventuali errori da codice
  
	IF (@chaNotVis = 1 AND @counterRecord = 1) 
	BEGIN
		UPDATE DPA_ASS_RUOLO_STATI_DIAGRAMMA
		SET cha_not_vis    = '1'
		WHERE id_diagramma = @idDiagramma
			AND id_stato = @idStato
			AND id_gruppo = @idGruppo;
    
		SET @result = 1
	END
  
	-- Non esiste l'associazione ruolo stato del diagramma. Aggiungo il record
  
	IF (@chaNotVis = 1 AND @counterRecord = 0)
	BEGIN
		INSERT INTO DOCSADM.DPA_ASS_RUOLO_STATI_DIAGRAMMA 
		VALUES (@idGruppo,@idDiagramma,@idStato,'1')
    
		SET @result = 2;
	END
END
