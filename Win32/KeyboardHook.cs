/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：c5508f2e-c56d-4015-b3f3-de9866eb2a0a
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：Wenli.IEM.Win32
 * 类名称：KeyBordHook
 * 创建时间：2017/2/28 15:43:25
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wenli.IEM.Win32
{
    /// <summary>
    /// 键盘钩子

    /// </summary>
    class KeyboardHook
    {

        #region win32
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        static int hKeyboardHook = 0; //声明键盘钩子处理的初始值
        public const int WH_KEYBOARD_LL = 13;   //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13
        HookProc KeyboardHookProcedure; //声明KeyboardHookProcedure作为HookProc类型




        //键盘结构
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode;  //定一个虚拟键码。该代码必须有一个价值的范围1至254
            public int scanCode; // 指定的硬件扫描码的关键
            public int flags;  // 键标志
            public int time; // 指定的时间戳记的这个讯息
            public int dwExtraInfo; // 指定额外信息相关的信息
        }


        //使用此功能，安装了一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


        //调用此函数卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);


        //使用此功能，通过信息钩子继续下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);


        // 取得当前线程编号（线程钩子需要用到）
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();


        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);




        private const int WM_KEYDOWN = 0x100;//KEYDOWN
        private const int WM_KEYUP = 0x101;//KEYUP
        private const int WM_SYSKEYDOWN = 0x104;//SYSKEYDOWN
        private const int WM_SYSKEYUP = 0x105;//SYSKEYUP

        //ToAscii职能的转换指定的虚拟键码和键盘状态的相应字符或字符
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, //[in] 指定虚拟关键代码进行翻译。
                                         int uScanCode, // [in] 指定的硬件扫描码的关键须翻译成英文。高阶位的这个值设定的关键，如果是（不压）
                                         byte[] lpbKeyState, // [in] 指针，以256字节数组，包含当前键盘的状态。每个元素（字节）的数组包含状态的一个关键。如果高阶位的字节是一套，关键是下跌（按下）。在低比特，如果设置表明，关键是对切换。在此功能，只有肘位的CAPS LOCK键是相关的。在切换状态的NUM个锁和滚动锁定键被忽略。
                                         byte[] lpwTransKey, // [out] 指针的缓冲区收到翻译字符或字符。
                                         int fuState); // [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise.

        //获取按键的状态
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetAsyncKeyState(int vKey);
        #endregion


        public event KeyEventHandler KeyUpEvent;
        public event Action<int> OnSpaced;
        public event Action OnBacked;
        public event Action<int> OnPaged;

        public void Start()
        {
            // 安装键盘钩子
            if (hKeyboardHook == 0)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);

                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);

                //如果SetWindowsHookEx失败
                if (hKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("安装键盘钩子失败");
                }
            }
        }
        public void Stop()
        {
            bool retKeyboard = true;


            if (hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }

            if (!(retKeyboard))
                throw new Exception("卸载钩子失败！");
        }

        public void Send(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Stop();
                SendKeys.Send("{RIGHT}" + msg);
                Start();
            }
        }

        bool isLocked = true;

        bool isStarted = false;

        /// <summary>
        /// 按键处理
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns>如果返回1，则结束消息，这个消息到此为止，不再传递。如果返回0或调用CallNextHookEx函数则消息出了这个钩子继续往下传递，也就是传给消息真正的接受者</returns>
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 侦听键盘事件
            if (nCode >= 0 && wParam == 0x0100)
            {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                #region 开关
                if (MyKeyboardHookStruct.vkCode == 20 || MyKeyboardHookStruct.vkCode == 160 || MyKeyboardHookStruct.vkCode == 161)
                {
                    isLocked = isLocked ? false : true;
                }
                #endregion


                #region
                if (isLocked)
                {
                    #region page
                    if (MyKeyboardHookStruct.vkCode == 33)
                    {
                        OnPaged(-1);
                    }
                    if (MyKeyboardHookStruct.vkCode == 34)
                    {
                        OnPaged(1);
                    }
                    #endregion

                    if (isStarted && MyKeyboardHookStruct.vkCode >= 48 && MyKeyboardHookStruct.vkCode <= 57)
                    {
                        var c = int.Parse(((char)MyKeyboardHookStruct.vkCode).ToString());
                        OnSpaced(c);
                        isStarted = false;
                        return 1;
                    }
                    if (isStarted && MyKeyboardHookStruct.vkCode == 8)
                    {
                        OnBacked();
                        return 1;
                    }
                    if ((MyKeyboardHookStruct.vkCode >= 65 && MyKeyboardHookStruct.vkCode <= 90) || MyKeyboardHookStruct.vkCode == 32)
                    {
                        if (MyKeyboardHookStruct.vkCode >= 65 && MyKeyboardHookStruct.vkCode <= 90)
                        {
                            Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                            KeyEventArgs e = new KeyEventArgs(keyData);
                            KeyUpEvent(this, e);
                            isStarted = true;
                        }
                        if (MyKeyboardHookStruct.vkCode == 32)
                        {
                            OnSpaced(0);
                            isStarted = false;
                        }
                        return 1;
                    }
                    else
                        return 0;
                }
                #endregion
            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
        ~KeyboardHook()
        {
            Stop();
        }
    }
}


