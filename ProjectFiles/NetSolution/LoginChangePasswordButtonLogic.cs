#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.Retentivity;
#endregion

public class LoginChangePasswordButtonLogic : BaseNetLogic
{
    /// <summary>
    /// This method handles password change operations by validating the new and confirm passwords,
    /// and then updating the user's password if valid. It also updates the output message
    /// based on the result of the password change operation.
    /// </summary>
    /// <param name="oldPassword">The current password of the user.</param>
    /// <param name="newPassword">The new password to set for the user.</param>
    /// <param name="confirmPassword">The confirmation of the new password.</param>
    [ExportMethod]
    public void PerformChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        var outputMessageLabel = Owner.Owner.GetObject("ChangePasswordFormOutputMessage");
        var outputMessageLogic = outputMessageLabel.GetObject("LoginChangePasswordFormOutputMessageLogic");

        if (newPassword != confirmPassword)
        {
            outputMessageLogic.ExecuteMethod("SetOutputMessage", new object[] { 7 });
        }
        else
        {
            var username = Session.User.BrowseName;
            try
            {
                var userWithExpiredPassword = Owner.GetAlias("UserWithExpiredPassword");
                if (userWithExpiredPassword != null)
                    username = userWithExpiredPassword.BrowseName;
            }
            catch
            {
            }

            var result = Session.ChangePassword(username, newPassword, oldPassword);
            if (result.ResultCode == ChangePasswordResultCode.Success)
            {
                var parentDialog = Owner.Owner?.Owner?.Owner as Dialog;
                if (parentDialog != null && result.Success)
                    parentDialog.Close();
            }
            outputMessageLogic.ExecuteMethod("SetOutputMessage", new object[] { (int)result.ResultCode });
        }
    }
}
