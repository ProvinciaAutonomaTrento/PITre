-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 05/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- GETCONTATOREDOC
-- =============================================


ALTER FUNCTION [DOCSADM].[getContatoreDoc]
(
	@docNumber				INT 
	, @tipoContatore		VARCHAR(2000)
)

RETURNS VARCHAR(4000) 
AS
BEGIN

	DECLARE @risultato			VARCHAR(255)
	DECLARE @valoreContatore	VARCHAR(255)
	DECLARE @annoContatore		VARCHAR(255)
	DECLARE @codiceRegRf		VARCHAR(255)

	DECLARE @REPERTORIO			INT

	SET @valoreContatore = ''
	SET @annoContatore = ''
	SET @codiceRegRf = ''

	
	SELECT @valoreContatore = ASS.Valore_Oggetto_Db
		, @annoContatore = ASS.ANNO
		, @REPERTORIO = OC.REPERTORIO
	FROM DOCSADM.DPA_ASSOCIAZIONE_TEMPLATES AS ASS
		, DOCSADM.DPA_OGGETTI_CUSTOM AS OC
		, DOCSADM.DPA_TIPO_OGGETTO AS OT
	WHERE ASS.DOC_NUMBER = @docNumber
		AND ASS.ID_OGGETTO = OC.SYSTEM_ID
		AND OC.ID_TIPO_OGGETTO = OT.SYSTEM_ID
		AND OT.Descrizione = 'Contatore'
		--AND OC.CHA_TIPO_TAR = @tipoContatore
		AND OC.DA_VISUALIZZARE_RICERCA = '1'


	IF(@REPERTORIO = 1) 
	BEGIN
		SET @risultato = '#CONTATORE_DI_REPERTORIO#'
		RETURN @risultato
	END


	IF(@tipoContatore <> 'T')
	BEGIN
	
		SELECT @codiceRegRf = R.VAR_CODICE
		FROM DOCSADM.DPA_ASSOCIAZIONE_TEMPLATES AS ASS
			, DOCSADM.DPA_OGGETTI_CUSTOM AS OC
			, DOCSADM.DPA_TIPO_OGGETTO OT
			, DOCSADM.DPA_EL_REGISTRI AS R
		WHERE ASS.Doc_Number = @docNumber
			AND ASS.ID_OGGETTO = OC.SYSTEM_ID
			AND OC.ID_TIPO_OGGETTO = OC.SYSTEM_ID
			AND OT.Descrizione = 'Contatore'
			AND OC.DA_VISUALIZZARE_RICERCA = '1'
			AND ASS.ID_AOO_RF = R.SYSTEM_ID
	END


	IF(@codiceRegRf IS NULL)
		SET @risultato = ISNULL(@valoreContatore,'')+'-'+ISNULL(@annoContatore,'') 
	ELSE  
		SET @risultato = ISNULL(@codiceRegRf,'')+'-'+ISNULL(@annoContatore,'')+'-'+ISNULL(@valoreContatore,'')

	RETURN @risultato;

END