<table id="tbl-doc" summary="Elenco dei documenti grigi trovati">
	<caption>Elenco documenti grigi trovati</caption>

	<thead>
		<tr>
			<th scope="col">Numero e Data Documento</th>
			<th scope="col">Oggetto</th>
			<th scope="col">Azione</th>
		</tr>

	</thead>
	<tfoot>
		<tr>
			<td colspan="3">
				<span class="etichetta">
					<label>pagina <%=searchDocOutcome.PageNumber%> di <%=searchDocOutcome.PageCount%></label>
				</span>	
			<%								
				if (searchDocOutcome.PageNumber>1)
				{
			%>	
			
				<a href="./EsitoRicercaDocumenti.aspx?action=chpage&amp;table=tbl-doc&amp;pgnum=<%=(searchDocOutcome.PageNumber-1)%>" class="img-link">precedente</a>
			<%								
				}
											
				if (searchDocOutcome.PageNumber<searchDocOutcome.PageCount)
				{
			%>	
				<a href="./EsitoRicercaDocumenti.aspx?action=chpage&amp;table=tbl-doc&amp;pgnum=<%=(searchDocOutcome.PageNumber+1)%>" class="img-link">successiva</a>																	
			<%								
				}
			%>	
			</td>
		</tr>
	</tfoot>
	<tbody>
	<%							
		foreach (DocsPAWA.DocsPaWR.InfoDocumento doc in searchDocOutcome.Documenti)
		{		
	%>					
		<tr>
			<td>
				<%=doc.idProfile%>
				del
				<%=doc.dataApertura%>
			</td>
			<td>
				<%=doc.oggetto%>
			</td>
			<td>
				<a href="../Documenti/DettagliDocumento.aspx?iddoc=<%=doc.idProfile%>&amp;docnum=<%=doc.docNumber%>&amp;caller=SearchDocuments">Apri</a>							
			</td>
		</tr>
	<%								
		}
	%>	
	</tbody>
</table>