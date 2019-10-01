create or replace 
FUNCTION           CountAllegati_StampaReg (Id INT)
RETURN number IS risultato number;

BEGIN
begin

SELECT COUNT(SYSTEM_ID) INTO risultato
FROM PROFILE P
WHERE P.ID_DOCUMENTO_PRINCIPALE = Id;


EXCEPTION
WHEN OTHERS THEN
risultato:=-1;
end;
RETURN risultato;
END CountAllegati_StampaReg;
