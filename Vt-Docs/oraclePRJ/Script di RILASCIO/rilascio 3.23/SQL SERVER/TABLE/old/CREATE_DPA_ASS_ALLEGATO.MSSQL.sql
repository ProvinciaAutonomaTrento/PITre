IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_ASS_ALLEGATO]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
begin
-- import pregressi by Veltri   
CREATE TABLE @db_user.DPA_ASS_ALLEGATO ( 
			SYSTEM_ID	INTEGER IDENTITY(1,1) , 
            ID_ITEM	 	INTEGER, 
			ERRORE 		VARCHAR(1000), 
			ESITO 		CHAR(1)    )
end
GO

BEGIN
utl_add_index '3.23','@db_user',
	'DPA_ASS_ALLEGATO','IX_T_DPA_ASS_ALLEGATOK2',null, 
    'ID_ITEM',null,null,null,   -- lista colonne
    'NORMAL', null, null, null     
END
GO




