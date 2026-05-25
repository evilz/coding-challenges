open System
#r "../packages/expecto/8.10.1/lib/netstandard2.0/Expecto.dll"
#r "netstandard"

(*
    # Add Two Linked Lists Numbers

    For exemple given 
    9 -> 9
    5 -> 2
   
   you should return 4->2->1
*)

open Expecto
open System



let addNumberLists l1 l2 =

    let initialState = (0,[])

    let (carry, sumList) = 
        List.zip l1 l2
        |> List.fold (fun (carry, sumList) (x,y) -> 
            let sum = x + y + carry
            if sum >= 10 
            then 1, (sum - 10) :: sumList
            else 0, sum :: sumList 
        ) initialState

    if carry > 0 
        then carry :: sumList
        else sumList
    |> List.rev
    

let tests =
  testList "addNumberLists" [
      testCase "addNumberLists" <| fun () ->

        let l1 = [9;9] 
        let l2 = [5;2]

        let expected = [4;2;1] 
        Expect.sequenceEqual (addNumberLists l1 l2) expected  "linked list should be reversed"

      
  ]

runTests defaultConfig tests
