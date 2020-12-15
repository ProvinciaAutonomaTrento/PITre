--------------------------------------------------------
--  DDL for Function GETVALPROFOBJPRJ
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETVALPROFOBJPRJ" (PrjId INT, CustomObjectId INT)
RETURN VARCHAR IS result VARCHAR(255);

tipoOggetto varchar(255);tipoCont varchar(1);

BEGIN

select b.descrizione,cha_tipo_Tar
into tipoOggetto,tipoCont
from 
dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b 
where 
a.system_id = CustomObjectId
and
a.id_tipo_oggetto = b.system_id;

if (tipoOggetto = 'Corrispondente') then
      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 
      from dpa_CORR_globali cg where cg.SYSTEM_ID = (
      select valore_oggetto_db from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = to_char(PrjId));
     else
      
--Casella di selezione (Per la casella di selezione serve un caso particolare perch?? i valori sono multipli)
if(tipoOggetto = 'CasellaDiSelezione') then
    BEGIN
        declare item varchar(255);
        CURSOR curCasellaDiSelezione IS select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = to_char(PrjId) and valore_oggetto_db is not null; 
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
     elsif(tipoOggetto = 'Contatore') then
    begin
        select getContatoreFasc(PrjId,tipoCont)  into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = PrjId; 

    end;
else 
--Tutti gli altri
    select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = to_char(PrjId); 
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

END GetValProfObjPrj; 

/
