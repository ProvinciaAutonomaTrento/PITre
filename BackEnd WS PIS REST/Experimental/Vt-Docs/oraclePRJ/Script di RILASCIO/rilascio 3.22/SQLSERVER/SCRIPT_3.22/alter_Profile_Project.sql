

DECLARE	@return_value int

EXEC	@return_value = [DOCSADM].[utl_add_column]
		@versioneCD = N'3.22',
		@nome_utente = N'DOCSADM',
		@nome_tabella = N'PROFILE',
		@nome_colonna = N'COD_EXT_APP',
		@tipo_dato = N'varchar2(32)',
		@val_default = N'null',
		@condizione_modifica_pregresso = N'null',
		@condizione_check = N'null',
		@RFU = N'null'

SELECT	'Return Value' = @return_value

GO

DECLARE	@return_value int

EXEC	@return_value = [DOCSADM].[utl_add_column]
		@versioneCD = N'3.22',
		@nome_utente = N'DOCSADM',
		@nome_tabella = N'PROJECT',
		@nome_colonna = N'COD_EXT_APP',
		@tipo_dato = N'varchar2(32)',
		@val_default = N'null',
		@condizione_modifica_pregresso = N'null',
		@condizione_check = N'null',
		@RFU = N'null'

SELECT	'Return Value' = @return_value

GO

