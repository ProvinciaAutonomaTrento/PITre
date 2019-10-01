begin
--Note della Bozzi by De Marco

-- CREATE INDEX IX_ELENCO_NOTE_K2 ON DPA_ELENCO_NOTE (ID_REG_RF) ; 
utl_add_index('3.24','@db_user','DPA_ELENCO_NOTE',
    'IX_ELENCO_NOTE_K2',null,
    'ID_REG_RF',null,null,null,
    'NORMAL', null,null,null     );

delete Dpa_Elenco_Note where Cod_Reg_Rf= 'TUTTI' and Id_Reg_Rf=0
and Var_Desc_Nota in 
('N.B. Irregolarità nel documento principale'
,'N.B. Irregolarità negli allegati'
,'N.B. Irregolarità nel documento principale e negli allegati');

-- need this dummy record if table is empty
Insert Into Dpa_Elenco_Note (System_Id, Id_Reg_Rf, Var_Desc_Nota, Cod_Reg_Rf)
                      Values (1       ,0         ,'dummy note'  ,'TUTTI'); 

Insert Into Dpa_Elenco_Note (System_Id, Id_Reg_Rf, Var_Desc_Nota, Cod_Reg_Rf)
select max(system_id) +1  ,0 
, 'N.B. Irregolarità nel documento principale'
, 'TUTTI' from  dpa_elenco_note 
;

Insert Into Dpa_Elenco_Note (System_Id, Id_Reg_Rf, Var_Desc_Nota, Cod_Reg_Rf)
Select Max(System_Id) +1 ,0 
, 'N.B. Irregolarità negli allegati'
, 'TUTTI' 
From  Dpa_Elenco_Note;

Insert Into Dpa_Elenco_Note (System_Id, Id_Reg_Rf, Var_Desc_Nota, Cod_Reg_Rf)
Select Max(System_Id) +1 ,0 
, 'N.B. Irregolarità nel documento principale e negli allegati'
, 'TUTTI' 
From  Dpa_Elenco_Note;

Delete Dpa_Elenco_Note Where Cod_Reg_Rf= 'TUTTI' And Id_Reg_Rf=0
And Var_Desc_Nota ='dummy note';

END;
/

