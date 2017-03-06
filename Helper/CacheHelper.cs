/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：8ebc884b-ee5f-45de-8638-c054b832e0ce
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：Wenli.IEM
 * 类名称：CacheHelper
 * 创建时间：2017/3/3 16:18:14
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Windows.Forms;

namespace Wenli.IEM.Helper
{
    public static class CacheHelper
    {
        static MemoryCache _wubiCache = new MemoryCache("wubi");

        static MemoryCache _pinyinCache = new MemoryCache("pinyin");

        static CacheHelper()
        {
            var path = Application.StartupPath + "\\Win32\\world.dll";
            var arr = File.ReadAllLines(path);
            foreach (string item in arr)
            {
                var key = item.Substring(0, item.IndexOf(" "));
                var value = item.Substring(item.IndexOf(" ") + 1);
                _wubiCache.Add(key, (object)value, DateTimeOffset.MaxValue);
            }

            //

            path = Application.StartupPath + "\\Win32\\pinyin.dll";
            arr = File.ReadAllLines(path);
            foreach (string item in arr)
            {
                var key = item.Substring(0, item.IndexOf(" "));
                var value = item.Substring(item.IndexOf(" ") + 1);
                _pinyinCache.Add(key, (object)value, DateTimeOffset.MaxValue);
            }
        }

        public static string[] Get(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var str = string.Empty;

                try
                {
                    if (_wubiCache.Contains(key))
                        str = _wubiCache[key].ToString();
                }
                catch { }
                try
                {
                    if (_pinyinCache.Contains(key))
                        str += " " + _pinyinCache[key].ToString();
                }
                catch { }

                if (!string.IsNullOrEmpty(str))
                {
                    var arr = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr[i].IndexOf("*") > -1)
                        {
                            arr[i] = arr[i].Substring(0, arr[i].IndexOf("*"));
                        }
                    }
                    return arr;
                }
            }

            return null;
        }


        public static bool ContainsKey(string key)
        {
            if (_wubiCache.Contains(key))
                return true;
            if (_pinyinCache.Contains(key))
                return true;
            return false;
        }

        public static void Clear()
        {
            _wubiCache.Dispose();
            GC.Collect(-1);
        }
    }
}
