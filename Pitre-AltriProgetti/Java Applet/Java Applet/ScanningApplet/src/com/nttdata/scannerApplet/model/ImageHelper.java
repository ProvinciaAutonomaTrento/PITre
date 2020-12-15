package com.nttdata.scannerApplet.model;

import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.List;

public class ImageHelper {

	public static List<BufferedImage> createBufferedImages(List<Image> images) {
		List<BufferedImage> bImages = new ArrayList<BufferedImage>();
		for (Image img : images)
			bImages.add(createBufferedImage(img));
		return bImages;
	}

	public static BufferedImage createBufferedImage(Image img) {
		BufferedImage buf = new BufferedImage(img.getWidth(null), img.getHeight(null), BufferedImage.TYPE_INT_RGB);
		buf.getGraphics().drawImage(img, 0, 0, null);
		return buf;
	}

	public static BufferedImage rotateBufferedImage(BufferedImage src, double degrees) {
		double radians = Math.toRadians(degrees);

		int srcWidth = src.getWidth();
		int srcHeight = src.getHeight();

		/*
		 * Calculate new image dimensions
		 */
		double sin = Math.abs(Math.sin(radians));
		double cos = Math.abs(Math.cos(radians));
		int newWidth = (int) Math.floor(srcWidth * cos + srcHeight * sin);
		int newHeight = (int) Math.floor(srcHeight * cos + srcWidth * sin);

		/*
		 * Create new image and rotate it
		 */
		BufferedImage result = new BufferedImage(newWidth, newHeight, src.getType());
		Graphics2D g = result.createGraphics();
		g.translate((newWidth - srcWidth) / 2, (newHeight - srcHeight) / 2);
		g.rotate(radians, srcWidth / 2, srcHeight / 2);
		g.drawRenderedImage(src, null);

		return result;
	}
}
