

/****** Object:  StoredProcedure [DOCSADM].[SP_MODIFY_UO_FATTURAZIONE]    Script Date: 04/19/2013 13:57:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[SP_MODIFY_UO_FATTURAZIONE]') AND type in (N'P', N'PC'))
DROP PROCEDURE [DOCSADM].[SP_MODIFY_UO_FATTURAZIONE]
GO




-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 12/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- =============================================

CREATE PROCEDURE [DOCSADM].[SP_MODIFY_UO_FATTURAZIONE]
(
	@oldCodiceUO	VARCHAR(256),
	@newCodiceUO	VARCHAR(256),
	@idAmm			INT,
	@codiceAoo		VARCHAR(256),
	@returnValue	INT OUTPUT
)
AS
BEGIN

	DECLARE @codiceAmmIpa			VARCHAR(128)
	DECLARE @codiceAooIpa			VARCHAR(128)
	DECLARE @codiceUac				VARCHAR(128)
	DECLARE @codiceClassificazione	VARCHAR(128)
	DECLARE @isFatturazione			VARCHAR(128)
	
	-- selezione il codice ipa amministrazione tramite l'id
	SELECT @codiceAmmIpa = VAR_CODICE_AMM_IPA 
	FROM DOCSADM.DPA_AMMINISTRA
	WHERE SYSTEM_ID = @idAmm
	
	--selezione del codice ipa aoo tramite il codice AOO
	SELECT @codiceAooIpa = VAR_CODICE_AOO_IPA
	FROM DOCSADM.DPA_EL_REGISTRI
	WHERE VAR_CODICE = @codiceAoo 
		AND CHA_RF = 0
		
	
	SELECT @isFatturazione = VAR_DESC_ATTO
    FROM DOCSADM.DPA_TIPO_ATTO
	WHERE ID_AMM= @idAmm 
		AND VAR_DESC_ATTO = 'Fattura elettronica';
		
	
	-- caso di inserimento della UO in PITRE: mi accorgo che si tratta di una nuova UO perchè oldCodiceUO è null
	IF (@oldCodiceUO IS NULL)
    BEGIN
		SELECT @codiceUac = CODICE_UAC
			,@codiceClassificazione = CODICE_CLASSIFICAZIONE
        FROM DOCSADM.DPA_EL_REGISTRI
        WHERE ID_AMM= @idAmm 
			AND VAR_CODICE = @codiceAoo
    END

    IF(@isFatturazione IS NOT NULL)
    BEGIN
        INSERT INTO DOCSADM.DPA_DATI_FATTURAZIONE 
        (
			CODICE_AMM
			, CODICE_AOO
			, CODICE_UO
			, CODICE_UAC
			, CODICE_CLASSIFICAZIONE
			, VAR_UTENTE_PROPRIETARIO
			, VAR_TIPOLOGIA_DOCUMENTO
			, VAR_RAGIONE_TRASMISSIONE
		)
        VALUES
        (
			@codiceAmmIpa
			, @codiceAooIpa
			, @newCodiceUO
			, @codiceUac
			, @codiceClassificazione
			, 'PROVA_TIBCO'
			, 'Fattura elettronica'
			, 'Ricevimento fattura'
		)
		
        SET @returnValue = @@ROWCOUNT
    END
    ELSE				
    BEGIN				
		UPDATE DOCSADM.DPA_DATI_FATTURAZIONE					-- se outValue è 0, allora la UO non è presente nella tabella TIBCO; 
		SET CODICE_UO = @newCodiceUO							-- in tal caso si avvia il job che si occupa di ritentare l'aggiornamento nella tabella
		WHERE UPPER(CODICE_AMM) = UPPER(@codiceAmmIpa)			-- caso di modifica del codice UO in PITRE
			AND UPPER(CODICE_AOO) = UPPER(@codiceAooIpa) 
			AND UPPER(CODICE_UO) = UPPER(@oldCodiceUO)
			
		SET @returnValue = @@ROWCOUNT
	END
END 




  
  


	


GO

