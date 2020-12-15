create or replace
Function Utl_Get_Titolo_Pitre Return Varchar2 As
v_title Varchar2(24) ;
Begin
Select Title Into V_Title
    From (Select Id_Versions_U Title From Dpa_Docspa Order By System_Id Desc) Where Rownum=1;
Return V_Title;
end;
/
