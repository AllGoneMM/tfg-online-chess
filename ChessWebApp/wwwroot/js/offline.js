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
    document.getElementById('<div id="mainContainer" class="row ps-lg-4">
    <div class="col-12 col-lg-9 col-xl-8 pt-3 pb-3 shadow-lg rounded" id="chessBoardPlayersContainer">
        <div class="mx-auto d-flex pb-3 gap-2 align-items-center" id="div1">
            <div class="rounded rounded bg-danger border border-1 border-dark" style="width:25px; height:25px;"></div>
            <span class="fw-semibold fs-6">Opponent</span>
        </div>

        <div class="mx-auto shadow-lg" id="chessBoard"></div>

        <div class="mx-auto d-flex pt-3 gap-2 align-items-center" id="div2">
            <div class="rounded rounded bg-success border border-1 border-dark" style="width:25px; height:25px;"></div>
            <span class="fw-semibold fs-6" asp-authenticated="true">@User.Identity.Name</span>
            <span class="fw-semibold fs-6" asp-authenticated="false">Guest</span>
        </div>
    </div>
    <div class="col-12 col-lg-3 col-xl-4 p-4" style="background-color:#efefef">
        <div class="d-flex h-100 flex-column p-4 shadow-lg rounded" style="background-color:#e6ebea;">
            <div class="d-block h-100">

            </div>
            <button id="startGame" class="btn btn-success fs-4 fw-bolder align-self-end w-100">Start game</button>
        </div>
    </div>
</div > <div id="mainContainer" class="row ps-lg-4">
            <div class="col-12 col-lg-9 col-xl-8 pt-3 pb-3 shadow-lg rounded" id="chessBoardPlayersContainer">
                <div class="mx-auto d-flex pb-3 gap-2 align-items-center" id="div1">
                    <div class="rounded rounded bg-danger border border-1 border-dark" style="width:25px; height:25px;"></div>
                    <span class="fw-semibold fs-6">Opponent</span>
                </div>

                <div class="mx-auto shadow-lg" id="chessBoard"></div>

                <div class="mx-auto d-flex pt-3 gap-2 align-items-center" id="div2">
                    <div class="rounded rounded bg-success border border-1 border-dark" style="width:25px; height:25px;"></div>
                    <span class="fw-semibold fs-6" asp-authenticated="true">@User.Identity.Name</span>
                    <span class="fw-semibold fs-6" asp-authenticated="false">Guest</span>
                </div>
            </div>
            <div class="col-12 col-lg-3 col-xl-4 p-4" style="background-color:#efefef">
                <div class="d-flex h-100 flex-column p-4 shadow-lg rounded" style="background-color:#e6ebea;">
                    <div class="d-block h-100">

                    </div>
                    <button id="startGame" class="btn btn-success fs-4 fw-bolder align-self-end w-100">Start game</button>
                </div>
            </div>
        </div>gameInfoBody').innerText = StateText[chessGame.State];
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


function removeGreySquares() {
    $('#chessBoard .greyed-square, #chessBoard .enemy-piece').each(function () {
        var $square = $(this);
        $square.removeClass('greyed-square enemy-piece').css('background', '');
    });
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
        $square.addClass('enemy-piece'); // Add class for enemy pieces
    } else {
        $square.addClass('greyed-square'); // Add class for greyed squares
    }
}


