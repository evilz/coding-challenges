open System

type Player = {
    Health: int
    Mana: int
    Deck: int
    Rune: int
}

type Location =
| Hand
| Board
| OpponentBoard

type CardType =
| Creature
| GreenItem
| RedItem
| BlueItem

type BoardStatus =
| Playable
| Played

type Card = {
    Id: int
    InstanceId: int
    Location: Location
    Type: CardType
    Cost: int
    Attack: int
    Defense: int
    Heal: int
    Curse: int
    Draw: int
    Breakthrough: bool
    Charge: bool
    Guard: bool
    Drain: bool
    Lethal: bool
    Ward: bool
}

type Hand = {
    Cards: Card list
    PlayableCards: Card list
}

type GameTurn = {
    Player: Player
    Opponent: Player
    Hand: Hand
    Board: (BoardStatus * Card) list
    OpponentBoard: Card list
    Actions: string list
}

let readPlayer() =
    let token = (Console.In.ReadLine()).Split [|' '|]

    let playerHealth = int(token.[0])
    let playerMana = int(token.[1])
    let playerDeck = int(token.[2])
    let playerRune = int(token.[3])

    { Health = playerHealth; Mana = playerMana; Deck = playerDeck; Rune = playerRune }

// Evilz : cardIndex pas utilisé
let readCard cardIndex =
    let token = (Console.In.ReadLine()).Split [|' '|]
    let abilities = token.[7]

    {
        Id = int(token.[0])
        InstanceId = int(token.[1])
        Location = 
            match int(token.[2]) with
            | 0 -> Hand
            | 1 -> Board
            | -1 -> OpponentBoard
            | _ -> failwith "Location value must be part of [-1; 0; 1]"
        Type =
            match int(token.[3]) with
                | 0 -> Creature
                | 1 -> GreenItem
                | 2 -> RedItem
                | 3 -> BlueItem
                | _ -> failwith "Card type value must be part of [0; 1; 2; 3]"
        Cost = int(token.[4])
        Attack = int(token.[5])
        Defense = int(token.[6])
        Heal = int(token.[8])
        Curse = int(token.[9])
        Draw = int(token.[10])
        Breakthrough = abilities.[0] = 'B'
        Charge = abilities.[1] = 'C'
        Drain = abilities.[2] = 'D'
        Guard = abilities.[3] = 'G'
        Lethal = abilities.[4] = 'L'
        Ward = abilities.[5] = 'W'
    }

let readCards() =
    let cardCount = int(Console.In.ReadLine())

    [ 0 .. cardCount - 1 ]
    |> Seq.map readCard
    |> Seq.toArray

// Evilz : Arguments pas utilise ??
let playDraft turn player opponent cards =
    let creatures =
        cards
        |> Seq.filter (fun card -> card.Type = Creature)
        |> Seq.filter (fun card -> card.Cost <= 9)
        |> Seq.toList

    if List.isEmpty creatures then
        let rnd = System.Random()
        let randomPick = rnd.Next(0, 3)

        printfn "PICK %d" randomPick
        ()
    else
        let bestCombinaison =
            creatures
            |> Seq.map(fun card -> (card.Guard, card.Ward, card.Charge, card)) // Evilz : ??? WTF so smart

            |> Seq.sort
            |> Seq.last

        let (_, _, _, bestCard) = bestCombinaison

        let bestPick = Array.findIndex (fun card -> card = bestCard) cards

        printfn "PICK %d" bestPick
        ()

let removeCard card cards =
    cards
    |> Seq.filter (fun c -> c <> card)
    |> Seq.toList  // Evilz Tu utilises pas les list

let rec playBattleSummon gameTurn =
    let playableCreatures =
        gameTurn.Hand.PlayableCards
        |> Seq.filter (fun card -> card.Type = Creature)
        |> Seq.toList

    if List.isEmpty playableCreatures then
        gameTurn
    else
        // TODO : Improve by choosing best creature is sutable
        let mostCostlyCreature =
            playableCreatures
            |> Seq.sortBy(fun card -> -card.Attack)
            |> Seq.head

        let statusBoard =
            match mostCostlyCreature.Charge with
            | true -> Playable
            | false -> Played

        let summonCommand = sprintf "SUMMON %d" mostCostlyCreature.InstanceId

        playBattleSummon { gameTurn with
            Actions = gameTurn.Actions @ [ summonCommand ]
            Player = { gameTurn.Player with
                Mana = gameTurn.Player.Mana - mostCostlyCreature.Cost
            }
            Board = (statusBoard, mostCostlyCreature) :: gameTurn.Board
            Hand = { gameTurn.Hand with
                PlayableCards =
                    removeCard mostCostlyCreature gameTurn.Hand.PlayableCards
                    |> Seq.filter (fun card -> card.Cost <= gameTurn.Player.Mana - mostCostlyCreature.Cost)
                    |> Seq.toList
            }
        }

let rec playBattleAttackGuarded gameTurn =
    let guardedCreatures =
        gameTurn.OpponentBoard
        |> Seq.filter (fun creature -> creature.Guard)
        |> Seq.toList

    let attackerCreatures =
        gameTurn.Board
        |> Seq.filter(fun boardCreature -> fst boardCreature = Playable)
        |> Seq.map(fun boardCreature -> snd boardCreature)

    if Seq.isEmpty guardedCreatures || Seq.isEmpty attackerCreatures then
        gameTurn
    else
        let attacker = Seq.head attackerCreatures
        let target = Seq.head guardedCreatures

        let attackCommand = 
            sprintf "ATTACK %d %d" attacker.InstanceId target.InstanceId

        playBattleAttackGuarded { gameTurn with
            Actions = gameTurn.Actions @ [ attackCommand ]
            OpponentBoard =
                gameTurn.OpponentBoard
                |> Seq.map (fun boardCard ->
                    if boardCard = target then
                        { target with Defense = target.Defense - attacker.Attack }
                    else
                        boardCard)
                |> Seq.filter (fun card -> card.Defense > 0) 
                |> Seq.toList
            Board = 
                gameTurn.Board
                |> Seq.map (fun boardCard -> if boardCard = (Playable, attacker) then (Played, attacker) else boardCard)
                |> Seq.toList
        }

let playBattleAttack gameTurn =
    let attackCommands = 
        gameTurn.Board
        |> Seq.filter (fun boardCreature -> fst boardCreature = Playable)
        |> Seq.map(fun boardCreature -> snd boardCreature)
        |> Seq.map(fun creature -> sprintf "ATTACK %d -1" creature.InstanceId)
        |> Seq.toList

    { gameTurn with Actions = gameTurn.Actions @ attackCommands }

let startBattle player opponent cards =
    let handCards =
        cards
        |> Seq.filter (fun card -> card.Location = Hand)
        |> Seq.toList

    let playableCards =
        cards
        |> Seq.filter (fun card -> card.Location = Hand)
        |> Seq.filter (fun card -> card.Cost <= player.Mana)
        |> Seq.toList

    let onBoard =
        cards
        |> Seq.filter (fun card -> card.Location = Board)
        |> Seq.map(fun card -> (Playable, card))
        |> Seq.toList

    let opponentBoardCards =
        cards
        |> Seq.filter (fun card -> card.Location = OpponentBoard)
        |> Seq.toList

    { Player = player; Opponent = opponent; Hand = { Cards = handCards; PlayableCards = playableCards }; Board = onBoard; OpponentBoard = opponentBoardCards; Actions = [] }

let playBattle turn player opponent cards =
    let initialState = startBattle player opponent cards
    let strategy =
        playBattleSummon
        >> playBattleAttackGuarded
        >> playBattleAttack

    let outputState = strategy initialState
    let outputCommands = (";", outputState.Actions) |> String.Join
    
    printfn "%s" outputCommands
    ()

let play turn =
    let player = readPlayer()
    let opponent = readPlayer()

    let opponentHand = int(Console.In.ReadLine())
    let cards = readCards()

    eprintfn "Turn #%d" turn

    let playStrategy =
        match turn with
        | t when 1 <= t && t <= 30 -> playDraft
        | t when 30 < t -> playBattle
        | _ -> failwith "Strategy for this turn had not been defined"

    playStrategy turn player opponent cards

// Evilz Make recursive function ?
[ 1 .. 3000 ]
|> Seq.map play
|> Seq.toArray