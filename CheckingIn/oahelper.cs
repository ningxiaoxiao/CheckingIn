using System;
using System.Data;
using System.IO;
using System.Web;
using DotNet4.Utilities;
using LitJson2;

namespace CheckingIn
{
    //自动拉取oa数据
    public static class oahelper
    {

        private static string jidcookie = "";
        private static HttpHelper http;
        private static void login()
        {
            if (jidcookie != "") return;//已经登录了.
            http = new HttpHelper();
            var item = new HttpItem()
            {
                URL = "http://192.168.1.254/seeyon/main.do?method=login",//URL     必需项  
                Method = "POST",//URL     可选项 默认为Get  
                Timeout = 100000,//连接超时时间     可选项默认为100000  
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000  
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写  
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0",//用户的浏览器类型，版本，操作系统     可选项有默认值  
                Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值  
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值  
                Postdata = "authorization=&login_username=oatool&login_password=123456&random=&fontSize=12&screenWidth=1920&screenHeight=1080",
            };
            var result = http.GetHtml(item);

            var cs = result.Cookie.Split(';');
            foreach (var c in cs)
            {
                if (c.Contains("JSESSIONID"))
                    jidcookie = c;
            }
        }

        private static DateTime _dateTime;

        /// <summary>
        /// 得到数据
        /// </summary>
        /// <param name="t">开始时间 包含</param>
        public static void GetData(DateTime t)
        {
            _dateTime = t;
            readjson("加班", 7, 3, 4, 3);
            readjson("外出", 1, 4, 8, 4);
            readjson("休假", 1, 8, 9, 8, 7);
            readjson("补登", 3, 5, 5, 5, 6);
            readjson("出差", 1, 12, 12, 13, 5);

        }

        private static void readjson(string m, int namearg, int startarg, int endarg, int timearg, int subreasonarg = -1)
        {

            login();
            var d = getoaData(m + ".txt");

            var trans = DB.Context.BeginTransaction();
            var count = 0;
            foreach (JsonData jrow in d)
            {
                var sqrj = DateTime.Parse(jrow["field00" + timearg.ToString("00")].ToString());

                if (sqrj.Date < _dateTime.Date)//数据合法时间
                    continue;

                var o = new Dos.Model.oa()
                {
                    name = jrow["field00" + namearg.ToString("00")].ToString(),
                    date = DateTime.Parse(jrow["field00" + startarg.ToString("00")].ToString()).Date,
                    start = DateTime.Parse(jrow["field00" + startarg.ToString("00")].ToString()),
                    end = DateTime.Parse(jrow["field00" + endarg.ToString("00")].ToString()),
                    reason = m,
                };



                if (subreasonarg != -1)
                {
                    o.subreason = jrow["field00" + subreasonarg.ToString("00")].ToString();
                }

                //加入oa表中
                DB.Context.Insert<Dos.Model.oa>(trans, o);
                count++;
            }

            trans.Commit();

            Log.Info($"读取{m}完成,共查询{d.Count}条记录,合法{count}条,合法时间{_dateTime}");
        }

        private static JsonData getoaData(string findTxtPath)
        {

            var t = File.OpenText(findTxtPath).ReadToEnd();
            var item = new HttpItem()
            {
                URL = "http://192.168.1.254/seeyon/ajax.do?method=ajaxAction&managerName=formQueryResultManager&rnd=" + new Random().Next(10000),
                Method = "post",
                Cookie = jidcookie,
                IsToLower = false,
                Postdata = "managerMethod=getQueryResult&arguments=" + HttpUtility.UrlEncode(t),
                ContentType = "application/x-www-form-urlencoded; charset=UTF-8",
            };
            var html = http.GetHtml(item).Html;

            var j = JsonMapper.ToObject(html);
            return j["data"];

        }
    }
}