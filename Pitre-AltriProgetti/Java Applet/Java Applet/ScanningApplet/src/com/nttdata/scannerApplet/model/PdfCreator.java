package com.nttdata.scannerApplet.model;

import java.awt.Image;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.List;

import com.lowagie.text.Document;
import com.lowagie.text.DocumentException;
import com.lowagie.text.pdf.PdfWriter;
import com.nttdata.scannerApplet.gui.ProgressDialog;
import com.nttdata.scannerApplet.gui.ScanApplet;
import com.nttdata.scannerApplet.lang.GUI_LABELS;

public class PdfCreator {

	public static String createPdf(String tempDir, List<Image> imageList, boolean applyCompression) {
		String path = null;
		
			Float quality = 0.7f;
		//for (Float quality = 0.2f; quality <= 1.0f; quality+=0.2f){
			String fName;
			
			fName = "PDF_" + Integer.toString((int)(quality * 10)) + "_temp.pdf";
			path = pdfCompress(tempDir, fName, imageList, applyCompression, quality);
			
			/*}
			else {
				fName = "DOC_" + Integer.toString((int)(quality * 10)) + "_Uncompressed.pdf";
				path = pdfUncompress(tempDir, fName, imageList, applyCompression);			
			}*/
		//}
			
		return path;
	}

	private static String pdfCompress(String tempDir, String fName, List<Image> imageList, boolean applyCompression, float quality){
		String path = tempDir + fName;
		ProgressDialog pg = new ProgressDialog(GUI_LABELS.attesa, GUI_LABELS.attesa, ScanApplet.dialog);
		int nPages = imageList.size();
		
		System.out.println("pdfCompress");

		try {
			
			Document document = new Document();			
			
			PdfWriter writer = PdfWriter.getInstance(document, new FileOutputStream(path));
			if (applyCompression) writer.setFullCompression();
			
			document.open();

			int docWidth = (int)(document.getPageSize().getWidth() - document.leftMargin() - document.rightMargin());
			int docHeight = (int)(document.getPageSize().getHeight() - document.topMargin() - document.bottomMargin());

			int page = 0;
			for (Image img : imageList) {
				page++;
				pg.setProgress(page*100/nPages);
				
				int h;
				int w;
				
				int origWidth = img.getWidth(null);
				int origHeight = img.getHeight(null);
				
				if (((float)docWidth/(float)origWidth) < ((float)docHeight/(float)origHeight)){
					w = docWidth;
					h = (int)(((float)docWidth/(float)origWidth) * (float)origHeight);
				}
				else{
					h = docHeight;
					w = (int)(((float)docHeight/(float)origHeight) * (float)origWidth);
				}
			
				com.lowagie.text.Image image = null;
				
				if (applyCompression) {
					Image scaledImage = img.getScaledInstance(w, h, Image.SCALE_SMOOTH);
					image = com.lowagie.text.Image.getInstance(writer, scaledImage, quality);
				}
				else {
					image = com.lowagie.text.Image.getInstance(img, null);
				}
				
				document.add(image);
				document.newPage();
				System.out.println("Salvato: " + path + " - " + 100 * (float)page / (float)nPages  + "% - Quality: " + quality);
			}
			document.close();
			pg.setVisible(false);
			
		} catch (FileNotFoundException e) {
			path = null;
			e.printStackTrace();
		} catch (DocumentException e) {
			path = null;
			e.printStackTrace();
		} catch (IOException e) {
			path = null;
			e.printStackTrace();
		}

		return path;
	}
	
	public static String pdfUncompress(String tempDir, String fName, List<Image> imageList, boolean applyCompression) {
		Document document = new Document();
		String path = tempDir + fName;
		
		System.out.println("pdfUncompress");
		try {
			PdfWriter writer = PdfWriter.getInstance(document, new FileOutputStream(path));
			
			writer.setFullCompression();
			document.open();

			for (Image img : imageList) {
				com.lowagie.text.Image image = com.lowagie.text.Image.getInstance(img, null);
				float scaler = ((document.getPageSize().getWidth() - document.leftMargin() - document.rightMargin()) / image
						.getWidth()) * 100;
				image.scalePercent(scaler);
				//image.setCompressionLevel(9);
				
				document.add(image);
				document.newPage();
				
				System.out.println("Salvato: " + path);
			}
			document.close();
		} catch (FileNotFoundException e) {
			path = null;
			e.printStackTrace();
		} catch (DocumentException e) {
			path = null;
			e.printStackTrace();
		} catch (IOException e) {
			path = null;
			e.printStackTrace();
		}
		
		return path;
	}
}
