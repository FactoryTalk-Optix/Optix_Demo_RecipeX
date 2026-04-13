#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.Core;
#endregion

public class OpenLoginPageButtonLogic : BaseNetLogic
{
    public override void Start()
    {
        try
        {
            var button = (Button)Owner;
            button.OnMouseClick += Button_OnMouseClick;
        }
        catch (Exception e)
        {
            Log.Error("OpenLoginPageButtonLogic", e.Message);
        }
    }

    private void OAuth2Completed(OAuth2ResultCode result)
    {
        if (result == OAuth2ResultCode.Success)
        {
            Log.Info($"OAuth2 flow completed successfully");
            return;
        }

        try
        {
            var button = (Button)Owner;
            var panel = (Panel)button.Owner;
            var statusLabel = panel.Get<Label>("StatusLabel");
            var message = OAuth2ResultHelper.GetResultMessage(result);
            statusLabel.Text = message;
            Log.Info($"OAuth2 Flow completed ({result}): {message}");
        }
        catch (Exception ex)
        {
            Log.Error("OpenLoginPageButtonLogic", ex.Message);
        }
    }

    private void Button_OnMouseClick(object sender, MouseClickEvent e)
    {
        try
        {
            var button = (Button)Owner;
            var panel = (Panel)button.Owner;
            var statusLabel = panel.Get<Label>("StatusLabel");
            statusLabel.Text = "";
            Session.StartOAuth2WebFlow(OAuth2Completed);
        }
        catch (Exception ex)
        {
            Log.Error("OpenLoginPageButtonLogic", ex.Message);
        }
    }
}
