using MetaFrm.MVVM;
using Microsoft.Extensions.Localization;

namespace MetaFrm.Razor.Browser.ViewModels
{
    /// <summary>
    /// ErrorViewModel
    /// </summary>
    public partial class ErrorViewModel : BaseViewModel
    {
        /// <summary>
        /// Message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// ExceptionToString
        /// </summary>
        public string? ExceptionToString { get; set; }

        /// <summary>
        /// IsDebug
        /// </summary>
        public bool IsDebug { get; set; }

        /// <summary>
        /// ErrorViewModel
        /// </summary>
        public ErrorViewModel() : base() { }

        /// <summary>
        /// ErrorViewModel
        /// </summary>
        /// <param name="localization"></param>
        public ErrorViewModel(IStringLocalizer? localization) : base(localization) { }
    }
}