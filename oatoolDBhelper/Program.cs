using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Dos.ORM;
using log4net;
using zkemkeeper;


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


            //OpenDataFile("data.xls");


            var Device = new WorkThread("192.168.1.3", 4370);//You can custom the LAN Segment.

            Device.Connect();
            Device.GetUser();

            Device.GetLog();

            Device.DisConnect();


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
        //class of work thread
        class WorkThread
        {
            string sIP = "0.0.0.0";
            int iPort = 4370;
            int iMachineNumber = 1;

            private static int iCounter = 0;

            public CZKEMClass sdk = new CZKEMClass();//create Standalone SDK class dynamicly
            private static Object myObject = new Object();//create a new Object for the database operation

            //work thread
            public WorkThread(string swIP, int iwPort)
            {
                sIP = swIP;
                iPort = iwPort;
            }

            public void DisConnect()
            {
                sdk.Disconnect();
                log._logger.Info("Successfully DisConnect " + sIP);
            }

            public bool Connect()
            {
                bool bResult = sdk.Connect_Net(sIP, iPort);

                if (bResult)
                {
                    sdk.RegEvent(iMachineNumber, 65535);
                    log._logger.Info("Successfully Connect " + sIP);

                }
                else
                {
                    log._logger.Info("Connecting " + sIP + " Failed......Current Time:" + DateTime.Now.ToLongTimeString());
                }
                return bResult;

            }
            private Dictionary<string, string> users = new Dictionary<string, string>();
            public void GetUser()
            {
                //judge whether the device supports 9.0 fingerprint arithmetic
                string sOption = "~ZKFPVersion";
                string sValue = "";
                //if (axCZKEM1.GetSysOption(iMachineNumber, sOption, out sValue))
                //{
                //    if (sValue == "10")
                //    {
                //        MessageBox.Show("Your device is not using 9.0 arithmetic!", "Error");
                //        return;
                //    }
                //}

                int idwEnrollNumber = 0;
                string sName = "";
                string sPassword = "";
                int iPrivilege = 0;
                bool bEnabled = false;

                int idwFigerIndex;
                string sTmpData = "";
                int iTmpLength = 0;
                int iUpdateFlag = 1;

                sdk.EnableDevice(iMachineNumber, false);
                //sdk.BeginBatchUpdate(iMachineNumber, 1);//create memory space for batching data
                sdk.ReadAllUserID(iMachineNumber);//read all the user information to the memory
                                                  //  sdk.ReadAllTemplate(iMachineNumber);//read all the users' fingerprint templates to the memory

                var count = 0;
                while (sdk.GetAllUserInfo(iMachineNumber, ref idwEnrollNumber, ref sName, ref sPassword, ref iPrivilege, ref bEnabled))//get all the users' information from the memory
                {
                    users.Add(idwEnrollNumber.ToString(), sName);
                    count++;
                }
                sdk.EnableDevice(iMachineNumber, true);
                log._logger.Info($"得到{count}个用户");
            }

            public void GetLog()
            {
                int iLogCount = 0;
                int idwErrorCode = 0;

                sdk.EnableDevice(iMachineNumber, false);//disable the device
                var delcount = DB.Context.DeleteAll<Dos.Model.original>();
                log._logger.Info($"清楚考勤原有记录{delcount}条");
                var tran = DB.Context.BeginTransaction();
                try
                {
                    if (sdk.ReadGeneralLogData(iMachineNumber))
                    {
                        int idwTMachineNumber = 0;
                        int idwEnrollNumber = 0;
                        int idwEMachineNumber = 0;
                        int idwVerifyMode = 0;
                        int idwInOutMode = 0;
                        int idwYear = 0;
                        int idwMonth = 0;
                        var idwDay = 0;
                        var idwHour = 0;
                        var idwMinute = 0;
                        var sTime = "";


                        while (sdk.GetGeneralLogDataStr(iMachineNumber, ref idwEnrollNumber, ref idwVerifyMode, ref idwInOutMode, ref sTime))//get the records from memory
                        {
                            iLogCount++;//increase the number of attendance records

                            lock (myObject)//make the object exclusive 
                            {

                                // string k = $"enroll={idwEnrollNumber},VerifyMode={idwVerifyMode},time={sTime}";

                                // log._logger.Info(k);


                                //读出时间
                                var time = DateTime.Parse(sTime);

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
                                    name = users[idwEnrollNumber.ToString()],
                                    date = time.Date,
                                    time = tt.Ticks,

                                };

                                DB.Context.Insert(tran, o);


                            }
                        }
                    }
                    else
                    {
                        throw new Exception("没有读到数据");
                    }

                    tran.Commit();
                }
                catch (Exception)
                {
                    sdk.GetLastError(ref idwErrorCode);
                    log._logger.Info("General Log Data Count:0 ErrorCode=" + idwErrorCode);

                    tran.Rollback();
                }


                sdk.EnableDevice(iMachineNumber, true);//enable the device
                sdk.Disconnect();

                log._logger.Info($"共加入{iLogCount}条考勤数据");

            }

            //return the time in mimutes
            private int GetTimeInMinute()
            {
                return ((DateTime.Now.Hour * 24) + DateTime.Now.Minute);
            }

        }
    }
}
