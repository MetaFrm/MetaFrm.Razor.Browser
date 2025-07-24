using MetaFrm.Razor.Browser.ViewModels;

namespace MetaFrm.Razor.Browser.Shared
{
    /// <summary>
    /// Error
    /// </summary>
    public partial class Error
    {
        internal ErrorViewModel ErrorViewModel { get; set; } = new(null);

        /// <summary>
        /// OnInitialized
        /// </summary>
        /// <returns></returns>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.ErrorViewModel = this.CreateViewModel<ErrorViewModel>();
        }

        /// <summary>
        /// OnAfterRender
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                if (this.Parameter != null && this.Parameter is Exception exception)
                {
                    this.ErrorViewModel.Message = exception.Message;
#if DEBUG
                    this.ErrorViewModel.ExceptionToString = exception.ToString();
                    this.ErrorViewModel.IsDebug = true;
#endif
                    this.StateHasChanged();
                }
            }
        }
    }
}