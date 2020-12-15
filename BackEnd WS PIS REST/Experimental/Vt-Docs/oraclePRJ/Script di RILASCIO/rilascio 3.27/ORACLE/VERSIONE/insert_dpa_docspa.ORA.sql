begin
Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
--Values    (seq.nextval, sysdate, '3.24');
select max(system_id) +1 , sysdate,  'PITre 2.17' from @db_user.Dpa_Docspa;
end;
/



