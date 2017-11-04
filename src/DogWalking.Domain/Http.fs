namespace DogWalking

open Suave
open Suave.Successful
open Suave.Operators
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open DogWalking.Control
open Suave.ServerErrors
open Suave.RequestErrors
open System
open System.Text

module HttpHelpers =
    type ErrorMessage = { Message: string }

    let private defaultJsonSettings = new  JsonSerializerSettings (ContractResolver = new CamelCasePropertyNamesContractResolver())

    let toJson o = JsonConvert.SerializeObject(o, defaultJsonSettings) 
        
    let toJsonMessage s = { Message = s } |> toJson
    let fromJson<'a> json =
        try
            JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a |> Ok
        with 
            | :? Exception as ex -> Failure.validation [ex.ToString()]

    let JSON_OK o = toJson o |> OK
       
    let handleFailure = function
        | Failure e -> e |> INTERNAL_ERROR
        | Validation es -> es |> toJson |> BAD_REQUEST

    let handleResult = function
        | Ok x -> x |> JSON_OK
        | Error e -> e |> handleFailure

    let handleEmptyResult = function
        | Ok () -> NO_CONTENT
        | Error e -> e |> handleFailure
    
    let handleResultWithOption = function
        | Ok (Some x) ->  x |> JSON_OK
        | Ok None ->  "resource with this id was not found" |> toJsonMessage |> NOT_FOUND
        | Error e -> e |> handleFailure

    let readPayload<'a> (req : HttpRequest) =
        req.rawForm |> Encoding.UTF8.GetString |> fromJson<'a>
