open System
open System.Security.Cryptography.X509Certificates
#r "../packages/expecto/8.10.1/lib/netstandard2.0/Expecto.dll"
#r "netstandard"

(*
    # Generate Palindrome Pair

    Given a list of words, find all pairs of unique indices such that concatenation of the two words is a palindrome.

    For exemple, given the list ["code"; "edoc"; "da"; "d"], return [(0,1); (1,0); (2;3)]

    ## Solution

    Counter chars, and compare count
*)

open Expecto
open System


let isPalindrom (s1:string) = 
    s1 = String(s1.ToCharArray() |> Array.rev)

// O(n*m)
let generatePalindromePairs (words: string list) = 

    let wordsIndexed = words |> List.indexed

    [for w1 in wordsIndexed do
        for w2 in wordsIndexed do
        
        let index1, word1 = w1
        let index2, word2 = w2
        if index1 <> index2 && isPalindrom (word1 + word2) then
            yield index1,index2
    ]


let generatePalindromePairs2(words: string list) = 

    let wordsIndexed = words |> List.indexed

    let dico = wordsIndexed |> List.map (fun (a,b) -> b,a) |> Map.ofList

    let matchFn index s1 rev = 
        match s1 |> isPalindrom, dico.TryFind rev with
               | true, Some j when j <> index -> Seq.singleton (index, j)
               | _ -> Seq.empty

    printfn "%A" dico

    [for i, word in wordsIndexed do
        let matchFn = matchFn i

        for charIndex in 0..(word.Length-1) do

        let prefix = word.Substring(0,charIndex)
        let suffix = word.Substring(charIndex)

        let revPrefix = prefix.ToCharArray() |> Array.rev |> String
        let revSuffix = suffix.ToCharArray() |> Array.rev |> String

        yield! matchFn suffix revPrefix
        yield! matchFn prefix revSuffix
            
    ]

let tests =
  testList "generatePalindromePairs" [
      testCase "generatePalindromePairs" <| fun () ->
        let words = ["code"; "edoc"; "da"; "d"]
        let result = generatePalindromePairs words
        let expected = [(0,1); (1,0); (2,3)]
        Expect.sequenceEqual result expected  "generatePalindromePairs should generate 3 tuples"

      testCase "generatePalindromePairs2" <| fun () ->
        let words = ["code"; "edoc"; "da"; "d"]
        let result = generatePalindromePairs2 words
        let expected = [(0,1); (1,0); (2,3)]
        Expect.sequenceEqual result expected  "generatePalindromePairs2 should generate 3 tuples"

  ]

runTests defaultConfig tests