
-- lasciare una riga bianca vuota all'inizio e a fine file per evitare efetti di concatenazione tra i vari script
-- inserire SEMPRE il carattere / a fine script altrimenti lo script non va in esecuzione!

begin
-- MEV Fatturazione elettronica by Panici - Iazzetta

--	ALTER TABLE DPA_EL_REGISTRI ADD (VAR_CODICE_AOO_IPA  VARCHAR2(16 BYTE) );
--	ALTER TABLE DPA_EL_REGISTRI ADD (CODICE_UAC          VARCHAR2(128 BYTE) );
--	ALTER TABLE DPA_EL_REGISTRI ADD (CODICE_CLASSIFICAZIONE VARCHAR2(20 BYTE) );

utl_add_column ('3.26',
                   '@db_user',
                   'DPA_EL_REGISTRI',
                   'VAR_CODICE_AOO_IPA',
                   'VARCHAR2(16 BYTE)',
                   NULL,NULL,NULL,NULL);

utl_add_column ('3.26',
                   '@db_user',
                   'DPA_EL_REGISTRI',
                   'CODICE_UAC',
                   'VARCHAR2(128 BYTE)',
                   NULL,NULL,NULL,NULL);

utl_add_column ('3.26',
                   '@db_user',
                   'DPA_EL_REGISTRI',
                   'CODICE_CLASSIFICAZIONE',
                   'VARCHAR2(20 BYTE)',
                   NULL,NULL,NULL,NULL);

end;
/


