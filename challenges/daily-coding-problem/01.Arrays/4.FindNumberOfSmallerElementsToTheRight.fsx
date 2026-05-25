open System
#r "../packages/expecto/8.10.1/lib/netstandard2.0/Expecto.dll"
#r "netstandard"

(*
    # Find Number Of Smaller Elements To The Right

    Given an array of of numbers, return a new array where each element in the new
    array is the numbers of smaller elements to the right of that element in the 
    original input array.

    Given [|3; 4; 9; 6 ; 1|] return [|1; 1; 2 ;1; 0|]


    ## Solution
  Iterate over the array from right,keep track in sorted array number of smaller elements seen

*)

open Expecto

let bisectLeft (array: 'a array) item =
    let rec loop (array:'a array) item low high =
      if   low < 0     then failwith "lo must be non-negative"
      elif low >= high then low
      else
          let mid = (low + high) >>> 1
          match array.[mid] < item with
          | true -> loop array item (mid+1) high 
          | false -> loop array item low mid 
      
    loop array item 0 (array |> Array.length)

let findNumberOfSmallerElementsToTheRight (input: int array) = 
    let state = [],[]
    (input ,state)
    ||> Array.foldBack (fun item (result, seen) ->
      let i = bisectLeft (seen |> Array.ofList) item// not so good ?
      //printfn "%A" i
      let result = i :: result
      let seen = (item :: seen) |> List.sort
      result,seen
      ) 
    |> fst |> Array.ofList
    


let tests =
  testCase "findNumberOfSmallerElementsToTheRight" <| fun () ->
    let array = [|3; 4; 9; 6 ; 1|] 
    let expected = [|1; 1; 2 ;1; 0|]
    let result = findNumberOfSmallerElementsToTheRight array
    Expect.sequenceEqual result expected  "calculateMaximumSubarraySum should be the expected"
  

runTests defaultConfig tests
