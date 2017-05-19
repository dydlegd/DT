using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Btn_Result : MonoBehaviour
    {
        public ResultType type;
        
        public void Init()
        {
            UIEventListener.Get(this.gameObject).onClick = delegate
            {
                ControlCenter.Instance.Btn_Result_OnClick(type);
            };
        }

        public void SetPosition(Vector3 pos)
        {
            transform.localPosition = pos;
        }

        public void Show(bool bShow)
        {
            this.gameObject.SetActive(bShow);
        }
    }
}

