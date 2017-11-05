namespace DogWalking.Boundary

[<AutoOpen>]
module InputDto = 
    type DogInputDto = { 
        Name: string;
        IsSmall: bool;
        IsAggressive: bool  
    }
    
    type CustomerInputDto = {
        Name: string;
        Phone: string;
        Email: string;
        Dogs: DogInputDto list
    }
