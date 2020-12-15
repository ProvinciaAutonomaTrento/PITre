-- file sql di update per il CD -- 
---- dpa_chiavi_configurazione.MSSQL.sql  marcatore per ricerca ----
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC'))
BEGIN	
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           ([ID_AMM]
           ,[VAR_CODICE]
           ,[VAR_DESCRIZIONE]
           ,[VAR_VALORE]
           ,[CHA_TIPO_CHIAVE]
           ,[CHA_VISIBILE]
           ,[CHA_MODIFICABILE]
           ,[CHA_GLOBALE]
           ,[VAR_CODICE_OLD_WEBCONFIG])
     VALUES (0
           ,'BE_RIC_MITT_INTEROP_BY_MAIL_DESC'
           ,'ATTIVA LA RICERCA DEL MITTENTE PER INTEROPERABILITA PER DESCRIZIONE E MAIL ANZICHE SOLO MAIL. VALORI POSSIBILI 0 o 1'
           ,'1'
           ,'B'
           ,'1'
           ,'1'
           ,'1'
           ,'')

END
GO               
----------- FINE -
              
---- getValCampoProfDoc.MSSQL.sql  marcatore per ricerca ----
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[getValCampoProfDoc]')
AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
DROP FUNCTION [@db_user].[getValCampoProfDoc]
GO
CREATE FUNCTION @db_user.getValCampoProfDoc
(@DocNumber INT, @CustomObjectId INT)

RETURNS VARCHAR(4000) AS
BEGIN

   DECLARE @result VARCHAR(255)



   DECLARE @tipoOggetto VARCHAR(255)
   DECLARE @tipoCont VARCHAR(1)
   DECLARE @error INT
   DECLARE @no_data_err INT



   select   @tipoOggetto = b.descrizione, @tipoCont = cha_tipo_Tar
   from
   @db_user.dpa_oggetti_custom a, @db_user.dpa_tipo_oggetto b
   where
   a.system_id = @CustomObjectId
   and
   a.id_tipo_oggetto = b.system_id
   SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
   IF (@error = 0  and  @no_data_err <> 0)
   begin
      if (@tipoOggetto = 'Corrispondente')
      begin
         select   @result = cg.var_cod_rubrica+' - '+cg.var_DESC_CORR
         from @db_user.dpa_CORR_globali cg where cg.SYSTEM_ID =
         (select valore_oggetto_db from @db_user.dpa_associazione_templates 
         where id_oggetto = @CustomObjectId and doc_number = STR(@DocNumber))
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end
   else if (@tipoOggetto = 'CasellaDiSelezione')
      BEGIN
         DECLARE @item VARCHAR(255)
         DECLARE @curCasellaDiSelezione CURSOR
         SET @curCasellaDiSelezione = CURSOR  FOR select  valore_oggetto_db  from @db_user.dpa_associazione_templates where id_oggetto = @CustomObjectId and doc_number = STR(@DocNumber) and valore_oggetto_db is not null
         OPEN @curCasellaDiSelezione
         while 1 = 1
         begin
            FETCH @curCasellaDiSelezione INTO @item
            if @@FETCH_STATUS <> 0
            BREAK
            IF(@result IS NOT NULL)
               SET @result = @result+'; '+@item 
         ELSE
            SET @result = @result+@item
         end
         CLOSE @curCasellaDiSelezione
      END
   else if (@tipoOggetto = 'Contatore')
      begin
         select   @result = @db_user.getContatoreDoc(@DocNumber,@tipoCont) 
         from @db_user.dpa_associazione_templates 
         where id_oggetto = @CustomObjectId and doc_number = @DocNumber
      end
   else 

--Tutti gli altri

      begin
         select   @result = valore_oggetto_db from @db_user.dpa_associazione_templates 
         where id_oggetto = @CustomObjectId and doc_number = STR(@DocNumber)
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end

      IF (@error = 0  and @no_data_err <> 0)
      RETURN @result
   end
   IF (@error <> 0)
   begin
      SET @result = 'Errore: '+str(@error)
      RETURN @result
   end
ELSE IF (@no_data_err = 0)
   begin
      SET @result = null --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 

      RETURN @result
   end
   RETURN 0
END 
GO              
----------- FINE -
              
---- getValCampoProfDocOrder.MSSQL.sql  marcatore per ricerca ----
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[getValCampoProfDocOrder]')
AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
DROP FUNCTION [@db_user].[getValCampoProfDocOrder]
GO

CREATE FUNCTION @db_user.getValCampoProfDocOrder
(@DocNumber INT, @CustomObjectId INT)
RETURNS VARCHAR(4000) AS
BEGIN

   DECLARE @result VARCHAR(255)

   DECLARE @tipoOggetto VARCHAR(255)
   DECLARE @error INT
   DECLARE @no_data_err INT

   select   @tipoOggetto = b.descrizione
   from
   @db_user.dpa_oggetti_custom a, @db_user.dpa_tipo_oggetto b
   where
   a.system_id = @CustomObjectId
   and
   a.id_tipo_oggetto = b.system_id
   SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
   IF (@error = 0  and  @no_data_err <> 0)
   begin
      if (@tipoOggetto = 'Corrispondente')
      begin
         select   @result = cg.var_cod_rubrica+' - '+cg.var_DESC_CORR
         from @db_user.dpa_CORR_globali cg 
         where cg.SYSTEM_ID =
         (select valore_oggetto_db from @db_user.dpa_associazione_templates 
         where id_oggetto = @CustomObjectId and doc_number = STR(@DocNumber)
		 )
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end
   else 
   --Casella di selezione (Per la casella di selezione serve un caso particolare perch i valori sono multipli)
   if(@tipoOggetto = 'CasellaDiSelezione')
      BEGIN
         DECLARE @item VARCHAR(255)
         DECLARE @curCasellaDiSelezione CURSOR
         SET @curCasellaDiSelezione = CURSOR  FOR 
         select  valore_oggetto_db  from dpa_associazione_templates 
         where id_oggetto = @CustomObjectId and doc_number = STR(@DocNumber) 
         and valore_oggetto_db is not null
         OPEN @curCasellaDiSelezione
         while 1 = 1
         begin
            FETCH @curCasellaDiSelezione INTO @item
            if @@FETCH_STATUS <> 0
            BREAK
            IF(@result IS NOT NULL)
               SET @result = @result+'; '+@item 
         ELSE
            SET @result = @result+@item
         end
         CLOSE @curCasellaDiSelezione
      END
   else 
--Tutti gli altri
      begin
         select   @result = valore_oggetto_db from @db_user.dpa_associazione_templates 
         where id_oggetto = @CustomObjectId and doc_number = @DocNumber
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end

      IF (@error = 0  and @no_data_err <> 0)
      RETURN @result
   end
   IF (@error <> 0)
   begin
      SET @result = 'Errore: '+str(@error)
      RETURN @result
   end
ELSE IF (@no_data_err = 0)
   begin
      SET @result = null --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 
      RETURN @result
   end
   RETURN 0

END 

GO              
----------- FINE -
              
---- GetValProfObjPrj.MSSQL.sql  marcatore per ricerca ----
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[GetValProfObjPrj]')
AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
DROP FUNCTION [@db_user].[GetValProfObjPrj]
GO
CREATE FUNCTION @db_user.GetValProfObjPrj(@PrjId INT, @CustomObjectId INT)

RETURNS VARCHAR(4000) AS
BEGIN

   DECLARE @result VARCHAR(255)
   DECLARE @tipoOggetto VARCHAR(255)
   DECLARE @tipoCont VARCHAR(1)
   DECLARE @error INT
   DECLARE @no_data_err INT



   select   @tipoOggetto = b.descrizione, @tipoCont = cha_tipo_Tar
   from
   @db_user.dpa_oggetti_custom_fasc a, @db_user.dpa_tipo_oggetto_fasc b
   where
   a.system_id = @CustomObjectId
   and
   a.id_tipo_oggetto = b.system_id
   SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
   IF (@error = 0  and  @no_data_err <> 0)
   begin
      if (@tipoOggetto = 'Corrispondente')
      begin
         select   @result = cg.var_cod_rubrica+' - '+cg.var_DESC_CORR
         from @db_user.dpa_CORR_globali cg where cg.SYSTEM_ID =
         (select valore_oggetto_db from @db_user.dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = STR(@PrjId))
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end
   else if (@tipoOggetto = 'CasellaDiSelezione')
      BEGIN
         DECLARE @item VARCHAR(255)
         DECLARE @curCasellaDiSelezione CURSOR
         SET @curCasellaDiSelezione = CURSOR  FOR 
         select valore_oggetto_db  from @db_user.dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = STR(@PrjId) 
         and valore_oggetto_db is not null
         OPEN @curCasellaDiSelezione
         while 1 = 1
         begin
            FETCH @curCasellaDiSelezione INTO @item
            if @@FETCH_STATUS <> 0
            BREAK
            IF(@result IS NOT NULL)
               SET @result = @result+'; '+@item 
         ELSE
            SET @result = @result+@item
         end
         CLOSE @curCasellaDiSelezione
      END
   else if (@tipoOggetto = 'Contatore')
      begin
         select   @result = @db_user.getContatoreFasc(@PrjId,@tipoCont) 
         from @db_user.dpa_ass_templates_fasc where id_oggetto = @CustomObjectId 
         and id_project = @PrjId
      end
   else 

--Tutti gli altri

      begin
         select   @result = valore_oggetto_db from @db_user.dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = STR(@PrjId)
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end

      IF (@error = 0  and @no_data_err <> 0)
      RETURN @result
   end
   IF (@error <> 0)
   begin
      SET @result = 'Errore: '+str(@error)
      RETURN @result
   end
ELSE IF (@no_data_err = 0)
   begin
      SET @result = null --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber;  

      RETURN @result
   end
   RETURN 0

END 
GO

              
----------- FINE -
              
---- GetValProfObjPrjOrder.MSSQL.sql  marcatore per ricerca ----

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@db_user].[GetValProfObjPrjOrder]')
AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
DROP FUNCTION [@db_user].[GetValProfObjPrjOrder]
GO
CREATE FUNCTION @db_user.GetValProfObjPrjOrder
(@PrjId INT, @CustomObjectId INT)
RETURNS VARCHAR(4000) AS
BEGIN

   DECLARE @result VARCHAR(255)

   DECLARE @tipoOggetto VARCHAR(255)
   DECLARE @error INT
   DECLARE @no_data_err INT

   select   @tipoOggetto = b.descrizione
   from
   @db_user.dpa_oggetti_custom_fasc a, @db_user.dpa_tipo_oggetto_fasc b
   where
   a.system_id = @CustomObjectId
   and
   a.id_tipo_oggetto = b.system_id
   SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
   IF (@error = 0  and  @no_data_err <> 0)
   begin
      if (@tipoOggetto = 'Corrispondente')
      begin
         select   @result = cg.var_cod_rubrica+' - '+cg.var_DESC_CORR
         from @db_user.dpa_CORR_globali cg where cg.SYSTEM_ID =
         (select valore_oggetto_db from @db_user.dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = @PrjId)
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end
   else if(@tipoOggetto = 'CasellaDiSelezione')
      BEGIN
         DECLARE @item VARCHAR(255)
         DECLARE @curCasellaDiSelezione CURSOR
         SET @curCasellaDiSelezione = CURSOR  FOR 
         select valore_oggetto_db  from dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = STR(@PrjId) and valore_oggetto_db is not null
         OPEN @curCasellaDiSelezione
         while 1 = 1
         begin
            FETCH @curCasellaDiSelezione INTO @item
            if @@FETCH_STATUS <> 0
            BREAK
            IF(@result IS NOT NULL)
               SET @result = @result+'; '+@item 
         ELSE
            SET @result = @result+@item
         end
         CLOSE @curCasellaDiSelezione
      END
   else 
--Tutti gli altri
      begin
         select   @result = valore_oggetto_db from @db_user.dpa_ass_templates_fasc 
         where id_oggetto = @CustomObjectId and id_project = @PrjId
         SELECT   @error = @@ERROR , @no_data_err = @@ROWCOUNT
      end

      IF (@error = 0  and @no_data_err <> 0)
      RETURN @result
   end
   IF (@error <> 0)
   begin
      SET @result = 'Errore: '+str(@error)
      RETURN @result
   end
ELSE IF (@no_data_err = 0)
   begin
      SET @result = null --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber;  
      RETURN @result
   end
   RETURN 0

END 

GO              
----------- FINE -
              
---- insert_DPA_DOCSPA.MSSQL.sql  marcatore per ricerca ----
Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
 Values      (getdate(), '3.16.9')
GO

              
