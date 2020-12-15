CREATE function [DOCSADM].[GETSTATOCONSERVAZIONE] (@IDPROFILE int)
returns varchar(2000)
as BEGIN
declare @result VARCHAR(3000)
declare @idDocPr INT;
  SELECT @result = cha_stato
    FROM dpa_versamento
    WHERE id_profile = @IDPROFILE
  IF @@ROWCOUNT = 0
      -- gestione allegati
      BEGIN
        SELECT @idDocPr = id_documento_principale
        FROM profile
        where system_id = @IDPROFILE
        IF NOT @idDocPr IS NULL
        begin
          SELECT @result = cha_stato
          FROM dpa_versamento
          WHERE id_profile = @idDocPr
        end
        ELSE
        begin
          set @result = 'N'
        end
       END
  
    RETURN @result
END