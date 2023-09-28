using Galaxy_Swapper_v2.Workspace.Utilities;
using System.Windows.Media;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class Colors
    {
        public static readonly Color Main = "#090B0E".HexToColor();
        public static readonly Brush Main_Brush = "#090B0E".HexToBrush();

        public static readonly Color Accent = "#060709".HexToColor();
        public static readonly Brush Accent_Brush = "#060709".HexToBrush();

        public static readonly Color External = "#0D1118".HexToColor();
        public static readonly Brush External_Brush = "#0D1118".HexToBrush();

        public static readonly Color Blue = "#0091E6".HexToColor();
        public static readonly Brush Blue_Brush = "#0091E6".HexToBrush();

        public static readonly Color ControlHover = "#0D1118".HexToColor();
        public static readonly Brush ControlHover_Brush = "#0D1118".HexToBrush();

        public static readonly SolidColorBrush Text = "#D3D3D6".HexToBrush();
        public static readonly SolidColorBrush Text2 = "#454B56".HexToBrush();
        public static readonly SolidColorBrush Red = "#D30000".HexToBrush();
        public static readonly SolidColorBrush Yellow = "#F7F700".HexToBrush();
    }
}