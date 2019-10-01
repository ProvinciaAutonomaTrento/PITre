declare 

c_seq number;

c_seq2 number;

cnt1 int;

cnt2 int;


begin


select @db_user.SEQ.nextval into c_seq from dual;

select @db_user.SEQ.nextval into c_seq2 from dual;

 
 begin
select count(*) into cnt1 from @db_user.DPA_ANAGRAFICA_LOG l where l.var_codice ='AMM_LOGIN';

if (cnt1 = 0) then
Insert into DPA_ANAGRAFICA_LOG

   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)

Values

   (c_seq, 'AMM_LOGIN', 'Accesso Admin all''applicazione', 'UTENTE', 'AMM_LOGIN');

Insert into @db_user.DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (c_seq, 0);


 end if;
 end;
 
  begin
select count(*) into cnt2 from @db_user.DPA_ANAGRAFICA_LOG l where l.var_codice ='AMM_LOGOFF';

if (cnt2 = 0) then

Insert into DPA_ANAGRAFICA_LOG

   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)

Values

   (c_seq2, 'AMM_LOGOFF', 'Uscita Admin dall''applicazione', 'UTENTE', 'AMM_LOGOFF');
Insert into @db_user.DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (c_seq2, 0);

end if;
end;
 
END;
/


begin
Insert into @db_user.DPA_DOCSPA
   (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values
   (seq.nextval, sysdate, '3.12.10');
end;
/

