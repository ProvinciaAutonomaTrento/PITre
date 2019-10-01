package com.nttdata.scannerApplet.gui;

import java.awt.Color;
import java.awt.Cursor;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.Rectangle;
import java.awt.Stroke;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.awt.event.MouseMotionListener;

import javax.swing.JPanel;

public class ImagePanel extends JPanel implements MouseMotionListener, MouseListener {
	private static final long serialVersionUID = 2710398706704489454L;
	private static final double ZOOM_AMOUNT = 1.3;
	private static final double UPPER_ZOOM_LIMIT = 2.0;
	private static final double LOWER_ZOOM_LIMIT = 0.10;
	private Image image;
	private double zoom = 1.0;
	private boolean isZoomSelectionMode;
	private boolean drawSelection;
	private int originX;
	private int originY;
	private int endX;
	private int endY;
	private final ZoomableScrollPane scrollingContainer;
	private Rectangle selectedRect;

	public ImagePanel(ZoomableScrollPane parent) {
		super();
		this.addMouseMotionListener(this);
		this.addMouseListener(this);
		this.originX = this.originY = this.endX = this.endY = 0;
		this.scrollingContainer = parent;
		this.selectedRect = null;
	}

	@Override
	public void paint(Graphics g) {
		super.paint(g);
		Graphics2D g2d = (Graphics2D) g.create();
		if (this.zoom != 1.0) g2d.scale(this.zoom, this.zoom);
		if (this.isZoomSelectionMode && this.selectedRect != null) {
			g2d.drawImage(this.image, 0, 0, this.selectedRect.width, this.selectedRect.height, this.selectedRect.x,
					this.selectedRect.y, this.selectedRect.x + this.selectedRect.width, this.selectedRect.y
							+ this.selectedRect.height, null);
		} else
			g2d.drawImage(this.image, 0, 0, null);

		g2d = (Graphics2D) g;
		if (this.isZoomSelectionMode && this.drawSelection) {
			int x, y, width, height;
			width = Math.abs(this.endX - this.originX);
			height = Math.abs(this.endY - this.originY);
			if (this.originX > this.endX)
				x = this.endX;
			else
				x = this.originX;
			if (this.originY > this.endY)
				y = this.endY;
			else
				y = this.originY;
			Graphics2D g2 = (Graphics2D) g;
			Stroke oldStroke = g2.getStroke();
			g2.setColor(new Color(0, 0, 0));
			// float[] dash = { 10.0f, 3.0f };
			// g2.setStroke(new BasicStroke(2.0f, BasicStroke.CAP_BUTT, BasicStroke.JOIN_MITER, 10.0f, dash, 0.0f));
			g.drawRect(x, y, width, height);
			g2.setStroke(oldStroke);
		}
	}

	public void setImage(Image image) {
		this.image = image;
		this.setupPreferredSize();
	}

	private void setupPreferredSize() {
		if (this.image != null) {
			int w = (int) Math.round(this.image.getWidth(null) * this.zoom);
			int h = (int) Math.round(this.image.getHeight(null) * this.zoom);
			if (this.isZoomSelectionMode && this.selectedRect != null) {
				w = (int) (this.selectedRect.width * this.zoom);
				h = (int) (this.selectedRect.height * this.zoom);
			}
			Dimension dim = new Dimension(w, h);
			this.setPreferredSize(dim);
		}
	}

	public void setZoom(double currentZoom) {
		this.zoom = currentZoom;
		if (this.zoom > UPPER_ZOOM_LIMIT)
			this.zoom = UPPER_ZOOM_LIMIT;
		else if (this.zoom < LOWER_ZOOM_LIMIT) this.zoom = LOWER_ZOOM_LIMIT;
		this.setupPreferredSize();
	}

	public void fitImage(int panel_w, int panel_h) {
		int image_w = this.image.getWidth(null);
		int image_h = this.image.getHeight(null);
		if (this.isZoomSelectionMode && this.selectedRect != null) {
			image_w = this.selectedRect.width;
			image_h = this.selectedRect.height;
		}
		double xScale, yScale;
		xScale = (double) panel_w / image_w;
		yScale = (double) panel_h / image_h;
		this.zoom = Math.min(xScale, yScale);
		this.zoom *= 0.98; // make it a bit smaller
		this.setupPreferredSize();
	}

	public void zoomIn() {
		this.zoom *= ZOOM_AMOUNT;
		if (this.zoom > UPPER_ZOOM_LIMIT) this.zoom = UPPER_ZOOM_LIMIT;
		this.setupPreferredSize();
	}

	public void zoomOut() {
		this.zoom /= ZOOM_AMOUNT;
		if (this.zoom < LOWER_ZOOM_LIMIT) this.zoom = LOWER_ZOOM_LIMIT;
		this.setupPreferredSize();
	}

	public double getZoom() {
		return this.zoom;
	}

	public void fitImageWidth(int panel_w) {
		int image_w = this.image.getWidth(null);
		this.zoom = (double) panel_w / image_w;
		this.zoom *= 0.98; // make it a bit smaller
		this.setupPreferredSize();
	}

	public void zoomToSelection() {
		int w = Math.abs(this.originX - this.endX);
		int h = Math.abs(this.originY - this.endY);
		int x, y;
		if (this.originX > this.endX)
			x = this.endX;
		else
			x = this.originX;
		if (this.originY > this.endY)
			y = this.endY;
		else
			y = this.originY;
		x = (int) Math.round(x / this.zoom) + this.selectedRect.x;
		y = (int) Math.round(y / this.zoom) + this.selectedRect.y;
		w = (int) Math.round(w / this.zoom);
		h = (int) Math.round(h / this.zoom);
		this.selectedRect = new Rectangle(x, y, w, h);
		int pW = this.scrollingContainer.getWidth();
		int pH = this.scrollingContainer.getHeight();
		this.setupPreferredSize();
		this.fitImage(pW, pH);
		this.scrollingContainer.setVieportToNewView();
		this.repaint();
	}

	public void setZoomSelectionMode(boolean value) {
		this.isZoomSelectionMode = value;
		if (this.isZoomSelectionMode) {
			this.selectedRect = new Rectangle(this.image.getWidth(null), this.image.getHeight(null));
		} else {
			this.selectedRect = null;
			this.fitImage(this.scrollingContainer.getWidth(), this.scrollingContainer.getHeight());
			this.setupPreferredSize();
		}
	}

	public void mouseDragged(MouseEvent e) {
		if (this.isZoomSelectionMode && !this.drawSelection) {
			this.drawSelection = true;
			this.originX = e.getX();
			this.originY = e.getY();
		}
		this.endX = e.getX();
		this.endY = e.getY();
		this.repaint();

	}

	
	public void mouseMoved(MouseEvent e) {
	}

	
	public void mouseClicked(MouseEvent e) {
	}

	
	public void mousePressed(MouseEvent e) {
	}

	
	public void mouseReleased(MouseEvent e) {
		if (this.isZoomSelectionMode) {
			this.drawSelection = false;
			this.endX = e.getX();
			if (this.endX < 0)
				this.endX = 0;
			else if (this.endX > this.getWidth()) this.endX = this.getWidth();
			this.endY = e.getY();
			if (this.endY < 0)
				this.endY = 0;
			else if (this.endY > this.getHeight()) this.endY = this.getHeight();
			this.zoomToSelection();
		}

	}

	
	public void mouseEntered(MouseEvent e) {
		if (this.isZoomSelectionMode) this.setCursor(Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR));
	}

	
	public void mouseExited(MouseEvent e) {
		this.setCursor(Cursor.getDefaultCursor());
	}
}
