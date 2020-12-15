package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.ImageIcon;
import javax.swing.JDialog;
//import javax.swing.JLabel;
//import javax.swing.JOptionPane;

//import com.nttdata.scannerApplet.lang.GUI_LABELS;
import com.nttdata.scannerApplet.model.Controller;
import com.nttdata.scannerApplet.model.utils.AppletOptions;

public class SaveAction extends AbstractAction implements Runnable {
	private static final long serialVersionUID = 8608443163950579118L;
	private final JDialog parentDialog;
	
	public SaveAction(String text, ImageIcon imageIcon, JDialog dialog) {
		super(text, imageIcon);
		this.parentDialog = dialog;
	}

	
	public void actionPerformed(ActionEvent event) {
		new Thread(this).start();
	}

	
	public synchronized void run() {
		PanelCallback.block();
		
		Controller instance = Controller.getInstance();
		AppletOptions options = AppletOptions.getInstance();
		String savePath = options.getSavePath();
		System.out.println(savePath);
		boolean applyCompression = options.isCompressionApplied();
		
		instance.writeFile(savePath, applyCompression);
		this.parentDialog.setVisible(false);
	}

	@Override
	public boolean isEnabled() {
		return true;
	}
}
