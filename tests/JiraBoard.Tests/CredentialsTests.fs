module JiraBoard.Tests.CredentialsTests

open System
open JiraBoard.App
open Xunit

[<Fact>]
let ``credential store rejects an empty credential identifier without exposing a token`` () =
    let store = Credentials.nativeStore ()
    let token = $"spk-004-{Guid.NewGuid():N}"

    let result = store.Save "" token

    Assert.Equal(Error CredentialStoreError.InvalidCredentialId, result)
    Assert.DoesNotContain(token, string result)

[<Fact>]
let ``credential store rejects an empty token`` () =
    let store = Credentials.nativeStore ()

    Assert.Equal(Error CredentialStoreError.InvalidToken, store.Save "spk-004" "")

[<Fact>]
let ``Windows credential store round trips and removes a disposable credential`` () =
    if OperatingSystem.IsWindows() then
        let store = Credentials.nativeStore ()
        let credentialId = $"spk-004-{Guid.NewGuid():N}"
        let token = $"spk-004-{Guid.NewGuid():N}"

        try
            Assert.Equal(Ok(), store.Save credentialId token)
            Assert.Equal(Ok(Some token), store.Read credentialId)
            Assert.Equal(Ok(), store.Delete credentialId)
            Assert.Equal(Ok None, store.Read credentialId)
        finally
            store.Delete credentialId |> ignore