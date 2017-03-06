/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：256ed368-b1f5-4eb6-b012-ee38b8188bb0
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：Wenli.IEM.Helper
 * 类名称：Generate
 * 创建时间：2017/3/6 10:32:14
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wenli.IEM.Model;

namespace Wenli.IEM.Helper
{
    public static class Generate
    {
        static Dictionary<string, List<PinYinModel>> dic = new Dictionary<string, List<PinYinModel>>();
        public static void GenerateFile()
        {
            var path = Application.StartupPath + "\\汉字拼音对照表.txt";
            dic = new Dictionary<string, List<PinYinModel>>();
            var arr = File.ReadAllLines(path);
            foreach (var item in arr)
            {
                var itemArr = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                string key = string.Empty;
                string val = string.Empty;

                List<PinYinModel> vals = new List<PinYinModel>();

                for (int i = 0; i < itemArr.Length; i++)
                {
                    if (i == 0)
                    {
                        val = itemArr[0];
                    }
                    else
                    {
                        key = itemArr[i];
                        if (!dic.ContainsKey(key))
                        {
                            var list = new List<PinYinModel>();
                            list.Add(new PinYinModel()
                            {
                                PinYin = key,
                                HanZi = val,
                                Score = 0
                            });
                            dic.Add(key, list);
                        }
                        else
                        {
                            var list = dic[key];
                            list.Add(new PinYinModel()
                            {
                                PinYin = key,
                                HanZi = val,
                                Score = list.Count + 1
                            });
                            dic[key] = list;
                        }
                    }
                }
            }
            List<string> lines = new List<string>();
            string msg = string.Empty;
            foreach (var item in dic)
            {
                msg = item.Key;
                foreach (var sitem in item.Value)
                {
                    msg += " " + sitem.HanZi + "*" + sitem.Score;
                }
                lines.Add(msg);
            }
            File.AppendAllLines(Application.StartupPath + "\\Win32\\pinyin.dll", lines);
        }
    }
}
