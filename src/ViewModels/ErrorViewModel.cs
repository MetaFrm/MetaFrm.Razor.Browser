using MetaFrm.MVVM;

namespace MetaFrm.Razor.Browser.ViewModels
{
    public partial class ErrorViewModel : BaseViewModel
    {
        public string? Message { get; set; }

        public string? ExceptionToString { get; set; }

        public bool IsDebug { get; set; }
    }
}