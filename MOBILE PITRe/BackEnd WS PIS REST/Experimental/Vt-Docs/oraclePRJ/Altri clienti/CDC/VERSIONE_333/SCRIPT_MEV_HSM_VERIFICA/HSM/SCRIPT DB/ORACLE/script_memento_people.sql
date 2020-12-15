BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols
                where table_name='PEOPLE' and column_name='VAR_MEMENTO';

if cntcol = 0 then
                execute immediate
                'alter table PEOPLE add (VAR_MEMENTO VARCHAR2(50))';
end if;

end;
END;
