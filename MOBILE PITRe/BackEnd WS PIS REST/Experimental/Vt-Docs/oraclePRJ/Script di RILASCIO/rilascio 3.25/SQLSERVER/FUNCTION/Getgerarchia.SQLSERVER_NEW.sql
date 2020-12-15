-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 05/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- GETGERARCHIA
-- =============================================


ALTER FUNCTION [DOCSADM].[getGerarchia] 
(
	@id_amm			VARCHAR(64)
	, @codRubrica	VARCHAR(64)
	, @tipoCorr		VARCHAR(200)
	, @id_ruolo		INT
	--, @tipo			VARCHAR(200)
)

RETURNS varchar(4000)
AS
BEGIN

	DECLARE @codes		VARCHAR(4000)
	DECLARE @c_type		VARCHAR(2)
	DECLARE @p_cod		VARCHAR(64)
	DECLARE @system_id	INT
	DECLARE @id_parent	INT
	DECLARE @id_uo		INT
	DECLARE @id_utente	INT
	DECLARE @mydebug	VARCHAR(2)

	SET @codes = ''
	
	SELECT @id_parent = ID_PARENT
		, @system_id = SYSTEM_ID
		, @id_uo = ID_UO
		, @id_utente = ID_PEOPLE
		, @c_type = CHA_TIPO_URP 
	FROM DOCSADM.DPA_CORR_GLOBALI 
	WHERE var_cod_rubrica = @codRubrica 
		AND cha_tipo_ie = @tipoCorr  
		AND id_amm = @id_amm 
		AND dta_fine IS NULL

    WHILE (1 > 0)
    BEGIN
		IF @c_type IS NULL
			BREAK
			
		IF @c_type = 'U'
		BEGIN
			IF(@id_parent IS NULL OR @id_parent = 0)
				BREAK
			
			SELECT @p_cod = VAR_COD_RUBRICA
				,@system_id = SYSTEM_ID 
			FROM DOCSADM.DPA_CORR_GLOBALI 
			WHERE SYSTEM_ID = @id_parent 
				AND ID_AMM = @id_amm 
				AND DTA_FINE IS NULL
				AND CHA_TIPO_URP = 'U'
	
		END
		
		IF @c_type = 'R'
		BEGIN
			IF(@id_uo IS NULL OR @id_uo = 0)
				BREAK
			
			SELECT @p_cod = VAR_COD_RUBRICA
				,@system_id = SYSTEM_ID 
			FROM DOCSADM.DPA_CORR_GLOBALI 
			WHERE SYSTEM_ID = @id_uo 
				AND ID_AMM = @id_amm 
				AND DTA_FINE IS NULL
				AND CHA_TIPO_URP = 'R'
		
		END
		
		IF @c_type = 'P'
		BEGIN
		
			SELECT @p_cod = VAR_COD_RUBRICA
			FROM DOCSADM.DPA_CORR_GLOBALI 
			WHERE ID_GRUPPO = @id_ruolo 
				AND ID_AMM = @id_amm 
				AND DTA_FINE IS NULL
		
		END
		
		IF @p_cod IS NULL
			BREAK
			
		SELECT @id_parent = ID_PARENT
			, @system_id = SYSTEM_ID
			, @id_uo = ID_UO
			, @tipoCorr = CHA_TIPO_URP
		FROM DOCSADM.DPA_CORR_GLOBALI
		WHERE VAR_COD_RUBRICA = @p_cod
			AND ID_AMM = @id_amm
			AND DTA_FINE IS NULL
			AND CHA_TIPO_URP NOT IN ('F','P')
			
		SET @codes = @p_cod+':'+@codes
    
    END

	SET @codes += @codRubrica
return @codes
END