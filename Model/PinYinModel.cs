/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：0fee097d-6b71-41e4-9349-376ff10d1382
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：Wenli.IEM.Model
 * 类名称：PinYin
 * 创建时间：2017/3/6 10:25:07
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wenli.IEM.Model
{
    public class PinYinModel
    {
        public string PinYin
        {
            get;set;
        }

        public string HanZi
        {
            get;set;
        }

        public double Score
        {
            get;set;
        }
    }
}
