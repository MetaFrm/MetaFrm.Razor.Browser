﻿using MetaFrm.MVVM;
using Microsoft.AspNetCore.Components;

namespace MetaFrm.Razor.Browser.ViewModels
{
    public partial class MainLayoutViewModel : BaseViewModel
    {
        public Type? NavMenuType { get; set; }
        public DynamicComponent? NavMenu { get; set; }

        public Type? CurrentPageType { get; set; }
        public DynamicComponent? CurrentPage { get; set; }
        public Dictionary<string, object>? CurrentPagePara { get; set; }

        public Dictionary<string, object>? ToastPara { get; set; }
        public Type? ToastType { get; set; }

        public Dictionary<string, object>? ModalPara { get; set; }
        public Type? ModalType { get; set; }

        public Type? TmpBrowserType { get; set; }

        public Type? HomeType { get; set; }

        public MainLayoutViewModel()
        {
            this.Title = "Home";

            this.NavMenuType = Factory.LoadTypeFromServiceAttribute("NavMenu");
            //this.NavMenuType = typeof(MetaFrm.Razor.Menu.NavMenu);

            this.CurrentPageType = Factory.LoadTypeFromServiceAttribute("Home");
            //this.CurrentPageType = typeof(MetaFrm.Razor.Home);

            this.ToastType = Factory.LoadTypeFromServiceAttribute("Toast");
            //this.ToastType = typeof(MetaFrm.Razor.Alert.Toast);

            this.ModalType = Factory.LoadTypeFromServiceAttribute("Modal");
            //this.ModalType = typeof(MetaFrm.Razor.Alert.Modal);
        }
    }
}