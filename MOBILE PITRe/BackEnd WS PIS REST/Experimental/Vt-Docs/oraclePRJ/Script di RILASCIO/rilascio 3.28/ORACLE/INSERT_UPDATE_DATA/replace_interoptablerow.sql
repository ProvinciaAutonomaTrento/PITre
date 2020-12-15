

-- DE LUCA - modifica interoptablerow per gestione prospetto R5

begin
	execute immediate
		'drop TYPE        "INTEROPTABLEROW"';
end;
/

begin
	execute immediate
		'Drop Type          Interoptabletype';
end;
/		

begin
	execute immediate
		'Create Or Replace Type              Interoptabletype Is 
OBJECT (var_cod_amm varchar (256), Var_Cod_Aoo Varchar(256) -- prev 100
, Gennaio Varchar(20), Febbraio Varchar(20), Marzo Varchar(20), Aprile Varchar(20)
, maggio varchar(20), giugno varchar(20),
Luglio Varchar(20), Agosto Varchar(20), Settembre Varchar(20), Ottobre Varchar(20), Novembre Varchar(20)
, Dicembre Varchar(20), Tot_M_Sped Varchar(20),
Tot_Sped Varchar(20))';
end;
/

begin
	execute immediate
		'Create Or Replace Type "INTEROPTABLEROW" Is Table Of Interoptabletype';
end;		
/


