BEGIN
declare cntcol int;
 
begin
select count(*) into cntcol from cols
                where table_name='DPA_DOCSPA' and column_name='VAR_MESSAGGIO_LOGIN';
 
if cntcol = 0 then
                execute immediate
                'alter table DPA_DOCSPA add (VAR_MESSAGGIO_LOGIN VARCHAR2(2000))';
end if;
 
end;
END;
/
