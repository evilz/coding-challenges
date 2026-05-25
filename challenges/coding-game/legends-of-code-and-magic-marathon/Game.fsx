open System

[<AutoOpen>]
module Helpers =
    /// Splits the given string at the given delimiter
    let inline split (delimiter:char) (text:string) = text.Split [|delimiter|]

    let inline readLine() = Console.ReadLine()

    let shuffleR (r : Random) xs = xs |> Seq.sortBy (fun _ -> r.Next()) 
    let shuffle xs = xs |> shuffleR (Random())
    let shuffleG xs = xs |> Seq.sortBy (fun _ -> Guid.NewGuid())

    
    let headAndTail s = 
        let s = s|> Seq.toList
        match s with
        | [] -> None, []
        | [x] -> Some x, []
        | x::xs -> Some x, xs

//#region TYPES

type Health = int
type Mana = int
type Rune = int

type Player = {health: Health; mana: Mana; deck:int; rune: Rune}
module Player =
    
    let create line = 
        //eprintfn "line %s" line 
        let token = line |> split ' ' |> Array.map int
        {health =  token.[0]; mana =  token.[1]; deck =  token.[2]; rune =  token.[3]} 
    let empty =  {health = 0; mana = 0; deck = 0; rune = 0} 


type CardLocation = InMyHand = 0 | OnMyBoard = 1 | OnOpponentBoard = -1
type CardType = Summon = 0 | GreenItem = 1 | RedItem = 2 | BlueItem = 3

module Ability =
    let parseAbilities (s:string) =
        
        let abilities = s.ToCharArray()
        abilities.[0] = 'B',
        abilities.[1] = 'C',
        abilities.[2] = 'G',
        abilities.[3] = 'D',
        abilities.[4] = 'L',
        abilities.[5] = 'W'

type Card = {
    cardNumber : int;
    instanceId:int;
    location: CardLocation;
    cardType:CardType;
    cost:Mana;
    attack:int;
    defense:int;
    breakthrough: bool; //(Percée) : une créature avec Breakthrough peut infliger des dégâts supplémentaires à l'adversaire quand elle attaque une autre créature. Si ses dégâts d'attaque sont supérieurs à la défense de la créature défenseuse, alors l'excès de dégâts est infligé au joueur défenseur.
    charge: bool; // : une créature avec Charge peut attaquer le tour où elle est invoquée.
    guard: bool; //(Garde) : une créature attaquante doit attaquer une creature avec Guard du joueur défenseur en priorité.
    drain: bool; // (Vol de vie) : une créature avec Drain ajoute autant de PVs que de dégâts qu'elle inflige (quand elle attaque seulement).
    lethal: bool; //  (Létalité) : une créature avec Lethal tue toutes les créatures auxquelles elle inflige des dégâts.
    ward: bool; // (Bouclier) : une créature avec Ward ignore les prochains dégâts qu'elle se verrait recevoir (une seule fois). Le "bouclier" procuré par la capacité Ward est ensuite perdu.

    myHealthChange:Health;
    opponentHealthChange:Health;
    cardDraw:int
    } with // TODO CHANGE THIS TO FUNCTIONS
    member this.breakthroughVal with get() = if this.breakthrough then 5 else 0
    member this.chargeVal with get() = if this.charge then 5 else 0
    member this.guardVal with get() = if this.guard then 5 else 0
    member this.drainVal with get() = if this.drain then 5 else 0
    member this.lethalVal with get() = if this.lethal then 5 else 0
    member this.wardVal with get() = if this.ward then 5 else 0

module Card =
    let create line = 
        //eprintfn "CARD LINE : %s" line
        let token = line |> split ' '
        let breakthrough, charge, guard, drain, lethal, ward = token.[7] |> Ability.parseAbilities
        {   cardNumber = token.[0] |> int
            instanceId = token.[1] |> int
            location = token.[2] |> int |> enum<CardLocation>
            cardType = token.[3] |> int |> enum<CardType>
            cost = token.[4] |> int
            attack = token.[5] |> int
            defense = token.[6] |> int
            //abilities = token.[7] |> Ability.parseAbilities
            myHealthChange = token.[8] |> int
            opponentHealthChange = token.[9] |> int
            cardDraw = token.[10] |> int
            breakthrough = breakthrough
            charge = charge
            guard = guard
            drain = drain
            lethal = lethal
            ward = ward
        }
        

    let isSummon c = c.cardType = CardType.Summon
    
    let isDead c = 
        let dead = c.defense <= 0
        if dead then eprintfn "Card #%i will die" c.instanceId
        dead

    let attack (mine) (ennemy) =
        let newEnnemy = {ennemy with defense = (ennemy.defense) - (mine.attack) }
        let newMine = {mine with defense = mine.defense - ennemy.attack}
        newMine , newEnnemy

    let summons cards = cards |> List.filter isSummon

    let breakthroughVal card = if card.breakthrough then 5 else 0

    let value card = 
        ( card.defense 
        + card.attack
        + card.breakthroughVal
        + card.chargeVal
        + card.drainVal
        + card.guardVal
        + card.lethalVal
        + card.wardVal) / card.cost

    let cardsByLocation (cards:Card seq) =  cards |> Seq.toList |> List.groupBy (fun x -> x.location) |> Map.ofList

type Deck = {
    inDeck: Card list
    rejected: Card list
}

module Deck =
    let addIn deck card rejected  = {deck with inDeck = (card :: deck.inDeck); rejected = (deck.rejected @ rejected)}
    let empty = {inDeck = list.Empty; rejected = list.Empty;}

type GameDraft = {
    cards: Card list
}

open Player
open Card
open Deck

type Command =
    | Pick of int
    | Pass // PASS pour passer son tour.
    | Summon of int //SUMMON id pour invoquer la créature id.
    | AttackSummon of int * int //ATTACK id1 id2 pour attaquer la créature id2 avec la créature id1.
    | AttackOpponet of int // ATTACK id -1 pour attaquer l'adversaire avec la créature id.

module Command = 

    let private getCommandStr cmd = 
        match cmd with
            | Pick id -> sprintf "PICK %i" id
            | Pass -> "PASS"
            | Summon id -> sprintf "SUMMON %i" id
            | AttackSummon (id1,id2) -> sprintf "ATTACK %i %i" id1 id2
            | AttackOpponet id1 -> sprintf "ATTACK %i -1" id1
    //    eprintfn  "EXECUTE %s" strCmd
    //    printf "%s" strCmd

    let executeCommands cmds = 
        match cmds with
        | [c] -> c |> getCommandStr |> printfn "%s" 
        | _ -> cmds |> List.toArray |> Array.map getCommandStr |> String.concat ";" |> printfn "%s"
    
    // say something



type GameBattle = {
    me: Player;
    opponent: Player
    
    myHand: Card list
    myBoard: Card list
    oppBoard: Card list

   // usedORDead: Card list // todo add this
}

module GameBattle =
    let create me opp cards =  
        let cardsByLocation = cards |> cardsByLocation
        let myHand = cardsByLocation.[CardLocation.InMyHand] |> Seq.toList
        let myBoard = cardsByLocation |> Map.tryFind CardLocation.OnMyBoard |> Option.defaultValue  List.empty
        let oppBoard = cardsByLocation|> Map.tryFind CardLocation.OnOpponentBoard |> Option.defaultValue List.empty
        
        { me = me
        ; opponent = opp
        ; myHand = myHand
        ; myBoard = myBoard
        ; oppBoard = oppBoard 
        }
    let empty = {me = Player.empty; opponent = Player.empty; myHand = list.Empty; myBoard= list.Empty; oppBoard= list.Empty}

//#endregion


//#region GAME Functions



let withCostAtMust mana (cards:Card list) = cards |> List.filter (fun x -> x.cost <= mana)


let updateBoardwith cardNewState otherOnBoard =
    match cardNewState |> isDead with
    | true -> otherOnBoard
    | false -> cardNewState :: otherOnBoard


//#endregion

// let draftCards (state:GameDraft) = 
//     state.cards 
//     |> List.mapi (fun i x -> i,x )
//     |> List.sortByDescending (fun (i,x)-> 
//                                 x.abilities.Length,
//                                 x.attack, 
//                                 x.defense)
//     |> List.head
//     |> fun (i,x) -> Pick i
//     |> fun x -> [x]
//     |> executeCommands

let playGame (state:GameBattle) =

    // summon
    let rec getSummon state toPlay = 
        let playableCards = state.myHand |> withCostAtMust state.me.mana
        match playableCards with
        | [] -> toPlay
        | _ -> 
            let (cardplayed, notPlayed) = playableCards |> shuffle |> headAndTail
            match cardplayed with
            | None -> toPlay
            | Some c -> 
                let newState = {state with 
                                        me = {state.me with mana = state.me.mana - (c.cost)}
                                        myHand = notPlayed
                                        }
                let cardsPlayed = c :: toPlay

                match notPlayed , newState.me.mana with
                | [], _ -> cardsPlayed
                | _, 0 -> cardsPlayed
                | _, _ -> getSummon newState cardsPlayed

    eprintfn "Start choose card in hand"
    let cardsToPlay = getSummon state List.empty
    let summonCmds =  match cardsToPlay.Length with
                        | 0 -> [Pass]
                        | _ -> cardsToPlay |> List.map (fun x -> Summon (x.instanceId))
    
    // Attack

    // ADD CARD with CHARGE
    let withCarge = cardsToPlay |> summons |> List.filter haveCharge
    let state = {state with myBoard = state.myBoard @ withCarge}

    let rec getSummonAttack state (commands:Command list) = 
        match state.myBoard  with
        | [] -> commands
        | _ ->
            let (attackWith, myOthers) = state.myBoard |> summons |> shuffle |>  shuffleAndTakeHead
            let oppGuard, oppNotGuard = state.oppBoard |> summons |> List.partition isGuard

            // BUG some where
            let attackTo, oppOthers = 
                match oppGuard with
                | []  ->
                    eprintfn "NO GUARD"
                    state.oppBoard |> shuffleAndTakeHead
                | _ ->
                    eprintfn "FUCKING GUARDS %i" oppGuard.Length
                    let a, b = oppGuard |> shuffleAndTakeHead
                    a, b @ oppNotGuard

            match attackWith, attackTo with
                | None,_ -> commands
                | Some mine , None -> getSummonAttack {state with myBoard = myOthers} (AttackOpponet (mine.instanceId) :: commands)
                | Some mine , Some ennemy -> 
                    let newmine , newEnnemy  = attack mine ennemy
                    let newMine = {mine with defense = mine.defense - ennemy.attack}
                    let myBoard = myOthers |> updateBoardwith newMine
                    let oppBoard = oppOthers |> updateBoardwith newEnnemy 
        
                    getSummonAttack {state with myBoard = myBoard; oppBoard = oppBoard} (AttackSummon (mine.instanceId, ennemy.instanceId) :: commands) // need calc life

    eprintfn "Start attack"
    let AttackCmds = getSummonAttack state List.empty

    let cmds = summonCmds @ AttackCmds

    match cmds with
    | [] -> [Pass] |> executeCommands
    | cmds -> cmds |> executeCommands

let (|InDraft|InBattle|) mana = 
    match mana with
    | 0 -> InDraft
    | _ -> InBattle

let play state =
    match state with
    | Draft d -> draftCards d
    | GameBattle b -> playGame b



type GameState = {
        turn:int
        myDeck:Deck
        battle:GameBattle
    }
module GameState =
    let empty = {turn = 0; myDeck = Deck.empty; battle = GameBattle.empty }  

open GameState
open Command
module GameLogic =

    let playCards state =

        let rec selectCards playableCards selected =
            match playableCards  with
            | [] -> selected
            | _ ->
                

        let cardsToPlay = 
        state 
    
    let playAttacks state = 

        state 

    let playDraft state cards=
        let picked, rejected = 
            cards 
            |> List.mapi (fun i x -> i,x )
            |> List.sortByDescending (fun (i,x)-> x |> value)
            |> fun x -> (x|> List.head, x|> List.tail)

        let pickedIndex, pickedCard = picked
        [Pick pickedIndex] |> executeCommands

        let rejected = rejected |> List.map (fun (i,x) -> x)

        let newDeck = (pickedCard, rejected) ||> addIn state.myDeck
        {state with myDeck = newDeck}

    let playBattleWithDeck deck gameBattle =

        playCards deck gameBattle
        playAttacks deck gameBattle

        deck


(* game loop *)
let rec mainLoop (gameState:GameState) = 
    let me = readLine() |> Player.create
    let opponent = readLine() |> Player.create

    let opponentHand = readLine() |> int
    let cardCount = readLine() |> int

    let cards = 
        [0 .. cardCount - 1 ] 
        |> Seq.map (fun _ -> readLine() |> Card.create) 
        |> Seq.toList

    let deck = match me.mana with
                    | InDraft -> cards |> GameLogic.playDraft gameState 
                    | InBattle ->   
                        GameBattle.create me opponent cards 
                        |> GameLogic.playBattleWithDeck gameState.myDeck 

                    | _ -> failwith "Houston, we have a problem !"



    mainLoop {gameState with myDeck = deck; turn = gameState.turn + 1}


// start 
mainLoop GameState.empty
