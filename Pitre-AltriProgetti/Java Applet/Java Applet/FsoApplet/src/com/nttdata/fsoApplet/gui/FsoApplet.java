package com.nttdata.fsoApplet.gui;

import java.applet.Applet;
import java.awt.Desktop;
import java.awt.Toolkit;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.datatransfer.Clipboard;
import java.awt.datatransfer.StringSelection;
import java.io.File;
import java.io.FileFilter;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.PrintWriter;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;
//import java.io.FileWriter;
//import java.util.ArrayList;
import javax.swing.JFileChooser;
import javax.swing.UIManager;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;

import com.nttdata.fsoApplet.ServletIOManager;
import com.sun.org.apache.xpath.internal.XPathAPI;


public class FsoApplet extends Applet implements ActionListener{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final String SEPARATORE_RIGA_REPORT = System.getProperty("line.separator");
	private static final String SEPARATORE_CAMPO_REPORT = "@-@";
	private static final String ESITO_SUCCESSO = "Successo";
	private static final String TIPO_DIRECTORY = "Directory";
	private static final String ESITO_FALLITO = "Fallito";
	private static final String TIPO_FILE = "File";
	private static final String MESSAGGIO_SUCCESSO = "Esportazione dati fascicolo completata con successo.";
	private static final String TIPO_RESULT = "Result";
	private static final String MESSAGGIO_FALLITO = "Esportazione dati fascicolo fallita.";

	/**
	 * 
	 */
//	private static final long serialVersionUID = 1L;
	public FsoApplet(){}
	
	@Override 
	public void init()
	{
		System.out.println("FsoApplet init");
		try {
			UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
			
			//String cmd = "C:\\Documents and Settings\\Abbatangeli\\Documents\\12569466.pdf";
			//Runtime.getRuntime().exec("rundll32 url.dll,FileProtocolHandler " + cmd);
			//openFile(cmd);
			
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		//String xmlTest = "<?xml version=\"1.0\"?><MetaInfoFascicolo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" id=\"12546477\" nome=\"17.6-2012-1\">  <Fascicoli>    <MetaInfoFascicolo id=\"12567386\" nome=\"CARTELLA_A\">      <Fascicoli>        <MetaInfoFascicolo id=\"12567395\" nome=\"CARTELLA_B\">          <Fascicoli />          <Documenti>            <MetaInfoDocumento id=\"12567397\" nome=\"12567397_\" fullName=\"12567398.txt\" isProtocollo=\"false\" isAllegato=\"false\" />          </Documenti>        </MetaInfoFascicolo>      </Fascicoli>      <Documenti>        <MetaInfoDocumento id=\"12567257\" nome=\"12567257_\" fullName=\"12567492.txt.P7M\" isProtocollo=\"false\" isAllegato=\"false\" />      </Documenti>    </MetaInfoFascicolo>  </Fascicoli>  <Documenti /></MetaInfoFascicolo>";
		//String pathTest = "E:\\DEV\\File vari\\Da Importare\\NovoTest\\Cart_1_no_File\\";
		//String pathTest2 = "C:\\Users\\ABBATA~1\\Documents\\12568329.txt";

		//String pathTest2 = "E:\\DEV\\File vari\\Da Importare\\NovoTest\\Cart_1_no_File\\";
		//getFiles(pathTest2);
		
		//String urlTest = "http://localhost/NttDataWA/Project/ImportExport/Import/ImportDocumentAppletService.aspx?Absolutepath=E%3A%5CDEV%5CFile%20vari%5CDa%20Importare%5CNovoTest%5CCart_1_no_File&codFasc=12568327&foldName=Cart_1_no_File&type=DIR";
		//String pathTest = "E:\\DEV\\File vari\\Da Importare\\NovoTest\\Cart_1_no_File";
		//sendFiletoURL(pathTest, urlTest);
		
		//String pathTest3 = "E:\\DEV\\File vari\\Da Importare\\NovoTest\\Cart_1_no_File\\File_1.txt";
		//String urlTest3 = "http://localhost/NttDataWA/Project/ImportExport/Import/ImportDocumentAppletService.aspx?Absolutepath=E%3A%5CDEV%5CFile%20vari%5CDa%20Importare%5CNovoTest%5CCart_1_no_File%5CFile_1.txt&codFasc=1.1-2013-98&foldName=E%3A%5CDEV%5CFile%20vari%5CDa%20Importare%5CNovoTest%5CCart_1_no_File&type=FILE&idTitolario=7067503";
						  
		//sendFiletoURL(pathTest3, urlTest3);
		
		//String urlTest = "http://localhost/NttDataWA/Project/ImportExport/getFileInProject.aspx";
		//String urlParameters = ""; //idDocumento
		//String tempo = projectToFS(pathTest,xmlTest,urlTest);	
		//String[] temp = getFolders(pathTest);
		//String[] temp2 = getFiles(pathTest);
		
		//openFile(pathTest2);
		
		//String urlTest = "http://localhost/NttDataWA/Project/ImportExport/getFileInProject.aspx";
		//String xmlTest = "<?xml version=\"1.0\"?><MetaInfoFascicolo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" id=\"12568869\" nome=\"1.1-2013-98\">  <Fascicoli>    <MetaInfoFascicolo id=\"12569058\" nome=\"Cart_1_no_File\">      <Fascicoli>        <MetaInfoFascicolo id=\"12569094\" nome=\"CARTELLA_A\">          <Fascicoli>            <MetaInfoFascicolo id=\"12569096\" nome=\"CARTELLA_B\"><Fascicoli><MetaInfoFascicolo id=\"12569105\" nome=\"1.1-2013-98\"><Fascicoli /><Documenti /></MetaInfoFascicolo></Fascicoli><Documenti><MetaInfoDocumento id=\"12569098\" nome=\"12569098_\" fullName=\"12569113.txt.P7M\" isProtocollo=\"false\" isAllegato=\"false\" /></Documenti>            </MetaInfoFascicolo>          </Fascicoli>          <Documenti />        </MetaInfoFascicolo>        <MetaInfoFascicolo id=\"12569067\" nome=\"Cart_2_File\">          <Fascicoli>            <MetaInfoFascicolo id=\"12569083\" nome=\"Sottocartella 2\"><Fascicoli><MetaInfoFascicolo id=\"12569138\" nome=\"Cart_2_File\"><Fascicoli /><Documenti>  <MetaInfoDocumento id=\"12569140\" nome=\"12569140_\" fullName=\"12569184.txt.pdf.P7M\" isProtocollo=\"false\" isAllegato=\"false\" /></Documenti></MetaInfoFascicolo></Fascicoli><Documenti><MetaInfoDocumento id=\"12569131\" nome=\"12569131_\" fullName=\"12569203.txt.pdf.P7M\" isProtocollo=\"false\" isAllegato=\"false\" /><MetaInfoDocumento id=\"12569085\" nome=\"12569085_\" fullName=\"12569304.txt.P7M\" isProtocollo=\"false\" isAllegato=\"false\" /></Documenti>            </MetaInfoFascicolo>          </Fascicoli>          <Documenti>            <MetaInfoDocumento id=\"12569076\" nome=\"12569076_\" fullName=\"12569315.txt.P7M\" isProtocollo=\"false\" isAllegato=\"false\" />            <MetaInfoDocumento id=\"12569069\" nome=\"12569069_\" fullName=\"12569339.txt.P7M\" isProtocollo=\"false\" isAllegato=\"false\" />          </Documenti>        </MetaInfoFascicolo>      </Fascicoli>      <Documenti>        <MetaInfoDocumento id=\"12569060\" nome=\"12569060_\" fullName=\"12569348.txt.P7M\" isProtocollo=\"false\" isAllegato=\"false\" />      </Documenti>    </MetaInfoFascicolo>  </Fascicoli>  <Documenti /></MetaInfoFascicolo>";
		//String pathTest = "C:\\Users\\Abbatangeli\\Documents\\ExportTest";
		//this.projectToFS(pathTest,xmlTest,urlTest);

	}
	
	public void actionPerformed(ActionEvent e) {
		
	}
	
	public String selectFolder(String choosertitle, String startPath){
		System.out.println("selectFolder");
		
		final String _choosertitle = choosertitle;
		final String _startPath = startPath;
		
		return java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			            return _selectFolder(_choosertitle, _startPath);
			        }
			    }
			 );
	}
	
	private String _selectFolder(String choosertitle, String startPath){
		String result="";
		
		if (startPath != null && startPath.length() != 0) {
			if (!_folderExists(startPath))
				startPath = getSpecialFolder();
		}
		else {
			startPath = getSpecialFolder();
		}
		
		JFileChooser chooser;
		
	    chooser = new JFileChooser(); 
	    //chooser.setCurrentDirectory(new java.io.File("."));
	    chooser.setCurrentDirectory(new java.io.File(startPath));
	    chooser.setDialogTitle(choosertitle);
	    chooser.setFileSelectionMode(JFileChooser.DIRECTORIES_ONLY);
	    //
	    // disable the "All files" option.
	    //
	    chooser.setAcceptAllFileFilterUsed(false);
	    //    
	    if (chooser.showOpenDialog(this) == JFileChooser.APPROVE_OPTION) { 
	      System.out.println("getCurrentDirectory(): " 
	         +  chooser.getSelectedFile());
	      //result = chooser.getCurrentDirectory().toString();
	      result = chooser.getSelectedFile().toString();
	    }
	    else {
	      System.out.println("No Selection ");
	    }
	    
	    return result;
	}
	
	public boolean folderExists(String path) {
		final String _path = path;
		System.out.println("folderExists");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			            return _folderExists(_path);
			        }
			    }
			 );
		return b.booleanValue();
	}
		
	private boolean _folderExists(String path) {
				boolean ret = false;
		try{
			System.out.println(path);
			File folder = new File(path);
			ret = folder.exists() && folder.isDirectory();
			System.out.println("survive");
		}
		catch (Exception ex){
			System.out.println(ex.toString());

			ret = false;
		}
		
		return ret;
	}

	public boolean fileExists(String path) {
		final String _path = path;
		
		System.out.println("fileExists");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			            return _fileExists(_path);
			        }
			    }
			 );
		return b.booleanValue();
	}
		
	public boolean _fileExists(String path) {
		boolean ret = false;
		try{
			File file = new File(path);
			ret = file.exists() && file.isFile();
		}
		catch (Exception ex){
			ret = false;
		}
		
		return ret;
	}

	public boolean createFolder(String path) {
		final String _path = path;
		
		System.out.println("createFolder");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			            return _createFolder(_path);
			        }
			    }
			 );
		return b.booleanValue();
	}
		
	public boolean _createFolder(String path) {
		boolean ret = false;
		try{
			File folder = new File(path);
			ret = folder.mkdir();
		}
		catch (Exception ex){
			ret = false;
		}
		
		return ret;
	}
	
	public String getFiles(String path) {
		final String _path = path;
		
		System.out.println("getFiles");
		
		return java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			            return _getFiles(_path);
			        }
			    }
			 );
	}
	
	public String _getFiles(String path) {
		String ret = ""; 
		try{
			File folder = new File(path);
			
			FileFilter fileFilter = new FileFilter() {
				public boolean accept(File file) {
					return file.isFile();
					}
				};
				
			File[] listOfFiles = folder.listFiles(fileFilter);
			
			for (int i = 0; i < listOfFiles.length; i++) {
				ret += listOfFiles[i].getName();
				if (i < listOfFiles.length -1 && ret != "")
					ret += "|";
			}
			
		}
		catch (Exception ex){
			ex.printStackTrace();
		}
		
		return ret;
	}
	
	public String getFolders(String path) {
		final String _path = path;
		
		System.out.println("getFolders");
		
		return java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			            return _getFolders(_path);
			        }
			    }
			 );
	}
		
	public String _getFolders(String path) {
		String ret = ""; 
		try{
			File folder = new File(path);
			
			FileFilter fileFilter = new FileFilter() {
				public boolean accept(File file) {
					return file.isDirectory();
					}
				};
				
			File[] subFolders = folder.listFiles(fileFilter);
				
			for (int i = 0; i < subFolders.length; i++) {
				ret += subFolders[i].getName();
				if (i < subFolders.length && ret != "")
					ret += "|";
			}
		}
		catch (Exception ex){
			ex.printStackTrace();
		}
		
		return ret;
	}
	
	public String getUniqueFileName(String prefix, String extension) {
		final String _prefix = prefix;
		final String _extension = extension;
		
		System.out.println("getUniqueFileName");
		
		return java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			            return _getUniqueFileName(_prefix, _extension);
			        }
			    }
			 );
	}
		
	private String _getUniqueFileName(String prefix, String extension) {
	    return new StringBuilder().append(prefix)
	        .append(UUID.randomUUID()).append(".").append(extension).toString();
	}
	
	
	public String getTempName() {
		
		System.out.println("getTempName");
		
		return java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			            return _getTempName();
			        }
			    }
			 );
	}
		
	public String _getTempName(){
		String ret = "";
		try{
			ret =getUniqueFileName("", "tmp");
		}
		catch (Exception ex){
			ex.printStackTrace();
		}
		return ret;		
	}
	
	public String getSpecialFolder() {
		System.out.println("getSpecialFolder");
		
		return java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			            return _getSpecialFolder();
			        }
			    }
			 );
	}
		
	private String _getSpecialFolder(){
		String property = "java.io.tmpdir";
		String tempDir = "";
		
		try
		{
			if (isLinuxOs()) {
				System.out.println("isLinuxOs");
				property = "user.home"; // for linux save to the home path
				tempDir = System.getProperty(property) + "/";
			} else {
				System.out.println("isMicrosoftOs");
				tempDir = System.getProperty(property);
			}
				
			System.out.println("return: " + tempDir);
		}
		catch (Exception ex){
			ex.printStackTrace();
			tempDir = "c:\\temp";
		}
		
		return tempDir;
	}
	
	static public String getExtensionName(String path) {
		final String _path = path;
		
		System.out.println("getExtensionName");
		
		return java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			            return _getExtensionName(_path);
			        }
			    }
			 );
	}
		
	protected static String _getExtensionName(String path){
		String ret = "";
		
		int dot = path.lastIndexOf(".");
		if (dot > -1) ret = path.substring(dot+1);
		
		return ret;
	}
	
	public boolean deleteFile(String path, boolean force) {
		final String _path = path;
		final boolean _force = force;
		
		System.out.println("deleteFile");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			            return _deleteFile(_path, _force);
			        }
			    }
			 );
		return b.booleanValue();
	}
		
	public boolean _deleteFile(String path, boolean force){
		boolean ret = false;
		try{
			File file = new File(path);
			if (!file.canWrite() || force)
				ret = file.delete();
		}
		catch (Exception ex){
			ex.printStackTrace();
		}
		return ret;			
	}
	
	public boolean createEmptyFile(String path, boolean overwrite) {
		final String _path = path;
		final boolean _overwrite = overwrite;
		
		System.out.println("createEmptyFile");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			            return _createEmptyFile(_path, _overwrite);
			        }
			    }
			 );
		return b.booleanValue();
	}
		
	public boolean _createEmptyFile(String path, boolean overwrite){
		boolean ret = false;
		try{
			File file = new File(path);
			if (!file.exists() || overwrite){
				file.delete();
				ret = file.createNewFile();
			}
			else ret = false;
		}
		catch (Exception ex){
			ex.printStackTrace();
		}
		return ret;			
	}
	
	public boolean createTextFile(String path, boolean overwrite, String[] text) {
		final String _path = path;
		final boolean _overwrite = overwrite;
		final String[] _text = text; 
		System.out.println("createTextFile");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			            return _createTextFile(_path, _overwrite, _text);
			        }
			    }
			 );
		return b.booleanValue();
	}

	public boolean _createTextFile(String path, boolean overwrite, String[] text){
		boolean ret = false;
		PrintWriter pw = null;
		try{
			File file = new File(path);
			if (!file.exists() || overwrite){
				pw = new PrintWriter(file);
			
				for (int i=0; i<text.length; i++)
					pw.println(text[i]);

				pw.flush();
				pw.close();
				ret = true;
			}
			else ret = false;
		}
		catch (Exception ex){
			ex.printStackTrace();
		}
		finally {
			
			if (pw != null){
				pw.close();
			}
				
		}
		
		return ret;			
	}

	public boolean copyToClipboard(String text) {
		final String _text = text;

		System.out.println("copyToClipboard: " + text);
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			        	Boolean ret = false;
			        	try{
				        	StringSelection stringSelection = new StringSelection(_text);
				        	Clipboard clipboard = Toolkit.getDefaultToolkit().getSystemClipboard();
				        	clipboard.setContents( stringSelection, null );
			        		ret = true;
			        	}
			        	catch(Exception e){
			        		e.printStackTrace();
			        	}
			        	return ret;
			        }
			    }
			 );
		return b.booleanValue();
	}
	
	public boolean openFile(String path) {
		final String _path = path;

		System.out.println("openFile: " + _path);
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			        	Boolean ret = false;
			        	try{
			        		File fileToOpen = new File(_path);
			        		if (fileToOpen.exists()) {
			        			if (isWindowsOs())
				        	        {
				        				System.out.println("Sistema OP Windows");
				        	            Runtime.getRuntime().exec(new String[]
				        	            {"rundll32", "url.dll,FileProtocolHandler",_path});
				        	            ret =  true;
				        	        } 
			        			else if (isLinuxOs() || isMacOs())
				        	        {
				        	        	System.out.println("Sistema OP Mac/Linux");
				        	            Runtime.getRuntime().exec(new String[]{"/usr/bin/open",_path});
				        	            ret =  true;
				        	        } 
			        			else
				        	        {
				        	            if (Desktop.isDesktopSupported())
				        	            {
				        	            	System.out.println("Sistema OP Non definito");
				        	                Desktop.getDesktop().open(fileToOpen);
				        	                ret =  true;
				        	            }
				        	            else
				        	            {
				        	            	ret =  false;
				        	            }
				        	        }
			        		}
			        		else {
			        			System.out.println("File non trovato!");
			        		}
			        	}
			        	catch(Exception e){
			        		e.printStackTrace();
			        	}
			        	return ret;
			        }
			    }
			 );
		return b.booleanValue();
	}
	
	public boolean saveFile(String path, String encodedText) {
		final String _path = path;
		final String _encodedText = encodedText;

		System.out.println("saveFile");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			        	Boolean ret = false;
			        	try{
			        		byte[] ba = Base64.decode(_encodedText.toCharArray());
			        		
			        		ret = _saveFile(_path, ba);
			        	}
			        	catch(Exception e){
			        		e.printStackTrace();
			        	}
			        	return ret;
			        }
			    }
			 );
		return b.booleanValue();
	}
		
	private boolean _saveFile(String path, byte[] content) {
		boolean ret = false;
		
		File file = new File(path);
		
		try{
			if (file.exists()){
				file.delete();
			}
			FileOutputStream filestream = new FileOutputStream(path);
			filestream.write(content, 0, content.length);
			filestream.flush();
			filestream.close();

			ret = true;
		}
		catch (Exception ex){
			ret = false;
		}
		
		return ret;
	}
	
	public boolean saveFileFromURL(String path, String url, String parameters) {
		final String _path = path;
		final String _url = url;
		final String _parameters = parameters;
		//PARAMITERS <<NAME:VALUE>,<NAME:VALUE>,...>
		
		System.out.println("saveFileFromURL");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			        	Boolean ret = false;
			        	try{
			        		ret = ServletIOManager._receiveGenericFile(_path, _url, _parameters);
			        	}
			        	catch(Exception e){
			        		//e.printStackTrace();
			        		System.err.print(e.getMessage());
			        		ret = false;
			        	}
			        	
			        	return ret;
			        }
			    }
			 );
		return b.booleanValue();
	}
	
	public boolean sendFiletoURL(String path, String url) {
		final String _path = path;
		final String _url = url;
		//PARAMITERS <<NAME:VALUE>,<NAME:VALUE>,...>
		
		System.out.println("saveFileFromURL");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			        	Boolean ret = false;
			        	try{
			        		ret = ServletIOManager._sendGenericFileToUrl(_path, _url);
			        	}
			        	catch(Exception e){
			        		//e.printStackTrace();
			        		System.err.print(e.getMessage());
			        		ret = false;
			        	}
			        	
			        	return ret;
			        }
			    }
			 );
		return b.booleanValue();
	}
	
	public String getEncodedContent(String path) {
		final String _path = path;
		
		System.out.println("getEncodedContent");
		
		String encodedText = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			        	String ret = "";
			        	try{
			        		//byte[] ba = Base64.decode(_encodedText.toCharArray());
			        		
			        		ret = _getEncodedContent(_path);
			        	}
			        	catch(Exception e){
			        		e.printStackTrace();
			        	}
			        	return ret;
			        }
			    }
			 );
		return encodedText;
	}
	
	private String _getEncodedContent(String path) {
		String ret = "";
		
		File file = new File(path);
		
		try{
			if (file.exists()){
				FileInputStream filestream = new FileInputStream(path);
				byte[] ba = new byte[(int) file.length()];
				filestream.read(ba);
				
				filestream.close();
				
				ret = Base64.encode(ba) ;
			}
		}
		catch (Exception ex){
			ret = "";
		}
		
		return ret;
	}
	
	public String projectToFS(String path, String xmlProject, String urlDoc) {
		final String _path = path;
		final String _xmlProject = xmlProject;
		final String _urlDoc = urlDoc;
		
		System.out.println("projectToFS(" + _path + ", xmlProject, " + urlDoc + ")");
		String retVal = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			        	String ret = null;
			        	try{			        				        	
			        		ret = _projectToFS(_path, _xmlProject, _urlDoc);
			        	}
			        	catch(Exception e){
			        		e.printStackTrace();
			        	}
			        	return ret;
			        }
			    }
			 );
		return retVal;

	}
	
	private String _projectToFS(String path, String xmlProject, String urlDoc){
		List<String> l = null;
		Document document = null;
		String result = "";
		
		try{
			document = loadXMLFromString(xmlProject);
		
//			
//			
//			Node pairsNode = XPathAPI.selectSingleNode(document, ".//MetaInfoFascicolo");
//			Node pairNode = null;
			l = projectNodeToFS(document, path, urlDoc);
//		    if (pairsNode  != null) {
//		    	for (int i = 0; i< pairsNode.getChildNodes().getLength(); i++){
//		    		pairNode = pairsNode.getChildNodes().item(i);
//		    		if (!isWhiteSPaceNode(pairNode)){
//		    				//m_model.map.put(pairNode.getAttributes().getNamedItem("key").getTextContent(),
//		    				//pairNode.getAttributes().getNamedItem("value").getTextContent());
//		    			if (pairNode.getAttributes().getNamedItem("name")!=null)
//		    				System.out.println("pair [Node:" + pairNode.getAttributes().getNamedItem("name").getTextContent()+ "]");
//		    		}
//		    	}
//		    }
//			
			
			for (String s: l)
				result += s + SEPARATORE_RIGA_REPORT;
			
			String rigaReport;
			if (result.indexOf(ESITO_FALLITO) == -1)
				rigaReport = TIPO_RESULT + SEPARATORE_CAMPO_REPORT + ESITO_SUCCESSO + SEPARATORE_CAMPO_REPORT + MESSAGGIO_SUCCESSO;
			else
				rigaReport = TIPO_RESULT + SEPARATORE_CAMPO_REPORT + ESITO_FALLITO + SEPARATORE_CAMPO_REPORT + MESSAGGIO_FALLITO;
			
			result += rigaReport + SEPARATORE_RIGA_REPORT;

		}
		catch(Exception e){
    		e.printStackTrace();
    	}
		
		return result;
	}
	
	private List<String> projectNodeToFS(Node fasc,String path, String urlDoc) throws Exception{
		List<String> log = new ArrayList<String>();
		String rigaReport = null;
		NodeList fascElements = fasc.getChildNodes();
		String localPath = path;
		for (int i=0; i<fascElements.getLength(); i++){
			Node elem = fascElements.item(i);
			
			if (elem.getNodeType() == Node.ELEMENT_NODE){
				if (elem.getLocalName().equalsIgnoreCase("METAINFOFASCICOLO")){
					//crea Cartella
					try{
						String nomeCartella = elem.getAttributes().getNamedItem("nome").getNodeValue();
						localPath += File.separator + nomeCartella; 
						_createFolder(localPath);
						rigaReport = TIPO_DIRECTORY + SEPARATORE_CAMPO_REPORT + localPath + SEPARATORE_CAMPO_REPORT + ESITO_SUCCESSO + SEPARATORE_CAMPO_REPORT ;
						
					}
					catch(Exception e){
						rigaReport = TIPO_DIRECTORY + SEPARATORE_CAMPO_REPORT + localPath + SEPARATORE_CAMPO_REPORT + ESITO_FALLITO + SEPARATORE_CAMPO_REPORT + e.getMessage();
					}
					finally{
						log.add(rigaReport);
						System.out.println(rigaReport);
						List<String> l = projectNodeToFS(elem, localPath, urlDoc);
						log.addAll(l);
					}
				}
				
				if (elem.getLocalName().equalsIgnoreCase("FASCICOLI")){// il tag non sarebbe necessario
					//non fa nulla
					List<String> l = projectNodeToFS(elem, localPath, urlDoc);
					log.addAll(l);
				}
	
				if (elem.getLocalName().equalsIgnoreCase("DOCUMENTI")){// il tag non sarebbe necessario
					List<String> l = projectNodeToFS(elem, localPath, urlDoc);
					log.addAll(l);
				}
	
				if (elem.getLocalName().equalsIgnoreCase("METAINFODOCUMENTO")){
					String nomeFile="";
					String idDocumento="" ;
					try{
						//scarica il documento dal server e lo salva nel path locale
						String nomeDocumento = elem.getAttributes().getNamedItem("fullName").getNodeValue();

						String[] tempExt = nomeDocumento.split("\\.");
						String ext = "";
								
						if (tempExt.length>0)
							ext = tempExt[tempExt.length-1];
						
						nomeFile = localPath + File.separator + elem.getAttributes().getNamedItem("nome").getNodeValue() + "." + ext;
						
						idDocumento = elem.getAttributes().getNamedItem("id").getNodeValue();
						boolean ok = ServletIOManager._receiveGenericFile(nomeFile, urlDoc, "<<ID: " + idDocumento + ">>");
						if (ok){
							rigaReport = TIPO_FILE + SEPARATORE_CAMPO_REPORT + nomeFile + SEPARATORE_CAMPO_REPORT + ESITO_SUCCESSO + SEPARATORE_CAMPO_REPORT ;
						}
						else{
							rigaReport = TIPO_FILE + SEPARATORE_CAMPO_REPORT + nomeFile + SEPARATORE_CAMPO_REPORT + ESITO_FALLITO + SEPARATORE_CAMPO_REPORT + "Errore interno";
							}
						}
					catch(Exception e){
						rigaReport = TIPO_FILE + SEPARATORE_CAMPO_REPORT + nomeFile + SEPARATORE_CAMPO_REPORT + ESITO_FALLITO + SEPARATORE_CAMPO_REPORT + e.getMessage();
					}
					finally{
						log.add(rigaReport);
						System.out.println(rigaReport);
					}
				}		
			}
			
		}
		return log;
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
	
	public Document loadXMLFromString(String xml) throws Exception
	{
	    DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();

	    factory.setNamespaceAware(true);
	    factory.setIgnoringElementContentWhitespace(true);
	    DocumentBuilder builder = factory.newDocumentBuilder();

	    return builder.parse(new InputSource(new StringReader(xml)));
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

	/*
	public String FsToProject(String path, String idProject, String folderCod, String urlPost) {
		final String _path = path;
		final String _idProject = idProject;
		final String _folderCod = folderCod;
		final String _urlPost = urlPost;
		
		System.out.println("FsToProject(" + _path + ", idProject, " + _idProject + ", folderCod, " + _folderCod + ", url" + _urlPost + ")");
		String retVal = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			        	String ret = null;
			        	try{			        				        	
			        		ret = _FsToProject(_path, _idProject, _folderCod, _urlPost);
			        	}
			        	catch(Exception e){
			        		e.printStackTrace();
			        	}
			        	return ret;
			        }
			    }
			 );
		return retVal;

	}
	
	private String _FsToProject(String path, String idProject, String folderCod, String urlPost){
		List<String> l = null;
		Document document = null;
		String result = "";
		
		try{
			
			_getFolders(path)
			
			l = projectNodeToFS(document, idProject, urlPost);

			for (String s: l)
				result += s + SEPARATORE_RIGA_REPORT;
			
			String rigaReport;
			if (result.indexOf(ESITO_FALLITO) == -1)
				rigaReport = TIPO_RESULT + SEPARATORE_CAMPO_REPORT + ESITO_SUCCESSO + SEPARATORE_CAMPO_REPORT + MESSAGGIO_SUCCESSO;
			else
				rigaReport = TIPO_RESULT + SEPARATORE_CAMPO_REPORT + ESITO_FALLITO + SEPARATORE_CAMPO_REPORT + MESSAGGIO_FALLITO;
			
			result += rigaReport + SEPARATORE_RIGA_REPORT;

		}
		catch(Exception e){
    		e.printStackTrace();
    	}
		
		return result;
	}
	
	private List<String> _FsToProject(String path, String idProject, String folderCod, String urlPost) throws Exception{
		String localPath = path;
		
		
		
		for (int i=0; i<fascElements.getLength(); i++){
			Node elem = fascElements.item(i);
			
			if (elem.getNodeType() == Node.ELEMENT_NODE){
				if (elem.getLocalName().equalsIgnoreCase("METAINFOFASCICOLO")){
					//crea Cartella
					try{
						String nomeCartella = elem.getAttributes().getNamedItem("nome").getNodeValue();
						localPath += File.separator + nomeCartella; 
						_createFolder(localPath);
						rigaReport = TIPO_DIRECTORY + SEPARATORE_CAMPO_REPORT + localPath + SEPARATORE_CAMPO_REPORT + ESITO_SUCCESSO + SEPARATORE_CAMPO_REPORT ;
						
					}
					catch(Exception e){
						rigaReport = TIPO_DIRECTORY + SEPARATORE_CAMPO_REPORT + localPath + SEPARATORE_CAMPO_REPORT + ESITO_FALLITO + SEPARATORE_CAMPO_REPORT + e.getMessage();
					}
					finally{
						log.add(rigaReport);
						System.out.println(rigaReport);
						List<String> l = projectNodeToFS(elem, localPath, urlDoc);
						log.addAll(l);
					}
				}
				
				if (elem.getLocalName().equalsIgnoreCase("FASCICOLI")){// il tag non sarebbe necessario
					//non fa nulla
					List<String> l = projectNodeToFS(elem, localPath, urlDoc);
					log.addAll(l);
				}
	
				if (elem.getLocalName().equalsIgnoreCase("DOCUMENTI")){// il tag non sarebbe necessario
					//non fa nulla
					List<String> l = projectNodeToFS(elem, localPath, urlDoc);
					log.addAll(l);
				}
	
				if (elem.getLocalName().equalsIgnoreCase("METAINFODOCUMENTO")){
					String nomeDocumento="";
					String idDocumento="" ;
					try{
						//scarica il documento dal server e lo salva nel path locale
						nomeDocumento = localPath + File.separator + elem.getAttributes().getNamedItem("fullName").getNodeValue();
						idDocumento = elem.getAttributes().getNamedItem("id").getNodeValue();
						boolean ok = ServletIOManager._receiveGenericFile(nomeDocumento, urlDoc, "<<ID: " + idDocumento + ">>");
						if (ok){
							rigaReport = TIPO_FILE + SEPARATORE_CAMPO_REPORT + nomeDocumento + SEPARATORE_CAMPO_REPORT + ESITO_SUCCESSO + SEPARATORE_CAMPO_REPORT ;
						}
						else{
							rigaReport = TIPO_FILE + SEPARATORE_CAMPO_REPORT + nomeDocumento + SEPARATORE_CAMPO_REPORT + ESITO_FALLITO + SEPARATORE_CAMPO_REPORT + "Errore interno";
							}
						}
					catch(Exception e){
						rigaReport = TIPO_FILE + SEPARATORE_CAMPO_REPORT + nomeDocumento + SEPARATORE_CAMPO_REPORT + ESITO_FALLITO + SEPARATORE_CAMPO_REPORT + e.getMessage();
					}
					finally{
						log.add(rigaReport);
						System.out.println(rigaReport);
					}
				}		
			}
			
		}
		return log;
	}
	*/
	
	public static boolean isWindowsOs() {
		String os = System.getProperty("os.name");
		return os.toLowerCase().startsWith("windows");
	}

	public static boolean isMacOs() {
		String os = System.getProperty("os.name");
		return os.toLowerCase().contains("mac");
	}

	public static boolean isLinuxOs() {
		String os = System.getProperty("os.name");
		return os.toLowerCase().contains("linux");
	}
	
}
