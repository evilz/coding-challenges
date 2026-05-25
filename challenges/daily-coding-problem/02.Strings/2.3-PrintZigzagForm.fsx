open System
open System.Security.Cryptography.X509Certificates
#r "../packages/expecto/8.10.1/lib/netstandard2.0/Expecto.dll"
#r "netstandard"

(*
    # Print zigzag form

    Given a string and a number of line k, print the string in zigzag form.
    In zigzag, characters are printed out diagonally from top left to bottom right until reaching the kth line,
    then back up to top right, and so on.
    
    For exemple, given the sentence "thisiszigzag" and k = 4, you should print:
    t     a     g
     h   s z   a 
      i i   i z  
       s     g


    ## Solution

   
*)

open Expecto
open System


let isPalindrom (s1:string) = 
    s1 = String(s1.ToCharArray() |> Array.rev)


let printZigzag (word:string) (k:int) = 
    
    // +1 char for \n
    let initialState = 0,0,true, [| for i in 0..k-1 -> Array.create (word.Length) ' ' |]

    let (_,_, _, lines) = ( initialState , word.ToCharArray())
                        ||> Array.fold (fun (line, col, goDown, lines ) c -> 
                            

                            lines.[line].[col] <- c

                            let line, goDown = 
                                match line, goDown with
                                | l, true when l = k-1 ->   line-1,false
                                | 0, false ->               1, true
                                | _, true ->                line+1,true
                                | _, false ->               line-1, false

                            let col = col+1

                            (line, col, goDown, lines)
                        )

    String.Join("\n", lines |> Array.map String)



let tests =
  testList "printZigzag" [
      testCase "printZigzag" <| fun () ->
        let word = "thisisazigzag"
        let k = 4
        
        let expected = "t     a     g
 h   s z   a 
  i i   i z  
   s     g   "

        let result = printZigzag word k
        Expect.sequenceEqual result expected  "printZigzag should print word in zigzag"

      
  ]

runTests defaultConfig tests
