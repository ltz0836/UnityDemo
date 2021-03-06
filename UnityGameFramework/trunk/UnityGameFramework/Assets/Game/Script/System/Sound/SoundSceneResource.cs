﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

public class SoundSceneResource : MonoBehaviour {

    public bool adjustPanel = false;
    //bool hideMenu = false;

    [Tooltip("Filename was default name, you can setting alias name, if you want.")]
    public List<soundAudioInfo> soundAudioInfos = new List<soundAudioInfo>();
    
    //! 從Resource Load來的聲音source.
    public List<soundAudioLoadFromResourceInfo> loadFromResourcedAudioInfos = new List<soundAudioLoadFromResourceInfo>();

    // audio mixer
    public AudioMixer gameAudioMixer;



    /// <summary>
    /// 
    /// </summary>
    void Start ()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        LoadAudioClipFromResource();

        SoundMgr.Singleton.InitAudioMixer(gameAudioMixer);
        SoundMgr.Singleton.RegisterAudioSource(soundAudioInfos, this);
    }
    /// <summary>
    /// 
    /// </summary>
    void Update ()
    {
	    
	}

    /// <summary>
    /// Load跨場景BGM
    /// 目前assetbundle有一個bug，audioclip如果放在場景中，有被don't destroy on load
    /// 的物件參考到，在第2次進都同樣場景時，場景結束會把audioclip實體刪除，導致error，
    /// 所以跨場景BGM改用resource load.
    /// </summary>
    void LoadAudioClipFromResource()
    {

        for (int i = 0; i < loadFromResourcedAudioInfos.Count; i++)
        {
            string audioName = loadFromResourcedAudioInfos[i].name;
            string groupName = loadFromResourcedAudioInfos[i].group;

            if (SoundMgr.Singleton.IsBgmExist(audioName))
            {
                continue;
            }

            AudioClip audioClip = Resources.Load<AudioClip>(loadFromResourcedAudioInfos[i].resourcePath);

            if (audioClip != null)
            {
                soundAudioInfo audioInfo = new soundAudioInfo();
                audioInfo.name = audioName;
                audioInfo.group = groupName;
                audioInfo.audioClip = audioClip;

                soundAudioInfos.Add(audioInfo);
            }
        }
    }

        //void OnGUI()
        //{
        //    if (adjustPanel == false)
        //    {
        //        return;
        //    }

        //    if (GUI.Button(new Rect((Screen.width - 100), 50, 100, 33), "Menu"))
        //    {
        //        hideMenu = !hideMenu;
        //    }

        //    if (hideMenu == true)
        //    {
        //        return;
        //    }

        //    if (GUI.Button(new Rect((Screen.width / 2), 50, 100, 33), "All Stop"))
        //    {

        //        for (int i = 0; i < soundAudioInfos.Count; i++)
        //        //foreach (KeyValuePair<string, soundAudioInfo> kvp in audioDic)
        //        {
        //            if (soundAudioInfos[i].audioClip != null)
        //            {

        //                if (soundAudioInfos[i].group == "BGM")
        //                {
        //                    SoundMgr.Singleton.StopBGM(soundAudioInfos[i].name);
        //                }
        //                else 
        //                {
        //                    SoundMgr.Singleton.StopSound(soundAudioInfos[i].name);
        //                }
        //            }
        //        }


        //    }

        //    int posY = Screen.height / 4;
        //    int posX = 10;
        //    int idx = 0;
        //    int width = 100;

        //    for (int i = 0; i < soundAudioInfos.Count;i++)
        //    //foreach (KeyValuePair<string, soundAudioInfo> kvp in audioDic)
        //    {
        //        if ((posX + width) > Screen.width)
        //        {
        //            posY += 45;
        //            posX = 10;
        //        }

        //        if (GUI.Button(new Rect(posX, posY, width, 33), soundAudioInfos[i].name))
        //        {

        //            if (soundAudioInfos[i].audioClip != null)
        //            {

        //                if (soundAudioInfos[i].group == "BGM")
        //                {
        //                    SoundMgr.Singleton.PlayBGM(soundAudioInfos[i].name);
        //                }
        //                else 
        //                {
        //                    SoundMgr.Singleton.PlaySound(soundAudioInfos[i].name);
        //                }
        //            }


        //        }
        //        posX += width;
        //        idx++;
        //    }

        //}

        /// <summary>
        /// deconstruction.
        /// </summary>
        void OnDestroy()
    {
        // 場景A非同步場景B, A可能在B awake 後才 ondestory, soundMgr 
        // SoundMgr.Singleton.UnRegisterAudioSource();

        if (SoundMgr.instance != null)
        {
            //! 清除這個場景註冊的Sound reference.
            for (int i = 0; i < soundAudioInfos.Count; i++)
            {
                SoundMgr.Singleton.RemoveAudioInfoByName(soundAudioInfos[i].name);
            }

            for (int i = 0; i < loadFromResourcedAudioInfos.Count; i++)
            {
                SoundMgr.Singleton.RemoveAudioInfoByName(loadFromResourcedAudioInfos[i].name);
            }

            //! 清除音效.
            SoundMgr.Singleton.ClearSound();
            SoundMgr.Singleton.UnRegisterSoundSceneResource();
        }
    }
}


[Serializable]
public class soundAudioInfo
{
    public string name;
    public AudioClip audioClip;
    public string group;

    [NonSerialized]
    public bool fadeCheck;
    [NonSerialized]
    public bool inOrOut;
    [NonSerialized]
    public float fadeTime;
    [NonSerialized]
    public float initailTime; 
    [NonSerialized]
    public float audioVolume;

}

[Serializable]
public class soundAudioLoadFromResourceInfo
{
    public string name;
    public string resourcePath;
    public string group;
}