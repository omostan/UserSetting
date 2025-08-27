using MvvmHelpers;

namespace UserSetting.WPF.ViewModels;

internal class ViewModelBase : BaseViewModel, IDisposable
{
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

