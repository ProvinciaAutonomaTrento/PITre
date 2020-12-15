BEGIN
declare cntcol int;
 
begin
select count(*) into cntcol from cols
                where table_name='DPA_TIPO_ATTO' and column_name='CHA_ASSOC_MANUALE';
 
if cntcol = 0 then
                execute immediate
                'alter table DPA_TIPO_ATTO add (CHA_ASSOC_MANUALE VARCHAR2(1) default '0')';
end if;
 
end;
END;
/