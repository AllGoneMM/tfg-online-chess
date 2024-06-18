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
    DRAW_INSUFFICIENT_MATERIAL: 7,
    WIN_WHITE_BLACK_ABORT: 8,
    WIN_BLACK_WHITE_ABORT: 9,
    WIN_WHITE_BLACK_RESIGN: 10,
    WIN_BLACK_WHITE_RESIGN:11
});

const StateText = Object.freeze({
    0: 'NONE',
    1: 'IN_PROGRESS',
    2: 'WIN_WHITE',
    3: 'WIN_BLACK',
    4: 'DRAW_STALEMATE',
    5: 'DRAW_THREEFOLD_REPETITION',
    6: 'DRAW_FIFTY_MOVE_RULE',
    7: 'DRAW_INSUFFICIENT_MATERIAL',
    8: 'WIN_WHITE_BLACK_ABORT',
    9: 'WIN_BLACK_WHITE_ABORT',
    10: 'WIN_WHITE_BLACK_RESIGN',
    11: 'WIN_BLACK_WHITE_RESIGN'
});

// Global variables
let game;
var gameInfo;
let signalRConnection;
let playerTeam;
var enableColor;
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


// ENTER QUEUE
document.getElementById("queueUP").addEventListener("click", handleGameStart);
async function handleGameStart() {
    if (signalRConnection && signalRConnection.state === signalR.HubConnectionState.Connected) {
        signalRConnection.stop().then(() => {
            document.getElementById("queueUP").classList.remove("queue-btn");
            document.getElementById("cancelQueueIcon").classList.remove("cancel-queue-icon");
            document.getElementById("spinner").classList.remove("spinner-border", "loading-spinner");
        });
    } else {
        document.getElementById("queueUP").classList.add("queue-btn");
        document.getElementById("cancelQueueIcon").classList.add("cancel-queue-icon");
        document.getElementById("spinner").classList.add("spinner-border", "loading-spinner");

        try {
            await initializeSignalRConnection();
            await enterQueue();
        }
        catch (error) {
            window.alert(error);
        }
    }
}

async function initializeSignalRConnection() {
    if (!signalRConnection) {
        signalRConnection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/onlinegame", {
                transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents | signalR.HttpTransportType.LongPolling
            })
            .withAutomaticReconnect()
            .build();

    }
    signalRConnection
        .on("ReceiveInitialResponse",
            (initialResponse) => {

                document.getElementById("queueUP").classList.remove("queue-btn");
                document.getElementById("cancelQueueIcon").classList.remove("cancel-queue-icon");
                document.getElementById("spinner").classList.remove("spinner-border", "loading-spinner");
                document.getElementById("queueUP").classList.add("hidden");


                // TODO: Implement opponent info loading
                game = JSON.parse(initialResponse);
                playerTeam = PieceTeamText[game.Team];
                enableColor = playerTeam === 'white' ? COLOR.white : COLOR.black;

                board.setPosition(game.Fen, true);
                board.setOrientation(enableColor, true);
                board.disableMoveInput();
                document.getElementById("opponentUsername").innerText = game.OpponentUsername;
                let userUsername = document.getElementById("userUsername");
                if (userUsername) {
                    userUsername.innerText = game.Username;
                }
                boardStartAudio.play();

                if (game.Turn === PieceTeamInt[playerTeam]) {
                    if (game.LegalMoves) {
                        game.LegalMoves = transformLegalMovesToDictionary(game.LegalMoves);
                        board.enableMoveInput(inputHandler, enableColor);
                        console.log(game.LegalMoves);
                    }
                }
            });
    signalRConnection.on("ReceiveResponse",
        (response) => {
            handleResponse(response);
        });
}


async function enterQueue() {
    // Enter the queue
    if (signalRConnection.state !== signalR.HubConnectionState.Connected) {
        await signalRConnection.start()
            .then(() => {
                
            })
            .catch((error) => {
                window.alert(error);
            });
    }
}

function handleResponse(response) {
    game = JSON.parse(response);
    if (game.LegalMoves) {
        game.LegalMoves = transformLegalMovesToDictionary(game.LegalMoves);
    }
    board.setPosition(game.Fen, true);
    if (game.State !== State.IN_PROGRESS) {
        board.disableMoveInput();
        openModal();
    } else {
        board.enableMoveInput(inputHandler, enableColor);
    }
    if (game.MoveHistory) {
        drawMoveHistory(game.MoveHistory);
    }
}

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

// INPUT HANDLER
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
            event.chessboard.removeMarkers(MARKER_TYPE.frameDanger);
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
        event.chessboard.setPosition(game.Fen);
        audio.play();
        if (game.MoveHistory) {
            drawMoveHistory(game.MoveHistory);
        }

        if (game.Promotion) {
            event.chessboard.showPromotionDialog(event.squareTo, enableColor, (result) => {
                console.log("Promotion result", result)
                if (result && result.piece) {
                    signalRConnection.invoke("Promote", result.piece.charAt(1)).then((response => {
                        game = JSON.parse(response)
                        event.chessboard.setPosition(game.Fen);
                        event.chessboard.disableMoveInput();
                    }));

                } else {
                    signalRConnection.invoke("Promote", "q").then((response => {
                        game = JSON.parse(response)
                        event.chessboard.setPosition(game.Fen);
                        event.chessboard.disableMoveInput();
                    }));
                }
            })
        }
        else if (game.State !== State.IN_PROGRESS) {
            event.chessboard.disableMoveInput();
            openModal();
        } else {
            event.chessboard.disableMoveInput();

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
    signalRConnection.stop();
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
        case State.WIN_WHITE_BLACK_ABORT:
            endGameTitle = "White wins";
            endGameBody = "Black aborted!";
            break;
        case State.WIN_BLACK_WHITE_ABORT:
            endGameTitle = "Black wins";
            endGameBody = "White aborted!";
            break;
        default:
            break;

    }
    document.getElementById('endGameTitle').innerText = endGameTitle;
    document.getElementById('endGameBody').innerText = endGameBody;
    const gameInfo = new bootstrap.Modal(document.getElementById('endGameModal'));
    const gameInfoElement = (document.getElementById('endGameModal').addEventListener('hidden.bs.modal', event => {
        gameInfo.dispose();
    }));
    gameInfo.show();
    document.getElementById("queueUP").classList.remove("hidden");
    document.getElementById("queueUP").classList.remove("queue-btn");
    document.getElementById("cancelQueueIcon").classList.remove("cancel-queue-icon");
    document.getElementById("spinner").classList.remove("spinner-border", "loading-spinner");
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
