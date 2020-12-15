--------------------------------------------------------
--  DDL for Procedure SP_INS_MODELLI_DEST_NOTIFICA
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INS_MODELLI_DEST_NOTIFICA" IS
VAR_ID_MITT_DEST NUMBER;
VAR_ID_PEOPLE NUMBER;
VAR_ID_MODELLO NUMBER;

CURSOR cursorINS IS
      SELECT      
        MMD.SYSTEM_ID, 
        PG.PEOPLE_SYSTEM_ID,
        MMD.ID_MODELLO 
      FROM DPA_CORR_GLOBALI CG, DPA_MODELLI_MITT_DEST MMD, PEOPLEGROUPS PG
      WHERE CG.SYSTEM_ID = MMD.ID_CORR_GLOBALI
      AND CG.ID_GRUPPO = PG.GROUPS_SYSTEM_ID
      AND MMD.CHA_TIPO_MITT_DEST = 'D'
      AND MMD.CHA_TIPO_URP = 'R'
      AND PG.DTA_FINE IS NULL
      AND MMD.SYSTEM_ID NOT IN (SELECT DISTINCT ID_MODELLO_MITT_DEST FROM DPA_MODELLI_DEST_CON_NOTIFICA)
      ORDER BY MMD.ID_MODELLO;  
      
  CURSOR cursorINS_people IS    
         SELECT      
        MMD.SYSTEM_ID, 
        CG.id_people,
        MMD.ID_MODELLO 
      FROM DPA_CORR_GLOBALI CG, DPA_MODELLI_MITT_DEST MMD--, PEOPLEGROUPS PG
      WHERE  CG.SYSTEM_ID = MMD.ID_CORR_GLOBALI
    --  AND CG.ID_PEOPLE = PG.PEOPLE_SYSTEM_ID
      and MMD.CHA_TIPO_MITT_DEST = 'D'
      AND MMD.CHA_TIPO_URP in( 'P')
      --AND PG.DTA_FINE IS NULL
      AND MMD.SYSTEM_ID NOT IN (SELECT  ID_MODELLO_MITT_DEST FROM DPA_MODELLI_DEST_CON_NOTIFICA)
      ORDER BY MMD.ID_MODELLO;
      
BEGIN
            
    OPEN cursorINS;
        LOOP 
            FETCH cursorINS INTO VAR_ID_MITT_DEST,VAR_ID_PEOPLE,VAR_ID_MODELLO;
            EXIT WHEN cursorINS%NOTFOUND;
                
            BEGIN
                                            
                INSERT INTO DPA_MODELLI_DEST_CON_NOTIFICA
                (SYSTEM_ID,
                ID_MODELLO_MITT_DEST,
                ID_PEOPLE,
                ID_MODELLO)
                VALUES
                (SEQ.NEXTVAL,
                VAR_ID_MITT_DEST,
                VAR_ID_PEOPLE,
                VAR_ID_MODELLO);
                    
            END;
                
        END LOOP;
        
        close cursorINS;
        
       OPEN cursorINS_People;
        LOOP 
            FETCH cursorINS_People INTO VAR_ID_MITT_DEST,VAR_ID_PEOPLE,VAR_ID_MODELLO;
            EXIT WHEN cursorINS_People%NOTFOUND;
                
            BEGIN
                                            
                INSERT INTO DPA_MODELLI_DEST_CON_NOTIFICA
                (SYSTEM_ID,
                ID_MODELLO_MITT_DEST,
                ID_PEOPLE,
                ID_MODELLO)
                VALUES
                (SEQ.NEXTVAL,
                VAR_ID_MITT_DEST,
                VAR_ID_PEOPLE,
                VAR_ID_MODELLO);
                    
            END;
                
        END LOOP;
        
        close cursorINS_people;
        
  
END SP_INS_MODELLI_DEST_NOTIFICA; 

/
