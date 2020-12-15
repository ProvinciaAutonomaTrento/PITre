<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" 
	xmlns:ds="http://www.tempuri.org/dsWolfSeco.xsd" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:fo="http://www.w3.org/1999/XSL/Format" 
	xmlns:fox="http://xml.apache.org/fop/extensions"
	exclude-result-prefixes="ds fo fox">
		
	<xsl:output method="xml"/>
		
	<xsl:template match="EXPORT">											
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
									<fo:block text-align="start" font-size="11pt" font-family="Arial">
										<xsl:value-of select="@admin" /> - Stampa del <xsl:value-of select="@date" /> - Righe stampate: <xsl:value-of select="@rows" /> - Pagina: <fo:page-number/> di <fo:page-number-citation ref-id="terminator" />
									</fo:block>  
								</fo:table-cell>
							</fo:table-row>
							<fo:table-row>
								<fo:table-cell>
									<fo:block text-align="start" font-size="9pt" font-family="Arial">
										<xsl:value-of select="@title" />										
									</fo:block>  
								</fo:table-cell>
							</fo:table-row>
						</fo:table-body>
					</fo:table>
				</fo:static-content>															
				<fo:flow flow-name="xsl-region-body">																					
					<fo:block>		
						<fo:table table-layout="fixed">					
							<fo:table-column column-width="60pt" />	
							<fo:table-column column-width="60pt" />	
							<fo:table-column column-width="55pt" />	
							<fo:table-column column-width="230pt" />								
							<fo:table-column column-width="30pt" />								
							<fo:table-column column-width="160pt" />	
							<fo:table-column column-width="120pt" />
							<fo:table-column column-width="55pt" />
              <fo:table-column column-width="30pt" />
              <fo:table-body>		
								<fo:table-row>
									<fo:table-cell background-color="#C0C0C0"  border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
										<fo:block font-family="Arial" font-size="10pt" font-weight="bold" text-align="center">																			
											<fo:inline color="#000000">											
												Registro
											</fo:inline>
										</fo:block>
									</fo:table-cell>
                  <fo:table-cell background-color="#C0C0C0" border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
                    <fo:block font-family="Arial" font-size="10pt" font-weight="bold" text-align="center">
                      <fo:inline color="#000000">
                        Num. documento
                      </fo:inline>
                    </fo:block>
                  </fo:table-cell>
                  <fo:table-cell background-color="#C0C0C0" border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
										<fo:block font-family="Arial" font-size="10pt" font-weight="bold" text-align="center">																			
											<fo:inline color="#000000">											
												Data
											</fo:inline>
										</fo:block>
									</fo:table-cell>
                  <fo:table-cell background-color="#C0C0C0" border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
                    <fo:block font-family="Arial" font-size="10pt" font-weight="bold" text-align="center">
                      <fo:inline color="#000000">
                        Oggetto
                      </fo:inline>
                    </fo:block>
                  </fo:table-cell>
                  <fo:table-cell background-color="#C0C0C0" border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
										<fo:block font-family="Arial" font-size="10pt" font-weight="bold" text-align="center">																			
											<fo:inline color="#000000">											
												Tipo
											</fo:inline>
										</fo:block>
									</fo:table-cell>
                  <fo:table-cell background-color="#C0C0C0" border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
										<fo:block font-family="Arial" font-size="10pt" font-weight="bold" text-align="center">																			
											<fo:inline color="#000000">											
												Mitt. / Dest.
											</fo:inline>
										</fo:block>
									</fo:table-cell>
                  <fo:table-cell background-color="#C0C0C0" border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
                    <fo:block font-family="Arial" font-size="10pt" font-weight="bold" text-align="center">
                      <fo:inline color="#000000">
                        Motivo rimoz.
                      </fo:inline>
                    </fo:block>
                  </fo:table-cell>
								</fo:table-row>											
								<xsl:for-each select="RECORD">	
									<fo:table-row>
										<fo:table-cell border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
											<fo:block font-family="Arial" font-size="10pt" text-align="center">																			
												<fo:inline color="#000000">											
													<xsl:value-of select="COD_REG" />														
												</fo:inline>
											</fo:block>
										</fo:table-cell>
										<fo:table-cell border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">									
											<fo:block font-family="Arial" font-size="10pt" text-align="center">																			
												<fo:inline color="#000000">											
													<xsl:value-of select="NUM_DOCUMENTO" />														
												</fo:inline>
											</fo:block>		
										</fo:table-cell>
										<fo:table-cell border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
											<fo:block font-family="Arial" font-size="10pt" text-align="center">																			
												<fo:inline color="#000000">											
													<xsl:value-of select="DATA_CREAZIONE" />														
												</fo:inline>
											</fo:block>
										</fo:table-cell>
										<fo:table-cell border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
											<fo:block font-family="Arial" font-size="10pt">																			
												<fo:inline color="#000000">											
													<xsl:value-of select="OGGETTO" />														
												</fo:inline>
											</fo:block>
										</fo:table-cell>										
										<fo:table-cell border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
											<fo:block font-family="Arial" font-size="10pt" text-align="center">																			
												<fo:inline color="#000000">											
													<xsl:value-of select="TIPO_DOC" />														
												</fo:inline>
											</fo:block>
										</fo:table-cell>													
										<fo:table-cell border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
											<fo:block font-family="Arial" font-size="10pt">																			
												<fo:inline color="#000000">											
													<xsl:value-of select="MITT_DEST" />														
												</fo:inline>
											</fo:block>
										</fo:table-cell>
										<fo:table-cell border-style="solid" border-color="black" border-width="0.5pt" padding-before="2pt" padding-after="2pt" padding-start="2pt" padding-end="2pt">
											<fo:block font-family="Arial" font-size="10pt">																			
												<fo:inline color="#000000">											
													<xsl:value-of select="NOTE_CEST" />														
												</fo:inline>
											</fo:block>
										</fo:table-cell>
                  </fo:table-row>																																					
								</xsl:for-each> 
							</fo:table-body>
						</fo:table>                                          
					</fo:block>
					<fo:block id="terminator" />
				</fo:flow>
			</fo:page-sequence>
		</fo:root>
	</xsl:template>
</xsl:stylesheet>
