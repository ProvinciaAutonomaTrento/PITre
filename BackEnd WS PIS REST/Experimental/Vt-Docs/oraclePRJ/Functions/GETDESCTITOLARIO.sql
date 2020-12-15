--------------------------------------------------------
--  DDL for Function GETDESCTITOLARIO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETDESCTITOLARIO" (IdTit INT)
RETURN varchar IS risultato varchar2(2000);

varStato varchar2(1);
dataInizio varchar2(20);
dataFine varchar2(20);

CURSOR cur IS
select cha_stato, 
    to_char(dtA_ATTIVAZIONE, 'DD/MM/YYYY') as dataInizio2,
    to_char(dta_cessazione, 'DD/MM/YYYY') as dataFine2
    from project where system_id=IdTit;


BEGIN

    varStato := '0';

OPEN cur;
LOOP
FETCH cur INTO varStato, dataInizio, dataFine;
EXIT WHEN cur%NOTFOUND;

begin
    if(varStato != '0')
    then
        begin
        if(varStato = 'A')
        then
            risultato := 'Titolario Attivo';
        else
            risultato := 'Titolario valido dal ' || dataInizio || ' al ' || dataFine;
        end if;
        end;
    end if;
end;

END LOOP;

    RETURN risultato;
END getDescTitolario; 

/
