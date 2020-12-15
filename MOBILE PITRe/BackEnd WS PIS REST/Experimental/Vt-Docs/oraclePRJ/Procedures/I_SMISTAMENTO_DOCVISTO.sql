--------------------------------------------------------
--  DDL for Procedure I_SMISTAMENTO_DOCVISTO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."I_SMISTAMENTO_DOCVISTO" (
IDTrasmissioneUtenteMittente IN NUMBER,
TrasmissioneConWorkflow IN CHAR,
IDTrasmissione in NUMBER,
IDPeopleMittente IN NUMBER,
ReturnValue OUT NUMBER) IS

/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione del tasto "VISTO" nello smistamento: imposta il documento come VISTO e lo toglie
-- dalla lista delle COSE DA FARE (Todo-List).
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- 1: Altrimenti
-------------------------------------------------------------------------------------------------------
*/

TipoTrasmSingola CHAR(1) := NULL;

BEGIN
/* Aggiornamento trasmissione del mittente */
IF TrasmissioneConWorkflow = '1' THEN
BEGIN
UPDATE  DPA_TRASM_UTENTE
SET  DTA_VISTA = SYSDATE(),
CHA_VISTA = '1',
DTA_ACCETTATA = SYSDATE(),
CHA_ACCETTATA = '1',
CHA_IN_TODOLIST = '0'
WHERE SYSTEM_ID = IDTrasmissioneUtenteMittente;
END;
ELSE
BEGIN
UPDATE  DPA_TRASM_UTENTE
SET  DTA_VISTA = SYSDATE(),
CHA_VISTA = '1',
CHA_IN_TODOLIST = '0'
WHERE SYSTEM_ID = IDTrasmissioneUtenteMittente;
END;
END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
BEGIN
SELECT    CHA_TIPO_TRASM INTO TipoTrasmSingola
FROM     DPA_TRASM_SINGOLA
WHERE     SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID = IDTrasmissioneUtenteMittente);
END;

IF TipoTrasmSingola = 'S' AND TrasmissioneConWorkflow = '1' THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
BEGIN
UPDATE  DPA_TRASM_UTENTE
SET  CHA_VALIDA = '0',
CHA_IN_TODOLIST = '0'
WHERE ID_TRASM_SINGOLA = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID = IDTrasmissioneUtenteMittente)
AND  SYSTEM_ID NOT IN (IDTrasmissioneUtenteMittente);
END;
END IF;

ReturnValue := 0;

END; 

/
