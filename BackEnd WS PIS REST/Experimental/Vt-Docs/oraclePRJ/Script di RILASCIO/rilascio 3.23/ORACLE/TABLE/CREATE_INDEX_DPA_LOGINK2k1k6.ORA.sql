BEGIN
--Create Index Ix_Dpa_Login_K2k1k6 On Dpa_Login (Id_Amm, Upper(User_Id), Upper(Session_Id))  Compute Statistics ;

 utl_add_index('3.23','@db_user','Dpa_Login',
    'Ix_Dpa_Login_K2k1k6',null,
    'Id_Amm','Upper(User_Id)','Upper(Session_Id)',null,
    'NORMAL', null,null,null     );
END;
/
