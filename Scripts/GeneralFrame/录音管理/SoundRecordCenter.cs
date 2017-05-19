using UnityEngine;
using System.Collections;
using System;

namespace DYD
{
    [RequireComponent(typeof(AudioSource))]  
    public class SoundRecordCenter
    {
        private static AudioClip clip;
        private static int maxRecordTime = 10;
        private static int samplingRate = 8000;

        public static bool TryStartRecording()
        {
            try
            {
                Microphone.End(null);
                clip = Microphone.Start(null, false, maxRecordTime, samplingRate);
                Debuger.Log("开始录音...");
            }
            catch (System.Exception e)
            {
                return false;
            }
            return true;
        }


        public static bool EndRecording(out int length, out AudioClip outClip)
        {
            int lastPos = Microphone.GetPosition(null);

            if(Microphone.IsRecording(null))
            {
                length = lastPos / samplingRate;
            }
            else
            {
                length = maxRecordTime;
            }

            Microphone.End(null);

            if(length<1.0f)
            {
                outClip = null;
                Debuger.Log("录音时长太短");
                return false;
            }
            outClip = clip;
            Debuger.Log("录音结束：" + length);
            return true;
        }

        public static byte[] GetClipData(AudioClip clip)
        {
            float[] samples = new float[clip.samples];
            clip.GetData(samples, 0);

            byte[] outData = new byte[samples.Length * 2];
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
            if (outData==null||outData.Length<0)
            {
                Debuger.LogError("SoundRecordCenter GetClipData() Error !");
                return null;   
            }
            return outData;
        }

        public static void PlayClipData(Int16[] intArr)
        {
            if (intArr.Length==0)
            {
                Debuger.LogError("SoundRecordCenter PlayClipData() Error !get intarr clipdata is null");
                return;
            }
            float[] samples = new float[intArr.Length];
            int rescaleFactor = 32767;
            for (int i = 0; i < intArr.Length; i++)
            {
                samples[i] = (float)intArr[i] / rescaleFactor;
            } 
        }

        public static byte[] GetData(AudioClip clip)
        {
            var data = new float[clip.samples * clip.channels];

            clip.GetData(data, 0);

            byte[] bytes = new byte[data.Length * 4];
            Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length);
            return bytes;
        }
        
        public static AudioClip SetData(AudioClip clip, byte[] bytes)
        {
            float[] data = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, data, 0, data.Length);
            clip.SetData(data, 0);
            return clip;
        }
    }
}

