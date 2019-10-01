package Models;

import java.util.Map;

//import com.sun.star.container.NoSuchElementException;
//import com.sun.star.container.XNameAccess;
//import com.sun.star.frame.XComponentLoader;
//import com.sun.star.frame.XController;
//import com.sun.star.lang.WrappedTargetException;
//import com.sun.star.lang.XComponent;
//import com.sun.star.text.XBookmarksSupplier;
//import com.sun.star.text.XText;
//import com.sun.star.text.XTextContent;
//import com.sun.star.text.XTextCursor;
//import com.sun.star.text.XTextDocument;
//import com.sun.star.text.XTextRange;
//import com.sun.star.text.XTextViewCursor;
//import com.sun.star.text.XTextViewCursorSupplier;
//import com.sun.star.uno.UnoRuntime;
//import com.sun.star.util.XReplaceDescriptor;
//import com.sun.star.util.XReplaceable;

public class OOProcessor extends AppProcessor {
	@Override
	public boolean processModel(//UserInfo userInfo, 
			ModelResponse response, String outputFilePath) throws Exception{
	return false;
	}

//	    	@Override
//	    	public boolean processModel(//UserInfo userInfo, 
//	    			ModelResponse response, String outputFilePath) throws Exception{
//	    	    try{
//
//	    //LogHelper.WriteLog CLASS_NAME, "IModelProcessor_ProcessModel", "INIT"
//
//	    //' Salvataggio del modello openoffice sul client
//	    String modelFilePath = null;
//	    modelFilePath = Util.GetTempFilePath(response.m_model.m_file.m_fileName);
//
//	    com.sun.star.uno.XComponentContext xContext = null;
//
//	    //com.sun.star.frame.XComponentLoader xCompLoader = null;
//	    
//	       
//
//	    	
//	        // get the remote office component context
//	        xContext = com.sun.star.comp.helper.Bootstrap.bootstrap();
//	        System.out.println("Connected to a running office ...");
//	        
//	        // get the remote office service manager
//	        com.sun.star.lang.XMultiComponentFactory xMCF =
//	            xContext.getServiceManager();
//	        
//	        Object oDesktop = xMCF.createInstanceWithContext(
//	            "com.sun.star.frame.Desktop", xContext);
//	    
//	        com.sun.star.frame.XComponentLoader xCompLoader =
//	            (com.sun.star.frame.XComponentLoader)
//	                 UnoRuntime.queryInterface(
//	                     com.sun.star.frame.XComponentLoader.class, oDesktop);
//	        
//	        
//	        com.sun.star.beans.PropertyValue propertyValues[] = new com.sun.star.beans.PropertyValue[1];
//	        propertyValues[0] = new com.sun.star.beans.PropertyValue();
//	        propertyValues[0].Name = "Hidden";
//	        propertyValues[0].Value = new Boolean(true);
//
//	        String sUrl = Util.convertToURL(modelFilePath);//args[0];
//
//	        XComponent oDocToStore =
//	                xCompLoader.loadComponentFromURL(
//	                    sUrl, "_blank", 0, propertyValues);
//	        
//	        XReplaceable xr = (XReplaceable) UnoRuntime.queryInterface(XReplaceable.class, oDocToStore);
//	        //XReplaceable xr = (XReplaceable)oDocToStore;
//	        
//	        XReplaceDescriptor rd = xr.createReplaceDescriptor();
//	        
//	        
//	        ExecuteSearchAndReplace(xr, rd, response);
//
////	    ' Sostituzione dei placeholder nel file del modello con i valori del documento
////	    Dim search As Object
////	    Set search = document.createReplaceDescriptor
//	//    
////	        ExecuteSearchAndReplace document, search, response
//
//	        
////	        IncludeSection section
////	    Dim section As IncludeSection
//	    for (IncludeSection entry : response.m_sections)
//	        //' Il modello corrente prevede di inserire del contenuto tra i due bookmark appositamente
//	        //' predisposti nel documento che si sta elaborando
//	        appendTextFromCurrentVersion(xCompLoader, oDocToStore, entry);
//	    
//
//	    //' Salvataggio del file di output openoffice
//	    propertyValues = new com.sun.star.beans.PropertyValue[1];
//	    
//	    propertyValues[0] = new com.sun.star.beans.PropertyValue();
//	    propertyValues[0].Name = "Hidden";
//	    propertyValues[0].Value = new Boolean(true);
//	    
//	    if (Util.isRTFFormat(outputFilePath)) {
////	        ' Il file di output deve essere in formato RTF,
////	        ' pertanto viene definito il parametro di salvataggio
//	        propertyValues[0] = new com.sun.star.beans.PropertyValue();
//	        propertyValues[0].Name = "FilterName";
//	        propertyValues[0].Value = "Rich Text Format";
//	    }
//
//	    com.sun.star.frame.XStorable xStorable =
//	    (com.sun.star.frame.XStorable)UnoRuntime.queryInterface(
//	        com.sun.star.frame.XStorable.class, oDocToStore );
//
//	    com.sun.star.beans.PropertyValue[] propertyValue = new com.sun.star.beans.PropertyValue[ 2 ];
//	    
//		propertyValue[0] = new com.sun.star.beans.PropertyValue();
//		propertyValue[0].Name = "Overwrite";
//		propertyValue[0].Value = new Boolean(true);
//		propertyValue[1] = new com.sun.star.beans.PropertyValue();
//		propertyValue[1].Name = "FilterName";
//		propertyValue[1].Value = "StarOffice XML (Writer)";
//		xStorable.storeAsURL(Util.convertToURL(outputFilePath), propertyValue );
//
//		
//	    
//	    //LogHelper.WriteLog CLASS_NAME, "IModelProcessor_ProcessModel", "OpenOfficeDocumentSaved - FilePath: " & outputUrl
//	    
//	    com.sun.star.util.XCloseable xCloseable =
//	            (com.sun.star.util.XCloseable)UnoRuntime.queryInterface(
//	                com.sun.star.util.XCloseable.class, oDocToStore);
//
//	        if ( xCloseable != null ) {
//	            xCloseable.close(false);
//	        } else 
//	        	oDocToStore.dispose();
//
//	 
//	 return true;
//	    } catch(Exception e){
//	    		throw (new Exception(e.getMessage() + ".IModelProcessor_Execute"));
//	    		}
//	}	
//
//    private XTextCursor getBookmarkByName(XComponent xComponent, String nameBookmark) {
//        
//    	XBookmarksSupplier xBookmarksSupplier = UnoRuntime.queryInterface(XBookmarksSupplier.class, xComponent);
//        XNameAccess xNamedBookmarks = xBookmarksSupplier.getBookmarks();
//        try {
//            Object bookmark = xNamedBookmarks.getByName(nameBookmark);
//            XTextContent xTextContent = UnoRuntime.queryInterface(XTextContent.class, bookmark);
//            XTextRange xTextRange = xTextContent.getAnchor();
//            XText xLocalText = xTextRange.getText();
//            XTextCursor xLocalCursor = xLocalText.createTextCursorByRange(xTextRange);
//            return xLocalCursor;
//        } catch (NoSuchElementException e) {
//            throw new IllegalArgumentException("bookmark '" + nameBookmark + "' does not exist");
//        } catch (WrappedTargetException e) {
//            throw new IllegalStateException("unknown problem with bookmark '" + nameBookmark + "'");
//        }    
//    }
//
//    private String getBookmarkRangeText(XComponent xComponent, String startBookmark, String endBookmark) {
//        
//    	XBookmarksSupplier xBookmarksSupplier = UnoRuntime.queryInterface(XBookmarksSupplier.class, xComponent);
//        XNameAccess xNamedBookmarks = xBookmarksSupplier.getBookmarks();
//        try {
//        	XTextContent sBookmark = (XTextContent)UnoRuntime.queryInterface(XTextContent.class, xNamedBookmarks.getByName(startBookmark));
//        	XTextContent eBookmark = (XTextContent)UnoRuntime.queryInterface(XTextContent.class, xNamedBookmarks.getByName(endBookmark));
//            //Object eBookmark = xNamedBookmarks.getByName(endBookmark);
//            
//            XTextDocument intDoc = (XTextDocument)UnoRuntime.queryInterface(XTextDocument.class,xComponent); 
//            XController intCont = intDoc.getCurrentController(); 
//          
//            
//            XTextViewCursorSupplier xTVCS =  (XTextViewCursorSupplier)UnoRuntime.queryInterface(XTextViewCursorSupplier.class,intCont);
//            XTextViewCursor xTVC = xTVCS.getViewCursor();
//            
//            
//            xTVC.gotoRange(sBookmark.getAnchor().getStart(), false);
//            xTVC.gotoRange(eBookmark.getAnchor().getEnd(), true);
//            
//            
//            return xTVC.getText().getString();
//            
//        } catch (NoSuchElementException e) {
//            throw new IllegalArgumentException("bookmark '" + startBookmark + "/" + endBookmark + "' does not exist");
//        } catch (WrappedTargetException e) {
//            throw new IllegalStateException("unknown problem with bookmark '" + startBookmark + "/" + endBookmark + "'");
//        }    
//    }
//
//
//    XComponent loadDocument(XComponentLoader xCompLoader, String path) throws com.sun.star.io.IOException, com.sun.star.lang.IllegalArgumentException{
//        com.sun.star.beans.PropertyValue propertyValues[] = new com.sun.star.beans.PropertyValue[1];
//        propertyValues[0] = new com.sun.star.beans.PropertyValue();
//        propertyValues[0].Name = "Hidden";
//        propertyValues[0].Value = new Boolean(true);
//
//        String sUrl = Util.convertToURL(path);//args[0];
//
//        XComponent oDoc =
//                xCompLoader.loadComponentFromURL(
//                    sUrl, "_blank", 0, propertyValues);
//        
//        return oDoc;
//    }
//
//    
//    void appendTextFromCurrentVersion(XComponentLoader xCompLoader, XComponent document, IncludeSection section){
//        try{
//        
//        //LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "INIT - BeginSection: " & section.BeginSection & "; EndSection: " & section.EndSection
//        
//        //' Reperimento bookmark iniziale e finale
//        XTextCursor startBck = getBookmarkByName(document, section.m_begin);
//        
//        XTextCursor endBck = getBookmarkByName(document, section.m_end);
//        
//        if (startBck != null && endBck != null) {
//            //' Viene salvato su file temporaneo la versione corrente del documento
//            String currentVersionFilePath;
//            currentVersionFilePath = Util.GetTempFilePath(section.m_file.m_fileName);
//            Util.saveBinaryData(currentVersionFilePath, section.m_file.m_content);
//        
//            //' Apertura del file della versione corrente
//            
//            XComponent docCurrentVersion = loadDocument(xCompLoader, currentVersionFilePath);
//        
//            //' Reperimento, dal file della versione corrente, del testo compreso tra i due bookmark richiesti
//            String rangeText = getBookmarkRangeText(docCurrentVersion, section.m_begin, section.m_end);
//            
//            com.sun.star.util.XCloseable xCloseable =
//                    (com.sun.star.util.XCloseable)UnoRuntime.queryInterface(
//                        com.sun.star.util.XCloseable.class, docCurrentVersion);
//
//                if ( xCloseable != null ) {
//                    xCloseable.close(false);
//                } else 
//                	docCurrentVersion.dispose();
//                	
//            
//            //LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "Copy from current version"
//            
//            //' Inserimento del testo estratto dalla versione corrente del documento nel modello tra i due bookmark
//            startBck.getEnd().setString(rangeText);
//            
//            //LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "Paste to document"
//                }
//        }
//        catch( Exception e ) {
//            e.printStackTrace(System.err);
//            System.exit(1);
//        }
//        
//        //LogHelper.WriteLog CLASS_NAME, "AppendContentFromCurrentVersion", "END"
//        
//        }
//    //eh:
////        ' Se aperto, il documento viene chiuso
////        If (Not docCurrentVersion Is Nothing) Then
////            docCurrentVersion.Close (True)
////            Set docCurrentVersion = Nothing
////        End If
//    //    
////        Err.Raise Err.Number, _
////                  Err.Source & vbCrLf & vbTab & CLASS_NAME & ".AppendTextFromCurrentVersion", _
////                  Err.Description
//
//
////    	void loadDocument(String filePath)
////    	{
////            if ( args.length < 1 ) {
////                System.out.println(
////                    "usage: java -jar DocumentLoader.jar \"<URL|path>\"" );
////                System.out.println( "\ne.g.:" );
////                System.out.println(
////                    "java -jar DocumentLoader.jar \"private:factory/swriter\"" );
////                System.exit(1);
////            }
//            
////
////            ModelResponse response = new ModelResponse();
////            
////            response.m_model.map.put("key", "value");
////            
////            com.sun.star.uno.XComponentContext xContext = null;
////
////            //com.sun.star.frame.XComponentLoader xCompLoader = null;
////            
////            try {
////            	
////            	
////                
////
////            	
////                // get the remote office component context
////                xContext = com.sun.star.comp.helper.Bootstrap.bootstrap();
////                System.out.println("Connected to a running office ...");
////                
////                // get the remote office service manager
////                com.sun.star.lang.XMultiComponentFactory xMCF =
////                    xContext.getServiceManager();
////                
////                Object oDesktop = xMCF.createInstanceWithContext(
////                    "com.sun.star.frame.Desktop", xContext);
////            
////                com.sun.star.frame.XComponentLoader xCompLoader =
////                    (com.sun.star.frame.XComponentLoader)
////                         UnoRuntime.queryInterface(
////                             com.sun.star.frame.XComponentLoader.class, oDesktop);
////                
////                
////                com.sun.star.beans.PropertyValue propertyValues[] = new com.sun.star.beans.PropertyValue[1];
////                propertyValues[0] = new com.sun.star.beans.PropertyValue();
////                propertyValues[0].Name = "Hidden";
////                propertyValues[0].Value = new Boolean(true);
////     
////                String sUrl = Util.convertToURL("C:\\Dati\\ValueTeam\\Applet\\sviluppo\\ModuloWordApplet\\src\\integra\\prova.odt");//args[0];
////
////                Object oDocToStore =
////                        xCompLoader.loadComponentFromURL(
////                            sUrl, "_blank", 0, propertyValues);
////                
////                XReplaceable xr = (XReplaceable) UnoRuntime.queryInterface(XReplaceable.class, oDocToStore);
////                //XReplaceable xr = (XReplaceable)oDocToStore;
////                
////                XReplaceDescriptor rd = xr.createReplaceDescriptor();
////                
////                
////                ExecuteSearchAndReplace(xr, rd, response);
////
//////                if ( sUrl.indexOf("private:") != 0) {
//////                    java.io.File sourceFile = new java.io.File(args[0]);
//////                    StringBuffer sbTmp = new StringBuffer("file:///");
//////                    sbTmp.append(sourceFile.getCanonicalPath().replace('\\', '/'));
//////                    sUrl = sbTmp.toString();
//////                }    
////          
////                // Load a Writer document, which will be automaticly displayed
////                com.sun.star.lang.XComponent xComp = xCompLoader.loadComponentFromURL(
////                    sUrl, "_blank", 0, new com.sun.star.beans.PropertyValue[0]);
////
////                if ( xComp != null )
////                    System.exit(0);
////                else
////                    System.exit(1);
////            }
////            catch( Exception e ) {
////                e.printStackTrace(System.err);
////                System.exit(1);
////            }
////        }
//    	static boolean SearchAndReplace(XReplaceable document, XReplaceDescriptor search, String key, String value) throws Exception{
//        	
//    	try{
//	        search.setSearchString (key);
//	        search.setReplaceString (value);
//	        		
//	        return (document.replaceAll(search) == 1);
//	        }
//    	catch(Exception e){
//    		throw new Exception("Errore " + e.getMessage() + " in SearchAndReplace");
//    	
//    	}
//    }    	
//    	
//        static void ExecuteSearchAndReplace(XReplaceable document, XReplaceDescriptor search, ModelResponse response){
//            try{
//            	// Sostituzione dei placeholder nel file del modello con i valori del documento
//            
//            	//AbstractMap.SimpleEntry keyValuePair; 
//            
//            	for (Map.Entry<String, String> entry : response.m_model.map.entrySet())
//            	{
//            		String key = "#" + entry.getKey().toUpperCase() + "#";
//             		String value = entry.getValue();
//                		
//            		SearchAndReplace( document, search, key, value);
//                
//                //LogHelper.WriteLog CLASS_NAME, "ExecuteSearchAndReplace", "Key: " & Key & " - Value: " & value
//            	}
//            }
//        catch(Exception e){
//        	//throw new Exception("Errore " + e.getMessage() + " in SearchAndReplace");
//
//        }
//            }
    }
