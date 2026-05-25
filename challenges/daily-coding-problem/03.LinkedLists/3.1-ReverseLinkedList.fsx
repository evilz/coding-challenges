open System
#r "../packages/expecto/8.10.1/lib/netstandard2.0/Expecto.dll"
#r "netstandard"

(*
    # Given the head of a singly linked list, reverse it in place 
   
*)

open Expecto
open System


type Node<'a> = { mutable next:Node<'a> option; data:'a }

let reverseLinkedList root =
    
  let mutable prev = None
  let mutable current = Some root
  while current.IsSome do
    let tmp = current.Value.next
    current.Value.next <- prev
    prev <- current
    current <- tmp
  
  prev


let tests =
  testList "reverseLinkedList" [
      testCase "reverseLinkedList" <| fun () ->

        let l = { 
                    data = 1;
                    next = Some{
                        data=2
                        next= Some {
                            data = 3
                            next = None
                        }
                    }
                    }
        let r = l |> reverseLinkedList

        let expected = { 
                    data = 3;
                    next = Some{
                        data=2
                        next= Some {
                            data = 1
                            next = None
                        }
                    }
                    }

        Expect.equal r.Value expected  "linked list should be reversed"

      
  ]

runTests defaultConfig tests
