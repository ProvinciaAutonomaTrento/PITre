/*
***                        ATTENZIONE                       ***
*** Script convertito da Oracle a SQL Server ma non testato ***
*** Testare prima di utilizzare                             ***
*/

CREATE PROCEDURE sp_getcontatorealertcons
(
@p_userid INTEGER, 
@p_groupid INTEGER,
@p_ammid INTEGER, 
@p_operaz VARCHAR ,
@p_risultato OUT INTEGER
)

-- 0: le condizioni per l'invio dell'alert non sono soddisfatte
-- 1: le condizioni per l'invio dell'alert sono soddisfatte
-- -1: errore nella funzione
BEGIN

DECLARE
	@isPresente INTEGER;
	@contatore INTEGER;
	@intervallo INTEGER;
	@soglia INTEGER;
	@dta_fine_mon DATE;
	--@sysid INTEGER;

BEGIN TRY
-- 1.
-- esiste già un contatore per l'utente
-- per l'alert selezionato?
SELECT @isPresente = COUNT(*)
FROM dpa_alert_conservazione
WHERE
id_amm = @p_ammid AND
id_people = @p_userid AND
id_gruppo = @p_groupid AND
var_codice = @p_operaz;
-- 2a.
-- il contatore non c'è > creo il record
IF(@isPresente = 0) 
	BEGIN
		INSERT INTO dpa_alert_conservazione
		( id_amm, id_people, id_gruppo, var_codice, num_operazioni, dta_inizio_monitoraggio)
		VALUES
		( @p_ammid, @p_userid, @p_groupid, @p_operaz, 1, GETDATE());
		
		SET @p_risultato = 0;
	END;
ELSE
	BEGIN
	-- 2b.
	-- il contatore esiste
	-- controllo se il periodo di monitoraggio è terminato
	-- cioè se data_inizio_mon + intervallo è successiva alla
	-- data attuale
		SELECT @intervallo = CASE
			WHEN @p_operaz = 'LEGGIBILITA_SINGOLO' THEN num_legg_sing_periodo_mon
			WHEN @p_operaz = 'DOWNLOAD' THEN num_download_periodo_mon
			WHEN @p_operaz = 'SFOGLIA' THEN num_sfoglia_periodo_mon
			END
		FROM dpa_config_alert_cons
		WHERE id_amm=@p_ammid;
  
		SELECT @dta_fine_mon = dta_inizio_monitoraggio
		FROM dpa_alert_conservazione
		WHERE id_amm = @p_ammid AND
		id_people = @p_userid AND
		id_gruppo = @p_groupid AND
		var_codice = @p_operaz;
	
		SET @dta_fine_mon = DATEADD(day,@intervallo,@dta_fine_mon);
		
		-- 3a.
		-- il periodo di monitoraggio è terminato
		-- inizializzo il contatore a 1 e alla data attuale
		--IF (@dta_fine_mon < GETDATE())
		IF (DATEDIFF(day,@dta_fine_mon,GETDATE()) >= 0)
			BEGIN
				UPDATE dpa_alert_conservazione
				SET
				num_operazioni=1,
				dta_inizio_monitoraggio=GETDATE()
				WHERE
				id_people = @p_userid AND
				id_gruppo = @p_groupid AND 
				id_amm = @p_ammid AND
				var_codice = @p_operaz;
				
				SET @p_risultato = 0;
			END;
		ELSE
			BEGIN
			-- 3b.
			-- il periodo di monitoraggio è in corso
			-- incremento il contatore e verifico se ha raggiunto la soglia
			-- per l'invio dell'alert
				SELECT @soglia = CASE
					WHEN @p_operaz = 'LEGGIBILITA_SINGOLO' THEN num_legg_sing_max_oper
					WHEN @p_operaz = 'DOWNLOAD' THEN num_download_max_oper
					WHEN @p_operaz = 'SFOGLIA' THEN num_sfoglia_max_oper END
				FROM dpa_config_alert_cons
				WHERE id_amm=@p_ammid;
		
				UPDATE dpa_alert_conservazione
				SET
				num_operazioni = num_operazioni+1
				WHERE
				id_people = @p_userid AND
				id_gruppo = @p_groupid AND 
				id_amm = @p_ammid AND
				var_codice = @p_operaz;
	
				SELECT @contatore = num_operazioni
				FROM dpa_alert_conservazione
				WHERE
				id_people = @p_userid AND
				id_gruppo = @p_groupid AND 
				id_amm = @p_ammid AND
				var_codice = @p_operaz;
				
				-- 4a.
				-- il contatore è inferiore alla soglia
				-- non invio alert
				IF(@contatore<=@soglia)
					SET @p_risultato = 0;
				ELSE 
					BEGIN
					-- 4b.
					-- è stata raggiunta la soglia
					-- azzero il contatore e aggiorno la data
						UPDATE dpa_alert_conservazione
						SET
						num_operazioni = 1,
						dta_inizio_monitoraggio = GETDATE()
						WHERE
						id_people = @p_userid AND
						id_gruppo = @p_groupid AND 
						id_amm = @p_ammid AND
						var_codice = @p_operaz;
						
						SET @p_risultato = 1;
					END
			END
	END
END TRY;

BEGIN CATCH
	SET @p_risultato = -1;
END CATCH;

END sp_getcontatorealertcons;
