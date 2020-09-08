$(document).ready(() => {
    var heightTotal = $(document).height();
    var heightNavbar = $("#navBarID").height();
    $(".mainBody").height(heightTotal - heightNavbar);


});

