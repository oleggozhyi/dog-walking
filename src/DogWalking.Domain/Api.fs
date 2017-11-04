namespace DogWalking

open Suave
open Suave.Filters
open Suave.Successful
open Suave.Operators
open DogWalking.AppServices
open DogWalking.HttpHelpers
open Boundary.InputDto

module Api = 
    open Suave.RequestErrors

    let app = choose [ POST   >=> path "/customers" >=>   (readsPayloadReturns204 CustomersService.addCustomer)
                       GET    >=> path "/customers" >=>   (returns200 CustomersService.getCustomers) 
                       GET    >=> pathScan "customers/%s" (fun id -> returns200 (fun () -> CustomersService.getCustomer id))
                       DELETE >=> pathScan "customers/%s" (fun id -> returns204 (fun () -> CustomersService.removeCustomer id))

                       NOT_FOUND "No such endpoint" ]

