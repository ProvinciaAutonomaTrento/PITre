create or replace
PROCEDURE sp_getcontatorealertcons
(
p_userid NUMBER, 
p_groupid NUMBER,
p_ammid NUMBER, 
p_operaz VARCHAR ,
p_risultato OUT NUMBER
)

-- 0: le condizioni per l'invio dell'alert non sono soddisfatte
-- 1: le condizioni per l'invio dell'alert sono soddisfatte
-- -1: errore nella funzione

IS
isPresente number;
contatore number;
intervallo number;
soglia number;
dta_fine_mon date;
sysid number;
BEGIN
-- 1.
-- esiste già un contatore per l'utente
-- per l'alert selezionato?
SELECT COUNT(*)
INTO isPresente
FROM dpa_alert_conservazione
WHERE
id_amm = p_ammid AND
id_people = p_userid AND
id_gruppo = p_groupid AND
var_codice = p_operaz;
-- 2a.
-- il contatore non c'è > creo il record
DBMS_OUTPUT.PUT_LINE('isPresente = ' || isPresente); -- REMOVE
IF(isPresente = 0) THEN
  SELECT seq_dpa_alert_conservazione.nextval
  INTO sysid
  FROM dual;
  
	INSERT INTO dpa_alert_conservazione
	(system_id, id_amm, id_people, id_gruppo, var_codice, num_operazioni, dta_inizio_monitoraggio)
	VALUES
	( sysid, p_ammid, p_userid, p_groupid, p_operaz, 1, SYSDATE);
	p_risultato := 0;
ELSE
	-- 2b.
	-- il contatore esiste
	-- controllo se il periodo di monitoraggio è terminato
	-- cioè se data_inizio_mon + intervallo è successiva alla
	-- data attuale
	IF(p_operaz = 'LEGGIBILITA_SINGOLO') THEN
		SELECT num_legg_sing_periodo_mon
		INTO intervallo
		FROM dpa_config_alert_cons
		WHERE id_amm=p_ammid;
	ELSIF(p_operaz = 'DOWNLOAD') THEN
		SELECT num_download_periodo_mon
		INTO intervallo
		FROM dpa_config_alert_cons
		WHERE id_amm=p_ammid;	
	ELSIF(p_operaz = 'SFOGLIA') THEN
		SELECT num_sfoglia_periodo_mon
		INTO intervallo
		FROM dpa_config_alert_cons
		WHERE id_amm=p_ammid;
	END IF;
	DBMS_OUTPUT.PUT_LINE('INTERVALLO = ' || intervallo); -- REMOVE
  
	SELECT dta_inizio_monitoraggio
	INTO dta_fine_mon
	FROM dpa_alert_conservazione
	WHERE id_amm = p_ammid AND
	id_people = p_userid AND
	id_gruppo = p_groupid AND
	var_codice = p_operaz;
	dta_fine_mon := dta_fine_mon + intervallo;
  DBMS_OUTPUT.PUT_LINE('data fine mon = ' || dta_fine_mon); -- REMOVE
	-- 3a.
	-- il periodo di monitoraggio è terminato
	-- inizializzo il contatore a 1 e alla data attuale
	IF (dta_fine_mon < SYSDATE) THEN
		UPDATE dpa_alert_conservazione
		SET
		num_operazioni=1,
		dta_inizio_monitoraggio=SYSDATE
		WHERE
		id_people = p_userid AND
		id_gruppo = p_groupid AND 
		id_amm = p_ammid AND
		var_codice = p_operaz;
		p_risultato := 0;
	ELSE
	-- 3b.
	-- il periodo di monitoraggio è in corso
	-- incremento il contatore e verifico se ha raggiunto la soglia
	-- per l'invio dell'alert
		IF(p_operaz = 'LEGGIBILITA_SINGOLO') THEN
			SELECT num_legg_sing_max_oper
			INTO soglia
			FROM dpa_config_alert_cons
			WHERE id_amm=p_ammid;
		ELSIF(p_operaz = 'DOWNLOAD') THEN
			SELECT num_download_max_oper
			INTO soglia
			FROM dpa_config_alert_cons
			WHERE id_amm=p_ammid;
		ELSIF(p_operaz = 'SFOGLIA') THEN
			SELECT num_sfoglia_max_oper
			INTO soglia
			FROM dpa_config_alert_cons
			WHERE id_amm=p_ammid;		
		END IF;
		
		UPDATE dpa_alert_conservazione
		SET
		num_operazioni = num_operazioni+1
		WHERE
		id_people = p_userid AND
		id_gruppo = p_groupid AND 
		id_amm = p_ammid AND
		var_codice = p_operaz;
	
		SELECT num_operazioni
		INTO contatore
		FROM dpa_alert_conservazione
		WHERE
		id_people = p_userid AND
		id_gruppo = p_groupid AND 
		id_amm = p_ammid AND
		var_codice = p_operaz;
		-- 4a.
		-- il contatore è inferiore alla soglia
		-- non invio alert
		IF(contatore<=soglia) THEN
			p_risultato := 0;
		ELSE
		-- 4b.
		-- è stata raggiunta la soglia
		-- azzero il contatore e aggiorno la data
			UPDATE dpa_alert_conservazione
			SET
			num_operazioni = 1,
			dta_inizio_monitoraggio = SYSDATE
			WHERE
			id_people = p_userid AND
			id_gruppo = p_groupid AND 
			id_amm = p_ammid AND
			var_codice = p_operaz;
			
			p_risultato := 1;
		END IF;
	END IF;
END IF;

EXCEPTION
WHEN OTHERS THEN
	p_risultato := -1;

END sp_getcontatorealertcons;
