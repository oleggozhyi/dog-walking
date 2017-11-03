namespace DogWalking 

open Domain.DogsAndCustomers
module Dal = 
    let private customers = new ResizeArray<Customer>()

    let addCustomer customer = customers.Add customer |> Ok

    let getCustomers() = seq<Customer> customers |> Ok

    let getCustomer id = 
        customers 
        |> Seq.tryFind (fun c -> c.Id = id) 
        |> Ok

    let removeCustomer id = 
        customers 
        |> Seq.tryFind (fun c -> c.Id = id) 
        |> Option.iter (fun c -> customers.Remove c |> ignore)
        |> Ok
    
    let updateCustomer customer = 
        customers 
        |> Seq.tryFind (fun c -> c.Id = customer.Id) 
        |> Option.iter (fun c -> customers.Remove c |> ignore
                                 customers.Add customer |> ignore )
        |> Ok