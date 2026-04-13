#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FTOptix.Core;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.RecipeX;
using UAManagedCore;
using static RecipeCommandsLogic;
#endregion

public class RecipeCommandsLogic : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void ApplyRecipeToAllOvens(NodeId recipeSchemaNodeId, NodeId recipeIdNodeId, out TransferFromStoreToTargetResultCode resultCode)
    {
        var recipeSchema = InformationModel.Get<FTOptix.RecipeX.RecipeSchema>(recipeSchemaNodeId);
        var recipeId = (FTOptix.RecipeX.RecipeId)InformationModel.GetVariable(recipeIdNodeId).Value;
        var ovensFolder = Project.Current.Get<Folder>("Model/Ovens");

        resultCode = TransferFromStoreToTargetResultCode.Success;

        foreach (var oven in ovensFolder.Children.OfType<OptixOven>())
        {
            var result = recipeSchema.TransferFromStoreToTarget(recipeId, oven.NodeId, ErrorPolicy.Strict);
            if (result != TransferFromStoreToTargetResultCode.Success)
            {
                resultCode = result;
            }
        }
    }
}
