﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace myALPR1.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.1.192")]
        public string ACTiServerIP {
            get {
                return ((string)(this["ACTiServerIP"]));
            }
            set {
                this["ACTiServerIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Admin")]
        public string ACTiUser {
            get {
                return ((string)(this["ACTiUser"]));
            }
            set {
                this["ACTiUser"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("123456")]
        public string ACTiPAssword {
            get {
                return ((string)(this["ACTiPAssword"]));
            }
            set {
                this["ACTiPAssword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DBConnectionString {
            get {
                return ((string)(this["DBConnectionString"]));
            }
            set {
                this["DBConnectionString"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string COMPortName {
            get {
                return ((string)(this["COMPortName"]));
            }
            set {
                this["COMPortName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("9600")]
        public int COMPortBaudRate {
            get {
                return ((int)(this["COMPortBaudRate"]));
            }
            set {
                this["COMPortBaudRate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("None")]
        public global::System.IO.Ports.Parity COMPortParity {
            get {
                return ((global::System.IO.Ports.Parity)(this["COMPortParity"]));
            }
            set {
                this["COMPortParity"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8")]
        public int COMPortDataBits {
            get {
                return ((int)(this["COMPortDataBits"]));
            }
            set {
                this["COMPortDataBits"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Two")]
        public global::System.IO.Ports.StopBits COMPortStopBits {
            get {
                return ((global::System.IO.Ports.StopBits)(this["COMPortStopBits"]));
            }
            set {
                this["COMPortStopBits"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DBIDUser {
            get {
                return ((string)(this["DBIDUser"]));
            }
            set {
                this["DBIDUser"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DBPassword {
            get {
                return ((string)(this["DBPassword"]));
            }
            set {
                this["DBPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int ACTiServerMediaType {
            get {
                return ((int)(this["ACTiServerMediaType"]));
            }
            set {
                this["ACTiServerMediaType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int ACTiServerID {
            get {
                return ((int)(this["ACTiServerID"]));
            }
            set {
                this["ACTiServerID"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int ACTiServerMediaChannel {
            get {
                return ((int)(this["ACTiServerMediaChannel"]));
            }
            set {
                this["ACTiServerMediaChannel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("80")]
        public int ACTiServerHttpPort {
            get {
                return ((int)(this["ACTiServerHttpPort"]));
            }
            set {
                this["ACTiServerHttpPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6000")]
        public int ACTiServerRegisterPort {
            get {
                return ((int)(this["ACTiServerRegisterPort"]));
            }
            set {
                this["ACTiServerRegisterPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6001")]
        public int ACTiServerControlPort {
            get {
                return ((int)(this["ACTiServerControlPort"]));
            }
            set {
                this["ACTiServerControlPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("6002")]
        public int ACTiServerStreamingPort {
            get {
                return ((int)(this["ACTiServerStreamingPort"]));
            }
            set {
                this["ACTiServerStreamingPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5000")]
        public int ACTiServerMulticastPort {
            get {
                return ((int)(this["ACTiServerMulticastPort"]));
            }
            set {
                this["ACTiServerMulticastPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("9600")]
        public int ACTiServerBaudRate {
            get {
                return ((int)(this["ACTiServerBaudRate"]));
            }
            set {
                this["ACTiServerBaudRate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ACTiServerMulticastIP {
            get {
                return ((string)(this["ACTiServerMulticastIP"]));
            }
            set {
                this["ACTiServerMulticastIP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4600")]
        public int MinCorrelation {
            get {
                return ((int)(this["MinCorrelation"]));
            }
            set {
                this["MinCorrelation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public int FramesSleepTime {
            get {
                return ((int)(this["FramesSleepTime"]));
            }
            set {
                this["FramesSleepTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoConnectACTi {
            get {
                return ((bool)(this["AutoConnectACTi"]));
            }
            set {
                this["AutoConnectACTi"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoOpenCOMPort {
            get {
                return ((bool)(this["AutoOpenCOMPort"]));
            }
            set {
                this["AutoOpenCOMPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseEdgeBased {
            get {
                return ((bool)(this["UseEdgeBased"]));
            }
            set {
                this["UseEdgeBased"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseVerticalDetection {
            get {
                return ((bool)(this["UseVerticalDetection"]));
            }
            set {
                this["UseVerticalDetection"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoStartDetection {
            get {
                return ((bool)(this["AutoStartDetection"]));
            }
            set {
                this["AutoStartDetection"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SaveCapturedPlates {
            get {
                return ((bool)(this["SaveCapturedPlates"]));
            }
            set {
                this["SaveCapturedPlates"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DOutput1 {
            get {
                return ((bool)(this["DOutput1"]));
            }
            set {
                this["DOutput1"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DOutput2 {
            get {
                return ((bool)(this["DOutput2"]));
            }
            set {
                this["DOutput2"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RTS {
            get {
                return ((bool)(this["RTS"]));
            }
            set {
                this["RTS"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DTR {
            get {
                return ((bool)(this["DTR"]));
            }
            set {
                this["DTR"] = value;
            }
        }
    }
}
