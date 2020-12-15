declare @ultimoID int

begin

Insert into [@db_user].DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ( 'AMM_LOGIN', 'Accesso Admin all''applicazione', 'UTENTE', 'AMM_LOGIN')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into [@db_user].DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)


Insert into [@db_user].DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ('AMM_LOGOFF', 'Uscita Admin dall''applicazione', 'UTENTE', 'AMM_LOGOFF')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into [@db_user].DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)

END
GO


Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
 Values      (getdate(), '3.12.10')
GO
