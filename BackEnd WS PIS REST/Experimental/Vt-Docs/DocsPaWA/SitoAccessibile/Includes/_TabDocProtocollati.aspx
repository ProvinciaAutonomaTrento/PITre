<table id="tbl-prot" summary="Elenco dei documenti protocollati trovati">
	<caption>Elenco documenti protocollati trovati</caption>

	<thead>
		<tr>
			<th scope="col">Numero e Data del Protocollo</th>
			<th scope="col" style="width: 7%;">Registro</th>
			<th scope="col">Direzione del protocollo</th>
			<th scope="col">Oggetto</th>
			<th scope="col">Mittente/Destinatario</th>
			<th scope="col" style="width: 7%;">Azione</th>
		</tr>

	</thead>
	<tfoot>
		<tr>
			<td colspan="6">
				<span class="etichetta">
					<label>pagina <%=searchProtOutcome.PageNumber%> di <%=searchProtOutcome.PageCount%></label>
				</span>
			<%								
				if (searchProtOutcome.PageNumber>1)
				{
			%>	
			
				<a href="./EsitoRicercaDocumenti.aspx?action=chpage&amp;table=tbl-prot&amp;pgnum=<%=(searchProtOutcome.PageNumber-1)%>" class="img-link">precedente</a>
			<%								
				}							
				if (searchProtOutcome.PageNumber<searchProtOutcome.PageCount)
				{
			%>	
				<a href="./EsitoRicercaDocumenti.aspx?action=chpage&amp;table=tbl-prot&amp;pgnum=<%=(searchProtOutcome.PageNumber+1)%>" class="img-link">successiva</a>																	
			<%								
				}
			%>	
			</td>
		</tr>
	</tfoot>
	<tbody>
	<%							
		foreach (DocsPAWA.DocsPaWR.InfoDocumento doc in searchProtOutcome.Documenti)
		{		
	%>					
		<tr>
			<td>
				<%=doc.numProt%>
				del
				<%=doc.dataApertura%>
			</td>
			<td>
				<%=(doc.codRegistro!=null && doc.codRegistro!="") ? doc.codRegistro : "-"%>
			</td>
			<td>
				<%=(doc.tipoProto!=null && doc.tipoProto!="") ? doc.tipoProto : "-"%>
			</td>
			<td>
				<%=doc.oggetto%>
			</td>
			<td>
				<%=(doc.mittDest!=null && doc.mittDest.Length>0) ? CorrCat(doc.mittDest) : "-"%>
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