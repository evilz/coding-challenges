open System
#r "../packages/expecto/8.10.1/lib/netstandard2.0/Expecto.dll"
#r "netstandard"

(*
    # Calculate maximum subarray sum

    Given an array of of numbers, find the maximum sum of any contiguous subarray of the array.
    For Example, given the array [|34; -50; 42; 14; -5; 86 |] the maximum sum would be 137


    ## Solution
  Iterate over the array ,keep track of the maximimun subarray we've seen so far, if there is a better update the value

  This algo is known as Kadane's algorithm
    
*)

open Expecto

let calculateMaximumSubarraySum input = 
    input 
    |> Array.fold (fun (a, b) item ->
                    let maxEndingHere = (max item (a + item))
                    let maxSoFar = (max b maxEndingHere)
                    //printfn "%i %i" maxEndingHere maxSoFar
                    (maxEndingHere, maxSoFar)
    )(0,0) |> fst
    


let tests =
  testCase "calculateMaximumSubarraySum" <| fun () ->
    let array = [|34; -50; 42; 14; -5; 86 |]
    let expected = 137
    let result = calculateMaximumSubarraySum array
    Expect.equal result expected  "calculateMaximumSubarraySum should be the expected"
  

runTests defaultConfig tests