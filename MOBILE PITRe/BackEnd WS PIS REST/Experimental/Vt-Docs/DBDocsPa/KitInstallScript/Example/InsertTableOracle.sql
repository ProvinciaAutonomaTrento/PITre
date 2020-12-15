begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_TIPO_OGGETTO where Descrizione='Corrispondente';
	if (cnt = 0) then		
	   insert into DPA_TIPO_OGGETTO VALUES(SEQ_DPA_TIPO_OGGETTO.nextval,'Corrispondente','Corrispondente');
	end if;
	end;
end;
/