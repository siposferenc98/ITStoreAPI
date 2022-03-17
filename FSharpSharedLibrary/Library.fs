module FSharpLib
open EFCoreShared.DB
open System.Collections.Generic

let md5 (input : string) : string =
    use md5 = System.Security.Cryptography.MD5.Create()
    input
    |> System.Text.Encoding.ASCII.GetBytes
    |> md5.ComputeHash
    |> Seq.map (fun c -> c.ToString("x2"))
    |> Seq.reduce (+)