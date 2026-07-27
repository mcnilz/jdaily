namespace JiraBoard.App

open System.IO

type private LicenseNoticesAssemblyMarker = class end

[<RequireQualifiedAccess>]
module LicenseNotices =
    [<Literal>]
    let private ResourceName = "JiraBoard.App.THIRD-PARTY-NOTICES.txt"

    let read () =
        let assembly = typeof<LicenseNoticesAssemblyMarker>.Assembly

        use stream = assembly.GetManifestResourceStream ResourceName

        if isNull stream then
            invalidOp $"Embedded resource '{ResourceName}' is missing."

        use reader = new StreamReader(stream)
        reader.ReadToEnd()
