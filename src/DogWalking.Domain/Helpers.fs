namespace DogWalking.Helpers

open System
open DogWalking.Control

type ThreeOrLess<'a> = ThreeOrLess of 'a list
type FiveOrLess<'a> = FiveOrLess of 'a list

module List =
    let splitBy3 list = list|> List.chunkBySize 3 |> List.map ThreeOrLess

    let splitBy5 list = list |> List.chunkBySize 5 |> List.map FiveOrLess

    let splitItoPairs list = 
        let rec splitItoPairsRec acc getFirstEl = function
        | [] -> acc
        | [x1] -> (x1, getFirstEl())::acc
        | x1::x2::tail -> splitItoPairsRec ((x1,x2)::acc) getFirstEl tail
        splitItoPairsRec [] (fun ()-> List.head list) list

    let traverseResultM f list =
        let folder head tail = result {
            let! h = f head
            let! t = tail
            return h::t
         } in List.foldBack folder list (Ok []) 

    let traverseResultA f list =
        let cons head tail = head :: tail
        let folder head tail = 
            cons <!> f head <*> tail
        List.foldBack folder list (Ok []) 

module Option = 
    let traverseResultA f = function
        | None -> Ok None 
        | Some x -> Ok Some <*> f x

    let toString (f:'a -> string) opt  = opt |> Option.map f |> Option.toObj

module Id = 
    let parse id = 
        match Guid.TryParse id with 
        | true, guid -> Ok guid
        | false, _ -> Error ["Expected Id as Guid but found: " + id]

module Http = 
    open Suave
    //ype WebPart<'a> = 'a -> Async<'a option>
    let toWebPart (result: Result<'a,ErrorMessageList>) (httpContext: HttpContext) : Async<HttpContext option> = 
        { httpContext with result = 
            {
                status        
            }}



