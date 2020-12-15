package Models;


import java.io.StringReader;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.xml.sax.InputSource;

import com.sun.org.apache.xpath.internal.XPathAPI;

import Models.Util;

public class ModelResponse {
	public ModelContent  m_model = new ModelContent();
	
	public String m_documentId = null;
	public String m_exception = null;
	public ModelProcessorInfo  m_processorInfo = new ModelProcessorInfo();
	public List<IncludeSection> m_sections = new ArrayList<IncludeSection>(); 

	public Document loadXMLFromString(String xml) throws Exception
	{
	    DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();

	    factory.setNamespaceAware(true);
	    factory.setIgnoringElementContentWhitespace(true);
	    DocumentBuilder builder = factory.newDocumentBuilder();

	    return builder.parse(new InputSource(new StringReader(xml)));
	}
	
	
	private void saveTempFile(FileContent f) throws Exception{
	    String modelFilePath = null;
	    
	   
	    modelFilePath = Util.GetTempFilePath(f.m_fileName);
	    
	    
	    
	    Util.saveBinaryData(modelFilePath, f.m_content);

	}
	
	
	private boolean isWhiteSPaceNode(Node n){
	boolean ret = false;
	
	if (n.getNodeType() == Node.TEXT_NODE){
		String val = n.getNodeValue();
		ret = val.trim().length() == 0;
	}
	else ret = false;
	
	return ret;
		
		
	}

	
	public boolean initialize(String xmlText){
		boolean ret = false;
		Document document = null;
		
		try{
			document = loadXMLFromString(xmlText);
			
			m_documentId = getAttributeValue(document, ".//modelResponse", "documentId");
			m_exception = getAttributeValue(document, ".//modelResponse", "exception");
	    
			//' Parsing delle informazioni relative al word processor
			m_processorInfo.m_id = Long.parseLong(getAttributeValue(document, ".//processorInfo", "id"));
			m_processorInfo.m_name = getAttributeValue(document, ".//processorInfo", "name");
			m_processorInfo.m_classId = getAttributeValue(document, ".//processorInfo", "classId");
			m_processorInfo.m_supportedExtensions = getAttributeValue(document, ".//processorInfo", "supportedExtensions");
	    
		    //' Parsing del modello utilizzato e del suo contentuto
		    m_model.m_modelType = getAttributeValue(document, ".//model", "modelType");
		    m_model.m_file.m_fileName = getAttributeValue(document, ".//model/file", "name");
		    
		    m_model.m_file.m_content = getAttributeValue(document, ".//model/file", "content");
		    
		    //saveTempFile(m_model.m_file);
	    
		    //' Parsing dei tag con i relativi valori da sostituire nel modello
		    Node pairsNode = XPathAPI.selectSingleNode(document, ".//keyValuePairs");

		    Node pairNode = null;
		    if (pairsNode  != null) {
		    	for (int i = 0; i< pairsNode.getChildNodes().getLength(); i++){
		    		pairNode = pairsNode.getChildNodes().item(i);
		    		if (!isWhiteSPaceNode(pairNode)){
		    		m_model.map.put(pairNode.getAttributes().getNamedItem("key").getTextContent(),
		    						pairNode.getAttributes().getNamedItem("value").getTextContent());
		    		System.out.println("pair [key:" + pairNode.getAttributes().getNamedItem("key").getTextContent()+ ", value:" + pairNode.getAttributes().getNamedItem("value").getTextContent() + "]");
		    		}
		    	}
		    }
	    
		    //' Sezioni (provenienti da un'altro documento) da includere nel documento da elaborare
		    Node includeSectionsNode = XPathAPI.selectSingleNode(document, ".//includeSections");

		    Node sectionNode = null;
		    IncludeSection sect = null;
		    if (includeSectionsNode  != null) {
		    	for (int i = 0; i< includeSectionsNode.getChildNodes().getLength(); i++){
		    		sectionNode = includeSectionsNode.getChildNodes().item(i);
		    		sect = new IncludeSection();
		    		
		    		if (!isWhiteSPaceNode(sectionNode)){
			    		sect.m_begin = sectionNode.getAttributes().getNamedItem("begin").getTextContent();
			    		sect.m_end = sectionNode.getAttributes().getNamedItem("end").getTextContent();
			    		
			    		//Node fileNode = sectionNode.getFirstChild();
			    		Node fileNode = getFirstValidNode(sectionNode);
			    		
			    		if (fileNode != null){
			    			sect.m_file.m_fileName = fileNode.getAttributes().getNamedItem("name").getTextContent();
			    			sect.m_file.m_content = fileNode.getAttributes().getNamedItem("content").getTextContent();
			    			saveTempFile(sect.m_file);
			    		    
			    		}
			    		else throw new Exception("file node non trovato");
			            
			            m_sections.add(sect);
			    		}
		    		}
		    }
		    ret = true;
		}
		catch(Exception e)
		{		
			e.printStackTrace();
		}
		
		return ret;
	}
	
	
	private Node getFirstValidNode(Node padre){
		Node n = null;
		
    	for (int i = 0; i< padre.getChildNodes().getLength(); i++){
    		Node n1 = padre.getChildNodes().item(i);
    		if (!isWhiteSPaceNode(n1) && n == null)
    			n = n1;

    	}
    	
		return n;
	}

	
	private String getAttributeValue(Document document, String elementPath, String attributeName){
	   String retValue = null; 
	   try{
	    	Node xNode = XPathAPI.selectSingleNode(
	    		        document,
	    		        elementPath);
	    	
	    	org.w3c.dom.Node xAttribute;
	    	
		    if (xNode != null) {
		    	xAttribute = xNode.getAttributes().getNamedItem(attributeName);
		        if (xAttribute != null)
		            retValue =  xAttribute.getTextContent();
		    	}
	    }
	    catch(Exception e){
	    	e.printStackTrace();
	    }
	   return retValue;
	                }
}
