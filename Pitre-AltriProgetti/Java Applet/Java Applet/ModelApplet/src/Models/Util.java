package Models;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PrintWriter;

public class Util {
	public static String getSpecialFolder(int sysCode) {
		final int _sysCode = sysCode;
		
		System.out.println("privileged");
		
		return java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<String>() {
			        public String run() {
			            return _getSpecialFolder(_sysCode);
			        }
			    }
			 );
	}
		
	private static String _getSpecialFolder(int i){
		String ret = null;
		switch(i){
		case 2:
			ret = System.getProperty("java.io.tmpdir");
			break;
		default:
			ret = "error";
		}
		
		return ret;			
	}
	

	public static String GetTempFilePath(String fileName ) {
//    On Error GoTo eh
//        
//    Dim fso As Scripting.FileSystemObject
//    Set fso = New Scripting.FileSystemObject
//    
    return (getSpecialFolder(2) + fileName);
//    
//    Set fso = Nothing
//    
//    Exit Function
//eh:
//    Err.Raise Err.Number, _
//              Err.Source & vbCrLf & vbTab & CLASS_NAME & ".GetTempFilePath", _
//              Err.Description
	}

	    
	protected static String _getExtensionName(String path){
		String ret = "";
		
		int dot = path.lastIndexOf(".");
		if (dot > -1) ret = path.substring(dot+1);
		
		return ret;
	}
	
    public static boolean isRTFFormat(String filePath){
     
    	return (_getExtensionName(filePath).indexOf("RTF") > 0);
    }
   
    
    
    public static boolean saveBinaryData(String filePath, String base64Content) throws IOException{
    final String _path = filePath;
    final byte[] decoded = Base64.decode(base64Content.toCharArray());
    	Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			        	FileOutputStream  fos = null;
			        	Boolean ret = false;
			        	try{
				        	fos = new FileOutputStream(_path);
				        	fos.write(decoded);
				        	fos.close();
				        	ret = true;
			        	}
			        	catch (Exception e){e.printStackTrace();}
			        	finally{if (fos!=null)
							try {
								fos.close();
							} catch (IOException e) {
								// TODO Auto-generated catch block
								e.printStackTrace();
							}}
						return ret;
			        }
			    }
			 );
		return b.booleanValue();
	}
    
    
    	
    public static String convertToURL(String fileName){ 
	   if (!fileName.startsWith("/")) 
		   fileName = "/" + fileName; 
	   
	    
	   fileName = fileName.replace('\\', '/'); 
	   fileName = "file://" + fileName;
	   return fileName; 
	}
	


    	

    	//CreateTextFile
    public static boolean createTextFile(String path, boolean overwrite, String[] text) {
    		final String _path = path;
    		final boolean _overwrite = overwrite;
    		final String[] _text = text; 
    		System.out.println("privileged");
    		
    		Boolean b = java.security.AccessController.doPrivileged(
    			    new java.security.PrivilegedAction<Boolean>() {
    			        public Boolean run() {
    			            return _createTextFile(_path, _overwrite, _text);
    			        }
    			    }
    			 );
    		return b.booleanValue();
    	}

    public static boolean _createTextFile(String path, boolean overwrite, String[] text){
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
    			
    		}
    		finally {
    			
    			if (pw != null){
    				pw.close();
    			}
    				
    		}
    		
    		return ret;			
    	}


}
