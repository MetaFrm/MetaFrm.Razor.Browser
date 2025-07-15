using MetaFrm.Razor.Browser.ViewModels;

namespace MetaFrm.Razor.Browser.Shared
{
    public partial class Error
    {
        internal ErrorViewModel ErrorViewModel { get; set; } = new();

        /// <summary>
        /// OnInitializedAsync
        /// </summary>
        /// <returns></returns>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.ErrorViewModel = this.CreateViewModel<ErrorViewModel>();
        }

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