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

public class OAuth2ResultHelper : BaseNetLogic
{
    public static string GetResultMessage(OAuth2ResultCode result)
    {
        return result switch
        {
            OAuth2ResultCode.Success => "Success",
            OAuth2ResultCode.ConfigurationError => "Configuration error",
            OAuth2ResultCode.InvalidState => "The state string received does not match the state string sent",
            OAuth2ResultCode.HttpClientError => "Http client error",
            OAuth2ResultCode.InvalidToken => "Invalid JWT token",
            OAuth2ResultCode.ChangeUserError => "Error while changing user",
            _ => "Unknown result"
        };
    }
}
