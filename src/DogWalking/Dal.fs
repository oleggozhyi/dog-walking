namespace DogWalking 

open DogWalking.Domain.DogsAndCustomers
open DogWalking.Control

module Dal = 
    let private customers = new ResizeArray<Customer>()

    let addCustomer customer = customers.Add customer |> Ok

    let getCustomers() = seq<Customer> customers |> Ok

    let getCustomersWithFailure(): Result<seq<Customer>, Failure> = Failure.failure "Db connection can't be established"

    let getCustomer id = 
        customers 
        |> Seq.tryFind (fun c -> c.Id = id) 
        |> Ok

    let removeCustomer id = 
        customers 
        |> Seq.tryFind (fun c -> c.Id = id) 
        |> Option.iter (fun c -> customers.Remove c |> ignore)
        |> Ok
