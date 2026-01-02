using MetaFrm.MVVM;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace MetaFrm.Razor.Browser.ViewModels
{
    /// <summary>
    /// MainLayoutViewModel
    /// </summary>
    public partial class MainLayoutViewModel : BaseViewModel
    {
        /// <summary>
        /// NavMenuType
        /// </summary>
        public static Type? NavMenuType { get; set; }
        /// <summary>
        /// NavMenu
        /// </summary>
        public DynamicComponent? NavMenu { get; set; }
        /// <summary>
        /// CurrentPageType
        /// </summary>
        public Type? CurrentPageType { get; set; }
        /// <summary>
        /// CurrentPageHomeType
        /// </summary>
        public static Type? CurrentPageHomeType { get; set; }
        /// <summary>
        /// CurrentPage
        /// </summary>
        public DynamicComponent? CurrentPage { get; set; }
        /// <summary>
        /// CurrentPagePara
        /// </summary>
        public Dictionary<string, object>? CurrentPagePara { get; set; }
        /// <summary>
        /// ToastPara
        /// </summary>
        public Dictionary<string, object>? ToastPara { get; set; }
        /// <summary>
        /// ToastType
        /// </summary>
        public static Type? ToastType { get; set; }
        /// <summary>
        /// ModalPara
        /// </summary>
        public Dictionary<string, object>? ModalPara { get; set; }
        /// <summary>
        /// ModalType
        /// </summary>
        public static Type? ModalType { get; set; }
        /// <summary>
        /// TmpBrowserType
        /// </summary>
        public Type? TmpBrowserType { get; set; }
        /// <summary>
        /// HomeType
        /// </summary>
        public Type? HomeType { get; set; }

        /// <summary>
        /// MainLayoutViewModel
        /// </summary>
        public MainLayoutViewModel() { }

        /// <summary>
        /// MainLayoutViewModel
        /// </summary>
        /// <param name="localization"></param>
        public MainLayoutViewModel(IStringLocalizer? localization) : base(localization)
        {
            this.Title = "Home";

            if (NavMenuType == null)
                NavMenuType = Factory.LoadType(this.GetAttribute("NavMenu"), null, true);
            //NavMenuType = typeof(MetaFrm.Razor.NavMenu);

            if (CurrentPageHomeType == null)
                CurrentPageHomeType = Factory.LoadType(this.GetAttribute("Home"), null, true);

            this.CurrentPageType = CurrentPageHomeType;
            //this.CurrentPageType = typeof(MetaFrm.Razor.Home);

            if (ToastType == null)
                ToastType = Factory.LoadType(this.GetAttribute("Toast"), null, true);
            //ToastType = typeof(MetaFrm.Razor.Alert.Toast);

            if (ModalType == null)
                ModalType = Factory.LoadType(this.GetAttribute("Modal"), null, true);
            //ModalType = typeof(MetaFrm.Razor.Alert.Modal);
        }
    }
}