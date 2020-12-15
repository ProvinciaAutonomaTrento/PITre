if exists(select * from syscolumns where name='path' and id in 
		(select id from sysobjects where name='components' and xtype='U'))
begin		
	alter table @db_user.components alter column path VARCHAR(500)
end
GO
