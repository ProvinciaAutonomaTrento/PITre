begin 
	utl_backup_sp ('UTL_ADD_INDEX','3.23');
end;
/

create or replace PROCEDURE         @db_user.utl_add_index(
/*
-- =============================================
-- Author:		Gabriele Serpi - De Luca
-- Create date: 25/07/2011 -- riv. mar 2012
-- Description:	permette di aggiungere un indice
				se l''indice esiste con nome differente rispetto a quello passato come argomento, fa la rinomina 

-- =============================================

es. invocazione:
DECLARE
  VERSIONE_CD VARCHAR2(200)             := 'TEST';
  NOMEUTENTE VARCHAR2(200)               := 'PAT_TEST'  ;
  Nome_Tabella Varchar2(200) := 'TEST';
  Nome_Indice Varchar2(200)  := 'IDX_CONTEXT';
  IS_UNIQUE VARCHAR2(200)        := NULL;
  NOME_COLONNA1 VARCHAR2(200) := 'ID';
  NOME_COLONNA2 VARCHAR2(200) := '';
  Nome_Colonna3 Varchar2(200) := '';
  Nome_Colonna4 Varchar2(200) := '';
  Index_Type Varchar2(200) := 'DOMAIN' ;
  Ityp_Name Varchar2(200) := 'CONTEXT';
  Optional_Ityp_Parameters Varchar2(200) := Null ;
  RFU VARCHAR2(200) := NULL;
BEGIN

  UTL_ADD_INDEX(
    VERSIONE_CD => VERSIONE_CD,
    NOMEUTENTE => NOMEUTENTE,
    NOME_TABELLA => NOME_TABELLA,
    NOME_INDICE => NOME_INDICE,
    IS_UNIQUE => IS_UNIQUE,
    NOME_COLONNA1 => NOME_COLONNA1,
    NOME_COLONNA2 => NOME_COLONNA2,
    NOME_COLONNA3 => NOME_COLONNA3,
    Nome_Colonna4 => Nome_Colonna4,
    Index_Type => Index_Type ,
    ITYP_NAME => ITYP_NAME,
    OPTIONAL_ITYP_PARAMETERS => OPTIONAL_ITYP_PARAMETERS,
    RFU => RFU
  );
End;

*/
    
    versione_CD              VARCHAR2,
    nomeutente               VARCHAR2,
    Nome_Tabella             Varchar2,
    Nome_Indice              VARCHAR2,
    is_unique          VARCHAR2, -- passare valore 'UNIQUE' se si vuole creare vincolo di unicità
    nome_colonna1            VARCHAR2,
    nome_colonna2            VARCHAR2,
    Nome_Colonna3            Varchar2,
    Nome_Colonna4            Varchar2,
    Index_Type               Varchar2, -- must be supplied, can be NORMAL or DOMAIN
    Ityp_Name                VARCHAR2, -- if Index_Type == DOMAIN, must be supplied, can be CTXCAT or CONTEXT 
    OPTIONAL_ITYP_PARAMETERS VARCHAR2, -- es sync (on commit) stoplist ctxsys.empty_stoplist
    RFU                      VARCHAR2)
IS
  istruzione          VARCHAR2(2000);
  cnt                 INT;
  cntdati             INT;
  colonne             VARCHAR2(2000);
  Errore_Msg          VARCHAR(255);
  Mygranted_Role      VARCHAR2(20);
  Domain_Index_Clause VARCHAR(2000);
  Rename_Clause       VARCHAR(2000);
  Old_Index_Name      Varchar2(32);
  Old_Index_Type      Varchar2(32) := 'non nullo';
  old_Ityp_Name       Varchar2(32) := 'non nullo';
Begin
  IF (nomeutente IS NULL OR nome_tabella IS NULL OR index_type is null OR Nome_Indice IS NULL OR nome_colonna1 IS NULL) THEN
    Raise_Application_Error (-20010,'in creazione di un indice
    , nessuno tra gli argomenti nomeutente OR nome_tabella OR index_type OR Nome_Indice OR nome_colonna1 può essere NULL ');
  End If;
  If (Index_Type <> 'DOMAIN' AND Index_Type <> 'NORMAL') Then
    Raise_Application_Error (-20010,'per il parametro Index_Type, sono supportati solo tipi NORMAL o DOMAIN');
  END IF;
  If Length(Nome_Indice) >32 Then
    Raise_Application_Error (-20011,'in creazione di un indice, il nome dell''indice non può essere più lungo di 32 caratteri') ;
  End If;
  If Ityp_Name          Is Not Null And (Nome_Colonna2 Is Not Null Or Nome_Colonna3 Is Not Null Or Nome_Colonna4 Is Not Null) Then
    Raise_Application_Error (-20012,'se Ityp_Name valorizzato, perchè si vuole creare un indice di dominio
        , non è possibile specificare più di una colonna, gli argomenti nome_colonna da 2 a 4 devono essere null ')    ;
  END IF; 
  
  SELECT COUNT (*)
  INTO cnt
  FROM all_indexes
  WHERE index_name = upper(nome_indice)
  AND owner        =nomeutente;
  
  IF (Cnt = 0) THEN
    dbms_output.put_line ('ok indice non esiste, o almeno non esiste con il nome passato come parametro ');
    -- verifico se esiste l'indice sulle stesse colonne, con altro nome
    IF Nome_Colonna4 IS NOT NULL AND Nome_Colonna3 IS NOT NULL AND Nome_Colonna2 IS NOT NULL AND Nome_Colonna1 IS NOT NULL THEN
      dbms_output.put_line ('4 colonne');
      SELECT Index_Name
      INTO Old_Index_Name
      FROM
        (SELECT I1.Index_Name
        FROM All_Ind_Columns I1,          all_Ind_Columns I2,          all_Ind_Columns I3,          all_Ind_Columns I4
        WHERE I1.Index_Name    = I2.Index_Name
        AND I2.Index_Name      = I3.Index_Name
        AND I3.Index_Name      = I4.Index_Name
        AND I1.Table_Name      = Upper(Nome_Tabella)
        AND I1.Index_Owner     = Upper(Nomeutente)
        AND I1.Column_Name     =Nome_Colonna1        AND I1.Column_Position = 1
        AND I2.Column_Name     =Nome_Colonna2        AND I2.Column_Position = 2
        AND I3.Column_Name     =Nome_Colonna3        AND I3.Column_Position = 3
        AND I4.Column_Name     =Nome_Colonna4        AND I4.Column_Position = 4
        UNION
        SELECT '00000000n.a.' AS Index_Name FROM Dual ORDER BY 1
        )
      WHERE rownum       =1;
      IF Old_Index_Name <> '00000000n.a.' THEN
        Dbms_Output.Put_Line ('4 colonne e indice esiste già su quelle colonne');
      ELSE -- non esiste indice su quelle colonne
        Colonne := Nome_Colonna1|| ' ,'||Nome_Colonna2|| ' ,'||Nome_Colonna3|| ' ,'||Nome_Colonna4;
      END IF;
    Elsif Nome_Colonna3 IS NOT NULL AND Nome_Colonna2 IS NOT NULL AND Nome_Colonna1 IS NOT NULL THEN
      SELECT Index_Name
      INTO Old_Index_Name
      FROM
        (SELECT I1.Index_Name
        FROM all_Ind_Columns I1,          all_Ind_Columns I2,          all_Ind_Columns I3
        WHERE I1.Index_Name    = I2.Index_Name
        AND I2.Index_Name      = I3.Index_Name
        AND I1.Table_Name      = Upper(Nome_Tabella)
        AND I1.Index_Owner     = Upper(Nomeutente)
        And I1.Column_Name     =Nome_Colonna1        AND I1.Column_Position = 1
        AND I2.Column_Name     =Nome_Colonna2        AND I2.Column_Position = 2
        AND I3.Column_Name     =Nome_Colonna3        AND I3.Column_Position = 3
        UNION
        SELECT '00000000n.a.' AS Index_Name FROM Dual ORDER BY 1
        )
      WHERE rownum       =1;
      IF Old_Index_Name <> '00000000n.a.' THEN
        Dbms_Output.Put_Line ('3 colonne e indice esiste già su quelle colonne');
      ELSE -- non esiste indice su quelle colonne
        Colonne := Nome_Colonna1|| ' ,'||Nome_Colonna2|| ' ,'||Nome_Colonna3;
      END IF;
    Elsif Nome_Colonna2 IS NOT NULL AND Nome_Colonna1 IS NOT NULL THEN
      SELECT Index_Name
      INTO Old_Index_Name
      FROM
        (SELECT I1.Index_Name
        From All_Ind_Columns I1,          All_Ind_Columns I2        
        WHERE I1.Index_Name    = I2.Index_Name
        AND I1.Table_Name      = Upper(Nome_Tabella)
        AND I1.Index_Owner     = Upper(Nomeutente)
        AND I1.Column_Name     =Nome_Colonna1        AND I1.Column_Position = 1
        AND I2.Column_Name     =Nome_Colonna2        AND I2.Column_Position = 2
        UNION
        SELECT '00000000n.a.' AS Index_Name FROM Dual ORDER BY 1
        )
      WHERE Rownum       =1;
      IF Old_Index_Name <> '00000000n.a.' THEN
        Dbms_Output.Put_Line ('2 colonne e indice esiste già su quelle colonne');
      ELSE -- non esiste indice su quelle colonne
        Colonne := Nome_Colonna1||' ,'||Nome_Colonna2;
      END IF;
    Else -- Nome_Colonna1 IS NOT NULL
      Select Index_Name  , Index_Type, Ityp_Name  
      Into Old_Index_Name, old_index_type, old_Ityp_Name
      From
        (Select I1.Index_Name, ind.index_type, ind.Ityp_Name
        From All_Ind_Columns I1, All_Indexes Ind
        Where I1.Index_Name    = Ind.Index_Name
        AND I1.Table_Name    = Upper(Nome_Tabella)
        AND I1.Index_Owner     = Upper(Nomeutente)
        AND I1.Column_Name     =Nome_Colonna1
        And I1.Column_Position = 1
        and ind.index_type <> 'DOMAIN'
        Union
        SELECT '00000000n.a.' AS Index_Name, 'index_type' as index_type,  'Ityp_Name' as Ityp_Name FROM Dual ORDER BY 1
        )
      Where Rownum       =1;
      If (Old_Index_Name <> '00000000n.a.' And Old_Index_Type = Index_Type) Then
        Dbms_Output.Put_Line ('indice, di stesso tipo (dominio o normale), esiste già sulla colonna');
      ELSE -- non esiste indice su quelle colonne
        Colonne := Nome_Colonna1;
      END IF;
    End If;
    -- fine if che cicla su valori delle colonne
    
    IF index_type          = 'DOMAIN' AND OPTIONAL_ITYP_PARAMETERS IS NOT NULL THEN
      Domain_Index_Clause := ' INDEXTYPE IS CTXSYS.'||Ityp_Name||' PARAMETERS ('''||OPTIONAL_ITYP_PARAMETERS||''')';
    Elsif index_type          = 'DOMAIN' THEN
      Domain_Index_Clause := ' INDEXTYPE IS CTXSYS.'||Ityp_Name ;
    ELSE
      Domain_Index_Clause := ' ';
    End If;
    
    IF Old_Index_Name <> '00000000n.a.' and Old_Index_Name <> Nome_Indice and colonne is null THEN -- esiste già indice su quelle colonne, devo solo rinominare
      istruzione      := 'alter index '||Old_Index_Name||' rename to '||Nome_Indice ;
    ELSE -- non esiste indice su quelle colonne, proseguo normale esecuzione.....
      Istruzione := 'CREATE '||Is_Unique||' INDEX ' ||Nomeutente||'.'||Nome_Indice ||' 
      ON '||nome_tabella||'  ('||colonne||') ' 
      ||Domain_Index_Clause;
    END IF;
    Dbms_Output.Put_Line ('istruzione '||Istruzione);
    EXECUTE IMMEDIATE istruzione;
    Utl_Insert_Log (Nomeutente -- nome utente
    , NULL                     -- data
    ,'Added index '||Nome_Indice||' on table '||Nome_Tabella , Versione_Cd , 'esito positivo' );
  ELSE                         -- esiste indice con stesso nome
    utl_insert_log (nomeutente -- nome utente
    , NULL                     -- data
    ,'Adding index '||nome_indice||' on table '||nome_tabella , versione_CD , 'Indice già esistente con stesso nome' );
  END IF;
  -- fine if  count su indici con nome= nome passato come argomento
EXCEPTION
WHEN OTHERS THEN
  dbms_output.put_line ('errore '||SQLERRM);
  errore_msg := SUBSTR(SQLERRM,1,255);
  utl_insert_log (nomeutente -- nome utente
  , NULL                     -- data
  ,'Adding index '||nome_indice||' on table '||nome_tabella , versione_CD , 'esito negativo - '||Errore_Msg||'' );
  RAISE;
END utl_add_index;
/

