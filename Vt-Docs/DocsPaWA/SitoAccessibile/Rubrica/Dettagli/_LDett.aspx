<%
	const int MAX_TABLE_LENGTH = 10;
	DocsPAWA.DocsPaWR.Corrispondente[] lista = Corrispondenti;

	int min = 0;
	double div = (double)lista.Length/(double)MAX_TABLE_LENGTH;
	int max = (int)Math.Round(div+0.49);
	int curr = 0;
	try
	{
		curr = Int32.Parse((string)this.Context.Request.Params["pg"]);
	}
	catch (Exception) {}
	
	DocsPAWA.DocsPaWR.Corrispondente[] items = new DocsPAWA.DocsPaWR.Corrispondente[0];
	if (curr>=min && curr<=max)
	{
		int minIdx = curr*MAX_TABLE_LENGTH;
		int maxIdx = Math.Min((minIdx + MAX_TABLE_LENGTH),lista.Length);
		items = new DocsPAWA.DocsPaWR.Corrispondente[maxIdx-minIdx];
		for (int i=minIdx; i<maxIdx; i++)
			items[i-minIdx] = lista[i];
	}
	
%>					
						<table summary="Dettagli del corrispondente selezionato" id="tbl">
							<caption>Lista '<%=ElementoRubrica.descrizione%>'</caption>
							<thead>
			    				<tr>
									<th scope="col">Codice</th>
									<th scope="col">Descrizione</th>
									<th scope="col">Azioni</th>
								</tr>
							</thead>
							<tfoot>
								<tr>
									<th colspan="3">
<%								
	if (curr>0)
	{
%>	
										<a href="./Dettagli.aspx?cod=<%=Codice%>&amp;pg=<%=curr-1%>">&lt;&lt;</a>
<%								
	}
%>	
										pagina <%=(curr+1)%> di <%=max%>
<%								
	if (curr<(max-1))
	{
%>	
										<a href="./Dettagli.aspx?cod=<%=Codice%>&amp;pg=<%=curr+1%>">&gt;&gt;</a>
<%								
	}
%>	
									
									</th>
								</tr>
							</tfoot>
							<tbody>
<%
	foreach (DocsPAWA.DocsPaWR.Corrispondente c in items)
	{
%>	
								<tr>
									<td scope="row">
										<%=(c.codiceRubrica!=null) ? c.codiceRubrica : "&nbsp;"%>
									</td>
									<td>
										<%=(c.descrizione!=null) ? c.descrizione : "&nbsp;"%>
									</td>
									<td>
										<a href="./Dettagli.aspx?cod=<%=c.codiceRubrica%>&amp;er=<%=ElementoRubrica.codice%>">Vedi Dettagli</a>
									</td>										
								</tr>
<%
	}
%>	
						</tbody>
						</table>
