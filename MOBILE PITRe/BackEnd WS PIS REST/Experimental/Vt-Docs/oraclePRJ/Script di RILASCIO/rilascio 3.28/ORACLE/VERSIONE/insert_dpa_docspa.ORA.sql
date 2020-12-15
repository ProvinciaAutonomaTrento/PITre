
-- ATTENZIONE !!!! Occorre verificare l'ID tipo_oggetto prima di lanciare questi script

begin
  execute immediate
          'UPDATE  DPA_OGGETTI_CUSTOM_fasc D SET RICERCA_CORR=''INTERNI/ESTERNI''  WHERE NVL(RICERCA_CORR,''0'') =''0'' AND ID_TIPO_OGGETTO=7';
end;
/


begin
  execute immediate
          'UPDATE  DPA_OGGETTI_CUSTOM D SET RICERCA_CORR=''INTERNI/ESTERNI''  WHERE NVL(RICERCA_CORR,''0'') =''0'' AND ID_TIPO_OGGETTO=1';
end;
/


begin
Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
--Values    (seq.nextval, sysdate, '3.24');
select max(system_id) +1 , sysdate,  'PITre 2.18' from @db_user.Dpa_Docspa;
end;
/



