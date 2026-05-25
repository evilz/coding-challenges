namespace DailyCodingProblem
open Types

module Arrays =

    type Problems = ProductOfthers | SmallestSortedWindow | MaxSumSubArray | SmallerElementsToTheRight


    let productOfthers = 
        {
            title=  "Get product of all other elements"
            description = "Given an array of integers, return a new arrray such that each element at index i of
the new array is the product of all the numbers in the original array except the one at i."

            solution= "The value at ith element is the product of all numbers beore i and after i. We can store this in two temp arrays."
            fsharpAnswerFile= "productOfthers.fs"
        }

    let menuItems = 
        [
            (ProductOfthers , productOfthers)
            (SmallestSortedWindow , {Problem.empty with title = "SmallestSortedWindow"})
            (MaxSumSubArray , {Problem.empty with title = "MaxSumSubArray"})
            (SmallerElementsToTheRight , {Problem.empty with title = "SmallerElementsToTheRight"})
        ] |> Map.ofList
    
    let getProblemTitle p =
        menuItems.Item p


    
    
    
