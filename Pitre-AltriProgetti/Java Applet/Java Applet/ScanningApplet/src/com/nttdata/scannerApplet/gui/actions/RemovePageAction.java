package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.ImageIcon;

import com.nttdata.scannerApplet.model.Controller;

public class RemovePageAction extends AbstractAction implements Runnable {
	private static final long serialVersionUID = 2900250994137411230L;

	public RemovePageAction(String removepage, ImageIcon imageIcon) {
		super(removepage, imageIcon);
	}

	
	public void actionPerformed(ActionEvent event) {
		new Thread(this).start();
	}

	
	public synchronized void run() {
		Controller instance = Controller.getInstance();
		instance.removeCurrentPage();
		PanelCallback.updateApplet();
	}

	@Override
	public boolean isEnabled() {
		return true;
	}
}
