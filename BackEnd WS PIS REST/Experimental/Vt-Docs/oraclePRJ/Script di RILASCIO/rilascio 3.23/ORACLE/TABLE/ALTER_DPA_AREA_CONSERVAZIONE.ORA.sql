BEGIN
-- MEV Area conservazione by Faillace - Lembo 
-- maschera di bit per validazione verifica
-- es. dato contenuto: 0,1,2,4,8.. a potenze di due
-- 0 = non verificato
-- 1 = ...
-- 
   Utl_Add_Column ('3.23','@db_user',
                   'DPA_AREA_CONSERVAZIONE',
                   'VALIDATION_MASK', 
                   'INTEGER',
                   0,
                   NULL,
                   NULL,
                   NULL
                  );
End;
/

