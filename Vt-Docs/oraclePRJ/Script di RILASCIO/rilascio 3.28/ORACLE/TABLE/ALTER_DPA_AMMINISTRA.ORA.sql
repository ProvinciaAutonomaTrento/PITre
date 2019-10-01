
-- lasciare una riga bianca vuota all'inizio e a fine file per evitare efetti di concatenazione tra i vari script
-- inserire SEMPRE il carattere / a fine script altrimenti lo script non va in esecuzione!

begin

-- Mauro Pace

utl_add_column ('3.28',
                   '@db_user',
                   'DPA_AMMINISTRA',
                   'VAR_DETTAGLIO_FIRMA',
                   'VARCHAR2(256 BYTE)',
                   NULL,NULL,NULL,NULL);
                   
end;
/

