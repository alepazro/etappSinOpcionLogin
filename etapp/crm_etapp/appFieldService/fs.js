
function newJob() {
    try {
        window.open('job.html?' + 'id=0', target = "_blank");
    }
    catch (err) {
    }
}

function editJob(jobId) {
    try {
        window.open('job.html?' + 'id=' + jobId, target = "_blank");
    }
    catch (err) {

    }
}
