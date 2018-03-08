using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

public class APIManager: MonoBehaviour {
	public string _host;
	public string _port;

	public delegate void callback(string dataAsJSON);
	public static callback _done;
	public static callback _fail;

	void Start()
	{
		_port = (_port.Length > 0) ? ":" + _port : "";
	}

	public APIManager on(string eventName, callback method)
	{
		switch (eventName)
		{
			case "success":
				_done = method;
			break;

			case "error":
				_fail = method;
            break;
		}

		return this;
	}

	IEnumerator _get(string path)
	{
		string url = _host + _port + path;

		WWW request = new WWW(url);

		yield return request;

		if (request.isDone)
		{
			if (!string.IsNullOrEmpty(request.error))
			{
				_fail("{ error: " + request.error + ", error_description: " + request.text + " }");
			}
			else
			{
				_done(request.text);
			}
		}
	}

	IEnumerator _post (string path, string jsonData)
	{
        string url = _host + _port + path;
		byte[] data = Encoding.ASCII.GetBytes(jsonData);
		Dictionary<string, string> headers = new Dictionary<string, string>();

		headers.Add("Content-Type", "application/json");
		WWW request = new WWW(url, data, headers);

		yield return request;

		if (request.isDone)
		{
			if (!string.IsNullOrEmpty(request.error))
			{
				_fail("{ error: "+ request.error +", error_description: "+ request.text +" }");
			}
			else
			{
				_done(request.text);
			}
		}
	}

	public APIManager post(string path, string jsonData)
	{
		StartCoroutine(_post(path, jsonData));
		return this;
	}

	public APIManager get(string path)
	{
		StartCoroutine(_get(path));
		return this;
	}
}
