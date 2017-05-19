using UnityEngine;
using System.Collections;

namespace DYD
{
    [RequireComponent(typeof(UILabel))]
    public class UITextRoll_LED : MonoBehaviour
    {
        /// <summary>
        /// 每秒显示多少个字
        /// </summary>
        public int charsPerSecond = 2;
        public UIPanel panel;
        private UILabel textLabel { get { return GetComponent<UILabel>(); } }
        private TweenPosition tp;
        void Awake()
        {
            SetText(textLabel.text);
        }

        public void SetText(string text)
        {
            textLabel.text = text;
            if(tp==null)
            {
                tp = gameObject.AddComponent<TweenPosition>();
                tp.style = UITweener.Style.Loop;                
            }
            tp.duration = (float)text.Length / charsPerSecond;
            tp.from = new Vector3(panel.width / 2 + textLabel.width / 2, 0, 0);
            tp.to = new Vector3(-panel.width / 2 - textLabel.width / 2, 0, 0);
        }
    }
}

