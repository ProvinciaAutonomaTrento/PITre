CREATE OR REPLACE PROCEDURE SP_INS_REL_PROJECT_TEMPLATE
( 
    ID_FASCICOLO IN VARCHAR2,
    ID_TITOLARIO IN VARCHAR2,
    ID_TEMPLATE  IN VARCHAR2,
    returnvalue OUT INTEGER
)
IS
/******************************************************************************
   NAME:       SP_INS_REL_PROJECT_TEMPLATE
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        01/02/2017   crinaldi       1. Created this procedure.

   NOTES:
   
******************************************************************************/
v_id_rel INTEGER := -1;
BEGIN

    IF(ID_TEMPLATE IS NULL) THEN
        
        IF(ID_FASCICOLO IS NOT NULL) THEN
            DELETE FROM REL_PROJECT_STRUCTURE WHERE ID_TIPO_FASCICOLO = ID_FASCICOLO;
        ELSE
            DELETE FROM REL_PROJECT_STRUCTURE WHERE ID_TITOLARIO = ID_TITOLARIO;
        END IF;
        
    ELSE
        v_id_rel := SEQ_REL_PROJECT_STRUCTURE.NEXTVAL;
    
        INSERT INTO REL_PROJECT_STRUCTURE (SYSTEM_ID, ID_TEMPLATE, ID_TIPO_FASCICOLO, ID_TITOLARIO) 
        VALUES (v_id_rel, ID_TEMPLATE, ID_FASCICOLO, ID_TITOLARIO);
    END IF;
    
    returnvalue := 0;
    
    EXCEPTION
    WHEN OTHERS THEN
        returnvalue := -1;
END;