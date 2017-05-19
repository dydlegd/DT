using UnityEngine;
using System.Collections;

namespace DYD
{

    [RequireComponent(typeof(UIScrollView))]
    public class TweenScroll : UITweener
    {

        public int from = 0;
        public int to = 0;
        public bool horizontal = false;
        float mSpeed = 0;
        public float speed
        {
            get { return mSpeed; }
            set { mSpeed = value; duration = Mathf.Abs(to - from) / mSpeed; }
        }

        Transform mTrans;
        UIScrollView mScrollView;

        int itemHeight = 0;

        public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public Vector3 value
        {
            get { return mScrollView.localScroll; }
            set
            {
                mScrollView.localScroll = value;
            }
        }

        void Awake() { mScrollView = GetComponent<UIScrollView>(); }

        protected override void Start()
        {
            base.Start();
            UIWrapContent wrap = cachedTransform.GetComponentInChildren<UIWrapContent>();
            if (wrap != null)
            {
                itemHeight = wrap.itemSize;
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            if (horizontal)
            {
                value = (new Vector3(from, 0) * itemHeight * (1f - factor) + new Vector3(to, 0) * itemHeight * factor);
            }
            else
            {
                value = (new Vector3(0, from) * itemHeight * (1f - factor) + new Vector3(0, to) * itemHeight * factor);
            }
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        static public TweenScroll Begin(GameObject go, float duration, int to)
        {
            TweenScroll comp = UITweener.Begin<TweenScroll>(go, duration);
            comp.from = 0;
            comp.to = to;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        //public void WrapContent ()
        //{
        //    UIWrapContent wrap = cachedTransform.GetComponentInChildren<UIWrapContent>();
        //    if (wrap != null)
        //    {
        //        //wrap.WrapContent();
        //        wrap.ResetChildPositions();
        //    }
        //}
    }

}
