CREATE OR REPLACE FUNCTION getInEsibizione (IDPROFILE number, Idproject number, typeID char, idPeople number, idGruppo number, idConservazione number)

RETURN INT IS risultato INT;
res_appo INT;
idRuoloInUo NUMBER;
BEGIN
    
    begin

        SELECT DPA_CORR_GLOBALI.SYSTEM_ID INTO idRuoloInUo 
        FROM DPA_CORR_GLOBALI 
        WHERE DPA_CORR_GLOBALI.ID_GRUPPO = idGruppo;

        -- Documenti
        IF (typeID = 'D' AND Idproject is null ) 
        THEN
            SELECT COUNT(DPA_ITEMS_ESIBIZIONE.SYSTEM_ID) INTO risultato 
            FROM DPA_AREA_ESIBIZIONE, DPA_ITEMS_ESIBIZIONE 
            WHERE
            DPA_ITEMS_ESIBIZIONE.ID_ESIBIZIONE=DPA_AREA_ESIBIZIONE.SYSTEM_ID 
            AND DPA_ITEMS_ESIBIZIONE.ID_PROFILE = IDPROFILE
            AND DPA_AREA_ESIBIZIONE.CHA_STATO ='N' 
            AND DPA_AREA_ESIBIZIONE.ID_PEOPLE=idPeople 
            AND    DPA_AREA_ESIBIZIONE.ID_RUOLO_IN_UO = idRuoloInUo  
            AND DPA_ITEMS_ESIBIZIONE.ID_PROJECT IS  NULL
            --AND DPA_ITEMS_ESIBIZIONE.ID_CONSERVAZIONE = idConservazione
            ;
        ELSE
            IF (typeID = 'D' AND Idproject is NOT null ) 
            THEN
                SELECT COUNT(DPA_ITEMS_ESIBIZIONE.SYSTEM_ID) INTO risultato 
                FROM DPA_AREA_ESIBIZIONE, DPA_ITEMS_ESIBIZIONE 
                WHERE DPA_ITEMS_ESIBIZIONE.ID_ESIBIZIONE=DPA_AREA_ESIBIZIONE.SYSTEM_ID 
                AND DPA_ITEMS_ESIBIZIONE.ID_PROFILE = IDPROFILE
                AND DPA_AREA_ESIBIZIONE.CHA_STATO ='N' 
                AND DPA_AREA_ESIBIZIONE.ID_PEOPLE=idPeople 
                AND DPA_AREA_ESIBIZIONE.ID_RUOLO_IN_UO = idRuoloInUo  
                AND DPA_ITEMS_ESIBIZIONE.ID_PROJECT =Idproject
                --AND DPA_ITEMS_ESIBIZIONE.ID_CONSERVAZIONE = idConservazione
                ;
            END IF;
        END IF;
    
        -- Fascicoli
        IF (typeID = 'F') 
        THEN
            SELECT COUNT(DPA_ITEMS_ESIBIZIONE.SYSTEM_ID) INTO risultato 
            FROM DPA_AREA_ESIBIZIONE, DPA_ITEMS_ESIBIZIONE 
            WHERE DPA_ITEMS_ESIBIZIONE.ID_ESIBIZIONE=DPA_AREA_ESIBIZIONE.SYSTEM_ID 
            AND DPA_ITEMS_ESIBIZIONE.ID_PROJECT = Idproject
            AND DPA_ITEMS_ESIBIZIONE.ID_PROFILE = IDPROFILE
            AND DPA_AREA_ESIBIZIONE.CHA_STATO ='N'
            AND DPA_AREA_ESIBIZIONE.ID_PEOPLE=idPeople 
            AND DPA_AREA_ESIBIZIONE.ID_RUOLO_IN_UO = idRuoloInUo
            --AND DPA_ITEMS_ESIBIZIONE.ID_CONSERVAZIONE = idConservazione
            ;
        END IF;

        IF (risultato > 0) THEN
        risultato := 1;
        ELSE
        risultato:=0;
        END IF;

        EXCEPTION
        WHEN NO_DATA_FOUND 
            THEN risultato := 0;
        WHEN OTHERS 
            THEN risultato := 0;
    end;
    
    RETURN risultato;

END getInEsibizione;
/
