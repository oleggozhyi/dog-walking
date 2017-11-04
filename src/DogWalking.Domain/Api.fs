namespace DogWalking

open Suave
open Suave.Filters
open Suave.Operators
open DogWalking.AppServices
open DogWalking.HttpHelpers

module Api = 
    open Suave.RequestErrors
    open DogWalking.Control

    let app = 
        choose 
            [ POST   >=> path "/customers" >=> request (readPayload >>=> CustomersService.addCustomer >> handleEmptyResult)
              GET    >=> path "/customers" >=> warbler (fun _ -> CustomersService.getCustomers() |> handleResult)
              GET    >=> pathScan "/customers/%s"      (fun id -> CustomersService.getCustomer id |> handleResultWithOption)
              DELETE >=> pathScan "/customers/%s"      (fun id -> CustomersService.removeCustomer id |> handleEmptyResult)
              NOT_FOUND (toJsonMessage "No such endpoint") ]
        >=> Writers.setMimeType "application/json; charset=utf-8"

