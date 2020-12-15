ALTER TABLE DPA_CORR_GLOBALI
ADD (ID_PESO_ORG NUMBER(10));
/

CREATE OR REPLACE PROCEDURE SP_INSERT_ORDINAMENTO_ORG IS

recordCorrC1 NUMBER;
idParent NUMBER;

recordCorrC2 NUMBER;
idUO NUMBER;

recordCorrC3 NUMBER;
idRuolo NUMBER;

contatoreUO NUMBER;
contatoreRUOLO NUMBER;

--cursore UO (prende id_parent)
CURSOR cursor_IDParent IS
      select distinct id_parent 
      from dpa_corr_globali 
      where cha_tipo_urp = 'U' and 
      cha_tipo_ie = 'I' and 
      dta_fine is null and 
      id_parent not in (0)
      order by id_parent;

-- cursore uo per id_parent
CURSOR cursor_UO (ID_PARENT_UO NUMBER) IS
      select system_id 
      from dpa_corr_globali 
      where id_parent = ID_PARENT_UO and
      cha_tipo_urp = 'U' and 
      cha_tipo_ie = 'I' and 
      dta_fine is null;
      
-- cursore ruoli per uo
CURSOR cursor_RUOLI (ID_UO_PADRE NUMBER) IS
      select system_id 
      from dpa_corr_globali 
      where id_uo = ID_UO_PADRE and
      cha_tipo_urp = 'R' and 
      cha_tipo_ie = 'I' and 
      dta_fine is null;     
          
BEGIN
   
   OPEN cursor_IDParent;
   
   LOOP
    
        FETCH cursor_IDParent INTO recordCorrC1;
        EXIT WHEN cursor_IDParent%NOTFOUND;
        idParent := recordCorrC1;
   
        BEGIN
        
            OPEN cursor_UO(idParent);
            
            contatoreUO := 0;
            
            LOOP
            
                FETCH cursor_UO INTO recordCorrC2;
                EXIT WHEN cursor_UO%NOTFOUND;
                idUO := recordCorrC2;
                
                BEGIN
                    
                    contatoreUO := contatoreUO + 1; 
                    UPDATE DPA_CORR_GLOBALI SET ID_PESO_ORG = contatoreUO WHERE SYSTEM_ID = idUO; 
                    
                    OPEN cursor_RUOLI(idUO);
                    
                    contatoreRUOLO := 0;
                    
                    LOOP
                    
                        FETCH cursor_RUOLI INTO recordCorrC3;
                        EXIT WHEN cursor_RUOLI%NOTFOUND;
                        idRuolo := recordCorrC3;
                        
                        BEGIN
                        
                            contatoreRUOLO := contatoreRUOLO + 1;
                            UPDATE DPA_CORR_GLOBALI SET ID_PESO_ORG = contatoreRUOLO WHERE SYSTEM_ID = idRuolo;
                        
                        END;
                    
                    END LOOP;
                    
                    CLOSE cursor_RUOLI;
                    
                END;
                
            END LOOP;
            
            CLOSE cursor_UO;
        
        END;
   
   END LOOP;
   
   CLOSE cursor_IDParent;
   
END SP_INSERT_ORDINAMENTO_ORG;
/

BEGIN 
  SP_INSERT_ORDINAMENTO_ORG;
  COMMIT; 
END; 
/