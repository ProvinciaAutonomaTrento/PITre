/*
***                        ATTENZIONE                       ***
*** Script convertito da Oracle a SQL Server ma non testato ***
*** Testare prima di utilizzare                             ***
*/
CREATE TABLE DPA_ALERT_CONSERVAZIONE
(
SYSTEM_ID				INT IDENTITY(1,1) NOT NULL,
ID_AMM					INT,
ID_PEOPLE				INT,
ID_GRUPPO				INT,
VAR_CODICE				VARCHAR(250 BYTE),
NUM_OPERAZIONI			INT,
DTA_INIZIO_MONITORAGGIO	DATE
);