etApp.factory('popupModalService', [function () {
    return {
        open: function (elementId) {
            $(elementId).modal({
                show: true,
                backdrop: false
            });
        },
        close: function (elementId) {
            $(elementId).modal('hide');
        }
    };
}]);