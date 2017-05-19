using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{

    public class ViewCenter : SingletonMonoBase<ViewCenter>
    {
        private Dictionary<PanelType, UIPanelBase> panelDic = new Dictionary<PanelType, UIPanelBase>();

        public void Init()
        {
            AddPanel(PanelType.Main, "Panel_Main", Panel_Depth.Main);
            AddPanel(PanelType.Player_BOTTOM, "Panel_Player_Bottom", Panel_Depth.Player_Bottom);
            AddPanel(PanelType.Player_LEFT, "Panel_Player_Left", Panel_Depth.Player_Left);
            AddPanel(PanelType.Player_TOP, "Panel_Player_Top", Panel_Depth.Player_Top);
            AddPanel(PanelType.Player_RIGHT, "Panel_Player_Right", Panel_Depth.Player_Right);
            AddPanel(PanelType.Hall, "Panel_Hall", Panel_Depth.Hall);
            AddPanel(PanelType.Setting, "Panel_Setting", Panel_Depth.Setting);
            AddPanel(PanelType.Exit, "Panel_Exit", Panel_Depth.Exit);
            AddPanel(PanelType.GameOver, "Panel_GameOver", Panel_Depth.GameOver);
            AddPanel(PanelType.UIManager, "Panel_UI", Panel_Depth.UIManager);
            AddPanel(PanelType.JoinRoom, "Panel_JoinRoom", Panel_Depth.JoinRoom);
            AddPanel(PanelType.Prompt, "Panel_Prompt", Panel_Depth.Prompt);
            AddPanel(PanelType.MessageBox, "Panel_MessageBox", Panel_Depth.MessageBox);
            AddPanel(PanelType.CreateRoom, "Panel_CreateRoom", Panel_Depth.CreateRoom);
            AddPanel(PanelType.Prepare, "Panel_Prepare", Panel_Depth.Prepare);
            AddPanel(PanelType.Help, "Panel_Help", Panel_Depth.Help);
            AddPanel(PanelType.Record, "Panel_Record", Panel_Depth.Record);
            AddPanel(PanelType.Chat, "Panel_Chat", Panel_Depth.Chat);
            AddPanel(PanelType.GameOver_Record, "Panel_GameOver_Record", Panel_Depth.GameOver_Record);
            AddPanel(PanelType.Login, "Panel_Login", Panel_Depth.Login);
            AddPanel(PanelType.HuiFang, "Panel_HuiFang", Panel_Depth.HuiFang);
            Debuger.Log("ViewCenter Init Succeed !");
        }

        private void AddPanel(PanelType type, string prefabPath, Panel_Depth depth = Panel_Depth.Zero)
        {
            GameObject prefab = ResourcesManager.GetGameObject(prefabPath);
            if (prefab)
            {
                GameObject panelGO = GDFunc.CreateGameObject(prefab,transform);
                AddPanel(type, panelGO.GetComponent<UIPanelBase>(), depth);
            }
            else
            {
                Debuger.LogError("ViewCenter AddPanel() Error !");
            }            
        }

        private void AddPanel(PanelType type, UIPanelBase panel, Panel_Depth depth)
        {
            if (panel)
            {
                panelDic[type] = panel;
                panelDic[type].Init(type);
                panel.Depth = (int)depth;
            }
        }

        public T GetPanel<T>(PanelType type) where T : UIPanelBase
        {
            if (panelDic.ContainsKey(type))
            {
                return panelDic[type] as T;
            }
            Debuger.LogError("ViewCenter GetPanel() Error !");
            return null;
        }

        public void ShowPanel<T>(PanelType type) where T : UIPanelBase
        {
            GetPanel<T>(type).ShowPanel();
        }

        public void HidePanel<T>(PanelType type) where T : UIPanelBase
        {
            GetPanel<T>(type).HidePanel();
        }

        public void HideAllPanel()
        {
            foreach (KeyValuePair<PanelType, UIPanelBase> kv in panelDic)
            {
                kv.Value.HidePanelDirectly();
            }
        }
    }

}