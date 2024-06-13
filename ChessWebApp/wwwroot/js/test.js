import { Chessboard, FEN, COLOR, INPUT_EVENT_TYPE } from "./cm-chessboard-master/src/Chessboard.js"
const board = new Chessboard(document.getElementById("board"), {
    position: "rn2k1r1/ppp1pp1p/3p2p1/5bn1/P7/2N2B2/1PPPPP2/2BNK1RR w Gkq - 4 11",
    assetsUrl: "./js/cm-chessboard-master/assets/" // wherever you copied the assets folder to, could also be in the node_modules folder
})
board.enableMoveInput(inputHandler)
function inputHandler(event) {
    console.log(event)
    switch (event.type) {
        case INPUT_EVENT_TYPE.moveInputStarted:
            console.log(`moveInputStarted: ${event.squareFrom}`)
            return true // false cancels move
        case INPUT_EVENT_TYPE.validateMoveInput:
            console.log(`validateMoveInput: ${event.squareFrom}-${event.squareTo}`)
            return true // false cancels move
        case INPUT_EVENT_TYPE.moveInputCanceled:
            console.log(`moveInputCanceled`)
            break
        case INPUT_EVENT_TYPE.moveInputFinished:
            console.log(`moveInputFinished`)
            break
        case INPUT_EVENT_TYPE.movingOverSquare:
            console.log(`movingOverSquare: ${event.squareTo}`)
            break
    }
}