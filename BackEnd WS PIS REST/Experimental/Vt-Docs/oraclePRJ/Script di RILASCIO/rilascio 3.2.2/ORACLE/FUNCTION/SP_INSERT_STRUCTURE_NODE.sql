CREATE OR REPLACE PROCEDURE SP_INSERT_STRUCTURE_NODE
( 
    NAME IN VARCHAR,
    PARENT_ID IN INTEGER,
    TEMPLATE_ID IN INTEGER,
    ID OUT INTEGER,
    returnvalue OUT INTEGER
)
IS
/******************************************************************************
   NAME:       SP_INSERT_STRUCTURE_NODE
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        01/02/2017   crinaldi       1. Created this procedure.

   NOTES:
   
******************************************************************************/
    key_value INTEGER := 0;
BEGIN

    key_value := SEQ_PROJECT_STRUCTURE.NEXTVAL;
    
    INSERT INTO PROJECT_STRUCTURE(SYSTEM_ID, ID_PARENT, NAME, ID_TEMPLATE)
    VALUES (key_value, PARENT_ID, NAME, TEMPLATE_ID);
    
    returnvalue := 0;
    ID := key_value;
    
    EXCEPTION
    WHEN OTHERS THEN
        returnvalue := -1;
END;