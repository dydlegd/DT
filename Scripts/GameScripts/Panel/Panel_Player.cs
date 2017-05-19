using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD{
public class Panel_Player : UIPanelBase {

    [HideInInspector]
    public SEAT_DIR dir { get { return GameGD.GetSeatDir(data.playerId); } }
    public override int Depth
    {
        get
        {
            return base.Depth;
        }
        set
        {
            base.Depth = value;
            GameUtility.FindDeepChild(gameObject, "Player").GetComponent<UIPanel>().depth = Depth + 5;
        }
    }

    public Transform MJ_CPG_START_TRANS;
    public Transform MJ_IN_START_TRANS;
    public Transform MJ_OUT_START_TRANS;
    

    protected List<MJ> MJ_IN_List = new List<MJ>();//手中的麻将
    protected List<MJ> MJ_OUT_List = new List<MJ>();//打出去的麻将
    protected List<MJ> MJ_CPG_List = new List<MJ>();//吃碰杠的麻将
    protected PlayerData data;
    public MJ LastOutMJ { get { if (MJ_OUT_List.Count > 0)return MJ_OUT_List[MJ_OUT_List.Count - 1]; return null; } }

    protected MJ_Manager _MJMgr;

    protected MJ curSelectMJ;

    private GameObject InfoObj;
    private GameObject Anim_胡_Obj;
    private GameObject Anim_吃_Obj;
    private GameObject Anim_碰_Obj;
    private GameObject Anim_杠_Obj;
    private GameObject Anim_暗杠_Obj;
    private GameObject Anim_自摸_Obj;
    private GameObject ChatMsg_Dialogue_Obj;//对话聊天
    private GameObject ChatMsg_Face_Obj;//表情聊天
    private GameObject ZhuangObj;

    private UISprite headIconSprite;
    private UILabel label_Name;
    private UILabel label_Coin;
    private UILabel label_Geolocation;

    private Player_中鱼 _player_中鱼;
    
    public override void Init(PanelType type)
    {
        if (MJ_CPG_START_TRANS) MJ_CPG_START_TRANS.gameObject.SetActive(false);
        if (MJ_IN_START_TRANS) MJ_IN_START_TRANS.gameObject.SetActive(false);
        if (MJ_OUT_START_TRANS) MJ_OUT_START_TRANS.gameObject.SetActive(false);
        base.Init(type);
        ShowPanelDirectly();

        _MJMgr = GetComponentInChildren<MJ_Manager>();
        _MJMgr.Init();

        EventInit();
        AnimsInit();
        PlayerInfoInit();

        ChatMsg_Dialogue_Obj = GameUtility.FindDeepChild(gameObject, "ChatMsg_Dialogue").gameObject;
        ChatMsg_Face_Obj = GameUtility.FindDeepChild(gameObject, "ChatMsg_Face").gameObject;
        ChatMsg_Dialogue_Obj.GetComponent<UISprite>().alpha = 0;
        ChatMsg_Face_Obj.GetComponent<UISprite>().alpha = 0;

        ZhuangObj = GameUtility.FindDeepChild(gameObject, "Zhuang").gameObject;
        ShowZhuang(false);

        headIconSprite = GameUtility.FindDeepChild(gameObject, "HeadIcon").GetComponent<UISprite>();
        label_Name = GameUtility.FindDeepChild(gameObject, "Name").GetComponent<UILabel>();
        label_Coin = GameUtility.FindDeepChild(gameObject, "Score").GetComponent<UILabel>();
        label_Geolocation = GameUtility.FindDeepChild(gameObject, "Geolocation").GetComponent<UILabel>();

        _player_中鱼 = GetComponentInChildren<Player_中鱼>();
        _player_中鱼.Init(_MJMgr);

    }

    public virtual void ReInit()
    {
        _player_中鱼.ReInit();
        DespawnAllMJ();
    }

    //吃碰杠胡等的动画初始化
    private void AnimsInit()
    {
        Anim_胡_Obj = GameUtility.FindDeepChild(gameObject, "Anim_胡").gameObject;
        Anim_胡_Obj.SetActive(false);
        Anim_胡_Obj.GetComponent<UISpriteAnimation>().isHideComplete = true;
        Anim_吃_Obj = GameUtility.FindDeepChild(gameObject, "Anim_吃").gameObject;
        Anim_吃_Obj.SetActive(false);
        Anim_吃_Obj.GetComponent<UISpriteAnimation>().isHideComplete = true;
        Anim_碰_Obj = GameUtility.FindDeepChild(gameObject, "Anim_碰").gameObject;
        Anim_碰_Obj.SetActive(false);
        Anim_碰_Obj.GetComponent<UISpriteAnimation>().isHideComplete = true;
        Anim_杠_Obj = GameUtility.FindDeepChild(gameObject, "Anim_杠").gameObject;
        Anim_杠_Obj.SetActive(false);
        Anim_杠_Obj.GetComponent<UISpriteAnimation>().isHideComplete = true;
        Anim_暗杠_Obj = GameUtility.FindDeepChild(gameObject, "Anim_暗杠").gameObject;
        Anim_暗杠_Obj.SetActive(false);
        Anim_暗杠_Obj.GetComponent<UISpriteAnimation>().isHideComplete = true;
        Anim_自摸_Obj = GameUtility.FindDeepChild(gameObject, "Anim_自摸").gameObject;
        Anim_自摸_Obj.SetActive(false);
        Anim_自摸_Obj.GetComponent<UISpriteAnimation>().isHideComplete = true;
    }

    public void ShowAnim(ResultType type)
    {
        GameObject curAnimObj = null;
        switch (type)
        {
            case ResultType.吃:
                curAnimObj = Anim_吃_Obj;                
                break;
            case ResultType.碰:
                curAnimObj = Anim_碰_Obj;                
                break;
            case ResultType.杠:
                curAnimObj = Anim_杠_Obj;                
                break;
            case ResultType.暗杠:
                curAnimObj = Anim_暗杠_Obj;
                break;
            case ResultType.糊:
                curAnimObj = Anim_胡_Obj;                
                break;
            case ResultType.过:
                break;
            case ResultType.自摸:
                curAnimObj = Anim_自摸_Obj;                
                break;
            default:
                break;
        }

        if (curAnimObj!=null)
        {
            curAnimObj.GetComponent<UISpriteAnimation>().ResetToBeginning();
            curAnimObj.GetComponent<UISpriteAnimation>().Play();
            curAnimObj.SetActive(true);
        }
    }

    private void PlayerInfoInit()
    {
        InfoObj = GameUtility.FindDeepChild(gameObject, "Info_BG").gameObject;
        HideInfo();
    }

    private void EventInit()
    {
        UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "HeadFrame").gameObject).onClick = delegate
        {
            ShowInfo();
        };
        UIEventListener.Get(GameUtility.FindDeepChild(gameObject, "ColliderBg").gameObject).onClick = delegate
        {
            HideInfo();
        };
    }

    public void ReflushPanel(PlayerData data)
    {
        this.data = data;
        DespawnAllMJ();
        SpawnAllMJ();
        HideAllBtnResult();
    }

    protected virtual void SortMJDepth()
    {

    }

    private void SpawnAllMJ()
    {
        SpawnMJ_CPG();
        SpawnMJ_IN();
        SpawnMJ_OUT();
        SortMJDepth();
        UpdatePosition();
        //Panel.SortWidgets();
    }

    /// <summary>
    /// 生成手中的麻将
    /// </summary>
    private void SpawnMJ_IN()
    {
        for (int i = 0; i < data.MJ_IN_List.Count; i++)
        {
            MJ mj = AddMJ(data.MJ_IN_List[i], MJ_STAGE.IN);
        }
    }

    /// <summary>
    /// 生成打出去的麻将
    /// </summary>
    private void SpawnMJ_OUT()
    {
        for (int i = 0; i < data.MJ_OUT_List.Count; i++)
        {
            MJ mj = AddMJ(data.MJ_OUT_List[i], MJ_STAGE.OUT);
        }
    }

    /// <summary>
    /// 生成吃碰杠的麻将
    /// </summary>
    private void SpawnMJ_CPG()
    {
        for (int i = 0; i < data.MJ_CPG_List.Count; i++)
        {
            SpawnMJ_CPG(data.MJ_CPG_List[i]);            
        }
    }

    protected virtual void SpawnMJ_CPG(CPG_Struct data)
    {
        for (int i = 0; i < data.MJList.Count; i++)
        {
            if (data.type == CPG_TYPE.暗杠)
            {
                if (i < 3 || dir != SEAT_DIR.DIR_BOTTOM)
                {
                    AddMJ(data.MJList[i], MJ_STAGE.CPG, true);
                    continue;
                }                
            }
            MJ mj = AddMJ(data.MJList[i], MJ_STAGE.CPG);
            switch (data.dir)
            {
                case SEAT_DIR.DIR_BOTTOM:
                    break;
                case SEAT_DIR.DIR_RIGHT:
                    if (i == 2) mj.sprite.color = Color.green;
                    break;
                case SEAT_DIR.DIR_TOP:
                    if (data.type == CPG_TYPE.杠)
                    {
                        if (i == 3) mj.sprite.color = Color.green;
                    }
                    else
                    {
                        if (i == 1) mj.sprite.color = Color.green;
                    }
                    break;
                case SEAT_DIR.DIR_LEFT:
                    if (i == 0) mj.sprite.color = Color.green;
                    break;
                case SEAT_DIR.DIR_NULL:
                    break;
                default:
                    break;
            }
        }
    }

    public void UpdatePosition()
    {
        UpdatePosition_IN();
        UpdatePosition_OUT();
        UpdatePosition_CPG();
    }
    protected virtual void UpdatePosition_IN() { curSelectMJ = null; }
    protected virtual void UpdatePosition_OUT() { }
    protected virtual void UpdatePosition_CPG() { }
    protected virtual Vector3 GetLastPosition_CPG()
    {
        return Vector3.zero;
    }

    /// <summary>
    /// 添加麻将
    /// </summary>
    /// <param name="type">麻将类型</param>
    /// <param name="stage"> 是否是手中的麻将 </param>
    /// <returns></returns>
    protected virtual MJ AddMJ(MJType type, MJ_STAGE stage, bool bBack=false)
    {
        MJ mj = _MJMgr.SpawnMJ(transform, type, data.playerId, stage, bBack, this);
        switch (stage)
        {
            case MJ_STAGE.IN:
                MJ_IN_List.Add(mj);
                break;
            case MJ_STAGE.OUT:
                MJ_OUT_List.Add(mj);
                break;
            case MJ_STAGE.CPG:
                MJ_CPG_List.Add(mj);
                break;
            default:
                break;
        }        
        return mj;
    }

    private void DespawnAllMJ()
    {
        while(MJ_IN_List.Count>0)
        {
            DespawnMJ(MJ_IN_List[0]);
        }
        while (MJ_OUT_List.Count > 0)
        {
            DespawnMJ(MJ_OUT_List[0]);
        }
        while (MJ_CPG_List.Count > 0)
        {
            DespawnMJ(MJ_CPG_List[0]);
        }
    }

    protected void DespawnMJ(MJ mj)
    {        
        _MJMgr.DespawnMJ(mj.transform);
        switch (mj.stage)
        {
            case MJ_STAGE.IN:
                MJ_IN_List.Remove(mj);
                break;
            case MJ_STAGE.OUT:
                MJ_OUT_List.Remove(mj);
                break;
            case MJ_STAGE.CPG:
                MJ_CPG_List.Remove(mj);
                break;
            default:
                break;
        }        
    }


    protected virtual Vector3 GetMJStartPos_IN() { return MJ_IN_START_TRANS.localPosition; }

    public virtual void SelectMJ(MJ mj, bool bClick=false)
    {     
    }

    private Vector3 GetHeadWorldPos()
    {
        Transform headTrans = GameUtility.FindDeepChild(this.gameObject, "HeadIcon");
        Vector3 pos = UICamera.currentCamera.WorldToViewportPoint(headTrans.position);
        Vector3 headWorldPos = GDFunc.NGUIPosToWorld(headTrans.gameObject);
        headWorldPos = Camera.main.ViewportToWorldPoint(pos);
        headWorldPos.z = -Camera.main.transform.position.z;
        return headWorldPos;
    }

    public void SetCanDragMJ(bool bDrag)
    {
        for (int i = 0; i < MJ_IN_List.Count; i++)
        {
            MJ_Drag md = MJ_IN_List[i].GetComponent<MJ_Drag>();
            if (md) md.enabled = bDrag;
        }
    }

    private void ShowInfo()
    {        
        InfoObj.SetActive(true);
        PlayerInfo playerInfo = DataCenter.Instance.players[data.playerId].playerInfo;
        GameUtility.FindDeepChild(InfoObj, "ID").GetComponent<UILabel>().text = playerInfo.GameID;
        GameUtility.FindDeepChild(InfoObj, "昵称").GetComponent<UILabel>().text = playerInfo.WXName;
        GameUtility.FindDeepChild(InfoObj, "金币").GetComponent<UILabel>().text = "" + playerInfo.coin;
        GameUtility.FindDeepChild(InfoObj, "IP").GetComponent<UILabel>().text = playerInfo.ip;
        GameUtility.FindDeepChild(InfoObj, "房卡").GetComponent<UILabel>().text = "" + playerInfo.diamond;

        CancelInvoke("HideInfo");
        Invoke("HideInfo", 3);
    }

    private void HideInfo()
    {
        InfoObj.SetActive(false);
    }

    public void ShowMsg_Dialogue(string context)
    {
        TweenAlpha tween = ChatMsg_Dialogue_Obj.GetComponent<TweenAlpha>();
        tween.value = 1;
        tween.ResetToBeginning();
        tween.PlayForward();
        ChatMsg_Dialogue_Obj.GetComponentInChildren<UILabel>().text = context;
    }

    public void ShowMsg_Face(string faceName)
    {
        TweenAlpha tween = ChatMsg_Face_Obj.GetComponent<TweenAlpha>();
        tween.value = 1;
        tween.ResetToBeginning();
        tween.PlayForward();
        ChatMsg_Face_Obj.GetComponent<UISprite>().spriteName = faceName;
        ChatMsg_Face_Obj.GetComponent<Animator>().Play("ChatMsg_Face_Animation");
        //ActiveAnimation.Play(ChatMsg_Face_Obj.GetComponent<Animation>(), AnimationOrTween.Direction.Forward);
        //ChatMsg_Face_Obj.GetComponent<Animation>().enabled = true;
        //ChatMsg_Face_Obj.GetComponent<Animation>().Play();
    }

    public void StopFaceAnimation()
    {
        //ChatMsg_Face_Obj.GetComponent<Animation>().enabled = false;
        //ChatMsg_Face_Obj.GetComponent<Animation>().Stop();
    }

    public void ShowZhuang(bool bShow)
    {
        ZhuangObj.SetActive(bShow);
    }

    public virtual void RefleshPlayerInfo(PlayerData data)
    {
        int playerId = data.playerId;
        PlayerInfo info = DataCenter.Instance.players[playerId].playerInfo;        
        headIconSprite.atlas = null;
        if (info.WXName != "")
        {
            headIconSprite.atlas = DynamicAtlas.Instance.Atla;
            headIconSprite.spriteName = info.WXTX_Icon_SpriteName;
            label_Name.text = info.WXName;
            label_Coin.text = "" + info.coin;
            label_Geolocation.text = info.Geolocation;
            //headIconSprite.SetDirty();
        }   
        else
        {
            label_Name.text = "";
            label_Coin.text = "";
            headIconSprite.spriteName = "";
            label_Geolocation.text = "";
        }
    }

    public virtual void ShowZhongYu()
    {
        if (data.gameOver.IsWin)
        {
            _player_中鱼.ShowZhongYu(data.gameOver.ZhongYuList);
        }        
    }

    //设置不能选择麻将
    public void SetCannotSelect(MJType type)
    {
        for (int i = 0; i < MJ_IN_List.Count; i++)
        {
            if (MJ_IN_List[i].type==type)
            {
                MJ_IN_List[i].SetColor(new Color(0.3f, 0.3f, 0.3f));
                MJ_Drag md = MJ_IN_List[i].GetComponent<MJ_Drag>();
                if (md) md.enabled = false;
            }            
        }
    }

    public virtual void HideAllBtnResult()
    {

    }
}
}

