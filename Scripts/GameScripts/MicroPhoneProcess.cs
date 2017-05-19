using UnityEngine;
using System.Collections;

namespace DYD{
public class MicroPhoneProcess :  MonoBehaviour{

    private float RecordTime = 0;
    private float fTimer = 0;
    private bool bStartRecord = false;
    private float fProcess = 0;
    private UISprite sprite_Process;
    private int sprite_w;

	// Use this for initialization
	void Start () {
        UIEventListener.Get(this.gameObject).onPress = MicroPhonePress;
        sprite_Process = GameUtility.FindDeepChild(gameObject, "Process").GetComponent<UISprite>();
        sprite_Process.enabled = false;
        Debuger.Log("McroPhoneProcess Start !");
        MicrophoneInput.getInstance().StartRecord();
        MicrophoneInput.getInstance().StopRecord();
	}
	
	// Update is called once per frame
	void Update () {
	    if(bStartRecord)
        {
            fTimer += Time.deltaTime;
            fProcess = fTimer / RecordTime;
            if (fProcess >= 1)
            {
                fProcess = 1;
                StopRecord();
            }
            sprite_Process.transform.localScale = new Vector3(1 - fProcess, 1, 1);
            //sprite_Process.SetRect(0, 0, (1 - fProcess) * sprite_w, sprite_Process.height);
        }
	}

    public void StartRecord(float recordTime)
    {
        fTimer = 0;
        RecordTime = recordTime;
        bStartRecord = true;
        fProcess = 0;
        sprite_Process.transform.localScale = new Vector3(1, 1, 1);
        sprite_Process.enabled = true;
        //sprite_Process.SetRect(0, 0, sprite_w, sprite_Process.height);

    }

    public void StopRecord()
    {
        fTimer = 0;
        RecordTime = 0;
        fProcess = 0;
        bStartRecord = false;
        sprite_Process.enabled = false;
    }

    private void MicroPhonePress(GameObject go, bool isPressed)
    {
        if (isPressed)
        {
            MicrophoneInput.getInstance().StartRecord();
            StartRecord(10);
            AudioManager.SetMute(true);
        }
        else
        {
            StopRecord();
            MicrophoneInput.getInstance().StopRecord();
            byte[] dataBuf = MicrophoneInput.getInstance().GetClipData();
            Debuger.Log("录间数据长度：" + dataBuf.Length);
            byte[] pressBuf = DataZipCenter.CompressByGZIP(dataBuf);
            Debuger.Log("压缩后数据长度：" + pressBuf.Length);
            MicrophoneInput.getInstance().PlayRecord();
            DealCommand.Instance.SendYuYing(pressBuf.Length, pressBuf);
            AudioManager.SetMute(false);
        }
    }
}

}
