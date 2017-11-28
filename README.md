# DynamicPropertyObject
A tool to drive WinForms PropertyGrid from dynamic data

This is a cleaned-up and encapsulated version of https://www.codeproject.com/Articles/415070/Dynamic-Type-Description-Framework-for-PropertyGri

This version is specifically for driving the Windows.Forms.PropertyGrid control from dynamic data

## To do

- NuGet package

## Usage

### Population

Create a new container object, and use the `AddProperty` extension method to add fields to be rendered in the property grid control.

```csharp
var props = DynamicPropertyObject.NewObject();
props.AddProperty(key:"Property1", displayName:"Property One", description:"This was generated", initialValue:"init value", standardValues:new []{"Option 1", "Option 2" });
props.AddProperty(key:"Property2", displayName:"Property Two", description:"An enum", initialValue: MyEnum.One);
```

### Set-up

Add the resulting object as the `SelectedObject` on your property grid.
If you make any further changes to the property object, make sure you call `.Refresh()` on the PropertyGrid control.

```csharp
MyPropGrid.SelectedObject = props;
. . .
MyPropGrid.Refresh();
```

### Reading results

The current values can be read directly out of the property object by using the `key` name supplied to the `AddProperty` method.

```csharp
// Direct access to the object:
var currentValue = props["Property1"]);

// Reference through the PropertyGrid control:
var currentValue = ((PropertyTarget) JourneyStatusGrid.SelectedObject)["Property1"];
```

