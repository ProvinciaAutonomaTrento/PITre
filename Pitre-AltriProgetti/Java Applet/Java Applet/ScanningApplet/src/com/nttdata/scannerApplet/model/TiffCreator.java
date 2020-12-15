package com.nttdata.scannerApplet.model;

import java.awt.Image;
import java.awt.image.BufferedImage;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.util.List;
import java.util.Vector;

import com.nttdata.scannerApplet.gui.ProgressDialog;
import com.nttdata.scannerApplet.gui.ScanApplet;
import com.nttdata.scannerApplet.lang.GUI_LABELS;
import com.sun.media.jai.codec.ImageCodec;
import com.sun.media.jai.codec.ImageEncoder;
import com.sun.media.jai.codec.TIFFEncodeParam;

public class TiffCreator {
	public static String createTiff(String tempDir, List<Image> images, boolean applyCompression) {
		String path = tempDir + "tmp.tif";
		ProgressDialog pg = new ProgressDialog(GUI_LABELS.attesa, GUI_LABELS.attesa, ScanApplet.dialog);
		int nPages = images.size();
		
		TIFFEncodeParam params = new TIFFEncodeParam();
		OutputStream out;
		try {
			out = new FileOutputStream(path);
			ImageEncoder encoder = ImageCodec.createImageEncoder("tiff", out, params);
			Vector<BufferedImage> vector = new Vector<BufferedImage>();
			List<BufferedImage> bufferedImages = ImageHelper.createBufferedImages(images);
			for (int i = 1; i < bufferedImages.size(); i++) {
				pg.setProgress(i*100/nPages);
				vector.add(bufferedImages.get(i));
			}
			params.setExtraImages(vector.iterator());
			//if (applyCompression) params.setCompression(TIFFEncodeParam.COMPRESSION_DEFLATE);
			if (applyCompression) params.setCompression(TIFFEncodeParam.COMPRESSION_JPEG_TTN2);
			encoder.encode(bufferedImages.get(0));
			out.close();
		} catch (IOException e) {
			path = null;
			e.printStackTrace();
		}

		pg.setVisible(false);
		return path;
	}
}
