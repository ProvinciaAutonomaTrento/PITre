CREATE OR REPLACE PROCEDURE @db_user.SP_DELETE_CORR_ESTERNO (IDCorrGlobale IN NUMBER, liste IN NUMBER, ReturnValue OUT NUMBER)  IS
countDoc number; -- variabile usata per contenere il numero di documenti che hanno IL CORRISPONDENTE come
cha_tipo_urp VARCHAR2(1);
var_inLista VARCHAR2(1); -- valore 'N' (il corr non è presente in nessuna lista di sistribuzione), 'Y' altrimenti

countLista number;

BEGIN

select cha_tipo_urp INTO cha_tipo_urp from dpa_corr_globali where system_id = IDCorrGlobale;

var_inLista := 'N'; 

SELECT count(SYSTEM_ID) into countLista FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = IDCorrGlobale;

IF (countLista > 0) THEN 
BEGIN
IF (liste = 1) THEN

BEGIN

ReturnValue := 2;
RETURN;
END;
ELSE

BEGIN
var_inLista := 'Y';
END;

END IF;

END;
END IF;


SELECT count(ID_PROFILE) INTO countDoc  FROM DPA_DOC_ARRIVO_PAR WHERE ID_MITT_DEST = IDCorrGlobale;

IF (countDoc = 0) THEN
BEGIN

DELETE FROM DPA_CORR_GLOBALI WHERE  SYSTEM_ID = IDCorrGlobale;

EXCEPTION
WHEN OTHERS THEN
ReturnValue:=3;

END;
IF (ReturnValue=3) THEN
RETURN; --ESCO DALLA PROCEDURA

ELSE
BEGIN

ReturnValue:=0;

DELETE FROM DPA_T_CANALE_CORR WHERE  ID_CORR_GLOBALE = IDCorrGlobale;

IF(cha_tipo_urp != 'R') THEN

BEGIN

DELETE FROM DPA_DETT_GLOBALI WHERE  ID_CORR_GLOBALI = IDCorrGlobale;

EXCEPTION
WHEN OTHERS THEN
ReturnValue:=4;
END;

IF (ReturnValue=4) THEN

RETURN; -- ESCO DALLA PROCEDURA

ELSE

ReturnValue:=0; -- CANCELLAZIONE ANDATA A BUON FINE

END IF;

END IF;

IF (ReturnValue=0 AND liste = 0 AND var_inLista = 'Y')	THEN

BEGIN

DELETE FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = IDCorrGlobale;

EXCEPTION
WHEN OTHERS THEN
ReturnValue:=6;

END;

END IF;

END;
END IF;


ELSE

-- CASO 4 -  il corrispondente è  stato utilizzato come mitt/dest di protocolli
BEGIN
-- 4.1) disabilitazione del corrispondente
UPDATE DPA_CORR_GLOBALI SET DTA_FINE = SYSDATE() WHERE SYSTEM_ID = IDCorrGlobale;


EXCEPTION
WHEN OTHERS THEN
ReturnValue:=5;-- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).

END;

IF(ReturnValue=5) THEN

RETURN;

ELSE

ReturnValue:=1;	-- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 1).

END IF;



END IF;

END;
/