var chessBoard;
var signalRConnection;

document.addEventListener('DOMContentLoaded', async function () {
    document.getElementById("newGame").addEventListener("click", async function () {
        try {
            await signalRConnection.invoke("JoinQueue"); // Join the queue
        } catch (err) {
            alert("Error al unirse a la cola: " + err.toString());
        }
    });

    signalRConnection = new signalR.HubConnectionBuilder()
        .withUrl("hubs/onlinegame")
        .withAutomaticReconnect()
        .build();

    signalRConnection.on("ReceiveMove", function (fen) {
        chessBoard.position(fen);
    });

    signalRConnection.on("InvalidMove", function (errorMessage) {
        chessBoard.position(errorMessage);
    });
    signalRConnection.on("NotYourTurn", function (errorMessage) {
        chessBoard.position(errorMessage);
    });

    signalRConnection.start()
        .then(async function () { await signalRConnection.invoke("JoinQueue") })
        .catch(error => alert("Error al abrir la conexión: " + error.toString()));
});

async function onDrop(source, target, piece, newPos, oldPos, orientation) {
    let move = source + target;

    try {
        await signalRConnection.invoke("SendMove", move); // Send the move to the server
    } catch (err) {
        chessBoard.position(oldPos);
    }
}
