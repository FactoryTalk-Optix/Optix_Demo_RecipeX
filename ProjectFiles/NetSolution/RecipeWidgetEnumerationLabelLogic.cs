#region Using directives
using System;
using System.Linq;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
#endregion

public class RecipeWidgetEnumerationLabelLogic : BaseNetLogic
{
    public override void Start()
    {
        enumerationVariable = InformationModel.GetVariable(Owner.GetVariable("EnumVariable").Value);
        if (enumerationVariable == null)
        {
            Log.Error(Owner.BrowseName, "EnumVariable is not linked to any variable");
            return;
        }

        IUADataType variableDataType = InformationModel.Get<IUADataType>(enumerationVariable.DataType);
        if (variableDataType?.IsSubTypeOf(OpcUa.DataTypes.Enumeration) != true)
        {
            Log.Error(Owner.BrowseName, $"The variable \"{Log.Node(enumerationVariable)}\" linked to the EnumVariable property is not a valid enumeration instance");
            return;
        }

        enumerationDataType = variableDataType;
        if (enumerationDataType.Children.Count == 0 || enumerationDataType.GetByType<IUAVariable>() == null)
        {
            Log.Error(Owner.BrowseName, $"The Enumeration datatype of node \"{Log.Node(enumerationVariable)}\" linked to the EnumVariable property not contain any valid children variable");
            return;
        }

        enumerationVariable.VariableChange += UpdateLabelText;
        UpdateLabelText(null, null);
    }

    private void UpdateLabelText(object sender, VariableChangeEventArgs e)
    {
        LocalizedText enumerationDisplayName = null;
        var labelText = new LocalizedText("", "");
        if (Owner.GetVariable("Default text")?.Value != null)
        {
            labelText = Owner.GetVariable("Default text").Value;
        }

        var enumerationValues = enumerationDataType.Children[0] as IUAVariable;
        if (enumerationValues.BrowseName == "EnumValues")
        {
            var enumerationValueStruct = (Struct[])enumerationValues.Value.Value;
            var matchingEnum = enumerationValueStruct
                .Select(x => new OpcUaEnumeration(x.Values.ToArray()))
                .FirstOrDefault(enumeration => enumeration.Value == enumerationVariable.Value);

            enumerationDisplayName = matchingEnum?.DisplayName;
        }
        else if (enumerationValues.BrowseName == "EnumStrings")
        {
            LocalizedText[] enumerationValueArray = (LocalizedText[])enumerationValues.Value.Value;
            if (enumerationValueArray.Length > 0 && enumerationVariable.Value < enumerationValueArray.Length && enumerationVariable.Value >= 0)
            {
                enumerationDisplayName = enumerationValueArray[enumerationVariable.Value];
            }
        }

        if (enumerationDisplayName != null)
        {
            labelText = enumerationDisplayName;
        }

        ((Label)Owner).TextVariable.Value = labelText;
    }

    public override void Stop()
    {
        if (enumerationVariable != null)
        {
            enumerationVariable.VariableChange -= UpdateLabelText;
        }
    }

    private IUAVariable enumerationVariable;
    private IUADataType enumerationDataType;

    private sealed class OpcUaEnumeration
    {
        public OpcUaEnumeration(object[] structValues)
        {
            if (structValues.Length != 3)
                throw new ArgumentException("Wrong array length");
            if (!int.TryParse(structValues[0].ToString(), out int i))
                throw new ArgumentException("Index 0 is not an integer");
            if (structValues[1] is not LocalizedText)
                throw new ArgumentException("Index 1 is not a LocalizedText");
            if (structValues[2] is not LocalizedText)
                throw new ArgumentException("Index 2 is not a LocalizedText");
            Value = i;
            DisplayName = (LocalizedText)structValues[1];
            Description = (LocalizedText)structValues[2];
        }

        public int Value { get; }
        public LocalizedText DisplayName { get; }
        public LocalizedText Description { get; }
    }
}
