using UnityEngine;
using System.Collections;

namespace DYD
{
    public class FaceItem : MonoBehaviour
    {
        public enum FaceType
        {
            FaceType_0,
            FaceType_1,
            FaceType_2,
            FaceType_3,
            FaceType_4,
            FaceType_5,
            FaceType_6,
            FaceType_7,
            FaceType_8,
            FaceType_9,
            FaceType_10,
            FaceType_11,
            FaceType_12,
            FaceType_13,
            FaceType_14,
            FaceType_15,
            FaceType_16,
            FaceType_17,
            FaceType_18,
            FaceType_19
        }

        public FaceType type;
        

        public void OnClick()
        {
            Debuger.Log("FaceClick: " + type);
            string spriteName = GetComponent<UISprite>().spriteName;
            ViewCenter.Instance.GetPanel<Panel_Player_Bottom>(PanelType.Player_BOTTOM).ShowMsg_Face(spriteName);
            ViewCenter.Instance.HidePanel<Panel_Chat>(PanelType.Chat);
            DealCommand.Instance.SendBQ(GetComponent<UISprite>().spriteName);
        }
    }
}

