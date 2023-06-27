function ElementHide(elementId, isHide) {
    if (isHide)
        document.getElementById(elementId).style.display = "none"; 
    else
        document.getElementById(elementId).style.display = ""; 
}

function ElementFocus(elementId) {
    document.getElementById(elementId).focus();
}

function ModalClose(dataBsTarget) {
    var myModalEl = document.getElementById(dataBsTarget);
    var modal = bootstrap.Modal.getInstance(myModalEl)
    modal.hide();
}

window.getDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};