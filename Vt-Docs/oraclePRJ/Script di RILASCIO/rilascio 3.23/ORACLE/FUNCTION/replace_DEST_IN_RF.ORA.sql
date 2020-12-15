begin
	utl_backup_sp('DEST_IN_RF','3.23');
end;
/

Create Or Replace Function          Dest_In_Rf (Idprofile Int)
RETURN varchar IS risultato varchar2(32767); -- max length for varchar2 is 32767

/*item clob;

CURSOR cur IS
SELECT B.VAR_DESC_CORR
FROM DPA_DOC_ARRIVO_PAR L, DPA_L_RUOLO_REG C, DPA_CORR_GLOBALI B, DPA_CORR_GLOBALI D
WHERE
L.ID_PROFILE = IdProfile AND
L.CHA_TIPO_MITT_DEST = 'F' AND
L.ID_MITT_DEST = D.SYSTEM_ID AND
D.ID_RF = C.ID_REGISTRO AND
C.ID_RUOLO_IN_UO = B.SYSTEM_ID; */

Begin
Risultato := Null;
/*OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

risultato := risultato||item || '(D) ';

END LOOP;*/

With Data As
-- Rif. http://www.oracletips.info/Concatenating_Multiple_Rows_Into_A_Single_String.htm 
  (SELECT myvalues,    row_number() over (order by myvalues) rn,    COUNT(*) over () cnt
  FROM
    (SELECT B.VAR_DESC_CORR myvalues 
    FROM DPA_DOC_ARRIVO_PAR L,
      DPA_L_RUOLO_REG C,
      DPA_CORR_GLOBALI B,
      Dpa_Corr_Globali D
    WHERE L.ID_PROFILE       =  IdProfile  
    AND L.CHA_TIPO_MITT_DEST = 'F'
    AND L.ID_MITT_DEST       = D.SYSTEM_ID
    AND D.ID_RF              = C.ID_REGISTRO
    AND C.ID_RUOLO_IN_UO     = B.SYSTEM_ID
    )
  )
SELECT ltrim(sys_connect_by_path(myvalues, '(D) '),'(D)') into risultato 
FROM data
WHERE rn              = cnt
  START WITH Rn       = 1
  Connect By Prior Rn = Rn-1;

RETURN risultato;
END DEST_IN_RF; 
/

