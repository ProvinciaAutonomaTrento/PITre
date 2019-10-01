
if (not exists (select * from [@db_user].[PROJECT] where (cha_tipo_proj ='F') and (cha_tipo_fascicolo ='P') and (cha_privato is null)))
BEGIN
	UPDATE @db_user.PROJECT SET CHA_PRIVATO = '0' where (cha_tipo_proj ='F') and (cha_tipo_fascicolo ='P') and (cha_privato is null) ;
END;
GO