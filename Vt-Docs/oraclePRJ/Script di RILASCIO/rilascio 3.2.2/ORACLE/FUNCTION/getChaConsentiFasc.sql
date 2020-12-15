create or replace
FUNCTION getChaConsentiFasc (p_id_parent number, p_cha_tipo_proj varchar2, p_cha_tipo_fascicolo varchar2, p_id_fascicolo number)
RETURN VARCHAR2
IS
risultato   VARCHAR2 (16);
BEGIN
	BEGIN
		risultato := '1';
		if(p_cha_tipo_proj = 'F' AND p_cha_tipo_fascicolo = 'P')
			then
				select cha_consenti_fasc into risultato from project where system_id = p_id_parent;
		end if;
	END;
	RETURN risultato;
END getChaConsentiFasc; 