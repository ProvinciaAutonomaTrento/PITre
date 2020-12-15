package com.nttdata.fsoApplet;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLConnection;
import java.net.URLEncoder;
import java.util.HashMap;
import java.util.UUID;

public class ServletIOManager {
			public static String _getSpecialFolder(int i){
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
			
			protected static String _getUniqueFileName(String prefix, String extension) {
			    return new StringBuilder().append(prefix)
			        .append(UUID.randomUUID()).append(".").append(extension).toString();
			}
			
			
			protected static InputStream getServletInputStream(String url, String idProject){
				InputStream input = null;
				try {
					OutputStream os = null;
					String params = "idFascicolo="+idProject;
					String host = "";//getDocumentBase().getHost();
					URL servletURL = null;
					if (url.toUpperCase().contains("HTTP://") || url.toUpperCase().contains("HTTPS://"))
						servletURL = new URL(url);
					else
						servletURL = new URL("http://" + host + url);
					
					System.out.println("servlet invoked:" + servletURL);
					URLConnection uc = servletURL.openConnection();
					if (uc instanceof HttpURLConnection)
						((HttpURLConnection)uc).setRequestMethod("POST");
					else
						throw new Exception("this connection is NOT an HttpUrlConnection connection");
					
					
					((HttpURLConnection)uc).setRequestProperty("idFascicolo", idProject);
	//				FileInputStream fis = new FileInputStream(zipFile);
					
					uc.setDoOutput(true);
					uc.setDoInput(true);
					uc.setUseCaches(false);
	//				uc.setRequestProperty("Content-type","application/zip");
					
					os = uc.getOutputStream();					
					OutputStreamWriter writer = new OutputStreamWriter(os);

					writer.write(params);
					writer.close();
					os.close();					
					
					input = uc.getInputStream();
					
				}
				catch (Exception e) {
				e.printStackTrace();
				}				
				return input;
			}
			
			protected static InputStream getGenericServletInputStream(String url, HashMap<String, String> parameters){
				InputStream input = null;
				try {
					OutputStream os = null;
					
					String params = "";

					URL servletURL = null;
					if (url.toUpperCase().contains("HTTP://") || url.toUpperCase().contains("HTTPS://"))
						servletURL = new URL(url);
					else
						servletURL = new URL("http://" + url);
					
					System.out.println("servlet invoked:" + servletURL);
					URLConnection uc = servletURL.openConnection();
					if (uc instanceof HttpURLConnection)
						((HttpURLConnection)uc).setRequestMethod("POST");
					else
						throw new Exception("this connection is NOT an HttpUrlConnection connection");
					
					int i = 0;
					for (String k : parameters.keySet()){	
						i +=1;
						String encoded = URLEncoder.encode(parameters.get(k),"UTF-8");
						((HttpURLConnection)uc).setRequestProperty(k, encoded);
						
						params += k + "=" + encoded;
						
						if (i<parameters.keySet().size()) 
							params += "&";
					}
					
					uc.setDoOutput(true);
					uc.setDoInput(true);
					uc.setUseCaches(false);
					
					os = uc.getOutputStream();					
					OutputStreamWriter writer = new OutputStreamWriter(os);

					writer.write(params);
					writer.close();
					os.close();					
					
					input = uc.getInputStream();
					
				}
				catch (Exception e) {
				e.printStackTrace();
				}				
				return input;
			}
			
			protected static boolean _receiveProjectStream(String url, String idProject, File directory){
				boolean b = false;
				InputStream input = null;
				
				try {
					input = getServletInputStream(url, idProject);

					if (input != null){
						zipUtil.unzip(input, directory);

						input.close();
					}	               
				}

				catch (Exception e) {
					e.printStackTrace();
					if (input != null){
						try {
							input.close();
						} catch (IOException e1) {
							// TODO Auto-generated catch block
							e1.printStackTrace();
						}
					}
				}				
				
				return b;				
			}

			protected static boolean _receiveProjectFile(String zipFileName, String url, String idProject, File directory){
				boolean b = false;
				InputStream input = null;
				//byte[] ba = GetContentXmlByteArray(fileName);
				
				try {
					
					input = getServletInputStream(url, idProject);

					if (input != null){
		                byte[] buffer = new byte[4096];
		                int n = - 1;
	
		                OutputStream output = new FileOutputStream( zipFileName );
		                while ( (n = input.read(buffer)) != -1)
//		                {
//	                        if (n > 0)
	                        output.write(buffer, 0, n);
//		                }
		                output.close();
					}
					input.close();
				}

				catch (Exception e) {
					e.printStackTrace();
					if (input != null){
						try {
							input.close();
						} catch (IOException e1) {
							// TODO Auto-generated catch block
							e1.printStackTrace();
						}
					}
				}				
				
				return b;				
			}

			
			
			protected static HashMap<String, String> _parametersFromString(String sParameters) throws Exception{
				HashMap<String, String> parameters = new HashMap<String, String>();
				
				// <<key, value>,<key, value>, ...>
				
				String work = sParameters.trim();
				int len = work.length();
				// skip < e >
				
				if (len >= 2 ){
					
					if (!work.substring(0,1).equalsIgnoreCase("<") || !work.substring(len-1,len).equalsIgnoreCase(">") )
						throw(new Exception("Malformed string"));
					
					work = work.substring(1, len-1); 
					len = work.length();
					
					// ogni elemento è separato da ","
					String[] tokens = work.split(",");
					
					for (int i=0; i< tokens.length; i++){
						String token = tokens[i].trim();
						len = token.length();
						
						if (len < 3 || !token.substring(0,1).equalsIgnoreCase("<") || !token.substring(len-1,len).equalsIgnoreCase(">") )
							throw(new Exception("Malformed string"));
						
						token = token.substring(1, len-1); 
						
						String[] item = token.split(":");
						
						parameters.put(item[0].trim(), item[1].trim());						
					}
				}
				return parameters;
				
			}
			
			public static boolean _receiveGenericFile(String fileName, String url, String sParameters) throws Exception{
				HashMap<String, String> parameters = _parametersFromString(sParameters);
				
				return _receiveGenericFile(fileName, url, parameters);
			}
				
			public static boolean _sendGenericFileToUrl(String fileName, String url) throws Exception{
				return _sendGenericFile(fileName, url);
			}
			
			protected static boolean _receiveGenericFile(String fileName, String url, HashMap<String, String> parameters){
				boolean b = false;
				InputStream input = null;
				
				try {
					
					input = getGenericServletInputStream(url, parameters);

					if (input != null){
		                byte[] buffer = new byte[4096];
		                int n = - 1;
	
		                OutputStream output = new FileOutputStream(fileName);
		                while ( (n = input.read(buffer)) != -1)
	                        output.write(buffer, 0, n);

		                output.close();
					}
					input.close();
					
					b = true;
				}

				catch (Exception e) {
					e.printStackTrace();
					if (input != null){
						try {
							input.close();
						} catch (IOException e1) {
							// TODO Auto-generated catch block
							e1.printStackTrace();
						}
					}
				}				
				
				return b;				
			}

			protected static boolean _sendProjectFile(String fileName, String url, String idProject){
				boolean b = false;				
				try {
					
					File zipFile = new File(fileName);

					String host = "";
					URL servletURL = null;
					if (url.toUpperCase().contains("HTTP://") || url.toUpperCase().contains("HTTPS://"))
						servletURL = new URL(url);
					else
						servletURL = new URL("http://" + host + url);
					
					System.out.println("servlet invoked:" + servletURL);

				    final String Boundary = "7d021a37605f0";

			        HttpURLConnection uc = (HttpURLConnection) servletURL.openConnection();
			        uc.setUseCaches(false);
			        uc.setDoOutput(true);
			        uc.setDoInput(true);

			        uc.setRequestMethod("POST");

			        uc.setRequestProperty("Connection", "Keep-Alive");
			        uc.setRequestProperty("Content-Type", "multipart/form-data;boundary=" + Boundary);
			        
			        DataOutputStream httpOut = new DataOutputStream(uc.getOutputStream());
			        httpOut.writeBytes("--" + Boundary + "\r\n");
//			        System.out.print("--" + Boundary + "\r\n");
			        
			        if (idProject!=null && !idProject.equalsIgnoreCase("")){
			        	httpOut.writeBytes("Content-Disposition: form-data; name=\"idProject\"\r\n");
//				        System.out.print("Content-Disposition: form-data; name=\"idProject\"\r\n");
	
				        httpOut.writeBytes("\r\n");
//				        System.out.print("\r\n");
				        
				        httpOut.writeBytes(idProject + "\r\n");
//				        System.out.print(idProject + "\r\n");
				        
				        httpOut.writeBytes("--" + Boundary + "\r\n");
//				        System.out.print("--" + Boundary + "\r\n");
			        }
			        
			        httpOut.writeBytes("Content-Disposition: form-data;name=\"filezip\"; filename=\"" + zipFile.getName() + "\"\r\n");
//			        System.out.print("Content-Disposition: form-data;name=\"filezip\"; filename=\"" + zipFile.getName() + "\"\r\n");

			        httpOut.writeBytes("\r\n");
//			        System.out.print("\r\n");

		            FileInputStream fis = new FileInputStream(zipFile);

		            int i;    
				    byte bytes[]=new byte[1024]; 
				    while((i=fis.read(bytes))!=-1)
				    	httpOut.write(bytes, 0, i);
		            
			        httpOut.writeBytes("\r\n");
//			        System.out.print("\r\n");

			        httpOut.writeBytes("--" + Boundary + "\r\n");
//			        System.out.print("--" + Boundary + "\r\n");
			        
			        httpOut.flush();
			        httpOut.close();
				
			        fis.close();
				
					BufferedReader in = new BufferedReader(new InputStreamReader(uc.getInputStream()));
					String inputLine;

					while ((inputLine = in.readLine()) != null) 
						System.out.println(inputLine);
					 
					in.close();					
				}

					catch (Exception e) {
					e.printStackTrace();
					}				
				
				return b;
				
			}

			protected static boolean _sendFileBinary(String fileName, String url){
				boolean b = false;				
				try {
					
					File zipFile = new File(fileName);

					String host = "";//getDocumentBase().getHost();
					URL servletURL = null;
					if (url.toUpperCase().contains("HTTP://") || url.toUpperCase().contains("HTTPS://"))
						servletURL = new URL(url);
					else
						servletURL = new URL("http://" + host + url);
					
					System.out.println("servlet invoked:" + servletURL);
					URLConnection uc = servletURL.openConnection();
					if (uc instanceof HttpURLConnection)
						((HttpURLConnection)uc).setRequestMethod("POST");
					else
						throw new Exception("this connection is NOT an HttpUrlConnection connection");
					
					FileInputStream fis = new FileInputStream(zipFile);
					
					uc.setDoOutput(true);
					uc.setDoInput(true);
					uc.setUseCaches(false);
					uc.setRequestProperty("Content-type","application/zip");

					OutputStream objOut = uc.getOutputStream();

				    int i;    
				    byte bytes[]=new byte[4096]; 
				    while((i=fis.read(bytes))!=-1)
				    {
				    	objOut.write(bytes,0,i);
				    	objOut.flush();
				    }
				    objOut.close();
				    fis.close();

					BufferedReader in = new BufferedReader(new InputStreamReader(uc.getInputStream()));
					String inputLine;

					while ((inputLine = in.readLine()) != null) 
						System.out.println(inputLine);
					 
					in.close();	
					
					b=true;
				}

					catch (Exception e) {
						e.printStackTrace();
					}				
				
				return b;
				
			}

			protected static boolean _sendGenericFile(String fileName, String url){
				boolean b = false;				
				try {
					
					File tempFile = new File(fileName);

					URL servletURL = null;
					
					if (url.toUpperCase().contains("HTTP://") || url.toUpperCase().contains("HTTPS://"))
						servletURL = new URL(url);
					else
						servletURL = new URL("http://" + url);
					
					System.out.println("servlet invoked: " + servletURL);
					URLConnection uc = servletURL.openConnection();
					if (uc instanceof HttpURLConnection)
						((HttpURLConnection)uc).setRequestMethod("POST");
					else
						throw new Exception("this connection is NOT an HttpUrlConnection connection");
					
					uc.setDoOutput(true);
					uc.setDoInput(true);
					uc.setUseCaches(false);
					uc.setRequestProperty("Content-type","application/octet-stream");

					OutputStream objOut = uc.getOutputStream();
					
					if (!tempFile.isDirectory()) {
						FileInputStream fis = new FileInputStream(tempFile);
							
					    int i;    
					    byte bytes[]=new byte[4096]; 
					    
					    while((i=fis.read(bytes))!=-1)
					    {
					    	objOut.write(bytes,0,i);
					    	objOut.flush();
					    }
					    objOut.close();
					    fis.close();
					}
					
						BufferedReader in = new BufferedReader(new InputStreamReader(uc.getInputStream()));
						String inputLine;
	
						while ((inputLine = in.readLine()) != null) 
							System.out.println(inputLine);
						 
						in.close();	
					
					b=true;
				}

				catch (Exception e) {
					//e.printStackTrace();
					System.err.print(e.getMessage());
	        		b = false;
				}				
				
				return b;
				
			}

//			private void AppendAttribute(Document document,Node node,String name,String value)
//			{
//				Attr a = document.createAttribute(name);
//				a.setValue(value);
//				node.getAttributes().setNamedItem(a);
//			}		
	}


