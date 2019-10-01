package com.nttdata.scannerApplet.model.scanner_manager;


import java.awt.Image;
import java.awt.Toolkit;
import java.awt.image.ImageConsumer;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import javax.swing.JOptionPane;

import com.nttdata.scannerApplet.gui.SaneConnectionOptionPane;
import com.nttdata.scannerApplet.gui.SaneOptionsPanel;
import com.nttdata.scannerApplet.lang.GUI_LABELS;
import com.nttdata.scannerApplet.model.utils.SaneConnectionInfo;

import SK.gnome.morena.Morena;
import SK.gnome.morena.MorenaException;
import SK.gnome.morena.MorenaImage;
import SK.gnome.morena.MorenaSource;
import SK.gnome.sane.SaneConnection;

public class LinuxScannerManager implements ScannerManagerInterface {

	private SaneConnectionInfo saneInfo;

	public LinuxScannerManager() {
		this.saneInfo = null;
	}

	
	public void selectSource() {
		String host, userName;
		int port;
		SaneConnectionOptionPane saneConnectionOptionPane = new SaneConnectionOptionPane(
				"localhost", 6566, "saned");
		int answer = JOptionPane.showConfirmDialog(null,
				saneConnectionOptionPane, GUI_LABELS.scegliConnessione,
				JOptionPane.OK_CANCEL_OPTION);
		if (JOptionPane.OK_OPTION == answer) {
			host = saneConnectionOptionPane.getHost();
			port = saneConnectionOptionPane.getPort();
			userName = saneConnectionOptionPane.getUserName();
			this.saneInfo = new SaneConnectionInfo(host, userName, port);
		}
	}

	
	public List<Image> acquireImages() {
		List<Image> scannedImages = new ArrayList<Image>();
		try {
			MorenaSource scanSource = null;
			if (this.saneInfo == null) {
				JOptionPane.showMessageDialog(null, GUI_LABELS.prepareSane,
						GUI_LABELS.prepareSaneTitolo,
						JOptionPane.WARNING_MESSAGE);
				return null;
			}
			SaneOptionsPanel saneOptionsPanel = new SaneOptionsPanel();
			int answer = JOptionPane.showConfirmDialog(null, saneOptionsPanel,
					GUI_LABELS.scegliParametri, JOptionPane.OK_CANCEL_OPTION);
			if (JOptionPane.OK_OPTION == answer) {
				String host, userName;
				int port;
				host = this.saneInfo.getHost();
				userName = this.saneInfo.getUsername();
				port = this.saneInfo.getPort();
				SaneConnection sc = null;
				try {
					sc = SaneConnection.connect(host, port, userName);
					scanSource = sc.getDefaultSource();
					int resolution = saneOptionsPanel.getResolution();
					scanSource.setResolution(resolution);
					if (saneOptionsPanel.isColorMode())
						scanSource.setColorMode();
					else if (saneOptionsPanel.isGrayscaleMode())
						scanSource.setGrayScaleMode();
					else if (saneOptionsPanel.isBlackAndWhiteMode())
						scanSource.setBitDepth(1);
				} catch (IOException e) {
					JOptionPane.showMessageDialog(null,
							GUI_LABELS.connessioneFallita,
							GUI_LABELS.connessioneFallitaTitolo,
							JOptionPane.ERROR_MESSAGE);
					return null;
				}catch (MorenaException e) {
					JOptionPane.showMessageDialog(null,
							GUI_LABELS.connessioneFallita,
							GUI_LABELS.connessioneFallitaTitolo,
							JOptionPane.ERROR_MESSAGE);
					return null;
				}
				while (true) {
					MorenaImage image = new MorenaImage(scanSource);
					int imageStatus = image.getStatus();
					if (imageStatus == ImageConsumer.STATICIMAGEDONE) {
						System.out.println("Size of acquired image is "
								+ image.getWidth() + " x " + image.getHeight()
								+ " x " + image.getPixelSize());

						scannedImages.add(Toolkit.getDefaultToolkit()
								.createImage(image));
						break; // TODO multi page on linux?
					} else if ((imageStatus == ImageConsumer.IMAGEABORTED)
							|| (imageStatus == ImageConsumer.IMAGEERROR))
						break;
				}
			}
		} finally {
			try {
				Morena.close();
			} catch (MorenaException e) {
				e.printStackTrace();
			}
		}
		return scannedImages;
	}
}
