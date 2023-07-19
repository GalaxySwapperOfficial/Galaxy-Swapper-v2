using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Interface
    {
        #region Elements
        public class ThicknessAnim
        {
            public dynamic Element { get; set; } = default!;
            public ThicknessAnimation ElementAnim { get; set; } = default!;
        }
        public class BaseAnim
        {
            public dynamic Element { get; set; } = default!;
            public DoubleAnimationBase ElementAnim { get; set; } = default!;
            public PropertyPath Property { get; set; } = default!;
        }
        #endregion

        public static Storyboard SetThicknessAnimations(params ThicknessAnim[] Elements)
        {
            var storyboard = new Storyboard();

            foreach (var Element in Elements)
            {
                var ElementAnim = Element.ElementAnim;

                ElementAnim.SetValue(Storyboard.TargetProperty, Element.Element);
                Storyboard.SetTargetProperty(ElementAnim, new PropertyPath(FrameworkElement.MarginProperty));

                storyboard.Children.Add(ElementAnim);
            }

            return storyboard;
        }

        public static Storyboard SetElementAnimations(params BaseAnim[] Elements)
        {
            var storyboard = new Storyboard();

            foreach (var Element in Elements)
            {
                var ElementAnim = Element.ElementAnim;

                ElementAnim.SetValue(Storyboard.TargetProperty, Element.Element);
                Storyboard.SetTargetProperty(ElementAnim, Element.Property);

                storyboard.Children.Add(ElementAnim);
            }

            return storyboard;
        }

        public static void SetBlur(params Decorator[] decorators)
        {
            foreach (var decorator in decorators)
            {
                var Blur = new BlurEffect() { Radius = 10 };
                decorator.Effect = Blur;
            }
        }
    }
}