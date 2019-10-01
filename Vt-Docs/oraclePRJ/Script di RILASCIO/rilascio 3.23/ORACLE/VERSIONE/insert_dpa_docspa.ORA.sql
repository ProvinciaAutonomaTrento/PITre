begin
Insert Into @db_user.Dpa_Docspa (System_Id, Dta_Update, Id_Versions_U)
select max(system_id) +1 , sysdate,  'PITre 2.13' from @db_user.Dpa_Docspa;
end;
/              

