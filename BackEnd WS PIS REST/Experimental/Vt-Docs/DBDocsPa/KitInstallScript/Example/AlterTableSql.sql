if not exists (SELECT * FROM syscolumns WHERE name='CSS' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_AMMINISTRA' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_AMMINISTRA]
	ADD CSS INT;
END;


GO
