begin 
	Utl_Backup_Plsql_code ('PROCEDURE','UTL_ADD_FOREIGN_KEY'); 
end;
/


create or replace PROCEDURE  UTL_ADD_FOREIGN_KEY(
    versione_CD     VARCHAR2,
    Nomeutente      Varchar2,
    Nome_Tabella_Pk   Varchar2,
    Nome_Colonna_Pk    Varchar2,
    nome_tabella_fk  VARCHAR2,
    nome_colonna_fk VARCHAR2,
    Condizione_Join  Varchar2,
    Delete_Rule     Varchar2,
    --disable_At_creation         Varchar2,  only for 11g
    Validate_At_Creation        Varchar2,
    cifra_nome_FK Varchar2)
Is
  Istruzione       Varchar2(2000); Delete_Condition Varchar2(200);
  Disable_Condition Varchar2(200); validate_Condition Varchar2(200);
  cnt              INT; cntPK int;
  Cntdati          Int;
  Tab_Pk           Varchar2(200)  := Substr(Nome_Tabella_Pk,1,10);
  tab_fk           Varchar2(200)  := Substr(Nome_Tabella_Fk,1,8);
  col_fK           VARCHAR2(200)  := SUBSTR(Nome_Colonna_Pk,1,8);
  
  nome_foreign_key VARCHAR2(2000) :='FK_'||tab_fk||'_'||col_fK||'_'||tab_PK||'';
  errore_msg       VARCHAR(255);
BEGIN
    
Select Count(*) Into Cntpk
From All_Cons_Columns Cons_Cols_Pk Join All_Constraints Cons_Pk On Cons_Pk.Constraint_Name=Cons_Cols_Pk.Constraint_Name 
Where Cons_Pk.Constraint_Type='P'  And Cons_Pk.Owner = ''||Nomeutente||''
    And Cons_Pk.Table_Name       = ''||Nome_Tabella_Pk||''
    And Cons_Cols_Pk.Column_Name = ''||Nome_Colonna_Pk||''         ;

IF (Cntpk = 1) Then   -- ok il vincolo di PK esiste

   Select count(*) into cnt
   From All_Constraints Cons_Fk 
   Join All_Constraints Cons_Pk         
    On Cons_Fk.R_Constraint_Name = Cons_Pk.Constraint_Name And Cons_Pk.Owner = Cons_Fk.Owner
    Join All_Cons_Columns Cons_Cols_Pk  
    On Cons_Pk.Constraint_Name = Cons_Cols_Pk.Constraint_Name  And Cons_Pk.Owner = Cons_Cols_Pk.Owner
    Join All_Cons_Columns Cons_Cols_Fk   
    On Cons_Fk.Constraint_Name = Cons_Cols_Fk.Constraint_Name  and Cons_Fk.Owner = Cons_Cols_Fk.Owner
     Where Cons_Fk.Constraint_Type='R' And Cons_Pk.Constraint_Type='P'  And Cons_Pk.Owner = ''||Nomeutente||''
     And Cons_Pk.Table_Name       = ''||Nome_Tabella_Pk||''
     And Cons_Cols_Pk.Column_Name = ''||Nome_Colonna_Pk||''         
     And Cons_Fk.Table_Name       = ''||nome_tabella_fk||''
     And Cons_Cols_Fk.Column_Name = ''||nome_colonna_fk||''         ;
              
     If (Cnt = 0) Then   -- ok il vincolo non esiste
        Istruzione :=        'Select count(*) cntdati From '
        ||Nomeutente||'.'||nome_tabella_fk||' Child Left Outer Join '
        ||Nomeutente||'.'||Nome_Tabella_Pk||' Parent On Parent.'||Nome_Colonna_Pk||' = Child.'||nome_colonna_fk
        ||'  Where Parent.'||Nome_Colonna_Pk||' Is Null and Child.'||Nome_Colonna_Fk||' is not null '
        || Condizione_Join ; --es and Cha_Tipo_Urp=''P'' And Cha_Tipo_Ie=''I'' 
                
        dbms_output.put_line ('istruzione cntdati:'||istruzione||to_char(sysdate,'HH24:mi:ss'));
                Execute Immediate Istruzione Into Cntdati ;
                If (Cntdati = 0) Then -- non esistono record che violano la chiave
                  Dbms_Output.Put_Line ('cntdati = 0; inizio calcolo opzioni...: ');  
                  
                  -- calcolo opzioni per create constraint
                    If Delete_Rule Is Null Then Delete_Condition := ' ' ; 
                      Else Delete_Condition := ' on delete '||Delete_Rule ;
                    end if;
                
                    --If Disable_At_Creation  = 'Y' Then Disable_Condition  := ' DISABLED '  ; Else Disable_Condition  := '' ; End If;
                    If Validate_At_Creation = 'N' Then Validate_Condition := ' ENABLE NOVALIDATE '; Else Validate_Condition := '' ; End If;
                
                Dbms_Output.Put_Line ('calcolo opzioni completato; inizio alter... at: '||To_Char(Sysdate,'HH24:mi:ss'));  

If Cifra_Nome_Fk = 'Y'                 Then 
  Select Colfk_Id||Tabfk_Id||Tabpk_Id into nome_foreign_key 
  From 
  (Select 'K'||Column_Id Colfk_Id From Cols Where Table_Name=''||nome_tabella_fk||'' And Column_Name=''||nome_colonna_fk ||'')
  ,(Select '_T'||Object_Id Tabfk_Id From User_Objects Pk Where Object_Name=''||Nome_Tabella_Fk||'') 
  ,(Select 'REF_P'||Object_Id Tabpk_Id From User_Objects Pk Where Object_Name=''||nome_tabella_fk||'') ;
end if;
                
                 Istruzione := 'ALTER TABLE '||Nomeutente||'.'||nome_tabella_fk||
                  ' ADD CONSTRAINT '||Nome_Foreign_Key||
                  ' FOREIGN KEY('          ||nome_colonna_fk||
                  ') REFERENCES '            ||Nomeutente||'.'||Nome_Tabella_Pk||'('||Nome_Colonna_Pk||') '
                  ||Delete_Condition||Validate_Condition;
                      
                  Dbms_Output.Put_Line ('istruzione da eseguire : '||Istruzione );
                  
                  Execute Immediate Istruzione;
                  dbms_output.put_line ('istruzione eseguita con codice uscita: '||Sqlcode );
                  If Sqlcode = 0 Then 
                   --dbms_output.put_line ('istruzione eseguita: '||istruzione);  
                  utl_insert_log (nomeutente -- nome utente
                  , Null                     -- data
                  ,'Added Foreign Key '||Nome_Foreign_Key||' on table '||nome_tabella_fk , Versione_Cd , 'esito positivo' );
                  End If; 
                  --dbms_output.put_line ('utl_insert_log eseguita: ');  
                Else -- ci sono record che violano la chiave
                 
                Dbms_Output.Put_Line ('NOT Added Foreign Key '||nome_foreign_key||' on table '||Nome_Tabella_Pk||'; ci sono record che violano la chiave');
                
                  utl_insert_log (nomeutente -- nome utente
                  , Null                     -- data
                  ,'NOT Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella_fk , versione_CD , 'Ci sono record che violano la chiave' );
                END IF;
              Else 
                utl_insert_log (nomeutente -- nome utente
                , Null                     -- data
                ,'NOT Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella_fk , versione_CD , 'Chiave esterna già esistente' );
              End If;
              --dbms_output.put_line ('sqlerrm: '||sqlerrm);
Else -- non esiste la PK relativa
utl_insert_log (nomeutente -- nome utente
 , Null                     -- data
 ,'NOT Added Foreign Key '||Nome_Foreign_Key||' on table '||Nome_Tabella_Fk , Versione_Cd , 'Chiave PK non trovata o non corrispondente' );
        
end if; 

EXCEPTION -- qui cominciano le istruzioni da eseguire se le SP va in errore
WHEN OTHERS THEN
  dbms_output.put_line ('errore '||SQLERRM);
  errore_msg := SUBSTR(SQLERRM,1,255);
  utl_insert_log (nomeutente -- nome utente
  , Null                     -- data
  ,'NOT Added Foreign Key '||nome_foreign_key||' on table '||nome_tabella_fk , versione_CD , 'esito negativo - '||errore_msg||'' );
END utl_add_foreign_key;
/

