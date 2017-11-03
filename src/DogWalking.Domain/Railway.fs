namespace DogWalking.Control

[<AutoOpen>]
module Railway =
    type ErrorMessageList = string list

    let bind xRes f = 
        match xRes with
        | Ok x -> f x
        | Error e -> Error e
    let (>>=) = bind
    let (>=>) f1 f2 x = f1 x >>= f2

    type ResultBuilder() =
        member this.Bind(x, f) = x >>= f
        member this.Return(x) = Ok x
        member this.ReturnFrom(x) = x
    let result = new ResultBuilder()

    let map f xRes = xRes >>= (f >> Ok)
    let (<!>) = map

    let apply fRes xRes : Result<'a, ErrorMessageList> = 
        match fRes, xRes with
        | Ok f, Ok x -> f x |> Ok
        | Error e, Ok _ -> Error e
        | Ok _, Error e -> Error e
        | Error e1, Error e2 -> e1 @ e2 |> Error
    let (<*>) = apply



