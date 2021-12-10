using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy_Swapper_v2.Workspace.Other
{
    public static class Colors
    {
        public static Color MHex()
        {
            return ColorTranslator.FromHtml(SettingsController.SettingsReturn("MHex"));
        }
        public static Color SHex()
        {
            return ColorTranslator.FromHtml(SettingsController.SettingsReturn("SHex"));
        }
        public static Color HHex()
        {
            return ColorTranslator.FromHtml(SettingsController.SettingsReturn("HHex"));
        }
        public static Color TextHex()
        {
            return ColorTranslator.FromHtml(SettingsController.SettingsReturn("TextHex"));
        }
        public static Color SecTextHex()
        {
            return ColorTranslator.FromHtml(SettingsController.SettingsReturn("SecTextHex"));
        }
        public static Color ButtonHex()
        {
            return ColorTranslator.FromHtml(SettingsController.SettingsReturn("ButtonHex"));
        }
    }
}
