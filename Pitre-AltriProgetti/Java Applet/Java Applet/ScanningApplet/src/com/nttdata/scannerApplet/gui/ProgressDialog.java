package com.nttdata.scannerApplet.gui;

import java.awt.Color;

import javax.swing.JDialog;
import javax.swing.JPanel;
import javax.swing.JProgressBar;

public class ProgressDialog extends JDialog {
	private JProgressBar progressBar = null;
	private String progressText = null;

	public ProgressDialog(String title, String text, JDialog parent){
		super(parent);
		JPanel jp = new JPanel();
		
		jp.setSize(170, 50);
		jp.setBackground(Color.ORANGE);
		
		
		progressBar = new JProgressBar(0, 100);
		progressBar.setSize(170, 50);
		
		progressText = text;
	    progressBar.setStringPainted(true); 
	    
	    jp.add(progressBar);
	    progressBar.setVisible(true);
	    
	    this.setTitle(title);
	    this.setSize(180, 60);
	    this.setVisible(true);
	    this.setAlwaysOnTop(true);
	    this.setResizable(false);
	    this.setModal(true);
	    this.add(jp);
	    this.setLocationRelativeTo(parent);
        
	    setProgress(0);
	}

	public void setProgress(int value){
		progressBar.setValue(value);
		progressBar.setString(progressText + " " + value + "%");
		
	}
}
