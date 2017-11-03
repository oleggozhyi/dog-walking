namespace DogWalking.Boundary

[<AutoOpen>]
module InputDto = 
    type DogDto = { 
        Name: string;
        IsSmall: bool;
        IsAggressive: bool  
    }
    
    type CustomerDto = {
        Name: string;
        Phone: string;
        Email: string;
        Dogs: DogDto list
    }

module OutputDto = 
    open System

    type DogResponse = { 
        Id : Guid
        Name: string;
        IsSmall: bool;
        IsAggressive: bool  
    }
    
    type CustomerResponse = {
        Id : Guid
        Name: string;
        Phone: string;
        Email: string;
        Dogs: DogResponse list
    }
