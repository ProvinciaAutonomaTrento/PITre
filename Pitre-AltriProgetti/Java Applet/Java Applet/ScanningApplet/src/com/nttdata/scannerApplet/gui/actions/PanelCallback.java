package com.nttdata.scannerApplet.gui.actions;

import com.nttdata.scannerApplet.gui.ScanningPanel;

public class PanelCallback {
	private static ScanningPanel panel;

	public static void setPanel(ScanningPanel thePanel) {
		panel = thePanel;
	}

	public static void updateApplet() {
		panel.updateData();
	}

	public static void setZoomInfo(int zoom) {
		panel.setZoomInfo(zoom);
	}

	public static boolean isRotateAllActivated() {
		return panel.isRotateAllActivated();
	}

	public static void setZoomSelectionMode() {
		panel.setZoomSelectionMode();
	}

	public static void undoZoomSelectionMode() {
		panel.undoZoomSelectionMode();

	}

	public static void block() {
		panel.block();
	}

	public static void unblock() {
		panel.unblock();
	}
}
