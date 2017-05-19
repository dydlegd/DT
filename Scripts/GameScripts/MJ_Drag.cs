using UnityEngine;
using System.Collections;

namespace DYD
{
    public class MJ_Drag : UIDragDropItem
    {
        protected override void OnDragDropStart()
        {
            base.OnDragDropStart();
            MJ mj = GetComponent<MJ>();
            if (mj)
            {
                mj.Depth += 1000;
            }
        }

        protected override void OnDragDropRelease(GameObject surface)
        {
            base.OnDragDropRelease(surface);

            MJ mj = GetComponent<MJ>();
            bool bChuPai = false;
            if (surface != null)
            {
                Debuger.Log(surface);
                if (surface.name=="DragRegion" && ControlCenter.Instance.IsChuPaing)
                {
                    bChuPai = true;
                    ControlCenter.Instance.ChuPai(mj);                    
                }
            }   
         
            if(!bChuPai)
            {
                mj.Depth -= 1000;
                ControlCenter.Instance.ResetPlayerPanelPosition(GetComponent<MJ>().PlayerID);
            }
        }
    }
}

