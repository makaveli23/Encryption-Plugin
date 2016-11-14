// Compiler options:  /unsafe /optimize /debug- /target:library /out:"C:\Users\micha\Desktop\Odszyfrowanie PO.dll"
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

[assembly: AssemblyTitle("Odszyfrowanie PO Plugin for Paint.NET")]
[assembly: AssemblyDescription("Odszyfrowanie")]
[assembly: AssemblyConfiguration("odszyfrowanie")]
[assembly: AssemblyCompany("Michał Migdałek Adrian Markiewicz")]
[assembly: AssemblyProduct("Odszyfrowanie PO")]
[assembly: AssemblyCopyright("Copyright © Michał Migdałek Adrian Markiewicz")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.*")]

namespace OdszyfrowaniePOEffect
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

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Odszyfrowanie")]
    public class OdszyfrowaniePOEffectPlugin : PropertyBasedEffect
    {
        public static string StaticName
        {
            get
            {
                return "Odszyfrowanie";
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

        public OdszyfrowaniePOEffectPlugin()
            : base(StaticName, StaticIcon, SubmenuName, EffectFlags.None)
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            return new PropertyCollection(props);
        }
        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Render(dstArgs.Surface, srcArgs.Surface, srcArgs.Bounds);
        }
        protected override unsafe void OnRender(Rectangle[] rois, int startIndex, int length)
        {
            return;
        }

        #region User Entered Code
        // Name:
        // Submenu:
        // Author:
        // Title:
        // Version:
        // Desc:
        // Keywords:
        // URL:
        // Help:

        private static string encryptedText;
        void Render(Surface dst, Surface src, Rectangle rect)
        {
            var width = rect.Width;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int i = 0;
            do
            {
                var pixel0 = src[(0 + i) % width, (0 + i) / width];
                var pixel1 = src[(1 + i) % width, (1 + i) / width];
                var pixel2 = src[(2 + i) % width, (2 + i) / width];
                var pixel3 = src[(3 + i) % width, (3 + i) / width];

                char x = (char)(((pixel0.R & 3) << 6) | ((pixel1.R & 3) << 4) |
                ((pixel2.R & 3) << 2) | ((pixel3.R & 3)));
                if (x != 0)
                    sb.Append(x);
                else break;
                i += 4;
            } while (i < rect.Height * width);
            encryptedText = sb.ToString();
            Thread t = new Thread(new ThreadStart(SaveToFile));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

        }
        public static void SaveToFile()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "EncryptedText.txt";
            save.Filter = "Plik tekstowy | *.txt";
            if (save.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(save.OpenFile());
                writer.Write(encryptedText);

                writer.Dispose();
                writer.Close();
            }
        }
       

        #endregion
    }
}
