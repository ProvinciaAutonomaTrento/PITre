
---- Utl_insert_chiave_log.ORA.sql  marcatore per ricerca ----
ALTER PROCEDURE [DOCSADM].[Utl_insert_chiave_log]
(
    @Codice					VARCHAR(100),
    @Descrizione			VARCHAR(2000),
    @Oggetto				VARCHAR(128),
    @Metodo					Varchar(128),
    @Forza_Aggiornamento	Varchar(1),
    @Myversione_cd			Varchar(32),
    @RFU					VARCHAR(10) 
)
AS

DECLARE @Cnt			INT
DECLARE @Maxid			INT
DECLARE @Nomeutente		VARCHAR(32)
DECLARE @Stringa_Msg	VARCHAR(200)
  
BEGIN
  
-- CONTROLLI LUNGHEZZA VALORI PASSATI

	IF DOCSADM.Utl_IsValore_Lt_Column(@Codice, 'dpa_anagrafica_log', 'VAR_CODICE') = 1 
	BEGIN
		PRINT 'parametro CODICE too large for column VAR_CODICE' 
		RETURN -1
	END
  
	IF DOCSADM.Utl_IsValore_Lt_Column(@Descrizione, 'dpa_anagrafica_log', 'VAR_Descrizione') = 1 
	BEGIN
		PRINT 'parametro Descrizione too large for column VAR_Descrizione'  
		RETURN -1
	END
  
	IF DOCSADM.Utl_IsValore_Lt_Column(@Oggetto, 'dpa_anagrafica_log', 'VAR_OGGETTO') = 1 
	BEGIN
		PRINT 'parametro Valore too large for column VAR_OGGETTO'
		RETURN -1
	END

	IF DOCSADM.Utl_IsValore_Lt_Column(@Metodo, 'dpa_anagrafica_log', 'VAR_METODO') = 1  
    BEGIN
		PRINT 'parametro Valore too large for column VAR_METODO'  
		RETURN -1
	END
	
-- FINE CONTROLLI LUNGHEZZA VALORI PASSATI
 
	SELECT @Cnt = COUNT(*)  
	FROM DOCSADM.DPA_ANAGRAFICA_LOG
	WHERE VAR_CODICE = @Codice
  
-- INSERISCO LA CHIAVE GLOBALE NON ESISTENTE 

	IF (@Cnt = 0 )
	BEGIN 
       INSERT INTO DOCSADM.DPA_ANAGRAFICA_LOG
       ( 
			System_Id
			,var_Codice
			,Var_Descrizione
			,Var_Oggetto
			,Var_Metodo 
		) 
		SELECT MAX(SYSTEM_ID)+1  AS SYSTEM_ID       
			,@Codice
			,@Descrizione        
			,@Oggetto
			,@Metodo
		FROM DOCSADM.DPA_ANAGRAFICA_LOG
      
		PRINT 'inserita nuova chiave log ' + @Codice   
	END          
  
  
	IF (@Cnt = 1 and @Forza_Aggiornamento = '1')
	BEGIN
		UPDATE DOCSADM.DPA_ANAGRAFICA_LOG
		SET Var_Descrizione = @Descrizione
			, Var_Oggetto = @oggetto
			, Var_Metodo = @metodo
		WHERE Var_Codice = @Codice
		PRINT 'Aggiornati Descrizione, Oggetto, Metodo per chiave log: ' + @Codice
	END
END
