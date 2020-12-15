if not exists(select * from @db_user.dpa_tipo_oggetto where Descrizione='Corrispondente')
BEGIN
insert into @db_user.dpa_tipo_oggetto VALUES('Corrispondente','Corrispondente')
END
GO