
begin
  execute immediate
          'ALTER TABLE VERSIONS '||
          'MODIFY (COMMENTS VARCHAR2(200 CHAR) )'          ;
end;
/

