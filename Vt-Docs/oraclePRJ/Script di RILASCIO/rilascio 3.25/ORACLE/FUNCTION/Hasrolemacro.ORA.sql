begin 
Utl_Backup_Plsql_code ('Function','Hasrolemacro'); 
end;
/

create or replace
Function Hasrolemacro (Roleid Int, Codice Varchar2)
Return Varchar2 Is 
  Outvalue Varchar2(20);  
  Num Int;
Begin
Select count('x') into num
  From Dpa_Corr_Globali Cg_Ruolo
  , Dpa_Tipo_F_Ruolo Tfr
  , Dpa_Tipo_Funzione tf
  Where Tfr.Id_Ruolo_In_Uo (+) = Cg_Ruolo.System_Id  
  And Tfr.Id_Tipo_Funz  = Tf.System_Id (+)
  And Cg_Ruolo.System_Id = Roleid
  And Var_Cod_Tipo = Codice;

If Num > 0 Then Outvalue := 'Sì';  Else Outvalue := 'No';
End If; 

Return Outvalue; 
END; 
/
