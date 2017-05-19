using UnityEngine;
using System.Collections;

namespace DYD
{
    public class TimeDown : SingletonMonoBase<TimeDown>
    {
        public void StartDownTime(int iStartTime=0)
        {
            if (iStartTime == 0) iStartTime = DataCenter.Instance.gamedata.MaxDownTime;
            StopDownTime();
            SetTimeDown(iStartTime);
            StartCoroutine(DownTimeLogic());
        }
        public void StopDownTime()
        {
            SetTimeDown(0);
            StopAllCoroutines();
        }
        private IEnumerator DownTimeLogic()
        {
            while ( DataCenter.Instance.gamedata.CurDownTime>0 )
            {
                yield return new WaitForSeconds(1);
                SetTimeDown(--DataCenter.Instance.gamedata.CurDownTime);
            }
            if (ControlCenter.Instance.IsChuPaing 
                && DataCenter.Instance.gamedata.gameMode==GAME_MODE.自由匹配
                )
            {
                ControlCenter.Instance.SetTrusteeship(true);
            }        
        }

        private void SetTimeDown(int time)
        {
            DataCenter.Instance.gamedata.CurDownTime = time;
            ViewCenter.Instance.GetPanel<Panel_Main>(PanelType.Main).SetLabelTime(time);
        }
    }
}

