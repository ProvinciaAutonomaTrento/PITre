-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 22/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- Update_APPS
-- =============================================

--Update Apps	Set Mime_Type='text/plain'  where application in ('PUB XML','GEN_XML')  and Mime_Type <> 'text/plain';

UPDATE DOCSADM.APPS
SET Mime_Type = 'application/msword'
WHERE Application = 'MS WORD'
	AND Mime_Type !=  'application/msword'
