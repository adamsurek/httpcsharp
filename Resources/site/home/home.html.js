function toggleColorMode() {
    var body = document.getElementById("body");
    
    
    if (body.style.backgroundColor === "white") {
        body.style.backgroundColor = "#262626";
        body.style.color = "white"
    } else {
        body.style.backgroundColor = "white";
        body.style.color = "black"
    }
}