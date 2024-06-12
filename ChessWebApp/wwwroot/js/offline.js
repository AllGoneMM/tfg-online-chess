var chessBoard;
var chessGame;
var playerTeam;
var signalRConnection;
const State = Object.freeze({
    NONE: 0,
    IN_PROGRESS: 1,
    WIN_WHITE: 2,
    WIN_BLACK: 3,
    DRAW_STALEMATE: 4,
    DRAW_THREEFOLD_REPETITION: 5,
    DRAW_FIFTY_MOVE_RULE: 6,
    DRAW_INSUFFICIENT_MATERIAL: 7,
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
document.addEventListener('DOMContentLoaded', async function () {
    // Start the connection with OfflineGameHub
    signalRConnection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/offlinegame", signalR.HttpTransportType.WebSockets)
        .withAutomaticReconnect()
        .build();
    signalRConnection.on("ReceiveStockfishMove",
        (response) => {
            chessGame = JSON.parse(response);
            if (chessGame.LegalMoves) {
                chessGame.LegalMoves = transformLegalMovesToDictionary(chessGame.LegalMoves);
            }
            chessBoard.position(chessGame.Fen);
            if (chessGame.State !== State.IN_PROGRESS) {
                openModal();
            };
        });

    // Event listener for the start game button
    document.getElementById("startGame").addEventListener("click", async function () {
        chessGame = null;
        try {
            if (signalRConnection.state !== signalR.HubConnectionState.Connected) {
                await signalRConnection.start();
            }

            let orientation = "black";
            playerTeam = (orientation === "black") ? 2 : 1;
            chessBoard = Chessboard('chessBoard', {
                pieceTheme: 'assets/img/chesspieces/{piece}.png',
                draggable: true,
                position: 'start',
                onDrop: onDrop,
                onSnapEnd: onSnapEnd,
                onDragStart: onDragStart,
                orientation: orientation

            });
            let serverJson = await signalRConnection.invoke("StartGame", orientation)
                .then((response) => {
                    response = JSON.parse(response);
                    chessGame = response;
                    chessBoard.position(response.Fen);
                    chessGame.LegalMoves = transformLegalMovesToDictionary(response.LegalMoves);
                })
                .catch((error) => {
                    chessBoard = Chessboard('chessBoard', {
                        pieceTheme: 'assets/img/chesspieces/{piece}.png',
                        position: 'start',
                        orientation: playerTeam
                    });
                    window.alert("Error al iniciar la partida " + error.toString());
                });
        } catch (error) {
            window.alert("Error al conectar cone el servidor");
        }
    });
});
function openModal() {
    document.getElementById('gameInfoTitle').innerText = "Game Over";
    document.getElementById('gameInfoBody').innerText = StateText[chessGame.State];
    const gameInfo = new bootstrap.Modal(document.getElementById('gameInfo'));
    gameInfo.show();
}
async function onDrop(source, target, piece, newPos, oldPos, orientation) {
    await removeGreySquares();

    // Validación en lado del cliente
    if (chessGame.LegalMoves) {
        let legalMoves = chessGame.LegalMoves[source];
        if (!legalMoves || legalMoves.indexOf(target) === -1) {
            return 'snapback';
        }
    } else {
        return 'snapback';
    }

    // Validación en lado del servidor
    let move = source + target;
    return await signalRConnection.invoke("ProcessMove", move)
        .then((response) => {
            chessGame = JSON.parse(response);
            if (!chessGame.MoveResult.MoveSquareResult.IsSuccessful) {
                return 'snapback';
            }
            if (chessGame.State !== State.IN_PROGRESS) {
                openModal();
            };
        })
        .catch((error) => {
            console.error("Error processing move:", error);
            return 'snapback';
        });
}
function onSnapEnd() {
    chessBoard.position(chessGame.Fen);
    signalRConnection.invoke("GetStockfishMove")
        .catch((error) => {
            console.error("Error processing move:", error);
            return 'snapback';
        });
}


async function removeGreySquares() {
    $('#chessBoard .square-55d63').css('background', '')
}

async function onDragStart(source, piece, position, orientation) {
    if (!chessGame || !chessGame.Turn || chessGame.Turn !== playerTeam) {
        return false;
    }

    if ((playerTeam === 1 && piece.search(/^w/) === -1) ||
        (playerTeam === 2 && piece.search(/^b/) === -1)) {
        return false;
    }
    if (chessGame.LegalMoves) {
        let sourceLegalMoves = chessGame.LegalMoves[source];
        if (sourceLegalMoves) {
            sourceLegalMoves.forEach((item) => {
                greySquare(item);
            });

        }
    }

}
function convertIndexToChessNotation(squareIndex) {
    // Calcula la columna de la casilla basándose en el índice de casilla.
    const column = String.fromCharCode('a'.charCodeAt(0) + (squareIndex % 8));
    // Calcula la fila de la casilla basándose en el índice de casilla.
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

function isEnemyPiece(square) {
    var piece = $('#chessBoard .square-' + square).find('.piece-417db');
    if (piece.length === 0) return false; // No hay pieza en la casilla

    var pieceData = piece.data('piece');
    if (playerTeam === 1) {
        return pieceData && pieceData.startsWith('b'); // El jugador es blanco, la pieza enemiga es negra
    } else {
        return pieceData && pieceData.startsWith('w'); // El jugador es negro, la pieza enemiga es blanca
    }
}

function greySquare(square) {
    var $square = $('#chessBoard .square-' + square);

    if (isEnemyPiece(square, playerTeam)) {
        $square.css('background', '#dc3545');
    } else {
        var background = '#a9a9a9';
        if ($square.hasClass('black-3c85d')) {
            background = '#696969';
        }
        $square.css('background', background);
    }
}

