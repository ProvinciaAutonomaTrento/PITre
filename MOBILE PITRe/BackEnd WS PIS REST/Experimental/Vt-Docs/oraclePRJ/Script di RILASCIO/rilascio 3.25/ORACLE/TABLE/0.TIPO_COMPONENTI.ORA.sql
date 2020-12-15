begin
-- nuova grafica by Abbatangeli
	declare cnt int;
 	declare cnt1 int;
 	declare cnt2 int;
    begin
        
        select count(*) into cnt from user_tables 
				where table_name='TIPO_COMPONENTI';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE TIPO_COMPONENTI
				(  CHA_TIPO_COMPONENTI CHAR(1) NOT NULL
				, DESCRIZIONE VARCHAR2(20) NOT NULL
				, Constraint Tipo_Componenti_Pk Primary Key
					(CHA_TIPO_COMPONENTI   )   ENABLE) ';

			execute immediate    
            'Insert Into Tipo_Componenti (Cha_Tipo_Componenti,Descrizione) 
				Values (''0'',''Nessuna scelta'')		'; 
			execute immediate    
            'Insert Into Tipo_Componenti (Cha_Tipo_Componenti,Descrizione) 
				Values (''1'',''Active X'')		'; 
			execute immediate    
            'Insert Into Tipo_Componenti (Cha_Tipo_Componenti,Descrizione) 
				Values (''2'',''Smart Client'')		'; 
			-- only on future releases
			execute immediate    
            'Insert Into Tipo_Componenti (Cha_Tipo_Componenti,Descrizione) 
				Values (''3'',''Java Applet'')		'; 

			select count(*) into cnt1 from people where cha_tipo_componenti = '1';
			
			if (cnt1 > 0) then
				execute immediate    
					update people set cha_tipo_componenti = '2' where cha_tipo_componenti = '1';
		  
			end if;

			select count(*) into cnt2 from people where cha_tipo_componenti = '0';
			
			if (cnt2 > 0) then
				execute immediate    
					update people set cha_tipo_componenti = '1' where cha_tipo_componenti = '0';
		  
			end if;

        end if;
    end;    
end;    
/



