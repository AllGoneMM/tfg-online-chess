var chessBoard;
var chessGame;
var signalRConnection;
var waiting;

document.addEventListener('DOMContentLoaded', async function () {
    // Inicia la conexión con el hub del juego online
    signalRConnection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/onlinegame", signalR.HttpTransportType.WebSockets)
        .withAutomaticReconnect()
        .build();

    signalRConnection.start()
        .catch(error => alert("Error al conectar con el servidor"));

    // Event listener para el botón de iniciar juego
    document.getElementById("startGame").addEventListener("click", async function () {
        try {
            await signalRConnection.invoke("JoinQueue"); // Unirse a la cola
            chessBoard = Chessboard('chessBoard', {
                pieceTheme: 'assets/img/chesspieces/{piece}.png',
                draggable: true,
                position: 'start',
                onDrop: onDrop,
                onSnapEnd: onSnapEnd,
                onDragStart: onDragStart
            });
        } catch (err) {
            alert("Error al unirse a la cola: " + err.toString());
        }
    });

    // Event listener para recibir movimientos del oponente
    signalRConnection.on("ReceiveMove", function (fen) {
        chessBoard.position(fen);
        chessGame = fen;
    });

    // Event listener para manejar errores de movimientos inválidos
    signalRConnection.on("InvalidMove", function (errorMessage) {
    });

    signalRConnection.on("NotYourTurn", function (errorMessage) {
    });
});

// Función para manejar el evento onDrop
async function onDrop(source, target, piece, newPos, oldPos, orientation) {
    await removeGreySquares();
    let move = source + target;
    return await signalRConnection.invoke("SendMove", move)
        .then((response) => {
            response = JSON.parse(response);
            if (!response.Success) {
                return 'snapback';
            }
            chessGame = response.Fen;
        })
        .catch((error) => {
            console.error("Error processing move:", error);
            return 'snapback';
        });
}

// Función para manejar el evento onSnapEnd
async function onSnapEnd() {
    chessBoard.position(chessGame);
    waiting = true;
    await signalRConnection.invoke("GetStockfishMove")
        .then((response) => {
            chessBoard.position(response);
            chessGame = response;
            waiting = false;
        })
        .catch((error) => {
            console.error("Error processing move:", error);
            return 'snapback';
        });
}

// Función para remover los cuadros grises
async function removeGreySquares() {
    document.querySelectorAll('#chessBoard .square-55d63').forEach(square => {
        square.style.background = '';
    });
}

// Función para manejar el evento onDragStart
async function onDragStart(source, piece, position, orientation) {
    if (waiting) {
        return false;
    }
    await signalRConnection.invoke("GetLegalMoves", source)
        .then(async (response) => {
            console.log("Movimientos legales: " + response);
            response = JSON.parse(response);
            for (var i = 0; i < response.length; i++) {
                await greySquare(response[i]);
            }
        })
        .catch(() => {
            alert("Error al obtener movimientos legales");
            return 'snapback';
        });
}

// Función para colorear los cuadros grises
function greySquare(square) {
    var $square = document.querySelector('#chessBoard .square-' + square);

    var background = '#a9a9a9';
    if ($square.classList.contains('black-3c85d')) {
        background = '#696969';
    }
    $square.style.background = background;
}
