<%
	DocsPAWA.DocsPaWR.Utente myCorr = (DocsPAWA.DocsPaWR.Utente)Corrispondenti[0];
%>
						<table summary="Dettagli del corrispondente selezionato" id="tbl">
							<caption>Utente Organizzativa "<%=myCorr.descrizione%>"</caption>
							<thead>
			    				<tr>
									<th scope="col" class="noteDetailsItem">Dettaglio</th>
									<th scope="col" class="noteDetailsValue">Valore</th>
								</tr>
							</thead>
							<tfoot>
								<tr>
									<th colspan="2">&nbsp;</th>
								</tr>
							</tfoot>
							<tbody>
								<tr>
									<td scope="row">Codice Amministrativo</td>
									<td> 
										<%=(myCorr.codiceAmm!=null && myCorr.codiceAmm!="") ? myCorr.codiceAmm : "&nbsp;"%>
									</td>
								</tr>
								<tr>
									<td scope="row">
										Codice <acronym title="Area Organizzativa Omogenea">AOO</acronym>
									</td>
									<td>
										<%=(myCorr.codiceAOO!=null && myCorr.codiceAOO!="") ? myCorr.codiceAOO : "&nbsp;"%>
									</td>
								</tr>
								<tr>
									<td scope="row">Codice Corrispondente</td>
									<td>
										<%=(myCorr.codiceRubrica!=null && myCorr.codiceRubrica!="") ? myCorr.codiceRubrica : "&nbsp;"%>
									</td>
								</tr>
								<tr>
									<td scope="row">Descrizione</td>
									<td>
										<%=(myCorr.descrizione!=null && myCorr.descrizione!="") ? myCorr.descrizione : "&nbsp;"%>
									</td>
								</tr>
								<tr>
									<td scope="row">Nome</td>
									<td>
										<%=(myCorr.nome!=null && myCorr.nome!="") ? myCorr.nome : "&nbsp;"%>
									</td>
								</tr>
								<tr>
									<td scope="row">Cognome</td>
									<td>
										<%=(myCorr.cognome!=null && myCorr.cognome!="") ? myCorr.cognome : "&nbsp;"%>
									</td>
								</tr>
								<tr>
									<td scope="row">Sede</td>
									<td>
										<%=(myCorr.sede!=null && myCorr.sede!="") ? myCorr.sede : "&nbsp;"%>
									</td>
								</tr>
								<tr>
									<td scope="row">Telefono</td>
									<td>
										<%=(myCorr.telefono!=null && myCorr.telefono!="") ? myCorr.telefono : "&nbsp;"%>
									</td>
								</tr>
								<tr>
									<td scope="row">Email</td>
									<td>
										<%=(myCorr.email!=null && myCorr.email!="") ? myCorr.email : "&nbsp;"%>
									</td>
								</tr>
							</tbody>
						</table>
