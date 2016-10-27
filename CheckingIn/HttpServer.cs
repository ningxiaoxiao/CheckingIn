using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using ILog = log4net.ILog;
using LitJson;


namespace CheckingIn
{
    public class HttpSever
    {
        private HttpAppServer _server;
        private ServerConfig _config;

        private readonly string _filepath = Directory.GetCurrentDirectory() + "\\wwwroot";

        public HttpSever()
        {
            _config = new ServerConfig();
            _config.Port = 8080;

            _server = new HttpAppServer();

            if (!_server.Setup(_config))
                Log.info("http setup err");

            _server.NewRequestReceived += Server_NewRequestReceived;
            _server.NewSessionConnected += Server_NewSessionConnected;
            _server.SessionClosed += Server_SessionClosed;
            if (_server.Start())
                Log.info("httpserver.started");
            else
                Log.info("http start err");

        }



        private void Server_SessionClosed(HttpSession session, CloseReason value)
        {
            //throw new System.NotImplementedException();

        }

        private void Server_NewSessionConnected(HttpSession session)
        {
            // throw new System.NotImplementedException();
        }

        private void Server_NewRequestReceived(HttpSession session, HttpRequsetInfo requestInfo)
        {

            var fileallpath = _filepath + requestInfo.URL;

            var filestring = "ERR:no file " + requestInfo.URL;
            try
            {
                //把文件读出来 发送过去
                if (File.Exists(fileallpath))
                {
                    var sr = new StreamReader(File.OpenRead(fileallpath));
                    filestring = sr.ReadToEnd();
                    sr.Close();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                //没有文件的.应该就是命令了
                

                var name = requestInfo.parameter["name"];

                var p = DB.persons[name];
                p.GetData();
                filestring = p.GetJson();
            }






            //组装头
            var sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 200 OK");




            //chome 会自己进行相关头增加 加个头就好了

            var ex = Path.GetExtension(fileallpath);
            switch (ex)
            {
                case "html":
                    sb.AppendLine("Content-Type: text/html; charset=utf-8");
                    break;
                case "js":
                    // sb.AppendLine("Content-Type: text/html; charset=utf-8");
                    break;
                case "css":
                    //  sb.AppendLine("Content-Type: text/html; charset=utf-8");
                    break;
                default:
                    //sb.AppendLine("Content-Type: text/html; charset=utf-8");
                    break;

            }



            //  sb.AppendLine("Connection: keep-alive");
            sb.AppendLine();//一个空行
            sb.Append(filestring);

            var bs = Encoding.UTF8.GetBytes(sb.ToString());

            session.Send(bs, 0, bs.Length);
            session.Close();

        }
    }


    public class HttpRequsetInfo : IRequestInfo
    {

        public string URL { get; set; }

        public string Key { get; }


        public Dictionary<string, string> parameter = new Dictionary<string, string>();
        internal bool ishavepars;
    }

    public class HttpReceiveFilter : IReceiveFilter<HttpRequsetInfo>
    {
        public HttpRequsetInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            //解出http包

            var data = Encoding.UTF8.GetString(readBuffer, offset, length);
            rest = 0;
            var keys = data.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var hr = new HttpRequsetInfo();

            var methodstr = keys[0].Split(' ');

            if (methodstr[1] == "/")
                methodstr[1] = "/index.html";

            hr.URL = methodstr[1].Replace("/?", "/index.html?");



            if (hr.URL.IndexOf("?") >= 0)
            {
                var ps = hr.URL.Split('?');
                hr.URL = ps[0];
                hr.ishavepars = true;
                var pss = ps[1].Split('&');
                foreach (var i in pss)
                {
                    var kv = i.Split('=');
                    hr.parameter.Add(kv[0], HttpUtility.UrlDecode(kv[1],Encoding.UTF8));
                }

            }
            //hr.Method=



            return hr;

        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public int LeftBufferSize { get; }
        public IReceiveFilter<HttpRequsetInfo> NextReceiveFilter { get; }
        public FilterState State { get; }
    }

    public class HttpSession : AppSession<HttpSession, HttpRequsetInfo>
    {

    }

    public class HttpAppServer : AppServer<HttpSession, HttpRequsetInfo>
    {
        public ILog Loger = Log.logger;
        public Encoding TextEncoding = Encoding.UTF8;
        public HttpAppServer()
            : base(new DefaultReceiveFilterFactory<HttpReceiveFilter, HttpRequsetInfo>())
        {

        }
    }
}