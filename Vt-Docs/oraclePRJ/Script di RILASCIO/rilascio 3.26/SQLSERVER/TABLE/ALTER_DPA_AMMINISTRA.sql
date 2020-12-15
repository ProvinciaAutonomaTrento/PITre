
-- lasciare una riga bianca vuota all'inizio e a fine file per evitare efetti di concatenazione tra i vari script
-- inserire SEMPRE il carattere / a fine script altrimenti lo script non va in esecuzione!

begin

-- MEV Fatturazione elettronica by Panici - Iazzetta

--	ALTER TABLE DPA_AMMINISTRA ADD (VAR_EMAIL_RES_IPA  VARCHAR2(128 BYTE) );
--	ALTER TABLE DPA_AMMINISTRA ADD (VAR_CODICE_AMM_IPA VARCHAR2(16 BYTE) );

exec utl_add_column '3.26', 'DOCSADM',  'DPA_AMMINISTRA', 'VAR_EMAIL_RES_IPA',   'VARCHAR2(128 BYTE)', NULL,NULL,NULL,NULL               
                   

exec utl_add_column '3.26','DOCSADM','DPA_AMMINISTRA','VAR_CODICE_AMM_IPA','VARCHAR2(16 BYTE)', NULL,NULL,NULL,NULL
                   
                  
                   
end;
GO

