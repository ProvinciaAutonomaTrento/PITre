IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_ASS_PREGRESSI]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
begin
-- import pregressi by Veltri 
CREATE TABLE DPA_ASS_PREGRESSI ( 
			SYSTEM_ID			INTEGER IDENTITY(1,1) , 
            ID_PREGRESSO 		INTEGER , 
            ID_REGISTRO 		INTEGER , 
			ID_DOCUMENTO 		INTEGER , 
			ID_UTENTE 			INTEGER , 
			ID_RUOLO 			INTEGER , 
			TIPO_OPERAZIONE 	CHAR(1 ), 
			DATA 				DATE, 
			ERRORE 			VARCHAR(1000 ), 
			ESITO 				CHAR(1 ), 
			ID_NUM_PROTO_EXCEL VARCHAR(255 ) )
end
GO


BEGIN
utl_add_index '3.23','@db_user',
			'DPA_ASS_PREGRESSI','IX_T_DPA_ASS_PREGRESSIK1','UNIQUE', 
				'SYSTEM_ID',null,null,null, -- lista colonne
				'NORMAL',null, null, null                     

utl_add_index '3.23','@db_user',
			'DPA_ASS_PREGRESSI','IX_T_DPA_PREGRESSIK2',null, 
				'ID_PREGRESSO',null,null,null,  -- lista colonne
				'NORMAL', null, null, null                     
END
GO

