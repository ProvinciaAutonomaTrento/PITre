CREATE OR REPLACE PROCEDURE SVILUPPO_PITRE.SP_UPD_REL_PROJECT_TEMPLATE
( 
    ID_FASCICOLO IN VARCHAR,
    ID_TITOLARIO IN VARCHAR,
    ID_TEMPLATE  IN VARCHAR,
    returnvalue OUT INTEGER
)
IS
    v_code  NUMBER;
    v_errm  VARCHAR2(64);
    v_id_titolario varchar2(255) := ID_TITOLARIO;
    v_id_template varchar2(255) := ID_TEMPLATE;
    v_id_fascicolo varchar2(255) := ID_FASCICOLO;
BEGIN
    
    IF(ID_TEMPLATE IS NULL) THEN
        
        IF(ID_FASCICOLO IS NOT NULL) THEN
            DELETE FROM REL_PROJECT_STRUCTURE WHERE ID_TIPO_FASCICOLO = v_id_fascicolo;
        ELSE
            DELETE FROM REL_PROJECT_STRUCTURE WHERE ID_TITOLARIO = v_id_titolario;
        END IF;
    
    ELSIF(ID_FASCICOLO IS NOT NULL) THEN
        UPDATE REL_PROJECT_STRUCTURE SET ID_TEMPLATE = v_id_template WHERE ID_TIPO_FASCICOLO = v_id_fascicolo;
    ELSIF(ID_TITOLARIO IS NOT NULL) THEN
        UPDATE REL_PROJECT_STRUCTURE SET ID_TEMPLATE = v_id_template WHERE ID_TITOLARIO = v_id_titolario;
    END IF;
    
    returnvalue := 0;
    
    EXCEPTION
    WHEN OTHERS THEN
        returnvalue := -1;
        v_code := SQLCODE;
        v_errm := SUBSTR(SQLERRM, 1, 64);
        DBMS_OUTPUT.PUT_LINE (v_code || ' ' || v_errm);

END;
/
