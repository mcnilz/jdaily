namespace JiraBoard.UiCatalog

open System
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Hosts
open Avalonia.Themes.Fluent
open Elmish

type MainWindow() as this =
    inherit HostWindow()

    do
        base.Title <- "JiraBoard UI Catalog"
        base.Width <- CatalogShell.layout.InitialWindowWidth
        base.Height <- CatalogShell.layout.InitialWindowHeight
        base.MinWidth <- CatalogShell.layout.MinimumWindowWidth
        base.MinHeight <- CatalogShell.layout.MinimumWindowHeight

        Program.mkSimple CatalogShell.init CatalogShell.update CatalogView.view
        |> Program.withHost this
        |> Program.run

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->
            desktop.MainWindow <- MainWindow()
        | _ -> ()

        base.OnFrameworkInitializationCompleted()

module Program =
    [<EntryPoint; STAThread>]
    let main args =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args)
