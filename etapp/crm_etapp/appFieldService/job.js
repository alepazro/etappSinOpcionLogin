function loadJob() {
    try{
        var token = getTokenCookie('ETTK');
        var id = getParameterByName('id');

        if (id != '') {
            var data = 'token=' + token + '&id=' + escape(id);
            var job = getDb('jobs.svc', 'getJob', data, false);

            document.title = 'Job ' + job.jobNumber;
            $('#jobNumber').text(job.jobNumber);

            $('#custName').val(job.custName);
            $('#custContact').val(job.custContact);
            $('#custPhone').val(job.custPhone);
            $('#custEmail').val(job.custEmail);
            $('#custAddress').val(job.custAddress);

            $('#dueDate').val(job.dueDate);
            $('#estDuration').val(job.estDuration);
            $('#durationHHMM').val(job.durationHHMM);
            $('#jobDescription').val(job.jobDescription);

            $('#categoryName').val(job.categoryName);
            $('#statusName').val(job.statusName);
            $('#priorityName').val(job.priorityName);
            $('#userName').val(job.userName);

            if (!_.isNull(job.notes)) {
                if (job.notes.length > 0) {
                    for (i = 0; i < job.notes.length; i++) {
                        $("#__jobNoteTmpl").tmpl(job.notes[i]).appendTo("#notesCollection");
                    }
                }
                else {
                    $('#noNotesTitle').show();
                    $('#notesDiv').hide();
                }
            }
            else {
                $('#noNotesTitle').show();
                $('#notesDiv').hide();
            }

            if (!_.isNull(job.picturesList)) {
                if (job.picturesList.length > 0) {
                    for (i = 0; i < job.picturesList.length; i++) {
                        $("#__jobPicTmpl").tmpl(job.picturesList[i]).appendTo("#picsCollection");
                    }
                }
                else {
                    $('#noPicsTitle').show();
                }
            }
            else {
                $('#noPicsTitle').show();
            }


            if (!_.isNull(job.signature)) {
                $('#signedBy').text(job.signature.imgName);
                $('#signedOn').text(job.signature.eventDate);
                $('#btnShowSignature').attr('data-imageId', job.signature.imageId)
            }
            else {
                $('#noSignatureTitle').show();
                $('#signatureDiv').hide();
            }
        }
    }
    catch (err) {

    }
}

function showImage(obj) {
    try{
        var token = getTokenCookie('ETTK');
        var imgId = $(obj).attr('data-imageId');

        var data = 'token=' + escape(token) + '&id=' + imgId;
        var img = getDb('pilot.svc', 'getImage', data, false);
        $('#jobImg').attr("src", img.imgData);
    }
    catch (err) {

    }
}

function showSignature(obj) {
    try{
        var token = getTokenCookie('ETTK');
        var imgId = $(obj).attr('data-imageId');

        var data = 'token=' + escape(token) + '&id=' + imgId;
        var img = getDb('pilot.svc', 'getImage', data, false);
        $('#imgJobSignature').attr("src", img.imgData);
    }
    catch (err) {

    }
}