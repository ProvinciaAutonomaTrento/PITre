/*
***                        ATTENZIONE                       ***
*** Script convertito da Oracle a SQL Server ma non testato ***
*** Testare prima di utilizzare                             ***
*/
ALTER TABLE dpa_supporto ADD
(
data_ultima_verifica_legg	DATE,
esito_ultima_verifica_legg	INTEGER,
verifiche_legg_effettuate	INTEGER,
data_prox_verifica_legg		DATE,
perc_verifica_legg			INTEGER
);