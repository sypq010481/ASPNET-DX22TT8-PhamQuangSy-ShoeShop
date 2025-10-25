//document.addEventListener("DOMContentLoaded", function () {
//    const sizeLinks = document.querySelectorAll("#size-options a");
//    const hiddenInput = document.getElementById("selected-size");

//    sizeLinks.forEach(link => {
//        link.addEventListener("click", function (e) {
//            e.preventDefault();

//            // Bỏ class active cũ
//            sizeLinks.forEach(l => l.classList.remove("active"));

//            // Thêm class active mới
//            this.classList.add("active");

//            // Lưu size được chọn
//            hiddenInput.value = this.getAttribute("data-size");
//        });
//    });
//});