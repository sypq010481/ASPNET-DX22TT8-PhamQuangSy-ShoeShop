/**
 * Main
 */

'use strict';

let menu, animate;

(function () {
  // Initialize menu
  //-----------------

  let layoutMenuEl = document.querySelectorAll('#layout-menu');
  layoutMenuEl.forEach(function (element) {
    menu = new Menu(element, {
      orientation: 'vertical',
      closeChildren: false
    });
    // Change parameter to true if you want scroll animation
    window.Helpers.scrollToActive((animate = false));
    window.Helpers.mainMenu = menu;
  });

  // Initialize menu togglers and bind click on each
  let menuToggler = document.querySelectorAll('.layout-menu-toggle');
  menuToggler.forEach(item => {
    item.addEventListener('click', event => {
      event.preventDefault();
      window.Helpers.toggleCollapsed();
    });
  });

  // Display menu toggle (layout-menu-toggle) on hover with delay
  let delay = function (elem, callback) {
    let timeout = null;
    elem.onmouseenter = function () {
      // Set timeout to be a timer which will invoke callback after 300ms (not for small screen)
      if (!Helpers.isSmallScreen()) {
        timeout = setTimeout(callback, 300);
      } else {
        timeout = setTimeout(callback, 0);
      }
    };

    elem.onmouseleave = function () {
      // Clear any timers set to timeout
      document.querySelector('.layout-menu-toggle').classList.remove('d-block');
      clearTimeout(timeout);
    };
  };
  if (document.getElementById('layout-menu')) {
    delay(document.getElementById('layout-menu'), function () {
      // not for small screen
      if (!Helpers.isSmallScreen()) {
        document.querySelector('.layout-menu-toggle').classList.add('d-block');
      }
    });
  }

  // Display in main menu when menu scrolls
  let menuInnerContainer = document.getElementsByClassName('menu-inner'),
    menuInnerShadow = document.getElementsByClassName('menu-inner-shadow')[0];
  if (menuInnerContainer.length > 0 && menuInnerShadow) {
    menuInnerContainer[0].addEventListener('ps-scroll-y', function () {
      if (this.querySelector('.ps__thumb-y').offsetTop) {
        menuInnerShadow.style.display = 'block';
      } else {
        menuInnerShadow.style.display = 'none';
      }
    });
  }

  // Init helpers & misc
  // --------------------

  // Init BS Tooltip
  const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
  tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });

  // Accordion active class
  const accordionActiveFunction = function (e) {
    if (e.type == 'show.bs.collapse' || e.type == 'show.bs.collapse') {
      e.target.closest('.accordion-item').classList.add('active');
    } else {
      e.target.closest('.accordion-item').classList.remove('active');
    }
  };

  const accordionTriggerList = [].slice.call(document.querySelectorAll('.accordion'));
  const accordionList = accordionTriggerList.map(function (accordionTriggerEl) {
    accordionTriggerEl.addEventListener('show.bs.collapse', accordionActiveFunction);
    accordionTriggerEl.addEventListener('hide.bs.collapse', accordionActiveFunction);
  });

  // Auto update layout based on screen size
  window.Helpers.setAutoUpdate(true);

  // Toggle Password Visibility
  window.Helpers.initPasswordToggle();

  // Speech To Text
  window.Helpers.initSpeechToText();

  // Manage menu expanded/collapsed with templateCustomizer & local storage
  //------------------------------------------------------------------

  // If current layout is horizontal OR current window screen is small (overlay menu) than return from here
  if (window.Helpers.isSmallScreen()) {
    return;
  }

  // If current layout is vertical and current window screen is > small

  // Auto update menu collapsed/expanded based on the themeConfig
  window.Helpers.setCollapsed(true, false);
})();


  window.openPopupEdit = function (id, size, quantity) {
    $.ajax({
        url: '/Admin/Product/EditModal',
        data: { Id: id, size: size, Quantity: quantity },
        dataType: 'json',
        type: "GET",
        success: function (data) {

            $("#ImageId").val(data.id);
            $("#sizeBackdrop").val(data.size);
            $("#quantityBackdrop").val(data.quantity);

            var modal = new bootstrap.Modal(document.getElementById("updateSize"));
            modal.show();
        }
    });
  }

window.openPopupEditOrder = function (id, productid, quantity, total) {
    $.ajax({
        url: '/Admin/Order/EditModal',
        data: { Id: id, ProductId: productid, Quantity: quantity, Total: total },
        dataType: 'json',
        type: "GET",
        success: function (data) {            
            $("#orderid").val(data.id);
            $("#quantityOrder").val(data.quantity);
            var $select = $("#productIdOrder");
            $select.empty(); // clear option cũ

            data.products.forEach(item => {
                // xác định selected ngay khi append
                var selected = item.value === data.productid ? "selected" : "";
                $select.append(`<option value="${item.value}" ${selected}>${item.text}</option>`);
            });

            // set value + trigger change cho select2
            $select.val(data.productid).trigger('change');
            $("#priceOrder").val(data.total);

            var modal = new bootstrap.Modal(document.getElementById("updateOrder"));
            modal.show();
        }
    });
}

window.onDeleteProductImage = function (id) {    
    $.ajax({
        url: '/Admin/Product/DeleteModal', //additional parameters
        data: { Id: id },
        dataType: 'json',
        type: "POST",
        success: function (data) {
            window.location.reload()
        }
    });
}

window.onDeleteOrderDetail = function (id) {
    $.ajax({
        url: '/Admin/Order/DeleteModal', //additional parameters
        data: { Id: id },
        dataType: 'json',
        type: "POST",
        success: function (data) {
            window.location.reload()
        }
    });
}

window.openCreateOrderDetail = function (id) {
    $.ajax({
        url: '/Admin/Order/CreateModal', //additional parameters
        data: { Id: id },
        dataType: 'json',
        type: "POST",
        success: function (data) {
            $("#orderIdDetail").val(data.Id);
            var modal = new bootstrap.Modal(document.getElementById("createOrder"));
            modal.show();
        }
    });
}