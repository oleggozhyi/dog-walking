namespace DogWalking.Control


[<AutoOpen>]
module Railway =
    type Failure = 
        | Validation of string list
        | Failure of string

    module Failure =
        let validation xs = xs |> Validation |> Error
        let failure s = s |> Failure |> Error

    let bind xRes f = 
        match xRes with
        | Ok x -> f x
        | Error e -> Error e
    let (>>=) = bind
    let (>>=>) f1 f2 x = f1 x >>= f2 // to distinguish from >=> in Suave

    type ResultBuilder() =
        member this.Bind(x, f) = x >>= f
        member this.Return(x) = Ok x
        member this.ReturnFrom(x) = x
    let result = new ResultBuilder()

    let map f xRes = xRes >>= (f >> Ok)
    let (<!>) = map

    let apply fRes xRes : Result<'a, Failure> = 
        match fRes, xRes with
        | Ok f, Ok x -> f x |> Ok
        | Error e, Ok _ -> Error e
        | Ok _, Error e -> Error e
        | Error (Validation e1), Error (Validation e2) -> e1 @ e2 |> (Validation >> Error)
        | Error (Failure e), _ -> (Failure >> Error) e
        | _, Error (Failure e) -> (Failure >> Error) e
    let (<*>) = apply



