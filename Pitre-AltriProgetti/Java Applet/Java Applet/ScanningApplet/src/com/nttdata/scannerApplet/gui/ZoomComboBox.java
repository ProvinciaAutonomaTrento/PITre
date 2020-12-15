package com.nttdata.scannerApplet.gui;


import java.awt.event.ActionEvent;

import javax.swing.JComboBox;
import javax.swing.JOptionPane;

import com.nttdata.scannerApplet.gui.actions.PanelCallback;
import com.nttdata.scannerApplet.lang.GUI_LABELS;

public class ZoomComboBox extends JComboBox{ //<String> non è possibile in 1.6
	private static final long serialVersionUID = -4357685045701799047L;
	private final ZoomableScrollPane imageViewer;

	public ZoomComboBox(ZoomableScrollPane imgScrollPane) {
		super();
		this.addActionListener(this);
		this.imageViewer = imgScrollPane;
	}

	@Override
	public void actionPerformed(ActionEvent e) {
		super.actionPerformed(e);
		String zoomLevel = (String) this.getSelectedItem();
		if (zoomLevel.equals(GUI_LABELS.adatta))
			this.imageViewer.fitImageToPanel();
		else if (zoomLevel.equals(GUI_LABELS.adattaLarghezza))
			this.imageViewer.fitImageToWidth();
		else {
			zoomLevel = zoomLevel.replaceAll("[^\\d]", "");
			try {
				@SuppressWarnings("boxing")
				int zoomPercent = Integer.valueOf(zoomLevel);
				this.imageViewer.setZoom(zoomPercent);
			} catch (NumberFormatException exception) {
				JOptionPane.showMessageDialog(null, GUI_LABELS.nonUnNumero, GUI_LABELS.attenzione,
						JOptionPane.ERROR_MESSAGE);
			}
		}
		int zoom = (int) (100 * this.imageViewer.getZoom());
		PanelCallback.setZoomInfo(zoom);
	}

	public void setCurrentText(String zoom) {
		this.setSelectedItem(zoom);
	}

	public void resetZoom() {
		this.imageViewer.fitImageToPanel();
		int zoom = (int) (100 * this.imageViewer.getZoom());
		PanelCallback.setZoomInfo(zoom);
	}

}
