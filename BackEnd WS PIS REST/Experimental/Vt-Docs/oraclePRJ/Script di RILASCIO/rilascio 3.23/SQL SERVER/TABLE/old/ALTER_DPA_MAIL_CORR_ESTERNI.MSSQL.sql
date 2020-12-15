BEGIN
--create index IX_Dpa_Mail_Corr_Esterni_K2  on Dpa_Mail_Corr_Esterni(ID_CORR); 
 utl_add_index'3.23','@db_user','Dpa_Mail_Corr_Esterni',
    'IX_Dpa_Mail_Corr_Esterni_K2',null,
    'ID_CORR',null,null,null,
    'NORMAL', null,null,null     
END
GO

