import {
    INPUT_EVENT_TYPE,
    COLOR,
    Chessboard,
    BORDER_TYPE,
    FEN
} from "../lib/cm-chessboard-master/src/Chessboard.js";
import {
    MARKER_TYPE,
    Markers
} from "../lib/cm-chessboard-master/src/extensions/markers/Markers.js";
import {
    PROMOTION_DIALOG_RESULT_TYPE,
    PromotionDialog
} from "../lib/cm-chessboard-master/src/extensions/promotion-dialog/PromotionDialog.js";
import { Accessibility } from "../lib/cm-chessboard-master/src/extensions/accessibility/Accessibility.js";
import { Chess } from "https://cdn.jsdelivr.net/npm/chess.mjs@1/src/chess.mjs/Chess.js";
import { } from "https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.js";

// Constants
const PieceTeam = Object.freeze({
    NONE: 0,
    WHITE: 1,
    BLACK: 2
});

const PieceTeamInt = Object.freeze({
    'NONE': 0,
    'white': 1,
    'black': 2
});

const PieceTeamText = Object.freeze({
    0: 'NONE',
    1: 'white',
    2: 'black'
});

const State = Object.freeze({
    NONE: 0,
    IN_PROGRESS: 1,
    WIN_WHITE: 2,
    WIN_BLACK: 3,
    DRAW_STALEMATE: 4,
    DRAW_THREEFOLD_REPETITION: 5,
    DRAW_FIFTY_MOVE_RULE: 6,
    DRAW_INSUFFICIENT_MATERIAL: 7
});

const StateText = Object.freeze({
    0: 'NONE',
    1: 'IN_PROGRESS',
    2: 'WIN_WHITE',
    3: 'WIN_BLACK',
    4: 'DRAW_STALEMATE',
    5: 'DRAW_THREEFOLD_REPETITION',
    6: 'DRAW_FIFTY_MOVE_RULE',
    7: 'DRAW_INSUFFICIENT_MATERIAL'
});

// Global variables
let game;
let signalRConnection;
let playerTeam;
var audio = new Audio('/assets/mp3/chess-move-sound.mp3');
var boardStartAudio = new Audio('/assets/mp3/board-start.mp3');


// Chessboard setup
const board = new Chessboard(document.getElementById("board"), {
    position: FEN.start,
    assetsUrl: "./lib/cm-chessboard-master/assets/",
    orientation: COLOR.white,
    style: {
        cssClass: "green", // set the css theme of the board, try "green", "blue" or "chess-club"
        showCoordinates: true, // show ranks and files
        borderType:
            BORDER_TYPE.frame, // "thin" thin border, "frame" wide border with coordinates in it, "none" no border
        aspectRatio: 1, // height/width of the board
        pieces: {
            file: "./pieces/staunty.svg", // the filename of the sprite in `assets/pieces/` or an absolute url like `https://…` or `/…`
            tileSize: 40 // the tile size in the sprite
        },
        animationDuration: 300
    }, // pieces animation duration in milliseconds. Disable all animations with `0`
    extensions: [
        { class: Markers, props: { autoMarkers: MARKER_TYPE.square } },
        { class: PromotionDialog },
        { class: Accessibility, props: { visuallyHidden: true } }
    ]
});

// ------------------------------------------------------
///////////////// EVENT HANDLERS ////////////////////////
// ------------------------------------------------------


// Start game button
document.getElementById("startAIGame").addEventListener("click", handleGameStart);
async function handleGameStart() {
    try {
        await initializeSignalRConnection();
        await startSignalRConnection();
        await invokeStartGame();
    }
    catch (error) {
        window.alert(error);
    }

}

async function initializeSignalRConnection() {
    if (!signalRConnection) {
        signalRConnection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/offlinegame", {
                transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents | signalR.HttpTransportType.LongPolling
            })
            .withAutomaticReconnect()
            .build();

        signalRConnection.on("ReceiveStockfishMove", handleStockfishMove);
    }
}

async function startSignalRConnection() {
    if (signalRConnection.state !== signalR.HubConnectionState.Connected) {
        await signalRConnection.start();
    }
}

async function invokeStartGame() {
    board.setPosition(FEN.start, true);
    boardStartAudio.play();

    playerTeam = document.getElementById('black').checked ? 'black' : 'white';
    if (playerTeam === 'black') {
        board.setOrientation(COLOR.black, true);
    } else {
        board.setOrientation(COLOR.white, true);
    }
    board.disableMoveInput();
    let enableColor = playerTeam === 'white' ? COLOR.white : COLOR.black;
    try {
        const response = await signalRConnection.invoke("StartGame", playerTeam);
        game = JSON.parse(response);
        game.LegalMoves = transformLegalMovesToDictionary(game.LegalMoves);
        board.disableMoveInput();
        board.setPosition(game.Fen, true);
        if (playerTeam === "black") {
            audio.play();

        }

        if (game.Turn === PieceTeamInt[playerTeam]) {
            board.enableMoveInput(inputHandler, enableColor);
        }
    } catch (error) {
        console.error("Error starting game:", error);
        window.alert("Error al iniciar la partida: " + error.toString());
    }
}

function handleStockfishMove(response) {
    game = JSON.parse(response);
    board.setPosition(game.Fen, true);
    drawMoveHistory(game.MoveHistory);
    audio.play();

    if (game.State === State.IN_PROGRESS) {
        if (game.LegalMoves) {
            game.LegalMoves = transformLegalMovesToDictionary(game.LegalMoves);
            let enableColor = playerTeam === 'white' ? COLOR.white : COLOR.black;
            board.enableMoveInput(inputHandler, enableColor);
        }
    } else {
        openModal();
    }
}


// Input handler
async function inputHandler(event) {
    console.log("inputHandler", event);

    switch (event.type) {
        case INPUT_EVENT_TYPE.movingOverSquare:
            handleMovingOverSquare(event);
            return;

        case INPUT_EVENT_TYPE.moveInputStarted:
            return handleMoveInputStarted(event);

        case INPUT_EVENT_TYPE.validateMoveInput:
            return await handleValidateMoveInput(event);

        case INPUT_EVENT_TYPE.moveInputFinished:
            handleMoveInputFinished(event);
            break;
        default:
            event.chessboard.removeMarkers(MARKER_TYPE.dot);
            event.chessboard.removeMarkers(MARKER_TYPE.bevel);
            break;
    }
}

// Move started
function handleMoveInputStarted(event) {
    if (game.LegalMoves) {
        const sourceLegalMoves = game.LegalMoves[event.squareFrom];
        if (sourceLegalMoves) {
            sourceLegalMoves.forEach((move) => {
                if (event.chessboard.getPiece(move)) {
                    event.chessboard.addMarker(MARKER_TYPE.frameDanger, move);
                } else {
                    event.chessboard.addMarker(MARKER_TYPE.dot, move);
                }
            });
            return sourceLegalMoves.length > 0;
        }
    }
    return false;
}

// Move over square
function handleMovingOverSquare(event) {
}

// Validate move input
async function handleValidateMoveInput(event) {
    const move = event.squareFrom + event.squareTo;
    let moveResult = false;

    if (!game.LegalMoves) return false;
    if (!game.LegalMoves[event.squareFrom]) return false;
    if (!game.LegalMoves[event.squareFrom].includes(event.squareTo)) return false;

    try {
        const response = await signalRConnection.invoke("ProcessMove", move);

        let auxGame = JSON.parse(response);
        moveResult = auxGame.MoveResult.MoveSquareResult.IsSuccessful;

        if (moveResult) {
            game = auxGame;
            drawMoveHistory(game.MoveHistory);
        }
    }
    catch (error) {
        console.error("Error processing move:", error);
        return false;
    }

    finally {
        return moveResult;
    }
}

// Move finished
function handleMoveInputFinished(event) {
    if (event.legalMove) {
        audio.play();
        board.setPosition(game.Fen);
        event.chessboard.disableMoveInput();
        if (game.Promotion) {
            //TODO: Show promotion dialog, if canceled automatically promote to queen
        }
        else if (game.State !== State.IN_PROGRESS) {
            openModal();
        } else {
            signalRConnection.invoke("GetStockfishMove")
                .catch((error) => {
                    console.error("Error processing move:", error);
                });
        }
    }
    event.chessboard.removeMarkers(MARKER_TYPE.dot);
    event.chessboard.removeMarkers(MARKER_TYPE.frameDanger);
}


// ------------------------------------------------------
///////////////// UTILITY ////////////////////////
// ------------------------------------------------------
function convertIndexToChessNotation(squareIndex) {
    const column = String.fromCharCode('a'.charCodeAt(0) + (squareIndex % 8));
    const row = String.fromCharCode('8'.charCodeAt(0) - Math.floor(squareIndex / 8));
    return `${column}${row}`;
}
function transformLegalMovesToDictionary(legalMoves) {
    const result = {};

    legalMoves.forEach(move => {
        const origin = convertIndexToChessNotation(move.OriginIndex);
        const target = convertIndexToChessNotation(move.TargetIndex);

        if (!result[origin]) {
            result[origin] = [];
        }

        result[origin].push(target);
    });

    return result;
}

function openModal() {
    let endGameTitle = "";
    let endGameBody = "";
    switch (game.State) {
        case State.WIN_WHITE:
            endGameTitle = "White wins";
            endGameBody = "Checkmate!";
            break;
        case State.WIN_BLACK:
            endGameTitle = "Black wins";
            endGameBody = "Checkmate!";
            break;
        case State.DRAW_STALEMATE:
            endGameTitle = "Draw";
            endGameBody = "Stalemate!";
            break;
        case State.DRAW_THREEFOLD_REPETITION:
            endGameTitle = "Draw";
            endGameBody = "Threefold repetition!";
            break;
        case State.DRAW_FIFTY_MOVE_RULE:
            endGameTitle = "Draw";
            endGameBody = "Fifty move rule!";
            break;
        case State.DRAW_INSUFFICIENT_MATERIAL:
            endGameTitle = "Draw";
            endGameBody = "Insufficient material!";
            break;
        default:
            break;
    }
    document.getElementById('endGameTitle').innerText = endGameTitle;
    document.getElementById('endGameBody').innerText = endGameBody;
    const gameInfo = new bootstrap.Modal(document.getElementById('endGameModal'));
    gameInfo.show();
}


// ------------------------------------------------------
///////////////// UI ADJUSTMENTS ////////////////////////
// ------------------------------------------------------
function setDynamicHeight() {
    const nav = document.querySelector('nav');
    const row = document.querySelector('.row');
    const navHeight = nav.offsetHeight;
    row.style.height = `calc(100vh - ${navHeight}px)`;

    const boardContainer = document.querySelector('#boardContainer');
    const rowHeight = row.offsetHeight;
    const opponentInfoHeight = document.querySelector('#opponentInfo').offsetHeight;
    const userInfoHeight = document.querySelector('#userInfo').offsetHeight;
    const boardContainerMaxWidth = rowHeight - opponentInfoHeight - userInfoHeight;
    boardContainer.style.maxWidth = `${boardContainerMaxWidth}px`;
    const opponentInfo = document.querySelector('#auxDiv').style.width = boardContainerMaxWidth + "px"
}
window.addEventListener('load', setDynamicHeight);
window.addEventListener('resize', setDynamicHeight);


function drawMoveHistory(moveHistory) {
    let moveHistoryTable = document.getElementById("moveHistoryTable");
    moveHistoryTable.innerHTML = ''; // Clear previous history

    moveHistory.forEach((move, index) => {
        let row = document.createElement('tr');

        // Create the "Move #" column (th)
        let th = document.createElement('th');
        th.textContent = index + 1;
        th.classList.add("transparent-background");
        row.appendChild(th);

        // Extract white and black moves from the move string
        let whiteMove = move.substring(0, 2); // First two characters
        let blackMove = move.substring(2, 4); // Next two characters

        // Create the "White" column (td)
        let whiteMoveCell = document.createElement('td');
        whiteMoveCell.textContent = whiteMove;
        whiteMoveCell.classList.add("transparent-background");
        row.appendChild(whiteMoveCell);

        // Create the "Black" column (td)
        let blackMoveCell = document.createElement('td');
        blackMoveCell.textContent = blackMove || ''; // Handle cases where there might not be a black move
        blackMoveCell.classList.add("transparent-background");
        row.appendChild(blackMoveCell);

        // Append the row to the table body
        moveHistoryTable.appendChild(row);
    });
}