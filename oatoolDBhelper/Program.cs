using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Dos.ORM;
using log4net;


namespace oatoolDBhelper
{
    class Program
    {

        static void Main(string[] args)
        {

            

            //得到当月第一天
            var t = DateTime.Today;
            t = t.AddDays(-t.Day + 1);
            oahelper.GetData(t);


            OpenDataFile("data.xls");

        }


        private static void OpenDataFile(string path)
        {
            if (!File.Exists(path))
            {
                log._logger.Error("文件不存在");
                return;

            }


            var dt = new ExcelHelper(path).ExcelToDataTable("", true);
            var readcount = 0;

            DB.Context.DeleteAll<Dos.Model.original>();

            var tran = DB.Context.BeginTransaction();
            try
            {
                //进行遍历处理 生成新的表
                foreach (DataRow i in dt.Rows)
                {
                    //读出时间
                    var time = DateTime.Parse(i["日期时间"].ToString());

                    //如果 时间是 05:00前的 就把日期算到前一天上面去
                    TimeSpan tt;
                    if (time.TimeOfDay < new TimeSpan(5, 0, 0))
                    {
                        time = time.AddDays(-1);
                        tt = time.TimeOfDay.Add(new TimeSpan(1, 0, 0, 0));//时间值多一天
                    }
                    else
                    {
                        tt = time.TimeOfDay;
                    }

                    var o = new Dos.Model.original()
                    {
                        name = i["姓名"].ToString(),
                        date = time.Date,
                        time = tt.Ticks,

                    };

                    DB.Context.Insert(tran, o);
                    readcount++;

                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                log._logger.Error("读取考勤器文件出错." + ex.Message);
            }

            log._logger.Info($"共{dt.Rows.Count}记录,加入{readcount}条");
            log._logger.Info("读取考勤器文件完成");
        }

    }
}
