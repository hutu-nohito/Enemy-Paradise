﻿using UnityEngine;
using System.Collections;

public class ControllerExample : MonoBehaviour {

    public Magic_ControllerVR Player_Magic;
    public bool right = false;//右のコントローラ

    public SaveRune save;
    public bool flag_save = false;

    //とりあえずメッセージ送りをできるようにしとく
    private GameObject GM;

    void Start()
    {
        
        if (GM == null)
        {
            GM = GameObject.FindGameObjectWithTag("Manager");//マネージャは絶対あるからこれでいいや
        }
    }

    void Update()
    {
        SteamVR_TrackedObject trackedObject = GetComponent<SteamVR_TrackedObject>();
        var device = SteamVR_Controller.Input((int)trackedObject.index);

        //組み込み済み////////////////////////////////////////////////////////////////////////////

        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (Player_Magic != null)
            {
                //あったら送る
                Player_Magic.ControllerPulse(Magic_ControllerVR.VRButton.TriggerTouchDown, right);
            }
            
            //Debug.Log("トリガーを浅く引いた");

        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (flag_save)
            {
                save.ControllerPulse(SaveRune.VRButton.TriggerPressDown, right);
            }
            else
            {
                if (Player_Magic != null)
                {
                    Player_Magic.ControllerPulse(Magic_ControllerVR.VRButton.TriggerPressDown, right);
                }
                    
            }

            //Debug.Log("トリガーを深く引いた");

            GM.GetComponentInChildren<uGUI_Msg>().Viveclick();//メッセージ送り
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (flag_save)
            {
                save.ControllerPulse(SaveRune.VRButton.TriggerUp, right);
            }
            else
            {
                if (Player_Magic != null)
                {
                    Player_Magic.ControllerPulse(Magic_ControllerVR.VRButton.TriggerUp, right);
                }
            }
            
            Debug.Log("トリガーを離した");
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            if (flag_save)
            {
                save.ControllerPulse(SaveRune.VRButton.GripDown, right);
            }
            else
            {
                if (Player_Magic != null)
                {
                    Player_Magic.ControllerPulse(Magic_ControllerVR.VRButton.GripDown, right);
                }
            }
            
            Debug.Log("グリップボタンをクリックした");
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            if (Player_Magic != null)
            {
                Player_Magic.ControllerPulse(Magic_ControllerVR.VRButton.GripUp, right);
            }
            Debug.Log("グリップボタンを離した");
        }

        //押しっぱ系
        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (Player_Magic != null)
            {
                Player_Magic.ControllerTrigger(right);
            }
            //Debug.Log("トリガーを深く引いている");
        }

        //まだ////////////////////////////////////////////////////////////////////////////

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("タッチパッドをクリックした");
        }
        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("タッチパッドをクリックしている");
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("タッチパッドをクリックして離した");
        }
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("タッチパッドに触った");
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("タッチパッドを離した");
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            Debug.Log("メニューボタンをクリックした");
        }
        
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            //Debug.Log("トリガーを浅く引いている");
        }
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //Debug.Log("タッチパッドに触っている");
        }
    }
}
