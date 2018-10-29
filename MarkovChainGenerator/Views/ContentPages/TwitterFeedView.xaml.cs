using MarkovChainGenerator.ViewModels;
using Xamarin.Forms.Xaml;

namespace MarkovChainGenerator.Views.ContentPages
{
    public class TwitterFeedViewBase : ViewPageBase<TwitterFeedViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TwitterFeedView : TwitterFeedViewBase
	{
		public TwitterFeedView ()
		{
			InitializeComponent ();
		}
	}
}