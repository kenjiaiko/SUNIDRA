using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class TitleSceneCtrl : MonoBehaviour {
    
    public Texture2D bgTexture;
	public static string playername = "Player";
	public static string systemcode = "";

	public static string get_playername()
	{
		return playername;
	}

	public static string get_systemcode()
	{
		return systemcode;
	}

	void Start()
	{
		if (Application.platform == RuntimePlatform.Android) {
			System.IO.FileStream fs = new System.IO.FileStream (
				Application.dataPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			byte[] bs = new byte[fs.Length];
			fs.Read (bs, 0, bs.Length);
			fs.Close ();
			SHA1 sha = new SHA1CryptoServiceProvider ();
			byte[] hashBytes = sha.ComputeHash (bs);
			systemcode = System.Convert.ToBase64String (hashBytes);
		}
	}

	void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	void OnGUI()
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.identity,
            new Vector3(Screen.width / 854.0f, Screen.height / 480.0f, 1.0f));

        GUI.DrawTexture(new Rect(0.0f, 0.0f, 854.0f, 480.0f), bgTexture);

        if (GUI.Button(new Rect(327, 290, 200, 54), "Start", buttonStyle))
        {
			if (playername == "") {
				playername = "Player";
			}
			Application.LoadLevel("GameScene");
            
        }

		GUIStyle style = new GUIStyle();
		style.fontSize = 16;
		style.normal.textColor = Color.gray;
		GUI.Label(new Rect(327, 190, 200, 30), "Name", style);

		playername = GUI.TextField(new Rect(327, 210, 200, 30), playername, 12);
		if (playername == "Dragon" || playername == "Warg")
			playername = "Player";
		playername = chk_name (playername);
	}

	private string chk_name(string name)
	{
		string ok_str = "";
		ok_str += "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		ok_str += "0123456789" + "_";
		string r = "";
		for (int i=0; i < name.Length; i++) {
			if (ok_str.IndexOf(name[i]) != -1) 
				r += name[i];
		}
		return r;
	}
}
