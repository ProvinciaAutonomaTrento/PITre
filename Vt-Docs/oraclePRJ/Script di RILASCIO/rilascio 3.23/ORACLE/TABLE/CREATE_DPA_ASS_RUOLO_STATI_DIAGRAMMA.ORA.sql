begin
-- by C. Ferlito per albo
--tabella gestione associazione ruolo/stati del diagramma
    declare cnt int;
    begin
      select count(*) into cnt from user_tables 
	  where table_name='DPA_ASS_RUOLO_STATI_DIAGRAMMA';
        if (cnt = 0) then
          execute immediate
            'CREATE TABLE DPA_ASS_RUOLO_STATI_DIAGRAMMA
                  ( SYSTEM_ID NUMBER(10, 0) NOT NULL
                  , ID_GRUPPO NUMBER(10, 0) NOT NULL
                  , ID_DIAGRAMMA NUMBER(38, 0) NOT NULL
                  , ID_STATO NUMBER(38, 0) NOT NULL
                  , CHA_NOT_VIS VARCHAR2(1) DEFAULT ''1'' NOT NULL
                  , CONSTRAINT DPA_ASS_RUOLO_STATI_DIAGR_PK PRIMARY KEY
                    ( SYSTEM_ID) ENABLE
                  )';
        end if;
    end;
end;
/
