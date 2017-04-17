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

        /// <summary>
        /// 以日期为开始 
        /// </summary>
        /// <param name="st">开始日期 不包含</param>
        /// <returns></returns>
        public static DataTable getAddwork(DateTime st)
        {



            //3     4    5         6        7       8    9
            //姓名,部门,申请日期,开始时间,结束时间,事由,小计
            var ret = new DataTable();
            ret.Columns.Add("姓名", typeof(string));
            ret.Columns.Add("开始时间", typeof(DateTime));
            ret.Columns.Add("结束时间", typeof(DateTime));
            ret.Columns.Add("事由", typeof(string));
            ret.Columns.Add("小计", typeof(int));





            login();
            var t = File.OpenText("加班.txt").ReadToEnd();
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
            var d = j["data"];
            foreach (JsonData jrow in d)
            {
                var kssj = DateTime.Parse(jrow["field0006"].ToString());
                if (kssj.Date <= st.Date)
                    continue;



                var row = ret.NewRow();
                row["姓名"] = jrow["field0003"].ToString();
                row["开始时间"] = kssj;
                row["结束时间"] = DateTime.Parse(jrow["field0007"].ToString());
                row["事由"] = jrow["field0008"].ToString();
                row["小计"] = int.Parse(jrow["field0009"].ToString());

                ret.Rows.Add(row);

            }

            Log.Info("读取加班完成");
            return ret;

        }
    }
}