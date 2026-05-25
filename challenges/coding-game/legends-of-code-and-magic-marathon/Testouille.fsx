open System


type LeftistHeap<'a> =
    | E
    | T of int * 'a * LeftistHeap<'a> * LeftistHeap<'a>
    
let rank = function
    | E -> 0
    | T(r, _, _, _) -> r

let makeT x a b =
    if rank a > rank b 
    then T(rank b + 1, x, a, b)
    else T(rank a + 1, x, b, a)

let rec merge a b =
    match a, b with
    | E, _ -> b
    | _, E -> a
    | T(_, x, a1, a2), T(_, y, b1, b2) ->
        if x < y 
        then makeT x a1 (merge a2 b)
        else makeT y b1 (merge b2 a)

let insert x a = merge a (T(1, x, E, E))

let findMin = function
    | E -> failwith "Minimum of an empty Heap"
    | T(_, x, _, _) -> x

let deleteMin = function
    | E -> E
    | T(_, _, a, b) -> merge a b

(* Exercise 3.2 Define "insert" directly rather than via a call to merge *)
let rec insert2 x = function
    | E -> T(1, x, E, E)
    | T(r, y, a, b) ->
        if x < y 
        then makeT x a (insert2 y b)
        else makeT y a (insert2 x b)

let fromList xs =
    let rec merge_pairs acc ts =
        match acc, ts with
        | [a], [] -> a
        | _, [] -> merge_pairs [] acc
        | _, a :: [] -> merge_pairs [] (a :: acc)
        | _, a :: b :: tss -> merge_pairs ((merge a b) :: acc) tss
        
    in xs |> List.map (fun x -> T(1, x, E, E)) |> merge_pairs []

let bfs<'a> (start:'a) (getNeighbours:'a -> 'a list) (isEnd: 'a->bool) = 
    let rec innerLoop (q:'a list)  = seq {
        
        match q with
        | [] -> ()
        | h :: t when h |> isEnd -> yield h //Some(h)
        | h :: t -> 
                //yield h
                let q1  = t @ (h |> getNeighbours)
                //let r = 
                yield! innerLoop q1
                // match r with
                // | None -> ()
                // | Some x -> printfn "%A" x
                //r
        
    }

    
    innerLoop [start]


let start = 1
let n a = [a+1; a+2]
let isEnd a = a = 5

bfs start n isEnd


[1;4;7;8;2;0] |> fromList



-9.000000 <  -4.800000