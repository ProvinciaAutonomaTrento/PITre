package com.nttdata.scannerApplet.model.utils;

public class SaneConnectionInfo {
	private final String host, username;
	private final int port;

	public SaneConnectionInfo(String host, String username, int port) {
		this.host = host;
		this.username = username;
		this.port = port;
	}

	public String getHost() {
		return this.host;
	}

	public String getUsername() {
		return this.username;
	}

	public int getPort() {
		return this.port;
	}

}
