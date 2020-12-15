

/****** Object:  StoredProcedure [DOCSADM].[ins_occ_2]    Script Date: 04/18/2013 23:56:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ins_occ_2]') AND type in (N'P', N'PC'))
DROP PROCEDURE [DOCSADM].[ins_occ_2]
GO



/****** Object:  StoredProcedure [DOCSADM].[ins_occ_2]    Script Date: 04/18/2013 23:56:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [DOCSADM].[ins_occ_2]
(@id_reg int, @idamm int, @Prefix_cod_rub varchar (128), 
@desc_corr varchar (128),
@cha_dettagli varchar (1),
@ID_Corr_Globali  varchar (128),
@EMAIL varchar (128), @RESULT INT OUT)

as

BEGIN
declare @sys_dpa_corr_globali int
declare @myprofile int
declare @Countprofilato int
declare @Countprofdoc int
declare @Countproffasc int
declare @new_var_cod_rubrica1 varchar

set @sys_dpa_corr_globali = @ID_Corr_Globali
	-- verifica preesistenza dell'occ corrente
	-- per sviluppi futuri...

--	select top 1 @sys_dpa_corr_globali = system_id from dpa_corr_globali 
--	where upper (var_desc_corr) = upper (@desc_corr) and cha_tipo_corr = 'O' AND ID_AMM=@IDAMM
	-- ritorno il system_id già presente

	if (@sys_dpa_corr_globali <> 0) 
	BEGIN
		SET @RESULT = @sys_dpa_corr_globali 
	
	
	--inizio modifica
		
	--BEGIN
		SELECT @myprofile = COUNT (id_profile)
		FROM dpa_doc_arrivo_par
		Where Id_Mitt_Dest = @ID_Corr_Globali;
		-- verifico se il corrispondente  stato usato o meno nei campi profilati
		Select @Countprofdoc= Count(System_Id) 
		From Dpa_Associazione_Templates 
		where valore_oggetto_db = convert(varchar,@ID_Corr_Globali)
	          
		Select @Countproffasc = Count(System_Id)  
		from dpa_ass_templates_fasc
		where valore_oggetto_db = convert(varchar,@ID_Corr_Globali)
	      
		set @Countprofilato = @Countprofdoc + @Countproffasc
	
	END	
	
	IF(@myprofile = 0 AND @Countprofilato = 0)
	
	--else
	BEGIN

		if(@id_reg=0)
		begin
		set @id_reg=null

			--inserisco il nuovo occ_
				INSERT INTO 
				[docsadm].DPA_CORR_GLOBALI (ID_REGISTRO,
				ID_AMM,VAR_DESC_CORR,
				ID_OLD,DTA_INIZIO,ID_PARENT
				,CHA_TIPO_CORR,CHA_DETTAGLI,VAR_EMAIL)
				VALUES (@id_reg,@idamm,@desc_corr,0,getdate(),0,'O',0,@EMAIL)

				SET @RESULT = SCOPE_IDENTITY()
				
				UPDATE [docsadm].DPA_CORR_GLOBALI SET 
				VAR_COD_RUBRICA=@Prefix_cod_rub+convert(nvarchar(10),@RESULT) ,
				VAR_CODICE=@Prefix_cod_rub+convert(nvarchar(10),@RESULT)
				WHERE SYSTEM_ID=@RESULT
		end
		else
		
				INSERT INTO 
				[docsadm].DPA_CORR_GLOBALI (
				ID_REGISTRO,
				ID_AMM,
				VAR_DESC_CORR,
				ID_OLD,
				DTA_INIZIO,
				ID_PARENT,
				CHA_TIPO_CORR,
				CHA_DETTAGLI,
				VAR_EMAIL)
				VALUES (@id_reg,@idamm,@desc_corr,0,getdate(),0,'O',0,@EMAIL)

				SET @RESULT = SCOPE_IDENTITY()
				
				UPDATE [docsadm].DPA_CORR_GLOBALI SET 
				VAR_COD_RUBRICA=@pREFIX_COD_RUB+convert(nvarchar(10),@RESULT) ,
				VAR_CODICE=@pREFIX_COD_RUB+convert(nvarchar(10),@RESULT)
				WHERE SYSTEM_ID=@RESULT
		 
	end
		else
		begin
		
				Select @new_var_cod_rubrica1 = Var_Cod_Rubrica,@id_reg = id_registro, @idamm= id_amm
				FROM dpa_corr_globali
				WHERE system_id =@ID_Corr_Globali;
		        
				-- Costruisco il codice rubrica da attribuire al corrispondente storicizzato
				set @new_var_cod_rubrica1 += @new_var_cod_rubrica1 + '_' + convert(varchar,@ID_Corr_Globali)
				
				UPDATE [docsadm].DPA_CORR_GLOBALI SET 
				DTA_FINE=GETDATE(),
				VAR_COD_RUBRICA=@new_var_cod_rubrica1,
				VAR_CODICE= @new_var_cod_rubrica1,
                Id_Parent = Null
				WHERE SYSTEM_ID=@ID_Corr_Globali

				--UPDATE [docsadm].DPA_CORR_GLOBALI SET 
				--VAR_COD_RUBRICA=@pREFIX_COD_RUB+convert(nvarchar(10),@RESULT) ,
				--VAR_CODICE=@pREFIX_COD_RUB+convert(nvarchar(10),@RESULT)
				--WHERE SYSTEM_ID=@RESULT
				
		-- sab SET @RESULT = SCOPE_IDENTITY()
         
         INSERT INTO 
				[docsadm].DPA_CORR_GLOBALI (
				ID_REGISTRO,
				ID_AMM,
				
				VAR_DESC_CORR,
				ID_OLD,
				DTA_INIZIO,
				ID_PARENT,
				
				CHA_TIPO_CORR,
				CHA_DETTAGLI,
				VAR_EMAIL)
				VALUES (@id_reg,@idamm,@desc_corr,@ID_Corr_Globali,getdate(),0,'O',0,@EMAIL)
		
		SET @RESULT = SCOPE_IDENTITY()
				
				UPDATE [docsadm].DPA_CORR_GLOBALI SET 
				VAR_COD_RUBRICA=@pREFIX_COD_RUB+convert(nvarchar(10),@RESULT) ,
				VAR_CODICE=@pREFIX_COD_RUB+convert(nvarchar(10),@RESULT)
				WHERE SYSTEM_ID=@RESULT
		
		end
	END


return @RESULT

  
         
     




GO


