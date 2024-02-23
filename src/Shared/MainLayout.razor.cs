using MetaFrm.Alert;
using MetaFrm.Config;
using MetaFrm.Control;
using MetaFrm.Razor.Browser.ViewModels;
using MetaFrm.Maui.Devices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MetaFrm.Razor.Browser.Shared
{
    public partial class MainLayout : IDisposable
    {
        internal MainLayoutViewModel MainLayoutViewModel { get; set; } = Factory.CreateViewModel<MainLayoutViewModel>();

        private bool isFirstLoad = true;

        private readonly List<MetaFrmEventArgs> Navigationqueue = new();

        [Inject]
        protected NavigationManager? Navigation { get; set; }

        [Inject]
        private Maui.Notification.ICloudMessaging? CloudMessaging { get; set; }

        [Inject]
        public IJSRuntime? JSRuntime { get; set; }

        private string DisplayName
        {
            get
            {
                if (this.AuthState != null)
                    return this.AuthState.Nickname();

                return "";
            }
        }

        private string Responsibility
        {
            get
            {
                if (this.AuthState != null)
                    return this.AuthState.UserClaim("Account.RESPONSIBILITY_NAME");

                return "";
            }
        }

        private string DisplayInfo { get; set; } = string.Empty;
        private string ProfileImage { get; set; } = string.Empty;
        private string FooterInfo01 { get; set; } = string.Empty;
        private string FooterInfo02 { get; set; } = string.Empty;
        private string FooterInfo03 { get; set; } = string.Empty;
        private string FooterInfo04 { get; set; } = string.Empty;
        private string Copyright { get; set; } = string.Empty;


        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (this.Navigation != null)
                this.Navigation.LocationChanged += Navigation_LocationChanged;

            try
            {
                this.FooterInfo01 = "MetaFrm.Razor.Browser".GetAttribute("FooterInfo01");
                this.FooterInfo02 = "MetaFrm.Razor.Browser".GetAttribute("FooterInfo02");
                this.FooterInfo03 = "MetaFrm.Razor.Browser".GetAttribute("FooterInfo03");
                this.FooterInfo04 = "MetaFrm.Razor.Browser".GetAttribute("FooterInfo04");
                this.Copyright = "MetaFrm.Razor.Browser".GetAttribute("Copyright");
            }
            catch (Exception)
            {
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                if (this.MainLayoutViewModel.NavMenu != null && this.MainLayoutViewModel.NavMenu.Instance != null && this.MainLayoutViewModel.NavMenu.Instance is IAction action)
                {
                    action.Action -= MainLayout_Begin;
                    action.Action += MainLayout_Begin;
                }

                if (this.MainLayoutViewModel.CurrentPage != null && this.MainLayoutViewModel.CurrentPage.Instance != null && this.MainLayoutViewModel.CurrentPage.Instance is IAction action1
                    && this.MainLayoutViewModel.TmpBrowserType == null)
                {
                    action1.Action -= MainLayout_Begin;
                    action1.Action += MainLayout_Begin;
                }

                //this.LoadLocalStorage();

                if (Factory.Platform != DevicePlatform.Web)
                    this.HomeLoadAsync();

                if (this.CloudMessaging != null)
                {
                    this.CloudMessaging.NotificationTappedEvent -= CloudMessaging_NotificationTappedEvent;
                    this.CloudMessaging.NotificationTappedEvent += CloudMessaging_NotificationTappedEvent;
                }
            }
            else
            {
                if (Factory.Platform == DevicePlatform.Web)
                    this.HomeLoadAsync();
            }

            if (this.MainLayoutViewModel.CurrentPage != null && this.MainLayoutViewModel.CurrentPage.Instance != null && this.MainLayoutViewModel.CurrentPage.Instance is IAction action2
                && this.MainLayoutViewModel.TmpBrowserType != null && this.MainLayoutViewModel.TmpBrowserType == this.MainLayoutViewModel.CurrentPageType)
            {
                action2.Action -= MainLayout_Begin;
                action2.Action += MainLayout_Begin;

                this.MainLayoutViewModel.TmpBrowserType = null;
            }
        }

        bool firstIsNavigationIntercepted = true;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:올바르게 ValueTasks 사용", Justification = "<보류 중>")]
        private void Navigation_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            if (!e.IsNavigationIntercepted && this.Navigationqueue.Count > 0)
            {
                //앞으로 가다가 처음 뒤로 가기 이면 마지막에 있는 항목은 현재 화면이기 때문에 현재 화면을 제거 한다.
                if (this.firstIsNavigationIntercepted)
                {
                    this.firstIsNavigationIntercepted = false;

                    if (this.Navigationqueue.Count > 1)
                        this.Navigationqueue.Remove(this.Navigationqueue[^1]);
                }

                MetaFrmEventArgs metaFrmEventArgs = this.Navigationqueue[^1];


                if (metaFrmEventArgs.Value is List<int> pairs)
                {
                    object? tmp = Client.GetAttribute("Modal.DataBsTarget");

                    if (tmp != null && tmp is string value && value.Length > 0)
                        this.JSRuntime?.InvokeVoidAsync("ModalClose", tmp);
                    else
                        this.JSRuntime?.InvokeVoidAsync("ModalClose", "Modal001");

                    pairs.Add(-1);
                    this.Navigationqueue.Remove(metaFrmEventArgs);
                    this.MainLayout_Begin(this, metaFrmEventArgs);
                }
            }
            else
                this.firstIsNavigationIntercepted = true;
        }

#pragma warning disable CA1816 // Dispose 메서드는 SuppressFinalize를 호출해야 합니다.
        public void Dispose()
#pragma warning restore CA1816 // Dispose 메서드는 SuppressFinalize를 호출해야 합니다.
        {
            if (this.CloudMessaging != null)
                this.CloudMessaging.NotificationTappedEvent -= CloudMessaging_NotificationTappedEvent;

            if (Navigation != null)
            {
                this.Navigation.LocationChanged -= Navigation_LocationChanged;
                this.Navigationqueue.Clear();
            }
        }

        private void CloudMessaging_NotificationTappedEvent(object sender, Maui.Notification.NotificationTappedEventArgs e)
        {
            if (e.NotificationData.Data != null)
            {
                try
                {
                    if (e.NotificationData.Data.ContainsKey("Menu") && e.NotificationData.Data["Menu"].Contains(','))
                    {
                        Task.Run(async delegate
                        {
                            string tmp = e.NotificationData.Data["Menu"];

                            Client.SetAttribute("Menu", tmp);//"MENU_ID,ASSEMBLY_ID"

                            tmp = tmp.Split(',')[1];//ASSEMBLY_ID

                            foreach (var item in e.NotificationData.Data.Keys)
                                Client.SetAttribute($"{tmp}.{item}", e.NotificationData.Data[item]);

                            await Task.Delay(TimeSpan.FromSeconds(1.6));

                            //Factory.ViewModelClear();
                            if (Client.GetAttribute("Menu") != null)
                                this.Navigation?.NavigateTo("/", true);
                        });
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        //private async void LoadLocalStorage()
        //{
        //    //if (this.LocalStorage != null)
        //    //{
        //    //    bool testBool = await this.LocalStorage.GetItemAsync<bool>(nameof(this.MainLayoutViewModel) + nameof(this.MainLayoutViewModel.TestBool));

        //    //    if (this.MainLayoutViewModel.TestBool != testBool)
        //    //    {
        //    //        this.MainLayoutViewModel.TestBool = testBool;
        //    //        this.StateHasChanged();
        //    //    }
        //    //}
        //}

        private async void HomeLoadAsync()
        {
            object? obj;
            string[] tmps;
            string? email = null;
            string? password = null;

            if (this.isFirstLoad)
            {
                this.isFirstLoad = false;

                obj = Client.GetAttribute("Menu");

                if (obj != null && obj is string tmp && tmp.Contains(','))
                {
                    if (!this.AuthState.IsLogin())
                    {
                        this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Login" });
                    }
                    else
                    {
                        Client.RemoveAttribute("Menu");
                        tmps = tmp.Split(",");
                        this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Menu", Value = new List<int> { tmps[0].ToInt(), tmps[1].ToInt() } });
                    }
                }
                else
                {
                    //로그인 안되어 있고 자동로그인 이면 로그인 화면으로
                    if (this.LocalStorage != null)
                    {
                        email = await this.LocalStorage.GetItemAsStringAsync("Login.Email");
                        password = await this.LocalStorage.GetItemAsStringAsync("Login.Password");
                    }

                    if (!this.AuthState.IsLogin() && !email.IsNullOrEmpty() && !password.IsNullOrEmpty())
                        this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Login" });
                    else
                        this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Menu", Value = new List<int> { 0, 0 } });
                }

                this.StateHasChanged();
            }
        }

        private async void MainLayout_Begin(ICore sender, MetaFrmEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case "ToastPara":
                        if (e.Value != null && e.Value is IToast toast1)
                        {
                            this.MainLayoutViewModel.ToastPara = new Dictionary<string, object> { { "ToastMessage", toast1 } };
                            toast1.Show();
                            this.StateHasChanged();
                        }

                        break;
                    case "ModalPara":
                        if (e.Value != null && e.Value is IModal modal1)
                        {
                            this.MainLayoutViewModel.ModalPara = new Dictionary<string, object> { { "ModalMessage", modal1 } };
                            modal1.Show();
                            this.StateHasChanged();
                        }
                        break;

                    case "ProfileImage":
                        if (e.Value != null && e.Value is string profileImage)
                        {
                            this.ProfileImage = profileImage;
                            //this.StateHasChanged();
                        }
                        break;
                    case "DisplayInfo":
                        if (e.Value != null && e.Value is string displayInfo)
                        {
                            this.DisplayInfo = displayInfo;
                            //this.StateHasChanged();
                        }
                        break;
                    case "Menu":
                        if (e.Value is List<int> pairs)
                        {
                            object? debugInfo;
                            TypeInfo? typeTitle;
                            Type? type = null;

                            debugInfo = Client.GetAttribute("DebugDLL");

                            if (debugInfo != null && debugInfo is Dictionary<int, Type> debugInfoDictionary)
                            {
                                if (debugInfoDictionary.ContainsKey(pairs[1]))
                                    type = debugInfoDictionary[pairs[1]];
                            }

                            bool isLogin = this.AuthState.IsLogin();

                            typeTitle = await Factory.GetDevelopmentTypeInfo(isLogin ? this.AuthState.Token() : null
                                , isLogin ? this.AuthState.UserID() : null
                                , pairs[0], pairs[1], type);

                            if (typeTitle != null)
                            {
                                this.MainLayoutViewModel.TmpBrowserType = type ?? typeTitle.Type;
                                this.MainLayoutViewModel.Title = $"{typeTitle.Title} ({this.MainLayoutViewModel.TmpBrowserType?.Assembly.GetName().Version})";
                            }

                            if (pairs.Count < 3)
                                this.Navigationqueue.Add(e);

                            if (this.Navigationqueue.Count > 0)
                            {
                                switch (Factory.Platform)
                                {
                                    case DevicePlatform.Web:
                                        if (this.Navigationqueue.Count > 20)
                                            this.Navigationqueue.Remove(this.Navigationqueue[0]);
                                        break;

                                    default:
                                        if (this.Navigationqueue.Count > 50)
                                            this.Navigationqueue.Remove(this.Navigationqueue[0]);
                                        break;
                                }
                            }
                        }

                        break;

                    default:
                        if (e.Action != null)
                            this.MainLayoutViewModel.TmpBrowserType = Factory.LoadTypeFromServiceAttribut(e.Action);

                        //switch (e.Action)
                        //{
                        //    case "Login":
                        //        this.MainLayoutViewModel.TmpBrowserType = typeof(MetaFrm.Razor.Login); break;
                        //    case "Logout":
                        //        this.MainLayoutViewModel.TmpBrowserType = typeof(MetaFrm.Razor.Logout); break;
                        //    case "Profile":
                        //        this.MainLayoutViewModel.TmpBrowserType = typeof(MetaFrm.Razor.Profile); break;
                        //    case "Register":
                        //        this.MainLayoutViewModel.TmpBrowserType = typeof(MetaFrm.Razor.Register); break;
                        //    case "PasswordReset"
                        //        this.MainLayoutViewModel.TmpBrowserType = typeof(MetaFrm.Razor.PasswordReset); break;
                        //    default:
                        //        if (e.Action != null)
                        //            this.MainLayoutViewModel.TmpBrowserType = Factory.LoadTypeFromServiceAttribut(e.Action);
                        //        break;
                        //}

                        this.MainLayoutViewModel.Title = $"{e.Action} ({this.MainLayoutViewModel.TmpBrowserType?.Assembly.GetName().Version})";
                        break;
                }
            }
            catch (Exception ex)
            {
                this.MainLayoutViewModel.TmpBrowserType = typeof(Error);
                this.MainLayoutViewModel.CurrentPagePara = new Dictionary<string, object> { { "Parameter", ex } };
            }

            if (this.MainLayoutViewModel.TmpBrowserType != null)
            {
                this.MainLayoutViewModel.CurrentPageType = null;
                this.MainLayoutViewModel.CurrentPageType = this.MainLayoutViewModel.TmpBrowserType;
            }

            this.StateHasChanged();
        }

        private void OnLoginClick()
        {
            if (!this.AuthState.IsLogin())
                this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Login" });
        }
        private void OnLogoutClick()
        {
            if (this.AuthState.IsLogin())
                this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Logout" });
        }

        private void OnProfileClick()
        {
            if (this.AuthState.IsLogin())
                this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Profile" });
        }

        private async void OnLayoutMenuExpandeClick()
        {
            if (this.JSRuntime != null)
                await this.JSRuntime.InvokeVoidAsync("LayoutMenuExpande");
        }
    }
}