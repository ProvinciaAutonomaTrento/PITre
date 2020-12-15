create or replace FUNCTION GetMotivoADL (
    idoggetto      INT,
    typeid     CHAR,
    idgruppo   INT,
    idpeople   INT
) RETURN VARCHAR IS
    risultato   VARCHAR(200);
BEGIN --generale
    BEGIN --interno
        IF (typeid = 'D')
        THEN
            SELECT DISTINCT (VAR_MOTIVO) INTO risultato FROM dpa_area_lavoro
            WHERE id_profile = idoggetto AND (id_people = idpeople OR id_people = 0)
			AND id_ruolo_in_uo = (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = idgruppo);
        END IF;
        IF (typeid = 'F')
        THEN
            SELECT DISTINCT (VAR_MOTIVO) INTO risultato FROM dpa_area_lavoro
            WHERE id_project = idoggetto  AND (id_people = idpeople OR id_people = 0)
			AND id_ruolo_in_uo = (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = idgruppo);
        END IF;
    EXCEPTION
        WHEN no_data_found THEN
            risultato := '';
        WHEN OTHERS THEN
            risultato := '';
    END;
    RETURN risultato;
END GetMotivoADL;