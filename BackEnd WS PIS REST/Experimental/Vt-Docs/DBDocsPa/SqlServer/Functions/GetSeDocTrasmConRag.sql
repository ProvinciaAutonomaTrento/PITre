SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE function [@db_user].[GetSeDocTrasmConRag](@idprofile INT,@vardescragione VARCHAR(256))
returns INT
as
begin
declare @risultato INT

if(UPPER(@vardescragione) <> 'TUTTE')
begin
SELECT @risultato = count(tx.system_id)
FROM dpa_trasmissione tx, dpa_trasm_singola ts, dpa_ragione_trasm tr
WHERE tx.system_id = ts.id_trasmissione
AND ts.id_ragione = tr.system_id
AND UPPER(tr.var_desc_ragione) = UPPER(@varDescRagione)
AND tx.id_profile = @idprofile
end
else
begin
--se  stato trasmesso almeno una volta
SELECT @risultato = count(tx.system_id)
FROM dpa_trasmissione tx
WHERE tx.id_profile = @idprofile
end

if (@risultato > 0) set @risultato = 1 else set @risultato = 0

return @risultato
end


GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
