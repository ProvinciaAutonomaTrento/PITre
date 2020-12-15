-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 22/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- INSERT_DPA_CHIAVI_CONFIGURAZIONE
-- =============================================

ALTER PROCEDURE DOCSADM.Utl_Insert_Chiave_Config
(
    @Codice               VARCHAR(100) ,
    @Descrizione          VARCHAR(2000) ,
    @Valore               VARCHAR(128) ,
    @Tipo_Chiave          VARCHAR(1) ,
    @Visibile             VARCHAR(1) ,
    @Modificabile         VARCHAR(1) ,
    @Globale              VARCHAR(1) ,
    @Myversione_Cd        VARCHAR(32) ,
    @Codice_Old_Webconfig VARCHAR(128) ,
    @Forza_Update         VARCHAR(1) , 
    @RFU                  VARCHAR(10) 
)
AS
BEGIN

	DECLARE @Cnt		INT
	DECLARE @Nomeutente VARCHAR(128)
	
	SET @Nomeutente = 'DOCSADM'

	-- CONTROLLI LUNGHEZZA VALORI PASSATI
	
	If docsadm.Utl_IsValore_Lt_Column(@Codice, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_CODICE') = 1 
	BEGIN
		PRINT 'parametro CODICE too large for column VAR_CODICE' 
		RETURN -1
	END 
  
	IF docsadm.Utl_IsValore_Lt_Column(@Descrizione, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Descrizione') = 1 
	BEGIN
		PRINT 'parametro Descrizione too large for column VAR_Descrizione'  
		RETURN -1
	END 
  
	IF docsadm.Utl_IsValore_Lt_Column(@Valore, 'DPA_CHIAVI_CONFIGURAZIONE', 'VAR_Valore') = 1 
	BEGIN
		PRINT 'parametro Valore too large for column VAR_VALORE'
		RETURN -1
	END 

	-- FINE CONTROLLI LUNGHEZZA VALORI PASSATI
  
	SELECT @Cnt = COUNT(*) 
	FROM SYS.TABLES AS T 
		INNER JOIN SYS.columns AS C ON T.object_id = C.object_id
	WHERE T.name = 'DPA_CHIAVI_CONFIGURAZIONE' 
		AND C.name In ('DTA_INSERIMENTO','VERSIONE_CD')
 
	IF (@Cnt < 2)  
	BEGIN 
		EXECUTE DOCSADM.Utl_Add_Column @Myversione_Cd,@Nomeutente,'DPA_CHIAVI_CONFIGURAZIONE','DTA_INSERIMENTO','DATE','SYSDATE', Null,Null,Null
		EXECUTE DOCSADM.utl_add_column @Myversione_Cd,@Nomeutente,'DPA_CHIAVI_CONFIGURAZIONE','VERSIONE_CD','VARCHAR(1)(32)', Null, Null,Null,Null  
	END  
  
	-- PER SUCCESSIVO CONTROLLO CHE SEQUENCE SIA PI AVANTI DEL MAX SYSTEM_ID
  
	SELECT @Cnt = COUNT(*) 
	FROM DOCSADM.DPA_CHIAVI_CONFIGURAZIONE
	WHERE Var_Codice = @Codice
  
	-- INSERISCO LA CHIAVE GLOBALE NON ESISTENTE
	  
	IF (@Cnt = 0 AND @Globale = 1)
	BEGIN 
	 
		INSERT  INTO DOCSADM.DPA_CHIAVI_CONFIGURAZIONE
		( 
			Id_Amm
			, Var_Codice        
			, Var_Descrizione  
			, Var_Valore 
			, Cha_Tipo_Chiave   
			, Cha_Visibile 
			, CHA_MODIFICABILE  
			, CHA_GLOBALE
			, VAR_CODICE_OLD_WEBCONFIG
			, VERSIONE_CD 
		)
		VALUES
		( 
			0
			, @Codice        
			, @Descrizione          
			, @valore   
			, @Tipo_Chiave   
			, @Visibile
			, @Modificabile  
			, @Globale 
			, @Codice_Old_Webconfig
			, @myVERSIONE_CD 
		)
	
		PRINT 'INSERITA NUOVA CHIAVE GLOBALE ' + @Codice
	END           
  
	-- INSERISCO LA CHIAVE NON GLOBALE NON ESISTENTE
	
	IF (@Cnt = 0 AND @Globale = 0)  
    BEGIN       
		INSERT INTO DOCSADM.DPA_CHIAVI_CONFIGURAZIONE
        ( 
			Id_Amm
			, Var_Codice 
			, VAR_DESCRIZIONE 
			, VAR_VALORE 
			, Cha_Tipo_Chiave
			, Cha_Visibile 
			, CHA_MODIFICABILE
			, CHA_GLOBALE
			, VAR_CODICE_OLD_WEBCONFIG 
			, VERSIONE_CD
		)
		SELECT Amm.System_Id As Id_Amm
			, @Codice As Var_Codice
			, @Descrizione As Var_Descrizione 
			, @Valore As Var_Valore 
			, @Tipo_Chiave As Cha_Tipo_Chiave
			, @Visibile As Cha_Visibile 
			, @Modificabile As Cha_Modificabile
			, @Globale as CHA_GLOBALE 
			, @CODICE_OLD_WEBCONFIG as VAR_CODICE_OLD_WEBCONFIG 
			, @myVERSIONE_CD as VERSIONE_CD
		FROM DOCSADM.DPA_AMMINISTRA Amm
		WHERE NOT EXISTS (SELECT  'X'
		                   FROM DOCSADM.DPA_CHIAVI_CONFIGURAZIONE Cc
                           WHERE Cc.Var_Codice = @Codice 
                                and cc.Id_Amm = Amm.System_Id);
                             
        PRINT 'INSERITE NUOVE CHIAVI LOCALI PER LE N AMMINISTRAZIONI: ' + CAST(@CODICE AS VARCHAR)
  
	END
  
	-- CHIAVE GIÀ ESISTENTE
	
	IF (@Cnt >= 1) 
	BEGIN
	
		PRINT 'CHIAVE ' + CAST(@Codice AS VARCHAR) + ' GIÀ ESISTENTE'
		IF @Forza_Update = '1'  
		BEGIN 
    
			UPDATE DOCSADM.DPA_CHIAVI_CONFIGURAZIONE
			SET VAR_DESCRIZIONE = @Descrizione
				,Var_Valore = @Valore
				,Cha_Visibile = @Visibile
				,Cha_Modificabile = @Modificabile
				,cha_Tipo_Chiave = @Tipo_Chiave	
			WHERE Var_Codice = @Codice       
				AND @modificabile = '1';
				
			PRINT  'AGGIORNATO VALORE, VISIBILITÀ, MODIFICABILITÀ E TIPO, PER LA CHIAVE: ' + CAST(@Codice AS VARCHAR) + ' GIÀ ESISTENTE' 
		END
		ELSE
		BEGIN

			UPDATE DOCSADM.DPA_CHIAVI_CONFIGURAZIONE
			SET Var_Descrizione = @Descrizione 
			WHERE Var_Codice = @Codice       
				AND @modificabile = '1';
		END 
	END
END
    