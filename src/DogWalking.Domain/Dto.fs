namespace DogWalking.Domain

module Dto = 

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

