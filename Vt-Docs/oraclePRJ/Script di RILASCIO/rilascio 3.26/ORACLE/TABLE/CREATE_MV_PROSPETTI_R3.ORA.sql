
begin
-- Prospetti by De Luca
	declare cnt int;
        begin

           select count(*) into cnt from user_mviews
                  Where mview_Name='MV_PROSPETTI_R3';

        if (cnt = 0) then
          execute immediate '
              CREATE MATERIALIZED VIEW MV_PROSPETTI_R3
              USING INDEX
              Refresh Complete
              Start With Trunc(Sysdate) + 1 + 24/1440
                    next trunc(sysdate) + 1 + 24/1440
              DISABLE QUERY REWRITE AS
              Select cg.Id_Amm , P.Id_Registro , Num_Anno_Proto, Count(*) Totprotuo, cg.System_Id, cg.Var_Desc_Corr
              From  Profile P Join Dpa_Corr_Globali Cg On Id_Uo_Prot = Cg.System_Id
                              Join Dpa_El_Registri  El On P.Id_Registro = El.System_Id and cha_rf=''0''
              Where  Cha_Da_Proto = ''0'' And Nvl(Cha_In_Cestino,''0'') = ''0''
              Group By Cg.Id_Amm ,P.Id_Registro , Num_Anno_Proto, Cg.Var_Desc_Corr, Cg.System_Id ';
         end if;

         end;
END;
/