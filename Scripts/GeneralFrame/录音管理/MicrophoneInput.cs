using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace DYD
{
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneInput : MonoBehaviour
    {

        private static MicrophoneInput m_instance;

        public float sensitivity = 100;
        public float loudness = 0;

        private static string[] micArray = null;

        const int HEADER_SIZE = 44;

        const int RECORD_TIME = 10;

        // Use this for initialization  
        void Start()
        {
            
        }

        public static MicrophoneInput getInstance()
        {
            if (m_instance == null)
            {
                micArray = Microphone.devices;
                if (micArray.Length == 0)
                {
                    Debuger.LogError("Microphone.devices is null");
                }
                foreach (string deviceStr in Microphone.devices)
                {
                    Debuger.Log("device name = " + deviceStr);
                }
                if (micArray.Length == 0)
                {
                    Debuger.LogError("no mic device");
                }

                GameObject MicObj = new GameObject("MicObj");
                m_instance = MicObj.AddComponent<MicrophoneInput>();
            }
            return m_instance;
        }

        public void StartRecord()
        {
            audio.Stop();
            if (micArray.Length == 0)
            {
                Debuger.Log("No Record Device!");
                return;
            }
            audio.loop = false;
            audio.mute = true;
            audio.clip = Microphone.Start(null, false, RECORD_TIME, 8000); //22050   
            while (!(Microphone.GetPosition(null) > 0))
            {
            }
            audio.Play();
            Debuger.Log("StartRecord");
            //倒计时  
            StartCoroutine(TimeDown());

        }

        public void StopRecord()
        {
            if (micArray.Length == 0)
            {
                Debuger.Log("No Record Device!");
                return;
            }
            if (!Microphone.IsRecording(null))
            {
                return;
            }
            Microphone.End(null);
            audio.Stop();

            Debuger.Log("StopRecord");

        }

        public Byte[] GetClipData()
        {
            if (audio.clip == null)
            {
                Debuger.Log("GetClipData audio.clip is null");
                return null;
            }

            float[] samples = new float[audio.clip.samples];

            audio.clip.GetData(samples, 0);


            Byte[] outData = new byte[samples.Length * 2];
            //Int16[] intData = new Int16[samples.Length];  
            //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]  

            int rescaleFactor = 32767; //to convert float to Int16  

            for (int i = 0; i < samples.Length; i++)
            {
                short temshort = (short)(samples[i] * rescaleFactor);

                Byte[] temdata = System.BitConverter.GetBytes(temshort);

                outData[i * 2] = temdata[0];
                outData[i * 2 + 1] = temdata[1];


            }
            if (outData == null || outData.Length <= 0)
            {
                Debuger.Log("GetClipData intData is null");
                return null;
            }
            //return intData;  
            return outData;
        }
        public void PlayClipData(byte[] byteArr)
        {
            Int16[] intArr = new Int16[byteArr.Length / 2];
            for (int i = 0; i < intArr.Length; i++)
            {
                intArr[i] = BitConverter.ToInt16(byteArr, i * 2);
            }
            PlayClipData(intArr);
        }
        private void PlayClipData(Int16[] intArr)
        {

            //string aaastr = intArr.ToString();
            //long aaalength = aaastr.Length;
            //Debug.Log("aaalength=" + aaalength);

            //string aaastr1 = Convert.ToString(intArr);
            //aaalength = aaastr1.Length;
            //Debuger.Log("aaalength=" + aaalength);

            if (intArr.Length == 0)
            {
                Debuger.Log("get intarr clipdata is null");
                return;
            }
            //从Int16[]到float[]  
            float[] samples = new float[intArr.Length];
            int rescaleFactor = 32767;
            for (int i = 0; i < intArr.Length; i++)
            {
                samples[i] = (float)intArr[i] / rescaleFactor;
            }

            //从float[]到Clip  
            AudioSource audioSource = this.GetComponent<AudioSource>();
            if (audioSource.clip == null)
            {
                audioSource.clip = AudioClip.Create("playRecordClip", intArr.Length, 1, 44100, false, false);
            }
            audioSource.clip.SetData(samples, 0);
            audioSource.mute = false;
            audioSource.Play();
        }
        public void PlayRecord()
        {
            if (audio.clip == null)
            {
                Debug.Log("audio.clip=null");
                return;
            }
            audio.mute = false;
            audio.loop = false;
            audio.Play();
            Debuger.Log("PlayRecord");

        }



        public float GetAveragedVolume()
        {
            float[] data = new float[256];
            float a = 0;
            audio.GetOutputData(data, 0);
            foreach (float s in data)
            {
                a += Mathf.Abs(s);
            }
            return a / 256;
        }

        // Update is called once per frame  
        void Update()
        {
            loudness = GetAveragedVolume() * sensitivity;
            if (loudness > 1)
            {
                //Debuger.Log("loudness = " + loudness);
            }
        }

        private IEnumerator TimeDown()
        {
            Debuger.Log(" IEnumerator TimeDown()");

            int time = 0;
            while (time < RECORD_TIME)
            {
                if (!Microphone.IsRecording(null))
                { //如果没有录制  
                    Debuger.Log("IsRecording false");
                    yield break;
                }
                Debuger.Log("yield return new WaitForSeconds " + time);
                yield return new WaitForSeconds(1);
                time++;
            }
            if (time >= 10)
            {
                Debuger.Log("RECORD_TIME is out! stop record!");
                StopRecord();
            }
            yield return 0;
        }  
    }
}

