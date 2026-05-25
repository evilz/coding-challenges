open System
#r "../packages/expecto/8.10.1/lib/netstandard2.0/Expecto.dll"
#r "netstandard"

(*
    # Find Anagram Indices

    Given a word `w` and a string `s`, find all indices in `s` witch are the starting locations of anagrams of `w`.
    For exemple given `w` is ab and s is abxaba, return [0, 3, 4] 


    ## Solution

    Counter chars, and compare count
*)

open Expecto
open System

let oldcounter (s:string) = 
 (Map.empty,s.ToCharArray()) ||> Array.fold (fun counter c -> 
        if not (counter.ContainsKey(c))
        then counter.Add(c,1)
        else 
            let old = counter.[c]
            counter.Add(c,old+1) )


let counter = Seq.countBy id >> Map.ofSeq
   

let isAnagram (a: Map<char,int>) (b: Map<char,int>) = 
    a = b 

let findAnagramIndicesBruteForce (word:string) (s:string) = 

    let wordCounts = word |> counter
    [
        for i in 0..(s.Length - word.Length) do
            let sub = s.Substring(i,word.Length)
            let subCount = sub |> counter
            if subCount |> isAnagram wordCounts 
            then yield i
    ]

let removeIfZero key (map: Map<char,int>) =
    match map.TryFind key with
    | Some x -> if x = 0 then map.Remove key else map
    | _ -> map

let inc key (map: Map<char,int>) =
    map.TryFind key 
    |> Option.defaultValue 0
    |> fun x -> map.Add(key,x+1)

let dec key (map: Map<char,int>) =
    map.TryFind key 
    |> Option.defaultValue 0
    |> fun x -> map.Add(key,x-1)
    
let findAnagramIndicesSmart (word:string) (s:string) = 

    let wordCounts = word |> counter

    [
        let initialInputCount = 
            (wordCounts, (s.Substring(0, word.Length) |> (fun s -> s.ToCharArray())))
            ||> Array.fold (fun counts c -> 
                let actual = counts.TryFind c
                if actual.IsSome 
                then counts.Add(c, actual.Value - 1) |> removeIfZero c
                else counts
            )

        // first index
        if initialInputCount.IsEmpty then
            yield 0
            
        let indices = 
            ((initialInputCount, List.empty),[for i in word.Length..(s.Length-1) -> i,s.[i-word.Length], s.[i]])
            ||> List.fold (fun (counts,indices) (i, startChar,endChar) -> 

                counts 
                  |> inc startChar 
                  |> removeIfZero startChar
                  |> dec endChar
                  |> removeIfZero endChar
                  |> (fun x -> 
                        if x.IsEmpty 
                        then x,(i - word.Length + 1) :: indices    
                        else x, indices)
            )  
            |> snd |> List.rev

        yield! indices
    ]
    

let tests =
  testList "findAnagramIndices" [
  testCase "findAnagramIndicesBruteForce" <| fun () ->
    let word = "ab"
    let input = "abxaba"
    let result = findAnagramIndicesBruteForce word input
    let expected = [0; 3; 4] 
    Expect.sequenceEqual result expected  "findAnagramIndicesBruteForce should find 3 indices"

  testCase "findAnagramIndicesSmart" <| fun () ->
    let word = "ab"
    let input = "abxaba"
    let result = findAnagramIndicesSmart word input
    let expected = [0; 3; 4] 
    Expect.sequenceEqual result expected  "findAnagramIndicesSmart should find 3 indices"
  ]
  

runTests defaultConfig tests
