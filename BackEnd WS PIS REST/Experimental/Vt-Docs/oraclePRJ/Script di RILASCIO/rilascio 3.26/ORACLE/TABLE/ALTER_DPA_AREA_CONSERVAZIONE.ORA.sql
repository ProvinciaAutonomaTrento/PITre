
begin
  execute immediate
          'ALTER TABLE DPA_AREA_CONSERVAZIONE '||
          'MODIFY (VAR_MARCA_TEMPORALE VARCHAR2(4000 CHAR) )'          ;
end;
/

