package com.nttdata.scannerApplet.model.utils;

public class OsUtils {

	public static boolean isWindowsOs() {
		String os = System.getProperty("os.name");
		return os.startsWith("Windows");
	}

	public static boolean isMacOs() {
		String os = System.getProperty("os.name");
		return os.toLowerCase().contains("mac");
	}

	public static boolean isLinuxOs() {
		String os = System.getProperty("os.name");
		return os.toLowerCase().contains("linux");
	}

}
