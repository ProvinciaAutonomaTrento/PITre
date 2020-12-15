BEGIN
-- by C. Ferlito per albo
-- flag gestione allegati da applicativo esterno
   utl_add_column( '3.23',
                   '@db_user',
                   'VERSIONS',
                   'CHA_ALLEGATI_ESTERNO',
                   'CHAR(1)',
                   '0',NULL,NULL,NULL);
END;
/


