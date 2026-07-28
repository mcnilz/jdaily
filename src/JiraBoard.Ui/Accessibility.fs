namespace JiraBoard.Ui

open Avalonia
open Avalonia.Automation
open Avalonia.FuncUI.Builder
open Avalonia.FuncUI.Types

[<RequireQualifiedAccess>]
module Accessibility =
    let name<'control when 'control :> AvaloniaObject> value : IAttr<'control> =
        AttrBuilder<'control>.CreateProperty<string>(
            AutomationProperties.NameProperty,
            value,
            ValueNone
        )
