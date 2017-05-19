using UnityEngine;
using System.Collections;

namespace DYD
{
    public class ScreenAdapt : MonoBehaviour
    {

        private static int ManualWidth = GD.DESIGN_SCREEN_W;
        private static int ManualHeight = GD.DESIGN_SCREEN_H;

        void Awake()
        {
            //AdaptiveUI();
            //Camera.main.fieldOfView = getCameraFOV(60);
            //Debug.Log("aspectOld: " + Camera.main.aspect);
            //Camera.main.aspect = (float)ManualWidth / ManualHeight;
            //Debug.Log("aspectNew: " + Camera.main.aspect);
            //Screen.SetResolution(GD.DESIGN_SCREEN_W, GD.DESIGN_SCREEN_H, true);
        }

        // Use this for initialization
        void Start()
        {

            foreach (Camera camera in Camera.allCameras)
            {
                camera.aspect = (float)ManualWidth / ManualHeight;
            }
        }

        static private void AdaptiveUI()
        {
            UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot>();
            if (uiRoot != null)
            {
                if (System.Convert.ToSingle(Screen.height) / Screen.width > System.Convert.ToSingle(ManualHeight) / ManualWidth)
                {
                    uiRoot.manualHeight = Mathf.RoundToInt(System.Convert.ToSingle(ManualWidth) / Screen.width * Screen.height);
                }
                else
                {
                    uiRoot.manualHeight = ManualHeight;
                }
            }
        }

        static public float getCameraFOV(float currentFOV)
        {
            UIRoot root = GameObject.FindObjectOfType<UIRoot>();
            float scale = System.Convert.ToSingle(root.manualHeight / ManualHeight);
            return currentFOV * scale;
        }
    }

}
