begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='PROJECT' and column_name='CHA_PRIVATO';
    if (cnt > 0) then        
             update PROJECT SET CHA_PRIVATO = '0'where (cha_tipo_proj ='F') and (cha_tipo_fascicolo ='P') and (cha_privato is null) ;
	end if;
	end;
end;
/
