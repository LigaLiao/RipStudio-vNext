using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using RipStudio.Pages.Settings;

namespace RipStudio
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mySingleInstanceMutex = new Mutex(true, "RipStudio");
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!mySingleInstanceMutex.WaitOne(0, false))
            {
                Application.Current.Shutdown();
                return;
            }
            else
            {
                base.OnStartup(e);
            }
        }

        public ListView LV = new ListView() ;
        
        public RipStudio.Pages.ScriptCreater.Main avs_Main = null;
        public RipStudio.Pages.ScriptCreater.Cut avs_Cut = null;

        public RipStudio.Pages.ScriptCreater.AudioContent.AudioMain AudioMain = null;

        public RipStudio.Pages.ScriptCreater.VideoContent.CropResize VideoMain = null;
        public RipStudio.Pages.ScriptCreater.VideoContent.ColorPG VideoColor = null;
        public RipStudio.Pages.ScriptCreater.VideoContent.TIVTC VideoTIVTC = null;
        public RipStudio.Pages.ScriptCreater.VideoContent.De VideoDe = null;
        public RipStudio.Pages.ScriptCreater.VideoContent.Subtitle VideoSubtitle = null;
        public RipStudio.Pages.ScriptCreater.VideoContent.Other VideoOther = null;

        public SettingsPage SP = null;
    }
}
