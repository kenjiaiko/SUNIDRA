using UnityEngine;
using System.Collections;

[System.Reflection.Obfuscation(Exclude=true, Feature="renaming")]
public class EnemyGeneratorCtrl : MonoBehaviour {
	// 生まれてくる敵プレハブ
	public GameObject enemyPrefab;
	// 敵を格納
	public GameObject[] existEnemys;
	// アクティブの最大数
	public int maxEnemy = 2;

	void Start()
	{
		// 配列確保
		existEnemys = new GameObject[maxEnemy];
		// 周期的に実行したい場合はコルーチンを使うと簡単に実装できます。
		StartCoroutine(Exec());
	}

	// 敵を作成します
	IEnumerator Exec()
	{
		while(true){ 
			Generate();
			yield return new WaitForSeconds( 3.0f );
		}
	}

	void Generate()
	{
		for(int enemyCount = 0; enemyCount < existEnemys.Length; ++ enemyCount)
		{
			if( existEnemys[enemyCount] == null ){
				// 敵作成
				existEnemys[enemyCount] = Instantiate(enemyPrefab,transform.position,transform.rotation) as GameObject;
				return;
			}
		}
	}
	
}
