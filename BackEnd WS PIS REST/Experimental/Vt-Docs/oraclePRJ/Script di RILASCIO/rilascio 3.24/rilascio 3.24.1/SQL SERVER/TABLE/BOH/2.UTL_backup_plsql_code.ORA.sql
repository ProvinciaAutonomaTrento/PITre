Create Or Replace
Procedure UTL_backup_plsql_code (tipoplsql Varchar2, Nomeplsql Varchar2) 
authid current_user Is
Cursor C_Sp Is
Select Text As Testo 
From User_Source Where Name=Upper(Nomeplsql)  and type= upper(tipoplsql); 
v_clob CLOB := 'CREATE OR REPLACE ';
Stringa_Msg Varchar2 (200); 
Nomeutente  Varchar2 (32);  Myversione_Cd Varchar2 (32);  
Begin
select username into Nomeutente from user_users ; 
For Sp In C_Sp Loop
    Dbms_Lob.Append (V_Clob, To_Char (Sp.Testo) || ' ');
End Loop;

--If Length(V_Clob) >= 32767 Then Raise_Application_Error (-20101, Nomeplsql||' SP text goes beyond 32767 chars, can''t copy here, use SQL Developer')   ; end if;

--Dbms_Output.Put_Line (V_Clob)  ; Execute Immediate To_Char(V_Clob) ; 
Myversione_Cd := Utl_Get_Titolo_Pitre;

Insert Into Utl_Backup_Plsql (Var_Dict_Obj_Type ,      Var_Dict_Obj_Name ,      Var_Pitre_Title   
    , Dta_Of_Change ,	    Clob_Code_Safe_To_Restore )
    values (tipoplsql , Nomeplsql, Myversione_Cd, sysdate,  V_Clob);
    
Stringa_Msg := Nomeplsql||' copied to Utl_Backup_Plsql table'    ; 

Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ; 

Exception 
When no_data_found Then 
	Stringa_Msg := 'no_data_found while trying to copy '|| Nomeplsql; 
  Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'KO') ; 
When Others Then 
  Stringa_Msg := 'KO trying to copy '|| Nomeplsql; 
  Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'KO') ; 

end;
/
