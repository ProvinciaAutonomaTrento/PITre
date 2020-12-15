-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 04/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- UTL_INSERT_CHIAVE_MICROFUNZ
-- =============================================

ALTER PROCEDURE [DOCSADM].[Utl_Insert_Chiave_Microfunz]
(
    @Codice					VARCHAR(100),
    @Descrizione			VARCHAR(2000),
    @Tipo_Chiave            VARCHAR(10),
    @disabilitata           VARCHAR(1),
    @Forza_Disabilitazione  VARCHAR(1), 
    @myversione_CD          VARCHAR(32),
    @RFU					VARCHAR(10) 
) 
    
AS  

BEGIN

	DECLARE @Cnt			INT
	DECLARE @Nomeutente		VARCHAR(20)
	DECLARE @stringa_msg	VARCHAR(200)

	-- CONTROLLI LUNGHEZZA VALORI PASSATI
	
	IF DOCSADM.Utl_IsValore_Lt_Column(@Codice, 'dpa_anagrafica_funzioni', 'COD_FUNZIONE') = 1  
	BEGIN
		SET @stringa_msg =  'parametro CODICE too large for column COD_FUNZIONE' 
		RETURN -1
	END 
  
	IF DOCSADM.Utl_IsValore_Lt_Column(@Descrizione, 'dpa_anagrafica_funzioni', 'VAR_DESC_FUNZIONE') = 1 
	BEGIN
		SET @stringa_msg =  'parametro Descrizione too large for column VAR_DESC_FUNZIONE'  
		RETURN -1
	END 
  
	IF DOCSADM.Utl_IsValore_Lt_Column(@disabilitata, 'dpa_anagrafica_funzioni', 'DISABLED') = 1 
	BEGIN
		SET @stringa_msg = 'parametro Valore too large for column DISABLED'
		RETURN -1
	END 
	
	-- FINE CONTROLLI LUNGHEZZA VALORI PASSATI
	
	SELECT @cnt = COUNT(*)  
	FROM DOCSADM.dpa_anagrafica_funzioni
	WHERE COD_FUNZIONE = @Codice

	-- INSERISCO LA MICROFUNZIONE NON ESISTENTE
	
	IF (@Cnt = 0 )
	BEGIN   
		INSERT  INTO DOCSADM.DPA_ANAGRAFICA_FUNZIONI
        ( 
			COD_FUNZIONE
			,VAR_DESC_FUNZIONE
			,CHA_TIPO_FUNZ
			,DISABLED 
		)
        Values
        ( 
			@Codice
			,@Descrizione          
			,@Tipo_Chiave          
			,@disabilitata  
		)
		
        SET @stringa_msg = 'INSERITA NUOVA MICROFUNZIONE: ' + CAST(@Codice AS VARCHAR) 
	END           

	-- CHIAVE GIÀ ESISTENTE
	
	IF (@Cnt = 1) 
	BEGIN 
		
		IF @Forza_Disabilitazione = '1' 
		BEGIN
			UPDATE DOCSADM.DPA_ANAGRAFICA_FUNZIONI
			SET VAR_DESC_FUNZIONE = @Descrizione,
				DISABLED = @disabilitata
			WHERE COD_FUNZIONE = @Codice
			
			SET @stringa_msg =  'AGGIORNATO VALORE DISABLED: '+ CAST(@disabilitata AS VARCHAR) +' PER MICROFUNZIONE ' + CAST(@Codice AS VARCHAR) + ' GIÀ ESISTENTE'
		
		END 
		ELSE 
		BEGIN 
			UPDATE DOCSADM.DPA_ANAGRAFICA_FUNZIONI
			SET VAR_DESC_FUNZIONE = @Descrizione 
			WHERE COD_FUNZIONE    = @Codice
		END 
   END
    
	SELECT TOP 1 @Nomeutente =  SCHEMA_NAME(schema_id) FROM SYS.tables 
	
	EXEC DOCSADM.Utl_Insert_Log  @Nomeutente, GETDATE, @stringa_msg, @Myversione_Cd ,'ok' 

END

