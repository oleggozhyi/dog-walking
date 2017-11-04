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
    let getCustomer = Id.parse >>=> Dal.getCustomer 
    let getCustomers = Dal.getCustomers
