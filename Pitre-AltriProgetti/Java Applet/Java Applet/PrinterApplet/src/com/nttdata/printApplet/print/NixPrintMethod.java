package com.nttdata.printApplet.print;

import java.io.BufferedReader;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
//import java.util.Arrays;
import java.util.List;

import javax.swing.JList;
import javax.swing.JOptionPane;
import javax.swing.JScrollPane;
import javax.swing.ListSelectionModel;

public class NixPrintMethod implements AppPrinterInterface {

	public void acceptJob(byte[] data) {
		String path = System.getProperty("user.home"); //$NON-NLS-1$
		path += "/.printdata"; //$NON-NLS-1$
		String printerName = this.getPrinterName();
		if (printerName == null) return;
		FileOutputStream bos;
		try {
			bos = new FileOutputStream(path);
			bos.write(data);
			bos.flush();
			bos.close();
			String s = null;
			String command;
			command = "lpr -l -P " + printerName + " " + path; //$NON-NLS-1$ //$NON-NLS-2$

			Process p = Runtime.getRuntime().exec(command);
			BufferedReader stdInput = new BufferedReader(new InputStreamReader(p.getInputStream()));

			BufferedReader stdError = new BufferedReader(new InputStreamReader(p.getErrorStream()));

			// read the output from the command
			System.out.println("Here is the standard output of the command:\n"); //$NON-NLS-1$
			while ((s = stdInput.readLine()) != null) {
				System.out.println(s);
			}

			// read any errors from the attempted command
			System.out.println("Here is the standard error of the command (if any):\n"); //$NON-NLS-1$
			while ((s = stdError.readLine()) != null) {
				System.out.println(s);
			}
			System.out.println(path);
		} catch (IOException e) {
			e.printStackTrace();
		}

	}

	private String getPrinterName() {
		String printerName = null;
		List<String> printers = this.getPrinterList();
		printerName = this.chooseFromList(printers);

		return printerName;
	}

	private List<String> getPrinterList() {
		Process p;
		List<String> printers = null;
		try {
			p = Runtime.getRuntime().exec("lpstat -a"); //$NON-NLS-1$
			BufferedReader stdInput = new BufferedReader(new InputStreamReader(p.getInputStream()));

			// read the output from the command
			System.out.println("Here is the standard output of the command:\n"); //$NON-NLS-1$
			String s;
			printers = new ArrayList<String>();
			while ((s = stdInput.readLine()) != null) {
				// System.out.println(s);
				printers.add(s.split(" ")[0]); //$NON-NLS-1$
			}
			System.out.println(printers);
		} catch (IOException e) {
			e.printStackTrace();
		}
		return printers;
	}

	private String chooseFromList(List<String> printerList) {
		String[] options = { Messages.getString("NixMessage.9"), Messages.getString("NixMessage.10") }; //$NON-NLS-1$ //$NON-NLS-2$
		if (printerList == null) return null;
		String printerName = null;
		String[] choices = null;
		System.arraycopy(printerList.toArray(),0,choices,0,printerList.size());
		//_ 1.6 _  String[] choices = Arrays.copyOf(printerList.toArray(), printerList.size(), String[].class);
		JList list = new JList(choices);
		list.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
		list.setLayoutOrientation(JList.VERTICAL_WRAP);
		JScrollPane scrollPane = new JScrollPane(list);
		int res = JOptionPane.showOptionDialog(null, scrollPane,
				Messages.getString("NixMessage.11"), JOptionPane.OK_CANCEL_OPTION, //$NON-NLS-1$
				JOptionPane.QUESTION_MESSAGE, null, options, options[0]);
		if (res == JOptionPane.OK_OPTION) {
			printerName = (String) list.getSelectedValue();
		}
		return printerName;
	}
}
