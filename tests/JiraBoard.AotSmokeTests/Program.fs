module JiraBoard.AotSmokeTests.Program

open System
open System.IO
open System.Security.Cryptography
open System.Text

[<EntryPoint>]
let main _ =
    let notices: string = JiraBoard.App.LicenseNotices.read ()

    let requiredMarkers =
        [| "SkiaSharp.NativeAssets.* 2.88.9"
           "HarfBuzzSharp.NativeAssets.* 8.3.1.1"
           "DNG SDK License Agreement"
           "Version: MPL 1.1/GPL 2.0/LGPL 2.1"
           "GNU LESSER GENERAL PUBLIC LICENSE"
           "The FreeType Project LICENSE"
           "ICU License"
           "libjpeg-turbo Licenses" |]

    let mutable missingMarker: string = null

    for marker in requiredMarkers do
        if isNull missingMarker && not (notices.Contains marker) then
            missingMarker <- marker

    if not (isNull missingMarker) then
        Console.Error.WriteLine $"Missing third-party notice marker: {missingMarker}"
        1
    elif String.IsNullOrWhiteSpace notices then
        Console.Error.WriteLine "Third-party notices are empty."
        1
    else
        let distributedPath =
            Path.Combine(AppContext.BaseDirectory, "THIRD-PARTY-NOTICES.txt")

        if not (File.Exists distributedPath) then
            Console.Error.WriteLine $"Distributed third-party notices are missing: {distributedPath}"
            1
        elif File.ReadAllText distributedPath <> notices then
            Console.Error.WriteLine "Distributed and embedded third-party notices differ."
            1
        else
            let vendorStart = notices.IndexOf "THIRD-PARTY SOFTWARE NOTICES AND INFORMATION"

            if vendorStart < 0 then
                Console.Error.WriteLine "The verbatim native vendor notice is missing."
                1
            else
                let vendorNotice =
                    notices.Substring(vendorStart).Replace("\r\n", "\n")

                let vendorHash =
                    Convert
                        .ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes vendorNotice))
                        .ToLowerInvariant()

                let expectedVendorHash =
                    "98acf9d4d6083959988c884f630cdff760f94bfeb9acf57774653e08c23d1e45"

                if vendorHash <> expectedVendorHash then
                    Console.Error.WriteLine $"Native vendor notice hash differs: {vendorHash}"
                    1
                else
                    0
