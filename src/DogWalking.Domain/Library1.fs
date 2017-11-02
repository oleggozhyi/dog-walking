namespace DogWalking.Domain
open System

type Email = Email of string
type Name = Name of string


type Dog = {
    Id: Guid;
    Name: string;
    Small: bool;
    Aggressive: bool
}

type Customer = {
    Id: Guid;
    Name: string;
    Phone: string;
    Email: string;
    Dogs: Dog list
}
type Pack =  Pack of Dog list

