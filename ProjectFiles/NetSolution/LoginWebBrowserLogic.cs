#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.UI;
using FTOptix.NetLogic;
using FTOptix.CoreBase;
using FTOptix.Core;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
#endregion

public class LoginWebBrowserLogic : BaseNetLogic
{
    /// <summary>
    /// This method starts the OAuth2 authentication flow for a web browser.
    /// It sets up an event handler for URL redirection and logs the start of the flow.
    /// </summary>
    public override void Start()
    {
        try
        {
            var webBrowser = (WebBrowser)Owner;

            webBrowser.OnURLRedirection += WebBrowser_OnURLRedirection;

            Log.Info("Starting OAuth2 authentication flow");
            Session.StartOAuth2Flow(webBrowser.NodeId, OAuth2Completed);
        }
        catch (Exception e)
        {
            Log.Error("LoginWebBrowserLogic", e.Message);
        }
    }

    /// <summary>
    /// This method handles URL redirection events for a WebBrowser control.
    /// It logs the received URL and hides the WebBrowser control.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The URLRedirectionEvent containing the URL information.</param>
    private void WebBrowser_OnURLRedirection(object sender, URLRedirectionEvent e)
    {
        try
        {
            Log.Info($"Redirect event received with URL {e.URL}");
            var webBrowser = (WebBrowser)sender;
            webBrowser.Visible = false;
        }
        catch (Exception ex)
        {
            Log.Error("LoginWebBrowserLogic", ex.Message);
        }
    }

    /// <summary>
    /// This method handles the completion of an OAuth 2.0 flow. If the result is successful,
    /// it logs the success and returns. If not, it hides the web browser, displays a status
    /// message, and logs the result.
    /// </summary>
    /// <param name="result">The result of the OAuth 2.0 flow.</param>
    /// <remarks>
    /// The method assumes that `Owner` is a valid parent object that can be cast to `WebBrowser`
    /// and `Panel`. It also assumes that `GetResultMessage` returns a string message based on
    /// the result code.
    /// </remarks>
    private void OAuth2Completed(OAuth2ResultCode result)
    {
        if (result == OAuth2ResultCode.Success)
        {
            Log.Info($"OAuth2 flow completed successfully");
            return;
        }

        try
        {
            var webBrowser = (WebBrowser)Owner;
            var panel = (Panel)webBrowser.Owner;
            var statusLabel = panel.Get<Label>("StatusLabel");
            var message = OAuth2ResultHelper.GetResultMessage(result);
            webBrowser.Visible = false;
            statusLabel.Text = message;
            Log.Info($"OAuth2 Flow completed ({result}): {message}");
        }
        catch (Exception ex)
        {
            Log.Error("LoginWebBrowserLogic", ex.Message);
        }
    }
}
