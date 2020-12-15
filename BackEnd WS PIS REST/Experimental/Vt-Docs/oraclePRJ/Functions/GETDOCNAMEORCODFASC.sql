--------------------------------------------------------
--  DDL for Function GETDOCNAMEORCODFASC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETDOCNAMEORCODFASC" (Id Integer)

return varchar IS 
Returnvalue Varchar2(2000); 
Mytipo_Proj Varchar2(20);  
Myid_Fascicolo Varchar2(20);  
myId_Parent Varchar2(20);  
Begin

/* dato un ID, o e un documento oppure un fascicolo 
INFATTI QUESTA QUERY TORNA ZERO RECORD: 
Select System_Id From ProJECT
  Intersect
Select System_Id From Profile */

Select nvl(Docname,system_id) into returnvalue From Profile 
  Where System_Id = Id; 

return  Returnvalue; 
Exception 
When No_Data_Found Then  -- SE LA QUERY NON HA TORNATO VALORI, dovrebbe essere un fascicolo

Select  cha_tipo_Proj , id_Fascicolo , Id_Parent into Mytipo_Proj , Myid_Fascicolo , myId_Parent 
From Project   Where System_Id = Id; 

If Mytipo_Proj = 'F' Then 
  Select    'Fascicolo\' || Nvl(Var_Codice,Description)  into Returnvalue
  From Project   Where System_Id = Id; 
end if;  

If MyTipo_Proj = 'C' Then 
  If Myid_Fascicolo = Myid_Parent Then 
    Select   'CartellaPrincipale\'|| Nvl(Var_Codice,Description) Into Returnvalue
    From Project   Where System_Id = Id; 
  End If;   
  If Myid_Fascicolo <> Myid_Parent Then 
    Select   'SottoFascicolo\'|| Nvl(Var_Codice,Description)  into Returnvalue
    From Project   Where System_Id = Id; 
  end if; 
End If;  


return  Returnvalue; 
When Others Then Returnvalue := Null; -- richiesta esplicita che non si intercetti l'eccezione
return  Returnvalue; 
End; 

/
