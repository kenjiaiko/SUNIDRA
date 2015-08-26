using UnityEngine;
using System.Collections;

[System.Reflection.Obfuscation(Exclude=true, Feature="renaming")]
public class AttackArea : MonoBehaviour {
	CharacterStatus status;
	
	public AudioClip hitSeClip;
	AudioSource hitSeAudio;
	
	void Start()
	{
		status = transform.root.GetComponent<CharacterStatus>();
		
		// オーティオの初期化.
		hitSeAudio = gameObject.AddComponent<AudioSource>();
		hitSeAudio.clip = hitSeClip;
		hitSeAudio.loop = false;
	}
	
	
	public class AttackInfo
	{
		public int attackPower; // この攻撃の攻撃力.
		public Transform attacker; // 攻撃者.
	}
	
	
	// 攻撃情報を取得する.
	AttackInfo GetAttackInfo()
	{			
		AttackInfo attackInfo = new AttackInfo();
		// 攻撃力の計算.
		attackInfo.attackPower = status.Power;
		
		// 攻撃強化中
		if (status.powerBoost)
			attackInfo.attackPower += 7;
		
		attackInfo.attacker = transform.root;
		
		return attackInfo;
	}
	
	// 当たった.
	void OnTriggerEnter(Collider other)
	{
		// 攻撃が当たった相手のDamageメッセージをおくる.
		other.SendMessage("Damage",GetAttackInfo());
		
		// オーディオ再生.
		hitSeAudio.Play();
	}
	
	
	// 攻撃判定を有効にする.
	public void OnAttack()
	{
		GetComponent<Collider>().enabled = true;
	}
	
	
	// 攻撃判定を無効にする.
	public void OnAttackTermination()
	{
		GetComponent<Collider>().enabled = false;
	}
}
