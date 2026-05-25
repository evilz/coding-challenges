open System
#r "../packages/expecto/8.10.1/lib/netstandard2.0/Expecto.dll"
#r "netstandard"

(*
    # Get product of all other elements

    Given an array of integers, return a new array such that each element at 
    index i of the new array is the product of all the numbers in original 
    array except the one at i.


    ## Solution

    Compute the product of all numbers before i and after i, then multiply them 
    Complexity is 3n
*)

open Expecto

let getOtherProduct input = 

  let length = input |> Array.length
  
  let left = Array.create length 1
  for i = 1 to length - 1 do
    left.[i] <- left.[i - 1] * input.[i - 1]

  let right = Array.create length 1
  for i = length - 2 downto 0 do
    right.[i] <- right.[i + 1] * input.[i + 1]

  let productArray = [| for i = 0 to length - 1 do
                          yield left.[i] * right.[i] |]
  productArray


let tests =
  testCase "getOtherProduct" <| fun () ->
    let array = [|1; 2; 3; 4; 5|]
    let expected = [|120; 60; 40; 30; 24|]
    let result = getOtherProduct array
    Expect.sequenceEqual result expected  "getOtherProduct should be the expected"
  

runTests defaultConfig tests

// let getOtherProduct