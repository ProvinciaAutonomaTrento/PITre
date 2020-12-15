<%			
	if (!(SearchResult.PageRecipient == null || SearchResult.PageRecipient.Length == 0))
	{
%>
		<form method="post" action="Rubrica.aspx">
			<!--input type="hidden" id="add" name="action" value="add" /-->
<%			
		if (Properties.Capacity==RecipientCapacity.Many)		
		{		
%>					
			<p>
				<a href="./Rubrica.aspx?action=add&amp;selection=all">Aggiungi Tutti</a>
			</p>
<%			
		}
%>					
			<table summary="Elenco dei risultati della ricerca in rubrica" id="tbl">
				<caption>Elenco dei corrispondenti trovati in rubrica</caption>
				<thead>
    				<tr>
						<th scope="col">Tipologia</th>
						<th scope="col">Codice</th>
						<th scope="col">Descrizione</th>
						<th scope="col" colspan="2">Azioni</th>
					</tr>

				</thead>
				<tfoot>
					<tr>
						<td colspan="5">
						
								pagina <%=(SearchResult.PageNumber+1)%> di <%=SearchResult.PageCount%>
								
							<%								
								if (SearchResult.PageNumber>0)
								{
							%>	
							
									<a href="./Rubrica.aspx?action=chpage&amp;table=search&amp;pgnum=<%=(SearchResult.PageNumber-1)%>" class="img-link">Precedente</a>
							<%								
								}							
								if (SearchResult.PageNumber<(SearchResult.PageCount-1))
								{
							%>	
									<a href="./Rubrica.aspx?action=chpage&amp;table=search&amp;pgnum=<%=(SearchResult.PageNumber+1)%>" class="img-link">Successiva</a>																	
							<%								
								}
							%>	
						</td>
					</tr>
				</tfoot>
				<tbody>
				<%							
					foreach (DocsPAWA.DocsPaWR.ElementoRubrica er in SearchResult.PageRecipient)
					{		
						string name = (Properties.Capacity==RecipientCapacity.Many) ? er.codice : "elem";
						string id = "id"+er.codice.Replace(" ","-");
				%>					
					<tr>
						<td>
							<%=this.ParseCorrType(er.tipo)%>
						</td>
						<td>
							<%=er.codice%>						
						</td>
						<td>
							<%=er.descrizione%>						
						</td>
						<td>
							<a href="./Dettagli.aspx?cod=<%=this.Escape(er.codice)%>" class="img-link" title="Visualizza i dettagli del corrispondente">Dettagli</a>
						</td>
						<td>
							<a href="./Rubrica.aspx?action=add&amp;selection=single&amp;cod=<%=this.Escape(er.codice)%>" class="img-link">Seleziona</a>
						</td>
					</tr>
				<%								
					}
				%>	
				</tbody>
			</table>
			<%			
				if (Properties.Capacity==RecipientCapacity.Many)		
				{		
			%>
					<p>
						<a href="./Rubrica.aspx?action=add&amp;selection=all">Aggiungi Tutti</a>
					</p>
			<%			
				}
			%>					
		</form>
<%				
	}
%>
