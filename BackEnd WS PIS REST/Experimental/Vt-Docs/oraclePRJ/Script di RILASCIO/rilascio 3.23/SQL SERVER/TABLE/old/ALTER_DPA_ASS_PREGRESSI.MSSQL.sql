BEGIN
 utl_add_index '3.23','@db_user','DPA_ASS_PREGRESSI',
    'IX_T_DPA_ASS_PREGRESSIK2','UNIQUE',
    'SYSTEM_ID',null,null,null,
	'NORMAL', null,null,    null     

 utl_add_index  '3.23','@db_user','DPA_ASS_PREGRESSI',
    'IX_T_DPA_ASS_PREGRESSIK1',null,
    'ID_PREGRESSO',null,null,null,
    'NORMAL', null,null,null     
END
GO

