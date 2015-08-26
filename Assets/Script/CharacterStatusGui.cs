using UnityEngine;
using System.Collections;

public class CharacterStatusGui : MonoBehaviour
{
    //float baseWidth = 854f;
    //float baseHeight = 480f;

    // ステータス.
    CharacterStatus playerStatus;
    //Vector2 playerStatusOffset = new Vector2(8f, 8f);

    // 名前.
    Rect nameRect = new Rect(0f, 0f, 120f, 15f);
    public GUIStyle nameLabelStyle;

    // ライフバー.
    public Texture backLifeBarTexture;
    public Texture frontLifeBarTexture;
    float frontLifeBarOffsetX = 2f;
    float lifeBarTextureWidth = 128f;
    Rect playerLifeBarRect = new Rect(0f, 0f, 128f, 10f);
    Color playerFrontLifeBarColor = Color.green;
    Rect enemyLifeBarRect = new Rect(0f, 0f, 128f, 10f);
    Color enemyFrontLifeBarColor = Color.red;

	// get camera component
	Camera ca;

	void Start(){
		GameObject c = GameObject.Find("Main Camera");
		ca = c.GetComponent<Camera> ();
	}

    // プレイヤーステータスの描画.
    void DrawPlayerStatus()
	{
		DrawCharacterStatus(playerStatus, playerLifeBarRect, playerFrontLifeBarColor);
    }

    // 敵ステータスの描画.
    void DrawEnemyStatus()
    {
		if (playerStatus.lastAttackTarget != null)
        {
			CharacterStatus target_status = playerStatus.lastAttackTarget.GetComponent<CharacterStatus>();
			DrawCharacterStatus(target_status, enemyLifeBarRect, enemyFrontLifeBarColor);
        }
    }

    // キャラクターステータスの描画.
    void DrawCharacterStatus(CharacterStatus status, Rect bar_rect, Color front_color)
    {
		Vector3 a = status.transform.position;
		a.y += 2;
		Vector3 pos = ca.WorldToScreenPoint (a);

		float x = pos.x;
		float y = ca.pixelHeight - pos.y;

		if (status.characterName == TitleSceneCtrl.get_playername())
			x -= 140;
		else
			x += 20;

        // 名前.
        GUI.Label(
            new Rect(x, y, nameRect.width, nameRect.height),
			status.characterName,
            nameLabelStyle);

		float life_value = (float)status.HP / status.MaxHP;
		if(backLifeBarTexture != null)
		{
			// 背面ライフバー.
			y += nameRect.height;
			GUI.DrawTexture(new Rect(x, y, bar_rect.width, bar_rect.height), backLifeBarTexture);
		}

        // 前面ライフバー.
		if(frontLifeBarTexture != null)
		{
			float resize_front_bar_offset_x = frontLifeBarOffsetX * bar_rect.width / lifeBarTextureWidth;
			float front_bar_width = bar_rect.width - resize_front_bar_offset_x * 2;
			var gui_color = GUI.color;
			GUI.color = front_color;
			GUI.DrawTexture(new Rect(x + resize_front_bar_offset_x, y, front_bar_width * life_value, bar_rect.height), frontLifeBarTexture);
			GUI.color = gui_color;
		}
    }

    void Awake()
    {
        PlayerCtrl player_ctrl = GameObject.FindObjectOfType(typeof(PlayerCtrl)) as PlayerCtrl;
        playerStatus = player_ctrl.GetComponent<CharacterStatus>();
    }

    void OnGUI()
    {
		/*
        // 解像度対応.
        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.identity,
            new Vector3(Screen.width / baseWidth, Screen.height / baseHeight, 1f));
		*/
        // ステータス.
        DrawPlayerStatus();
        DrawEnemyStatus();
    }
}