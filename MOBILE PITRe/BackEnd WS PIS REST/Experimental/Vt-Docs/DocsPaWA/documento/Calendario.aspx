<%@ Page language="c#" Codebehind="Calendario.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.Calendario" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <HEAD>
		<title>Seleziona...</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<style> td {font-family: Verdana, sans-serif; font-size: 9px;}
		</style>
		<script language="JavaScript">
	
		// months as they appear in the calendar's title
		var ARR_MONTHS = ["Gennaio", "Febbraio", "Marzo", "Aprile", "Maggio", "Giugno", "Luglio", "Agosto", "Settembre", "Ottobre", "Novembre", "Dicembre"];

		// week day titles as they appear on the calendar
		var ARR_WEEKDAYS = ["Dom", "Lun", "Mar", "Mer", "Gio", "Ven", "Sab"];

		// day week starts from (normally 0-Su or 1-Mo)
		var NUM_WEEKSTART = 1;

		// path to the directory where calendar images are stored. trailing slash req.
		var STR_ICONPATH = '../images/';

		var re_url = new RegExp('datetime=(\\-?\\d+)');

		var dt_current = (re_url.exec(String(window.location))

			? new Date(new Number(RegExp.$1)) : new Date());

		var re_id = new RegExp('id=(\\d+)');

		var num_id = (re_id.exec(String(window.location))

			? new Number(RegExp.$1) : 0);

		var obj_caller = (window.opener ? window.opener.calendars[num_id] : null);

		if (obj_caller && obj_caller.year_scroll) {
			// get same date in the previous year
			var dt_prev_year = new Date(dt_current);

			dt_prev_year.setFullYear(dt_prev_year.getFullYear() - 1);

			if (dt_prev_year.getDate() != dt_current.getDate())
				dt_prev_year.setDate(0);
				
			// get same date in the next year

			var dt_next_year = new Date(dt_current);

			dt_next_year.setFullYear(dt_next_year.getFullYear() + 1);

			if (dt_next_year.getDate() != dt_current.getDate())
				dt_next_year.setDate(0);
		}

		// get same date in the previous month
		var dt_prev_month = new Date(dt_current);

		dt_prev_month.setMonth(dt_prev_month.getMonth() - 1);

		if (dt_prev_month.getDate() != dt_current.getDate())
			dt_prev_month.setDate(0);

		// get same date in the next month
		var dt_next_month = new Date(dt_current);

		dt_next_month.setMonth(dt_next_month.getMonth() + 1);

		if (dt_next_month.getDate() != dt_current.getDate())
			dt_next_month.setDate(0);

		// get first day to display in the grid for current month
		var dt_firstday = new Date(dt_current);

		dt_firstday.setDate(1);

		dt_firstday.setDate(1 - (7 + dt_firstday.getDay() - NUM_WEEKSTART) % 7);

		// function passing selected date to calling window
		function set_datetime(n_datetime, b_close) {

			if (!obj_caller) return;

			var dt_datetime = obj_caller.prs_time(
				(document.cal ? document.cal.time.value : ''),
				new Date(n_datetime)
			);

			if (!dt_datetime) return;

			if (b_close) {
				obj_caller.target.value = (document.cal
					? obj_caller.gen_tsmp(dt_datetime)
					: obj_caller.gen_date(dt_datetime)
				);
				
				//Controllo se la pagina chiamante è l'anteprima delle profilazione in quanto
				//in questo caso la stringa della data deve essere spezzata in tre e settati 
				//i campi giorno mese e anno
				
				if( (window.opener).document.title.indexOf('Anteprima') != -1) {	
					
					var oggetto_array = obj_caller.target.name.split("_");
					var idOggetto = oggetto_array[2];
					var campoGiorno = "giorno"+idOggetto;
					var campoMese = "mese"+idOggetto;
					var campoAnno = "anno"+idOggetto;
					
					var data_array = obj_caller.target.value.split("/");
					var giorno = data_array[0];
					var mese = data_array[1];
					var anno = data_array[2];
					
					window.opener.document.getElementById(campoGiorno).value = giorno;
					window.opener.document.getElementById(campoMese).value = mese;
					window.opener.document.getElementById(campoAnno).value = anno;					
				}
				//Fine controllo anteprima profilazione
								
				window.close();				
			}
			else obj_caller.popup(dt_datetime.valueOf());
		}
		</script>
	</HEAD>
	<body bgcolor="#ffffff" bottomMargin="0" leftMargin="3" topMargin="3" rightMargin="3" MS_POSITIONING="GridLayout">
		<table class="clsOTable" cellspacing="0" border="0" width="100%">
			<tr>
				<td bgcolor="#800000">
					<table cellspacing="1" cellpadding="3" border="0" width="100%">
						<tr>
							<td colspan="7"><table cellspacing="0" cellpadding="0" border="0" width="100%">
									<tr>
										<script language="JavaScript">
											document.write(
											'<td>'+(obj_caller&&obj_caller.year_scroll?'<a href="javascript:set_datetime('+dt_prev_year.valueOf()+')"><img src="'+STR_ICONPATH+'prev_year.gif" width="16" height="16" border="0" alt="anno precedente"></a>&nbsp;':'')+'<a href="javascript:set_datetime('+dt_prev_month.valueOf()+')"><img src="'+STR_ICONPATH+'prev.gif" width="16" height="16" border="0" alt="mese precedente"></a></td>'+
											'<td align="center" width="100%"><font color="#ffffff">'+ARR_MONTHS[dt_current.getMonth()]+' '+dt_current.getFullYear() + '</font></td>'+
											'<td><a href="javascript:set_datetime('+dt_next_month.valueOf()+')"><img src="'+STR_ICONPATH+'next.gif" width="16" height="16" border="0" alt="mese successivo"></a>'+(obj_caller && obj_caller.year_scroll?'&nbsp;<a href="javascript:set_datetime('+dt_next_year.valueOf()+')"><img src="'+STR_ICONPATH+'next_year.gif" width="16" height="16" border="0" alt="anno successivo"></a>':'')+'</td>'
											);
										</script>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<script language="JavaScript">
								// print weekdays titles
								for (var n=0; n<7; n++)
									document.write('<td bgcolor="#C0C0C0" align="center"><font color="#ffffff">'+ARR_WEEKDAYS[(NUM_WEEKSTART+n)%7]+'</font></td>');
									document.write('</tr>');

								// print calendar table
								var dt_current_day = new Date(dt_firstday);

								while (dt_current_day.getMonth() == dt_current.getMonth() || dt_current_day.getMonth() == dt_firstday.getMonth()) {

									// print row heder
									document.write('<tr>');

									for (var n_current_wday=0; n_current_wday<7; n_current_wday++) {

										if (dt_current_day.getDate() == dt_current.getDate() && dt_current_day.getMonth() == dt_current.getMonth())
											// print current date
											document.write('<td bgcolor="#AD736B" align="center" width="14%">');
										else if (dt_current_day.getDay() == 0 || dt_current_day.getDay() == 6)
											// weekend days
											document.write('<td bgcolor="#E0E0E0" align="center" width="14%">');
										else
											// print working days of current month
											document.write('<td bgcolor="#ffffff" align="center" width="14%">');

										document.write('<a href="javascript:set_datetime('+dt_current_day.valueOf() +', true);">');

										if (dt_current_day.getMonth() == this.dt_current.getMonth())
											// print days of current month
											document.write('<font color="#000000">');
										else 
											// print days of other months
											document.write('<font color="#606060">');											

										document.write(dt_current_day.getDate()+'</font></a></td>');
										dt_current_day.setDate(dt_current_day.getDate()+1);
									}
									// print row footer
									document.write('</tr>');
								}

								if (obj_caller && obj_caller.time_comp)
									document.write('<form onsubmit="javascript:set_datetime('+dt_current.valueOf()+', true)" name="cal"><tr><td colspan="7" bgcolor="#87CEFA"><font color="White" face="Verdana" size="2">Time: <input type="text" name="time" value="'+obj_caller.gen_time(this.dt_current)+'" size="8" maxlength="8"></font></td></tr></form>');
							</script>
						</tr>
					</table>
				</td>
				</TD></tr>
		</table>
	</body>
</html>
