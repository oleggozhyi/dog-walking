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

    let private defaultJsonSettings = new  JsonSerializerSettings (ContractResolver = new CamelCasePropertyNamesContractResolver())

    let toJson o = JsonConvert.SerializeObject(o, defaultJsonSettings) 

    let fromJson<'a> json =
        try
            JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a |> Ok
        with 
            | :? Exception as ex -> Failure.validation [ex.ToString()]

    let JSON_OK o = toJson o |> OK >=> Writers.setMimeType "application/json; charset=utf-8"
       
    let handleResult = function
        | Ok x -> x |> JSON_OK
        | Error (Failure e) -> e |> INTERNAL_ERROR
        | Error (Validation es) -> es |> toJson |> BAD_REQUEST

    let handleEmptyResult = function
        | Ok () -> NO_CONTENT
        | Error (Failure e) -> e |> INTERNAL_ERROR
        | Error (Validation es) -> es |> toJson |> BAD_REQUEST

    let readPayload<'a> (req : HttpRequest) =
        req.rawForm |> Encoding.UTF8.GetString |> fromJson<'a>

    let readsPayloadReturns200 f = request (readPayload >>=> f >> handleResult)
    let returns200 f = request (fun _ -> f() |> handleResult)
    let returns204 f = request (fun _ -> f() |> handleEmptyResult)
    let readsPayloadReturns204 f = request(readPayload >>=> f >> handleEmptyResult)
    
    