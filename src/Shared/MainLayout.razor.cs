using MetaFrm.Alert;
using MetaFrm.Config;
using MetaFrm.Control;
using MetaFrm.Maui.Devices;
using MetaFrm.Razor.Browser.ViewModels;
using MetaFrm.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MetaFrm.Razor.Browser.Shared
{
    /// <summary>
    /// MainLayout
    /// </summary>
    public partial class MainLayout : IDisposable
    {
        internal MainLayoutViewModel MainLayoutViewModel { get; set; } = new(null);

        private bool isFirstLoad = true;

        private readonly List<MetaFrmEventArgs> Navigationqueue = [];

        [Inject]
        private Maui.Notification.ICloudMessaging? CloudMessaging { get; set; }

        [Inject]
        IActionEvent? ActionEvent { get; set; }

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

        private bool BackButtonPressedPageBackward { get; set; } = true;
        private string DisplayInfo { get; set; } = string.Empty;
        private string ProfileImage { get; set; } = string.Empty;
        private string FooterInfo01 { get; set; } = string.Empty;
        private string FooterInfo02 { get; set; } = string.Empty;
        private string FooterInfo03 { get; set; } = string.Empty;
        private string FooterInfo04 { get; set; } = string.Empty;
        private string Copyright { get; set; } = string.Empty;
        private List<int> SettingsMenu { get; set; } = [];
        private List<int> HomeMenuAssemblyID { get; set; } = [];
        private bool IsLoginView { get; set; } = true;
        private bool IsLoginShowMenu { get; set; } = false;
        private string PageCss { get; set; } = string.Empty;
        private string NavCss { get; set; } = string.Empty;
        private string ContentCss { get; set; } = string.Empty;
        private bool IsLogin { get; set; } = false;
        private string TemplateName { get; set; } = string.Empty;

        /// <summary>
        /// MainLayout
        /// </summary>
        public MainLayout(Factory _)
        {
            string tmp = this.GetType()?.FullName ?? "";
            if (!string.IsNullOrEmpty(tmp))
            {
                Factory.LoadInstance(tmp)?.Dispose();
                Factory.RegisterInstance(this, tmp);
            }
        }

        /// <summary>
        /// OnInitialized
        /// </summary>
        protected override void OnInitialized()
         {
            base.OnInitialized();

            this.MainLayoutViewModel = this.CreateViewModel<MainLayoutViewModel>();

            if (this.Navigation != null)
                this.Navigation.LocationChanged += Navigation_LocationChanged;

            try
            {
                if (this.ActionEvent != null)
                {
                    this.ActionEvent.Action -= MainLayout_Begin;
                    this.ActionEvent.Action += MainLayout_Begin;
                }
            }
            catch (Exception)
            {
            }

            try
            {
                this.BackButtonPressedPageBackward = this.GetAttributeBool(nameof(this.BackButtonPressedPageBackward));
                this.Copyright = this.GetAttribute(nameof(this.Copyright));
                this.FooterInfo01 = this.GetAttribute(nameof(this.FooterInfo01));
                this.FooterInfo02 = this.GetAttribute(nameof(this.FooterInfo02));
                this.FooterInfo03 = this.GetAttribute(nameof(this.FooterInfo03));
                this.FooterInfo04 = this.GetAttribute(nameof(this.FooterInfo04));
                this.IsLoginView = this.GetAttributeBool(nameof(this.IsLoginView));
                this.IsLoginShowMenu = this.GetAttributeBool(nameof(this.IsLoginShowMenu));
                this.PageCss = this.GetAttribute(nameof(this.PageCss));
                this.NavCss = this.GetAttribute(nameof(this.NavCss));
                this.ContentCss = this.GetAttribute(nameof(this.ContentCss));
                string tmp = this.GetAttribute(nameof(this.SettingsMenu));

                if (!string.IsNullOrEmpty(tmp) && tmp.Contains(','))
                {
                    string[] tmps = tmp.Split(',');
                    this.SettingsMenu.Add(tmps[0].ToInt());
                    this.SettingsMenu.Add(tmps[1].ToInt());
                }

                tmp = this.GetAttribute(nameof(this.HomeMenuAssemblyID));
                if (!string.IsNullOrEmpty(tmp) && tmp.Contains(','))
                {
                    string[] tmps = tmp.Split(',');
                    this.HomeMenuAssemblyID.Add(tmps[0].ToInt());
                    this.HomeMenuAssemblyID.Add(tmps[1].ToInt());
                }

                this.TemplateName = this.GetAttribute(nameof(this.TemplateName));
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// OnAfterRender
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            try
            {
                if (firstRender)
                {
                    this.InitScale();

                    if (this.MainLayoutViewModel.NavMenu != null && this.MainLayoutViewModel.NavMenu.Instance != null && this.MainLayoutViewModel.NavMenu.Instance is IAction action)
                    {
                        action.Action -= MainLayout_Begin;
                        action.Action += MainLayout_Begin;
                    }

                    if (this.MainLayoutViewModel.CurrentPage != null && this.MainLayoutViewModel.CurrentPage.Instance != null && this.MainLayoutViewModel.CurrentPage.Instance is IAction action1 && this.MainLayoutViewModel.TmpBrowserType == null)
                    {
                        action1.Action -= MainLayout_Begin;
                        action1.Action += MainLayout_Begin;
                    }

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
            catch (Exception ex)
            {
                this.MainLayoutViewModel.TmpBrowserType = typeof(Error);
                this.MainLayoutViewModel.CurrentPagePara = new Dictionary<string, object> { { "Parameter", ex } };
            }
        }
        private async void InitScale()
        {
            if (this.LocalStorage != null && this.JSRuntime != null)
            {
                var scale = await this.LocalStorage.GetItemAsStringAsync("Viewport.Scale");

                if (Factory.Platform == DevicePlatform.Android)
                {
                    if (scale != null)
                        await this.JSRuntime.InvokeVoidAsync("InitAndroidViewportScale", $"", $"{scale:0.#}");
                    else
                        await this.JSRuntime.InvokeVoidAsync("InitAndroidViewportScale", $"", $"{2.0M:0.#}");
                }
                else if (Factory.Platform == DevicePlatform.iOS)
                {
                    if (scale != null)
                        await this.JSRuntime.InvokeVoidAsync("SetViewportScale", $"", $"{scale:0.#}");
                    else
                        await this.JSRuntime.InvokeVoidAsync("SetViewportScale", $"", $"{1.0M:0.#}");
                }
            }
        }

        bool firstIsNavigationIntercepted = true;
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
                    {
                        ValueTask? _ = this.JSRuntime?.InvokeVoidAsync("ModalClose", tmp);
                    }
                    else
                    {
                        ValueTask? _ = this.JSRuntime?.InvokeVoidAsync("ModalClose", "Modal001");
                    }

                    pairs.Add(-1);
                    this.Navigationqueue.Remove(metaFrmEventArgs);
                    this.MainLayout_Begin(this, metaFrmEventArgs);
                }
            }
            else
                this.firstIsNavigationIntercepted = true;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.MainLayoutViewModel.NavMenu != null && this.MainLayoutViewModel.NavMenu.Instance != null && this.MainLayoutViewModel.NavMenu.Instance is IAction action)
                    action.Action -= MainLayout_Begin;

                if (this.MainLayoutViewModel.CurrentPage != null && this.MainLayoutViewModel.CurrentPage.Instance != null && this.MainLayoutViewModel.CurrentPage.Instance is IAction action1 && this.MainLayoutViewModel.TmpBrowserType == null)
                    action1.Action -= MainLayout_Begin;

                if (this.CloudMessaging != null)
                    this.CloudMessaging.NotificationTappedEvent -= CloudMessaging_NotificationTappedEvent;

                if (this.ActionEvent != null)
                    this.ActionEvent.Action -= MainLayout_Begin;

                if (this.Navigation != null)
                {
                    this.Navigation.LocationChanged -= Navigation_LocationChanged;
                    this.Navigationqueue.Clear();
                }
            }
        }

        private void CloudMessaging_NotificationTappedEvent(object sender, Maui.Notification.NotificationTappedEventArgs e)
        {
            if (e.NotificationData.Data != null)
            {
                try
                {
                    if (e.NotificationData.Data.TryGetValue("Menu", out string? value) && value.Contains(','))
                    {
                        Task.Run(async delegate
                        {
                            Client.SetAttribute("Menu", value);//"MENU_ID,ASSEMBLY_ID"

                            value = value.Split(',')[1];//ASSEMBLY_ID

                            foreach (var item in e.NotificationData.Data.Keys)
                                Client.SetAttribute($"{value}.{item}", e.NotificationData.Data[item]);

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
        private async void HomeLoadAsync()
        {
            object? obj;
            string[] tmps;
            string? email = null;
            string? password = null;

            if (this.isFirstLoad)
            {
                this.IsLogin = this.AuthState.IsLogin();

                this.isFirstLoad = false;

                obj = Client.GetAttribute("Menu");

                if (obj != null && obj is string tmp && tmp.Contains(','))
                {
                    if (!this.IsLogin)
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

                    if (!this.IsLogin && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                        this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Login" });
                    else
                    {
                        if (this.HomeMenuAssemblyID.Count == 2)
                        {
                            this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Menu", Value = new List<int> { this.HomeMenuAssemblyID[0], this.HomeMenuAssemblyID[1] } });
                        }
                        else
                            this.MainLayout_Begin(this, new MetaFrmEventArgs { Action = "Menu", Value = new List<int> { 0, 0 } });
                    }
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
                            toast1.Show();
                            this.MainLayoutViewModel.ToastPara = new Dictionary<string, object> { { "ToastMessage", toast1 } };
                        }

                        break;
                    case "ModalPara":
                        if (e.Value != null && e.Value is IModal modal1)
                        {
                            this.MainLayoutViewModel.ModalPara = new Dictionary<string, object> { { "ModalMessage", modal1 } };
                            modal1.Show();
                        }
                        break;

                    case "ProfileImage":
                        if (e.Value != null && e.Value is string profileImage)
                        {
                            this.ProfileImage = profileImage;
                        }
                        else
                            this.SearchProfileImage();

                        break;
                    case "DisplayInfo":
                        if (e.Value != null && e.Value is string displayInfo)
                        {
                            this.DisplayInfo = displayInfo;
                        }
                        else
                            this.SearchDisplayInfo();

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
                                if (debugInfoDictionary.TryGetValue(pairs[1], out Type? value))
                                    type = value;
                            }

                            bool isLogin = this.AuthState.IsLogin();

                            typeTitle = await DevelopmentService.GetTypeInfoAsync(isLogin ? this.AuthState.Token() : null
                                , isLogin ? this.AuthState.UserID() : null
                                , pairs[0], pairs[1], type);

                            if (typeTitle != null)
                            {
                                this.MainLayoutViewModel.TmpBrowserType = type ?? typeTitle.Type;
                                this.MainLayoutViewModel.Title = $"{typeTitle.Title} ({this.MainLayoutViewModel.TmpBrowserType?.Assembly.GetName().Version})";
                            }

                            if (pairs.Count < 3 && this.BackButtonPressedPageBackward)
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
                    case "InvokeAsync":
                        break;

                    case "Menu.Active":
                        break;

                    default:
                        if (e.Action != null)
                            this.MainLayoutViewModel.TmpBrowserType = await Factory.LoadTypeAsync(this.GetAttribute(e.Action));

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
                        //    case "PasswordReset":
                        //        this.MainLayoutViewModel.TmpBrowserType = typeof(MetaFrm.Razor.PasswordReset); break;
                        //    default:
                        //        if (e.Action != null)
                        //            this.MainLayoutViewModel.TmpBrowserType = await Factory.LoadTypeAsync(this.GetAttribute(e.Action));
                        //        break;
                        //}

                        this.MainLayoutViewModel.Title = $"{e.Action} ({this.MainLayoutViewModel.TmpBrowserType?.Assembly.GetName().Version})";
                        break;
                }

                this.OnAction(this, e);
            }
            catch (Exception ex)
            {
                this.MainLayoutViewModel.TmpBrowserType = typeof(Error);
                this.MainLayoutViewModel.CurrentPagePara = new Dictionary<string, object> { { "Parameter", ex } };
            }

            if (this.MainLayoutViewModel.TmpBrowserType != null)
            {
                if (this.MainLayoutViewModel.CurrentPage != null && this.MainLayoutViewModel.CurrentPage.Instance != null && this.MainLayoutViewModel.CurrentPage.Instance is IAction action)
                    action.Action -= MainLayout_Begin;

                this.MainLayoutViewModel.CurrentPageType = null;
                this.MainLayoutViewModel.CurrentPageType = this.MainLayoutViewModel.TmpBrowserType;
            }

            try
            {
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception)
            {
            }
        }

        private void OnLoginClick()
        {
            if (!this.AuthState.IsLogin())
            {
                this.MainLayout_Begin(this, new() { Action = "Login" });

                this.OnAction(this, new() { Action = "Menu.Active", Value = new List<int> { 0, 0 } });
            }
        }
        private void OnLogoutClick()
        {
            if (this.AuthState.IsLogin())
                this.MainLayout_Begin(this, new () { Action = "Logout" });
        }

        private void OnProfileClick()
        {
            if (this.AuthState.IsLogin())
            {
                this.MainLayout_Begin(this, new() { Action = "Profile" });

                this.OnAction(this, new() { Action = "Menu.Active", Value = new List<int> { 0, 0 } });
            }
        }

        private void OnSettingsClick()
        {
            if (this.AuthState.IsLogin() && this.SettingsMenu.Count == 2)
            {
                this.MainLayout_Begin(this, new() { Action = "Menu", Value = this.SettingsMenu });

                this.OnAction(this, new() { Action = "Menu.Active", Value = this.SettingsMenu });
            }
        }

        private async void OnLayoutMenuExpandeClick()
        {
            if (this.JSRuntime != null)
                await this.JSRuntime.InvokeVoidAsync("LayoutMenuExpande");
        }


        private async void SearchDisplayInfo()
        {
            Response response;

            if (this.MainLayoutViewModel.IsBusy) return;

            try
            {
                this.MainLayoutViewModel.IsBusy = true;

                bool isLogin = this.AuthState.IsLogin();

                ServiceData data = new()
                {
                    Token = isLogin ? this.AuthState.Token() : Factory.AccessKey
                };

                data["1"].CommandText = this.GetAttribute("SearchDisplayInfo");
                data["1"].AddParameter("USER_ID", Database.DbType.Int, 3, isLogin ? this.AuthState.UserID() : null);

                response = await this.ServiceRequestAsync(data);

                if (response.Status == Status.OK)
                {
                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0 && response.DataSet.DataTables[0].DataRows.Count > 0)
                    {
                        this.DisplayInfo = response.DataSet.DataTables[0].DataRows[0].String("DISPLAY_INFO") ?? "";
                        await InvokeAsync(StateHasChanged);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                this.MainLayoutViewModel.IsBusy = false;
            }
        }
        private async void SearchProfileImage()
        {
            Response response;

            if (this.MainLayoutViewModel.IsBusy) return;

            try
            {
                this.MainLayoutViewModel.IsBusy = true;

                bool isLogin = this.AuthState.IsLogin();

                ServiceData data = new()
                {
                    Token = isLogin ? this.AuthState.Token() : Factory.AccessKey
                };

                data["1"].CommandText = this.GetAttribute("SearchProfileImage");
                data["1"].AddParameter("USER_ID", Database.DbType.Int, 3, isLogin ? this.AuthState.UserID() : null);

                response = await this.ServiceRequestAsync(data);

                if (response.Status == Status.OK)
                {
                    if (response.DataSet != null && response.DataSet.DataTables.Count > 0 && response.DataSet.DataTables[0].DataRows.Count > 0)
                    {
                        this.ProfileImage = response.DataSet.DataTables[0].DataRows[0].String("PROFILE_IMAGE") ?? "";
                        await InvokeAsync(StateHasChanged);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                this.MainLayoutViewModel.IsBusy = false;
            }
        }
    }
}