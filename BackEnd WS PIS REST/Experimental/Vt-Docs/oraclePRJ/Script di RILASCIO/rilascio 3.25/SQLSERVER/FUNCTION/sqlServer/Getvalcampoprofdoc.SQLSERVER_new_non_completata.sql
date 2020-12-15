-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 05/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- GETVALCAMPOPROFDOC
-- =============================================


ALTER FUNCTION [DOCSADM].[getValCampoProfDoc]
(
	@DocNumber INT
	, @CustomObjectId INT
)
RETURNS VARCHAR(400) 
AS 
BEGIN

	DECLARE @result         VARCHAR(255)
	DECLARE @tipoOggetto	VARCHAR(255)
	DECLARE @tipoCont		VARCHAR(1)
	DECLARE @repert         INT
	DECLARE @tipologiaDoc	INT
	
	SET @repert = 0
	SET @tipologiaDoc = 0
	
	-- restituisce 1 se il documento DocNumber associato alla tipologia di documento contenente il contatore di repertorio con id = CustomObjectId
	
	SELECT @tipologiaDoc = (CASE WHEN ID_OGGETTO IS NOT NULL THEN 1 ELSE 0 END)
	FROM DOCSADM.DPA_ASSOCIAZIONE_TEMPLATES
	WHERE DOC_NUMBER = @DocNumber
		AND ID_OGGETTO = @CustomObjectId
		AND @@ROWCOUNT = 1

END 



-- restituisce 1 se il documento DocNumber associato alla tipologia di documento contenente il contatore di repertorio con id = CustomObjectId
SELECT (case when id_oggetto is not null then 1 else 0 end) as res
into tipologiaDoc
from dpa_associazione_templates
where doc_number=DocNumber and id_oggetto=CustomObjectId and rownum = 1;

select b.descrizione,cha_tipo_Tar, a.repertorio
into tipoOggetto,tipoCont,repert
from 
dpa_oggetti_custom a, dpa_tipo_oggetto b 
where 
a.system_id = CustomObjectId
and
a.id_tipo_oggetto = b.system_id;

if (tipoOggetto = 'Corrispondente') then
      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 
      from dpa_CORR_globali cg where cg.SYSTEM_ID = (
      select valore_oggetto_db from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber));
     else
      
--Casella di selezione (Per la casella di selezione serve un caso particolare perch?? i valori sono multipli)
if(tipoOggetto = 'CasellaDiSelezione') then
    BEGIN
        declare item varchar(255);
        CURSOR curCasellaDiSelezione IS select  valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber) and valore_oggetto_db is not null; 
        BEGIN
            OPEN curCasellaDiSelezione;
            LOOP
            FETCH curCasellaDiSelezione INTO item;
            EXIT WHEN curCasellaDiSelezione%NOTFOUND;
                IF(result IS NOT NULL) THEN
                result := result||'; '||item ;
                ELSE
                result := result||item;
                END IF;        
            END LOOP;
            CLOSE curCasellaDiSelezione;
        END;    
    END;
    elsif(tipoOggetto = 'Contatore' AND repert = 1 And tipologiadoc = 1)
    then
    begin
    RETURN '#CONTATORE_DI_REPERTORIO#';
    end;
    elsif(tipoOggetto = 'Contatore' OR tipoOggetto = 'ContatoreSottocontatore')  then
    begin
        select getContatoreDoc(DocNumber,tipoCont)  into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber; 

    end;
else 
--Tutti gli altri
    select valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber); 
end if;    
end if;

RETURN result;

exception
when no_data_found
then
result := null; --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 
RETURN result;
when others
then
result := SQLERRM; 
RETURN result;

End Getvalcampoprofdoc;
/
              
----------- FINE -