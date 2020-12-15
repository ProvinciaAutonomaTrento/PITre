-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 06/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- GETSTATODELEGA : ritorna lo stato della delega in base ai valori di data_scadenza e data_decorrenza
--	se data_scadenza < sysdate						--> lo stato  'SCADUTA'
--	se sysdate < data_decorrenza < data_scadenza		--> lo stato  'IMPOSTATA'
--	se data_decorrenza < sysdate < data_scadenza		--> lo stato  'ATTIVA'
-- =============================================


CREATE FUNCTION [DOCSADM].[GETSTATODELEGA]
(
	@ID_DELEGA	INT
)
RETURNS VARCHAR(128)
AS
BEGIN
	DECLARE @STATO_DELEGA	INT

	SELECT @STATO_DELEGA = (CASE WHEN ISNULL(DATA_SCADENZA,GETDATE()+1) <= GETDATE() THEN 'SCADUTA'
								ELSE CASE WHEN DATA_DECORRENZA IS NULL THEN 'DECORRENZA NULLA'
										WHEN DATA_DECORRENZA > GETDATE() THEN 'IMPOSTATA'
										WHEN DATA_DECORRENZA <= GETDATE() THEN 'ATTIVA'
										ELSE 'CASO NON PREVISTO'
									END
							END)
	FROM DOCSADM.DPA_DELEGHE
	WHERE SYSTEM_ID = @ID_DELEGA
	
	RETURN @STATO_DELEGA
END 

