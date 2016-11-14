// Compiler options:  /unsafe /optimize /debug- /target:library /out:"C:\Users\micha\Desktop\Zaszyfrowanie PO.dll"
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Drawing.Text;
using System.Windows.Forms;
using System.IO.Compression;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Registry = Microsoft.Win32.Registry;
using RegistryKey = Microsoft.Win32.RegistryKey;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.AppModel;
using PaintDotNet.IndirectUI;
using PaintDotNet.Collections;
using PaintDotNet.PropertySystem;
using IntSliderControl = System.Int32;
using CheckboxControl = System.Boolean;
using ColorWheelControl = PaintDotNet.ColorBgra;
using AngleControl = System.Double;
using PanSliderControl = PaintDotNet.Pair<double, double>;
using TextboxControl = System.String;
using DoubleSliderControl = System.Double;
using ListBoxControl = System.Byte;
using RadioButtonControl = System.Byte;
using ReseedButtonControl = System.Byte;
using MultiLineTextboxControl = System.String;
using RollControl = System.Tuple<double, double, double>;

[assembly: AssemblyTitle("Zaszyfrowanie PO Plugin for Paint.NET")]
[assembly: AssemblyDescription("Zaszyfrowanie PO")]
[assembly: AssemblyConfiguration("zaszyfrowanie po")]
[assembly: AssemblyCompany("Michał Migdałek Adrian Markiewicz")]
[assembly: AssemblyProduct("Zaszyfrowanie PO")]
[assembly: AssemblyCopyright("Copyright © Michał Migdałek Adrian Markiewicz")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.*")]

namespace ZaszyfrowaniePOEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author
        {
            get
            {
                return ((AssemblyCopyrightAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
            }
        }
        public string Copyright
        {
            get
            {
                return ((AssemblyDescriptionAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
            }
        }

        public string DisplayName
        {
            get
            {
                return ((AssemblyProductAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
            }
        }

        public Version Version
        {
            get
            {
                return base.GetType().Assembly.GetName().Version;
            }
        }

        public Uri WebsiteUri
        {
            get
            {
                return new Uri("http://www.getpaint.net/redirect/plugins.html");
            }
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Zaszyfrowanie")]
    public class ZaszyfrowaniePOEffectPlugin : PropertyBasedEffect
    {
        public static string StaticName
        {
            get
            {
                return "Zaszyfrowanie";
            }
        }

        public static Image StaticIcon
        {
            get
            {
                return null;
            }
        }

        public static string SubmenuName
        {
            get
            {
                return "Szyfrowanie";
            }
        }

        public ZaszyfrowaniePOEffectPlugin()
            : base(StaticName, StaticIcon, SubmenuName, EffectFlags.Configurable)
        {
        }

        public enum PropertyNames
        {
            Amount1
        }


        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new StringProperty(PropertyNames.Amount1, "", 32767));

            return new PropertyCollection(props);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            ControlInfo configUI = CreateDefaultConfigUI(props);

            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.DisplayName, "Input");
            configUI.SetPropertyControlType(PropertyNames.Amount1, PropertyControlType.TextBox);
            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.Multiline, true);

            return configUI;
        }

        

        protected override void OnCustomizeConfigUIWindowProperties(PropertyCollection props)
        {
            // Change the effect's window title
            props[ControlInfoPropertyNames.WindowTitle].Value = "Szyfrowanie";
            // Add help button to effect UI
            props[ControlInfoPropertyNames.WindowHelpContentType].Value = WindowHelpContentType.PlainText;
            props[ControlInfoPropertyNames.WindowHelpContent].Value = "Zaszyfrowanie PO v1,0\nCopyright ©2016 by US\nAll rights reserved.";
            base.OnCustomizeConfigUIWindowProperties(props);
        }

        protected override unsafe void OnRender(Rectangle[] rois, int startIndex, int length)
        {
            return;
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Amount1 = newToken.GetProperty<StringProperty>(PropertyNames.Amount1).Value;

            Render(dstArgs.Surface, srcArgs.Surface, srcArgs.Bounds);
        }

        #region User Entered Code
        #region UICode
        MultiLineTextboxControl Amount1 = ""; // [1,32767] Input
        #endregion

        void Render(Surface dst, Surface src, Rectangle rect)
        {
            var msg = System.Text.Encoding.UTF8.GetBytes(Amount1);
            if (msg.Length == 0 || msg.Last() != 0)
            {
                var msg2 = msg.ToList();
                msg2.Add(0);
                msg = msg2.ToArray();
            }
            var pixelCount = msg.Length * 4;
            var width = rect.Right;
            int character = 0;
            ColorBgra CurrentPixel;
            for (int y = 0; y < pixelCount; y += 4)
            {
                if (IsCancelRequested) return;
                if (character >= msg.Length) break;
                for (int x = 0; x < 4; x++)
                {
                    CurrentPixel = src[(x + y) % width, (x + y) / width];
                    var pixelR = (byte)(((CurrentPixel.R >> 2) << 2) |
                            ((msg[character] >> ((3 - x) * 2))) & 3);
                    CurrentPixel.R = pixelR;
                    dst[(x + y) % width, (x + y) / width] = CurrentPixel;
                }
                character++;
            }
        }
        #endregion
    }
}
