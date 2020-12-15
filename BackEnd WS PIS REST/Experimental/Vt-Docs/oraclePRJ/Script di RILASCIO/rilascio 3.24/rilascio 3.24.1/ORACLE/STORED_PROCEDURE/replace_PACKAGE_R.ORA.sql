begin 
	utl_backup_sp ('R','3.24');
end;
/

  
  CREATE OR REPLACE PACKAGE @db_user.R AS

  TYPE T_CURSOR IS REF CURSOR;
  
    -- Procedure per la gestione degli utenti
  PROCEDURE GetUserCredentials(pNomeUtente NVARCHAR2, pPassword NVARCHAR2, cur_OUT OUT T_CURSOR);
  
  PROCEDURE GetUsers(cur_OUT OUT T_CURSOR, pFiltro NVARCHAR2, pOrdinamento NVARCHAR2, pPagina INT, pOggettiPagina INT, pTotaleOggetti OUT INT);
  
  PROCEDURE GetUser(pId INTEGER, cur_OUT OUT T_CURSOR);

  PROCEDURE DeleteUser(pId INTEGER, pDataUltimaModifica DATE);
  
  PROCEDURE InsertUser(pNomeUtente NVARCHAR2, pPassword NVARCHAR2, pAmministratore CHAR, pDataCreazione DATE, pDataUltimaModifica DATE, pId OUT INTEGER);
  
  PROCEDURE UpdateUser(pId INTEGER, pAmministratore CHAR,pDataUltimaModifica DATE, pOldDataUltimaModifica DATE);
    
  PROCEDURE ChangeUserPassword(pNomeUtente NVARCHAR2, pPassword NVARCHAR2, pNewPassword NVARCHAR2);
 
  PROCEDURE ContainsUser(pNomeUtente NVARCHAR2, pRet OUT INTEGER);

 -- Procedure per la gestione degli elementi rubrica
  PROCEDURE GetElementiRubrica(cur_OUT OUT T_CURSOR, pFiltro NVARCHAR2, pOrdinamento NVARCHAR2, pPagina INT, pOggettiPagina INT, pTotaleOggetti OUT INT);

  PROCEDURE InsertElementoRubrica(pCodice NVARCHAR2, pDescrizione NVARCHAR2, 
                                    pIndirizzo NVARCHAR2, pCitta NVARCHAR2, pCap NVARCHAR2, 
                                    pProvincia NVARCHAR2, pNazione NVARCHAR2, 
                                  pTelefono NVARCHAR2, pFax NVARCHAR2, 
                                  pAOO NVARCHAR2,
                                  pDataCreazione DATE, pDataUltimaModifica DATE, 
                                  pUtenteCreatore NVARCHAR2, 
                                  pTipoCorrispondente NVarChar2,
                                  pAmministrazione Nvarchar2,
                                  pUrl Nvarchar2,
                                  pChaPubblica Nvarchar2,
                                  pId OUT INTEGER , pCodiceFiscale NVARCHAR2, pPartitaIva NVARCHAR2);

 PROCEDURE UpdateElementoRubrica(pId INTEGER, pDescrizione NVARCHAR2,
                                    pIndirizzo NVARCHAR2, 
                                     pCitta NVARCHAR2, pCap NVARCHAR2, pProvincia NVARCHAR2,
                                    pNazione NVARCHAR2, pTelefono NVARCHAR2, pFax NVARCHAR2, 
                                  pAOO NVARCHAR2,
                                  pDataUltimaModifica DATE, pOldDataUltimaModifica DATE, 
                                  pTipoCorrispondente NVARCHAR2, pAmministrazione Nvarchar2,
                                  pUrl Nvarchar2,
                                  pChaPubblica Nvarchar2 , pCodiceFiscale NVARCHAR2, pPartitaIva NVARCHAR2);
  
  PROCEDURE DeleteElementoRubrica(pId INTEGER, pDataUltimaModifica DATE); 
   
  PROCEDURE GetElementoRubrica(cur_OUT OUT T_CURSOR, pId IN INTEGER);
  
  PROCEDURE ContainsElementoRubrica(pCodice NVARCHAR2, pRet OUT INTEGER);
  
  PROCEDURE InsertAmministrazione(pCodice NVARCHAR2, pUrl nvarchar2, pId OUT INTEGER);
  
  PROCEDURE UpdateAmministrazione(pCodice NVARCHAR2, pUrl nvarchar2, pIdAmministrazione INTEGER);
  
  PROCEDURE InsertEmail(pId Number, pEmail nvarchar2, pNote nvarchar2, pPreferita Number);
  
  PROCEDURE RemoveEmails(pId Number);
  
  PROCEDURE GetEmails(cur_OUT OUT T_CURSOR, pId IN INTEGER);

END R;

/



