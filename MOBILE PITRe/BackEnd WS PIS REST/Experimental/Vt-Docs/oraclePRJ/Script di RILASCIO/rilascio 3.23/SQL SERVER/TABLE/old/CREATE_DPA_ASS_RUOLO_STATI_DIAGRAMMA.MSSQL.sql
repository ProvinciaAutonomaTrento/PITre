IF NOT EXISTS (SELECT * FROM dbo.sysobjects 
				WHERE id = OBJECT_ID(N'[@db_user].[DPA_ASS_RUOLO_STATI_DIAGRAMMA]') 
					AND OBJECTPROPERTY(id, N'IsUserTable') = 1
					)
begin
CREATE TABLE @db_user.DPA_ASS_RUOLO_STATI_DIAGRAMMA
                  ( SYSTEM_ID	INT NOT NULL PRIMARY KEY
                  , ID_GRUPPO	INT NOT NULL
                  , ID_DIAGRAMMA INT NOT NULL
                  , ID_STATO	INT NOT NULL
                  , CHA_NOT_VIS CHAR(1) DEFAULT '1' NOT NULL)
end
GO

