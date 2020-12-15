CREATE OR REPLACE FUNCTION getDiagrammiProcessiFirma ( processoid   INT ) RETURN VARCHAR IS
    risultato   CLOB;
    item        CLOB;
    CURSOR cur IS
        SELECT DISTINCT( d.var_descrizione )
        FROM dpa_diagrammi_stato d JOIN dpa_stati s ON d.system_id = s.id_diagramma
        WHERE s.id_processo_firma = processoid;

BEGIN
    risultato := '';
    OPEN cur;
    LOOP
        FETCH cur INTO item;
        EXIT WHEN cur%notfound;
        IF (risultato IS NOT NULL)
        THEN
            risultato := risultato || '; ' || item;
        ELSE
            risultato := risultato || item;
        END IF;
    END LOOP;
    RETURN risultato;
END getDiagrammiProcessiFirma;

