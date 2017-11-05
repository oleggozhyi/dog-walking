namespace DogWalking

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open DogWalking.AppServices
open DogWalking.HttpHelpers
open DogWalking.Control

module Api = 

    let app = 
        choose 
            [ 
             // customers
              POST   >=> path "/customers"      >=> request (readPayload >>=> CustomersService.addCustomer >> handleEmptyResult)
              GET    >=> path "/customers"      >=> warbler (fun _ -> CustomersService.getCustomers() |> handleResult)
              GET    >=> path "/customers-fail" >=> warbler (fun _ -> CustomersService.getCustomersWithFailure() |> handleResult)
              GET    >=> pathScan "/customers/%s"      (fun id -> CustomersService.getCustomer id |> handleResultWithOption)
              DELETE >=> pathScan "/customers/%s"      (fun id -> CustomersService.removeCustomer id |> handleEmptyResult)
                                                   
              //schedule                           
              GET    >=> path "/schedule" >=>  warbler (fun _ -> WalkingService.calcSchedule() |> handleResult)

              NOT_FOUND (toJsonMessage "No such endpoint") ]
        >=> Writers.setMimeType "application/json; charset=utf-8"

