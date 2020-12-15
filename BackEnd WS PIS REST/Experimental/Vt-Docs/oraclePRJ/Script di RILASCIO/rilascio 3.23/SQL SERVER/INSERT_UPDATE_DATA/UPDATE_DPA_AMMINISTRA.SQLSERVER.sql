-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 22/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- UPDATE_DPA_AMMINISTRA
-- Attivazione Trasmissione Automatica
-- Se il campo spedizione automatica già presente e valorizzato con 1 anche il campo trasmissione automatica andrà valorizzato con 1 altrimenti di default  0
-- =============================================

--Uso un cursore nella dpa_amministra per prevedere una futura multiamministrazione.
 
DECLARE @V_SPEDIZIONE_AUTO_DOC		CHAR(1)
DECLARE @V_SYSTEM_ID				INT

DECLARE c1 CURSOR FOR	
SELECT SYSTEM_ID, SPEDIZIONE_AUTO_DOC 
FROM DOCSADM.DPA_AMMINISTRA

OPEN c1

FETCH NEXT FROM c1
INTO @V_SYSTEM_ID,@V_SPEDIZIONE_AUTO_DOC

WHILE @@FETCH_STATUS = 0
BEGIN

	IF (@V_SPEDIZIONE_AUTO_DOC = '1') 
	BEGIN
		UPDATE DOCSADM.DPA_AMMINISTRA
		SET TRASMISSIONE_AUTO_DOC = '1'
		WHERE SYSTEM_ID = @V_SYSTEM_ID
			AND TRASMISSIONE_AUTO_DOC IS NULL
	END 
	ELSE
	BEGIN
		UPDATE DOCSADM.DPA_AMMINISTRA
		SET TRASMISSIONE_AUTO_DOC = '0'
		WHERE SYSTEM_ID = @V_SYSTEM_ID
			AND TRASMISSIONE_AUTO_DOC IS NULL
	END 
	
	FETCH NEXT FROM c1
	INTO @V_SYSTEM_ID,@V_SPEDIZIONE_AUTO_DOC
       
END

CLOSE c1
DEALLOCATE c1