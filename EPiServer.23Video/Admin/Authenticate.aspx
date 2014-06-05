<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Authenticate.aspx.cs" Inherits="EPiServer._23Video.Admin.Authenticate" %>

<%@ Import assembly="EPiServer.23Video" %>
<%@ Register TagPrefix="episerverui" Namespace="EPiServer.UI.WebControls" Assembly="EPiServer.UI" %>
<asp:Content ContentPlaceHolderID="MainRegion" runat="server">
    
    <EPiServerUI:TabStrip  runat="server" id="actionTab" EnableViewState="true" GeneratesPostBack="False" targetid="tabView" supportedpluginarea="SystemSettings">
        <EPiServerUI:Tab Text="Summary" runat="server" ID="TabSummary" />
        <EPiServerUI:Tab Text="Configuration"  runat="server" ID="Tab2" />
    </EPiServerUI:TabStrip>

    <asp:Panel runat="server" ID="tabView" CssClass="epi-formArea epi-paddingHorizontal epi-padding">
        <div runat="server">
            <fieldset>
                <legend>Overview</legend>    
                <dl>
                    <dt>Access Token</dt>
                    <dd><%= CurrentSettings.AccessToken %></dd>
                    <dt>Refresh Token</dt>
                    <dd><%= CurrentSettings.RefreshToken %></dd>
                </dl>
            </fieldset>
        </div>
        
        <div runat="server">
            <fieldset>
                <legend>OAuth 2.0 Settings</legend>    
                <dl>
                    <dt>Domain</dt>
                    <dd><asp:TextBox ID="Domain" runat="server" /></dd>
                    <dt>Customer Key</dt>
                    <dd><asp:TextBox ID="CustomerKey" runat="server" /></dd>
                    <dt>Customer Secret</dt>
                    <dd><asp:TextBox ID="CustomerSecret" runat="server" /></dd>
                    <dt>Access token</dt>
                    <dd><asp:TextBox ID="AccessToken" runat="server" /></dd>
                    <dt>Access Token Secret</dt>
                    <dd><asp:TextBox ID="AccessTokenSecret" runat="server" /></dd>

                </dl>
                <div class="floatright">
                    <episerverui:toolbutton id="ButtonSaveSettings" runat="server" OnClick="ButtonSaveSettings_Click" text="<%$ Resources: EPiServer, button.save %>"  tooltip="<%$ Resources: EPiServer, button.save %>"  skinid="Save" />
                </div>
        </fieldset>
    
            <%--<a href="https://accounts.google.com/o/oauth2/auth?approval_prompt=force&client_id=<%= CurrentSettings.ClientId %>&redirect_uri=<%= HttpUtility.UrlEncode(GetRedirectUri()) %>&scope=https://www.googleapis.com/auth/youtube&response_type=code&access_type=offline" 
                target="_top">Authenticate</a>
           
            <%= UriSupport.ResolveUrlFromUIBySettings("Admin/Default.aspx") %>--%>
        </div>
    </asp:Panel>

    
 </asp:Content>