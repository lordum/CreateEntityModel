﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CreateEntityModel.AddDatabase
{
    public class EntityBuildModel
    {
        /// <summary>
        /// 引用类
        /// </summary>
        public List<string> Using { get; set; }
        /// <summary>
        /// 命名空间的名字
        /// </summary>
        public string NamespaceName { get; set; } = "GISQ.NJQ.DataEntity";
        /// <summary>
        /// 使用委托自定义类名规则
        /// </summary>
        public Func<string,string> CustomClassName { get; set; }
    }
}
