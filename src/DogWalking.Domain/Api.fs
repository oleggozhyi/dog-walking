namespace DogWalking

module Api = 

    open Suave
    open Suave.Filters
    open Suave.Successful
    open Suave.Operators
    open DogWalking.AppServices
    let app = 
        choose [
            GET >=> choose [
                //path "/customers" >=> (OK CustomersService.getCustomers())
                path "/store" >=> (OK "Store")
                path "/store/browse" >=> (OK "Browse")
                path "/store/details" >=> (OK "Details")
            ]
        ]