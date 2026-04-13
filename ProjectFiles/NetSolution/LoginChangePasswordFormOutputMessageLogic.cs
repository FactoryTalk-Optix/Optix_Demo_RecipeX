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

public class LoginChangePasswordFormOutputMessageLogic : BaseNetLogic
{
    /// <summary>
    /// This method initiates a delayed task that hides the message label and sets a flag to indicate task started.
    /// The method also retrieves a variable named "ChangePasswordResultCode" from the owner.
    /// </summary>
    /// <remarks>
    /// The method uses a <see cref="DelayedTask"/> to perform the delayed action.
    /// </remarks>
    public override void Start()
    {
        HideMessageLabel();
        changePasswordResultCodeVariable = Owner.GetVariable("ChangePasswordResultCode");

        task = new DelayedTask(() =>
        {
            HideMessageLabel();
            taskStarted = false;
        }, 10000, LogicObject);
    }

    public override void Stop()
    {
        task?.Dispose();
    }

    /// <summary>
    /// This method sets the output message based on the provided result code,
    /// cancels a running task if it's active, and starts a new task.
    /// </summary>
    /// <param name="resultCode">The result code to set in the output message.</param>
    /// <remarks>
    /// The method first checks if the <see cref="changePasswordResultCodeVariable"/> is null.
    /// If it is, it logs an error and returns.
    /// Then it sets the result code, shows the message label, cancels any running task,
    /// and starts a new task to continue the process.
    /// </remarks>
    [ExportMethod]
    public void SetOutputMessage(int resultCode)
    {
        if (changePasswordResultCodeVariable == null)
        {
            Log.Error("ChangePasswordFormOutputMessageLogic", "Unable to find ChangePasswordResultCode variable in ChangePasswordFormOutputMessage label");
            return;
        }

        changePasswordResultCodeVariable.Value = resultCode;
        ShowMessageLabel();

        if (taskStarted)
        {
            task?.Cancel();
            taskStarted = false;
        }

        task.Start();
        taskStarted = true;
    }

    /// <summary>
    /// This method sets the <see cref="Visible"/> property of the owner control to true.
    /// </summary>
    /// <remarks>
    /// The method retrieves the <see cref="Label"/> control from the owner and sets its <see cref="Visible"/> property to true.
    /// </remarks>
    private void ShowMessageLabel()
    {
        var messageLabel = (Label)Owner;
        messageLabel.Visible = true;
    }

    /// <summary>
    /// This method hides the message label by setting its visibility to false.
    /// </summary>
    private void HideMessageLabel()
    {
        var messageLabel = (Label)Owner;
        messageLabel.Visible = false;
    }

    private DelayedTask task;
    private bool taskStarted = false;
    private IUAVariable changePasswordResultCodeVariable;
}
