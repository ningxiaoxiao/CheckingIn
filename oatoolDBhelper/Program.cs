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
            
            //得到这两个月的OA数据
            var t = DateTime.Today;
            t = t.AddDays(-40);
            oahelper.GetData(t);

    


          
            var delcount = DB.Context.DeleteAll<Dos.Model.original>();
            log._logger.Info($"清楚考勤原有记录{delcount}条");

            var Device = new WorkThread("192.168.4.31", 4370);//You can custom the LAN Segment.

            Device.Connect();
            Device.GetUser();

            Device.GetLog();

            Device.DisConnect();

             Device = new WorkThread("192.168.4.32", 4370);//You can custom the LAN Segment.

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


                string idwEnrollNumber ="";
                string sName = "";
                string sPassword = "";
                int iPrivilege = 0;
                bool bEnabled = false;

                int idwFigerIndex;
                string sTmpData = "";
                int iTmpLength = 0;
                int iUpdateFlag = 1;

                sdk.EnableDevice(iMachineNumber, false);
                sdk.ReadAllUserID(iMachineNumber);//read all the user information to the memory
                                                  //  sdk.ReadAllTemplate(iMachineNumber);//read all the users' fingerprint templates to the memory
                sdk.ReadAllTemplate(iMachineNumber);
                var count = 0;
                while (sdk.SSR_GetAllUserInfo(iMachineNumber, out idwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get all the users' information from the memory
                {
                    
                    var sb = sName.Split('\0');


                    users.Add(idwEnrollNumber.ToString(), sb[0]);
                    count++;
                }
                sdk.EnableDevice(iMachineNumber, true);
                log._logger.Info($"得到{count}个用户");
            }

            public void GetLog()
            {
                int iLogCount = 0;
                int idwErrorCode = 0;

               
                var tran = DB.Context.BeginTransaction();
                try
                {
                    if (sdk.ReadGeneralLogData(iMachineNumber))
                    {
                        string sdwEnrollNumber = "";
                        int idwVerifyMode = 0;
                        int idwInOutMode = 0;
                        int idwYear = 0;
                        int idwMonth = 0;
                        int idwDay = 0;
                        int idwHour = 0;
                        int idwMinute = 0;
                        int idwSecond = 0;
                        int idwWorkcode = 0;


                        sdk.EnableDevice(iMachineNumber, false);//disable the device
                        if (sdk.ReadAllGLogData(iMachineNumber))
                        {
                            while (sdk.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                            out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get the records from memory
                            {
                                iLogCount++;//increase the number of attendance records

                                lock (myObject)//make the object exclusive 
                                {

                                    // string k = $"enroll={idwEnrollNumber},VerifyMode={idwVerifyMode},time={sTime}";

                                    // log._logger.Info(k);
                                  

                                    //读出时间
                                    var time = new DateTime(idwYear,idwMonth,idwDay,idwHour,idwMinute,idwSecond);

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

                                    if (!users.ContainsKey(sdwEnrollNumber.ToString()))
                                    {
                                        log._logger.Error("找不到用户,id=" + sdwEnrollNumber);
                                        continue;
                                    }


                                    var o = new Dos.Model.original()
                                    {
                                        name = users[sdwEnrollNumber.ToString()],
                                        date = time.Date,
                                        time = tt.Ticks,

                                    };

                                    DB.Context.Insert(tran, o);


                                }
                            }
                        }
                        else
                        {
                            sdk.GetLastError(ref idwErrorCode);

                            if (idwErrorCode != 0)
                            {
                                log._logger.Error("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString());
                            }
                            else
                            {
                                log._logger.Error("No data from terminal returns!");
                            }
                        }
                       
                    }
                    else
                    {
                        throw new Exception("没有读到数据");
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    sdk.GetLastError(ref idwErrorCode);
                    log._logger.Error(ex + "readcount=" + iLogCount + " ErrorCode=" + idwErrorCode);

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
