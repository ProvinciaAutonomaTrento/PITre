begin
	utl_backup_sp ('UTL_ISVALORE_LT_COLUMN','3.24');
end;
/


create or replace Function UTL_ISVALORE_LT_COLUMN
(Valore Varchar2,Mytable Varchar2,mycol varchar2)
Return Integer -- returns 0 if OK, that is: lentgh(valore) less then Data_Length of the column mycol
Is Cnt Int;
returnvalue int :=1;
Begin
Select  Data_Length - NVL(Length(Valore),0) Into Cnt   From Cols
  Where Lower(Table_Name)=Lower(Mytable) And Lower(Column_Name) = Lower(Mycol) ;

If Cnt>=0 Then Returnvalue := 0 ; End If;

Return Returnvalue ;
Exception When Others Then Raise;
end;
/
