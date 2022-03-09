module App

open Browser.Dom
open Browser.Types
open Fable.Core.JsInterop

open Snake


let startButton = document.getElementById "startButton" :?> HTMLButtonElement
let widthInput = document.getElementById "gridWidth" :?> HTMLInputElement
let heightInput = document.getElementById "gridHeight" :?> HTMLInputElement

let canvas = document.getElementById "canvas" :?> HTMLCanvasElement
let ctx = canvas.getContext_2d()

let mutable latestKey = ""
document.addEventListener("keydown", fun e ->
    latestKey <- (e :?> KeyboardEvent).key
, true)

(document.getElementById "leftButton").onclick <- fun _ ->
    latestKey <- "ArrowLeft"
(document.getElementById "upButton").onclick <- fun _ ->
    latestKey <- "ArrowUp"
(document.getElementById "downButton").onclick <- fun _ ->
    latestKey <- "ArrowDown"
(document.getElementById "rightButton").onclick <- fun _ ->
    latestKey <- "ArrowRight"

let createImage path =
    let img = document.createElement "img" :?> HTMLImageElement
    img.src <- path
    img
let foodImage = createImage "food.png"
let snakeImage = createImage "snake.png"
let snakeHeadImage = createImage "snakehead.png"

let drawImage image (x, y) =
    let offset z = float <| 32*(z-1)
    ctx.drawImage(!^image, offset x, offset y)
let drawFood = drawImage foodImage
let drawSnake = drawImage snakeImage
let drawSnakeHead = drawImage snakeHeadImage

let rec playContext direction context =
    if not context.IsAlive then
        ctx.fillStyle <- !^"rgba(0,0,0,0.7)"
        ctx.fillRect(0., 0., canvas.width, canvas.height)
        
        ctx.textAlign <- "center"
        ctx.fillStyle <- !^"white"
        ctx.fillText("Game over!", canvas.width/2., canvas.height/2.)

        startButton.disabled <- false
    else
        let (width, height) = context.Size
        canvas.width <- float <| width*32
        canvas.height <- float <| height*32

        drawFood context.Food
        drawSnakeHead context.Snake.Head
        context.Snake.Tail |> List.iter drawSnake

        window.setTimeout((fun _ ->
            let newDirection =
                match latestKey with
                | "ArrowUp" | "w" | "W" -> Up
                | "ArrowLeft" | "a" | "A" -> Left
                | "ArrowRight" | "s" | "S" -> Right
                | "ArrowDown" | "d" | "D" -> Down
                | _ -> direction
    
            playContext newDirection <| advanceContext newDirection context
        ), 200) |> ignore


startButton.onclick <- fun _ ->
    startButton.disabled <- true

    let clampSizeToInt (x: float) = int x |> max 3
    let width = widthInput.valueAsNumber |> clampSizeToInt
    let height = heightInput.valueAsNumber |> clampSizeToInt

    latestKey <- "" // to prevent any state from previous games
    playContext <|| getInitialState (width, height)

ctx.textAlign <- "center"
ctx.fillStyle <- !^"black"
ctx.fillText("Press 'Start new game' to start", canvas.width/2., canvas.height/2.)