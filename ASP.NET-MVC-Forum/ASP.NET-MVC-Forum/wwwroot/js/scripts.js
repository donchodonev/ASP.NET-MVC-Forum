/*!
    * Start Bootstrap - SB Admin v7.0.4 (https://startbootstrap.com/template/sb-admin)
    * Copyright 2013-2021 Start Bootstrap
    * Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-sb-admin/blob/master/LICENSE)
    */
// 
// Scripts
// 


// Toggle the side navigation
const sidebarToggle = document.body.querySelector('#sidebarToggle');
const sidenavContent = document.body.querySelector("#layoutSidenav");

if (sidebarToggle || sidenavContent) {
    sidebarToggle.addEventListener('click', event => {
        event.preventDefault();
        document.body.classList.toggle('sb-sidenav-toggled');

        let isToggled = document.body.classList.value == "sb-sidenav-toggled";

        if (isToggled) {
            sidenavContent.addEventListener('click', event => {
                event.preventDefault();
                if (isToggled) {
                    document.body.classList.toggle('sb-sidenav-toggled');
                    isToggled = false;
                }
            });
        }
    });
}
