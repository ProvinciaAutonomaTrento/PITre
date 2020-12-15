package com.nttdata.printApplet.utils;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;

import javax.swing.JFileChooser;

public class FileUtils {

	public static byte[] loadFile() {
		final JFileChooser fc = new JFileChooser();
		int returnVal = fc.showOpenDialog(null);
		byte[] byteData = null;

		if (returnVal == JFileChooser.APPROVE_OPTION) {
			File file = fc.getSelectedFile();
			try {
				InputStream is = new FileInputStream(file);
				long length = file.length();
				byteData = new byte[(int) length];
				int offset = 0;
				int numRead = 0;
				while (offset < byteData.length && (numRead = is.read(byteData, offset, byteData.length - offset)) >= 0) {
					offset += numRead;
				}
				is.close();
			} catch (FileNotFoundException e) {
				e.printStackTrace();
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		return byteData;
	}

}
