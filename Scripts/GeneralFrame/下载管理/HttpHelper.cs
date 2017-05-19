using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;

namespace DYD
{
    public class HttpHelper
    {

        internal class WebReqState
        {
            public byte[] Buffer;

            public FileStream fs;

            public const int BufferSize = 1024;

            public Stream OrginalStream;

            public HttpWebResponse WebResponse;

            public WebReqState(string path)
            {
                Buffer = new byte[1024];
                fs = new FileStream(path, FileMode.Create);
            }

            public void Close()
            {
                fs.Close();
                OrginalStream.Close();
                WebResponse.Close();
            }
        }

        private string url = "";
        private string path = "";
        private string assetName = "";
        public bool IsDownAsseting { get; set; }
        public Action Oncomplete { get; set; }
        public Action<float> ProcessFunc;

        private int tryCount;
        private bool bCancelDownLoad = false;
        private float mProcess;
        public float Process
        {
            get { return mProcess; }
            private set
            {
                mProcess = value;
                //if (ProcessFunc != null) ProcessFunc(mProcess);
            }
        }

        public bool IsDownSucceed { get; set; }

        public HttpHelper(string url, string path, string assetName)
        {
            this.url = url;
            this.path = path;
            this.assetName = assetName;
            Init();
            AsyDownLoad(this.url);
        }

        public void Init()
        {
            Process = 0f;
            IsDownAsseting = false;
            IsDownSucceed = false;
            tryCount = 0;
            bCancelDownLoad = false;
        }

        public void ReDownLoad()
        {
            Init();
            AsyDownLoad(this.url);
        }

        private void AsyDownLoad(string url)
        {
            //Debuger.Log(url);
            //assetName = url.Split('/')[4];
            //Debuger.Log(assetName);         
            try
            {
                IsDownAsseting = true;
                HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
                httpRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), httpRequest);
            }
            catch (System.Exception e)
            {
                IsDownAsseting = false;
            }
        }

        void ResponseCallback(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest req = ar.AsyncState as HttpWebRequest;
                if (req == null) return;
                HttpWebResponse response = req.EndGetResponse(ar) as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    response.Close();
                    IsDownAsseting = false;
                    return;
                }
                Debuger.Log(path + "/" + assetName);
                WebReqState st = new WebReqState(path + "/" + assetName);
                st.WebResponse = response;
                Stream responseStream = response.GetResponseStream();
                st.OrginalStream = responseStream;
                responseStream.BeginRead(st.Buffer, 0, WebReqState.BufferSize, new AsyncCallback(ReadDataCallback), st);
            }
            catch (Exception e)
            {
                Debuger.Log(e);
                Debuger.LogError("ResponseCallback");
                Init();
            }
        }

        void ReadDataCallback(IAsyncResult ar)
        {
            WebReqState rs = ar.AsyncState as WebReqState;
            int read = rs.OrginalStream.EndRead(ar);
            if (bCancelDownLoad)
            {
                rs.Close();
                return;
            }

            if (read > 0)
            {
                rs.fs.Write(rs.Buffer, 0, read);
                rs.fs.Flush();
                Process = (float)rs.fs.Length / rs.WebResponse.ContentLength;
                if (ProcessFunc != null) ProcessFunc(Process);
                rs.OrginalStream.BeginRead(rs.Buffer, 0, WebReqState.BufferSize, new AsyncCallback(ReadDataCallback), rs);
            }
            else
            {
                if (rs.fs.Length == rs.WebResponse.ContentLength && rs.WebResponse.ContentLength > 0)
                {
                    IsDownAsseting = false;
                    IsDownSucceed = true;
                    if (Oncomplete != null) Oncomplete();

                    rs.Close();

                    Debuger.Log(assetName + ":::: success");
                }
                else
                {
                    tryCount++;
                    if (tryCount >= 50)
                    {
                        Init();
                        Debuger.Log("DownLoad Failed! Read Length:" + rs.fs.Length + " TotalLen:" + rs.WebResponse.ContentLength);

                        rs.Close();
                    }
                    System.Threading.Thread.Sleep(1);
                }
            }
        }

        public void CancelDownLoad()
        {
            bCancelDownLoad = true;
        }
    }
}

