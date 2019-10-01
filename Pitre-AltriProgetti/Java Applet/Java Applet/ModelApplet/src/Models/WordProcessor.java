package Models;

import java.io.ByteArrayInputStream;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import com.aspose.words.Bookmark;
import com.aspose.words.CompositeNode;
import com.aspose.words.Document;
import com.aspose.words.DocumentBuilder;
import com.aspose.words.HeaderFooterType;
import com.aspose.words.Node;
import com.aspose.words.NodeType;
import com.aspose.words.Paragraph;
import com.aspose.words.Range;
import com.aspose.words.Section;

public class WordProcessor extends AppProcessor {
 
//  boolean saveBinaryData(String filePath, String base64Content) throws IOException{
//  final String _path = filePath;
//  final byte[] decoded = Base64.decode(base64Content.toCharArray());
//  	Boolean b = java.security.AccessController.doPrivileged(
//			    new java.security.PrivilegedAction<Boolean>() {
//			        public Boolean run() {
//			        	FileOutputStream  fos = null;
//			        	Boolean ret = false;
//			        	try{
//				        	fos = new FileOutputStream(_path);
//				        	fos.write(decoded);
//				        	fos.close();
//				        	ret = true;
//			        	}
//			        	catch (Exception e){e.printStackTrace();}
//			        	finally{if (fos!=null)
//							try {
//								fos.close();
//							} catch (IOException e) {
//								// TODO Auto-generated catch block
//								e.printStackTrace();
//							}}
//						return ret;
//			        }
//			    }
//			 );
//		return b.booleanValue();
//	}

	@Override
	public boolean processModel(//UserInfo userInfo, 
			ModelResponse response, String outputFilePath) throws Exception{
	    try{

	    //LogHelper.WriteLog CLASS_NAME, "IModelProcessor_ProcessModel", "INIT"

	    //' Salvataggio del modello openoffice sul client
	    String modelFilePath = null;
	    modelFilePath = Util.GetTempFilePath(response.m_model.m_file.m_fileName);//Util.GetTempFilePath(response.m_model.m_file.m_fileName);
	    
	    //Se ho 2 file separati:
	    //Document doc = new Document(modelFilePath);
		
	    byte[] baContenuto = Base64.decode(response.m_model.m_file.m_content.toCharArray());
		System.out.println("prima di aspose");

	    InputStream is = new ByteArrayInputStream(baContenuto);
	    Document doc = new Document(is);
		System.out.println("dopo di ASPOSE");

        // Once the builder is created, its cursor is positioned at the beginning of the document.
        DocumentBuilder builder = new DocumentBuilder(doc);
    
        
        
//        String replaced = replaceInvalidChars("PAPERO MOLLO");
//        Range range = doc.getRange();
//    	int n = range.replace("#NUMERO PROTOCOLLO#", replaced, true, false);
        
	    ExecuteSearchAndReplace(doc, response);

	    for (IncludeSection entry : response.m_sections)
	        //' Il modello corrente prevede di inserire del contenuto tra i due bookmark appositamente
	        //' predisposti nel documento che si sta elaborando
	        appendContentFromCurrentVersion(doc, entry);
	    
	    doc.save(outputFilePath);

	 return true;
	    } catch(Exception e){
	    		throw (new Exception(e.getMessage() + ".IModelProcessor_Execute"));
	    		}
	}	

	private boolean isAncestor(Node n, CompositeNode a){
		boolean ret = false;
		
		while (n.getParentNode() !=null){
			if (((Node)n.getParentNode()).equals(a))
				ret = true;				
		}
			
		return ret;
	}

	private Node nextNode(Node node){
		Node ret = null;
		
		if (node.getNextSibling() != null){
			node = node.getNextSibling();
			while (node.isComposite() && ((CompositeNode)node).hasChildNodes())
				node = ((CompositeNode)node).getFirstChild();
				
			ret = node;
		}
		else {
			node = node.getParentNode();
			
//			if (node.isComposite()){
//				if (((CompositeNode) node).getFirstChild() != null)
//				while (node.getParentNode() != null && node.getNextSibling() == null)
//					node = node.getParentNode();
//				
//				node = node.getNextSibling();
//				
//	//			if (node != null){
//	//				while (node.isComposite() && ((CompositeNode)node).getFirstChild() != null)
//	//					node = ((CompositeNode)node).getFirstChild();
//	//			}
//			}
			ret = node;
		}		
						
		return ret;
	}
	
	
	private void deleteContent(Document doc, String m_begin, String m_end, boolean isInclusive) throws Exception
	{
		Bookmark startBck = doc.getRange().getBookmarks().get(m_begin);
		Bookmark endBck = doc.getRange().getBookmarks().get(m_end);
		
	    // First check that the nodes passed to this method are valid for use.
	   // VerifyParameterNodes(startNode, endNode);

	    // Create a list to store the extracted nodes.
	    //ArrayList nodes = new ArrayList();

	    // Keep a record of the original nodes passed to this method so we can split marker nodes if needed.
		Node startNode = startBck.getBookmarkEnd();
		Node endNode = endBck.getBookmarkStart();
	    
//	    while (startNode.getParentNode().getNodeType() != NodeType.BODY)
//	        startNode = startNode.getParentNode();
//
//	    while (endNode.getParentNode().getNodeType() != NodeType.BODY)
//	        endNode = endNode.getParentNode();

	    // Extract content based on block level nodes (paragraphs and tables). Traverse through parent nodes to find them.
	    // We will split the content of first and last nodes depending if the marker nodes are inline
	    
		
		
		
//		while (startNode.getParentNode().getNodeType() != NodeType.BODY)
//	        startNode = startNode.getParentNode();
//
//	    while (endNode.getParentNode().getNodeType() != NodeType.BODY)
//	        endNode = endNode.getParentNode();

	    boolean isExtracting = true;
	    boolean isStartingNode = true;
	    boolean isEndingNode = false;
	    // The current node we are extracting from the document.
	    Node currNode = startNode;
	    Node nextNode = startNode;

	    // Begin extracting content. Process all block level nodes and specifically split the first and last nodes when needed so paragraph formatting is retained.
	    // Method is little more complex than a regular extractor as we need to factor in extracting using inline nodes, fields, bookmarks etc as to make it really useful.
	    while (isExtracting && currNode != null)
	    {
	    	
	    	System.out.println(currNode.getParentNode().getText() + ":" + Node.nodeTypeToString(currNode.getNodeType()) + ":" + currNode.getText());
	        isEndingNode = currNode.equals(endNode);
	        if (isEndingNode){
	        	nextNode = null;
	        	isExtracting = false;
	        }
	        else {
	        	
	        	nextNode = nextNode(currNode);
	        	
	        	
//		        if (currNode.getNextSibling() == null && isExtracting)
//		        {	// Move to the next section.
//		            Section nextSection = (Section)currNode.getAncestor(NodeType.SECTION).getNextSibling();
//		            nextNode = (nextSection != null) ? nextSection.getBody().getFirstChild() : null;
//		        }
//		        else
//		        {	// Move to the next node in the body.
//		        	nextNode = currNode.getNextSibling();
//		        }
		        
	        	if (isStartingNode){
	        		isStartingNode = false;
	        	}
		        else{
		        	if (!currNode.isComposite() || !((CompositeNode)currNode).hasChildNodes())
		        		currNode.getParentNode().removeChild(currNode); 
		        		//	currNode.remove();
		        }
	        }
	        currNode = nextNode;
	    }

	    // Return the nodes between the node markers.
	}
	
	private void appendContentFromCurrentVersion(Document doc, IncludeSection section ){
	    try{
	//    LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "INIT - BeginSection: " & section.BeginSection & "; EndSection: " & section.EndSection
	    	List<Node> content = new ArrayList<Node>();	 
	    	boolean samePara = false;
	    	
	    	if (doc.getRange().getBookmarks().get(section.m_begin) != null && doc.getRange().getBookmarks().get(section.m_end) != null ) {
	        	String currentVersionFilePath = Util.GetTempFilePath(section.m_file.m_fileName);
	
	//        	LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "currentVersionFilePath: " & currentVersionFilePath
	
	//        	SaveBinaryData currentVersionFilePath, section.File.Content
	
	        	deleteContent(doc, section.m_begin, section.m_end, true);
	        	
	        	Document wDocCurrentVersion = new Document(currentVersionFilePath);
	        
	        	samePara = copyBookmarkRangeContent(wDocCurrentVersion, section.m_begin, section.m_end, content); 
	        	if (!content.isEmpty()) {
	//            LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "Copy from current version"
	    
	        		pasteContentToBookmarkRange(doc, section.m_begin, section.m_end, content, samePara);
	            
	//            LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "Paste to document"
	        	}
	//        	else
	//            LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "Copia contenuto da versione precedente non effettuata, i bookmark potrebbero non essere inseriti correttamente"
		    
	        
	        	wDocCurrentVersion = null;
		    }		   
	    }
	    catch(Exception e){
	    	e.printStackTrace();
	    }
	    
	    //LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "END"
    
	}
	
  	private void pasteContentToBookmarkRange(Document doc, String m_begin,
			String m_end, List<Node> content, boolean samePara) {
		// TODO Auto-generated method stub
		Bookmark startBck = doc.getRange().getBookmarks().get(m_begin);
		//Bookmark endBck = doc.getRange().getBookmarks().get(m_end);
		Node insertAfterNode = null;
		if (samePara){
			try {
				insertAfterNode = startBck.getBookmarkEnd();
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			
			
		}
		else{
			try {
				insertAfterNode = startBck.getBookmarkEnd().getParentNode();
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

			while ((insertAfterNode != null) && (insertAfterNode.getNodeType() != NodeType.PARAGRAPH) &&
	  				(insertAfterNode.getNodeType() != NodeType.TABLE)) 
				insertAfterNode = insertAfterNode.getParentNode();
		}

		CompositeNode insertParentNode = insertAfterNode.getParentNode();
		
		Node importNode = null;
		//NodeImporter importer = new NodeImporter(srcDoc, insertAfterNode.getDocument(), ImportFormatMode.KEEP_SOURCE_FORMATTING);
		for (Node newNode : (Iterable<Node>) content){
		        // Let's skip the node if it is a last empty paragraph in a section.
		    if (newNode.getNodeType() == (NodeType.PARAGRAPH))
		    {
		        Paragraph para = (Paragraph)newNode;
		        if (para.isEndOfSection() && !para.hasChildNodes())
		            continue;
		    }
		
		    // This creates a clone of the node, suitable for insertion into the destination document.
		    //Node newNode = importer.importNode(srcNode, true);
		
		    // Insert new node after the reference node.
		    
		    importNode = doc.importNode(newNode, true);
		    insertParentNode.insertAfter(importNode, insertAfterNode);
		    insertAfterNode = (CompositeNode) importNode;
		}
	}


	private boolean copyBookmarkRangeContent(Document wDocCurrentVersion,
			String m_begin, String m_end, List<Node> listNode) throws Exception {
		// TODO Auto-generated method stub
  		Bookmark startBck = wDocCurrentVersion.getRange().getBookmarks().get(m_begin);
  		Bookmark endBck = wDocCurrentVersion.getRange().getBookmarks().get(m_end);		  		  	
  		
		Node startNode = startBck.getBookmarkEnd();
		Node endNode = endBck.getBookmarkStart();
	    
	    // Extract content based on block level nodes (paragraphs and tables). Traverse through parent nodes to find them.
	    // We will split the content of first and last nodes depending if the marker nodes are inline
	    
	    boolean isExtracting = true;
	    boolean isStartingNode = true;
	    boolean isEndingNode = false;
	    	    
	    // The current node we are extracting from the document.
	    Node currNode = startNode;
	    Node nextNode = startNode;
		
	    boolean samePara = startNode.getAncestor(NodeType.PARAGRAPH).equals(endNode.getAncestor(NodeType.PARAGRAPH)); 
		if (samePara){
			while(isExtracting && currNode != null){
		        isEndingNode = currNode.equals(endNode);
		        if (isEndingNode){
		        	nextNode = null;
		        	isExtracting = false;
		        }
		        else {
//		        	nextNode = nextNode(currNode);
		        	
		        	nextNode = currNode.getNextSibling();

		        	if (isStartingNode){
		        		isStartingNode = false;
		        	}
			        else{
				        // Clone the current node and its children to obtain a copy.
				    	CompositeNode cloneNode = (CompositeNode)(currNode.deepClone(true));
			        	listNode.add(cloneNode);		        				    				    
			        }
		        }		       
		        currNode = nextNode;				
			}			
		}
		else{				
			while (startNode.getParentNode().getNodeType() != NodeType.BODY)
		        startNode = startNode.getParentNode();
	
		    while (endNode.getParentNode().getNodeType() != NodeType.BODY)
		        endNode = endNode.getParentNode();
	
		    currNode = startNode;
		    // Begin extracting content. Process all block level nodes and specifically split the first and last nodes when needed so paragraph formatting is retained.
		    // Method is little more complex than a regular extractor as we need to factor in extracting using inline nodes, fields, bookmarks etc as to make it really useful.
		    while (isExtracting && currNode != null)
		    {
		        isEndingNode = currNode.equals(endNode);
		        if (isEndingNode){
		        	nextNode = null;
		        	isExtracting = false;
		        }
		        else {
	//	        	nextNode = nextNode(currNode);
		        	
	
			        if (currNode.getNextSibling() == null && isExtracting)
			        {	// Move to the next section.
			            Section nextSection = (Section)currNode.getAncestor(NodeType.SECTION).getNextSibling();
			            nextNode = (nextSection != null) ? nextSection.getBody().getFirstChild() : null;
			        }
			        else
			        {	// Move to the next node in the body.
			        	nextNode = currNode.getNextSibling();
			        }
		        	if (isStartingNode){
		        		isStartingNode = false;
		        	}
			        else{
				        // Clone the current node and its children to obtain a copy.
				    	CompositeNode cloneNode = (CompositeNode)(currNode.deepClone(true));
			        	listNode.add(cloneNode);		        				    				    
			        }
		        }
		       
		        currNode = nextNode;
		    }
		}
		
		return samePara;

	}


//	static boolean SearchAndReplace(Document document, String key, String value) throws Exception{
//    	
//    	try{    		
//	        return (document.getRange().replace(key, value, true, true) == 1);
//	       }
//    	catch(Exception e){
//    		throw new Exception("Errore " + e.getMessage() + " in SearchAndReplace");
//    	
//    	}
//    } 
  	
    void ExecuteSearchAndReplace(Document document, ModelResponse response){
        try{
        	// Sostituzione dei placeholder nel file del modello con i valori del documento
        
        	//AbstractMap.SimpleEntry keyValuePair; 
        
        	for (Map.Entry<String, String> entry : response.m_model.map.entrySet())
        	{
        		String key = "#" + entry.getKey().toUpperCase() + "#";
         		String value = entry.getValue();
            		
        		searchAndReplace( document, key, value);
            
            //LogHelper.WriteLog CLASS_NAME, "ExecuteSearchAndReplace", "Key: " & Key & " - Value: " & value
        	}
        }
    catch(Exception e){
    	//throw new Exception("Errore " + e.getMessage() + " in SearchAndReplace");
    	e.printStackTrace();
    }
        }

    void searchAndReplace(Document document, String key, String value){
    	try{
    	
    		if (key != null && !key.equalsIgnoreCase("")){
		        Range r = document.getRange();
		        
		        replaceText(r, key, value);
	        
		        //LogHelper.WriteLog CLASS_NAME, "ReplaceText", "Key: " & Key & " - Value: " & value
		    
		        if (document.getSections().getCount() > 0) {
		        	
		        
		            for (Section section : document.getSections()){
		                
		                //' Sostituzione valori nelle Header sections del documento
		            	replaceTextOnHeaderFooterSections(document, section, HeaderFooterType.HEADER_PRIMARY, key, value);
		            	replaceTextOnHeaderFooterSections(document, section, HeaderFooterType.HEADER_FIRST, key, value);
		            	replaceTextOnHeaderFooterSections(document, section, HeaderFooterType.HEADER_EVEN, key, value);
		                
		                //' Sostituzione valori nelle Footer sections del documento
		            	replaceTextOnHeaderFooterSections(document, section, HeaderFooterType.FOOTER_PRIMARY, key, value);
		            	replaceTextOnHeaderFooterSections(document, section, HeaderFooterType.FOOTER_FIRST, key, value);
		            	replaceTextOnHeaderFooterSections(document, section, HeaderFooterType.FOOTER_EVEN, key, value);
		            }
		        }
	    	}
    	}
    	catch (Exception e){}
    
  }

//' Sostituzione del testo nei campi presenti nelle sezioni Header del documento
//void replaceTextOnHeaderSections(ByVal document As Word.document, _
//                                        ByVal section As Word.section, _
//                                        ByVal sectionType As Word.WdHeaderFooterIndex, _
//                                        ByVal Key As String, ByVal value As String)
//    On Error GoTo eh
//    
//    If (section.Headers(sectionType).Exists) Then
//        Dim rangeSearch As Word.Range
//        Set rangeSearch = section.Headers(sectionType).Range
//        
//        If (Not rangeSearch Is Nothing) Then
//            ReplaceText rangeSearch, Key, value
//            LogHelper.WriteLog CLASS_NAME, "ReplaceTextOnHeaderSections", "HeaderSection - Key: " & Key & " - Value: " & value
//        End If
//    End If
//    
//    Exit Sub
//eh:
//    Err.Raise Err.Number, _
//              Err.Source & vbCrLf & vbTab & CLASS_NAME & ".ReplaceTextOnHeaderSections", _
//              Err.Description
//End Sub

//' Sostituzione del testo nei campi presenti nelle sezioni Footer del documento
void replaceTextOnHeaderFooterSections(Document document, 
                                        Section section,
                                        int hfType,
                                        String key, String value) throws Exception
    {
    
    //' Footer sections su tutte le pagine
    if (section.getHeadersFooters().getCount() > 0){ 
    		//Footers(sectionType).Exists) Then
        //Dim rangeSearch As Word.Range
    
        Range rangeSearch = section.getHeadersFooters().getByHeaderFooterType(hfType).getRange();
        
        if (rangeSearch != null) {
            replaceText(rangeSearch, key, value);
            //LogHelper.WriteLog CLASS_NAME, "ReplaceTextOnFooterSections", "FooterSection - Key: " & Key & " - Value: " & value
        }
    }
    
//    Exit Sub
//eh:
//    Err.Raise Err.Number, _
//              Err.Source & vbCrLf & vbTab & CLASS_NAME & ".ReplaceTextOnFooterSections", _
//              Err.Description
 }

//' Sostituzione, in un determinato range del documento word, dei placeholder con il corrispondente valore del documento
void replaceText(Range range, String key, String value) throws Exception{
    
    if (key != null){
        int MAX_PART_LENGTH = 200;
        
        if (value.length() > MAX_PART_LENGTH) {
            String PLACEHOLDER = "#$PART$#";
            
            int partLength = (MAX_PART_LENGTH - PLACEHOLDER.length());
            
            int parts;
            parts = value.length() / MAX_PART_LENGTH;
            if ((value.length() % MAX_PART_LENGTH) > 0)
                parts = parts + 1;
            
            int startIndex = 1;
            
            String item = null;
            
            for (int i = 0; i < parts; i++){
                item = value.substring(startIndex, startIndex + partLength - 1);
                
                if (i < parts) item = item + PLACEHOLDER;
                                
                if (i == 1) 
                    replaceText(range, key, item);
                else
                    replaceText(range, PLACEHOLDER, item);
                
                
                startIndex = startIndex + partLength;
            }
        }
        else{
        	String replaced = replaceInvalidChars(value);
        	int n = range.replace(key, replaced, false, false);
        	// isMatchWholeWord = FALSE altrimenti il replcae di #.....# non funziona
//        	ret As Boolean
//            With r.Find
//                .ClearFormatting
//                .Text = Key
//                .Replacement.ClearFormatting
//                .Replacement.Text = ReplaceInvalidChars(value)
//                
//                ret = .Execute(Replace:=Word.WdReplace.wdReplaceAll)
//            End With
        	System.out.println(Integer.toString(n) + " sostituzioni della stringa [" + key + "] con la stringa [" +  replaced + "]");
        }
    }
}
    
	String replaceInvalidChars(String value){
	    
	    //' ^ rappresenta il carattere wildcard
	    value = value.replace("^", "^^");
	    
	    //' carattere linefeed (senza carriage return), equivalente a Chr(10)
	    value = value.replace(System.getProperty("line.separator"), "^p");
	    
	    return value;
	}
	
	
	public void testDocument(String path) throws Exception{
	    //String modelFilePath = Util.GetTempFilePath(path);//Util.GetTempFilePath(response.m_model.m_file.m_fileName);
	    
	    
	    Document doc = new Document(path);
	    
	    //replaceText(doc.getRange(), "cerca", "trova");
	    
	    //doc.save(path, SaveOptions.)
	    
	    //deleteContent(doc, "BK1", "BK2", true);
	    
	    IncludeSection section = new IncludeSection();
	    
	    section.m_begin = "BK1";
	    section.m_end = "BK2";
	    section.m_file = new FileContent();
	    section.m_file.m_content = "";
	    section.m_file.m_fileName = "PIPPO2.docx";
	    
	    
	    appendContentFromCurrentVersion(doc, section);
	    
	    doc.save(path);
		
	}

}
