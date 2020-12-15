package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.ImageIcon;

import com.nttdata.scannerApplet.model.Controller;

public class ScanImageAction extends AbstractAction implements Runnable {
	private static final long serialVersionUID = 2639046446750196602L;

	public ScanImageAction(String text, ImageIcon image) {
		super(text, image);
	}

	
	public void actionPerformed(ActionEvent event) {
		new Thread(this).start();
	}

	
	public synchronized void run() {
		PanelCallback.block();
		Controller instance = Controller.getInstance();
		boolean success = instance.acquireImages();
		if (success) PanelCallback.updateApplet();
		PanelCallback.unblock();
	}

	@Override
	public boolean isEnabled() {
		return true;
	}
}
