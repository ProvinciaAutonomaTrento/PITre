package com.nttdata.fsoApplet.gui;

import java.applet.Applet;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.BufferedReader;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLConnection;

//import javax.swing.JButton;
//import javax.swing.JLabel;
//import javax.swing.JTextField;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.TransformerConfigurationException;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.TransformerFactoryConfigurationError;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Attr;
import org.w3c.dom.Document;
import org.w3c.dom.Node;

@SuppressWarnings("serial")
public class UploaderManager extends Applet implements ActionListener{
		/// <summary>
	private long longlength = 200*1000*1000;
	// Mapping table from 6-bit nibbles to Base64 characters.
	
	private static final char[] map1 = new char[64];
	   static {
	      int i=0;
	      for (char c='A'; c<='Z'; c++) map1[i++] = c;
	      for (char c='a'; c<='z'; c++) map1[i++] = c;
	      for (char c='0'; c<='9'; c++) map1[i++] = c;
	      map1[i++] = '+'; map1[i++] = '/'; }

	// Mapping table from Base64 characters to 6-bit nibbles.
	private static final byte[] map2 = new byte[128];
	   static {
	      for (int i=0; i<map2.length; i++) map2[i] = -1;
	      for (int i=0; i<64; i++) map2[map1[i]] = (byte)i; }

		@Override 
		public void init()
		{
			System.out.println("UploadManager INIT ");
		}
		
		
		public void actionPerformed(ActionEvent e) {
			
			
		}
			
			private String getName(String fullName)
			{
				File info = new File(fullName);
				return info.getName();
				
			}

			public void closeApplet(){
				this.destroy(); 
			} 
			
			public String getExtension(String fullName)
			{
				return FsoApplet._getExtensionName(fullName);
			}

			
			private byte[] GetContentByteArray(String fileName) throws Exception
			{
				File f = null;
				FileInputStream is = null;
				byte[] bytes = null;
				
				try{
					f = new File(fileName);
					is = new FileInputStream(f);
	
					long length = f.length();
	
					if (length >= longlength ) throw new IOException("File size >= " + Long.toString(longlength / 1000000) + " MBytes");
					
					
					// Create the byte array to hold the data
					bytes = new byte[(int)length];
	
					// Read in the bytes
					int offset = 0;
					int numRead = 0;
					while (offset < bytes.length && (numRead=is.read(bytes, offset, bytes.length-offset)) >= 0) {
					    offset += numRead;
					}
	
					// Ensure all the bytes have been read in
					if (offset < bytes.length)
					    throw new IOException("Could not completely read file "+f.getName());
				}
				catch (Exception e){
					e.printStackTrace();
					throw e;
				}
		        finally {		        
		           is.close();		           
		        }
				return bytes;
			}

			private Document GetContentXml(String fileName)
			{
				
				Document doc = null;
				try {
					doc = DocumentBuilderFactory.newInstance().newDocumentBuilder().newDocument();
									
					// Creazione della dichiarazione XML
					//XmlDeclaration xmlDeclaration=document.CreateXmlDeclaration("1.0",null,null); 				
					doc.setXmlVersion("1.0");
					
					//document.InsertBefore(xmlDeclaration,document.DocumentElement);
					//xmlDeclaration=null;
	
					
					
					Node root = doc.createElement("File");
					//XmlNode root=document.CreateNode(XmlNodeType.Element,"File",string.Empty);
					doc.appendChild(root);
					
					//document.InsertBefore(root,document.DocumentElement);
					AppendAttribute(doc, root, "name", this.getName(fileName));
					AppendAttribute(doc, root, "fullPath", fileName);
					AppendAttribute(doc, root, "extension", this.getExtension(fileName));
					AppendAttribute(doc, root, "content", encode(GetContentByteArray(fileName)));
					
	//				this.AppendAttribute(document,root,"name",this.Name);
	//				this.AppendAttribute(document,root,"fullPath",this.FullName);
	//				this.AppendAttribute(document,root,"extension",this.Extension);
	//				this.AppendAttribute(document,root,"content",Convert.ToBase64String(this.GetContentByteArray()));

				} catch (ParserConfigurationException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (Exception e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				return doc;
			}

			private static String encode (byte[] in) {
				int iOff = 0;
				int iLen = in.length;
				   int oDataLen = (iLen*4+2)/3;       // output length without padding
				   int oLen = ((iLen+2)/3)*4;         // output length including padding
				   char[] out = new char[oLen];
				   int ip = iOff;
				   int iEnd = iOff + iLen;
				   int op = 0;
				   while (ip < iEnd) {
				      int i0 = in[ip++] & 0xff;
				      int i1 = ip < iEnd ? in[ip++] & 0xff : 0;
				      int i2 = ip < iEnd ? in[ip++] & 0xff : 0;
				      int o0 = i0 >>> 2;
				      int o1 = ((i0 &   3) << 4) | (i1 >>> 4);
				      int o2 = ((i1 & 0xf) << 2) | (i2 >>> 6);
				      int o3 = i2 & 0x3F;
				      out[op++] = map1[o0];
				      out[op++] = map1[o1];
				      out[op] = op < oDataLen ? map1[o2] : '='; op++;
				      out[op] = op < oDataLen ? map1[o3] : '='; op++; }
				   
				   return new String(out); }

			
			public byte[] GetContentXmlByteArray(String fileName)
			{
				long startTime = System.currentTimeMillis();
				  
				ByteArrayOutputStream stream = null;
				try {
				Document document=this.GetContentXml(fileName);

				DOMSource source = new DOMSource(document);  
				stream = new ByteArrayOutputStream();				  
				StreamResult result = new StreamResult(stream);  
				  
					TransformerFactory.newInstance().newTransformer().transform(source, result);
				} catch (TransformerConfigurationException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (TransformerException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (TransformerFactoryConfigurationError e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}  
				
				long endTime = System.currentTimeMillis();
				
				System.out.println("tempo " + Long.toString(endTime-startTime));

				return stream.toByteArray();
			}

			
			public boolean sendFile(String fileName, String url, String sessionId) {
				final String _fileName = fileName;
				final String _sessionId = sessionId;
				final String _url = url;
				
				System.out.println("privileged");
				
				Boolean b = java.security.AccessController.doPrivileged(
					    new java.security.PrivilegedAction<Boolean>() {
					        public Boolean run() {
					            return _sendFile(_fileName, _url, _sessionId);
					        }
					    }
					 );
				return b.booleanValue();
			}
				
			private boolean _sendFile(String fileName, String url, String sessionId){
				boolean b = false;
				byte[] ba = GetContentXmlByteArray(fileName);
				
				try {
					
					//String host = "";//getDocumentBase().getHost();
					URL servletURL = null;
					servletURL = new URL(url);
					System.out.println("servlet invoked:" + servletURL);
					URLConnection uc = servletURL.openConnection();
					if (uc instanceof HttpURLConnection)
						((HttpURLConnection)uc).setRequestMethod("POST");
					else
						throw new Exception("this connection is NOT an HttpUrlConnection connection");
					
					
					uc.setDoOutput(true);
					uc.setDoInput(true);
					uc.setUseCaches(false);
					uc.setRequestProperty("Content-type","application/octet-stream");
					uc.setRequestProperty("ASP.NET_SessionId", sessionId);

					OutputStream objOut = uc.getOutputStream();

					objOut.write(ba);
					objOut.flush();
					objOut.close();


					BufferedReader in = new BufferedReader(new InputStreamReader(uc.getInputStream()));
					String inputLine;

					while ((inputLine = in.readLine()) != null) 
						System.out.println(inputLine);
					 
					in.close();					
					
					b = true;
				}

					catch (Exception e) {
					e.printStackTrace();
					b=false;
					}				
				
				return b;
				
			}

			private void AppendAttribute(Document document,Node node,String name,String value)
			{
				Attr a = document.createAttribute(name);
				a.setValue(value);
				node.getAttributes().setNamedItem(a);
			}		
	}


