using MarkovChainGenerator.ViewModels;
using Xamarin.Forms.Xaml;

namespace MarkovChainGenerator.Views.ContentPages
{
    public class InitialViewBase : ViewPageBase<InitialViewModel> { }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InitialView : InitialViewBase
    {
        public InitialView()
        {
            InitializeComponent();
        }
    }
}
