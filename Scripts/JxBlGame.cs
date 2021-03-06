//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行시版本:4.0.30319.17929
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
// </By JianxgXiBolong>
//------------------------------------------------------------------------------

using UnityEngine;
using System;
using GameDefine;
using BlGame.GameData;
using System.Collections;
using BlGame.GameEntity;
using JT.FWW.Tools;
using System.Collections.Generic;
using BlGame.GuideDate;
using BlGame.GameState;
using BlGame.Network;
using BlGame.Effect;
using BlGame.Resource;
using BlGame.View;
using BlGame;
using BlGame.Ctrl;
using BlGame.Model;

public class JxBlGame : MonoBehaviour {

	public e_BattleState Battle_State {
		private set;
		get;
	}

	public static JxBlGame Instance{
		set;get;
	}


	private bool IsCutLine = false;

    public bool IsInitialize = false;

    public bool IsQuickBattle = false;

    public List<string> ipList = new List<string>();

    //public List<string> ServerIpList = new List<string>();
    //public string LoginServerAdress = "10.30.16.181";
	public string LoginServerAdress = "127.0.0.1";
    public int LoginServerPort = 49996;

    public BlGame.AudioManager AudioPlay
    {
        get;
        private set;
    }

    public bool SkipNewsGuide = false;

	void Awake(){
		if (Instance != null) {
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad (this.gameObject);
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //GameUI/Camera속에 각종 UIclone들을 생성한다.한편 eventlister와handler를 추가한다.
        WindowManager.Instance.ChangeScenseToLogin(EScenesType.EST_None);
    }

	// Use this for initialization
	void Start () {
		new PlayerManager ();
		new NpcManager(); //NonPlayerCaractor=>Npc보트를 의미한다.
        NetworkManager.Instance.Close();
        //LoginState의 Enter()함수호출
        GameStateManager.Instance.EnterDefaultState();

        //初始化逻辑对象
        CGLCtrl_GameLogic logini = CGLCtrl_GameLogic.Instance;

        //预加载，减少进入游戏资源加载卡顿
        ConfigReader.Init();
        GameMethod.FileRead();
        
        //预加载特效信息
        ReadPreLoadConfig.Instance.Init();
        //需要释放的资源信息
        ReadReleaseResourceConfig.Instance.Init();
       
	}

    void OnDestroy()
    {
       
    }

    
  
	// Update is called once per frame
	void Update ()
    {
        //更新buff
        BlGame.Skill.BuffManager.Instance.Update();
		//更新特效
        BlGame.Effect.EffectManager.Instance.UpdateSelf();      
        //更新提示消失
        MsgInfoManager.Instance.Update();
        //场景声音更新
        SceneSoundManager.Instance.Update();
        //声音更新
        BlGame.AudioManager.Instance.OnUpdate();
        //更新游戏状态机
        GameStateManager.Instance.Update(Time.deltaTime);
        //更新网络模块
        NetworkManager.Instance.Update(Time.deltaTime);
        //更新界面引导
        IGuideTaskManager.Instance().OnUpdate();
        //小地图更新
        MiniMapManager.Instance.Update();

        //UI更新
        WindowManager.Instance.Update(Time.deltaTime);

        //特效后删除机制 
        BlGame.Effect.EffectManager.Instance.HandleDelete();

        //GameObjectPool更新
        GameObjectPool.Instance.OnUpdate();

        //游戏시间设置
        GameTimeData.Instance.OnUpdate();
	}

 
	void OnEnable()
	{
        //event
        EventCenter.AddListener(EGameEvent.eGameEvent_ConnectServerSuccess, GameConnectServerSuccess);
        EventCenter.AddListener(EGameEvent.eGameEvent_ConnectServerFail, OpenConnectUI);
        EventCenter.AddListener(EGameEvent.eGameEvent_ReconnectToBatttle, OpenConnectUI);
        EventCenter.AddListener(EGameEvent.eGameEvent_BeginWaiting, OpenWaitingUI);   

        if (PlayerPrefs.HasKey(UIGameSetting.voiceKey))
        {
            int vKey = PlayerPrefs.GetInt(UIGameSetting.voiceKey);
            bool state = (vKey == 1) ? true : false;
            AudioManager.Instance.EnableVoice(state);
        }
        if (PlayerPrefs.HasKey(UIGameSetting.soundKey)) {
            int sKey = PlayerPrefs.GetInt(UIGameSetting.soundKey);
            bool state = (sKey == 1) ? true : false;
            AudioManager.Instance.EnableSound(state);
        }       
	}

	void OnDisable()
	{
        EventCenter.RemoveListener(EGameEvent.eGameEvent_ConnectServerSuccess, GameConnectServerSuccess);
        EventCenter.RemoveListener(EGameEvent.eGameEvent_ConnectServerFail, OpenConnectUI);
        EventCenter.RemoveListener(EGameEvent.eGameEvent_ReconnectToBatttle, OpenConnectUI);
        EventCenter.RemoveListener(EGameEvent.eGameEvent_BeginWaiting, OpenWaitingUI);   
	}

    //游戏退出前执行（玩家强行关闭游戏）
    void OnApplicationQuit()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || SKIP_SDK
#else
         SdkConector.Quit();
#endif

		Debug.Log("게임완료");
        /*
	//	PlatformManager.GetSingleton ().OnAction (EActionType.eA_Logout,null,null,null);
        #region  talkingdata
        CEvent eve = new CEvent(EGameEvent.eGameEvent_TalkgameAction);
        eve.AddParam("type", EActionType.eA_Logout); 
        EventCenter.SendEvent(eve);
        #endregion 
        */

        NetworkManager.Instance.Close();
    }

    public void OpenConnectUI()
    {
        PlayerManager.Instance.CleanPlayerWhenGameOver();
        EntityManager.Instance.DestoryAllEntity();
        EffectManager.Instance.DestroyAllEffect();
        JxBlGame.Instance.IsInitialize = true;
	}

    private void OpenWaitingUI()
    {
        if (WaitingInterface.Instance == null)
        {
            BlGameUI.Instance.OnOpenUIPathCamera(GameDefine.GameConstDefine.WaitingUI);
        }
    }

    /// <summary>
    /// 봉사기와의 접속이 성공하면 SceneServer에게 Ping을 날린다.
    /// </summary>
    private void GameConnectServerSuccess()
    {
        StopCoroutine("PingToServer");

        StartCoroutine("PingToServer");
    }

    private IEnumerator PingToServer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            CGLCtrl_GameLogic.Instance.EmsgToss_AskPing();
        }
    }

    public void PlayEnd()
    {
        EntityManager.AllEntitys.Clear();
        if (PlayerManager.Instance.LocalPlayer != null)
        {
            PlayerManager.Instance.AccountDic.Clear();
            PlayerManager.Instance.LocalPlayer.AbsorbMonsterType = null;
        }
        
        BlGame.AudioManager.Instance.StopHeroAudio();
    }

	public  void PlayStart()
    {
        Int32 state = 0;
        if (PlayerManager.Instance.LocalAccount.ObType == ObPlayerOrPlayer.PlayerObType)
        {
            state = 1;
        }
        GameMapObjs GameBuilding = GameObject.FindObjectOfType(typeof(GameMapObjs)) as GameMapObjs;
        EntityManager.ClearHomeBase();
        if (GameBuilding != null)
        {
            for (int id = 0; id < GameBuilding.transform.childCount; id++)
            {
                Transform child = GameBuilding.transform.GetChild(id);
                int objId = 0;
                try
                {
                    objId = Convert.ToInt32(child.name);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    continue;
                }

                int infoId = GetMapObjIndex(objId);
                
                if (ConfigReader.MapObjXmlInfoDict.ContainsKey(infoId))
                {
                    MapObjConfigInfo configInfp = ConfigReader.MapObjXmlInfoDict[infoId];
                    int type = configInfp.eObjectTypeID;
                    int index = configInfp.un32ObjIdx;
                    int camp = configInfp.n32Camp;
                    UInt64 sGUID = (UInt64)index;
                    EntityManager.HandleDelectEntity(sGUID);
                    Ientity item = NpcManager.Instance.HandleCreateEntity(sGUID, (EntityCampType)camp);
                    item.MapObgId = objId;
                    item.realObject = child.gameObject;
                    item.objTransform = child.gameObject.transform;
                    item.GameObjGUID = sGUID;
                    item.NpcGUIDType = type;
                    item.ObjTypeID = (uint)type;
                    item.entityType = (EntityType)ConfigReader.GetNpcInfo(type).NpcType;
                    item.SetHp(1);
                    item.SetHpMax(1);
                    EntityManager.Instance.SetCommonProperty(item, type);
                    item.RealEntity = EntityManager.AddBuildEntityComponent(item);
                    NpcManager.Instance.AddEntity(sGUID, item);
                    EntityManager.AddHomeBase(item);
                    GuideBuildingTips.Instance.AddBuildingTips(item);
                }
            }
        } 
        LoadBaseDate.Instance().LoadBase();
	}

    private int GetMapObjIndex(int objId){
        foreach (var item in ConfigReader.MapObjXmlInfoDict.Values) {
            if (item.un32ObjIdx == objId && (int)GameUserModel.Instance.GameMapID == item.un32MapID)
            {
                return item.un32Id;
            }
        }
        return -1;
    }

//###################################################游戏状态初始化###################################################################
//###################################################游戏状态初始化###################################################################
//###################################################游戏状态初始化###################################################################

}
