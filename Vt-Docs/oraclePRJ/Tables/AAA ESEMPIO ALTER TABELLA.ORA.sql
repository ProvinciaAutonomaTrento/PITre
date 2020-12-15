-- inizio file -- 
BEGIN
 UTL_ADD_COLUMN( '3.22.1', '@db_user',   'DPA_CHIAVI_CONFIGURAZIONE',
    'DTA_INSERIMENTO',    'DATE',    'SYSDATE',    Null,    Null,   Null  );  
End;
/

-- fine file -- necessario(!) inserire uno spazio dopo lo / al fine di poter unificare i file per l’installazione via CD



-- Specifica campi della SP utl_add_column 

BEGIN
utl_add_column (
   '3.22.1',
   '@db_user',      
   'nome_tabella',
   'nome_colonna1',
   'tipo_dato',
   'val_default',					-- can be null
   'condizione_modifica_pregresso', -- can be null
   'condizione_check',				-- can be null
   'RFU'							-- Reserved future use, can be null
   ) ;                         

utl_add_column(..,tab1, col2, ..);  

End;
/

-- fine file -- 

