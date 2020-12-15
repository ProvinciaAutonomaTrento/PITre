<%
	DocsPAWA.DocsPaWR.UnitaOrganizzativa myCorr = (DocsPAWA.DocsPaWR.UnitaOrganizzativa)Corrispondenti[0];
%>
						<table summary="Dettagli del corrispondente selezionato" id="tbl">
							<caption>Unit&agrave; Organizzativa "<%=myCorr.descrizione%>"</caption>
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
									<td scope="row">Indirizzo</td>
									<td>
										<%=(myCorr.indirizzo!=null && myCorr.indirizzo!="") ? myCorr.indirizzo : "&nbsp;"%>
									</td>
								</tr>
								<tr>
									<td scope="row">
										<acronym title="Codice avviamento postale">CAP</acronym>
									</td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td scope="row">Citt&agrave;</td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td scope="row">Provincia</td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td scope="row">Nazione</td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td scope="row">Telefono (1)</td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td scope="row">Telefono (2)</td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td scope="row">Fax</td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td scope="row">Codice fiscale / Partita IVA</td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td scope="row">Email</td>
									<td>
										<%=(myCorr.email!=null && myCorr.email!="") ? myCorr.email : "&nbsp;"%>
									</td>
								</tr>
							</tbody>
						</table>
