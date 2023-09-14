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

var isExpanded = false;
function LayoutMenuInit() {
    document.documentElement.setAttribute('class', 'light-style layout-menu-fixed layout-navbar-fixed');
    isExpanded = false;
}
function LayoutMenuExpande() {
    if (!isExpanded) {
        //document.documentElement.setAttribute('class', 'light-style layout-menu-fixed layout-transitioning layout-menu');
        document.documentElement.setAttribute('class', 'light-style layout-menu-fixed layout-navbar-fixed layout-menu-expanded');
        isExpanded = true;
    }
    else {
        document.documentElement.setAttribute('class', 'light-style layout-menu-fixed layout-navbar-fixed');
        isExpanded = false;
    }
}