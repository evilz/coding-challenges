

// private IEnumerable<string> BFS(Node root)
// {
//     var queue = new Queue<Tuple<string, Node>>();
//     queue.Enqueue(new Tuple<string, Node>(root.Name, root));

//     while (queue.Any())
//     {
//         var node = queue.Dequeue();
//         if (node.Item2.Children.Any())
//         {
//             foreach (var child in node.Item2.Children)
//             {
//                 queue.Enqueue(new Tuple<string, Node>(node.Item1 + "-" + child.Name, child));
//             }
//         }
//         else
//         {
//             yield return node.Item1;
//         }
//     }
// }



type Action =
    | Pick of int
    | Pass // PASS pour passer son tour.
    | Summon of obj //SUMMON id pour invoquer la créature id.
    | AttackEnnemy of obj * obj //ATTACK id1 id2 pour attaquer la créature id2 avec la créature id1.
    | AttackOpponent of obj // ATTACK id -1 pour attaquer l'adversaire avec la créature id.
    | UseGreenItem of obj * obj // USE id1 id2 pour utiliser l'objet id1 sur la créature id2.
    | UseRedItem of obj * obj // USE id1 id2 pour utiliser l'objet id1 sur la créature id2.
    | UseBlueItem of obj * obj option //pour utiliser l'objet id.



type GameBattle = {
    me: obj;
    opponent: obj
    
    myHand: obj
    myBoard: Set<int>
    oppBoard: Set<int>

    opponentHandCount: int

    myCardDraw : int
    
    haveBeenUsed: Set<int>
}






(* ### Sortest path ###

A star : fast just need priority queue
astar<T> (start:T) getNeighbours  getCost heuristic isEnd = null)  // should return path

T = state * action
getNeighbours = allposibleactions
getCost = score inverted
heuristic = 30 - oppenent health
isEnd = oppenont health = 0  
*)



module Astar =
    open System.Collections.Generic
    module PriorityQueue =
        let empty<'TPriority,'TItem> = new SortedDictionary<'TPriority, Queue<'TItem>>() //Comparer<TPriority>.Default);

        let enqueue<'TPriority,'TItem>  (priority:'TPriority, item:'TItem) (pq:SortedDictionary<'TPriority, Queue<'TItem>>) =
            match pq.ContainsKey priority with
            | true -> pq.[priority].Enqueue(item);
            | false -> 
                let q = new Queue<'TItem>()
                q.Enqueue item

        let dequeue<'TPriority, 'TItem> (pq:SortedDictionary<'TPriority, Queue<'TItem>>) =
            match pq.Count with
            | 0 -> failwith "The queue is empty"
            | _ ->
                let first = pq |> Seq.head
                let nextItem = first.Value.Dequeue()
                match first.Value.Count with
                | 0 -> pq.Remove first.Key
                | _ -> false
                |> ignore
                new KeyValuePair<'TPriority, 'TItem>(first.Key, nextItem)
                
        let toSeq<'TPriority, 'TItem> (pq:SortedDictionary<'TPriority, Queue<'TItem>>) =
            pq |> Seq.collect (fun pair -> pair.Value |> Seq.map (fun item -> new KeyValuePair<TPriority, TItem>(pair.Key, item))
            


    // let astar<'a when 'a : comparison> (start:'a) (getNeighbours:'a -> 'a list) getCost  heuristic (isEnd: 'a->bool) = 
        
    //     let rec getPath (visitFrom:Map<'a,'a>) next = seq{
    //         match visitFrom.TryFind next with
    //         | None -> ()
    //         | Some x-> 
    //             yield x
    //             match x = start with
    //             | true -> ()
    //             | false -> yield! getPath visitFrom x
    //     }

    //     let rec innerLoop (toVisit:LeftistHeap<'a>) (visitFrom:Map<'a,'a*float>) = seq {
            
    //         //         foreach (var x in neighboursWithCost)
    //         //         {
    //         //             var priority = x.NewCost + heuristic(x.Neighbour);
    //         //             toVisit.Enqueue(priority, x.Neighbour);
    //         //             visitedFromAndCost[x.Neighbour] = new Tuple<T, int>(current, x.NewCost);
    //         //         }

            
    //         match toVisit with
    //         | E -> ()
    //         | T ->
    //             let currentNode = toVisit |> findMin
    //             let current = currentNode;
    //             var currentCost = currentNode.Key;
    //         | [] -> ()
    //         | h :: t when h |> isEnd -> 
    //             //yield h
    //             yield! getPath visitFrom h
    //         | h :: t -> 
    //                 yield h
    //                 let neighbours = h 
    //                                 |> getNeighbours 
    //                                 |> List.filter (fun n -> visitFrom.ContainsKey n |> not )
    //                                 |> List.map (fun n ->
    //                                     let visitedFrom, cost = visitFrom.[n]
    //                                     let newCost = cost + (getCost h n)
    //                                     n, newCost)
    //                 let toVisit, visitedFrom = 
    //                     ( (toVisit,visitFrom) , neighbours) 
    //                     ||> List.fold (fun (x, y) n -> 
    //                         x.
    //                         v.Add(n,h) )
    //                 yield! innerLoop (t @ neighbours) visitedFrom
    //     }

        
    //     innerLoop [start] Map.empty // |> Seq.rev




    