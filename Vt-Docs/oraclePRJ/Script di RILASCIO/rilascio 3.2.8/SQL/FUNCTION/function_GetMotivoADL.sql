create FUNCTION GetMotivoADL (
    @idoggetto      INT,
    @typeid     CHAR,
    @idgruppo   INT,
    @idpeople   INT
) RETURNS VARCHAR(200) AS
BEGIN
    DECLARE @risultato   VARCHAR(200)
    BEGIN --interno
	set @risultato = ''
        IF (@typeid = 'D')
        BEGIN
            SET @risultato = (SELECT DISTINCT TOP (1) (VAR_MOTIVO) FROM dpa_area_lavoro
            WHERE id_profile = @idoggetto AND (id_people = @idpeople OR id_people = 0)
			AND id_ruolo_in_uo = (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = @idgruppo))
        END
        IF (@typeid = 'F')
        BEGIN
            SET @risultato = (SELECT DISTINCT TOP (1) (VAR_MOTIVO) FROM dpa_area_lavoro
            WHERE id_project = @idoggetto  AND (id_people = @idpeople OR id_people = 0)
			AND id_ruolo_in_uo = (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = @idgruppo))
        END 
    END
    RETURN @risultato
END 