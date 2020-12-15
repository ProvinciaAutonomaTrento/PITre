
-- lasciare una riga bianca vuota all'inizio e a fine file per evitare efetti di concatenazione tra i vari script
-- inserire SEMPRE il carattere / a fine script altrimenti lo script non va in esecuzione!

begin

-- Faillace

utl_add_column ('3.27',
                   '@db_user',
                   'COMPONENTS',
                   'VAR_NOMEORIGINALE',
                   'VARCHAR2(256 BYTE)',
                   NULL,NULL,NULL,NULL);

end;
/

