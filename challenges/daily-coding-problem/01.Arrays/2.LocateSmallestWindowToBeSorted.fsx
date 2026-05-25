#r "../packages/expecto/8.10.1/lib/netstandard2.0/Expecto.dll"
#r "netstandard"

(*
    # Get product of all other elements

    Given an array of integers that are out of order, determine the bounds of
    the smallest window that must be sorted in order for the entire array to be
    sorted.
    For exemple, given [|3;7;5;6;9|] you should return (1,3)


    ## Solution

    
*)

open System
open Expecto

let locateSmallestWindowToBeSorted (input:int array) = 

    let getRight (maxFound, right) (idx, current) = 
        match max maxFound current with
        | x when current < x -> x,idx 
        | x -> x, right
        
    let (maxFound,right) =  input 
                            |> Array.indexed
                            |> Array.fold  getRight (Int32.MinValue, 0)
                            
    let getLeft (minFound, left) (idx, current) = 
        match min minFound current with
        | x when current > x -> x,idx 
        | x -> x, left
        
    let (minFound,left) =  input 
                            |> Array.indexed
                            |> Array.rev
                            |> Array.fold  getLeft (Int32.MaxValue, 0)

    (left,right)


let locateSmallestWindowToBeSorted2 input =
    let runningMins = Array.mapFoldBack (fun acc i -> (min acc i, min acc i)) input System.Int32.MaxValue |> fst
    let runningMaxs = Array.mapFold (fun acc i -> (max acc i, max acc i)) System.Int32.MinValue input |> fst
    let merged = Array.zip3 runningMins runningMaxs input
    let left = Array.tryFindIndex (fun (min, _, num) -> min <> num) merged
    let right = Array.tryFindIndexBack (fun (_, max, num) -> max <> num) merged
    (left, right)

let tests =
  testCase "locateSmallestWindowToBeSorted" <| fun () ->
    let array = [|3;7;5;6;9|]
    let expected = (1, 3)
    let result = locateSmallestWindowToBeSorted array
    Expect.equal result expected  "locateSmallestWindowToBeSorted should be the expected"

    let result2 = locateSmallestWindowToBeSorted2 array
    Expect.equal result2 (Some 1, Some 3)  "locateSmallestWindowToBeSorted2 should be the expected"
  

runTests defaultConfig tests
