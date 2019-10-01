package com.nttdata.scannerApplet.gui;

import java.util.HashMap;
import java.util.Map;

import javax.swing.ImageIcon;

public class IconLoader {
	private static IconLoader instance = null;
	private final Map<String, ImageIcon> icons;
	public final static String acquire = "acquire", cancel = "cancel", close = "close", previous = "previous",
			next = "next", rotate90Clock = "rotate90Clockwise", rotate90CounterClock = "rotate90NotClockwise",
			rotate180 = "rotate180", createDoc = "createDoc", zoomIn = "zoomIn", zoomOut = "zoomOut";

	public static synchronized IconLoader getInstance() {
		if (instance == null) instance = new IconLoader();
		return instance;
	}

	private IconLoader() {
		this.icons = new HashMap<String, ImageIcon>();
		this.loadIcons();
	}

	private void loadIcons() {
		ImageIcon acquireIco = new ImageIcon(IconLoader.class.getResource("Icons/Acquire.jpg"));
		this.icons.put(acquire, acquireIco);
		ImageIcon cancelIco = new ImageIcon(IconLoader.class.getResource("Icons/Cancel.jpg"));
		this.icons.put(cancel, cancelIco);
		ImageIcon closeIco = new ImageIcon(IconLoader.class.getResource("Icons/Close.jpg"));
		this.icons.put(close, closeIco);
		ImageIcon previousIco = new ImageIcon(IconLoader.class.getResource("Icons/MovePrevious.jpg"));
		this.icons.put(previous, previousIco);
		ImageIcon nextIco = new ImageIcon(IconLoader.class.getResource("Icons/MoveNext.jpg"));
		this.icons.put(next, nextIco);
		ImageIcon Rotate90Ico = new ImageIcon(IconLoader.class.getResource("Icons/Rotate90.jpg"));
		this.icons.put(rotate90Clock, Rotate90Ico);
		ImageIcon Rotate90CIco = new ImageIcon(IconLoader.class.getResource("Icons/Rotate90counter.png"));
		this.icons.put(rotate90CounterClock, Rotate90CIco);
		ImageIcon Rotate180Ico = new ImageIcon(IconLoader.class.getResource("Icons/Rotate180.jpg"));
		this.icons.put(rotate180, Rotate180Ico);
		ImageIcon creaIco = new ImageIcon(IconLoader.class.getResource("Icons/Save.jpg"));
		this.icons.put(createDoc, creaIco);
		ImageIcon zoomPlus = new ImageIcon(IconLoader.class.getResource("Icons/ZoomIn.png"));
		this.icons.put(zoomIn, zoomPlus);
		ImageIcon zoomMinus = new ImageIcon(IconLoader.class.getResource("Icons/ZoomOut.png"));
		this.icons.put(zoomOut, zoomMinus);
	}

	public ImageIcon getIcon(String icon) {
		return this.icons.get(icon);
	}
}
