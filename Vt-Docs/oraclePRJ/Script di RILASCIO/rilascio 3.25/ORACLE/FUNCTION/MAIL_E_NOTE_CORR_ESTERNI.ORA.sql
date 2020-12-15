create or replace
Function          mail_e_note_corr_esterni (myId_Corr Int)
Return Varchar Is Risultato Varchar2(32767); -- max length for varchar2 is 32767

Begin
Risultato := Null;

With Data As
-- Rif. http://www.oracletips.info/Concatenating_Multiple_Rows_Into_A_Single_String.htm 
  (Select Myvalues,    Row_Number() Over (Order By Var_Principale Desc Nulls Last) Rn,    Count(*) Over () Cnt
  From
    (Select Me.Var_Email||(Case When Var_Note Is Null Then ' ' Else '##'||Var_Note||'##' End) As  Myvalues
    ,Var_Principale
     From Dpa_Mail_Corr_Esterni Me
      Where Me.Id_Corr = myId_Corr 
     )
  )
SELECT ltrim(sys_connect_by_path(myvalues, '; '),';') into risultato 
FROM data
WHERE rn              = cnt
  Start With Rn       = 1
  Connect By Prior Rn = Rn-1;
Return Risultato;
Exception When NO_DATA_FOUND Then Risultato:= null; Return Risultato;
 when Others then Risultato:= myId_Corr||SQLERRM; Return Risultato;
End Mail_E_Note_Corr_Esterni ;
/
