-- will be replaced in 3.25 by new SP 
--begin 	utl_backup_sp ('UTL_BACKUP_SP','3.24');end;
--/

create or replace Procedure UTL_BACKUP_SP (Nomesp Varchar2, Myversione_Cd Varchar2)
authid current_user Is
Cursor C_Sp Is 
Select Case Rownum When 1 Then  
                    Replace(  Upper(Text)
                              ,Upper(Nomesp)
                              --SP name may be at most 30 characters long (25+5=30)
                              ,Substr(Upper(Nomesp),1,25 )  || To_Char(Sysdate,'ddMON')  
                            )   Else text End       as testo 
From User_Source Where Name=Upper(Nomesp) ; 
v_clob CLOB := 'CREATE OR REPLACE ';
Stringa_Msg Varchar2 (200); 
Nomeutente  Varchar2 (32); 
Begin
select username into Nomeutente from user_users ; 
For Sp In C_Sp Loop
    Dbms_Lob.Append (V_Clob, To_Char (Sp.Testo) || ' ');
End Loop;

If Length(V_Clob) >= 32767 Then 
	Raise_Application_Error (-20101, Nomesp||' SP text goes beyond 32767 chars, can''t copy here, use SQL Developer')   ; 
end if;

--Dbms_Output.Put_Line (V_Clob)  ; 
Execute Immediate To_Char(V_Clob) ; 
Stringa_Msg := Nomesp||' copied to ' || Substr(Upper(Nomesp),1,25 )  || To_Char(Sysdate,'ddMON')   ; 

Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'ok') ; 

Exception 
When no_data_found Then 
	Stringa_Msg := 'no_data_found while trying to copy '|| Nomesp; 
  Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'KO') ; 
When Others Then 
  Stringa_Msg := 'KO trying to copy '|| Nomesp; 
  Utl_Insert_Log (Nomeutente, Sysdate, Stringa_Msg, Myversione_Cd ,'KO') ; 

end;
/
