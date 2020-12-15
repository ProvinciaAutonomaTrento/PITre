package com.nttdata.scannerApplet.model;

import java.awt.Image;
import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.List;

public class ImageHandler {
	private final List<Image> scannedImages;
	private int curPage;

	public ImageHandler() {
		this.scannedImages = new ArrayList<Image>();
		this.curPage = 0;
	}

	public void addImage(Image image) {
		this.scannedImages.add(image);
		this.curPage = this.getNumberOfPages();
	}

	public Image getCurrentImage() {
		if (this.scannedImages.isEmpty()) throw new IllegalStateException("No image has been added!");
		return this.scannedImages.get(this.curPage - 1);
	}

	public List<Image> getImageList() {
		return this.scannedImages;
	}

	public int getNumberOfPages() {
		return this.scannedImages.size();
	}

	public int getCurrentPageNumber() {
		return this.curPage;
	}

	public void nextPage() {
		this.curPage++;
		if (this.curPage > this.getNumberOfPages()) this.curPage = 1;
	}

	public void prevPage() {
		this.curPage--;
		if (this.curPage <= 0) this.curPage = this.getNumberOfPages();
	}

	public void removeCurrentPage() {
		if (!this.scannedImages.isEmpty()) this.scannedImages.remove(this.curPage - 1);
		this.prevPage();
	}

	public void rotateImage(int imageNumber, int rotationAmount) {
		Image img = this.scannedImages.get(imageNumber);
		BufferedImage bim = ImageHelper.createBufferedImage(img);
		bim = ImageHelper.rotateBufferedImage(bim, rotationAmount);
		this.scannedImages.set(imageNumber, bim);
	}

	public void rotateCurrentImage(int rotationAmount) {
		this.rotateImage(this.curPage - 1, rotationAmount);
	}

	public boolean hasImages() {
		return !this.scannedImages.isEmpty();
	}

	public void setCurrentPage(int pageNumber) {
		if ((pageNumber >= 1) && (pageNumber <= this.getNumberOfPages())) this.curPage = pageNumber;
	}

	public void rotateAllImages(int rotationAmount) {
		for (int k = 0; k < this.scannedImages.size(); k++)
			this.rotateImage(k, rotationAmount);
	}
}
