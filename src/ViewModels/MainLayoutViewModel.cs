using MetaFrm.MVVM;
using Microsoft.AspNetCore.Components;

namespace MetaFrm.Razor.Browser.ViewModels
{
    public partial class MainLayoutViewModel : BaseViewModel
    {
        public static Type? NavMenuType { get; set; }
        public DynamicComponent? NavMenu { get; set; }

        public Type? CurrentPageType { get; set; }
        public DynamicComponent? CurrentPage { get; set; }
        public Dictionary<string, object>? CurrentPagePara { get; set; }

        public Dictionary<string, object>? ToastPara { get; set; }
        public static Type? ToastType { get; set; }

        public Dictionary<string, object>? ModalPara { get; set; }
        public static Type? ModalType { get; set; }

        public Type? TmpBrowserType { get; set; }

        public Type? HomeType { get; set; }

        public MainLayoutViewModel()
        {
            this.Title = "Home";

            if (NavMenuType == null)
                NavMenuType = Factory.LoadType(this.GetAttribute("NavMenu"), null, true);
            //NavMenuType = typeof(MetaFrm.Razor.Menu.NavMenu);

            this.CurrentPageType = Factory.LoadType(this.GetAttribute("Home"), null, true);
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