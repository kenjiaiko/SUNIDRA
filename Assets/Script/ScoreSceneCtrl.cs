using UnityEngine;
using System.Collections;

public class ScoreSceneCtrl : MonoBehaviour {

	// タイトル画面テクスチャ
	public Texture2D bgTexture;

	private string score_str = "";
	private string my_score_str = "";
	private string to_message = "Thank you for playing!";
	private string playername = "";

	// Use this for initialization
	void Start () {
		GET ("https://cedec2015.seccon.jp/cedec2015/GameCtrl/");

		int my_score = GameRuleCtrl.get_score ();
		my_score_str = "Your Time: " + my_score.ToString ();

		playername = TitleSceneCtrl.get_playername ();
		if (my_score != -1) {
			playername += GameRuleCtrl.get_gamecookie ();
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
		// スタイルを準備.
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
		
		// 解像度対応
		GUI.matrix = Matrix4x4.TRS(
			Vector3.zero,
			Quaternion.identity,
			new Vector3(Screen.width / 854.0f, Screen.height / 480.0f, 1.0f));
		// タイトル画面テクスチャ表示
		GUI.DrawTexture(new Rect(0.0f, 0.0f, 854.0f, 480.0f), bgTexture);
		
		// スタートボタンを作成します。
		if (GUI.Button(new Rect(477, 280, 200, 54), "Try again", buttonStyle))
		{
			Application.LoadLevel("TitleScene");
		}

		GUIStyle style = new GUIStyle();
		style.fontSize = 24;
		style.normal.textColor = Color.white;
		GUI.Label(new Rect(127, 32, 200, 260), score_str, style);
		GUI.Label(new Rect(477, 100, 200, 30), playername, style);
		GUI.Label(new Rect(477, 150, 200, 30), my_score_str, style);
		GUI.Label(new Rect(477, 200, 200, 30), to_message, style);
	}

	WWW GET(string url)
	{
		WWW www = new WWW (url);
		StartCoroutine (WaitForRequest (www));
		return www;
	}
	
	private IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		if (www.error == null) {
			//Debug.Log("WWW OK: " + www.text);  //www.text
			score_str = www.text;
		} else {
			//Debug.Log("WWW ERR:" + www.error); //www.error
			score_str = "";
		}
	}
}
