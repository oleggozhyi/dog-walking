namespace DogWalking.AppServices 

open DogWalking
open DogWalking.Helpers
open DogWalking.Control.Railway
open DogWalking.Domain.DogsAndCustomers

module CustomersService =
    let addCustomer = Customer.createFromDto >=> Dal.addCustomer
    let removeCustomer = Id.parse >=> Dal.removeCustomer
    let updateCustomer = Customer.createFromDto >=> Dal.updateCustomer
    let getCustomer = Id.parse >=> Dal.getCustomer
    let getCustomers = Dal.getCustomers
