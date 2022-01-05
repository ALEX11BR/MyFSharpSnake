open Snake
open System

let rec playContext direction context =
    let writeAt char position =
        Console.SetCursorPosition position
        Console.Write(char: char)
    let writeWall = writeAt '█'
    let writeSnake = writeAt 'O'
    let writeSnakeHead = writeAt '@'
    let writeFood = writeAt '*'

    let limiter = async{
        do! Async.Sleep 200
    }

    Console.Clear()
    if not context.IsAlive then
        Console.WriteLine "Game over!"
        Environment.Exit 0

    let (xLimit, yLimit) = context.Size

    for x in 0..xLimit+1 do
        writeWall (x, 0)
        writeWall (x, yLimit+1)
    for y in 1..yLimit do
        writeWall (0, y)
        writeWall (xLimit+1, y)

    writeSnakeHead context.Snake.Head

    context.Snake.Tail
    |> List.iter writeSnake

    writeFood context.Food

    Console.SetCursorPosition context.Snake.Head

    Async.RunSynchronously limiter

    let newDirection =
        if Console.KeyAvailable then
            match Console.ReadKey().Key with
            | ConsoleKey.UpArrow -> Up
            | ConsoleKey.DownArrow -> Down
            | ConsoleKey.LeftArrow -> Left
            | ConsoleKey.RightArrow -> Right
            | _ -> direction
        else direction

    playContext newDirection <| advanceContext newDirection context

[<EntryPoint>]
let main argv =
    playContext <|| getInitialState (7, 7)

    0