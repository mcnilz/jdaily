namespace JiraBoard.App

open System
open System.Runtime.InteropServices
open System.Text

[<RequireQualifiedAccess>]
type CredentialStoreError =
    | InvalidCredentialId
    | InvalidToken
    | NotFound
    | AccessDenied
    | StoreUnavailable
    | OperationFailed

type CredentialStore = {
    Save: string -> string -> Result<unit, CredentialStoreError>
    Read: string -> Result<string option, CredentialStoreError>
    Delete: string -> Result<unit, CredentialStoreError>
}

[<Struct; StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)>]
type private NativeCredential =
    val mutable Flags: uint32
    val mutable Type: uint32
    val mutable TargetName: IntPtr
    val mutable Comment: IntPtr
    val mutable LastWrittenLowDateTime: uint32
    val mutable LastWrittenHighDateTime: uint32
    val mutable CredentialBlobSize: uint32
    val mutable CredentialBlob: IntPtr
    val mutable Persist: uint32
    val mutable AttributeCount: uint32
    val mutable Attributes: IntPtr
    val mutable TargetAlias: IntPtr
    val mutable UserName: IntPtr

module private NativeMethods =
    [<Literal>]
    let CredentialTypeGeneric = 1u

    [<Literal>]
    let CredentialPersistLocalMachine = 2u

    [<DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)>]
    extern bool CredWrite(NativeCredential& credential, uint32 flags)

    [<DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)>]
    extern bool CredRead(string targetName, uint32 credentialType, uint32 flags, IntPtr& credential)

    [<DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)>]
    extern bool CredDelete(string targetName, uint32 credentialType, uint32 flags)

    [<DllImport("Advapi32.dll", SetLastError = true)>]
    extern unit CredFree(IntPtr credential)

[<RequireQualifiedAccess>]
module Credentials =
    let private prefix = "JiraBoard/"

    let private errorFromLastWin32Error () =
        match Marshal.GetLastWin32Error() with
        | 5 -> CredentialStoreError.AccessDenied
        | 1168 -> CredentialStoreError.NotFound
        | 1312 -> CredentialStoreError.StoreUnavailable
        | _ -> CredentialStoreError.OperationFailed

    let private targetName credentialId =
        prefix + credentialId

    let private validateCredentialId credentialId =
        if String.IsNullOrWhiteSpace credentialId then
            Error CredentialStoreError.InvalidCredentialId
        else
            Ok()

    let private unavailableStore =
        { Save = fun credentialId token ->
              match validateCredentialId credentialId with
              | Error error -> Error error
              | Ok () when String.IsNullOrWhiteSpace token -> Error CredentialStoreError.InvalidToken
              | Ok () -> Error CredentialStoreError.StoreUnavailable
          Read = fun credentialId ->
              match validateCredentialId credentialId with
              | Error error -> Error error
              | Ok () -> Error CredentialStoreError.StoreUnavailable
          Delete = fun credentialId ->
              match validateCredentialId credentialId with
              | Error error -> Error error
              | Ok () -> Error CredentialStoreError.StoreUnavailable }

    let private windowsStore =
        let save credentialId token =
            match validateCredentialId credentialId with
            | Error error -> Error error
            | Ok () when String.IsNullOrWhiteSpace token -> Error CredentialStoreError.InvalidToken
            | Ok () ->
                let target = Marshal.StringToCoTaskMemUni(targetName credentialId)
                let blob = Encoding.Unicode.GetBytes token
                let credentialBlob = Marshal.AllocHGlobal blob.Length

                try
                    Marshal.Copy(blob, 0, credentialBlob, blob.Length)

                    let mutable credential = Unchecked.defaultof<NativeCredential>
                    credential.Type <- NativeMethods.CredentialTypeGeneric
                    credential.TargetName <- target
                    credential.CredentialBlobSize <- uint32 blob.Length
                    credential.CredentialBlob <- credentialBlob
                    credential.Persist <- NativeMethods.CredentialPersistLocalMachine

                    if NativeMethods.CredWrite(&credential, 0u) then
                        Ok()
                    else
                        Error(errorFromLastWin32Error ())
                finally
                    Marshal.FreeHGlobal credentialBlob
                    Marshal.FreeCoTaskMem target

        let read credentialId =
            match validateCredentialId credentialId with
            | Error error -> Error error
            | Ok () ->
                let mutable credentialPointer = IntPtr.Zero

                if NativeMethods.CredRead(targetName credentialId, NativeMethods.CredentialTypeGeneric, 0u, &credentialPointer) then
                    try
                        let credential = Marshal.PtrToStructure<NativeCredential> credentialPointer
                        let blob = Array.zeroCreate<byte> (int credential.CredentialBlobSize)
                        Marshal.Copy(credential.CredentialBlob, blob, 0, blob.Length)
                        Ok(Some(Encoding.Unicode.GetString blob))
                    finally
                        NativeMethods.CredFree credentialPointer
                else
                    match errorFromLastWin32Error () with
                    | CredentialStoreError.NotFound -> Ok None
                    | error -> Error error

        let delete credentialId =
            match validateCredentialId credentialId with
            | Error error -> Error error
            | Ok () ->
                if NativeMethods.CredDelete(targetName credentialId, NativeMethods.CredentialTypeGeneric, 0u) then
                    Ok()
                else
                    match errorFromLastWin32Error () with
                    | CredentialStoreError.NotFound -> Ok()
                    | error -> Error error

        { Save = save
          Read = read
          Delete = delete }

    let nativeStore () =
        if OperatingSystem.IsWindows() then windowsStore else unavailableStore