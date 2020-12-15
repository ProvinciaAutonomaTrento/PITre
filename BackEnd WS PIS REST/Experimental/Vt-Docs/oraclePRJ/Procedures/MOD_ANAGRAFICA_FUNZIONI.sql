--------------------------------------------------------
--  DDL for Procedure MOD_ANAGRAFICA_FUNZIONI
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."MOD_ANAGRAFICA_FUNZIONI" 
IS

cont1 number;
cont2 number;
cont3 number;

BEGIN
cont1 := 0;
cont2 := 0;
cont3 := 0;

select count(*) into cont1 from dpa_anagrafica_funzioni where cod_funzione = 'DO_FASC_PRIVATO';
if(cont1 = 0)
    then
    begin
        select count(*) into cont2 from dpa_anagrafica_funzioni where cod_funzione = 'FASC_NUOVO_PRIVATO';
        if(cont2 <> 0)
        then
            begin
                update dpa_anagrafica_funzioni set cod_funzione = 'DO_FASC_PRIVATO' where cod_funzione = 'FASC_NUOVO_PRIVATO';
                
                select count(*) into cont3 from dpa_funzioni where cod_funzione = 'FASC_NUOVO_PRIVATO';
                if(cont3 <> 0)
                then
                    begin
                        update dpa_funzioni set cod_funzione = 'DO_FASC_PRIVATO', var_desc_funzione = 'DO_FASC_PRIVATO' where cod_funzione = 'FASC_NUOVO_PRIVATO';
                    END;
                end if;
            end;
        else
            begin
                Insert into DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED) Values ('DO_FASC_PRIVATO', 'Abilita il check privato nella pagina di creazione di un fascicolo', 'N');
            end;
        end if;
    end;

end if; 

END mod_anagrafica_funzioni; 

/
