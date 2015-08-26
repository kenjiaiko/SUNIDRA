using UnityEngine;
using System.Collections;

[System.Reflection.Obfuscation(Exclude=true, Feature="renaming")]
public class AttackAreaActivator : MonoBehaviour {
	Collider[] attackAreaColliders; // 攻撃判定コライダの配列.
	
	public AudioClip attackSeClip;
	AudioSource attackSeAudio;
	
	void Start () {
		// 子供のGameObjectからAttackAreaスクリプトがついているGameObjectを探す。
		AttackArea[] attackAreas = GetComponentsInChildren<AttackArea>();
		attackAreaColliders = new Collider[attackAreas.Length];
		
		for (int attackAreaCnt = 0; attackAreaCnt < attackAreas.Length; attackAreaCnt++) {
			// AttackAreaスクリプトがついているGameObjectのコライダを配列に格納する.
			attackAreaColliders[attackAreaCnt] = attackAreas[attackAreaCnt].GetComponent<Collider>();
			attackAreaColliders[attackAreaCnt].enabled = false;  // 初期はfalseにしておく.
		}
		
		// オーディオの初期化.
		attackSeAudio = gameObject.AddComponent<AudioSource>();
		attackSeAudio.clip = attackSeClip;
		attackSeAudio.loop = false;
	}
	
	// アニメーションイベントのStartAttackHitを受け取ってコライダを有効にする
	public void StartAttackHit()
	{
		foreach (Collider attackAreaCollider in attackAreaColliders)
			attackAreaCollider.enabled = true;
		
		// オーディオ再生.
		attackSeAudio.Play();
	}

	// アニメーションイベントのEndAttackHitを受け取ってコライダを無効にする
	public void EndAttackHit()
	{
		foreach (Collider attackAreaCollider in attackAreaColliders)
			attackAreaCollider.enabled = false;
	}
}
