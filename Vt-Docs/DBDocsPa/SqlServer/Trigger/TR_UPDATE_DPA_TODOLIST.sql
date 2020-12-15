USE [GFD_29b_07_2010]
GO

/****** Object:  Trigger [DOCSADM].[TR_UPDATE_DPA_TODOLIST]    Script Date: 09/06/2010 07:39:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   TRIGGER [DOCSADM].[TR_UPDATE_DPA_TODOLIST] ON [DOCSADM].[DPA_TRASM_UTENTE]
AFTER UPDATE
AS
IF UPDATE(CHA_IN_TODOLIST)
BEGIN
declare @cha_in_todolist varchar(1)
set @cha_in_todolist = (select top 1 cha_in_todolist  from INSERTED)
if(@cha_in_todolist = '0') begin
DELETE docsadm.DPA_TODOLIST WHERE ID_TRASM_UTENTE IN (select system_id from INSERTED)
end
END

GO


