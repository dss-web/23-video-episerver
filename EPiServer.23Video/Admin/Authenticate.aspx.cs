using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web.Helpers;
using System.Web.UI;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Web;
using EPiServer._23Video.Initialize;
using PlugInArea = EPiServer.PlugIn.PlugInArea;

namespace EPiServer._23Video.Admin
{
    [GuiPlugIn(
        DisplayName = "23 Video", 
        Description = "Save settings for 23 Video in DDS",
        Area = PlugInArea.AdminMenu, Url = "~/Admin/Authenticate.aspx")]
        //UrlFromModuleFolder = "Admin/Authenticate.aspx")]
    public partial class Authenticate : Shell.WebForms.WebFormsBase
    {
        private _23VideoSettings _currentSettings;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            this.SystemMessageContainer.Heading = "23Video";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                CustomerKey.Text = CurrentSettings.CustomerKey;
                CustomerSecret.Text = CurrentSettings.CustomerSecret;
                AccessToken.Text = CurrentSettings.AccessToken;
                AccessTokenSecret.Text = CurrentSettings.AccessTokenSecret;
            }

            //if (!string.IsNullOrEmpty(Request.QueryString["code"]))
            //{
            //    var webClient = new WebClient();
            //    var resultData = webClient.UploadValues("https://accounts.google.com/o/oauth2/token",
            //        new NameValueCollection
            //        {
            //            { "client_id", CurrentSettings.ClientId },
            //            { "client_secret", CurrentSettings.ClientSecret },
            //            { "redirect_uri", GetRedirectUri()},
            //            { "grant_type", "authorization_code" },
            //            { "code", Request.QueryString["code"] }
            //        });
            //    var result = Json.Decode(Encoding.UTF8.GetString(resultData));


            //    CurrentSettings.AccessToken = result.access_token;
            //    CurrentSettings.RefreshToken = result.refresh_token;
            //    CurrentSettings.TokenCreated = DateTime.Now;
            //    CurrentSettings.TokenExpires = result.expires_in;
            //    SettingsRepository.Service.SaveSettings(CurrentSettings);

            //    Response.Redirect(UriSupport.ResolveUrlFromUIBySettings("Admin/Default.aspx") + "?customdefaultpage=" + Paths.ToResource("EPiServer.23Video", "Admin/Authenticate.aspx"));
            //}
        }

        protected void ButtonSaveSettings_Click(object sender, EventArgs e)
        {
            CurrentSettings.CustomerKey = CustomerKey.Text.Trim();
            CurrentSettings.CustomerSecret = CustomerSecret.Text.Trim();
            CurrentSettings.AccessToken = AccessToken.Text.Trim();
            CurrentSettings.AccessTokenSecret = AccessTokenSecret.Text.Trim();
            
            SettingsRepository.Service.SaveSettings(CurrentSettings);
        }

        protected _23VideoSettings CurrentSettings
        {
            get { return _currentSettings ?? (_currentSettings = SettingsRepository.Service.LoadSettings()); }
        }

        public Injected<_23VideoSettingsRepository> SettingsRepository { get; set; }

        protected string GetRedirectUri()
        {
            return  (SiteDefinition.Current.SiteUrl + Paths.ToResource("EPiServer.23Video","Admin/Authenticate.aspx").Substring(1)).ToLower();
        }
    }
}