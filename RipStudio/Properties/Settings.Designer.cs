﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34014
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace RipStudio.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FFA4C400")]
        public global::System.Windows.Media.Color AccentColor {
            get {
                return ((global::System.Windows.Media.Color)(this["AccentColor"]));
            }
            set {
                this["AccentColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/FirstFloor.ModernUI;component/Assets/ModernUI.Dark.xaml")]
        public global::System.Uri Theme_Uri {
            get {
                return ((global::System.Uri)(this["Theme_Uri"]));
            }
            set {
                this["Theme_Uri"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SimplifiedChinese")]
        public string Language_DisplayName {
            get {
                return ((string)(this["Language_DisplayName"]));
            }
            set {
                this["Language_DisplayName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Dark")]
        public string Theme_DisplayName {
            get {
                return ((string)(this["Theme_DisplayName"]));
            }
            set {
                this["Theme_DisplayName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/RipStudio;component/Language/SimplifiedChinese.xaml")]
        public global::System.Uri Language_Uri {
            get {
                return ((global::System.Uri)(this["Language_Uri"]));
            }
            set {
                this["Language_Uri"] = value;
            }
        }
    }
}
