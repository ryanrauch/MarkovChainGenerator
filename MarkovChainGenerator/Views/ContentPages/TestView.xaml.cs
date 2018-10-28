using MarkovChainGenerator.ViewModels;
using Xamarin.Forms.Xaml;

namespace MarkovChainGenerator.Views.ContentPages
{
    public class TestViewBase : ViewPageBase<TestViewModel> {}

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestView : TestViewBase
    {
        public TestView()
        {
            InitializeComponent();
        }
    }
}
