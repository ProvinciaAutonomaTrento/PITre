package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.ImageIcon;
import javax.swing.JOptionPane;

import com.nttdata.scannerApplet.model.ScannerManager;

public class SavePdfAction extends AbstractAction implements Runnable {
	private static final long serialVersionUID = 8608443163950579118L;

	public SavePdfAction(String text, ImageIcon imageIcon) {
		super(text, imageIcon);
	}

	
	public void actionPerformed(ActionEvent event) {
		new Thread(this).start();
	}

	
	public synchronized void run() {
		ScannerManager instance = ScannerManager.getInstance();
		String path = instance.writePdf();
		JOptionPane.showMessageDialog(null, "File saved at: " + path);
	}

	@Override
	public boolean isEnabled() {
		return true;
	}
}
