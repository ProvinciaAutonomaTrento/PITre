/*
Palumbo Ch
 lunedì 30 maggio 2011 15:38
 funzionalità di smistamento: in test filtrava le ragioni
 e quindi nella funzionalità di smistamento avevamo meno trasmissioni rispetto alla TDL
*/
if exists(select * from syscolumns where name='cha_cede_diritti' and id in
		(select id from sysobjects where name='dpa_ragione_trasm' and xtype='U'))
BEGIN		   
   UPDATE @db_user.dpa_ragione_trasm
      SET cha_cede_diritti = 'N'
    WHERE cha_cede_diritti IS NULL
END
GO


/*
AUTORE:						Veltri F.
Data creazione:				23/06/2011
Scopo dell'inserimento:		
Il tasto rimuovi funziona soltanto con i documenti grigi 
ricevuti per interoperabilità e ancora non protocollati.
L’icona rimuovi è visibile soltanto per i documenti interessati e non gli altri

*/

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] 
where COD_FUNZIONE='DO_TODOLIST_RIMUOVI'))
BEGIN
	INSERT INTO [@db_user].DPA_ANAGRAFICA_FUNZIONI 
	VALUES('DO_TODOLIST_RIMUOVI'
	, 'Abilita il pulsante di rimozione documento dalla todolist nel caso di documenti ricevuti per interoperabilità'
	, '', 'N');
END 
GO


IF  EXISTS (
			SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[gettestoultimanota]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[gettestoultimanota] 
go

CREATE FUNCTION [@db_user].[gettestoultimanota](@p_TIPOOGGETTOASSOCIATO varchar	-- accetta valori F o D
, @p_IDOGGETTOASSOCIATO int
, @p_ID_RUOLO_IN_UO int
, @p_IDUTENTECREATORE int
, @p_IDRUOLOCREATORE int)
RETURNS varchar(2000)						-- ritorna -1 in caso di errore
AS
-- =============================================
-- Author:		P. De Luca
-- Create date: 15 giu 2011
-- Description:	ritorna il testo della nota più recente associata al documento  o al fascicolo
-- =============================================

BEGIN
	DECLARE @ultimanota varchar(2000)

IF (@p_TIPOOGGETTOASSOCIATO  <> 'F' AND @p_TIPOOGGETTOASSOCIATO  <> 'D')
		 BEGIN
			   SET @ultimanota=-1
			   RETURN @ultimanota
		 END

IF @p_TIPOOGGETTOASSOCIATO = 'F' -- join con la project
	BEGIN
	SELECT      TOP 1  @ultimanota=N.TESTO 
        FROM    DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN PROJECT PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID 
      WHERE   
      N.TIPOOGGETTOASSOCIATO = @p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO = @p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' 
      OR
		(N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
			(select id_registro from dpa_l_ruolo_reg rr where  rr.ID_RUOLO_IN_UO = @p_ID_RUOLO_IN_UO)) 
      OR
		(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @p_IDUTENTECREATORE) 
	  OR
		(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @p_IDRUOLOCREATORE))
      ORDER BY N.DATACREAZIONE DESC
	END

IF (@@ERROR <> 0)
		 BEGIN
		     SET @ultimanota=-1
			 RETURN @ultimanota
		 END


IF @p_TIPOOGGETTOASSOCIATO = 'D' -- join con la profile
	BEGIN
	SELECT      TOP 1  @ultimanota=N.TESTO 
        FROM    DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN PROFILE PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID 
      WHERE   
      N.TIPOOGGETTOASSOCIATO = @p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO = @p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' OR
      (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
      (select id_registro from dpa_l_ruolo_reg rr where  rr.ID_RUOLO_IN_UO = @p_ID_RUOLO_IN_UO)) OR
      (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = @p_IDUTENTECREATORE) OR
      (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = @p_IDRUOLOCREATORE))
      ORDER BY N.DATACREAZIONE DESC
	END

IF (@@ERROR <> 0)
		 BEGIN
			   SET @ultimanota=-1
			   RETURN @ultimanota
		 END
RETURN @ultimanota

-- The last statement included within a function must be a return statement.

END
GO

/*
AUTORE :						Frezza S.
Data creazione:				01/07/2011
Scopo dell'inserimento:		
							Abilita il pulsante della firma di un documento 
							acquisito o presente sul file-system tramite algoritmo SHA256
*/

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] 
where COD_FUNZIONE='DO_DOC_FIRMA_256'))
BEGIN
INSERT INTO [@db_user].DPA_ANAGRAFICA_FUNZIONI
(   COD_FUNZIONE,
    VAR_DESC_FUNZIONE,
    CHA_TIPO_FUNZ,
    DISABLED
) VALUES ( 
   'DO_DOC_FIRMA_256',
    'Abilita il pulsante della firma di un documento acquisito o presente sul file-system tramite algoritmo SHA256',
    NULL,
    'N')  
END 
GO


/*
AUTORE:						Veltri F.
Data creazione:				23/06/2011
Scopo dell'inserimento:		
Il tasto rimuovi funziona soltanto con i documenti grigi 
ricevuti per interoperabilità e ancora non protocollati.
L’icona rimuovi è visibile soltanto per i documenti interessati e non gli altri

*/

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_FUNZIONI] 
where COD_FUNZIONE='DO_TODOLIST_RIMUOVI'))
BEGIN
	INSERT INTO [@db_user].DPA_ANAGRAFICA_FUNZIONI 
	VALUES('DO_TODOLIST_RIMUOVI'
	, 'Abilita il pulsante di rimozione documento dalla todolist nel caso di documenti ricevuti per interoperabilità'
	, '', 'N');
END 
GO


Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
Values
   (getdate(), '3.12.9')
GO

