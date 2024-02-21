function HighLight(nWord) {
    var n = -1;
    document.getElementById('hitsnum1207').name = "";
    if (nWord != '') {
        //nWord = document.selection.createRange().text;
        var keyword = document.body.createTextRange();
        while (keyword.findText(nWord)) {
            n++;
            //            alert(keyword.htmlText);
            keyword.pasteHTML("<span style='color:#000000; background:#FFFF66; font-weight:bold' id='hit" + n + "'>" + keyword.htmlText + "</span>");
            keyword.moveStart('character', 1);
        }
    }
    if (n >= 0)
        document.getElementById('hitsnum1207').name = n;
}