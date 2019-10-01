package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.ImageIcon;

import com.nttdata.scannerApplet.model.Controller;

public class PrevPageAction extends AbstractAction implements Runnable {
	private static final long serialVersionUID = -5084762014923031389L;

	public PrevPageAction(String text, ImageIcon imageIcon) {
		super(text, imageIcon);
	}

	
	public void actionPerformed(ActionEvent event) {
		new Thread(this).start();
	}

	
	public synchronized void run() {
		Controller instance = Controller.getInstance();
		instance.showPrevPage();
		PanelCallback.updateApplet();
	}

	@Override
	public boolean isEnabled() {
		return true;
	}
}
