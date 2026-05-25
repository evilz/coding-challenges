

// [<EntryPoint>]
// let main args =
    
//     let state = {
//         me= {health = 30; mana = 5; deck = 2; rune = 5}
//         opponent= {health = 30; mana = 0; deck = 2; rune = 5}
        
//         myHand = { 
//                 greenItems= Set.empty; 
//                 minion= Set.empty.Add(MinionInfo {
//                     cardNumber = 2;
//                     instanceId= 2;
//                     location= CardLocation.InMyHand;
//                     cardType= CardType.Creature;
//                     cost= 2;
//                     attack= 1;
//                     defense= 1;
//                     abilities= {breakthrough=false;charge=false;guard=false;drain=false;lethal=false;ward=false}
//                     myHealthChange= 0;
//                     opponentHealthChange= 0;
//                     cardDraw= 0
//                 }).Add(MinionInfo {
//                     cardNumber = 2;
//                     instanceId= 3;
//                     location= CardLocation.InMyHand;
//                     cardType= CardType.Creature;
//                     cost= 2;
//                     attack= 10;
//                     defense= 1;
//                     abilities= {breakthrough=false;charge=true;guard=false;drain=false;lethal=false;ward=false}
//                     myHealthChange= 0;
//                     opponentHealthChange= 0;
//                     cardDraw= 0
//                 }) 
//                 redItems= Set.empty; 
//                 blueItems= Set.empty}
//         myBoard= Set.empty.Add( MinionInfo {
//                     cardNumber = 1;
//                     instanceId= 1;
//                     location= CardLocation.OnMyBoard;
//                     cardType= CardType.Creature;
//                     cost= 2;
//                     attack= 10;
//                     defense= 10;
//                     abilities= {breakthrough=false;charge=false;guard=false;drain=false;lethal=false;ward=false}
//                     myHealthChange= 0;
//                     opponentHealthChange= 0;
//                     cardDraw= 0
//                 })
//         oppBoard= Set.empty
//         opponentHandCount= 4
//         myCardDraw = 1
//         haveBeenUsed= Set.empty
//     }

//     let results = GameLogic.bfs state

//     results |> Seq.iter (fun (s,p) -> printfn "- %f %A" (s|>score) p)  // REV path


//     let state, actions = GameLogic.playBattle state List.empty
//     let score = state |> score
//     0