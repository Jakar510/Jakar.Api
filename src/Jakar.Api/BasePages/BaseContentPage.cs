using Jakar.Extensions;


#pragma warning disable 1591

#nullable enable
namespace Jakar.Api.BasePages
{
	public abstract class BaseContentPage : OrientationContentPage
	{
		protected BaseContentPage() : base() { }
	}



	public abstract class BaseContentPage<TViewModel> : OrientationContentPage where TViewModel : BaseViewModel<BaseContentPage>, new()
	{
		public TViewModel ViewModel { get; protected set; }


		protected BaseContentPage() : base() => ViewModel = InstanceCreator<TViewModel>.Create(this);
	}
}
