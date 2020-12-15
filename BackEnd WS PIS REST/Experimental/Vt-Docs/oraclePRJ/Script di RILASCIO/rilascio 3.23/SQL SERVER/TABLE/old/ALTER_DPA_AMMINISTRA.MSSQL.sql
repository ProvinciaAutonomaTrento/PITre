-- MEV Attivazione Trasmissione Automatica
-- by Iacozzilli Giordano.
-- Inserisco nella Dpa amministra un flag specchio della colonna spedizione automatica
BEGIN

   Utl_Add_Column '3.23','@db_user',
                   'DPA_AMMINISTRA',
                   'TRASMISSIONE_AUTO_DOC', 
                   'CHAR(1 byte)',
                   NULL,
                   NULL,
                   NULL,
                   NULL
              
END
GO