
/****** Object:  UserDefinedFunction [DOCSADM].[getContatoreDoc2]    Script Date: 05/06/2013 17:35:44 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[getContatoreDoc2]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DOCSADM].[getContatoreDoc2]
GO



/****** Object:  UserDefinedFunction [DOCSADM].[getContatoreDoc2]    Script Date: 05/06/2013 17:35:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 29/03/2013
-- NUOVA VERSIONE DELLA FUNZIONE:getContatoreDoc 
-- PALUMBO: modifica per problema ricerca con griglia
-- =============================================


CREATE FUNCTION [DOCSADM].[getContatoreDoc2] 
(
	@docNumber INT
	, @tipoContatore NVARCHAR(2000)
	, @oggettoCustomId INT
)
RETURNS VARCHAR(4000)
AS 
BEGIN
	DECLARE @risultato			VARCHAR(255)
	DECLARE @valoreContatore	VARCHAR(255)
	DECLARE @annoContatore		VARCHAR(255)
	DECLARE @codiceRegRf		VARCHAR(255)
	DECLARE @repertorio			INT

	SET @valoreContatore = ''
	SET @annoContatore = ''
	SET @codiceRegRf = ''

	SELECT @valoreContatore = valore_oggetto_db
		, @annoContatore = anno
		, @repertorio = repertorio
	FROM DOCSADM.DPA_ASSOCIAZIONE_TEMPLATES AT
		, DOCSADM.DPA_OGGETTI_CUSTOM OC
		, DOCSADM.DPA_TIPO_OGGETTO TOG
	WHERE AT.doc_number = CAST(@docNumber AS VARCHAR)
		AND AT.ID_OGGETTO = OC.SYSTEM_ID
		AND OC.ID_TIPO_OGGETTO = TOG.SYSTEM_ID
		AND TOG.Descrizione = 'Contatore'
		AND TOG.SYSTEM_ID = @oggettoCustomId
		
	IF(@repertorio = 1) 
	BEGIN
		SET @risultato = '#CONTATORE_DI_REPERTORIO#'
		RETURN @risultato
	END
	
	IF(@tipoContatore<>'T') 
	BEGIN
		SELECT @codiceRegRf = R.var_codice
		FROM DOCSADM.DPA_ASSOCIAZIONE_TEMPLATES AT, 
			DOCSADM.DPA_OGGETTI_CUSTOM OC, 
			DOCSADM.DPA_TIPO_OGGETTO TOG, 
			DOCSADM.DPA_EL_REGISTRI R
		WHERE AT.doc_number = CAST(@docNumber AS VARCHAR)
			AND AT.ID_OGGETTO = OC.SYSTEM_ID
			AND OC.ID_TIPO_OGGETTO = TOG.SYSTEM_ID
			AND TOG.Descrizione = 'Contatore'
			AND OC.SYSTEM_ID = @oggettoCustomId
			AND AT.ID_AOO_RF = R.SYSTEM_ID
	END

	IF(@codiceRegRf is  null)
		SET @risultato = ISNULL(convert(varchar(255),@valoreContatore),' ')+'-'+ISNULL(convert(varchar(255),@annoContatore),' ')
	ELSE  
		SET @repertorio =  ISNULL(convert(varchar(255),@codiceRegRf),' ') +'-'+ ISNULL(convert(varchar(255),@annoContatore),' ') +'-'+ ISNULL(convert(varchar(255),@valoreContatore),' ')

	RETURN @risultato
END




GO


