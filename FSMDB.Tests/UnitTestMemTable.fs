module FSMDB.Tests.MemTable

open NUnit.Framework
open FSMDB.MemTable

[<TestFixture>]
type TestMemTable() =
    [<Test>]
    static member public Set() =
        let table = MemTable.Create()

        table.Set(
            [ ("OCaml", "Real World OCaml 2nd") // 5 + 20 + 9
              ("F#", "Stylish F# 6") // 2 + 12 + 9
              ("Rust", "The Rust Programming Language 2nd") ] // 4 + 33 + 9
        )

        match table.Get("F#") with
        | Ok(entry) ->
            Assert.AreEqual(false, entry.Deleted)
            Assert.AreEqual("F#", entry.Key)
            Assert.AreEqual(Some("Stylish F# 6"), entry.Value)

            Assert.AreEqual(103, table.Size)
        | Error() -> Assert.Fail()

    [<Test>]
    static member public Overrite() =
        let table = MemTable.Create()

        table.Set("OCaml", "Real World OCaml 2nd")
        table.Set("OCaml", "Practical OCaml") // 5 + 15 + 9

        match table.Get("OCaml") with
        | Ok(entry) ->
            Assert.AreEqual(false, entry.Deleted)
            Assert.AreEqual("OCaml", entry.Key)
            Assert.AreEqual(Some("Practical OCaml"), entry.Value)

            Assert.AreEqual(29, table.Size)
        | Error() -> Assert.Fail()

    [<Test>]
    static member public NotExists() =
        let table = MemTable.Create()

        table.Set("OCaml", "Real World OCaml 2nd")

        match table.Get("F#") with
        | Ok(entry) -> Assert.Fail()
        | Error() -> Assert.Pass()


    [<Test>]
    static member public DeleteExists() =
        let table = MemTable.Create()

        table.Set("OCaml", "Real World OCaml 2nd")
        table.Delete("OCaml")

        match table.Get("OCaml") with
        | Ok(entry) ->
            Assert.AreEqual(true, entry.Deleted)
            Assert.AreEqual("OCaml", entry.Key)
            Assert.AreEqual(None, entry.Value)
            Assert.AreEqual(14, table.Size)
        | Error() -> Assert.Fail()

    [<Test>]
    static member public DeleteNotExists() =
        let table = MemTable.Create()

        table.Set("OCaml", "Real World OCaml 2nd") // 5 + 20 + 9
        table.Delete("F#") // 2 + 0 + 9

        match table.Get("F#") with
        | Ok(entry) ->
            Assert.AreEqual(true, entry.Deleted)
            Assert.AreEqual("F#", entry.Key)
            Assert.AreEqual(None, entry.Value)
            Assert.AreEqual(34 + 11, table.Size)
        | Error() -> Assert.Fail()

        match table.Get("OCaml") with
        | Ok(entry) ->
            Assert.AreEqual(false, entry.Deleted)
            Assert.AreEqual("OCaml", entry.Key)
            Assert.AreEqual(Some("Real World OCaml 2nd"), entry.Value)
        | Error() -> Assert.Fail()
