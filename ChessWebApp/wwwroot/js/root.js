window.onload = function () {
    adjustContentHeight();
};

window.onresize = function () {
    adjustContentHeight();
};

function adjustContentHeight() {
    var navbarHeight = document.getElementById('navbar').offsetHeight;
    let rowContent = document.getElementById('rowContent');
    rowContent.style.cssText = "min-height: calc(100vh - " + navbarHeight + "px) !important;";
}

