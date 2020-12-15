CREATE OR REPLACE PROCEDURE dpa3_get_hierarchy (
	   p_id_amm in varchar,
	   p_cod in varchar,
	   p_tipo_ie in varchar,
	   p_codes out varchar)

as

begin
  declare
	v_c_type varchar(2);
	v_p_cod varchar(64);
	v_system_id int;
	v_id_parent int;
	v_id_uo int;
	v_id_utente int;

  begin

	p_codes := '';
	v_p_cod := p_cod;

	select
		 id_parent, system_id, id_uo, id_people, cha_tipo_urp
	into
		 v_id_parent, v_system_id, v_id_uo, v_id_utente, v_c_type
	from
		 dpa_corr_globali
	where
		 var_cod_rubrica = v_p_cod and
		 cha_tipo_ie = p_tipo_ie and
		 id_amm = p_id_amm and
		 dta_fine is null;


	while (1 > 0) loop
		if v_c_type = 'U' then
		   if (v_id_parent is null or v_id_parent = 0) then
		   	  exit;
		   end if;

		   select var_cod_rubrica, system_id into v_p_cod, v_system_id from dpa_corr_globali where system_id = v_id_parent and id_amm = p_id_amm and dta_fine is null;
		end if;

		if v_c_type = 'R' then
		    if (v_id_uo is null or v_id_uo = 0) then
			   exit;
			end if;

			select var_cod_rubrica, system_id into v_p_cod, v_system_id from dpa_corr_globali where system_id = v_id_uo and id_amm = p_id_amm and dta_fine is null ;
		end if;

		if v_c_type = 'P' then
		   select var_cod_rubrica into v_p_cod from dpa_corr_globali where id_gruppo = 
		   		  (	
					  case when exists
					  	  (select groups_system_id from peoplegroups where people_system_id = v_id_utente and cha_preferito = 1)
					  then 
					  	  (select groups_system_id from peoplegroups where people_system_id = v_id_utente  and cha_preferito = 1)
					  else
					  	  (select max(groups_system_id) from peoplegroups where people_system_id = v_id_utente and dta_fine is null)
					  end 
				  ) and id_amm = p_id_amm and dta_fine is null;
		end if;
		if v_p_cod is null then
			exit;
		end if;
		
		select
			   id_parent, system_id, id_uo, cha_tipo_urp
		into
			   v_id_parent, v_system_id, v_id_uo, v_c_type
		from
			   dpa_corr_globali
		where
			   var_cod_rubrica = v_p_cod and
			   id_amm = p_id_amm and
			   dta_fine is null;

		p_codes := v_p_cod || ':' || p_codes;
	end loop;

	p_codes := p_codes || p_cod;

  end;
end;
/