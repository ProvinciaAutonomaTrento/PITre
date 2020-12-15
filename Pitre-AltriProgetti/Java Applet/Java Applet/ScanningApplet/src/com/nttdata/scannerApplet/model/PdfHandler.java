package com.nttdata.scannerApplet.model;

import java.awt.Image;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.List;

import com.lowagie.text.Document;
import com.lowagie.text.DocumentException;
import com.lowagie.text.pdf.PdfWriter;

public class PdfHandler {

	public String createPdf(String tempDir, List<Image> imageList) {
		Document document = new Document();
		String path = tempDir + "tmp.pdf";
		try {
			PdfWriter.getInstance(document, new FileOutputStream(path));
			document.open();

			for (Image img : imageList) {
				com.lowagie.text.Image image = com.lowagie.text.Image.getInstance(img, null);
				float scaler = ((document.getPageSize().getWidth() - document.leftMargin() - document.rightMargin()) / image
						.getWidth()) * 100;
				image.scalePercent(scaler);
				document.add(image);
				document.newPage();
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
