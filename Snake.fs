module Snake

type Direction =
    | Up
    | Down
    | Left
    | Right

type Point = int * int

type Snake = Point list

type GameContext = {
    IsAlive: bool
    Food: Point
    Snake: Snake
    Size: int * int
}

let initialState = {
    IsAlive = true
    Food = 1,5
    Snake = [1,2; 2,2; 2,3]
    Size = 10, 9
}

let rnd = System.Random()

let addTo (x, y) (xx, yy) = (x+xx, y+yy)

let existsIn list element =
    List.exists (fun x -> x = element) list

let rec newFood body (xLimit, yLimit) =
    let proposal = 1 + rnd.Next(xLimit), 1 + rnd.Next(yLimit)

    if proposal |> existsIn body
    then newFood body (xLimit, yLimit)
    else proposal
        
let getInitialState size =
    let middleOf (x, y) = (x/2+1, y/2+1)

    let head = middleOf size
    let direction, tail1Direction, tail2Direction =
        match rnd.Next(4) with
        | 0 -> Up, (0, 1), (1, 1)
        | 1 -> Down, (0, -1), (-1, -1)
        | 2 -> Left, (1, 0), (1, 1)
        | _ -> Right, (-1, 0), (-1, -1)
    let body = [head; addTo head tail1Direction; addTo head tail2Direction]

    direction, {
        IsAlive = true
        Size = size
        Snake = body
        Food = newFood body size
    }

let advanceContext direction context =
    let rec initOf list =
        match list with
        | [] -> []
        | [e] -> []
        | e :: rest -> e :: initOf rest

    let newHeadOf snake direction =
        let (x, y) = List.head snake
        match direction with
        | Up -> (x, y-1)
        | Down -> (x, y+1)
        | Left -> (x-1, y)
        | Right -> (x+1, y)

    let halfArea (x, y) = x*y/2

    let outOfBounds (x, y) (xLimit, yLimit) =
        x > xLimit || y > yLimit || x < 1 || y < 1

    if context.IsAlive then
        let snake = context.Snake
        let newHead = newHeadOf snake direction
        let growSnake = newHead = context.Food
        let newBody =
            if growSnake
            then newHead :: snake
            else initOf (newHead :: snake)
        let selfCcollided = newHead |> existsIn newBody.Tail
        let newSize =
            if newBody.Length > (halfArea context.Size) && not selfCcollided
            then addTo context.Size (1, 1)
            else context.Size
                    
        { context with
            IsAlive = not (selfCcollided || outOfBounds newHead newSize)
            Snake = newBody
            Food =
                if growSnake
                then newFood newBody context.Size
                else context.Food
            Size = newSize
        }
    else context
