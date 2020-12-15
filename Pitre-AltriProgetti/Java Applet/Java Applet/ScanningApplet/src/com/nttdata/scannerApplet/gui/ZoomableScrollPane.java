package com.nttdata.scannerApplet.gui;


import java.awt.Graphics;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Image;
import java.awt.Point;
import java.awt.Rectangle;

import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.SwingConstants;

import com.nttdata.scannerApplet.lang.GUI_LABELS;

public class ZoomableScrollPane extends JScrollPane {
	private static final long serialVersionUID = -3226396152051697980L;
	private boolean hasImage;
	private final ImagePanel imagePanel;
	private double oldZoom;
	private Rectangle oldView;

	public ZoomableScrollPane() {
		super();

		int verticalScrollSpeed = 16;
		this.getVerticalScrollBar().setUnitIncrement(verticalScrollSpeed);

		this.imagePanel = new ImagePanel(this);
		this.saveZoomInformation();
		this.hasImage = false;

		this.updateImage();
	}

	private void updateImage() {
		if (this.hasImage == false)
			this.setViewportView(new JLabel(GUI_LABELS.noImage, SwingConstants.HORIZONTAL));
		else {
			JPanel centeringPanel = new JPanel(new GridBagLayout());
			centeringPanel.add(this.imagePanel, new GridBagConstraints());
			this.setViewportView(centeringPanel);

			// calculate the new view position
			double newZoom = this.imagePanel.getZoom();
			Point newViewPos = new Point();
			newViewPos.x = (int) Math.max(0, (this.oldView.x + this.oldView.width / 2) * newZoom / this.oldZoom
					- this.oldView.width / 2);
			newViewPos.y = (int) Math.max(0, (this.oldView.y + this.oldView.height / 2) * newZoom / this.oldZoom
					- this.oldView.height / 2);
			this.getViewport().setViewPosition(newViewPos);
		}
	}

	public void setImage(Image image) {
		if (image != null) {
			this.hasImage = true;
			this.imagePanel.setImage(image);
		} else
			this.hasImage = false;
		this.updateImage();
	}

	private void saveZoomInformation() {
		this.oldZoom = this.imagePanel.getZoom();
		this.oldView = this.getViewport().getViewRect();
	}

	public void fitImageToPanel() {
		this.saveZoomInformation();
		this.imagePanel.fitImage(this.getWidth(), this.getHeight());
		this.updateImage();
	}

	public void setZoom(int zoom) {
		this.saveZoomInformation();
		this.imagePanel.setZoom(zoom / 100.0);
		this.updateImage();
	}

	public void zoomIn() {
		this.saveZoomInformation();
		this.imagePanel.zoomIn();
		this.updateImage();
	}

	public void zoomOut() {
		this.saveZoomInformation();
		this.imagePanel.zoomOut();
		this.updateImage();
	}

	public double getZoom() {
		return this.imagePanel.getZoom();
	}

	public void fitImageToWidth() {
		this.saveZoomInformation();
		this.imagePanel.fitImageWidth(this.getWidth());
		this.updateImage();
	}

	public void setZoomSelectionMode(boolean value) {
		this.imagePanel.setZoomSelectionMode(value);
		this.updateImage();
	}

	@Override
	public void paint(Graphics g) {
		super.paint(g);
	}

	public void setVieportToNewView() {
		JPanel centeringPanel = new JPanel(new GridBagLayout());
		centeringPanel.add(this.imagePanel, new GridBagConstraints());
		this.setViewportView(centeringPanel);
	}

	public void showTrueSize() {
		this.saveZoomInformation();
		this.imagePanel.setZoom(1);
		this.updateImage();
	}
}
