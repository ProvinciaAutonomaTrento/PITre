--------------------------------------------------------
--  DDL for Function GETINCONSERVAZIONENOSEC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETINCONSERVAZIONENOSEC" (IDPROFILE number,Idproject number, typeID char)
RETURN VARCHAR IS result VARCHAR(3000);

BEGIN

--Casella di selezione (Per la casella di selezione serve un caso particolare perch?? i valori sono multipli)
if(typeID = 'D') then
    BEGIN
        declare item varchar(3000);
        CURSOR curCasellaDiSelezione IS select ID_CONSERVAZIONE into result from DPA_ITEMS_CONSERVAZIONE,DPA_AREA_CONSERVAZIONE where DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE=DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND ID_PROFILE = IDPROFILE AND DPA_ITEMS_CONSERVAZIONE.CHA_STATO != 'N'; 
        BEGIN
            OPEN curCasellaDiSelezione;
            LOOP
            FETCH curCasellaDiSelezione INTO item;
            EXIT WHEN curCasellaDiSelezione%NOTFOUND;
                IF(result IS NOT NULL) THEN
                result := result||'-'||item ;
                ELSE
                result := result||item;
                END IF;        
            END LOOP;
            CLOSE curCasellaDiSelezione;
        END;    
    END;
    elsif(typeID = 'F') then
     BEGIN
        declare item varchar(3000);
        CURSOR curCasellaDiSelezione IS select ID_CONSERVAZIONE into result from DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE WHERE DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE=DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND ID_PROJECT = Idproject AND DPA_ITEMS_CONSERVAZIONE.CHA_STATO != 'N' group by ID_CONSERVAZIONE;
        BEGIN
            OPEN curCasellaDiSelezione;
            LOOP
            FETCH curCasellaDiSelezione INTO item;
            EXIT WHEN curCasellaDiSelezione%NOTFOUND;
                IF(result IS NOT NULL) THEN
                result := result||'-'||item ;
                ELSE
                result := result||item;
                END IF;        
            END LOOP;
            CLOSE curCasellaDiSelezione;
        END;    
    END;
end if;

RETURN result;

exception
when no_data_found
then
result := null; 
RETURN result;
when others
then
result := SQLERRM; 
RETURN result;

END getInConservazioneNoSec;

/
