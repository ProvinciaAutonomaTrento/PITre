package com.nttdata.scannerApplet.gui.actions;


import java.awt.Image;
import java.awt.event.ActionEvent;
import java.awt.image.BufferedImage;
import java.io.File;

import javax.imageio.ImageIO;
import javax.swing.AbstractAction;
import javax.swing.JFileChooser;
import javax.swing.JOptionPane;
import javax.swing.filechooser.FileFilter;

import com.nttdata.scannerApplet.gui.ScanApplet;
import com.nttdata.scannerApplet.model.Controller;

public class SaveImageAction extends AbstractAction implements Runnable {
	private static final long serialVersionUID = 2639066446750196602L;

	private class Filter extends FileFilter {
		String type;

		Filter(String type) {
			this.type = type;
		}

		@Override
		public boolean accept(File file) {
			return file.getName().endsWith(this.type);
		}

		@Override
		public String getDescription() {
			return this.type + " Files";
		}
	}

	private final ScanApplet applet;

	public SaveImageAction(ScanApplet applet) {
		super("save to file");
		this.applet = applet;
	}

	
	public void actionPerformed(ActionEvent event) {
		new Thread(this).start();
	}

	
	public synchronized void run() {
		try {
			Image image = Controller.getInstance().getCurrentImage();
			BufferedImage bufferedImage = new BufferedImage(image.getWidth(null), image.getHeight(null),
					BufferedImage.TYPE_INT_RGB);
			bufferedImage.createGraphics().drawImage(image, 0, 0, null);
			JFileChooser chooser = new JFileChooser();
			
			//String e[] = ImageIO.getWriterFileSuffixes();
			String e[] = ImageIO.getReaderFormatNames();
			for (int i = 0; i < e.length; i++)
				chooser.addChoosableFileFilter(new Filter(e[i]));
			int result = chooser.showSaveDialog(this.applet);
			if (result == JFileChooser.APPROVE_OPTION) {
				String ext = chooser.getFileFilter().getDescription();
				ext = ext.substring(0, ext.indexOf(' ')).toLowerCase();
				File file = chooser.getSelectedFile();
				String name = file.getName();
				if (!name.endsWith(ext)) file = new File(file.getParentFile(), name + "." + ext);
				ImageIO.write(bufferedImage, ext, file);
			}
		} catch (Throwable exception) {
			JOptionPane.showMessageDialog(this.applet, exception.toString(), "Error", JOptionPane.ERROR_MESSAGE);
			exception.printStackTrace();
		}
	}

	@Override
	public boolean isEnabled() {
		return true;
	}
}
