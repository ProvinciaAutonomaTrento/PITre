package com.nttdata.fsoApplet;

import java.io.Closeable;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URI;
import java.util.Enumeration;
import java.util.LinkedList;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;
import java.util.zip.ZipInputStream;
import java.util.zip.ZipOutputStream;

public class zipUtil {
	public static void zip(File directory, File zipfile) throws IOException {
	    URI base = directory.toURI();
	    

	    LinkedList <File> queue = new LinkedList<File>();
	    //queue.push(directory);
	    queue.addFirst(directory);
	    OutputStream out = new FileOutputStream(zipfile);
	    Closeable res = out;
	    try {
	      ZipOutputStream zout = new ZipOutputStream(out);

	      while (!queue.isEmpty()) {
//	        directory = queue.pop();
	    	directory = queue.removeFirst();  
	        for (File kid : directory.listFiles()) {
	          String name = base.relativize(kid.toURI()).getPath();
	          if (kid.isDirectory()) {
//	            queue.push(kid);
	        	queue.addFirst(kid);
	            name = name.endsWith("/") ? name : name + "/";
	            zout.putNextEntry(new ZipEntry(name));
	          } else {
	            zout.putNextEntry(new ZipEntry(name));
	            copy(kid, zout);
	            zout.closeEntry();
	          }
	        }
	      }
	    } finally {
	      res.close();
	    }
	  }
	
	
	public static void unzip(File zipfile, File directory) throws IOException {
	    ZipFile zfile = new ZipFile(zipfile);
	    Enumeration<? extends ZipEntry> entries = zfile.entries();
	    while (entries.hasMoreElements()) {
	      ZipEntry entry = entries.nextElement();
	      File file = new File(directory, entry.getName());
	      if (entry.isDirectory()) {
	        file.mkdirs();
	      } else {
	        file.getParentFile().mkdirs();
	        InputStream in = zfile.getInputStream(entry);
	        try {
	          copy(in, file);
	        } finally {
	          in.close();
	        }
	      }
	    }
	  }

	public static void unzip(InputStream zipIS, File directory) throws IOException {
	    final int BUFFER = 4096;
	    byte[] data = new byte[BUFFER];
		ZipInputStream zis = new ZipInputStream(zipIS);
		
//		ZipFile zfile = new ZipFile(zipfile);
	    
		FileOutputStream fos  = null;
		
		ZipEntry entry = null;
		int count = 0;
		
		while ((entry = zis.getNextEntry()) != null){
		      File file = new File(directory, entry.getName());
		      if (entry.isDirectory()) {
		        file.mkdirs();
		      } else {
		        file.getParentFile().mkdirs();
		      
		        fos = new FileOutputStream(directory + "\\" + entry.getName());
//		        dest = new BufferedOutputStream(fos, BUFFER);
    	        while ((count = zis.read(data, 0, BUFFER)) >0){//!= -1) {
    	        	
    	        	fos.write(data, 0, count);
//		        	           dest.write(data, 0, count);
    	        }
    	        fos.flush();
    	        fos.close();
    	        zis.closeEntry();
		      }			
		}
		
		zis.close();
		
	  }
	
	private static void copy(InputStream in, OutputStream out) throws IOException {
	    byte[] buffer = new byte[1024];
	    while (true) {
	      int readCount = in.read(buffer);
	      if (readCount < 0) {
	        break;
	      }
	      out.write(buffer, 0, readCount);
	    }
	  }

	  private static void copy(File file, OutputStream out) throws IOException {
	    InputStream in = new FileInputStream(file);
	    try {
	      copy(in, out);
	    } finally {
	      in.close();
	    }
	  }

	  private static void copy(InputStream in, File file) throws IOException {
	    OutputStream out = new FileOutputStream(file);
	    try {
	      copy(in, out);
	    } finally {
	      out.close();
	    }
	  }

}
