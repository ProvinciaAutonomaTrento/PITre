--------------------------------------------------------
--  DDL for Function GETVALCAMPOPROFDOCORDER
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETVALCAMPOPROFDOCORDER" (DocNumber INT, CustomObjectId INT)
RETURN VARCHAR IS result VARCHAR(255);

tipoOggetto varchar(255);

BEGIN

select b.descrizione
into tipoOggetto
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
else 
--Tutti gli altri
    select valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber; 
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

END getValCampoProfDocOrder;

/
