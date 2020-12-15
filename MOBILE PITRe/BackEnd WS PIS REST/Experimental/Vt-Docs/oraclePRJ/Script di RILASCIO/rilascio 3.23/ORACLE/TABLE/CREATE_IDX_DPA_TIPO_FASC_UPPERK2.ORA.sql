BEGIN
--create index IDX_DPA_TIPO_FASC_UPPERK2		on DPA_TIPO_FASC(UPPER(VAR_DESC_FASC) )

 utl_add_index('3.23','@db_user','DPA_TIPO_FASC',
    'IDX_DPA_TIPO_FASC_UPPERK2',null,
    'UPPER(VAR_DESC_FASC)',null,null,null,
    'NORMAL', null,null,null     );
END;
/
