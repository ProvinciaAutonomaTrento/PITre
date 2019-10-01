-- MICROFUNZIONI PASSI AUTOMATICI
begin
Utl_Insert_Chiave_Microfunz('DO_PROT_PROTOCOLLA_AUTOMATICO'
,'Abilita la protocollazione automatica'
,Null,'N',Null,'3.2.13',Null);
End;

begin
Utl_Insert_Chiave_Microfunz('DO_REPERTORIAZIONE_ATOMATICA'
,'Abilita la repertoriazione automatica'
,Null,'N',Null,'3.2.13',Null);
End;

begin
Utl_Insert_Chiave_Microfunz('DO_OUT_SPEDISCI_AUTOMATICA'
,'Abilita la spedizione automatica'
,Null,'N',Null,'3.2.13',Null);
End;

begin
Utl_Insert_Chiave_Microfunz('CREA_PASSO_PROTO_AUTO'
,'Abilita la creazione di un passo automatico di protocollazione in un processo di firma'
,Null,'N',Null,'3.2.13',Null);
End;

begin
Utl_Insert_Chiave_Microfunz('CREA_PASSO_REPERTORIAZIONE_AUTO'
,'Abilita la creazione di un passo automatico di repertoriazione in un processo di firma'
,Null,'N',Null,'3.2.13',Null);
End;

begin
Utl_Insert_Chiave_Microfunz('CREA_PASSO_SPEDIZIONE_AUTO'
,'Abilita la creazione di un passo automatico di spedizione in un processo di firma'
,Null,'N',Null,'3.2.13',Null);
End;

-- MICROFUNZIONE TASTO PREDISPONI
begin
Utl_Insert_Chiave_Microfunz('DO_DOC_PREDISPONI'
,'Abilita la predisposizione alla protocollazione di un documento non protocollato'
,Null,'N',Null,'3.2.13',Null);
End;