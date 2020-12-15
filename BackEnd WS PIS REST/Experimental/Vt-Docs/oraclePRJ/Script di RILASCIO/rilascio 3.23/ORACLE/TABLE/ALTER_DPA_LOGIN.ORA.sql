BEGIN
-- 64 byte will be enough for IPv6
   Utl_Add_Column ('3.23','@db_user',
                   'DPA_LOGIN',
                   'IP_ADDRESS', 
                   'VARCHAR2(64 BYTE) ',
                   NULL,
                   NULL,
                   NULL,
                   NULL
                  );
End;
/
