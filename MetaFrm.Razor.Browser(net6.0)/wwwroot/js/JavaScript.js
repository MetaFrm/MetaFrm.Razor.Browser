function ElementHide(elementId, isHide) {
    if (isHide)
        document.getElementById(elementId).style.display = "none"; 
    else
        document.getElementById(elementId).style.display = ""; 
}

function ElementFocus(elementId) {
    document.getElementById(elementId).focus();
}

window.getDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};