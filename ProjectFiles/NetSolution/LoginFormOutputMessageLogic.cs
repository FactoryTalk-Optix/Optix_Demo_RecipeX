#region Using directives
using System;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.OPCUAServer;
#endregion

public class LoginFormOutputMessageLogic : BaseNetLogic
{
    /// <summary>
    /// This method starts a process by hiding the message label, retrieving a login result code variable,
    /// and initiating a delayed task that hides the message label again and sets a flag to indicate task completion.
    /// </summary>
    /// <remarks>
    /// The method uses a <see cref="DelayedTask"/> to delay the execution of the task for 10 seconds.
    /// The <see cref="HideMessageLabel"/> method is called twice within the task to ensure the message is hidden
    /// both before and after the delay.
    /// </remarks>
    public override void Start()
    {
        HideMessageLabel();
        loginResultCodeVariable = Owner.GetVariable("LoginResultCode");

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
    /// Sets the output message based on the provided result code and handles task cancellation.
    /// </summary>
    /// <param name="resultCode">The result code to set in the output message.</param>
    /// <remarks>
    /// The method checks if the <see cref="loginResultCodeVariable"/> exists. If not, it logs an error and returns.
    /// It then sets the result code, shows the message label, cancels the task if it's already running, and starts a new task.
    /// </remarks>
    [ExportMethod]
    public void SetOutputMessage(int resultCode)
    {
        if (loginResultCodeVariable == null)
        {
            Log.Error("LoginFormOutputMessageLogic", "Unable to find LoginResultCode variable in LoginFormOutputMessage label");
            return;
        }

        loginResultCodeVariable.Value = resultCode;
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
    /// The method assumes that <see cref="Owner"/> is a <see cref="Label" /> control.
    /// </remarks>
    private void ShowMessageLabel()
    {
        var messageLabel = (Label)Owner;
        messageLabel.Visible = true;
    }

    /// <summary>
    /// This method hides the message label by setting its visibility to false.
    /// </summary>
    /// <param name="messageLabel">The label to hide.</param>
    private void HideMessageLabel()
    {
        var messageLabel = (Label)Owner;
        messageLabel.Visible = false;
    }

    private DelayedTask task;
    private bool taskStarted = false;
    private IUAVariable loginResultCodeVariable;
}
