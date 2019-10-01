if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[TR_UPDATE_DPA_TODOLIST]') and OBJECTPROPERTY(id, N'IsTrigger') = 1)
drop TRIGGER [TR_UPDATE_DPA_TODOLIST] 
GO


CREATE TRIGGER [TR_UPDATE_DPA_TODOLIST] ON [@db_user].[DPA_TRASM_UTENTE] 
AFTER UPDATE 
AS
IF UPDATE(CHA_IN_TODOLIST)
BEGIN
	DELETE @db_user.DPA_TODOLIST WHERE ID_TRASM_UTENTE = (select system_id from INSERTED)
END
go
