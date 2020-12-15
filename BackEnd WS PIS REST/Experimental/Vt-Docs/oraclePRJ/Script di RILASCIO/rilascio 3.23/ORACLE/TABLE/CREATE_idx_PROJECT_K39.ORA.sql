BEGIN
--create index idx_PROJECT_K39 on PROJECT(ID_TIPO_FASC)

 utl_add_index('3.23','@db_user','PROJECT',
    'idx_PROJECT_K39',null,
    'ID_TIPO_FASC',null,null,null,
    'NORMAL', null,null,null     );
END;
/
