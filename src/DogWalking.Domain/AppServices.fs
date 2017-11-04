namespace DogWalking.AppServices 

open DogWalking
open DogWalking.HttpHelpers
open DogWalking.Helpers
open DogWalking.Control.Railway
open DogWalking.Domain.DogsAndCustomers
open DogWalking.Boundary

module CustomersService =
    let addCustomer = Customer.createFromDto >>=> Dal.addCustomer
    let removeCustomer = Id.parse >>=> Dal.removeCustomer
    let getCustomer idStr = result {
            let! id =  idStr|>Id.parse 
            let! customerOpt = Dal.getCustomer id
            let customerDto = customerOpt |> Option.map Customer.createDto 
            return customerDto
        }
    let getCustomers() = result {
            let! customers = Dal.getCustomers()
            let dtos = customers |> Seq.map Customer.createDto
            return dtos 
        }

