using UnityEngine;
using System.Collections;

public class Iceball2 : Magic_Parameter {

    private Magic_Controller MC;
    private Player_ControllerZ pcZ;

    private Animator animator;//アニメ
    private AudioSource SE;//音

    public GameObject Bullet;//雪玉のObject

    private GameObject bullet;
    private GameObject Player;
    private Vector3 oldPos;//プレイヤーの動きをとる
    private float oldSpeed = 0;//元のスピードを保持しとく
    private float oldRotSpeed = 0;//元の回転速度

    private bool isIceball = false;//弾を出しているかどうか

    // Use this for initialization
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        MC = Player.GetComponent<Magic_Controller>();
        pcZ = Player.GetComponent<Player_ControllerZ>();

        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
        SE = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Hold()
    {
        //ﾌﾟﾚｲﾔのスピードをゆっくりに
        if (!isIceball)
        {
            oldSpeed = pcZ.GetSpeed();
            //pcZ.SetSpeed(oldSpeed / 2);
            pcZ.Dash = false;
            pcZ.speed = oldSpeed / 2;
            oldRotSpeed = pcZ.RotSpeed;
            pcZ.RotSpeed = pcZ.RotSpeed / 10;

        }

        //まず雪玉を出します
        if (!isIceball)
        {
            bullet = GameObject.Instantiate(Bullet);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.LookRotation(Parent.transform.TransformDirection(Vector3.forward));//回転させて矢じりを進行方向に向ける

            bullet.GetComponent<Attack_Parameter>().Parent = this.Parent;//もらった親を渡しておく必要がある

            //物理挙動をなくしとく
            bullet.GetComponent<Rigidbody>().useGravity = false;
            bullet.GetComponent<SphereCollider>().isTrigger = true;

            pcZ.SetMP(pcZ.GetMP() - GetSMP());

            isIceball = true;

            //雪玉がついてくるように さすがに親子にしたほうが早い
            //bullet.transform.position += Player.transform.position - oldPos;
            bullet.transform.parent = Player.transform;

            oldPos = Player.transform.position;
        }

        if (bullet == null)
        {

            //ﾌﾟﾚｲﾔのスピードを戻す
            pcZ.RotSpeed = oldRotSpeed;
            pcZ.SetSpeed(oldSpeed);
            pcZ.Dash = true;
            return;//雪玉が壊れてたら処理しない
        }

        //雪玉を大きく
        if (oldPos != Player.transform.position)
        {
            if (bullet.transform.localScale.y <= 5)
            {//一応制限しとく
                bullet.transform.localScale += new Vector3(0.005f, 0.005f, 0.005f);
                bullet.transform.localPosition += new Vector3(0, 0.001f, 0);
            }

            //転がってるように見せる　回転
            bullet.transform.Rotate(1, 0, 0);
        }

        //大きさで強さを変化
        bullet.GetComponent<Attack_Parameter>().power = (int)bullet.transform.localScale.y * 2;
        Debug.Log(bullet.GetComponent<Attack_Parameter>().power);

        oldPos = Player.transform.position;

    }

    void Fire()
    {
        //ﾌﾟﾚｲﾔのスピードを戻す
        pcZ.RotSpeed = oldRotSpeed;
        pcZ.SetSpeed(oldSpeed);
        pcZ.Dash = true;

        //弾が残ってたら物理挙動を付ける
        if (bullet != null)
        {
            bullet.GetComponent<SphereCollider>().isTrigger = false;
            bullet.GetComponent<Rigidbody>().useGravity = true;
        }

        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        //雪玉が壊れてたら処理を抜ける
        if (bullet == null)
        {
            yield return new WaitForSeconds(0.2f);//ちょっと待つ
            isIceball = false;
            yield break;
        }

        Parent.GetComponent<Character_Manager>().SetKeylock();
        GameObject bullet_Shot;
        bullet.transform.parent = null;
        bullet_Shot = bullet;
        bullet = null;
        isIceball = false;

        MC.AddExistBullet(bullet_Shot);//現在の弾数を増やす

        animator.SetTrigger("Shoot");

        //弾を飛ばす処理
        bullet_Shot.GetComponent<Rigidbody>().velocity = (Parent.transform.TransformDirection(Vector3.forward) * Bullet.GetComponent<Attack_Parameter>().speed);

        //効果音と演出
        if (!SE.isPlaying)
        {

            SE.PlayOneShot(SE.clip);//SE

        }

        Destroy(bullet_Shot, bullet_Shot.GetComponent<Attack_Parameter>().GetA_Time());

        yield return new WaitForSeconds(bullet_Shot.GetComponent<Attack_Parameter>().GetR_Time());//撃った後の硬直

        //硬直を解除
        Parent.GetComponent<Character_Manager>().SetActive();
    }

}
