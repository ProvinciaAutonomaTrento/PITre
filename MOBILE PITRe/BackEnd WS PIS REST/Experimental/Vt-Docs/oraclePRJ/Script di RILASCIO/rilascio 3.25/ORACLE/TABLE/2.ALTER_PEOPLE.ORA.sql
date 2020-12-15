BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='PEOPLE' and column_name='CHA_TIPO_COMPONENTI';

--add column
--ALTER TABLE PEOPLE ADD ( "CHA_TIPO_COMPONENTI" CHAR(1 BYTE) default '0' not null );
utl_add_column ('3.25',
                   '@db_user',
                   'PEOPLE',
                   'CHA_TIPO_COMPONENTI',
                   'CHAR(1 BYTE)',
                   ' 0 not null',NULL,NULL,NULL);


--update values on new column
if cntcol = 0 then 
	execute immediate 
	'Update people
	Set   Cha_Tipo_Componenti = Nvl(people.Is_Enabled_Smart_Client,''0'') 	
		where                   Nvl(people.Is_Enabled_Smart_Client,''0'') <> ''0''	';
end if;

-- add FK
--ALTER TABLE People add CONSTRAINT FK_TIPO_COMPONENTI    
--FOREIGN KEY (CHA_TIPO_COMPONENTI) REFERENCES TIPO_COMPONENTI (CHA_TIPO_COMPONENTI);

utl_add_foreign_key(
	'3.25'			--versione_CD     
	,'@db_user'		--Nomeutente      
	,'TIPO_COMPONENTI'		--Nome_Tabella_Pk   
	,'CHA_TIPO_COMPONENTI'	--Nome_Colonna_Pk    
	,'PEOPLE'				--nome_tabella_fk  
	,'CHA_TIPO_COMPONENTI' --nome_colonna_fk 
	,NULL			--Condizione_Join  
	,NULL			--Delete_Rule     
	,NULL			--Validate_At_Creation        
	,'Y'); 			--cifra_nome_FK 
	
END;
END;
/
