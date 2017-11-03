namespace DogWalking.Domain

open System
open System.Text.RegularExpressions
open Dto

module DomainTypes =

    /// Email primitive wrapper
    [<Struct>]
    type Email = Email of string 
        with static member create s =
                if Regex.IsMatch(s, @"^\S+@\S+\.\S+$") then Email s |> Ok
                else Error ["Email format is invalid"]
    
    /// Phone primitive wrapper
    [<Struct>] 
    type Phone = Phone of string
        with static member create s =
                if Regex.IsMatch(s, @"^[\d\(\)+\s-]$") then Phone s |> Ok
                else Error ["Phone format is invalid"]

    /// Human's or dog's name primitive wrapper
    [<Struct>]  
    type Name = Name of string
        with static member create s =
                match String.length s with 
                | l when l < 2 -> Error ["Name is too short"]
                | l when l >= 50 -> Error ["Name should be less than 50"]
                | _ -> Name s |> Ok

    type Dog = {
        Id: Guid;
        Name: Name;
        IsSmall: bool;
        IsAggressive: bool
    } with 
        static member create name isSmall isAggressive = 
            { Id = Guid.NewGuid(); Name = name; IsSmall = isSmall; IsAggressive = isAggressive }
        static member createFromDto (dogDto: DogDto) = 
            Dog.create <!> Name.create dogDto.Name
                       <*> Ok dogDto.IsSmall
                       <*> Ok dogDto.IsAggressive

    type Customer = {
        Id: Guid;
        Name: Name;
        Phone: Phone;
        Email: Email;
        Dogs: Dog list
    } with
        static member create name phone email dogs =
            { Id = Guid.NewGuid(); Name = name; Phone = phone; Email = email; Dogs = dogs }
        static member createFromDto (customerDto: CustomerDto) = 
            Customer.create 
            <!> Name.create customerDto.Name
            <*> Phone.create customerDto.Phone
            <*> Email.create customerDto.Email
            <*> traverseResultM Dog.createFromDto customerDto.Dogs
    
    [<Struct>]
    type ThreeDogsOrLess = ThreeDogsOrLess of Dog list
        with static member splitBy3 dogs = 
              dogs |> List.splitInto 3 |> List.map ThreeDogsOrLess   

    [<Struct>]
    type FiveDogsOrLess = FiveDogsOrLess of Dog list
        with static member splitBy5 dogs = 
              dogs |> List.splitInto 5 |> List.map FiveDogsOrLess

    type DogPack = 
        | AggressiveSmallDog of Dog
        | AggressiveBigDog of Dog
        | SmallDogsPack of FiveDogsOrLess
        | BigDogsPack of ThreeDogsOrLess

    let getSmallDogs = List.filter (fun d -> d.IsSmall && not d.IsAggressive)
    let getSmallAggressiveDogs = List.filter (fun d -> d.IsSmall && d.IsAggressive)
    let getBigDogs = List.filter (fun d -> not d.IsSmall && not d.IsAggressive)
    let getBigAggressiveDogs = List.filter (fun d -> not d.IsSmall && d.IsAggressive)
    
    let createPacks dogs = [
        yield! dogs |> getSmallAggressiveDogs  |> List.map AggressiveSmallDog
        yield! dogs |> getBigAggressiveDogs |> List.map AggressiveBigDog
        yield! dogs |> getSmallDogs |>  FiveDogsOrLess.splitBy5 |> List.map SmallDogsPack
        yield! dogs |> getBigDogs |>  ThreeDogsOrLess.splitBy3 |> List.map BigDogsPack
    ]

    let getAllDogs customers = 
        customers |> List.map (fun c -> c.Dogs) |> List.concat
    

       
    let scheduleNext allPacks lastDogPack = 
        let allPacksDoubled = allPacks @ allPacks // protection against odd numbers of packs
        allPacksDoubled 
        |> List.skipWhile (fun pack -> pack <> lastDogPack) 
        |> List.take 2 
        
