open System

[<AutoOpen>]
module Helpers =
    
    let inline split (delimiter:char) (text:string) = text.Split [|delimiter|]
    let inline readLine() = Console.ReadLine()
    
    type OptionalBuilder () =
        member __.Bind(x, f) =  
            match x with
            | None -> None
            | Some a -> f a

        member __.Return(x) = Some x
        
        //member __.ReturnFrom(x) = x

    let optionnal = new OptionalBuilder()

module Config =
    let deckSize = 30
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
    
    let changeHealth x player = {player with health = player.health + x}
    
type Abilities = {
    /// Excess damage done on a player's turn is dealt to the opponent. 
    /// 
    /// **Important cases: The opponent creature has to be removed (e.g. there is never excess damage against creatures with ward).**
    breakthrough: bool

    /// When summoned, such creature can attack the turn it is played.
    /// 
    /// **Important cases: If the creature gains charge keyword during the turn it can attack only if it was summoned this turn.**
    charge: bool

    /// Enemies must attack this creature.
    guard: bool

    /// When deals damage on a player's turn, the player gain health equal to the creature's attack.
    /// 
    /// **Important cases: If the defender has ward, no damage is done so no health is gained by the player.**
    drain: bool

    /// Removes any creature it damages.
    /// 
    /// **Important cases: It does not remove creatures with ward. If lethal attack is equal zero, ability do not trigger (no damage done).**
    lethal: bool

    /// Prevent the first time the creature would take damage.
    /// 
    /// **Important cases: After preventing damage, the ward disappears. Attacking with creature with 0 attack does not remove ward.**
    ward: bool
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
type Creature =  Minion of Minion | Ennemy of Ennemy

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

    let toDeath card = {card with defense = 0}

    let removeWard c = {c with abilities = {c.abilities with ward = false }}

    let hitted value card = 
        if card.abilities.ward 
        then card |> removeWard
        else {card with defense = card.defense - value}

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
                | true , _ -> ennemy |> removeWard
                | _ , true ->  ennemy |> toDeath
                | false, _ -> ennemy |> hitted minion.attack


        let m = match minion.abilities.ward,  e.abilities.lethal with
                | true, _ -> minion
                | false, true -> minion |> toDeath
                | false, false -> minion |> hitted ennemy.attack

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
        let basicValue = ((abs card.defense) * 2 + (abs card.attack) * 4)  /// ((card.cost + 1) |> float)

        // bonus
        basicValue
        //+ (if card.cardType <> CardType.Creature then basicValue * 2 else 0)   // 90
        + (if card.abilities.guard then (1+ card.defense) * 6 else 0)   // 90
        + (if card.abilities.drain then (1+ card.attack) * 4 else 0)  // 20
        + (if card.abilities.lethal then (1+ card.defense) * 10 else 0)  // 30
        + (if card.abilities.ward then (1+ card.defense) * 5 else 0)    // 5
        + (if card.abilities.charge then (1+ card.attack) * 2 else 0)  // 3
        + (if card.abilities.breakthrough then (1+ card.attack) * 2 else 0)  //1
        + (card.cardDraw)
        + (card.myHealthChange) 
        + (-card.opponentHealthChange)

    let draftControl = 
        [ 0, 0
        ; 1, 6
        ; 2, 8 
        ; 3, 4
        ; 4, 2
        ; 5, 3
        ; 6, 3
        ; 7, 1
        ; 8, 1
        ; 9, 1
        ; 10, 1
        ; 11, 1
        ; 12, 1
        ]

    let draftCustom = 
        [ 0, 6
        ; 1, 15
        ; 2, 15
        ; 3, 10
        ; 4, 10
        ; 5, 6
        ; 6, 4
        ; 7, 3
        ; 8, 1
        ; 9, 1
        ; 10, 1
        ; 11, 1
        ; 12, 1
        ]

    let draftAggro = 
        [ 0, 2
        ; 1, 9
        ; 2, 8 
        ; 3, 10
        ; 4, 1
        ; 5, 0
        ; 6, 0
        ; 7, 0
        ; 8, 0
        ; 9, 0
        ; 10, 0
        ; 11, 0
        ; 12, 0
        ]

    let draftMidRange = 
        [ 0, 2
        ; 1, 4
        ; 2, 5 
        ; 3, 4
        ; 4, 7
        ; 5, 2
        ; 6, 5
        ; 7, 1
        ; 8, 1
        ; 9, 1
        ; 10, 1
        ; 11, 1
        ; 12, 1
        ]

    let draftWanted = 
        draftCustom |> Map.ofList


    let cardsByLocation  =  Seq.toList >> Seq.groupBy (fun x -> x.location) >> Map.ofSeq

type Deck = {
    inDeck: Card list
    rejected: Card list
}

module Deck =
    let addIn deck card rejected  = {deck with inDeck = (card :: deck.inDeck); rejected = (deck.rejected @ rejected)}
    let empty = {inDeck = list.Empty; rejected = list.Empty;}


let valueForDraft draftState card =
    let defaultValue = (card |> Card.value)

    let groups = draftState.inDeck |>  List.groupBy (fun x -> x.cost ) |> Map.ofList
    let ingroup = groups |> Map.tryFind card.cost |> Option.defaultValue List.empty
    let stillToPickForMana = (( (Card.draftWanted.[card.cost]) - ingroup.Length ))
    defaultValue +  (stillToPickForMana * 30 )

open Player
open Card
open Deck

type Action =
    /// Pick card with instance ID
    | Pick of InstanceId
    /// Does nothing
    | Pass
    /// Puts a card of creature type on board
    | Summon of Minion
    /// Attacks with a Minion an opponent's onboard creature
    | AttackEnnemy of Minion * Ennemy
    /// Attacks with a Minion the opponent player
    | AttackOpponent of Minion
    /// Uses a green item on the Minion
    | UseGreenItem of GreenItem * Minion
    /// Uses a red item on the Ennemy
    | UseRedItem of RedItem * Ennemy
    /// Uses a blue item on the Ennemy or on opponent player
    | UseBlueItem of BlueItem * Ennemy option


module Action = 

    let getActionStr cmd = 
        match cmd with
            | Pick id -> sprintf "PICK %i" id
            | Pass -> "PASS"
            | Summon (MinionInfo(m)) -> sprintf "SUMMON %i" m.instanceId
            | AttackEnnemy (MinionInfo(m),EnnemyInfo(e)) -> sprintf "ATTACK %i %i" m.instanceId e.instanceId
            | AttackOpponent (MinionInfo(m)) -> sprintf "ATTACK %i -1" m.instanceId
            | UseGreenItem (GreenItemInfo(g), MinionInfo(m)) -> sprintf "USE %i %i" g.instanceId m.instanceId
            | UseRedItem (RedItemInfo(r), EnnemyInfo(e)) -> sprintf "USE %i %i" r.instanceId e.instanceId
            | UseBlueItem (BlueItemInfo(b), Some(EnnemyInfo(e))) -> sprintf "USE %i %i" b.instanceId e.instanceId
            | UseBlueItem (BlueItemInfo(b), None) -> sprintf "USE %i -1" b.instanceId


    let executeActions (cmds: Action list) = 
        match cmds with
        | [] -> Pass |> getActionStr
        | _ -> cmds |> List.toArray |> Array.map getActionStr |> String.concat ";"

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

    /// Fitness function to score state
    let score state = 
        let cardValue card = card |> value

        let scoreOpp = state.oppBoard |> Seq.map (fun (EnnemyInfo e) -> e |> cardValue) |> Seq.sum
        let scoreMyBoard = state.myBoard |> Seq.map (fun (MinionInfo m) -> m |> cardValue) |> Seq.sum

        scoreMyBoard - scoreOpp
        - (state.me.mana * 3)
        + (state.myBoard.Count * 150)
        - (state.oppBoard.Count * 180)
        + (state.me.health * 3)
        - (state.opponent.health * 3)

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
        
        let myHand = cardsByLocation.TryFind CardLocation.InMyHand 
                        |> Option.defaultValue Seq.empty 
                        |> createHand
        
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

open GameBattle
open Action

module GameLogic =

// ============  SIMPLE EVAL FUNCTIONS  ============
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

    let evalOpponentHealthChange healthChange state = {state with opponent = state.opponent |> changeHealth healthChange}

    let evalAttackOpponent attack state = {state with opponent = state.opponent |> changeHealth (-attack)}

    let evalCardDraw count state = {state with myCardDraw = state.myCardDraw + count} 

    let evalBreakthrough damage state = {state with opponent = state.opponent |> changeHealth damage}

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

            
            let! state = state |> (evalCanBeUsed minion.instanceId)
            let! state = match ennemy.abilities.guard with
                            | true -> Some state
                            | false -> state |> evalGuardFirst

            
            let ackResult = Card.attack minion ennemy


            let state = state |> (evalBreakthrough ackResult.breakthrough)
            let state = state |> (evalDrain ackResult.drain)
            let state = state |> (evalEnnemy ennemy ackResult.ennemy)
            let state = state |> (evalMinion minion ackResult.minion)
            let! state = state |> (evalMyHealthChange minion.myHealthChange)        
            let state = state |> (evalOpponentHealthChange minion.opponentHealthChange)        
            let state = state |> (evalCardDraw minion.cardDraw)     
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

    let evalBetterScore beginState endState = 
        if (beginState |> score ) >= (endState |> score)
        then None
        else Some endState

//#region ============  ACTIONS EVAL FUNCTIONS  ============
    let evaluateSummon minion (state:GameBattle) = 
        
        match state.myBoard.Count >= Config.maxCardsOnBoard with
            | true -> None
            | false -> optionnal {
                            let! state = state |> (evalCanBeUsed minion.instanceId)
                            let! state = state |> (evalConsumeMana minion.cost)
                            let! state = state |> (evalMyHealthChange minion.myHealthChange)        
                            let state =  state |> (evalOpponentHealthChange minion.opponentHealthChange)        
                            let state =  state |> (evalCardDraw minion.cardDraw)
                            let state = {state with 
                                            myBoard = state.myBoard.Add (MinionInfo minion)
                                            myHand = {state.myHand with minion = state.myHand.minion.Remove(MinionInfo minion)}
                                            haveBeenUsed = match minion.abilities.charge with
                                                            | true -> state.haveBeenUsed
                                                            | false -> state.haveBeenUsed.Add minion.instanceId}
                            //eprintfn "evaluateSummon %i with score %f" minion.instanceId (state |> score)
                            return state
                        }

    let evaluateBlueItemOnEnnemy item ennemy state = 
        
        let newEnnemy = ennemy 
                        |> evalAbilities item  false
                        |> hitted (-item.defense)
                        |> incrementAttack item.attack

        let board = match newEnnemy |>isDead with
                    | true -> state.oppBoard.Remove(EnnemyInfo ennemy)
                    | false -> state.oppBoard.Remove(EnnemyInfo ennemy).Add(EnnemyInfo newEnnemy)

        let newstate = {state with oppBoard = board }
        optionnal {
            let! newstate = newstate |> (evalCanBeUsed item.instanceId)
            let! newstate = newstate |> (evalConsumeMana item.cost)
            let! newstate = newstate |> (evalMyHealthChange item.myHealthChange)        
            let newstate = newstate |> (evalOpponentHealthChange item.opponentHealthChange)        
            let newstate = newstate |> (evalCardDraw item.cardDraw)
            let newstate = {newstate with 
                                myHand = {newstate.myHand with 
                                            blueItems = state.myHand.blueItems.Remove(BlueItemInfo item)}
                                haveBeenUsed = newstate.haveBeenUsed.Add item.instanceId
                        }
            let! newstate = evalBetterScore state newstate
            return newstate
        }

    let evaluateBlueItemOnOpponent item state = 
        
        optionnal {
            let! newstate = state |> (evalCanBeUsed item.instanceId)
            let! newstate = newstate |> (evalConsumeMana item.cost)
            let! newstate = newstate |> (evalMyHealthChange item.myHealthChange)        
            let newstate = newstate |> (evalOpponentHealthChange item.opponentHealthChange)        
            let newstate = newstate |> (evalOpponentHealthChange item.defense)        
            let newstate = newstate |> (evalCardDraw item.cardDraw)
            let newstate = {newstate with 
                                myHand = {newstate.myHand with 
                                            blueItems = newstate.myHand.blueItems.Remove(BlueItemInfo item)}
                                haveBeenUsed = newstate.haveBeenUsed.Add item.instanceId
                        }
            let! newstate = evalBetterScore state newstate
            return newstate
        }

    let evaluateRedItem item ennemy state = 
        
        let newEnnemy = ennemy 
                        |> evalAbilities item  false
                        |> hitted (-item.defense)
                        |> incrementAttack item.attack

        let board = match newEnnemy |>isDead with
                    | true -> state.oppBoard.Remove(EnnemyInfo ennemy)
                    | false -> state.oppBoard.Remove(EnnemyInfo ennemy).Add(EnnemyInfo newEnnemy)

        let newstate = {state with oppBoard = board }
        optionnal {
            let! newstate = newstate |> (evalCanBeUsed item.instanceId)
            let! newstate = newstate |> (evalConsumeMana item.cost)
            let! newstate = newstate |> (evalMyHealthChange item.myHealthChange)    
            let newstate = newstate |> (evalOpponentHealthChange item.opponentHealthChange)   
            let newstate = newstate |> (evalCardDraw item.cardDraw)
            let newstate = {newstate with 
                                myHand = {newstate.myHand with 
                                            redItems = newstate.myHand.redItems.Remove(RedItemInfo item)}
                                haveBeenUsed = newstate.haveBeenUsed.Add item.instanceId
                        }

            let! newstate = evalBetterScore state newstate
            return newstate
        }

    let evaluateGreenItem item minion state = 
        
        let newMinion = minion 
                        |> evalAbilities item true  
                        |> incrementAttack item.attack
                        |> incrementDefense item.defense

        let newState = {state with myBoard = state.myBoard.Remove(MinionInfo minion).Add(MinionInfo newMinion) }
        optionnal {
            let! newState = newState |> (evalCanBeUsed item.instanceId)
            let! newState = newState |> (evalConsumeMana item.cost)

            // TODO : dont use if minion have all same power 

            // dont use if any lethal, can add if ward ! 
            let anyLethaalEnnemy = newState.oppBoard |> Set.toList |> List.tryFind (fun (EnnemyInfo x) -> x.abilities.lethal)
            
            let! newState = match anyLethaalEnnemy, (item.abilities.ward || minion.abilities.ward) with
                            | Some _ , true   -> Some newState
                            | Some _ , false -> None
                            | None , _ -> Some newState

            // dont use when still can die
            // let! newState = match newState.oppBoard |> Set.toList |> List.tryFind (fun (EnnemyInfo x) -> x.attack >= minion.defense) with
            //                 | Some _ -> None
            //                 | None -> Some state

            let! newState = newState |> (evalMyHealthChange item.myHealthChange)        
            //let! state = state |> (evalOpponentHealthChange item.opponentHealthChange)   // # dont need in green     
            let newState = newState |> (evalCardDraw item.cardDraw)
            let newState = {newState with 
                                myHand = {newState.myHand with greenItems = newState.myHand.greenItems.Remove(GreenItemInfo item)}
                                haveBeenUsed = newState.haveBeenUsed.Add item.instanceId
            }
            
            //eprintfn "eval green OK"
            return newState
        }
//#endregion

//#region ============  ACTION EVAL DISPATCHER  ============
    let evaluateAction (state:GameBattle) action =

        optionnal {
            match action with
                | Pick id -> return state // should not be used
                | Pass -> return state
                | Summon (MinionInfo(m)) -> return! (state |> (evaluateSummon m))
                | AttackEnnemy (MinionInfo(m),EnnemyInfo(e)) -> return! (state |> evaluateAttackEnemy m e)
                | AttackOpponent (MinionInfo(m)) -> return! (state |> evaluateAttackOpponent m )
                | UseBlueItem (BlueItemInfo(i), Some(EnnemyInfo(e))) -> return! (state |> (evaluateBlueItemOnEnnemy i e) )
                | UseBlueItem (BlueItemInfo(i), None ) -> return! (state |> (evaluateBlueItemOnOpponent i ) )
                | UseGreenItem (GreenItemInfo(i), MinionInfo(m)) -> return! (state |> (evaluateGreenItem i m) )
                | UseRedItem (RedItemInfo(i), EnnemyInfo(e)) -> return! (state |> (evaluateRedItem i e) )    
        }

    let summonActions state = seq{yield! state.myHand.minion |> Set.map Summon}
    
    let blueActions state =  
        seq{
            for item in state.myHand.blueItems do
                yield (UseBlueItem(item, None)) 
                for ennemy in state.oppBoard do
                    yield (UseBlueItem(item, Some ennemy)) 
        }

    let redActions state = 
        seq{
            for item in state.myHand.redItems do
                for ennemy in state.oppBoard do
                    yield (UseRedItem(item, ennemy)) 
        }

    let greenActions state =  
        seq{
            for item in state.myHand.greenItems do
                for minion in state.myBoard do
                    yield (UseGreenItem(item, minion)) 
        }

    let attackActions state =  
        seq{
            for m in state.myBoard do
                yield (AttackOpponent m )
                for e in state.oppBoard do
                    yield (AttackEnnemy(m, e)) 
        }

    let availableActions state = 
        seq {
            yield! summonActions state
            yield! blueActions state
            yield! redActions state
            yield! greenActions state
            yield! attackActions state
        }
//#endregion

    let playDraft (state:GameState) (cards: Card[])=
        
        let cardVals = cards |> Array.map (valueForDraft state.myDeck)

        let sorted = 
            cardVals
            |> Array.mapi (fun i v -> i,v)
            |> Array.sortByDescending (fun (i, v)-> v)
            
        let pickedIndex = sorted.[0] |> fst
        
        printfn "%s %i %i %i" ([Pick pickedIndex] |> executeActions) cardVals.[0] cardVals.[1] cardVals.[2]

        let rejected = [cards.[sorted.[1]|> fst] ; cards.[sorted.[2]|> fst] ]
        let pickedCard = cards.[sorted.[0]|> fst]

        let newDeck = (pickedCard, rejected) ||> addIn state.myDeck
        {state with myDeck = newDeck}

    let rec innerBattleLoop (state:GameBattle) cmds fnGetActions =
            
            let actions = 
                fnGetActions state
                |> Seq.map (fun action -> action, action |> evaluateAction state)
                |> Seq.filter (fun (a, s) -> s.IsSome)
                |> Seq.map (fun (a, s) -> a, s.Value)
                |> List.ofSeq

            //eprintfn "Possible ACTIONs IS %i" (actions |>Seq.length)

            let actions = actions |> List.sortByDescending (fun (a, s) -> s|> score)

            actions 
                |> function
                | [] -> 
                    state, cmds
                | (a,s)::_ -> 
                    //eprintfn "NEXT ACTION IS %s -> %f" (a |> getActionStr) (s |> score) 
                    // eprintfn ""
                    innerBattleLoop s (a:: cmds) fnGetActions

    let rec innerBattleLoopBst (state:GameBattle) (cmds: Action list) fnGetActions =
            
            let actions = 
                fnGetActions state
                |> Seq.map (fun action -> action, action |> evaluateAction state)
                |> Seq.filter (fun (a, s) -> s.IsSome)
                |> Seq.map (fun (a, s) -> a, s.Value)
                |> Seq.filter (fun (a, s) -> (s |> score) >= (state |> score) ) // BOF BOF
                |> List.ofSeq

            // NOT TAIL !!!!
            match actions with
            | [] -> state,cmds
            | _ -> 
                let newState, newCmds = 
                    actions 
                    |> List.map (fun (act, nextState ) ->  innerBattleLoopBst nextState (act :: cmds) fnGetActions )
                    |> List.maxBy (fun (nextState, _  ) -> (nextState|> score))
    
                //eprintfn "Actions : %i - Score: %i" newCmds.Length (newState |> score)
                newState,newCmds    

    let  playSummon (state:GameBattle) = 
        summonActions |> innerBattleLoop state List.empty

    let  playAttacks (state:GameBattle) = 
        
        let getActions s = [
            for m in s.myBoard do
                yield (AttackOpponent m )
                for e in s.oppBoard do
                    yield (AttackEnnemy(m, e))  
        ] 
        getActions |> innerBattleLoop state List.empty

    let  playAttackOpponent (state:GameBattle) = 
        
        let getActions s = [
            for m in state.myBoard do
                yield (AttackOpponent m )
        ] 
        getActions |> innerBattleLoop state List.empty

    let  playAllItems (state:GameBattle) = 
        
        let getActions s = [
            // GREEN
            for item in state.myHand.greenItems do
                for minion in state.myBoard do
                    yield (UseGreenItem(item, minion)) 
            // BLUE
            for item in s.myHand.blueItems do
                yield (UseBlueItem(item, None)) 
                for ennemy in s.oppBoard do
                        yield (UseBlueItem(item, Some ennemy)) 
        
            // RED
            for item in s.myHand.redItems do
                for ennemy in s.oppBoard do
                    yield (UseRedItem(item, ennemy)) 
                
        ] 
        getActions |> innerBattleLoop state List.empty


    let playBattleBst (state:GameBattle) =

            let anyGuard = state.oppBoard |> Seq.tryFind (fun (EnnemyInfo x) -> x.abilities.guard)
            let sumAtk = state.myBoard |> Seq.sumBy (fun (MinionInfo m) -> m.attack)
            // fast kill
            match anyGuard, sumAtk with
            | None , atk when atk >= state.opponent.health -> 
                let newState, attackActions = (playAttackOpponent state)
                newState, (attackActions|>List.rev), "Good Game"
            | _ -> 

            let redBlueActions state = [ 
                yield! (redActions state)
                yield! (blueActions state)
            ]  
            let newState, allActions = innerBattleLoopBst state List.empty redBlueActions

            let newState, allActions = innerBattleLoopBst newState allActions summonActions
            let otherActions state = [ 
                yield! (greenActions state)
                yield! (attackActions state)
            ]  
            let newState, allActions = innerBattleLoopBst newState allActions otherActions


            let oldScore = state |> score
            let newScore = newState |> score
            let message = 
                        if oldScore < newScore then "Should be nice :)"
                        else if oldScore > newScore  then "Outch"
                        else "Keep cool and summon more"

            newState, allActions |>List.rev, message

    let innerPlayBattleMut (state:GameBattle) fnGetActions=
            
        let anyGuard = state.oppBoard |> Seq.tryFind (fun (EnnemyInfo x) -> x.abilities.guard)
        let sumAtk = state.myBoard |> Seq.sumBy (fun (MinionInfo m) -> m.attack)
        // fast kill
        match anyGuard, sumAtk with
        | None , atk when atk >= state.opponent.health -> 
            let newState, attackActions = (playAttackOpponent state)
            newState, (attackActions|>List.rev), "Good Game"
        | _ -> 
            let mutable best = (state)
            let visitedFrom = new System.Collections.Generic.Dictionary<GameBattle,GameBattle * Action>()
            let toVisit = new System.Collections.Generic.Stack<GameBattle>()

            toVisit.Push(state)

            visitedFrom.Add(best, (GameBattle.empty,Action.Pass))

            while toVisit.Count > 0 do
                let current = toVisit.Pop()
                
                
                if (current |> score) > (best |> score)
                then best <- current
                else () 

                fnGetActions current 
                |> Seq.map (fun action -> action, action |> evaluateAction current)
                |> Seq.filter (fun (a, s) -> s.IsSome)
                |> Seq.map (fun (a, s) -> a, s.Value)
                |> Seq.filter (fun (a, s) -> visitedFrom.ContainsKey(s) |> not && (s |>score) > (state |>score ) )
                |> Seq.iter (fun (a, s) -> 
                    toVisit.Push(s);
                    visitedFrom.Add(s, (current, a))
                    //eprintfn "ADD %i => %i  - %s" (current |> score ) (s |> score ) (a |> getActionStr) 
                )

            let newState = best
            let allActions = seq {
                                while best <> GameBattle.empty do
                                    let s,a = visitedFrom.[best]
                                    //eprintfn "ACTION DONE %s" (a |> getActionStr) 
                                    match a with
                                    | Pass -> ()
                                    | _ -> yield a 
                                    best <- s
                            }


            let oldScore = state |> score
            let newScore = newState |> score
            let message = 
                        if oldScore < newScore then "Should be nice :)"
                        else if oldScore > newScore  then "Outch"
                        else "Keep cool and summon more"

            newState, allActions |> Seq.rev |> List.ofSeq , message
            
    let playBattleMut (state:GameBattle) =

        // let redBlueActions state = [ 
        //         yield! (getRedActions state)
        //         yield! (getBlueActions state)
        //     ]  
        // let newState, allActions1, message1 = innerPlayBattleMut state redBlueActions

        // let newState, allActions2, message2 = innerPlayBattleMut newState getSummonActions

        // let otherActions state = [ 
        //         yield! (getGreenActions state)
        //         yield! (getAttackActions state)
        //     ]  
        // let newState, allActions3, message3 = innerPlayBattleMut newState otherActions

        // newState, [
        //     yield!allActions1
        //     yield!allActions2
        //     yield!allActions3

        // ], message3

        innerPlayBattleMut state availableActions

    let playBattle (state:GameBattle) =

            let anyGuard = state.oppBoard |> Seq.tryFind (fun (EnnemyInfo x) -> x.abilities.guard)
            let sumAtk = state.myBoard |> Seq.sumBy (fun (MinionInfo m) -> m.attack)
            // fast kill
            match anyGuard, sumAtk with
            | None , atk when atk >= state.opponent.health -> 
                let newState, attackActions = (playAttackOpponent state)
                newState, (attackActions|>List.rev), "Good Game"
            | _ -> 
                let newState, summonActions = playSummon state
                let newState, itemActions = playAllItems newState
                let newState, attackActions = playAttacks newState

                let allActions = 
                    [
                            yield! (summonActions |> Seq.rev)
                            yield! (itemActions |> Seq.rev)
                            yield! (attackActions |> Seq.rev)
                    ]
                let oldScore = state |> score
                let newScore = newState |> score
                let message = 
                            if oldScore < newScore then "Should be nice :)"
                            else if oldScore > newScore  then "Outch"
                            else "Keep cool and summon more"

                newState, allActions, message
        
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
                    | InDraft -> (cards|> Array.ofList) |> GameLogic.playDraft state 
                    | InBattle ->   

                        let battle  = GameBattle.create me opponent cards opponentHand
                        eprintfn "BEGIN : %i" (battle |> score)
                        //eprintfn "BEGIN== opp H:%i with %i creatures" battle.opponent.health battle.oppBoard.Count
                        let battleState1, actions1, message1 = ((GameBattle.create me opponent cards opponentHand) |> GameLogic.playBattle )
                        let outString1 = actions1 |> List.ofSeq |> executeActions
                        //printfn "%s Score is %f" outString (battleState |> score)
                        //printfn "%s %s" outString1 message1
                        //eprintfn "END : %i" (battleState1 |> score)
                        //{state with battle = battleState1}

                        // TWICE
                        let battleState2, actions2, message2 = (battleState1 |> GameLogic.playBattleMut )
                        
                        let outString2 = actions2 |> List.ofSeq |> executeActions
                        //printfn "%s Score is %f" outString (battleState |> score)
                        printfn "%s %s;%s %s" outString1 message1 outString2 message2
                        eprintfn "END : %i" (battleState2 |> score)
                        //eprintfn "END== opp H:%i with %i creatures" battle.opponent.health battle.oppBoard.Count
                        {state with battle = battleState2}
                        
    stopWatch.Stop()
    eprintfn "%f" stopWatch.Elapsed.TotalMilliseconds
    mainLoop {newState with turn = newState.turn + 1}

// start 
mainLoop GameState.empty

// [<EntryPoint>]
// let main args =
    
//     let stopWatch = System.Diagnostics.Stopwatch.StartNew()
    

//     let state = {
//         me= {health = 30; mana = 3; deck = 2; rune = 5}
//         opponent= {health = 30; mana = 0; deck = 2; rune = 5}
        
//         myHand = { 
//                 greenItems= Set.empty.Add(GreenItemInfo {
//                     cardNumber = 123;
//                     instanceId= 13;
//                     location= CardLocation.InMyHand;
//                     cardType= CardType.GreenItem;
//                     cost= 2;
//                     attack= 4;
//                     defense= 0;
//                     abilities= {breakthrough=false;charge=false;guard=false;drain=false;lethal=false;ward=false}
//                     myHealthChange= 0;
//                     opponentHealthChange= 0;
//                     cardDraw= 0
//                 })
//                 minion= Set.empty 
//                 redItems= Set.empty.Add(RedItemInfo {
//                     cardNumber = 147;
//                     instanceId= 9;
//                     location= CardLocation.InMyHand;
//                     cardType= CardType.RedItem;
//                     cost= 2;
//                     attack= 0;
//                     defense= -1;
//                     abilities= {breakthrough=false;charge=false;guard=false;drain=false;lethal=false;ward=false}
//                     myHealthChange= 0;
//                     opponentHealthChange= 0;
//                     cardDraw= 1
//                 })
//                 blueItems= Set.empty
//                 }
//         myBoard= Set.empty.Add( MinionInfo {
//                     cardNumber = 92;
//                     instanceId= 3;
//                     location= CardLocation.OnMyBoard;
//                     cardType= CardType.Creature;
//                     cost= 1;
//                     attack= 0;
//                     defense= 1;
//                     abilities= {breakthrough=false;charge=false;guard=true;drain=false;lethal=false;ward=false}
//                     myHealthChange= 0;
//                     opponentHealthChange= 0;
//                     cardDraw= 0
//                 })
//                 .Add( MinionInfo {
//                     cardNumber = 2;
//                     instanceId= 5;
//                     location= CardLocation.OnMyBoard;
//                     cardType= CardType.Creature;
//                     cost= 1;
//                     attack= 1;
//                     defense= 2;
//                     abilities= {breakthrough=false;charge=false;guard=false;drain=false;lethal=false;ward=false}
//                     myHealthChange= 0;
//                     opponentHealthChange= -1;
//                     cardDraw= 0
//                 })
//         oppBoard= Set.empty.Add( EnnemyInfo {
//                     cardNumber = 7;
//                     instanceId= 8;
//                     location= CardLocation.OnOpponentBoard;
//                     cardType= CardType.Creature;
//                     cost= 2;
//                     attack= 2;
//                     defense= 2;
//                     abilities= {breakthrough=false;charge=false;guard=false;drain=false;lethal=false;ward=true}
//                     myHealthChange= 0;
//                     opponentHealthChange= 0;
//                     cardDraw= 0
//                 })
//         opponentHandCount= 4
//         myCardDraw = 1
//         haveBeenUsed= Set.empty
//     }

//     stopWatch.Stop()
//     eprintfn "%f" stopWatch.Elapsed.TotalMilliseconds
//     stopWatch.Restart()
//     //let newState, actions, messqge = GameLogic.playBattleBst state
//     let newState, actions, messqge = GameLogic.playBattleMut state
//     let score = newState |> score
//     stopWatch.Stop()
//     eprintfn "%f" stopWatch.Elapsed.TotalMilliseconds
//     actions |> Seq.iter  (getActionStr >> printfn "%s")
//     0