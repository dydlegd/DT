using UnityEngine;
using System.Collections;

namespace DYD
{
    public class Panel_Exit : UIPanelBase
    {

        public override void Init(DYD.PanelType type)
        {
            base.Init(type);
            AddColliderMode(PanelColliderMode.WithBg);
        }
    }
}

