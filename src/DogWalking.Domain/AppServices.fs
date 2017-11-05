namespace DogWalking.AppServices 

open System
open DogWalking
open DogWalking.Helpers
open DogWalking.Control.Railway
open DogWalking.Domain.DogsAndCustomers
open DogWalking.Domain.Price
open DogWalking.Domain

[<AutoOpen>]
module OutputModels = 
    type DogDto = { 
        Id : Guid
        Name: string;
        IsSmall: bool;
        IsAggressive: bool;
        Price: decimal<USD>
    }
    
    type CustomerDto = {
        Id : Guid
        Name: string;
        Phone: string;
        Email: string;
        Dogs: DogDto list
    }

    type DayWalksDto = {
        DayNumber: int;
        MorningWalk: DogDto list
        EveningWalk: DogDto list
        ProfitUsd: decimal<USD>
     }

    let createDogDto (dog: Dog) = 
        { Id = dog.Id; 
          Name = dog.Name |> Option.toString Name.valueOf;
          IsSmall = dog.IsSmall;
          IsAggressive = dog.IsAggressive;
          Price = calcDoWalkPrice dog }

    let createCustomerDto (customer: Customer) =
        { CustomerDto.Id = customer.Id;
          Name = customer.Name |> Name.valueOf;
          Email = customer.Email |> Option.toString Email.valueOf
          Phone = customer.Phone |> Phone.valueOf
          Dogs = customer.Dogs |> List.map createDogDto }

    let createDayWalksScheduleDto (schedule: WalingSchedule) =
        let (Schedule days) = schedule
        days |> List.mapi (fun dayNumber walkDay -> 
                            let (WalksForDay (morningWalk, eveningWalk)) = walkDay
                            { DayNumber = (dayNumber + 1);
                              MorningWalk = morningWalk |> DogPack.getDogs |> List.map createDogDto
                              EveningWalk = eveningWalk |> DogPack.getDogs |> List.map createDogDto
                              ProfitUsd = calDayProfit walkDay })

module CustomersService =
    let addCustomer = Customer.createFromDto >>=> Dal.addCustomer
    let removeCustomer = Id.parse >>=> Dal.removeCustomer
    let getCustomer idStr = result {
        let! id =  idStr |> Id.parse 
        let! customerOpt = Dal.getCustomer id
        let customerDto = customerOpt |> Option.map createCustomerDto
        return customerDto
    }
    let getCustomers() = result {
        let! customers = Dal.getCustomers() 
        let dtos = customers |> Seq.map createCustomerDto
        return dtos 
    }
    let getCustomersWithFailure() = result {
        let! customers = Dal.getCustomersWithFailure() 
        let dtos = customers |> Seq.map createCustomerDto
        return dtos 
    }

module WalkingService = 
    let calcSchedule() =  result {
        let! customers = Dal.getCustomers()
        return customers 
              |> WalingSchedule.create 
              |> createDayWalksScheduleDto
    }


