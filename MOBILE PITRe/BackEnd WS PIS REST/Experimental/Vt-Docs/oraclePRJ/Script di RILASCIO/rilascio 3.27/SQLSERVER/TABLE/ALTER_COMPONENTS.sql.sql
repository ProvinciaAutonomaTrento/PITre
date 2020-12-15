
-- lasciare una riga bianca vuota all'inizio e a fine file per evitare efetti di concatenazione tra i vari script
-- inserire SEMPRE il carattere / a fine script altrimenti lo script non va in esecuzione!

begin

-- Faillace

execute utl_add_column '3.27',  'DOCSADM','COMPONENTS','VAR_NOMEORIGINALE','VARCHAR(256)', NULL,NULL,NULL,NULL

end;
GO

