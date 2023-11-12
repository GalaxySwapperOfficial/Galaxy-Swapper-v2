using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Interface
    {
        public class ThicknessAnim
        {
            public FrameworkElement Element { get; set; }
            public ThicknessAnimation ElementAnim { get; set; }
        }

        public class BaseAnim
        {
            public DependencyObject Element { get; set; }
            public DoubleAnimationBase ElementAnim { get; set; }
            public PropertyPath Property { get; set; }
        }

        public static Storyboard SetThicknessAnimations(params ThicknessAnim[] elements)
        {
            var storyboard = new Storyboard();

            foreach (var element in elements)
            {
                Storyboard.SetTarget(element.ElementAnim, element.Element);
                Storyboard.SetTargetProperty(element.ElementAnim, new PropertyPath(FrameworkElement.MarginProperty));
                storyboard.Children.Add(element.ElementAnim);
            }

            return storyboard;
        }

        public static Storyboard SetElementAnimations(params BaseAnim[] elements)
        {
            var storyboard = new Storyboard();

            foreach (var element in elements)
            {
                Storyboard.SetTarget(element.ElementAnim, element.Element);
                Storyboard.SetTargetProperty(element.ElementAnim, element.Property);
                storyboard.Children.Add(element.ElementAnim);
            }

            return storyboard;
        }

        public static void SetBlur(params Decorator[] decorators)
        {
            foreach (var decorator in decorators)
            {
                decorator.Effect = new BlurEffect { Radius = 10 };
            }
        }
    }
}
