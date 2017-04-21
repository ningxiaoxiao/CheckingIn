﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//     Website: http://ITdos.com/Dos/ORM/Index.html
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using Dos.ORM;

namespace Dos.Model
{
    /// <summary>
    /// 实体类person。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Table("person")]
    [Serializable]
    public partial class person : Entity
    {
        #region Model
        private string _name;
        private string _mail;
        private string _worktimeclass;
        private string _password;

        /// <summary>
        /// 
        /// </summary>
        [Field("name")]
        public string name
        {
            get { return _name; }
            set
            {
                this.OnPropertyValueChange("name");
                this._name = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("mail")]
        public string mail
        {
            get { return _mail; }
            set
            {
                this.OnPropertyValueChange("mail");
                this._mail = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("worktimeclass")]
        public string worktimeclass
        {
            get { return _worktimeclass; }
            set
            {
                this.OnPropertyValueChange("worktimeclass");
                this._worktimeclass = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("password")]
        public string password
        {
            get { return _password; }
            set
            {
                this.OnPropertyValueChange("password");
                this._password = value;
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 获取实体中的主键列
        /// </summary>
        public override Field[] GetPrimaryKeyFields()
        {
            return new Field[] {
                _.name,
            };
        }
        /// <summary>
        /// 获取列信息
        /// </summary>
        public override Field[] GetFields()
        {
            return new Field[] {
                _.name,
                _.mail,
                _.worktimeclass,
                _.password,
            };
        }
        /// <summary>
        /// 获取值信息
        /// </summary>
        public override object[] GetValues()
        {
            return new object[] {
                this._name,
                this._mail,
                this._worktimeclass,
                this._password,
            };
        }
        /// <summary>
        /// 是否是v1.10.5.6及以上版本实体。
        /// </summary>
        /// <returns></returns>
        public override bool V1_10_5_6_Plus()
        {
            return true;
        }
        #endregion

        #region _Field
        /// <summary>
        /// 字段信息
        /// </summary>
        public class _
        {
            /// <summary>
            /// * 
            /// </summary>
            public readonly static Field All = new Field("*", "person");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field name = new Field("name", "person", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field mail = new Field("mail", "person", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field worktimeclass = new Field("worktimeclass", "person", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field password = new Field("password", "person", "");
        }
        #endregion
    }


    /// <summary>
    /// 实体类original。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Table("original")]
    [Serializable]
    public partial class original : Entity
    {
        #region Model
        private string _name;
        private DateTime? _date;
        private long? _time;
        private string _info;

        /// <summary>
        /// 
        /// </summary>
        [Field("name")]
        public string name
        {
            get { return _name; }
            set
            {
                this.OnPropertyValueChange("name");
                this._name = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("date")]
        public DateTime? date
        {
            get { return _date; }
            set
            {
                this.OnPropertyValueChange("date");
                this._date = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("time")]
        public long? time
        {
            get { return _time; }
            set
            {
                this.OnPropertyValueChange("time");
                this._time = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("info")]
        public string info
        {
            get { return _info; }
            set
            {
                this.OnPropertyValueChange("info");
                this._info = value;
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 获取实体中的主键列
        /// </summary>
        public override Field[] GetPrimaryKeyFields()
        {
            return new Field[] {
            };
        }
        /// <summary>
        /// 获取列信息
        /// </summary>
        public override Field[] GetFields()
        {
            return new Field[] {
                _.name,
                _.date,
                _.time,
                _.info,
            };
        }
        /// <summary>
        /// 获取值信息
        /// </summary>
        public override object[] GetValues()
        {
            return new object[] {
                this._name,
                this._date,
                this._time,
                this._info,
            };
        }
        /// <summary>
        /// 是否是v1.10.5.6及以上版本实体。
        /// </summary>
        /// <returns></returns>
        public override bool V1_10_5_6_Plus()
        {
            return true;
        }
        #endregion

        #region _Field
        /// <summary>
        /// 字段信息
        /// </summary>
        public class _
        {
            /// <summary>
            /// * 
            /// </summary>
            public readonly static Field All = new Field("*", "original");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field name = new Field("name", "original", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field date = new Field("date", "original", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field time = new Field("time", "original", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field info = new Field("info", "original", "");
        }
        #endregion
    }

    /// <summary>
    /// 实体类oa。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Table("oa")]
    [Serializable]
    public partial class oa : Entity
    {
        #region Model
        private int _no;
        private DateTime? _date;
        private string _name;
        private DateTime? _start;
        private DateTime? _end;
        private string _reason;
        private string _subreason;

        /// <summary>
        /// 
        /// </summary>
        [Field("no")]
        public int no
        {
            get { return _no; }
            set
            {
                this.OnPropertyValueChange("no");
                this._no = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("date")]
        public DateTime? date
        {
            get { return _date; }
            set
            {
                this.OnPropertyValueChange("date");
                this._date = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("name")]
        public string name
        {
            get { return _name; }
            set
            {
                this.OnPropertyValueChange("name");
                this._name = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("start")]
        public DateTime? start
        {
            get { return _start; }
            set
            {
                this.OnPropertyValueChange("start");
                this._start = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("end")]
        public DateTime? end
        {
            get { return _end; }
            set
            {
                this.OnPropertyValueChange("end");
                this._end = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("reason")]
        public string reason
        {
            get { return _reason; }
            set
            {
                this.OnPropertyValueChange("reason");
                this._reason = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Field("subreason")]
        public string subreason
        {
            get { return _subreason; }
            set
            {
                this.OnPropertyValueChange("subreason");
                this._subreason = value;
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 获取实体中的主键列
        /// </summary>
        public override Field[] GetPrimaryKeyFields()
        {
            return new Field[] {
                _.no,
            };
        }
        /// <summary>
        /// 获取实体中的标识列
        /// </summary>
        public override Field GetIdentityField()
        {
            return _.no;
        }
        /// <summary>
        /// 获取列信息
        /// </summary>
        public override Field[] GetFields()
        {
            return new Field[] {
                _.no,
                _.date,
                _.name,
                _.start,
                _.end,
                _.reason,
                _.subreason,
            };
        }
        /// <summary>
        /// 获取值信息
        /// </summary>
        public override object[] GetValues()
        {
            return new object[] {
                this._no,
                this._date,
                this._name,
                this._start,
                this._end,
                this._reason,
                this._subreason,
            };
        }
        /// <summary>
        /// 是否是v1.10.5.6及以上版本实体。
        /// </summary>
        /// <returns></returns>
        public override bool V1_10_5_6_Plus()
        {
            return true;
        }
        #endregion

        #region _Field
        /// <summary>
        /// 字段信息
        /// </summary>
        public class _
        {
            /// <summary>
            /// * 
            /// </summary>
            public readonly static Field All = new Field("*", "oa");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field no = new Field("no", "oa", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field date = new Field("date", "oa", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field name = new Field("name", "oa", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field start = new Field("start", "oa", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field end = new Field("end", "oa", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field reason = new Field("reason", "oa", "");
            /// <summary>
			/// 
			/// </summary>
			public readonly static Field subreason = new Field("subreason", "oa", "");
        }
        #endregion
    }

}