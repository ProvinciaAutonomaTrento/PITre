<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" 
	xmlns:ds="http://www.tempuri.org/dsWolfSeco.xsd" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:fo="http://www.w3.org/1999/XSL/Format" 
	xmlns:fox="http://xml.apache.org/fop/extensions"
	exclude-result-prefixes="ds fo fox">
		
	<xsl:output method="xml"/>
		
	<xsl:template match="ORGANIGRAMMA">											
		<fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format">			
			<fo:layout-master-set>
				<fo:simple-page-master 
					master-name="A4-landscape" 
					page-width="297mm" 
					page-height="210mm" 
					margin-top="5mm"
					margin-bottom="5mm" 
					margin-left="5mm" 
					margin-right="5mm">
					<fo:region-body 
						margin-top="10mm" 
						margin-bottom="5mm" 
						margin-left="0mm" 
						margin-right="0mm" />
					<fo:region-start region-name="alto" />
				</fo:simple-page-master>				
			</fo:layout-master-set>
			<fo:page-sequence master-reference="A4-landscape">																
				
				<fo:static-content flow-name="alto">					
					<fo:table>						
						<fo:table-column column-width="600pt" />					
						<fo:table-body>
							<fo:table-row>
								<fo:table-cell>
									<fo:block text-align="start" font-size="12pt" font-family="Courier">
										DocsPA - <xsl:value-of select="@title" /> - Pagina: <fo:page-number/> di <fo:page-number-citation ref-id="terminator" />
									</fo:block>  
								</fo:table-cell>
							</fo:table-row>
						</fo:table-body>
					</fo:table>
				</fo:static-content>															
				<fo:flow flow-name="xsl-region-body">																					
					<fo:block>					
						<xsl:for-each select="RECORD">		
							<xsl:choose>		
								<xsl:when test="@tipo='U'">
									<fo:block font-family="Courier" font-size="12pt">																			
										<fo:inline color="#000000" font-weight="bold">											
											<xsl:value-of select="@desc" />												
										</fo:inline>
									</fo:block>	
								</xsl:when>								
								<xsl:otherwise>
									<fo:block font-family="Courier" font-size="12pt">																			
										<fo:inline color="#000000">											
											<xsl:value-of select="@desc" />												
										</fo:inline>
									</fo:block>	
								</xsl:otherwise>
							</xsl:choose>																																															
                        </xsl:for-each>                                            
					</fo:block>
					<fo:block id="terminator" />
				</fo:flow>
			</fo:page-sequence>
		</fo:root>
	</xsl:template>
</xsl:stylesheet>
