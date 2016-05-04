using UnityEngine;
using System.Collections;

public class Iai : Magic_Parameter {

	public GameObject Bullet;//矢のObject
	private Z_Camera z_camera;

	private Vector3 Iti;
	private Vector3 StartPos;

	// Use this for initialization
	void Start () {
	
		z_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Z_Camera>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Fire()
	{
		Parent.GetComponent<Character_Manager>().SetKeylock();
		GameObject bullet;
		
		bullet = GameObject.Instantiate(Bullet);
		bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

		StartPos = Parent.transform.position;//ここで元の位置を記録しておく
		
		Parent.GetComponent<Player_ControllerZ> ().SetMP (Parent.GetComponent<Player_ControllerZ> ().GetMP() - GetSMP());

		if(Parent.GetComponent<Player_ControllerZ>().GetF_Watch())
		{
			//これで敵の目の前に行ける
			Iti = new Vector3(z_camera.Target.transform.position.x - Parent.transform.position.x - Parent.GetComponent<Player_ControllerZ>().GetDirection().x * z_camera.Target.transform.localScale.x,
			                                        z_camera.Target.transform.position.y - (Parent.transform.position.y + 0.8f) - Parent.GetComponent<Player_ControllerZ>().GetDirection().y * z_camera.Target.transform.localScale.y,
			                                        z_camera.Target.transform.position.z - Parent.transform.position.z - Parent.GetComponent<Player_ControllerZ>().GetDirection().z * z_camera.Target.transform.localScale.z);

			Parent.transform.position += Iti;
		}
		
		//弾を飛ばす処理
		bullet.transform.position = transform.position + Parent.GetComponent<Character_Parameters>().direction;
		/*if(!audioSource.isPlaying){
			
            audioSource.Play();
			
        }*/

		//硬直
		Invoke("Active", Bullet.GetComponent<Attack_Parameter>().rigid_time);
		
		Destroy(bullet, Bullet.GetComponent<Attack_Parameter>().attack_time);
		
	}
	
	void Active()
	{
		Parent.GetComponent<Character_Manager>().SetActive();
		//if (Parent.GetComponent<Player_ControllerZ> ().GetF_Watch ()) {
			//後ろに抜けるやつ
		//Parent.transform.position += Iti;
		//}
		Parent.transform.position = StartPos;//硬直後戻る
	}
}
