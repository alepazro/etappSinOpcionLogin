app.trim = function (str, chars) {
    return ltrim(rtrim(str, chars), chars);
}

//ltrim quita los espacios o caracteres indicados al inicio de la cadena
app.ltrim = function (str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
}
//rtrim quita los espacios o caracteres indicados al final de la cadena
app.rtrim = function (str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
}

Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);
    return this;
}
