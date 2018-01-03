using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using ILog = log4net.ILog;


namespace CheckingIn
{
    public class HttpSever
    {
        private readonly HttpAppServer _server;
        private readonly ServerConfig _config;

        private readonly string _filepath = Directory.GetCurrentDirectory() + "\\wwwroot";

        public HttpSever()
        {
            _config = new ServerConfig();
            _config.Port = 8080;

            _server = new HttpAppServer();

            if (!_server.Setup(_config))
                Log.Err("http setup err");

            _server.NewRequestReceived += Server_NewRequestReceived;
            _server.NewSessionConnected += Server_NewSessionConnected;
            _server.SessionClosed += Server_SessionClosed;
            if (_server.Start())
                Log.Info("httpserver.started");
            else
                Log.Info("http start err");

        }

        private Dictionary<string, string> buffer = new Dictionary<string, string>();

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

            var fileallpath = _filepath + requestInfo.Url;

            var retHtml = "ERR:no file " + requestInfo.Url;

            //查看文件缓存
            if (buffer.ContainsKey(fileallpath))
            {
                retHtml = buffer[fileallpath];
            }
            else if (File.Exists(fileallpath))
            {
                //把文件读出来 发送过去
                var sr = new StreamReader(File.OpenRead(fileallpath));
                retHtml = sr.ReadToEnd();

                buffer.Add(fileallpath, retHtml);

                sr.Close();
            }
            else if (requestInfo.Parameter.Count > 0)
            {
                switch (requestInfo.Url)
                {
                    case "/changepw":
                        retHtml = Changepw(requestInfo);
                        break;
                    case "/getdata":
                        retHtml = GetData(requestInfo);
                        break;
                    case "/getout.xls":
                        //生成文件
                        retHtml = CheckingIn.Inst.GetOutXlsString(int.Parse(requestInfo.Parameter["year"].ToString()), int.Parse(requestInfo.Parameter["month"].ToString()));
                        break;
                }
            }

            //组装头
            var sb = new StringBuilder();
            sb.AppendLine("HTTP/1.1 200 OK");

            //chome 会自己进行相关头增加 加个头就好了

            var ex = Path.GetExtension(fileallpath);
            switch (ex)
            {
                case ".html":
                    sb.AppendLine("Content-Type: text/html; charset=utf-8");
                    break;
                case ".js":
                    //sb.AppendLine("Content-Type: text/html; charset=utf-8");
                    break;
                case ".css":
                    sb.AppendLine("Content-Type: text/css");
                    break;
                case ".xls":
                    sb.AppendLine("Content-Type: application/octet-stream");
                    sb.AppendLine("Content-Disposition: attachment; filename=\"getout.xls\";");
                    break;
            }
            /*
            if (requestInfo.headers.ContainsKey("Content-Type"))
            {
                sb.Append("Content-Type: " + requestInfo.headers["Content-Type"]);
            }

            if (requestInfo.headers.ContainsKey("Accept"))
            {
                var accepts = requestInfo.headers["Accept"].Split(',');
                if (accepts.Length >= 2)
                    sb.Append("Content-Type: " + accepts[0]);
            }
            */
            //sb.AppendLine("Content-Length: ")
            sb.AppendLine();//一个空行
            sb.Append(retHtml);

            var bs = Encoding.UTF8.GetBytes(sb.ToString());

            session.Send(bs, 0, bs.Length);
            session.Close();

        }

        private string Changepw(HttpRequsetInfo requestInfo)
        {
            var name = requestInfo.Parameter["name"];
            var password = requestInfo.Parameter["pw"];

            var newpassword = requestInfo.Parameter["newpw"];


            Log.Info($"changePw,{name},{password},{newpassword}");

            var getp = DB.Context.From<Dos.Model.person>().Where(p => p.name == name && p.password == password).First();
            if (getp == null)
                return "用户名or密码错误";
            getp.password = newpassword;


            var i = DB.Context.Update(getp);

            if (i > 0)
                return "成功";
            return "失败";


        }

        private string GetData(HttpRequsetInfo requestInfo)
        {

            var name = requestInfo.Parameter["name"];
            var password = requestInfo.Parameter["password"];
            var month = requestInfo.Parameter["month"];
            var year = requestInfo.Parameter["year"];


            if (name == null || password == null || month == null)
                return "用户名or密码错误";




            var getp = DB.Context.From<Dos.Model.person>().Where(p => p.name == name).First();


            if (getp == null)
                return "用户名or密码错误";

            //超级密码 chaoji666
            if (password != "chaoji666" && getp.password != password)
                return "用户名or密码错误";


            //建立这个用户
            if (!DB.Persons.ContainsKey(name))
            {
                DB.Persons.Add(name, new PersonInfo(name));
            }
            var pp = DB.Persons[name];

            pp.GetData(int.Parse(year), int.Parse(month));
            Log.Info($"GetData,{name},{password},{month}");
            return pp.GetJson().ToJson();


        }

        internal void Close()
        {
            _server.Stop();
        }
    }


    public class HttpRequsetInfo : IRequestInfo
    {
        public string Key { get; }

        public string Url { get; set; }
        private readonly string _data;
        public string name;
        public HttpRequsetInfo(string data)
        {
            _data = data;

            var keys = data.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var methodstr = keys[0].Split(' ');


            //解出头
            foreach (var str in keys)
            {
                var kav = str.Split(':');
                if (kav.Length == 2)
                {
                    headers.Add(kav[0], kav[1]);
                }

            }


            if (methodstr[1] == "/")
                methodstr[1] = "/index.html";

            Url = methodstr[1].Replace("/?", "/index.html?");

            //解参数

            if (Url.IndexOf("?") >= 0)
            {
                var ps = Url.Split('?');
                Url = ps[0];
                IsHavePars = true;
                var pss = ps[1].Split('&');
                foreach (var i in pss)
                {
                    var kv = i.Split('=');

                    var v = kv[1];

                    if (kv[1].Contains("%"))
                        v = HttpUtility.UrlDecode(v);
                    Parameter.Add(kv[0], v);
                }

            }




            //hr.Method=
        }

        public Dictionary<string, string> headers = new Dictionary<string, string>();
        public Dictionary<string, string> Parameter = new Dictionary<string, string>();

        internal bool IsHavePars;
    }

    public class HttpReceiveFilter : IReceiveFilter<HttpRequsetInfo>
    {
        public HttpRequsetInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            //解出http包

            var data = Encoding.UTF8.GetString(readBuffer, offset, length);
            rest = 0;

            var hr = new HttpRequsetInfo(data);




            return hr;

        }

        public void Reset()
        {
            throw new NotImplementedException();
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
        public ILog Loger = Log._logger;
        public Encoding TextEncoding = Encoding.UTF8;
        public HttpAppServer()
            : base(new DefaultReceiveFilterFactory<HttpReceiveFilter, HttpRequsetInfo>())
        {

        }
    }
}