function DynamicRow() {
    var division = document.createElement('DIV');
    division.innerHTML = DynamicTextBox("");
    document.getElementById("firstdiv").appendChild(division);
}
function DynamicTextBox(value) {
    return '<div><input type="text" name="dyntxt"/><input type="button" onclick="ReTextBox(this)" value="Remove"/></div>';
}
function ReTextBox(div) {
    document.getElementById("firstdiv").removeChild(div.parentNode.parentNode);
}