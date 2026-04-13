#region Using directives
using System;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.Core;
using FTOptix.UI;
#endregion

public class LoginButtonLogic : BaseNetLogic
{
    /// <summary>
    /// This method sets the mode of a ComboBox based on the authentication mode of the current project.
    /// If the authentication mode is ModelOnly, the ComboBox is set to Normal mode; otherwise, it is set to Editable mode.
    /// </summary>
    /// <param name="comboBox">The ComboBox instance to modify.</param>
    public override void Start()
    {
        ComboBox comboBox = Owner.Owner.Get<ComboBox>("Username");
        if (Project.Current.Authentication.AuthenticationMode == AuthenticationMode.ModelOnly)
        {
            comboBox.Mode = ComboBoxMode.Normal;
        }
        else
        {
            comboBox.Mode = ComboBoxMode.Editable;
        }
    }

    public override void Stop()
    {
        // No cleanup needed
    }
    
    /// <summary>
    /// This method performs the login operation using the provided username and password.
    /// It retrieves the Users alias and PasswordExpiredDialogType from the logic object,
    /// attempts to log in using the Session object,
    /// and handles the result of the login attempt.
    /// If the login is successful, it enables the login button;
    /// if the password is expired, it opens a dialog for the user to change their password.
    /// If the login fails, it sets an output message on the login form.
    /// </summary>
    /// <param name="username">The username to log in with.</param>
    /// <param name="password">The password to log in with.</param>
    [ExportMethod]
    public void PerformLogin(string username, string password)
    {
        var usersAlias = LogicObject.GetAlias("Users");
        if (usersAlias == null || usersAlias.NodeId == NodeId.Empty)
        {
            Log.Error("LoginButtonLogic", "Missing Users alias");
            return;
        }

        var passwordExpiredDialogType = LogicObject.GetAlias("PasswordExpiredDialogType") as DialogType;
        if (passwordExpiredDialogType == null)
        {
            Log.Error("LoginButtonLogic", "Missing PasswordExpiredDialogType alias");
            return;
        }

        Button loginButton = (Button)Owner;
        loginButton.Enabled = false;

        try
        {
            var loginResult = Session.Login(username, password);
            if (loginResult.ResultCode == ChangeUserResultCode.PasswordExpired)
            {
                loginButton.Enabled = true;
                var user = usersAlias.Get<User>(username);
                var ownerButton = (Button)Owner;
                ownerButton.OpenDialog(passwordExpiredDialogType, user.NodeId);
                return;
            }
            else if (loginResult.ResultCode != ChangeUserResultCode.Success)
            {
                loginButton.Enabled = true;
                Log.Error("LoginButtonLogic", "Authentication failed");
            }

            if (loginResult.ResultCode != ChangeUserResultCode.Success)
            {
                var outputMessageLabel = Owner.Owner.GetObject("LoginFormOutputMessage");
                var outputMessageLogic = outputMessageLabel.GetObject("LoginFormOutputMessageLogic");
                outputMessageLogic.ExecuteMethod("SetOutputMessage", new object[] { (int)loginResult.ResultCode });
            }
        }
        catch (Exception e)
        {
            Log.Error("LoginButtonLogic", e.Message);
        }

        loginButton.Enabled = true;
    }
}
