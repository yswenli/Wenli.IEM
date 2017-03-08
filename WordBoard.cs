using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Wenli.IEM.Helper;

namespace Wenli.IEM
{
    public partial class WordBoard : CCWin.Skin_Mac
    {
        public WordBoard()
        {
            InitializeComponent();
        }

        private void WordBoard_Load(object sender, EventArgs e)
        {
            Program.keyBordHook.KeyUpEvent += KeyBordHook_KeyUpEvent;
            Program.keyBordHook.OnSpaced += KeyBordHook_OnSpaced;
            Program.keyBordHook.OnBacked += KeyBordHook_OnBacked;
        }



        string keys = "";

        private void KeyBordHook_KeyUpEvent(object sender, KeyEventArgs e)
        {
            keys += e.KeyCode.ToString().ToLower();
            this.ShowCharatar();
        }

        private void KeyBordHook_OnSpaced(int choose)
        {
            try
            {
                if (CacheHelper.ContainsKey(keys))
                {
                    if (choose > 0)
                    {
                        choose = choose - 1;
                    }

                    Program.keyBordHook.Send(CacheHelper.Get(keys)[choose]);
                    label1.Text = "";
                    this.listView1.Clear();
                }
            }
            catch
            {

            }
            keys = "";
        }

        private void KeyBordHook_OnBacked()
        {
            if (!string.IsNullOrEmpty(keys))
            {
                keys = keys.Substring(0, keys.Length - 1);
            }
            this.ShowCharatar();
        }



        private void ShowCharatar()
        {
            this.listView1.BeginInvoke(new Action(() =>
            {
                label1.Text = keys;

                try
                {
                    this.listView1.Items.Clear();
                    var arr = CacheHelper.Get(keys);
                    if (arr != null)
                        for (int i = 0; i < (arr.Length > 10 ? 9 : arr.Length); i++)
                        {
                            this.listView1.Items.Add((i + 1) + "、" + arr[i]);
                        }
                }
                catch
                {
                    label1.Text = keys = "";
                }
            }));
        }

    }
}
