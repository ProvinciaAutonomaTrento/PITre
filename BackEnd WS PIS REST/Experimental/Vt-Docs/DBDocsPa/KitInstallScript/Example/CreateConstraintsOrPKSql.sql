if exists (select id from sysobjects where name='DPA_ASSOCIAZIONE_TEMPLATES' and xtype='U')
begin
if not exists(select id from sysobjects where name='FK_ASSTEMPL_IDOGG_OGGCUST_SYSID' and xtype='F')
ALTER TABLE [@db_user].[DPA_ASSOCIAZIONE_TEMPLATES] 
ADD CONSTRAINT FK_ASSTEMPL_IDOGG_OGGCUST_SYSID
	FOREIGN KEY ([ID_OGGETTO]) 
	REFERENCES [@db_user].[DPA_OGGETTI_CUSTOM] ([SYSTEM_ID])
GO	
--oppure

ALTER TABLE [@db_user].[DPA_STATI] ADD CONSTRAINT [PK__DPA_STATI] PRIMARY KEY CLUSTERED  ([SYSTEM_ID])
GO	
	
	