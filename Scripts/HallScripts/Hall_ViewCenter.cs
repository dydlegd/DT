using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DYD
{
    public class Hall_ViewCenter : SingletonMonoBase<Hall_ViewCenter>
    {
        private Dictionary<PanelType, UIPanelBase> panelDic = new Dictionary<PanelType, UIPanelBase>();

        public void Init()
        {
            AddPanel(PanelType.MessageBox, "Hall_Panel_MessageBox", Panel_Depth.MessageBox);
            Debuger.Log("ViewCenter Init Succeed !");
        }

        private void AddPanel(PanelType type, string prefabPath, Panel_Depth depth = Panel_Depth.Zero)
        {
            GameObject prefab = ResourcesManager.GetGameObject(prefabPath);
            if (prefab)
            {
                GameObject panelGO = GDFunc.CreateGameObject(prefab, transform);
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

