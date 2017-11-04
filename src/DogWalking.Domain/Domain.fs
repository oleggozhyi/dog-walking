namespace DogWalking.Domain

open System
open System.Text.RegularExpressions
open DogWalking
open DogWalking.Boundary
open DogWalking.Helpers
open DogWalking.Control
open DogWalking.Boundary.OutputDto

[<AutoOpen>]
module PrimitiveWappers =
    type Email = Email of string 
    type Phone = Phone of string
    type Name = Name of string

    module Email =
        let create = function
                | null | "" -> Failure.validation ["Email can't be empty"]
                | s when Regex.IsMatch(s, @"^\S+@\S+\.\S+$") ->  Email s |> Ok
                | s -> Failure.validation ["Email format is invalid: " + s]
        let  valueOf email = let (Email s) = email in s

    module Phone =
        let valueOf phone = let (Phone s) = phone in s
        let create = function
            | null | "" -> Failure.validation ["Phone can't be empty"]
            | s when  Regex.IsMatch(s, @"^[\d\(\)+\s-]+$") ->  Phone s |> Ok
            | s -> Failure.validation ["Phone format is invalid: " + s]

    module Name =
        let valueOf name = let (Name s) = name in s
        let create = function
                | null | "" -> Failure.validation ["Name can't be empty"]
                | s when String.length s < 2 -> Failure.validation ["Name is too short"]
                | s when String.length s >= 30 -> Failure.validation ["Name should be less than 30: " + s]
                | s -> Name s |> Ok

[<AutoOpen>]
module DogsAndCustomers = 
    type Dog = { Id: Guid;
                 Name: Name option;
                 IsSmall: bool;
                 IsAggressive: bool }

    type Customer = { Id: Guid;
                      Name: Name;
                      Phone: Phone;
                      Email: Email option;
                      Dogs: Dog list } 

    module Dog = 
        let isSmallAggressive dog = dog.IsSmall && dog.IsAggressive
        let isSmall dog = dog.IsSmall && not dog.IsAggressive
        let isBigAggressive dog = not dog.IsSmall && dog.IsAggressive
        let isBig dog = not dog.IsSmall && not dog.IsAggressive

        let create name isSmall isAggressive = 
            { Id = Guid.NewGuid(); Name = name; IsSmall = isSmall; IsAggressive = isAggressive }

        let createFromDto (dogDto: DogDto) = 
            let dogName = dogDto.Name |> Option.ofObj |> Option.traverseResultA  Name.create 
            in create <!> dogName
                   <*> Ok dogDto.IsSmall
                   <*> Ok dogDto.IsAggressive

        let createDto (dog: Dog) = { DogResponse.Id = dog.Id; 
                                     Name = dog.Name |> Option.toString Name.valueOf;
                                     IsSmall = dog.IsSmall;
                                     IsAggressive = dog.IsAggressive }

    module Customer = 
        let getAllDogs customers = customers |> List.map (fun c -> c.Dogs) |> List.concat

        let create name phone email dogs =
            { Id = Guid.NewGuid(); Name = name; Phone = phone; Email = email; Dogs = dogs }

        let createFromDto (customerDto: CustomerDto) = 
            let email = Option.ofObj customerDto.Email |> Option.traverseResultA Email.create
            create <!> Name.create customerDto.Name
                   <*> Phone.create customerDto.Phone
                   <*> email
                   <*> List.traverseResultA Dog.createFromDto customerDto.Dogs

        let createDto customer = { CustomerResponse.Id = customer.Id;
                                   Name = customer.Name |> Name.valueOf;
                                   Email = customer.Email |> Option.toString Email.valueOf
                                   Phone = customer.Phone |> Phone.valueOf
                                   Dogs = customer.Dogs |> List.map Dog.createDto }

    type DogPack = 
        | AggressiveSmallDog of Dog
        | AggressiveBigDog of Dog
        | SmallDogsPack of FiveOrLess<Dog>
        | BigDogsPack of ThreeOrLess<Dog>

    module DogPack =
        let createPacks dogs = 
             [ yield! dogs |> List.filter Dog.isSmallAggressive  |> List.map AggressiveSmallDog
               yield! dogs |> List.filter Dog.isBigAggressive |> List.map AggressiveBigDog
               yield! dogs |> List.filter Dog.isSmall |> List.splitBy5 |> List.map SmallDogsPack
               yield! dogs |> List.filter Dog.isBig |> List.splitBy3 |> List.map BigDogsPack ]

[<AutoOpen>]
module Schedule = 
    type WalksForDay = WalksForDay of morningWalk: DogPack * eveningWalk: DogPack
    type WalingSchedule = Schedule of  WalksForDay list

    module WalingSchedule =
        let create = Customer.getAllDogs 
                     >> DogPack.createPacks 
                     >> List.splitItoPairs 
                     >> List.map WalksForDay