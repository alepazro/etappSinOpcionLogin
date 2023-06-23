function setMapsHeights() {
    try {
        var h2 = ((h - 90) / 2) - 1;
        var h3 = h2 - 15;

        $(document.getElementById('box1')).attr('style', 'height:' + h2 + 'px;');
        $(document.getElementById('box2')).attr('style', 'height:' + h2 + 'px;');
        $(document.getElementById('box3')).attr('style', 'height:' + h2 + 'px;');
        $(document.getElementById('box4')).attr('style', 'height:' + h2 + 'px;');

        $(document.getElementById('map1')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map1Side')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map1Main')).attr('style', 'height:' + h3 + 'px;');

        $(document.getElementById('map2')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map2Side')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map2Main')).attr('style', 'height:' + h3 + 'px;');

        $(document.getElementById('map3')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map3Side')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map3Main')).attr('style', 'height:' + h3 + 'px;');

        $(document.getElementById('map4')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map4Side')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map4Main')).attr('style', 'height:' + h3 + 'px;');
    }
    catch (err) {
        alert('setMapsHeights: ' + err.description);
    }
}
