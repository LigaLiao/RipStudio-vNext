﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RipStudio.Pages.ScriptCreater.AudioContent
{
    /// <summary>
    /// Interaction logic for ScriptCreaterA.xaml
    /// </summary>
    public partial class AudioMain : UserControl
    {
        public AudioMain()
        {
            InitializeComponent();
            ((App)(Application.Current)).AudioMain = this;
        }
    }
}