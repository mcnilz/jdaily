namespace JiraBoard.VisualTests

open System
open System.Diagnostics
open System.IO
open System.Threading
open Avalonia
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI.Types
open Avalonia.Headless
open Avalonia.Skia
open Avalonia.VisualTree

type BoardSurfaceMeasurements =
    { CpuMilliseconds: float
      ManagedMemoryBytes: int64
      FrameMilliseconds: float
      VisualTreeNodeCount: int }

type HeadlessTestApplication() =
    inherit Application()

    static member BuildAvaloniaApp() =
        let options = AvaloniaHeadlessPlatformOptions()
        options.UseHeadlessDrawing <- false

        AppBuilder.Configure<HeadlessTestApplication>().UseSkia().UseHeadless(options)

[<RequireQualifiedAccess>]
module HeadlessTestHost =
    let private sessionGate = obj ()

    let createApplicationBuilder () = HeadlessTestApplication.BuildAvaloniaApp()

    let captureFrame width height (createView: unit -> IView) =
        lock sessionGate (fun () ->
            use session = HeadlessUnitTestSession.StartNew(typeof<HeadlessTestApplication>)

            let renderTask =
                session.Dispatch(
                    (fun () ->
                        let window = HostWindow()

                        window.Width <- float width
                        window.Height <- float height
                        (window :> IViewHost).Update(Some(createView ()))
                        window.Show()

                        let frame = window.CaptureRenderedFrame()
                        window.Close()
                        frame),
                    CancellationToken.None
                )

            renderTask.GetAwaiter().GetResult())

    let capturePngFrame width height (createView: unit -> IView) =
        use frame = captureFrame width height createView
        use png = new MemoryStream()
        frame.Save(png)
        png.ToArray()

    let measurePngFrame width height (createView: unit -> IView) =
        lock sessionGate (fun () ->
            let cpuBefore = Process.GetCurrentProcess().TotalProcessorTime
            let memoryBefore = GC.GetTotalMemory(false)
            let stopwatch = Stopwatch.StartNew()
            use session = HeadlessUnitTestSession.StartNew(typeof<HeadlessTestApplication>)

            let renderTask =
                session.Dispatch(
                    (fun () ->
                        let window = HostWindow()

                        window.Width <- float width
                        window.Height <- float height
                        (window :> IViewHost).Update(Some(createView ()))
                        window.Show()

                        let visualTreeNodeCount = window.GetVisualDescendants() |> Seq.length
                        let frame = window.CaptureRenderedFrame()
                        window.Close()
                        frame, visualTreeNodeCount),
                    CancellationToken.None
                )

            let frame, visualTreeNodeCount = renderTask.GetAwaiter().GetResult()
            stopwatch.Stop()

            use frame = frame
            use png = new MemoryStream()
            frame.Save(png)

            let measurements =
                { CpuMilliseconds = (Process.GetCurrentProcess().TotalProcessorTime - cpuBefore).TotalMilliseconds
                  ManagedMemoryBytes = max 0L (GC.GetTotalMemory(false) - memoryBefore)
                  FrameMilliseconds = stopwatch.Elapsed.TotalMilliseconds
                  VisualTreeNodeCount = visualTreeNodeCount }

            png.ToArray(), measurements)

    let run width height (createView: unit -> IView) (assertion: HostWindow -> unit) =
        lock sessionGate (fun () ->
            use session = HeadlessUnitTestSession.StartNew(typeof<HeadlessTestApplication>)

            let runTask =
                session.Dispatch(
                    (fun () ->
                        let window = HostWindow()

                        try
                            window.Width <- float width
                            window.Height <- float height
                            (window :> IViewHost).Update(Some(createView ()))
                            window.Show()

                            assertion window
                        finally
                            window.Close()),
                    CancellationToken.None
                )

            runTask.GetAwaiter().GetResult())