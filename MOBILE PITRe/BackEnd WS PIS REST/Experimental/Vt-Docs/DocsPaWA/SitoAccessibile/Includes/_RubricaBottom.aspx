<%			
	if (!(Result.PageRecipient == null || Result.PageRecipient.Length == 0))
	{	
%>
		<form method="post" action="Rubrica.aspx">
			<input type="hidden" id="remove" name="action" value="remove" />
				<table id="tbl-c" summary="Elenco dei corrispondenti selezionati dalla rubrica">
					<caption>
						Elenco dei corrispondenti selezionati dalla rubrica
					</caption>
					<thead>
						<tr>
							<th scope="col">Tipologia</th>
							<th scope="col">Codice</th>
							<th scope="col">Descrizione</th>
							<th scope="col">Email</th>
							<th scope="col" colspan="2">Azioni</th>
						</tr>
					</thead>
					<tfoot>
						<tr>
							<td colspan="6">
								<%								
									if (Result.PageNumber>0)
									{
								%>
								<a href="./Rubrica.aspx?action=chpage&amp;table=recipient&amp;pgnum=<%=(Result.PageNumber-1)%>" class="img-link">
									&lt;&lt;</a>
								<%								
									}
								%>
								<span>
									pagina <%=(Result.PageNumber+1)%> di <%=Result.PageCount%>
								</span>
								<%								
									if (Result.PageNumber<(Result.PageCount-1))
									{
								%>
								<a href="./Rubrica.aspx?action=chpage&amp;table=recipient&amp;pgnum=<%=(Result.PageNumber+1)%>" class="img-link">
									&gt;&gt;</a>
								<%								
									}
								%>
							</td>
						</tr>
					</tfoot>
					<tbody>
					<%							
						foreach (DocsPAWA.DocsPaWR.Corrispondente corr in Result.PageRecipient)
						{		
							string name = corr.codiceRubrica;
							string id = "id"+corr.systemId;
					%>
						<tr>
							<td>
								<%=this.ParseCorrType(corr.tipoCorrispondente)%>
							</td>
							<td>
								<%=corr.codiceRubrica%>
							</td>
							<td>
								<%=corr.descrizione%>
							</td>
							<td>
								<%=(corr.email!=null && corr.email!="") ? corr.email : "&nbsp;"%>
							</td>
							<td>
								<a href="./Rubrica.aspx?action=remove&amp;selection=single&amp;cod=<%=this.Escape(corr.codiceRubrica)%>" class="img-link">
									Rimuovi</a>
							</td>
							<td>
								<a href="./Rubrica.aspx?action=remove&amp;selection=all">Rimuovi Tutti</a>
							</td>
						</tr>
					<%								
						}
					%>
					</tbody>
				</table>
		</form>
<%		
	}
%>
