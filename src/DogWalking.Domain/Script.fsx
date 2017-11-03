// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Railway.fs"
#load "Dto.fs"
#load "Domain.fs"

open DogWalking.Boundary
open DogWalking.Control
open DogWalking.Domain

let dog1 = { Name ="Big1"; IsSmall = false; IsAggressive = true }
let dog2 = { Name ="Smal1"; IsSmall = true; IsAggressive = true }
let dog3 = { Name = "Big2"; IsSmall = false; IsAggressive = false }
let customerDto1 = {     Name = "John"; Phone = "+1 23 456 789";  Email = null; Dogs = [dog1;dog2;dog3] }

let dog11 = { Name ="Small2"; IsSmall = true; IsAggressive = false }
let dog12 = { Name ="Small3"; IsSmall = true; IsAggressive = false }
let dog13 = { Name = "Big3"; IsSmall = false; IsAggressive = false }
let customerDto2 = {     Name = "Ian"; Phone = "+1 987 654 321"; Email = "aaa@qqq.xx"; Dogs = [dog11;dog12;dog13] }

let schedule = result {
    let! customer1 = Customer.createFromDto customerDto1
    let! customer2 = Customer.createFromDto customerDto2
    
    return [customer1; customer2] |> WalingSchedule.create
    }




// Define your library scripting code here

