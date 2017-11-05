namespace DogWalking.Tests

open FsCheck
open FsCheck.Xunit
open DogWalking.Domain

module ``Dog walking domain logic tests`` = 

    [<Property>]
    let ``Email should countain [at]`` (s: string) =
        (s <> null && s.Contains "@") 
            ==> match Email.create s with
                | Ok _ -> true
                | Error _ -> false


