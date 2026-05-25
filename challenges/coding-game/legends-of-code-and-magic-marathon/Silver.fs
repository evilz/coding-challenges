open System

[<AutoOpen>]
module Helpers =
    
    /// Splits the given string at the given delimiter
    let inline split (delimiter:char) (text:string) = text.Split [|delimiter|]
    let inline readLine() = Console.ReadLine()

    type OptionalBuilder () =

        member this.Bind(x, f) = 
            match x with
            | None -> None
            | Some a -> f a

        member this.Return(x) = 
            Some x
        
        member this.ReturnFrom(x) =
            x

    let optionnal = new OptionalBuilder()

module Config =
    let startHealth = 30
    let maxMana = 12
    let maxCardsInHand = 8
    let maxCardsOnBoard = 6
    let maxTurn = 50

type Health = int
type Mana = int
type Rune = int
type InstanceId = int
type Player = {health: Health; mana: Mana; deck:int; rune: Rune}

module Player =
    
    let create line = 
        let token = line |> split ' ' |> Array.map int
        {health =  token.[0]; mana =  token.[1]; deck =  token.[2]; rune =  token.[3]} 
    
    let empty =  {health = 0; mana = 0; deck = 0; rune = 0} 

    let tryConsumeMana count player =
        match player.mana >= count with
            | true -> Some { player with mana = player.mana - count}
            | false -> None
    


    let changeHealth x player =
        {player with health = player.health + x}
    
type Abilities = {
    breakthrough: bool; //(Percée) : une créature avec Breakthrough peut infliger des dégâts supplémentaires à l'adversaire quand elle attaque une autre créature. Si ses dégâts d'attaque sont supérieurs à la défense de la créature défenseuse, alors l'excès de dégâts est infligé au joueur défenseur.
    charge: bool; // : une créature avec Charge peut attaquer le tour où elle est invoquée.
    guard: bool; //(Garde) : une créature attaquante doit attaquer une creature avec Guard du joueur défenseur en priorité.
    drain: bool; // (Vol de vie) : une créature avec Drain ajoute autant de PVs que de dégâts qu'elle inflige (quand elle attaque seulement).
    lethal: bool; //  (Létalité) : une créature avec Lethal tue toutes les créatures auxquelles elle inflige des dégâts.
    ward: bool; //  (Bouclier) : une créature avec Ward ignore les prochains dégâts qu'elle se verrait recevoir (une seule fois). Le "bouclier" procuré par la capacité Ward est ensuite perdu.
    }
module Abilities =
    let parseAbilities (s:string) =
        let abilities = s.ToCharArray()
        { breakthrough = abilities.[0] = 'B'
        ; charge = abilities.[1] = 'C'
        ; drain = abilities.[2] = 'D'
        ; guard = abilities.[3] = 'G'
        ; lethal = abilities.[4] = 'L'
        ; ward = abilities.[5] = 'W'
        }

type CardLocation = InMyHand = 0 | OnMyBoard = 1 | OnOpponentBoard = -1
type CardType = Creature = 0 | GreenItem = 1 | RedItem = 2 | BlueItem = 3

type Card = {
    cardNumber : int;
    instanceId:InstanceId;
    location: CardLocation;
    cardType:CardType;
    cost:Mana;
    attack:int;
    defense:int;
    abilities: Abilities
    myHealthChange:Health;
    opponentHealthChange:Health;
    cardDraw:int
    }

type Minion = MinionInfo of Card
type Ennemy = EnnemyInfo of Card
type Creature = 
    | Minion of Minion
    | Ennemy of Ennemy

type GreenItem = GreenItemInfo of Card
type RedItem = RedItemInfo of Card
type BlueItem = BlueItemInfo of Card

type Item = Green of GreenItem | Red of RedItem | Blue of BlueItem

type AttackEnnemyResult = {minion: Minion option; ennemy: Ennemy option; drain:int ; breakthrough: int}
module Card =
    let create line = 
        let token = line |> split ' '
        {   cardNumber = token.[0] |> int
            instanceId = token.[1] |> int
            location = token.[2] |> int |> enum<CardLocation>
            cardType = token.[3] |> int |> enum<CardType>
            cost = token.[4] |> int
            attack = token.[5] |> int
            defense = token.[6] |> int
            abilities = token.[7] |> Abilities.parseAbilities
            myHealthChange = token.[8] |> int
            opponentHealthChange = token.[9] |> int
            cardDraw = token.[10] |> int
            
        }

    let isDead c = c.defense <= 0

    let toDeath card = 
        {card with defense = 0}


    let decrementDefense value card  = {card with defense = card.defense - value}
    let decrementAttack value card  = {card with attack = card.attack - value}
    
    let incrementDefense value card  = {card with defense = card.defense + value}
    let incrementAttack value card  = {card with attack = card.attack + value}

    let attack minion ennemy =

        // BREACKTHROUGH
        let breakthrough = match minion.abilities.breakthrough, ennemy.abilities.ward with
                            | _ , true -> 0
                            | true, false -> minion.attack - ennemy.defense |> max 0
                            | false, false -> 0

        let e = match ennemy.abilities.ward, minion.abilities.lethal with
                | true , _ -> ennemy
                | _ , true ->  ennemy |> toDeath
                | false, _ -> ennemy |> decrementDefense minion.attack


        let m = match minion.abilities.ward,  e.abilities.lethal with
                | true, _ -> minion
                | false, true -> minion |> toDeath
                | false, false -> minion |> decrementDefense ennemy.attack

        // check drain when dead ?
        let drain = match minion.abilities.drain, ennemy.abilities.ward with
                    | true, false -> m.attack
                    | _ , true -> 0
                    | false, _ -> 0

        match m |> isDead , e |> isDead with
        | false, false -> {minion= (Some (MinionInfo m)); ennemy= (Some (EnnemyInfo e)); drain= drain; breakthrough = breakthrough}
        | true, false -> {minion= None; ennemy= (Some (EnnemyInfo e)); drain= drain; breakthrough = breakthrough}
        | false, true -> {minion= (Some (MinionInfo m)); ennemy= None; drain= drain; breakthrough = breakthrough}
        | true, true -> {minion= None; ennemy= None; drain= drain; breakthrough = breakthrough}

    let value card = 
        (( card.defense 
        + card.attack
        + (if card.cardType = CardType.GreenItem then 6 else 0)
        + (if card.abilities.guard then 13 else 0)
        + (if card.abilities.drain then 8 else 0)
        + (if card.abilities.lethal then 5 else 0)
        + (if card.abilities.charge then 3 else 0)
        + (if card.abilities.ward then 2 else 0)
        + (if card.abilities.breakthrough then 1 else 0)
        + card.cardDraw
        + card.myHealthChange
        + card.opponentHealthChange
        ) 
        |> float )
        / (card.cost + 1 |> float) // is this good ??

    let draftWanted = 
        [ 2, 4 // (5,8) 
        ; 3, 7 //(6,7)
        ; 4, 7 //(6,7)
        ; 5, 5 //(2,5)
        ; 6, 5 //(2,3)
        ; 7, 5 // (2,3)
        ]
        |> Map.ofList


    let cardsByLocation (cards:Card seq) =  cards |> Seq.toList |> Seq.groupBy (fun x -> x.location) |> Map.ofSeq

type Deck = {
    inDeck: Card list
    rejected: Card list
}

module Deck =
    let addIn deck card rejected  = {deck with inDeck = (card :: deck.inDeck); rejected = (deck.rejected @ rejected)}
    let empty = {inDeck = list.Empty; rejected = list.Empty;}

let simpleMana card = 
    match card.cost with
    | 0 | 1 | 2 -> 2
    | 7 | 8 | 9 | 10 | 11 | 12 -> 7
            | _ -> card.cost

let valueForDraft draftState card =
    let defaultValue = card |> Card.value
    let mana = card |> simpleMana
    let groups = draftState.inDeck |> List.map (fun x -> x |> simpleMana) |> List.groupBy (fun x -> x ) |> Map.ofList

    let ingroup = groups |> Map.tryFind mana |> Option.defaultValue List.empty
    
    let stillToPick = 30 - draftState.inDeck.Length |> float

    let stillToPickForMana = (( (Card.draftWanted.[mana]) - ingroup.Length ) |> float) / stillToPick
    
    defaultValue * stillToPickForMana

open Player
open Card
open Deck

type Action =
    | Pick of int
    | Pass // PASS pour passer son tour.
    | Summon of Minion //SUMMON id pour invoquer la créature id.
    | AttackEnnemy of Minion * Ennemy //ATTACK id1 id2 pour attaquer la créature id2 avec la créature id1.
    | AttackOpponent of Minion // ATTACK id -1 pour attaquer l'adversaire avec la créature id.
    | UseGreenItem of GreenItem * Minion // USE id1 id2 pour utiliser l'objet id1 sur la créature id2.
    | UseRedItem of RedItem * Ennemy // USE id1 id2 pour utiliser l'objet id1 sur la créature id2.
    | UseBlueItem of BlueItem * Ennemy option //pour utiliser l'objet id.


module Action = 

    let getActionStr cmd = 
        match cmd with
            | Pick id -> sprintf "PICK %i" id
            | Pass -> "PASS"
            | Summon (MinionInfo(m)) -> sprintf "SUMMON %i" m.instanceId
            | AttackEnnemy (MinionInfo(m),EnnemyInfo(e)) -> sprintf "ATTACK %i %i" m.instanceId e.instanceId
            | AttackOpponent (MinionInfo(m)) -> sprintf "ATTACK %i -1" m.instanceId
            | UseGreenItem (GreenItemInfo(g), MinionInfo(m)) -> sprintf "USE %i %i" g.instanceId m.instanceId // id1 id2 pour utiliser l'objet id1 sur la créature id2.
            | UseRedItem (RedItemInfo(r), EnnemyInfo(e)) -> sprintf "USE %i %i" r.instanceId e.instanceId // USE id1 id2 pour utiliser l'objet id1 sur la créature id2.
            | UseBlueItem (BlueItemInfo(b), Some(EnnemyInfo(e))) -> sprintf "USE %i %i" b.instanceId e.instanceId
            | UseBlueItem (BlueItemInfo(b), None) -> sprintf "USE %i -1" b.instanceId


    let executeActions (cmds: Action list) = 
        eprintfn "execute %i action" (cmds.Length)
        match cmds with
        | [] -> Pass |> getActionStr
        | _ -> cmds |> List.toArray |> Array.map getActionStr |> String.concat ";"
    
    // say something

type Hand = {
    minion: Set<Minion> 
    greenItems: Set<GreenItem>
    redItems: Set<RedItem>
    blueItems: Set<BlueItem>
}
with static member Empty = { minion= Set.empty; greenItems= Set.empty; redItems=Set.empty; blueItems= Set.empty}


type GameBattle = {
    me: Player;
    opponent: Player
    
    myHand: Hand
    myBoard: Set<Minion>
    oppBoard: Set<Ennemy>

    opponentHandCount: int

    myCardDraw : int
    
    haveBeenUsed: Set<InstanceId>
}



module GameBattle =

    let score state = 

        let cardValue card =
            (( card.defense 
                + card.attack
                //+ (if card.cardType = CardType.Creature then 2 else 0)
                + (if card.abilities.guard then 13 else 0)
                + (if card.abilities.drain then 8 else 0)
                + (if card.abilities.lethal then 5 else 0)
                + (if card.abilities.charge then 3 else 0)
                + (if card.abilities.ward then 2 else 0)
                + (if card.abilities.breakthrough then 1 else 0)
                + card.cardDraw
                + card.myHealthChange
                + card.opponentHealthChange
                ) 
                |> float ) 

        let scoreOpp = state.oppBoard |> Seq.map (fun (EnnemyInfo e) -> e |> cardValue) |> Seq.sum
        let scoreMyBoard = state.myBoard |> Seq.map (fun (MinionInfo m) -> m |> cardValue) |> Seq.sum

        let result = scoreMyBoard - scoreOpp  
                    + ((float state.me.health) * 0.5)
                    - ((float state.opponent.health) * 0.5)
                   // - ((float state.me.mana) * 3.0)

        result
    

    let private createHand cards =
        (Hand.Empty, cards)
        ||> Seq.fold (fun hand card -> 
                        match card.cardType with
                        | CardType.Creature -> {hand with minion = hand.minion.Add(MinionInfo(card))  } 
                        | CardType.GreenItem -> {hand with greenItems = hand.greenItems.Add(GreenItemInfo(card)) } 
                        | CardType.RedItem -> {hand with redItems = hand.redItems.Add(RedItemInfo(card)) } 
                        | CardType.BlueItem -> {hand with blueItems = hand.blueItems.Add(BlueItemInfo(card)) } 
                        | _ -> failwithf "card type is fucked"
                    )

    let empty = {
        me = Player.empty; 
        opponent = Player.empty; 
        myHand = Hand.Empty; 
        myBoard= Set.empty; 
        oppBoard= Set.empty; 
        opponentHandCount = 0; 
        myCardDraw = 1; 
        haveBeenUsed = Set.empty}
    let create me opp cards opponentHandCount =  
        let cardsByLocation = cards |> cardsByLocation
        
        let myHand = cardsByLocation.[CardLocation.InMyHand] |> createHand
        
        let myBoard = cardsByLocation 
                        |> Map.tryFind CardLocation.OnMyBoard 
                        |> Option.defaultValue Seq.empty 
                        |> Seq.map MinionInfo
                        |> Set.ofSeq
        
        let oppBoard = cardsByLocation
                        |> Map.tryFind CardLocation.OnOpponentBoard 
                        |> Option.defaultValue Seq.empty 
                        |> Seq.map EnnemyInfo
                        |> Set.ofSeq

        
        { empty with me = me
                    ; opponent = opp
                    ; myHand = myHand
                    ; myBoard = myBoard
                    ; oppBoard = oppBoard
                    ; opponentHandCount = opponentHandCount
        }
    

let (|InDraft|InBattle|) mana = 
    match mana with
    | 0 -> InDraft
    | _ -> InBattle

type GameState = {
        turn:int
        myDeck:Deck
        battle:GameBattle
    }
module GameState =
    let empty = {turn = 0; myDeck = Deck.empty; battle = GameBattle.empty }  


open GameState
open GameBattle
open Action

module GameLogic =

    let evalCanBeUsed id state =
        match state.haveBeenUsed.Contains id with
        | true -> None
        | false -> Some state

    let evalConsumeMana mana state  =
        match state.me |> tryConsumeMana mana with
            | None -> None
            | Some p -> Some {state with me = p}

    let evalMyHealthChange healthChange state =
        match state.me |> changeHealth healthChange with
            | p when p.health <= 0 -> None
            | p -> Some {state with me = p}

    let evalOpponentHealthChange healthChange state =
        Some {state with opponent = state.opponent |> changeHealth healthChange}

    let evalAttackOpponent attack state = {state with opponent = state.opponent |> changeHealth (-attack)}

    let evalCardDraw count state =
        Some {state with myCardDraw = state.myCardDraw + count} 

    let evaluateSummon minion (state:GameBattle) = 
        
        
        match state.myBoard.Count >= Config.maxCardsOnBoard with
            | true -> None
            | false -> optionnal {
                            let! state = state |> (evalCanBeUsed minion.instanceId)
                            let! state = state |> (evalConsumeMana minion.cost)
                            let! state = state |> (evalMyHealthChange minion.myHealthChange)        
                            let! state = state |> (evalOpponentHealthChange minion.opponentHealthChange)        
                            let! state = state |> (evalCardDraw minion.cardDraw)
                            let state = {state with 
                                            myBoard = state.myBoard.Add (MinionInfo minion)
                                            myHand = {state.myHand with minion = state.myHand.minion.Remove(MinionInfo minion)}
                                            haveBeenUsed = match minion.abilities.charge with
                                                            | true -> state.haveBeenUsed
                                                            | false -> state.haveBeenUsed.Add minion.instanceId}
                            //eprintfn "evaluateSummon %i with score %f" minion.instanceId (state |> score)
                            return state
                        }

    let evalBreakthrough damage state =
        Some {state with opponent = state.opponent |> changeHealth damage}

    let evalDrain gain state =
        {state with me = state.me |> changeHealth gain}

    let evalEnnemy ennemy ennemyState state =
        let board = match ennemyState with
                    | None -> state.oppBoard.Remove (EnnemyInfo ennemy)
                    | Some e -> state.oppBoard.Remove(EnnemyInfo ennemy).Add(e)

        {state with oppBoard = board }          

    let evalMinion minion minionState state =
        let board = match minionState with
                    | None -> state.myBoard.Remove(MinionInfo minion)
                    | Some m -> state.myBoard.Remove(MinionInfo minion).Add(m)

        {state with myBoard = board }

    let evalGuardFirst (state:GameBattle) =
        match state.oppBoard |> Seq.tryFind (fun (EnnemyInfo x) -> x.abilities.guard) with
        | None -> Some state
        | Some _ -> None 

    let evaluateAttackEnemy minion ennemy (state:GameBattle) = 
        optionnal {
            let! state =match minion.attack with
                        | 0 -> None
                        | _ -> Some state

            let ackResult = Card.attack minion ennemy

            
            let! state = state |> (evalCanBeUsed minion.instanceId)
            let! state = match ennemy.abilities.guard with
                            | true -> Some state
                            | false -> state |> evalGuardFirst
            let! state = state |> (evalBreakthrough ackResult.breakthrough)
            let state = state |> (evalDrain ackResult.drain)
            let state = state |> (evalEnnemy ennemy ackResult.ennemy)
            let state = state |> (evalMinion minion ackResult.minion)
            let! state = state |> (evalMyHealthChange minion.myHealthChange)        
            let! state = state |> (evalOpponentHealthChange minion.opponentHealthChange)        
            let! state = state |> (evalCardDraw minion.cardDraw)     
            let state = {state with haveBeenUsed = state.haveBeenUsed.Add minion.instanceId}   
            return state
            
        }

    let evaluateAttackOpponent minion (state:GameBattle) = 
        optionnal {
            let! state =match minion.attack with
                        | 0 -> None
                        | _ -> Some state

            let drain = match minion.abilities.drain with
                        | true -> minion.attack
                        | false -> 0

            let! state = state |> (evalCanBeUsed minion.instanceId)
            let! state = state |> evalGuardFirst
            let state = state |> (evalDrain drain)        
            let state = state |> (evalAttackOpponent minion.attack)
            let state = {state with haveBeenUsed = state.haveBeenUsed.Add minion.instanceId}   
            return state
        }

    let evalAbilities item add creature = 
        let abilities = 
            { creature.abilities with 
                breakthrough = if item.abilities.breakthrough then add else creature.abilities.breakthrough
                charge = if item.abilities.charge then add else creature.abilities.charge
                drain = if item.abilities.drain then add else creature.abilities.drain
                guard = if item.abilities.guard then add else creature.abilities.guard
                lethal = if item.abilities.lethal then add else creature.abilities.lethal
                ward = if item.abilities.ward then add else creature.abilities.ward
            } 
        {creature with abilities = abilities}

    let evaluateBlueItemOnEnnemy item ennemy state = 
        
        let ennemy =  // evalAbilities item ennemy false  // # not needed for blue
                    ennemy   |> decrementDefense item.defense

        let board = match ennemy |>isDead with
                    | true -> state.oppBoard.Remove(EnnemyInfo ennemy)
                    | false -> state.oppBoard.Remove(EnnemyInfo ennemy).Add(EnnemyInfo ennemy)

        let state = {state with oppBoard = board }
        optionnal {
            let! state = state |> (evalCanBeUsed item.instanceId)
            let! state = state |> (evalConsumeMana item.cost)
            let! state = state |> (evalMyHealthChange item.myHealthChange)        
            let! state = state |> (evalOpponentHealthChange item.opponentHealthChange)        
            let! state = state |> (evalCardDraw item.cardDraw)
            let state = {state with 
                            myHand = {state.myHand with 
                                        blueItems = state.myHand.blueItems.Remove(BlueItemInfo item)}
                            haveBeenUsed = state.haveBeenUsed.Add item.instanceId
                        }
            return state
        }

    let evaluateBlueItemOnOpponent item state = 
        
        optionnal {
            let! state = state |> (evalCanBeUsed item.instanceId)
            let! state = state |> (evalConsumeMana item.cost)
            let! state = state |> (evalMyHealthChange item.myHealthChange)        
            let! state = state |> (evalOpponentHealthChange item.opponentHealthChange)        
            let! state = state |> (evalOpponentHealthChange item.defense)        
            let! state = state |> (evalCardDraw item.cardDraw)
            let state = {state with 
                            myHand = {state.myHand with 
                                        blueItems = state.myHand.blueItems.Remove(BlueItemInfo item)}
                            haveBeenUsed = state.haveBeenUsed.Add item.instanceId
                        }
            return state
        }

    let evaluateRedItem item ennemy state = 
        
        let ennemy = ennemy 
                        |> evalAbilities item  false
                        |> incrementDefense item.defense // defense is negatif
                        |> incrementAttack item.attack

        let board = match ennemy |>isDead with
                    | true -> state.oppBoard.Remove(EnnemyInfo ennemy)
                    | false -> state.oppBoard.Remove(EnnemyInfo ennemy).Add(EnnemyInfo ennemy)

        let state = {state with oppBoard = board }
        optionnal {
            let! state = state |> (evalCanBeUsed item.instanceId)
            let! state = state |> (evalConsumeMana item.cost)
            let! state = state |> (evalMyHealthChange item.myHealthChange)        
            let! state = state |> (evalOpponentHealthChange item.opponentHealthChange)        
            let! state = state |> (evalCardDraw item.cardDraw)
            let state = {state with 
                            myHand = {state.myHand with 
                                        redItems = state.myHand.redItems.Remove(RedItemInfo item)}
                            haveBeenUsed = state.haveBeenUsed.Add item.instanceId
                        }
            return state
        }

    let evaluateGreenItem item minion state = 
        
        let board = state.myBoard.Remove(MinionInfo minion)

        let minion = minion 
                        |> evalAbilities item true  
                        |> incrementAttack item.attack
                        |> incrementDefense item.defense

        let state = {state with myBoard = board.Add(MinionInfo minion) }
        optionnal {
            let! state = state |> (evalCanBeUsed item.instanceId)
            let! state = state |> (evalConsumeMana item.cost)
            let! state = state |> (evalMyHealthChange item.myHealthChange)        
            //let! state = state |> (evalOpponentHealthChange item.opponentHealthChange)   // # dont need in green     
            let! state = state |> (evalCardDraw item.cardDraw)
            let state = {state with 
                            myHand = 
                                    {state.myHand with greenItems = state.myHand.greenItems.Remove(GreenItemInfo item)}
                            haveBeenUsed = state.haveBeenUsed.Add item.instanceId
            }
            return state
        }
        

    let evaluateAction (state:GameBattle) action =

        let evaluatedState = optionnal {
            match action with
                | Pick id -> return state
                | Pass -> return state
                | Summon (MinionInfo(m)) -> return! (state |> (evaluateSummon m))
                | AttackEnnemy (MinionInfo(m),EnnemyInfo(e)) -> return! (state |> evaluateAttackEnemy m e)
                | AttackOpponent (MinionInfo(m)) -> return! (state |> evaluateAttackOpponent m )
                | UseBlueItem (BlueItemInfo(i), Some(EnnemyInfo(e))) -> return! (state |> (evaluateBlueItemOnEnnemy i e) )
                | UseBlueItem (BlueItemInfo(i), None ) -> return! (state |> (evaluateBlueItemOnOpponent i ) )
                | UseGreenItem (GreenItemInfo(i), MinionInfo(m)) -> return! (state |> (evaluateGreenItem i m) )
                | UseRedItem (RedItemInfo(i), EnnemyInfo(e)) -> return! (state |> (evaluateRedItem i e) )    
        }
        // match evaluatedState with
        // | Some x -> eprintfn "score : %f" (x |> score)
        // | _ -> eprintfn "action cancel"
        
        evaluatedState
        // match evaluatedState with
        // | Some x when (x |> score) > (state |> score) -> Some x
        // | _ -> None
        
    let availableActions state = 
        // 
        [
            // SUMMON
            yield! state.myHand.minion |> Set.map Summon
            
            // BLUE
            for item in state.myHand.blueItems do
                yield (UseBlueItem(item, None)) 
                for ennemy in state.oppBoard do
                    yield (UseBlueItem(item, Some ennemy)) 
            
            // RED
            for item in state.myHand.redItems do
                for ennemy in state.oppBoard do
                    yield (UseRedItem(item, ennemy)) 

            // GREEN
            for item in state.myHand.greenItems do
                for minion in state.myBoard do
                    yield (UseGreenItem(item, minion)) 

            // ATTACK
            for m in state.myBoard do
                yield (AttackOpponent m )
                for e in state.oppBoard do
                    yield (AttackEnnemy(m, e)) 
        ]

    let playDraft (state:GameState) cards=
        (*
        TODO :
            - take abilities in value
            - abilities with combo   guard + defense 

        *)
        let picked, rejected = 
            cards 
            |> List.mapi (fun i x -> i,x )
            |> List.sortByDescending (fun (i,x)-> x |> valueForDraft state.myDeck)
            |> fun x -> (x|> List.head, x|> List.tail)

        let pickedIndex, pickedCard = picked
        [Pick pickedIndex] |> executeActions |> printfn "%s"

        let rejected = rejected |> List.map (fun (i,x) -> x)

        let newDeck = (pickedCard, rejected) ||> addIn state.myDeck
        {state with myDeck = newDeck}

    let rec playBattle (state:GameBattle) cmds =
        
        let sortedAction = availableActions state 
                            |> Seq.map (fun action -> action, action |> evaluateAction state)
                            |> Seq.filter (fun (a, s) -> s.IsSome)
                            |> Seq.map (fun (a, s) -> a, s.Value)
                            |> Seq.sortByDescending (fun (a, s) -> s |> score)
                            |> Seq.toList

        // sortedAction
        // |> List.iter (fun (a,s) -> eprintfn "%s -> %f" (a |> getActionStr) (s |> score) )

        sortedAction 
        |> function
            | [] -> state, cmds
            | (a,s)::_ -> 
                // eprintfn "NEXT ACTION IS %s -> %f" (a |> getActionStr) (s |> score) 
                // eprintfn ""
                playBattle s (a:: cmds)


(* game loop *)
let rec mainLoop (state:GameState) = 

    let stopWatch = System.Diagnostics.Stopwatch.StartNew()
    let me = readLine() |> Player.create
    let opponent = readLine() |> Player.create

    let opponentHand = readLine() |> int
    let cardCount = readLine() |> int

    let cards = 
        [0 .. cardCount - 1 ] 
        |> Seq.map (fun _ -> readLine() |> Card.create) 
        |> Seq.toList

    let newState = match me.mana with
                    | InDraft -> cards |> GameLogic.playDraft state 
                    | InBattle ->   
                        //{state with battle  = GameBattle.create me opponent cards opponentHand }
                        let battleState, actions = ((GameBattle.create me opponent cards opponentHand), List.empty) ||> GameLogic.playBattle 
                        let outString = actions |> List.rev |> executeActions
                        printfn "%s Score is %f" outString (battleState |> score)
                        {state with battle = battleState}
                        
    stopWatch.Stop()
    eprintfn "%f" stopWatch.Elapsed.TotalMilliseconds
    mainLoop {newState with turn = newState.turn + 1}


// start 
mainLoop GameState.empty


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
//                 }); 
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

//     let state, actions = GameLogic.playBattle state List.empty
//     let score = state |> score
//     0